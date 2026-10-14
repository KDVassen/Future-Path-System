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
    /// Report generation (spec Section 6): fills the typed datasets FpLsrData, FpDieData and
    /// FpLerData that the RDLC layouts display, and logs every export in ExportReport. The .rdlc
    /// files hold no queries and no rules; every count, rate, shortfall and note is worked out
    /// here. Subject gaps come from DegreeProgramme.EvaluateSubjectAndMarkMatch. Inactive
    /// programmes and funding are excluded, and every report carries its generation date and the
    /// disclaimer. FP-DIE and FP-LER (and staff FP-LSR) need a Career Advisor or Administrator.
    ///
    /// The reporting period: a session is "in the period" when its SessionDate is from
    /// fromDate.Date up to, but not including, toDate.Date.AddDays(1). So both dates are whole
    /// days and a session at 16:30 on the to-date is counted. Only Completed sessions count, each
    /// joined to its selected RecommendationResult (IsSelectedProgramme = 1), whose stored
    /// OutcomeCode is read back and never recalculated.
    /// </summary>
    public class ReportController
    {
        #region Data Members
        /// <summary>
        /// The standard advisory disclaimer (FP-LSR, FP-DIE). The typed datasets have no column for
        /// it, so each .rdlc prints its own copy of this wording; forms can show this constant.
        /// </summary>
        public const string AdvisoryDisclaimer =
            "Future Path gives preliminary guidance only. It is not an admission decision. Programme requirements " +
            "and funding details are locally stored sample data; check them against the official university or " +
            "funder source before relying on them.";

        /// <summary>The FP-LER disclaimer: the standard wording plus what the two exception outcomes do and do not mean.</summary>
        public const string NotEligibleDisclaimer = AdvisoryDisclaimer +
            " Not Currently Eligible is not a refusal and not a statement about any other qualification." +
            " More Information Needed only means the session was incomplete. Grade 11 marks are projected and will change.";

        // FP-LSR status vocabulary for ResultCode / AverageResultCode (FpLsrData.xsd).
        private const string ResultMet = "Met";
        private const string ResultGap = "Gap";
        private const string ResultNotAssessed = "NotAssessed";

        private const string OverallAverageLabel = "Overall average";
        private const string IncompleteReason = "Advisory session incomplete";
        private const string AverageReason = "Overall average below the minimum";
        private const string OtherReason = "Other reason (see the listing)";
        private const string UnknownReportMessage = "Choose a report: FP-LSR, FP-DIE or FP-LER.";
        private const int ParametersMaxLength = 255;

        private ReportRepository reportRepository;
        private ExportReportRepository exportRepository;
        private LearnerRepository learnerRepository;
        private DegreeProgrammeRepository programmeRepository;
        private AdvisorySessionRepository sessionRepository;
        private FundingRepository fundingRepository;
        private InterestRepository interestRepository;
        private LoginController loginController;
        #endregion

        #region Constructor
        /// <summary>Creates the controller and its repositories (no database work yet).</summary>
        public ReportController()
        {
            reportRepository = new ReportRepository();
            exportRepository = new ExportReportRepository();
            learnerRepository = new LearnerRepository();
            programmeRepository = new DegreeProgrammeRepository();
            sessionRepository = new AdvisorySessionRepository();
            fundingRepository = new FundingRepository();
            interestRepository = new InterestRepository();
            loginController = new LoginController();
        }
        #endregion

        #region Report catalogue
        /// <summary>The report's name, e.g. "FP-LER" gives "Learners Not Currently Eligible".</summary>
        public string GetReportName(string reportCode)
        {
            string reportName;
            string scope;
            string[] formats;
            LookUpReport(NormaliseCode(reportCode), out reportName, out scope, out formats);
            return reportName;
        }

        /// <summary>
        /// The export formats a report allows (FP-LSR and FP-LER: PDF, Word; FP-DIE: PDF, Excel):
        /// ExportFormat (file extension ".pdf" / ".docx" / ".xlsx"), FormatName, FormatCode
        /// (stored value PDF / DOCX / XLSX) and RenderFormat (ReportViewer: PDF / WORDOPENXML / EXCELOPENXML).
        /// An unknown report code gives an empty table.
        /// </summary>
        public DataTable GetExportFormats(string reportCode)
        {
            DataTable table = new DataTable("ExportFormats");
            table.Columns.Add("ExportFormat", typeof(string));
            table.Columns.Add("FormatName", typeof(string));
            table.Columns.Add("FormatCode", typeof(string));
            table.Columns.Add("RenderFormat", typeof(string));

            string reportName;
            string scope;
            string[] formats;
            LookUpReport(NormaliseCode(reportCode), out reportName, out scope, out formats);
            foreach (string formatCode in formats)
            {
                string extension;
                string formatName;
                string renderFormat;
                LookUpFormat(formatCode, out extension, out formatName, out renderFormat);
                table.Rows.Add(extension, formatName, formatCode, renderFormat);
            }
            return table;
        }

        /// <summary>
        /// When the report was last generated (from ExportReport), or null. It takes no user ID
        /// (fixed signature), so it has no role check; it returns only a date, never learner data.
        /// </summary>
        public DateTime? GetLastGenerated(string reportCode)
        {
            string code = NormaliseCode(reportCode);
            if (!ExportReport.IsValidReportCode(code))
            {
                return null;
            }
            return exportRepository.GetLastGenerated(code);
        }

        /// <summary>
        /// UI 10.3c shortcuts (staff): ReportCode, ReportName, Scope, RowsInScope, LastGenerated for
        /// the three reports over the period and filters. Completed sessions only; FP-LER counts
        /// only Not Currently Eligible and More Information Needed.
        /// LastGenerated is DBNull when the report was never generated. FP-LER ignores the outcome
        /// filter (it always lists both exception outcomes), and inactive programmes are excluded.
        /// </summary>
        public DataTable GetReportCatalogue(int actorUserID, DateTime fromDate, DateTime toDate, int? facultyID, int? gradeLevelID, string outcomeCode)
        {
            loginController.CheckStaff(actorUserID);
            CheckPeriod(fromDate, toDate);
            int? faculty = OptionalId(facultyID);
            int? grade = OptionalId(gradeLevelID);
            string outcome = OptionalOutcome(outcomeCode);

            DataTable catalogue = new DataTable("ReportCatalogue");
            catalogue.Columns.Add("ReportCode", typeof(string));
            catalogue.Columns.Add("ReportName", typeof(string));
            catalogue.Columns.Add("Scope", typeof(string));
            catalogue.Columns.Add("RowsInScope", typeof(int));
            catalogue.Columns.Add("LastGenerated", typeof(DateTime));

            string[] codes = { ExportReport.CodeLearnerSummary, ExportReport.CodeDegreeInterest, ExportReport.CodeNotEligible };
            foreach (string code in codes)
            {
                string reportName;
                string scope;
                string[] formats;
                LookUpReport(code, out reportName, out scope, out formats);

                int sessionCount;
                int facultyCount;
                int programmeCount;
                CountScope(code, fromDate, toDate, faculty, grade, outcome, false, out sessionCount, out facultyCount, out programmeCount);
                DateTime? lastGenerated = exportRepository.GetLastGenerated(code);

                DataRow row = catalogue.NewRow();
                row["ReportCode"] = code;
                row["ReportName"] = reportName;
                row["Scope"] = scope;
                row["RowsInScope"] = sessionCount;
                row["LastGenerated"] = lastGenerated.HasValue ? (object)lastGenerated.Value : DBNull.Value;
                catalogue.Rows.Add(row);
            }
            return catalogue;
        }

        /// <summary>
        /// UI 11 readiness panel (staff): sessions, faculties and programmes in scope for a report request.
        /// FP-LER always counts only its two exception outcomes (the outcome filter is ignored) and
        /// never includes inactive programmes; only FP-DIE honours includeInactive.
        /// </summary>
        public void GetScope(int actorUserID, string reportCode, DateTime fromDate, DateTime toDate, int? facultyID, int? gradeLevelID,
                             string outcomeCode, bool includeInactive, out int sessionCount, out int facultyCount, out int programmeCount)
        {
            loginController.CheckStaff(actorUserID);
            string code = NormaliseCode(reportCode);
            if (!ExportReport.IsValidReportCode(code))
            {
                throw new ArgumentException(UnknownReportMessage);
            }
            CheckPeriod(fromDate, toDate);
            CountScope(code, fromDate, toDate, OptionalId(facultyID), OptionalId(gradeLevelID), OptionalOutcome(outcomeCode),
                       includeInactive, out sessionCount, out facultyCount, out programmeCount);
        }

        /// <summary>
        /// Counts the completed sessions (and their faculties and programmes) a report would cover.
        /// FP-LER counts only its two exception outcomes and ignores the outcome filter; only
        /// FP-DIE may include inactive programmes.
        /// </summary>
        private void CountScope(string reportCode, DateTime fromDate, DateTime toDate, int? facultyID, int? gradeLevelID, string outcomeCode,
                                bool includeInactive, out int sessionCount, out int facultyCount, out int programmeCount)
        {
            bool exceptionsOnly = reportCode == ExportReport.CodeNotEligible;
            string outcomeFilter = exceptionsOnly ? null : outcomeCode;
            bool inactiveIncluded = includeInactive && reportCode == ExportReport.CodeDegreeInterest;
            DataTable counts = reportRepository.GetScopeCounts(PeriodStart(fromDate), PeriodEnd(toDate), facultyID, gradeLevelID,
                                                               outcomeFilter, inactiveIncluded, exceptionsOnly);
            sessionCount = Convert.ToInt32(counts.Rows[0]["SessionCount"]);
            facultyCount = Convert.ToInt32(counts.Rows[0]["FacultyCount"]);
            programmeCount = Convert.ToInt32(counts.Rows[0]["ProgrammeCount"]);
        }

        /// <summary>
        /// Checks a UI 11 request: a known report code, from-date not after to-date, and a format
        /// the report allows. Returns one message per problem. The format may be given as the
        /// stored code (PDF) or the file extension (.pdf).
        /// </summary>
        public Collection<string> ValidateReportRequest(string reportCode, DateTime fromDate, DateTime toDate, string exportFormat)
        {
            Collection<string> messages = new Collection<string>();
            string reportName;
            string scope;
            string[] formats;
            bool knownReport = LookUpReport(NormaliseCode(reportCode), out reportName, out scope, out formats);
            if (!knownReport)
            {
                messages.Add(UnknownReportMessage);
            }
            if (fromDate.Date > toDate.Date)
            {
                messages.Add("Sessions from must be on or before Sessions to.");
            }
            string format = NormaliseFormat(exportFormat);
            if (format.Length == 0)
            {
                messages.Add("Choose an output format.");
            }
            else if (knownReport && !AllowsFormat(formats, format))
            {
                messages.Add(reportName + " can only be exported as " + DescribeFormats(formats) + ".");
            }
            return messages;
        }

        /// <summary>
        /// The ExportReport.ParametersUsed text for FP-DIE / FP-LER, e.g.
        /// "Period 2026-01-01..2026-09-15; Faculty=All; Grade=All; Outcome=All; Inactive=Excluded" (max 255).
        /// Names are printed, not IDs. FP-LER always shows its two outcomes and Inactive=Excluded.
        /// FP-LSR (and an unknown code) returns null, because an FP-LSR row stores its SessionID instead.
        /// </summary>
        public string BuildParametersUsed(string reportCode, DateTime fromDate, DateTime toDate, int? facultyID, int? gradeLevelID, string outcomeCode, bool includeInactive)
        {
            string code = NormaliseCode(reportCode);
            if (code != ExportReport.CodeDegreeInterest && code != ExportReport.CodeNotEligible)
            {
                return null;
            }
            int? faculty = OptionalId(facultyID);
            int? grade = OptionalId(gradeLevelID);

            string outcomeText;
            if (code == ExportReport.CodeNotEligible)
            {
                outcomeText = AdvisoryOutcome.GetCategory(AdvisoryOutcome.CodeNotEligible) + " or "
                              + AdvisoryOutcome.GetCategory(AdvisoryOutcome.CodeMoreInformation);
            }
            else
            {
                string outcome = OptionalOutcome(outcomeCode);
                outcomeText = outcome == null ? "All" : AdvisoryOutcome.GetCategory(outcome);
            }
            bool inactiveIncluded = code == ExportReport.CodeDegreeInterest && includeInactive;

            string text = "Period " + fromDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                          + ".." + toDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                          + "; Faculty=" + (faculty.HasValue ? FacultyText(faculty.Value) : "All")
                          + "; Grade=" + (grade.HasValue ? GradeText(grade.Value) : "All")
                          + "; Outcome=" + outcomeText
                          + "; Inactive=" + (inactiveIncluded ? "Included" : "Excluded");
            return Truncate(text, ParametersMaxLength);
        }
        #endregion

        #region FP-LSR - Learner Advisory Summary (6.1)
        /// <summary>
        /// The learner's own FP-LSR for UI 7 (no login; the session may still be in progress).
        /// Fills ReportHeader, SubjectRequirements, MarkComparison, InterestProfile, Alternatives
        /// (only if AlternativesInSummary) and FundingOptions (only if FundingInfoRequested). The
        /// outcome and alternative fit are read back from the stored results, not recalculated.
        /// Logs nothing. Throws InvalidOperationException if the eligibility check has not been run.
        /// </summary>
        public FpLsrData GetLearnerSummary(int sessionID)
        {
            AdvisorySession session = sessionRepository.FindById(sessionID);
            if (session == null)
            {
                throw new InvalidOperationException("This advisory session could not be found. Please start a new session.");
            }
            return BuildLearnerSummary(session);
        }

        /// <summary>FP-LSR regenerated by staff (Career Advisor or Administrator): Completed sessions only. Logs nothing.</summary>
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

        /// <summary>
        /// UI 7 Save and export: marks the session Completed (DateCompleted = generatedOn) and logs
        /// an FP-LSR ExportReport row with GeneratedByUserID null. exportFormat is PDF or DOCX.
        /// Refused (InvalidOperationException) when the eligibility check has not been run. A
        /// second export of an already completed session keeps its first completion date.
        /// </summary>
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

        /// <summary>
        /// Fills FpLsrData for one session. The session's subjects are checked against the selected
        /// programme's requirements (DegreeProgramme.EvaluateSubjectAndMarkMatch) to show each
        /// gap; the outcome and its reason are read back from the stored selected result.
        /// </summary>
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

        /// <summary>FP-LSR ReportHeader: masthead, identity line (no contact details), outcome, counts and guidance.</summary>
        private void AddSummaryHeader(FpLsrData data, AdvisorySession session, Learner learner, DegreeProgramme programme,
                                      RecommendationResult selected, MatchResult match)
        {
            FpLsrData.ReportHeaderRow header = data.ReportHeader.NewReportHeaderRow();
            header.ReportCode = ExportReport.CodeLearnerSummary;
            header.SessionNumber = session.SessionNumber;
            header.SessionDate = session.SessionDate;
            header.DateGenerated = DateTime.Now;

            // Name only: contact details are never printed on FP-LSR.
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

            string gradeDescription;
            string markBasis;
            ReadGradeLevel(session.GradeLevelID, out gradeDescription, out markBasis);
            header.GradeDescription = gradeDescription;
            header.MarkBasis = markBasis;
            header.SchoolName = learner.SchoolName ?? "";
            header.Province = learner.Province ?? "";
            header.SubjectCount = learner.GetSubjects().Count;
            if (match.OverallAverage.HasValue)
            {
                header.OverallAverage = match.OverallAverage.Value;
            }
            else
            {
                header.SetOverallAverageNull();
            }

            header.ProgrammeName = !string.IsNullOrEmpty(programme.ProgrammeName) ? programme.ProgrammeName : (selected.ProgrammeName ?? "");
            header.FacultyName = !string.IsNullOrEmpty(programme.FacultyName) ? programme.FacultyName : (selected.FacultyName ?? "");
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

            // Requirements checked and met: every active requirement (Required and Recommended)
            // plus the overall-average requirement when the programme sets one (FpLsrData.xsd).
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

            // The outcome and its reason are READ BACK from the stored result, never recalculated.
            header.OutcomeCode = selected.OutcomeCode ?? "";
            header.OutcomeReason = selected.OutcomeReason ?? "";
            header.QuestionsAnswered = learner.GetResponses().Count;
            header.OverallGuidance = BuildGuidance(selected.OutcomeCode, header.ProgrammeName, match, session.Grade);
            header.IncludeAlternatives = session.AlternativesInSummary;
            header.IncludeFunding = session.FundingInfoRequested;
            data.ReportHeader.AddReportHeaderRow(header);
        }

        /// <summary>
        /// Table 1 and the subject bars of Chart 1, by subject name A-Z: one row per captured
        /// subject (merged with its requirement check) plus one row per Required subject that was
        /// not taken (Mark null, ResultCode Gap, no shortfall).
        /// </summary>
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

        /// <summary>
        /// One Table 1 row for a captured subject. With no requirement for it the row is
        /// NotAssessed; otherwise Met (MET or ALT) or Gap with the shortfall in points.
        /// </summary>
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
                row.ResultCode = check.IsMet ? ResultMet : ResultGap;
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

            // Chart 1 shows only the subjects that have a stored minimum.
            if (check != null && check.MinimumMark.HasValue)
            {
                AddMarkComparisonRow(data, subject.SubjectName, subject.Mark, check.MinimumMark, row.ResultCode, false);
            }
        }

        /// <summary>One Table 1 row (and Chart 1 bar, if it has a minimum) for a Required subject that was not taken.</summary>
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

        /// <summary>One Chart 1 row: a subject (or the overall average) with the learner's mark and the stored minimum.</summary>
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

        /// <summary>
        /// Chart 2: each ACTIVE interest category the learner answered, with
        /// Learner.CalculateCategoryScore as the percentage, highest first.
        /// </summary>
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

        /// <summary>
        /// Table 2: the session's stored alternatives in rank order. The fit is the stored
        /// OverallScore, never recalculated. Programmes that are now inactive (or under an inactive
        /// faculty), and alternatives with no stored score, are left out.
        /// </summary>
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
                row.ProgrammeName = result.ProgrammeName ?? "";
                row.FacultyName = result.FacultyName ?? "";
                row.OverallScore = result.OverallScore.Value;
                row.OutcomeCode = result.OutcomeCode ?? "";
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

        /// <summary>
        /// Table 3: active funding linked to the learner's preferred faculty or selected programme,
        /// nearest closing date first (open-ended last). Funding whose closing date has passed
        /// is expired and left out, as are inactive options.
        /// </summary>
        private void AddFundingRows(FpLsrData data, AdvisorySession session)
        {
            foreach (FundingOption option in fundingRepository.GetActiveFundingFor(session.PreferredFacultyID, session.SelectedProgrammeID))
            {
                if (!option.IsActive || !option.IsOpen)
                {
                    continue;
                }
                FpLsrData.FundingOptionsRow row = data.FundingOptions.NewFundingOptionsRow();
                row.FundingName = option.FundingName ?? "";
                row.FundingType = option.FundingType ?? "";
                row.LinkedTo = option.LinkedTo.Length > 0 ? option.LinkedTo : "All faculties";
                row.EligibilityDescription = option.EligibilityDescription ?? "";
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

        /// <summary>
        /// The overall average against the programme's minimum: NotAssessed when the programme
        /// sets no minimum or no subject was captured, otherwise Met or Gap.
        /// </summary>
        private static string GetAverageResultCode(MatchResult match)
        {
            if (!match.MinimumOverallAverage.HasValue || !match.OverallAverage.HasValue)
            {
                return ResultNotAssessed;
            }
            return match.AverageMet ? ResultMet : ResultGap;
        }

        /// <summary>
        /// The "Overall guidance" paragraph: plain next steps that follow from the stored outcome
        /// and the gaps found. Grade 11 learners are reminded that their marks are projected.
        /// </summary>
        private static string BuildGuidance(string outcomeCode, string programmeName, MatchResult match, GradeLevel grade)
        {
            string gaps = JoinSentences(match.GapList);
            string guidance;
            switch ((outcomeCode ?? "").Trim().ToUpperInvariant())
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
                    guidance = "You do not currently meet one or more stored requirements for " + programmeName + "."
                               + (gaps.Length > 0 ? " " + gaps : "")
                               + " Talk to your teacher or a Career Advisor about your subject choices and marks, and look at the "
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

        /// <summary>Reads the grade's description and mark basis (e.g. "Grade 11", "Projected"); "" if it is not found.</summary>
        private void ReadGradeLevel(int gradeLevelID, out string gradeDescription, out string markBasis)
        {
            gradeDescription = "";
            markBasis = "";
            DataTable grades = reportRepository.GetGradeLevel(gradeLevelID);
            if (grades.Rows.Count > 0)
            {
                gradeDescription = TextOf(grades.Rows[0]["GradeDescription"]);
                markBasis = TextOf(grades.Rows[0]["MarkBasis"]);
            }
        }

        /// <summary>Sets Table 1's AdvisoryNote, or null when the requirement has none.</summary>
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

        /// <summary>True for a Required requirement whose subject (and any alternative) was not captured.</summary>
        private static bool IsMissingRequired(RequirementCheck check)
        {
            return check.IsRequired && check.ResultCode == RequirementCheck.ResultMissing;
        }

        /// <summary>The captured subject with this name, or null.</summary>
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

        /// <summary>The requirement check that used this captured subject (SubjectTakenName), or null if none did.</summary>
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

        /// <summary>The Required check for this subject that found it missing.</summary>
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

        #region Export log
        /// <summary>
        /// Logs a staff export (Career Advisor or Administrator) in ExportReport. For FP-LSR give
        /// the sessionID and null parameters; for FP-DIE / FP-LER give null sessionID and the
        /// parameters text. exportFormat is PDF, DOCX or XLSX.
        /// FP-LSR needs an existing, completed session and stores no parameters; FP-DIE and
        /// FP-LER store no session and at most 255 characters of parameters. The format must be
        /// one the report allows (".pdf" is read as PDF).
        /// </summary>
        public void RecordExport(int actorUserID, string reportCode, int? sessionID, string parametersUsed, string exportFormat, DateTime generatedAt)
        {
            loginController.CheckStaff(actorUserID);
            string code = NormaliseCode(reportCode);
            if (!ExportReport.IsValidReportCode(code))
            {
                throw new ArgumentException(UnknownReportMessage);
            }
            string format = CheckFormat(code, exportFormat);

            ExportReport entry;
            if (code == ExportReport.CodeLearnerSummary)
            {
                if (!sessionID.HasValue || sessionID.Value <= 0)
                {
                    throw new ArgumentException("Choose the advisory session whose summary was exported.");
                }
                string status = reportRepository.GetSessionStatus(sessionID.Value);
                if (status.Length == 0)
                {
                    throw new ArgumentException("That advisory session could not be found. Search for it again.");
                }
                if (status != AdvisorySession.StatusCompleted)
                {
                    throw new InvalidOperationException("An advisory summary can only be exported for a completed session.");
                }
                entry = new ExportReport(code, sessionID, null, actorUserID, generatedAt, format);
            }
            else
            {
                entry = new ExportReport(code, null, Truncate(parametersUsed, ParametersMaxLength), actorUserID, generatedAt, format);
            }
            exportRepository.Insert(entry);
        }
        #endregion

        #region Catalogue and format helpers
        /// <summary>
        /// The report catalogue: each report code's name, what it covers (UI 10.3c "Scope") and
        /// the stored formats it may be exported in. Returns false (and empty values) for an unknown code.
        /// </summary>
        private static bool LookUpReport(string reportCode, out string reportName, out string scope, out string[] formats)
        {
            switch (reportCode)
            {
                case ExportReport.CodeLearnerSummary:
                    reportName = "Learner Advisory Summary";
                    scope = "One summary per completed advisory session";
                    formats = new string[] { ExportReport.FormatPdf, ExportReport.FormatWord };
                    return true;
                case ExportReport.CodeDegreeInterest:
                    reportName = "Degree Interest and Eligibility";
                    scope = "Completed advisory sessions in the period";
                    formats = new string[] { ExportReport.FormatPdf, ExportReport.FormatExcel };
                    return true;
                case ExportReport.CodeNotEligible:
                    reportName = "Learners Not Currently Eligible";
                    scope = "Completed sessions that are Not Currently Eligible or need more information";
                    formats = new string[] { ExportReport.FormatPdf, ExportReport.FormatWord };
                    return true;
                default:
                    reportName = "";
                    scope = "";
                    formats = new string[0];
                    return false;
            }
        }

        /// <summary>A stored format's file extension, display name and ReportViewer render format.</summary>
        private static void LookUpFormat(string formatCode, out string extension, out string formatName, out string renderFormat)
        {
            switch (formatCode)
            {
                case ExportReport.FormatPdf:
                    extension = ".pdf";
                    formatName = "PDF document (.pdf)";
                    renderFormat = "PDF";
                    break;
                case ExportReport.FormatWord:
                    extension = ".docx";
                    formatName = "Word document (.docx)";
                    renderFormat = "WORDOPENXML";
                    break;
                case ExportReport.FormatExcel:
                    extension = ".xlsx";
                    formatName = "Excel workbook (.xlsx)";
                    renderFormat = "EXCELOPENXML";
                    break;
                default:
                    extension = "";
                    formatName = "";
                    renderFormat = "";
                    break;
            }
        }

        /// <summary>
        /// Returns the stored format code (PDF / DOCX / XLSX) if the report allows it, otherwise
        /// throws ArgumentException naming the formats it does allow.
        /// </summary>
        private static string CheckFormat(string reportCode, string exportFormat)
        {
            string reportName;
            string scope;
            string[] formats;
            LookUpReport(reportCode, out reportName, out scope, out formats);
            string format = NormaliseFormat(exportFormat);
            if (!AllowsFormat(formats, format))
            {
                throw new ArgumentException(reportName + " can only be exported as " + DescribeFormats(formats) + ".");
            }
            return format;
        }

        /// <summary>True if the format is in the list.</summary>
        private static bool AllowsFormat(string[] formats, string format)
        {
            foreach (string allowed in formats)
            {
                if (allowed == format)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>The formats as words, e.g. "PDF document (.pdf) or Word document (.docx)".</summary>
        private static string DescribeFormats(string[] formats)
        {
            string text = "";
            for (int i = 0; i < formats.Length; i++)
            {
                string extension;
                string formatName;
                string renderFormat;
                LookUpFormat(formats[i], out extension, out formatName, out renderFormat);
                if (i > 0)
                {
                    text += i == formats.Length - 1 ? " or " : ", ";
                }
                text += formatName;
            }
            return text;
        }

        /// <summary>A report code as stored: trimmed and upper case ("fp-die " gives "FP-DIE").</summary>
        private static string NormaliseCode(string reportCode)
        {
            return (reportCode ?? "").Trim().ToUpperInvariant();
        }

        /// <summary>A format as stored: ".pdf", "pdf" and "PDF" all give "PDF".</summary>
        private static string NormaliseFormat(string exportFormat)
        {
            return (exportFormat ?? "").Trim().TrimStart('.').ToUpperInvariant();
        }
        #endregion

        #region Period and filter helpers
        /// <summary>Throws ArgumentException when the from-date is after the to-date (validated again in the BLL).</summary>
        private static void CheckPeriod(DateTime fromDate, DateTime toDate)
        {
            if (fromDate.Date > toDate.Date)
            {
                throw new ArgumentException("Sessions from must be on or before Sessions to.");
            }
        }

        /// <summary>The first moment of the period: the start of fromDate.</summary>
        private static DateTime PeriodStart(DateTime fromDate)
        {
            return fromDate.Date;
        }

        /// <summary>The end of the period, not included: the start of the day after toDate.</summary>
        private static DateTime PeriodEnd(DateTime toDate)
        {
            return toDate.Date.AddDays(1);
        }

        /// <summary>An optional ID filter: null or 0 (the "All" row) means no filter.</summary>
        private static int? OptionalId(int? id)
        {
            if (!id.HasValue || id.Value <= 0)
            {
                return null;
            }
            return id;
        }

        /// <summary>
        /// An optional outcome filter: blank (the "All" row) means no filter; otherwise one of the
        /// four stored codes, or ArgumentException.
        /// </summary>
        private static string OptionalOutcome(string outcomeCode)
        {
            if (string.IsNullOrWhiteSpace(outcomeCode))
            {
                return null;
            }
            if (!AdvisoryOutcome.IsValidCode(outcomeCode))
            {
                throw new ArgumentException("Choose an advisory outcome from the list.");
            }
            return outcomeCode.Trim().ToUpperInvariant();
        }

        /// <summary>The faculty's name for a parameter line, or "Unknown faculty".</summary>
        private string FacultyText(int facultyID)
        {
            string name = reportRepository.GetFacultyName(facultyID);
            return name.Length > 0 ? name : "Unknown faculty";
        }

        /// <summary>The grade's description for a parameter line (e.g. "Grade 12"), or "Unknown grade".</summary>
        private string GradeText(int gradeLevelID)
        {
            string gradeDescription;
            string markBasis;
            ReadGradeLevel(gradeLevelID, out gradeDescription, out markBasis);
            return gradeDescription.Length > 0 ? gradeDescription : "Unknown grade";
        }

        /// <summary>The FP-LER period line, e.g. "1 January - 15 September 2026" with an en dash (the year once when both dates share it).</summary>
        private static string FormatPeriod(DateTime fromDate, DateTime toDate)
        {
            string start = fromDate.Year == toDate.Year
                ? fromDate.ToString("d MMMM", CultureInfo.InvariantCulture)
                : fromDate.ToString("d MMMM yyyy", CultureInfo.InvariantCulture);
            return start + " \u2013 " + toDate.ToString("d MMMM yyyy", CultureInfo.InvariantCulture);
        }
        #endregion

        #region Counting and text helpers
        /// <summary>part as a percentage of whole, one decimal (0 when whole is 0).</summary>
        private static decimal Percent(int part, int whole)
        {
            if (whole == 0)
            {
                return 0;
            }
            return Math.Round(100m * part / whole, 1);
        }

        /// <summary>How many rows have this stored OutcomeCode.</summary>
        private static int CountOutcome(DataTable sessions, string outcomeCode)
        {
            int count = 0;
            foreach (DataRow session in sessions.Rows)
            {
                if (TextOf(session["OutcomeCode"]) == outcomeCode)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>How many different values an ID column holds (e.g. distinct learners).</summary>
        private static int CountDistinct(DataTable rows, string column)
        {
            List<int> seen = new List<int>();
            foreach (DataRow row in rows.Rows)
            {
                int value = Convert.ToInt32(row[column]);
                if (!seen.Contains(value))
                {
                    seen.Add(value);
                }
            }
            return seen.Count;
        }

        /// <summary>The tally for this ID, added (with zero counts) if it is not in the list yet.</summary>
        private static OutcomeTally FindOrAddTally(List<OutcomeTally> tallies, int id, string name, string facultyName)
        {
            foreach (OutcomeTally tally in tallies)
            {
                if (tally.ID == id)
                {
                    return tally;
                }
            }
            OutcomeTally added = new OutcomeTally();
            added.ID = id;
            added.Name = name;
            added.FacultyName = facultyName;
            tallies.Add(added);
            return added;
        }

        /// <summary>The count for this name, added (at zero) if it is not in the list yet.</summary>
        private static CountTally FindOrAddCount(List<CountTally> tallies, string name)
        {
            foreach (CountTally tally in tallies)
            {
                if (tally.Name == name)
                {
                    return tally;
                }
            }
            CountTally added = new CountTally();
            added.Name = name;
            tallies.Add(added);
            return added;
        }

        /// <summary>Adds the text unless the collection already has it.</summary>
        private static void AddOnce(Collection<string> items, string text)
        {
            if (!items.Contains(text))
            {
                items.Add(text);
            }
        }

        /// <summary>Sort order: most sessions first, then name A-Z.</summary>
        private static int CompareSessionsDescending(OutcomeTally a, OutcomeTally b)
        {
            if (a.SessionCount != b.SessionCount)
            {
                return b.SessionCount.CompareTo(a.SessionCount);
            }
            return string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>Sort order: highest count first, then name A-Z.</summary>
        private static int CompareCountsDescending(CountTally a, CountTally b)
        {
            if (a.Count != b.Count)
            {
                return b.Count.CompareTo(a.Count);
            }
            return string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>Sort order: most sessions offered first, then most added to a summary, then type A-Z.</summary>
        private static int CompareFundingDescending(FundingTally a, FundingTally b)
        {
            if (a.OfferedCount != b.OfferedCount)
            {
                return b.OfferedCount.CompareTo(a.OfferedCount);
            }
            if (a.AddedCount != b.AddedCount)
            {
                return b.AddedCount.CompareTo(a.AddedCount);
            }
            return string.Compare(a.FundingType, b.FundingType, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>Sort order: highest interest score first, then category name A-Z.</summary>
        private static int CompareScoresDescending(KeyValuePair<string, decimal> a, KeyValuePair<string, decimal> b)
        {
            if (a.Value != b.Value)
            {
                return b.Value.CompareTo(a.Value);
            }
            return string.Compare(a.Key, b.Key, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>Outcome words in sentence case for the reports: "Not Currently Eligible" gives "Not currently eligible".</summary>
        private static string SentenceCase(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }
            return text.Substring(0, 1) + text.Substring(1).ToLowerInvariant();
        }

        /// <summary>"First Last" for the FP-LER listing; either part may be blank, both blank gives "Name not given".</summary>
        private static string LearnerName(object firstName, object lastName)
        {
            string name = (TextOf(firstName) + " " + TextOf(lastName)).Trim();
            return name.Length > 0 ? name : "Name not given";
        }

        /// <summary>The short form of a stored reason for the FP-LER listing: its first sentence.</summary>
        private static string FirstSentence(string text)
        {
            int end = text.IndexOf(". ", StringComparison.Ordinal);
            if (end < 0)
            {
                return text;
            }
            return text.Substring(0, end + 1);
        }

        /// <summary>Joins sentences with a space, e.g. the gap list for the overall guidance.</summary>
        private static string JoinSentences(Collection<string> sentences)
        {
            string text = "";
            foreach (string sentence in sentences)
            {
                text += (text.Length > 0 ? " " : "") + sentence;
            }
            return text;
        }

        /// <summary>Text from a DataRow value: DBNull or null gives "", otherwise trimmed.</summary>
        private static string TextOf(object value)
        {
            if (value == null || value is DBNull)
            {
                return "";
            }
            return Convert.ToString(value, CultureInfo.InvariantCulture).Trim();
        }

        /// <summary>A number without trailing zeros: 66.7, 50.</summary>
        private static string FormatNumber(decimal value)
        {
            return value.ToString("0.#", CultureInfo.InvariantCulture);
        }

        /// <summary>Trimmed text cut to maxLength characters; blank gives null.</summary>
        private static string Truncate(string text, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }
            string trimmed = text.Trim();
            return trimmed.Length > maxLength ? trimmed.Substring(0, maxLength) : trimmed;
        }
        #endregion

        #region Tally classes (counting helpers for FP-DIE and FP-LER)
        /// <summary>The four outcome counts for one group of sessions: a faculty or a programme.</summary>
        private class OutcomeTally
        {
            /// <summary>FacultyID or ProgrammeID.</summary>
            public int ID;
            /// <summary>The faculty or programme name.</summary>
            public string Name;
            /// <summary>The faculty name (the programme's faculty for a programme tally).</summary>
            public string FacultyName;
            /// <summary>Strong Match sessions.</summary>
            public int StrongCount;
            /// <summary>Possible Match sessions.</summary>
            public int PossibleCount;
            /// <summary>Not Currently Eligible sessions.</summary>
            public int NotEligibleCount;
            /// <summary>More Information Needed sessions.</summary>
            public int MoreInfoCount;

            /// <summary>All sessions in the group.</summary>
            public int SessionCount
            {
                get { return StrongCount + PossibleCount + NotEligibleCount + MoreInfoCount; }
            }

            /// <summary>(Strong + Possible) / sessions x 100, one decimal.</summary>
            public decimal EligibilityRate
            {
                get { return Percent(StrongCount + PossibleCount, SessionCount); }
            }

            /// <summary>Counts one session with this stored outcome code.</summary>
            public void Add(string outcomeCode)
            {
                switch (outcomeCode)
                {
                    case AdvisoryOutcome.CodeStrong:
                        StrongCount++;
                        break;
                    case AdvisoryOutcome.CodePossible:
                        PossibleCount++;
                        break;
                    case AdvisoryOutcome.CodeNotEligible:
                        NotEligibleCount++;
                        break;
                    default:
                        MoreInfoCount++;
                        break;
                }
            }

            /// <summary>The count for one outcome code.</summary>
            public int CountFor(string outcomeCode)
            {
                switch (outcomeCode)
                {
                    case AdvisoryOutcome.CodeStrong:
                        return StrongCount;
                    case AdvisoryOutcome.CodePossible:
                        return PossibleCount;
                    case AdvisoryOutcome.CodeNotEligible:
                        return NotEligibleCount;
                    default:
                        return MoreInfoCount;
                }
            }
        }

        /// <summary>A count for one subject or reason; for marks below the minimum also the shortfall total and the lowest mark.</summary>
        private class CountTally
        {
            /// <summary>The subject, reason or faculty name.</summary>
            public string Name;
            /// <summary>How many times it occurred.</summary>
            public int Count;
            /// <summary>The sum of the shortfalls in points (mark gaps only).</summary>
            public decimal ShortfallTotal;
            /// <summary>The lowest mark seen (mark gaps only).</summary>
            public decimal LowestMark;
        }

        /// <summary>The FP-DIE section 7 figures for one funding type.</summary>
        private class FundingTally
        {
            /// <summary>Scholarship, Bursary, Financial Aid or External Funding.</summary>
            public string FundingType;
            /// <summary>Sessions offered at least one active option of this type.</summary>
            public int OfferedCount;
            /// <summary>Of those, sessions that added funding to their summary.</summary>
            public int AddedCount;
            /// <summary>The faculty with the most links to options of this type, or "".</summary>
            public string MostLinkedFaculty;
        }
        #endregion
    }
}
