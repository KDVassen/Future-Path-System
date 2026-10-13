using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 10.2b — Advisory session tab of the Sessions dashboard (spec 2.3.10, read-only).
    /// The advisor searches on session number, learner surname, a session date range and an
    /// advisory outcome; none is required. On <b>Find</b>, a session number not in the form
    /// yyyy-NNNN, or a from-date later than the to-date, turns its label and control red with a
    /// field message. The grid lists matching sessions with the outcome read back from the stored
    /// RecommendationResult (never recalculated). The status band turns amber when nothing matches
    /// and blue when results show. <b>Open advisory summary</b> and <b>Export advisory summary</b>
    /// regenerate the FP-LSR report for the selected session.
    /// </summary>
    public partial class StaffConsoleForm
    {
        private const string SessionSearchCompletedStatus = "Completed";

        // Set by SessionSearchShowLearnerSessions (UI 10.1a → 10.2b); cleared by Clear or by editing the surname.
        private int? sessionSearchLearnerFilterId;
        private string sessionSearchLearnerFilterSurname;

        /// <summary>Caption of the primary button while UI 10.2b is on show.</summary>
        private string SessionSearchPrimaryText
        {
            get { return "Open advisory summary"; }
        }

        /// <summary>
        /// Called by <see cref="ShowPage"/> each time UI 10.2b appears. The first time, it prepares the
        /// grid and binds the outcome list, then shows the search guidance.
        /// </summary>
        /// <param name="firstTime">True the first time the tab is shown in this console.</param>
        private void SessionSearchShown(bool firstTime)
        {
            if (!firstTime)
            {
                return;
            }

            grdSessions.AutoGenerateColumns = false;
            lblSesResultsCount.Text = "Search to list sessions";
            SessionSearchLoadLists();
            ReportStatus("Search on any field — none is required. Tick a date box to use that date.", false);
        }

        /// <summary>Opens the FP-LSR advisory summary of the selected session (the tab's primary action).</summary>
        private void SessionSearchPrimary()
        {
            SessionSearchOpenSummary(false);
        }

        /// <summary>
        /// Lists the sessions of one learner, used when the advisor opens a learner from UI 10.1a.
        /// The caller shows this tab first.
        /// </summary>
        /// <param name="learnerId">Learner.LearnerID of the learner chosen on UI 10.1a.</param>
        /// <param name="learnerName">Display name, e.g. "Rademeyer, L"; the surname is shown in the search field.</param>
        private void SessionSearchShowLearnerSessions(int learnerId, string learnerName)
        {
            SessionSearchClearCriteria();
            string surname = (learnerName ?? string.Empty).Split(',')[0].Trim();
            txtSesSurname.Text = surname;
            sessionSearchLearnerFilterId = learnerId;
            sessionSearchLearnerFilterSurname = surname;
            SessionSearchFind();
        }

        #region Business Logic Layer hooks

        /// <summary>Binds the Advisory outcome list to stored data (never literal Items).</summary>
        private void SessionSearchLoadLists()
        {
            try
            {
                // TODO BLL: cboSesOutcome.DataSource = ReferenceDataController.GetOutcomes();
                //           cboSesOutcome.DisplayMember = "OutcomeName"; cboSesOutcome.ValueMember = "OutcomeCode";
                //           (the four outcomes of RecommendationResult.OutcomeCode; no table yet — see report)
                cboSesOutcome.SelectedIndex = -1;
            }
            catch (Exception)
            {
                ReportStatus("The outcome list could not be loaded. You can still search on the other fields.", true);
            }
        }

        /// <summary>
        /// Asks the BLL for matching sessions. Each row joins AdvisorySession, Learner, DegreeProgramme
        /// and the selected RecommendationResult: SessionID, SessionStatus, SessionNumber (yyyy-NNNN,
        /// built by the BLL), SessionDate, LearnerName, Grade, ProgrammeName, OutcomeCode (stored,
        /// never recalculated) and IsExported (an ExportReport row exists).
        /// </summary>
        private DataTable SessionSearchFindSessions(string sessionNumber, string surname, DateTime? from, DateTime? to,
            string outcomeCode, int? learnerId)
        {
            // TODO BLL: return SessionSearchController.FindSessions(sessionNumber, surname, from, to, outcomeCode, learnerId, UserId);
            //           (the BLL checks the role before returning session data — Section 8)
            return null;
        }

        /// <summary>Total number of stored sessions, for the "n of N sessions match" caption.</summary>
        private int SessionSearchCountSessions()
        {
            // TODO BLL: return SessionSearchController.CountSessions(UserId);
            return 0;
        }

        /// <summary>
        /// Regenerates the FP-LSR Learner Advisory Summary for one session. The BLL checks the role,
        /// refuses an incomplete session and writes an ExportReport row (ReportCode "FP-LSR",
        /// SessionID set, GeneratedByUserID = UserId).
        /// </summary>
        /// <param name="sessionId">AdvisorySession.SessionID of the selected row.</param>
        /// <param name="sessionNumber">The session number yyyy-NNNN, for the subtitle and the file name.</param>
        /// <param name="filePath">Where to save the export, or null to open it in the report viewer (UI 11.1a).</param>
        private void SessionSearchGenerateLearnerSummary(int sessionId, string sessionNumber, string filePath)
        {
            System.Data.DataSet data = null;
            // TODO BLL: data = ReportController.GetLearnerSummary(sessionId, UserId);   // FpLsrData
            //           The BLL checks the role and refuses an incomplete session (reports.md FP-LSR "Who generates it").
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

        /// <summary>Validates the criteria, runs the search and reports the result in the status band.</summary>
        private void SessionSearchFind()
        {
            bool numberValid = SessionSearchCheckSessionNumber();
            bool datesValid = SessionSearchCheckDateRange();
            if (!numberValid || !datesValid)
            {
                ReportStatus(!numberValid
                    ? "Session number: use the form yyyy-NNNN, e.g. 2026-0417, or leave it blank."
                    : "Session from must be on or before Session to.", true);
                (numberValid ? (Control)dtpSessionFrom : txtSesSessionNumber).Focus();
                return;
            }

            if (sessionSearchLearnerFilterId.HasValue && txtSesSurname.Text.Trim() != sessionSearchLearnerFilterSurname)
            {
                sessionSearchLearnerFilterId = null;
            }

            DateTime? from = dtpSessionFrom.Checked ? (DateTime?)dtpSessionFrom.Value.Date : null;
            DateTime? to = dtpSessionTo.Checked ? (DateTime?)dtpSessionTo.Value.Date : null;
            string outcome = cboSesOutcome.SelectedIndex >= 0 ? Convert.ToString(cboSesOutcome.SelectedValue, CultureInfo.InvariantCulture) : null;

            DataTable results;
            int total;
            try
            {
                results = SessionSearchFindSessions(txtSesSessionNumber.Text.Trim(), txtSesSurname.Text.Trim(), from, to, outcome,
                    sessionSearchLearnerFilterId);
                total = SessionSearchCountSessions();
            }
            catch (Exception)
            {
                ReportStatus("The sessions could not be loaded. Please try again.", true);
                return;
            }

            grdSessions.DataSource = results;
            int count = results == null ? 0 : results.Rows.Count;
            lblSesResultsCount.Text = count + " of " + Math.Max(total, count) + " sessions match";
            if (count == 0)
            {
                ReportStatus("No sessions match these criteria. Clear a field to widen the search.", true);
            }
            else
            {
                ReportStatus(count + (count == 1 ? " session" : " sessions") + " shown. Select one to open or export its advisory summary.", false);
            }
        }

        /// <summary>Session number is optional; when given it must be yyyy-NNNN.</summary>
        private bool SessionSearchCheckSessionNumber()
        {
            string text = txtSesSessionNumber.Text.Trim();
            bool valid = text.Length == 0 || Regex.IsMatch(text, @"^\d{4}-\d{4}$");
            return CheckField(valid, txtSesSessionNumber, lblSesSessionNumber, lblSesSessionNumberError,
                "Use the form yyyy-NNNN, e.g. 2026-0417.");
        }

        /// <summary>
        /// When both dates are ticked, Session from must not be later than Session to. Both labels turn
        /// red and share one field message (a DateTimePicker ignores BackColor, so only its label shows it).
        /// </summary>
        private bool SessionSearchCheckDateRange()
        {
            bool valid = !(dtpSessionFrom.Checked && dtpSessionTo.Checked && dtpSessionFrom.Value.Date > dtpSessionTo.Value.Date);
            const string message = "Session from is after Session to.";
            CheckField(valid, dtpSessionFrom, lblSessionFrom, lblSesDateRangeError, message);
            return CheckField(valid, dtpSessionTo, lblSessionTo, lblSesDateRangeError, message);
        }

        /// <summary>Checks the session number as soon as the field is left.</summary>
        private void txtSesSessionNumber_Leave(object sender, EventArgs e)
        {
            SessionSearchCheckSessionNumber();
        }

        /// <summary>Find validates the criteria and runs the session search.</summary>
        private void btnSesFind_Click(object sender, EventArgs e)
        {
            SessionSearchFind();
        }

        /// <summary>Clears every criterion, any field message and the result list.</summary>
        private void btnSesClear_Click(object sender, EventArgs e)
        {
            SessionSearchClearCriteria();
            ReportStatus("Search criteria cleared.", false);
            txtSesSessionNumber.Focus();
        }

        /// <summary>Returns every criterion, the learner filter, the field messages and the grid to their empty state.</summary>
        private void SessionSearchClearCriteria()
        {
            txtSesSessionNumber.Clear();
            txtSesSurname.Clear();
            dtpSessionFrom.Value = DateTime.Today;
            dtpSessionTo.Value = DateTime.Today;
            dtpSessionFrom.Checked = false;
            dtpSessionTo.Checked = false;
            cboSesOutcome.SelectedIndex = -1;
            sessionSearchLearnerFilterId = null;
            sessionSearchLearnerFilterSurname = null;
            SessionSearchCheckSessionNumber();
            SessionSearchCheckDateRange();
            grdSessions.DataSource = null;
            lblSesResultsCount.Text = "Search to list sessions";
        }

        #endregion

        #region Advisory summary (FP-LSR)

        /// <summary>Export advisory summary: confirms, asks where to save, then exports the FP-LSR.</summary>
        private void btnExportSummary_Click(object sender, EventArgs e)
        {
            SessionSearchOpenSummary(true);
        }

        /// <summary>Double-clicking a session is the same as Open advisory summary.</summary>
        private void grdSessions_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                SessionSearchOpenSummary(false);
            }
        }

        /// <summary>
        /// Opens (preview) or exports (PDF or Word, after confirmation) the FP-LSR of the selected
        /// session. A row must be selected, and the session must be completed: an in-progress session
        /// is refused with a status message.
        /// </summary>
        /// <param name="export">True to export to a file; false to open the preview.</param>
        private void SessionSearchOpenSummary(bool export)
        {
            if (grdSessions.CurrentRow == null)
            {
                ReportStatus("Select a session in the list first.", true);
                return;
            }

            DataGridViewRow row = grdSessions.CurrentRow;
            int sessionId;
            if (!int.TryParse(Convert.ToString(row.Cells[colSessionId.Index].Value, CultureInfo.InvariantCulture), out sessionId))
            {
                ReportStatus("This session could not be opened. Please search again.", true);
                return;
            }

            string sessionNumber = Convert.ToString(row.Cells[colSession.Index].Value, CultureInfo.InvariantCulture);
            if (!string.Equals(Convert.ToString(row.Cells[colSessionStatus.Index].Value, CultureInfo.InvariantCulture),
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
                    save.Filter = "PDF document (*.pdf)|*.pdf|Word document (*.docx)|*.docx";
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

        /// <summary>Formats dates as dd/MM/yyyy, Exported as Yes/No and the outcome as marker + words.</summary>
        private void grdSessions_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null || e.Value == DBNull.Value)
            {
                return;
            }

            if (e.ColumnIndex == colDate.Index && e.Value is DateTime)
            {
                e.Value = ((DateTime)e.Value).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                e.FormattingApplied = true;
            }
            else if (e.ColumnIndex == colExported.Index && e.Value is bool)
            {
                e.Value = (bool)e.Value ? "Yes" : "No";
                e.FormattingApplied = true;
            }
            else if (e.ColumnIndex == colOutcome.Index)
            {
                e.Value = SessionSearchOutcomeChip(Convert.ToString(e.Value, CultureInfo.InvariantCulture)).Text;
                e.FormattingApplied = true;
            }
        }

        /// <summary>
        /// Paints the outcome as a chip: tinted box, coloured border and marker + words, so the
        /// outcome is never shown by colour alone and never in red (spec 2.2.6).
        /// </summary>
        private void grdSessions_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != colOutcome.Index || e.Value == null || e.Value == DBNull.Value)
            {
                return;
            }

            e.PaintBackground(e.CellBounds, true);
            SessionSearchChip chip = SessionSearchOutcomeChip(Convert.ToString(e.Value, CultureInfo.InvariantCulture));
            Font chipFont = e.CellStyle.Font ?? grdSessions.Font;
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

        /// <summary>Display settings for one outcome chip on UI 10.2b.</summary>
        private struct SessionSearchChip
        {
            public string Text;
            public Color Fore;
            public Color Back;
            public Color Border;
        }

        /// <summary>
        /// Maps a stored outcome (code or name) to its marker, words and colours. Display only:
        /// Strong ✓ green · Possible ! amber · Not currently eligible ✗ dark amber (an unmet requirement is
        /// amber, never red — decisions.md §2) · More information ? blue. Matches UI 6 and UI 6.1a.
        /// </summary>
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
