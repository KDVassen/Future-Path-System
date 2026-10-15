using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using FuturePath.ApplicationLayer;

namespace FuturePath.DatabaseLayer
{
    /// <summary>
    /// Read-only queries for the staff session screens (UI 9 dashboard counts, UI 10.2b session
    /// search). Outcomes are read back from the stored RecommendationResult, never recalculated.
    /// Only SessionSearchController uses this class; its methods are defined with it.
    /// Dates arrive as a start and an exclusive end (the controller works them out), so
    /// "SessionDate &gt;= @Start AND SessionDate &lt; @End" covers whole days.
    /// </summary>
    public class SessionSearchRepository : DB
    {
        #region Data Members
        private string table = "SessionSearch";

        // One row per session: the learner, grade, selected programme, the selected result's
        // outcome and how many exports were logged (LEFT JOINs, so sessions without a programme
        // or result still appear).
        private string sqlSearch =
            "SELECT s.SessionID, s.SessionStatus, s.SessionDate," +
            " LTRIM(RTRIM(ISNULL(l.FirstName, '') + ' ' + ISNULL(l.LastName, ''))) AS LearnerName," +
            " g.GradeDescription AS Grade," +
            " ISNULL(p.ProgrammeName, '') AS ProgrammeName," +
            " ISNULL(r.OutcomeCode, '') AS OutcomeCode," +
            " (SELECT COUNT(*) FROM ExportReport e WHERE e.SessionID = s.SessionID) AS ExportCount" +
            " FROM AdvisorySession s" +
            " INNER JOIN Learner l ON l.LearnerID = s.LearnerID" +
            " INNER JOIN GradeLevel g ON g.GradeLevelID = s.GradeLevelID" +
            " LEFT JOIN DegreeProgramme p ON p.ProgrammeID = s.SelectedProgrammeID" +
            " LEFT JOIN RecommendationResult r ON r.SessionID = s.SessionID AND r.IsSelectedProgramme = 1";
        #endregion

        #region Constructor
        /// <summary>Creates the repository (no database work until a method is called).</summary>
        public SessionSearchRepository() : base()
        {
        }
        #endregion

        #region Session search (UI 10.2b)
        /// <summary>
        /// Sessions matching every filter given, newest first. A sessionID, year or learnerID of 0,
        /// a blank surname or outcome code, and a null date mean "no filter". sessionID and
        /// sessionYear go together (a session number yyyy-NNNN). surname matches the start of
        /// Learner.LastName. Columns: SessionID, SessionStatus, SessionDate, LearnerName, Grade,
        /// ProgrammeName, OutcomeCode ("" when there is no result yet) and IsExported.
        /// Returns a new table every call, so the caller may add columns to it.
        /// </summary>
        public DataTable FindSessions(int sessionID, int sessionYear, string surname, DateTime? fromDate, DateTime? beforeDate,
                                      string outcomeCode, int learnerID)
        {
            // Only the filters that were given are added to the WHERE clause (each as a parameter)
            string sql = sqlSearch + " WHERE 1 = 1";
            List<SqlParameter> parameters = new List<SqlParameter>();
            if (sessionID > 0)
            {
                sql += " AND s.SessionID = @SessionID AND YEAR(s.SessionDate) = @SessionYear";
                parameters.Add(Param("@SessionID", SqlDbType.Int, sessionID));
                parameters.Add(Param("@SessionYear", SqlDbType.Int, sessionYear));
            }
            if (!string.IsNullOrWhiteSpace(surname))
            {
                sql += " AND l.LastName LIKE @Surname";
                parameters.Add(Param("@Surname", SqlDbType.VarChar, surname.Trim() + "%"));
            }
            if (fromDate.HasValue)
            {
                sql += " AND s.SessionDate >= @FromDate";
                parameters.Add(Param("@FromDate", SqlDbType.DateTime, fromDate.Value));
            }
            if (beforeDate.HasValue)
            {
                sql += " AND s.SessionDate < @BeforeDate";
                parameters.Add(Param("@BeforeDate", SqlDbType.DateTime, beforeDate.Value));
            }
            if (!string.IsNullOrWhiteSpace(outcomeCode))
            {
                sql += " AND r.OutcomeCode = @OutcomeCode";
                parameters.Add(Param("@OutcomeCode", SqlDbType.VarChar, outcomeCode.Trim()));
            }
            if (learnerID > 0)
            {
                sql += " AND s.LearnerID = @LearnerID";
                parameters.Add(Param("@LearnerID", SqlDbType.Int, learnerID));
            }
            FillDataSet(sql + " ORDER BY s.SessionDate DESC, s.SessionID DESC", table, parameters.ToArray());

            // "Exported" (UI 10.2b) means at least one ExportReport row exists for the session
            DataTable sessions = GetTable(table).Copy();
            sessions.Columns.Add("IsExported", typeof(bool));
            foreach (DataRow row in sessions.Rows)
            {
                row["IsExported"] = Convert.ToInt32(row["ExportCount"]) > 0;
            }
            sessions.Columns.Remove("ExportCount");
            return sessions;
        }

        /// <summary>The total number of advisory sessions stored.</summary>
        public int CountSessions()
        {
            return Convert.ToInt32(ExecuteScalar("SELECT COUNT(*) FROM AdvisorySession"));
        }

        /// <summary>True if a session with this ID exists and was started in this year (session number yyyy-NNNN).</summary>
        public bool SessionExists(int sessionID, int sessionYear)
        {
            object count = ExecuteScalar("SELECT COUNT(*) FROM AdvisorySession WHERE SessionID = @SessionID AND YEAR(SessionDate) = @SessionYear",
                                         Param("@SessionID", SqlDbType.Int, sessionID),
                                         Param("@SessionYear", SqlDbType.Int, sessionYear));
            return Convert.ToInt32(count) > 0;
        }
        #endregion

        #region Daily counts (UI 9 "Today" card)
        /// <summary>Sessions completed (DateCompleted) from dayStart up to, but not including, dayEnd.</summary>
        public int CountCompleted(DateTime dayStart, DateTime dayEnd)
        {
            object count = ExecuteScalar("SELECT COUNT(*) FROM AdvisorySession WHERE SessionStatus = @Completed" +
                                         " AND DateCompleted >= @DayStart AND DateCompleted < @DayEnd",
                                         Param("@Completed", SqlDbType.VarChar, AdvisorySession.StatusCompleted),
                                         Param("@DayStart", SqlDbType.DateTime, dayStart),
                                         Param("@DayEnd", SqlDbType.DateTime, dayEnd));
            return Convert.ToInt32(count);
        }

        /// <summary>Sessions started (SessionDate) in the day that are still In progress.</summary>
        public int CountStartedInProgress(DateTime dayStart, DateTime dayEnd)
        {
            object count = ExecuteScalar("SELECT COUNT(*) FROM AdvisorySession WHERE SessionStatus = @InProgress" +
                                         " AND SessionDate >= @DayStart AND SessionDate < @DayEnd",
                                         Param("@InProgress", SqlDbType.VarChar, AdvisorySession.StatusInProgress),
                                         Param("@DayStart", SqlDbType.DateTime, dayStart),
                                         Param("@DayEnd", SqlDbType.DateTime, dayEnd));
            return Convert.ToInt32(count);
        }

        /// <summary>
        /// Sessions completed in the day whose stored selected outcome is Not Currently Eligible
        /// or More Information Needed (the follow-up list).
        /// </summary>
        public int CountExceptionsCompleted(DateTime dayStart, DateTime dayEnd)
        {
            object count = ExecuteScalar("SELECT COUNT(*) FROM AdvisorySession s" +
                                         " INNER JOIN RecommendationResult r ON r.SessionID = s.SessionID AND r.IsSelectedProgramme = 1" +
                                         " WHERE s.SessionStatus = @Completed AND s.DateCompleted >= @DayStart AND s.DateCompleted < @DayEnd" +
                                         " AND r.OutcomeCode IN (@NotEligible, @MoreInfo)",
                                         Param("@Completed", SqlDbType.VarChar, AdvisorySession.StatusCompleted),
                                         Param("@DayStart", SqlDbType.DateTime, dayStart),
                                         Param("@DayEnd", SqlDbType.DateTime, dayEnd),
                                         Param("@NotEligible", SqlDbType.VarChar, AdvisoryOutcome.CodeNotEligible),
                                         Param("@MoreInfo", SqlDbType.VarChar, AdvisoryOutcome.CodeMoreInformation));
            return Convert.ToInt32(count);
        }
        #endregion
    }
}
