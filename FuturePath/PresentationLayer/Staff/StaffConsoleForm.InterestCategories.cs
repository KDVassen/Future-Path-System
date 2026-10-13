using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 13.4 — Reference data, Interest categories (Administrators only; page <c>tabCategories</c> in the
    /// Reference data group). Built to the UI 13.2 maintenance pattern: a read-only grid of stored
    /// <c>InterestCategory</c> rows (Category · Questions · Mapped programmes · Status chip) with a count
    /// line, and a maintenance form below it (Category name*, Active). The tab's own buttons run
    /// <see cref="btnCategoriesAdd_Click"/>, <see cref="btnCategoriesUpdate_Click"/>,
    /// <see cref="btnCategoriesDeactivate_Click"/> and <see cref="btnCategoriesCancel_Click"/>. A second panel
    /// maintains the programme ↔ category relevance weights (<c>DegreeInterestMapping.RelevanceWeight</c>)
    /// for the selected category, which the spec needs for the interest match but gives no screen of its
    /// own. Nothing is ever deleted.
    /// </summary>
    public partial class StaffConsoleForm
    {
        private const string CategoriesScopePrompt = "Select a category in the grid to see and edit its programme weights.";

        /// <summary>CategoryID of the row being edited; null while adding a new category.</summary>
        private int? categoriesSelectedId;

        /// <summary>IsActive of the row being edited, so Deactivate can say it is already inactive.</summary>
        private bool categoriesSelectedIsActive;

        /// <summary>True while the grids are being rebound, so selection events are ignored.</summary>
        private bool categoriesLoading;

        #region Page shown

        /// <summary>
        /// Called by <see cref="PageShown"/> each time UI 13.4 appears. The first time, it prepares the grids
        /// and loads the categories and the programme list; afterwards the page keeps its state.
        /// </summary>
        /// <param name="firstTime">True the first time the page is shown in this console.</param>
        private void CategoriesShown(bool firstTime)
        {
            if (!firstTime)
            {
                return;
            }

            grdCategories.AutoGenerateColumns = false;
            grdWeights.AutoGenerateColumns = false;
            colWeightValue.DefaultCellStyle.Format = "0.00";
            CategoriesLoadProgrammeList();
            CategoriesLoadCategories();
            CategoriesClearForm();
        }

        #endregion

        #region Button click events

        /// <summary>Add category: adds a new category from the form (validated here and again in the BLL).</summary>
        private void btnCategoriesAdd_Click(object sender, EventArgs e)
        {
            if (!CategoriesValidateForm())
            {
                return;
            }

            string name = txtCategoryName.Text.Trim();
            try
            {
                CategoriesSaveCategory(null);
            }
            catch (Exception)
            {
                ReportStatus("The category could not be added. Please try again.", true);
                return;
            }

            CategoriesLoadCategories();
            CategoriesClearForm();
            ReportStatus("Category \"" + name + "\" added. Changes apply to new advisory sessions only.", false);
        }

        /// <summary>Update category: saves the form over the category selected in the grid.</summary>
        private void btnCategoriesUpdate_Click(object sender, EventArgs e)
        {
            if (!categoriesSelectedId.HasValue)
            {
                ReportStatus("Select a category in the grid before you update it.", true);
                return;
            }

            if (!CategoriesValidateForm())
            {
                return;
            }

            string name = txtCategoryName.Text.Trim();
            try
            {
                CategoriesSaveCategory(categoriesSelectedId);
            }
            catch (Exception)
            {
                ReportStatus("The category could not be updated. Please try again.", true);
                return;
            }

            CategoriesLoadCategories();
            CategoriesClearForm();
            ReportStatus("Category \"" + name + "\" updated. Changes apply to new advisory sessions only.", false);
        }

        /// <summary>Deactivate: deactivates the selected category after confirmation (never deletes it).</summary>
        private void btnCategoriesDeactivate_Click(object sender, EventArgs e)
        {
            if (!categoriesSelectedId.HasValue)
            {
                ReportStatus("Select a category in the grid before you deactivate it.", true);
                return;
            }

            if (!categoriesSelectedIsActive)
            {
                ReportStatus("This category is already inactive. Tick Active and update it to use it again.", true);
                return;
            }

            string name = txtCategoryName.Text.Trim();
            if (!Confirm("Deactivate interest category",
                "Deactivate \"" + name + "\"? Its questions are no longer shown to learners and it stops counting in new " +
                "interest scores. Stored answers and past results are kept — nothing is deleted.", "Deactivate"))
            {
                return;
            }

            try
            {
                CategoriesDeactivateCategory(categoriesSelectedId.Value);
            }
            catch (Exception)
            {
                ReportStatus("The category could not be deactivated. Please try again.", true);
                return;
            }

            CategoriesLoadCategories();
            CategoriesClearForm();
            ReportStatus("Category \"" + name + "\" deactivated. Changes apply to new advisory sessions only.", false);
        }

        /// <summary>Cancel: discards the edit, clears the form and the grid selection. Nothing is saved.</summary>
        private void btnCategoriesCancel_Click(object sender, EventArgs e)
        {
            CategoriesClearForm();
            ReportStatus("Edit cancelled. Nothing was saved.", false);
        }

        #endregion

        #region Business Logic Layer hooks

        /// <summary>Binds the category grid (active and inactive rows) and refreshes the count line.</summary>
        private void CategoriesLoadCategories()
        {
            categoriesLoading = true;
            try
            {
                // TODO BLL: grdCategories.DataSource = InterestController.GetInterestCategories(includeInactive: true);
                //           Columns: CategoryID, CategoryName, QuestionCount (active questions), MappedProgrammeCount, IsActive.
                grdCategories.ClearSelection();
            }
            catch (Exception)
            {
                ReportStatus("The interest categories could not be loaded. Please try again, or sign out and back in.", true);
            }
            finally
            {
                categoriesLoading = false;
            }

            CategoriesUpdateCountLine();
        }

        /// <summary>Binds the programme list used by the relevance-weight panel (never literal Items).</summary>
        private void CategoriesLoadProgrammeList()
        {
            try
            {
                // TODO BLL: cboWeightProgramme.DataSource = ReferenceDataController.GetDegreeProgrammes(includeInactive: false);
                //           cboWeightProgramme.DisplayMember = "ProgrammeName"; cboWeightProgramme.ValueMember = "ProgrammeID";
                cboWeightProgramme.SelectedIndex = -1;
            }
            catch (Exception)
            {
                ReportStatus("The degree programme list could not be loaded. Please try again, or sign out and back in.", true);
            }
        }

        /// <summary>Binds the programmes mapped to one category, with their relevance weights.</summary>
        /// <param name="categoryId">InterestCategory.CategoryID of the selected category.</param>
        private void CategoriesLoadWeights(int categoryId)
        {
            categoriesLoading = true;
            try
            {
                // TODO BLL: grdWeights.DataSource = InterestController.GetRelevanceWeights(categoryId);
                //           Columns: ProgrammeID, ProgrammeName, RelevanceWeight (DegreeInterestMapping, DECIMAL(4,2)).
                grdWeights.ClearSelection();
            }
            catch (Exception)
            {
                ReportStatus("The programme weights for this category could not be loaded.", true);
            }
            finally
            {
                categoriesLoading = false;
            }
        }

        /// <summary>Adds (categoryId null) or updates a category. The BLL validates again.</summary>
        /// <param name="categoryId">The category being updated, or null to add a new one.</param>
        private void CategoriesSaveCategory(int? categoryId)
        {
            // TODO BLL: InterestController.SaveInterestCategory(UserId, categoryId, txtCategoryName.Text.Trim(),
            //               chkCategoryActive.Checked);
            //           The BLL checks that UserId's Role = Administrator before running, re-checks the name
            //           (required, max 60) and rejects a CategoryName already in use.
        }

        /// <summary>Sets IsActive = 0 on a category. Nothing is deleted.</summary>
        /// <param name="categoryId">The category to deactivate.</param>
        private void CategoriesDeactivateCategory(int categoryId)
        {
            // TODO BLL: InterestController.DeactivateInterestCategory(UserId, categoryId);
            //           The BLL checks that UserId's Role = Administrator before running.
        }

        /// <summary>Adds or updates one DegreeInterestMapping row for the selected category.</summary>
        /// <param name="categoryId">The selected category.</param>
        /// <param name="weight">The relevance weight, 0.00–1.00.</param>
        private void CategoriesSaveWeight(int categoryId, decimal weight)
        {
            // TODO BLL: InterestController.SaveRelevanceWeight(UserId, (int)cboWeightProgramme.SelectedValue, categoryId, weight);
            //           Inserts or updates DegreeInterestMapping(ProgrammeID, CategoryID, RelevanceWeight).
            //           The BLL checks that UserId's Role = Administrator before running and re-checks 0.00–1.00.
        }

        #endregion

        #region Grid display and selection

        /// <summary>Writes "N categories · M active" from the bound rows.</summary>
        private void CategoriesUpdateCountLine()
        {
            int total = grdCategories.Rows.Count;
            int active = 0;
            foreach (DataGridViewRow row in grdCategories.Rows)
            {
                if (CategoriesIsTrue(row.Cells[colCategoryStatus.Index].Value))
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

            bool active = CategoriesIsTrue(e.Value);
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
            bool wasLoading = categoriesLoading;
            categoriesLoading = true;
            grdCategories.ClearSelection();
            grdCategories.CurrentCell = null;
            categoriesLoading = wasLoading;
            CategoriesUpdateCountLine();
        }

        /// <summary>After (re)binding the weights, no mapped programme is pre-selected.</summary>
        private void grdWeights_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            bool wasLoading = categoriesLoading;
            categoriesLoading = true;
            grdWeights.ClearSelection();
            grdWeights.CurrentCell = null;
            categoriesLoading = wasLoading;
        }

        /// <summary>
        /// Copies the category the administrator selects into the form and shows its programme weights.
        /// Only a selection made in the focused grid counts, not the automatic first-row selection on binding.
        /// </summary>
        private void grdCategories_SelectionChanged(object sender, EventArgs e)
        {
            if (categoriesLoading || !grdCategories.Focused || grdCategories.SelectedRows.Count == 0)
            {
                return;
            }

            DataGridViewRow row = grdCategories.SelectedRows[0];
            object id = row.Cells[colCategoryId.Index].Value;
            if (id == null || id == DBNull.Value)
            {
                return;
            }

            CategoriesClearValidation();
            categoriesSelectedId = Convert.ToInt32(id, CultureInfo.InvariantCulture);
            categoriesSelectedIsActive = CategoriesIsTrue(row.Cells[colCategoryStatus.Index].Value);
            txtCategoryName.Text = Convert.ToString(row.Cells[colCategoryName.Index].Value, CultureInfo.CurrentCulture);
            chkCategoryActive.Checked = categoriesSelectedIsActive;

            lblWeightsScope.Text = "Programme weights for \"" + txtCategoryName.Text + "\".";
            CategoriesSetWeightInputsEnabled(true);
            CategoriesLoadWeights(categoriesSelectedId.Value);
        }

        /// <summary>Choosing a mapped programme puts it and its current weight into the weight fields.</summary>
        private void grdWeights_SelectionChanged(object sender, EventArgs e)
        {
            if (categoriesLoading || !grdWeights.Focused || grdWeights.SelectedRows.Count == 0)
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

            CategoriesClearWeightValidation();
        }

        #endregion

        #region Relevance weight panel

        /// <summary>Save weight → validates the programme and weight, then saves the mapping.</summary>
        private void btnSaveWeight_Click(object sender, EventArgs e)
        {
            if (!categoriesSelectedId.HasValue)
            {
                ReportStatus("Select a category in the grid before you set a programme weight.", true);
                return;
            }

            if (!CategoriesCheckWeightFields())
            {
                ReportStatus("Correct the programme weight before you save it.", true);
                return;
            }

            decimal weight;
            CategoriesTryReadWeight(out weight);
            string programme = cboWeightProgramme.Text;
            try
            {
                CategoriesSaveWeight(categoriesSelectedId.Value, weight);
            }
            catch (Exception)
            {
                ReportStatus("The programme weight could not be saved. Please try again.", true);
                return;
            }

            CategoriesLoadWeights(categoriesSelectedId.Value);
            CategoriesLoadCategories();
            ReportStatus("Weight " + weight.ToString("0.00", CultureInfo.CurrentCulture) + " saved for " + programme +
                ". Changes apply to new advisory sessions only.", false);
        }

        /// <summary>Checks both weight fields; the first problem is shown in the panel's field message.</summary>
        private bool CategoriesCheckWeightFields()
        {
            decimal weight;
            bool programmeValid = cboWeightProgramme.SelectedIndex >= 0;
            bool weightValid = CategoriesTryReadWeight(out weight);

            CategoriesMarkField(programmeValid, cboWeightProgramme, lblWeightProgramme);
            CategoriesMarkField(weightValid, numRelevanceWeight, lblRelevanceWeight);

            string message = !programmeValid ? "Degree programme: choose the programme this weight applies to."
                : !weightValid ? "Weight: enter a value from 0.00 to 1.00, with up to two decimals." : string.Empty;
            lblWeightError.Text = message.Length == 0 ? string.Empty : "⚠ " + message;
            lblWeightError.Visible = message.Length > 0;
            return programmeValid && weightValid;
        }

        /// <summary>Reads the weight from the raw text, before the NumericUpDown clamps it.</summary>
        private bool CategoriesTryReadWeight(out decimal weight)
        {
            bool parsed = decimal.TryParse(numRelevanceWeight.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out weight);
            return parsed && weight >= numRelevanceWeight.Minimum && weight <= numRelevanceWeight.Maximum
                   && decimal.Round(weight, 2) == weight;
        }

        /// <summary>Re-checks the weight fields when the programme list loses focus (only once a category is chosen).</summary>
        private void cboWeightProgramme_Leave(object sender, EventArgs e) { if (categoriesSelectedId.HasValue) CategoriesCheckWeightFields(); }

        /// <summary>Re-checks the weight fields when the weight box loses focus (only once a category is chosen).</summary>
        private void numRelevanceWeight_Leave(object sender, EventArgs e) { if (categoriesSelectedId.HasValue) CategoriesCheckWeightFields(); }

        /// <summary>Turns the weight inputs and Save weight on (a category is selected) or off.</summary>
        private void CategoriesSetWeightInputsEnabled(bool enabled)
        {
            cboWeightProgramme.Enabled = enabled;
            numRelevanceWeight.Enabled = enabled;
            btnSaveWeight.Enabled = enabled;
        }

        /// <summary>Clears the weight panel's field marks and message.</summary>
        private void CategoriesClearWeightValidation()
        {
            CategoriesMarkField(true, cboWeightProgramme, lblWeightProgramme);
            CategoriesMarkField(true, numRelevanceWeight, lblRelevanceWeight);
            lblWeightError.Text = string.Empty;
            lblWeightError.Visible = false;
        }

        #endregion

        #region Maintenance form validation (validated again in the BLL on save)

        /// <summary>Validates the whole form; shows field messages and the validation summary.</summary>
        private bool CategoriesValidateForm()
        {
            var problems = new List<string>();
            if (!CategoriesCheckName())
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
            ReportStatus(problems.Count + (problems.Count == 1 ? " entry" : " entries") + " to correct before the category can be saved.", true);
            txtCategoryName.Focus();
            return false;
        }

        /// <summary>Category name: required, at most 60 characters.</summary>
        private bool CategoriesCheckName()
        {
            string text = txtCategoryName.Text.Trim();
            return CheckField(text.Length > 0 && text.Length <= 60, txtCategoryName, lblCategoryName, lblCategoryNameError,
                "Category name: enter a name of up to 60 characters.");
        }

        /// <summary>Checks the category name as the administrator leaves the box.</summary>
        private void txtCategoryName_Leave(object sender, EventArgs e) { CategoriesCheckName(); }

        /// <summary>Red label and red-tint control when invalid; normal colours otherwise (no field message).</summary>
        private static void CategoriesMarkField(bool valid, Control input, Label fieldLabel)
        {
            input.BackColor = valid ? SystemColors.Window : RedTint;
            fieldLabel.ForeColor = valid ? InkColour : RedColour;
        }

        /// <summary>Clears every field message, the validation summary and the weight panel's marks.</summary>
        private void CategoriesClearValidation()
        {
            CheckField(true, txtCategoryName, lblCategoryName, lblCategoryNameError, string.Empty);
            pnlCategorySummary.Visible = false;
            CategoriesClearWeightValidation();
        }

        #endregion

        #region Helpers

        /// <summary>Empties the form, the selection and the weight panel, ready to add a new category.</summary>
        private void CategoriesClearForm()
        {
            categoriesLoading = true;
            grdCategories.ClearSelection();
            grdWeights.DataSource = null;
            categoriesLoading = false;

            categoriesSelectedId = null;
            categoriesSelectedIsActive = false;
            txtCategoryName.Clear();
            chkCategoryActive.Checked = true;
            cboWeightProgramme.SelectedIndex = -1;
            numRelevanceWeight.Value = numRelevanceWeight.Minimum;
            lblWeightsScope.Text = CategoriesScopePrompt;
            CategoriesSetWeightInputsEnabled(false);
            CategoriesClearValidation();
        }

        /// <summary>True for a bound BIT value of 1.</summary>
        private static bool CategoriesIsTrue(object value)
        {
            return value is bool && (bool)value;
        }

        #endregion
    }
}
