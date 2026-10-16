using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using FuturePath.ApplicationLayer;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 6.1a — Alternative Programmes (step 5 of 6; spec 2.3.7, Sequence 2).
    /// Future Path ranks every <b>active</b> programme against the learner's captured subjects, marks
    /// and interest profile. Nothing is typed: selecting a ranked row rewrites the reasons panel, so
    /// every recommendation states why it is suggested, not only its rank and score. The
    /// "Include these alternative programmes" box is unticked by default and is saved to
    /// <c>AdvisorySession.AlternativesInSummary</c>. There is no validation on this screen.
    /// Its buttons (Continue to summary, Compare with my selection, Back) sit along the bottom of the tab.
    /// </summary>
    public partial class MainForm
    {
        private const string AlternativesReadyStatus = "Step 5 of 6 · Select a programme to see why it is suggested.";

        /// <summary>The programme the current list was generated for, so returning here does not re-rank.</summary>
        private int? alternativesRankedFor;
        private bool alternativesRanked;

        /// <summary>Set while the stored tick-box value is being shown, so it is not saved straight back.</summary>
        private bool alternativesLoadingChoice;

        /// <summary>The outcome of the learner's own selection in words, used in the guidance notice.</summary>
        private string alternativesSelectedOutcome;

        /// <summary>True while no programmes can be ranked because information is missing (outcome "More Information Needed").</summary>
        private bool alternativesMoreInfo;

        /// <summary>Why more information is needed, as returned with that outcome.</summary>
        private string alternativesMoreInfoText;

        #region Shown, button clicks and reset

        /// <summary>
        /// Called by PageShown each time UI 6.1a appears: shows the ready status and the guidance notice,
        /// then ranks the programmes (Sequence 2) unless they were already ranked for this selection.
        /// </summary>
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

        /// <summary>Continue to summary → UI 7; Back on UI 7 returns here.</summary>
        private void btnAlternativesSummary_Click(object sender, EventArgs e)
        {
            OpenSummary(tabAlternatives);
        }

        /// <summary>Compare with my selection: selects and shows the learner's own programme row. Changes nothing.</summary>
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

        /// <summary>Back → UI 6 (Eligibility results).</summary>
        private void btnAlternativesBack_Click(object sender, EventArgs e)
        {
            tcResults.SelectedTab = tabOutcome;
        }

        /// <summary>
        /// Returns UI 6.1a to a brand-new state for the next learner (spec 2.1): no ranked list, no
        /// count, the default reasons text, the include tick box unticked (without saving), Compare
        /// enabled, and nothing remembered about the previous outcome or ranking.
        /// </summary>
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

        /// <summary>
        /// The UI 6.1a half of <c>displayOutcome(outcome)</c>, called by <see cref="DisplayOutcome"/>.
        /// The learner's own outcome is remembered so the guidance notice can name it. "More Information
        /// Needed" (Sequence 2, when the learner has no captured subjects or interests) clears the list
        /// and explains what is missing instead.
        /// </summary>
        /// <param name="code">The outcome code: STRONG, POSSIBLE, NCE or MOREINFO.</param>
        /// <param name="explanation">Why the outcome was reached (shown with it, never the label alone).</param>
        private void AlternativesShowOutcome(string code, string explanation)
        {
            alternativesRanked = false;   // a new eligibility check makes the earlier ranking stale (Sequence 2 runs again)
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

            // The score is shown in the grid row; the notice names the outcome in words.
            alternativesMoreInfo = false;
            alternativesMoreInfoText = null;
            alternativesSelectedOutcome = AlternativesOutcomeWords(code).ToLower(CultureInfo.CurrentCulture);
            AlternativesShowNotice();
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

        /// <summary>
        /// Runs Sequence 2 for the learner's selection, unless the list was already ranked for it,
        /// and shows the stored "include in summary" choice.
        /// </summary>
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
                // Sequence 2: every other active programme is scored and ranked, and the ranking is saved. The
                // learner's own selection comes first (no rank), so it can be compared.
                RecommendationController recommendationController = new RecommendationController();
                Collection<RecommendationResult> ranked =
                    recommendationController.GenerateAlternatives(LearnerId.Value, SelectedProgrammeId.Value);
                if (recommendationController.MissingInformation != null)
                {
                    // Only this tab's half of displayOutcome: UI 6 keeps the outcome of the learner's own check.
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

        /// <summary>Saves the tick box to AdvisorySession.AlternativesInSummary.</summary>
        private void AlternativesSaveChoice()
        {
            RecommendationController recommendationController = new RecommendationController();
            recommendationController.SetAlternativesInSummary(SessionId.Value, chkIncludeAlternatives.Checked);
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

            AlternativesShowReason(dgvAlternatives.CurrentRow);
        }

        /// <summary>Selects the first row once data arrives, so a reason is always showing.</summary>
        private void dgvAlternatives_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (dgvAlternatives.Rows.Count > 0)
            {
                dgvAlternatives.CurrentCell = dgvAlternatives.Rows[0].Cells[colAlternativesProgramme.Index];
                AlternativesShowReason(dgvAlternatives.Rows[0]);
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

        /// <summary>Writes "Why rank n is suggested — programme" and the stored reason into the reasons panel.</summary>
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

        /// <summary>True when the row is the learner's own programme from UI 5.</summary>
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

        /// <summary>
        /// Classifies a stored OutcomeCode (AdvisoryOutcome.Code*) for display: S strong, P possible,
        /// N not currently eligible, I more information needed (MOREINFO or no outcome yet).
        /// </summary>
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

        /// <summary>The words shown for an outcome.</summary>
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

        /// <summary>The marker shown with an outcome (✓ ! ✗ ?), so colour is never the only signal.</summary>
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

        /// <summary>
        /// Fills the notice: blue guidance naming the learner's own outcome, or amber when more
        /// information is needed before anything can be ranked.
        /// </summary>
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

        /// <summary>Saves the learner's include / leave-out choice as soon as it changes.</summary>
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
