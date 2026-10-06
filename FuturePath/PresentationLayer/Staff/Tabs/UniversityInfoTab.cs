using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 13.1 — Reference Data, University info tab (faculties and degree programmes), built to the
    /// UI 13.2 pattern (hosted by <see cref="ReferenceDataForm"/>).
    /// <para>
    /// Left: the Faculty ListBox scopes the view (inactive faculties stay listed, marked "(inactive)"),
    /// and a small "Faculty details" group below it maintains faculties themselves with its own amber
    /// group buttons — <b>Add faculty</b>, <b>Rename</b> and <b>Deactivate faculty</b> — which save at once.
    /// Right: a read-only grid of the selected faculty's programmes (Programme · Min. overall average ·
    /// Status chip) and the "Add or edit a programme" form, driven by the host's action band
    /// (<see cref="AddNew"/>, <see cref="UpdateSelected"/>, <see cref="DeactivateSelected"/>, <see cref="CancelEdit"/>).
    /// </para>
    /// Nothing is deleted; the BLL re-validates every save and checks that the user is an Administrator.
    /// </summary>
    public partial class UniversityInfoTab : UserControl
    {
        // Colours used at runtime (spec 2.2.6). Everything else is set in the Designer.
        private static readonly Color InkColour = Color.FromArgb(27, 31, 36);
        private static readonly Color RedColour = Color.FromArgb(164, 38, 44);
        private static readonly Color RedTint = Color.FromArgb(250, 234, 235);
        private static readonly Color GreenColour = Color.FromArgb(31, 107, 58);
        private static readonly Color GreenTint = Color.FromArgb(230, 242, 234);
        private static readonly Color GreyColour = Color.FromArgb(88, 96, 108);
        private static readonly Color GreyTint = Color.FromArgb(236, 238, 241);

        private int? selectedProgrammeId;

        /// <summary>Creates the tab. Lists stay empty until the host calls <see cref="LoadData"/>.</summary>
        public UniversityInfoTab()
        {
            InitializeComponent();
            dgvProgrammes.AutoGenerateColumns = false;
            ClearForm();
            UpdateProgrammeCount();
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
            get { return "programme"; }
        }

        /// <summary>Binds the faculty lists. Called when the tab is first shown.</summary>
        public void LoadData()
        {
            LoadFaculties();
            ClearForm();
            Report("Select a faculty to see its programmes. Faculty changes use the amber buttons under the list.", false);
        }

        /// <summary>Validates the form and adds a new degree programme.</summary>
        public void AddNew()
        {
            if (!ValidateForm())
            {
                return;
            }

            string name = txtProgrammeName.Text.Trim();
            try
            {
                // TODO BLL: ReferenceDataController.SaveProgramme(UserId, null, (int)cboProgrammeFaculty.SelectedValue, name,
                //               MinimumAverageOrNull(), chkActive.Checked);
                //           Writes DEGREEPROGRAMME. The BLL checks Role = Administrator before it writes (Section 8), validates again,
                //           and only accepts an existing faculty (referential integrity, Section 7).
            }
            catch (Exception)
            {
                Report("The programme could not be added. Please check the entries and try again.", true);
                return;
            }

            LoadProgrammes();
            Report("Programme added: " + name + ".", false);
        }

        /// <summary>Validates the form and saves it over the programme selected in the grid.</summary>
        public void UpdateSelected()
        {
            if (!selectedProgrammeId.HasValue)
            {
                Report("Select a programme in the grid first, then choose Update programme.", true);
                return;
            }

            if (!ValidateForm())
            {
                return;
            }

            try
            {
                // TODO BLL: ReferenceDataController.SaveProgramme(UserId, selectedProgrammeId, (int)cboProgrammeFaculty.SelectedValue,
                //               txtProgrammeName.Text.Trim(), MinimumAverageOrNull(), chkActive.Checked);
                //           The BLL checks Role = Administrator before it writes (Section 8).
            }
            catch (Exception)
            {
                Report("The programme could not be updated. Please check the entries and try again.", true);
                return;
            }

            LoadProgrammes();
            Report("Programme updated.", false);
        }

        /// <summary>Deactivates the selected programme after confirmation. Nothing is deleted.</summary>
        public void DeactivateSelected()
        {
            if (!selectedProgrammeId.HasValue || dgvProgrammes.SelectedRows.Count == 0)
            {
                Report("Select a programme in the grid first, then choose Deactivate.", true);
                return;
            }

            if (!RowIsActive(dgvProgrammes.SelectedRows[0]))
            {
                Report("This programme is already inactive.", true);
                return;
            }

            string name = txtProgrammeName.Text.Trim();
            using (var dialog = new ConfirmationDialog("Deactivate programme",
                "Deactivate " + name + "? It stays on record and in past advisory sessions, but new sessions, " +
                "alternatives and reports will no longer offer it.", "Deactivate"))
            {
                if (dialog.ShowDialog(FindForm()) != DialogResult.OK)
                {
                    return;
                }
            }

            try
            {
                // TODO BLL: ReferenceDataController.DeactivateProgramme(UserId, selectedProgrammeId.Value);
                //           Sets DEGREEPROGRAMME.IsActive = 0. The BLL checks Role = Administrator before it writes.
            }
            catch (Exception)
            {
                Report("The programme could not be deactivated. Please try again.", true);
                return;
            }

            LoadProgrammes();
            Report(name + " deactivated.", false);
        }

        /// <summary>Discards the form's changes and clears the grid selection.</summary>
        public void CancelEdit()
        {
            dgvProgrammes.ClearSelection();
            ClearForm();
            Report("Changes discarded. The form is ready for a new programme.", false);
        }

        #endregion

        #region Business Logic Layer hooks

        /// <summary>Binds the faculty scope list (with inactive faculties) and the form's faculty list (active only).</summary>
        private void LoadFaculties()
        {
            try
            {
                // TODO BLL: lstFaculties.DataSource = ReferenceDataController.GetFaculties(includeInactive: true);
                //           lstFaculties.DisplayMember = "FacultyName"; lstFaculties.ValueMember = "FacultyID";
                // TODO BLL: cboProgrammeFaculty.DataSource = ReferenceDataController.GetFaculties(includeInactive: false);
                //           cboProgrammeFaculty.DisplayMember = "FacultyName"; cboProgrammeFaculty.ValueMember = "FacultyID";
                lstFaculties.SelectedIndex = -1;
                cboProgrammeFaculty.SelectedIndex = -1;
            }
            catch (Exception)
            {
                Report("The faculty list could not be loaded. Please close this screen and try again.", true);
            }

            UpdateFacultyCount();
        }

        /// <summary>Fills the grid with the selected faculty's programmes, active and inactive.</summary>
        private void LoadProgrammes()
        {
            dgvProgrammes.DataSource = null;
            ClearForm();
            if (lstFaculties.SelectedIndex < 0)
            {
                lblGridHeading.Text = "Programmes — no faculty selected";
                UpdateProgrammeCount();
                return;
            }

            lblGridHeading.Text = "Programmes — " + lstFaculties.GetItemText(lstFaculties.SelectedItem);
            SelectValue(cboProgrammeFaculty, lstFaculties.SelectedValue);
            try
            {
                // TODO BLL: dgvProgrammes.DataSource = ReferenceDataController.GetProgrammes((int)lstFaculties.SelectedValue, includeInactive: true);
                //           Columns: ProgrammeID, FacultyID, ProgrammeName, MinimumOverallAverage, IsActive.
            }
            catch (Exception)
            {
                Report("The programmes for this faculty could not be loaded. Please try again.", true);
            }

            dgvProgrammes.ClearSelection();
            UpdateProgrammeCount();
        }

        /// <summary>
        /// 0.00 is what the form shows when no minimum average is set, so it is sent as null
        /// (DegreeProgramme.MinimumOverallAverage is nullable).
        /// </summary>
        private decimal? MinimumAverageOrNull()
        {
            return numMinimumAverage.Value == 0m ? (decimal?)null : numMinimumAverage.Value;
        }

        #endregion

        #region Faculty group (its own amber buttons, saved at once)

        private void btnAddFaculty_Click(object sender, EventArgs e)
        {
            if (!CheckFacultyName())
            {
                Report("Enter the new faculty's name, then choose Add faculty.", true);
                return;
            }

            string name = txtFacultyName.Text.Trim();
            try
            {
                // TODO BLL: ReferenceDataController.SaveFaculty(UserId, null, name, true);
                //           Writes FACULTY. The BLL checks Role = Administrator and refuses a duplicate name.
            }
            catch (Exception)
            {
                Report("The faculty could not be added. Please try again.", true);
                return;
            }

            LoadFaculties();
            txtFacultyName.Text = string.Empty;
            Report("Faculty added: " + name + ".", false);
        }

        private void btnRenameFaculty_Click(object sender, EventArgs e)
        {
            if (lstFaculties.SelectedIndex < 0)
            {
                Report("Select the faculty to rename in the list first.", true);
                return;
            }

            if (!CheckFacultyName())
            {
                Report("Enter the faculty's new name, then choose Rename.", true);
                return;
            }

            string name = txtFacultyName.Text.Trim();
            try
            {
                // TODO BLL: ReferenceDataController.SaveFaculty(UserId, (int)lstFaculties.SelectedValue, name, null);
                //           Renames FACULTY.FacultyName and keeps IsActive. The BLL checks Role = Administrator.
            }
            catch (Exception)
            {
                Report("The faculty could not be renamed. Please try again.", true);
                return;
            }

            LoadFaculties();
            Report("Faculty renamed to " + name + ".", false);
        }

        private void btnDeactivateFaculty_Click(object sender, EventArgs e)
        {
            if (lstFaculties.SelectedIndex < 0)
            {
                Report("Select the faculty to deactivate in the list first.", true);
                return;
            }

            if (IsInactive(lstFaculties.SelectedItem))
            {
                Report("This faculty is already inactive.", true);
                return;
            }

            string name = lstFaculties.GetItemText(lstFaculties.SelectedItem);
            using (var dialog = new ConfirmationDialog("Deactivate faculty",
                "Deactivate " + name + "? It stays on record, but learners will no longer be able to choose it " +
                "in new advisory sessions. Its programmes are not changed.", "Deactivate"))
            {
                if (dialog.ShowDialog(FindForm()) != DialogResult.OK)
                {
                    return;
                }
            }

            try
            {
                // TODO BLL: ReferenceDataController.DeactivateFaculty(UserId, (int)lstFaculties.SelectedValue);
                //           Sets FACULTY.IsActive = 0. The BLL checks Role = Administrator before it writes.
            }
            catch (Exception)
            {
                Report("The faculty could not be deactivated. Please try again.", true);
                return;
            }

            LoadFaculties();
            Report(name + " deactivated.", false);
        }

        private bool CheckFacultyName()
        {
            return CheckField(txtFacultyName.Text.Trim().Length > 0, txtFacultyName, lblFacultyName, lblFacultyNameError,
                "Enter a faculty name.");
        }

        private void txtFacultyName_Leave(object sender, EventArgs e)
        {
            // Blank is fine until a faculty button is pressed; only clear an earlier message here.
            if (txtFacultyName.Text.Trim().Length > 0) CheckFacultyName();
        }

        #endregion

        #region Scope list and grid

        private void lstFaculties_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFacultyName.Text = lstFaculties.SelectedIndex < 0 ? string.Empty
                : Convert.ToString(ReadProperty(lstFaculties.SelectedItem, "FacultyName"), CultureInfo.CurrentCulture);
            CheckField(true, txtFacultyName, lblFacultyName, lblFacultyNameError, null);
            LoadProgrammes();
        }

        /// <summary>Display formatting only: an inactive faculty stays listed with "(inactive)" after its name.</summary>
        private void lstFaculties_Format(object sender, ListControlConvertEventArgs e)
        {
            if (IsInactive(e.ListItem))
            {
                e.Value = Convert.ToString(e.Value, CultureInfo.CurrentCulture) + " (inactive)";
            }
        }

        private void UpdateFacultyCount()
        {
            int inactive = 0;
            foreach (object item in lstFaculties.Items)
            {
                if (IsInactive(item)) inactive++;
            }

            int total = lstFaculties.Items.Count;
            lblFacultyCount.Text = total + (total == 1 ? " faculty" : " faculties") + " · " + inactive +
                " inactive. Select one to list its programmes.";
        }

        private void UpdateProgrammeCount()
        {
            int active = 0;
            foreach (DataGridViewRow row in dgvProgrammes.Rows)
            {
                if (RowIsActive(row)) active++;
            }

            int total = dgvProgrammes.Rows.Count;
            lblProgrammeCount.Text = total + (total == 1 ? " programme" : " programmes") + " · " + active + " active";
        }

        /// <summary>Copies the selected programme into the form for editing.</summary>
        private void dgvProgrammes_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProgrammes.SelectedRows.Count == 0)
            {
                return;
            }

            DataGridViewRow row = dgvProgrammes.SelectedRows[0];
            ClearValidation();
            object id = row.Cells[colProgrammeId.Index].Value;
            selectedProgrammeId = IsEmpty(id) ? (int?)null : Convert.ToInt32(id, CultureInfo.InvariantCulture);
            SelectValue(cboProgrammeFaculty, row.Cells[colFacultyId.Index].Value);
            txtProgrammeName.Text = Convert.ToString(row.Cells[colProgrammeName.Index].Value, CultureInfo.CurrentCulture);
            object average = row.Cells[colMinimumAverage.Index].Value;
            decimal value = IsEmpty(average) ? 0m : Convert.ToDecimal(average, CultureInfo.InvariantCulture);
            numMinimumAverage.Value = Math.Max(numMinimumAverage.Minimum, Math.Min(numMinimumAverage.Maximum, value));
            chkActive.Checked = RowIsActive(row);
            Report("Editing the selected programme. Choose Update programme to save it, or Cancel.", false);
        }

        /// <summary>Display formatting only: status chips and a dash when no minimum average is set.</summary>
        private void dgvProgrammes_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
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
            else if (e.ColumnIndex == colMinimumAverage.Index)
            {
                if (IsEmpty(e.Value))
                {
                    e.Value = "—";
                    e.FormattingApplied = true;
                }
                else if (e.Value is decimal)
                {
                    e.Value = ((decimal)e.Value).ToString("0.00", CultureInfo.CurrentCulture) + " %";
                    e.FormattingApplied = true;
                }
            }
        }

        #endregion

        #region Field validation (validated again in the BLL on save)

        /// <summary>Checks every programme field, shows field messages and the validation summary.</summary>
        private bool ValidateForm()
        {
            var problems = new List<string>();
            if (!CheckProgrammeFaculty()) problems.Add("Faculty");
            if (!CheckProgrammeName()) problems.Add("Programme name");
            if (!CheckMinimumAverage()) problems.Add("Minimum overall average %");

            if (problems.Count == 0)
            {
                pnlSummary.Visible = false;
                return true;
            }

            lblSummary.Text = "⚠ " + problems.Count + (problems.Count == 1 ? " entry" : " entries") + " to correct: " +
                string.Join(", ", problems) + ".";
            pnlSummary.Visible = true;
            Report(problems.Count + (problems.Count == 1 ? " entry" : " entries") +
                " to correct before the programme can be saved.", true);
            return false;
        }

        private bool CheckProgrammeFaculty()
        {
            return CheckField(cboProgrammeFaculty.SelectedIndex >= 0, cboProgrammeFaculty, lblProgrammeFaculty,
                lblProgrammeFacultyError, "Choose the programme's faculty.");
        }

        private bool CheckProgrammeName()
        {
            return CheckField(txtProgrammeName.Text.Trim().Length > 0, txtProgrammeName, lblProgrammeName,
                lblProgrammeNameError, "Enter the programme name, e.g. BSc Computer Science.");
        }

        private bool CheckMinimumAverage()
        {
            // Read the typed text before the NumericUpDown clamps it on losing focus.
            decimal average;
            bool valid = decimal.TryParse(numMinimumAverage.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out average)
                         && average >= 0m && average <= 100m;
            return CheckField(valid, numMinimumAverage, lblMinimumAverage, lblMinimumAverageError,
                "Enter an average from 0 to 100 (two decimals at most).");
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
            CheckField(true, cboProgrammeFaculty, lblProgrammeFaculty, lblProgrammeFacultyError, null);
            CheckField(true, txtProgrammeName, lblProgrammeName, lblProgrammeNameError, null);
            CheckField(true, numMinimumAverage, lblMinimumAverage, lblMinimumAverageError, null);
            pnlSummary.Visible = false;
        }

        private void cboProgrammeFaculty_Leave(object sender, EventArgs e) { CheckProgrammeFaculty(); }

        private void txtProgrammeName_Leave(object sender, EventArgs e) { CheckProgrammeName(); }

        private void numMinimumAverage_Leave(object sender, EventArgs e) { CheckMinimumAverage(); }

        #endregion

        #region Helpers (this tab only)

        /// <summary>Empties the programme form; the faculty defaults to the one selected in the list.</summary>
        private void ClearForm()
        {
            selectedProgrammeId = null;
            SelectValue(cboProgrammeFaculty, lstFaculties.SelectedIndex < 0 ? null : lstFaculties.SelectedValue);
            txtProgrammeName.Text = string.Empty;
            numMinimumAverage.Value = numMinimumAverage.Minimum;
            chkActive.Checked = true;
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
            if (item == null) return null;
            PropertyDescriptor property = TypeDescriptor.GetProperties(item)[name];
            return property == null ? null : property.GetValue(item);
        }

        private static bool IsInactive(object item)
        {
            object value = ReadProperty(item, "IsActive");
            return value is bool && !(bool)value;
        }

        private static bool IsEmpty(object value)
        {
            return value == null || value == DBNull.Value || (value is string && ((string)value).Length == 0);
        }

        /// <summary>Selects a value in a bound ComboBox, or clears it when there is nothing to match.</summary>
        private static void SelectValue(ComboBox box, object value)
        {
            if (IsEmpty(value) || box.DataSource == null)
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
