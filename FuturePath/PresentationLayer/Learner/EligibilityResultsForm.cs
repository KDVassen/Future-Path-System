using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 6 — Eligibility Results (standard, step 5 of 6; spec 2.3.6). The outcome screen for
    /// Sequence 1 (class diagram: EligibilityCheckView.displayOutcome). Nothing is entered here.
    /// The outcome band carries a status chip (marker + words) and always states <b>why</b> the
    /// outcome was reached. Green marks a requirement met, amber a gap or "not currently
    /// eligible", grey-blue "more information needed" — red is never used for an outcome
    /// (decisions.md §2). Back, Funding options and Alternative programmes leave the stored outcome
    /// unchanged, and Continue to summary carries it through to UI 7.
    /// </summary>
    public partial class EligibilityResultsForm : Form
    {
        // Colours used at runtime (spec 2.2.6). Everything else is set in the Designer.
        private static readonly Color InkColour = Color.FromArgb(27, 31, 36);
        private static readonly Color GreenColour = Color.FromArgb(31, 107, 58);
        private static readonly Color GreenTint = Color.FromArgb(230, 242, 234);
        private static readonly Color AmberColour = Color.FromArgb(200, 137, 43);
        private static readonly Color AmberText = Color.FromArgb(138, 93, 24);
        private static readonly Color AmberTint = Color.FromArgb(252, 243, 224);
        private static readonly Color GreyBlue = Color.FromArgb(91, 107, 130);
        private static readonly Color BlueTint = Color.FromArgb(232, 238, 246);
        private static readonly Color DeepNavy = Color.FromArgb(11, 31, 58);

        private const string ReadyStatus =
            "Step 5 of 6 · Preliminary guidance only. Continue to the summary, or look at alternatives and funding first.";

        private readonly Form homeForm;
        private readonly Form previousForm;
        private AlternativeProgrammesForm alternativesForm;
        private FundingOptionsForm fundingForm;
        private AdvisorySummaryForm summaryForm;

        private string outcomeCategory;
        private string outcomeExplanation;
        private double? outcomeScore;

        /// <summary>Creates the eligibility results screen. Only stores its arguments.</summary>
        /// <param name="homeForm">UI 1, shown again when the session is left.</param>
        /// <param name="previousForm">UI 5, shown again by Back to change the programme.</param>
        public EligibilityResultsForm(Form homeForm, Form previousForm)
        {
            InitializeComponent();
            this.homeForm = homeForm;
            this.previousForm = previousForm;
            DisplayOutcome(null, null, null);
        }

        #region Session values handed to the next screens

        /// <summary>AdvisorySession.SessionID of the in-progress session.</summary>
        public int? SessionId { get; set; }

        /// <summary>Learner.LearnerID of the learner in this session.</summary>
        public int? LearnerId { get; set; }

        /// <summary>The learner context strip text built on UI 2.</summary>
        public string LearnerContext { get; set; }

        /// <summary>AdvisorySession.PreferredFacultyID chosen on UI 5.</summary>
        public int? PreferredFacultyId { get; set; }

        /// <summary>AdvisorySession.SelectedProgrammeID chosen on UI 5.</summary>
        public int? SelectedProgrammeId { get; set; }

        /// <summary>DegreeProgramme.ProgrammeName of the selected programme.</summary>
        public string SelectedProgrammeName { get; set; }

        #endregion

        #region Displaying the outcome (class diagram: displayOutcome)

        /// <summary>
        /// Shows the AdvisoryOutcome returned by EligibilityController.CheckEligibility: fills the
        /// outcome band, its status chip and the Overall score tile. An empty category means the
        /// check could not be completed, and the "More information needed" wording is shown.
        /// </summary>
        /// <param name="category">AdvisoryOutcome.category (RecommendationResult.OutcomeCode).</param>
        /// <param name="explanation">AdvisoryOutcome.explanation (RecommendationResult.OutcomeReason).</param>
        /// <param name="overallScore">AdvisoryOutcome.overallScore (0–100), or null when not calculated.</param>
        public void DisplayOutcome(string category, string explanation, double? overallScore)
        {
            outcomeCategory = category;
            outcomeExplanation = explanation;
            outcomeScore = overallScore;

            string programme = string.IsNullOrWhiteSpace(SelectedProgrammeName) ? "the selected programme" : SelectedProgrammeName;
            string key = string.IsNullOrWhiteSpace(category) ? string.Empty : category.Replace(" ", string.Empty).ToUpperInvariant();
            string chip, headline, fallback;
            Color back, bar, chipBack, chipFore;

            switch (key)
            {
                case "STRONGMATCH":
                case "STRONG":
                    chip = "✓ Strong match";
                    headline = "You are a strong match for " + programme;
                    fallback = "You appear to meet the minimum requirements and your interests align strongly with this programme.";
                    back = GreenTint; bar = GreenColour; chipBack = GreenColour; chipFore = Color.White;
                    break;
                case "POSSIBLEMATCH":
                case "POSSIBLE":
                    chip = "! Possible match";
                    headline = "You are a possible match for " + programme;
                    fallback = "You meet some of the requirements, but there are one or more gaps or a weaker interest alignment.";
                    back = AmberTint; bar = AmberColour; chipBack = AmberColour; chipFore = InkColour;
                    break;
                case "NOTCURRENTLYELIGIBLE":
                case "NOTELIGIBLE":
                    chip = "✗ Not currently eligible";
                    headline = "You are not currently eligible for " + programme;
                    fallback = "You do not currently meet one or more important requirements for this programme.";
                    back = AmberTint; bar = AmberText; chipBack = AmberText; chipFore = Color.White;
                    break;
                default:
                    chip = "? More information needed";
                    headline = "More information is needed to check " + programme;
                    fallback = "The check could not be completed because some required information is missing, such as a " +
                               "subject, a mark or a questionnaire answer. Go back to complete the earlier steps, then check again.";
                    back = BlueTint; bar = GreyBlue; chipBack = GreyBlue; chipFore = Color.White;
                    break;
            }

            pnlOutcome.BackColor = back;
            pnlOutcomeBar.BackColor = bar;
            lblOutcomeChip.BackColor = chipBack;
            lblOutcomeChip.ForeColor = chipFore;
            lblOutcomeChip.Text = chip;
            lblOutcomeHeadline.Text = headline;
            lblOutcomeExplanation.Text = string.IsNullOrWhiteSpace(explanation) ? fallback : explanation.Trim();
            lblOutcomeChip.AccessibleDescription = headline + ". " + lblOutcomeExplanation.Text;

            pnlOverallTileValue.Text = overallScore.HasValue
                ? overallScore.Value.ToString("0", CultureInfo.CurrentCulture)
                : "—";
        }

        /// <summary>
        /// Shows the three component figures on their running-figure tiles (display only; the
        /// figures and notes come from the stored RecommendationResult row).
        /// </summary>
        private void ShowFigures(double? subjectScore, double? markScore, double? interestScore)
        {
            pnlSubjectTileValue.Text = FormatFigure(subjectScore);
            pnlMarkTileValue.Text = FormatFigure(markScore);
            pnlInterestTileValue.Text = FormatFigure(interestScore);
        }

        private static string FormatFigure(double? value)
        {
            return value.HasValue ? value.Value.ToString("0", CultureInfo.CurrentCulture) : "—";
        }

        /// <summary>
        /// Colours and marks the Outcome column: green with ✓ for a requirement met, amber text
        /// with ! for a gap. Words are always shown, so colour is never the only signal.
        /// </summary>
        private void dgvChecked_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != colRequirementOutcome.Index || e.Value == null)
            {
                return;
            }

            object met = dgvChecked.Rows[e.RowIndex].Cells[colIsMet.Index].Value;
            bool isMet = met is bool && (bool)met;
            e.Value = (isMet ? "✓ " : "! ") + e.Value;
            e.CellStyle.ForeColor = isMet ? GreenColour : AmberText;
            e.CellStyle.SelectionForeColor = Color.White;
            e.FormattingApplied = true;
        }

        /// <summary>
        /// Clears the default first-row highlight after binding, so the blue selection never hides
        /// a coloured cell or suggests a choice the learner did not make.
        /// </summary>
        private void Grid_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            ((DataGridView)sender).ClearSelection();
        }

        #endregion

        #region Business Logic Layer hooks

        /// <summary>
        /// Loads the requirement-by-requirement check and the component scores for the selected
        /// programme from the stored RecommendationResult (written by Sequence 1).
        /// </summary>
        private void LoadCheckedRequirements()
        {
            if (!SessionId.HasValue || !SelectedProgrammeId.HasValue)
            {
                return;
            }

            try
            {
                // TODO BLL: dgvChecked.DataSource = EligibilityController.GetCheckedRequirements(SessionId.Value, SelectedProgrammeId.Value);
                //           (rows: SubjectName, MinimumMark, Mark, RequirementOutcome, IsMet — from MatchResult)
                // TODO BLL: var result = EligibilityController.GetSelectedResult(SessionId.Value);
                //           ShowFigures(result.SubjectMatchScore, result.MarkMatchScore, result.InterestMatchScore);
                //           pnlSubjectTileNote.Text = result.SubjectNote; pnlMarkTileNote.Text = result.MarkNote;
                //           pnlInterestTileNote.Text = result.StrongestCategory;
            }
            catch (Exception)
            {
                SetStatus("The requirement check could not be loaded. The outcome above is unchanged.", AmberColour, InkColour);
            }
        }

        #endregion

        #region Status band

        private void SetStatus(string message, Color back, Color fore)
        {
            pnlStatus.BackColor = back;
            lblStatus.ForeColor = fore;
            lblStatus.Text = message;
        }

        #endregion

        #region Buttons and navigation (none of them changes the stored outcome)

        /// <summary>Continue to summary → UI 7, carrying the outcome through.</summary>
        private void btnContinue_Click(object sender, EventArgs e)
        {
            OpenSummary(this);
        }

        /// <summary>
        /// Opens (or reuses) UI 7 for this session. Called from UI 6, UI 6.1a and UI 6.2b, so that
        /// UI 7's Back returns to whichever screen opened it.
        /// </summary>
        /// <param name="from">The screen that is hidden and that UI 7's Back returns to.</param>
        public void OpenSummary(Form from)
        {
            if (summaryForm == null || summaryForm.IsDisposed)
            {
                summaryForm = new AdvisorySummaryForm(homeForm, this);
            }

            summaryForm.PreviousForm = from;
            summaryForm.SessionId = SessionId;
            summaryForm.LearnerId = LearnerId;
            summaryForm.LearnerContext = LearnerContext;
            summaryForm.PreferredFacultyId = PreferredFacultyId;
            summaryForm.SelectedProgrammeId = SelectedProgrammeId;
            summaryForm.SelectedProgrammeName = SelectedProgrammeName;
            summaryForm.Show();
            from.Hide();
        }

        /// <summary>Alternative programmes → UI 6.1a, where Sequence 2 runs.</summary>
        private void btnAlternatives_Click(object sender, EventArgs e)
        {
            if (alternativesForm == null || alternativesForm.IsDisposed)
            {
                alternativesForm = new AlternativeProgrammesForm(homeForm, this);
            }

            alternativesForm.SessionId = SessionId;
            alternativesForm.LearnerId = LearnerId;
            alternativesForm.LearnerContext = LearnerContext;
            alternativesForm.PreferredFacultyId = PreferredFacultyId;
            alternativesForm.SelectedProgrammeId = SelectedProgrammeId;
            alternativesForm.SelectedProgrammeName = SelectedProgrammeName;
            alternativesForm.DisplayOutcome(outcomeCategory, outcomeExplanation, outcomeScore);
            alternativesForm.Show();
            Hide();
        }

        /// <summary>Funding options → UI 6.2b.</summary>
        private void btnFunding_Click(object sender, EventArgs e)
        {
            if (fundingForm == null || fundingForm.IsDisposed)
            {
                fundingForm = new FundingOptionsForm(homeForm, this);
            }

            fundingForm.SessionId = SessionId;
            fundingForm.LearnerId = LearnerId;
            fundingForm.LearnerContext = LearnerContext;
            fundingForm.PreferredFacultyId = PreferredFacultyId;
            fundingForm.SelectedProgrammeId = SelectedProgrammeId;
            fundingForm.SelectedProgrammeName = SelectedProgrammeName;
            fundingForm.Show();
            Hide();
        }

        /// <summary>Back → UI 5 to choose a different programme.</summary>
        private void btnBack_Click(object sender, EventArgs e)
        {
            previousForm.Show();
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

        /// <summary>Refreshes the context strip and the requirement check whenever the screen is shown.</summary>
        private void EligibilityResultsForm_VisibleChanged(object sender, EventArgs e)
        {
            if (!Visible)
            {
                return;
            }

            lblContext.Text = LearnerContext ?? string.Empty;
            SetStatus(ReadyStatus, DeepNavy, Color.White);
            LoadCheckedRequirements();
        }

        /// <summary>The caption-bar X asks first, because it closes Future Path.</summary>
        private void EligibilityResultsForm_FormClosing(object sender, FormClosingEventArgs e)
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
        private void EligibilityResultsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!homeForm.Visible)
            {
                Application.Exit();
            }
        }

        #endregion
    }
}
