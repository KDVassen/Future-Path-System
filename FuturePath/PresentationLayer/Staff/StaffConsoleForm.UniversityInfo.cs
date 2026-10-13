using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 13.1 — Reference data, University info (faculties and degree programmes), built to the UI 13.2
    /// pattern. Page <c>tabUniversity</c> of the Reference data group; Administrators only.
    /// <para>
    /// Left: the Faculty ListBox scopes the view (inactive faculties stay listed, marked "(inactive)"),
    /// and a small "Faculty details" group below it maintains faculties themselves with its own amber
    /// group buttons — <b>Add faculty</b>, <b>Rename</b> and <b>Deactivate faculty</b> — which save at once.
    /// Right: a read-only grid of the selected faculty's programmes (Programme · Min. overall average ·
    /// Status chip) and the "Add or edit a programme" form, driven by the tab's own buttons
    /// (<see cref="btnUniversityAdd_Click"/>, <see cref="btnUniversityUpdate_Click"/>,
    /// <see cref="btnUniversityDeactivate_Click"/>, <see cref="btnUniversityCancel_Click"/>).
    /// </para>
    /// Nothing is deleted; the BLL re-validates every save and checks that the user is an Administrator.
    /// </summary>
    public partial class StaffConsoleForm
    {
        /// <summary>ProgrammeID of the programme being edited, or null when the form holds a new programme.</summary>
        private int? universitySelectedProgrammeId;

        #region Page shown

        /// <summary>
        /// Called by <see cref="PageShown"/> each time UI 13.1 is shown. The first time it binds the faculty lists and empties the
        /// programme form (replaces the old tab's constructor and <c>LoadData</c>).
        /// </summary>
        /// <param name="firstTime">True the first time this page is shown in this console.</param>
        private void UniversityShown(bool firstTime)
        {
            if (!firstTime)
            {
                return;
            }

            dgvProgrammes.AutoGenerateColumns = false;
            UniversityClearForm();
            UniversityUpdateProgrammeCount();
            UniversityLoadFaculties();
            UniversityClearForm();
            ReportStatus("Select a faculty to see its programmes. Faculty changes use the amber buttons under the list.", false);
        }

        #endregion

        #region Button click events

        /// <summary>Add programme: validates the form and adds a new degree programme.</summary>
        private void btnUniversityAdd_Click(object sender, EventArgs e)
        {
            if (!UniversityValidateForm())
            {
                return;
            }

            string name = txtProgrammeName.Text.Trim();
            try
            {
                // TODO BLL: ReferenceDataController.SaveProgramme(UserId, null, (int)cboProgrammeFaculty.SelectedValue, name,
                //               UniversityMinimumAverageOrNull(), chkUniActive.Checked);
                //           Writes DEGREEPROGRAMME. The BLL checks Role = Administrator before it writes (Section 8), validates again,
                //           and only accepts an existing faculty (referential integrity, Section 7).
            }
            catch (Exception)
            {
                ReportStatus("The programme could not be added. Please check the entries and try again.", true);
                return;
            }

            UniversityLoadProgrammes();
            ReportStatus("Programme added: " + name + ".", false);
        }

        /// <summary>Update programme: validates the form and saves it over the programme selected in the grid.</summary>
        private void btnUniversityUpdate_Click(object sender, EventArgs e)
        {
            if (!universitySelectedProgrammeId.HasValue)
            {
                ReportStatus("Select a programme in the grid first, then choose Update programme.", true);
                return;
            }

            if (!UniversityValidateForm())
            {
                return;
            }

            try
            {
                // TODO BLL: ReferenceDataController.SaveProgramme(UserId, universitySelectedProgrammeId, (int)cboProgrammeFaculty.SelectedValue,
                //               txtProgrammeName.Text.Trim(), UniversityMinimumAverageOrNull(), chkUniActive.Checked);
                //           The BLL checks Role = Administrator before it writes (Section 8).
            }
            catch (Exception)
            {
                ReportStatus("The programme could not be updated. Please check the entries and try again.", true);
                return;
            }

            UniversityLoadProgrammes();
            ReportStatus("Programme updated.", false);
        }

        /// <summary>Deactivate: deactivates the selected programme after confirmation. Nothing is deleted.</summary>
        private void btnUniversityDeactivate_Click(object sender, EventArgs e)
        {
            if (!universitySelectedProgrammeId.HasValue || dgvProgrammes.SelectedRows.Count == 0)
            {
                ReportStatus("Select a programme in the grid first, then choose Deactivate.", true);
                return;
            }

            if (!UniversityRowIsActive(dgvProgrammes.SelectedRows[0]))
            {
                ReportStatus("This programme is already inactive.", true);
                return;
            }

            string name = txtProgrammeName.Text.Trim();
            if (!Confirm("Deactivate programme",
                "Deactivate " + name + "? It stays on record and in past advisory sessions, but new sessions, " +
                "alternatives and reports will no longer offer it.", "Deactivate"))
            {
                return;
            }

            try
            {
                // TODO BLL: ReferenceDataController.DeactivateProgramme(UserId, universitySelectedProgrammeId.Value);
                //           Sets DEGREEPROGRAMME.IsActive = 0. The BLL checks Role = Administrator before it writes.
            }
            catch (Exception)
            {
                ReportStatus("The programme could not be deactivated. Please try again.", true);
                return;
            }

            UniversityLoadProgrammes();
            ReportStatus(name + " deactivated.", false);
        }

        /// <summary>Cancel: discards the form's changes and clears the grid selection.</summary>
        private void btnUniversityCancel_Click(object sender, EventArgs e)
        {
            dgvProgrammes.ClearSelection();
            UniversityClearForm();
            ReportStatus("Changes discarded. The form is ready for a new programme.", false);
        }

        #endregion

        #region Business Logic Layer hooks

        /// <summary>Binds the faculty scope list (with inactive faculties) and the form's faculty list (active only).</summary>
        private void UniversityLoadFaculties()
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
                ReportStatus("The faculty list could not be loaded. Please try again, or sign out and back in.", true);
            }

            UniversityUpdateFacultyCount();
        }

        /// <summary>Fills the grid with the selected faculty's programmes, active and inactive.</summary>
        private void UniversityLoadProgrammes()
        {
            dgvProgrammes.DataSource = null;
            UniversityClearForm();
            if (lstFaculties.SelectedIndex < 0)
            {
                lblUniGridHeading.Text = "Programmes — no faculty selected";
                UniversityUpdateProgrammeCount();
                return;
            }

            lblUniGridHeading.Text = "Programmes — " + lstFaculties.GetItemText(lstFaculties.SelectedItem);
            UniversitySelectValue(cboProgrammeFaculty, lstFaculties.SelectedValue);
            try
            {
                // TODO BLL: dgvProgrammes.DataSource = ReferenceDataController.GetProgrammes((int)lstFaculties.SelectedValue, includeInactive: true);
                //           Columns: ProgrammeID, FacultyID, ProgrammeName, MinimumOverallAverage, IsActive.
            }
            catch (Exception)
            {
                ReportStatus("The programmes for this faculty could not be loaded. Please try again.", true);
            }

            dgvProgrammes.ClearSelection();
            UniversityUpdateProgrammeCount();
        }

        /// <summary>
        /// 0.00 is what the form shows when no minimum average is set, so it is sent as null
        /// (DegreeProgramme.MinimumOverallAverage is nullable).
        /// </summary>
        private decimal? UniversityMinimumAverageOrNull()
        {
            return numMinimumAverage.Value == 0m ? (decimal?)null : numMinimumAverage.Value;
        }

        #endregion

        #region Faculty group (its own amber buttons, saved at once)

        /// <summary>Amber "Add faculty": adds the name typed in Faculty details as a new, active faculty.</summary>
        private void btnAddFaculty_Click(object sender, EventArgs e)
        {
            if (!UniversityCheckFacultyName())
            {
                ReportStatus("Enter the new faculty's name, then choose Add faculty.", true);
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
                ReportStatus("The faculty could not be added. Please try again.", true);
                return;
            }

            UniversityLoadFaculties();
            txtFacultyName.Text = string.Empty;
            ReportStatus("Faculty added: " + name + ".", false);
        }

        /// <summary>Amber "Rename": renames the faculty selected in the list to the name typed in Faculty details.</summary>
        private void btnRenameFaculty_Click(object sender, EventArgs e)
        {
            if (lstFaculties.SelectedIndex < 0)
            {
                ReportStatus("Select the faculty to rename in the list first.", true);
                return;
            }

            if (!UniversityCheckFacultyName())
            {
                ReportStatus("Enter the faculty's new name, then choose Rename.", true);
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
                ReportStatus("The faculty could not be renamed. Please try again.", true);
                return;
            }

            UniversityLoadFaculties();
            ReportStatus("Faculty renamed to " + name + ".", false);
        }

        /// <summary>Amber "Deactivate faculty": deactivates the selected faculty after confirmation. Nothing is deleted.</summary>
        private void btnDeactivateFaculty_Click(object sender, EventArgs e)
        {
            if (lstFaculties.SelectedIndex < 0)
            {
                ReportStatus("Select the faculty to deactivate in the list first.", true);
                return;
            }

            if (UniversityIsInactive(lstFaculties.SelectedItem))
            {
                ReportStatus("This faculty is already inactive.", true);
                return;
            }

            string name = lstFaculties.GetItemText(lstFaculties.SelectedItem);
            if (!Confirm("Deactivate faculty",
                "Deactivate " + name + "? It stays on record, but learners will no longer be able to choose it " +
                "in new advisory sessions. Its programmes are not changed.", "Deactivate"))
            {
                return;
            }

            try
            {
                // TODO BLL: ReferenceDataController.DeactivateFaculty(UserId, (int)lstFaculties.SelectedValue);
                //           Sets FACULTY.IsActive = 0. The BLL checks Role = Administrator before it writes.
            }
            catch (Exception)
            {
                ReportStatus("The faculty could not be deactivated. Please try again.", true);
                return;
            }

            UniversityLoadFaculties();
            ReportStatus(name + " deactivated.", false);
        }

        /// <summary>Checks the Faculty details name field. Returns true when a name was entered.</summary>
        private bool UniversityCheckFacultyName()
        {
            return CheckField(txtFacultyName.Text.Trim().Length > 0, txtFacultyName, lblFacultyName, lblFacultyNameError,
                "Enter a faculty name.");
        }

        /// <summary>Leaving the faculty name: blank is fine until a faculty button is pressed, so only an earlier message is cleared.</summary>
        private void txtFacultyName_Leave(object sender, EventArgs e)
        {
            // Blank is fine until a faculty button is pressed; only clear an earlier message here.
            if (txtFacultyName.Text.Trim().Length > 0) UniversityCheckFacultyName();
        }

        #endregion

        #region Scope list and grid

        /// <summary>A faculty was chosen: copies its name into Faculty details and lists its programmes.</summary>
        private void lstFaculties_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFacultyName.Text = lstFaculties.SelectedIndex < 0 ? string.Empty
                : Convert.ToString(UniversityReadProperty(lstFaculties.SelectedItem, "FacultyName"), CultureInfo.CurrentCulture);
            CheckField(true, txtFacultyName, lblFacultyName, lblFacultyNameError, null);
            UniversityLoadProgrammes();
        }

        /// <summary>Display formatting only: an inactive faculty stays listed with "(inactive)" after its name.</summary>
        private void lstFaculties_Format(object sender, ListControlConvertEventArgs e)
        {
            if (UniversityIsInactive(e.ListItem))
            {
                e.Value = Convert.ToString(e.Value, CultureInfo.CurrentCulture) + " (inactive)";
            }
        }

        /// <summary>Shows "n faculties · m inactive" under the faculty list.</summary>
        private void UniversityUpdateFacultyCount()
        {
            int inactive = 0;
            foreach (object item in lstFaculties.Items)
            {
                if (UniversityIsInactive(item)) inactive++;
            }

            int total = lstFaculties.Items.Count;
            lblFacultyCount.Text = total + (total == 1 ? " faculty" : " faculties") + " · " + inactive +
                " inactive. Select one to list its programmes.";
        }

        /// <summary>Shows "n programmes · m active" above the grid.</summary>
        private void UniversityUpdateProgrammeCount()
        {
            int active = 0;
            foreach (DataGridViewRow row in dgvProgrammes.Rows)
            {
                if (UniversityRowIsActive(row)) active++;
            }

            int total = dgvProgrammes.Rows.Count;
            lblUniProgrammeCount.Text = total + (total == 1 ? " programme" : " programmes") + " · " + active + " active";
        }

        /// <summary>Copies the selected programme into the form for editing.</summary>
        private void dgvProgrammes_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProgrammes.SelectedRows.Count == 0)
            {
                return;
            }

            DataGridViewRow row = dgvProgrammes.SelectedRows[0];
            UniversityClearValidation();
            object id = row.Cells[colProgrammeId.Index].Value;
            universitySelectedProgrammeId = UniversityIsEmpty(id) ? (int?)null : Convert.ToInt32(id, CultureInfo.InvariantCulture);
            UniversitySelectValue(cboProgrammeFaculty, row.Cells[colFacultyId.Index].Value);
            txtProgrammeName.Text = Convert.ToString(row.Cells[colProgrammeName.Index].Value, CultureInfo.CurrentCulture);
            object average = row.Cells[colMinimumAverage.Index].Value;
            decimal value = UniversityIsEmpty(average) ? 0m : Convert.ToDecimal(average, CultureInfo.InvariantCulture);
            numMinimumAverage.Value = Math.Max(numMinimumAverage.Minimum, Math.Min(numMinimumAverage.Maximum, value));
            chkUniActive.Checked = UniversityRowIsActive(row);
            ReportStatus("Editing the selected programme. Choose Update programme to save it, or Cancel.", false);
        }

        /// <summary>Display formatting only: status chips and a dash when no minimum average is set.</summary>
        private void dgvProgrammes_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == colUniStatus.Index && e.Value is bool)
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
                if (UniversityIsEmpty(e.Value))
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
        private bool UniversityValidateForm()
        {
            var problems = new List<string>();
            if (!UniversityCheckProgrammeFaculty()) problems.Add("Faculty");
            if (!UniversityCheckProgrammeName()) problems.Add("Programme name");
            if (!UniversityCheckMinimumAverage()) problems.Add("Minimum overall average %");

            if (problems.Count == 0)
            {
                pnlUniSummary.Visible = false;
                return true;
            }

            lblUniSummary.Text = "⚠ " + problems.Count + (problems.Count == 1 ? " entry" : " entries") + " to correct: " +
                string.Join(", ", problems) + ".";
            pnlUniSummary.Visible = true;
            ReportStatus(problems.Count + (problems.Count == 1 ? " entry" : " entries") +
                " to correct before the programme can be saved.", true);
            return false;
        }

        /// <summary>Checks that the programme's faculty is chosen.</summary>
        private bool UniversityCheckProgrammeFaculty()
        {
            return CheckField(cboProgrammeFaculty.SelectedIndex >= 0, cboProgrammeFaculty, lblProgrammeFaculty,
                lblProgrammeFacultyError, "Choose the programme's faculty.");
        }

        /// <summary>Checks that a programme name was entered.</summary>
        private bool UniversityCheckProgrammeName()
        {
            return CheckField(txtProgrammeName.Text.Trim().Length > 0, txtProgrammeName, lblProgrammeName,
                lblProgrammeNameError, "Enter the programme name, e.g. BSc Computer Science.");
        }

        /// <summary>Checks that the minimum overall average is a number from 0 to 100.</summary>
        private bool UniversityCheckMinimumAverage()
        {
            // Read the typed text before the NumericUpDown clamps it on losing focus.
            decimal average;
            bool valid = decimal.TryParse(numMinimumAverage.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out average)
                         && average >= 0m && average <= 100m;
            return CheckField(valid, numMinimumAverage, lblMinimumAverage, lblMinimumAverageError,
                "Enter an average from 0 to 100 (two decimals at most).");
        }

        /// <summary>Removes every programme field message and hides the validation summary.</summary>
        private void UniversityClearValidation()
        {
            CheckField(true, cboProgrammeFaculty, lblProgrammeFaculty, lblProgrammeFacultyError, null);
            CheckField(true, txtProgrammeName, lblProgrammeName, lblProgrammeNameError, null);
            CheckField(true, numMinimumAverage, lblMinimumAverage, lblMinimumAverageError, null);
            pnlUniSummary.Visible = false;
        }

        /// <summary>Validates the faculty when the administrator leaves it.</summary>
        private void cboProgrammeFaculty_Leave(object sender, EventArgs e) { UniversityCheckProgrammeFaculty(); }

        /// <summary>Validates the programme name when the administrator leaves it.</summary>
        private void txtProgrammeName_Leave(object sender, EventArgs e) { UniversityCheckProgrammeName(); }

        /// <summary>Validates the minimum average when the administrator leaves it.</summary>
        private void numMinimumAverage_Leave(object sender, EventArgs e) { UniversityCheckMinimumAverage(); }

        #endregion

        #region Helpers (this screen only)

        /// <summary>Empties the programme form; the faculty defaults to the one selected in the list.</summary>
        private void UniversityClearForm()
        {
            universitySelectedProgrammeId = null;
            UniversitySelectValue(cboProgrammeFaculty, lstFaculties.SelectedIndex < 0 ? null : lstFaculties.SelectedValue);
            txtProgrammeName.Text = string.Empty;
            numMinimumAverage.Value = numMinimumAverage.Minimum;
            chkUniActive.Checked = true;
            UniversityClearValidation();
        }

        /// <summary>True when the grid row's IsActive cell is true.</summary>
        private bool UniversityRowIsActive(DataGridViewRow row)
        {
            object value = row.Cells[colUniStatus.Index].Value;
            return value is bool && (bool)value;
        }

        /// <summary>Reads a property from a bound list item (a BLL object or a DataRowView).</summary>
        private static object UniversityReadProperty(object item, string name)
        {
            if (item == null) return null;
            PropertyDescriptor property = TypeDescriptor.GetProperties(item)[name];
            return property == null ? null : property.GetValue(item);
        }

        /// <summary>True when a bound list item carries IsActive = false.</summary>
        private static bool UniversityIsInactive(object item)
        {
            object value = UniversityReadProperty(item, "IsActive");
            return value is bool && !(bool)value;
        }

        /// <summary>True for null, DBNull and the empty string.</summary>
        private static bool UniversityIsEmpty(object value)
        {
            return value == null || value == DBNull.Value || (value is string && ((string)value).Length == 0);
        }

        /// <summary>Selects a value in a bound ComboBox, or clears it when there is nothing to match.</summary>
        private static void UniversitySelectValue(ComboBox box, object value)
        {
            if (UniversityIsEmpty(value) || box.DataSource == null)
            {
                box.SelectedIndex = -1;
                return;
            }

            box.SelectedValue = value;
        }

        #endregion
    }
}
