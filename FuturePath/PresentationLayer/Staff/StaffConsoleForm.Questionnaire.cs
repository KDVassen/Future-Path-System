using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 13.5 — Reference data, Questionnaire questions (Administrators only; page <c>tabQuestionnaire</c>
    /// in the Reference data group). Built to the UI 13.2 maintenance pattern: a Category scope on the
    /// left (with "Show all categories"), a read-only grid of stored <c>InterestQuestion</c> rows
    /// (Order · Statement · Category · Status chip) with a count line, and a maintenance form below it
    /// (Statement* with an "n / 255" counter, Category*, Display order*, Active). Learners answer every
    /// active question on UI 4 on a 1–5 scale. The tab's own buttons run
    /// <see cref="btnQuestionnaireAdd_Click"/>, <see cref="btnQuestionnaireUpdate_Click"/>,
    /// <see cref="btnQuestionnaireDeactivate_Click"/> and <see cref="btnQuestionnaireCancel_Click"/>.
    /// Nothing is ever deleted.
    /// </summary>
    public partial class StaffConsoleForm
    {
        private const int QuestionnaireStatementMaxLength = 255;

        /// <summary>QuestionID of the row being edited; null while adding a new question.</summary>
        private int? questionnaireSelectedId;

        /// <summary>IsActive of the row being edited, so Deactivate can say it is already inactive.</summary>
        private bool questionnaireSelectedIsActive;

        /// <summary>True while the grid is being rebound, so selection events are ignored.</summary>
        private bool questionnaireLoading;

        #region Page shown

        /// <summary>
        /// Called by <see cref="PageShown"/> each time UI 13.5 appears. The first time, it prepares the grid
        /// and loads the category lists and the questions; afterwards the page keeps its state.
        /// </summary>
        /// <param name="firstTime">True the first time the page is shown in this console.</param>
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

        /// <summary>Add question: adds a new question from the form (validated here and again in the BLL).</summary>
        private void btnQuestionnaireAdd_Click(object sender, EventArgs e)
        {
            if (!QuestionnaireValidateForm())
            {
                return;
            }

            try
            {
                QuestionnaireSaveQuestion(null);
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

        /// <summary>Update question: saves the form over the question selected in the grid.</summary>
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
                QuestionnaireSaveQuestion(questionnaireSelectedId);
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

        /// <summary>Deactivate: deactivates the selected question after confirmation (never deletes it).</summary>
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
            catch (Exception)
            {
                ReportStatus("The question could not be deactivated. Please try again.", true);
                return;
            }

            QuestionnaireLoadQuestions();
            QuestionnaireClearForm();
            ReportStatus("Question deactivated. Changes apply to new advisory sessions only.", false);
        }

        /// <summary>Cancel: discards the edit, clears the form and the grid selection. Nothing is saved.</summary>
        private void btnQuestionnaireCancel_Click(object sender, EventArgs e)
        {
            QuestionnaireClearForm();
            ReportStatus("Edit cancelled. Nothing was saved.", false);
        }

        #endregion

        #region Business Logic Layer hooks

        /// <summary>Binds the scope and form category lists (each to its own list; never literal Items).</summary>
        private void QuestionnaireLoadCategoryLists()
        {
            try
            {
                // TODO BLL: cboScopeCategory.DataSource = InterestController.GetInterestCategories(includeInactive: true);
                //           cboScopeCategory.DisplayMember = "CategoryName"; cboScopeCategory.ValueMember = "CategoryID";
                // TODO BLL: cboQuestionCategory.DataSource = InterestController.GetInterestCategories(includeInactive: false);
                //           cboQuestionCategory.DisplayMember = "CategoryName"; cboQuestionCategory.ValueMember = "CategoryID";
                //           (a separate list for each combo, so they do not share one selection)
                cboScopeCategory.SelectedIndex = -1;
                cboQuestionCategory.SelectedIndex = -1;
            }
            catch (Exception)
            {
                ReportStatus("The interest categories could not be loaded. Please try again, or sign out and back in.", true);
            }
        }

        /// <summary>Binds the question grid for the scope category (or every category), in display order.</summary>
        private void QuestionnaireLoadQuestions()
        {
            questionnaireLoading = true;
            try
            {
                int? scopeCategoryId = cboScopeCategory.SelectedIndex >= 0 && cboScopeCategory.SelectedValue != null
                    ? (int?)Convert.ToInt32(cboScopeCategory.SelectedValue, CultureInfo.InvariantCulture)
                    : null;
                // TODO BLL: dgvQuestions.DataSource = InterestController.GetQuestions(scopeCategoryId, includeInactive: true);
                //           Columns: QuestionID, CategoryID, DisplayOrder, QuestionText, CategoryName, IsActive;
                //           ordered by DisplayOrder. A null scope returns every category.
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

        /// <summary>Adds (questionId null) or updates a question. The BLL validates again.</summary>
        /// <param name="questionId">The question being updated, or null to add a new one.</param>
        private void QuestionnaireSaveQuestion(int? questionId)
        {
            // TODO BLL: InterestController.SaveQuestion(UserId, questionId, txtStatement.Text.Trim(),
            //               (int)cboQuestionCategory.SelectedValue, (int)numDisplayOrder.Value, chkQuestionActive.Checked);
            //           The BLL checks that UserId's Role = Administrator before running and re-checks the
            //           statement (required, max 255), the category (an existing, active one) and the order (1–99).
        }

        /// <summary>Sets IsActive = 0 on a question. Nothing is deleted.</summary>
        /// <param name="questionId">The question to deactivate.</param>
        private void QuestionnaireDeactivateQuestion(int questionId)
        {
            // TODO BLL: InterestController.DeactivateQuestion(UserId, questionId);
            //           The BLL checks that UserId's Role = Administrator before running.
        }

        #endregion

        #region Scope, grid display and selection

        /// <summary>Choosing a scope category lists only its questions.</summary>
        private void cboScopeCategory_SelectionChangeCommitted(object sender, EventArgs e)
        {
            QuestionnaireLoadQuestions();
            QuestionnaireClearForm();
        }

        /// <summary>"Show all categories" clears the scope and lists every question.</summary>
        private void lnkShowAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            cboScopeCategory.SelectedIndex = -1;
            QuestionnaireLoadQuestions();
            QuestionnaireClearForm();
        }

        /// <summary>Writes "N questions · M active" (and the scope) from the bound rows.</summary>
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

        /// <summary>Shows IsActive as a status chip (display formatting only).</summary>
        private void dgvQuestions_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == colQuestionsStatus.Index)
            {
                FormatStatusChip(e, QuestionnaireIsTrue(e.Value));
            }
        }

        /// <summary>
        /// After (re)binding, starts with no row selected, so the form stays ready for a new
        /// question until the administrator picks one, and refreshes the count line.
        /// </summary>
        private void dgvQuestions_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            bool wasLoading = questionnaireLoading;
            questionnaireLoading = true;
            dgvQuestions.ClearSelection();
            dgvQuestions.CurrentCell = null;
            questionnaireLoading = wasLoading;
            QuestionnaireUpdateCountLine();
        }

        /// <summary>
        /// Copies the question the administrator selects into the form. Only a selection made in the
        /// focused grid counts, not the automatic first-row selection on binding.
        /// </summary>
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

        /// <summary>Keeps the "n / 255" counter beside the statement up to date.</summary>
        private void txtStatement_TextChanged(object sender, EventArgs e)
        {
            lblStatementLength.Text = txtStatement.TextLength + " / " + QuestionnaireStatementMaxLength;
        }

        #endregion

        #region Maintenance form validation (validated again in the BLL on save)

        /// <summary>Validates the whole form; shows field messages and the validation summary.</summary>
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

        /// <summary>Statement: required, at most 255 characters.</summary>
        private bool QuestionnaireCheckStatement()
        {
            string text = txtStatement.Text.Trim();
            return CheckField(text.Length > 0 && text.Length <= QuestionnaireStatementMaxLength, txtStatement, lblStatement, lblStatementError,
                "Statement: enter the statement learners rate, up to 255 characters.");
        }

        /// <summary>Category: one must be chosen.</summary>
        private bool QuestionnaireCheckCategory()
        {
            return CheckField(cboQuestionCategory.SelectedIndex >= 0, cboQuestionCategory, lblQuestionCategory, lblQuestionCategoryError,
                "Category: choose the category it scores.");
        }

        /// <summary>Display order: a whole number from 1 to 99, read from the typed text.</summary>
        private bool QuestionnaireCheckDisplayOrder()
        {
            // Read the typed text before the NumericUpDown clamps it on losing focus.
            int order;
            bool valid = int.TryParse(numDisplayOrder.Text.Trim(), NumberStyles.Integer, CultureInfo.CurrentCulture, out order)
                         && order >= numDisplayOrder.Minimum && order <= numDisplayOrder.Maximum;
            return CheckField(valid, numDisplayOrder, lblDisplayOrder, lblDisplayOrderError,
                "Display order: enter a whole number from 1 to 99.");
        }

        /// <summary>Checks the statement as the administrator leaves the box.</summary>
        private void txtStatement_Leave(object sender, EventArgs e) { QuestionnaireCheckStatement(); }

        /// <summary>Checks the category as the administrator leaves the list.</summary>
        private void cboQuestionCategory_Leave(object sender, EventArgs e) { QuestionnaireCheckCategory(); }

        /// <summary>Checks the display order as the administrator leaves the box.</summary>
        private void numDisplayOrder_Leave(object sender, EventArgs e) { QuestionnaireCheckDisplayOrder(); }

        /// <summary>Clears every field message and the validation summary.</summary>
        private void QuestionnaireClearValidation()
        {
            CheckField(true, txtStatement, lblStatement, lblStatementError, string.Empty);
            CheckField(true, cboQuestionCategory, lblQuestionCategory, lblQuestionCategoryError, string.Empty);
            CheckField(true, numDisplayOrder, lblDisplayOrder, lblDisplayOrderError, string.Empty);
            pnlQuestionSummary.Visible = false;
        }

        #endregion

        #region Helpers

        /// <summary>Empties the form and the selection, ready to add a new question.</summary>
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

        /// <summary>True for a bound BIT value of 1.</summary>
        private static bool QuestionnaireIsTrue(object value)
        {
            return value is bool && (bool)value;
        }

        #endregion
    }
}
