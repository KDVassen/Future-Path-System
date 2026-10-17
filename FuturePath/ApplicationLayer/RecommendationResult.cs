
namespace FuturePath.ApplicationLayer
{
    // Stores the results of checking a learner against a degree programme
    // Includes the match scores, advisory outcome and alternative ranking
    public class RecommendationResult
    {
        #region Data Members
        private int resultID;
        private int sessionID;
        private int programmeID;
        private string programmeName;
        private string facultyName;
        private decimal? subjectMatchScore;
        private decimal? markMatchScore;
        private decimal? interestMatchScore;
        private bool facultyBonus;
        private decimal? overallScore;
        private string outcomeCode;
        private string outcomeReason;
        private int? alternativeRank;
        private bool isSelectedProgramme;
        #endregion

        #region Properties
        public int ResultID
        {
            get { return resultID; }
            set { resultID = value; }
        }

        public int SessionID
        {
            get { return sessionID; }
            set { sessionID = value; }
        }

        public int ProgrammeID
        {
            get { return programmeID; }
            set { programmeID = value; }
        }

        public string ProgrammeName
        {
            get { return programmeName; }
            set { programmeName = value; }
        }

        public string FacultyName
        {
            get { return facultyName; }
            set { facultyName = value; }
        }

        public decimal? SubjectMatchScore
        {
            get { return subjectMatchScore; }
            set { subjectMatchScore = value; }
        }

        public decimal? MarkMatchScore
        {
            get { return markMatchScore; }
            set { markMatchScore = value; }
        }

        public decimal? InterestMatchScore
        {
            get { return interestMatchScore; }
            set { interestMatchScore = value; }
        }

        public bool FacultyBonus
        {
            get { return facultyBonus; }
            set { facultyBonus = value; }
        }

        public decimal? OverallScore
        {
            get { return overallScore; }
            set { overallScore = value; }
        }

        public string OutcomeCode
        {
            get { return outcomeCode; }
            set { outcomeCode = value; }
        }

        public string OutcomeReason
        {
            get { return outcomeReason; }
            set { outcomeReason = value; }
        }

        public int? AlternativeRank
        {
            get { return alternativeRank; }
            set { alternativeRank = value; }
        }

        public bool IsSelectedProgramme
        {
            get { return isSelectedProgramme; }
            set { isSelectedProgramme = value; }
        }

        // Convert the outcome code into a readable category
        // Return an empty string if the code is invalid
        public string OutcomeCategory
        {
            get
            {
                if (AdvisoryOutcome.IsValidCode(outcomeCode))
                {
                    return AdvisoryOutcome.GetCategory(outcomeCode);
                }
                return "";
            }
        }
        #endregion

        #region Constructors
        // Create a new recommendation result with default values
        public RecommendationResult()
        {
            resultID = 0;
            sessionID = 0;
            programmeID = 0;
            programmeName = "";
            facultyName = "";
            subjectMatchScore = null;
            markMatchScore = null;
            interestMatchScore = null;
            facultyBonus = false;
            overallScore = null;
            outcomeCode = AdvisoryOutcome.CodeMoreInformation;
            outcomeReason = "";
            alternativeRank = null;
            isSelectedProgramme = false;
        }

        // Create a recommendation result using the programme and calculated scores
        // Store the advisory outcome and whether this is the selected programme
        public RecommendationResult(int pSessionID, DegreeProgramme programme, MatchResult matchResult, decimal? interestScore,
                                    bool pFacultyBonus, AdvisoryOutcome outcome, bool pIsSelectedProgramme)
            : this()
        {
            sessionID = pSessionID;
            if (programme != null)
            {
                programmeID = programme.ProgrammeID;
                programmeName = programme.ProgrammeName;
                facultyName = programme.FacultyName;
            }
            if (matchResult != null)
            {
                subjectMatchScore = matchResult.SubjectScore;
                markMatchScore = matchResult.MarkScore;
            }
            interestMatchScore = interestScore;
            facultyBonus = pFacultyBonus;
            overallScore = outcome.OverallScore;
            outcomeCode = outcome.Code;
            outcomeReason = outcome.Explanation;
            isSelectedProgramme = pIsSelectedProgramme;
        }
        #endregion

        #region Methods
        // Convert the stored result into an AdvisoryOutcome object
        public AdvisoryOutcome ToAdvisoryOutcome()
        {
            return new AdvisoryOutcome(AdvisoryOutcome.FromCode(outcomeCode), outcomeReason, overallScore);
        }

        // Return the programme name and its advisory outcome
        public override string ToString()
        {
            return programmeName + ": " + OutcomeCategory;
        }
        #endregion
    }
}
