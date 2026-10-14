using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using FuturePath.ApplicationLayer;

namespace FuturePath.DatabaseLayer
{
    /// <summary>
    /// Data access for degree programmes and their requirements (class diagram
    /// DegreeProgrammeRepository: findByIdWithRequirements, findActiveProgrammes), plus the
    /// Administrator maintenance of programmes (UI 13.1) and requirements (UI 13.2) in the
    /// Waitering DataSetChange / UpdateDataSource pattern.
    /// </summary>
    public class DegreeProgrammeRepository : DB
    {
        #region Data Members
        private string programmeTable = "DegreeProgramme";
        private string requirementTable = "ProgrammeRequirement";
        private string mappingTable = "DegreeInterestMapping";

        // Programmes with their faculty name (the join is only for reading; the adapter commands write DegreeProgramme)
        private string sqlProgrammes = "SELECT p.ProgrammeID, p.FacultyID, f.FacultyName, p.ProgrammeName, p.MinimumOverallAverage, p.IsActive "
                                     + "FROM DegreeProgramme p INNER JOIN Faculty f ON f.FacultyID = p.FacultyID";

        // Requirements with the subject name and the accepted alternative's name (LEFT JOIN: there may be no alternative)
        private string sqlRequirements = "SELECT r.RequirementID, r.ProgrammeID, r.SubjectID, s.SubjectName, r.RequirementType, r.MinimumMark, "
                                       + "r.AlternativeSubjectID, a.SubjectName AS AlternativeSubjectName, r.AlternativeMinimumMark, r.AdvisoryNote, r.IsActive "
                                       + "FROM ProgrammeRequirement r INNER JOIN Subject s ON s.SubjectID = r.SubjectID "
                                       + "LEFT JOIN Subject a ON a.SubjectID = r.AlternativeSubjectID";

        // A programme's mappings to ACTIVE interest categories, with the category names
        private string sqlMappings = "SELECT m.ProgrammeID, m.CategoryID, c.CategoryName, m.RelevanceWeight "
                                   + "FROM DegreeInterestMapping m INNER JOIN InterestCategory c ON c.CategoryID = m.CategoryID "
                                   + "WHERE m.ProgrammeID = @ProgrammeID AND c.IsActive = 1 ORDER BY c.CategoryName";

        private Collection<DegreeProgramme> programmes;
        private Collection<ProgrammeRequirement> requirements;
        #endregion

        #region Constructor
        /// <summary>Creates the repository (no database work until a method is called).</summary>
        public DegreeProgrammeRepository() : base()
        {
            programmes = new Collection<DegreeProgramme>();
            requirements = new Collection<ProgrammeRequirement>();
        }
        #endregion

        #region Read - eligibility engine
        /// <summary>
        /// Class diagram findByIdWithRequirements(programmeId). Loads the programme (with
        /// FacultyName), its ACTIVE requirements (with subject and alternative names, in
        /// RequirementID order) and its interest mappings to ACTIVE categories (with category
        /// names and weights). Returns null if it does not exist (active or not).
        /// </summary>
        public DegreeProgramme FindByIdWithRequirements(int programmeID)
        {
            DegreeProgramme programme = FindById(programmeID);
            if (programme == null)
            {
                return null;
            }
            LoadRequirementsAndMappings(programme);
            return programme;
        }

        /// <summary>
        /// Class diagram findActiveProgrammes(excludedProgrammeId). Every active programme under
        /// an active faculty except the excluded one, each loaded as FindByIdWithRequirements does.
        /// </summary>
        public Collection<DegreeProgramme> FindActiveProgrammes(int excludedProgrammeID)
        {
            FillDataSet(sqlProgrammes + " WHERE p.IsActive = 1 AND f.IsActive = 1 AND p.ProgrammeID <> @ExcludedID ORDER BY p.ProgrammeName",
                        programmeTable, Param("@ExcludedID", SqlDbType.Int, excludedProgrammeID));
            Add2Collection();
            Collection<DegreeProgramme> candidates = programmes;

            // Load each candidate's requirements and interest mappings (a handful of programmes, so one query each is fine)
            foreach (DegreeProgramme programme in candidates)
            {
                LoadRequirementsAndMappings(programme);
            }
            return candidates;
        }
        #endregion

        #region Read - lists
        /// <summary>One programme with FacultyName, without requirements, or null.</summary>
        public DegreeProgramme FindById(int programmeID)
        {
            FillDataSet(sqlProgrammes + " WHERE p.ProgrammeID = @ProgrammeID", programmeTable,
                        Param("@ProgrammeID", SqlDbType.Int, programmeID));
            Add2Collection();
            return programmes.Count > 0 ? programmes[0] : null;
        }

        /// <summary>
        /// Programmes with FacultyName, ordered by name. facultyID null means every faculty.
        /// Without includeInactive, inactive programmes and programmes under an inactive faculty are left out.
        /// </summary>
        public Collection<DegreeProgramme> GetProgrammes(int? facultyID, bool includeInactive)
        {
            FillDataSet(sqlProgrammes + " WHERE (@FacultyID IS NULL OR p.FacultyID = @FacultyID)"
                        + " AND (@IncludeInactive = 1 OR (p.IsActive = 1 AND f.IsActive = 1)) ORDER BY p.ProgrammeName",
                        programmeTable,
                        Param("@FacultyID", SqlDbType.Int, facultyID),
                        Param("@IncludeInactive", SqlDbType.Bit, includeInactive));
            Add2Collection();
            return programmes;
        }

        /// <summary>
        /// Active programmes under active faculties whose name contains the text (UI 5 search mode).
        /// Blank text returns an empty collection.
        /// </summary>
        public Collection<DegreeProgramme> SearchActiveProgrammes(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new Collection<DegreeProgramme>();
            }
            FillDataSet(sqlProgrammes + " WHERE p.IsActive = 1 AND f.IsActive = 1 AND p.ProgrammeName LIKE '%' + @Text + '%' ORDER BY p.ProgrammeName",
                        programmeTable, Param("@Text", SqlDbType.VarChar, EscapeLike(text.Trim())));
            Add2Collection();
            return programmes;
        }

        /// <summary>True if another programme already has this name (not case-sensitive).</summary>
        public bool ProgrammeNameExists(string programmeName, int? excludeProgrammeID)
        {
            object count = ExecuteScalar("SELECT COUNT(*) FROM DegreeProgramme WHERE ProgrammeName = @Name AND (@ExcludeID IS NULL OR ProgrammeID <> @ExcludeID)",
                                         Param("@Name", SqlDbType.VarChar, (programmeName ?? "").Trim()),
                                         Param("@ExcludeID", SqlDbType.Int, excludeProgrammeID));
            return Convert.ToInt32(count) > 0;
        }

        /// <summary>A programme's requirements with SubjectName and AlternativeSubjectName, in RequirementID order.</summary>
        public Collection<ProgrammeRequirement> GetRequirements(int programmeID, bool includeInactive)
        {
            FillDataSet(sqlRequirements + " WHERE r.ProgrammeID = @ProgrammeID AND (@IncludeInactive = 1 OR r.IsActive = 1) ORDER BY r.RequirementID",
                        requirementTable,
                        Param("@ProgrammeID", SqlDbType.Int, programmeID),
                        Param("@IncludeInactive", SqlDbType.Bit, includeInactive));
            AddRequirements2Collection();
            return requirements;
        }

        /// <summary>One requirement with its subject names, or null.</summary>
        public ProgrammeRequirement FindRequirementById(int requirementID)
        {
            FillDataSet(sqlRequirements + " WHERE r.RequirementID = @RequirementID", requirementTable,
                        Param("@RequirementID", SqlDbType.Int, requirementID));
            AddRequirements2Collection();
            return requirements.Count > 0 ? requirements[0] : null;
        }

        /// <summary>True if the programme already has a requirement for this subject (UNIQUE(ProgrammeID, SubjectID)).</summary>
        public bool RequirementExists(int programmeID, int subjectID, int? excludeRequirementID)
        {
            object count = ExecuteScalar("SELECT COUNT(*) FROM ProgrammeRequirement WHERE ProgrammeID = @ProgrammeID AND SubjectID = @SubjectID"
                                         + " AND (@ExcludeID IS NULL OR RequirementID <> @ExcludeID)",
                                         Param("@ProgrammeID", SqlDbType.Int, programmeID),
                                         Param("@SubjectID", SqlDbType.Int, subjectID),
                                         Param("@ExcludeID", SqlDbType.Int, excludeRequirementID));
            return Convert.ToInt32(count) > 0;
        }
        #endregion

        #region Utility Methods
        /// <summary>Turns the filled DegreeProgramme rows into DegreeProgramme objects (Waitering Add2Collection).</summary>
        private void Add2Collection()
        {
            programmes = new Collection<DegreeProgramme>();
            foreach (DataRow row in GetTable(programmeTable).Rows)
            {
                DegreeProgramme programme = new DegreeProgramme(ReadInt(row, "ProgrammeID").Value, ReadInt(row, "FacultyID").Value,
                                                                ReadString(row, "ProgrammeName"), ReadDecimal(row, "MinimumOverallAverage"),
                                                                ReadBool(row, "IsActive"));
                programme.FacultyName = ReadString(row, "FacultyName");
                programmes.Add(programme);
            }
        }

        /// <summary>Turns the filled ProgrammeRequirement rows into ProgrammeRequirement objects.</summary>
        private void AddRequirements2Collection()
        {
            requirements = new Collection<ProgrammeRequirement>();
            foreach (DataRow row in GetTable(requirementTable).Rows)
            {
                requirements.Add(new ProgrammeRequirement(ReadInt(row, "RequirementID").Value, ReadInt(row, "ProgrammeID").Value,
                                                          ReadInt(row, "SubjectID").Value, ReadString(row, "SubjectName"),
                                                          ReadString(row, "RequirementType"), ReadDecimal(row, "MinimumMark"),
                                                          ReadInt(row, "AlternativeSubjectID"), ReadString(row, "AlternativeSubjectName"),
                                                          ReadDecimal(row, "AlternativeMinimumMark"), ReadString(row, "AdvisoryNote"),
                                                          ReadBool(row, "IsActive")));
            }
        }

        /// <summary>
        /// Adds a programme's ACTIVE requirements (RequirementID order) and its mappings to ACTIVE
        /// interest categories to the programme object. Used by FindByIdWithRequirements and FindActiveProgrammes.
        /// </summary>
        private void LoadRequirementsAndMappings(DegreeProgramme programme)
        {
            // 1. Active requirements, in RequirementID order
            foreach (ProgrammeRequirement requirement in GetRequirements(programme.ProgrammeID, false))
            {
                programme.GetRequirements().Add(requirement);
            }

            // 2. Interest mappings to active categories, each with its relevance weight
            FillDataSet(sqlMappings, mappingTable, Param("@ProgrammeID", SqlDbType.Int, programme.ProgrammeID));
            foreach (DataRow row in GetTable(mappingTable).Rows)
            {
                programme.InterestMappings.Add(new DegreeInterestMapping(ReadInt(row, "ProgrammeID").Value, ReadInt(row, "CategoryID").Value,
                                                                         ReadString(row, "CategoryName"), ReadDecimal(row, "RelevanceWeight").Value));
            }
        }

        /// <summary>Copies a programme's values into a DataRow (Waitering FillRow).</summary>
        private void FillRow(DataRow row, DegreeProgramme programme)
        {
            row["FacultyID"] = programme.FacultyID;
            row["ProgrammeName"] = (programme.ProgrammeName ?? "").Trim();
            row["MinimumOverallAverage"] = DbValue(programme.MinimumOverallAverage);
            row["IsActive"] = programme.IsActive;
        }

        /// <summary>Copies a requirement's values into a DataRow (Waitering FillRow). An alternative of 0 is stored as no alternative.</summary>
        private void FillRow(DataRow row, ProgrammeRequirement requirement)
        {
            row["ProgrammeID"] = requirement.ProgrammeID;
            row["SubjectID"] = requirement.SubjectID;
            row["RequirementType"] = requirement.RequirementType;
            row["MinimumMark"] = DbValue(requirement.MinimumMark);
            if (requirement.AlternativeSubjectID.HasValue && requirement.AlternativeSubjectID.Value > 0)
            {
                row["AlternativeSubjectID"] = requirement.AlternativeSubjectID.Value;
            }
            else
            {
                row["AlternativeSubjectID"] = DBNull.Value;
            }
            row["AlternativeMinimumMark"] = DbValue(requirement.AlternativeMinimumMark);
            row["AdvisoryNote"] = DbText(requirement.AdvisoryNote);
            row["IsActive"] = requirement.IsActive;
        }

        /// <summary>Makes LIKE wildcards in typed text (%, _ and [) match themselves.</summary>
        private static string EscapeLike(string text)
        {
            return text.Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]");
        }
        #endregion

        #region Maintenance - Waitering pattern
        /// <summary>
        /// Adds a new programme row to the DataSet, or changes the existing row (Edit).
        /// Throws ArgumentException if the programme to edit does not exist.
        /// </summary>
        public void DataSetChange(DegreeProgramme programme, DB.DBOperation operation)
        {
            DataRow row;
            if (operation == DB.DBOperation.Add)
            {
                // An empty copy of the table gives the columns for the new row
                FillDataSet(sqlProgrammes + " WHERE 1 = 0", programmeTable);
                row = GetTable(programmeTable).NewRow();
                FillRow(row, programme);
                GetTable(programmeTable).Rows.Add(row);
            }
            else
            {
                FillDataSet(sqlProgrammes + " WHERE p.ProgrammeID = @ProgrammeID", programmeTable,
                            Param("@ProgrammeID", SqlDbType.Int, programme.ProgrammeID));
                row = FindRow(programmeTable, "ProgrammeID", programme.ProgrammeID);
                if (row == null)
                {
                    throw new ArgumentException("The degree programme could not be found. Refresh the list and try again.");
                }
                FillRow(row, programme);
            }
        }

        /// <summary>
        /// Sends the programme change (made by DataSetChange) to the database. Returns the
        /// ProgrammeID (the new key for Add, which is also set on the programme).
        /// </summary>
        public int UpdateDataSource(DegreeProgramme programme, DB.DBOperation operation)
        {
            if (operation == DB.DBOperation.Add)
            {
                Create_INSERT_Command(programme);
            }
            else
            {
                Create_UPDATE_Command(programme);
            }
            UpdateDataSource(programmeTable);

            if (operation == DB.DBOperation.Add)
            {
                // The INSERT returned the new key into the row; read it before any other fill
                DataTable table = GetTable(programmeTable);
                programme.ProgrammeID = Convert.ToInt32(table.Rows[table.Rows.Count - 1]["ProgrammeID"]);
            }
            return programme.ProgrammeID;
        }

        /// <summary>
        /// Adds a new requirement row to the DataSet, or changes the existing row (Edit).
        /// Throws ArgumentException if the requirement to edit does not exist.
        /// </summary>
        public void DataSetChange(ProgrammeRequirement requirement, DB.DBOperation operation)
        {
            DataRow row;
            if (operation == DB.DBOperation.Add)
            {
                FillDataSet(sqlRequirements + " WHERE 1 = 0", requirementTable);
                row = GetTable(requirementTable).NewRow();
                FillRow(row, requirement);
                GetTable(requirementTable).Rows.Add(row);
            }
            else
            {
                FillDataSet(sqlRequirements + " WHERE r.RequirementID = @RequirementID", requirementTable,
                            Param("@RequirementID", SqlDbType.Int, requirement.RequirementID));
                row = FindRow(requirementTable, "RequirementID", requirement.RequirementID);
                if (row == null)
                {
                    throw new ArgumentException("The requirement could not be found. Refresh the list and try again.");
                }
                FillRow(row, requirement);
            }
        }

        /// <summary>
        /// Sends the requirement change (made by DataSetChange) to the database. Returns the
        /// RequirementID (the new key for Add, which is also set on the requirement).
        /// </summary>
        public int UpdateDataSource(ProgrammeRequirement requirement, DB.DBOperation operation)
        {
            if (operation == DB.DBOperation.Add)
            {
                Create_INSERT_Command(requirement);
            }
            else
            {
                Create_UPDATE_Command(requirement);
            }
            UpdateDataSource(requirementTable);

            if (operation == DB.DBOperation.Add)
            {
                DataTable table = GetTable(requirementTable);
                requirement.RequirementID = Convert.ToInt32(table.Rows[table.Rows.Count - 1]["RequirementID"]);
            }
            return requirement.RequirementID;
        }
        #endregion

        #region Build Parameters and Create Commands
        /// <summary>The INSERT for a programme; the new ProgrammeID comes back into the row.</summary>
        private void Create_INSERT_Command(DegreeProgramme programme)
        {
            daMain.InsertCommand = new SqlCommand("INSERT INTO DegreeProgramme (FacultyID, ProgrammeName, MinimumOverallAverage, IsActive) "
                                                  + "VALUES (@FacultyID, @ProgrammeName, @MinimumOverallAverage, @IsActive); "
                                                  + "SELECT CAST(SCOPE_IDENTITY() AS INT) AS ProgrammeID", cnMain);
            daMain.InsertCommand.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;
            Build_INSERT_Parameters(programme);
        }

        /// <summary>One parameter per programme column, each reading its value from the row (Waitering Build_INSERT_Parameters).</summary>
        private void Build_INSERT_Parameters(DegreeProgramme programme)
        {
            daMain.InsertCommand.Parameters.Add(SourceParam("@FacultyID", SqlDbType.Int, "FacultyID"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@ProgrammeName", SqlDbType.VarChar, "ProgrammeName"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@MinimumOverallAverage", SqlDbType.Decimal, "MinimumOverallAverage"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@IsActive", SqlDbType.Bit, "IsActive"));
        }

        /// <summary>The UPDATE for a programme, matched on ProgrammeID.</summary>
        private void Create_UPDATE_Command(DegreeProgramme programme)
        {
            daMain.UpdateCommand = new SqlCommand("UPDATE DegreeProgramme SET FacultyID = @FacultyID, ProgrammeName = @ProgrammeName, "
                                                  + "MinimumOverallAverage = @MinimumOverallAverage, IsActive = @IsActive "
                                                  + "WHERE ProgrammeID = @ProgrammeID", cnMain);
            Build_UPDATE_Parameters(programme);
        }

        /// <summary>The UPDATE parameters: every column plus the key.</summary>
        private void Build_UPDATE_Parameters(DegreeProgramme programme)
        {
            daMain.UpdateCommand.Parameters.Add(SourceParam("@FacultyID", SqlDbType.Int, "FacultyID"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@ProgrammeName", SqlDbType.VarChar, "ProgrammeName"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@MinimumOverallAverage", SqlDbType.Decimal, "MinimumOverallAverage"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@IsActive", SqlDbType.Bit, "IsActive"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@ProgrammeID", SqlDbType.Int, "ProgrammeID"));
        }

        /// <summary>The INSERT for a requirement; the new RequirementID comes back into the row.</summary>
        private void Create_INSERT_Command(ProgrammeRequirement requirement)
        {
            daMain.InsertCommand = new SqlCommand("INSERT INTO ProgrammeRequirement (ProgrammeID, SubjectID, RequirementType, MinimumMark, "
                                                  + "AlternativeSubjectID, AlternativeMinimumMark, AdvisoryNote, IsActive) "
                                                  + "VALUES (@ProgrammeID, @SubjectID, @RequirementType, @MinimumMark, "
                                                  + "@AlternativeSubjectID, @AlternativeMinimumMark, @AdvisoryNote, @IsActive); "
                                                  + "SELECT CAST(SCOPE_IDENTITY() AS INT) AS RequirementID", cnMain);
            daMain.InsertCommand.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;
            Build_INSERT_Parameters(requirement);
        }

        /// <summary>One parameter per requirement column, each reading its value from the row.</summary>
        private void Build_INSERT_Parameters(ProgrammeRequirement requirement)
        {
            daMain.InsertCommand.Parameters.Add(SourceParam("@ProgrammeID", SqlDbType.Int, "ProgrammeID"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@SubjectID", SqlDbType.Int, "SubjectID"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@RequirementType", SqlDbType.VarChar, "RequirementType"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@MinimumMark", SqlDbType.Decimal, "MinimumMark"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@AlternativeSubjectID", SqlDbType.Int, "AlternativeSubjectID"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@AlternativeMinimumMark", SqlDbType.Decimal, "AlternativeMinimumMark"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@AdvisoryNote", SqlDbType.VarChar, "AdvisoryNote"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@IsActive", SqlDbType.Bit, "IsActive"));
        }

        /// <summary>The UPDATE for a requirement, matched on RequirementID.</summary>
        private void Create_UPDATE_Command(ProgrammeRequirement requirement)
        {
            daMain.UpdateCommand = new SqlCommand("UPDATE ProgrammeRequirement SET ProgrammeID = @ProgrammeID, SubjectID = @SubjectID, "
                                                  + "RequirementType = @RequirementType, MinimumMark = @MinimumMark, "
                                                  + "AlternativeSubjectID = @AlternativeSubjectID, AlternativeMinimumMark = @AlternativeMinimumMark, "
                                                  + "AdvisoryNote = @AdvisoryNote, IsActive = @IsActive "
                                                  + "WHERE RequirementID = @RequirementID", cnMain);
            Build_UPDATE_Parameters(requirement);
        }

        /// <summary>The UPDATE parameters: every column plus the key.</summary>
        private void Build_UPDATE_Parameters(ProgrammeRequirement requirement)
        {
            daMain.UpdateCommand.Parameters.Add(SourceParam("@ProgrammeID", SqlDbType.Int, "ProgrammeID"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@SubjectID", SqlDbType.Int, "SubjectID"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@RequirementType", SqlDbType.VarChar, "RequirementType"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@MinimumMark", SqlDbType.Decimal, "MinimumMark"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@AlternativeSubjectID", SqlDbType.Int, "AlternativeSubjectID"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@AlternativeMinimumMark", SqlDbType.Decimal, "AlternativeMinimumMark"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@AdvisoryNote", SqlDbType.VarChar, "AdvisoryNote"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@IsActive", SqlDbType.Bit, "IsActive"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@RequirementID", SqlDbType.Int, "RequirementID"));
        }
        #endregion
    }
}
