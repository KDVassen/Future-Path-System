using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using FuturePath.ApplicationLayer;

namespace FuturePath.DatabaseLayer
{
    /// <summary>
    /// Data access for faculties (UI 5 list, UI 13.1 maintenance), in the Waitering
    /// DataSetChange / UpdateDataSource pattern. Faculties are never deleted, only deactivated.
    /// </summary>
    public class FacultyRepository : DB
    {
        #region Data Members
        private string table = "Faculty";
        private string sqlLocal = "SELECT FacultyID, FacultyName, IsActive FROM Faculty";
        private Collection<Faculty> faculties;
        #endregion

        #region Constructor
        /// <summary>Creates the repository (no database work until a method is called).</summary>
        public FacultyRepository() : base()
        {
            faculties = new Collection<Faculty>();
        }
        #endregion

        #region Read
        /// <summary>Faculties ordered by name; inactive ones only when includeInactive is true.</summary>
        public Collection<Faculty> GetFaculties(bool includeInactive)
        {
            FillDataSet(sqlLocal + " WHERE (@IncludeInactive = 1 OR IsActive = 1) ORDER BY FacultyName", table,
                        Param("@IncludeInactive", SqlDbType.Bit, includeInactive));
            Add2Collection();
            return faculties;
        }

        /// <summary>One faculty, or null.</summary>
        public Faculty FindById(int facultyID)
        {
            FillDataSet(sqlLocal + " WHERE FacultyID = @FacultyID", table, Param("@FacultyID", SqlDbType.Int, facultyID));
            Add2Collection();
            return faculties.Count > 0 ? faculties[0] : null;
        }

        /// <summary>True if another faculty already has this name.</summary>
        public bool FacultyNameExists(string facultyName, int? excludeFacultyID)
        {
            object count = ExecuteScalar("SELECT COUNT(*) FROM Faculty WHERE FacultyName = @FacultyName AND (@ExcludeID IS NULL OR FacultyID <> @ExcludeID)",
                                         Param("@FacultyName", SqlDbType.VarChar, (facultyName ?? "").Trim()),
                                         Param("@ExcludeID", SqlDbType.Int, excludeFacultyID));
            return Convert.ToInt32(count) > 0;
        }
        #endregion

        #region Utility Methods
        /// <summary>Turns the filled Faculty rows into Faculty objects (Waitering Add2Collection).</summary>
        private void Add2Collection()
        {
            faculties = new Collection<Faculty>();
            foreach (DataRow row in GetTable(table).Rows)
            {
                faculties.Add(new Faculty(ReadInt(row, "FacultyID").Value, ReadString(row, "FacultyName"), ReadBool(row, "IsActive")));
            }
        }

        /// <summary>Copies the faculty's values into a row (Waitering FillRow). The key is never changed here.</summary>
        private void FillRow(DataRow row, Faculty faculty)
        {
            row["FacultyName"] = faculty.FacultyName.Trim();
            row["IsActive"] = faculty.IsActive;
        }
        #endregion

        #region Maintenance - Waitering pattern
        /// <summary>Adds a new faculty row to the DataSet, or changes the existing row (Edit).</summary>
        public void DataSetChange(Faculty faculty, DB.DBOperation operation)
        {
            DataRow row;
            if (operation == DB.DBOperation.Add)
            {
                // An empty fill gives the table its columns; the new row is added to it.
                FillDataSet(sqlLocal + " WHERE 1 = 0", table);
                row = GetTable(table).NewRow();
                FillRow(row, faculty);
                GetTable(table).Rows.Add(row);
            }
            else
            {
                FillDataSet(sqlLocal + " WHERE FacultyID = @FacultyID", table, Param("@FacultyID", SqlDbType.Int, faculty.FacultyID));
                row = FindRow(table, "FacultyID", faculty.FacultyID);
                if (row == null)
                {
                    throw new InvalidOperationException("The faculty to update was not found.");
                }
                FillRow(row, faculty);
            }
        }

        /// <summary>Sends the faculty change to the database. Returns the FacultyID (the new key for Add).</summary>
        public int UpdateDataSource(Faculty faculty, DB.DBOperation operation)
        {
            if (operation == DB.DBOperation.Add)
            {
                Create_INSERT_Command(faculty);
            }
            else
            {
                Create_UPDATE_Command(faculty);
            }
            UpdateDataSource(table);
            if (operation == DB.DBOperation.Add)
            {
                // The INSERT returned the new key into the row (FirstReturnedRecord).
                DataTable facultyTable = GetTable(table);
                faculty.FacultyID = ReadInt(facultyTable.Rows[facultyTable.Rows.Count - 1], "FacultyID").Value;
            }
            return faculty.FacultyID;
        }
        #endregion

        #region Build Parameters, Create Commands
        /// <summary>The INSERT parameters, each read from a column of the new row (Waitering Build_INSERT_Parameters).</summary>
        private void Build_INSERT_Parameters(Faculty faculty)
        {
            daMain.InsertCommand.Parameters.Add(SourceParam("@FacultyName", SqlDbType.VarChar, "FacultyName"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@IsActive", SqlDbType.Bit, "IsActive"));
        }

        /// <summary>The INSERT command. It returns the new FacultyID into the row.</summary>
        private void Create_INSERT_Command(Faculty faculty)
        {
            daMain.InsertCommand = new SqlCommand("INSERT INTO Faculty (FacultyName, IsActive) VALUES (@FacultyName, @IsActive); " +
                                                  "SELECT CAST(SCOPE_IDENTITY() AS INT) AS FacultyID", cnMain);
            daMain.InsertCommand.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;
            Build_INSERT_Parameters(faculty);
        }

        /// <summary>The UPDATE parameters, each read from a column of the changed row.</summary>
        private void Build_UPDATE_Parameters(Faculty faculty)
        {
            daMain.UpdateCommand.Parameters.Add(SourceParam("@FacultyName", SqlDbType.VarChar, "FacultyName"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@IsActive", SqlDbType.Bit, "IsActive"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@FacultyID", SqlDbType.Int, "FacultyID"));
        }

        /// <summary>The UPDATE command for one faculty.</summary>
        private void Create_UPDATE_Command(Faculty faculty)
        {
            daMain.UpdateCommand = new SqlCommand("UPDATE Faculty SET FacultyName = @FacultyName, IsActive = @IsActive WHERE FacultyID = @FacultyID", cnMain);
            Build_UPDATE_Parameters(faculty);
        }
        #endregion
    }
}
