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
        /// for More Information Needed when the session and programme exist). Always returns an
        /// outcome with its explanation.
        /// </summary>
        public AdvisoryOutcome CheckEligibility(int learnerID, int programmeID)
        {
            // Sequence 1 step 2: findById(learnerId)
            Learner learner = learnerRepository.FindById(learnerID);
            // Step 3: findByIdWithRequirements(programmeId)
            DegreeProgramme programme = programmeRepository.FindByIdWithRequirements(programmeID);

            // Step 4 (alt): required learner or programme information missing. Nothing can be stored.
            if (learner == null || !learner.CurrentSessionID.HasValue)
            {
                return AdvisoryOutcome.MoreInformationNeeded("We could not find your advisory session. Start again from step 1.");
            }
            if (programme == null || !programme.IsActive)
            {
                return AdvisoryOutcome.MoreInformationNeeded("Choose an active degree programme and check again.");
            }

            // Step 4 (alt): no subjects or no interest answers. The selected row is still stored (database.md).
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

            // Step 5 (else): getSubjects() and getPreferredFacultyId()
            Collection<LearnerSubject> subjects = learner.GetSubjects();
            int facultyID = learner.GetPreferredFacultyId();
            // Step 6: evaluateSubjectAndMarkMatch(learnerSubjects)
            MatchResult matchResult = programme.EvaluateSubjectAndMarkMatch(subjects);
            // Step 7: getInterestCategories()
            Collection<InterestCategory> categories = programme.GetInterestCategories();
            // Step 8: calculateInterestScore(categories)
            decimal interestScore = learner.CalculateInterestScore(categories);
            // Step 9: matchesPreferredFaculty(facultyId)
            bool facultyBonus = programme.MatchesPreferredFaculty(facultyID);
            // Step 10: computeOutcome(matchResult, interestScore, facultyBonus)
            outcome = engine.ComputeOutcome(matchResult, interestScore, facultyBonus);

            // Step 11: save(session), then save(result)
            SaveSelectedResult(learner, programme, matchResult, interestScore, facultyBonus, outcome);
            // Step 12: the view shows the outcome with its explanation
            return outcome;
        }

        /// <summary>
        /// UI 6 requirement grid: the check of each active requirement of the programme against
        /// the session's subjects (SubjectName, MinimumMark, Mark, RequirementOutcome, IsMet).
        /// Empty if the session or the programme does not exist.
        /// </summary>
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

        /// <summary>
        /// UI 6 figure tiles for the stored selected result: one row with SubjectMatchScore,
        /// MarkMatchScore, InterestMatchScore, OverallScore (decimal or DBNull), OutcomeCode,
        /// OutcomeCategory, OutcomeReason, SubjectNote (e.g. "5 of 5 required subjects captured"),
        /// MarkNote (e.g. "2 of 3 minimum marks met") and StrongestCategory (the learner's
        /// highest-scoring category mapped to the programme, or ""). Empty table if none is stored.
        /// </summary>
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

            // The notes come from checking the session's subjects against the programme again
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
        /// <summary>
        /// Sequence 1 step 11. save(session): the session records the selected programme (and the
        /// preferred faculty, or the programme's faculty if none was chosen) when it is not stored
        /// yet. save(result): the selected programme's RecommendationResult replaces the old one.
        /// </summary>
        private void SaveSelectedResult(Learner learner, DegreeProgramme programme, MatchResult matchResult, decimal? interestScore,
                                        bool facultyBonus, AdvisoryOutcome outcome)
        {
            int sessionID = learner.CurrentSessionID.Value;
            AdvisorySession session = sessionRepository.FindById(sessionID);
            if (session.SelectedProgrammeID != programme.ProgrammeID)
            {
                sessionRepository.SaveProgrammeSelection(sessionID, learner.PreferredFacultyID ?? programme.FacultyID, programme.ProgrammeID);
            }
            // Class diagram recordOutcome(outcome): the session's outcome is the selected result's OutcomeCode
            session.RecordOutcome(outcome.Code);
            sessionRepository.Save(new RecommendationResult(sessionID, programme, matchResult, interestScore, facultyBonus, outcome, true));
        }

        /// <summary>The name of the learner's highest-scoring category among those mapped to the programme, or "" if none was answered.</summary>
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

        /// <summary>The value to put in a DataTable cell: null becomes DBNull.</summary>
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
