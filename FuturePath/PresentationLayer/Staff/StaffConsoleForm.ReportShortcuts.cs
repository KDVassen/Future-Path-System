
using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using FuturePath.ApplicationLayer;

namespace FuturePath.PresentationLayer
{
    // Handles report searches and shortcuts from the Sessions dashboard
    public partial class StaffConsoleForm
    {
        // Initialise the report search screen and its filters
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

        // Open the selected report with the current search filters
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
                SelectedId(cboRscFaculty), SelectedId(cboRscGrade), SelectedCode(cboRscOutcome));
        }

        #endregion

        #region Business Logic Layer hooks

        // Load the faculty, grade and outcome filter lists
        private void ReportShortcutsLoadLists()
        {
            try
            {
                // Leaving a filter blank searches all matching records
                ReferenceDataController referenceDataController = new ReferenceDataController();
                cboRscFaculty.DisplayMember = "FacultyName";
                cboRscFaculty.ValueMember = "FacultyID";
                cboRscFaculty.DataSource = referenceDataController.GetFaculties(false, false);
                cboRscGrade.DisplayMember = "GradeDescription";
                cboRscGrade.ValueMember = "GradeLevelID";
                cboRscGrade.DataSource = referenceDataController.GetGradeLevels(false);
                cboRscOutcome.DisplayMember = "OutcomeName";
                cboRscOutcome.ValueMember = "OutcomeCode";
                cboRscOutcome.DataSource = referenceDataController.GetOutcomes(false);
                cboRscFaculty.SelectedIndex = -1;
                cboRscGrade.SelectedIndex = -1;
                cboRscOutcome.SelectedIndex = -1;
            }
            catch (Exception)
            {
                ReportStatus("The filter lists could not be loaded. You can still search on the date range.", true);
            }
        }

        // Retrieve available reports and their matching session counts
        private DataTable ReportShortcutsGetReportCatalogue(DateTime from, DateTime to, int facultyId, int gradeLevelId, string outcomeCode)
        {
            ReportController reportController = new ReportController();
            return reportController.GetReportCatalogue(UserId, from, to, facultyId, gradeLevelId, outcomeCode);
        }

        #endregion

        #region Search and validation

        // Validate the date range and display matching reports
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
                    SelectedId(cboRscFaculty), SelectedId(cboRscGrade), SelectedCode(cboRscOutcome));
            }
            catch (ArgumentException ex)
            {
                ReportStatus(ex.Message, true);
                return;
            }
            catch (Exception)
            {
                ReportStatus("The report list could not be loaded. Please try again.", true);
                return;
            }

            dgvReports.DataSource = reports;
            int count = reports.Rows.Count;
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

        // Check that the start date is not after the end date
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

        // Validate the date range when leaving a date field
        private void DateRange_Leave(object sender, EventArgs e)
        {
            ReportShortcutsCheckDateRange();
        }

        // Run the report search
        private void btnRscFind_Click(object sender, EventArgs e)
        {
            ReportShortcutsFind();
        }

        // Reset the search criteria and clear the report list
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

        // Format the last generated date or display Never
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

        // Open a report when its row is double-clicked
        private void dgvReports_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                btnReportShortcutsGenerate_Click(sender, e);
            }
        }

        #endregion
    }
}
