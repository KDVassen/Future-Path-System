
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using FuturePath.ApplicationLayer;

namespace FuturePath.DatabaseLayer
{
    // Handles database operations for learners and their advisory sessions
    // Loads learner details, subjects and interest responses, and saves changes
    public class LearnerRepository : DB
    {
        #region Data Members
        private string table = "Learner";
        private string sessionTable = "AdvisorySession";
        private string subjectTable = "LearnerSubject";
        private string responseTable = "LearnerInterestResponse";
        private string searchTable = "LearnerSearch";

        private string sqlLocal = "SELECT LearnerID, FirstName, LastName, SchoolName, Province, ContactEmail, ContactNumber FROM Learner";

        // Load a session's subjects with their names and levels
        private string sqlSubjects = "SELECT ls.LearnerSubjectID, ls.SessionID, ls.SubjectID, s.SubjectName, s.SubjectLevel, ls.Mark "
                                   + "FROM LearnerSubject ls INNER JOIN Subject s ON s.SubjectID = ls.SubjectID "
                                   + "WHERE ls.SessionID = @SessionID ORDER BY s.SubjectName";

        // Load a session's interest responses with their categories
        private string sqlResponses = "SELECT r.ResponseID, r.SessionID, r.QuestionID, q.CategoryID, r.ResponseValue "
                                    + "FROM LearnerInterestResponse r INNER JOIN InterestQuestion q ON q.QuestionID = r.QuestionID "
                                    + "WHERE r.SessionID = @SessionID ORDER BY q.DisplayOrder, r.QuestionID";

        // Search learners and show their session count, latest session and highest grade
        private string sqlSearch = "SELECT l.LearnerID, LTRIM(RTRIM(ISNULL(l.FirstName, '') + ' ' + ISNULL(l.LastName, ''))) AS LearnerName, "
                                 + "l.SchoolName, l.Province, COUNT(s.SessionID) AS SessionCount, "
                                 + "MAX(s.SessionDate) AS LastSessionDate, MAX(s.GradeLevelID) AS GradeLevelID "
                                 + "FROM Learner l LEFT JOIN AdvisorySession s ON s.LearnerID = l.LearnerID";
        #endregion

        #region Constructor
        // Initialise the learner repository
        public LearnerRepository() : base()
        {
        }
        #endregion

        #region Read
        // Find a learner and load their latest in-progress or completed session
        // Include their grade, preferred faculty, subjects and interest responses
        public Learner FindById(int learnerID)
        {
            // Look for the learner's latest in-progress session
            object sessionID = ExecuteScalar("SELECT MAX(SessionID) FROM AdvisorySession WHERE LearnerID = @LearnerID AND SessionStatus = @InProgress",
                                             Param("@LearnerID", SqlDbType.Int, learnerID),
                                             Param("@InProgress", SqlDbType.VarChar, AdvisorySession.StatusInProgress));

            // Use the most recent session if none are in progress
            if (sessionID == null)
            {
                sessionID = ExecuteScalar("SELECT MAX(SessionID) FROM AdvisorySession WHERE LearnerID = @LearnerID",
                                          Param("@LearnerID", SqlDbType.Int, learnerID));
            }

            // Load only the learner profile if no sessions exist
            if (sessionID == null)
            {
                return LoadLearner(learnerID, 0);
            }
            return LoadLearner(learnerID, Convert.ToInt32(sessionID));
        }

        // Find a learner using an advisory session ID
        // Load the details associated with that specific session
        public Learner FindBySessionId(int sessionID)
        {
            object learnerID = ExecuteScalar("SELECT LearnerID FROM AdvisorySession WHERE SessionID = @SessionID",
                                             Param("@SessionID", SqlDbType.Int, sessionID));
            if (learnerID == null)
            {
                return null;
            }
            return LoadLearner(Convert.ToInt32(learnerID), sessionID);
        }

        // Search learners using optional name, grade, school and province filters
        // Return a table of matching learners and their session details
        public DataTable FindLearners(string surname, string firstName, int gradeLevelID, string schoolName, string province)
        {
            // Add search conditions for the filters provided
            string sql = sqlSearch + " WHERE 1 = 1";
            List<SqlParameter> parameters = new List<SqlParameter>();
            if (!string.IsNullOrWhiteSpace(surname))
            {
                sql += " AND l.LastName LIKE @Surname";
                parameters.Add(Param("@Surname", SqlDbType.VarChar, surname.Trim() + "%"));
            }
            if (!string.IsNullOrWhiteSpace(firstName))
            {
                sql += " AND l.FirstName LIKE @FirstName";
                parameters.Add(Param("@FirstName", SqlDbType.VarChar, firstName.Trim() + "%"));
            }
            if (!string.IsNullOrWhiteSpace(schoolName))
            {
                sql += " AND l.SchoolName LIKE @SchoolName";
                parameters.Add(Param("@SchoolName", SqlDbType.VarChar, "%" + schoolName.Trim() + "%"));
            }
            if (!string.IsNullOrWhiteSpace(province))
            {
                sql += " AND l.Province = @Province";
                parameters.Add(Param("@Province", SqlDbType.VarChar, province.Trim()));
            }
            sql += " GROUP BY l.LearnerID, l.FirstName, l.LastName, l.SchoolName, l.Province";
            if (gradeLevelID > 0)
            {
                sql += " HAVING MAX(s.GradeLevelID) = @GradeLevelID";
                parameters.Add(Param("@GradeLevelID", SqlDbType.Int, gradeLevelID));
            }

            // Include the grade description in the search results
            sql = "SELECT x.LearnerID, x.LearnerName, g.GradeDescription, x.SchoolName, x.Province, x.SessionCount, x.LastSessionDate "
                + "FROM (" + sql + ") x LEFT JOIN GradeLevel g ON g.GradeLevelID = x.GradeLevelID "
                + "ORDER BY x.LearnerName, x.LearnerID";
            FillDataSet(sql, searchTable, parameters.ToArray());
            return GetTable(searchTable).Copy();
        }
        #endregion

        #region Utility Methods
        // Load a learner profile and the details of the selected session
        // Return null if the learner does not exist
        private Learner LoadLearner(int learnerID, int sessionID)
        {
            // Load the learner's personal details
            FillDataSet(sqlLocal + " WHERE LearnerID = @LearnerID", table, Param("@LearnerID", SqlDbType.Int, learnerID));
            if (GetTable(table).Rows.Count == 0)
            {
                return null;
            }
            Learner learner = Add2Collection(GetTable(table).Rows[0]);
            if (sessionID == 0)
            {
                return learner;
            }

            // Load the session's grade and preferred faculty
            FillDataSet("SELECT SessionID, GradeLevelID, PreferredFacultyID FROM AdvisorySession WHERE SessionID = @SessionID", sessionTable,
                        Param("@SessionID", SqlDbType.Int, sessionID));
            if (GetTable(sessionTable).Rows.Count == 0)
            {
                return learner;
            }
            DataRow sessionRow = GetTable(sessionTable).Rows[0];
            learner.CurrentSessionID = sessionID;
            learner.Grade = (GradeLevel)ReadInt(sessionRow, "GradeLevelID");
            learner.PreferredFacultyID = ReadNullableInt(sessionRow, "PreferredFacultyID");

            // Load the learner's subjects and marks for this session
            FillDataSet(sqlSubjects, subjectTable, Param("@SessionID", SqlDbType.Int, sessionID));
            foreach (DataRow row in GetTable(subjectTable).Rows)
            {
                learner.GetSubjects().Add(new LearnerSubject(ReadInt(row, "LearnerSubjectID"), ReadInt(row, "SessionID"),
                                                             ReadInt(row, "SubjectID"), ReadString(row, "SubjectName"),
                                                             ReadString(row, "SubjectLevel"), ReadDecimal(row, "Mark").Value));
            }

            // Load the session's interest questionnaire responses
            FillDataSet(sqlResponses, responseTable, Param("@SessionID", SqlDbType.Int, sessionID));
            foreach (DataRow row in GetTable(responseTable).Rows)
            {
                learner.GetResponses().Add(new LearnerInterestResponse(ReadInt(row, "ResponseID"), ReadInt(row, "SessionID"),
                                                                       ReadInt(row, "QuestionID"), ReadInt(row, "CategoryID"),
                                                                       ReadInt(row, "ResponseValue")));
            }
            return learner;
        }

        // Convert a database row into a Learner object
        private Learner Add2Collection(DataRow row)
        {
            Learner learner = new Learner();
            learner.LearnerID = ReadInt(row, "LearnerID");
            learner.FirstName = ReadString(row, "FirstName");
            learner.LastName = ReadString(row, "LastName");
            learner.SchoolName = ReadString(row, "SchoolName");
            learner.Province = ReadString(row, "Province");
            learner.ContactEmail = ReadString(row, "ContactEmail");
            learner.ContactNumber = ReadString(row, "ContactNumber");
            return learner;
        }

        // Copy a learner's details into a DataRow
        // Store optional blank values as database NULL values
        private void FillRow(DataRow row, Learner learner)
        {
            row["FirstName"] = DbText(learner.FirstName);
            row["LastName"] = DbText(learner.LastName);
            row["SchoolName"] = learner.SchoolName.Trim();
            row["Province"] = learner.Province.Trim();
            row["ContactEmail"] = DbText(learner.ContactEmail);
            row["ContactNumber"] = DbText(learner.ContactNumber);
        }
        #endregion

        #region Write
        // Insert a new learner or update an existing learner profile
        public void Save(Learner learner)
        {
            DB.DBOperation operation = DB.DBOperation.Edit;
            if (learner.LearnerID == 0)
            {
                operation = DB.DBOperation.Add;
            }
            DataSetChange(learner, operation);
            UpdateDataSource(learner, operation);
        }

        // Replace a session's saved subjects and marks
        // Remove old recommendation results because the marks may have changed
        public void SaveSubjects(int sessionID, Collection<LearnerSubject> subjects)
        {
            SqlTransaction transaction = BeginTransaction();
            try
            {
                // Remove recommendations calculated using previous marks
                ExecuteNonQuery(transaction, "DELETE FROM RecommendationResult WHERE SessionID = @SessionID", Param("@SessionID", SqlDbType.Int, sessionID));

                // Remove the previously saved subjects
                ExecuteNonQuery(transaction, "DELETE FROM LearnerSubject WHERE SessionID = @SessionID", Param("@SessionID", SqlDbType.Int, sessionID));

                // Insert the updated subjects and marks
                foreach (LearnerSubject subject in subjects)
                {
                    object newID = ExecuteScalar(transaction, "INSERT INTO LearnerSubject (SessionID, SubjectID, Mark) VALUES (@SessionID, @SubjectID, @Mark); "
                                                 + "SELECT CAST(SCOPE_IDENTITY() AS INT)",
                                                 Param("@SessionID", SqlDbType.Int, sessionID),
                                                 Param("@SubjectID", SqlDbType.Int, subject.SubjectID),
                                                 Param("@Mark", SqlDbType.Decimal, subject.Mark));
                    subject.SessionID = sessionID;
                    subject.LearnerSubjectID = Convert.ToInt32(newID);
                }
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                CloseConnection();
            }
        }

        // Replace a session's interest questionnaire responses
        // Remove old recommendation results because interest scores may have changed
        public void SaveResponses(int sessionID, Collection<LearnerInterestResponse> responses)
        {
            SqlTransaction transaction = BeginTransaction();
            try
            {
                // Remove recommendations calculated using previous responses
                ExecuteNonQuery(transaction, "DELETE FROM RecommendationResult WHERE SessionID = @SessionID", Param("@SessionID", SqlDbType.Int, sessionID));

                // Remove the previously saved questionnaire responses
                ExecuteNonQuery(transaction, "DELETE FROM LearnerInterestResponse WHERE SessionID = @SessionID", Param("@SessionID", SqlDbType.Int, sessionID));

                // Insert the updated responses
                foreach (LearnerInterestResponse response in responses)
                {
                    object newID = ExecuteScalar(transaction, "INSERT INTO LearnerInterestResponse (SessionID, QuestionID, ResponseValue) "
                                                 + "VALUES (@SessionID, @QuestionID, @ResponseValue); SELECT CAST(SCOPE_IDENTITY() AS INT)",
                                                 Param("@SessionID", SqlDbType.Int, sessionID),
                                                 Param("@QuestionID", SqlDbType.Int, response.QuestionID),
                                                 Param("@ResponseValue", SqlDbType.Int, response.ResponseValue));
                    response.SessionID = sessionID;
                    response.ResponseID = Convert.ToInt32(newID);
                }
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                CloseConnection();
            }
        }
        #endregion

        #region Database Operations - Waitering pattern
        // Add a new learner row or update an existing row in the DataSet
        private void DataSetChange(Learner learner, DB.DBOperation operation)
        {
            DataRow row;
            if (operation == DB.DBOperation.Add)
            {
                // Load the table structure before adding a new learner
                FillDataSet(sqlLocal + " WHERE 1 = 0", table);
                row = GetTable(table).NewRow();
                FillRow(row, learner);
                GetTable(table).Rows.Add(row);
            }
            else
            {
                FillDataSet(sqlLocal + " WHERE LearnerID = @LearnerID", table, Param("@LearnerID", SqlDbType.Int, learner.LearnerID));
                row = FindRow(table, "LearnerID", learner.LearnerID);
                if (row == null)
                {
                    throw new ArgumentException("We could not find your learner profile. Start again from step 1.");
                }
                FillRow(row, learner);
            }
        }

        // Save learner changes from the DataSet to the database
        // Retrieve the new learner ID after insertion
        private void UpdateDataSource(Learner learner, DB.DBOperation operation)
        {
            if (operation == DB.DBOperation.Add)
            {
                Create_INSERT_Command(learner);
            }
            else
            {
                Create_UPDATE_Command(learner);
            }
            UpdateDataSource(table);

            if (operation == DB.DBOperation.Add)
            {
                // Read the new learner ID from the inserted row
                DataTable learnerTable = GetTable(table);
                learner.LearnerID = Convert.ToInt32(learnerTable.Rows[learnerTable.Rows.Count - 1]["LearnerID"]);
            }
        }
        #endregion

        #region Build Parameters and Create Commands
        // Create the SQL command for inserting a new learner
        private void Create_INSERT_Command(Learner learner)
        {
            daMain.InsertCommand = new SqlCommand("INSERT INTO Learner (FirstName, LastName, SchoolName, Province, ContactEmail, ContactNumber) "
                                                  + "VALUES (@FirstName, @LastName, @SchoolName, @Province, @ContactEmail, @ContactNumber); "
                                                  + "SELECT CAST(SCOPE_IDENTITY() AS INT) AS LearnerID", cnMain);
            daMain.InsertCommand.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;
            Build_INSERT_Parameters(learner);
        }

        // Add the learner parameters to the INSERT command
        private void Build_INSERT_Parameters(Learner learner)
        {
            daMain.InsertCommand.Parameters.Add(SourceParam("@FirstName", SqlDbType.VarChar, "FirstName"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@LastName", SqlDbType.VarChar, "LastName"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@SchoolName", SqlDbType.VarChar, "SchoolName"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@Province", SqlDbType.VarChar, "Province"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@ContactEmail", SqlDbType.VarChar, "ContactEmail"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@ContactNumber", SqlDbType.VarChar, "ContactNumber"));
        }

        // Create the SQL command for updating an existing learner
        private void Create_UPDATE_Command(Learner learner)
        {
            daMain.UpdateCommand = new SqlCommand("UPDATE Learner SET FirstName = @FirstName, LastName = @LastName, SchoolName = @SchoolName, "
                                                  + "Province = @Province, ContactEmail = @ContactEmail, ContactNumber = @ContactNumber "
                                                  + "WHERE LearnerID = @LearnerID", cnMain);
            Build_UPDATE_Parameters(learner);
        }

        // Add the learner parameters and ID to the UPDATE command
        private void Build_UPDATE_Parameters(Learner learner)
        {
            daMain.UpdateCommand.Parameters.Add(SourceParam("@FirstName", SqlDbType.VarChar, "FirstName"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@LastName", SqlDbType.VarChar, "LastName"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@SchoolName", SqlDbType.VarChar, "SchoolName"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@Province", SqlDbType.VarChar, "Province"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@ContactEmail", SqlDbType.VarChar, "ContactEmail"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@ContactNumber", SqlDbType.VarChar, "ContactNumber"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@LearnerID", SqlDbType.Int, "LearnerID"));
        }
        #endregion
    }
}
