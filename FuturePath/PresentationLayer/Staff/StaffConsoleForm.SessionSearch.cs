
using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FuturePath.ApplicationLayer;

namespace FuturePath.PresentationLayer
{
    // Handles advisory session searches and summary reports
    public partial class StaffConsoleForm
    {
        private const string SessionSearchCompletedStatus = "Completed";

        // Learner filter set when opening sessions from a learner profile
        // A value of 0 means no learner filter is applied
        private int sessionSearchLearnerFilterId;
        private string sessionSearchLearnerFilterName;

        // Initialise the session search screen and outcome list
        private void SessionSearchShown(bool firstTime)
        {
            if (!firstTime)
            {
                return;
            }

            dgvSessions.AutoGenerateColumns = false;
            lblSesResultsCount.Text = "Search to list sessions";
            SessionSearchLoadLists();
            ReportStatus("Search on any field — none is required. Tick a date box to use that date.", false);
        }

        #region Button click events

        // Open the selected session's advisory summary
        private void btnSessionSearchOpen_Click(object sender, EventArgs e)
        {
            SessionSearchOpenSummary(false);
        }

        #endregion

        // Show all advisory sessions belonging to the selected learner
        private void SessionSearchShowLearnerSessions(int learnerId, string learnerName)
        {
            // Filter using the learner ID rather than their display name
            SessionSearchClearCriteria();
            sessionSearchLearnerFilterId = learnerId;
            sessionSearchLearnerFilterName = learnerName;
            SessionSearchFind();
        }

        #region Business Logic Layer hooks

        // Load the available advisory outcomes
        private void SessionSearchLoadLists()
        {
            try
            {
                // Retrieve outcome codes and names from reference data
                ReferenceDataController referenceDataController = new ReferenceDataController();
                cboSesOutcome.DisplayMember = "OutcomeName";
                cboSesOutcome.ValueMember = "OutcomeCode";
                cboSesOutcome.DataSource = referenceDataController.GetOutcomes(false);
                cboSesOutcome.SelectedIndex = -1;
            }
            catch (Exception)
            {
                ReportStatus("The outcome list could not be loaded. You can still search on the other fields.", true);
            }
        }

        // Retrieve advisory sessions matching the search criteria
        private DataTable SessionSearchFindSessions(string sessionNumber, string surname, DateTime? from, DateTime? to,
            string outcomeCode, int learnerId)
        {
            SessionSearchController sessionSearchController = new SessionSearchController();
            return sessionSearchController.FindSessions(UserId, sessionNumber, surname, from, to, outcomeCode, learnerId);
        }

        // Get the total number of stored advisory sessions
        private int SessionSearchCountSessions()
        {
            SessionSearchController sessionSearchController = new SessionSearchController();
            return sessionSearchController.CountSessions(UserId);
        }

        // Generate or export the learner advisory summary for a session
        private void SessionSearchGenerateLearnerSummary(int sessionId, string sessionNumber, string filePath)
        {
            ReportController reportController = new ReportController();
            DataSet data = reportController.GetLearnerSummary(UserId, sessionId);
            if (filePath == null)
            {
                OpenReportViewer(LsrCode, data, "Session " + sessionNumber, sessionId, null, null, null,
                                 "FP-LSR_" + sessionNumber, tabSessionSearch);
            }
            else
            {
                SaveReport(LsrCode, data, filePath, sessionId, null);
            }
        }

        #endregion

        #region Search and validation

        // Validate the search criteria and display matching sessions
        private void SessionSearchFind()
        {
            if (!SessionSearchCheckSessionNumber())
            {
                ReportStatus("Session number: use the form yyyy-NNNN, e.g. 2026-0417, or leave it blank.", true);
                txtSesSessionNumber.Focus();
                return;
            }

            if (!SessionSearchCheckDateRange())
            {
                ReportStatus("Session from must be on or before Session to.", true);
                dtpSessionFrom.Focus();
                return;
            }

            // Remove the learner filter when searching by surname
            if (txtSesSurname.Text.Trim().Length > 0)
            {
                sessionSearchLearnerFilterId = 0;
            }

            // Only apply the start date if its checkbox is selected
            DateTime? from = null;
            if (dtpSessionFrom.Checked)
            {
                from = dtpSessionFrom.Value.Date;
            }

            DateTime? to = null;
            if (dtpSessionTo.Checked)
            {
                to = dtpSessionTo.Value.Date;
            }

            DataTable results;
            int total;
            try
            {
                results = SessionSearchFindSessions(txtSesSessionNumber.Text.Trim(), txtSesSurname.Text.Trim(), from, to,
                    SelectedCode(cboSesOutcome), sessionSearchLearnerFilterId);
                total = SessionSearchCountSessions();
            }
            catch (ArgumentException ex)
            {
                // Display validation messages returned by the controller
                ReportStatus(ex.Message, true);
                return;
            }
            catch (Exception)
            {
                ReportStatus("The sessions could not be loaded. Please try again.", true);
                return;
            }

            dgvSessions.DataSource = results;
            int count = results.Rows.Count;
            lblSesResultsCount.Text = count + " of " + Math.Max(total, count) + " sessions match";
            if (sessionSearchLearnerFilterId > 0)
            {
                lblSesResultsCount.Text = "Sessions of " + sessionSearchLearnerFilterName + ": " + count;
            }
            if (count == 0)
            {
                ReportStatus("No sessions match these criteria. Clear a field to widen the search.", true);
            }
            else
            {
                ReportStatus(count + (count == 1 ? " session" : " sessions") + " shown. Select one to open or export its advisory summary.", false);
            }
        }

        // Check that the optional session number follows yyyy-NNNN
        private bool SessionSearchCheckSessionNumber()
        {
            string text = txtSesSessionNumber.Text.Trim();
            bool valid = text.Length == 0 || Regex.IsMatch(text, @"^\d{4}-\d{4}$");
            return CheckField(valid, txtSesSessionNumber, lblSesSessionNumber, lblSesSessionNumberError,
                "Use the form yyyy-NNNN, e.g. 2026-0417.");
        }

        // Check that the selected date range is valid
        private bool SessionSearchCheckDateRange()
        {
            bool valid = !(dtpSessionFrom.Checked && dtpSessionTo.Checked && dtpSessionFrom.Value.Date > dtpSessionTo.Value.Date);
            const string message = "Session from is after Session to.";
            CheckField(valid, dtpSessionFrom, lblSessionFrom, lblSesDateRangeError, message);
            return CheckField(valid, dtpSessionTo, lblSessionTo, lblSesDateRangeError, message);
        }

        // Validate the session number when leaving its field
        private void txtSesSessionNumber_Leave(object sender, EventArgs e)
        {
            SessionSearchCheckSessionNumber();
        }

        // Run the session search
        private void btnSesFind_Click(object sender, EventArgs e)
        {
            SessionSearchFind();
        }

        // Clear the session search criteria
        private void btnSesClear_Click(object sender, EventArgs e)
        {
            SessionSearchClearCriteria();
            ReportStatus("Search criteria cleared.", false);
            txtSesSessionNumber.Focus();
        }

        // Reset filters, validation messages and search results
        private void SessionSearchClearCriteria()
        {
            txtSesSessionNumber.Clear();
            txtSesSurname.Clear();
            dtpSessionFrom.Value = DateTime.Today;
            dtpSessionTo.Value = DateTime.Today;
            dtpSessionFrom.Checked = false;
            dtpSessionTo.Checked = false;
            cboSesOutcome.SelectedIndex = -1;
            sessionSearchLearnerFilterId = 0;
            sessionSearchLearnerFilterName = null;
            SessionSearchCheckSessionNumber();
            SessionSearchCheckDateRange();
            dgvSessions.DataSource = null;
            lblSesResultsCount.Text = "Search to list sessions";
        }

        #endregion

        #region Advisory summary (FP-LSR)

        // Export the selected session's advisory summary
        private void btnExportSummary_Click(object sender, EventArgs e)
        {
            SessionSearchOpenSummary(true);
        }

        // Open the advisory summary when a session is double-clicked
        private void dgvSessions_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                SessionSearchOpenSummary(false);
            }
        }

        // Preview or export the selected completed session's summary
        private void SessionSearchOpenSummary(bool export)
        {
            if (dgvSessions.CurrentRow == null)
            {
                ReportStatus("Select a session in the list first.", true);
                return;
            }

            DataGridViewRow row = dgvSessions.CurrentRow;
            int sessionId;
            if (!int.TryParse(Convert.ToString(row.Cells[colSessionsId.Index].Value, CultureInfo.InvariantCulture), out sessionId))
            {
                ReportStatus("This session could not be opened. Please search again.", true);
                return;
            }

            string sessionNumber = Convert.ToString(row.Cells[colSessionsNumber.Index].Value, CultureInfo.InvariantCulture);
            if (!string.Equals(Convert.ToString(row.Cells[colSessionsStatus.Index].Value, CultureInfo.InvariantCulture),
                    SessionSearchCompletedStatus, StringComparison.OrdinalIgnoreCase))
            {
                ReportStatus("Session " + sessionNumber + " is still in progress. Its advisory summary exists only once the session is completed.", true);
                return;
            }

            string filePath = null;
            if (export)
            {
                if (!Confirm("Export advisory summary",
                        "Export the advisory summary (FP-LSR) for session " + sessionNumber + "? The export is recorded against your account.",
                        "Export"))
                {
                    return;
                }

                using (var save = new SaveFileDialog())
                {
                    save.Title = "Save advisory summary";
                    save.Filter = ExportFilterOf(LsrCode);
                    save.FileName = "FP-LSR " + sessionNumber;
                    if (save.ShowDialog(this) != DialogResult.OK)
                    {
                        return;
                    }

                    filePath = save.FileName;
                }
            }

            try
            {
                SessionSearchGenerateLearnerSummary(sessionId, sessionNumber, filePath);
            }
            catch (UnauthorizedAccessException)
            {
                ReportStatus("The advisory summary could not be saved to that folder. Choose another folder and try again.", true);
                return;
            }
            catch (Exception)
            {
                ReportStatus("The advisory summary for session " + sessionNumber + " could not be generated. Please try again.", true);
                return;
            }

            ReportStatus(export
                ? "Advisory summary for session " + sessionNumber + " exported to " + filePath + "."
                : "Advisory summary for session " + sessionNumber + " opened.", false);
        }

        #endregion

        #region Grid display (formatting only — the outcome is read back, never recalculated)

        // Format session dates, export status and advisory outcomes
        private void dgvSessions_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null || e.Value == DBNull.Value)
            {
                return;
            }

            if (e.ColumnIndex == colSessionsDate.Index && e.Value is DateTime)
            {
                e.Value = ((DateTime)e.Value).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                e.FormattingApplied = true;
            }
            else if (e.ColumnIndex == colSessionsExported.Index && e.Value is bool)
            {
                e.Value = (bool)e.Value ? "Yes" : "No";
                e.FormattingApplied = true;
            }
            else if (e.ColumnIndex == colSessionsOutcome.Index)
            {
                e.Value = SessionSearchOutcomeChip(Convert.ToString(e.Value, CultureInfo.InvariantCulture)).Text;
                e.FormattingApplied = true;
            }
        }

        // Draw a coloured outcome label with text and a border
        private void dgvSessions_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != colSessionsOutcome.Index || e.Value == null || e.Value == DBNull.Value)
            {
                return;
            }

            e.PaintBackground(e.CellBounds, true);
            SessionSearchChip chip = SessionSearchOutcomeChip(Convert.ToString(e.Value, CultureInfo.InvariantCulture));
            Font chipFont = e.CellStyle.Font ?? dgvSessions.Font;
            Size textSize = TextRenderer.MeasureText(chip.Text, chipFont);
            var box = new Rectangle(e.CellBounds.X + 6, e.CellBounds.Y + 4,
                Math.Min(textSize.Width + 12, e.CellBounds.Width - 12), e.CellBounds.Height - 9);
            using (var fill = new SolidBrush(chip.Back))
            using (var border = new Pen(chip.Border))
            {
                e.Graphics.FillRectangle(fill, box);
                e.Graphics.DrawRectangle(border, box);
            }

            TextRenderer.DrawText(e.Graphics, chip.Text, chipFont, box, chip.Fore,
                TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.EndEllipsis);
            e.Handled = true;
        }

        // Stores the text and colours used for an outcome label
        private struct SessionSearchChip
        {
            public string Text;
            public Color Fore;
            public Color Back;
            public Color Border;
        }

        // Select the text and colours for a stored advisory outcome
        private static SessionSearchChip SessionSearchOutcomeChip(string outcome)
        {
            string key = (outcome ?? string.Empty).Trim().ToUpperInvariant();
            if (key.StartsWith("STRONG", StringComparison.Ordinal))
            {
                return new SessionSearchChip { Text = "✓ Strong match", Fore = GreenColour, Back = GreenTint, Border = GreenColour };
            }

            if (key.StartsWith("POSSIBLE", StringComparison.Ordinal))
            {
                return new SessionSearchChip { Text = "! Possible match", Fore = AmberText, Back = AmberTint, Border = AmberColour };
            }

            if (key.StartsWith("NOT", StringComparison.Ordinal) || key == "NCE")
            {
                return new SessionSearchChip { Text = "✗ Not currently eligible", Fore = AmberText, Back = AmberTint, Border = AmberText };
            }

            if (key.StartsWith("MORE", StringComparison.Ordinal) || key == "MIN")
            {
                return new SessionSearchChip { Text = "? More information needed", Fore = BlueColour, Back = BlueTint, Border = BlueColour };
            }

            return new SessionSearchChip { Text = outcome, Fore = InkColour, Back = Color.White, Border = MutedColour };
        }

        #endregion
    }
}
