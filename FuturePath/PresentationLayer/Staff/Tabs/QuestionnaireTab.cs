using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 13.5 — Reference data, Questionnaire questions tab (Administrators only; hosted by
    /// <c>ReferenceDataForm</c>). Built to the UI 13.2 maintenance pattern: a Category scope on the left,
    /// a read-only grid of stored <c>InterestQuestion</c> rows (Order · Statement · Category · Status chip)
    /// with a count line, and a maintenance form below it (Statement*, Category*, Display order*, Active).
    /// Learners answer every active question on UI 4 on a 1–5 scale. The host's action band calls
    /// <see cref="AddNew"/>, <see cref="UpdateSelected"/>, <see cref="DeactivateSelected"/> and
    /// <see cref="CancelEdit"/>. Nothing is ever deleted.
    /// </summary>
    public partial class QuestionnaireTab : UserControl
    {
        // Colours used at runtime (spec 2.2.6). Everything else is set in the Designer.
        private static readonly Color InkColour = Color.FromArgb(27, 31, 36);
        private static readonly Color MutedColour = Color.FromArgb(88, 96, 108);
        private static readonly Color RedColour = Color.FromArgb(164, 38, 44);
        private static readonly Color RedTint = Color.FromArgb(250, 234, 235);
        private static readonly Color GreenColour = Color.FromArgb(31, 107, 58);
        private static readonly Color GreenTint = Color.FromArgb(230, 242, 234);
        private static readonly Color GreyTint = Color.FromArgb(238, 240, 243);
        private static readonly Font ChipFont = new Font("Segoe UI Semibold", 9F);

        private const int StatementMaxLength = 255;

        /// <summary>QuestionID of the row being edited; null while adding a new question.</summary>
        private int? selectedQuestionId;

        /// <summary>IsActive of the row being edited, so Deactivate can say it is already inactive.</summary>
        private bool selectedIsActive;

        /// <summary>True while the grid is being rebound, so selection events are ignored.</summary>
        private bool loading;

        /// <summary>Creates the tab. The host sets <see cref="UserId"/> and <see cref="Role"/>, then calls <see cref="LoadData"/>.</summary>
        public QuestionnaireTab()
        {
            InitializeComponent();
            grdQuestions.AutoGenerateColumns = false;
            txtStatement.MaxLength = StatementMaxLength;
            UpdateCountLine();
        }

        #region Tab contract (ReferenceDataForm)

        /// <summary>UserAccount.UserID of the signed-in Administrator; passed to every BLL save.</summary>
        public int UserId { get; set; }

        /// <summary>UserAccount.Role of the signed-in user ("Administrator").</summary>
        public string Role { get; set; }

        /// <summary>Noun used by the host's action band: "Add question", "Update question".</summary>
        public string ItemNoun
        {
            get { return "question"; }
        }

        /// <summary>Raised with a status-band message; isWarning = true shows amber, otherwise blue.</summary>
        public event Action<string, bool> StatusChanged;

        /// <summary>Loads the category lists and the questions. Called by the host when the tab is first shown.</summary>
        public void LoadData()
        {
            LoadCategoryLists();
            LoadQuestions();
            ClearForm();
        }

        /// <summary>Adds a new question from the form (validated here and again in the BLL).</summary>
        public void AddNew()
        {
            if (!ValidateForm())
            {
                return;
            }

            try
            {
                SaveQuestion(null);
            }
            catch (Exception)
            {
                RaiseStatus("The question could not be added. Please try again.", true);
                return;
            }

            LoadQuestions();
            ClearForm();
            RaiseStatus("Question added. Changes apply to new advisory sessions only.", false);
        }

        /// <summary>Saves the form over the question selected in the grid.</summary>
        public void UpdateSelected()
        {
            if (!selectedQuestionId.HasValue)
            {
                RaiseStatus("Select a question in the grid before you update it.", true);
                return;
            }

            if (!ValidateForm())
            {
                return;
            }

            try
            {
                SaveQuestion(selectedQuestionId);
            }
            catch (Exception)
            {
                RaiseStatus("The question could not be updated. Please try again.", true);
                return;
            }

            LoadQuestions();
            ClearForm();
            RaiseStatus("Question updated. Changes apply to new advisory sessions only.", false);
        }

        /// <summary>Deactivates the selected question after confirmation (never deletes it).</summary>
        public void DeactivateSelected()
        {
            if (!selectedQuestionId.HasValue)
            {
                RaiseStatus("Select a question in the grid before you deactivate it.", true);
                return;
            }

            if (!selectedIsActive)
            {
                RaiseStatus("This question is already inactive. Tick Active and update it to use it again.", true);
                return;
            }

            using (var dialog = new ConfirmationDialog("Deactivate question",
                "Deactivate question " + numDisplayOrder.Value + "? It is no longer shown on the interest questionnaire " +
                "in new advisory sessions. Answers already given are kept — nothing is deleted.", "Deactivate"))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }
            }

            try
            {
                DeactivateQuestion(selectedQuestionId.Value);
            }
            catch (Exception)
            {
                RaiseStatus("The question could not be deactivated. Please try again.", true);
                return;
            }

            LoadQuestions();
            ClearForm();
            RaiseStatus("Question deactivated. Changes apply to new advisory sessions only.", false);
        }

        /// <summary>Discards the edit, clears the form and the grid selection. Nothing is saved.</summary>
        public void CancelEdit()
        {
            ClearForm();
            RaiseStatus("Edit cancelled. Nothing was saved.", false);
        }

        #endregion

        #region Business Logic Layer hooks

        /// <summary>Binds the scope and form category lists (each to its own list; never literal Items).</summary>
        private void LoadCategoryLists()
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
                RaiseStatus("The interest categories could not be loaded. Please close the tab and try again.", true);
            }
        }

        /// <summary>Binds the question grid for the scope category (or every category), in display order.</summary>
        private void LoadQuestions()
        {
            loading = true;
            try
            {
                int? scopeCategoryId = cboScopeCategory.SelectedIndex >= 0 && cboScopeCategory.SelectedValue != null
                    ? (int?)Convert.ToInt32(cboScopeCategory.SelectedValue, CultureInfo.InvariantCulture)
                    : null;
                // TODO BLL: grdQuestions.DataSource = InterestController.GetQuestions(scopeCategoryId, includeInactive: true);
                //           Columns: QuestionID, CategoryID, DisplayOrder, QuestionText, CategoryName, IsActive;
                //           ordered by DisplayOrder. A null scope returns every category.
                grdQuestions.ClearSelection();
            }
            catch (Exception)
            {
                RaiseStatus("The questions could not be loaded. Please close the tab and try again.", true);
            }
            finally
            {
                loading = false;
            }

            UpdateCountLine();
        }

        /// <summary>Adds (questionId null) or updates a question. The BLL validates again.</summary>
        private void SaveQuestion(int? questionId)
        {
            // TODO BLL: InterestController.SaveQuestion(UserId, questionId, txtStatement.Text.Trim(),
            //               (int)cboQuestionCategory.SelectedValue, (int)numDisplayOrder.Value, chkQuestionActive.Checked);
            //           The BLL checks that UserId's Role = Administrator before running and re-checks the
            //           statement (required, max 255), the category (an existing, active one) and the order (1–99).
        }

        /// <summary>Sets IsActive = 0 on a question. Nothing is deleted.</summary>
        private void DeactivateQuestion(int questionId)
        {
            // TODO BLL: InterestController.DeactivateQuestion(UserId, questionId);
            //           The BLL checks that UserId's Role = Administrator before running.
        }

        #endregion

        #region Scope, grid display and selection

        /// <summary>Choosing a scope category lists only its questions.</summary>
        private void cboScopeCategory_SelectionChangeCommitted(object sender, EventArgs e)
        {
            LoadQuestions();
            ClearForm();
        }

        /// <summary>"Show all categories" clears the scope and lists every question.</summary>
        private void lnkShowAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            cboScopeCategory.SelectedIndex = -1;
            LoadQuestions();
            ClearForm();
        }

        /// <summary>Writes "N questions · M active" (and the scope) from the bound rows.</summary>
        private void UpdateCountLine()
        {
            int total = grdQuestions.Rows.Count;
            int active = 0;
            foreach (DataGridViewRow row in grdQuestions.Rows)
            {
                if (IsTrue(row.Cells[colQuestionStatus.Index].Value))
                {
                    active++;
                }
            }

            string scope = cboScopeCategory.SelectedIndex >= 0 ? " in " + cboScopeCategory.Text : " in all categories";
            lblQuestionCount.Text = total + (total == 1 ? " question" : " questions") + scope + " · " + active + " active";
        }

        /// <summary>Shows IsActive as a status chip: words plus colour, green Active / grey Inactive.</summary>
        private void grdQuestions_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != colQuestionStatus.Index)
            {
                return;
            }

            bool active = IsTrue(e.Value);
            e.Value = active ? "✓ Active" : "Inactive";
            e.CellStyle.ForeColor = active ? GreenColour : MutedColour;
            e.CellStyle.BackColor = active ? GreenTint : GreyTint;
            e.CellStyle.Font = ChipFont;
            e.FormattingApplied = true;
        }

        /// <summary>
        /// After (re)binding, starts with no row selected, so the form stays ready for a new
        /// question until the administrator picks one, and refreshes the count line.
        /// </summary>
        private void grdQuestions_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            bool wasLoading = loading;
            loading = true;
            grdQuestions.ClearSelection();
            grdQuestions.CurrentCell = null;
            loading = wasLoading;
            UpdateCountLine();
        }

        /// <summary>
        /// Copies the question the administrator selects into the form. Only a selection made in the
        /// focused grid counts, not the automatic first-row selection on binding.
        /// </summary>
        private void grdQuestions_SelectionChanged(object sender, EventArgs e)
        {
            if (loading || !grdQuestions.Focused || grdQuestions.SelectedRows.Count == 0)
            {
                return;
            }

            DataGridViewRow row = grdQuestions.SelectedRows[0];
            object id = row.Cells[colQuestionId.Index].Value;
            if (id == null || id == DBNull.Value)
            {
                return;
            }

            ClearValidation();
            selectedQuestionId = Convert.ToInt32(id, CultureInfo.InvariantCulture);
            selectedIsActive = IsTrue(row.Cells[colQuestionStatus.Index].Value);
            txtStatement.Text = Convert.ToString(row.Cells[colQuestionText.Index].Value, CultureInfo.CurrentCulture);
            chkQuestionActive.Checked = selectedIsActive;

            object order = row.Cells[colDisplayOrder.Index].Value;
            if (order != null && order != DBNull.Value)
            {
                decimal value = Convert.ToDecimal(order, CultureInfo.InvariantCulture);
                numDisplayOrder.Value = Math.Max(numDisplayOrder.Minimum, Math.Min(numDisplayOrder.Maximum, value));
            }

            object categoryId = row.Cells[colQuestionCategoryId.Index].Value;
            if (cboQuestionCategory.DataSource != null && categoryId != null && categoryId != DBNull.Value)
            {
                cboQuestionCategory.SelectedValue = categoryId;
            }
            else
            {
                cboQuestionCategory.SelectedIndex = cboQuestionCategory.FindStringExact(
                    Convert.ToString(row.Cells[colQuestionCategory.Index].Value, CultureInfo.CurrentCulture));
            }
        }

        /// <summary>Keeps the "n / 255" counter beside the statement up to date.</summary>
        private void txtStatement_TextChanged(object sender, EventArgs e)
        {
            lblStatementLength.Text = txtStatement.TextLength + " / " + StatementMaxLength;
        }

        #endregion

        #region Maintenance form validation (validated again in the BLL on save)

        /// <summary>Validates the whole form; shows field messages and the validation summary.</summary>
        private bool ValidateForm()
        {
            var problems = new List<KeyValuePair<Control, string>>();
            if (!CheckStatement()) problems.Add(new KeyValuePair<Control, string>(txtStatement, lblStatementError.Text.TrimStart('⚠', ' ')));
            if (!CheckCategory()) problems.Add(new KeyValuePair<Control, string>(cboQuestionCategory, lblQuestionCategoryError.Text.TrimStart('⚠', ' ')));
            if (!CheckDisplayOrder()) problems.Add(new KeyValuePair<Control, string>(numDisplayOrder, lblDisplayOrderError.Text.TrimStart('⚠', ' ')));

            if (problems.Count == 0)
            {
                pnlQuestionSummary.Visible = false;
                return true;
            }

            var lines = new List<string>();
            foreach (KeyValuePair<Control, string> problem in problems)
            {
                lines.Add("• " + problem.Value);
            }

            lblQuestionSummary.Text = string.Join(Environment.NewLine, lines);
            pnlQuestionSummary.Visible = true;
            RaiseStatus(problems.Count + (problems.Count == 1 ? " entry" : " entries") + " to correct before the question can be saved.", true);
            problems[0].Key.Focus();
            return false;
        }

        private bool CheckStatement()
        {
            string text = txtStatement.Text.Trim();
            return CheckField(text.Length > 0 && text.Length <= StatementMaxLength, txtStatement, lblStatement, lblStatementError,
                "Statement: enter the statement learners rate, up to 255 characters.");
        }

        private bool CheckCategory()
        {
            return CheckField(cboQuestionCategory.SelectedIndex >= 0, cboQuestionCategory, lblQuestionCategory, lblQuestionCategoryError,
                "Category: choose the category it scores.");
        }

        private bool CheckDisplayOrder()
        {
            // Read the typed text before the NumericUpDown clamps it on losing focus.
            int order;
            bool valid = int.TryParse(numDisplayOrder.Text.Trim(), NumberStyles.Integer, CultureInfo.CurrentCulture, out order)
                         && order >= numDisplayOrder.Minimum && order <= numDisplayOrder.Maximum;
            return CheckField(valid, numDisplayOrder, lblDisplayOrder, lblDisplayOrderError,
                "Display order: enter a whole number from 1 to 99.");
        }

        private void txtStatement_Leave(object sender, EventArgs e) { CheckStatement(); }

        private void cboQuestionCategory_Leave(object sender, EventArgs e) { CheckCategory(); }

        private void numDisplayOrder_Leave(object sender, EventArgs e) { CheckDisplayOrder(); }

        /// <summary>
        /// Shows or clears a field message (spec 2.2.7): red text with a warning glyph under the
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

        private void ClearValidation()
        {
            CheckField(true, txtStatement, lblStatement, lblStatementError, string.Empty);
            CheckField(true, cboQuestionCategory, lblQuestionCategory, lblQuestionCategoryError, string.Empty);
            CheckField(true, numDisplayOrder, lblDisplayOrder, lblDisplayOrderError, string.Empty);
            pnlQuestionSummary.Visible = false;
        }

        #endregion

        #region Helpers

        /// <summary>Empties the form and the selection, ready to add a new question.</summary>
        private void ClearForm()
        {
            loading = true;
            grdQuestions.ClearSelection();
            loading = false;

            selectedQuestionId = null;
            selectedIsActive = false;
            txtStatement.Clear();
            cboQuestionCategory.SelectedIndex = -1;
            numDisplayOrder.Value = numDisplayOrder.Minimum;
            chkQuestionActive.Checked = true;
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
