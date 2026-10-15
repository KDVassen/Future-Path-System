using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// One advisory session: a learner's visit from UI 2 to the exported summary on UI 7.
    /// A session is saved as it goes with SessionStatus "In progress", and "Save and export"
    /// on UI 7 marks it "Completed" (decisions.md D3). Reports count completed sessions only.
    /// Maps to AdvisorySession. Its outcome (class diagram "outcome") is the OutcomeCode of the
    /// selected programme's RecommendationResult.
    /// </summary>
    public class AdvisorySession
    {
        #region Data Members
        /// <summary>Status while the learner is still working through the steps.</summary>
        public const string StatusInProgress = "In progress";
        /// <summary>Status once the summary has been exported on UI 7.</summary>
        public const string StatusCompleted = "Completed";
        /// <summary>UI 2 "Captured by" value: learners always capture their own session.</summary>
        public const string CapturedBySelfService = "Self-service";

        private int sessionID;
        private int learnerID;
        private GradeLevel grade;
        private int? preferredFacultyID;
        private int? selectedProgrammeID;
        private DateTime sessionDate;
        private short advisingYear;
        private bool fundingInfoRequested;
        private bool alternativesInSummary;
        private string sessionStatus;
        private DateTime? dateCompleted;
        private string outcome;
        private Collection<RecommendationResult> results;
        #endregion

        #region Properties
        /// <summary>Primary key (AdvisorySession.SessionID).</summary>
        public int SessionID
        {
            get { return sessionID; }
            set { sessionID = value; }
        }

        /// <summary>The learner (FK to Learner).</summary>
        public int LearnerID
        {
            get { return learnerID; }
            set { learnerID = value; }
        }

        /// <summary>The learner's grade in this session.</summary>
        public GradeLevel Grade
        {
            get { return grade; }
            set { grade = value; }
        }

        /// <summary>The grade as its database key (AdvisorySession.GradeLevelID: 1 or 2).</summary>
        public int GradeLevelID
        {
            get { return (int)grade; }
            set { grade = (GradeLevel)value; }
        }

        /// <summary>The learner's preferred faculty (UI 5), or null.</summary>
        public int? PreferredFacultyID
        {
            get { return preferredFacultyID; }
            set { preferredFacultyID = value; }
        }

        /// <summary>The programme the learner checked (UI 5), or null.</summary>
        public int? SelectedProgrammeID
        {
            get { return selectedProgrammeID; }
            set { selectedProgrammeID = value; }
        }

        /// <summary>When the session was started.</summary>
        public DateTime SessionDate
        {
            get { return sessionDate; }
            set { sessionDate = value; }
        }

        /// <summary>The advising year chosen on UI 2.</summary>
        public short AdvisingYear
        {
            get { return advisingYear; }
            set { advisingYear = value; }
        }

        /// <summary>True once "Add to Summary" was pressed on UI 6.2b (funding prints on FP-LSR).</summary>
        public bool FundingInfoRequested
        {
            get { return fundingInfoRequested; }
            set { fundingInfoRequested = value; }
        }

        /// <summary>True if the learner ticked "Include these alternatives in my summary" on UI 6.1a.</summary>
        public bool AlternativesInSummary
        {
            get { return alternativesInSummary; }
            set { alternativesInSummary = value; }
        }

        /// <summary>"In progress" or "Completed".</summary>
        public string SessionStatus
        {
            get { return sessionStatus; }
            set { sessionStatus = value; }
        }

        /// <summary>When the summary was exported (class diagram dateCompleted), or null.</summary>
        public DateTime? DateCompleted
        {
            get { return dateCompleted; }
            set { dateCompleted = value; }
        }

        /// <summary>The selected programme's outcome code, or "" before the eligibility check.</summary>
        public string Outcome
        {
            get { return outcome; }
            set { outcome = value; }
        }

        /// <summary>The session number shown to people: yyyy-NNNN, built from SessionDate and SessionID (not stored).</summary>
        public string SessionNumber
        {
            get
            {
                if (sessionID == 0)
                {
                    return "";
                }
                return FormatSessionNumber(sessionDate, sessionID);
            }
        }

        /// <summary>True once the session has been completed on UI 7.</summary>
        public bool IsCompleted
        {
            get { return sessionStatus == StatusCompleted; }
        }

        /// <summary>The session's recommendation results (selected programme and alternatives), when loaded.</summary>
        public Collection<RecommendationResult> Results
        {
            get { return results; }
        }
        #endregion

        #region Constructors
        /// <summary>Creates a new In-progress session dated now.</summary>
        public AdvisorySession()
        {
            sessionID = 0;
            learnerID = 0;
            grade = GradeLevel.Grade12;
            preferredFacultyID = null;
            selectedProgrammeID = null;
            sessionDate = DateTime.Now;
            advisingYear = (short)DateTime.Today.Year;
            fundingInfoRequested = false;
            alternativesInSummary = false;
            sessionStatus = StatusInProgress;
            dateCompleted = null;
            outcome = "";
            results = new Collection<RecommendationResult>();
        }

        /// <summary>Creates a new In-progress session for a learner.</summary>
        public AdvisorySession(int pLearnerID, GradeLevel pGrade, short pAdvisingYear)
            : this()
        {
            learnerID = pLearnerID;
            grade = pGrade;
            advisingYear = pAdvisingYear;
        }
        #endregion

        #region Methods
        /// <summary>Class diagram recordOutcome(outcome): records the selected programme's outcome code.</summary>
        public void RecordOutcome(string outcomeCode)
        {
            outcome = outcomeCode;
        }

        /// <summary>Marks the session Completed at the given time (UI 7 Save and export).</summary>
        public void MarkCompleted(DateTime completedOn)
        {
            sessionStatus = StatusCompleted;
            dateCompleted = completedOn;
        }

        /// <summary>Builds a session number, e.g. 2026 and 7 gives "2026-0007".</summary>
        public static string FormatSessionNumber(DateTime sessionDate, int sessionID)
        {
            return sessionDate.Year.ToString("0000", CultureInfo.InvariantCulture) + "-" + sessionID.ToString("0000", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// True if staff typed a session number in the yyyy-NNNN format, e.g. "2026-0007"
        /// (four digits, a dash, four digits, and not session 0000).
        /// </summary>
        public static bool IsValidSessionNumber(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }
            string number = text.Trim();
            if (!Regex.IsMatch(number, @"^\d{4}-\d{4}$"))
            {
                return false;
            }
            return number.Substring(5) != "0000";
        }

        /// <summary>The year part of a session number ("2026-0007" gives 2026), or 0 if it is not valid.</summary>
        public static int GetSessionYear(string text)
        {
            if (!IsValidSessionNumber(text))
            {
                return 0;
            }
            return int.Parse(text.Trim().Substring(0, 4), CultureInfo.InvariantCulture);
        }

        /// <summary>The SessionID part of a session number ("2026-0007" gives 7), or 0 if it is not valid.</summary>
        public static int GetSessionID(string text)
        {
            if (!IsValidSessionNumber(text))
            {
                return 0;
            }
            return int.Parse(text.Trim().Substring(5), CultureInfo.InvariantCulture);
        }

        /// <summary>Returns the session number and status.</summary>
        public override string ToString()
        {
            return SessionNumber + " (" + sessionStatus + ")";
        }
        #endregion
    }
}
