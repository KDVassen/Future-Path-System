using System;
using System.Drawing;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 5 — Faculty &amp; Department Selection (step 4 of 6; spec 2.3.5).
    /// The learner picks a search mode, a faculty and one programme (with its in-grid
    /// <b>Select</b> button). The requirements panel shows the stored ProgrammeRequirement rows for
    /// that programme, and the interest-alignment strip shows how the questionnaire answers from
    /// UI 4 align with it. Everything shown is system-supplied, so there is no validation dialog:
    /// a missing faculty or programme gets an amber field message or status instead.
    /// <b>Check eligibility</b> runs Sequence 1, saves the preferred faculty and selected programme
    /// on the session, and opens UI 6. It stays active, so the learner can come back and check
    /// other programmes.
    /// <para>
    /// The faculty and programme chosen on this screen only become the session's
    /// <c>PreferredFacultyId</c> / <c>SelectedProgrammeId</c> once they have been checked, so UI 6,
    /// 6.1a, 6.2b and 7 always describe the programme whose outcome is on record.
    /// </para>
    /// </summary>
    public partial class MainForm
    {
        private const string FacultyReadyStatus = "Step 4 of 6 · Choose a faculty, select a programme, then check eligibility.";

        /// <summary>True once the Faculty combo has been bound for this session.</summary>
        private bool facultyListsLoaded;

        /// <summary>The programme chosen with its Select button (not yet checked).</summary>
        private int? facultyProgrammeId;

        /// <summary>The chosen programme's DegreeProgramme.ProgrammeName (not yet checked).</summary>
        private string facultyProgrammeName;

        #region Shown, button clicks and reset

        /// <summary>
        /// Called by PageShown each time UI 5 appears: binds the faculty list once per session, restores
        /// the preferred faculty if the list was reloaded, and shows the ready status.
        /// </summary>
        private void FacultyShown()
        {
            if (!facultyListsLoaded)
            {
                FacultyLoadFaculties();
            }

            if (PreferredFacultyId.HasValue && cboSelFaculty.DataSource != null && cboSelFaculty.SelectedIndex < 0)
            {
                cboSelFaculty.SelectedValue = PreferredFacultyId.Value;
                FacultyLoadProgrammes();
            }

            SetStatus(FacultyReadyStatus, DeepNavy, Color.White);
        }

        /// <summary>
        /// Check eligibility → saves the selection, runs Sequence 1 and shows the outcome on UI 6.
        /// The button stays active, so another programme can be checked after coming Back.
        /// </summary>
        private void btnFacultyCheck_Click(object sender, EventArgs e)
        {
            if (!FacultyCheckFaculty())
            {
                SetStatus("Choose a faculty before you check eligibility.", RedColour, Color.White);
                cboSelFaculty.Focus();
                return;
            }

            if (!facultyProgrammeId.HasValue)
            {
                SetStatus("Choose a programme with its Select button before you check eligibility.", AmberColour, InkColour);
                dgvProgrammes.Focus();
                return;
            }

            int? facultyId = cboSelFaculty.SelectedValue is int ? (int?)(int)cboSelFaculty.SelectedValue : null;
            try
            {
                FacultySaveSelection(facultyId, facultyProgrammeId.Value);
                FacultyRunEligibilityCheck(facultyProgrammeId.Value);
            }
            catch (Exception)
            {
                SetStatus("Eligibility could not be checked for this programme. Please try again.", AmberColour, InkColour);
                return;
            }

            PreferredFacultyId = facultyId;
            SelectedProgrammeId = facultyProgrammeId;
            SelectedProgrammeName = facultyProgrammeName;
            GoToStep(tabOutcome, 5);
        }

        /// <summary>Back → UI 4. Nothing is written.</summary>
        private void btnFacultyBack_Click(object sender, EventArgs e)
        {
            tcMain.SelectedTab = tabInterests;
        }

        /// <summary>Cancel leaves the session (the Home tab asks first).</summary>
        private void btnFacultyCancel_Click(object sender, EventArgs e)
        {
            GoHome();
        }

        /// <summary>
        /// Returns UI 5 to a brand-new state for the next learner (spec 2.1): browse mode, no faculty,
        /// no search text, no programmes, no selection, no requirements or alignment, no amber message,
        /// and the faculty list is bound again the next time the screen is shown.
        /// </summary>
        private void FacultyReset()
        {
            facultyListsLoaded = false;
            facultyProgrammeId = null;
            facultyProgrammeName = null;

            cboSelFaculty.SelectedIndex = -1;
            txtProgrammeSearch.Text = string.Empty;
            rdoBrowse.Checked = true;
            txtProgrammeSearch.Enabled = false;
            lblSearchHint.Text = "Disabled while browsing by faculty";

            // Bind only to the Designer columns: auto-generated columns would replace them, and the
            // column fields used below (colProgrammesSelect, colProgrammesId, …) would no longer be in the grid.
            dgvProgrammes.AutoGenerateColumns = false;
            dgvRequirements.AutoGenerateColumns = false;
            dgvProgrammes.DataSource = null;
            lblProgrammeCount.Text = "Choose a faculty to list its active programmes.";
            FacultyLoadProgrammeDetails();

            CheckField(true, cboSelFaculty, lblSelFaculty, lblFacultyError, null);
        }

        #endregion

        #region Business Logic Layer hooks

        /// <summary>Binds the Faculty combo to the active faculties (never literal Items).</summary>
        private void FacultyLoadFaculties()
        {
            try
            {
                // TODO BLL: cboSelFaculty.DataSource = ReferenceDataController.GetActiveFaculties();
                //           cboSelFaculty.DisplayMember = "FacultyName"; cboSelFaculty.ValueMember = "FacultyID";
                cboSelFaculty.SelectedIndex = -1;
                facultyListsLoaded = true;
            }
            catch (Exception)
            {
                SetStatus("The faculty list could not be loaded. Please close and try again.", AmberColour, InkColour);
            }
        }

        /// <summary>
        /// Fills the programme grid: the active programmes of the chosen faculty while browsing,
        /// or the active programmes whose name contains the search text while searching.
        /// </summary>
        private void FacultyLoadProgrammes()
        {
            try
            {
                if (rdoBrowse.Checked)
                {
                    // TODO BLL: dgvProgrammes.DataSource = cboSelFaculty.SelectedIndex < 0 ? null
                    //               : ReferenceDataController.GetActiveProgrammes((int)cboSelFaculty.SelectedValue);
                    lblProgrammeCount.Text = cboSelFaculty.SelectedIndex < 0
                        ? "Choose a faculty to list its active programmes."
                        : dgvProgrammes.Rows.Count + " active programmes in " + cboSelFaculty.Text;
                }
                else
                {
                    string text = txtProgrammeSearch.Text.Trim();
                    // TODO BLL: dgvProgrammes.DataSource = text.Length == 0 ? null
                    //               : ReferenceDataController.SearchActiveProgrammes(text);
                    lblProgrammeCount.Text = text.Length == 0
                        ? "Type part of a programme name to search every faculty."
                        : dgvProgrammes.Rows.Count + " active programmes matching \"" + text + "\"";
                }

                dgvProgrammes.Invalidate();
            }
            catch (Exception)
            {
                SetStatus("The programme list could not be loaded. Please try again.", AmberColour, InkColour);
            }
        }

        /// <summary>
        /// Fills the requirements panel from ProgrammeRequirement (and the programme's minimum
        /// overall average) and the interest-alignment strip for the chosen programme.
        /// </summary>
        private void FacultyLoadProgrammeDetails()
        {
            lblRequirementsProgramme.Text = facultyProgrammeName ?? "No programme selected yet";
            lblAlignmentValue.Text = "—";
            lblStrongestCategories.Text = "Select a programme to see how your questionnaire answers align with it.";
            if (!facultyProgrammeId.HasValue)
            {
                dgvRequirements.DataSource = null;
                return;
            }

            try
            {
                // TODO BLL: dgvRequirements.DataSource =
                //               ReferenceDataController.GetActiveRequirements(facultyProgrammeId.Value);
                //           (rows: SubjectName, MinimumMark, RequirementType; plus an "Overall average" row
                //            from DegreeProgramme.MinimumOverallAverage)
                // TODO BLL: var alignment = InterestController.GetProgrammeAlignment(SessionId.Value, facultyProgrammeId.Value);
                //           lblAlignmentValue.Text = alignment.AlignmentPercent.ToString("0") + "%";
                //           lblStrongestCategories.Text = "Strongest categories: " + string.Join(", ", alignment.StrongestCategories);
            }
            catch (Exception)
            {
                SetStatus("The requirements for this programme could not be loaded. Please try again.", AmberColour, InkColour);
            }
        }

        /// <summary>
        /// Saves the preferred faculty and the selected programme on the in-progress session
        /// (AdvisorySession.PreferredFacultyID and SelectedProgrammeID).
        /// </summary>
        /// <param name="facultyId">The faculty chosen in the Faculty combo.</param>
        /// <param name="programmeId">The programme chosen with its Select button.</param>
        private void FacultySaveSelection(int? facultyId, int programmeId)
        {
            // TODO BLL: LearnerController.SaveProgrammeSelection(SessionId.Value, facultyId.Value, programmeId);
        }

        /// <summary>
        /// Sequence 1 — Check degree eligibility. The controller loads the learner and the
        /// programme, runs the RecommendationEngine, stores the RecommendationResult row
        /// (IsSelectedProgramme = 1) and returns the AdvisoryOutcome, which UI 6 displays with its
        /// explanation (<see cref="DisplayOutcome"/>, in MainForm.Outcome.cs).
        /// </summary>
        /// <param name="programmeId">The programme being checked.</param>
        private void FacultyRunEligibilityCheck(int programmeId)
        {
            // TODO BLL: AdvisoryOutcome outcome = EligibilityController.CheckEligibility(LearnerId.Value, programmeId);
            //           DisplayOutcome(outcome.Category, outcome.Explanation, outcome.OverallScore);
        }

        #endregion

        #region Selections

        /// <summary>Switches between browsing by faculty and searching by programme name.</summary>
        private void rbSearchMode_CheckedChanged(object sender, EventArgs e)
        {
            if (!((RadioButton)sender).Checked)
            {
                return;
            }

            bool searching = rdoSearch.Checked;
            txtProgrammeSearch.Enabled = searching;
            lblSearchHint.Text = searching
                ? "Searches all faculties"
                : "Disabled while browsing by faculty";
            FacultyClearSelectedProgramme();
            FacultyLoadProgrammes();
            if (searching)
            {
                txtProgrammeSearch.Focus();
            }
        }

        /// <summary>A new faculty is checked as the preferred faculty and, while browsing, relists the programmes.</summary>
        private void cboSelFaculty_SelectionChangeCommitted(object sender, EventArgs e)
        {
            FacultyCheckFaculty();
            if (rdoBrowse.Checked)
            {
                FacultyClearSelectedProgramme();
                FacultyLoadProgrammes();
            }
        }

        /// <summary>Shows the amber message when the learner leaves the Faculty combo without a choice.</summary>
        private void cboSelFaculty_Leave(object sender, EventArgs e)
        {
            FacultyCheckFaculty();
        }

        /// <summary>Relists the programmes as the search text changes (search mode only).</summary>
        private void txtProgrammeSearch_TextChanged(object sender, EventArgs e)
        {
            if (rdoSearch.Checked)
            {
                FacultyClearSelectedProgramme();
                FacultyLoadProgrammes();
            }
        }

        /// <summary>The in-grid Select button chooses that programme and loads its requirements.</summary>
        private void dgvProgrammes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != colProgrammesSelect.Index)
            {
                return;
            }

            DataGridViewRow row = dgvProgrammes.Rows[e.RowIndex];
            object id = row.Cells[colProgrammesId.Index].Value;
            if (!(id is int))
            {
                return;
            }

            facultyProgrammeId = (int)id;
            facultyProgrammeName = Convert.ToString(row.Cells[colProgrammesName.Index].Value);
            dgvProgrammes.Invalidate();
            FacultyLoadProgrammeDetails();
            SetStatus("Selected " + facultyProgrammeName + ". Check eligibility when you are ready.", DeepNavy, Color.White);
        }

        /// <summary>Shows "Selected" on the chosen programme's button and "Select" on every other row.</summary>
        private void dgvProgrammes_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != colProgrammesSelect.Index)
            {
                return;
            }

            object id = dgvProgrammes.Rows[e.RowIndex].Cells[colProgrammesId.Index].Value;
            bool chosen = facultyProgrammeId.HasValue && id is int && (int)id == facultyProgrammeId.Value;
            e.Value = chosen ? "✓ Selected" : "Select";
            e.CellStyle.BackColor = chosen ? GreenColour : NavyColour;
            e.CellStyle.SelectionBackColor = e.CellStyle.BackColor;
            e.FormattingApplied = true;
        }

        /// <summary>
        /// Clears the default first-row highlight after binding (programmes and requirements grids), so
        /// the blue selection never hides a coloured cell or suggests a choice the learner did not make.
        /// </summary>
        private void SelGrid_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            ((DataGridView)sender).ClearSelection();
        }

        /// <summary>Forgets the chosen programme when the list changes.</summary>
        private void FacultyClearSelectedProgramme()
        {
            facultyProgrammeId = null;
            facultyProgrammeName = null;
            FacultyLoadProgrammeDetails();
        }

        /// <summary>
        /// Faculty is required: a missing faculty shows red, like every other required field
        /// (decisions.md D12 — a deliberate change from the amber in spec 2.3.5).
        /// </summary>
        /// <returns>True when a faculty is chosen.</returns>
        private bool FacultyCheckFaculty()
        {
            return CheckField(cboSelFaculty.SelectedIndex >= 0, cboSelFaculty, lblSelFaculty, lblFacultyError,
                "Faculty: choose your preferred faculty.");
        }

        #endregion
    }
}
