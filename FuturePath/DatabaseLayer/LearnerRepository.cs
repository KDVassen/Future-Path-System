using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using FuturePath.ApplicationLayer;

namespace FuturePath.DatabaseLayer
{
    /// <summary>
    /// Data access for learners (class diagram LearnerRepository: findById, save).
    /// The in-memory Learner carries its current session's grade, preferred faculty, subjects and
    /// interest responses, so this repository reads Learner together with the AdvisorySession,
    /// LearnerSubject and LearnerInterestResponse rows (classes-and-sequences.md "Mapping the
    /// classes to storage").
    /// </summary>
    public class LearnerRepository : DB
    {
        #region Data Members
        private string table = "Learner";
        private string sessionTable = "AdvisorySession";
        private string subjectTable = "LearnerSubject";
        private string responseTable = "LearnerInterestResponse";
        private string searchTable = "LearnerSearch";

        private string sqlLocal = "SELECT LearnerID, FirstName, LastName, SchoolName, Province, ContactEmail, ContactNumber FROM Learner";

        // The session's subjects with the subject name and level
        private string sqlSubjects = "SELECT ls.LearnerSubjectID, ls.SessionID, ls.SubjectID, s.SubjectName, s.SubjectLevel, ls.Mark "
                                   + "FROM LearnerSubject ls INNER JOIN Subject s ON s.SubjectID = ls.SubjectID "
                                   + "WHERE ls.SessionID = @SessionID ORDER BY s.SubjectName";

        // The session's answers with each question's category
        private string sqlResponses = "SELECT r.ResponseID, r.SessionID, r.QuestionID, q.CategoryID, r.ResponseValue "
                                    + "FROM LearnerInterestResponse r INNER JOIN InterestQuestion q ON q.QuestionID = r.QuestionID "
                                    + "WHERE r.SessionID = @SessionID ORDER BY q.DisplayOrder, r.QuestionID";

        // UI 10.1a search: one row per learner, with the grade of the latest session. No contact details.
        private string sqlSearch = "SELECT l.LearnerID, LTRIM(RTRIM(ISNULL(l.FirstName, '') + ' ' + ISNULL(l.LastName, ''))) AS LearnerName, "
                                 + "g.GradeDescription, l.SchoolName, l.Province, "
                                 + "(SELECT COUNT(*) FROM AdvisorySession c WHERE c.LearnerID = l.LearnerID) AS SessionCount, "
                                 + "latest.SessionDate AS LastSessionDate "
                                 + "FROM Learner l "
                                 + "OUTER APPLY (SELECT TOP 1 s.GradeLevelID, s.SessionDate FROM AdvisorySession s "
                                 + "WHERE s.LearnerID = l.LearnerID ORDER BY s.SessionID DESC) latest "
                                 + "LEFT JOIN GradeLevel g ON g.GradeLevelID = latest.GradeLevelID "
                                 + "WHERE (@Surname IS NULL OR l.LastName LIKE @Surname + '%') "
                                 + "AND (@FirstName IS NULL OR l.FirstName LIKE @FirstName + '%') "
                                 + "AND (@SchoolName IS NULL OR l.SchoolName LIKE '%' + @SchoolName + '%') "
                                 + "AND (@Province IS NULL OR l.Province = @Province) "
                                 + "AND (@GradeLevelID IS NULL OR latest.GradeLevelID = @GradeLevelID) "
                                 + "ORDER BY l.LastName, l.FirstName, l.LearnerID";
        #endregion

        #region Constructor
        /// <summary>Creates the repository (no database work until a method is called).</summary>
        public LearnerRepository() : base()
        {
        }
        #endregion

        #region Read
        /// <summary>
        /// Class diagram findById(learnerId). Loads the learner with their latest advisory session
        /// (the In-progress one if there is one, otherwise the most recent): grade, preferred
        /// faculty, CurrentSessionID, subjects (with names and levels) and responses (with
        /// CategoryID). A learner with no session has CurrentSessionID null. Returns null if the
        /// learner does not exist.
        /// </summary>
        public Learner FindById(int learnerID)
        {
            // In-progress sessions first, then the highest SessionID
            object sessionID = ExecuteScalar("SELECT TOP 1 SessionID FROM AdvisorySession WHERE LearnerID = @LearnerID "
                                             + "ORDER BY CASE WHEN SessionStatus = @InProgress THEN 0 ELSE 1 END, SessionID DESC",
                                             Param("@LearnerID", SqlDbType.Int, learnerID),
                                             Param("@InProgress", SqlDbType.VarChar, AdvisorySession.StatusInProgress));
            if (sessionID == null)
            {
                return LoadLearner(learnerID, null);
            }
            return LoadLearner(learnerID, Convert.ToInt32(sessionID));
        }

        /// <summary>
        /// Loads the learner of one session, with that session's grade, preferred faculty,
        /// subjects and responses (CurrentSessionID = sessionID). Returns null if the session
        /// does not exist.
        /// </summary>
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

        /// <summary>
        /// UI 10.1a learner search. Blank text or null means "no filter". Names match from the
        /// start, the school anywhere in its name, the province exactly, and the grade is the
        /// latest session's grade. Returns columns LearnerID, LearnerName, GradeDescription
        /// (latest session), SchoolName, Province, SessionCount, LastSessionDate (DateTime or
        /// DBNull), as a new table every call. Never returns contact details.
        /// </summary>
        public DataTable FindLearners(string surname, string firstName, int? gradeLevelID, string schoolName, string province)
        {
            FillDataSet(sqlSearch, searchTable,
                        Param("@Surname", SqlDbType.VarChar, FilterText(surname)),
                        Param("@FirstName", SqlDbType.VarChar, FilterText(firstName)),
                        Param("@SchoolName", SqlDbType.VarChar, FilterText(schoolName)),
                        Param("@Province", SqlDbType.VarChar, string.IsNullOrWhiteSpace(province) ? null : province.Trim()),
                        Param("@GradeLevelID", SqlDbType.Int, gradeLevelID));
            return GetTable(searchTable).Copy();
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// The shared loader of FindById and FindBySessionId: the Learner row, then (when there is a
        /// session) the session's grade and preferred faculty, its subjects and its responses.
        /// Returns null if the learner does not exist.
        /// </summary>
        private Learner LoadLearner(int learnerID, int? sessionID)
        {
            // 1. The Learner row
            FillDataSet(sqlLocal + " WHERE LearnerID = @LearnerID", table, Param("@LearnerID", SqlDbType.Int, learnerID));
            if (GetTable(table).Rows.Count == 0)
            {
                return null;
            }
            Learner learner = Add2Collection(GetTable(table).Rows[0]);
            if (!sessionID.HasValue)
            {
                return learner;
            }

            // 2. The session's grade and preferred faculty
            FillDataSet("SELECT SessionID, GradeLevelID, PreferredFacultyID FROM AdvisorySession WHERE SessionID = @SessionID", sessionTable,
                        Param("@SessionID", SqlDbType.Int, sessionID.Value));
            if (GetTable(sessionTable).Rows.Count == 0)
            {
                return learner;
            }
            DataRow sessionRow = GetTable(sessionTable).Rows[0];
            learner.CurrentSessionID = sessionID.Value;
            learner.Grade = (GradeLevel)ReadInt(sessionRow, "GradeLevelID").Value;
            learner.PreferredFacultyID = ReadInt(sessionRow, "PreferredFacultyID");

            // 3. The session's subjects (added straight to the learner's collection)
            FillDataSet(sqlSubjects, subjectTable, Param("@SessionID", SqlDbType.Int, sessionID.Value));
            foreach (DataRow row in GetTable(subjectTable).Rows)
            {
                learner.GetSubjects().Add(new LearnerSubject(ReadInt(row, "LearnerSubjectID").Value, ReadInt(row, "SessionID").Value,
                                                             ReadInt(row, "SubjectID").Value, ReadString(row, "SubjectName"),
                                                             ReadString(row, "SubjectLevel"), ReadDecimal(row, "Mark").Value));
            }

            // 4. The session's interest responses, each with its question's category
            FillDataSet(sqlResponses, responseTable, Param("@SessionID", SqlDbType.Int, sessionID.Value));
            foreach (DataRow row in GetTable(responseTable).Rows)
            {
                learner.GetResponses().Add(new LearnerInterestResponse(ReadInt(row, "ResponseID").Value, ReadInt(row, "SessionID").Value,
                                                                       ReadInt(row, "QuestionID").Value, ReadInt(row, "CategoryID").Value,
                                                                       ReadInt(row, "ResponseValue").Value));
            }
            return learner;
        }

        /// <summary>Turns a filled Learner row into a Learner object (Waitering Add2Collection, for one row).</summary>
        private Learner Add2Collection(DataRow row)
        {
            Learner learner = new Learner();
            learner.LearnerID = ReadInt(row, "LearnerID").Value;
            learner.FirstName = ReadString(row, "FirstName");
            learner.LastName = ReadString(row, "LastName");
            learner.SchoolName = ReadString(row, "SchoolName");
            learner.Province = ReadString(row, "Province");
            learner.ContactEmail = ReadString(row, "ContactEmail");
            learner.ContactNumber = ReadString(row, "ContactNumber");
            return learner;
        }

        /// <summary>Copies a learner's values into a DataRow (Waitering FillRow). Blank optional values are stored as NULL.</summary>
        private void FillRow(DataRow row, Learner learner)
        {
            row["FirstName"] = DbText(learner.FirstName);
            row["LastName"] = DbText(learner.LastName);
            row["SchoolName"] = (learner.SchoolName ?? "").Trim();
            row["Province"] = (learner.Province ?? "").Trim();
            row["ContactEmail"] = DbText(learner.ContactEmail);
            row["ContactNumber"] = DbText(learner.ContactNumber);
        }

        /// <summary>A search value for LIKE: null when blank (no filter), otherwise trimmed with %, _ and [ matching themselves.</summary>
        private static string FilterText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }
            return text.Trim().Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]");
        }
        #endregion

        #region Write
        /// <summary>
        /// Class diagram save(learner). Inserts the Learner row (and sets learner.LearnerID) when
        /// LearnerID is 0, otherwise updates it. Only the Learner table is written; sessions,
        /// subjects and responses have their own methods. Throws ArgumentException if the learner
        /// to update does not exist.
        /// </summary>
        public void Save(Learner learner)
        {
            DB.DBOperation operation = learner.LearnerID == 0 ? DB.DBOperation.Add : DB.DBOperation.Edit;
            DataSetChange(learner, operation);
            UpdateDataSource(learner, operation);
        }

        /// <summary>
        /// Replaces the session's LearnerSubject rows with these subjects, in one transaction.
        /// Also deletes the session's RecommendationResult rows, because they no longer match the
        /// marks. Sets each subject's SessionID and new LearnerSubjectID.
        /// </summary>
        public void SaveSubjects(int sessionID, Collection<LearnerSubject> subjects)
        {
            SqlTransaction transaction = BeginTransaction();
            try
            {
                // 1. The stored results were worked out from the old marks
                ExecuteNonQuery(transaction, "DELETE FROM RecommendationResult WHERE SessionID = @SessionID", Param("@SessionID", SqlDbType.Int, sessionID));
                // 2. Remove the old marks
                ExecuteNonQuery(transaction, "DELETE FROM LearnerSubject WHERE SessionID = @SessionID", Param("@SessionID", SqlDbType.Int, sessionID));
                // 3. Insert the new marks
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

        /// <summary>
        /// Replaces the session's LearnerInterestResponse rows with these responses, in one
        /// transaction. Also deletes the session's RecommendationResult rows (stale interest
        /// scores). Sets each response's SessionID and new ResponseID.
        /// </summary>
        public void SaveResponses(int sessionID, Collection<LearnerInterestResponse> responses)
        {
            SqlTransaction transaction = BeginTransaction();
            try
            {
                // 1. The stored results were worked out from the old answers
                ExecuteNonQuery(transaction, "DELETE FROM RecommendationResult WHERE SessionID = @SessionID", Param("@SessionID", SqlDbType.Int, sessionID));
                // 2. Remove the old answers
                ExecuteNonQuery(transaction, "DELETE FROM LearnerInterestResponse WHERE SessionID = @SessionID", Param("@SessionID", SqlDbType.Int, sessionID));
                // 3. Insert the new answers
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
        /// <summary>Adds a new learner row to the DataSet, or changes the existing row (Edit).</summary>
        private void DataSetChange(Learner learner, DB.DBOperation operation)
        {
            DataRow row;
            if (operation == DB.DBOperation.Add)
            {
                // An empty copy of the table gives the columns for the new row
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

        /// <summary>Sends the learner change to the database; for Add, sets the new LearnerID.</summary>
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
                // The INSERT returned the new key into the row; read it before any other fill
                DataTable learnerTable = GetTable(table);
                learner.LearnerID = Convert.ToInt32(learnerTable.Rows[learnerTable.Rows.Count - 1]["LearnerID"]);
            }
        }
        #endregion

        #region Build Parameters and Create Commands
        /// <summary>The INSERT for a learner; the new LearnerID comes back into the row.</summary>
        private void Create_INSERT_Command(Learner learner)
        {
            daMain.InsertCommand = new SqlCommand("INSERT INTO Learner (FirstName, LastName, SchoolName, Province, ContactEmail, ContactNumber) "
                                                  + "VALUES (@FirstName, @LastName, @SchoolName, @Province, @ContactEmail, @ContactNumber); "
                                                  + "SELECT CAST(SCOPE_IDENTITY() AS INT) AS LearnerID", cnMain);
            daMain.InsertCommand.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;
            Build_INSERT_Parameters(learner);
        }

        /// <summary>One parameter per learner column, each reading its value from the row (Waitering Build_INSERT_Parameters).</summary>
        private void Build_INSERT_Parameters(Learner learner)
        {
            daMain.InsertCommand.Parameters.Add(SourceParam("@FirstName", SqlDbType.VarChar, "FirstName"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@LastName", SqlDbType.VarChar, "LastName"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@SchoolName", SqlDbType.VarChar, "SchoolName"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@Province", SqlDbType.VarChar, "Province"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@ContactEmail", SqlDbType.VarChar, "ContactEmail"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@ContactNumber", SqlDbType.VarChar, "ContactNumber"));
        }

        /// <summary>The UPDATE for a learner, matched on LearnerID.</summary>
        private void Create_UPDATE_Command(Learner learner)
        {
            daMain.UpdateCommand = new SqlCommand("UPDATE Learner SET FirstName = @FirstName, LastName = @LastName, SchoolName = @SchoolName, "
                                                  + "Province = @Province, ContactEmail = @ContactEmail, ContactNumber = @ContactNumber "
                                                  + "WHERE LearnerID = @LearnerID", cnMain);
            Build_UPDATE_Parameters(learner);
        }

        /// <summary>The UPDATE parameters: every column plus the key.</summary>
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
