
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FuturePath.ApplicationLayer
{
    // Stores the details and progress of a learner's advisory session
    // A session is completed once the advisory summary has been exported
    public class AdvisorySession
    {
        #region Data Members
        // Session status values
        public const string StatusInProgress = "In progress";
        public const string StatusCompleted = "Completed";
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
        public int SessionID
        {
            get { return sessionID; }
            set { sessionID = value; }
        }

        public int LearnerID
        {
            get { return learnerID; }
            set { learnerID = value; }
        }

        public GradeLevel Grade
        {
            get { return grade; }
            set { grade = value; }
        }

        // Convert the learner's grade to its database ID
        public int GradeLevelID
        {
            get { return (int)grade; }
            set { grade = (GradeLevel)value; }
        }

        public int? PreferredFacultyID
        {
            get { return preferredFacultyID; }
            set { preferredFacultyID = value; }
        }

        public int? SelectedProgrammeID
        {
            get { return selectedProgrammeID; }
            set { selectedProgrammeID = value; }
        }

        public DateTime SessionDate
        {
            get { return sessionDate; }
            set { sessionDate = value; }
        }

        public short AdvisingYear
        {
            get { return advisingYear; }
            set { advisingYear = value; }
        }

        public bool FundingInfoRequested
        {
            get { return fundingInfoRequested; }
            set { fundingInfoRequested = value; }
        }

        public bool AlternativesInSummary
        {
            get { return alternativesInSummary; }
            set { alternativesInSummary = value; }
        }

        public string SessionStatus
        {
            get { return sessionStatus; }
            set { sessionStatus = value; }
        }

        public DateTime? DateCompleted
        {
            get { return dateCompleted; }
            set { dateCompleted = value; }
        }

        public string Outcome
        {
            get { return outcome; }
            set { outcome = value; }
        }

        // Generate the session number using the year and session ID
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

        // Check if the advisory session has been completed
        public bool IsCompleted
        {
            get { return sessionStatus == StatusCompleted; }
        }

        // Return the recommendation results for the session
        public Collection<RecommendationResult> Results
        {
            get { return results; }
        }
        #endregion

        #region Constructors
        // Create a new advisory session with default values
        // Set the session status to In progress
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

        // Create an advisory session for a specific learner
        public AdvisorySession(int pLearnerID, GradeLevel pGrade, short pAdvisingYear)
            : this()
        {
            learnerID = pLearnerID;
            grade = pGrade;
            advisingYear = pAdvisingYear;
        }
        #endregion

        #region Methods
        // Store the outcome of the learner's selected programme
        public void RecordOutcome(string outcomeCode)
        {
            outcome = outcomeCode;
        }

        // Mark the session as completed and record the completion date
        public void MarkCompleted(DateTime completedOn)
        {
            sessionStatus = StatusCompleted;
            dateCompleted = completedOn;
        }

        // Format the session number as yyyy-NNNN
        // For example, session 7 in 2026 becomes 2026-0007
        public static string FormatSessionNumber(DateTime sessionDate, int sessionID)
        {
            return sessionDate.Year.ToString("0000", CultureInfo.InvariantCulture) + "-" + sessionID.ToString("0000", CultureInfo.InvariantCulture);
        }

        // Check that the session number follows the yyyy-NNNN format
        // The session ID cannot be 0000
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

        // Get the year from the session number
        // Return 0 if the session number is invalid
        public static int GetSessionYear(string text)
        {
            if (!IsValidSessionNumber(text))
            {
                return 0;
            }
            return int.Parse(text.Trim().Substring(0, 4), CultureInfo.InvariantCulture);
        }

        // Get the session ID from the session number
        // Return 0 if the session number is invalid
        public static int GetSessionID(string text)
        {
            if (!IsValidSessionNumber(text))
            {
                return 0;
            }
            return int.Parse(text.Trim().Substring(5), CultureInfo.InvariantCulture);
        }

        // Return the session number and its current status
        public override string ToString()
        {
            return SessionNumber + " (" + sessionStatus + ")";
        }
        #endregion
    }
}
