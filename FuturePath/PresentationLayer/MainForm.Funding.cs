
using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using FuturePath.ApplicationLayer;

namespace FuturePath.PresentationLayer
{
    // Handles funding options and their details for learners
    public partial class MainForm
    {
        private const string FundingReadyStatus = "Step 5 of 6 · Select a funding option to see its details.";

        // Faculty previously used to filter funding options
        private int? fundingDefaultedFacultyId;

        // Tracks whether the funding filters have been loaded
        private bool fundingListsLoaded;

        #region Shown, button clicks and reset

        // Initialise the funding screen and load available options
        private void FundingShown()
        {
            SetStatus(FundingReadyStatus, DeepNavy, Color.White);
            if (!fundingListsLoaded)
            {
                FundingLoadLists();
            }

            FundingApplyDefaultFaculty();
        }

        // Add funding information to the advisory summary
        private void btnFundingAdd_Click(object sender, EventArgs e)
        {
            try
            {
                FundingRequestInSummary();
            }
            catch (Exception)
            {
                SetStatus("Funding could not be added to your summary. Please try again.", AmberColour, InkColour);
                return;
            }

            OpenSummary(tabFunding);
        }

        // Return to the eligibility results screen
        private void btnFundingBack_Click(object sender, EventArgs e)
        {
            tcResults.SelectedTab = tabOutcome;
        }

        // Reset funding filters and details for a new learner
        private void FundingReset()
        {
            fundingListsLoaded = false;          // Prevent loading funding while resetting filters
            fundingDefaultedFacultyId = null;

            cboFundFaculty.SelectedIndex = -1;
            cboFundingType.SelectedIndex = -1;
            chkOpenOnly.Checked = false;

            dgvFunding.AutoGenerateColumns = false;
            dgvFunding.DataSource = null;
            lblFundingCount.Text = string.Empty;
            lblFundingHeading.Text = "Funding linked to your faculty";
            FundingClearDetail();
        }

        #endregion

        #region Business Logic Layer hooks

        // Load faculty and funding type filters
        private void FundingLoadLists()
        {
            try
            {
                ReferenceDataController referenceDataController = new ReferenceDataController();
                cboFundFaculty.DisplayMember = "FacultyName";
                cboFundFaculty.ValueMember = "FacultyID";
                cboFundFaculty.DataSource = referenceDataController.GetActiveFaculties();
                cboFundFaculty.SelectedIndex = -1;

                // Load funding types with an option to show all
                FundingController fundingController = new FundingController();
                cboFundingType.DisplayMember = "ParameterValue";
                cboFundingType.ValueMember = "FundingType";
                cboFundingType.DataSource = fundingController.GetFundingTypes(true);
                cboFundingType.SelectedIndex = 0;
                fundingListsLoaded = true;
            }
            catch (Exception)
            {
                SetStatus("The faculty and funding type lists could not be loaded. Go back and try again.", AmberColour, InkColour);
            }
        }

        // Load active funding matching the selected filters
        private void FundingLoad()
        {
            FundingClearDetail();
            FundingUpdateHeading();
            try
            {
                // Zero and an empty string represent no filter
                int facultyId = 0;
                if (cboFundFaculty.SelectedIndex >= 0)
                {
                    facultyId = (int)cboFundFaculty.SelectedValue;
                }

                int programmeId = 0;
                if (SelectedProgrammeId.HasValue)
                {
                    programmeId = SelectedProgrammeId.Value;
                }

                string fundingType = string.Empty;
                if (cboFundingType.SelectedIndex >= 0)
                {
                    fundingType = Convert.ToString(cboFundingType.SelectedValue, CultureInfo.InvariantCulture);
                }

                // Show active funding for the faculty or programme
                FundingController fundingController = new FundingController();
                dgvFunding.DataSource = fundingController.GetActiveFunding(facultyId, programmeId, fundingType, chkOpenOnly.Checked);
            }
            catch (Exception)
            {
                SetStatus("Funding options could not be loaded. Please try again.", AmberColour, InkColour);
            }
        }

        // Record that funding details should appear in the summary
        private void FundingRequestInSummary()
        {
            FundingController fundingController = new FundingController();
            fundingController.RequestFundingInfo(SessionId.Value);
        }

        #endregion

        #region Filters

        // Default the funding faculty to the learner's selected faculty
        private void FundingApplyDefaultFaculty()
        {
            if (fundingDefaultedFacultyId == PreferredFacultyId && dgvFunding.DataSource != null)
            {
                return;
            }

            if (PreferredFacultyId.HasValue && cboFundFaculty.Items.Count > 0)
            {
                cboFundFaculty.SelectedValue = PreferredFacultyId.Value;
            }

            fundingDefaultedFacultyId = PreferredFacultyId;
            FundingLoad();
        }

        // Reload funding when the faculty changes
        private void cboFundFaculty_SelectionChangeCommitted(object sender, EventArgs e) { FundingLoad(); }

        // Reload funding when the funding type changes
        private void cboFundingType_SelectionChangeCommitted(object sender, EventArgs e) { FundingLoad(); }

        // Reload funding when the open-only filter changes
        private void chkOpenOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (fundingListsLoaded)
            {
                FundingLoad();
            }
        }

        // Update the funding heading to match the selected faculty
        private void FundingUpdateHeading()
        {
            string faculty = cboFundFaculty.SelectedIndex >= 0 ? cboFundFaculty.Text : string.Empty;
            if (faculty.Length == 0)
            {
                lblFundingHeading.Text = "Funding linked to your faculty";
            }
            else if (faculty.StartsWith("Faculty", StringComparison.OrdinalIgnoreCase))
            {
                lblFundingHeading.Text = "Funding linked to the " + faculty;
            }
            else
            {
                lblFundingHeading.Text = "Funding linked to the Faculty of " + faculty;
            }
        }

        #endregion

        #region Grid and detail card (display only)

        // Show the details of the selected funding option
        private void dgvFunding_SelectionChanged(object sender, EventArgs e)
        {
            DataGridViewRow row = dgvFunding.CurrentRow;
            if (row == null)
            {
                FundingClearDetail();
                return;
            }

            lblDetailHeading.Text = FundingCellText(row, colFundingName);
            txtEligibility.Text = FundingCellText(row, colFundingEligibility);
            txtDocuments.Text = FundingCellText(row, colFundingDocuments);
            txtContact.Text = FundingCellText(row, colFundingContact);
        }

        // Show the first funding option after loading the grid
        private void dgvFunding_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            int count = dgvFunding.Rows.Count;
            lblFundingCount.Text = count == 1 ? "1 funding option shown" : count + " funding options shown";
            if (count > 0)
            {
                dgvFunding.CurrentCell = dgvFunding.Rows[0].Cells[colFundingName.Index];
            }
            else
            {
                FundingClearDetail();
            }
        }

        // Format funding options as open or closed
        private void dgvFunding_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != colFundingStatus.Index)
            {
                return;
            }

            object closing = dgvFunding.Rows[e.RowIndex].Cells[colFundingClosingDate.Index].Value;
            bool open = closing == null || closing == DBNull.Value
                        || Convert.ToDateTime(closing, CultureInfo.InvariantCulture).Date >= DateTime.Today;
            e.Value = open ? "✓ Open" : "– Closed";
            e.CellStyle.Font = ChipFont;
            e.CellStyle.ForeColor = open ? GreenColour : MutedColour;
            e.CellStyle.BackColor = open ? GreenTint : RuleColour;
            e.FormattingApplied = true;
        }

        // Get a funding detail value or a dash if empty
        private static string FundingCellText(DataGridViewRow row, DataGridViewColumn column)
        {
            object value = row.Cells[column.Index].Value;
            string text = value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value, CultureInfo.CurrentCulture);
            return text.Length == 0 ? "—" : text;
        }

        // Clear the funding detail fields
        private void FundingClearDetail()
        {
            lblDetailHeading.Text = "Select a funding option to see its details";
            txtEligibility.Text = string.Empty;
            txtDocuments.Text = string.Empty;
            txtContact.Text = string.Empty;
        }

        #endregion
    }
}
