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
        /// Set by GenerateAlternatives when the learner, their session, their subjects or their
        /// interest answers are missing (sequence diagram "More information needed"), otherwise null.
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
        /// alternatives are saved with AdvisorySessionRepository.SaveAll, replacing earlier ones.
        /// If information is missing it returns an empty collection and sets MissingInformation.
        /// </summary>
        public Collection<RecommendationResult> GenerateAlternatives(int learnerID, int excludedProgrammeID)
        {
            missingInformation = null;
            Collection<RecommendationResult> shown = new Collection<RecommendationResult>();

            // Sequence 2 step 2: findById(learnerId)
            Learner learner = learnerRepository.FindById(learnerID);

            // Step 3 (alt): the learner has no session, no captured subjects or no interest answers
            if (learner == null || !learner.CurrentSessionID.HasValue)
            {
                missingInformation = AdvisoryOutcome.MoreInformationNeeded("We could not find your advisory session. Start again from step 1.");
                return shown;
            }
            if (!learner.HasSubjects)
            {
                missingInformation = AdvisoryOutcome.MoreInformationNeeded("Capture at least one subject and mark (step 2) so alternative programmes can be checked.");
                return shown;
            }
            if (!learner.HasInterestResponses)
            {
                missingInformation = AdvisoryOutcome.MoreInformationNeeded("Complete the interest questionnaire (step 3) so alternative programmes can be checked.");
                return shown;
            }
            int sessionID = learner.CurrentSessionID.Value;

            // Step 4 (else): getSubjects(), getPreferredFacultyId(), findActiveProgrammes(excludedProgrammeId)
            Collection<LearnerSubject> subjects = learner.GetSubjects();
            int facultyID = learner.GetPreferredFacultyId();
            Collection<DegreeProgramme> candidates = programmeRepository.FindActiveProgrammes(excludedProgrammeID);

            // The stored selected row is shown first; its programme is never also an alternative
            // (UNIQUE(SessionID, ProgrammeID)), even if a different programme was excluded
            RecommendationResult selected = sessionRepository.FindSelectedResult(sessionID);

            // Step 5 (loop): score each candidate exactly as Sequence 1 does
            Collection<RecommendationResult> candidateOutcomes = new Collection<RecommendationResult>();
            foreach (DegreeProgramme candidate in candidates)
            {
                if (selected != null && candidate.ProgrammeID == selected.ProgrammeID)
                {
                    continue;
                }
                MatchResult matchResult = candidate.EvaluateSubjectAndMarkMatch(subjects);
                Collection<InterestCategory> categories = candidate.GetInterestCategories();
                decimal interestScore = learner.CalculateInterestScore(categories);
                bool facultyBonus = candidate.MatchesPreferredFaculty(facultyID);
                AdvisoryOutcome outcome = engine.ComputeOutcome(matchResult, interestScore, facultyBonus);
                candidateOutcomes.Add(new RecommendationResult(sessionID, candidate, matchResult, interestScore, facultyBonus, outcome, false));
            }

            // Step 6: rankAlternatives(candidateOutcomes)
            Collection<RecommendationResult> rankedAlternatives = engine.RankAlternatives(candidateOutcomes);
            // Step 7: saveAll(rankedAlternatives)
            sessionRepository.SaveAll(rankedAlternatives);

            // Step 8: displayAlternatives - the selected programme's stored row first, then the ranked list
            if (selected != null)
            {
                shown.Add(selected);
            }
            foreach (RecommendationResult alternative in rankedAlternatives)
            {
                shown.Add(alternative);
            }
            return shown;
        }

        /// <summary>The UI 6.1a "include these alternatives in my summary" choice (false if the session does not exist).</summary>
        public bool GetAlternativesInSummary(int sessionID)
        {
            AdvisorySession session = sessionRepository.FindById(sessionID);
            return session != null && session.AlternativesInSummary;
        }

        /// <summary>Stores the UI 6.1a "include these alternatives in my summary" choice.</summary>
        public void SetAlternativesInSummary(int sessionID, bool include)
        {
            sessionRepository.SetAlternativesInSummary(sessionID, include);
        }
        #endregion
    }
}
