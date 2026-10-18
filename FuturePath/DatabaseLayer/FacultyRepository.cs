
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using FuturePath.ApplicationLayer;

namespace FuturePath.DatabaseLayer
{
    // Handles database operations for faculties
    // Loads faculty details and saves new or updated faculty records
    public class FacultyRepository : DB
    {
        #region Data Members
        private string table = "Faculty";
        private string sqlLocal = "SELECT FacultyID, FacultyName, IsActive FROM Faculty";
        private Collection<Faculty> faculties;
        #endregion

        #region Constructor
        // Initialise the faculty collection
        public FacultyRepository() : base()
        {
            faculties = new Collection<Faculty>();
        }
        #endregion

        #region Read
        // Load faculties ordered by name
        // Include inactive faculties when requested
        public Collection<Faculty> GetFaculties(bool includeInactive)
        {
            string sql = sqlLocal;
            if (!includeInactive)
            {
                sql += " WHERE IsActive = 1";
            }
            FillDataSet(sql + " ORDER BY FacultyName", table);
            Add2Collection();
            return faculties;
        }

        // Find a faculty by ID or return null if it does not exist
        public Faculty FindById(int facultyID)
        {
            FillDataSet(sqlLocal + " WHERE FacultyID = @FacultyID", table, Param("@FacultyID", SqlDbType.Int, facultyID));
            Add2Collection();
            if (faculties.Count == 0)
            {
                return null;
            }
            return faculties[0];
        }

        // Check whether another faculty already has the same name
        public bool FacultyNameExists(string facultyName, int excludeFacultyID)
        {
            object count = ExecuteScalar("SELECT COUNT(*) FROM Faculty WHERE FacultyName = @FacultyName AND FacultyID <> @ExcludeID",
                                         Param("@FacultyName", SqlDbType.VarChar, TextOrEmpty(facultyName)),
                                         Param("@ExcludeID", SqlDbType.Int, excludeFacultyID));
            return Convert.ToInt32(count) > 0;
        }
        #endregion

        #region Utility Methods
        // Convert database rows into Faculty objects
        private void Add2Collection()
        {
            faculties = new Collection<Faculty>();
            foreach (DataRow row in GetTable(table).Rows)
            {
                faculties.Add(new Faculty(ReadInt(row, "FacultyID"), ReadString(row, "FacultyName"), ReadBool(row, "IsActive")));
            }
        }

        // Copy a faculty's values into a DataRow
        private void FillRow(DataRow row, Faculty faculty)
        {
            row["FacultyName"] = faculty.FacultyName.Trim();
            row["IsActive"] = faculty.IsActive;
        }
        #endregion

        #region Maintenance - Waitering pattern
        // Add a new faculty row or update an existing row in the DataSet
        public void DataSetChange(Faculty faculty, DB.DBOperation operation)
        {
            DataRow row;
            if (operation == DB.DBOperation.Add)
            {
                // Load the table structure before adding a new faculty
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

        // Save faculty changes from the DataSet to the database
        // Return the faculty ID after saving
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
                // Read the new faculty ID from the inserted row
                DataTable facultyTable = GetTable(table);
                faculty.FacultyID = ReadInt(facultyTable.Rows[facultyTable.Rows.Count - 1], "FacultyID");
            }
            return faculty.FacultyID;
        }
        #endregion

        #region Build Parameters, Create Commands
        // Add the required parameters to the INSERT command
        private void Build_INSERT_Parameters(Faculty faculty)
        {
            daMain.InsertCommand.Parameters.Add(SourceParam("@FacultyName", SqlDbType.VarChar, "FacultyName"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@IsActive", SqlDbType.Bit, "IsActive"));
        }

        // Create the SQL command for inserting a new faculty
        private void Create_INSERT_Command(Faculty faculty)
        {
            daMain.InsertCommand = new SqlCommand("INSERT INTO Faculty (FacultyName, IsActive) VALUES (@FacultyName, @IsActive); " +
                                                  "SELECT CAST(SCOPE_IDENTITY() AS INT) AS FacultyID", cnMain);
            daMain.InsertCommand.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;
            Build_INSERT_Parameters(faculty);
        }

        // Add the required parameters to the UPDATE command
        private void Build_UPDATE_Parameters(Faculty faculty)
        {
            daMain.UpdateCommand.Parameters.Add(SourceParam("@FacultyName", SqlDbType.VarChar, "FacultyName"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@IsActive", SqlDbType.Bit, "IsActive"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@FacultyID", SqlDbType.Int, "FacultyID"));
        }

        // Create the SQL command for updating an existing faculty
        private void Create_UPDATE_Command(Faculty faculty)
        {
            daMain.UpdateCommand = new SqlCommand("UPDATE Faculty SET FacultyName = @FacultyName, IsActive = @IsActive WHERE FacultyID = @FacultyID", cnMain);
            Build_UPDATE_Parameters(faculty);
        }
        #endregion
    }
}
