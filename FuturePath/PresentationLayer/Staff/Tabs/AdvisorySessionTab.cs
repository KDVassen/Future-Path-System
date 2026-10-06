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
    public partial class AdvisorySessionTab : UserControl
    {
        // Colours used at runtime (spec 2.2.6). Everything else is set in the Designer.
        private static readonly Color InkColour = Color.FromArgb(27, 31, 36);
        private static readonly Color RedColour = Color.FromArgb(164, 38, 44);
        private static readonly Color RedTint = Color.FromArgb(250, 234, 235);
        private static readonly Color GreenColour = Color.FromArgb(31, 107, 58);
        private static readonly Color GreenTint = Color.FromArgb(230, 242, 234);
        private static readonly Color AmberColour = Color.FromArgb(200, 137, 43);
        private static readonly Color AmberText = Color.FromArgb(138, 93, 24);
        private static readonly Color AmberTint = Color.FromArgb(252, 243, 224);
        private static readonly Color BlueColour = Color.FromArgb(31, 95, 168);
        private static readonly Color BlueTint = Color.FromArgb(232, 238, 246);
        private static readonly Color MutedColour = Color.FromArgb(88, 96, 108);
        private static readonly Color NeutralTint = Color.FromArgb(237, 239, 242);

        private const string CompletedStatus = "Completed";

        // Set by ShowLearnerSessions (UI 10.1a → 10.2b); cleared by Clear or by editing the surname.
        private int? learnerFilterId;
        private string learnerFilterSurname;

        /// <summary>Creates the tab. The host sets <see cref="UserId"/> and <see cref="Role"/> before <see cref="LoadData"/>.</summary>
        public AdvisorySessionTab()
        {
            InitializeComponent();
            grdSessions.AutoGenerateColumns = false;
            lblResultsCount.Text = "Search to list sessions";
        }

        #region Tab contract

        /// <summary>UserAccount.UserID of the signed-in staff member (logged on ExportReport).</summary>
        public int UserId { get; set; }

        /// <summary>UserAccount.Role of the signed-in staff member ("Career Advisor" or "Administrator").</summary>
        public string Role { get; set; }

        /// <summary>Raised with a status message; isWarning = true shows amber, false shows blue.</summary>
        public event Action<string, bool> StatusChanged;

        /// <summary>Caption of the host's primary button while this tab is active.</summary>
        public string PrimaryActionText
        {
            get { return "Open advisory summary"; }
        }

        /// <summary>Binds the outcome list. The host calls this when the tab is first shown.</summary>
        public void LoadData()
        {
            LoadLists();
            RaiseStatus("Search on any field — none is required. Tick a date box to use that date.", false);
        }

        /// <summary>Opens the FP-LSR advisory summary of the selected session (the tab's primary action).</summary>
        public void RunPrimaryAction()
        {
            OpenSummary(false);
        }

        /// <summary>
        /// Lists the sessions of one learner, used when the advisor opens a learner from UI 10.1a.
        /// </summary>
        /// <param name="learnerId">Learner.LearnerID of the learner chosen on UI 10.1a.</param>
        /// <param name="learnerName">Display name, e.g. "Rademeyer, L"; the surname is shown in the search field.</param>
        public void ShowLearnerSessions(int learnerId, string learnerName)
        {
            ClearCriteria();
            string surname = (learnerName ?? string.Empty).Split(',')[0].Trim();
            txtSurname.Text = surname;
            learnerFilterId = learnerId;
            learnerFilterSurname = surname;
            Find();
        }

        #endregion

        #region Business Logic Layer hooks

        /// <summary>Binds the Advisory outcome list to stored data (never literal Items).</summary>
        private void LoadLists()
        {
            try
            {
                // TODO BLL: cboOutcome.DataSource = ReferenceDataController.GetOutcomes();
                //           cboOutcome.DisplayMember = "OutcomeName"; cboOutcome.ValueMember = "OutcomeCode";
                //           (the four outcomes of RecommendationResult.OutcomeCode; no table yet — see report)
                cboOutcome.SelectedIndex = -1;
            }
            catch (Exception)
            {
                RaiseStatus("The outcome list could not be loaded. You can still search on the other fields.", true);
            }
        }

        /// <summary>
        /// Asks the BLL for matching sessions. Each row joins AdvisorySession, Learner, DegreeProgramme
        /// and the selected RecommendationResult: SessionID, SessionStatus, SessionNumber (yyyy-NNNN,
        /// built by the BLL), SessionDate, LearnerName, Grade, ProgrammeName, OutcomeCode (stored,
        /// never recalculated) and IsExported (an ExportReport row exists).
        /// </summary>
        private DataTable FindSessions(string sessionNumber, string surname, DateTime? from, DateTime? to, string outcomeCode, int? learnerId)
        {
            // TODO BLL: return SessionSearchController.FindSessions(sessionNumber, surname, from, to, outcomeCode, learnerId, UserId);
            //           (the BLL checks the role before returning session data — Section 8)
            return null;
        }

        /// <summary>Total number of stored sessions, for the "n of N sessions match" caption.</summary>
        private int CountSessions()
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
        /// <param name="filePath">Where to save the export, or null to open it in the report viewer.</param>
        private void GenerateLearnerSummary(int sessionId, string filePath)
        {
            // TODO BLL: if (filePath == null)
            //               ReportController.GenerateLearnerSummary(sessionId, UserId, ReportOutput.Preview, null);
            //           else
            //               ReportController.GenerateLearnerSummary(sessionId, UserId,
            //                   filePath.EndsWith(".docx", StringComparison.OrdinalIgnoreCase) ? ReportOutput.Word : ReportOutput.Pdf, filePath);
            //           The preview opens in the RDLC ReportViewer (reports.md, "Technology").
        }

        #endregion

        #region Search and validation

        /// <summary>Validates the criteria, runs the search and reports the result in the status band.</summary>
        private void Find()
        {
            bool numberValid = CheckSessionNumber();
            bool datesValid = CheckDateRange();
            if (!numberValid || !datesValid)
            {
                RaiseStatus(!numberValid
                    ? "Session number: use the form yyyy-NNNN, e.g. 2026-0417, or leave it blank."
                    : "Session from must be on or before Session to.", true);
                (numberValid ? (Control)dtpSessionFrom : txtSessionNumber).Focus();
                return;
            }

            if (learnerFilterId.HasValue && txtSurname.Text.Trim() != learnerFilterSurname)
            {
                learnerFilterId = null;
            }

            DateTime? from = dtpSessionFrom.Checked ? (DateTime?)dtpSessionFrom.Value.Date : null;
            DateTime? to = dtpSessionTo.Checked ? (DateTime?)dtpSessionTo.Value.Date : null;
            string outcome = cboOutcome.SelectedIndex >= 0 ? Convert.ToString(cboOutcome.SelectedValue, CultureInfo.InvariantCulture) : null;

            DataTable results;
            int total;
            try
            {
                results = FindSessions(txtSessionNumber.Text.Trim(), txtSurname.Text.Trim(), from, to, outcome, learnerFilterId);
                total = CountSessions();
            }
            catch (Exception)
            {
                RaiseStatus("The sessions could not be loaded. Please try again.", true);
                return;
            }

            grdSessions.DataSource = results;
            int count = results == null ? 0 : results.Rows.Count;
            lblResultsCount.Text = count + " of " + Math.Max(total, count) + " sessions match";
            if (count == 0)
            {
                RaiseStatus("No sessions match these criteria. Clear a field to widen the search.", true);
            }
            else
            {
                RaiseStatus(count + (count == 1 ? " session" : " sessions") + " shown. Select one to open or export its advisory summary.", false);
            }
        }

        /// <summary>Session number is optional; when given it must be yyyy-NNNN.</summary>
        private bool CheckSessionNumber()
        {
            string text = txtSessionNumber.Text.Trim();
            bool valid = text.Length == 0 || Regex.IsMatch(text, @"^\d{4}-\d{4}$");
            return CheckField(valid, new Control[] { txtSessionNumber }, new[] { lblSessionNumber }, lblSessionNumberError,
                "Use the form yyyy-NNNN, e.g. 2026-0417.");
        }

        /// <summary>When both dates are ticked, Session from must not be later than Session to.</summary>
        private bool CheckDateRange()
        {
            bool valid = !(dtpSessionFrom.Checked && dtpSessionTo.Checked && dtpSessionFrom.Value.Date > dtpSessionTo.Value.Date);
            return CheckField(valid, new Control[] { dtpSessionFrom, dtpSessionTo }, new[] { lblSessionFrom, lblSessionTo },
                lblDateRangeError, "Session from is after Session to.");
        }

        /// <summary>
        /// Shows or clears a field message (spec 2.2.7): red caption under the control, red label and
        /// red-tinted control. A DateTimePicker ignores BackColor, so only its label turns red.
        /// </summary>
        private static bool CheckField(bool valid, Control[] inputs, Label[] fieldLabels, Label messageLabel, string message)
        {
            foreach (Control input in inputs)
            {
                input.BackColor = valid ? SystemColors.Window : RedTint;
                input.AccessibleDescription = valid ? null : message;
            }

            foreach (Label fieldLabel in fieldLabels)
            {
                fieldLabel.ForeColor = valid ? InkColour : RedColour;
            }

            messageLabel.Text = valid ? string.Empty : "⚠ " + message;
            messageLabel.Visible = !valid;
            return valid;
        }

        private void txtSessionNumber_Leave(object sender, EventArgs e)
        {
            CheckSessionNumber();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            Find();
        }

        /// <summary>Clears every criterion, any field message and the result list.</summary>
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearCriteria();
            RaiseStatus("Search criteria cleared.", false);
            txtSessionNumber.Focus();
        }

        private void ClearCriteria()
        {
            txtSessionNumber.Clear();
            txtSurname.Clear();
            dtpSessionFrom.Value = DateTime.Today;
            dtpSessionTo.Value = DateTime.Today;
            dtpSessionFrom.Checked = false;
            dtpSessionTo.Checked = false;
            cboOutcome.SelectedIndex = -1;
            learnerFilterId = null;
            learnerFilterSurname = null;
            CheckSessionNumber();
            CheckDateRange();
            grdSessions.DataSource = null;
            lblResultsCount.Text = "Search to list sessions";
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

        #endregion

        #region Advisory summary (FP-LSR)

        private void btnExportSummary_Click(object sender, EventArgs e)
        {
            OpenSummary(true);
        }

        private void grdSessions_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                OpenSummary(false);
            }
        }

        /// <summary>
        /// Opens (preview) or exports (PDF or Word, after confirmation) the FP-LSR of the selected
        /// session. A row must be selected, and the session must be completed.
        /// </summary>
        private void OpenSummary(bool export)
        {
            if (grdSessions.CurrentRow == null)
            {
                RaiseStatus("Select a session in the list first.", true);
                return;
            }

            DataGridViewRow row = grdSessions.CurrentRow;
            int sessionId;
            if (!int.TryParse(Convert.ToString(row.Cells[colSessionId.Index].Value, CultureInfo.InvariantCulture), out sessionId))
            {
                RaiseStatus("This session could not be opened. Please search again.", true);
                return;
            }

            string sessionNumber = Convert.ToString(row.Cells[colSession.Index].Value, CultureInfo.InvariantCulture);
            if (!string.Equals(Convert.ToString(row.Cells[colSessionStatus.Index].Value, CultureInfo.InvariantCulture),
                    CompletedStatus, StringComparison.OrdinalIgnoreCase))
            {
                RaiseStatus("Session " + sessionNumber + " is still in progress. Its advisory summary exists only once the session is completed.", true);
                return;
            }

            string filePath = null;
            if (export)
            {
                using (var confirm = new ConfirmationDialog("Export advisory summary",
                    "Export the advisory summary (FP-LSR) for session " + sessionNumber + "? The export is recorded against your account.",
                    "Export"))
                {
                    if (confirm.ShowDialog(this) != DialogResult.OK)
                    {
                        return;
                    }
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
                GenerateLearnerSummary(sessionId, filePath);
            }
            catch (UnauthorizedAccessException)
            {
                RaiseStatus("The advisory summary could not be saved to that folder. Choose another folder and try again.", true);
                return;
            }
            catch (Exception)
            {
                RaiseStatus("The advisory summary for session " + sessionNumber + " could not be generated. Please try again.", true);
                return;
            }

            RaiseStatus(export
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
                e.Value = OutcomeChip(Convert.ToString(e.Value, CultureInfo.InvariantCulture)).Text;
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
            Chip chip = OutcomeChip(Convert.ToString(e.Value, CultureInfo.InvariantCulture));
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

        /// <summary>Display settings for one outcome chip.</summary>
        private struct Chip
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
        private static Chip OutcomeChip(string outcome)
        {
            string key = (outcome ?? string.Empty).Trim().ToUpperInvariant();
            if (key.StartsWith("STRONG", StringComparison.Ordinal))
            {
                return new Chip { Text = "✓ Strong match", Fore = GreenColour, Back = GreenTint, Border = GreenColour };
            }

            if (key.StartsWith("POSSIBLE", StringComparison.Ordinal))
            {
                return new Chip { Text = "! Possible match", Fore = AmberText, Back = AmberTint, Border = AmberColour };
            }

            if (key.StartsWith("NOT", StringComparison.Ordinal) || key == "NCE")
            {
                return new Chip { Text = "✗ Not currently eligible", Fore = AmberText, Back = AmberTint, Border = AmberText };
            }

            if (key.StartsWith("MORE", StringComparison.Ordinal) || key == "MIN")
            {
                return new Chip { Text = "? More information needed", Fore = BlueColour, Back = BlueTint, Border = BlueColour };
            }

            return new Chip { Text = outcome, Fore = InkColour, Back = Color.White, Border = MutedColour };
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
