
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using FuturePath.ApplicationLayer.Reports;
using FuturePath.DatabaseLayer;

namespace FuturePath.ApplicationLayer
{
    // Stores report information and provides shared reporting methods
    // Uses the report datasets to create summaries and log exports
    public partial class ReportController
    {
        #region Data Members
        // Disclaimer shown with advisory reports
        public const string AdvisoryDisclaimer =
            "Future Path gives preliminary guidance only. It is not an admission decision. Programme requirements " +
            "and funding details are locally stored sample data; check them against the official university or " +
            "funder source before relying on them.";

        // Additional disclaimer for exception reports
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
        // Initialise the repositories used to generate reports
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
        // Get the name of a report from its code
        public string GetReportName(string reportCode)
        {
            switch (NormaliseCode(reportCode))
            {
                case ExportReport.CodeLearnerSummary:
                    return "Learner Advisory Summary";
                case ExportReport.CodeDegreeInterest:
                    return "Degree Interest and Eligibility";
                case ExportReport.CodeNotEligible:
                    return "Learners Not Currently Eligible";
                default:
                    return "";
            }
        }

        // Get the available export formats for a report
        public DataTable GetExportFormats(string reportCode)
        {
            DataTable table = new DataTable("ExportFormats");
            table.Columns.Add("ExportFormat", typeof(string));
            table.Columns.Add("FormatName", typeof(string));
            table.Columns.Add("FormatCode", typeof(string));
            table.Columns.Add("RenderFormat", typeof(string));

            foreach (string formatCode in GetReportFormats(NormaliseCode(reportCode)))
            {
                table.Rows.Add(GetFormatExtension(formatCode), GetFormatName(formatCode), formatCode, GetRenderFormat(formatCode));
            }
            return table;
        }

        // Get the date a report was most recently generated
        public DateTime? GetLastGenerated(string reportCode)
        {
            string code = NormaliseCode(reportCode);
            if (!ExportReport.IsValidReportCode(code))
            {
                return null;
            }
            return exportRepository.GetLastGenerated(code);
        }

        // Build the report catalogue with counts and last export dates
        // Only authorised staff can view the catalogue
        public DataTable GetReportCatalogue(int actorUserID, DateTime fromDate, DateTime toDate, int facultyID, int gradeLevelID, string outcomeCode)
        {
            loginController.CheckStaff(actorUserID);
            CheckPeriod(fromDate, toDate);
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
                ReportScope scope = CountScope(code, fromDate, toDate, facultyID, gradeLevelID, outcome, false);
                DateTime? lastGenerated = exportRepository.GetLastGenerated(code);

                DataRow row = catalogue.NewRow();
                row["ReportCode"] = code;
                row["ReportName"] = GetReportName(code);
                row["Scope"] = GetReportScope(code);
                row["RowsInScope"] = scope.SessionCount;
                row["LastGenerated"] = DBNull.Value;
                if (lastGenerated.HasValue)
                {
                    row["LastGenerated"] = lastGenerated.Value;
                }
                catalogue.Rows.Add(row);
            }
            return catalogue;
        }

        // Count the sessions, faculties and programmes covered by a report
        public ReportScope GetScope(int actorUserID, string reportCode, DateTime fromDate, DateTime toDate, int facultyID, int gradeLevelID,
                                    string outcomeCode, bool includeInactive)
        {
            loginController.CheckStaff(actorUserID);
            string code = NormaliseCode(reportCode);
            if (!ExportReport.IsValidReportCode(code))
            {
                throw new ArgumentException(UnknownReportMessage);
            }
            CheckPeriod(fromDate, toDate);
            return CountScope(code, fromDate, toDate, facultyID, gradeLevelID, OptionalOutcome(outcomeCode), includeInactive);
        }

        // Count completed sessions within the selected report filters
        // Only the degree interest report can include inactive programmes
        private ReportScope CountScope(string reportCode, DateTime fromDate, DateTime toDate, int facultyID, int gradeLevelID, string outcomeCode,
                                       bool includeInactive)
        {
            bool exceptionsOnly = reportCode == ExportReport.CodeNotEligible;
            string outcomeFilter = outcomeCode;
            if (exceptionsOnly)
            {
                outcomeFilter = "";
            }
            bool inactiveIncluded = includeInactive && reportCode == ExportReport.CodeDegreeInterest;
            DataTable counts = reportRepository.GetScopeCounts(PeriodStart(fromDate), PeriodEnd(toDate), facultyID, gradeLevelID,
                                                               outcomeFilter, inactiveIncluded, exceptionsOnly);
            ReportScope scope = new ReportScope();
            scope.SessionCount = Convert.ToInt32(counts.Rows[0]["SessionCount"]);
            scope.FacultyCount = Convert.ToInt32(counts.Rows[0]["FacultyCount"]);
            scope.ProgrammeCount = Convert.ToInt32(counts.Rows[0]["ProgrammeCount"]);
            return scope;
        }

        // Check the dates, report type and requested export format
        public Collection<string> ValidateReportRequest(string reportCode, DateTime fromDate, DateTime toDate, string exportFormat)
        {
            Collection<string> messages = new Collection<string>();
            string code = NormaliseCode(reportCode);
            bool knownReport = ExportReport.IsValidReportCode(code);
            string[] formats = GetReportFormats(code);
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
                messages.Add(GetReportName(code) + " can only be exported as " + DescribeFormats(formats) + ".");
            }
            return messages;
        }

        // Build the text describing the filters used for a period report
        // The learner summary stores a session ID instead of parameters
        public string BuildParametersUsed(string reportCode, DateTime fromDate, DateTime toDate, int facultyID, int gradeLevelID, string outcomeCode, bool includeInactive)
        {
            string code = NormaliseCode(reportCode);
            if (code != ExportReport.CodeDegreeInterest && code != ExportReport.CodeNotEligible)
            {
                return null;
            }

            string outcomeText;
            if (code == ExportReport.CodeNotEligible)
            {
                outcomeText = AdvisoryOutcome.GetCategory(AdvisoryOutcome.CodeNotEligible) + " or "
                              + AdvisoryOutcome.GetCategory(AdvisoryOutcome.CodeMoreInformation);
            }
            else
            {
                string outcome = OptionalOutcome(outcomeCode);
                outcomeText = "All";
                if (outcome.Length > 0)
                {
                    outcomeText = AdvisoryOutcome.GetCategory(outcome);
                }
            }
            string inactiveText = "Excluded";
            if (code == ExportReport.CodeDegreeInterest && includeInactive)
            {
                inactiveText = "Included";
            }

            string text = "Period " + fromDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                          + ".." + toDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                          + "; Faculty=" + FacultyText(facultyID, "All")
                          + "; Grade=" + GradeText(gradeLevelID, "All")
                          + "; Outcome=" + outcomeText
                          + "; Inactive=" + inactiveText;
            // ExportReport.ParametersUsed holds at most 255 characters
            if (text.Length > ParametersMaxLength)
            {
                text = text.Substring(0, ParametersMaxLength);
            }
            return text;
        }
        #endregion

        #region Export log
        // Log an exported report after checking staff access and format
        // Learner summaries require an existing completed session
        public void RecordExport(int actorUserID, string reportCode, int sessionID, string parametersUsed, string exportFormat, DateTime generatedAt)
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
                if (sessionID <= 0)
                {
                    throw new ArgumentException("Choose the advisory session whose summary was exported.");
                }
                string status = reportRepository.GetSessionStatus(sessionID);
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
                // No session for FP-DIE / FP-LER; the parameters are cut to the 255 characters the column holds
                string parameters = null;
                if (!string.IsNullOrWhiteSpace(parametersUsed))
                {
                    parameters = parametersUsed.Trim();
                    if (parameters.Length > ParametersMaxLength)
                    {
                        parameters = parameters.Substring(0, ParametersMaxLength);
                    }
                }
                entry = new ExportReport(code, null, parameters, actorUserID, generatedAt, format);
            }
            exportRepository.Insert(entry);
        }
        #endregion

        #region Catalogue and format helpers
        // Return a short description of what the report covers
        private static string GetReportScope(string reportCode)
        {
            switch (reportCode)
            {
                case ExportReport.CodeLearnerSummary:
                    return "One summary per completed advisory session";
                case ExportReport.CodeDegreeInterest:
                    return "Completed advisory sessions in the period";
                case ExportReport.CodeNotEligible:
                    return "Completed sessions that are Not Currently Eligible or need more information";
                default:
                    return "";
            }
        }

        // Return the export formats allowed for the selected report
        private static string[] GetReportFormats(string reportCode)
        {
            switch (reportCode)
            {
                case ExportReport.CodeLearnerSummary:
                    return new string[] { ExportReport.FormatPdf, ExportReport.FormatWord };
                case ExportReport.CodeDegreeInterest:
                    return new string[] { ExportReport.FormatPdf, ExportReport.FormatExcel };
                case ExportReport.CodeNotEligible:
                    return new string[] { ExportReport.FormatPdf, ExportReport.FormatWord };
                default:
                    return new string[0];
            }
        }

        // Convert the stored format to its file extension
        private static string GetFormatExtension(string formatCode)
        {
            switch (formatCode)
            {
                case ExportReport.FormatPdf:
                    return ".pdf";
                case ExportReport.FormatWord:
                    return ".docx";
                case ExportReport.FormatExcel:
                    return ".xlsx";
                default:
                    return "";
            }
        }

        // Get the display name for an export format
        private static string GetFormatName(string formatCode)
        {
            switch (formatCode)
            {
                case ExportReport.FormatPdf:
                    return "PDF document (.pdf)";
                case ExportReport.FormatWord:
                    return "Word document (.docx)";
                case ExportReport.FormatExcel:
                    return "Excel workbook (.xlsx)";
                default:
                    return "";
            }
        }

        // Get the format required by ReportViewer
        private static string GetRenderFormat(string formatCode)
        {
            switch (formatCode)
            {
                case ExportReport.FormatPdf:
                    return "PDF";
                case ExportReport.FormatWord:
                    return "WORDOPENXML";
                case ExportReport.FormatExcel:
                    return "EXCELOPENXML";
                default:
                    return "";
            }
        }

        // Check that the selected report supports the requested format
        private string CheckFormat(string reportCode, string exportFormat)
        {
            string[] formats = GetReportFormats(reportCode);
            string format = NormaliseFormat(exportFormat);
            if (!AllowsFormat(formats, format))
            {
                throw new ArgumentException(GetReportName(reportCode) + " can only be exported as " + DescribeFormats(formats) + ".");
            }
            return format;
        }

        // Check whether a format is in the list of allowed formats
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

        // Combine the available export formats into a readable message
        private static string DescribeFormats(string[] formats)
        {
            string text = "";
            for (int i = 0; i < formats.Length; i++)
            {
                if (i > 0 && i == formats.Length - 1)
                {
                    text += " or ";
                }
                else if (i > 0)
                {
                    text += ", ";
                }
                text += GetFormatName(formats[i]);
            }
            return text;
        }

        // Clean and capitalise a report code
        private static string NormaliseCode(string reportCode)
        {
            return TextOf(reportCode).ToUpperInvariant();
        }

        // Convert a file extension or format name to its stored code
        private static string NormaliseFormat(string exportFormat)
        {
            return TextOf(exportFormat).TrimStart('.').ToUpperInvariant();
        }
        #endregion

        #region Period and filter helpers
        // Check that the reporting start date is not after the end date
        private static void CheckPeriod(DateTime fromDate, DateTime toDate)
        {
            if (fromDate.Date > toDate.Date)
            {
                throw new ArgumentException("Sessions from must be on or before Sessions to.");
            }
        }

        // Get the start of the first day in a reporting period
        private static DateTime PeriodStart(DateTime fromDate)
        {
            return fromDate.Date;
        }

        // Get the start of the day after the reporting period ends
        private static DateTime PeriodEnd(DateTime toDate)
        {
            return toDate.Date.AddDays(1);
        }

        // Validate the optional advisory outcome filter
        // Return an empty string when all outcomes are included
        private static string OptionalOutcome(string outcomeCode)
        {
            if (string.IsNullOrWhiteSpace(outcomeCode))
            {
                return "";
            }
            if (!AdvisoryOutcome.IsValidCode(outcomeCode))
            {
                throw new ArgumentException("Choose an advisory outcome from the list.");
            }
            return outcomeCode.Trim().ToUpperInvariant();
        }

        // Get the selected faculty name for report parameters
        private string FacultyText(int facultyID, string allText)
        {
            if (facultyID <= 0)
            {
                return allText;
            }
            string name = reportRepository.GetFacultyName(facultyID);
            if (name.Length == 0)
            {
                return "Unknown faculty";
            }
            return name;
        }

        // Get the selected grade description for report parameters
        private string GradeText(int gradeLevelID, string allText)
        {
            if (gradeLevelID <= 0)
            {
                return allText;
            }
            DataTable grades = reportRepository.GetGradeLevel(gradeLevelID);
            if (grades.Rows.Count == 0)
            {
                return "Unknown grade";
            }
            return TextOf(grades.Rows[0]["GradeDescription"]);
        }

        // Format the reporting dates as a readable range
        private static string FormatPeriod(DateTime fromDate, DateTime toDate)
        {
            string start = fromDate.ToString("d MMMM yyyy", CultureInfo.InvariantCulture);
            if (fromDate.Year == toDate.Year)
            {
                start = fromDate.ToString("d MMMM", CultureInfo.InvariantCulture);
            }
            return start + " \u2013 " + toDate.ToString("d MMMM yyyy", CultureInfo.InvariantCulture);
        }
        #endregion

        #region Counting and text helpers
        // Calculate a percentage, returning zero for an empty total
        private static decimal Percent(int part, int whole)
        {
            if (whole == 0)
            {
                return 0;
            }
            return Math.Round(100m * part / whole, 1);
        }

        // Count sessions with the given advisory outcome
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

        // Count distinct IDs in a table column
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

        // Find an existing report tally or create a new one
        // Match by ID when given, otherwise by name
        private static ReportTally FindOrAddTally(List<ReportTally> tallies, int id, string name)
        {
            foreach (ReportTally tally in tallies)
            {
                if ((id > 0 && tally.ID == id) || (id == 0 && tally.Name == name))
                {
                    return tally;
                }
            }
            ReportTally added = new ReportTally();
            added.ID = id;
            added.Name = name;
            tallies.Add(added);
            return added;
        }

        // Add a value to a collection only once
        private static void AddOnce(Collection<string> items, string text)
        {
            if (!items.Contains(text))
            {
                items.Add(text);
            }
        }

        // Sort tallies by session count, then by name
        private static int CompareSessionsDescending(ReportTally a, ReportTally b)
        {
            if (a.SessionCount != b.SessionCount)
            {
                return b.SessionCount.CompareTo(a.SessionCount);
            }
            return string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);
        }

        // Sort tallies by count, then by name
        private static int CompareCountsDescending(ReportTally a, ReportTally b)
        {
            if (a.Count != b.Count)
            {
                return b.Count.CompareTo(a.Count);
            }
            return string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);
        }

        // Sort funding tallies by count, additions and type name
        private static int CompareFundingDescending(ReportTally a, ReportTally b)
        {
            if (a.Count != b.Count)
            {
                return b.Count.CompareTo(a.Count);
            }
            if (a.AddedCount != b.AddedCount)
            {
                return b.AddedCount.CompareTo(a.AddedCount);
            }
            return string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);
        }

        // Sort interest categories by score and then name
        private static int CompareScoresDescending(KeyValuePair<string, decimal> a, KeyValuePair<string, decimal> b)
        {
            if (a.Value != b.Value)
            {
                return b.Value.CompareTo(a.Value);
            }
            return string.Compare(a.Key, b.Key, StringComparison.OrdinalIgnoreCase);
        }

        // Convert the outcome wording to sentence case
        private static string SentenceCase(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }
            return text.Substring(0, 1) + text.Substring(1).ToLowerInvariant();
        }

        // Combine the learner's first name and surname
        private static string LearnerName(object firstName, object lastName)
        {
            string name = (TextOf(firstName) + " " + TextOf(lastName)).Trim();
            if (name.Length == 0)
            {
                return "Name not given";
            }
            return name;
        }

        // Return the first sentence from a longer reason
        private static string FirstSentence(string text)
        {
            int end = text.IndexOf(". ", StringComparison.Ordinal);
            if (end < 0)
            {
                return text;
            }
            return text.Substring(0, end + 1);
        }

        // Join explanation sentences with spaces
        private static string JoinSentences(Collection<string> sentences)
        {
            string text = "";
            foreach (string sentence in sentences)
            {
                if (text.Length > 0)
                {
                    text += " ";
                }
                text += sentence;
            }
            return text;
        }

        // Convert a database value to trimmed text
        private static string TextOf(object value)
        {
            if (value == null || value is DBNull)
            {
                return "";
            }
            return Convert.ToString(value, CultureInfo.InvariantCulture).Trim();
        }

        // Format numbers without unnecessary decimal zeros
        private static string FormatNumber(decimal value)
        {
            return value.ToString("0.#", CultureInfo.InvariantCulture);
        }
        #endregion

        #region Tally class (the counting helper for FP-DIE and FP-LER)
        // Stores counts and totals used to build period reports
        private class ReportTally
        {
            // ID of the faculty or programme being counted
            public int ID;
            // Name of the item being counted
            public string Name;
            // Faculty name for programme tallies
            public string FacultyName = "";
            // Number of Strong Match sessions
            public int StrongCount;
            // Number of Possible Match sessions
            public int PossibleCount;
            // Number of Not Currently Eligible sessions
            public int NotEligibleCount;
            // Number of More Information Needed sessions
            public int MoreInfoCount;
            // Count of a subject gap, reason or funding type
            public int Count;
            // Number of sessions adding this funding to their summary
            public int AddedCount;
            // Total shortfall across the marks being counted
            public decimal ShortfallTotal;
            // Lowest captured mark in the group
            public decimal LowestMark;
            // Faculty with the most links to this funding type
            public string MostLinkedFaculty = "";

            // Total sessions counted across all outcome types
            public int SessionCount
            {
                get { return StrongCount + PossibleCount + NotEligibleCount + MoreInfoCount; }
            }

            // Percentage of sessions with Strong or Possible outcomes
            public decimal EligibilityRate
            {
                get { return Percent(StrongCount + PossibleCount, SessionCount); }
            }

            // Increase the count for the given advisory outcome
            public void AddOutcome(string outcomeCode)
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

            // Return the count for the given advisory outcome
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
        #endregion
    }
}
