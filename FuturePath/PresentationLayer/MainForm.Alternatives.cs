
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using FuturePath.ApplicationLayer;

namespace FuturePath.PresentationLayer
{
    // Displays ranked alternative programmes and their recommendation reasons
    public partial class MainForm
    {
        private const string AlternativesReadyStatus = "Step 5 of 6 · Select a programme to see why it is suggested.";

        // Track which programme the alternatives were ranked for
        private int? alternativesRankedFor;
        private bool alternativesRanked;

        // Prevent saving while loading the stored checkbox value
        private bool alternativesLoadingChoice;

        // Outcome of the learner's selected programme
        private string alternativesSelectedOutcome;

        // Track when more information is needed for ranking
        private bool alternativesMoreInfo;

        // Explanation of the missing information
        private string alternativesMoreInfoText;

        #region Shown, button clicks and reset

        // Display guidance and load the ranked alternatives
        private void AlternativesShown()
        {
            if (alternativesMoreInfo)
            {
                SetStatus("More information is needed before alternative programmes can be ranked.", AmberColour, InkColour);
            }
            else
            {
                SetStatus(AlternativesReadyStatus, DeepNavy, Color.White);
            }

            AlternativesShowNotice();
            btnAlternativesCompare.Enabled = !alternativesMoreInfo;
            AlternativesLoad();
        }

        // Continue to the advisory summary
        private void btnAlternativesSummary_Click(object sender, EventArgs e)
        {
            OpenSummary(tabAlternatives);
        }

        // Highlight the learner's selected programme for comparison
        private void btnAlternativesCompare_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvAlternatives.Rows)
            {
                if (AlternativesIsSelectedRow(row))
                {
                    dgvAlternatives.CurrentCell = row.Cells[colAlternativesProgramme.Index];
                    row.Selected = true;
                    AlternativesShowReason(row);
                    SetStatus("Your own selection is highlighted in the list. Compare its fit and reason with the ranked programmes.",
                              DeepNavy, Color.White);
                    return;
                }
            }

            SetStatus("Your selected programme is not in this list. Go back to see its outcome on Eligibility results.",
                      AmberColour, InkColour);
        }

        // Return to the eligibility results screen
        private void btnAlternativesBack_Click(object sender, EventArgs e)
        {
            tcResults.SelectedTab = tabOutcome;
        }

        // Clear alternatives when starting a new learner session
        private void AlternativesReset()
        {
            alternativesLoadingChoice = true;
            chkIncludeAlternatives.Checked = false;
            alternativesLoadingChoice = false;

            alternativesRanked = false;
            alternativesRankedFor = null;
            alternativesSelectedOutcome = null;
            alternativesMoreInfo = false;
            alternativesMoreInfoText = null;

            dgvAlternatives.AutoGenerateColumns = false;
            dgvAlternatives.DataSource = null;
            lblRankedCount.Text = string.Empty;
            lblReasonsHeading.Text = "Why each programme is suggested";
            lblReasons.Text = "Select a programme in the list to see why it is suggested.";
            btnAlternativesCompare.Enabled = true;
        }

        #endregion

        #region Boundary methods from the class diagram (AlternativeProgrammesView)

        // Store the selected programme's outcome and update the guidance
        private void AlternativesShowOutcome(string code, string explanation)
        {
            alternativesRanked = false;   // Recalculate alternatives after a new eligibility check
            if (AlternativesOutcomeKind(code) == 'I')
            {
                alternativesMoreInfo = true;
                alternativesMoreInfoText = explanation;
                dgvAlternatives.DataSource = null;
                lblRankedCount.Text = "No programmes ranked";
                lblReasonsHeading.Text = "Why no programmes are listed";
                lblReasons.Text = string.IsNullOrEmpty(explanation)
                    ? "Future Path needs your subjects, marks and interest questionnaire before it can rank other programmes."
                    : explanation;
                AlternativesShowNotice();
                btnAlternativesCompare.Enabled = false;
                if (CurrentPage == tabAlternatives)
                {
                    SetStatus("More information is needed before alternative programmes can be ranked.", AmberColour, InkColour);
                }

                return;
            }

            // Display the learner's outcome in the guidance notice
            alternativesMoreInfo = false;
            alternativesMoreInfoText = null;
            alternativesSelectedOutcome = AlternativesOutcomeWords(code).ToLower(CultureInfo.CurrentCulture);
            AlternativesShowNotice();
        }

        // Display ranked programmes and their recommendation results
        public void DisplayAlternatives(object rankedAlternatives)
        {
            alternativesMoreInfo = false;
            alternativesMoreInfoText = null;
            btnAlternativesCompare.Enabled = true;
            dgvAlternatives.AutoGenerateColumns = false;
            dgvAlternatives.DataSource = rankedAlternatives;

            int ranked = dgvAlternatives.Rows.Cast<DataGridViewRow>().Count(r => !AlternativesIsSelectedRow(r));
            lblRankedCount.Text = ranked == 1 ? "1 active programme ranked" : ranked + " active programmes ranked";
            if (dgvAlternatives.Rows.Count == 0)
            {
                lblReasonsHeading.Text = "Why each programme is suggested";
                lblReasons.Text = "No other active programme could be ranked for your profile.";
            }

            AlternativesShowNotice();
        }

        #endregion

        #region Business Logic Layer hooks

        // Generate alternative recommendations unless already ranked
        private void AlternativesLoad()
        {
            if (!LearnerId.HasValue || !SelectedProgrammeId.HasValue)
            {
                return;
            }

            if (alternativesRanked && alternativesRankedFor == SelectedProgrammeId)
            {
                return;
            }

            try
            {
                // Rank active programmes and include the learner's selection for comparison
                RecommendationController recommendationController = new RecommendationController();
                Collection<RecommendationResult> ranked =
                    recommendationController.GenerateAlternatives(LearnerId.Value, SelectedProgrammeId.Value);
                if (recommendationController.MissingInformation != null)
                {
                    // Explain why alternatives cannot be ranked yet
                    AlternativesShowOutcome(AdvisoryOutcome.CodeMoreInformation, recommendationController.MissingInformation.Explanation);
                }
                else
                {
                    DisplayAlternatives(ranked);
                }

                alternativesLoadingChoice = true;
                chkIncludeAlternatives.Checked = recommendationController.GetAlternativesInSummary(SessionId.Value);
                alternativesLoadingChoice = false;
                alternativesRanked = true;
                alternativesRankedFor = SelectedProgrammeId;
            }
            catch (Exception)
            {
                alternativesLoadingChoice = false;
                SetStatus("Alternative programmes could not be loaded. Go back and try again.", AmberColour, InkColour);
            }
        }

        // Save whether alternatives should appear in the advisory summary
        private void AlternativesSaveChoice()
        {
            RecommendationController recommendationController = new RecommendationController();
            recommendationController.SetAlternativesInSummary(SessionId.Value, chkIncludeAlternatives.Checked);
        }

        #endregion

        #region Grid display (formatting only; ranking and outcomes come from the BLL)

        // Update the reasons panel when a programme is selected
        private void dgvAlternatives_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAlternatives.CurrentRow == null)
            {
                return;
            }

            AlternativesShowReason(dgvAlternatives.CurrentRow);
        }

        // Select the first programme after the grid loads
        private void dgvAlternatives_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (dgvAlternatives.Rows.Count > 0)
            {
                dgvAlternatives.CurrentCell = dgvAlternatives.Rows[0].Cells[colAlternativesProgramme.Index];
                AlternativesShowReason(dgvAlternatives.Rows[0]);
            }
        }

        // Format outcome labels, selected programmes and empty ranks
        private void dgvAlternatives_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            DataGridViewRow row = dgvAlternatives.Rows[e.RowIndex];
            if (e.ColumnIndex == colAlternativesOutcome.Index && e.Value != null)
            {
                string code = Convert.ToString(e.Value, CultureInfo.InvariantCulture);
                e.Value = AlternativesOutcomeMarker(code) + " " + AlternativesOutcomeWords(code);
                char kind = AlternativesOutcomeKind(code);
                e.CellStyle.Font = ChipFont;
                e.CellStyle.ForeColor = kind == 'S' ? GreenColour : (kind == 'I' ? BlueColour : AmberText);
                e.CellStyle.BackColor = kind == 'S' ? GreenTint : (kind == 'I' ? BlueTint : AmberTint);
                e.FormattingApplied = true;
            }
            else if (e.ColumnIndex == colAlternativesProgramme.Index && AlternativesIsSelectedRow(row))
            {
                e.Value = Convert.ToString(e.Value, CultureInfo.CurrentCulture) + " · your selection";
                e.FormattingApplied = true;
            }
            else if (e.ColumnIndex == colAlternativesRank.Index && (e.Value == null || e.Value == DBNull.Value))
            {
                e.Value = "—";
                e.FormattingApplied = true;
            }
        }

        // Display the selected programme's ranking and reason
        private void AlternativesShowReason(DataGridViewRow row)
        {
            string programme = Convert.ToString(row.Cells[colAlternativesProgramme.Index].Value, CultureInfo.CurrentCulture);
            string reason = Convert.ToString(row.Cells[colAlternativesReason.Index].Value, CultureInfo.CurrentCulture);
            object rank = row.Cells[colAlternativesRank.Index].Value;

            lblReasonsHeading.Text = AlternativesIsSelectedRow(row) || rank == null || rank == DBNull.Value
                ? "How your own selection compares — " + programme
                : "Why rank " + Convert.ToString(rank, CultureInfo.CurrentCulture) + " is suggested — " + programme;
            lblReasons.Text = string.IsNullOrEmpty(reason) ? "No reason was recorded for this programme." : reason;
        }

        // Check whether a row represents the learner's selected programme
        private bool AlternativesIsSelectedRow(DataGridViewRow row)
        {
            object flag = row.Cells[colAlternativesIsSelected.Index].Value;
            if (flag is bool && (bool)flag)
            {
                return true;
            }

            object id = row.Cells[colAlternativesProgrammeId.Index].Value;
            return SelectedProgrammeId.HasValue && id != null && id != DBNull.Value
                   && Convert.ToInt32(id, CultureInfo.InvariantCulture) == SelectedProgrammeId.Value;
        }

        // Identify the type of stored advisory outcome
        private static char AlternativesOutcomeKind(string code)
        {
            string value = string.Empty;
            if (code != null)
            {
                value = code.Trim().ToUpperInvariant();
            }

            if (value == AdvisoryOutcome.CodeStrong)
            {
                return 'S';
            }

            if (value == AdvisoryOutcome.CodePossible)
            {
                return 'P';
            }

            if (value == AdvisoryOutcome.CodeNotEligible)
            {
                return 'N';
            }

            return 'I';
        }

        // Get the display text for an advisory outcome
        private static string AlternativesOutcomeWords(string code)
        {
            switch (AlternativesOutcomeKind(code))
            {
                case 'S': return "Strong match";
                case 'P': return "Possible match";
                case 'N': return "Not currently eligible";
                default: return "More information needed";
            }
        }

        // Get the marker shown beside an advisory outcome
        private static string AlternativesOutcomeMarker(string code)
        {
            switch (AlternativesOutcomeKind(code))
            {
                case 'S': return "✓";
                case 'P': return "!";
                case 'N': return "✗";
                default: return "?";
            }
        }

        #endregion

        #region Notice and tick box

        // Show guidance based on the learner's eligibility outcome
        private void AlternativesShowNotice()
        {
            if (alternativesMoreInfo)
            {
                ShowNotice(lblAlternativesNotice, "? More information needed — " + (string.IsNullOrEmpty(alternativesMoreInfoText)
                        ? "go back and complete your subjects, marks and interest questionnaire, then try again."
                        : alternativesMoreInfoText),
                    AmberTint, AmberText);
                return;
            }

            string selection = string.IsNullOrEmpty(SelectedProgrammeName) ? "Your selected programme" : SelectedProgrammeName;
            string opening = string.IsNullOrEmpty(alternativesSelectedOutcome)
                ? "Every active programme"
                : selection + " returned a " + alternativesSelectedOutcome + ", so every active programme";
            ShowNotice(lblAlternativesNotice, opening + " has been ranked against your subjects, marks and interests. " +
                       "Your own selection stays in the list for comparison.",
                       BlueTint, InkColour);
        }

        // Save the learner's choice when the checkbox changes
        private void chkIncludeAlternatives_CheckedChanged(object sender, EventArgs e)
        {
            if (alternativesLoadingChoice)
            {
                return;
            }

            try
            {
                AlternativesSaveChoice();
                SetStatus(chkIncludeAlternatives.Checked
                    ? "These alternative programmes will be printed in your advisory summary."
                    : "Alternative programmes will be left out of your advisory summary.", DeepNavy, Color.White);
            }
            catch (Exception)
            {
                SetStatus("Your choice could not be saved. Please try again.", AmberColour, InkColour);
            }
        }

        #endregion
    }
}
