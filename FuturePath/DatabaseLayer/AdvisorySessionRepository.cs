using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using FuturePath.ApplicationLayer;

namespace FuturePath.DatabaseLayer
{
    /// <summary>
    /// Data access for advisory sessions and their recommendation results (class diagram
    /// AdvisorySessionRepository: save(session), save(result), saveAll(results)).
    /// Re-running a check is safe: UNIQUE(SessionID, ProgrammeID) is respected by replacing rows.
    /// </summary>
    public class AdvisorySessionRepository : DB
    {
        #region Data Members
        private string table = "AdvisorySession";
        private string resultTable = "RecommendationResult";

        private string sqlLocal = "SELECT SessionID, LearnerID, GradeLevelID, PreferredFacultyID, SelectedProgrammeID, SessionDate, AdvisingYear, "
                                + "FundingInfoRequested, AlternativesInSummary, SessionStatus, DateCompleted FROM AdvisorySession";

        // Results with the programme and faculty names, for display
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
        /// <summary>Creates the repository (no database work until a method is called).</summary>
        public AdvisorySessionRepository() : base()
        {
            sessions = new Collection<AdvisorySession>();
            results = new Collection<RecommendationResult>();
        }
        #endregion

        #region Read
        /// <summary>
        /// Loads one session, with Outcome set to the selected programme's OutcomeCode ("" if none).
        /// Results are not loaded. Returns null if it does not exist.
        /// </summary>
        public AdvisorySession FindById(int sessionID)
        {
            FillDataSet(sqlLocal + " WHERE SessionID = @SessionID", table, Param("@SessionID", SqlDbType.Int, sessionID));
            Add2Collection();
            if (sessions.Count == 0)
            {
                return null;
            }
            AdvisorySession session = sessions[0];

            // The session's outcome is the OutcomeCode of its selected programme's result row
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

        /// <summary>The selected programme's result (IsSelectedProgramme = 1) with ProgrammeName and FacultyName, or null.</summary>
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

        /// <summary>All results of a session with ProgrammeName and FacultyName: the selected one first, then alternatives by rank.</summary>
        public Collection<RecommendationResult> FindResults(int sessionID)
        {
            FillDataSet(sqlResults + " WHERE r.SessionID = @SessionID ORDER BY r.IsSelectedProgramme DESC, r.AlternativeRank, p.ProgrammeName",
                        resultTable, Param("@SessionID", SqlDbType.Int, sessionID));
            AddResults2Collection();
            return results;
        }
        #endregion

        #region Utility Methods
        /// <summary>Turns the filled AdvisorySession rows into AdvisorySession objects (Waitering Add2Collection).</summary>
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

        /// <summary>Turns the filled RecommendationResult rows into RecommendationResult objects.</summary>
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

        /// <summary>Copies a session's values into a DataRow (Waitering FillRow).</summary>
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

        /// <summary>
        /// Inserts one result row inside the open transaction and sets its new ResultID.
        /// Used by Save(result) and SaveAll.
        /// </summary>
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
        /// <summary>
        /// Class diagram save(session). Inserts the session (sets SessionID) when SessionID is 0,
        /// otherwise updates every column. Returns the same session. Throws ArgumentException if
        /// the session to update does not exist.
        /// </summary>
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

        /// <summary>
        /// Class diagram save(result): stores the selected programme's result. In one transaction
        /// it deletes the session's previous selected row and any row for the same programme,
        /// then inserts this row with IsSelectedProgramme = 1 (sets ResultID). Returns the result.
        /// </summary>
        public RecommendationResult Save(RecommendationResult result)
        {
            // The selected programme's row is never ranked
            result.IsSelectedProgramme = true;
            result.AlternativeRank = null;

            SqlTransaction transaction = BeginTransaction();
            try
            {
                // 1. Remove the old selected row, and an alternative row for the same programme (UNIQUE(SessionID, ProgrammeID))
                ExecuteNonQuery(transaction, "DELETE FROM RecommendationResult WHERE SessionID = @SessionID AND (IsSelectedProgramme = 1 OR ProgrammeID = @ProgrammeID)",
                                Param("@SessionID", SqlDbType.Int, result.SessionID),
                                Param("@ProgrammeID", SqlDbType.Int, result.ProgrammeID));
                // 2. Insert the new selected row
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

        /// <summary>
        /// Class diagram saveAll(results): stores the ranked alternatives. In one transaction it
        /// deletes the session's previous alternative rows (IsSelectedProgramme = 0) and inserts
        /// these (sets each ResultID). All results must belong to one session (ArgumentException
        /// if not). An empty collection changes nothing. Returns true.
        /// </summary>
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
                // 1. Remove the session's previous alternatives (the selected row stays)
                ExecuteNonQuery(transaction, "DELETE FROM RecommendationResult WHERE SessionID = @SessionID AND IsSelectedProgramme = 0",
                                Param("@SessionID", SqlDbType.Int, sessionID));
                // 2. Insert the new ranked alternatives
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

        /// <summary>Stores the preferred faculty and selected programme chosen on UI 5.</summary>
        public void SaveProgrammeSelection(int sessionID, int preferredFacultyID, int selectedProgrammeID)
        {
            ExecuteNonQuery("UPDATE AdvisorySession SET PreferredFacultyID = @PreferredFacultyID, SelectedProgrammeID = @SelectedProgrammeID WHERE SessionID = @SessionID",
                            Param("@PreferredFacultyID", SqlDbType.Int, preferredFacultyID),
                            Param("@SelectedProgrammeID", SqlDbType.Int, selectedProgrammeID),
                            Param("@SessionID", SqlDbType.Int, sessionID));
        }

        /// <summary>Stores the UI 6.1a "include these alternatives in my summary" choice.</summary>
        public void SetAlternativesInSummary(int sessionID, bool include)
        {
            ExecuteNonQuery("UPDATE AdvisorySession SET AlternativesInSummary = @Include WHERE SessionID = @SessionID",
                            Param("@Include", SqlDbType.Bit, include),
                            Param("@SessionID", SqlDbType.Int, sessionID));
        }

        /// <summary>Stores the UI 6.2b "Add to Summary" choice (FundingInfoRequested).</summary>
        public void SetFundingInfoRequested(int sessionID, bool requested)
        {
            ExecuteNonQuery("UPDATE AdvisorySession SET FundingInfoRequested = @Requested WHERE SessionID = @SessionID",
                            Param("@Requested", SqlDbType.Bit, requested),
                            Param("@SessionID", SqlDbType.Int, sessionID));
        }

        /// <summary>Sets SessionStatus = 'Completed' and DateCompleted (UI 7 Save and export).</summary>
        public void MarkCompleted(int sessionID, DateTime dateCompleted)
        {
            ExecuteNonQuery("UPDATE AdvisorySession SET SessionStatus = @Status, DateCompleted = @DateCompleted WHERE SessionID = @SessionID",
                            Param("@Status", SqlDbType.VarChar, AdvisorySession.StatusCompleted),
                            Param("@DateCompleted", SqlDbType.DateTime, dateCompleted),
                            Param("@SessionID", SqlDbType.Int, sessionID));
        }
        #endregion

        #region Database Operations - Waitering pattern
        /// <summary>Adds a new session row to the DataSet, or changes the existing row (Edit).</summary>
        private void DataSetChange(AdvisorySession session, DB.DBOperation operation)
        {
            DataRow row;
            if (operation == DB.DBOperation.Add)
            {
                // An empty copy of the table gives the columns for the new row
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

        /// <summary>Sends the session change to the database; for Add, sets the new SessionID.</summary>
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
                // The INSERT returned the new key into the row; read it before any other fill
                DataTable sessionTable = GetTable(table);
                session.SessionID = Convert.ToInt32(sessionTable.Rows[sessionTable.Rows.Count - 1]["SessionID"]);
            }
        }
        #endregion

        #region Build Parameters and Create Commands
        /// <summary>The INSERT for a session; the new SessionID comes back into the row.</summary>
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

        /// <summary>One parameter per session column, each reading its value from the row (Waitering Build_INSERT_Parameters).</summary>
        private void Build_INSERT_Parameters(AdvisorySession session)
        {
            AddSessionParameters(daMain.InsertCommand);
        }

        /// <summary>The UPDATE for a session: every column, matched on SessionID.</summary>
        private void Create_UPDATE_Command(AdvisorySession session)
        {
            daMain.UpdateCommand = new SqlCommand("UPDATE AdvisorySession SET LearnerID = @LearnerID, GradeLevelID = @GradeLevelID, "
                                                  + "PreferredFacultyID = @PreferredFacultyID, SelectedProgrammeID = @SelectedProgrammeID, "
                                                  + "SessionDate = @SessionDate, AdvisingYear = @AdvisingYear, FundingInfoRequested = @FundingInfoRequested, "
                                                  + "AlternativesInSummary = @AlternativesInSummary, SessionStatus = @SessionStatus, "
                                                  + "DateCompleted = @DateCompleted WHERE SessionID = @SessionID", cnMain);
            Build_UPDATE_Parameters(session);
        }

        /// <summary>The UPDATE parameters: every column plus the key.</summary>
        private void Build_UPDATE_Parameters(AdvisorySession session)
        {
            AddSessionParameters(daMain.UpdateCommand);
            daMain.UpdateCommand.Parameters.Add(SourceParam("@SessionID", SqlDbType.Int, "SessionID"));
        }

        /// <summary>The column parameters shared by the INSERT and the UPDATE.</summary>
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
