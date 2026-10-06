using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 13.3 — Reference Data, Funding tab, built to the UI 13.2 pattern (hosted by <see cref="ReferenceDataForm"/>).
    /// Funding options have no parent record to scope by, so the read-only grid of stored funding
    /// options (Funding name · Type · Closing date · Status chip) takes the scope list's place on the
    /// left, and the "Add or edit a funding option" form sits on the right: name, type (from
    /// <c>FUNDING_TYPE_n</c>), eligibility, documents, closing date, contact, merit / need flags, the
    /// linked faculties and programmes (<c>FUNDINGFACULTY</c> / <c>FUNDINGPROGRAMME</c>) and Active.
    /// The host's action band calls <see cref="AddNew"/>, <see cref="UpdateSelected"/>,
    /// <see cref="DeactivateSelected"/> and <see cref="CancelEdit"/>. Nothing is deleted, and the BLL
    /// re-validates every save and checks that the user is an Administrator.
    /// </summary>
    public partial class FundingTab : UserControl
    {
        // Colours used at runtime (spec 2.2.6). Everything else is set in the Designer.
        private static readonly Color InkColour = Color.FromArgb(27, 31, 36);
        private static readonly Color RedColour = Color.FromArgb(164, 38, 44);
        private static readonly Color RedTint = Color.FromArgb(250, 234, 235);
        private static readonly Color GreenColour = Color.FromArgb(31, 107, 58);
        private static readonly Color GreenTint = Color.FromArgb(230, 242, 234);
        private static readonly Color GreyColour = Color.FromArgb(88, 96, 108);
        private static readonly Color GreyTint = Color.FromArgb(236, 238, 241);

        private int? selectedFundingId;

        /// <summary>Creates the tab. Lists stay empty until the host calls <see cref="LoadData"/>.</summary>
        public FundingTab()
        {
            InitializeComponent();
            dgvFunding.AutoGenerateColumns = false;
            ClearForm();
            UpdateFundingCount();
        }

        #region Tab contract (UI 13 host)

        /// <summary>UserAccount.UserID of the signed-in administrator, passed to every BLL save.</summary>
        public int UserId { get; set; }

        /// <summary>UserAccount.Role of the signed-in user ("Administrator").</summary>
        public string Role { get; set; }

        /// <summary>Raised with a status-band message; the flag is true for a warning (amber).</summary>
        public event Action<string, bool> StatusChanged;

        /// <summary>The noun the host puts on its Add / Update buttons.</summary>
        public string ItemNoun
        {
            get { return "funding option"; }
        }

        /// <summary>Binds the funding type, faculty and programme lists and the grid. Called when the tab is first shown.</summary>
        public void LoadData()
        {
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
                Report("The funding type, faculty and programme lists could not be loaded. Please close this screen and try again.", true);
                return;
            }

            LoadFunding();
            Report("Select a funding option to edit it, or fill in the form to add one.", false);
        }

        /// <summary>Validates the form and adds a new funding option.</summary>
        public void AddNew()
        {
            if (!ValidateForm())
            {
                return;
            }

            string name = txtFundingName.Text.Trim();
            try
            {
                // TODO BLL: FundingController.SaveFunding(UserId, null, name, (string)cboFundingType.SelectedValue,
                //               txtEligibility.Text.Trim(), txtDocuments.Text.Trim(), ClosingDateOrNull(), txtContact.Text.Trim(),
                //               chkMeritBased.Checked, chkNeedBased.Checked, chkActive.Checked,
                //               CheckedIds(clbFaculties), CheckedIds(clbProgrammes));
                //           Writes FUNDINGOPTION plus its FUNDINGFACULTY / FUNDINGPROGRAMME links in one transaction.
                //           The BLL checks Role = Administrator before it writes (Section 8) and validates again.
            }
            catch (Exception)
            {
                Report("The funding option could not be added. Please check the entries and try again.", true);
                return;
            }

            LoadFunding();
            Report("Funding option added: " + name + ".", false);
        }

        /// <summary>Validates the form and saves it over the funding option selected in the grid.</summary>
        public void UpdateSelected()
        {
            if (!selectedFundingId.HasValue)
            {
                Report("Select a funding option in the grid first, then choose Update funding option.", true);
                return;
            }

            if (!ValidateForm())
            {
                return;
            }

            try
            {
                // TODO BLL: FundingController.SaveFunding(UserId, selectedFundingId, txtFundingName.Text.Trim(),
                //               (string)cboFundingType.SelectedValue, txtEligibility.Text.Trim(), txtDocuments.Text.Trim(),
                //               ClosingDateOrNull(), txtContact.Text.Trim(), chkMeritBased.Checked, chkNeedBased.Checked,
                //               chkActive.Checked, CheckedIds(clbFaculties), CheckedIds(clbProgrammes));
                //           Replaces the link rows for this FundingID. The BLL checks Role = Administrator before it writes.
            }
            catch (Exception)
            {
                Report("The funding option could not be updated. Please check the entries and try again.", true);
                return;
            }

            LoadFunding();
            Report("Funding option updated.", false);
        }

        /// <summary>Deactivates the selected funding option after confirmation. Nothing is deleted.</summary>
        public void DeactivateSelected()
        {
            if (!selectedFundingId.HasValue || dgvFunding.SelectedRows.Count == 0)
            {
                Report("Select a funding option in the grid first, then choose Deactivate.", true);
                return;
            }

            if (!RowIsActive(dgvFunding.SelectedRows[0]))
            {
                Report("This funding option is already inactive.", true);
                return;
            }

            string name = txtFundingName.Text.Trim();
            using (var dialog = new ConfirmationDialog("Deactivate funding option",
                "Deactivate " + name + "? It stays on record, but learners will no longer see it on the funding " +
                "options screen or in new advisory summaries.", "Deactivate"))
            {
                if (dialog.ShowDialog(FindForm()) != DialogResult.OK)
                {
                    return;
                }
            }

            try
            {
                // TODO BLL: FundingController.DeactivateFunding(UserId, selectedFundingId.Value);
                //           Sets FUNDINGOPTION.IsActive = 0. The BLL checks Role = Administrator before it writes.
            }
            catch (Exception)
            {
                Report("The funding option could not be deactivated. Please try again.", true);
                return;
            }

            LoadFunding();
            Report(name + " deactivated.", false);
        }

        /// <summary>Discards the form's changes and clears the grid selection.</summary>
        public void CancelEdit()
        {
            dgvFunding.ClearSelection();
            ClearForm();
            Report("Changes discarded. The form is ready for a new funding option.", false);
        }

        #endregion

        #region Business Logic Layer hooks

        /// <summary>Fills the grid with every stored funding option, active and inactive.</summary>
        private void LoadFunding()
        {
            dgvFunding.DataSource = null;
            ClearForm();
            try
            {
                // TODO BLL: dgvFunding.DataSource = FundingController.GetFundingOptions(includeInactive: true);
                //           Columns: FundingID, FundingName, FundingType, ClosingDate (null when none), IsActive.
            }
            catch (Exception)
            {
                Report("The funding options could not be loaded. Please try again.", true);
            }

            dgvFunding.ClearSelection();
            UpdateFundingCount();
        }

        /// <summary>Reads the selected option's full record into the form.</summary>
        private void LoadFundingDetail(int fundingId)
        {
            try
            {
                // TODO BLL: var funding = FundingController.GetFunding(fundingId);
                //           txtFundingName.Text = funding.FundingName; SelectValue(cboFundingType, funding.FundingType);
                //           txtEligibility.Text = funding.EligibilityDescription; txtDocuments.Text = funding.RequiredDocuments;
                //           SetClosingDate(funding.ClosingDate); txtContact.Text = funding.ContactInformation;
                //           chkMeritBased.Checked = funding.IsMeritBased; chkNeedBased.Checked = funding.IsNeedBased;
                //           chkActive.Checked = funding.IsActive;
                //           CheckIds(clbFaculties, funding.FacultyIds); CheckIds(clbProgrammes, funding.ProgrammeIds);
                Report("Editing the selected funding option. Choose Update funding option to save it, or Cancel.", false);
            }
            catch (Exception)
            {
                Report("The funding option could not be opened. Please try again.", true);
            }
        }

        /// <summary>An unticked closing date means "no closing date" (ClosingDate is nullable).</summary>
        private DateTime? ClosingDateOrNull()
        {
            return dtpClosingDate.Checked ? dtpClosingDate.Value.Date : (DateTime?)null;
        }

        /// <summary>Shows a stored closing date, or unticks the picker when there is none.</summary>
        private void SetClosingDate(DateTime? closingDate)
        {
            dtpClosingDate.Value = closingDate.HasValue ? closingDate.Value : DateTime.Today;
            dtpClosingDate.Checked = closingDate.HasValue;
        }

        /// <summary>The IDs (ValueMember) of every ticked item in a bound CheckedListBox.</summary>
        private static List<int> CheckedIds(CheckedListBox list)
        {
            var ids = new List<int>();
            foreach (object item in list.CheckedItems)
            {
                object value = ReadProperty(item, list.ValueMember);
                if (value != null && value != DBNull.Value)
                {
                    ids.Add(Convert.ToInt32(value, CultureInfo.InvariantCulture));
                }
            }

            return ids;
        }

        /// <summary>Ticks the items whose ID is in <paramref name="ids"/> and unticks the rest.</summary>
        private static void CheckIds(CheckedListBox list, ICollection<int> ids)
        {
            for (int i = 0; i < list.Items.Count; i++)
            {
                object value = ReadProperty(list.Items[i], list.ValueMember);
                bool ticked = ids != null && value != null && value != DBNull.Value
                              && ids.Contains(Convert.ToInt32(value, CultureInfo.InvariantCulture));
                list.SetItemChecked(i, ticked);
            }
        }

        #endregion

        #region Grid

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

            ClearValidation();
            selectedFundingId = Convert.ToInt32(id, CultureInfo.InvariantCulture);
            LoadFundingDetail(selectedFundingId.Value);
        }

        /// <summary>Display formatting only: status chips and a dash when there is no closing date.</summary>
        private void dgvFunding_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == colStatus.Index && e.Value is bool)
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

        private void UpdateFundingCount()
        {
            int active = 0;
            foreach (DataGridViewRow row in dgvFunding.Rows)
            {
                if (RowIsActive(row)) active++;
            }

            int total = dgvFunding.Rows.Count;
            lblFundingCount.Text = total + (total == 1 ? " funding option" : " funding options") + " · " + active +
                " active. Learners see active options only.";
        }

        #endregion

        #region Field validation (validated again in the BLL on save)

        /// <summary>Checks every required field, shows field messages and the validation summary.</summary>
        private bool ValidateForm()
        {
            var problems = new List<string>();
            if (!CheckFundingName()) problems.Add("Funding name");
            if (!CheckFundingType()) problems.Add("Funding type");
            if (!CheckEligibility()) problems.Add("Eligibility description");

            if (problems.Count == 0)
            {
                pnlSummary.Visible = false;
                return true;
            }

            lblSummary.Text = "⚠ " + problems.Count + (problems.Count == 1 ? " entry" : " entries") + " to correct: " +
                string.Join(", ", problems) + ".";
            pnlSummary.Visible = true;
            Report(problems.Count + (problems.Count == 1 ? " entry" : " entries") +
                " to correct before the funding option can be saved.", true);
            return false;
        }

        private bool CheckFundingName()
        {
            return CheckField(txtFundingName.Text.Trim().Length > 0, txtFundingName, lblFundingName, lblFundingNameError,
                "Enter the funding option's name.");
        }

        private bool CheckFundingType()
        {
            return CheckField(cboFundingType.SelectedIndex >= 0, cboFundingType, lblFundingType, lblFundingTypeError,
                "Choose a funding type.");
        }

        private bool CheckEligibility()
        {
            return CheckField(txtEligibility.Text.Trim().Length > 0, txtEligibility, lblEligibility, lblEligibilityError,
                "Describe who is eligible (up to 500 characters).");
        }

        /// <summary>
        /// Shows or clears the field message (spec 2.2.7): red text with a warning glyph under the
        /// control, a red tint on the control and a red label.
        /// </summary>
        private static bool CheckField(bool valid, Control input, Label fieldLabel, Label messageLabel, string message)
        {
            input.BackColor = valid ? SystemColors.Window : RedTint;
            fieldLabel.ForeColor = valid ? InkColour : RedColour;
            messageLabel.Text = valid ? string.Empty : "⚠ " + message;
            messageLabel.Visible = !valid;
            input.AccessibleDescription = valid ? null : message;
            return valid;
        }

        private void ClearValidation()
        {
            CheckField(true, txtFundingName, lblFundingName, lblFundingNameError, null);
            CheckField(true, cboFundingType, lblFundingType, lblFundingTypeError, null);
            CheckField(true, txtEligibility, lblEligibility, lblEligibilityError, null);
            pnlSummary.Visible = false;
        }

        private void txtFundingName_Leave(object sender, EventArgs e) { CheckFundingName(); }

        private void cboFundingType_Leave(object sender, EventArgs e) { CheckFundingType(); }

        private void txtEligibility_Leave(object sender, EventArgs e) { CheckEligibility(); }

        #endregion

        #region Helpers (this tab only)

        /// <summary>Empties the form, ready for a new funding option.</summary>
        private void ClearForm()
        {
            selectedFundingId = null;
            txtFundingName.Text = string.Empty;
            cboFundingType.SelectedIndex = -1;
            txtEligibility.Text = string.Empty;
            txtDocuments.Text = string.Empty;
            SetClosingDate(null);
            txtContact.Text = string.Empty;
            chkMeritBased.Checked = false;
            chkNeedBased.Checked = false;
            chkActive.Checked = true;
            CheckIds(clbFaculties, null);
            CheckIds(clbProgrammes, null);
            ClearValidation();
        }

        private bool RowIsActive(DataGridViewRow row)
        {
            object value = row.Cells[colStatus.Index].Value;
            return value is bool && (bool)value;
        }

        /// <summary>Reads a property from a bound list item (a BLL object or a DataRowView).</summary>
        private static object ReadProperty(object item, string name)
        {
            if (item == null || string.IsNullOrEmpty(name)) return null;
            PropertyDescriptor property = TypeDescriptor.GetProperties(item)[name];
            return property == null ? null : property.GetValue(item);
        }

        /// <summary>Selects a value in a bound ComboBox, or clears it when there is nothing to match.</summary>
        private static void SelectValue(ComboBox box, object value)
        {
            if (value == null || value == DBNull.Value || box.DataSource == null)
            {
                box.SelectedIndex = -1;
                return;
            }

            box.SelectedValue = value;
        }

        private void Report(string message, bool isWarning)
        {
            Action<string, bool> handler = StatusChanged;
            if (handler != null)
            {
                handler(message, isWarning);
            }
        }

        #endregion
    }
}
