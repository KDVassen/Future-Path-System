using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 6 — Eligibility Results (step 5 of 6; spec 2.3.6). The outcome screen for Sequence 1
    /// (class diagram: EligibilityCheckView.displayOutcome). Nothing is entered here.
    /// The outcome band carries a status chip (marker + words) and always states <b>why</b> the
    /// outcome was reached. Green marks a requirement met, amber a gap or "not currently
    /// eligible", grey-blue "more information needed" — red is never used for an outcome
    /// (decisions.md §2). Back, Funding options and Alternative programmes leave the
    /// stored outcome unchanged, and Continue to summary carries it through to UI 7.
    /// </summary>
    public partial class MainForm
    {
        /// <summary>Grey-blue used for "More information needed" (never red; decisions.md §2).</summary>
        private static readonly Color OutcomeGreyBlue = Color.FromArgb(91, 107, 130);

        private const string OutcomeReadyStatus =
            "Step 5 of 6 · Preliminary guidance only. Continue to the summary, or look at alternatives and funding first.";

        // The AdvisoryOutcome last displayed, kept so the band can be redrawn whenever UI 6 is shown.
        private string outcomeCategory;
        private string outcomeExplanation;
        private double? outcomeScore;

        #region Shown, button clicks and reset

        /// <summary>
        /// Called by PageShown each time UI 6 appears: redraws the outcome band for the checked
        /// programme, shows the ready status and reloads the requirement check.
        /// </summary>
        private void OutcomeShown()
        {
            SetStatus(OutcomeReadyStatus, DeepNavy, Color.White);
            OutcomeRender();
            OutcomeLoadCheckedRequirements();
        }

        /// <summary>Continue to summary → UI 7, carrying the outcome through.</summary>
        private void btnOutcomeSummary_Click(object sender, EventArgs e)
        {
            OpenSummary(tabOutcome);
        }

        /// <summary>Alternative programmes → UI 6.1a (Sequence 2 runs when the tab is shown).</summary>
        private void btnOutcomeAlternatives_Click(object sender, EventArgs e)
        {
            tcResults.SelectedTab = tabAlternatives;
        }

        /// <summary>Funding options → UI 6.2b, the learner's yes/no choice to view funding (spec Section 7).</summary>
        private void btnOutcomeFunding_Click(object sender, EventArgs e)
        {
            tcResults.SelectedTab = tabFunding;
        }

        /// <summary>Back → UI 5, where another programme can be checked.</summary>
        private void btnOutcomeBack_Click(object sender, EventArgs e)
        {
            tcMain.SelectedTab = tabFaculty;
        }

        /// <summary>
        /// Returns UI 6 to a brand-new state for the next learner (spec 2.1): no stored outcome (the band
        /// shows "More information needed"), an empty requirement check, and blank figure tiles with
        /// their original notes.
        /// </summary>
        private void OutcomeReset()
        {
            outcomeCategory = null;
            outcomeExplanation = null;
            outcomeScore = null;
            OutcomeRender();

            dgvChecked.AutoGenerateColumns = false;   // bind to the Designer columns only (colCheckedOutcome, colCheckedIsMet, …)
            dgvChecked.DataSource = null;
            OutcomeShowFigures(null, null, null);
            lblSubjectTileNote.Text = "required subjects present";
            lblMarkTileNote.Text = "marks against minimums";
            lblInterestTileNote.Text = "from your questionnaire";
        }

        #endregion

        #region Displaying the outcome (class diagram: displayOutcome)

        /// <summary>
        /// Shows an AdvisoryOutcome: fills the UI 6 outcome band, its status chip and the Overall score
        /// tile, and tells UI 6.1a (Alternative programmes) what the outcome was. Called by Sequence 1
        /// (EligibilityController.CheckEligibility, from UI 5) and by Sequence 2 when alternatives cannot
        /// be ranked ("More Information Needed", from UI 6.1a). An empty category means the check could
        /// not be completed, and the "More information needed" wording is shown.
        /// </summary>
        /// <param name="category">AdvisoryOutcome.category (RecommendationResult.OutcomeCode).</param>
        /// <param name="explanation">AdvisoryOutcome.explanation (RecommendationResult.OutcomeReason).</param>
        /// <param name="overallScore">AdvisoryOutcome.overallScore (0–100), or null when not calculated.</param>
        public void DisplayOutcome(string category, string explanation, double? overallScore)
        {
            outcomeCategory = category;
            outcomeExplanation = explanation;
            outcomeScore = overallScore;
            OutcomeRender();
            AlternativesShowOutcome(category, explanation);
        }

        /// <summary>
        /// Draws the outcome band and the Overall score tile from the stored outcome, naming the
        /// programme that was checked (SelectedProgrammeName).
        /// </summary>
        private void OutcomeRender()
        {
            string programme = string.IsNullOrWhiteSpace(SelectedProgrammeName) ? "the selected programme" : SelectedProgrammeName;
            string key = string.IsNullOrWhiteSpace(outcomeCategory)
                ? string.Empty
                : outcomeCategory.Replace(" ", string.Empty).ToUpperInvariant();
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
                    back = BlueTint; bar = OutcomeGreyBlue; chipBack = OutcomeGreyBlue; chipFore = Color.White;
                    break;
            }

            pnlOutcome.BackColor = back;
            pnlOutcomeBar.BackColor = bar;
            lblOutcomeChip.BackColor = chipBack;
            lblOutcomeChip.ForeColor = chipFore;
            lblOutcomeChip.Text = chip;
            lblOutcomeHeadline.Text = headline;
            lblOutcomeExplanation.Text = string.IsNullOrWhiteSpace(outcomeExplanation) ? fallback : outcomeExplanation.Trim();
            lblOutcomeChip.AccessibleDescription = headline + ". " + lblOutcomeExplanation.Text;

            lblOverallTileValue.Text = outcomeScore.HasValue
                ? outcomeScore.Value.ToString("0", CultureInfo.CurrentCulture)
                : "—";
        }

        /// <summary>
        /// Shows the three component figures on their running-figure tiles (display only; the
        /// figures and notes come from the stored RecommendationResult row).
        /// </summary>
        private void OutcomeShowFigures(double? subjectScore, double? markScore, double? interestScore)
        {
            lblSubjectTileValue.Text = OutcomeFormatFigure(subjectScore);
            lblMarkTileValue.Text = OutcomeFormatFigure(markScore);
            lblInterestTileValue.Text = OutcomeFormatFigure(interestScore);
        }

        /// <summary>A whole-number figure for a tile, or a dash when there is none.</summary>
        private static string OutcomeFormatFigure(double? value)
        {
            return value.HasValue ? value.Value.ToString("0", CultureInfo.CurrentCulture) : "—";
        }

        /// <summary>
        /// Colours and marks the Outcome column: green with ✓ for a requirement met, amber text
        /// with ! for a gap. Words are always shown, so colour is never the only signal.
        /// </summary>
        private void dgvChecked_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != colCheckedOutcome.Index || e.Value == null)
            {
                return;
            }

            object met = dgvChecked.Rows[e.RowIndex].Cells[colCheckedIsMet.Index].Value;
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
        private void OutGrid_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            ((DataGridView)sender).ClearSelection();
        }

        #endregion

        #region Business Logic Layer hooks

        /// <summary>
        /// Loads the requirement-by-requirement check and the component scores for the selected
        /// programme from the stored RecommendationResult (written by Sequence 1).
        /// </summary>
        private void OutcomeLoadCheckedRequirements()
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
                //           OutcomeShowFigures(result.SubjectMatchScore, result.MarkMatchScore, result.InterestMatchScore);
                //           lblSubjectTileNote.Text = result.SubjectNote; lblMarkTileNote.Text = result.MarkNote;
                //           lblInterestTileNote.Text = result.StrongestCategory;
            }
            catch (Exception)
            {
                SetStatus("The requirement check could not be loaded. The outcome above is unchanged.", AmberColour, InkColour);
            }
        }

        #endregion
    }
}
