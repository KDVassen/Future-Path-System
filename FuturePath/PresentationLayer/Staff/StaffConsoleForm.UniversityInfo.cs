
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using FuturePath.ApplicationLayer;

namespace FuturePath.PresentationLayer
{
    // Handles faculty and degree programme maintenance for Administrators
    public partial class StaffConsoleForm
    {
        // ID of the selected programme, or null for a new programme
        private int? universitySelectedProgrammeId;

        #region Page shown

        // Initialise the university information screen
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

        // Validate and add a new degree programme
        private void btnUniversityAdd_Click(object sender, EventArgs e)
        {
            if (!UniversityValidateForm())
            {
                return;
            }

            string name = txtProgrammeName.Text.Trim();
            try
            {
                // Save the programme through the controller
                // A minimum average of zero means no minimum is set
                ReferenceDataController referenceDataController = new ReferenceDataController();
                referenceDataController.SaveProgramme(UserId, 0, (int)cboProgrammeFaculty.SelectedValue, name,
                    numMinimumAverage.Value, chkUniActive.Checked);
            }
            catch (ArgumentException ex)
            {
                ReportStatus(ex.Message, true);
                return;
            }
            catch (Exception)
            {
                ReportStatus("The programme could not be added. Please check the entries and try again.", true);
                return;
            }

            UniversityLoadProgrammes();
            ReportStatus("Programme added: " + name + ".", false);
        }

        // Validate and update the selected degree programme
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
                // Save programme changes through the controller
                ReferenceDataController referenceDataController = new ReferenceDataController();
                referenceDataController.SaveProgramme(UserId, universitySelectedProgrammeId.Value, (int)cboProgrammeFaculty.SelectedValue,
                    txtProgrammeName.Text.Trim(), numMinimumAverage.Value, chkUniActive.Checked);
            }
            catch (ArgumentException ex)
            {
                ReportStatus(ex.Message, true);
                return;
            }
            catch (Exception)
            {
                ReportStatus("The programme could not be updated. Please check the entries and try again.", true);
                return;
            }

            UniversityLoadProgrammes();
            ReportStatus("Programme updated.", false);
        }

        // Deactivate the selected programme after confirmation
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
                // Mark the programme as inactive without deleting it
                ReferenceDataController referenceDataController = new ReferenceDataController();
                referenceDataController.DeactivateProgramme(UserId, universitySelectedProgrammeId.Value);
            }
            catch (ArgumentException ex)
            {
                ReportStatus(ex.Message, true);
                return;
            }
            catch (Exception)
            {
                ReportStatus("The programme could not be deactivated. Please try again.", true);
                return;
            }

            UniversityLoadProgrammes();
            ReportStatus(name + " deactivated.", false);
        }

        // Discard changes and reset the programme form
        private void btnUniversityCancel_Click(object sender, EventArgs e)
        {
            dgvProgrammes.ClearSelection();
            UniversityClearForm();
            ReportStatus("Changes discarded. The form is ready for a new programme.", false);
        }

        #endregion

        #region Business Logic Layer hooks

        // Load faculties for the scope list and programme form
        private void UniversityLoadFaculties()
        {
            try
            {
                // Use separate lists for independent selections
                ReferenceDataController referenceDataController = new ReferenceDataController();
                cboProgrammeFaculty.DisplayMember = "FacultyName";
                cboProgrammeFaculty.ValueMember = "FacultyID";
                cboProgrammeFaculty.DataSource = referenceDataController.GetFaculties(false, false);
                lstFaculties.DisplayMember = "FacultyName";
                lstFaculties.ValueMember = "FacultyID";
                lstFaculties.DataSource = referenceDataController.GetFaculties(true, false);
                lstFaculties.SelectedIndex = -1;
                cboProgrammeFaculty.SelectedIndex = -1;
            }
            catch (Exception)
            {
                ReportStatus("The faculty list could not be loaded. Please try again, or sign out and back in.", true);
            }

            UniversityUpdateFacultyCount();
        }

        // Load programmes belonging to the selected faculty
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
                // Retrieve both active and inactive programmes
                ReferenceDataController referenceDataController = new ReferenceDataController();
                dgvProgrammes.DataSource = referenceDataController.GetProgrammes((int)lstFaculties.SelectedValue, true);
            }
            catch (Exception)
            {
                ReportStatus("The programmes for this faculty could not be loaded. Please try again.", true);
            }

            dgvProgrammes.ClearSelection();
            UniversityUpdateProgrammeCount();
        }

        #endregion

        #region Faculty group (its own amber buttons, saved at once)

        // Validate and add a new faculty
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
                // Save the new active faculty
                ReferenceDataController referenceDataController = new ReferenceDataController();
                referenceDataController.SaveFaculty(UserId, 0, name, true);
            }
            catch (ArgumentException ex)
            {
                ReportStatus(ex.Message, true);
                return;
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

        // Rename the selected faculty
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
                // Keep the faculty's existing active status
                Faculty faculty = (Faculty)lstFaculties.SelectedItem;
                ReferenceDataController referenceDataController = new ReferenceDataController();
                referenceDataController.SaveFaculty(UserId, faculty.FacultyID, name, faculty.IsActive);
            }
            catch (ArgumentException ex)
            {
                ReportStatus(ex.Message, true);
                return;
            }
            catch (Exception)
            {
                ReportStatus("The faculty could not be renamed. Please try again.", true);
                return;
            }

            UniversityLoadFaculties();
            ReportStatus("Faculty renamed to " + name + ".", false);
        }

        // Deactivate the selected faculty after confirmation
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
                // Mark the faculty as inactive without deleting it
                ReferenceDataController referenceDataController = new ReferenceDataController();
                referenceDataController.DeactivateFaculty(UserId, (int)lstFaculties.SelectedValue);
            }
            catch (ArgumentException ex)
            {
                ReportStatus(ex.Message, true);
                return;
            }
            catch (Exception)
            {
                ReportStatus("The faculty could not be deactivated. Please try again.", true);
                return;
            }

            UniversityLoadFaculties();
            ReportStatus(name + " deactivated.", false);
        }

        // Check that the faculty name is provided
        private bool UniversityCheckFacultyName()
        {
            return CheckField(txtFacultyName.Text.Trim().Length > 0, txtFacultyName, lblFacultyName, lblFacultyNameError,
                "Enter a faculty name.");
        }

        // Clear an earlier validation error when leaving the faculty name
        private void txtFacultyName_Leave(object sender, EventArgs e)
        {
            // Only check non-empty names when leaving the field
            if (txtFacultyName.Text.Trim().Length > 0) UniversityCheckFacultyName();
        }

        #endregion

        #region Scope list and grid

        // Display the selected faculty and its programmes
        private void lstFaculties_SelectedIndexChanged(object sender, EventArgs e)
        {
            Faculty faculty = lstFaculties.SelectedItem as Faculty;
            if (faculty == null)
            {
                txtFacultyName.Text = string.Empty;
            }
            else
            {
                txtFacultyName.Text = faculty.FacultyName;
            }

            CheckField(true, txtFacultyName, lblFacultyName, lblFacultyNameError, null);
            UniversityLoadProgrammes();
        }

        // Mark inactive faculties in the list
        private void lstFaculties_Format(object sender, ListControlConvertEventArgs e)
        {
            if (UniversityIsInactive(e.ListItem))
            {
                e.Value = Convert.ToString(e.Value, CultureInfo.CurrentCulture) + " (inactive)";
            }
        }

        // Update the total and inactive faculty counts
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

        // Update the total and active programme counts
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

        // Load the selected programme into the editing form
        private void dgvProgrammes_SelectionChanged(object sender, EventArgs e)
        {
            // Ignore automatic selections when the grid is not focused
            if (!dgvProgrammes.Focused || dgvProgrammes.SelectedRows.Count == 0)
            {
                return;
            }

            DataGridViewRow row = dgvProgrammes.SelectedRows[0];
            UniversityClearValidation();
            object id = row.Cells[colProgrammesId.Index].Value;
            universitySelectedProgrammeId = UniversityIsEmpty(id) ? (int?)null : Convert.ToInt32(id, CultureInfo.InvariantCulture);
            UniversitySelectValue(cboProgrammeFaculty, row.Cells[colProgrammesFacultyId.Index].Value);
            txtProgrammeName.Text = Convert.ToString(row.Cells[colProgrammesName.Index].Value, CultureInfo.CurrentCulture);
            object average = row.Cells[colProgrammesMinimumAverage.Index].Value;
            decimal value = UniversityIsEmpty(average) ? 0m : Convert.ToDecimal(average, CultureInfo.InvariantCulture);
            numMinimumAverage.Value = Math.Max(numMinimumAverage.Minimum, Math.Min(numMinimumAverage.Maximum, value));
            chkUniActive.Checked = UniversityRowIsActive(row);
            ReportStatus("Editing the selected programme. Choose Update programme to save it, or Cancel.", false);
        }

        // Format programme status and minimum average values
        private void dgvProgrammes_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == colProgrammesStatus.Index && e.Value is bool)
            {
                FormatStatusChip(e, (bool)e.Value);
            }
            else if (e.ColumnIndex == colProgrammesMinimumAverage.Index)
            {
                if (UniversityIsEmpty(e.Value))
                {
                    e.Value = "—";
                    e.FormattingApplied = true;
                }
                else if (e.Value is decimal)
                {
                    e.Value = ((decimal)e.Value).ToString("0.#'%'", CultureInfo.CurrentCulture);
                    e.FormattingApplied = true;
                }
            }
        }

        #endregion

        #region Field validation (validated again in the BLL on save)

        // Validate the programme form and display any errors
        private bool UniversityValidateForm()
        {
            var problems = new List<string>();
            var invalid = new List<Control>();
            if (!UniversityCheckProgrammeFaculty()) { problems.Add("Faculty"); invalid.Add(cboProgrammeFaculty); }
            if (!UniversityCheckProgrammeName()) { problems.Add("Programme name"); invalid.Add(txtProgrammeName); }
            if (!UniversityCheckMinimumAverage()) { problems.Add("Minimum overall average %"); invalid.Add(numMinimumAverage); }

            if (problems.Count == 0)
            {
                pnlUniSummary.Visible = false;
                return true;
            }

            ShowFormSummary(pnlUniSummary, lblUniSummary, problems, invalid[0], "the programme");
            return false;
        }

        // Check that a faculty is selected for the programme
        private bool UniversityCheckProgrammeFaculty()
        {
            return CheckField(cboProgrammeFaculty.SelectedIndex >= 0, cboProgrammeFaculty, lblProgrammeFaculty,
                lblProgrammeFacultyError, "Choose the programme's faculty.");
        }

        // Check that a programme name is entered
        private bool UniversityCheckProgrammeName()
        {
            return CheckField(txtProgrammeName.Text.Trim().Length > 0, txtProgrammeName, lblProgrammeName,
                lblProgrammeNameError, "Enter the programme name, e.g. BSc Computer Science.");
        }

        // Check that the minimum average is between 0 and 100
        private bool UniversityCheckMinimumAverage()
        {
            // Validate the typed value before the control adjusts it
            decimal average;
            bool valid = decimal.TryParse(numMinimumAverage.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out average)
                         && average >= 0m && average <= 100m;
            return CheckField(valid, numMinimumAverage, lblMinimumAverage, lblMinimumAverageError,
                "Enter an average from 0 to 100 (two decimals at most).");
        }

        // Clear validation messages and hide the error summary
        private void UniversityClearValidation()
        {
            CheckField(true, cboProgrammeFaculty, lblProgrammeFaculty, lblProgrammeFacultyError, null);
            CheckField(true, txtProgrammeName, lblProgrammeName, lblProgrammeNameError, null);
            CheckField(true, numMinimumAverage, lblMinimumAverage, lblMinimumAverageError, null);
            pnlUniSummary.Visible = false;
        }

        // Validate the selected faculty when leaving the field
        private void cboProgrammeFaculty_Leave(object sender, EventArgs e) { UniversityCheckProgrammeFaculty(); }

        // Validate the programme name when leaving the field
        private void txtProgrammeName_Leave(object sender, EventArgs e) { UniversityCheckProgrammeName(); }

        // Validate the minimum average when leaving the field
        private void numMinimumAverage_Leave(object sender, EventArgs e) { UniversityCheckMinimumAverage(); }

        #endregion

        #region Helpers (this screen only)

        // Reset the programme form for a new entry
        private void UniversityClearForm()
        {
            universitySelectedProgrammeId = null;
            UniversitySelectValue(cboProgrammeFaculty, lstFaculties.SelectedIndex < 0 ? null : lstFaculties.SelectedValue);
            txtProgrammeName.Text = string.Empty;
            numMinimumAverage.Value = numMinimumAverage.Minimum;
            chkUniActive.Checked = true;
            UniversityClearValidation();
        }

        // Check whether a programme is active
        private bool UniversityRowIsActive(DataGridViewRow row)
        {
            object value = row.Cells[colProgrammesStatus.Index].Value;
            return value is bool && (bool)value;
        }

        // Check whether a faculty is inactive
        private static bool UniversityIsInactive(object item)
        {
            Faculty faculty = item as Faculty;
            return faculty != null && !faculty.IsActive;
        }

        // Check whether a stored value is empty
        private static bool UniversityIsEmpty(object value)
        {
            return value == null || value == DBNull.Value || (value is string && ((string)value).Length == 0);
        }

        // Select a dropdown value or clear its selection
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
