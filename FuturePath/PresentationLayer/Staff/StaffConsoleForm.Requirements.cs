using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 13.2 — Reference data, Degree requirements (spec 2.3.11). Page <c>tabRequirements</c> of the
    /// Reference data group; Administrators only.
    /// The administrator scopes the view with a Faculty ComboBox and a Degree programme ListBox
    /// (inactive programmes stay listed, marked "(inactive)"), reads the stored requirements in a
    /// read-only grid with green Active / grey Inactive status chips, and maintains one requirement
    /// at a time in the "Add or edit a requirement" form, driven by the tab's own buttons
    /// (<see cref="btnRequirementsAdd_Click"/>, <see cref="btnRequirementsUpdate_Click"/>,
    /// <see cref="btnRequirementsDeactivate_Click"/>, <see cref="btnRequirementsCancel_Click"/>). Nothing is
    /// ever deleted, and every save is re-checked by the BLL, which also confirms that the signed-in user
    /// is an Administrator.
    /// </summary>
    public partial class StaffConsoleForm
    {
        /// <summary>
        /// SubjectID carried by the BLL's "None" entry in the Accepted alternative list. It is stored
        /// as a NULL <c>AlternativeSubjectID</c>; the value itself is supplied by the BLL, not by this form.
        /// </summary>
        private const int RequirementsNoAlternativeValue = 0;

        /// <summary>RequirementID of the row being edited; null for a new requirement or the Minimum average row.</summary>
        private int? requirementsSelectedRequirementId;

        /// <summary>True while the programme's "Minimum average" row (DegreeProgramme.MinimumOverallAverage) is being edited.</summary>
        private bool requirementsEditingMinimumAverage;

        #region Page shown

        /// <summary>
        /// Called by <see cref="PageShown"/> each time UI 13.2 is shown. The first time it binds the faculty scope list and the form's
        /// lists and empties the form (replaces the old tab's constructor and <c>LoadData</c>).
        /// </summary>
        /// <param name="firstTime">True the first time this page is shown in this console.</param>
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
                // TODO BLL: cboReqFaculty.DataSource = ReferenceDataController.GetFaculties(includeInactive: true);
                //           cboReqFaculty.DisplayMember = "FacultyName"; cboReqFaculty.ValueMember = "FacultyID";
                // TODO BLL: cboRequirementType.DataSource = ReferenceDataController.GetRequirementTypes();
                //           Values of PROGRAMMEREQUIREMENT.RequirementType ("Required" / "Recommended") come from the BLL, never from this form.
                //           cboRequirementType.DisplayMember = "Description"; cboRequirementType.ValueMember = "RequirementType";
                // TODO BLL: cboSubject.DataSource = ReferenceDataController.GetSubjects();
                //           cboSubject.DisplayMember = "SubjectName"; cboSubject.ValueMember = "SubjectID";
                // TODO BLL: cboAlternative.DataSource = ReferenceDataController.GetAlternativeSubjects();
                //           The BLL puts a "None" entry first (SubjectID = RequirementsNoAlternativeValue, saved as NULL AlternativeSubjectID).
                //           cboAlternative.DisplayMember = "SubjectName"; cboAlternative.ValueMember = "SubjectID";
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

        /// <summary>Add requirement: validates the form and adds it as a new requirement of the selected programme.</summary>
        private void btnRequirementsAdd_Click(object sender, EventArgs e)
        {
            if (lstProgrammes.SelectedIndex < 0)
            {
                ReportStatus("Choose a faculty and a degree programme before you add a requirement.", true);
                return;
            }

            // A new row is always a subject requirement; the minimum average is edited from its own row.
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
                // TODO BLL: ReferenceDataController.SaveRequirement(UserId, null, (int)lstProgrammes.SelectedValue,
                //               (string)cboRequirementType.SelectedValue, (int)cboSubject.SelectedValue, RequirementsSelectedAlternativeId(),
                //               numMinimumMark.Value, chkReqActive.Checked, txtAdvisoryNote.Text.Trim());
                //           SaveRequirement checks Role = Administrator for UserId before it writes (Section 8), validates again,
                //           and refuses a second requirement for the same subject (UNIQUE(ProgrammeID, SubjectID)).
            }
            catch (Exception)
            {
                ReportStatus("The requirement could not be added. Please check the entries and try again.", true);
                return;
            }

            RequirementsLoadRequirements();
            ReportStatus("Requirement added for " + subject + ".", false);
        }

        /// <summary>Update requirement: validates the form and saves it over the requirement selected in the grid.</summary>
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
                if (requirementsEditingMinimumAverage)
                {
                    // TODO BLL: ReferenceDataController.SaveMinimumAverage(UserId, (int)lstProgrammes.SelectedValue, numMinimumMark.Value);
                    //           The "Minimum average" row maps to DegreeProgramme.MinimumOverallAverage (decisions.md §3), not to
                    //           a PROGRAMMEREQUIREMENT row. The BLL checks Role = Administrator before it writes.
                }
                else
                {
                    // TODO BLL: ReferenceDataController.SaveRequirement(UserId, requirementsSelectedRequirementId, (int)lstProgrammes.SelectedValue,
                    //               (string)cboRequirementType.SelectedValue, (int)cboSubject.SelectedValue, RequirementsSelectedAlternativeId(),
                    //               numMinimumMark.Value, chkReqActive.Checked, txtAdvisoryNote.Text.Trim());
                    //           SaveRequirement checks Role = Administrator for UserId before it writes (Section 8).
                }
            }
            catch (Exception)
            {
                ReportStatus("The requirement could not be updated. Please check the entries and try again.", true);
                return;
            }

            RequirementsLoadRequirements();
            ReportStatus("Requirement updated.", false);
        }

        /// <summary>Deactivate: deactivates the selected requirement after confirmation. Nothing is deleted.</summary>
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
                // TODO BLL: ReferenceDataController.DeactivateRequirement(UserId, requirementsSelectedRequirementId.Value);
                //           Sets PROGRAMMEREQUIREMENT.IsActive = 0. The BLL checks Role = Administrator before it writes.
            }
            catch (Exception)
            {
                ReportStatus("The requirement could not be deactivated. Please try again.", true);
                return;
            }

            RequirementsLoadRequirements();
            ReportStatus("Requirement for " + subject + " deactivated.", false);
        }

        /// <summary>Cancel: discards the form's changes and clears the grid selection, ready for a new requirement.</summary>
        private void btnRequirementsCancel_Click(object sender, EventArgs e)
        {
            dgvRequirements.ClearSelection();
            RequirementsClearForm();
            ReportStatus("Changes discarded. The form is ready for a new requirement.", false);
        }

        #endregion

        #region Business Logic Layer hooks

        /// <summary>Lists the selected faculty's programmes, active and inactive.</summary>
        private void RequirementsLoadProgrammes()
        {
            lstProgrammes.DataSource = null;
            lstProgrammes.Items.Clear();
            if (cboReqFaculty.SelectedIndex >= 0)
            {
                try
                {
                    // TODO BLL: lstProgrammes.DataSource = ReferenceDataController.GetProgrammes((int)cboReqFaculty.SelectedValue, includeInactive: true);
                    //           lstProgrammes.DisplayMember = "ProgrammeName"; lstProgrammes.ValueMember = "ProgrammeID";
                    //           Each item carries IsActive, which lstProgrammes_Format reads to add "(inactive)".
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

        /// <summary>Fills the grid with the selected programme's stored requirements.</summary>
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
                // TODO BLL: dgvRequirements.DataSource = ReferenceDataController.GetRequirements((int)lstProgrammes.SelectedValue, includeInactive: true);
                //           Columns: RequirementID, RequirementType, SubjectID, SubjectName, MinimumMark, AlternativeSubjectID,
                //           AlternativeSubjectName, AdvisoryNote, IsActive. The BLL adds one "Minimum average" row
                //           (RequirementID NULL) read from DegreeProgramme.MinimumOverallAverage (decisions.md §3).
            }
            catch (Exception)
            {
                ReportStatus("The requirements for this programme could not be loaded. Please try again.", true);
            }

            dgvRequirements.ClearSelection();
            RequirementsUpdateRequirementCount();
        }

        /// <summary>The "None" entry becomes a NULL AlternativeSubjectID.</summary>
        private int? RequirementsSelectedAlternativeId()
        {
            if (cboAlternative.SelectedValue == null || Equals(cboAlternative.SelectedValue, RequirementsNoAlternativeValue))
            {
                return null;
            }

            return Convert.ToInt32(cboAlternative.SelectedValue, CultureInfo.InvariantCulture);
        }

        #endregion

        #region Scope list and grid

        /// <summary>A faculty was chosen: lists its programmes.</summary>
        private void cboReqFaculty_SelectedIndexChanged(object sender, EventArgs e)
        {
            RequirementsLoadProgrammes();
        }

        /// <summary>A programme was chosen: lists its stored requirements.</summary>
        private void lstProgrammes_SelectedIndexChanged(object sender, EventArgs e)
        {
            RequirementsLoadRequirements();
        }

        /// <summary>Display formatting only: an inactive programme stays listed with "(inactive)" after its name.</summary>
        private void lstProgrammes_Format(object sender, ListControlConvertEventArgs e)
        {
            if (RequirementsIsInactive(e.ListItem))
            {
                e.Value = Convert.ToString(e.Value, CultureInfo.CurrentCulture) + " (inactive)";
            }
        }

        /// <summary>Shows "n programmes in this faculty · m inactive" under the list.</summary>
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

        /// <summary>Shows "n requirements · m active" above the grid.</summary>
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

        /// <summary>Copies the selected row into the form for editing.</summary>
        private void dgvRequirements_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRequirements.SelectedRows.Count == 0)
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

        /// <summary>Display formatting only: status chips and dashes for empty cells.</summary>
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

        /// <summary>Checks every field, shows field messages and the validation summary. True when all are valid.</summary>
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

        /// <summary>Checks that a requirement type is chosen.</summary>
        private bool RequirementsCheckRequirementType()
        {
            return CheckField(cboRequirementType.SelectedIndex >= 0, cboRequirementType, lblRequirementType,
                lblRequirementTypeError, "Choose a requirement type.");
        }

        /// <summary>Checks that a subject is chosen.</summary>
        private bool RequirementsCheckSubject()
        {
            return CheckField(cboSubject.SelectedIndex >= 0, cboSubject, lblSubject, lblSubjectError, "Choose a subject.");
        }

        /// <summary>Checks that an accepted alternative (or None) is chosen.</summary>
        private bool RequirementsCheckAlternative()
        {
            return CheckField(cboAlternative.SelectedIndex >= 0, cboAlternative, lblAlternative, lblAlternativeError,
                "Choose an alternative, or None.");
        }

        /// <summary>Checks that the minimum mark is a number from 0 to 100.</summary>
        private bool RequirementsCheckMinimumMark()
        {
            // Read the typed text before the NumericUpDown clamps it on losing focus.
            decimal mark;
            bool valid = decimal.TryParse(numMinimumMark.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out mark)
                         && mark >= 0m && mark <= 100m;
            return CheckField(valid, numMinimumMark, lblMinimumMark, lblMinimumMarkError, "Enter a mark from 0 to 100.");
        }

        /// <summary>Removes every field message and hides the validation summary.</summary>
        private void RequirementsClearValidation()
        {
            CheckField(true, cboRequirementType, lblRequirementType, lblRequirementTypeError, null);
            CheckField(true, cboSubject, lblSubject, lblSubjectError, null);
            CheckField(true, cboAlternative, lblAlternative, lblAlternativeError, null);
            CheckField(true, numMinimumMark, lblMinimumMark, lblMinimumMarkError, null);
            pnlReqSummary.Visible = false;
        }

        /// <summary>Validates the requirement type on leaving it (not while the Minimum average row is edited).</summary>
        private void cboRequirementType_Leave(object sender, EventArgs e) { if (!requirementsEditingMinimumAverage) RequirementsCheckRequirementType(); }

        /// <summary>Validates the subject on leaving it (not while the Minimum average row is edited).</summary>
        private void cboSubject_Leave(object sender, EventArgs e) { if (!requirementsEditingMinimumAverage) RequirementsCheckSubject(); }

        /// <summary>Validates the accepted alternative on leaving it (not while the Minimum average row is edited).</summary>
        private void cboAlternative_Leave(object sender, EventArgs e) { if (!requirementsEditingMinimumAverage) RequirementsCheckAlternative(); }

        /// <summary>Validates the minimum mark on leaving it.</summary>
        private void numMinimumMark_Leave(object sender, EventArgs e) { RequirementsCheckMinimumMark(); }

        #endregion

        #region Helpers (this screen only)

        /// <summary>Empties the form, ready for a new requirement.</summary>
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

        /// <summary>The minimum-average row only has a mark, so the subject fields are locked while it is edited.</summary>
        private void RequirementsSetSubjectFieldsEnabled(bool enabled)
        {
            cboRequirementType.Enabled = enabled;
            cboSubject.Enabled = enabled;
            cboAlternative.Enabled = enabled;
            chkReqActive.Enabled = enabled;
            txtAdvisoryNote.Enabled = enabled;
        }

        /// <summary>The selected programme's display name, or an empty string when none is selected.</summary>
        private string RequirementsProgrammeName()
        {
            return lstProgrammes.SelectedIndex < 0 ? string.Empty : lstProgrammes.GetItemText(lstProgrammes.SelectedItem);
        }

        /// <summary>True when the grid row's IsActive cell is true.</summary>
        private bool RequirementsRowIsActive(DataGridViewRow row)
        {
            object value = row.Cells[colRequirementsStatus.Index].Value;
            return value is bool && (bool)value;
        }

        /// <summary>Reads IsActive from a bound list item (a BLL object or a DataRowView).</summary>
        private static bool RequirementsIsInactive(object item)
        {
            if (item == null) return false;
            PropertyDescriptor property = TypeDescriptor.GetProperties(item)["IsActive"];
            object value = property == null ? null : property.GetValue(item);
            return value is bool && !(bool)value;
        }

        /// <summary>True for null, DBNull and the empty string.</summary>
        private static bool RequirementsIsEmpty(object value)
        {
            return value == null || value == DBNull.Value || (value is string && ((string)value).Length == 0);
        }

        /// <summary>Converts a cell value to an int, or null when the cell is empty.</summary>
        private static int? RequirementsToNullableInt(object value)
        {
            return RequirementsIsEmpty(value) ? (int?)null : Convert.ToInt32(value, CultureInfo.InvariantCulture);
        }

        /// <summary>Selects a value in a bound ComboBox, or clears it when there is nothing to match.</summary>
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
