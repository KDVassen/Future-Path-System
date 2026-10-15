using System;
using System.Collections.Generic;
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
    /// A faculty or grade of 0, or a blank outcome code, means "no filter".
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

        /// <summary>
        /// FP-DIE: one row per completed session in the period after the filters. Columns:
        /// SessionID, LearnerID, SessionDate, FundingInfoRequested, ProgrammeID, ProgrammeName,
        /// FacultyID, FacultyName, OutcomeCode. No learner names are read.
        /// </summary>
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

        /// <summary>
        /// FP-LER: completed sessions in the period whose stored outcome is Not Currently Eligible
        /// or More Information Needed, active programmes under active faculties only. Columns:
        /// SessionID, LearnerID, SessionDate, GradeLevelID, FirstName, LastName, SchoolName,
        /// ProgrammeID, ProgrammeName, FacultyID, FacultyName, OutcomeCode, OutcomeReason.
        /// Ordered by faculty name, then Not Currently Eligible before More Information Needed,
        /// then session date, newest first.
        /// </summary>
        public DataTable GetExceptionSessions(DateTime periodStart, DateTime periodEnd, int facultyID, int gradeLevelID)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            string sql = "SELECT s.SessionID, s.LearnerID, s.SessionDate, s.GradeLevelID, l.FirstName, l.LastName, l.SchoolName," +
                         " p.ProgrammeID, p.ProgrammeName, p.FacultyID, f.FacultyName, r.OutcomeCode, r.OutcomeReason" +
                         sqlPeriodFrom +
                         " INNER JOIN Learner l ON l.LearnerID = s.LearnerID" +
                         BuildPeriodWhere(parameters, periodStart, periodEnd, facultyID, gradeLevelID, "", false, true) +
                         // NCE sorts before MOREINFO when sorted Z to A (N comes after M)
                         " ORDER BY f.FacultyName, r.OutcomeCode DESC, s.SessionDate DESC, s.SessionID DESC";
            FillDataSet(sql, exceptionTable, parameters.ToArray());
            return GetTable(exceptionTable).Copy();
        }
        #endregion

        #region Single values
        /// <summary>The faculty's name, or "" if there is no such faculty.</summary>
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
            if (status == null)
            {
                return "";
            }
            return Convert.ToString(status).TrimEnd();
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// Builds the WHERE clause shared by the period queries: completed sessions in the period,
        /// plus only the filters that were given. Each value is added to parameters, never typed
        /// into the SQL text.
        /// </summary>
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
                // FP-LER: only Not Currently Eligible and More Information Needed
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
