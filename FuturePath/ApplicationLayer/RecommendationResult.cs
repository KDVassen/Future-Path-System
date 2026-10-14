namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// The stored result of checking one programme in a session. The selected programme's row
    /// has IsSelectedProgramme = true; each alternative has its own row with AlternativeRank.
    /// Scores are stored on purpose, because weights and requirements can change later
    /// (database.md). Maps to RecommendationResult. Class diagram names: category = OutcomeCode,
    /// explanation = OutcomeReason, rank = AlternativeRank.
    /// </summary>
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
        /// <summary>Primary key (RecommendationResult.ResultID).</summary>
        public int ResultID
        {
            get { return resultID; }
            set { resultID = value; }
        }

        /// <summary>The session that produced this result.</summary>
        public int SessionID
        {
            get { return sessionID; }
            set { sessionID = value; }
        }

        /// <summary>The programme checked.</summary>
        public int ProgrammeID
        {
            get { return programmeID; }
            set { programmeID = value; }
        }

        /// <summary>The programme name (for display).</summary>
        public string ProgrammeName
        {
            get { return programmeName; }
            set { programmeName = value; }
        }

        /// <summary>The programme's faculty name (for display).</summary>
        public string FacultyName
        {
            get { return facultyName; }
            set { facultyName = value; }
        }

        /// <summary>MatchResult.SubjectScore when the result was made (class diagram subjectScore).</summary>
        public decimal? SubjectMatchScore
        {
            get { return subjectMatchScore; }
            set { subjectMatchScore = value; }
        }

        /// <summary>MatchResult.MarkScore when the result was made (class diagram markScore).</summary>
        public decimal? MarkMatchScore
        {
            get { return markMatchScore; }
            set { markMatchScore = value; }
        }

        /// <summary>The learner's interest score for the programme (class diagram interestScore).</summary>
        public decimal? InterestMatchScore
        {
            get { return interestMatchScore; }
            set { interestMatchScore = value; }
        }

        /// <summary>True if the programme is in the learner's preferred faculty.</summary>
        public bool FacultyBonus
        {
            get { return facultyBonus; }
            set { facultyBonus = value; }
        }

        /// <summary>The weighted overall score, 0 to 100, or null for More Information Needed.</summary>
        public decimal? OverallScore
        {
            get { return overallScore; }
            set { overallScore = value; }
        }

        /// <summary>STRONG, POSSIBLE, NCE or MOREINFO (class diagram "category").</summary>
        public string OutcomeCode
        {
            get { return outcomeCode; }
            set { outcomeCode = value; }
        }

        /// <summary>Why the outcome was given (class diagram "explanation").</summary>
        public string OutcomeReason
        {
            get { return outcomeReason; }
            set { outcomeReason = value; }
        }

        /// <summary>The alternative's rank (1 = best), or null for the selected programme (class diagram "rank").</summary>
        public int? AlternativeRank
        {
            get { return alternativeRank; }
            set { alternativeRank = value; }
        }

        /// <summary>True for the programme the learner selected on UI 5.</summary>
        public bool IsSelectedProgramme
        {
            get { return isSelectedProgramme; }
            set { isSelectedProgramme = value; }
        }

        /// <summary>The outcome in words, e.g. "Possible Match".</summary>
        public string OutcomeCategory
        {
            get { return AdvisoryOutcome.IsValidCode(outcomeCode) ? AdvisoryOutcome.GetCategory(outcomeCode) : ""; }
        }
        #endregion

        #region Constructors
        /// <summary>Creates an empty result.</summary>
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

        /// <summary>
        /// Creates the result of checking a programme from the scores and the engine's outcome.
        /// matchResult may be null when the check stopped at More Information Needed.
        /// </summary>
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
        /// <summary>Returns the outcome as an AdvisoryOutcome (category, explanation, overall score).</summary>
        public AdvisoryOutcome ToAdvisoryOutcome()
        {
            return new AdvisoryOutcome(AdvisoryOutcome.FromCode(outcomeCode), outcomeReason, overallScore);
        }

        /// <summary>Returns "Programme: Outcome".</summary>
        public override string ToString()
        {
            return programmeName + ": " + OutcomeCategory;
        }
        #endregion
    }
}
