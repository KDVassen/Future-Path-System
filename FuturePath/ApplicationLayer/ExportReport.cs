using System;

namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// A log entry for every generated report (decisions.md D2). For FP-LSR, SessionID is set
    /// and ParametersUsed is null. For FP-DIE and FP-LER, SessionID is null and ParametersUsed
    /// holds the selection. GeneratedByUserID is null only when a learner exports their own
    /// FP-LSR on UI 7. Maps to ExportReport (class diagram: reportId, dateGenerated, format).
    /// </summary>
    public class ExportReport
    {
        #region Data Members
        /// <summary>Learner Advisory Summary report code.</summary>
        public const string CodeLearnerSummary = "FP-LSR";
        /// <summary>Degree Interest and Eligibility report code.</summary>
        public const string CodeDegreeInterest = "FP-DIE";
        /// <summary>Learners Not Currently Eligible (exception) report code.</summary>
        public const string CodeNotEligible = "FP-LER";

        /// <summary>Stored export format for a PDF file.</summary>
        public const string FormatPdf = "PDF";
        /// <summary>Stored export format for a Word (.docx) file.</summary>
        public const string FormatWord = "DOCX";
        /// <summary>Stored export format for an Excel (.xlsx) file.</summary>
        public const string FormatExcel = "XLSX";

        private int exportID;
        private string reportCode;
        private int? sessionID;
        private string parametersUsed;
        private int? generatedByUserID;
        private DateTime dateGenerated;
        private string exportFormat;
        #endregion

        #region Properties
        /// <summary>Primary key (ExportReport.ExportID; class diagram reportId).</summary>
        public int ExportID
        {
            get { return exportID; }
            set { exportID = value; }
        }

        /// <summary>FP-LSR, FP-DIE or FP-LER.</summary>
        public string ReportCode
        {
            get { return reportCode; }
            set { reportCode = value; }
        }

        /// <summary>The session exported (FP-LSR only), otherwise null.</summary>
        public int? SessionID
        {
            get { return sessionID; }
            set { sessionID = value; }
        }

        /// <summary>The selection used (FP-DIE / FP-LER), e.g. "Period 2026-01-01..2026-09-15; Faculty=All".</summary>
        public string ParametersUsed
        {
            get { return parametersUsed; }
            set { parametersUsed = value; }
        }

        /// <summary>The staff member who generated it, or null for a learner's own export.</summary>
        public int? GeneratedByUserID
        {
            get { return generatedByUserID; }
            set { generatedByUserID = value; }
        }

        /// <summary>When the report was generated (printed on every export).</summary>
        public DateTime DateGenerated
        {
            get { return dateGenerated; }
            set { dateGenerated = value; }
        }

        /// <summary>PDF, DOCX or XLSX (class diagram "format").</summary>
        public string ExportFormat
        {
            get { return exportFormat; }
            set { exportFormat = value; }
        }
        #endregion

        #region Constructors
        /// <summary>Creates an empty log entry dated now.</summary>
        public ExportReport()
        {
            exportID = 0;
            reportCode = "";
            sessionID = null;
            parametersUsed = null;
            generatedByUserID = null;
            dateGenerated = DateTime.Now;
            exportFormat = FormatPdf;
        }

        /// <summary>Creates a log entry with all its values.</summary>
        public ExportReport(string pReportCode, int? pSessionID, string pParametersUsed, int? pGeneratedByUserID, DateTime pDateGenerated, string pExportFormat)
        {
            exportID = 0;
            reportCode = pReportCode;
            sessionID = pSessionID;
            parametersUsed = pParametersUsed;
            generatedByUserID = pGeneratedByUserID;
            dateGenerated = pDateGenerated;
            exportFormat = pExportFormat;
        }
        #endregion

        #region Methods
        /// <summary>True if the code is one of the three report codes.</summary>
        public static bool IsValidReportCode(string code)
        {
            return code == CodeLearnerSummary || code == CodeDegreeInterest || code == CodeNotEligible;
        }

        /// <summary>True if the format is PDF, DOCX or XLSX.</summary>
        public static bool IsValidFormat(string format)
        {
            return format == FormatPdf || format == FormatWord || format == FormatExcel;
        }
        #endregion
    }
}
