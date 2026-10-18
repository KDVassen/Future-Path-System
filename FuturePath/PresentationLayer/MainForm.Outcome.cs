
using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using FuturePath.ApplicationLayer;

namespace FuturePath.PresentationLayer
{
    // Displays the learner's eligibility results and requirement checks
    public partial class MainForm
    {
        // Colour used when more information is needed
        private static readonly Color OutcomeGreyBlue = Color.FromArgb(91, 107, 130);

        private const string OutcomeReadyStatus =
            "Step 5 of 6 · Preliminary guidance only. Continue to the summary, or look at alternatives and funding first.";

        // Store the current outcome so it can be shown again
        private string outcomeCode;
        private string outcomeExplanation;
        private decimal? outcomeScore;

        #region Shown, button clicks and reset

        // Refresh the outcome and requirement checks when the page opens
        private void OutcomeShown()
        {
            SetStatus(OutcomeReadyStatus, DeepNavy, Color.White);
            OutcomeRender();
            OutcomeLoadCheckedRequirements();
        }

        // Continue to the advisory summary
        private void btnOutcomeSummary_Click(object sender, EventArgs e)
        {
            OpenSummary(tabOutcome);
        }

        // Open the alternative programmes screen
        private void btnOutcomeAlternatives_Click(object sender, EventArgs e)
        {
            tcResults.SelectedTab = tabAlternatives;
        }

        // Open the funding options screen
        private void btnOutcomeFunding_Click(object sender, EventArgs e)
        {
            tcResults.SelectedTab = tabFunding;
        }

        // Return to programme selection
        private void btnOutcomeBack_Click(object sender, EventArgs e)
        {
            tcMain.SelectedTab = tabFaculty;
        }

        // Clear eligibility results for a new learner session
        private void OutcomeReset()
        {
            outcomeCode = null;
            outcomeExplanation = null;
            outcomeScore = null;
            OutcomeRender();

            dgvChecked.AutoGenerateColumns = false;   // Use the existing Designer columns
            dgvChecked.DataSource = null;
            OutcomeShowFigures(null, null, null);
            lblSubjectTileNote.Text = "required subjects present";
            lblMarkTileNote.Text = "marks against minimums";
            lblInterestTileNote.Text = "from your questionnaire";
        }

        #endregion

        #region Displaying the outcome (class diagram: displayOutcome)

        // Store and display the eligibility outcome
        public void DisplayOutcome(string code, string explanation, decimal? overallScore)
        {
            outcomeCode = code;
            outcomeExplanation = explanation;
            outcomeScore = overallScore;
            OutcomeRender();
            AlternativesShowOutcome(code, explanation);
        }

        // Update the outcome banner, explanation and overall score
        private void OutcomeRender()
        {
            string programme = SelectedProgrammeName;
            if (string.IsNullOrWhiteSpace(programme))
            {
                programme = "the selected programme";
            }

            string code = string.Empty;
            if (outcomeCode != null)
            {
                code = outcomeCode.Trim().ToUpperInvariant();
            }

            string chip, headline, fallback;
            Color back, bar, chipBack, chipFore;

            // Set the display based on the stored outcome code
            switch (code)
            {
                case AdvisoryOutcome.CodeStrong:
                    chip = "✓ Strong match";
                    headline = "You are a strong match for " + programme;
                    fallback = "You appear to meet the minimum requirements and your interests align strongly with this programme.";
                    back = GreenTint; bar = GreenColour; chipBack = GreenColour; chipFore = Color.White;
                    break;
                case AdvisoryOutcome.CodePossible:
                    chip = "! Possible match";
                    headline = "You are a possible match for " + programme;
                    fallback = "You meet some of the requirements, but there are one or more gaps or a weaker interest alignment.";
                    back = AmberTint; bar = AmberColour; chipBack = AmberColour; chipFore = InkColour;
                    break;
                case AdvisoryOutcome.CodeNotEligible:
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
            if (string.IsNullOrWhiteSpace(outcomeExplanation))
            {
                lblOutcomeExplanation.Text = fallback;
            }
            else
            {
                lblOutcomeExplanation.Text = outcomeExplanation.Trim();
            }

            lblOutcomeChip.AccessibleDescription = headline + ". " + lblOutcomeExplanation.Text;
            lblOverallTileValue.Text = OutcomeFormatFigure(outcomeScore);
        }

        // Display the subject, mark and interest scores
        private void OutcomeShowFigures(decimal? subjectScore, decimal? markScore, decimal? interestScore)
        {
            lblSubjectTileValue.Text = OutcomeFormatFigure(subjectScore);
            lblMarkTileValue.Text = OutcomeFormatFigure(markScore);
            lblInterestTileValue.Text = OutcomeFormatFigure(interestScore);
        }

        // Format a score as a whole number or a dash if missing
        private static string OutcomeFormatFigure(decimal? value)
        {
            if (!value.HasValue)
            {
                return "—";
            }

            return value.Value.ToString("0", CultureInfo.CurrentCulture);
        }

        // Read a stored score or return null when unavailable
        private static decimal? OutcomeReadScore(DataRow row, string column)
        {
            if (row[column] == DBNull.Value)
            {
                return null;
            }

            return (decimal)row[column];
        }

        // Format requirement results as met or not met
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

        // Clear automatic grid selections after loading data
        private void OutGrid_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            ((DataGridView)sender).ClearSelection();
        }

        #endregion

        #region Business Logic Layer hooks

        // Load saved requirement checks and component scores
        private void OutcomeLoadCheckedRequirements()
        {
            if (!SessionId.HasValue || !SelectedProgrammeId.HasValue)
            {
                return;
            }

            try
            {
                // Retrieve the requirement checks for the selected programme
                EligibilityController eligibilityController = new EligibilityController();
                dgvChecked.DataSource = eligibilityController.GetCheckedRequirements(SessionId.Value, SelectedProgrammeId.Value);

                // Read the saved scores and notes for the selected result
                DataTable result = eligibilityController.GetSelectedResult(SessionId.Value);
                if (result.Rows.Count > 0)
                {
                    DataRow row = result.Rows[0];
                    OutcomeShowFigures(OutcomeReadScore(row, "SubjectMatchScore"), OutcomeReadScore(row, "MarkMatchScore"),
                                       OutcomeReadScore(row, "InterestMatchScore"));
                    lblSubjectTileNote.Text = Convert.ToString(row["SubjectNote"], CultureInfo.CurrentCulture);
                    lblMarkTileNote.Text = Convert.ToString(row["MarkNote"], CultureInfo.CurrentCulture);
                    string strongest = Convert.ToString(row["StrongestCategory"], CultureInfo.CurrentCulture);
                    if (strongest.Length > 0)
                    {
                        lblInterestTileNote.Text = "strongest: " + strongest;
                    }
                    else
                    {
                        lblInterestTileNote.Text = "from your questionnaire";
                    }
                }
            }
            catch (Exception)
            {
                SetStatus("The requirement check could not be loaded. The outcome above is unchanged.", AmberColour, InkColour);
            }
        }

        #endregion
    }
}
