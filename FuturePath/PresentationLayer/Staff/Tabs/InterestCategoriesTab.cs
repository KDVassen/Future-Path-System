using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 13.4 — Reference data, Interest categories tab (Administrators only; hosted by
    /// <c>ReferenceDataForm</c>). Built to the UI 13.2 maintenance pattern: a read-only grid of stored
    /// <c>InterestCategory</c> rows (Category · Questions · Mapped programmes · Status chip) with a count
    /// line, and a maintenance form below it (Category name*, Active). The host's action band calls
    /// <see cref="AddNew"/>, <see cref="UpdateSelected"/>, <see cref="DeactivateSelected"/> and
    /// <see cref="CancelEdit"/>. A second panel maintains the programme ↔ category relevance weights
    /// (<c>DegreeInterestMapping.RelevanceWeight</c>) for the selected category, which the spec needs
    /// for the interest match but gives no screen of its own. Nothing is ever deleted.
    /// </summary>
    public partial class InterestCategoriesTab : UserControl
    {
        // Colours used at runtime (spec 2.2.6). Everything else is set in the Designer.
        private static readonly Color InkColour = Color.FromArgb(27, 31, 36);
        private static readonly Color RedColour = Color.FromArgb(164, 38, 44);
        private static readonly Color RedTint = Color.FromArgb(250, 234, 235);
        private static readonly Color GreenColour = Color.FromArgb(31, 107, 58);
        private static readonly Color GreenTint = Color.FromArgb(230, 242, 234);
        private static readonly Color GreyColour = Color.FromArgb(88, 96, 108);
        private static readonly Color GreyTint = Color.FromArgb(238, 240, 243);
        private static readonly Font ChipFont = new Font("Segoe UI Semibold", 9F);

        private const string ScopePrompt = "Select a category in the grid to see and edit its programme weights.";

        /// <summary>CategoryID of the row being edited; null while adding a new category.</summary>
        private int? selectedCategoryId;

        /// <summary>IsActive of the row being edited, so Deactivate can say it is already inactive.</summary>
        private bool selectedIsActive;

        /// <summary>True while the grids are being rebound, so selection events are ignored.</summary>
        private bool loading;

        /// <summary>Creates the tab. The host sets <see cref="UserId"/> and <see cref="Role"/>, then calls <see cref="LoadData"/>.</summary>
        public InterestCategoriesTab()
        {
            InitializeComponent();
            grdCategories.AutoGenerateColumns = false;
            grdWeights.AutoGenerateColumns = false;
            colWeightValue.DefaultCellStyle.Format = "0.00";
            SetWeightInputsEnabled(false);
            UpdateCountLine();
        }

        #region Tab contract (ReferenceDataForm)

        /// <summary>UserAccount.UserID of the signed-in Administrator; passed to every BLL save.</summary>
        public int UserId { get; set; }

        /// <summary>UserAccount.Role of the signed-in user ("Administrator").</summary>
        public string Role { get; set; }

        /// <summary>Noun used by the host's action band: "Add category", "Update category".</summary>
        public string ItemNoun
        {
            get { return "category"; }
        }

        /// <summary>Raised with a status-band message; isWarning = true shows amber, otherwise blue.</summary>
        public event Action<string, bool> StatusChanged;

        /// <summary>Loads the categories and the programme list. Called by the host when the tab is first shown.</summary>
        public void LoadData()
        {
            LoadProgrammeList();
            LoadCategories();
            ClearForm();
        }

        /// <summary>Adds a new category from the form (validated here and again in the BLL).</summary>
        public void AddNew()
        {
            if (!ValidateForm())
            {
                return;
            }

            string name = txtCategoryName.Text.Trim();
            try
            {
                SaveCategory(null);
            }
            catch (Exception)
            {
                RaiseStatus("The category could not be added. Please try again.", true);
                return;
            }

            LoadCategories();
            ClearForm();
            RaiseStatus("Category \"" + name + "\" added. Changes apply to new advisory sessions only.", false);
        }

        /// <summary>Saves the form over the category selected in the grid.</summary>
        public void UpdateSelected()
        {
            if (!selectedCategoryId.HasValue)
            {
                RaiseStatus("Select a category in the grid before you update it.", true);
                return;
            }

            if (!ValidateForm())
            {
                return;
            }

            string name = txtCategoryName.Text.Trim();
            try
            {
                SaveCategory(selectedCategoryId);
            }
            catch (Exception)
            {
                RaiseStatus("The category could not be updated. Please try again.", true);
                return;
            }

            LoadCategories();
            ClearForm();
            RaiseStatus("Category \"" + name + "\" updated. Changes apply to new advisory sessions only.", false);
        }

        /// <summary>Deactivates the selected category after confirmation (never deletes it).</summary>
        public void DeactivateSelected()
        {
            if (!selectedCategoryId.HasValue)
            {
                RaiseStatus("Select a category in the grid before you deactivate it.", true);
                return;
            }

            if (!selectedIsActive)
            {
                RaiseStatus("This category is already inactive. Tick Active and update it to use it again.", true);
                return;
            }

            string name = txtCategoryName.Text.Trim();
            using (var dialog = new ConfirmationDialog("Deactivate interest category",
                "Deactivate \"" + name + "\"? Its questions are no longer shown to learners and it stops counting in new " +
                "interest scores. Stored answers and past results are kept — nothing is deleted.", "Deactivate"))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }
            }

            try
            {
                DeactivateCategory(selectedCategoryId.Value);
            }
            catch (Exception)
            {
                RaiseStatus("The category could not be deactivated. Please try again.", true);
                return;
            }

            LoadCategories();
            ClearForm();
            RaiseStatus("Category \"" + name + "\" deactivated. Changes apply to new advisory sessions only.", false);
        }

        /// <summary>Discards the edit, clears the form and the grid selection. Nothing is saved.</summary>
        public void CancelEdit()
        {
            ClearForm();
            RaiseStatus("Edit cancelled. Nothing was saved.", false);
        }

        #endregion

        #region Business Logic Layer hooks

        /// <summary>Binds the category grid (active and inactive rows) and refreshes the count line.</summary>
        private void LoadCategories()
        {
            loading = true;
            try
            {
                // TODO BLL: grdCategories.DataSource = InterestController.GetInterestCategories(includeInactive: true);
                //           Columns: CategoryID, CategoryName, QuestionCount (active questions), MappedProgrammeCount, IsActive.
                grdCategories.ClearSelection();
            }
            catch (Exception)
            {
                RaiseStatus("The interest categories could not be loaded. Please close the tab and try again.", true);
            }
            finally
            {
                loading = false;
            }

            UpdateCountLine();
        }

        /// <summary>Binds the programme list used by the relevance-weight panel (never literal Items).</summary>
        private void LoadProgrammeList()
        {
            try
            {
                // TODO BLL: cboWeightProgramme.DataSource = ReferenceDataController.GetDegreeProgrammes(includeInactive: false);
                //           cboWeightProgramme.DisplayMember = "ProgrammeName"; cboWeightProgramme.ValueMember = "ProgrammeID";
                cboWeightProgramme.SelectedIndex = -1;
            }
            catch (Exception)
            {
                RaiseStatus("The degree programme list could not be loaded. Please close the tab and try again.", true);
            }
        }

        /// <summary>Binds the programmes mapped to one category, with their relevance weights.</summary>
        private void LoadWeights(int categoryId)
        {
            loading = true;
            try
            {
                // TODO BLL: grdWeights.DataSource = InterestController.GetRelevanceWeights(categoryId);
                //           Columns: ProgrammeID, ProgrammeName, RelevanceWeight (DegreeInterestMapping, DECIMAL(4,2)).
                grdWeights.ClearSelection();
            }
            catch (Exception)
            {
                RaiseStatus("The programme weights for this category could not be loaded.", true);
            }
            finally
            {
                loading = false;
            }
        }

        /// <summary>Adds (categoryId null) or updates a category. The BLL validates again.</summary>
        private void SaveCategory(int? categoryId)
        {
            // TODO BLL: InterestController.SaveInterestCategory(UserId, categoryId, txtCategoryName.Text.Trim(),
            //               chkCategoryActive.Checked);
            //           The BLL checks that UserId's Role = Administrator before running, re-checks the name
            //           (required, max 60) and rejects a CategoryName already in use.
        }

        /// <summary>Sets IsActive = 0 on a category. Nothing is deleted.</summary>
        private void DeactivateCategory(int categoryId)
        {
            // TODO BLL: InterestController.DeactivateInterestCategory(UserId, categoryId);
            //           The BLL checks that UserId's Role = Administrator before running.
        }

        /// <summary>Adds or updates one DegreeInterestMapping row for the selected category.</summary>
        private void SaveWeight(int categoryId, decimal weight)
        {
            // TODO BLL: InterestController.SaveRelevanceWeight(UserId, (int)cboWeightProgramme.SelectedValue, categoryId, weight);
            //           Inserts or updates DegreeInterestMapping(ProgrammeID, CategoryID, RelevanceWeight).
            //           The BLL checks that UserId's Role = Administrator before running and re-checks 0.00–1.00.
        }

        #endregion

        #region Grid display and selection

        /// <summary>Writes "N categories · M active" from the bound rows.</summary>
        private void UpdateCountLine()
        {
            int total = grdCategories.Rows.Count;
            int active = 0;
            foreach (DataGridViewRow row in grdCategories.Rows)
            {
                if (IsTrue(row.Cells[colCategoryStatus.Index].Value))
                {
                    active++;
                }
            }

            lblCategoryCount.Text = total + (total == 1 ? " category" : " categories") + " · " + active + " active";
        }

        /// <summary>Shows IsActive as a status chip: words plus colour, green Active / grey Inactive.</summary>
        private void grdCategories_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != colCategoryStatus.Index)
            {
                return;
            }

            bool active = IsTrue(e.Value);
            e.Value = active ? "✓ Active" : "Inactive";
            e.CellStyle.ForeColor = active ? GreenColour : GreyColour;
            e.CellStyle.BackColor = active ? GreenTint : GreyTint;
            e.CellStyle.Font = ChipFont;
            e.FormattingApplied = true;
        }

        /// <summary>
        /// After (re)binding, starts with no row selected, so the form stays ready for a new
        /// category until the administrator picks one, and refreshes the count line.
        /// </summary>
        private void grdCategories_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            bool wasLoading = loading;
            loading = true;
            grdCategories.ClearSelection();
            grdCategories.CurrentCell = null;
            loading = wasLoading;
            UpdateCountLine();
        }

        /// <summary>After (re)binding the weights, no mapped programme is pre-selected.</summary>
        private void grdWeights_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            bool wasLoading = loading;
            loading = true;
            grdWeights.ClearSelection();
            grdWeights.CurrentCell = null;
            loading = wasLoading;
        }

        /// <summary>
        /// Copies the category the administrator selects into the form and shows its programme weights.
        /// Only a selection made in the focused grid counts, not the automatic first-row selection on binding.
        /// </summary>
        private void grdCategories_SelectionChanged(object sender, EventArgs e)
        {
            if (loading || !grdCategories.Focused || grdCategories.SelectedRows.Count == 0)
            {
                return;
            }

            DataGridViewRow row = grdCategories.SelectedRows[0];
            object id = row.Cells[colCategoryId.Index].Value;
            if (id == null || id == DBNull.Value)
            {
                return;
            }

            ClearValidation();
            selectedCategoryId = Convert.ToInt32(id, CultureInfo.InvariantCulture);
            selectedIsActive = IsTrue(row.Cells[colCategoryStatus.Index].Value);
            txtCategoryName.Text = Convert.ToString(row.Cells[colCategoryName.Index].Value, CultureInfo.CurrentCulture);
            chkCategoryActive.Checked = selectedIsActive;

            lblWeightsScope.Text = "Programme weights for \"" + txtCategoryName.Text + "\".";
            SetWeightInputsEnabled(true);
            LoadWeights(selectedCategoryId.Value);
        }

        /// <summary>Choosing a mapped programme puts it and its current weight into the weight fields.</summary>
        private void grdWeights_SelectionChanged(object sender, EventArgs e)
        {
            if (loading || !grdWeights.Focused || grdWeights.SelectedRows.Count == 0)
            {
                return;
            }

            DataGridViewRow row = grdWeights.SelectedRows[0];
            object programmeId = row.Cells[colWeightProgrammeId.Index].Value;
            object weight = row.Cells[colWeightValue.Index].Value;
            if (cboWeightProgramme.DataSource != null && programmeId != null && programmeId != DBNull.Value)
            {
                cboWeightProgramme.SelectedValue = programmeId;
            }

            if (weight != null && weight != DBNull.Value)
            {
                decimal value = Convert.ToDecimal(weight, CultureInfo.InvariantCulture);
                numRelevanceWeight.Value = Math.Max(numRelevanceWeight.Minimum, Math.Min(numRelevanceWeight.Maximum, value));
            }

            ClearWeightValidation();
        }

        #endregion

        #region Relevance weight panel

        /// <summary>Save weight → validates the programme and weight, then saves the mapping.</summary>
        private void btnSaveWeight_Click(object sender, EventArgs e)
        {
            if (!selectedCategoryId.HasValue)
            {
                RaiseStatus("Select a category in the grid before you set a programme weight.", true);
                return;
            }

            if (!CheckWeightFields())
            {
                RaiseStatus("Correct the programme weight before you save it.", true);
                return;
            }

            decimal weight;
            TryReadWeight(out weight);
            string programme = cboWeightProgramme.Text;
            try
            {
                SaveWeight(selectedCategoryId.Value, weight);
            }
            catch (Exception)
            {
                RaiseStatus("The programme weight could not be saved. Please try again.", true);
                return;
            }

            LoadWeights(selectedCategoryId.Value);
            LoadCategories();
            RaiseStatus("Weight " + weight.ToString("0.00", CultureInfo.CurrentCulture) + " saved for " + programme +
                ". Changes apply to new advisory sessions only.", false);
        }

        /// <summary>Checks both weight fields; the first problem is shown in the panel's field message.</summary>
        private bool CheckWeightFields()
        {
            decimal weight;
            bool programmeValid = cboWeightProgramme.SelectedIndex >= 0;
            bool weightValid = TryReadWeight(out weight);

            MarkField(programmeValid, cboWeightProgramme, lblWeightProgramme);
            MarkField(weightValid, numRelevanceWeight, lblRelevanceWeight);

            string message = !programmeValid ? "Degree programme: choose the programme this weight applies to."
                : !weightValid ? "Weight: enter a value from 0.00 to 1.00, with up to two decimals." : string.Empty;
            lblWeightError.Text = message.Length == 0 ? string.Empty : "⚠ " + message;
            lblWeightError.Visible = message.Length > 0;
            return programmeValid && weightValid;
        }

        /// <summary>Reads the weight from the raw text, before the NumericUpDown clamps it.</summary>
        private bool TryReadWeight(out decimal weight)
        {
            bool parsed = decimal.TryParse(numRelevanceWeight.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out weight);
            return parsed && weight >= numRelevanceWeight.Minimum && weight <= numRelevanceWeight.Maximum
                   && decimal.Round(weight, 2) == weight;
        }

        private void cboWeightProgramme_Leave(object sender, EventArgs e) { if (selectedCategoryId.HasValue) CheckWeightFields(); }

        private void numRelevanceWeight_Leave(object sender, EventArgs e) { if (selectedCategoryId.HasValue) CheckWeightFields(); }

        private void SetWeightInputsEnabled(bool enabled)
        {
            cboWeightProgramme.Enabled = enabled;
            numRelevanceWeight.Enabled = enabled;
            btnSaveWeight.Enabled = enabled;
        }

        private void ClearWeightValidation()
        {
            MarkField(true, cboWeightProgramme, lblWeightProgramme);
            MarkField(true, numRelevanceWeight, lblRelevanceWeight);
            lblWeightError.Text = string.Empty;
            lblWeightError.Visible = false;
        }

        #endregion

        #region Maintenance form validation (validated again in the BLL on save)

        /// <summary>Validates the whole form; shows field messages and the validation summary.</summary>
        private bool ValidateForm()
        {
            var problems = new List<string>();
            if (!CheckCategoryName())
            {
                problems.Add(lblCategoryNameError.Text.TrimStart('⚠', ' '));
            }

            if (problems.Count == 0)
            {
                pnlCategorySummary.Visible = false;
                return true;
            }

            lblCategorySummary.Text = "⚠ " + problems.Count + (problems.Count == 1 ? " entry" : " entries") + " to correct:" +
                Environment.NewLine + "• " + string.Join(Environment.NewLine + "• ", problems);
            pnlCategorySummary.Visible = true;
            RaiseStatus(problems.Count + (problems.Count == 1 ? " entry" : " entries") + " to correct before the category can be saved.", true);
            txtCategoryName.Focus();
            return false;
        }

        private bool CheckCategoryName()
        {
            string text = txtCategoryName.Text.Trim();
            return CheckField(text.Length > 0 && text.Length <= 60, txtCategoryName, lblCategoryName, lblCategoryNameError,
                "Category name: enter a name of up to 60 characters.");
        }

        private void txtCategoryName_Leave(object sender, EventArgs e) { CheckCategoryName(); }

        /// <summary>
        /// Shows or clears a field message (spec 2.2.7): red text with a warning glyph under the
        /// control, a red tint on the control and a red label.
        /// </summary>
        private static bool CheckField(bool valid, Control input, Label fieldLabel, Label messageLabel, string message)
        {
            MarkField(valid, input, fieldLabel);
            messageLabel.Text = valid ? string.Empty : "⚠ " + message;
            messageLabel.Visible = !valid;
            input.AccessibleDescription = valid ? null : message;
            return valid;
        }

        /// <summary>Red label and red-tint control when invalid; normal colours otherwise.</summary>
        private static void MarkField(bool valid, Control input, Label fieldLabel)
        {
            input.BackColor = valid ? SystemColors.Window : RedTint;
            fieldLabel.ForeColor = valid ? InkColour : RedColour;
        }

        private void ClearValidation()
        {
            CheckField(true, txtCategoryName, lblCategoryName, lblCategoryNameError, string.Empty);
            pnlCategorySummary.Visible = false;
            ClearWeightValidation();
        }

        #endregion

        #region Helpers

        /// <summary>Empties the form, the selection and the weight panel, ready to add a new category.</summary>
        private void ClearForm()
        {
            loading = true;
            grdCategories.ClearSelection();
            grdWeights.DataSource = null;
            loading = false;

            selectedCategoryId = null;
            selectedIsActive = false;
            txtCategoryName.Clear();
            chkCategoryActive.Checked = true;
            cboWeightProgramme.SelectedIndex = -1;
            numRelevanceWeight.Value = numRelevanceWeight.Minimum;
            lblWeightsScope.Text = ScopePrompt;
            SetWeightInputsEnabled(false);
            ClearValidation();
        }

        /// <summary>True for a bound BIT value of 1.</summary>
        private static bool IsTrue(object value)
        {
            return value is bool && (bool)value;
        }

        private void RaiseStatus(string message, bool isWarning)
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
