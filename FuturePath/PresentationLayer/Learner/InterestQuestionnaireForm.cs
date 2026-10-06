using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 4 — Interest Questionnaire (wide, step 3 of 6; spec 2.3.4).
    /// One five-point RadioButton group per active <c>InterestQuestion</c>, in <c>DisplayOrder</c>, with the
    /// <c>InterestCategory</c> name under each statement. The rows are built at runtime from the bound
    /// question list, each in its own panel so its five radio buttons form one group. Nothing is typed,
    /// so nothing can be invalid: unanswered statements are shaded amber and counted in the amber notice,
    /// and <b>Save and continue</b> stays disabled until every statement is answered.
    /// Red never appears on this screen and there is no validation dialog.
    /// </summary>
    public partial class InterestQuestionnaireForm : Form
    {
        // Colours used at runtime (spec 2.2.6). Red is deliberately absent from this screen.
        private static readonly Color InkColour = Color.FromArgb(27, 31, 36);
        private static readonly Color MutedColour = Color.FromArgb(88, 96, 108);
        private static readonly Color BlueColour = Color.FromArgb(31, 95, 168);
        private static readonly Color BlueTint = Color.FromArgb(232, 238, 246);
        private static readonly Color AmberColour = Color.FromArgb(200, 137, 43);
        private static readonly Color AmberText = Color.FromArgb(138, 93, 24);
        private static readonly Color AmberTint = Color.FromArgb(252, 243, 224);
        private static readonly Color GreenColour = Color.FromArgb(31, 107, 58);
        private static readonly Color GreenTint = Color.FromArgb(230, 242, 234);
        private static readonly Color DeepNavy = Color.FromArgb(11, 31, 58);

        // Fonts and sizes of the statement rows built at runtime (spec 2.2.5: body 10 pt, caption 8 pt).
        private static readonly Font StatementFont = new Font("Segoe UI", 10F);
        private static readonly Font CategoryFont = new Font("Segoe UI", 8F);
        private const int RowHeight = 36;
        private const int RowWidth = 881;            // statement panel client width less a vertical scroll bar
        private const int StatementWidth = 470;
        private const int FirstResponseColumn = 493; // matches the header labels in the Designer
        private const int ResponseColumnWidth = 78;

        /// <summary>Captions of the 1–5 scale, used for the radio buttons' accessible names.</summary>
        private static readonly string[] ResponseCaptions = { "Strongly disagree", "Disagree", "Neutral", "Agree", "Strongly agree" };

        private const string StepStatus = "Step 3 of 6 · Choose one response for every statement.";

        private readonly Form homeForm;
        private readonly Form previousForm;
        private FacultySelectionForm facultyForm;

        /// <summary>One entry per statement row, in display order.</summary>
        private readonly List<QuestionRow> questionRows = new List<QuestionRow>();

        /// <summary>Creates the interest questionnaire screen. Only stores its arguments.</summary>
        /// <param name="homeForm">UI 1, shown again when the session is left.</param>
        /// <param name="previousForm">UI 3, shown again by Back.</param>
        public InterestQuestionnaireForm(Form homeForm, Form previousForm)
        {
            InitializeComponent();
            this.homeForm = homeForm;
            this.previousForm = previousForm;

            SetStatus(StepStatus, DeepNavy, Color.White);
            LoadQuestions();
            RefreshProgress();
        }

        #region Session values handed to the next screens

        /// <summary>AdvisorySession.SessionID, set by UI 3 before this screen is shown.</summary>
        public int? SessionId { get; set; }

        /// <summary>Learner.LearnerID, set by UI 3 before this screen is shown.</summary>
        public int? LearnerId { get; set; }

        /// <summary>The learner context strip text (name · grade · school · province · session).</summary>
        public string LearnerContext { get; set; }

        #endregion

        #region Statement rows

        /// <summary>The controls and answer of one statement row.</summary>
        private sealed class QuestionRow
        {
            /// <summary>InterestQuestion.QuestionID.</summary>
            public int QuestionId { get; set; }

            /// <summary>InterestCategory.CategoryName shown under the statement.</summary>
            public string CategoryName { get; set; }

            /// <summary>Position in display order, from 0 (used for banding).</summary>
            public int Index { get; set; }

            /// <summary>The row container; it groups the five radio buttons.</summary>
            public Panel Container { get; set; }

            /// <summary>The caption under the statement (category, and "Not yet answered").</summary>
            public Label CategoryLabel { get; set; }

            /// <summary>The chosen response 1–5, or null while unanswered.</summary>
            public int? Response { get; set; }
        }

        /// <summary>
        /// Reads a named member from a bound list item, whether the source is a DataTable
        /// (DataRowView) or a list of entity objects.
        /// </summary>
        private static object ReadMember(object item, string member)
        {
            var rowView = item as DataRowView;
            if (rowView != null)
            {
                return rowView.Row.Table.Columns.Contains(member) ? rowView[member] : null;
            }

            var property = TypeDescriptor.GetProperties(item).Find(member, true);
            return property == null ? null : property.GetValue(item);
        }

        /// <summary>Reads a member as text; an empty string when it is missing or null.</summary>
        private static string ReadText(object item, string member)
        {
            object value = ReadMember(item, member);
            return value == null || value == DBNull.Value ? string.Empty : value.ToString();
        }

        /// <summary>
        /// Builds one statement row per item of the question list, in the order given (the BLL sorts by
        /// DisplayOrder). Each item must expose QuestionID, QuestionText and CategoryName; a DataTable,
        /// DataView or a list of entity objects all work.
        /// </summary>
        /// <param name="source">The active questions, already ordered.</param>
        private void BuildQuestionRows(object source)
        {
            var table = source as DataTable;
            IEnumerable items = table != null ? table.DefaultView : source as IEnumerable;

            pnlStatements.SuspendLayout();
            pnlStatements.Controls.Clear();
            questionRows.Clear();
            if (items != null)
            {
                foreach (object item in items)
                {
                    var row = new QuestionRow
                    {
                        QuestionId = Convert.ToInt32(ReadMember(item, "QuestionID"), CultureInfo.InvariantCulture),
                        CategoryName = ReadText(item, "CategoryName"),
                        Index = questionRows.Count
                    };
                    row.Container = BuildRowPanel(row, ReadText(item, "QuestionText"));
                    questionRows.Add(row);
                    pnlStatements.Controls.Add(row.Container);
                }
            }

            // Rows fill the panel when they all fit; otherwise leave room for the vertical scroll bar.
            if (questionRows.Count * RowHeight <= pnlStatements.ClientSize.Height)
            {
                foreach (QuestionRow row in questionRows)
                {
                    row.Container.Width = pnlStatements.ClientSize.Width;
                }
            }

            pnlStatements.ResumeLayout(true);
            RefreshProgress();
        }

        /// <summary>
        /// Creates the container for one statement: the statement, its category caption and five
        /// unlabelled radio buttons aligned under the Strongly disagree … Strongly agree headers.
        /// </summary>
        private Panel BuildRowPanel(QuestionRow row, string statement)
        {
            int number = row.Index + 1;
            var panel = new Panel
            {
                Name = "pnlQuestion" + number,
                Location = new Point(0, row.Index * RowHeight),
                Size = new Size(RowWidth, RowHeight),
                AccessibleName = "Statement " + number,
                Tag = row
            };

            var statementLabel = new Label
            {
                Name = "lblQuestion" + number,
                Text = statement,
                Font = StatementFont,
                ForeColor = InkColour,
                AutoEllipsis = true,
                Location = new Point(12, 1),
                Size = new Size(StatementWidth, 19)
            };

            row.CategoryLabel = new Label
            {
                Name = "lblQuestionCategory" + number,
                Font = CategoryFont,
                ForeColor = MutedColour,
                Location = new Point(12, 19),
                Size = new Size(StatementWidth, 16)
            };

            panel.Controls.Add(statementLabel);
            panel.Controls.Add(row.CategoryLabel);
            for (int value = 1; value <= 5; value++)
            {
                var option = new RadioButton
                {
                    Name = "rdoQuestion" + number + "Response" + value,
                    Tag = value,
                    AccessibleName = "Statement " + number + ": " + ResponseCaptions[value - 1],
                    Location = new Point(FirstResponseColumn + (value - 1) * ResponseColumnWidth + (ResponseColumnWidth - 16) / 2, 9),
                    Size = new Size(16, 18),
                    UseVisualStyleBackColor = true
                };
                option.CheckedChanged += Response_CheckedChanged;
                panel.Controls.Add(option);
            }

            return panel;
        }

        /// <summary>Records the chosen response for the row and refreshes the counts, shading and tiles.</summary>
        private void Response_CheckedChanged(object sender, EventArgs e)
        {
            var option = (RadioButton)sender;
            if (!option.Checked)
            {
                return;
            }

            var row = (QuestionRow)option.Parent.Tag;
            row.Response = (int)option.Tag;
            RefreshProgress();
        }

        #endregion

        #region Business Logic Layer hooks

        /// <summary>Loads the active questions (with category names) in DisplayOrder and builds the rows.</summary>
        private void LoadQuestions()
        {
            try
            {
                // TODO BLL: BuildQuestionRows(InterestController.GetActiveQuestions());
                //           Active InterestQuestion rows joined to InterestCategory, ordered by DisplayOrder:
                //           QuestionID, QuestionText, CategoryName.
                BuildQuestionRows(null);
            }
            catch (Exception)
            {
                SetStatus("The questionnaire could not be loaded. Please go back and try again.", AmberColour, InkColour);
            }
        }

        /// <summary>The responses given so far, QuestionID → ResponseValue (1–5).</summary>
        private Dictionary<int, int> Responses()
        {
            return questionRows.Where(r => r.Response.HasValue).ToDictionary(r => r.QuestionId, r => r.Response.Value);
        }

        /// <summary>
        /// Asks the BLL for the category scores of the answers so far and shows the three strongest
        /// in the tiles. Tiles with no score are hidden, never shown empty.
        /// </summary>
        private void RefreshCategoryScores()
        {
            var scores = new List<KeyValuePair<string, double>>();
            try
            {
                // TODO BLL: scores = InterestController.CalculateCategoryScores(SessionId, Responses());
                //           (Learner.calculateInterestScore per InterestCategory; highest first, 0–100.)
            }
            catch (Exception)
            {
                scores.Clear();
                SetStatus("Your category scores could not be worked out yet. Your answers are kept.", AmberColour, InkColour);
            }

            Panel[] tiles = { tileCategory1, tileCategory2, tileCategory3 };
            Label[] captions = { tileCategory1Caption, tileCategory2Caption, tileCategory3Caption };
            Label[] values = { tileCategory1Value, tileCategory2Value, tileCategory3Value };
            for (int i = 0; i < tiles.Length; i++)
            {
                bool show = i < scores.Count;
                if (show)
                {
                    captions[i].Text = scores[i].Key;
                    values[i].Text = scores[i].Value.ToString("0", CultureInfo.CurrentCulture);
                }

                tiles[i].Visible = show;
            }
        }

        /// <summary>
        /// Writes one <c>LearnerInterestResponse</c> per statement (ResponseValue 1–5,
        /// UNIQUE(SessionID, QuestionID)). Only Save and continue writes; Cancel and Back do not.
        /// </summary>
        private void SaveResponses()
        {
            // TODO BLL: InterestController.SaveResponses(SessionId.Value, Responses());
        }

        #endregion

        #region Progress, notice and status

        /// <summary>
        /// Shades each row (amber while unanswered, banded once answered), updates the Answered tile,
        /// the "n of N answered" caption, the notice, the category tiles, and enables Save and continue
        /// only when every statement is answered.
        /// </summary>
        private void RefreshProgress()
        {
            int total = questionRows.Count;
            int answered = questionRows.Count(r => r.Response.HasValue);
            int outstanding = total - answered;

            foreach (QuestionRow row in questionRows)
            {
                bool done = row.Response.HasValue;
                row.Container.BackColor = done ? (row.Index % 2 == 1 ? BlueTint : Color.White) : AmberTint;
                row.CategoryLabel.ForeColor = done ? MutedColour : AmberText;
                row.CategoryLabel.Text = done ? row.CategoryName : "Not yet answered  ·  " + row.CategoryName;
            }

            lblAnsweredCount.Text = answered + " of " + total + " answered";
            tileAnsweredValue.Text = answered + " / " + total;
            bool complete = total > 0 && outstanding == 0;
            tileAnswered.BackColor = complete ? GreenTint : AmberTint;
            tileAnsweredValue.ForeColor = complete ? GreenColour : AmberText;
            tileAnsweredNote.Text = total == 0 ? "no statements loaded" : complete ? "all answered" : outstanding + " outstanding";

            if (total == 0)
            {
                SetNotice("The questionnaire statements are read from stored data and appear here once they are loaded.",
                    BlueTint, BlueColour, InkColour);
            }
            else if (complete)
            {
                SetNotice("✓ All " + total + " statements answered. Save and continue to choose a faculty and programme.",
                    GreenTint, GreenColour, GreenColour);
            }
            else
            {
                SetNotice(outstanding + (outstanding == 1 ? " statement" : " statements") + " still to answer. The interest " +
                          "alignment score is calculated only once all " + total + " are answered; the outstanding statements " +
                          "are shaded below.", AmberTint, AmberColour, AmberText);
            }

            btnSaveContinue.Enabled = complete;
            RefreshCategoryScores();
        }

        private void SetNotice(string text, Color back, Color bar, Color fore)
        {
            pnlNotice.BackColor = back;
            pnlNoticeBar.BackColor = bar;
            lblNotice.ForeColor = fore;
            lblNotice.Text = text;
        }

        private void SetStatus(string message, Color back, Color fore)
        {
            pnlStatus.BackColor = back;
            lblStatus.ForeColor = fore;
            lblStatus.Text = message;
        }

        #endregion

        #region Buttons and navigation

        /// <summary>Save and continue (enabled only when every statement is answered) → UI 5.</summary>
        private void btnSaveContinue_Click(object sender, EventArgs e)
        {
            if (questionRows.Count == 0 || questionRows.Any(r => !r.Response.HasValue))
            {
                return;
            }

            try
            {
                SaveResponses();
            }
            catch (Exception)
            {
                SetStatus("Your answers could not be saved. Please try again.", AmberColour, InkColour);
                return;
            }

            if (facultyForm == null || facultyForm.IsDisposed)
            {
                facultyForm = new FacultySelectionForm(homeForm, this);
            }

            facultyForm.SessionId = SessionId;
            facultyForm.LearnerId = LearnerId;
            facultyForm.LearnerContext = LearnerContext;
            facultyForm.Show();
            Hide();
        }

        /// <summary>Back → UI 3. Answers stay in this (hidden) form.</summary>
        private void btnBack_Click(object sender, EventArgs e)
        {
            previousForm.Show();
            Hide();
        }

        /// <summary>Cancel abandons the session after confirmation.</summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            LeaveSession();
        }

        /// <summary>Home on the rail leaves the session after confirmation.</summary>
        private void btnNavHome_Click(object sender, EventArgs e)
        {
            LeaveSession();
        }

        /// <summary>
        /// Asks for confirmation, returns to UI 1 and closes every learner screen. Anything already
        /// saved stays on record as an "In progress" session (decisions.md D3).
        /// </summary>
        private void LeaveSession()
        {
            using (var dialog = new ConfirmationDialog("Leave this advisory session",
                "Return to the homepage? Answers not yet saved are discarded. Anything already saved stays on record as an " +
                "\"In progress\" session.", "Leave session"))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }
            }

            homeForm.Show();
            foreach (Form screen in Application.OpenForms.Cast<Form>().ToList())
            {
                if (screen != homeForm)
                {
                    screen.Close();
                }
            }
        }

        #endregion

        #region Showing and closing

        /// <summary>Shows the learner context strip each time the screen is shown.</summary>
        private void InterestQuestionnaireForm_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                lblContext.Text = LearnerContext ?? string.Empty;
                SetStatus(StepStatus, DeepNavy, Color.White);
            }
        }

        /// <summary>The caption-bar X asks first, because it closes Future Path.</summary>
        private void InterestQuestionnaireForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing || homeForm.Visible)
            {
                return;
            }

            using (var dialog = new ConfirmationDialog("Close Future Path",
                "Close Future Path? Anything already saved stays on record as an \"In progress\" session.", "Close"))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>Ends the application when this screen was closed with the caption-bar X.</summary>
        private void InterestQuestionnaireForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!homeForm.Visible)
            {
                Application.Exit();
            }
        }

        #endregion
    }
}
