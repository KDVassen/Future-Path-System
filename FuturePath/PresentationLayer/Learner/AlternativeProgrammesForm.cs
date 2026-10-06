using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 6.1a — Alternative Programmes (standard, rail step 5; spec 2.3.7, Sequence 2).
    /// Future Path ranks every <b>active</b> programme against the learner's captured subjects, marks
    /// and interest profile. Nothing is typed: selecting a ranked row rewrites the reasons panel, so
    /// every recommendation states why it is suggested, not only its rank and score. The
    /// "Include these alternative programmes" box is unticked by default and is saved to
    /// <c>AdvisorySession.AlternativesInSummary</c>. There is no validation on this screen.
    /// </summary>
    public partial class AlternativeProgrammesForm : Form
    {
        // Colours used at runtime (spec 2.2.6). Everything else is set in the Designer.
        private static readonly Color InkColour = Color.FromArgb(27, 31, 36);
        private static readonly Color BlueColour = Color.FromArgb(31, 95, 168);
        private static readonly Color BlueTint = Color.FromArgb(232, 238, 246);
        private static readonly Color AmberColour = Color.FromArgb(200, 137, 43);
        private static readonly Color AmberText = Color.FromArgb(138, 93, 24);
        private static readonly Color AmberTint = Color.FromArgb(252, 243, 224);
        private static readonly Color GreenColour = Color.FromArgb(31, 107, 58);
        private static readonly Color GreenTint = Color.FromArgb(230, 242, 234);
        private static readonly Color DeepNavy = Color.FromArgb(11, 31, 58);
        private static readonly Font ChipFont = new Font("Segoe UI Semibold", 9F);

        private const string ReadyStatus = "Step 5 of 6 · Select a programme to see why it is suggested.";

        private readonly Form homeForm;
        private readonly EligibilityResultsForm resultsForm;

        /// <summary>The programme the current list was generated for, so returning here does not re-rank.</summary>
        private int? rankedForProgrammeId;
        private bool hasRanked;

        /// <summary>Set while the stored tick-box value is being shown, so it is not saved straight back.</summary>
        private bool loadingChoice;

        /// <summary>The outcome of the learner's own selection, used in the advisory panel.</summary>
        private string selectedOutcome;

        /// <summary>Creates the Alternative Programmes screen. Only stores its arguments.</summary>
        /// <param name="homeForm">UI 1, shown again when the session is left.</param>
        /// <param name="resultsForm">UI 6, which opened this screen and opens UI 7.</param>
        public AlternativeProgrammesForm(Form homeForm, EligibilityResultsForm resultsForm)
        {
            InitializeComponent();
            this.homeForm = homeForm;
            this.resultsForm = resultsForm;

            dgvAlternatives.AutoGenerateColumns = false;
            ShowGuidance();
            SetStatus(ReadyStatus, false);
        }

        #region Session values handed on by the previous screen

        /// <summary>AdvisorySession.SessionID of the session in progress.</summary>
        public int? SessionId { get; set; }

        /// <summary>Learner.LearnerID of the learner in this session.</summary>
        public int? LearnerId { get; set; }

        /// <summary>The learner context strip text.</summary>
        public string LearnerContext { get; set; }

        /// <summary>AdvisorySession.PreferredFacultyID chosen on UI 5.</summary>
        public int? PreferredFacultyId { get; set; }

        /// <summary>AdvisorySession.SelectedProgrammeID chosen on UI 5 (excluded from the candidates).</summary>
        public int? SelectedProgrammeId { get; set; }

        /// <summary>DegreeProgramme.ProgrammeName of the learner's own selection.</summary>
        public string SelectedProgrammeName { get; set; }

        #endregion

        #region Boundary methods from the class diagram (AlternativeProgrammesView)

        /// <summary>
        /// <c>displayOutcome(outcome)</c>. UI 6 passes the learner's own outcome here so the advisory
        /// panel can name it. Sequence 2 calls it with "More Information Needed" when the learner has
        /// no captured subjects or interests; the panel then explains what is missing and the list is cleared.
        /// </summary>
        /// <param name="category">Strong Match, Possible Match, Not Currently Eligible or More Information Needed.</param>
        /// <param name="explanation">Why the outcome was reached (shown with it, never the label alone).</param>
        /// <param name="overallScore">The stored OverallScore, or null when no score was computed.</param>
        public void DisplayOutcome(string category, string explanation, double? overallScore)
        {
            if (OutcomeKind(category) == 'I')
            {
                dgvAlternatives.DataSource = null;
                lblRankedCount.Text = "No programmes ranked";
                lblReasonsHeading.Text = "Why no programmes are listed";
                lblReasons.Text = string.IsNullOrEmpty(explanation)
                    ? "Future Path needs your subjects, marks and interest questionnaire before it can rank other programmes."
                    : explanation;
                pnlNotice.BackColor = AmberTint;
                pnlNoticeBar.BackColor = AmberColour;
                lblNotice.ForeColor = AmberText;
                lblNotice.Text = "? More information needed — " + (string.IsNullOrEmpty(explanation)
                    ? "go back and complete your subjects, marks and interest questionnaire, then try again."
                    : explanation);
                btnCompare.Enabled = false;
                SetStatus("More information is needed before alternative programmes can be ranked.", true);
                return;
            }

            // The score is shown in the grid row; the panel names the outcome in words.
            selectedOutcome = OutcomeWords(category).ToLower(CultureInfo.CurrentCulture);

            ShowGuidance();
        }

        /// <summary>
        /// <c>displayAlternatives(rankedAlternatives)</c>. Binds the ranked RecommendationResult rows
        /// (AlternativeRank, ProgrammeName, FacultyName, OverallScore, OutcomeCode, OutcomeReason,
        /// ProgrammeID, IsSelectedProgramme) to the grid. The learner's own selection may be included,
        /// with no rank, so it can be compared.
        /// </summary>
        /// <param name="rankedAlternatives">Any list or table the grid can bind to, already ranked by the BLL.</param>
        public void DisplayAlternatives(object rankedAlternatives)
        {
            btnCompare.Enabled = true;
            dgvAlternatives.DataSource = rankedAlternatives;

            int ranked = dgvAlternatives.Rows.Cast<DataGridViewRow>().Count(r => !IsSelectedRow(r));
            lblRankedCount.Text = ranked == 1 ? "1 active programme ranked" : ranked + " active programmes ranked";
            if (dgvAlternatives.Rows.Count == 0)
            {
                lblReasonsHeading.Text = "Why each programme is suggested";
                lblReasons.Text = "No other active programme could be ranked for your profile.";
            }
        }

        #endregion

        #region Business Logic Layer hooks

        /// <summary>
        /// Runs Sequence 2 for the learner's selection, unless the list was already ranked for it,
        /// and shows the stored "include in summary" choice.
        /// </summary>
        private void LoadAlternatives()
        {
            if (hasRanked && rankedForProgrammeId == SelectedProgrammeId)
            {
                return;
            }

            try
            {
                // TODO BLL: var ranked = RecommendationController.GenerateAlternatives(LearnerId.Value, SelectedProgrammeId.Value);
                //           (active programmes only; the results are saved with AlternativeRank by AdvisorySessionRepository.saveAll)
                //           if (ranked.MoreInformationNeeded)
                //               DisplayOutcome("More Information Needed", ranked.Explanation, null);
                //           else
                //               DisplayAlternatives(ranked.Results);   // plus the selected programme's stored row (IsSelectedProgramme = 1)
                // TODO BLL: loadingChoice = true;
                //           chkIncludeAlternatives.Checked = RecommendationController.GetAlternativesInSummary(SessionId.Value);
                //           loadingChoice = false;
                hasRanked = true;
                rankedForProgrammeId = SelectedProgrammeId;
            }
            catch (Exception)
            {
                loadingChoice = false;
                SetStatus("Alternative programmes could not be loaded. Go back and try again.", true);
            }
        }

        /// <summary>Saves the tick box to AdvisorySession.AlternativesInSummary.</summary>
        private void SaveAlternativesChoice()
        {
            // TODO BLL: RecommendationController.SetAlternativesInSummary(SessionId.Value, chkIncludeAlternatives.Checked);
        }

        #endregion

        #region Grid display (formatting only; ranking and outcomes come from the BLL)

        /// <summary>Selecting a row rewrites the reasons panel for that programme.</summary>
        private void dgvAlternatives_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAlternatives.CurrentRow == null)
            {
                return;
            }

            ShowReason(dgvAlternatives.CurrentRow);
        }

        /// <summary>Selects the first row once data arrives, so a reason is always showing.</summary>
        private void dgvAlternatives_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (dgvAlternatives.Rows.Count > 0)
            {
                dgvAlternatives.CurrentCell = dgvAlternatives.Rows[0].Cells[colProgramme.Index];
                ShowReason(dgvAlternatives.Rows[0]);
            }
        }

        /// <summary>
        /// Shows the outcome as a chip (marker + words, tinted, never red and never colour alone),
        /// marks the learner's own selection, and shows a dash where the selection has no rank.
        /// </summary>
        private void dgvAlternatives_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            DataGridViewRow row = dgvAlternatives.Rows[e.RowIndex];
            if (e.ColumnIndex == colOutcome.Index && e.Value != null)
            {
                string code = Convert.ToString(e.Value, CultureInfo.InvariantCulture);
                e.Value = OutcomeMarker(code) + " " + OutcomeWords(code);
                char kind = OutcomeKind(code);
                e.CellStyle.Font = ChipFont;
                e.CellStyle.ForeColor = kind == 'S' ? GreenColour : (kind == 'I' ? BlueColour : AmberText);
                e.CellStyle.BackColor = kind == 'S' ? GreenTint : (kind == 'I' ? BlueTint : AmberTint);
                e.FormattingApplied = true;
            }
            else if (e.ColumnIndex == colProgramme.Index && IsSelectedRow(row))
            {
                e.Value = Convert.ToString(e.Value, CultureInfo.CurrentCulture) + " · your selection";
                e.FormattingApplied = true;
            }
            else if (e.ColumnIndex == colRank.Index && (e.Value == null || e.Value == DBNull.Value))
            {
                e.Value = "—";
                e.FormattingApplied = true;
            }
        }

        /// <summary>Writes "Why rank n is suggested — programme" and the stored reason into the reasons panel.</summary>
        private void ShowReason(DataGridViewRow row)
        {
            string programme = Convert.ToString(row.Cells[colProgramme.Index].Value, CultureInfo.CurrentCulture);
            string reason = Convert.ToString(row.Cells[colReason.Index].Value, CultureInfo.CurrentCulture);
            object rank = row.Cells[colRank.Index].Value;

            lblReasonsHeading.Text = IsSelectedRow(row) || rank == null || rank == DBNull.Value
                ? "How your own selection compares — " + programme
                : "Why rank " + Convert.ToString(rank, CultureInfo.CurrentCulture) + " is suggested — " + programme;
            lblReasons.Text = string.IsNullOrEmpty(reason) ? "No reason was recorded for this programme." : reason;
        }

        /// <summary>True when the row is the learner's own programme from UI 5.</summary>
        private bool IsSelectedRow(DataGridViewRow row)
        {
            object flag = row.Cells[colIsSelected.Index].Value;
            if (flag is bool && (bool)flag)
            {
                return true;
            }

            object id = row.Cells[colProgrammeId.Index].Value;
            return SelectedProgrammeId.HasValue && id != null && id != DBNull.Value
                   && Convert.ToInt32(id, CultureInfo.InvariantCulture) == SelectedProgrammeId.Value;
        }

        /// <summary>Classifies a stored OutcomeCode for display: S strong, P possible, N not eligible, I more information.</summary>
        private static char OutcomeKind(string code)
        {
            string c = (code ?? string.Empty).Trim().ToUpperInvariant();
            if (c.StartsWith("STRONG") || c == "SM") return 'S';
            if (c.StartsWith("POSSIBLE") || c == "PM") return 'P';
            if (c.StartsWith("NOT") || c == "NCE") return 'N';
            return 'I';
        }

        /// <summary>The words shown for an outcome.</summary>
        private static string OutcomeWords(string code)
        {
            switch (OutcomeKind(code))
            {
                case 'S': return "Strong match";
                case 'P': return "Possible match";
                case 'N': return "Not currently eligible";
                default: return "More information needed";
            }
        }

        /// <summary>The marker shown with an outcome (✓ ! ✗ ?), so colour is never the only signal.</summary>
        private static string OutcomeMarker(string code)
        {
            switch (OutcomeKind(code))
            {
                case 'S': return "✓";
                case 'P': return "!";
                case 'N': return "✗";
                default: return "?";
            }
        }

        #endregion

        #region Advisory panel and status band

        private void ShowGuidance()
        {
            string selection = string.IsNullOrEmpty(SelectedProgrammeName) ? "Your selected programme" : SelectedProgrammeName;
            string opening = string.IsNullOrEmpty(selectedOutcome)
                ? "Every active programme"
                : selection + " returned a " + selectedOutcome + ", so every active programme";
            pnlNotice.BackColor = BlueTint;
            pnlNoticeBar.BackColor = BlueColour;
            lblNotice.ForeColor = InkColour;
            lblNotice.Text = opening + " has been ranked against your subjects, marks and interests. " +
                             "Your own selection stays in the list for comparison.";
        }

        /// <summary>Status band: deep navy for normal messages, amber for a problem (never exception text).</summary>
        private void SetStatus(string message, bool warning)
        {
            pnlStatus.BackColor = warning ? AmberColour : DeepNavy;
            lblStatus.ForeColor = warning ? InkColour : Color.White;
            lblStatus.Text = message;
        }

        #endregion

        #region Buttons and navigation

        /// <summary>Saves the learner's include / leave-out choice as soon as it changes.</summary>
        private void chkIncludeAlternatives_CheckedChanged(object sender, EventArgs e)
        {
            if (loadingChoice)
            {
                return;
            }

            try
            {
                SaveAlternativesChoice();
                SetStatus(chkIncludeAlternatives.Checked
                    ? "These alternative programmes will be printed in your advisory summary."
                    : "Alternative programmes will be left out of your advisory summary.", false);
            }
            catch (Exception)
            {
                SetStatus("Your choice could not be saved. Please try again.", true);
            }
        }

        /// <summary>Compare with my selection: selects and shows the learner's own programme row. Changes nothing.</summary>
        private void btnCompare_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvAlternatives.Rows)
            {
                if (IsSelectedRow(row))
                {
                    dgvAlternatives.CurrentCell = row.Cells[colProgramme.Index];
                    row.Selected = true;
                    ShowReason(row);
                    SetStatus("Your own selection is highlighted in the list. Compare its fit and reason with the ranked programmes.", false);
                    return;
                }
            }

            SetStatus("Your selected programme is not in this list. Go back to see its outcome on Eligibility results.", true);
        }

        /// <summary>Continue to summary → UI 7, through UI 6 so the session values travel with it.</summary>
        private void btnContinue_Click(object sender, EventArgs e)
        {
            resultsForm.OpenSummary(this);
        }

        /// <summary>Back → UI 6. Nothing changes.</summary>
        private void btnBack_Click(object sender, EventArgs e)
        {
            resultsForm.Show();
            Hide();
        }

        /// <summary>Home on the rail leaves the session after confirmation.</summary>
        private void btnNavHome_Click(object sender, EventArgs e)
        {
            LeaveSession();
        }

        /// <summary>
        /// Asks for confirmation, returns to UI 1 and closes every learner screen. Anything already
        /// saved stays on record as an "In progress" session (decisions.md D3).
        /// </summary>
        private void LeaveSession()
        {
            using (var dialog = new ConfirmationDialog("Leave this advisory session",
                "Return to the homepage? Anything already saved stays on record as an \"In progress\" session. " +
                "Your session is only completed when you export the advisory summary.", "Leave session"))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }
            }

            homeForm.Show();
            foreach (Form screen in Application.OpenForms.Cast<Form>().ToList())
            {
                if (screen != homeForm)
                {
                    screen.Close();
                }
            }
        }

        #endregion

        #region Showing and closing

        /// <summary>Shows the learner context and ranks the programmes when the screen appears.</summary>
        private void AlternativeProgrammesForm_VisibleChanged(object sender, EventArgs e)
        {
            if (!Visible)
            {
                return;
            }

            lblContext.Text = LearnerContext;
            SetStatus(ReadyStatus, false);
            ShowGuidance();
            LoadAlternatives();
        }

        /// <summary>The caption-bar X asks first, because it closes Future Path.</summary>
        private void AlternativeProgrammesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing || homeForm.Visible)
            {
                return;
            }

            using (var dialog = new ConfirmationDialog("Close Future Path",
                "Close Future Path? Anything already saved stays on record as an \"In progress\" session.", "Close"))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>Ends the application when this screen was closed with the caption-bar X.</summary>
        private void AlternativeProgrammesForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!homeForm.Visible)
            {
                Application.Exit();
            }
        }

        #endregion
    }
}
