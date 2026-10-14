using System;
using System.Data;
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

        // One row per session: the learner, grade, selected programme and the selected result's
        // outcome (LEFT JOINs, so sessions without a programme or result still appear).
        private string sqlSearch =
            "SELECT s.SessionID, s.SessionStatus, s.SessionDate," +
            " LTRIM(RTRIM(ISNULL(l.FirstName, '') + ' ' + ISNULL(l.LastName, ''))) AS LearnerName," +
            " g.GradeDescription AS Grade," +
            " ISNULL(p.ProgrammeName, '') AS ProgrammeName," +
            " ISNULL(r.OutcomeCode, '') AS OutcomeCode," +
            " CAST(CASE WHEN EXISTS (SELECT 1 FROM ExportReport e WHERE e.SessionID = s.SessionID) THEN 1 ELSE 0 END AS BIT) AS IsExported" +
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
        /// Sessions matching every filter given (null = no filter), newest first. Columns:
        /// SessionID, SessionStatus, SessionDate, LearnerName, Grade, ProgrammeName, OutcomeCode
        /// ("" when there is no result yet) and IsExported. sessionID and sessionYear go together
        /// (a session number yyyy-NNNN). surname matches the start of Learner.LastName.
        /// Returns a new table every call, so the caller may add columns to it.
        /// </summary>
        public DataTable FindSessions(int? sessionID, int? sessionYear, string surname, DateTime? fromDate, DateTime? beforeDate,
                                      string outcomeCode, int? learnerID)
        {
            string sql = sqlSearch +
                " WHERE (@SessionID IS NULL OR (s.SessionID = @SessionID AND YEAR(s.SessionDate) = @SessionYear))" +
                " AND (@Surname IS NULL OR l.LastName LIKE @Surname)" +
                " AND (@FromDate IS NULL OR s.SessionDate >= @FromDate)" +
                " AND (@BeforeDate IS NULL OR s.SessionDate < @BeforeDate)" +
                " AND (@OutcomeCode IS NULL OR r.OutcomeCode = @OutcomeCode)" +
                " AND (@LearnerID IS NULL OR s.LearnerID = @LearnerID)" +
                " ORDER BY s.SessionDate DESC, s.SessionID DESC";
            FillDataSet(sql, table,
                        Param("@SessionID", SqlDbType.Int, sessionID),
                        Param("@SessionYear", SqlDbType.Int, sessionYear),
                        Param("@Surname", SqlDbType.VarChar, StartsWithPattern(surname)),
                        Param("@FromDate", SqlDbType.DateTime, fromDate),
                        Param("@BeforeDate", SqlDbType.DateTime, beforeDate),
                        Param("@OutcomeCode", SqlDbType.VarChar, outcomeCode),
                        Param("@LearnerID", SqlDbType.Int, learnerID));
            return GetTable(table).Copy();
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

        #region Utility Methods
        /// <summary>
        /// Turns typed text into a LIKE pattern for "starts with": the LIKE wildcards [ % _ are
        /// escaped so they are matched literally, then % is added. Blank text gives null (no filter).
        /// </summary>
        private static string StartsWithPattern(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }
            string escaped = text.Trim().Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]");
            return escaped + "%";
        }
        #endregion
    }
}
