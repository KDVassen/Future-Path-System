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
        public FpDieData BuildDegreeInterestReport(int actorUserID, DateTime fromDate, DateTime toDate, int? facultyID, int? gradeLevelID, string outcomeCode, bool includeInactive)
        {
            UserAccount actor = loginController.CheckStaff(actorUserID);
            CheckPeriod(fromDate, toDate);
            int? faculty = OptionalId(facultyID);
            int? grade = OptionalId(gradeLevelID);
            string outcome = OptionalOutcome(outcomeCode);

            DataTable sessions = reportRepository.GetCompletedSessions(PeriodStart(fromDate), PeriodEnd(toDate), faculty, grade, outcome, includeInactive);

            FpDieData data = new FpDieData();
            AddOutcomeSummaryRows(data, sessions);
            AddFacultyOutcomeRows(data, sessions);
            AddProgrammeOutcomeRows(data, sessions);
            int subjectGapSessions;
            int markGapSessions;
            decimal? averageShortfall;
            AddGapRows(data, sessions, out subjectGapSessions, out markGapSessions, out averageShortfall);
            AddFundingCategoryRows(data, sessions);

            int sessionCount = sessions.Rows.Count;
            int eligibleCount = CountOutcome(sessions, AdvisoryOutcome.CodeStrong) + CountOutcome(sessions, AdvisoryOutcome.CodePossible);

            FpDieData.ReportHeaderRow header = data.ReportHeader.NewReportHeaderRow();
            header.ReportCode = ExportReport.CodeDegreeInterest;
            header.PeriodStart = fromDate.Date;
            header.PeriodEnd = toDate.Date;
            header.FacultyFilter = faculty.HasValue ? FacultyText(faculty.Value) : "All faculties";
            header.GradeFilter = grade.HasValue ? GradeText(grade.Value) : "All grades";
            header.OutcomeFilter = outcome != null ? SentenceCase(AdvisoryOutcome.GetCategory(outcome)) : "All outcomes";
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
            header.SubjectGapSessions = subjectGapSessions;
            header.MarkGapSessions = markGapSessions;
            if (averageShortfall.HasValue)
            {
                header.AverageShortfall = averageShortfall.Value;
            }
            else
            {
                header.SetAverageShortfallNull();
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
        public FpLerData BuildNotEligibleReport(int actorUserID, DateTime fromDate, DateTime toDate, int? facultyID, int? gradeLevelID)
        {
            UserAccount actor = loginController.CheckStaff(actorUserID);
            CheckPeriod(fromDate, toDate);
            int? faculty = OptionalId(facultyID);
            int? grade = OptionalId(gradeLevelID);

            // Sessions in scope: every completed session in the period after the faculty and grade filters.
            DataTable scope = reportRepository.GetScopeCounts(PeriodStart(fromDate), PeriodEnd(toDate), faculty, grade, null, false, false);
            int sessionsInScope = Convert.ToInt32(scope.Rows[0]["SessionCount"]);
            DataTable exceptions = reportRepository.GetExceptionSessions(PeriodStart(fromDate), PeriodEnd(toDate), faculty, grade);

            FpLerData data = new FpLerData();
            List<OutcomeTally> faculties = new List<OutcomeTally>();
            List<CountTally> reasons = new List<CountTally>();
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

                string facultyName = TextOf(session["FacultyName"]);
                FindOrAddTally(faculties, sessionFacultyID, facultyName, facultyName).Add(TextOf(session["OutcomeCode"]));
                foreach (string reason in GetReasonCategories(session, programmes))
                {
                    FindOrAddCount(reasons, reason).Count++;
                }
            }

            // Reasons and faculties, most frequent first.
            reasons.Sort(CompareCountsDescending);
            foreach (CountTally reason in reasons)
            {
                data.ReasonSummary.AddReasonSummaryRow(reason.Name, reason.Count);
            }
            int exceptionCount = exceptions.Rows.Count;
            faculties.Sort(CompareSessionsDescending);
            foreach (OutcomeTally tally in faculties)
            {
                data.FacultySummary.AddFacultySummaryRow(tally.ID, tally.Name, tally.SessionCount, Percent(tally.SessionCount, exceptionCount));
            }

            FpLerData.ReportHeaderRow header = data.ReportHeader.NewReportHeaderRow();
            header.ReportCode = ExportReport.CodeNotEligible;
            header.PeriodText = FormatPeriod(fromDate, toDate);
            header.ParametersText = "Faculty: " + (faculty.HasValue ? FacultyText(faculty.Value) : "All")
                                    + " \u00B7 Grade: " + (grade.HasValue ? GradeText(grade.Value) : "All");
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
            List<OutcomeTally> faculties = new List<OutcomeTally>();
            foreach (DataRow session in sessions.Rows)
            {
                string facultyName = TextOf(session["FacultyName"]);
                FindOrAddTally(faculties, Convert.ToInt32(session["FacultyID"]), facultyName, facultyName).Add(TextOf(session["OutcomeCode"]));
            }
            faculties.Sort(CompareSessionsDescending);
            foreach (OutcomeTally tally in faculties)
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
            List<OutcomeTally> programmes = new List<OutcomeTally>();
            foreach (DataRow session in sessions.Rows)
            {
                FindOrAddTally(programmes, Convert.ToInt32(session["ProgrammeID"]), TextOf(session["ProgrammeName"]),
                               TextOf(session["FacultyName"])).Add(TextOf(session["OutcomeCode"]));
            }
            programmes.Sort(CompareSessionsDescending);
            foreach (OutcomeTally tally in programmes)
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
        /// Also returns the sessions with at least one of each and the average shortfall over all GAPs.
        /// </summary>
        private void AddGapRows(FpDieData data, DataTable sessions, out int subjectGapSessions, out int markGapSessions, out decimal? averageShortfall)
        {
            List<CountTally> subjectGaps = new List<CountTally>();
            List<CountTally> markGaps = new List<CountTally>();
            Dictionary<int, DegreeProgramme> programmes = new Dictionary<int, DegreeProgramme>();
            subjectGapSessions = 0;
            markGapSessions = 0;
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
                        FindOrAddCount(subjectGaps, check.SubjectName).Count++;
                        hasSubjectGap = true;
                    }
                    else if (check.ResultCode == RequirementCheck.ResultGap)
                    {
                        decimal shortfall = check.Shortfall ?? 0;
                        decimal mark = check.Mark ?? 0;
                        CountTally tally = FindOrAddCount(markGaps, check.SubjectTakenName);
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
            foreach (CountTally tally in subjectGaps)
            {
                gapOccurrences += tally.Count;
            }
            subjectGaps.Sort(CompareCountsDescending);
            foreach (CountTally tally in subjectGaps)
            {
                data.SubjectGaps.AddSubjectGapsRow(tally.Name, tally.Count, Percent(tally.Count, gapOccurrences));
            }
            markGaps.Sort(CompareCountsDescending);
            foreach (CountTally tally in markGaps)
            {
                data.MarkShortfalls.AddMarkShortfallsRow(tally.Name, tally.Count, Math.Round(tally.ShortfallTotal / tally.Count, 1), tally.LowestMark);
            }
            averageShortfall = null;
            if (shortfallCount > 0)
            {
                averageShortfall = Math.Round(shortfallTotal / shortfallCount, 1);
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
            Collection<FundingOption> options = fundingRepository.GetActiveFundingFor(null, null);
            List<FundingTally> tallies = new List<FundingTally>();
            foreach (string fundingType in SystemSettings.GetNumberedValues(SystemSettings.FundingTypePrefix))
            {
                FundingTally tally = new FundingTally();
                tally.FundingType = fundingType;
                foreach (DataRow session in sessions.Rows)
                {
                    if (HasFundingOfType(options, fundingType, Convert.ToInt32(session["FacultyID"]), Convert.ToInt32(session["ProgrammeID"])))
                    {
                        tally.OfferedCount++;
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
            foreach (FundingTally tally in tallies)
            {
                FpDieData.FundingCategoriesRow row = data.FundingCategories.NewFundingCategoriesRow();
                row.FundingType = tally.FundingType;
                row.OfferedCount = tally.OfferedCount;
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
        private static void AddExceptionRow(FpLerData data, DataRow session, int facultySortOrder)
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
            row.Grade = Convert.ToInt32(session["GradeLevelID"]) == (int)GradeLevel.Grade11 ? 11 : 12;
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
            row.NotEligibleCount = outcome == AdvisoryOutcome.CodeNotEligible ? 1 : 0;
            row.MoreInfoNeededCount = outcome == AdvisoryOutcome.CodeMoreInformation ? 1 : 0;
            row.OutcomeReason = FirstSentence(TextOf(session["OutcomeReason"]));
            string alternative = TextOf(session["AlternativeProgramme"]);
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
            DegreeProgramme programme;
            if (!programmes.TryGetValue(programmeID, out programme))
            {
                programme = programmeRepository.FindByIdWithRequirements(programmeID);
                programmes[programmeID] = programme;
            }
            if (programme == null)
            {
                return null;
            }
            return programme.EvaluateSubjectAndMarkMatch(learner.GetSubjects());
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
            List<CountTally> faculties = new List<CountTally>();
            foreach (FundingOption option in options)
            {
                if (!option.IsActive || !string.Equals(option.FundingType, fundingType, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                foreach (FundingFaculty link in option.FacultyLinks)
                {
                    FindOrAddCount(faculties, link.FacultyName).Count++;
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
