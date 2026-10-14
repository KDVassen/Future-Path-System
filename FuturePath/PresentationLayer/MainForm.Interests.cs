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
    /// UI 4 — Interest Questionnaire (step 3 of 6; spec 2.3.4).
    /// One five-point RadioButton group per active <c>InterestQuestion</c>, in <c>DisplayOrder</c>, with the
    /// <c>InterestCategory</c> name under each statement. The rows are built at runtime from the bound
    /// question list, each in its own panel so its five radio buttons form one group. Nothing is typed,
    /// so nothing can be invalid: unanswered statements are shaded amber and counted in the amber notice,
    /// and <b>Save and continue</b> stays disabled until every statement is answered.
    /// Red never appears on this screen and there is no validation dialog.
    /// </summary>
    public partial class MainForm
    {
        // Fonts and sizes of the statement rows built at runtime (spec 2.2.5: body 10 pt, caption 8 pt).
        private static readonly Font InterestsStatementFont = new Font("Segoe UI", 10F);
        private static readonly Font InterestsCategoryFont = new Font("Segoe UI", 8F);
        private const int InterestsRowHeight = 36;   // at 100% scaling

        /// <summary>Captions of the 1–5 scale, used for the radio buttons' accessible names.</summary>
        private static readonly string[] InterestsResponseCaptions = { "Strongly disagree", "Disagree", "Neutral", "Agree", "Strongly agree" };

        private const string InterestsStepStatus = "Step 3 of 6 · Choose one response for every statement.";

        /// <summary>One entry per statement row, in display order.</summary>
        private readonly List<InterestQuestionRow> interestsRows = new List<InterestQuestionRow>();

        /// <summary>True once the questions have been loaded and the rows built for this session.</summary>
        private bool interestsLoaded;

        #region Statement rows

        /// <summary>The controls and answer of one statement row.</summary>
        private sealed class InterestQuestionRow
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
        private static object InterestsReadMember(object item, string member)
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
        private static string InterestsReadText(object item, string member)
        {
            object value = InterestsReadMember(item, member);
            return value == null || value == DBNull.Value ? string.Empty : value.ToString();
        }

        /// <summary>
        /// Builds one statement row per item of the question list, in the order given (the BLL sorts by
        /// DisplayOrder). Each item must expose QuestionID, QuestionText and CategoryName; a DataTable,
        /// DataView or a list of entity objects all work.
        /// </summary>
        /// <param name="source">The active questions, already ordered.</param>
        private void InterestsBuildQuestionRows(object source)
        {
            var table = source as DataTable;
            IEnumerable items = table != null ? table.DefaultView : source as IEnumerable;

            pnlStatements.SuspendLayout();
            InterestsClearRows();
            if (items != null)
            {
                foreach (object item in items)
                {
                    var row = new InterestQuestionRow
                    {
                        QuestionId = Convert.ToInt32(InterestsReadMember(item, "QuestionID"), CultureInfo.InvariantCulture),
                        CategoryName = InterestsReadText(item, "CategoryName"),
                        Index = interestsRows.Count
                    };
                    row.Container = InterestsBuildRowPanel(row, InterestsReadText(item, "QuestionText"));
                    interestsRows.Add(row);
                    pnlStatements.Controls.Add(row.Container);
                }
            }

            // Rows fill the panel when they all fit; otherwise leave room for the vertical scroll bar.
            if (interestsRows.Count * LogicalToDeviceUnits(InterestsRowHeight) <= pnlStatements.ClientSize.Height)
            {
                foreach (InterestQuestionRow row in interestsRows)
                {
                    row.Container.Width = pnlStatements.ClientSize.Width;
                }
            }

            pnlStatements.ResumeLayout(true);
            InterestsRefreshProgress();
        }

        /// <summary>
        /// Removes every statement row and releases its controls, so a new session starts with none of the
        /// previous learner's answers (rows are rebuilt when the questions are next loaded).
        /// </summary>
        private void InterestsClearRows()
        {
            for (int i = pnlStatements.Controls.Count - 1; i >= 0; i--)
            {
                pnlStatements.Controls[i].Dispose();
            }

            interestsRows.Clear();
        }

        /// <summary>
        /// Creates the container for one statement: the statement, its category caption and five
        /// unlabelled radio buttons aligned under the Strongly disagree … Strongly agree headers.
        /// </summary>
        private Panel InterestsBuildRowPanel(InterestQuestionRow row, string statement)
        {
            // The radio buttons line up under the Strongly disagree … Strongly agree header labels, so the
            // columns are read from those Designer labels. The other sizes are pixels at 100% scaling, so
            // LogicalToDeviceUnits converts them for the screen's scale (e.g. 36 → 45 at 125%).
            int firstResponseColumn = lblHeaderResponse1.Left;
            int responseColumnWidth = lblHeaderResponse1.Width;
            int rowHeight = LogicalToDeviceUnits(InterestsRowHeight);
            int margin = LogicalToDeviceUnits(12);
            int statementWidth = firstResponseColumn - 2 * margin;
            int rowWidth = pnlStatements.ClientSize.Width - SystemInformation.VerticalScrollBarWidth;
            Size radioSize = new Size(LogicalToDeviceUnits(16), LogicalToDeviceUnits(18));

            int number = row.Index + 1;
            var panel = new Panel
            {
                Name = "pnlQuestion" + number,
                Location = new Point(0, row.Index * rowHeight),
                Size = new Size(rowWidth, rowHeight),
                AccessibleName = "Statement " + number,
                Tag = row
            };

            var statementLabel = new Label
            {
                Name = "lblQuestion" + number,
                Text = statement,
                Font = InterestsStatementFont,
                ForeColor = InkColour,
                AutoEllipsis = true,
                Location = new Point(margin, LogicalToDeviceUnits(1)),
                Size = new Size(statementWidth, LogicalToDeviceUnits(19))
            };

            row.CategoryLabel = new Label
            {
                Name = "lblQuestionCategory" + number,
                Font = InterestsCategoryFont,
                ForeColor = MutedColour,
                Location = new Point(margin, LogicalToDeviceUnits(19)),
                Size = new Size(statementWidth, LogicalToDeviceUnits(16))
            };

            panel.Controls.Add(statementLabel);
            panel.Controls.Add(row.CategoryLabel);
            for (int value = 1; value <= 5; value++)
            {
                var option = new RadioButton
                {
                    Name = "rdoQuestion" + number + "Response" + value,
                    Tag = value,
                    AccessibleName = "Statement " + number + ": " + InterestsResponseCaptions[value - 1],
                    Location = new Point(firstResponseColumn + (value - 1) * responseColumnWidth
                                         + (responseColumnWidth - radioSize.Width) / 2, LogicalToDeviceUnits(9)),
                    Size = radioSize,
                    UseVisualStyleBackColor = true
                };
                option.CheckedChanged += InterestsResponse_CheckedChanged;
                panel.Controls.Add(option);
            }

            return panel;
        }

        /// <summary>Records the chosen response for the row and refreshes the counts, shading and tiles.</summary>
        private void InterestsResponse_CheckedChanged(object sender, EventArgs e)
        {
            var option = (RadioButton)sender;
            if (!option.Checked)
            {
                return;
            }

            var row = (InterestQuestionRow)option.Parent.Tag;
            row.Response = (int)option.Tag;
            InterestsRefreshProgress();
        }

        #endregion

        #region Showing and resetting

        /// <summary>
        /// Called by PageShown each time UI 4 is shown: loads the questions the first time in a session, then
        /// refreshes the shading, tiles and notice, and keeps Save and continue disabled until every
        /// statement is answered.
        /// </summary>
        private void InterestsShown()
        {
            SetStatus(InterestsStepStatus, DeepNavy, Color.White);
            if (!interestsLoaded)
            {
                InterestsLoadQuestions();
            }

            InterestsRefreshProgress();
        }

        /// <summary>
        /// Returns UI 4 to a brand-new session: every statement row and answer removed (the questions are
        /// loaded again when the screen is next shown), the category tiles hidden and reset, and the Answered
        /// tile back to its empty state. No learner may see the previous learner's answers (spec 2.1).
        /// </summary>
        private void InterestsReset()
        {
            pnlStatements.SuspendLayout();
            InterestsClearRows();
            pnlStatements.ResumeLayout(true);
            interestsLoaded = false;

            Label[] captions = { lblCategory1TileCaption, lblCategory2TileCaption, lblCategory3TileCaption };
            Label[] values = { lblCategory1TileValue, lblCategory2TileValue, lblCategory3TileValue };
            for (int i = 0; i < captions.Length; i++)
            {
                captions[i].Text = "Category";
                values[i].Text = "—";
            }

            InterestsRefreshProgress();
        }

        #endregion

        #region Business Logic Layer hooks

        /// <summary>Loads the active questions (with category names) in DisplayOrder and builds the rows.</summary>
        private void InterestsLoadQuestions()
        {
            try
            {
                // TODO BLL: InterestsBuildQuestionRows(InterestController.GetActiveQuestions());
                //           Active InterestQuestion rows joined to InterestCategory, ordered by DisplayOrder:
                //           QuestionID, QuestionText, CategoryName.
                InterestsBuildQuestionRows(null);
                interestsLoaded = true;
            }
            catch (Exception)
            {
                SetStatus("The questionnaire could not be loaded. Please go back and try again.", AmberColour, InkColour);
            }
        }

        /// <summary>The responses given so far, QuestionID → ResponseValue (1–5).</summary>
        private Dictionary<int, int> InterestsResponses()
        {
            return interestsRows.Where(r => r.Response.HasValue).ToDictionary(r => r.QuestionId, r => r.Response.Value);
        }

        /// <summary>
        /// Asks the BLL for the category scores of the answers so far and shows the three strongest
        /// in the tiles. Tiles with no score are hidden, never shown empty.
        /// </summary>
        private void InterestsRefreshCategoryScores()
        {
            var scores = new List<KeyValuePair<string, double>>();
            try
            {
                // TODO BLL: scores = InterestController.CalculateCategoryScores(SessionId, InterestsResponses());
                //           (Learner.calculateInterestScore per InterestCategory; highest first, 0–100.)
            }
            catch (Exception)
            {
                scores.Clear();
                SetStatus("Your category scores could not be worked out yet. Your answers are kept.", AmberColour, InkColour);
            }

            Panel[] tiles = { pnlCategory1Tile, pnlCategory2Tile, pnlCategory3Tile };
            Label[] captions = { lblCategory1TileCaption, lblCategory2TileCaption, lblCategory3TileCaption };
            Label[] values = { lblCategory1TileValue, lblCategory2TileValue, lblCategory3TileValue };
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
        private void InterestsSave()
        {
            // TODO BLL: InterestController.SaveResponses(SessionId.Value, InterestsResponses());
        }

        #endregion

        #region Progress and notice

        /// <summary>
        /// Shades each row (amber while unanswered, banded once answered), updates the Answered tile,
        /// the "n of N answered" caption and the category tiles. While UI 4 is on screen it also updates
        /// the notice and enables Save and continue only when every statement is answered.
        /// </summary>
        private void InterestsRefreshProgress()
        {
            int total = interestsRows.Count;
            int answered = interestsRows.Count(r => r.Response.HasValue);
            int outstanding = total - answered;

            foreach (InterestQuestionRow row in interestsRows)
            {
                bool done = row.Response.HasValue;
                row.Container.BackColor = done ? (row.Index % 2 == 1 ? BlueTint : Color.White) : AmberTint;
                row.CategoryLabel.ForeColor = done ? MutedColour : AmberText;
                row.CategoryLabel.Text = done ? row.CategoryName : "Not yet answered  ·  " + row.CategoryName;
            }

            lblAnsweredCount.Text = answered + " of " + total + " answered";
            lblAnsweredTileValue.Text = answered + " / " + total;
            bool complete = total > 0 && outstanding == 0;
            pnlAnsweredTile.BackColor = complete ? GreenTint : AmberTint;
            lblAnsweredTileValue.ForeColor = complete ? GreenColour : AmberText;
            lblAnsweredTileNote.Text = total == 0 ? "no statements loaded" : complete ? "all answered" : outstanding + " outstanding";

            if (total == 0)
            {
                ShowNotice(lblInterestsNotice, "The questionnaire statements are read from stored data and appear here once they are loaded.",
                    BlueTint, InkColour);
            }
            else if (complete)
            {
                ShowNotice(lblInterestsNotice, "✓ All " + total + " statements answered. Save and continue to choose a faculty and programme.",
                    GreenTint, GreenColour);
            }
            else
            {
                ShowNotice(lblInterestsNotice, outstanding + (outstanding == 1 ? " statement" : " statements") + " still to answer. The interest " +
                           "alignment score is calculated only once all " + total + " are answered; the outstanding statements " +
                           "are shaded below.", AmberTint, AmberText);
            }

            SetPrimaryEnabled(btnInterestsSave, complete);
            InterestsRefreshCategoryScores();
        }

        #endregion

        #region Button click events

        /// <summary>Save and continue (enabled only when every statement is answered) → UI 5.</summary>
        private void btnInterestsSave_Click(object sender, EventArgs e)
        {
            if (interestsRows.Count == 0 || interestsRows.Any(r => !r.Response.HasValue))
            {
                return;
            }

            try
            {
                InterestsSave();
            }
            catch (Exception)
            {
                SetStatus("Your answers could not be saved. Please try again.", AmberColour, InkColour);
                return;
            }

            GoToStep(tabFaculty, 4);
        }

        /// <summary>Back → UI 3. The answers are kept.</summary>
        private void btnInterestsBack_Click(object sender, EventArgs e)
        {
            tcMain.SelectedTab = tabSubjects;
        }

        /// <summary>Cancel leaves the session (the Home tab asks first).</summary>
        private void btnInterestsCancel_Click(object sender, EventArgs e)
        {
            GoHome();
        }

        #endregion
    }
}
