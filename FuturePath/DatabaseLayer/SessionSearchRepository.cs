
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using FuturePath.ApplicationLayer;

namespace FuturePath.DatabaseLayer
{
    // Handles database queries for staff session searches and dashboard counts
    // Retrieves saved outcomes without recalculating recommendations
    public class SessionSearchRepository : DB
    {
        #region Data Members
        private string table = "SessionSearch";

        // Load sessions with learner details, selected programmes and export counts
        // LEFT JOINs keep sessions that do not yet have a programme or result
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
        // Initialise the session search repository
        public SessionSearchRepository() : base()
        {
        }
        #endregion

        #region Session search (UI 10.2b)
        // Search sessions using the provided filters, showing newest sessions first
        // Return session details and whether each session has been exported
        public DataTable FindSessions(int sessionID, int sessionYear, string surname, DateTime? fromDate, DateTime? beforeDate,
                                      string outcomeCode, int learnerID)
        {
            // Add SQL conditions only for the filters provided
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

            // A session is exported if an export record exists for it
            DataTable sessions = GetTable(table).Copy();
            sessions.Columns.Add("IsExported", typeof(bool));
            foreach (DataRow row in sessions.Rows)
            {
                row["IsExported"] = Convert.ToInt32(row["ExportCount"]) > 0;
            }
            sessions.Columns.Remove("ExportCount");
            return sessions;
        }

        // Count the total number of advisory sessions
        public int CountSessions()
        {
            return Convert.ToInt32(ExecuteScalar("SELECT COUNT(*) FROM AdvisorySession"));
        }

        // Check whether a session exists with the given ID and year
        public bool SessionExists(int sessionID, int sessionYear)
        {
            object count = ExecuteScalar("SELECT COUNT(*) FROM AdvisorySession WHERE SessionID = @SessionID AND YEAR(SessionDate) = @SessionYear",
                                         Param("@SessionID", SqlDbType.Int, sessionID),
                                         Param("@SessionYear", SqlDbType.Int, sessionYear));
            return Convert.ToInt32(count) > 0;
        }
        #endregion

        #region Daily counts (UI 9 "Today" card)
        // Count sessions completed within the selected day
        public int CountCompleted(DateTime dayStart, DateTime dayEnd)
        {
            object count = ExecuteScalar("SELECT COUNT(*) FROM AdvisorySession WHERE SessionStatus = @Completed" +
                                         " AND DateCompleted >= @DayStart AND DateCompleted < @DayEnd",
                                         Param("@Completed", SqlDbType.VarChar, AdvisorySession.StatusCompleted),
                                         Param("@DayStart", SqlDbType.DateTime, dayStart),
                                         Param("@DayEnd", SqlDbType.DateTime, dayEnd));
            return Convert.ToInt32(count);
        }

        // Count sessions started today that are still in progress
        public int CountStartedInProgress(DateTime dayStart, DateTime dayEnd)
        {
            object count = ExecuteScalar("SELECT COUNT(*) FROM AdvisorySession WHERE SessionStatus = @InProgress" +
                                         " AND SessionDate >= @DayStart AND SessionDate < @DayEnd",
                                         Param("@InProgress", SqlDbType.VarChar, AdvisorySession.StatusInProgress),
                                         Param("@DayStart", SqlDbType.DateTime, dayStart),
                                         Param("@DayEnd", SqlDbType.DateTime, dayEnd));
            return Convert.ToInt32(count);
        }

        // Count completed sessions with eligibility exceptions for the selected day
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
