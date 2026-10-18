
using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using FuturePath.ApplicationLayer;

namespace FuturePath.PresentationLayer
{
    // Handles faculty and degree programme selection for learners
    public partial class MainForm
    {
        private const string FacultyReadyStatus = "Step 4 of 6 · Choose a faculty, select a programme, then check eligibility.";

        // Tracks whether the faculty list has been loaded
        private bool facultyListsLoaded;

        // Programme selected but not yet checked for eligibility
        private int? facultyProgrammeId;

        // Name of the programme selected by the learner
        private string facultyProgrammeName;

        #region Shown, button clicks and reset

        // Load faculties and restore the learner's preferred faculty
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

        // Save the selected programme and check its eligibility
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

            int facultyId = (int)cboSelFaculty.SelectedValue;
            try
            {
                FacultySaveSelection(facultyId, facultyProgrammeId.Value);
                FacultyRunEligibilityCheck(facultyProgrammeId.Value);
            }
            catch (ArgumentException ex)
            {
                // Display errors returned by the controller
                SetStatus(ex.Message, AmberColour, InkColour);
                return;
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

        // Return to the interest questionnaire
        private void btnFacultyBack_Click(object sender, EventArgs e)
        {
            tcMain.SelectedTab = tabInterests;
        }

        // Cancel and return to the home screen
        private void btnFacultyCancel_Click(object sender, EventArgs e)
        {
            GoHome();
        }

        // Reset the faculty and programme selection for a new learner
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

            // Keep the existing Designer grid columns
            dgvProgrammes.AutoGenerateColumns = false;
            dgvRequirements.AutoGenerateColumns = false;
            dgvProgrammes.DataSource = null;
            lblProgrammeCount.Text = "Choose a faculty to list its active programmes.";
            FacultyLoadProgrammeDetails();

            CheckField(true, cboSelFaculty, lblSelFaculty, lblFacultyError, null);
        }

        #endregion

        #region Business Logic Layer hooks

        // Load active faculties into the dropdown
        private void FacultyLoadFaculties()
        {
            try
            {
                ReferenceDataController referenceDataController = new ReferenceDataController();
                cboSelFaculty.DisplayMember = "FacultyName";
                cboSelFaculty.ValueMember = "FacultyID";
                cboSelFaculty.DataSource = referenceDataController.GetActiveFaculties();
                cboSelFaculty.SelectedIndex = -1;
                facultyListsLoaded = true;
            }
            catch (Exception)
            {
                SetStatus("The faculty list could not be loaded. Please close and try again.", AmberColour, InkColour);
            }
        }

        // Load programmes based on the selected browse or search mode
        private void FacultyLoadProgrammes()
        {
            try
            {
                ReferenceDataController referenceDataController = new ReferenceDataController();
                if (rdoBrowse.Checked)
                {
                    if (cboSelFaculty.SelectedIndex < 0)
                    {
                        dgvProgrammes.DataSource = null;
                        lblProgrammeCount.Text = "Choose a faculty to list its active programmes.";
                    }
                    else
                    {
                        dgvProgrammes.DataSource = referenceDataController.GetActiveProgrammes((int)cboSelFaculty.SelectedValue);
                        lblProgrammeCount.Text = dgvProgrammes.Rows.Count + " active programmes in " + cboSelFaculty.Text;
                    }
                }
                else
                {
                    string text = txtProgrammeSearch.Text.Trim();
                    if (text.Length == 0)
                    {
                        dgvProgrammes.DataSource = null;
                        lblProgrammeCount.Text = "Type part of a programme name to search every faculty.";
                    }
                    else
                    {
                        dgvProgrammes.DataSource = referenceDataController.SearchActiveProgrammes(text);
                        lblProgrammeCount.Text = dgvProgrammes.Rows.Count + " active programmes matching \"" + text + "\"";
                    }
                }

                dgvProgrammes.Invalidate();
            }
            catch (Exception)
            {
                SetStatus("The programme list could not be loaded. Please try again.", AmberColour, InkColour);
            }
        }

        // Load requirements and interest alignment for the selected programme
        private void FacultyLoadProgrammeDetails()
        {
            lblAlignmentValue.Text = "—";
            lblStrongestCategories.Text = "Select a programme to see how your questionnaire answers align with it.";
            if (!facultyProgrammeId.HasValue)
            {
                lblRequirementsProgramme.Text = "No programme selected yet";
                dgvRequirements.DataSource = null;
                return;
            }

            lblRequirementsProgramme.Text = facultyProgrammeName;
            try
            {
                // Retrieve requirements and the programme's minimum average
                ReferenceDataController referenceDataController = new ReferenceDataController();
                dgvRequirements.DataSource = referenceDataController.GetActiveRequirements(facultyProgrammeId.Value);

                InterestController interestController = new InterestController();
                ProgrammeAlignment alignment = interestController.GetProgrammeAlignment(SessionId.Value, facultyProgrammeId.Value);
                lblAlignmentValue.Text = alignment.AlignmentPercent.ToString("0", CultureInfo.CurrentCulture) + "%";
                if (alignment.StrongestCategories.Length == 0)
                {
                    lblStrongestCategories.Text = "None of your questionnaire answers is linked to this programme's interest categories.";
                }
                else
                {
                    lblStrongestCategories.Text = "Strongest categories: " + alignment.StrongestCategories;
                }
            }
            catch (Exception)
            {
                SetStatus("The requirements for this programme could not be loaded. Please try again.", AmberColour, InkColour);
            }
        }

        // Save the learner's preferred faculty and selected programme
        private void FacultySaveSelection(int facultyId, int programmeId)
        {
            LearnerController learnerController = new LearnerController();
            learnerController.SaveProgrammeSelection(SessionId.Value, facultyId, programmeId);
        }

        // Check eligibility and display the result
        private void FacultyRunEligibilityCheck(int programmeId)
        {
            EligibilityController eligibilityController = new EligibilityController();
            AdvisoryOutcome outcome = eligibilityController.CheckEligibility(LearnerId.Value, programmeId);
            DisplayOutcome(outcome.Code, outcome.Explanation, outcome.OverallScore);
        }

        #endregion

        #region Selections

        // Switch between browsing faculties and searching programmes
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

        // Reload programmes when a different faculty is selected
        private void cboSelFaculty_SelectionChangeCommitted(object sender, EventArgs e)
        {
            FacultyCheckFaculty();
            if (rdoBrowse.Checked)
            {
                FacultyClearSelectedProgramme();
                FacultyLoadProgrammes();
            }
        }

        // Validate the faculty selection when leaving the field
        private void cboSelFaculty_Leave(object sender, EventArgs e)
        {
            FacultyCheckFaculty();
        }

        // Refresh search results as the programme name is typed
        private void txtProgrammeSearch_TextChanged(object sender, EventArgs e)
        {
            if (rdoSearch.Checked)
            {
                FacultyClearSelectedProgramme();
                FacultyLoadProgrammes();
            }
        }

        // Select a programme and load its details
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

        // Mark the chosen programme in the grid
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

        // Clear automatic grid selections after loading data
        private void SelGrid_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            ((DataGridView)sender).ClearSelection();
        }

        // Clear the currently selected programme
        private void FacultyClearSelectedProgramme()
        {
            facultyProgrammeId = null;
            facultyProgrammeName = null;
            FacultyLoadProgrammeDetails();
        }

        // Check that a preferred faculty has been selected
        private bool FacultyCheckFaculty()
        {
            return CheckField(cboSelFaculty.SelectedIndex >= 0, cboSelFaculty, lblSelFaculty, lblFacultyError,
                "Faculty: choose your preferred faculty.");
        }

        #endregion
    }
}
