using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using FuturePath.ApplicationLayer.Reports;
using FuturePath.DatabaseLayer;

namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// ReportController, part 3 of 3: the period reports FP-DIE (Degree Interest and Eligibility,
    /// spec 6.2) and FP-LER (Learners Not Currently Eligible, spec 6.3). Both cover the completed
    /// sessions in a period; subject gaps are worked out with RequirementCheck at report time.
    /// </summary>
    public partial class ReportController
    {
        #region FP-DIE and FP-LER (6.2, 6.3)
        /// <summary>
        /// FP-DIE (staff): completed sessions in the period, optional faculty / grade / outcome
        /// filters, inactive programmes excluded unless includeInactive. No learner names. Logs nothing.
        /// A session's faculty is the faculty of its selected programme. Subject and mark gaps are
        /// worked out at report time from each session's subjects and the programme's current
        /// requirements; sessions with no subjects captured are not counted as gaps.
        /// </summary>
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

            // One-line notes under the charts (sections 2, 4 and 5).
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

        /// <summary>
        /// FP-LER (staff): completed sessions in the period whose outcome is Not Currently Eligible
        /// or More Information Needed, optional faculty / grade filters, with each learner's reason
        /// and best recommended alternative. Logs nothing.
        /// Inactive programmes (and programmes under an inactive faculty) are always left out.
        /// The listing is ordered by faculty name, then Not Currently Eligible before More
        /// Information Needed, then session date, newest first.
        /// </summary>
        public FpLerData BuildNotEligibleReport(int actorUserID, DateTime fromDate, DateTime toDate, int facultyID, int gradeLevelID)
        {
            UserAccount actor = loginController.CheckStaff(actorUserID);
            CheckPeriod(fromDate, toDate);

            // Sessions in scope: every completed session in the period after the faculty and grade filters.
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
                // The rows arrive sorted by faculty, so a new FacultyID starts the next block.
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

            // Reasons and faculties, most frequent first.
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

        /// <summary>FP-DIE section 2 and the key figures: always four rows, Strong, Possible, Not currently eligible, More information needed.</summary>
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

        /// <summary>
        /// FP-DIE section 1 (one row per faculty, busiest first) and section 4 (four slices per
        /// faculty in the same order, fixed outcome order).
        /// </summary>
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

        /// <summary>FP-DIE section 3: one row per selected programme, most selections first.</summary>
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

        /// <summary>
        /// FP-DIE sections 5 and 6, from each session's Required requirement checks:
        /// - SubjectGaps: a Required subject MISSING (not taken, no accepted alternative);
        /// - MarkShortfalls: a Required check that is a GAP (the subject taken is below its minimum),
        ///   with the number of times, the average shortfall in points and the lowest mark.
        /// Also writes onto the header the sessions with at least one of each and the average
        /// shortfall over all GAPs.
        /// </summary>
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
                        // A GAP check always has the mark and the shortfall
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

        /// <summary>
        /// FP-DIE section 7: one row per funding type (the FUNDING_TYPE_n parameters). Definitions:
        /// - OfferedCount: sessions in scope whose selected programme, or that programme's faculty,
        ///   is linked to at least one ACTIVE funding option of the type (what UI 6.2b would list);
        /// - AddedToSummaryCount: of those sessions, the ones with FundingInfoRequested
        ///   (Add to Summary was pressed on UI 6.2b);
        /// - MostLinkedFaculty: the faculty with the most FundingFaculty links among the active
        ///   options of the type (null when the type has no faculty links).
        /// Sorted by OfferedCount, then AddedToSummaryCount, highest first.
        /// </summary>
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
                        // Count = sessions offered this funding type
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

        /// <summary>One FP-LER listing row from an exception session.</summary>
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
            // Each listing row counts towards one of the two exception outcomes
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

        /// <summary>
        /// The FP-LER reason categories for one exception session, each at most once, worked out
        /// from the session's Required requirement checks at report time:
        /// - More Information Needed: "Advisory session incomplete";
        /// - a Required subject not taken: "&lt;Subject&gt; not taken";
        /// - a Required subject below its minimum: "&lt;Subject taken&gt; mark below the minimum";
        /// - the overall average below the programme's minimum: "Overall average below the minimum";
        /// - none of these (the requirements changed after the session): "Other reason (see the listing)".
        /// </summary>
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

        /// <summary>
        /// Checks one session's subjects against its programme's CURRENT requirements (reports
        /// work gaps out at report time). Returns null when the learner captured no subjects.
        /// Each programme is read once and remembered in the dictionary.
        /// </summary>
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

        /// <summary>
        /// The recommended alternative for FP-LER: the best-ranked stored alternative of the session
        /// whose programme is still active (under an active faculty), or "" if there is none.
        /// </summary>
        private string FindBestAlternative(int sessionID)
        {
            // FindResults lists the selected programme first, then the alternatives by rank
            foreach (RecommendationResult result in sessionRepository.FindResults(sessionID))
            {
                if (!result.IsSelectedProgramme && reportRepository.IsProgrammeCurrent(result.ProgrammeID))
                {
                    return result.ProgrammeName;
                }
            }
            return "";
        }

        /// <summary>True if an active option of this type is linked to the faculty or the programme.</summary>
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

        /// <summary>The faculty with the most links to active options of this type ("" if none); ties go to the first name A-Z.</summary>
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

        /// <summary>The number of sessions with FundingInfoRequested.</summary>
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

        /// <summary>All subject-gap occurrences in FP-DIE section 5 (the bars add up to this).</summary>
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
