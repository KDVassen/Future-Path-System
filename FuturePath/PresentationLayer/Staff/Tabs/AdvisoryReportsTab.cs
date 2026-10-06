using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 10.3c — Advisory reports tab of the Sessions dashboard (read-only).
    /// The advisor sets a session date range and optional faculty, grade and outcome filters, and
    /// sees the three reports (FP-LSR, FP-DIE, FP-LER) with their scope, the number of rows those
    /// criteria put in scope and when each was last generated. <b>Generate report</b> checks the
    /// date range and asks the host to open Report generation (UI 11) already filtered.
    /// </summary>
    public partial class AdvisoryReportsTab : UserControl
    {
        // Colours used at runtime (spec 2.2.6). Everything else is set in the Designer.
        private static readonly Color InkColour = Color.FromArgb(27, 31, 36);
        private static readonly Color RedColour = Color.FromArgb(164, 38, 44);
        private static readonly Color MutedColour = Color.FromArgb(88, 96, 108);

        /// <summary>Creates the tab with the current year to date as the default period.</summary>
        public AdvisoryReportsTab()
        {
            InitializeComponent();
            grdReports.AutoGenerateColumns = false;
            dtpFrom.Value = new DateTime(DateTime.Today.Year, 1, 1);
            dtpTo.Value = DateTime.Today;
            lblResultsCount.Text = "Rows in scope follow the criteria above";
        }

        #region Tab contract

        /// <summary>UserAccount.UserID of the signed-in staff member.</summary>
        public int UserId { get; set; }

        /// <summary>UserAccount.Role of the signed-in staff member ("Career Advisor" or "Administrator").</summary>
        public string Role { get; set; }

        /// <summary>Raised with a status message; isWarning = true shows amber, false shows blue.</summary>
        public event Action<string, bool> StatusChanged;

        /// <summary>
        /// Raised by <b>Generate report</b>: report code ("FP-LSR" / "FP-DIE" / "FP-LER"), sessions
        /// from, sessions to, FacultyID, GradeLevelID and OutcomeCode (null = all). The host opens UI 11.
        /// </summary>
        public event Action<string, DateTime, DateTime, int?, int?, string> GenerateReportRequested;

        /// <summary>Caption of the host's primary button while this tab is active.</summary>
        public string PrimaryActionText
        {
            get { return "Generate report"; }
        }

        /// <summary>Binds the filter lists and lists the reports. The host calls this when the tab is first shown.</summary>
        public void LoadData()
        {
            LoadLists();
            Find();
        }

        /// <summary>Validates the period and asks the host to open UI 11 for the selected report.</summary>
        public void RunPrimaryAction()
        {
            if (!CheckDateRange())
            {
                RaiseStatus("Sessions from must be on or before Sessions to.", true);
                dtpFrom.Focus();
                return;
            }

            if (grdReports.CurrentRow == null)
            {
                RaiseStatus("Select a report in the list first, then choose Generate report.", true);
                return;
            }

            string reportCode = Convert.ToString(grdReports.CurrentRow.Cells[colReportCode.Index].Value, CultureInfo.InvariantCulture);
            if (string.IsNullOrEmpty(reportCode))
            {
                RaiseStatus("This report could not be opened. Please select Find and try again.", true);
                return;
            }

            if (GenerateReportRequested != null)
            {
                GenerateReportRequested(reportCode, dtpFrom.Value.Date, dtpTo.Value.Date,
                    SelectedId(cboFaculty), SelectedId(cboGrade),
                    cboOutcome.SelectedIndex >= 0 ? Convert.ToString(cboOutcome.SelectedValue, CultureInfo.InvariantCulture) : null);
            }
        }

        #endregion

        #region Business Logic Layer hooks

        /// <summary>Binds the Faculty, Grade and Advisory outcome lists to stored data (never literal Items).</summary>
        private void LoadLists()
        {
            try
            {
                // TODO BLL: cboFaculty.DataSource = ReferenceDataController.GetFaculties(activeOnly: true);
                //           cboFaculty.DisplayMember = "FacultyName"; cboFaculty.ValueMember = "FacultyID";
                // TODO BLL: cboGrade.DataSource = ReferenceDataController.GetGradeLevels();
                //           cboGrade.DisplayMember = "GradeDescription"; cboGrade.ValueMember = "GradeLevelID";
                // TODO BLL: cboOutcome.DataSource = ReferenceDataController.GetOutcomes();
                //           cboOutcome.DisplayMember = "OutcomeName"; cboOutcome.ValueMember = "OutcomeCode";
                cboFaculty.SelectedIndex = -1;
                cboGrade.SelectedIndex = -1;
                cboOutcome.SelectedIndex = -1;
            }
            catch (Exception)
            {
                RaiseStatus("The filter lists could not be loaded. You can still search on the date range.", true);
            }
        }

        /// <summary>
        /// Asks the BLL for the report catalogue under these criteria: one row per report
        /// (ReportCode, ReportName, Scope, RowsInScope, LastGenerated — the latest
        /// ExportReport.DateGenerated for that ReportCode). Rows come from the BLL, not literals.
        /// </summary>
        private DataTable GetReportCatalogue(DateTime from, DateTime to, int? facultyId, int? gradeLevelId, string outcomeCode)
        {
            // TODO BLL: return ReportController.GetReportCatalogue(from, to, facultyId, gradeLevelId, outcomeCode, UserId);
            //           (RowsInScope counts completed sessions only; FP-LER always counts only
            //            Not Currently Eligible and More Information Needed — the BLL applies these rules)
            return null;
        }

        #endregion

        #region Search and validation

        /// <summary>Validates the period, then lists the reports with their rows in scope.</summary>
        private void Find()
        {
            if (!CheckDateRange())
            {
                RaiseStatus("Sessions from must be on or before Sessions to.", true);
                return;
            }

            DataTable reports;
            try
            {
                reports = GetReportCatalogue(dtpFrom.Value.Date, dtpTo.Value.Date, SelectedId(cboFaculty), SelectedId(cboGrade),
                    cboOutcome.SelectedIndex >= 0 ? Convert.ToString(cboOutcome.SelectedValue, CultureInfo.InvariantCulture) : null);
            }
            catch (Exception)
            {
                RaiseStatus("The report list could not be loaded. Please try again.", true);
                return;
            }

            grdReports.DataSource = reports;
            int count = reports == null ? 0 : reports.Rows.Count;
            if (count == 0)
            {
                lblResultsCount.Text = "No reports listed";
                RaiseStatus("The report list is empty. Check the criteria and select Find again.", true);
            }
            else
            {
                lblResultsCount.Text = "Rows in scope for " + dtpFrom.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) +
                                       " to " + dtpTo.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                RaiseStatus("Select a report and choose Generate report to open it with these criteria.", false);
            }
        }

        /// <summary>Sessions from must not be later than Sessions to (field message on both labels).</summary>
        private bool CheckDateRange()
        {
            bool valid = dtpFrom.Value.Date <= dtpTo.Value.Date;
            string message = "Sessions from is after Sessions to.";
            lblFrom.ForeColor = valid ? InkColour : RedColour;
            lblTo.ForeColor = valid ? InkColour : RedColour;
            dtpFrom.AccessibleDescription = valid ? null : message;
            dtpTo.AccessibleDescription = valid ? null : message;
            lblDateRangeError.Text = valid ? string.Empty : "⚠ " + message;
            lblDateRangeError.Visible = !valid;
            return valid;
        }

        private static int? SelectedId(ComboBox list)
        {
            return list.SelectedIndex >= 0 && list.SelectedValue is int ? (int?)(int)list.SelectedValue : null;
        }

        private void DateRange_Leave(object sender, EventArgs e)
        {
            CheckDateRange();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            Find();
        }

        /// <summary>Enter in the search card runs Find instead of the host's primary button.</summary>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Return && pnlSearchCard.ContainsFocus)
            {
                Find();
                return true;
            }

            return base.ProcessDialogKey(keyData);
        }

        /// <summary>Shows Last generated as dd/MM/yyyy HH:mm, or "Never" (display formatting only).</summary>
        private void grdReports_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex != colLastGenerated.Index)
            {
                return;
            }

            if (e.Value is DateTime)
            {
                e.Value = ((DateTime)e.Value).ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                e.FormattingApplied = true;
            }
            else if (e.Value == null || e.Value == DBNull.Value)
            {
                e.Value = "Never";
                e.CellStyle.ForeColor = MutedColour;
                e.FormattingApplied = true;
            }
        }

        /// <summary>Double-clicking a report is the same as Generate report.</summary>
        private void grdReports_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                RunPrimaryAction();
            }
        }

        #endregion

        private void RaiseStatus(string message, bool isWarning)
        {
            if (StatusChanged != null)
            {
                StatusChanged(message, isWarning);
            }
        }
    }
}
