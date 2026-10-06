using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 13.2 — Reference Data, Degree requirements tab (spec 2.3.11; hosted by <see cref="ReferenceDataForm"/>).
    /// The administrator scopes the view with a Faculty ComboBox and a Degree programme ListBox
    /// (inactive programmes stay listed, marked "(inactive)"), reads the stored requirements in a
    /// read-only grid with green Active / grey Inactive status chips, and maintains one requirement
    /// at a time in the "Add or edit a requirement" form. The host's action band calls
    /// <see cref="AddNew"/>, <see cref="UpdateSelected"/>, <see cref="DeactivateSelected"/> and
    /// <see cref="CancelEdit"/>. Nothing is ever deleted, and every save is re-checked by the BLL,
    /// which also confirms that the signed-in user is an Administrator.
    /// </summary>
    public partial class DegreeRequirementsTab : UserControl
    {
        // Colours used at runtime (spec 2.2.6). Everything else is set in the Designer.
        private static readonly Color InkColour = Color.FromArgb(27, 31, 36);
        private static readonly Color RedColour = Color.FromArgb(164, 38, 44);
        private static readonly Color RedTint = Color.FromArgb(250, 234, 235);
        private static readonly Color GreenColour = Color.FromArgb(31, 107, 58);
        private static readonly Color GreenTint = Color.FromArgb(230, 242, 234);
        private static readonly Color GreyColour = Color.FromArgb(88, 96, 108);
        private static readonly Color GreyTint = Color.FromArgb(236, 238, 241);

        /// <summary>
        /// SubjectID carried by the BLL's "None" entry in the Accepted alternative list. It is stored
        /// as a NULL <c>AlternativeSubjectID</c>; the value itself is supplied by the BLL, not by this form.
        /// </summary>
        private const int NoAlternativeValue = 0;

        private int? selectedRequirementId;
        private bool editingMinimumAverage;

        /// <summary>Creates the tab. Lists stay empty until the host calls <see cref="LoadData"/>.</summary>
        public DegreeRequirementsTab()
        {
            InitializeComponent();
            dgvRequirements.AutoGenerateColumns = false;
            ClearForm();
            UpdateRequirementCount();
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
            get { return "requirement"; }
        }

        /// <summary>Binds the faculty scope list and the form's lists. Called when the tab is first shown.</summary>
        public void LoadData()
        {
            try
            {
                // TODO BLL: cboFaculty.DataSource = ReferenceDataController.GetFaculties(includeInactive: true);
                //           cboFaculty.DisplayMember = "FacultyName"; cboFaculty.ValueMember = "FacultyID";
                // TODO BLL: cboRequirementType.DataSource = ReferenceDataController.GetRequirementTypes();
                //           Values of PROGRAMMEREQUIREMENT.RequirementType ("Required" / "Recommended") come from the BLL, never from this form.
                //           cboRequirementType.DisplayMember = "Description"; cboRequirementType.ValueMember = "RequirementType";
                // TODO BLL: cboSubject.DataSource = ReferenceDataController.GetSubjects();
                //           cboSubject.DisplayMember = "SubjectName"; cboSubject.ValueMember = "SubjectID";
                // TODO BLL: cboAlternative.DataSource = ReferenceDataController.GetAlternativeSubjects();
                //           The BLL puts a "None" entry first (SubjectID = NoAlternativeValue, saved as NULL AlternativeSubjectID).
                //           cboAlternative.DisplayMember = "SubjectName"; cboAlternative.ValueMember = "SubjectID";
                cboFaculty.SelectedIndex = -1;
                ClearForm();
                Report("Choose a faculty and a degree programme to see its stored requirements.", false);
            }
            catch (Exception)
            {
                Report("The faculty and subject lists could not be loaded. Please close this screen and try again.", true);
            }
        }

        /// <summary>Validates the form and adds it as a new requirement of the selected programme.</summary>
        public void AddNew()
        {
            if (lstProgrammes.SelectedIndex < 0)
            {
                Report("Choose a faculty and a degree programme before you add a requirement.", true);
                return;
            }

            // A new row is always a subject requirement; the minimum average is edited from its own row.
            if (editingMinimumAverage)
            {
                editingMinimumAverage = false;
                SetSubjectFieldsEnabled(true);
            }

            if (!ValidateForm())
            {
                return;
            }

            string subject = cboSubject.Text;
            try
            {
                // TODO BLL: ReferenceDataController.SaveRequirement(UserId, null, (int)lstProgrammes.SelectedValue,
                //               (string)cboRequirementType.SelectedValue, (int)cboSubject.SelectedValue, SelectedAlternativeId(),
                //               numMinimumMark.Value, chkActive.Checked, txtAdvisoryNote.Text.Trim());
                //           SaveRequirement checks Role = Administrator for UserId before it writes (Section 8), validates again,
                //           and refuses a second requirement for the same subject (UNIQUE(ProgrammeID, SubjectID)).
            }
            catch (Exception)
            {
                Report("The requirement could not be added. Please check the entries and try again.", true);
                return;
            }

            LoadRequirements();
            Report("Requirement added for " + subject + ".", false);
        }

        /// <summary>Validates the form and saves it over the requirement selected in the grid.</summary>
        public void UpdateSelected()
        {
            if (dgvRequirements.SelectedRows.Count == 0)
            {
                Report("Select a requirement in the grid first, then choose Update requirement.", true);
                return;
            }

            if (!ValidateForm())
            {
                return;
            }

            try
            {
                if (editingMinimumAverage)
                {
                    // TODO BLL: ReferenceDataController.SaveMinimumAverage(UserId, (int)lstProgrammes.SelectedValue, numMinimumMark.Value);
                    //           The "Minimum average" row maps to DegreeProgramme.MinimumOverallAverage (decisions.md §3), not to
                    //           a PROGRAMMEREQUIREMENT row. The BLL checks Role = Administrator before it writes.
                }
                else
                {
                    // TODO BLL: ReferenceDataController.SaveRequirement(UserId, selectedRequirementId, (int)lstProgrammes.SelectedValue,
                    //               (string)cboRequirementType.SelectedValue, (int)cboSubject.SelectedValue, SelectedAlternativeId(),
                    //               numMinimumMark.Value, chkActive.Checked, txtAdvisoryNote.Text.Trim());
                    //           SaveRequirement checks Role = Administrator for UserId before it writes (Section 8).
                }
            }
            catch (Exception)
            {
                Report("The requirement could not be updated. Please check the entries and try again.", true);
                return;
            }

            LoadRequirements();
            Report("Requirement updated.", false);
        }

        /// <summary>Deactivates the selected requirement after confirmation. Nothing is deleted.</summary>
        public void DeactivateSelected()
        {
            if (dgvRequirements.SelectedRows.Count == 0 || !selectedRequirementId.HasValue)
            {
                Report(editingMinimumAverage
                    ? "The minimum average is part of the programme. Change it here with Update requirement, or on University info — 13.1."
                    : "Select a requirement in the grid first, then choose Deactivate.", true);
                return;
            }

            if (!RowIsActive(dgvRequirements.SelectedRows[0]))
            {
                Report("This requirement is already inactive.", true);
                return;
            }

            string subject = cboSubject.Text;
            using (var dialog = new ConfirmationDialog("Deactivate requirement",
                "Deactivate the " + subject + " requirement for " + ProgrammeName() + "? It stays on record and can be " +
                "made active again. New advisory sessions will no longer check it.", "Deactivate"))
            {
                if (dialog.ShowDialog(FindForm()) != DialogResult.OK)
                {
                    return;
                }
            }

            try
            {
                // TODO BLL: ReferenceDataController.DeactivateRequirement(UserId, selectedRequirementId.Value);
                //           Sets PROGRAMMEREQUIREMENT.IsActive = 0. The BLL checks Role = Administrator before it writes.
            }
            catch (Exception)
            {
                Report("The requirement could not be deactivated. Please try again.", true);
                return;
            }

            LoadRequirements();
            Report("Requirement for " + subject + " deactivated.", false);
        }

        /// <summary>Discards the form's changes and clears the grid selection, ready for a new requirement.</summary>
        public void CancelEdit()
        {
            dgvRequirements.ClearSelection();
            ClearForm();
            Report("Changes discarded. The form is ready for a new requirement.", false);
        }

        #endregion

        #region Business Logic Layer hooks

        /// <summary>Lists the selected faculty's programmes, active and inactive.</summary>
        private void LoadProgrammes()
        {
            lstProgrammes.DataSource = null;
            lstProgrammes.Items.Clear();
            if (cboFaculty.SelectedIndex >= 0)
            {
                try
                {
                    // TODO BLL: lstProgrammes.DataSource = ReferenceDataController.GetProgrammes((int)cboFaculty.SelectedValue, includeInactive: true);
                    //           lstProgrammes.DisplayMember = "ProgrammeName"; lstProgrammes.ValueMember = "ProgrammeID";
                    //           Each item carries IsActive, which lstProgrammes_Format reads to add "(inactive)".
                    lstProgrammes.SelectedIndex = -1;
                }
                catch (Exception)
                {
                    Report("The programmes for this faculty could not be loaded. Please try again.", true);
                }
            }

            UpdateProgrammeCount();
            LoadRequirements();
        }

        /// <summary>Fills the grid with the selected programme's stored requirements.</summary>
        private void LoadRequirements()
        {
            dgvRequirements.DataSource = null;
            ClearForm();
            if (lstProgrammes.SelectedIndex < 0)
            {
                lblGridHeading.Text = "Stored requirements — no programme selected";
                UpdateRequirementCount();
                return;
            }

            lblGridHeading.Text = "Stored requirements — " + ProgrammeName();
            try
            {
                // TODO BLL: dgvRequirements.DataSource = ReferenceDataController.GetRequirements((int)lstProgrammes.SelectedValue, includeInactive: true);
                //           Columns: RequirementID, RequirementType, SubjectID, SubjectName, MinimumMark, AlternativeSubjectID,
                //           AlternativeSubjectName, AdvisoryNote, IsActive. The BLL adds one "Minimum average" row
                //           (RequirementID NULL) read from DegreeProgramme.MinimumOverallAverage (decisions.md §3).
            }
            catch (Exception)
            {
                Report("The requirements for this programme could not be loaded. Please try again.", true);
            }

            dgvRequirements.ClearSelection();
            UpdateRequirementCount();
        }

        /// <summary>The "None" entry becomes a NULL AlternativeSubjectID.</summary>
        private int? SelectedAlternativeId()
        {
            if (cboAlternative.SelectedValue == null || Equals(cboAlternative.SelectedValue, NoAlternativeValue))
            {
                return null;
            }

            return Convert.ToInt32(cboAlternative.SelectedValue, CultureInfo.InvariantCulture);
        }

        #endregion

        #region Scope list and grid

        private void cboFaculty_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadProgrammes();
        }

        private void lstProgrammes_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadRequirements();
        }

        /// <summary>Display formatting only: an inactive programme stays listed with "(inactive)" after its name.</summary>
        private void lstProgrammes_Format(object sender, ListControlConvertEventArgs e)
        {
            if (IsInactive(e.ListItem))
            {
                e.Value = Convert.ToString(e.Value, CultureInfo.CurrentCulture) + " (inactive)";
            }
        }

        /// <summary>Shows "n programmes in this faculty · m inactive" under the list.</summary>
        private void UpdateProgrammeCount()
        {
            if (cboFaculty.SelectedIndex < 0)
            {
                lblProgrammeCount.Text = "Choose a faculty to list its programmes.";
                return;
            }

            int inactive = 0;
            foreach (object item in lstProgrammes.Items)
            {
                if (IsInactive(item)) inactive++;
            }

            int total = lstProgrammes.Items.Count;
            lblProgrammeCount.Text = total + (total == 1 ? " programme" : " programmes") + " in this faculty · " + inactive +
                " inactive. Inactive programmes stay visible here and nowhere in the learner lane.";
        }

        /// <summary>Shows "n requirements · m active" above the grid.</summary>
        private void UpdateRequirementCount()
        {
            int active = 0;
            foreach (DataGridViewRow row in dgvRequirements.Rows)
            {
                if (RowIsActive(row)) active++;
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
            ClearValidation();
            selectedRequirementId = ToNullableInt(row.Cells[colRequirementId.Index].Value);
            editingMinimumAverage = !selectedRequirementId.HasValue;
            SetSubjectFieldsEnabled(!editingMinimumAverage);

            SelectValue(cboRequirementType, row.Cells[colRequirementType.Index].Value);
            SelectValue(cboSubject, row.Cells[colSubjectId.Index].Value);
            object alternative = row.Cells[colAlternativeSubjectId.Index].Value;
            SelectValue(cboAlternative, IsEmpty(alternative) ? (object)NoAlternativeValue : alternative);

            object mark = row.Cells[colMinimumMark.Index].Value;
            decimal value = IsEmpty(mark) ? 0m : Convert.ToDecimal(mark, CultureInfo.InvariantCulture);
            numMinimumMark.Value = Math.Max(numMinimumMark.Minimum, Math.Min(numMinimumMark.Maximum, value));
            chkActive.Checked = RowIsActive(row);
            txtAdvisoryNote.Text = Convert.ToString(row.Cells[colAdvisoryNote.Index].Value, CultureInfo.CurrentCulture);

            Report(editingMinimumAverage
                ? "Editing the programme's minimum average. Set the mark, then choose Update requirement."
                : "Editing the selected requirement. Choose Update requirement to save it, or Cancel.", false);
        }

        /// <summary>Display formatting only: status chips and dashes for empty cells.</summary>
        private void dgvRequirements_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
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
            else if ((e.ColumnIndex == colSubject.Index || e.ColumnIndex == colAlternative.Index
                      || e.ColumnIndex == colMinimumMark.Index) && IsEmpty(e.Value))
            {
                e.Value = "—";
                e.FormattingApplied = true;
            }
            else if (e.ColumnIndex == colMinimumMark.Index && e.Value is decimal)
            {
                e.Value = ((decimal)e.Value).ToString("0.##", CultureInfo.CurrentCulture);
                e.FormattingApplied = true;
            }
        }

        #endregion

        #region Field validation (validated again in the BLL on save)

        /// <summary>Checks every field, shows field messages and the validation summary. True when all are valid.</summary>
        private bool ValidateForm()
        {
            var problems = new List<string>();
            if (!editingMinimumAverage)
            {
                if (!CheckRequirementType()) problems.Add("Requirement type");
                if (!CheckSubject()) problems.Add("Subject");
                if (!CheckAlternative()) problems.Add("Accepted alternative");
            }

            if (!CheckMinimumMark()) problems.Add("Min. mark %");

            if (problems.Count == 0)
            {
                pnlSummary.Visible = false;
                return true;
            }

            lblSummary.Text = "⚠ " + problems.Count + (problems.Count == 1 ? " entry" : " entries") + " to correct: " +
                string.Join(", ", problems) + ".";
            pnlSummary.Visible = true;
            Report(problems.Count + (problems.Count == 1 ? " entry" : " entries") +
                " to correct before the requirement can be saved.", true);
            return false;
        }

        private bool CheckRequirementType()
        {
            return CheckField(cboRequirementType.SelectedIndex >= 0, cboRequirementType, lblRequirementType,
                lblRequirementTypeError, "Choose a requirement type.");
        }

        private bool CheckSubject()
        {
            return CheckField(cboSubject.SelectedIndex >= 0, cboSubject, lblSubject, lblSubjectError, "Choose a subject.");
        }

        private bool CheckAlternative()
        {
            return CheckField(cboAlternative.SelectedIndex >= 0, cboAlternative, lblAlternative, lblAlternativeError,
                "Choose an alternative, or None.");
        }

        private bool CheckMinimumMark()
        {
            // Read the typed text before the NumericUpDown clamps it on losing focus.
            decimal mark;
            bool valid = decimal.TryParse(numMinimumMark.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out mark)
                         && mark >= 0m && mark <= 100m;
            return CheckField(valid, numMinimumMark, lblMinimumMark, lblMinimumMarkError, "Enter a mark from 0 to 100.");
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

        /// <summary>Removes every field message and hides the validation summary.</summary>
        private void ClearValidation()
        {
            CheckField(true, cboRequirementType, lblRequirementType, lblRequirementTypeError, null);
            CheckField(true, cboSubject, lblSubject, lblSubjectError, null);
            CheckField(true, cboAlternative, lblAlternative, lblAlternativeError, null);
            CheckField(true, numMinimumMark, lblMinimumMark, lblMinimumMarkError, null);
            pnlSummary.Visible = false;
        }

        private void cboRequirementType_Leave(object sender, EventArgs e) { if (!editingMinimumAverage) CheckRequirementType(); }

        private void cboSubject_Leave(object sender, EventArgs e) { if (!editingMinimumAverage) CheckSubject(); }

        private void cboAlternative_Leave(object sender, EventArgs e) { if (!editingMinimumAverage) CheckAlternative(); }

        private void numMinimumMark_Leave(object sender, EventArgs e) { CheckMinimumMark(); }

        #endregion

        #region Helpers (this tab only)

        /// <summary>Empties the form, ready for a new requirement.</summary>
        private void ClearForm()
        {
            selectedRequirementId = null;
            editingMinimumAverage = false;
            SetSubjectFieldsEnabled(true);
            cboRequirementType.SelectedIndex = -1;
            cboSubject.SelectedIndex = -1;
            cboAlternative.SelectedIndex = -1;
            numMinimumMark.Value = numMinimumMark.Minimum;
            chkActive.Checked = true;
            txtAdvisoryNote.Text = string.Empty;
            ClearValidation();
        }

        /// <summary>The minimum-average row only has a mark, so the subject fields are locked while it is edited.</summary>
        private void SetSubjectFieldsEnabled(bool enabled)
        {
            cboRequirementType.Enabled = enabled;
            cboSubject.Enabled = enabled;
            cboAlternative.Enabled = enabled;
            chkActive.Enabled = enabled;
            txtAdvisoryNote.Enabled = enabled;
        }

        private string ProgrammeName()
        {
            return lstProgrammes.SelectedIndex < 0 ? string.Empty : lstProgrammes.GetItemText(lstProgrammes.SelectedItem);
        }

        private bool RowIsActive(DataGridViewRow row)
        {
            object value = row.Cells[colStatus.Index].Value;
            return value is bool && (bool)value;
        }

        /// <summary>Reads IsActive from a bound list item (a BLL object or a DataRowView).</summary>
        private static bool IsInactive(object item)
        {
            if (item == null) return false;
            PropertyDescriptor property = TypeDescriptor.GetProperties(item)["IsActive"];
            object value = property == null ? null : property.GetValue(item);
            return value is bool && !(bool)value;
        }

        private static bool IsEmpty(object value)
        {
            return value == null || value == DBNull.Value || (value is string && ((string)value).Length == 0);
        }

        private static int? ToNullableInt(object value)
        {
            return IsEmpty(value) ? (int?)null : Convert.ToInt32(value, CultureInfo.InvariantCulture);
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
