
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using FuturePath.ApplicationLayer;

namespace FuturePath.PresentationLayer
{
    // Handles funding option maintenance for Administrators
    // Allows funding options to be added, updated and deactivated
    public partial class StaffConsoleForm
    {
        // ID of the selected funding option, or null for a new option
        private int? fundingAdminSelectedFundingId;

        #region Page shown

        // Load funding types, faculties, programmes and stored funding options
        private void FundingAdminShown(bool firstTime)
        {
            if (!firstTime)
            {
                return;
            }

            dgvFunding.AutoGenerateColumns = false;
            FundingAdminClearForm();
            FundingAdminUpdateFundingCount();
            try
            {
                // Load the predefined funding types
                FundingController fundingController = new FundingController();
                cboFundingType.DisplayMember = "ParameterValue";
                cboFundingType.ValueMember = "FundingType";
                cboFundingType.DataSource = fundingController.GetFundingTypes(false);

                // Load active faculties and programmes for funding links
                ReferenceDataController referenceDataController = new ReferenceDataController();
                clbFaculties.DisplayMember = "FacultyName";
                clbFaculties.ValueMember = "FacultyID";
                clbFaculties.DataSource = referenceDataController.GetFaculties(false, false);
                clbProgrammes.DisplayMember = "ProgrammeName";
                clbProgrammes.ValueMember = "ProgrammeID";
                clbProgrammes.DataSource = referenceDataController.GetProgrammes(0, false);
                cboFundingType.SelectedIndex = -1;
            }
            catch (Exception)
            {
                ReportStatus("The funding type, faculty and programme lists could not be loaded. Please try again, or sign out and back in.", true);
                return;
            }

            FundingAdminLoadFunding();
            ReportStatus("Select a funding option to edit it, or fill in the form to add one.", false);
        }

        #endregion

        #region Button click events

        // Validate and add a new funding option
        private void btnFundingAdminAdd_Click(object sender, EventArgs e)
        {
            if (!FundingAdminValidateForm())
            {
                return;
            }

            string name = txtFundingName.Text.Trim();
            try
            {
                // Save the new funding option and its faculty and programme links
                FundingController fundingController = new FundingController();
                fundingController.SaveFunding(UserId, 0, name, (string)cboFundingType.SelectedValue,
                    txtEligibility.Text.Trim(), txtDocuments.Text.Trim(), FundingAdminClosingDateOrNull(), txtContact.Text.Trim(),
                    chkMeritBased.Checked, chkNeedBased.Checked, chkFndActive.Checked,
                    FundingAdminCheckedIds(clbFaculties), FundingAdminCheckedIds(clbProgrammes));
            }
            catch (ArgumentException ex)
            {
                ReportStatus(ex.Message, true);
                return;
            }
            catch (Exception)
            {
                ReportStatus("The funding option could not be added. Please check the entries and try again.", true);
                return;
            }

            FundingAdminLoadFunding();
            ReportStatus("Funding option added: " + name + ".", false);
        }

        // Validate and update the selected funding option
        private void btnFundingAdminUpdate_Click(object sender, EventArgs e)
        {
            if (!fundingAdminSelectedFundingId.HasValue)
            {
                ReportStatus("Select a funding option in the grid first, then choose Update funding option.", true);
                return;
            }

            if (!FundingAdminValidateForm())
            {
                return;
            }

            try
            {
                // Update the funding option and its linked records
                FundingController fundingController = new FundingController();
                fundingController.SaveFunding(UserId, fundingAdminSelectedFundingId.Value, txtFundingName.Text.Trim(),
                    (string)cboFundingType.SelectedValue, txtEligibility.Text.Trim(), txtDocuments.Text.Trim(),
                    FundingAdminClosingDateOrNull(), txtContact.Text.Trim(), chkMeritBased.Checked, chkNeedBased.Checked,
                    chkFndActive.Checked, FundingAdminCheckedIds(clbFaculties), FundingAdminCheckedIds(clbProgrammes));
            }
            catch (ArgumentException ex)
            {
                ReportStatus(ex.Message, true);
                return;
            }
            catch (Exception)
            {
                ReportStatus("The funding option could not be updated. Please check the entries and try again.", true);
                return;
            }

            FundingAdminLoadFunding();
            ReportStatus("Funding option updated.", false);
        }

        // Deactivate the selected funding option after confirmation
        private void btnFundingAdminDeactivate_Click(object sender, EventArgs e)
        {
            if (!fundingAdminSelectedFundingId.HasValue || dgvFunding.SelectedRows.Count == 0)
            {
                ReportStatus("Select a funding option in the grid first, then choose Deactivate.", true);
                return;
            }

            if (!FundingAdminRowIsActive(dgvFunding.SelectedRows[0]))
            {
                ReportStatus("This funding option is already inactive.", true);
                return;
            }

            string name = txtFundingName.Text.Trim();
            if (!Confirm("Deactivate funding option",
                "Deactivate " + name + "? It stays on record, but learners will no longer see it on the funding " +
                "options screen or in new advisory summaries.", "Deactivate"))
            {
                return;
            }

            try
            {
                // Mark the funding option as inactive without deleting it
                FundingController fundingController = new FundingController();
                fundingController.DeactivateFunding(UserId, fundingAdminSelectedFundingId.Value);
            }
            catch (ArgumentException ex)
            {
                ReportStatus(ex.Message, true);
                return;
            }
            catch (Exception)
            {
                ReportStatus("The funding option could not be deactivated. Please try again.", true);
                return;
            }

            FundingAdminLoadFunding();
            ReportStatus(name + " deactivated.", false);
        }

        // Discard changes and clear the current funding selection
        private void btnFundingAdminCancel_Click(object sender, EventArgs e)
        {
            dgvFunding.ClearSelection();
            FundingAdminClearForm();
            ReportStatus("Changes discarded. The form is ready for a new funding option.", false);
        }

        #endregion

        #region Business Logic Layer hooks

        // Load all stored funding options into the grid
        private void FundingAdminLoadFunding()
        {
            dgvFunding.DataSource = null;
            FundingAdminClearForm();
            try
            {
                // Retrieve both active and inactive funding options
                FundingController fundingController = new FundingController();
                dgvFunding.DataSource = fundingController.GetFundingOptions(true);
            }
            catch (Exception)
            {
                ReportStatus("The funding options could not be loaded. Please try again.", true);
            }

            dgvFunding.ClearSelection();
            FundingAdminUpdateFundingCount();
        }

        // Load the selected funding option's details into the form
        private void FundingAdminLoadFundingDetail(int fundingId)
        {
            try
            {
                FundingController fundingController = new FundingController();
                FundingOption funding = fundingController.GetFunding(fundingId);
                if (funding == null)
                {
                    ReportStatus("The selected funding option no longer exists. Refresh the list and try again.", true);
                    return;
                }

                txtFundingName.Text = funding.FundingName;
                FundingAdminSelectValue(cboFundingType, funding.FundingType);
                txtEligibility.Text = funding.EligibilityDescription;
                txtDocuments.Text = funding.RequiredDocuments;
                FundingAdminSetClosingDate(funding.ClosingDate);
                txtContact.Text = funding.ContactInformation;
                chkMeritBased.Checked = funding.IsMeritBased;
                chkNeedBased.Checked = funding.IsNeedBased;
                chkFndActive.Checked = funding.IsActive;
                FundingAdminCheckIds(clbFaculties, funding.FacultyIds);
                FundingAdminCheckIds(clbProgrammes, funding.ProgrammeIds);
                ReportStatus("Editing the selected funding option. Choose Update funding option to save it, or Cancel.", false);
            }
            catch (Exception)
            {
                ReportStatus("The funding option could not be opened. Please try again.", true);
            }
        }

        // Return the selected closing date or null if none is chosen
        private DateTime? FundingAdminClosingDateOrNull()
        {
            return dtpClosingDate.Checked ? dtpClosingDate.Value.Date : (DateTime?)null;
        }

        // Display the saved closing date or clear the date selection
        private void FundingAdminSetClosingDate(DateTime? closingDate)
        {
            dtpClosingDate.Value = closingDate.HasValue ? closingDate.Value : DateTime.Today;
            dtpClosingDate.Checked = closingDate.HasValue;
        }

        // Get the IDs of checked faculties or programmes
        private static List<int> FundingAdminCheckedIds(CheckedListBox list)
        {
            List<int> ids = new List<int>();
            foreach (object item in list.CheckedItems)
            {
                ids.Add(FundingAdminItemId(item));
            }

            return ids;
        }

        // Check items whose IDs are included in the supplied list
        private static void FundingAdminCheckIds(CheckedListBox list, ICollection<int> ids)
        {
            for (int i = 0; i < list.Items.Count; i++)
            {
                bool ticked = ids != null && ids.Contains(FundingAdminItemId(list.Items[i]));
                list.SetItemChecked(i, ticked);
            }
        }

        #endregion

        #region Grid

        // Open the selected funding option for editing
        private void dgvFunding_SelectionChanged(object sender, EventArgs e)
        {
            // Ignore automatic selections when the grid is not focused
            if (!dgvFunding.Focused || dgvFunding.SelectedRows.Count == 0)
            {
                return;
            }

            object id = dgvFunding.SelectedRows[0].Cells[colFundingId.Index].Value;
            if (id == null || id == DBNull.Value)
            {
                return;
            }

            FundingAdminClearValidation();
            fundingAdminSelectedFundingId = Convert.ToInt32(id, CultureInfo.InvariantCulture);
            FundingAdminLoadFundingDetail(fundingAdminSelectedFundingId.Value);
        }

        // Format the funding status as active or inactive
        private void dgvFunding_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == colFundingStatus.Index && e.Value is bool)
            {
                FormatStatusChip(e, (bool)e.Value);
            }
        }

        // Update the total and active funding option counts
        private void FundingAdminUpdateFundingCount()
        {
            int active = 0;
            foreach (DataGridViewRow row in dgvFunding.Rows)
            {
                if (FundingAdminRowIsActive(row)) active++;
            }

            int total = dgvFunding.Rows.Count;
            lblFundingCount.Text = total + (total == 1 ? " funding option" : " funding options") + " · " + active + " active";
        }

        #endregion

        #region Field validation (validated again in the BLL on save)

        // Validate required funding fields and show any errors
        private bool FundingAdminValidateForm()
        {
            var problems = new List<string>();
            var invalid = new List<Control>();
            if (!FundingAdminCheckFundingName()) { problems.Add("Funding name"); invalid.Add(txtFundingName); }
            if (!FundingAdminCheckFundingType()) { problems.Add("Funding type"); invalid.Add(cboFundingType); }
            if (!FundingAdminCheckEligibility()) { problems.Add("Eligibility description"); invalid.Add(txtEligibility); }

            if (problems.Count == 0)
            {
                pnlFndSummary.Visible = false;
                return true;
            }

            ShowFormSummary(pnlFndSummary, lblFndSummary, problems, invalid[0], "the funding option");
            return false;
        }

        // Check that a funding name has been entered
        private bool FundingAdminCheckFundingName()
        {
            return CheckField(txtFundingName.Text.Trim().Length > 0, txtFundingName, lblFundingName, lblFundingNameError,
                "Enter the funding option's name.");
        }

        // Check that a funding type has been selected
        private bool FundingAdminCheckFundingType()
        {
            return CheckField(cboFundingType.SelectedIndex >= 0, cboFundingType, lblFundingType, lblFundingTypeError,
                "Choose a funding type.");
        }

        // Check that an eligibility description has been entered
        private bool FundingAdminCheckEligibility()
        {
            return CheckField(txtEligibility.Text.Trim().Length > 0, txtEligibility, lblEligibility, lblEligibilityError,
                "Describe who is eligible (up to 500 characters).");
        }

        // Clear validation messages and hide the error summary
        private void FundingAdminClearValidation()
        {
            CheckField(true, txtFundingName, lblFundingName, lblFundingNameError, null);
            CheckField(true, cboFundingType, lblFundingType, lblFundingTypeError, null);
            CheckField(true, txtEligibility, lblEligibility, lblEligibilityError, null);
            pnlFndSummary.Visible = false;
        }

        // Validate the funding name when leaving the field
        private void txtFundingName_Leave(object sender, EventArgs e) { FundingAdminCheckFundingName(); }

        // Validate the funding type when leaving the field
        private void cboFundingType_Leave(object sender, EventArgs e) { FundingAdminCheckFundingType(); }

        // Validate eligibility when leaving the field
        private void txtEligibility_Leave(object sender, EventArgs e) { FundingAdminCheckEligibility(); }

        #endregion

        #region Helpers (this screen only)

        // Clear the form for a new funding option
        private void FundingAdminClearForm()
        {
            fundingAdminSelectedFundingId = null;
            txtFundingName.Text = string.Empty;
            cboFundingType.SelectedIndex = -1;
            txtEligibility.Text = string.Empty;
            txtDocuments.Text = string.Empty;
            FundingAdminSetClosingDate(null);
            txtContact.Text = string.Empty;
            chkMeritBased.Checked = false;
            chkNeedBased.Checked = false;
            chkFndActive.Checked = true;
            FundingAdminCheckIds(clbFaculties, null);
            FundingAdminCheckIds(clbProgrammes, null);
            FundingAdminClearValidation();
        }

        // Check whether the selected funding row is active
        private bool FundingAdminRowIsActive(DataGridViewRow row)
        {
            object value = row.Cells[colFundingStatus.Index].Value;
            return value is bool && (bool)value;
        }

        // Get the ID of a faculty or degree programme
        private static int FundingAdminItemId(object item)
        {
            Faculty faculty = item as Faculty;
            if (faculty != null)
            {
                return faculty.FacultyID;
            }

            DegreeProgramme programme = item as DegreeProgramme;
            if (programme != null)
            {
                return programme.ProgrammeID;
            }

            return 0;
        }

        // Select a dropdown value or clear the selection if unavailable
        private static void FundingAdminSelectValue(ComboBox box, object value)
        {
            if (value == null || value == DBNull.Value || box.DataSource == null)
            {
                box.SelectedIndex = -1;
                return;
            }

            box.SelectedValue = value;
        }

        #endregion
    }
}
