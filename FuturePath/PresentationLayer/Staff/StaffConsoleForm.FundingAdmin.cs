using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using FuturePath.ApplicationLayer;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 13.3 — Reference data, Funding (maintenance), built to the UI 13.2 pattern. Page
    /// <c>tabFundingAdmin</c> of the Reference data group; Administrators only. (The learner's UI 6.2b
    /// Funding options screen is a different page, in MainForm.)
    /// <para>
    /// Funding options have no parent record to scope by, so the read-only grid of stored funding
    /// options (Funding name · Type · Closing date · Status chip) takes the scope list's place on the
    /// left, and the "Add or edit a funding option" form sits on the right: name, type (from
    /// <c>FUNDING_TYPE_n</c>), eligibility, documents, closing date, contact, merit / need flags, the
    /// linked faculties and programmes (<c>FUNDINGFACULTY</c> / <c>FUNDINGPROGRAMME</c>) and Active.
    /// </para>
    /// The tab's own buttons run <see cref="btnFundingAdminAdd_Click"/>, <see cref="btnFundingAdminUpdate_Click"/>,
    /// <see cref="btnFundingAdminDeactivate_Click"/> and <see cref="btnFundingAdminCancel_Click"/>. Nothing is
    /// deleted, and the BLL re-validates every save and checks that the user is an Administrator.
    /// </summary>
    public partial class StaffConsoleForm
    {
        /// <summary>FundingID of the funding option being edited, or null when the form holds a new one.</summary>
        private int? fundingAdminSelectedFundingId;

        #region Page shown

        /// <summary>
        /// Called by <see cref="PageShown"/> each time UI 13.3 is shown. The first time it binds the funding type, faculty and
        /// programme lists and the grid (replaces the old tab's constructor and <c>LoadData</c>).
        /// </summary>
        /// <param name="firstTime">True the first time this page is shown in this console.</param>
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
                // SystemParameter FUNDING_TYPE_1..4 (Section 7: funding type only from the predefined set).
                FundingController fundingController = new FundingController();
                cboFundingType.DisplayMember = "ParameterValue";
                cboFundingType.ValueMember = "FundingType";
                cboFundingType.DataSource = fundingController.GetFundingTypes(false);

                // The active faculties and every active programme (faculty 0 = all) that funding can be linked to.
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

        /// <summary>Add funding option: validates the form and adds a new funding option.</summary>
        private void btnFundingAdminAdd_Click(object sender, EventArgs e)
        {
            if (!FundingAdminValidateForm())
            {
                return;
            }

            string name = txtFundingName.Text.Trim();
            try
            {
                // FundingID 0 adds an option. Writes FundingOption plus its FundingFaculty / FundingProgramme links in one
                // transaction. The BLL checks Role = Administrator before it writes (Section 8) and validates again.
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

        /// <summary>Update funding option: validates the form and saves it over the funding option selected in the grid.</summary>
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
                // Replaces the link rows for this FundingID. The BLL checks Role = Administrator before it writes.
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

        /// <summary>Deactivate: deactivates the selected funding option after confirmation. Nothing is deleted.</summary>
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
                // Sets FundingOption.IsActive = 0. The BLL checks Role = Administrator before it writes.
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

        /// <summary>Cancel: discards the form's changes and clears the grid selection.</summary>
        private void btnFundingAdminCancel_Click(object sender, EventArgs e)
        {
            dgvFunding.ClearSelection();
            FundingAdminClearForm();
            ReportStatus("Changes discarded. The form is ready for a new funding option.", false);
        }

        #endregion

        #region Business Logic Layer hooks

        /// <summary>Fills the grid with every stored funding option, active and inactive.</summary>
        private void FundingAdminLoadFunding()
        {
            dgvFunding.DataSource = null;
            FundingAdminClearForm();
            try
            {
                // FundingOption objects: FundingID, FundingName, FundingType, ClosingDate (null when none), IsActive.
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

        /// <summary>Reads the selected option's full record into the form.</summary>
        /// <param name="fundingId">FundingOption.FundingID of the row selected in the grid.</param>
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

        /// <summary>An unticked closing date means "no closing date" (ClosingDate is nullable).</summary>
        private DateTime? FundingAdminClosingDateOrNull()
        {
            return dtpClosingDate.Checked ? dtpClosingDate.Value.Date : (DateTime?)null;
        }

        /// <summary>Shows a stored closing date, or unticks the picker when there is none.</summary>
        /// <param name="closingDate">FundingOption.ClosingDate, or null when the option has none.</param>
        private void FundingAdminSetClosingDate(DateTime? closingDate)
        {
            dtpClosingDate.Value = closingDate.HasValue ? closingDate.Value : DateTime.Today;
            dtpClosingDate.Checked = closingDate.HasValue;
        }

        /// <summary>The IDs of every ticked item in the faculty or programme list.</summary>
        private static List<int> FundingAdminCheckedIds(CheckedListBox list)
        {
            List<int> ids = new List<int>();
            foreach (object item in list.CheckedItems)
            {
                ids.Add(FundingAdminItemId(item));
            }

            return ids;
        }

        /// <summary>Ticks the items whose ID is in <paramref name="ids"/> and unticks the rest (null unticks all).</summary>
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

        /// <summary>Opens the funding option selected in the grid in the form.</summary>
        private void dgvFunding_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvFunding.SelectedRows.Count == 0)
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

        /// <summary>Shows IsActive as a status chip (display formatting only).</summary>
        private void dgvFunding_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == colFundingStatus.Index && e.Value is bool)
            {
                FormatStatusChip(e, (bool)e.Value);
            }
        }

        /// <summary>Shows "n funding options · m active" in the grid card's heading row.</summary>
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

        /// <summary>Checks every required field, shows field messages and the validation summary.</summary>
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

        /// <summary>Checks that a funding name was entered.</summary>
        private bool FundingAdminCheckFundingName()
        {
            return CheckField(txtFundingName.Text.Trim().Length > 0, txtFundingName, lblFundingName, lblFundingNameError,
                "Enter the funding option's name.");
        }

        /// <summary>Checks that a funding type is chosen.</summary>
        private bool FundingAdminCheckFundingType()
        {
            return CheckField(cboFundingType.SelectedIndex >= 0, cboFundingType, lblFundingType, lblFundingTypeError,
                "Choose a funding type.");
        }

        /// <summary>Checks that an eligibility description was entered.</summary>
        private bool FundingAdminCheckEligibility()
        {
            return CheckField(txtEligibility.Text.Trim().Length > 0, txtEligibility, lblEligibility, lblEligibilityError,
                "Describe who is eligible (up to 500 characters).");
        }

        /// <summary>Removes every field message and hides the validation summary.</summary>
        private void FundingAdminClearValidation()
        {
            CheckField(true, txtFundingName, lblFundingName, lblFundingNameError, null);
            CheckField(true, cboFundingType, lblFundingType, lblFundingTypeError, null);
            CheckField(true, txtEligibility, lblEligibility, lblEligibilityError, null);
            pnlFndSummary.Visible = false;
        }

        /// <summary>Validates the funding name on leaving it.</summary>
        private void txtFundingName_Leave(object sender, EventArgs e) { FundingAdminCheckFundingName(); }

        /// <summary>Validates the funding type on leaving it.</summary>
        private void cboFundingType_Leave(object sender, EventArgs e) { FundingAdminCheckFundingType(); }

        /// <summary>Validates the eligibility description on leaving it.</summary>
        private void txtEligibility_Leave(object sender, EventArgs e) { FundingAdminCheckEligibility(); }

        #endregion

        #region Helpers (this screen only)

        /// <summary>Empties the form, ready for a new funding option.</summary>
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

        /// <summary>True when the grid row's IsActive cell is true.</summary>
        private bool FundingAdminRowIsActive(DataGridViewRow row)
        {
            object value = row.Cells[colFundingStatus.Index].Value;
            return value is bool && (bool)value;
        }

        /// <summary>The ID of an item in the faculty list (a BLL Faculty) or the programme list (a DegreeProgramme).</summary>
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

        /// <summary>Selects a value in a bound ComboBox, or clears it when there is nothing to match.</summary>
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
