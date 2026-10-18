
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using FuturePath.ApplicationLayer.Reports;
using FuturePath.DatabaseLayer;

namespace FuturePath.ApplicationLayer
{
    // Builds the learner advisory summary report.
    public partial class ReportController
    {
        #region FP-LSR - Learner Advisory Summary (6.1)

        // Get a learner summary for an in-progress or completed session.
        public FpLsrData GetLearnerSummary(int sessionID)
        {
            AdvisorySession session = sessionRepository.FindById(sessionID);
            if (session == null)
            {
                throw new InvalidOperationException("This advisory session could not be found. Please start a new session.");
            }
            return BuildLearnerSummary(session);
        }

        // Get a completed learner summary for a staff member.
        public FpLsrData GetLearnerSummary(int actorUserID, int sessionID)
        {
            loginController.CheckStaff(actorUserID);
            AdvisorySession session = sessionRepository.FindById(sessionID);
            if (session == null)
            {
                throw new InvalidOperationException("This advisory session could not be found. Search for it again.");
            }
            if (!session.IsCompleted)
            {
                throw new InvalidOperationException("Session " + session.SessionNumber + " is still in progress. Its advisory summary exists only once the session is completed.");
            }
            return BuildLearnerSummary(session);
        }

        // Complete the learner session and record the summary export.
        public void CompleteLearnerSession(int sessionID, string exportFormat, DateTime generatedOn)
        {
            string format = CheckFormat(ExportReport.CodeLearnerSummary, exportFormat);
            AdvisorySession session = sessionRepository.FindById(sessionID);
            if (session == null)
            {
                throw new InvalidOperationException("Your advisory session could not be found. Please start a new session.");
            }
            if (sessionRepository.FindSelectedResult(sessionID) == null)
            {
                throw new InvalidOperationException("Run the eligibility check before exporting the summary.");
            }
            if (!session.IsCompleted)
            {
                sessionRepository.MarkCompleted(sessionID, generatedOn);
            }
            exportRepository.Insert(new ExportReport(ExportReport.CodeLearnerSummary, sessionID, null, null, generatedOn, format));
        }

        // Build the report data using the stored eligibility result.
        private FpLsrData BuildLearnerSummary(AdvisorySession session)
        {
            RecommendationResult selected = sessionRepository.FindSelectedResult(session.SessionID);
            if (selected == null)
            {
                throw new InvalidOperationException("Run the eligibility check before exporting the summary.");
            }
            if (session.SelectedProgrammeID.HasValue && session.SelectedProgrammeID.Value != selected.ProgrammeID)
            {
                throw new InvalidOperationException("Run the eligibility check again: the selected programme has changed since the last check.");
            }
            Learner learner = learnerRepository.FindBySessionId(session.SessionID);
            if (learner == null)
            {
                throw new InvalidOperationException("The learner for this advisory session could not be found.");
            }
            DegreeProgramme programme = programmeRepository.FindByIdWithRequirements(selected.ProgrammeID);
            if (programme == null)
            {
                throw new InvalidOperationException("The selected programme could not be found.");
            }
            MatchResult match = programme.EvaluateSubjectAndMarkMatch(learner.GetSubjects());

            FpLsrData data = new FpLsrData();
            AddSummaryHeader(data, session, learner, programme, selected, match);
            AddSubjectRows(data, learner, match);
            AddMarkComparisonRow(data, OverallAverageLabel, match.OverallAverage, match.MinimumOverallAverage, GetAverageResultCode(match), true);
            AddInterestRows(data, learner);
            if (session.AlternativesInSummary)
            {
                AddAlternativeRows(data, session.SessionID);
            }
            if (session.FundingInfoRequested)
            {
                AddFundingRows(data, session);
            }
            return data;
        }

        // Add the learner, programme and outcome details to the report header.
        private void AddSummaryHeader(FpLsrData data, AdvisorySession session, Learner learner, DegreeProgramme programme,
                                      RecommendationResult selected, MatchResult match)
        {
            FpLsrData.ReportHeaderRow header = data.ReportHeader.NewReportHeaderRow();
            header.ReportCode = ExportReport.CodeLearnerSummary;
            header.SessionNumber = session.SessionNumber;
            header.SessionDate = session.SessionDate;
            header.DateGenerated = DateTime.Now;

            // Leave contact details out of the learner summary.
            if (string.IsNullOrWhiteSpace(learner.FirstName))
            {
                header.SetFirstNameNull();
            }
            else
            {
                header.FirstName = learner.FirstName.Trim();
            }
            if (string.IsNullOrWhiteSpace(learner.LastName))
            {
                header.SetLastNameNull();
            }
            else
            {
                header.LastName = learner.LastName.Trim();
            }

            // Add the grade description and mark basis.
            header.GradeDescription = "";
            header.MarkBasis = "";
            DataTable grades = reportRepository.GetGradeLevel(session.GradeLevelID);
            if (grades.Rows.Count > 0)
            {
                header.GradeDescription = TextOf(grades.Rows[0]["GradeDescription"]);
                header.MarkBasis = TextOf(grades.Rows[0]["MarkBasis"]);
            }
            header.SchoolName = TextOf(learner.SchoolName);
            header.Province = TextOf(learner.Province);
            header.SubjectCount = learner.GetSubjects().Count;
            if (match.OverallAverage.HasValue)
            {
                header.OverallAverage = match.OverallAverage.Value;
            }
            else
            {
                header.SetOverallAverageNull();
            }

            header.ProgrammeName = TextOf(programme.ProgrammeName);
            header.FacultyName = TextOf(programme.FacultyName);
            if (programme.MinimumOverallAverage.HasValue)
            {
                header.MinimumOverallAverage = programme.MinimumOverallAverage.Value;
            }
            else
            {
                header.SetMinimumOverallAverageNull();
            }

            string averageCode = GetAverageResultCode(match);
            header.AverageResultCode = averageCode;
            if (averageCode == ResultGap)
            {
                header.AverageShortfall = match.AverageShortfall.Value;
            }
            else
            {
                header.SetAverageShortfallNull();
            }

            // Count the checked and met requirements, including the overall average.
            int requirementsChecked = match.Checks.Count;
            int requirementsMet = 0;
            foreach (RequirementCheck check in match.Checks)
            {
                if (check.IsMet)
                {
                    requirementsMet++;
                }
            }
            if (averageCode != ResultNotAssessed)
            {
                requirementsChecked++;
                if (averageCode == ResultMet)
                {
                    requirementsMet++;
                }
            }
            header.RequirementsChecked = requirementsChecked;
            header.RequirementsMet = requirementsMet;

            // Use the saved outcome and explanation.
            header.OutcomeCode = TextOf(selected.OutcomeCode);
            header.OutcomeReason = TextOf(selected.OutcomeReason);
            header.QuestionsAnswered = learner.GetResponses().Count;
            header.OverallGuidance = BuildGuidance(selected.OutcomeCode, header.ProgrammeName, match, session.Grade);
            header.IncludeAlternatives = session.AlternativesInSummary;
            header.IncludeFunding = session.FundingInfoRequested;
            data.ReportHeader.AddReportHeaderRow(header);
        }

        // Add captured and missing required subjects in alphabetical order.
        private void AddSubjectRows(FpLsrData data, Learner learner, MatchResult match)
        {
            List<string> names = new List<string>();
            foreach (LearnerSubject subject in learner.GetSubjects())
            {
                names.Add(subject.SubjectName);
            }
            foreach (RequirementCheck check in match.Checks)
            {
                if (IsMissingRequired(check) && !names.Contains(check.SubjectName))
                {
                    names.Add(check.SubjectName);
                }
            }
            names.Sort(StringComparer.OrdinalIgnoreCase);

            foreach (string name in names)
            {
                LearnerSubject subject = FindSubjectByName(learner.GetSubjects(), name);
                if (subject != null)
                {
                    AddCapturedSubjectRow(data, subject, FindCheckForSubjectTaken(match, name));
                }
                else
                {
                    AddMissingSubjectRow(data, FindMissingCheck(match, name));
                }
            }
        }

        // Add a captured subject and its requirement result.
        private static void AddCapturedSubjectRow(FpLsrData data, LearnerSubject subject, RequirementCheck check)
        {
            FpLsrData.SubjectRequirementsRow row = data.SubjectRequirements.NewSubjectRequirementsRow();
            row.SubjectName = subject.SubjectName;
            row.Mark = subject.Mark;
            if (check == null)
            {
                row.SetRequirementTypeNull();
                row.SetAdvisoryNoteNull();
                row.SetMinimumMarkNull();
                row.ResultCode = ResultNotAssessed;
                row.SetShortfallNull();
            }
            else
            {
                row.RequirementType = check.RequirementType;
                SetAdvisoryNote(row, check.AdvisoryNote);
                if (check.MinimumMark.HasValue)
                {
                    row.MinimumMark = check.MinimumMark.Value;
                }
                else
                {
                    row.SetMinimumMarkNull();
                }
                row.ResultCode = ResultGap;
                if (check.IsMet)
                {
                    row.ResultCode = ResultMet;
                }
                if (check.ResultCode == RequirementCheck.ResultGap && check.Shortfall.HasValue)
                {
                    row.Shortfall = check.Shortfall.Value;
                }
                else
                {
                    row.SetShortfallNull();
                }
            }
            data.SubjectRequirements.AddSubjectRequirementsRow(row);

            // Include subjects with a minimum mark in the chart.
            if (check != null && check.MinimumMark.HasValue)
            {
                AddMarkComparisonRow(data, subject.SubjectName, subject.Mark, check.MinimumMark, row.ResultCode, false);
            }
        }

        // Add a required subject that the learner has not taken.
        private static void AddMissingSubjectRow(FpLsrData data, RequirementCheck check)
        {
            FpLsrData.SubjectRequirementsRow row = data.SubjectRequirements.NewSubjectRequirementsRow();
            row.SubjectName = check.SubjectName;
            row.SetMarkNull();
            row.RequirementType = check.RequirementType;
            SetAdvisoryNote(row, check.AdvisoryNote);
            if (check.MinimumMark.HasValue)
            {
                row.MinimumMark = check.MinimumMark.Value;
            }
            else
            {
                row.SetMinimumMarkNull();
            }
            row.ResultCode = ResultGap;
            row.SetShortfallNull();
            data.SubjectRequirements.AddSubjectRequirementsRow(row);

            if (check.MinimumMark.HasValue)
            {
                AddMarkComparisonRow(data, check.SubjectName, null, check.MinimumMark, ResultGap, false);
            }
        }

        // Add a subject or overall average comparison for the report chart.
        private static void AddMarkComparisonRow(FpLsrData data, string label, decimal? mark, decimal? minimum, string resultCode, bool isOverallAverage)
        {
            FpLsrData.MarkComparisonRow row = data.MarkComparison.NewMarkComparisonRow();
            row.Label = label;
            if (mark.HasValue)
            {
                row.Mark = mark.Value;
            }
            else
            {
                row.SetMarkNull();
            }
            if (minimum.HasValue)
            {
                row.MinimumMark = minimum.Value;
            }
            else
            {
                row.SetMinimumMarkNull();
            }
            row.ResultCode = resultCode;
            row.IsOverallAverage = isOverallAverage;
            data.MarkComparison.AddMarkComparisonRow(row);
        }

        // Add the learner's interest category scores, highest first.
        private void AddInterestRows(FpLsrData data, Learner learner)
        {
            List<KeyValuePair<string, decimal>> scores = new List<KeyValuePair<string, decimal>>();
            foreach (InterestCategory category in interestRepository.GetCategories(false))
            {
                decimal? score = learner.CalculateCategoryScore(category.CategoryID);
                if (score.HasValue)
                {
                    scores.Add(new KeyValuePair<string, decimal>(category.CategoryName, Math.Round(score.Value, 1)));
                }
            }
            scores.Sort(CompareScoresDescending);
            foreach (KeyValuePair<string, decimal> score in scores)
            {
                data.InterestProfile.AddInterestProfileRow(score.Key, score.Value);
            }
        }

        // Add eligible stored alternatives to the report.
        private void AddAlternativeRows(FpLsrData data, int sessionID)
        {
            foreach (RecommendationResult result in sessionRepository.FindResults(sessionID))
            {
                if (result.IsSelectedProgramme || !result.OverallScore.HasValue)
                {
                    continue;
                }
                if (!reportRepository.IsProgrammeCurrent(result.ProgrammeID))
                {
                    continue;
                }
                FpLsrData.AlternativesRow row = data.Alternatives.NewAlternativesRow();
                if (result.AlternativeRank.HasValue)
                {
                    row.AlternativeRank = result.AlternativeRank.Value;
                }
                else
                {
                    row.SetAlternativeRankNull();
                }
                row.ProgrammeName = TextOf(result.ProgrammeName);
                row.FacultyName = TextOf(result.FacultyName);
                row.OverallScore = result.OverallScore.Value;
                row.OutcomeCode = TextOf(result.OutcomeCode);
                if (string.IsNullOrWhiteSpace(result.OutcomeReason))
                {
                    row.SetOutcomeReasonNull();
                }
                else
                {
                    row.OutcomeReason = result.OutcomeReason;
                }
                data.Alternatives.AddAlternativesRow(row);
            }
        }

        // Add active, open funding options linked to the session.
        private void AddFundingRows(FpLsrData data, AdvisorySession session)
        {
            // Use zero when no faculty or programme has been chosen.
            int facultyID = 0;
            if (session.PreferredFacultyID.HasValue)
            {
                facultyID = session.PreferredFacultyID.Value;
            }
            int programmeID = 0;
            if (session.SelectedProgrammeID.HasValue)
            {
                programmeID = session.SelectedProgrammeID.Value;
            }
            foreach (FundingOption option in fundingRepository.GetActiveFundingFor(facultyID, programmeID))
            {
                if (!option.IsActive || !option.IsOpen)
                {
                    continue;
                }
                FpLsrData.FundingOptionsRow row = data.FundingOptions.NewFundingOptionsRow();
                row.FundingName = TextOf(option.FundingName);
                row.FundingType = TextOf(option.FundingType);
                row.LinkedTo = option.LinkedTo;
                if (row.LinkedTo.Length == 0)
                {
                    row.LinkedTo = "All faculties";
                }
                row.EligibilityDescription = TextOf(option.EligibilityDescription);
                if (string.IsNullOrWhiteSpace(option.RequiredDocuments))
                {
                    row.SetRequiredDocumentsNull();
                }
                else
                {
                    row.RequiredDocuments = option.RequiredDocuments;
                }
                if (option.ClosingDate.HasValue)
                {
                    row.ClosingDate = option.ClosingDate.Value;
                }
                else
                {
                    row.SetClosingDateNull();
                }
                data.FundingOptions.AddFundingOptionsRow(row);
            }
        }

        // Check whether the overall average meets the programme minimum.
        private static string GetAverageResultCode(MatchResult match)
        {
            if (!match.MinimumOverallAverage.HasValue || !match.OverallAverage.HasValue)
            {
                return ResultNotAssessed;
            }
            if (match.AverageMet)
            {
                return ResultMet;
            }
            return ResultGap;
        }

        // Build guidance based on the stored outcome and remaining gaps.
        private static string BuildGuidance(string outcomeCode, string programmeName, MatchResult match, GradeLevel grade)
        {
            string gaps = JoinSentences(match.GapList);
            string guidance;
            switch (TextOf(outcomeCode).ToUpperInvariant())
            {
                case AdvisoryOutcome.CodeStrong:
                    guidance = "You appear to meet the stored requirements for " + programmeName + ". Confirm the requirements and "
                               + "closing dates with the institution, and apply before the deadline.";
                    break;
                case AdvisoryOutcome.CodePossible:
                    if (gaps.Length > 0)
                    {
                        guidance = "You meet some of the stored requirements for " + programmeName + ". To strengthen your position, "
                                   + "work on the following: " + gaps + " Then confirm the requirements with the institution before you apply.";
                    }
                    else
                    {
                        guidance = "You meet the stored requirements for " + programmeName + ", but your interest answers fit it less "
                                   + "strongly. Compare it with the alternative programmes and talk to a Career Advisor before you decide.";
                    }
                    break;
                case AdvisoryOutcome.CodeNotEligible:
                    guidance = "You do not currently meet one or more stored requirements for " + programmeName + ".";
                    if (gaps.Length > 0)
                    {
                        guidance += " " + gaps;
                    }
                    guidance += " Talk to your teacher or a Career Advisor about your subject choices and marks, and look at the "
                                + "alternative programmes, which may suit you now.";
                    break;
                default:
                    guidance = "The check could not be completed because some information was missing. Capture all your subjects "
                               + "and answer every interest statement to get a full result.";
                    break;
            }
            if (grade == GradeLevel.Grade11)
            {
                guidance += " Your Grade 11 marks are projected, so the outcome can change when your final marks are known.";
            }
            return guidance;
        }

        // Add an advisory note when one is available.
        private static void SetAdvisoryNote(FpLsrData.SubjectRequirementsRow row, string note)
        {
            if (string.IsNullOrWhiteSpace(note))
            {
                row.SetAdvisoryNoteNull();
            }
            else
            {
                row.AdvisoryNote = note;
            }
        }

        // Check whether a required subject is missing.
        private static bool IsMissingRequired(RequirementCheck check)
        {
            return check.IsRequired && check.ResultCode == RequirementCheck.ResultMissing;
        }

        // Find a captured subject by its name.
        private static LearnerSubject FindSubjectByName(Collection<LearnerSubject> subjects, string name)
        {
            foreach (LearnerSubject subject in subjects)
            {
                if (string.Equals(subject.SubjectName, name, StringComparison.OrdinalIgnoreCase))
                {
                    return subject;
                }
            }
            return null;
        }

        // Find the requirement check for a captured subject.
        private static RequirementCheck FindCheckForSubjectTaken(MatchResult match, string subjectName)
        {
            foreach (RequirementCheck check in match.Checks)
            {
                if (string.Equals(check.SubjectTakenName, subjectName, StringComparison.OrdinalIgnoreCase))
                {
                    return check;
                }
            }
            return null;
        }

        // Find the requirement check for a missing subject.
        private static RequirementCheck FindMissingCheck(MatchResult match, string subjectName)
        {
            foreach (RequirementCheck check in match.Checks)
            {
                if (IsMissingRequired(check) && string.Equals(check.SubjectName, subjectName, StringComparison.OrdinalIgnoreCase))
                {
                    return check;
                }
            }
            return null;
        }
        #endregion
    }
}
