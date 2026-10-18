
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using FuturePath.ApplicationLayer;

namespace FuturePath.PresentationLayer
{
    // Handles interest category maintenance and programme relevance weights
    // Allows Administrators to add, update and deactivate categories
    public partial class StaffConsoleForm
    {
        private const string CategoriesScopePrompt = "Select a category in the grid to see and edit its programme weights.";

        // ID of the selected category, or null when adding a new one
        private int? categoriesSelectedId;

        // Active status of the selected category
        private bool categoriesSelectedIsActive;

        // Prevent selection events while refreshing the grids
        private bool categoriesLoading;

        #region Page shown

        // Initialise the category and programme weight grids
        private void CategoriesShown(bool firstTime)
        {
            if (!firstTime)
            {
                return;
            }

            dgvCategories.AutoGenerateColumns = false;
            dgvWeights.AutoGenerateColumns = false;
            colWeightsValue.DefaultCellStyle.Format = "0.00";
            CategoriesLoadProgrammeList();
            CategoriesLoadCategories();
            CategoriesClearForm();
        }

        #endregion

        #region Button click events

        // Validate and add a new interest category
        private void btnCategoriesAdd_Click(object sender, EventArgs e)
        {
            if (!CategoriesValidateForm())
            {
                return;
            }

            string name = txtCategoryName.Text.Trim();
            try
            {
                CategoriesSaveCategory(0);
            }
            catch (ArgumentException ex)
            {
                ReportStatus(ex.Message, true);
                return;
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

        // Validate and update the selected interest category
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
                CategoriesSaveCategory(categoriesSelectedId.Value);
            }
            catch (ArgumentException ex)
            {
                ReportStatus(ex.Message, true);
                return;
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

        // Deactivate a category after confirmation without deleting it
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
            catch (ArgumentException ex)
            {
                ReportStatus(ex.Message, true);
                return;
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

        // Cancel editing and clear the form without saving
        private void btnCategoriesCancel_Click(object sender, EventArgs e)
        {
            CategoriesClearForm();
            ReportStatus("Edit cancelled. Nothing was saved.", false);
        }

        #endregion

        #region Business Logic Layer hooks

        // Load all interest categories and refresh their counts
        private void CategoriesLoadCategories()
        {
            categoriesLoading = true;
            try
            {
                // Retrieve category names, question counts, programme counts and status
                InterestController interestController = new InterestController();
                dgvCategories.DataSource = interestController.GetInterestCategories(true);
                dgvCategories.ClearSelection();
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

        // Load active degree programmes for relevance weight selection
        private void CategoriesLoadProgrammeList()
        {
            try
            {
                // Retrieve programmes from all faculties
                ReferenceDataController referenceDataController = new ReferenceDataController();
                cboWeightProgramme.DisplayMember = "ProgrammeName";
                cboWeightProgramme.ValueMember = "ProgrammeID";
                cboWeightProgramme.DataSource = referenceDataController.GetProgrammes(0, false);
                cboWeightProgramme.SelectedIndex = -1;
            }
            catch (Exception)
            {
                ReportStatus("The degree programme list could not be loaded. Please try again, or sign out and back in.", true);
            }
        }

        // Load the selected category's programme relevance weights
        private void CategoriesLoadWeights(int categoryId)
        {
            categoriesLoading = true;
            try
            {
                // Retrieve mapped programmes and their stored relevance weights
                InterestController interestController = new InterestController();
                dgvWeights.DataSource = interestController.GetRelevanceWeights(categoryId);
                dgvWeights.ClearSelection();
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

        // Add or update an interest category through the controller
        private void CategoriesSaveCategory(int categoryId)
        {
            InterestController interestController = new InterestController();
            interestController.SaveInterestCategory(UserId, categoryId, txtCategoryName.Text.Trim(), chkCategoryActive.Checked);
        }

        // Deactivate the selected category without deleting it
        private void CategoriesDeactivateCategory(int categoryId)
        {
            InterestController interestController = new InterestController();
            interestController.DeactivateInterestCategory(UserId, categoryId);
        }

        // Save a programme's relevance weight for the selected category
        private void CategoriesSaveWeight(int categoryId, decimal weight)
        {
            // Add or update the programme-to-category relevance mapping
            // The controller checks administrator access and the weight range
            InterestController interestController = new InterestController();
            interestController.SaveRelevanceWeight(UserId, (int)cboWeightProgramme.SelectedValue, categoryId, weight);
        }

        #endregion

        #region Grid display and selection

        // Update the total and active category counts
        private void CategoriesUpdateCountLine()
        {
            int total = dgvCategories.Rows.Count;
            int active = 0;
            foreach (DataGridViewRow row in dgvCategories.Rows)
            {
                if (CategoriesIsTrue(row.Cells[colCategoriesStatus.Index].Value))
                {
                    active++;
                }
            }

            lblCategoryCount.Text = total + (total == 1 ? " category" : " categories") + " · " + active + " active";
        }

        // Format categories as active or inactive
        private void dgvCategories_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == colCategoriesStatus.Index)
            {
                FormatStatusChip(e, CategoriesIsTrue(e.Value));
            }
        }

        // Clear automatic category selections after loading the grid
        private void dgvCategories_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            bool wasLoading = categoriesLoading;
            categoriesLoading = true;
            dgvCategories.ClearSelection();
            dgvCategories.CurrentCell = null;
            categoriesLoading = wasLoading;
            CategoriesUpdateCountLine();
        }

        // Clear automatic programme selections after loading weights
        private void dgvWeights_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            bool wasLoading = categoriesLoading;
            categoriesLoading = true;
            dgvWeights.ClearSelection();
            dgvWeights.CurrentCell = null;
            categoriesLoading = wasLoading;
        }

        // Load the selected category and its programme weights
        private void dgvCategories_SelectionChanged(object sender, EventArgs e)
        {
            if (categoriesLoading || !dgvCategories.Focused || dgvCategories.SelectedRows.Count == 0)
            {
                return;
            }

            DataGridViewRow row = dgvCategories.SelectedRows[0];
            object id = row.Cells[colCategoriesId.Index].Value;
            if (id == null || id == DBNull.Value)
            {
                return;
            }

            CategoriesClearValidation();
            categoriesSelectedId = Convert.ToInt32(id, CultureInfo.InvariantCulture);
            categoriesSelectedIsActive = CategoriesIsTrue(row.Cells[colCategoriesStatus.Index].Value);
            txtCategoryName.Text = Convert.ToString(row.Cells[colCategoriesName.Index].Value, CultureInfo.CurrentCulture);
            chkCategoryActive.Checked = categoriesSelectedIsActive;

            lblWeightsScope.Text = "Programme weights for \"" + txtCategoryName.Text + "\".";
            CategoriesSetWeightInputsEnabled(true);
            CategoriesLoadWeights(categoriesSelectedId.Value);
        }

        // Display the selected programme and its relevance weight
        private void dgvWeights_SelectionChanged(object sender, EventArgs e)
        {
            if (categoriesLoading || !dgvWeights.Focused || dgvWeights.SelectedRows.Count == 0)
            {
                return;
            }

            DataGridViewRow row = dgvWeights.SelectedRows[0];
            object programmeId = row.Cells[colWeightsProgrammeId.Index].Value;
            object weight = row.Cells[colWeightsValue.Index].Value;
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

        // Validate and save the selected programme's relevance weight
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
            catch (ArgumentException ex)
            {
                ReportStatus(ex.Message, true);
                return;
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

        // Validate the programme selection and relevance weight
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

        // Read and validate the entered weight before automatic adjustment
        private bool CategoriesTryReadWeight(out decimal weight)
        {
            bool parsed = decimal.TryParse(numRelevanceWeight.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out weight);
            return parsed && weight >= numRelevanceWeight.Minimum && weight <= numRelevanceWeight.Maximum
                   && decimal.Round(weight, 2) == weight;
        }

        // Validate programme weight fields when leaving the programme dropdown
        private void cboWeightProgramme_Leave(object sender, EventArgs e) { if (categoriesSelectedId.HasValue) CategoriesCheckWeightFields(); }

        // Validate programme weight fields when leaving the weight input
        private void numRelevanceWeight_Leave(object sender, EventArgs e) { if (categoriesSelectedId.HasValue) CategoriesCheckWeightFields(); }

        // Enable or disable the relevance weight inputs
        private void CategoriesSetWeightInputsEnabled(bool enabled)
        {
            cboWeightProgramme.Enabled = enabled;
            numRelevanceWeight.Enabled = enabled;
            btnSaveWeight.Enabled = enabled;
        }

        // Clear validation messages and highlighting from weight fields
        private void CategoriesClearWeightValidation()
        {
            CategoriesMarkField(true, cboWeightProgramme, lblWeightProgramme);
            CategoriesMarkField(true, numRelevanceWeight, lblRelevanceWeight);
            lblWeightError.Text = string.Empty;
            lblWeightError.Visible = false;
        }

        #endregion

        #region Maintenance form validation (validated again in the BLL on save)

        // Validate the category form and display any errors
        private bool CategoriesValidateForm()
        {
            var problems = new List<string>();
            var invalid = new List<Control>();
            if (!CategoriesCheckName()) { problems.Add("Category name"); invalid.Add(txtCategoryName); }

            if (problems.Count == 0)
            {
                pnlCategorySummary.Visible = false;
                return true;
            }

            ShowFormSummary(pnlCategorySummary, lblCategorySummary, problems, invalid[0], "the category");
            return false;
        }

        // Check that the category name is between 1 and 60 characters
        private bool CategoriesCheckName()
        {
            string text = txtCategoryName.Text.Trim();
            return CheckField(text.Length > 0 && text.Length <= 60, txtCategoryName, lblCategoryName, lblCategoryNameError,
                "Category name: enter a name of up to 60 characters.");
        }

        // Validate the category name when leaving its field
        private void txtCategoryName_Leave(object sender, EventArgs e) { CategoriesCheckName(); }

        // Highlight an invalid field and its label
        private static void CategoriesMarkField(bool valid, Control input, Label fieldLabel)
        {
            input.BackColor = valid ? SystemColors.Window : RedTint;
            fieldLabel.ForeColor = valid ? InkColour : RedColour;
        }

        // Clear the category and weight validation messages
        private void CategoriesClearValidation()
        {
            CheckField(true, txtCategoryName, lblCategoryName, lblCategoryNameError, string.Empty);
            pnlCategorySummary.Visible = false;
            CategoriesClearWeightValidation();
        }

        #endregion

        #region Helpers

        // Reset the category form and relevance weight panel
        private void CategoriesClearForm()
        {
            categoriesLoading = true;
            dgvCategories.ClearSelection();
            dgvWeights.DataSource = null;
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

        // Check whether a stored value is true
        private static bool CategoriesIsTrue(object value)
        {
            return value is bool && (bool)value;
        }

        #endregion
    }
}
