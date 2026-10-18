
using System.Collections.ObjectModel;
using FuturePath.DatabaseLayer;

namespace FuturePath.ApplicationLayer
{
    // Generates alternative degree recommendations for learners
    // Checks programme requirements, calculates scores and ranks the results
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
        // Store an explanation if there is not enough information for recommendations
        public AdvisoryOutcome MissingInformation
        {
            get { return missingInformation; }
        }
        #endregion

        #region Constructor
        // Initialise the repositories and recommendation engine
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
        // Generate and rank alternative programmes for the learner
        // Return the selected programme first, followed by ranked alternatives
        public Collection<RecommendationResult> GenerateAlternatives(int learnerID, int excludedProgrammeID)
        {
            missingInformation = null;
            Collection<RecommendationResult> shown = new Collection<RecommendationResult>();

            // Load the learner's details
            Learner learner = learnerRepository.FindById(learnerID);

            // Check that the learner has a session, subjects and interest responses
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

            // Get the learner's subjects, preferred faculty and available programmes
            Collection<LearnerSubject> subjects = learner.GetSubjects();
            int facultyID = learner.GetPreferredFacultyId();
            Collection<DegreeProgramme> candidates = programmeRepository.FindActiveProgrammes(excludedProgrammeID);

            // Load the selected programme so it is not included as an alternative
            RecommendationResult selected = sessionRepository.FindSelectedResult(sessionID);

            // Calculate match scores and outcomes for each alternative programme
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

            // Rank the alternatives and save their results
            Collection<RecommendationResult> rankedAlternatives = engine.RankAlternatives(candidateOutcomes);
            sessionRepository.SaveAll(rankedAlternatives);

            // Add the selected programme first, followed by the alternatives
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

        // Check if the learner wants alternatives included in their summary
        public bool GetAlternativesInSummary(int sessionID)
        {
            AdvisorySession session = sessionRepository.FindById(sessionID);
            return session != null && session.AlternativesInSummary;
        }

        // Save the learner's choice to include alternatives in the summary
        public void SetAlternativesInSummary(int sessionID, bool include)
        {
            sessionRepository.SetAlternativesInSummary(sessionID, include);
        }
        #endregion
    }
}
