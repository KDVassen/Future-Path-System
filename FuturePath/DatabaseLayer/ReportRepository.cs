
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using FuturePath.ApplicationLayer;

namespace FuturePath.DatabaseLayer
{
    // Retrieves the database records needed for the system's reports
    // Report calculations and summaries are handled by ReportController
    public class ReportRepository : DB
    {
        #region Data Members
        private string scopeTable = "ReportScope";
        private string degreeInterestTable = "DegreeInterestSession";
        private string exceptionTable = "ExceptionSession";
        private string gradeTable = "GradeLevel";

        // Join completed sessions with their selected programme and faculty
        private string sqlPeriodFrom =
            " FROM AdvisorySession s" +
            " INNER JOIN RecommendationResult r ON r.SessionID = s.SessionID AND r.IsSelectedProgramme = 1" +
            " INNER JOIN DegreeProgramme p ON p.ProgrammeID = r.ProgrammeID" +
            " INNER JOIN Faculty f ON f.FacultyID = p.FacultyID";
        #endregion

        #region Constructor
        // Initialise the report repository
        public ReportRepository() : base()
        {
        }
        #endregion

        #region Period queries (FP-DIE, FP-LER, UI 10.3c and UI 11)
        // Count completed sessions, faculties and programmes within the selected period
        // Apply any faculty, grade, outcome or activity filters
        public DataTable GetScopeCounts(DateTime periodStart, DateTime periodEnd, int facultyID, int gradeLevelID,
                                        string outcomeCode, bool includeInactive, bool exceptionsOnly)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            string sql = "SELECT COUNT(*) AS SessionCount, COUNT(DISTINCT p.FacultyID) AS FacultyCount," +
                         " COUNT(DISTINCT p.ProgrammeID) AS ProgrammeCount" + sqlPeriodFrom +
                         BuildPeriodWhere(parameters, periodStart, periodEnd, facultyID, gradeLevelID, outcomeCode, includeInactive, exceptionsOnly);
            FillDataSet(sql, scopeTable, parameters.ToArray());
            return GetTable(scopeTable).Copy();
        }

        // Load completed sessions used for the degree interest report
        // Include the selected programme, faculty and eligibility outcome
        public DataTable GetCompletedSessions(DateTime periodStart, DateTime periodEnd, int facultyID, int gradeLevelID,
                                              string outcomeCode, bool includeInactive)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            string sql = "SELECT s.SessionID, s.LearnerID, s.SessionDate, s.FundingInfoRequested," +
                         " p.ProgrammeID, p.ProgrammeName, p.FacultyID, f.FacultyName, r.OutcomeCode" +
                         sqlPeriodFrom +
                         BuildPeriodWhere(parameters, periodStart, periodEnd, facultyID, gradeLevelID, outcomeCode, includeInactive, false) +
                         " ORDER BY s.SessionDate, s.SessionID";
            FillDataSet(sql, degreeInterestTable, parameters.ToArray());
            return GetTable(degreeInterestTable).Copy();
        }

        // Load completed sessions with eligibility exceptions
        // Include learner, programme and outcome details
        public DataTable GetExceptionSessions(DateTime periodStart, DateTime periodEnd, int facultyID, int gradeLevelID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            string sql = "SELECT s.SessionID, s.LearnerID, s.SessionDate, s.GradeLevelID, l.FirstName, l.LastName, l.SchoolName," +
                         " p.ProgrammeID, p.ProgrammeName, p.FacultyID, f.FacultyName, r.OutcomeCode, r.OutcomeReason" +
                         sqlPeriodFrom +
                         " INNER JOIN Learner l ON l.LearnerID = s.LearnerID" +
                         BuildPeriodWhere(parameters, periodStart, periodEnd, facultyID, gradeLevelID, "", false, true) +
                         // Show Not Currently Eligible before More Information Needed
                         " ORDER BY f.FacultyName, r.OutcomeCode DESC, s.SessionDate DESC, s.SessionID DESC";
            FillDataSet(sql, exceptionTable, parameters.ToArray());
            return GetTable(exceptionTable).Copy();
        }
        #endregion

        #region Single values
        // Get a faculty's name or return an empty string if it does not exist
        public string GetFacultyName(int facultyID)
        {
            object name = ExecuteScalar("SELECT FacultyName FROM Faculty WHERE FacultyID = @FacultyID",
                                        Param("@FacultyID", SqlDbType.Int, facultyID));
            if (name == null)
            {
                return "";
            }
            return Convert.ToString(name).TrimEnd();
        }

        // Retrieve the grade level details for a given ID
        public DataTable GetGradeLevel(int gradeLevelID)
        {
            FillDataSet("SELECT GradeLevelID, GradeDescription, MarkBasis FROM GradeLevel WHERE GradeLevelID = @GradeLevelID", gradeTable,
                        Param("@GradeLevelID", SqlDbType.Int, gradeLevelID));
            return GetTable(gradeTable).Copy();
        }

        // Check whether a programme and its faculty are both active
        public bool IsProgrammeCurrent(int programmeID)
        {
            object count = ExecuteScalar("SELECT COUNT(*) FROM DegreeProgramme p INNER JOIN Faculty f ON f.FacultyID = p.FacultyID" +
                                         " WHERE p.ProgrammeID = @ProgrammeID AND p.IsActive = 1 AND f.IsActive = 1",
                                         Param("@ProgrammeID", SqlDbType.Int, programmeID));
            return Convert.ToInt32(count) > 0;
        }

        // Get an advisory session's current status
        public string GetSessionStatus(int sessionID)
        {
            object status = ExecuteScalar("SELECT SessionStatus FROM AdvisorySession WHERE SessionID = @SessionID",
                                          Param("@SessionID", SqlDbType.Int, sessionID));
            if (status == null)
            {
                return "";
            }
            return Convert.ToString(status).TrimEnd();
        }
        #endregion

        #region Utility Methods
        // Build the SQL filters shared by the report period queries
        // Include only completed sessions and use parameters for all filter values
        private static string BuildPeriodWhere(List<SqlParameter> parameters, DateTime periodStart, DateTime periodEnd, int facultyID,
                                               int gradeLevelID, string outcomeCode, bool includeInactive, bool exceptionsOnly)
        {
            string where = " WHERE s.SessionStatus = @Completed AND s.SessionDate >= @PeriodStart AND s.SessionDate < @PeriodEnd";
            parameters.Add(Param("@Completed", SqlDbType.VarChar, AdvisorySession.StatusCompleted));
            parameters.Add(Param("@PeriodStart", SqlDbType.DateTime, periodStart));
            parameters.Add(Param("@PeriodEnd", SqlDbType.DateTime, periodEnd));

            if (facultyID > 0)
            {
                where += " AND p.FacultyID = @FacultyID";
                parameters.Add(Param("@FacultyID", SqlDbType.Int, facultyID));
            }
            if (gradeLevelID > 0)
            {
                where += " AND s.GradeLevelID = @GradeLevelID";
                parameters.Add(Param("@GradeLevelID", SqlDbType.Int, gradeLevelID));
            }
            if (!string.IsNullOrEmpty(outcomeCode))
            {
                where += " AND r.OutcomeCode = @OutcomeCode";
                parameters.Add(Param("@OutcomeCode", SqlDbType.VarChar, outcomeCode));
            }
            if (exceptionsOnly)
            {
                // Include only Not Currently Eligible and More Information Needed outcomes
                where += " AND r.OutcomeCode IN (@NotEligible, @MoreInfo)";
                parameters.Add(Param("@NotEligible", SqlDbType.VarChar, AdvisoryOutcome.CodeNotEligible));
                parameters.Add(Param("@MoreInfo", SqlDbType.VarChar, AdvisoryOutcome.CodeMoreInformation));
            }
            if (!includeInactive)
            {
                where += " AND p.IsActive = 1 AND f.IsActive = 1";
            }
            return where;
        }
        #endregion
    }
}
