using System;
using System.Collections.ObjectModel;
using System.Data;
using FuturePath.DatabaseLayer;

namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// Sequence 1, "Check degree eligibility and generate an advisory outcome" (spec Section 3).
    /// UI 5's Check eligibility button calls CheckEligibility. The controller loads the learner
    /// and the programme, stops with More Information Needed if anything required is missing,
    /// otherwise asks the programme for the subject and mark match, the learner for the interest
    /// score and the programme for the faculty bonus, has the RecommendationEngine produce the
    /// outcome, and stores it as the session's selected RecommendationResult.
    /// </summary>
    public class EligibilityController
    {
        #region Data Members
        private LearnerRepository learnerRepository;
        private DegreeProgrammeRepository programmeRepository;
        private AdvisorySessionRepository sessionRepository;
        private RecommendationEngine engine;
        #endregion

        #region Constructor
        /// <summary>Creates the controller, its repositories and the engine (no database work yet).</summary>
        public EligibilityController()
        {
            learnerRepository = new LearnerRepository();
            programmeRepository = new DegreeProgrammeRepository();
            sessionRepository = new AdvisorySessionRepository();
            engine = new RecommendationEngine();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Class diagram checkEligibility(learnerId, programmeId). Checks the learner's current
        /// session against the programme and stores the result (IsSelectedProgramme = 1, also
        /// for More Information Needed). Always returns an outcome with its explanation.
        /// </summary>
        public AdvisoryOutcome CheckEligibility(int learnerID, int programmeID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// UI 6 requirement grid: the check of each active requirement of the programme against
        /// the session's subjects (SubjectName, MinimumMark, Mark, RequirementOutcome, IsMet).
        /// </summary>
        public Collection<RequirementCheck> GetCheckedRequirements(int sessionID, int programmeID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// UI 6 figure tiles for the stored selected result: one row with SubjectMatchScore,
        /// MarkMatchScore, InterestMatchScore, OverallScore, OutcomeCode, OutcomeCategory,
        /// OutcomeReason, SubjectNote, MarkNote, StrongestCategory. Empty table if none is stored.
        /// </summary>
        public DataTable GetSelectedResult(int sessionID)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
