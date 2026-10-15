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
    ///
    /// The class is split into three files, like the forms' partial classes: this file holds the
    /// catalogue, the export log and the shared helpers; ReportController.LearnerSummary.cs builds
    /// FP-LSR; ReportController.PeriodReports.cs builds FP-DIE and FP-LER.
    /// </summary>
    public partial class ReportController
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
