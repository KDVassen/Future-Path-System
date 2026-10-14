using System;
using System.Collections.ObjectModel;
using FuturePath.DatabaseLayer;

namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// Sequence 2, "Generate alternative degree recommendations" (spec Section 3). UI 6's
    /// Alternative programmes button calls GenerateAlternatives. Every active programme except
    /// the selected one is scored the same way as Sequence 1, ranked by the engine, and stored.
    /// </summary>
    public class RecommendationController
    {
        #region Data Members
        private LearnerRepository learnerRepository;
        private DegreeProgrammeRepository programmeRepository;
        private AdvisorySessionRepository sessionRepository;
        private RecommendationEngine engine;
        private AdvisoryOutcome missingInformation;
        #endregion

        #region Properties
        /// <summary>
        /// Set by GenerateAlternatives when the learner has no subjects or interest answers
        /// (sequence diagram "More information needed"), otherwise null.
        /// </summary>
        public AdvisoryOutcome MissingInformation
        {
            get { return missingInformation; }
        }
        #endregion

        #region Constructor
        /// <summary>Creates the controller, its repositories and the engine (no database work yet).</summary>
        public RecommendationController()
        {
            learnerRepository = new LearnerRepository();
            programmeRepository = new DegreeProgrammeRepository();
            sessionRepository = new AdvisorySessionRepository();
            engine = new RecommendationEngine();
            missingInformation = null;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Class diagram generateAlternatives(learnerId, excludedProgrammeId). Returns the
        /// session's stored selected result first (AlternativeRank null), then the ranked
        /// alternatives (rank 1 = best), each with its reason, ProgrammeName and FacultyName. The
        /// alternatives are saved with AdvisorySessionRepository.SaveAll. If information is
        /// missing it returns an empty collection and sets MissingInformation.
        /// </summary>
        public Collection<RecommendationResult> GenerateAlternatives(int learnerID, int excludedProgrammeID)
        {
            throw new NotImplementedException();
        }

        /// <summary>The UI 6.1a "include these alternatives in my summary" choice.</summary>
        public bool GetAlternativesInSummary(int sessionID)
        {
            throw new NotImplementedException();
        }

        /// <summary>Stores the UI 6.1a "include these alternatives in my summary" choice.</summary>
        public void SetAlternativesInSummary(int sessionID, bool include)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
