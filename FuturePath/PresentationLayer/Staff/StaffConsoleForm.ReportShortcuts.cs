using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 10.3c — Advisory reports tab of the Sessions dashboard (read-only).
    /// The advisor sets a session date range and optional faculty, grade and outcome filters, and
    /// sees the three reports (FP-LSR, FP-DIE, FP-LER) with their scope, the number of rows those
    /// criteria put in scope and when each was last generated. Find lists the reports for the criteria
    /// and Clear resets them, as on 10.1a and 10.2b. <b>Generate report</b> checks the
    /// date range and opens Report generation (UI 11) already filtered.
    /// </summary>
    public partial class StaffConsoleForm
    {
        /// <summary>
        /// Called by <see cref="PageShown"/> each time UI 10.3c appears. The first time, it prepares the
        /// grid, sets the current year to date as the default period, binds the filter lists and lists
        /// the reports.
        /// </summary>
        /// <param name="firstTime">True the first time the tab is shown in this console.</param>
        private void ReportShortcutsShown(bool firstTime)
        {
            if (!firstTime)
            {
                return;
            }

            dgvReports.AutoGenerateColumns = false;
            dtpRscFrom.Value = new DateTime(DateTime.Today.Year, 1, 1);
            dtpRscTo.Value = DateTime.Today;
            lblRscResultsCount.Text = "Rows in scope follow the criteria above";
            ReportShortcutsLoadLists();
            ReportShortcutsFind();
        }

        #region Button click events

        /// <summary>
        /// Generate report: validates the period, then opens UI 11 for the selected report with this
        /// tab's criteria applied.
        /// </summary>
        private void btnReportShortcutsGenerate_Click(object sender, EventArgs e)
        {
            if (!ReportShortcutsCheckDateRange())
            {
                ReportStatus("Sessions from must be on or before Sessions to.", true);
                dtpRscFrom.Focus();
                return;
            }

            if (dgvReports.CurrentRow == null)
            {
                ReportStatus("Select a report in the list first, then choose Generate report.", true);
                return;
            }

            string reportCode = Convert.ToString(dgvReports.CurrentRow.Cells[colReportsCode.Index].Value, CultureInfo.InvariantCulture);
            if (string.IsNullOrEmpty(reportCode))
            {
                ReportStatus("This report could not be opened. Please select Find and try again.", true);
                return;
            }

            OpenReports(reportCode, dtpRscFrom.Value.Date, dtpRscTo.Value.Date,
                ReportShortcutsSelectedId(cboRscFaculty), ReportShortcutsSelectedId(cboRscGrade),
                cboRscOutcome.SelectedIndex >= 0 ? Convert.ToString(cboRscOutcome.SelectedValue, CultureInfo.InvariantCulture) : null);
        }

        #endregion

        #region Business Logic Layer hooks

        /// <summary>Binds the Faculty, Grade and Advisory outcome lists to stored data (never literal Items).</summary>
        private void ReportShortcutsLoadLists()
        {
            try
            {
                // TODO BLL: cboRscFaculty.DataSource = ReferenceDataController.GetFaculties(activeOnly: true);
                //           cboRscFaculty.DisplayMember = "FacultyName"; cboRscFaculty.ValueMember = "FacultyID";
                // TODO BLL: cboRscGrade.DataSource = ReferenceDataController.GetGradeLevels();
                //           cboRscGrade.DisplayMember = "GradeDescription"; cboRscGrade.ValueMember = "GradeLevelID";
                // TODO BLL: cboRscOutcome.DataSource = ReferenceDataController.GetOutcomes();
                //           cboRscOutcome.DisplayMember = "OutcomeName"; cboRscOutcome.ValueMember = "OutcomeCode";
                cboRscFaculty.SelectedIndex = -1;
                cboRscGrade.SelectedIndex = -1;
                cboRscOutcome.SelectedIndex = -1;
            }
            catch (Exception)
            {
                ReportStatus("The filter lists could not be loaded. You can still search on the date range.", true);
            }
        }

        /// <summary>
        /// Asks the BLL for the report catalogue under these criteria: one row per report
        /// (ReportCode, ReportName, Scope, RowsInScope, LastGenerated — the latest
        /// ExportReport.DateGenerated for that ReportCode). Rows come from the BLL, not literals.
        /// </summary>
        private DataTable ReportShortcutsGetReportCatalogue(DateTime from, DateTime to, int? facultyId, int? gradeLevelId, string outcomeCode)
        {
            // TODO BLL: return ReportController.GetReportCatalogue(from, to, facultyId, gradeLevelId, outcomeCode, UserId);
            //           (RowsInScope counts completed sessions only; FP-LER always counts only
            //            Not Currently Eligible and More Information Needed — the BLL applies these rules)
            return null;
        }

        #endregion

        #region Search and validation

        /// <summary>Validates the period, then lists the reports with their rows in scope.</summary>
        private void ReportShortcutsFind()
        {
            if (!ReportShortcutsCheckDateRange())
            {
                ReportStatus("Sessions from must be on or before Sessions to.", true);
                return;
            }

            DataTable reports;
            try
            {
                reports = ReportShortcutsGetReportCatalogue(dtpRscFrom.Value.Date, dtpRscTo.Value.Date,
                    ReportShortcutsSelectedId(cboRscFaculty), ReportShortcutsSelectedId(cboRscGrade),
                    cboRscOutcome.SelectedIndex >= 0 ? Convert.ToString(cboRscOutcome.SelectedValue, CultureInfo.InvariantCulture) : null);
            }
            catch (Exception)
            {
                ReportStatus("The report list could not be loaded. Please try again.", true);
                return;
            }

            dgvReports.DataSource = reports;
            int count = reports == null ? 0 : reports.Rows.Count;
            if (count == 0)
            {
                lblRscResultsCount.Text = "No reports listed";
                ReportStatus("The report list is empty. Check the criteria and select Find again.", true);
            }
            else
            {
                lblRscResultsCount.Text = "Rows in scope for " + dtpRscFrom.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) +
                                          " to " + dtpRscTo.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                ReportStatus("Select a report and choose Generate report to open it with these criteria.", false);
            }
        }

        /// <summary>Sessions from must not be later than Sessions to (field message on both labels).</summary>
        private bool ReportShortcutsCheckDateRange()
        {
            bool valid = dtpRscFrom.Value.Date <= dtpRscTo.Value.Date;
            string message = "Sessions from is after Sessions to.";
            lblRscFrom.ForeColor = valid ? InkColour : RedColour;
            lblRscTo.ForeColor = valid ? InkColour : RedColour;
            dtpRscFrom.AccessibleDescription = valid ? null : message;
            dtpRscTo.AccessibleDescription = valid ? null : message;
            lblRscDateRangeError.Text = valid ? string.Empty : "⚠ " + message;
            lblRscDateRangeError.Visible = !valid;
            return valid;
        }

        /// <summary>The selected list's integer ID, or null when nothing (or no number) is selected.</summary>
        private static int? ReportShortcutsSelectedId(ComboBox list)
        {
            return list.SelectedIndex >= 0 && list.SelectedValue is int ? (int?)(int)list.SelectedValue : null;
        }

        /// <summary>Checks the period as soon as either date picker is left.</summary>
        private void DateRange_Leave(object sender, EventArgs e)
        {
            ReportShortcutsCheckDateRange();
        }

        /// <summary>Find validates the period and lists the reports.</summary>
        private void btnRscFind_Click(object sender, EventArgs e)
        {
            ReportShortcutsFind();
        }

        /// <summary>Clears the criteria (back to the current year to date), any field message and the report list.</summary>
        private void btnRscClear_Click(object sender, EventArgs e)
        {
            dtpRscFrom.Value = new DateTime(DateTime.Today.Year, 1, 1);
            dtpRscTo.Value = DateTime.Today;
            cboRscFaculty.SelectedIndex = -1;
            cboRscGrade.SelectedIndex = -1;
            cboRscOutcome.SelectedIndex = -1;
            ReportShortcutsCheckDateRange();
            dgvReports.DataSource = null;
            lblRscResultsCount.Text = "Rows in scope follow the criteria above";
            ReportStatus("Search criteria cleared.", false);
            dtpRscFrom.Focus();
        }

        /// <summary>Shows Last generated as dd/MM/yyyy HH:mm, or "Never" (display formatting only).</summary>
        private void dgvReports_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex != colReportsLastGenerated.Index)
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
        private void dgvReports_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                btnReportShortcutsGenerate_Click(sender, e);
            }
        }

        #endregion

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // StaffConsoleForm
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "StaffConsoleForm";
            this.Load += new System.EventHandler(this.StaffConsoleForm_Load);
            this.ResumeLayout(false);

        }

        private void StaffConsoleForm_Load(object sender, EventArgs e)
        {

        }
    }
}
