
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using FuturePath.ApplicationLayer;

namespace FuturePath.PresentationLayer
{
    // Handles questionnaire maintenance for Administrators
    // Allows questions to be added, updated and deactivated
    public partial class StaffConsoleForm
    {
        private const int QuestionnaireStatementMaxLength = 255;

        // ID of the selected question, or null when adding a new one
        private int? questionnaireSelectedId;

        // Active status of the selected question
        private bool questionnaireSelectedIsActive;

        // Prevent selection events while refreshing the grid
        private bool questionnaireLoading;

        #region Page shown

        // Initialise the question grid and load category lists
        private void QuestionnaireShown(bool firstTime)
        {
            if (!firstTime)
            {
                return;
            }

            dgvQuestions.AutoGenerateColumns = false;
            txtStatement.MaxLength = QuestionnaireStatementMaxLength;
            QuestionnaireLoadCategoryLists();
            QuestionnaireLoadQuestions();
            QuestionnaireClearForm();
        }

        #endregion

        #region Button click events

        // Validate and add a new questionnaire question
        private void btnQuestionnaireAdd_Click(object sender, EventArgs e)
        {
            if (!QuestionnaireValidateForm())
            {
                return;
            }

            try
            {
                QuestionnaireSaveQuestion(0);
            }
            catch (ArgumentException ex)
            {
                ReportStatus(ex.Message, true);
                return;
            }
            catch (Exception)
            {
                ReportStatus("The question could not be added. Please try again.", true);
                return;
            }

            QuestionnaireLoadQuestions();
            QuestionnaireClearForm();
            ReportStatus("Question added. Changes apply to new advisory sessions only.", false);
        }

        // Validate and update the selected question
        private void btnQuestionnaireUpdate_Click(object sender, EventArgs e)
        {
            if (!questionnaireSelectedId.HasValue)
            {
                ReportStatus("Select a question in the grid before you update it.", true);
                return;
            }

            if (!QuestionnaireValidateForm())
            {
                return;
            }

            try
            {
                QuestionnaireSaveQuestion(questionnaireSelectedId.Value);
            }
            catch (ArgumentException ex)
            {
                ReportStatus(ex.Message, true);
                return;
            }
            catch (Exception)
            {
                ReportStatus("The question could not be updated. Please try again.", true);
                return;
            }

            QuestionnaireLoadQuestions();
            QuestionnaireClearForm();
            ReportStatus("Question updated. Changes apply to new advisory sessions only.", false);
        }

        // Deactivate the selected question after confirmation
        private void btnQuestionnaireDeactivate_Click(object sender, EventArgs e)
        {
            if (!questionnaireSelectedId.HasValue)
            {
                ReportStatus("Select a question in the grid before you deactivate it.", true);
                return;
            }

            if (!questionnaireSelectedIsActive)
            {
                ReportStatus("This question is already inactive. Tick Active and update it to use it again.", true);
                return;
            }

            if (!Confirm("Deactivate question",
                "Deactivate question " + numDisplayOrder.Value + "? It is no longer shown on the interest questionnaire " +
                "in new advisory sessions. Answers already given are kept — nothing is deleted.", "Deactivate"))
            {
                return;
            }

            try
            {
                QuestionnaireDeactivateQuestion(questionnaireSelectedId.Value);
            }
            catch (ArgumentException ex)
            {
                ReportStatus(ex.Message, true);
                return;
            }
            catch (Exception)
            {
                ReportStatus("The question could not be deactivated. Please try again.", true);
                return;
            }

            QuestionnaireLoadQuestions();
            QuestionnaireClearForm();
            ReportStatus("Question deactivated. Changes apply to new advisory sessions only.", false);
        }

        // Discard changes and clear the question form
        private void btnQuestionnaireCancel_Click(object sender, EventArgs e)
        {
            QuestionnaireClearForm();
            ReportStatus("Edit cancelled. Nothing was saved.", false);
        }

        #endregion

        #region Business Logic Layer hooks

        // Load category lists for filtering and editing questions
        private void QuestionnaireLoadCategoryLists()
        {
            try
            {
                // Use separate lists so the dropdowns have independent selections
                InterestController interestController = new InterestController();
                cboScopeCategory.DisplayMember = "CategoryName";
                cboScopeCategory.ValueMember = "CategoryID";
                cboScopeCategory.DataSource = interestController.GetInterestCategories(true);
                cboQuestionCategory.DisplayMember = "CategoryName";
                cboQuestionCategory.ValueMember = "CategoryID";
                cboQuestionCategory.DataSource = interestController.GetInterestCategories(false);
                cboScopeCategory.SelectedIndex = -1;
                cboQuestionCategory.SelectedIndex = -1;
            }
            catch (Exception)
            {
                ReportStatus("The interest categories could not be loaded. Please try again, or sign out and back in.", true);
            }
        }

        // Load questionnaire questions for the selected category
        private void QuestionnaireLoadQuestions()
        {
            questionnaireLoading = true;
            try
            {
                // Retrieve questions in display order, optionally filtered by category
                InterestController interestController = new InterestController();
                dgvQuestions.DataSource = interestController.GetQuestions(SelectedId(cboScopeCategory), true);
                dgvQuestions.ClearSelection();
            }
            catch (Exception)
            {
                ReportStatus("The questions could not be loaded. Please try again, or sign out and back in.", true);
            }
            finally
            {
                questionnaireLoading = false;
            }

            QuestionnaireUpdateCountLine();
        }

        // Add or update a question using the controller
        private void QuestionnaireSaveQuestion(int questionId)
        {
            InterestController interestController = new InterestController();
            interestController.SaveQuestion(UserId, questionId, txtStatement.Text.Trim(),
                (int)cboQuestionCategory.SelectedValue, (int)numDisplayOrder.Value, chkQuestionActive.Checked);
        }

        // Deactivate a question without deleting it
        private void QuestionnaireDeactivateQuestion(int questionId)
        {
            InterestController interestController = new InterestController();
            interestController.DeactivateQuestion(UserId, questionId);
        }

        #endregion

        #region Scope, grid display and selection

        // Filter questions by the selected category
        private void cboScopeCategory_SelectionChangeCommitted(object sender, EventArgs e)
        {
            QuestionnaireLoadQuestions();
            QuestionnaireClearForm();
        }

        // Remove the category filter and show all questions
        private void lnkShowAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            cboScopeCategory.SelectedIndex = -1;
            QuestionnaireLoadQuestions();
            QuestionnaireClearForm();
        }

        // Update the total and active question counts
        private void QuestionnaireUpdateCountLine()
        {
            int total = dgvQuestions.Rows.Count;
            int active = 0;
            foreach (DataGridViewRow row in dgvQuestions.Rows)
            {
                if (QuestionnaireIsTrue(row.Cells[colQuestionsStatus.Index].Value))
                {
                    active++;
                }
            }

            string scope = cboScopeCategory.SelectedIndex >= 0 ? " in " + cboScopeCategory.Text : " in all categories";
            lblQuestionCount.Text = total + (total == 1 ? " question" : " questions") + scope + " · " + active + " active";
        }

        // Format questions as active or inactive
        private void dgvQuestions_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == colQuestionsStatus.Index)
            {
                FormatStatusChip(e, QuestionnaireIsTrue(e.Value));
            }
        }

        // Clear automatic row selection after loading the question grid
        private void dgvQuestions_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            bool wasLoading = questionnaireLoading;
            questionnaireLoading = true;
            dgvQuestions.ClearSelection();
            dgvQuestions.CurrentCell = null;
            questionnaireLoading = wasLoading;
            QuestionnaireUpdateCountLine();
        }

        // Load the selected question's details into the form
        private void dgvQuestions_SelectionChanged(object sender, EventArgs e)
        {
            if (questionnaireLoading || !dgvQuestions.Focused || dgvQuestions.SelectedRows.Count == 0)
            {
                return;
            }

            DataGridViewRow row = dgvQuestions.SelectedRows[0];
            object id = row.Cells[colQuestionsId.Index].Value;
            if (id == null || id == DBNull.Value)
            {
                return;
            }

            QuestionnaireClearValidation();
            questionnaireSelectedId = Convert.ToInt32(id, CultureInfo.InvariantCulture);
            questionnaireSelectedIsActive = QuestionnaireIsTrue(row.Cells[colQuestionsStatus.Index].Value);
            txtStatement.Text = Convert.ToString(row.Cells[colQuestionsText.Index].Value, CultureInfo.CurrentCulture);
            chkQuestionActive.Checked = questionnaireSelectedIsActive;

            object order = row.Cells[colQuestionsDisplayOrder.Index].Value;
            if (order != null && order != DBNull.Value)
            {
                decimal value = Convert.ToDecimal(order, CultureInfo.InvariantCulture);
                numDisplayOrder.Value = Math.Max(numDisplayOrder.Minimum, Math.Min(numDisplayOrder.Maximum, value));
            }

            object categoryId = row.Cells[colQuestionsCategoryId.Index].Value;
            if (cboQuestionCategory.DataSource != null && categoryId != null && categoryId != DBNull.Value)
            {
                cboQuestionCategory.SelectedValue = categoryId;
            }
            else
            {
                cboQuestionCategory.SelectedIndex = cboQuestionCategory.FindStringExact(
                    Convert.ToString(row.Cells[colQuestionsCategory.Index].Value, CultureInfo.CurrentCulture));
            }
        }

        // Update the character counter as the statement changes
        private void txtStatement_TextChanged(object sender, EventArgs e)
        {
            lblStatementLength.Text = txtStatement.TextLength + " / " + QuestionnaireStatementMaxLength;
        }

        #endregion

        #region Maintenance form validation (validated again in the BLL on save)

        // Validate the question form and show any errors
        private bool QuestionnaireValidateForm()
        {
            var problems = new List<string>();
            var invalid = new List<Control>();
            if (!QuestionnaireCheckStatement()) { problems.Add("Statement"); invalid.Add(txtStatement); }
            if (!QuestionnaireCheckCategory()) { problems.Add("Category"); invalid.Add(cboQuestionCategory); }
            if (!QuestionnaireCheckDisplayOrder()) { problems.Add("Display order"); invalid.Add(numDisplayOrder); }

            if (problems.Count == 0)
            {
                pnlQuestionSummary.Visible = false;
                return true;
            }

            ShowFormSummary(pnlQuestionSummary, lblQuestionSummary, problems, invalid[0], "the question");
            return false;
        }

        // Check that the statement is provided and within 255 characters
        private bool QuestionnaireCheckStatement()
        {
            string text = txtStatement.Text.Trim();
            return CheckField(text.Length > 0 && text.Length <= QuestionnaireStatementMaxLength, txtStatement, lblStatement, lblStatementError,
                "Statement: enter the statement learners rate, up to 255 characters.");
        }

        // Check that a category has been selected
        private bool QuestionnaireCheckCategory()
        {
            return CheckField(cboQuestionCategory.SelectedIndex >= 0, cboQuestionCategory, lblQuestionCategory, lblQuestionCategoryError,
                "Category: choose the category it scores.");
        }

        // Check that display order is a whole number from 1 to 99
        private bool QuestionnaireCheckDisplayOrder()
        {
            // Validate the typed value before the control adjusts it
            int order;
            bool valid = int.TryParse(numDisplayOrder.Text.Trim(), NumberStyles.Integer, CultureInfo.CurrentCulture, out order)
                         && order >= numDisplayOrder.Minimum && order <= numDisplayOrder.Maximum;
            return CheckField(valid, numDisplayOrder, lblDisplayOrder, lblDisplayOrderError,
                "Display order: enter a whole number from 1 to 99.");
        }

        // Validate the statement when leaving its field
        private void txtStatement_Leave(object sender, EventArgs e) { QuestionnaireCheckStatement(); }

        // Validate the category when leaving its dropdown
        private void cboQuestionCategory_Leave(object sender, EventArgs e) { QuestionnaireCheckCategory(); }

        // Validate display order when leaving its field
        private void numDisplayOrder_Leave(object sender, EventArgs e) { QuestionnaireCheckDisplayOrder(); }

        // Clear validation messages and hide the error summary
        private void QuestionnaireClearValidation()
        {
            CheckField(true, txtStatement, lblStatement, lblStatementError, string.Empty);
            CheckField(true, cboQuestionCategory, lblQuestionCategory, lblQuestionCategoryError, string.Empty);
            CheckField(true, numDisplayOrder, lblDisplayOrder, lblDisplayOrderError, string.Empty);
            pnlQuestionSummary.Visible = false;
        }

        #endregion

        #region Helpers

        // Reset the form for a new questionnaire question
        private void QuestionnaireClearForm()
        {
            questionnaireLoading = true;
            dgvQuestions.ClearSelection();
            questionnaireLoading = false;

            questionnaireSelectedId = null;
            questionnaireSelectedIsActive = false;
            txtStatement.Clear();
            cboQuestionCategory.SelectedIndex = -1;
            numDisplayOrder.Value = numDisplayOrder.Minimum;
            chkQuestionActive.Checked = true;
            QuestionnaireClearValidation();
        }

        // Check whether a stored value is true
        private static bool QuestionnaireIsTrue(object value)
        {
            return value is bool && (bool)value;
        }

        #endregion
    }
}
