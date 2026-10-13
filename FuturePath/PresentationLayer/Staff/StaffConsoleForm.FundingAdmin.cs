using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

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
    /// The console's action band calls <see cref="FundingAdminAddNew"/>, <see cref="FundingAdminUpdateSelected"/>,
    /// <see cref="FundingAdminDeactivateSelected"/> and <see cref="FundingAdminCancelEdit"/>. Nothing is
    /// deleted, and the BLL re-validates every save and checks that the user is an Administrator.
    /// </summary>
    public partial class StaffConsoleForm
    {
        /// <summary>FundingID of the funding option being edited, or null when the form holds a new one.</summary>
        private int? fundingAdminSelectedFundingId;

        #region Screen contract (called by the console)

        /// <summary>The noun the console puts on its Add / Update buttons.</summary>
        private string FundingAdminItemNoun
        {
            get { return "funding option"; }
        }

        /// <summary>
        /// Called each time UI 13.3 is shown. The first time it binds the funding type, faculty and
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
                // TODO BLL: cboFundingType.DataSource = FundingController.GetFundingTypes();
                //           Read from SystemParameter FUNDING_TYPE_1..4 (Section 7: funding type only from the predefined set).
                //           cboFundingType.DisplayMember = "ParameterValue"; cboFundingType.ValueMember = "ParameterValue";
                // TODO BLL: clbFaculties.DataSource = ReferenceDataController.GetFaculties(includeInactive: false);
                //           clbFaculties.DisplayMember = "FacultyName"; clbFaculties.ValueMember = "FacultyID";
                // TODO BLL: clbProgrammes.DataSource = ReferenceDataController.GetAllProgrammes(includeInactive: false);
                //           clbProgrammes.DisplayMember = "ProgrammeName"; clbProgrammes.ValueMember = "ProgrammeID";
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

        /// <summary>Validates the form and adds a new funding option.</summary>
        private void FundingAdminAddNew()
        {
            if (!FundingAdminValidateForm())
            {
                return;
            }

            string name = txtFundingName.Text.Trim();
            try
            {
                // TODO BLL: FundingController.SaveFunding(UserId, null, name, (string)cboFundingType.SelectedValue,
                //               txtEligibility.Text.Trim(), txtDocuments.Text.Trim(), FundingAdminClosingDateOrNull(), txtContact.Text.Trim(),
                //               chkMeritBased.Checked, chkNeedBased.Checked, chkFndActive.Checked,
                //               FundingAdminCheckedIds(clbFaculties), FundingAdminCheckedIds(clbProgrammes));
                //           Writes FUNDINGOPTION plus its FUNDINGFACULTY / FUNDINGPROGRAMME links in one transaction.
                //           The BLL checks Role = Administrator before it writes (Section 8) and validates again.
            }
            catch (Exception)
            {
                ReportStatus("The funding option could not be added. Please check the entries and try again.", true);
                return;
            }

            FundingAdminLoadFunding();
            ReportStatus("Funding option added: " + name + ".", false);
        }

        /// <summary>Validates the form and saves it over the funding option selected in the grid.</summary>
        private void FundingAdminUpdateSelected()
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
                // TODO BLL: FundingController.SaveFunding(UserId, fundingAdminSelectedFundingId, txtFundingName.Text.Trim(),
                //               (string)cboFundingType.SelectedValue, txtEligibility.Text.Trim(), txtDocuments.Text.Trim(),
                //               FundingAdminClosingDateOrNull(), txtContact.Text.Trim(), chkMeritBased.Checked, chkNeedBased.Checked,
                //               chkFndActive.Checked, FundingAdminCheckedIds(clbFaculties), FundingAdminCheckedIds(clbProgrammes));
                //           Replaces the link rows for this FundingID. The BLL checks Role = Administrator before it writes.
            }
            catch (Exception)
            {
                ReportStatus("The funding option could not be updated. Please check the entries and try again.", true);
                return;
            }

            FundingAdminLoadFunding();
            ReportStatus("Funding option updated.", false);
        }

        /// <summary>Deactivates the selected funding option after confirmation. Nothing is deleted.</summary>
        private void FundingAdminDeactivateSelected()
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
                // TODO BLL: FundingController.DeactivateFunding(UserId, fundingAdminSelectedFundingId.Value);
                //           Sets FUNDINGOPTION.IsActive = 0. The BLL checks Role = Administrator before it writes.
            }
            catch (Exception)
            {
                ReportStatus("The funding option could not be deactivated. Please try again.", true);
                return;
            }

            FundingAdminLoadFunding();
            ReportStatus(name + " deactivated.", false);
        }

        /// <summary>Discards the form's changes and clears the grid selection.</summary>
        private void FundingAdminCancelEdit()
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
                // TODO BLL: dgvFunding.DataSource = FundingController.GetFundingOptions(includeInactive: true);
                //           Columns: FundingID, FundingName, FundingType, ClosingDate (null when none), IsActive.
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
                // TODO BLL: var funding = FundingController.GetFunding(fundingId);
                //           txtFundingName.Text = funding.FundingName; FundingAdminSelectValue(cboFundingType, funding.FundingType);
                //           txtEligibility.Text = funding.EligibilityDescription; txtDocuments.Text = funding.RequiredDocuments;
                //           FundingAdminSetClosingDate(funding.ClosingDate); txtContact.Text = funding.ContactInformation;
                //           chkMeritBased.Checked = funding.IsMeritBased; chkNeedBased.Checked = funding.IsNeedBased;
                //           chkFndActive.Checked = funding.IsActive;
                //           FundingAdminCheckIds(clbFaculties, funding.FacultyIds); FundingAdminCheckIds(clbProgrammes, funding.ProgrammeIds);
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

        /// <summary>The IDs (ValueMember) of every ticked item in a bound CheckedListBox.</summary>
        private static List<int> FundingAdminCheckedIds(CheckedListBox list)
        {
            var ids = new List<int>();
            foreach (object item in list.CheckedItems)
            {
                object value = FundingAdminReadProperty(item, list.ValueMember);
                if (value != null && value != DBNull.Value)
                {
                    ids.Add(Convert.ToInt32(value, CultureInfo.InvariantCulture));
                }
            }

            return ids;
        }

        /// <summary>Ticks the items whose ID is in <paramref name="ids"/> and unticks the rest.</summary>
        private static void FundingAdminCheckIds(CheckedListBox list, ICollection<int> ids)
        {
            for (int i = 0; i < list.Items.Count; i++)
            {
                object value = FundingAdminReadProperty(list.Items[i], list.ValueMember);
                bool ticked = ids != null && value != null && value != DBNull.Value
                              && ids.Contains(Convert.ToInt32(value, CultureInfo.InvariantCulture));
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

        /// <summary>Display formatting only: green Active / grey Inactive status chips.</summary>
        private void dgvFunding_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == colFndStatus.Index && e.Value is bool)
            {
                bool active = (bool)e.Value;
                e.Value = active ? "Active" : "Inactive";
                e.CellStyle.ForeColor = active ? GreenColour : GreyColour;
                e.CellStyle.BackColor = active ? GreenTint : GreyTint;
                e.CellStyle.SelectionForeColor = e.CellStyle.ForeColor;
                e.CellStyle.SelectionBackColor = e.CellStyle.BackColor;
                e.FormattingApplied = true;
            }
        }

        /// <summary>Shows "n funding options · m active" above the grid.</summary>
        private void FundingAdminUpdateFundingCount()
        {
            int active = 0;
            foreach (DataGridViewRow row in dgvFunding.Rows)
            {
                if (FundingAdminRowIsActive(row)) active++;
            }

            int total = dgvFunding.Rows.Count;
            lblFundingCount.Text = total + (total == 1 ? " funding option" : " funding options") + " · " + active +
                " active. Learners see active options only.";
        }

        #endregion

        #region Field validation (validated again in the BLL on save)

        /// <summary>Checks every required field, shows field messages and the validation summary.</summary>
        private bool FundingAdminValidateForm()
        {
            var problems = new List<string>();
            if (!FundingAdminCheckFundingName()) problems.Add("Funding name");
            if (!FundingAdminCheckFundingType()) problems.Add("Funding type");
            if (!FundingAdminCheckEligibility()) problems.Add("Eligibility description");

            if (problems.Count == 0)
            {
                pnlFndSummary.Visible = false;
                return true;
            }

            lblFndSummary.Text = "⚠ " + problems.Count + (problems.Count == 1 ? " entry" : " entries") + " to correct: " +
                string.Join(", ", problems) + ".";
            pnlFndSummary.Visible = true;
            ReportStatus(problems.Count + (problems.Count == 1 ? " entry" : " entries") +
                " to correct before the funding option can be saved.", true);
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
            object value = row.Cells[colFndStatus.Index].Value;
            return value is bool && (bool)value;
        }

        /// <summary>Reads a property from a bound list item (a BLL object or a DataRowView).</summary>
        private static object FundingAdminReadProperty(object item, string name)
        {
            if (item == null || string.IsNullOrEmpty(name)) return null;
            PropertyDescriptor property = TypeDescriptor.GetProperties(item)[name];
            return property == null ? null : property.GetValue(item);
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
