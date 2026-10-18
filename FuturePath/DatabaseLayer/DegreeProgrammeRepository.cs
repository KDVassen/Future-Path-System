
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using FuturePath.ApplicationLayer;

namespace FuturePath.DatabaseLayer
{
    // Handles database operations for degree programmes and their requirements
    // Loads programme details, interest mappings and saves administrator changes
    public class DegreeProgrammeRepository : DB
    {
        #region Data Members
        private string programmeTable = "DegreeProgramme";
        private string requirementTable = "ProgrammeRequirement";
        private string mappingTable = "DegreeInterestMapping";

        // Load programmes together with their faculty names
        private string sqlProgrammes = "SELECT p.ProgrammeID, p.FacultyID, f.FacultyName, p.ProgrammeName, p.MinimumOverallAverage, p.IsActive "
                                     + "FROM DegreeProgramme p INNER JOIN Faculty f ON f.FacultyID = p.FacultyID";

        // Load requirements with subject names and optional alternative subjects
        private string sqlRequirements = "SELECT r.RequirementID, r.ProgrammeID, r.SubjectID, s.SubjectName, r.RequirementType, r.MinimumMark, "
                                       + "r.AlternativeSubjectID, a.SubjectName AS AlternativeSubjectName, r.AlternativeMinimumMark, r.AdvisoryNote, r.IsActive "
                                       + "FROM ProgrammeRequirement r INNER JOIN Subject s ON s.SubjectID = r.SubjectID "
                                       + "LEFT JOIN Subject a ON a.SubjectID = r.AlternativeSubjectID";

        // Load a programme's interest mappings for active categories
        private string sqlMappings = "SELECT m.ProgrammeID, m.CategoryID, c.CategoryName, m.RelevanceWeight "
                                   + "FROM DegreeInterestMapping m INNER JOIN InterestCategory c ON c.CategoryID = m.CategoryID "
                                   + "WHERE m.ProgrammeID = @ProgrammeID AND c.IsActive = 1 ORDER BY c.CategoryName";

        private Collection<DegreeProgramme> programmes;
        private Collection<ProgrammeRequirement> requirements;
        #endregion

        #region Constructor
        // Initialise the programme and requirement collections
        public DegreeProgrammeRepository() : base()
        {
            programmes = new Collection<DegreeProgramme>();
            requirements = new Collection<ProgrammeRequirement>();
        }
        #endregion

        #region Read - eligibility engine
        // Find a programme and load its requirements and interest mappings
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

        // Load active programmes except the selected programme
        // Include their requirements and interest mappings for recommendations
        public Collection<DegreeProgramme> FindActiveProgrammes(int excludedProgrammeID)
        {
            FillDataSet(sqlProgrammes + " WHERE p.IsActive = 1 AND f.IsActive = 1 AND p.ProgrammeID <> @ExcludedID ORDER BY p.ProgrammeName",
                        programmeTable, Param("@ExcludedID", SqlDbType.Int, excludedProgrammeID));
            Add2Collection();
            Collection<DegreeProgramme> candidates = programmes;

            // Load the requirements and mappings for each programme
            foreach (DegreeProgramme programme in candidates)
            {
                LoadRequirementsAndMappings(programme);
            }
            return candidates;
        }
        #endregion

        #region Read - lists
        // Find a programme by ID without loading its requirements
        public DegreeProgramme FindById(int programmeID)
        {
            FillDataSet(sqlProgrammes + " WHERE p.ProgrammeID = @ProgrammeID", programmeTable,
                        Param("@ProgrammeID", SqlDbType.Int, programmeID));
            Add2Collection();
            if (programmes.Count == 0)
            {
                return null;
            }
            return programmes[0];
        }

        // Load programmes for the selected faculty
        // Optionally include inactive programmes and faculties
        public Collection<DegreeProgramme> GetProgrammes(int facultyID, bool includeInactive)
        {
            string sql = sqlProgrammes + " WHERE 1 = 1";
            List<SqlParameter> parameters = new List<SqlParameter>();
            if (facultyID > 0)
            {
                sql += " AND p.FacultyID = @FacultyID";
                parameters.Add(Param("@FacultyID", SqlDbType.Int, facultyID));
            }
            if (!includeInactive)
            {
                sql += " AND p.IsActive = 1 AND f.IsActive = 1";
            }
            FillDataSet(sql + " ORDER BY p.ProgrammeName", programmeTable, parameters.ToArray());
            Add2Collection();
            return programmes;
        }

        // Search active programmes by name
        // Return an empty collection if the search text is blank
        public Collection<DegreeProgramme> SearchActiveProgrammes(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new Collection<DegreeProgramme>();
            }
            FillDataSet(sqlProgrammes + " WHERE p.IsActive = 1 AND f.IsActive = 1 AND p.ProgrammeName LIKE '%' + @Text + '%' ORDER BY p.ProgrammeName",
                        programmeTable, Param("@Text", SqlDbType.VarChar, text.Trim()));
            Add2Collection();
            return programmes;
        }

        // Check whether another programme already has the given name
        public bool ProgrammeNameExists(string programmeName, int excludeProgrammeID)
        {
            object count = ExecuteScalar("SELECT COUNT(*) FROM DegreeProgramme WHERE ProgrammeName = @Name AND ProgrammeID <> @ExcludeID",
                                         Param("@Name", SqlDbType.VarChar, TextOrEmpty(programmeName)),
                                         Param("@ExcludeID", SqlDbType.Int, excludeProgrammeID));
            return Convert.ToInt32(count) > 0;
        }

        // Load the requirements for a programme
        // Optionally include requirements that have been deactivated
        public Collection<ProgrammeRequirement> GetRequirements(int programmeID, bool includeInactive)
        {
            string sql = sqlRequirements + " WHERE r.ProgrammeID = @ProgrammeID";
            if (!includeInactive)
            {
                sql += " AND r.IsActive = 1";
            }
            FillDataSet(sql + " ORDER BY r.RequirementID", requirementTable, Param("@ProgrammeID", SqlDbType.Int, programmeID));
            AddRequirements2Collection();
            return requirements;
        }

        // Find a programme requirement by its ID
        public ProgrammeRequirement FindRequirementById(int requirementID)
        {
            FillDataSet(sqlRequirements + " WHERE r.RequirementID = @RequirementID", requirementTable,
                        Param("@RequirementID", SqlDbType.Int, requirementID));
            AddRequirements2Collection();
            if (requirements.Count == 0)
            {
                return null;
            }
            return requirements[0];
        }

        // Check if the programme already has a requirement for a subject
        public bool RequirementExists(int programmeID, int subjectID, int excludeRequirementID)
        {
            object count = ExecuteScalar("SELECT COUNT(*) FROM ProgrammeRequirement WHERE ProgrammeID = @ProgrammeID AND SubjectID = @SubjectID"
                                         + " AND RequirementID <> @ExcludeID",
                                         Param("@ProgrammeID", SqlDbType.Int, programmeID),
                                         Param("@SubjectID", SqlDbType.Int, subjectID),
                                         Param("@ExcludeID", SqlDbType.Int, excludeRequirementID));
            return Convert.ToInt32(count) > 0;
        }
        #endregion

        #region Utility Methods
        // Convert database rows into DegreeProgramme objects
        private void Add2Collection()
        {
            programmes = new Collection<DegreeProgramme>();
            foreach (DataRow row in GetTable(programmeTable).Rows)
            {
                DegreeProgramme programme = new DegreeProgramme(ReadInt(row, "ProgrammeID"), ReadInt(row, "FacultyID"),
                                                                ReadString(row, "ProgrammeName"), ReadDecimal(row, "MinimumOverallAverage"),
                                                                ReadBool(row, "IsActive"));
                programme.FacultyName = ReadString(row, "FacultyName");
                programmes.Add(programme);
            }
        }

        // Convert database rows into ProgrammeRequirement objects
        private void AddRequirements2Collection()
        {
            requirements = new Collection<ProgrammeRequirement>();
            foreach (DataRow row in GetTable(requirementTable).Rows)
            {
                requirements.Add(new ProgrammeRequirement(ReadInt(row, "RequirementID"), ReadInt(row, "ProgrammeID"),
                                                          ReadInt(row, "SubjectID"), ReadString(row, "SubjectName"),
                                                          ReadString(row, "RequirementType"), ReadDecimal(row, "MinimumMark"),
                                                          ReadNullableInt(row, "AlternativeSubjectID"), ReadString(row, "AlternativeSubjectName"),
                                                          ReadDecimal(row, "AlternativeMinimumMark"), ReadString(row, "AdvisoryNote"),
                                                          ReadBool(row, "IsActive")));
            }
        }

        // Load active subject requirements and interest mappings for a programme
        private void LoadRequirementsAndMappings(DegreeProgramme programme)
        {
            // Add the programme's active subject requirements
            foreach (ProgrammeRequirement requirement in GetRequirements(programme.ProgrammeID, false))
            {
                programme.GetRequirements().Add(requirement);
            }

            // Add interest categories and their relevance weights
            FillDataSet(sqlMappings, mappingTable, Param("@ProgrammeID", SqlDbType.Int, programme.ProgrammeID));
            foreach (DataRow row in GetTable(mappingTable).Rows)
            {
                programme.InterestMappings.Add(new DegreeInterestMapping(ReadInt(row, "ProgrammeID"), ReadInt(row, "CategoryID"),
                                                                         ReadString(row, "CategoryName"), ReadDecimal(row, "RelevanceWeight").Value));
            }
        }

        // Copy a programme's values into a DataRow
        private void FillRow(DataRow row, DegreeProgramme programme)
        {
            row["FacultyID"] = programme.FacultyID;
            row["ProgrammeName"] = TextOrEmpty(programme.ProgrammeName);
            row["MinimumOverallAverage"] = DbValue(programme.MinimumOverallAverage);
            row["IsActive"] = programme.IsActive;
        }

        // Copy a requirement's values into a DataRow
        // Store a missing alternative subject as DBNull
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
        #endregion

        #region Maintenance - Waitering pattern
        // Add a programme row or update an existing row in the DataSet
        public void DataSetChange(DegreeProgramme programme, DB.DBOperation operation)
        {
            DataRow row;
            if (operation == DB.DBOperation.Add)
            {
                // Load the table structure before adding a new programme
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

        // Save programme changes from the DataSet to the database
        // Return the programme ID after saving
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
                // Read the new programme ID from the inserted row
                DataTable table = GetTable(programmeTable);
                programme.ProgrammeID = Convert.ToInt32(table.Rows[table.Rows.Count - 1]["ProgrammeID"]);
            }
            return programme.ProgrammeID;
        }

        // Add a requirement row or update an existing row in the DataSet
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

        // Save requirement changes from the DataSet to the database
        // Return the requirement ID after saving
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
        // Create the SQL command for inserting a degree programme
        private void Create_INSERT_Command(DegreeProgramme programme)
        {
            daMain.InsertCommand = new SqlCommand("INSERT INTO DegreeProgramme (FacultyID, ProgrammeName, MinimumOverallAverage, IsActive) "
                                                  + "VALUES (@FacultyID, @ProgrammeName, @MinimumOverallAverage, @IsActive); "
                                                  + "SELECT CAST(SCOPE_IDENTITY() AS INT) AS ProgrammeID", cnMain);
            daMain.InsertCommand.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;
            Build_INSERT_Parameters(programme);
        }

        // Add the programme parameters to the INSERT command
        private void Build_INSERT_Parameters(DegreeProgramme programme)
        {
            daMain.InsertCommand.Parameters.Add(SourceParam("@FacultyID", SqlDbType.Int, "FacultyID"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@ProgrammeName", SqlDbType.VarChar, "ProgrammeName"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@MinimumOverallAverage", SqlDbType.Decimal, "MinimumOverallAverage"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@IsActive", SqlDbType.Bit, "IsActive"));
        }

        // Create the SQL command for updating a degree programme
        private void Create_UPDATE_Command(DegreeProgramme programme)
        {
            daMain.UpdateCommand = new SqlCommand("UPDATE DegreeProgramme SET FacultyID = @FacultyID, ProgrammeName = @ProgrammeName, "
                                                  + "MinimumOverallAverage = @MinimumOverallAverage, IsActive = @IsActive "
                                                  + "WHERE ProgrammeID = @ProgrammeID", cnMain);
            Build_UPDATE_Parameters(programme);
        }

        // Add the programme parameters and ID to the UPDATE command
        private void Build_UPDATE_Parameters(DegreeProgramme programme)
        {
            daMain.UpdateCommand.Parameters.Add(SourceParam("@FacultyID", SqlDbType.Int, "FacultyID"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@ProgrammeName", SqlDbType.VarChar, "ProgrammeName"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@MinimumOverallAverage", SqlDbType.Decimal, "MinimumOverallAverage"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@IsActive", SqlDbType.Bit, "IsActive"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@ProgrammeID", SqlDbType.Int, "ProgrammeID"));
        }

        // Create the SQL command for inserting a programme requirement
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

        // Add the requirement parameters to the INSERT command
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

        // Create the SQL command for updating a programme requirement
        private void Create_UPDATE_Command(ProgrammeRequirement requirement)
        {
            daMain.UpdateCommand = new SqlCommand("UPDATE ProgrammeRequirement SET ProgrammeID = @ProgrammeID, SubjectID = @SubjectID, "
                                                  + "RequirementType = @RequirementType, MinimumMark = @MinimumMark, "
                                                  + "AlternativeSubjectID = @AlternativeSubjectID, AlternativeMinimumMark = @AlternativeMinimumMark, "
                                                  + "AdvisoryNote = @AdvisoryNote, IsActive = @IsActive "
                                                  + "WHERE RequirementID = @RequirementID", cnMain);
            Build_UPDATE_Parameters(requirement);
        }

        // Add the requirement parameters and ID to the UPDATE command
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
