
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using FuturePath.ApplicationLayer.Reports;
using FuturePath.DatabaseLayer;

namespace FuturePath.ApplicationLayer
{
    // Builds the degree interest and learner eligibility reports
    public partial class ReportController
    {
        #region FP-DIE and FP-LER (6.2, 6.3)

        // Build the degree interest and eligibility report for a selected period
        public FpDieData BuildDegreeInterestReport(int actorUserID, DateTime fromDate, DateTime toDate, int facultyID, int gradeLevelID, string outcomeCode, bool includeInactive)
        {
            UserAccount actor = loginController.CheckStaff(actorUserID);
            CheckPeriod(fromDate, toDate);
            string outcome = OptionalOutcome(outcomeCode);

            DataTable sessions = reportRepository.GetCompletedSessions(PeriodStart(fromDate), PeriodEnd(toDate), facultyID, gradeLevelID, outcome, includeInactive);

            FpDieData data = new FpDieData();
            FpDieData.ReportHeaderRow header = data.ReportHeader.NewReportHeaderRow();
            AddOutcomeSummaryRows(data, sessions);
            AddFacultyOutcomeRows(data, sessions);
            AddProgrammeOutcomeRows(data, sessions);
            AddGapRows(data, sessions, header);
            AddFundingCategoryRows(data, sessions);

            int sessionCount = sessions.Rows.Count;
            int eligibleCount = CountOutcome(sessions, AdvisoryOutcome.CodeStrong) + CountOutcome(sessions, AdvisoryOutcome.CodePossible);

            header.ReportCode = ExportReport.CodeDegreeInterest;
            header.PeriodStart = fromDate.Date;
            header.PeriodEnd = toDate.Date;
            header.FacultyFilter = FacultyText(facultyID, "All faculties");
            header.GradeFilter = GradeText(gradeLevelID, "All grades");
            header.OutcomeFilter = "All outcomes";
            if (outcome.Length > 0)
            {
                header.OutcomeFilter = SentenceCase(AdvisoryOutcome.GetCategory(outcome));
            }
            header.IncludeInactive = includeInactive;
            header.GeneratedByName = actor.FullName + ", " + actor.RoleName;
            header.GeneratedAt = DateTime.Now;
            header.SessionsInScope = sessionCount;
            header.LearnerCount = CountDistinct(sessions, "LearnerID");
            if (sessionCount == 0)
            {
                header.SetEligibilityRateNull();
            }
            else
            {
                header.EligibilityRate = Percent(eligibleCount, sessionCount);
            }
            header.FundingRequestSessions = CountFundingRequests(sessions);

            // Add the notes displayed below the report charts
            if (sessionCount == 0)
            {
                header.OutcomeChartNote = "No completed sessions match this period and these filters.";
            }
            else
            {
                header.OutcomeChartNote = eligibleCount + " of " + sessionCount + " sessions (" + FormatNumber(Percent(eligibleCount, sessionCount))
                                          + "%) ended in a strong or possible match.";
            }
            if (data.FacultyOutcomes.Count == 0)
            {
                header.SetFacultyChartNoteNull();
            }
            else
            {
                FpDieData.FacultyOutcomesRow busiest = data.FacultyOutcomes[0];
                header.FacultyChartNote = busiest.FacultyName + " had the most sessions (" + busiest.SessionCount
                                          + "), with an eligibility rate of " + FormatNumber(busiest.EligibilityRate) + "%.";
            }
            if (data.SubjectGaps.Count == 0)
            {
                header.SubjectGapChartNote = "No session was missing a required subject.";
            }
            else
            {
                FpDieData.SubjectGapsRow top = data.SubjectGaps[0];
                header.SubjectGapChartNote = top.SubjectName + " is the required subject most often not taken: " + top.GapCount
                                             + " of " + CountGapOccurrences(data) + " gaps (" + FormatNumber(top.GapPercent) + "%).";
            }
            data.ReportHeader.AddReportHeaderRow(header);
            return data;
        }

        // Build the report of learners who need eligibility follow-up
        public FpLerData BuildNotEligibleReport(int actorUserID, DateTime fromDate, DateTime toDate, int facultyID, int gradeLevelID)
        {
            UserAccount actor = loginController.CheckStaff(actorUserID);
            CheckPeriod(fromDate, toDate);

            // Get completed sessions and the ones needing follow-up
            DataTable scope = reportRepository.GetScopeCounts(PeriodStart(fromDate), PeriodEnd(toDate), facultyID, gradeLevelID, "", false, false);
            int sessionsInScope = Convert.ToInt32(scope.Rows[0]["SessionCount"]);
            DataTable exceptions = reportRepository.GetExceptionSessions(PeriodStart(fromDate), PeriodEnd(toDate), facultyID, gradeLevelID);

            FpLerData data = new FpLerData();
            List<ReportTally> faculties = new List<ReportTally>();
            List<ReportTally> reasons = new List<ReportTally>();
            Dictionary<int, DegreeProgramme> programmes = new Dictionary<int, DegreeProgramme>();
            int facultySortOrder = 0;
            int previousFacultyID = 0;
            foreach (DataRow session in exceptions.Rows)
            {
                // Start a new group whenever the faculty changes
                int sessionFacultyID = Convert.ToInt32(session["FacultyID"]);
                if (sessionFacultyID != previousFacultyID)
                {
                    facultySortOrder++;
                    previousFacultyID = sessionFacultyID;
                }
                AddExceptionRow(data, session, facultySortOrder);

                FindOrAddTally(faculties, sessionFacultyID, TextOf(session["FacultyName"])).AddOutcome(TextOf(session["OutcomeCode"]));
                foreach (string reason in GetReasonCategories(session, programmes))
                {
                    FindOrAddTally(reasons, 0, reason).Count++;
                }
            }

            // Sort reasons and faculties by frequency
            reasons.Sort(CompareCountsDescending);
            foreach (ReportTally reason in reasons)
            {
                data.ReasonSummary.AddReasonSummaryRow(reason.Name, reason.Count);
            }
            int exceptionCount = exceptions.Rows.Count;
            faculties.Sort(CompareSessionsDescending);
            foreach (ReportTally tally in faculties)
            {
                data.FacultySummary.AddFacultySummaryRow(tally.ID, tally.Name, tally.SessionCount, Percent(tally.SessionCount, exceptionCount));
            }

            FpLerData.ReportHeaderRow header = data.ReportHeader.NewReportHeaderRow();
            header.ReportCode = ExportReport.CodeNotEligible;
            header.PeriodText = FormatPeriod(fromDate, toDate);
            header.ParametersText = "Faculty: " + FacultyText(facultyID, "All") + " \u00B7 Grade: " + GradeText(gradeLevelID, "All");
            header.GeneratedByName = actor.FullName + ", " + actor.RoleName;
            header.GeneratedAt = DateTime.Now;
            header.SessionsInScope = sessionsInScope;
            header.ExceptionCount = exceptionCount;
            header.ExceptionPercent = Percent(exceptionCount, sessionsInScope);
            header.NotEligibleCount = CountOutcome(exceptions, AdvisoryOutcome.CodeNotEligible);
            header.MoreInfoNeededCount = CountOutcome(exceptions, AdvisoryOutcome.CodeMoreInformation);
            header.LearnersAffected = CountDistinct(exceptions, "LearnerID");
            if (reasons.Count == 0)
            {
                header.ReasonChartNote = "No session in this period needs follow-up.";
            }
            else
            {
                header.ReasonChartNote = "The most common reason is \"" + reasons[0].Name + "\": " + reasons[0].Count + " of "
                                         + exceptionCount + " sessions. A session can record more than one reason.";
            }
            if (faculties.Count == 0)
            {
                header.FacultyChartNote = "No faculty has a session that needs follow-up.";
            }
            else
            {
                header.FacultyChartNote = faculties[0].Name + " carries the most follow-ups: " + faculties[0].SessionCount + " of "
                                          + exceptionCount + " (" + FormatNumber(Percent(faculties[0].SessionCount, exceptionCount)) + "%).";
            }
            data.ReportHeader.AddReportHeaderRow(header);
            return data;
        }

        // Add the outcome totals and percentages for the FP-DIE report
        private static void AddOutcomeSummaryRows(FpDieData data, DataTable sessions)
        {
            foreach (AdvisoryOutcome.OutcomeType type in AdvisoryOutcome.GetOutcomeTypes())
            {
                string code = AdvisoryOutcome.GetCode(type);
                int count = CountOutcome(sessions, code);
                data.OutcomeSummary.AddOutcomeSummaryRow(code, SentenceCase(AdvisoryOutcome.GetCategory(type)),
                                                         AdvisoryOutcome.GetDisplayOrder(type), count, Percent(count, sessions.Rows.Count));
            }
        }

        // Add faculty totals and outcome breakdowns
        private static void AddFacultyOutcomeRows(FpDieData data, DataTable sessions)
        {
            List<ReportTally> faculties = new List<ReportTally>();
            foreach (DataRow session in sessions.Rows)
            {
                FindOrAddTally(faculties, Convert.ToInt32(session["FacultyID"]), TextOf(session["FacultyName"])).AddOutcome(TextOf(session["OutcomeCode"]));
            }
            faculties.Sort(CompareSessionsDescending);
            foreach (ReportTally tally in faculties)
            {
                data.FacultyOutcomes.AddFacultyOutcomesRow(tally.Name, tally.SessionCount, tally.StrongCount, tally.PossibleCount,
                                                           tally.NotEligibleCount, tally.MoreInfoCount, tally.EligibilityRate);
                foreach (AdvisoryOutcome.OutcomeType type in AdvisoryOutcome.GetOutcomeTypes())
                {
                    string code = AdvisoryOutcome.GetCode(type);
                    data.FacultyOutcomeSlices.AddFacultyOutcomeSlicesRow(tally.Name, tally.SessionCount, code, SentenceCase(AdvisoryOutcome.GetCategory(type)),
                                                                         AdvisoryOutcome.GetDisplayOrder(type), tally.CountFor(code));
                }
            }
        }

        // Add programme totals, ordered by the number of selections
        private static void AddProgrammeOutcomeRows(FpDieData data, DataTable sessions)
        {
            List<ReportTally> programmes = new List<ReportTally>();
            foreach (DataRow session in sessions.Rows)
            {
                ReportTally programme = FindOrAddTally(programmes, Convert.ToInt32(session["ProgrammeID"]), TextOf(session["ProgrammeName"]));
                programme.FacultyName = TextOf(session["FacultyName"]);
                programme.AddOutcome(TextOf(session["OutcomeCode"]));
            }
            programmes.Sort(CompareSessionsDescending);
            foreach (ReportTally tally in programmes)
            {
                data.ProgrammeOutcomes.AddProgrammeOutcomesRow(tally.Name, tally.FacultyName, tally.SessionCount, tally.StrongCount,
                                                               tally.PossibleCount, tally.NotEligibleCount, tally.MoreInfoCount, tally.EligibilityRate);
            }
        }

        // Calculate missing subjects and mark shortfalls for completed sessions
        private void AddGapRows(FpDieData data, DataTable sessions, FpDieData.ReportHeaderRow header)
        {
            List<ReportTally> subjectGaps = new List<ReportTally>();
            List<ReportTally> markGaps = new List<ReportTally>();
            Dictionary<int, DegreeProgramme> programmes = new Dictionary<int, DegreeProgramme>();
            int subjectGapSessions = 0;
            int markGapSessions = 0;
            decimal shortfallTotal = 0;
            int shortfallCount = 0;

            foreach (DataRow session in sessions.Rows)
            {
                MatchResult match = EvaluateSession(Convert.ToInt32(session["SessionID"]), Convert.ToInt32(session["ProgrammeID"]), programmes);
                if (match == null)
                {
                    continue;
                }
                bool hasSubjectGap = false;
                bool hasMarkGap = false;
                foreach (RequirementCheck check in match.Checks)
                {
                    if (!check.IsRequired)
                    {
                        continue;
                    }
                    if (check.ResultCode == RequirementCheck.ResultMissing)
                    {
                        FindOrAddTally(subjectGaps, 0, check.SubjectName).Count++;
                        hasSubjectGap = true;
                    }
                    else if (check.ResultCode == RequirementCheck.ResultGap)
                    {
                        // Record the mark and shortfall for this requirement
                        decimal shortfall = check.Shortfall.Value;
                        decimal mark = check.Mark.Value;
                        ReportTally tally = FindOrAddTally(markGaps, 0, check.SubjectTakenName);
                        if (tally.Count == 0 || mark < tally.LowestMark)
                        {
                            tally.LowestMark = mark;
                        }
                        tally.Count++;
                        tally.ShortfallTotal += shortfall;
                        shortfallTotal += shortfall;
                        shortfallCount++;
                        hasMarkGap = true;
                    }
                }
                if (hasSubjectGap)
                {
                    subjectGapSessions++;
                }
                if (hasMarkGap)
                {
                    markGapSessions++;
                }
            }

            int gapOccurrences = 0;
            foreach (ReportTally tally in subjectGaps)
            {
                gapOccurrences += tally.Count;
            }
            subjectGaps.Sort(CompareCountsDescending);
            foreach (ReportTally tally in subjectGaps)
            {
                data.SubjectGaps.AddSubjectGapsRow(tally.Name, tally.Count, Percent(tally.Count, gapOccurrences));
            }
            markGaps.Sort(CompareCountsDescending);
            foreach (ReportTally tally in markGaps)
            {
                data.MarkShortfalls.AddMarkShortfallsRow(tally.Name, tally.Count, Math.Round(tally.ShortfallTotal / tally.Count, 1), tally.LowestMark);
            }

            header.SubjectGapSessions = subjectGapSessions;
            header.MarkGapSessions = markGapSessions;
            if (shortfallCount > 0)
            {
                header.AverageShortfall = Math.Round(shortfallTotal / shortfallCount, 1);
            }
            else
            {
                header.SetAverageShortfallNull();
            }
        }

        // Summarise funding types offered and added to learner summaries
        private void AddFundingCategoryRows(FpDieData data, DataTable sessions)
        {
            Collection<FundingOption> options = fundingRepository.GetActiveFundingFor(0, 0);
            List<ReportTally> tallies = new List<ReportTally>();
            foreach (string fundingType in SystemSettings.GetNumberedValues(SystemSettings.FundingTypePrefix))
            {
                ReportTally tally = new ReportTally();
                tally.Name = fundingType;
                foreach (DataRow session in sessions.Rows)
                {
                    if (HasFundingOfType(options, fundingType, Convert.ToInt32(session["FacultyID"]), Convert.ToInt32(session["ProgrammeID"])))
                    {
                        // Count sessions offered this type of funding
                        tally.Count++;
                        if (Convert.ToBoolean(session["FundingInfoRequested"]))
                        {
                            tally.AddedCount++;
                        }
                    }
                }
                tally.MostLinkedFaculty = FindMostLinkedFaculty(options, fundingType);
                tallies.Add(tally);
            }

            tallies.Sort(CompareFundingDescending);
            foreach (ReportTally tally in tallies)
            {
                FpDieData.FundingCategoriesRow row = data.FundingCategories.NewFundingCategoriesRow();
                row.FundingType = tally.Name;
                row.OfferedCount = tally.Count;
                row.AddedToSummaryCount = tally.AddedCount;
                if (tally.MostLinkedFaculty.Length == 0)
                {
                    row.SetMostLinkedFacultyNull();
                }
                else
                {
                    row.MostLinkedFaculty = tally.MostLinkedFaculty;
                }
                data.FundingCategories.AddFundingCategoriesRow(row);
            }
        }

        // Add a learner session to the FP-LER exception listing
        private void AddExceptionRow(FpLerData data, DataRow session, int facultySortOrder)
        {
            int sessionID = Convert.ToInt32(session["SessionID"]);
            DateTime sessionDate = Convert.ToDateTime(session["SessionDate"]);
            string outcome = TextOf(session["OutcomeCode"]);

            FpLerData.ExceptionListingRow row = data.ExceptionListing.NewExceptionListingRow();
            row.FacultyID = Convert.ToInt32(session["FacultyID"]);
            row.FacultyName = TextOf(session["FacultyName"]);
            row.FacultySortOrder = facultySortOrder;
            row.SessionID = sessionID;
            row.SessionNumber = AdvisorySession.FormatSessionNumber(sessionDate, sessionID);
            row.SessionDate = sessionDate;
            row.LearnerName = LearnerName(session["FirstName"], session["LastName"]);
            row.Grade = 12;
            if (Convert.ToInt32(session["GradeLevelID"]) == (int)GradeLevel.Grade11)
            {
                row.Grade = 11;
            }
            row.SchoolName = TextOf(session["SchoolName"]);
            string programmeName = TextOf(session["ProgrammeName"]);
            if (programmeName.Length == 0)
            {
                row.SetProgrammeNameNull();
            }
            else
            {
                row.ProgrammeName = programmeName;
            }
            row.OutcomeName = SentenceCase(AdvisoryOutcome.GetCategory(outcome));
            // Count the session under its matching exception outcome
            row.NotEligibleCount = 0;
            row.MoreInfoNeededCount = 0;
            if (outcome == AdvisoryOutcome.CodeNotEligible)
            {
                row.NotEligibleCount = 1;
            }
            if (outcome == AdvisoryOutcome.CodeMoreInformation)
            {
                row.MoreInfoNeededCount = 1;
            }
            row.OutcomeReason = FirstSentence(TextOf(session["OutcomeReason"]));
            string alternative = FindBestAlternative(sessionID);
            if (alternative.Length == 0)
            {
                row.SetAlternativeProgrammeNull();
            }
            else
            {
                row.AlternativeProgramme = alternative;
            }
            data.ExceptionListing.AddExceptionListingRow(row);
        }

        // Group the reasons why a learner needs eligibility follow-up
        private Collection<string> GetReasonCategories(DataRow session, Dictionary<int, DegreeProgramme> programmes)
        {
            Collection<string> reasons = new Collection<string>();
            if (TextOf(session["OutcomeCode"]) == AdvisoryOutcome.CodeMoreInformation)
            {
                reasons.Add(IncompleteReason);
                return reasons;
            }
            MatchResult match = EvaluateSession(Convert.ToInt32(session["SessionID"]), Convert.ToInt32(session["ProgrammeID"]), programmes);
            if (match != null)
            {
                foreach (RequirementCheck check in match.Checks)
                {
                    if (!check.IsRequired)
                    {
                        continue;
                    }
                    if (check.ResultCode == RequirementCheck.ResultMissing)
                    {
                        AddOnce(reasons, check.SubjectName + " not taken");
                    }
                    else if (check.ResultCode == RequirementCheck.ResultGap)
                    {
                        AddOnce(reasons, check.SubjectTakenName + " mark below the minimum");
                    }
                }
                if (!match.AverageMet)
                {
                    AddOnce(reasons, AverageReason);
                }
            }
            if (reasons.Count == 0)
            {
                reasons.Add(OtherReason);
            }
            return reasons;
        }

        // Check a session against the programme's current requirements
        private MatchResult EvaluateSession(int sessionID, int programmeID, Dictionary<int, DegreeProgramme> programmes)
        {
            Learner learner = learnerRepository.FindBySessionId(sessionID);
            if (learner == null || !learner.HasSubjects)
            {
                return null;
            }
            if (!programmes.ContainsKey(programmeID))
            {
                programmes[programmeID] = programmeRepository.FindByIdWithRequirements(programmeID);
            }
            DegreeProgramme programme = programmes[programmeID];
            if (programme == null)
            {
                return null;
            }
            return programme.EvaluateSubjectAndMarkMatch(learner.GetSubjects());
        }

        // Find the highest-ranked active alternative for a session
        private string FindBestAlternative(int sessionID)
        {
            // Results are ordered by selected programme and alternative rank
            foreach (RecommendationResult result in sessionRepository.FindResults(sessionID))
            {
                if (!result.IsSelectedProgramme && reportRepository.IsProgrammeCurrent(result.ProgrammeID))
                {
                    return result.ProgrammeName;
                }
            }
            return "";
        }

        // Check whether funding of a given type is linked to the faculty or programme
        private static bool HasFundingOfType(Collection<FundingOption> options, string fundingType, int facultyID, int programmeID)
        {
            foreach (FundingOption option in options)
            {
                if (option.IsActive && string.Equals(option.FundingType, fundingType, StringComparison.OrdinalIgnoreCase)
                    && option.LinksTo(facultyID, programmeID))
                {
                    return true;
                }
            }
            return false;
        }

        // Find the faculty with the most links to this funding type
        private static string FindMostLinkedFaculty(Collection<FundingOption> options, string fundingType)
        {
            List<ReportTally> faculties = new List<ReportTally>();
            foreach (FundingOption option in options)
            {
                if (!option.IsActive || !string.Equals(option.FundingType, fundingType, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                foreach (FundingFaculty link in option.FacultyLinks)
                {
                    FindOrAddTally(faculties, 0, link.FacultyName).Count++;
                }
            }
            if (faculties.Count == 0)
            {
                return "";
            }
            faculties.Sort(CompareCountsDescending);
            return faculties[0].Name;
        }

        // Count sessions where funding information was requested
        private static int CountFundingRequests(DataTable sessions)
        {
            int count = 0;
            foreach (DataRow session in sessions.Rows)
            {
                if (Convert.ToBoolean(session["FundingInfoRequested"]))
                {
                    count++;
                }
            }
            return count;
        }

        // Count the total missing-subject occurrences in the report
        private static int CountGapOccurrences(FpDieData data)
        {
            int total = 0;
            foreach (FpDieData.SubjectGapsRow row in data.SubjectGaps)
            {
                total += row.GapCount;
            }
            return total;
        }
        #endregion
    }
}
