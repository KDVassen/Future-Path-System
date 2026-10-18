
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using FuturePath.ApplicationLayer;

namespace FuturePath.DatabaseLayer
{
    // Handles database operations for advisory sessions and recommendation results
    // Loads, saves and updates sessions and their programme recommendations
    public class AdvisorySessionRepository : DB
    {
        #region Data Members
        private string table = "AdvisorySession";
        private string resultTable = "RecommendationResult";

        private string sqlLocal = "SELECT SessionID, LearnerID, GradeLevelID, PreferredFacultyID, SelectedProgrammeID, SessionDate, AdvisingYear, "
                                + "FundingInfoRequested, AlternativesInSummary, SessionStatus, DateCompleted FROM AdvisorySession";

        // Load recommendation results with their programme and faculty names
        private string sqlResults = "SELECT r.ResultID, r.SessionID, r.ProgrammeID, p.ProgrammeName, f.FacultyName, r.SubjectMatchScore, "
                                  + "r.MarkMatchScore, r.InterestMatchScore, r.FacultyBonus, r.OverallScore, r.OutcomeCode, r.OutcomeReason, "
                                  + "r.AlternativeRank, r.IsSelectedProgramme "
                                  + "FROM RecommendationResult r INNER JOIN DegreeProgramme p ON p.ProgrammeID = r.ProgrammeID "
                                  + "INNER JOIN Faculty f ON f.FacultyID = p.FacultyID";

        private string sqlInsertResult = "INSERT INTO RecommendationResult (SessionID, ProgrammeID, SubjectMatchScore, MarkMatchScore, "
                                       + "InterestMatchScore, FacultyBonus, OverallScore, OutcomeCode, OutcomeReason, AlternativeRank, IsSelectedProgramme) "
                                       + "VALUES (@SessionID, @ProgrammeID, @SubjectMatchScore, @MarkMatchScore, @InterestMatchScore, @FacultyBonus, "
                                       + "@OverallScore, @OutcomeCode, @OutcomeReason, @AlternativeRank, @IsSelectedProgramme); "
                                       + "SELECT CAST(SCOPE_IDENTITY() AS INT) AS ResultID";

        private Collection<AdvisorySession> sessions;
        private Collection<RecommendationResult> results;
        #endregion

        #region Constructor
        // Initialise the session and recommendation result collections
        public AdvisorySessionRepository() : base()
        {
            sessions = new Collection<AdvisorySession>();
            results = new Collection<RecommendationResult>();
        }
        #endregion

        #region Read
        // Find an advisory session using its ID
        // Load the outcome from its selected programme result
        public AdvisorySession FindById(int sessionID)
        {
            FillDataSet(sqlLocal + " WHERE SessionID = @SessionID", table, Param("@SessionID", SqlDbType.Int, sessionID));
            Add2Collection();
            if (sessions.Count == 0)
            {
                return null;
            }
            AdvisorySession session = sessions[0];

            // Get the saved outcome of the selected programme
            object outcomeCode = ExecuteScalar("SELECT OutcomeCode FROM RecommendationResult WHERE SessionID = @SessionID AND IsSelectedProgramme = 1",
                                               Param("@SessionID", SqlDbType.Int, sessionID));
            if (outcomeCode == null)
            {
                session.RecordOutcome("");
            }
            else
            {
                session.RecordOutcome(Convert.ToString(outcomeCode));
            }
            return session;
        }

        // Find the selected programme's recommendation result for a session
        public RecommendationResult FindSelectedResult(int sessionID)
        {
            FillDataSet(sqlResults + " WHERE r.SessionID = @SessionID AND r.IsSelectedProgramme = 1", resultTable,
                        Param("@SessionID", SqlDbType.Int, sessionID));
            AddResults2Collection();
            if (results.Count == 0)
            {
                return null;
            }
            return results[0];
        }

        // Load all recommendation results for a session
        // Show the selected programme first, followed by ranked alternatives
        public Collection<RecommendationResult> FindResults(int sessionID)
        {
            FillDataSet(sqlResults + " WHERE r.SessionID = @SessionID ORDER BY r.IsSelectedProgramme DESC, r.AlternativeRank, p.ProgrammeName",
                        resultTable, Param("@SessionID", SqlDbType.Int, sessionID));
            AddResults2Collection();
            return results;
        }
        #endregion

        #region Utility Methods
        // Convert database rows into AdvisorySession objects
        private void Add2Collection()
        {
            sessions = new Collection<AdvisorySession>();
            foreach (DataRow row in GetTable(table).Rows)
            {
                AdvisorySession session = new AdvisorySession();
                session.SessionID = ReadInt(row, "SessionID");
                session.LearnerID = ReadInt(row, "LearnerID");
                session.GradeLevelID = ReadInt(row, "GradeLevelID");
                session.PreferredFacultyID = ReadNullableInt(row, "PreferredFacultyID");
                session.SelectedProgrammeID = ReadNullableInt(row, "SelectedProgrammeID");
                session.SessionDate = ReadDate(row, "SessionDate").Value;
                session.AdvisingYear = Convert.ToInt16(row["AdvisingYear"]);
                session.FundingInfoRequested = ReadBool(row, "FundingInfoRequested");
                session.AlternativesInSummary = ReadBool(row, "AlternativesInSummary");
                session.SessionStatus = ReadString(row, "SessionStatus");
                session.DateCompleted = ReadDate(row, "DateCompleted");
                sessions.Add(session);
            }
        }

        // Convert database rows into RecommendationResult objects
        private void AddResults2Collection()
        {
            results = new Collection<RecommendationResult>();
            foreach (DataRow row in GetTable(resultTable).Rows)
            {
                RecommendationResult result = new RecommendationResult();
                result.ResultID = ReadInt(row, "ResultID");
                result.SessionID = ReadInt(row, "SessionID");
                result.ProgrammeID = ReadInt(row, "ProgrammeID");
                result.ProgrammeName = ReadString(row, "ProgrammeName");
                result.FacultyName = ReadString(row, "FacultyName");
                result.SubjectMatchScore = ReadDecimal(row, "SubjectMatchScore");
                result.MarkMatchScore = ReadDecimal(row, "MarkMatchScore");
                result.InterestMatchScore = ReadDecimal(row, "InterestMatchScore");
                result.FacultyBonus = ReadBool(row, "FacultyBonus");
                result.OverallScore = ReadDecimal(row, "OverallScore");
                result.OutcomeCode = ReadString(row, "OutcomeCode");
                result.OutcomeReason = ReadString(row, "OutcomeReason");
                result.AlternativeRank = ReadNullableInt(row, "AlternativeRank");
                result.IsSelectedProgramme = ReadBool(row, "IsSelectedProgramme");
                results.Add(result);
            }
        }

        // Copy an advisory session's values into a DataRow
        private void FillRow(DataRow row, AdvisorySession session)
        {
            row["LearnerID"] = session.LearnerID;
            row["GradeLevelID"] = session.GradeLevelID;
            row["PreferredFacultyID"] = DbValue(session.PreferredFacultyID);
            row["SelectedProgrammeID"] = DbValue(session.SelectedProgrammeID);
            row["SessionDate"] = session.SessionDate;
            row["AdvisingYear"] = session.AdvisingYear;
            row["FundingInfoRequested"] = session.FundingInfoRequested;
            row["AlternativesInSummary"] = session.AlternativesInSummary;
            row["SessionStatus"] = session.SessionStatus;
            row["DateCompleted"] = DbValue(session.DateCompleted);
        }

        // Insert a recommendation result using the current transaction
        // Store the generated result ID after insertion
        private void InsertResult(SqlTransaction transaction, RecommendationResult result)
        {
            object newID = ExecuteScalar(transaction, sqlInsertResult,
                                         Param("@SessionID", SqlDbType.Int, result.SessionID),
                                         Param("@ProgrammeID", SqlDbType.Int, result.ProgrammeID),
                                         Param("@SubjectMatchScore", SqlDbType.Decimal, result.SubjectMatchScore),
                                         Param("@MarkMatchScore", SqlDbType.Decimal, result.MarkMatchScore),
                                         Param("@InterestMatchScore", SqlDbType.Decimal, result.InterestMatchScore),
                                         Param("@FacultyBonus", SqlDbType.Bit, result.FacultyBonus),
                                         Param("@OverallScore", SqlDbType.Decimal, result.OverallScore),
                                         Param("@OutcomeCode", SqlDbType.VarChar, result.OutcomeCode),
                                         Param("@OutcomeReason", SqlDbType.VarChar, TextOrEmpty(result.OutcomeReason)),
                                         Param("@AlternativeRank", SqlDbType.Int, result.AlternativeRank),
                                         Param("@IsSelectedProgramme", SqlDbType.Bit, result.IsSelectedProgramme));
            result.ResultID = Convert.ToInt32(newID);
        }
        #endregion

        #region Write
        // Add a new advisory session or update an existing one
        // Save the generated session ID when a new record is inserted
        public AdvisorySession Save(AdvisorySession session)
        {
            DB.DBOperation operation = DB.DBOperation.Edit;
            if (session.SessionID == 0)
            {
                operation = DB.DBOperation.Add;
            }
            DataSetChange(session, operation);
            UpdateDataSource(session, operation);
            return session;
        }

        // Save the selected programme's recommendation result
        // Replace previous selected results within a single transaction
        public RecommendationResult Save(RecommendationResult result)
        {
            // Selected programmes do not have an alternative rank
            result.IsSelectedProgramme = true;
            result.AlternativeRank = null;

            SqlTransaction transaction = BeginTransaction();
            try
            {
                // Remove the previous selected result and any duplicate programme result
                ExecuteNonQuery(transaction, "DELETE FROM RecommendationResult WHERE SessionID = @SessionID AND (IsSelectedProgramme = 1 OR ProgrammeID = @ProgrammeID)",
                                Param("@SessionID", SqlDbType.Int, result.SessionID),
                                Param("@ProgrammeID", SqlDbType.Int, result.ProgrammeID));

                // Insert the new selected programme result
                InsertResult(transaction, result);
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
            return result;
        }

        // Save all ranked alternative recommendations for a session
        // Replace previous alternatives while keeping the selected programme
        public bool SaveAll(Collection<RecommendationResult> results)
        {
            if (results == null || results.Count == 0)
            {
                return true;
            }
            int sessionID = results[0].SessionID;
            foreach (RecommendationResult result in results)
            {
                if (result.SessionID != sessionID)
                {
                    throw new ArgumentException("All alternative results must belong to one advisory session.");
                }
            }

            SqlTransaction transaction = BeginTransaction();
            try
            {
                // Remove previous alternatives without deleting the selected result
                ExecuteNonQuery(transaction, "DELETE FROM RecommendationResult WHERE SessionID = @SessionID AND IsSelectedProgramme = 0",
                                Param("@SessionID", SqlDbType.Int, sessionID));

                // Insert the updated ranked alternative results
                foreach (RecommendationResult result in results)
                {
                    result.IsSelectedProgramme = false;
                    InsertResult(transaction, result);
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
            return true;
        }

        // Save the learner's preferred faculty and selected degree programme
        public void SaveProgrammeSelection(int sessionID, int preferredFacultyID, int selectedProgrammeID)
        {
            ExecuteNonQuery("UPDATE AdvisorySession SET PreferredFacultyID = @PreferredFacultyID, SelectedProgrammeID = @SelectedProgrammeID WHERE SessionID = @SessionID",
                            Param("@PreferredFacultyID", SqlDbType.Int, preferredFacultyID),
                            Param("@SelectedProgrammeID", SqlDbType.Int, selectedProgrammeID),
                            Param("@SessionID", SqlDbType.Int, sessionID));
        }

        // Save whether alternative programmes should appear in the learner's summary
        public void SetAlternativesInSummary(int sessionID, bool include)
        {
            ExecuteNonQuery("UPDATE AdvisorySession SET AlternativesInSummary = @Include WHERE SessionID = @SessionID",
                            Param("@Include", SqlDbType.Bit, include),
                            Param("@SessionID", SqlDbType.Int, sessionID));
        }

        // Save whether funding information should appear in the learner's summary
        public void SetFundingInfoRequested(int sessionID, bool requested)
        {
            ExecuteNonQuery("UPDATE AdvisorySession SET FundingInfoRequested = @Requested WHERE SessionID = @SessionID",
                            Param("@Requested", SqlDbType.Bit, requested),
                            Param("@SessionID", SqlDbType.Int, sessionID));
        }

        // Mark an advisory session as completed and save its completion date
        public void MarkCompleted(int sessionID, DateTime dateCompleted)
        {
            ExecuteNonQuery("UPDATE AdvisorySession SET SessionStatus = @Status, DateCompleted = @DateCompleted WHERE SessionID = @SessionID",
                            Param("@Status", SqlDbType.VarChar, AdvisorySession.StatusCompleted),
                            Param("@DateCompleted", SqlDbType.DateTime, dateCompleted),
                            Param("@SessionID", SqlDbType.Int, sessionID));
        }
        #endregion

        #region Database Operations - Waitering pattern
        // Add a new session row or update an existing row in the DataSet
        private void DataSetChange(AdvisorySession session, DB.DBOperation operation)
        {
            DataRow row;
            if (operation == DB.DBOperation.Add)
            {
                // Load the table structure before adding a new row
                FillDataSet(sqlLocal + " WHERE 1 = 0", table);
                row = GetTable(table).NewRow();
                FillRow(row, session);
                GetTable(table).Rows.Add(row);
            }
            else
            {
                FillDataSet(sqlLocal + " WHERE SessionID = @SessionID", table, Param("@SessionID", SqlDbType.Int, session.SessionID));
                row = FindRow(table, "SessionID", session.SessionID);
                if (row == null)
                {
                    throw new ArgumentException("We could not find your advisory session. Start again from step 1.");
                }
                FillRow(row, session);
            }
        }

        // Send session changes from the DataSet to the database
        // Retrieve the new session ID after an insert
        private void UpdateDataSource(AdvisorySession session, DB.DBOperation operation)
        {
            if (operation == DB.DBOperation.Add)
            {
                Create_INSERT_Command(session);
            }
            else
            {
                Create_UPDATE_Command(session);
            }
            UpdateDataSource(table);

            if (operation == DB.DBOperation.Add)
            {
                // Read the newly generated session ID from the inserted row
                DataTable sessionTable = GetTable(table);
                session.SessionID = Convert.ToInt32(sessionTable.Rows[sessionTable.Rows.Count - 1]["SessionID"]);
            }
        }
        #endregion

        #region Build Parameters and Create Commands
        // Create the SQL command for inserting a new advisory session
        private void Create_INSERT_Command(AdvisorySession session)
        {
            daMain.InsertCommand = new SqlCommand("INSERT INTO AdvisorySession (LearnerID, GradeLevelID, PreferredFacultyID, SelectedProgrammeID, "
                                                  + "SessionDate, AdvisingYear, FundingInfoRequested, AlternativesInSummary, SessionStatus, DateCompleted) "
                                                  + "VALUES (@LearnerID, @GradeLevelID, @PreferredFacultyID, @SelectedProgrammeID, @SessionDate, "
                                                  + "@AdvisingYear, @FundingInfoRequested, @AlternativesInSummary, @SessionStatus, @DateCompleted); "
                                                  + "SELECT CAST(SCOPE_IDENTITY() AS INT) AS SessionID", cnMain);
            daMain.InsertCommand.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;
            Build_INSERT_Parameters(session);
        }

        // Add the required parameters to the INSERT command
        private void Build_INSERT_Parameters(AdvisorySession session)
        {
            AddSessionParameters(daMain.InsertCommand);
        }

        // Create the SQL command for updating an existing advisory session
        private void Create_UPDATE_Command(AdvisorySession session)
        {
            daMain.UpdateCommand = new SqlCommand("UPDATE AdvisorySession SET LearnerID = @LearnerID, GradeLevelID = @GradeLevelID, "
                                                  + "PreferredFacultyID = @PreferredFacultyID, SelectedProgrammeID = @SelectedProgrammeID, "
                                                  + "SessionDate = @SessionDate, AdvisingYear = @AdvisingYear, FundingInfoRequested = @FundingInfoRequested, "
                                                  + "AlternativesInSummary = @AlternativesInSummary, SessionStatus = @SessionStatus, "
                                                  + "DateCompleted = @DateCompleted WHERE SessionID = @SessionID", cnMain);
            Build_UPDATE_Parameters(session);
        }

        // Add the required parameters to the UPDATE command
        private void Build_UPDATE_Parameters(AdvisorySession session)
        {
            AddSessionParameters(daMain.UpdateCommand);
            daMain.UpdateCommand.Parameters.Add(SourceParam("@SessionID", SqlDbType.Int, "SessionID"));
        }

        // Add the shared session parameters for INSERT and UPDATE commands
        private static void AddSessionParameters(SqlCommand command)
        {
            command.Parameters.Add(SourceParam("@LearnerID", SqlDbType.Int, "LearnerID"));
            command.Parameters.Add(SourceParam("@GradeLevelID", SqlDbType.Int, "GradeLevelID"));
            command.Parameters.Add(SourceParam("@PreferredFacultyID", SqlDbType.Int, "PreferredFacultyID"));
            command.Parameters.Add(SourceParam("@SelectedProgrammeID", SqlDbType.Int, "SelectedProgrammeID"));
            command.Parameters.Add(SourceParam("@SessionDate", SqlDbType.DateTime, "SessionDate"));
            command.Parameters.Add(SourceParam("@AdvisingYear", SqlDbType.SmallInt, "AdvisingYear"));
            command.Parameters.Add(SourceParam("@FundingInfoRequested", SqlDbType.Bit, "FundingInfoRequested"));
            command.Parameters.Add(SourceParam("@AlternativesInSummary", SqlDbType.Bit, "AlternativesInSummary"));
            command.Parameters.Add(SourceParam("@SessionStatus", SqlDbType.VarChar, "SessionStatus"));
            command.Parameters.Add(SourceParam("@DateCompleted", SqlDbType.DateTime, "DateCompleted"));
        }
        #endregion
    }
}
