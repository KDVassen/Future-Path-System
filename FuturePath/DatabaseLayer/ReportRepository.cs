using System;
using System.Data;
using System.Data.SqlClient;
using FuturePath.ApplicationLayer;

namespace FuturePath.DatabaseLayer
{
    /// <summary>
    /// Raw rows for the three reports (FP-LSR, FP-DIE, FP-LER). This class only reads; every
    /// count, rate, shortfall and chart note is worked out by ReportController, and subject gaps
    /// come from DegreeProgramme.EvaluateSubjectAndMarkMatch, never from SQL.
    /// Only ReportController uses this class; its methods are defined with it.
    ///
    /// The period queries all share one FROM / WHERE: COMPLETED sessions joined to their
    /// SELECTED RecommendationResult (IsSelectedProgramme = 1), that result's programme and the
    /// programme's faculty. The controller passes the period as a start and an exclusive end.
    /// Every filter is optional (null = no filter).
    /// </summary>
    public class ReportRepository : DB
    {
        #region Data Members
        private string scopeTable = "ReportScope";
        private string degreeInterestTable = "DegreeInterestSession";
        private string exceptionTable = "ExceptionSession";
        private string gradeTable = "GradeLevel";

        // Completed sessions -> selected result -> programme -> faculty.
        private string sqlPeriodFrom =
            " FROM AdvisorySession s" +
            " INNER JOIN RecommendationResult r ON r.SessionID = s.SessionID AND r.IsSelectedProgramme = 1" +
            " INNER JOIN DegreeProgramme p ON p.ProgrammeID = r.ProgrammeID" +
            " INNER JOIN Faculty f ON f.FacultyID = p.FacultyID";

        // The period and the optional filters. @ExceptionsOnly keeps only Not Currently Eligible
        // and More Information Needed; without @IncludeInactive, inactive programmes and
        // programmes under an inactive faculty are left out.
        private string sqlPeriodWhere =
            " WHERE s.SessionStatus = @Completed" +
            " AND s.SessionDate >= @PeriodStart AND s.SessionDate < @PeriodEnd" +
            " AND (@FacultyID IS NULL OR p.FacultyID = @FacultyID)" +
            " AND (@GradeLevelID IS NULL OR s.GradeLevelID = @GradeLevelID)" +
            " AND (@OutcomeCode IS NULL OR r.OutcomeCode = @OutcomeCode)" +
            " AND (@ExceptionsOnly = 0 OR r.OutcomeCode IN (@NotEligible, @MoreInfo))" +
            " AND (@IncludeInactive = 1 OR (p.IsActive = 1 AND f.IsActive = 1))";
        #endregion

        #region Constructor
        /// <summary>Creates the repository (no database work until a method is called).</summary>
        public ReportRepository() : base()
        {
        }
        #endregion

        #region Period queries (FP-DIE, FP-LER, UI 10.3c and UI 11)
        /// <summary>
        /// One row with SessionCount, FacultyCount and ProgrammeCount for the completed sessions
        /// in the period after the filters (UI 11 readiness panel, UI 10.3c rows in scope).
        /// </summary>
        public DataTable GetScopeCounts(DateTime periodStart, DateTime periodEnd, int? facultyID, int? gradeLevelID,
                                        string outcomeCode, bool includeInactive, bool exceptionsOnly)
        {
            string sql = "SELECT COUNT(*) AS SessionCount, COUNT(DISTINCT p.FacultyID) AS FacultyCount," +
                         " COUNT(DISTINCT p.ProgrammeID) AS ProgrammeCount" + sqlPeriodFrom + sqlPeriodWhere;
            FillDataSet(sql, scopeTable, PeriodParameters(periodStart, periodEnd, facultyID, gradeLevelID, outcomeCode,
                                                          includeInactive, exceptionsOnly));
            return GetTable(scopeTable).Copy();
        }

        /// <summary>
        /// FP-DIE: one row per completed session in the period after the filters. Columns:
        /// SessionID, LearnerID, SessionDate, FundingInfoRequested, ProgrammeID, ProgrammeName,
        /// FacultyID, FacultyName, OutcomeCode. No learner names are read.
        /// </summary>
        public DataTable GetCompletedSessions(DateTime periodStart, DateTime periodEnd, int? facultyID, int? gradeLevelID,
                                              string outcomeCode, bool includeInactive)
        {
            string sql = "SELECT s.SessionID, s.LearnerID, s.SessionDate, s.FundingInfoRequested," +
                         " p.ProgrammeID, p.ProgrammeName, p.FacultyID, f.FacultyName, r.OutcomeCode" +
                         sqlPeriodFrom + sqlPeriodWhere +
                         " ORDER BY s.SessionDate, s.SessionID";
            FillDataSet(sql, degreeInterestTable, PeriodParameters(periodStart, periodEnd, facultyID, gradeLevelID, outcomeCode,
                                                                   includeInactive, false));
            return GetTable(degreeInterestTable).Copy();
        }

        /// <summary>
        /// FP-LER: completed sessions in the period whose stored outcome is Not Currently Eligible
        /// or More Information Needed, active programmes under active faculties only. Columns:
        /// SessionID, LearnerID, SessionDate, GradeLevelID, FirstName, LastName, SchoolName,
        /// ProgrammeID, ProgrammeName, FacultyID, FacultyName, OutcomeCode, OutcomeReason and
        /// AlternativeProgramme (the best-ranked stored alternative that is still active, or NULL).
        /// Ordered by faculty name, then Not Currently Eligible before More Information Needed,
        /// then session date, newest first.
        /// </summary>
        public DataTable GetExceptionSessions(DateTime periodStart, DateTime periodEnd, int? facultyID, int? gradeLevelID)
        {
            string sql = "SELECT s.SessionID, s.LearnerID, s.SessionDate, s.GradeLevelID, l.FirstName, l.LastName, l.SchoolName," +
                         " p.ProgrammeID, p.ProgrammeName, p.FacultyID, f.FacultyName, r.OutcomeCode, r.OutcomeReason," +
                         " (SELECT TOP 1 ap.ProgrammeName FROM RecommendationResult a" +
                         "   INNER JOIN DegreeProgramme ap ON ap.ProgrammeID = a.ProgrammeID" +
                         "   INNER JOIN Faculty af ON af.FacultyID = ap.FacultyID" +
                         "   WHERE a.SessionID = s.SessionID AND a.IsSelectedProgramme = 0 AND a.AlternativeRank IS NOT NULL" +
                         "   AND ap.IsActive = 1 AND af.IsActive = 1" +
                         "   ORDER BY a.AlternativeRank) AS AlternativeProgramme" +
                         sqlPeriodFrom +
                         " INNER JOIN Learner l ON l.LearnerID = s.LearnerID" +
                         sqlPeriodWhere +
                         " ORDER BY f.FacultyName, CASE WHEN r.OutcomeCode = @NotEligible THEN 1 ELSE 2 END," +
                         " s.SessionDate DESC, s.SessionID DESC";
            FillDataSet(sql, exceptionTable, PeriodParameters(periodStart, periodEnd, facultyID, gradeLevelID, null, false, true));
            return GetTable(exceptionTable).Copy();
        }
        #endregion

        #region Single values
        /// <summary>The faculty's name, or "" if there is no such faculty.</summary>
        public string GetFacultyName(int facultyID)
        {
            object name = ExecuteScalar("SELECT FacultyName FROM Faculty WHERE FacultyID = @FacultyID",
                                        Param("@FacultyID", SqlDbType.Int, facultyID));
            return name == null ? "" : Convert.ToString(name).TrimEnd();
        }

        /// <summary>The grade with columns GradeLevelID, GradeDescription and MarkBasis (no rows if it does not exist).</summary>
        public DataTable GetGradeLevel(int gradeLevelID)
        {
            FillDataSet("SELECT GradeLevelID, GradeDescription, MarkBasis FROM GradeLevel WHERE GradeLevelID = @GradeLevelID", gradeTable,
                        Param("@GradeLevelID", SqlDbType.Int, gradeLevelID));
            return GetTable(gradeTable).Copy();
        }

        /// <summary>True if the programme is active and sits under an active faculty (it may appear in a report).</summary>
        public bool IsProgrammeCurrent(int programmeID)
        {
            object count = ExecuteScalar("SELECT COUNT(*) FROM DegreeProgramme p INNER JOIN Faculty f ON f.FacultyID = p.FacultyID" +
                                         " WHERE p.ProgrammeID = @ProgrammeID AND p.IsActive = 1 AND f.IsActive = 1",
                                         Param("@ProgrammeID", SqlDbType.Int, programmeID));
            return Convert.ToInt32(count) > 0;
        }

        /// <summary>The session's SessionStatus ("In progress" or "Completed"), or "" if there is no such session.</summary>
        public string GetSessionStatus(int sessionID)
        {
            object status = ExecuteScalar("SELECT SessionStatus FROM AdvisorySession WHERE SessionID = @SessionID",
                                          Param("@SessionID", SqlDbType.Int, sessionID));
            return status == null ? "" : Convert.ToString(status).TrimEnd();
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// A new set of parameters for the shared period WHERE clause (a parameter object can
        /// belong to only one command, so every query gets its own).
        /// </summary>
        private static SqlParameter[] PeriodParameters(DateTime periodStart, DateTime periodEnd, int? facultyID, int? gradeLevelID,
                                                       string outcomeCode, bool includeInactive, bool exceptionsOnly)
        {
            return new SqlParameter[]
            {
                Param("@Completed", SqlDbType.VarChar, AdvisorySession.StatusCompleted),
                Param("@PeriodStart", SqlDbType.DateTime, periodStart),
                Param("@PeriodEnd", SqlDbType.DateTime, periodEnd),
                Param("@FacultyID", SqlDbType.Int, facultyID),
                Param("@GradeLevelID", SqlDbType.Int, gradeLevelID),
                Param("@OutcomeCode", SqlDbType.VarChar, outcomeCode),
                Param("@ExceptionsOnly", SqlDbType.Bit, exceptionsOnly),
                Param("@NotEligible", SqlDbType.VarChar, AdvisoryOutcome.CodeNotEligible),
                Param("@MoreInfo", SqlDbType.VarChar, AdvisoryOutcome.CodeMoreInformation),
                Param("@IncludeInactive", SqlDbType.Bit, includeInactive)
            };
        }
        #endregion
    }
}
