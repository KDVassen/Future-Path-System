
using System;
using System.Collections.ObjectModel;
using System.Data;
using FuturePath.DatabaseLayer;

namespace FuturePath.ApplicationLayer
{
    // Handles eligibility checks for a learner's selected degree programme
    // Calculates match scores, generates advisory outcomes and saves the results
    public class EligibilityController
    {
        #region Data Members
        private LearnerRepository learnerRepository;
        private DegreeProgrammeRepository programmeRepository;
        private AdvisorySessionRepository sessionRepository;
        private RecommendationEngine engine;
        #endregion

        #region Constructor
        // Initialise the repositories and recommendation engine
        public EligibilityController()
        {
            learnerRepository = new LearnerRepository();
            programmeRepository = new DegreeProgrammeRepository();
            sessionRepository = new AdvisorySessionRepository();
            engine = new RecommendationEngine();
        }
        #endregion

        #region Methods
        // Check the learner's eligibility for a selected programme
        // Calculate subject, mark and interest scores before saving the outcome
        public AdvisoryOutcome CheckEligibility(int learnerID, int programmeID)
        {
            // Load the learner and programme details
            Learner learner = learnerRepository.FindById(learnerID);
            DegreeProgramme programme = programmeRepository.FindByIdWithRequirements(programmeID);

            // Check that the learner has a session and the programme is active
            if (learner == null || !learner.CurrentSessionID.HasValue)
            {
                return AdvisoryOutcome.MoreInformationNeeded("We could not find your advisory session. Start again from step 1.");
            }
            if (programme == null || !programme.IsActive)
            {
                return AdvisoryOutcome.MoreInformationNeeded("Choose an active degree programme and check again.");
            }

            // Return More Information Needed if subjects or interest responses are missing
            // Save the incomplete result so it can still be displayed later
            AdvisoryOutcome outcome;
            if (!learner.HasSubjects)
            {
                outcome = AdvisoryOutcome.MoreInformationNeeded("Capture at least one subject and mark (step 2) so your eligibility can be checked.");
                SaveSelectedResult(learner, programme, null, null, false, outcome);
                return outcome;
            }
            if (!learner.HasInterestResponses)
            {
                outcome = AdvisoryOutcome.MoreInformationNeeded("Complete the interest questionnaire (step 3) so your interests can be compared with this programme.");
                SaveSelectedResult(learner, programme, null, null, false, outcome);
                return outcome;
            }

            // Get the learner's subjects and preferred faculty
            Collection<LearnerSubject> subjects = learner.GetSubjects();
            int facultyID = learner.GetPreferredFacultyId();

            // Calculate the subject, mark and interest scores
            MatchResult matchResult = programme.EvaluateSubjectAndMarkMatch(subjects);
            Collection<InterestCategory> categories = programme.GetInterestCategories();
            decimal interestScore = learner.CalculateInterestScore(categories);

            // Check for a preferred faculty match and generate the advisory outcome
            bool facultyBonus = programme.MatchesPreferredFaculty(facultyID);
            outcome = engine.ComputeOutcome(matchResult, interestScore, facultyBonus);

            // Save the selected programme result for the session
            SaveSelectedResult(learner, programme, matchResult, interestScore, facultyBonus, outcome);
            return outcome;
        }

        // Return the results of checking each programme requirement
        // Return an empty collection if the learner or programme cannot be found
        public Collection<RequirementCheck> GetCheckedRequirements(int sessionID, int programmeID)
        {
            Learner learner = learnerRepository.FindBySessionId(sessionID);
            DegreeProgramme programme = programmeRepository.FindByIdWithRequirements(programmeID);
            if (learner == null || programme == null)
            {
                return new Collection<RequirementCheck>();
            }
            MatchResult matchResult = programme.EvaluateSubjectAndMarkMatch(learner.GetSubjects());
            return matchResult.Checks;
        }

        // Get the saved eligibility scores and outcome for the selected programme
        // Include notes showing how many subjects and minimum marks were met
        public DataTable GetSelectedResult(int sessionID)
        {
            DataTable table = new DataTable("SelectedResult");
            table.Columns.Add("SubjectMatchScore", typeof(decimal));
            table.Columns.Add("MarkMatchScore", typeof(decimal));
            table.Columns.Add("InterestMatchScore", typeof(decimal));
            table.Columns.Add("OverallScore", typeof(decimal));
            table.Columns.Add("OutcomeCode", typeof(string));
            table.Columns.Add("OutcomeCategory", typeof(string));
            table.Columns.Add("OutcomeReason", typeof(string));
            table.Columns.Add("SubjectNote", typeof(string));
            table.Columns.Add("MarkNote", typeof(string));
            table.Columns.Add("StrongestCategory", typeof(string));

            RecommendationResult result = sessionRepository.FindSelectedResult(sessionID);
            if (result == null)
            {
                return table;
            }

            // Recheck the learner's requirements to create the summary notes
            string subjectNote = "";
            string markNote = "";
            string strongestCategory = "";
            Learner learner = learnerRepository.FindBySessionId(sessionID);
            DegreeProgramme programme = programmeRepository.FindByIdWithRequirements(result.ProgrammeID);
            if (learner != null && programme != null)
            {
                MatchResult matchResult = programme.EvaluateSubjectAndMarkMatch(learner.GetSubjects());
                int required = 0;
                int captured = 0;
                int withMinimum = 0;
                int minimumsMet = 0;
                foreach (RequirementCheck check in matchResult.Checks)
                {
                    if (!check.IsRequired)
                    {
                        continue;
                    }
                    required++;
                    if (check.ResultCode != RequirementCheck.ResultMissing)
                    {
                        captured++;
                    }
                    if (check.MinimumMark.HasValue)
                    {
                        withMinimum++;
                        if (check.IsMet)
                        {
                            minimumsMet++;
                        }
                    }
                }
                subjectNote = captured + " of " + required + " required subjects captured";
                markNote = minimumsMet + " of " + withMinimum + " minimum marks met";
                strongestCategory = FindStrongestCategory(learner, programme);
            }

            // Add the saved scores and calculated notes to the result table
            DataRow row = table.NewRow();
            row["SubjectMatchScore"] = DbValue(result.SubjectMatchScore);
            row["MarkMatchScore"] = DbValue(result.MarkMatchScore);
            row["InterestMatchScore"] = DbValue(result.InterestMatchScore);
            row["OverallScore"] = DbValue(result.OverallScore);
            row["OutcomeCode"] = result.OutcomeCode;
            row["OutcomeCategory"] = AdvisoryOutcome.GetCategory(result.OutcomeCode);
            row["OutcomeReason"] = result.OutcomeReason;
            row["SubjectNote"] = subjectNote;
            row["MarkNote"] = markNote;
            row["StrongestCategory"] = strongestCategory;
            table.Rows.Add(row);
            return table;
        }
        #endregion

        #region Helpers
        // Save the programme selection and its recommendation result
        // Use the learner's preferred faculty or the programme faculty if none was selected
        private void SaveSelectedResult(Learner learner, DegreeProgramme programme, MatchResult matchResult, decimal? interestScore,
                                        bool facultyBonus, AdvisoryOutcome outcome)
        {
            int sessionID = learner.CurrentSessionID.Value;
            AdvisorySession session = sessionRepository.FindById(sessionID);
            if (session.SelectedProgrammeID != programme.ProgrammeID)
            {
                // Choose the faculty to save with the programme selection
                int facultyID = programme.FacultyID;
                if (learner.PreferredFacultyID.HasValue)
                {
                    facultyID = learner.PreferredFacultyID.Value;
                }
                sessionRepository.SaveProgrammeSelection(sessionID, facultyID, programme.ProgrammeID);
            }

            // Record the outcome and save the selected recommendation
            session.RecordOutcome(outcome.Code);
            sessionRepository.Save(new RecommendationResult(sessionID, programme, matchResult, interestScore, facultyBonus, outcome, true));
        }

        // Find the learner's highest-scoring interest category linked to the programme
        // Return an empty string if no category has been answered
        private static string FindStrongestCategory(Learner learner, DegreeProgramme programme)
        {
            string strongest = "";
            decimal bestScore = -1;
            foreach (DegreeInterestMapping mapping in programme.InterestMappings)
            {
                decimal? score = learner.CalculateCategoryScore(mapping.CategoryID);
                if (score.HasValue && score.Value > bestScore)
                {
                    bestScore = score.Value;
                    strongest = mapping.CategoryName;
                }
            }
            return strongest;
        }

        // Convert a nullable decimal into a value that can be stored in a DataTable
        // Use DBNull when the value is null
        private static object DbValue(decimal? value)
        {
            if (value.HasValue)
            {
                return value.Value;
            }
            return DBNull.Value;
        }
        #endregion
    }
}
