
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using FuturePath.ApplicationLayer;

namespace FuturePath.PresentationLayer
{
    // Handles the learner interest questionnaire and category scores
    public partial class MainForm
    {
        // Fonts and sizes used for questionnaire rows
        private static readonly Font InterestsStatementFont = new Font("Segoe UI", 10F);
        private static readonly Font InterestsCategoryFont = new Font("Segoe UI", 8F);
        private const int InterestsRowHeight = 36;   // at 100% scaling

        // Labels for the five response options
        private static readonly string[] InterestsResponseCaptions = { "Strongly disagree", "Disagree", "Neutral", "Agree", "Strongly agree" };

        private const string InterestsStepStatus = "Step 3 of 6 · Choose one response for every statement.";

        // Stores the questionnaire rows in display order
        private readonly List<InterestQuestionRow> interestsRows = new List<InterestQuestionRow>();

        // Tracks whether questions have been loaded for this session
        private bool interestsLoaded;

        #region Statement rows

        // Stores the controls and response for a questionnaire statement
        private sealed class InterestQuestionRow
        {
            // ID of the question
            public int QuestionId { get; set; }

            // Category shown below the statement
            public string CategoryName { get; set; }

            // Position of the statement in the list
            public int Index { get; set; }

            // Panel containing the statement and radio buttons
            public Panel Container { get; set; }

            // Label showing the category and answer status
            public Label CategoryLabel { get; set; }

            // Selected response from 1 to 5, or null if unanswered
            public int? Response { get; set; }
        }

        // Build questionnaire rows from the active questions
        private void InterestsBuildQuestionRows(DataTable questions)
        {
            pnlStatements.SuspendLayout();
            InterestsClearRows();
            foreach (DataRow question in questions.Rows)
            {
                InterestQuestionRow row = new InterestQuestionRow();
                row.QuestionId = Convert.ToInt32(question["QuestionID"], CultureInfo.InvariantCulture);
                row.CategoryName = Convert.ToString(question["CategoryName"], CultureInfo.CurrentCulture);
                row.Index = interestsRows.Count;
                row.Container = InterestsBuildRowPanel(row, Convert.ToString(question["QuestionText"], CultureInfo.CurrentCulture));
                interestsRows.Add(row);
                pnlStatements.Controls.Add(row.Container);
            }

            // Adjust row widths depending on available space
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

        // Remove previous questionnaire rows and their controls
        private void InterestsClearRows()
        {
            for (int i = pnlStatements.Controls.Count - 1; i >= 0; i--)
            {
                pnlStatements.Controls[i].Dispose();
            }

            interestsRows.Clear();
        }

        // Create a panel containing a statement and five response buttons
        private Panel InterestsBuildRowPanel(InterestQuestionRow row, string statement)
        {
            // Use the header labels to align the response buttons
            // Scale sizes to match the display settings
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

        // Save the selected response and update questionnaire progress
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

        // Load questions and refresh progress when the page opens
        private void InterestsShown()
        {
            SetStatus(InterestsStepStatus, DeepNavy, Color.White);
            if (!interestsLoaded)
            {
                InterestsLoadQuestions();
            }

            InterestsRefreshProgress();
        }

        // Clear answers and question rows for a new learner
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

        // Load active interest questions and build their rows
        private void InterestsLoadQuestions()
        {
            try
            {
                InterestController interestController = new InterestController();
                InterestsBuildQuestionRows(interestController.GetActiveQuestions());
                interestsLoaded = true;
            }
            catch (Exception)
            {
                SetStatus("The questionnaire could not be loaded. Please go back and try again.", AmberColour, InkColour);
            }
        }

        // Collect the responses answered so far
        private Dictionary<int, int> InterestsResponses()
        {
            Dictionary<int, int> responses = new Dictionary<int, int>();
            foreach (InterestQuestionRow row in interestsRows)
            {
                if (row.Response.HasValue)
                {
                    responses.Add(row.QuestionId, row.Response.Value);
                }
            }

            return responses;
        }

        // Calculate and display the strongest interest categories
        private void InterestsRefreshCategoryScores()
        {
            // Category scores are calculated from the current responses
            List<KeyValuePair<string, decimal>> scores = new List<KeyValuePair<string, decimal>>();
            try
            {
                InterestController interestController = new InterestController();
                scores = interestController.CalculateCategoryScores(InterestsResponses());
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

        // Save the learner's questionnaire responses
        private void InterestsSave()
        {
            InterestController interestController = new InterestController();
            interestController.SaveResponses(SessionId.Value, InterestsResponses());
        }

        #endregion

        #region Progress and notice

        // Update answered counts, row colours and category scores
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

        // Save all answers and continue to faculty selection
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
            catch (ArgumentException ex)
            {
                // Show any validation errors returned by the controller
                SetStatus(ex.Message, AmberColour, InkColour);
                return;
            }
            catch (Exception)
            {
                SetStatus("Your answers could not be saved. Please try again.", AmberColour, InkColour);
                return;
            }

            ForgetResults();
            GoToStep(tabFaculty, 4);
        }

        // Return to the subjects screen without clearing answers
        private void btnInterestsBack_Click(object sender, EventArgs e)
        {
            tcMain.SelectedTab = tabSubjects;
        }

        // Cancel the session and return home
        private void btnInterestsCancel_Click(object sender, EventArgs e)
        {
            GoHome();
        }

        #endregion
    }
}
