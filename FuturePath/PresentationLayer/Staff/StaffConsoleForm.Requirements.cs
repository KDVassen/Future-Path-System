
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using FuturePath.ApplicationLayer;

namespace FuturePath.PresentationLayer
{
    // Handles degree programme requirements for Administrators
    // Allows requirements to be added, updated and deactivated
    public partial class StaffConsoleForm
    {
        // Value used when no alternative subject is selected
        private const int RequirementsNoAlternativeValue = 0;

        // ID of the selected requirement, or null for a new requirement
        private int? requirementsSelectedRequirementId;

        // Tracks whether the programme's minimum average is being edited
        private bool requirementsEditingMinimumAverage;

        #region Page shown

        // Initialise the requirements screen and load reference data
        private void RequirementsShown(bool firstTime)
        {
            if (!firstTime)
            {
                return;
            }

            dgvRequirements.AutoGenerateColumns = false;
            RequirementsClearForm();
            RequirementsUpdateRequirementCount();
            try
            {
                ReferenceDataController referenceDataController = new ReferenceDataController();

                // Load the available requirement types and subjects
                cboRequirementType.DisplayMember = "Description";
                cboRequirementType.ValueMember = "RequirementType";
                cboRequirementType.DataSource = referenceDataController.GetRequirementTypes();
                cboSubject.DisplayMember = "SubjectName";
                cboSubject.ValueMember = "SubjectID";
                cboSubject.DataSource = referenceDataController.GetSubjects(false);

                // Load alternative subjects, including None
                cboAlternative.DisplayMember = "SubjectName";
                cboAlternative.ValueMember = "SubjectID";
                cboAlternative.DataSource = referenceDataController.GetAlternativeSubjects();

                // Load faculties for selecting a programme
                cboReqFaculty.DisplayMember = "FacultyName";
                cboReqFaculty.ValueMember = "FacultyID";
                cboReqFaculty.DataSource = referenceDataController.GetFaculties(true, false);
                cboReqFaculty.SelectedIndex = -1;
                RequirementsClearForm();
                ReportStatus("Choose a faculty and a degree programme to see its stored requirements.", false);
            }
            catch (Exception)
            {
                ReportStatus("The faculty and subject lists could not be loaded. Please try again, or sign out and back in.", true);
            }
        }

        #endregion

        #region Button click events

        // Validate and add a new programme requirement
        private void btnRequirementsAdd_Click(object sender, EventArgs e)
        {
            if (lstProgrammes.SelectedIndex < 0)
            {
                ReportStatus("Choose a faculty and a degree programme before you add a requirement.", true);
                return;
            }

            // Enable subject fields when adding a requirement
            if (requirementsEditingMinimumAverage)
            {
                requirementsEditingMinimumAverage = false;
                RequirementsSetSubjectFieldsEnabled(true);
            }

            if (!RequirementsValidateForm())
            {
                return;
            }

            string subject = cboSubject.Text;
            try
            {
                // Save the new requirement through the controller
                // Zero represents no separate alternative minimum mark
                ReferenceDataController referenceDataController = new ReferenceDataController();
                referenceDataController.SaveRequirement(UserId, 0, (int)lstProgrammes.SelectedValue,
                    (string)cboRequirementType.SelectedValue, (int)cboSubject.SelectedValue, (int)cboAlternative.SelectedValue,
                    numMinimumMark.Value, 0m, chkReqActive.Checked, txtAdvisoryNote.Text.Trim());
            }
            catch (ArgumentException ex)
            {
                ReportStatus(ex.Message, true);
                return;
            }
            catch (Exception)
            {
                ReportStatus("The requirement could not be added. Please check the entries and try again.", true);
                return;
            }

            RequirementsLoadRequirements();
            ReportStatus("Requirement added for " + subject + ".", false);
        }

        // Validate and update the selected requirement
        private void btnRequirementsUpdate_Click(object sender, EventArgs e)
        {
            if (dgvRequirements.SelectedRows.Count == 0)
            {
                ReportStatus("Select a requirement in the grid first, then choose Update requirement.", true);
                return;
            }

            if (!RequirementsValidateForm())
            {
                return;
            }

            try
            {
                // Save changes through the controller
                ReferenceDataController referenceDataController = new ReferenceDataController();
                if (requirementsEditingMinimumAverage)
                {
                    // Update the programme's minimum overall average
                    referenceDataController.SaveMinimumAverage(UserId, (int)lstProgrammes.SelectedValue, numMinimumMark.Value);
                }
                else
                {
                    referenceDataController.SaveRequirement(UserId, requirementsSelectedRequirementId.Value, (int)lstProgrammes.SelectedValue,
                        (string)cboRequirementType.SelectedValue, (int)cboSubject.SelectedValue, (int)cboAlternative.SelectedValue,
                        numMinimumMark.Value, RequirementsStoredAlternativeMinimum(), chkReqActive.Checked, txtAdvisoryNote.Text.Trim());
                }
            }
            catch (ArgumentException ex)
            {
                ReportStatus(ex.Message, true);
                return;
            }
            catch (Exception)
            {
                ReportStatus("The requirement could not be updated. Please check the entries and try again.", true);
                return;
            }

            RequirementsLoadRequirements();
            ReportStatus("Requirement updated.", false);
        }

        // Deactivate the selected requirement after confirmation
        private void btnRequirementsDeactivate_Click(object sender, EventArgs e)
        {
            if (dgvRequirements.SelectedRows.Count == 0 || !requirementsSelectedRequirementId.HasValue)
            {
                ReportStatus(requirementsEditingMinimumAverage
                    ? "The minimum average is part of the programme. Change it here with Update requirement, or on University info — 13.1."
                    : "Select a requirement in the grid first, then choose Deactivate.", true);
                return;
            }

            if (!RequirementsRowIsActive(dgvRequirements.SelectedRows[0]))
            {
                ReportStatus("This requirement is already inactive.", true);
                return;
            }

            string subject = cboSubject.Text;
            if (!Confirm("Deactivate requirement",
                "Deactivate the " + subject + " requirement for " + RequirementsProgrammeName() + "? It stays on record and can be " +
                "made active again. New advisory sessions will no longer check it.", "Deactivate"))
            {
                return;
            }

            try
            {
                // Mark the requirement as inactive without deleting it
                ReferenceDataController referenceDataController = new ReferenceDataController();
                referenceDataController.DeactivateRequirement(UserId, requirementsSelectedRequirementId.Value);
            }
            catch (ArgumentException ex)
            {
                ReportStatus(ex.Message, true);
                return;
            }
            catch (Exception)
            {
                ReportStatus("The requirement could not be deactivated. Please try again.", true);
                return;
            }

            RequirementsLoadRequirements();
            ReportStatus("Requirement for " + subject + " deactivated.", false);
        }

        // Discard changes and reset the requirement form
        private void btnRequirementsCancel_Click(object sender, EventArgs e)
        {
            dgvRequirements.ClearSelection();
            RequirementsClearForm();
            ReportStatus("Changes discarded. The form is ready for a new requirement.", false);
        }

        #endregion

        #region Business Logic Layer hooks

        // Load all programmes for the selected faculty
        private void RequirementsLoadProgrammes()
        {
            lstProgrammes.DataSource = null;
            lstProgrammes.Items.Clear();
            if (cboReqFaculty.SelectedIndex >= 0)
            {
                try
                {
                    // Include active and inactive degree programmes
                    ReferenceDataController referenceDataController = new ReferenceDataController();
                    lstProgrammes.DisplayMember = "ProgrammeName";
                    lstProgrammes.ValueMember = "ProgrammeID";
                    lstProgrammes.DataSource = referenceDataController.GetProgrammes((int)cboReqFaculty.SelectedValue, true);
                    lstProgrammes.SelectedIndex = -1;
                }
                catch (Exception)
                {
                    ReportStatus("The programmes for this faculty could not be loaded. Please try again.", true);
                }
            }

            RequirementsUpdateProgrammeCount();
            RequirementsLoadRequirements();
        }

        // Load the selected programme's stored requirements
        private void RequirementsLoadRequirements()
        {
            dgvRequirements.DataSource = null;
            RequirementsClearForm();
            if (lstProgrammes.SelectedIndex < 0)
            {
                lblReqGridHeading.Text = "Stored requirements — no programme selected";
                RequirementsUpdateRequirementCount();
                return;
            }

            lblReqGridHeading.Text = "Stored requirements — " + RequirementsProgrammeName();
            try
            {
                // Load subject requirements and the programme's minimum average
                ReferenceDataController referenceDataController = new ReferenceDataController();
                dgvRequirements.DataSource = referenceDataController.GetRequirements((int)lstProgrammes.SelectedValue, true);
            }
            catch (Exception)
            {
                ReportStatus("The requirements for this programme could not be loaded. Please try again.", true);
            }

            dgvRequirements.ClearSelection();
            RequirementsUpdateRequirementCount();
        }

        // Preserve the stored alternative minimum mark when updating
        private decimal RequirementsStoredAlternativeMinimum()
        {
            if (dgvRequirements.SelectedRows.Count == 0)
            {
                return 0m;
            }

            DataRowView row = dgvRequirements.SelectedRows[0].DataBoundItem as DataRowView;
            if (row == null || row["AlternativeMinimumMark"] == DBNull.Value)
            {
                return 0m;
            }

            return (decimal)row["AlternativeMinimumMark"];
        }

        #endregion

        #region Scope list and grid

        // Load programmes when a faculty is selected
        private void cboReqFaculty_SelectedIndexChanged(object sender, EventArgs e)
        {
            RequirementsLoadProgrammes();
        }

        // Load requirements when a programme is selected
        private void lstProgrammes_SelectedIndexChanged(object sender, EventArgs e)
        {
            RequirementsLoadRequirements();
        }

        // Mark inactive programmes in the list
        private void lstProgrammes_Format(object sender, ListControlConvertEventArgs e)
        {
            if (RequirementsIsInactive(e.ListItem))
            {
                e.Value = Convert.ToString(e.Value, CultureInfo.CurrentCulture) + " (inactive)";
            }
        }

        // Update the total and inactive programme counts
        private void RequirementsUpdateProgrammeCount()
        {
            if (cboReqFaculty.SelectedIndex < 0)
            {
                lblReqProgrammeCount.Text = "Choose a faculty to list its programmes.";
                return;
            }

            int inactive = 0;
            foreach (object item in lstProgrammes.Items)
            {
                if (RequirementsIsInactive(item)) inactive++;
            }

            int total = lstProgrammes.Items.Count;
            lblReqProgrammeCount.Text = total + (total == 1 ? " programme" : " programmes") + " in this faculty · " + inactive +
                " inactive. Inactive programmes stay visible here and nowhere in the learner lane.";
        }

        // Update the total and active requirement counts
        private void RequirementsUpdateRequirementCount()
        {
            int active = 0;
            foreach (DataGridViewRow row in dgvRequirements.Rows)
            {
                if (RequirementsRowIsActive(row)) active++;
            }

            int total = dgvRequirements.Rows.Count;
            lblRequirementCount.Text = total + (total == 1 ? " requirement" : " requirements") + " · " + active + " active";
        }

        // Load the selected requirement into the editing form
        private void dgvRequirements_SelectionChanged(object sender, EventArgs e)
        {
            // Ignore automatic grid selection when the grid is not focused
            if (!dgvRequirements.Focused || dgvRequirements.SelectedRows.Count == 0)
            {
                return;
            }

            DataGridViewRow row = dgvRequirements.SelectedRows[0];
            RequirementsClearValidation();
            requirementsSelectedRequirementId = RequirementsToNullableInt(row.Cells[colRequirementsId.Index].Value);
            requirementsEditingMinimumAverage = !requirementsSelectedRequirementId.HasValue;
            RequirementsSetSubjectFieldsEnabled(!requirementsEditingMinimumAverage);

            RequirementsSelectValue(cboRequirementType, row.Cells[colRequirementsType.Index].Value);
            RequirementsSelectValue(cboSubject, row.Cells[colRequirementsSubjectId.Index].Value);
            object alternative = row.Cells[colRequirementsAlternativeSubjectId.Index].Value;
            RequirementsSelectValue(cboAlternative, RequirementsIsEmpty(alternative) ? (object)RequirementsNoAlternativeValue : alternative);

            object mark = row.Cells[colRequirementsMinimumMark.Index].Value;
            decimal value = RequirementsIsEmpty(mark) ? 0m : Convert.ToDecimal(mark, CultureInfo.InvariantCulture);
            numMinimumMark.Value = Math.Max(numMinimumMark.Minimum, Math.Min(numMinimumMark.Maximum, value));
            chkReqActive.Checked = RequirementsRowIsActive(row);
            txtAdvisoryNote.Text = Convert.ToString(row.Cells[colRequirementsAdvisoryNote.Index].Value, CultureInfo.CurrentCulture);

            ReportStatus(requirementsEditingMinimumAverage
                ? "Editing the programme's minimum average. Set the mark, then choose Update requirement."
                : "Editing the selected requirement. Choose Update requirement to save it, or Cancel.", false);
        }

        // Format requirement status, marks and empty values
        private void dgvRequirements_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == colRequirementsStatus.Index && e.Value is bool)
            {
                FormatStatusChip(e, (bool)e.Value);
            }
            else if ((e.ColumnIndex == colRequirementsSubject.Index || e.ColumnIndex == colRequirementsAlternative.Index
                      || e.ColumnIndex == colRequirementsMinimumMark.Index) && RequirementsIsEmpty(e.Value))
            {
                e.Value = "—";
                e.FormattingApplied = true;
            }
            else if (e.ColumnIndex == colRequirementsMinimumMark.Index && e.Value is decimal)
            {
                e.Value = ((decimal)e.Value).ToString("0.#'%'", CultureInfo.CurrentCulture);
                e.FormattingApplied = true;
            }
        }

        #endregion

        #region Field validation (validated again in the BLL on save)

        // Validate the requirement form and display any errors
        private bool RequirementsValidateForm()
        {
            var problems = new List<string>();
            var invalid = new List<Control>();
            if (!requirementsEditingMinimumAverage)
            {
                if (!RequirementsCheckRequirementType()) { problems.Add("Requirement type"); invalid.Add(cboRequirementType); }
                if (!RequirementsCheckSubject()) { problems.Add("Subject"); invalid.Add(cboSubject); }
                if (!RequirementsCheckAlternative()) { problems.Add("Accepted alternative"); invalid.Add(cboAlternative); }
            }

            if (!RequirementsCheckMinimumMark()) { problems.Add("Min. mark %"); invalid.Add(numMinimumMark); }

            if (problems.Count == 0)
            {
                pnlReqSummary.Visible = false;
                return true;
            }

            ShowFormSummary(pnlReqSummary, lblReqSummary, problems, invalid[0], "the requirement");
            return false;
        }

        // Check that a requirement type is selected
        private bool RequirementsCheckRequirementType()
        {
            return CheckField(cboRequirementType.SelectedIndex >= 0, cboRequirementType, lblRequirementType,
                lblRequirementTypeError, "Choose a requirement type.");
        }

        // Check that a subject is selected
        private bool RequirementsCheckSubject()
        {
            return CheckField(cboSubject.SelectedIndex >= 0, cboSubject, lblSubject, lblSubjectError, "Choose a subject.");
        }

        // Check that an alternative subject or None is selected
        private bool RequirementsCheckAlternative()
        {
            return CheckField(cboAlternative.SelectedIndex >= 0, cboAlternative, lblAlternative, lblAlternativeError,
                "Choose an alternative, or None.");
        }

        // Check that the minimum mark is between 0 and 100
        private bool RequirementsCheckMinimumMark()
        {
            // Validate the typed value before the control adjusts it
            decimal mark;
            bool valid = decimal.TryParse(numMinimumMark.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out mark)
                         && mark >= 0m && mark <= 100m;
            return CheckField(valid, numMinimumMark, lblMinimumMark, lblMinimumMarkError, "Enter a mark from 0 to 100.");
        }

        // Clear validation messages and hide the error summary
        private void RequirementsClearValidation()
        {
            CheckField(true, cboRequirementType, lblRequirementType, lblRequirementTypeError, null);
            CheckField(true, cboSubject, lblSubject, lblSubjectError, null);
            CheckField(true, cboAlternative, lblAlternative, lblAlternativeError, null);
            CheckField(true, numMinimumMark, lblMinimumMark, lblMinimumMarkError, null);
            pnlReqSummary.Visible = false;
        }

        // Validate requirement type when leaving the field
        private void cboRequirementType_Leave(object sender, EventArgs e) { if (!requirementsEditingMinimumAverage) RequirementsCheckRequirementType(); }

        // Validate subject when leaving the field
        private void cboSubject_Leave(object sender, EventArgs e) { if (!requirementsEditingMinimumAverage) RequirementsCheckSubject(); }

        // Validate alternative subject when leaving the field
        private void cboAlternative_Leave(object sender, EventArgs e) { if (!requirementsEditingMinimumAverage) RequirementsCheckAlternative(); }

        // Validate the minimum mark when leaving the field
        private void numMinimumMark_Leave(object sender, EventArgs e) { RequirementsCheckMinimumMark(); }

        #endregion

        #region Helpers (this screen only)

        // Reset the form for a new requirement
        private void RequirementsClearForm()
        {
            requirementsSelectedRequirementId = null;
            requirementsEditingMinimumAverage = false;
            RequirementsSetSubjectFieldsEnabled(true);
            cboRequirementType.SelectedIndex = -1;
            cboSubject.SelectedIndex = -1;
            cboAlternative.SelectedIndex = -1;
            numMinimumMark.Value = numMinimumMark.Minimum;
            chkReqActive.Checked = true;
            txtAdvisoryNote.Text = string.Empty;
            RequirementsClearValidation();
        }

        // Enable or disable subject fields when editing minimum averages
        private void RequirementsSetSubjectFieldsEnabled(bool enabled)
        {
            cboRequirementType.Enabled = enabled;
            cboSubject.Enabled = enabled;
            cboAlternative.Enabled = enabled;
            chkReqActive.Enabled = enabled;
            txtAdvisoryNote.Enabled = enabled;
        }

        // Get the name of the selected degree programme
        private string RequirementsProgrammeName()
        {
            return lstProgrammes.SelectedIndex < 0 ? string.Empty : lstProgrammes.GetItemText(lstProgrammes.SelectedItem);
        }

        // Check whether a requirement is active
        private bool RequirementsRowIsActive(DataGridViewRow row)
        {
            object value = row.Cells[colRequirementsStatus.Index].Value;
            return value is bool && (bool)value;
        }

        // Check whether a degree programme is inactive
        private static bool RequirementsIsInactive(object item)
        {
            DegreeProgramme programme = item as DegreeProgramme;
            return programme != null && !programme.IsActive;
        }

        // Check whether a stored value is empty
        private static bool RequirementsIsEmpty(object value)
        {
            return value == null || value == DBNull.Value || (value is string && ((string)value).Length == 0);
        }

        // Convert a value to an integer or null
        private static int? RequirementsToNullableInt(object value)
        {
            return RequirementsIsEmpty(value) ? (int?)null : Convert.ToInt32(value, CultureInfo.InvariantCulture);
        }

        // Select a dropdown value or clear the selection
        private static void RequirementsSelectValue(ComboBox box, object value)
        {
            if (RequirementsIsEmpty(value) || box.DataSource == null)
            {
                box.SelectedIndex = -1;
                return;
            }

            box.SelectedValue = value;
        }

        #endregion
    }
}
