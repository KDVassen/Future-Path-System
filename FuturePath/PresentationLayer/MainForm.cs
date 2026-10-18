
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    // Main window for the homepage and learner advisory session
    // Handles session state, navigation, menus and shared UI behaviour
    public partial class MainForm : Form
    {
        #region Palette (spec 2.2.6) — runtime colours and fonts; everything else is set in the Designer

        private static readonly Color NavyColour = Color.FromArgb(19, 41, 75);
        private static readonly Color DeepNavy = Color.FromArgb(11, 31, 58);
        private static readonly Color InkColour = Color.FromArgb(27, 31, 36);
        private static readonly Color MutedColour = Color.FromArgb(88, 96, 108);
        private static readonly Color RuleColour = Color.FromArgb(215, 220, 227);
        private static readonly Color BlueColour = Color.FromArgb(31, 95, 168);
        private static readonly Color BlueTint = Color.FromArgb(232, 238, 246);
        private static readonly Color AmberColour = Color.FromArgb(200, 137, 43);
        private static readonly Color AmberText = Color.FromArgb(138, 93, 24);
        private static readonly Color AmberTint = Color.FromArgb(252, 243, 224);
        private static readonly Color RedColour = Color.FromArgb(164, 38, 44);
        private static readonly Color RedTint = Color.FromArgb(250, 234, 235);
        private static readonly Color GreenColour = Color.FromArgb(31, 107, 58);
        private static readonly Color GreenTint = Color.FromArgb(230, 242, 234);
        private static readonly Font ChipFont = new Font("Segoe UI Semibold", 9F);

        #endregion

        // Highest step reached by the learner; later tabs remain locked
        private int highestStepReached;

        // Tab to return to when leaving the summary screen
        private TabPage summaryReturnPage;

        // Initialise the main window and display the homepage
        public MainForm()
        {
            InitializeComponent();
            numMark.KeyPress += DecimalBox_KeyPress;

            // Display the homepage when the window first opens
            PageShown(tabHome);
        }

        #region The learner session (held once for every step — nothing is copied between screens)

        // ID of the current advisory session
        private int? SessionId { get; set; }

        // ID of the current learner
        private int? LearnerId { get; set; }

        // Formatted session number displayed to the user
        private string SessionNumber { get; set; }

        // Faculty selected by the learner
        private int? PreferredFacultyId { get; set; }

        // Programme selected by the learner
        private int? SelectedProgrammeId { get; set; }

        // Name of the selected programme for results and summaries
        private string SelectedProgrammeName { get; set; }

        // Learner details displayed in the status bar
        private string LearnerContext { get; set; }

        // Clear all session details before starting with another learner
        private void ResetLearnerSession()
        {
            SessionId = null;
            LearnerId = null;
            SessionNumber = null;
            PreferredFacultyId = null;
            SelectedProgrammeId = null;
            SelectedProgrammeName = null;
            LearnerContext = null;
            highestStepReached = 0;
            summaryReturnPage = null;

            ProfileReset();
            SubjectsReset();
            InterestsReset();
            FacultyReset();
            OutcomeReset();
            AlternativesReset();
            FundingReset();
            SummaryReset();
        }

        // Start a new advisory session from the homepage
        private void StartLearnerSession()
        {
            ResetLearnerSession();
            GoToStep(tabProfile, 1);
        }

        // Return to the homepage
        // The tab selection handler checks whether confirmation is needed
        private void GoHome()
        {
            tcMain.SelectedTab = tabHome;
        }

        // Clear previous eligibility results when subjects or interests change
        // Lock the results and summary until eligibility is checked again
        private void ForgetResults()
        {
            if (highestStepReached <= 4)
            {
                return;
            }

            highestStepReached = 4;
            OutcomeReset();
            AlternativesReset();
            FundingReset();
            SummaryReset();
        }

        // Open the summary and remember which results tab opened it
        private void OpenSummary(TabPage from)
        {
            summaryReturnPage = from;
            GoToStep(tabSummary, 6);
        }

        #endregion

        #region Moving between tabs

        // Check whether a tab belongs to the learner session
        private bool IsLearnerPage(TabPage page)
        {
            return page != null && page != tabHome;
        }

        // Get the currently displayed tab, including results sub-tabs
        private TabPage CurrentPage
        {
            get { return tcMain.SelectedTab == tabResults ? tcResults.SelectedTab : tcMain.SelectedTab; }
        }

        // Unlock and navigate to the requested step
        private void GoToStep(TabPage page, int step)
        {
            if (step > highestStepReached)
            {
                highestStepReached = step;
            }

            SelectPage(page);
        }

        // Open a main tab or select the correct results sub-tab
        private void SelectPage(TabPage page)
        {
            if (page == tabOutcome || page == tabAlternatives || page == tabFunding)
            {
                tcResults.SelectedTab = page;
                tcMain.SelectedTab = tabResults;
            }
            else
            {
                tcMain.SelectedTab = page;
            }
        }

        // Prevent learners from opening steps they have not reached
        // Confirm before returning home with an unfinished session
        private void tcMain_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage == tabHome)
            {
                if (highestStepReached > 0 && !summaryExported && !Confirm("Leave this advisory session",
                    "Return to the homepage? Anything already saved stays on record as an \"In progress\" session. " +
                    "Your session is only completed when you export the advisory summary.", "Leave session"))
                {
                    e.Cancel = true;
                    return;
                }

                ResetLearnerSession();
                return;
            }

            if (e.TabPageIndex > highestStepReached || (summaryExported && e.TabPage != tabSummary))
            {
                e.Cancel = true;
                SetStatus(highestStepReached == 0
                    ? "Choose Start advisory session to begin."
                    : summaryExported
                        ? "This session is complete. Choose Finish to return to the homepage."
                        : "Finish this step first: the steps open in order.", AmberColour, InkColour);
            }
        }

        // Refresh the screen when the selected main tab changes
        private void tcMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            PageShown(CurrentPage);
        }

        // Refresh the selected results tab when it is visible
        private void tcResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tcMain.SelectedTab == tabResults)
            {
                PageShown(CurrentPage);
            }
        }

        // Update the window, status bar and buttons for the selected page
        // Call the appropriate screen method to refresh its contents
        private void PageShown(TabPage page)
        {
            Text = "Future Path — " + page.Text;
            lblLearnerContext.Text = IsLearnerPage(page) && page != tabProfile ? LearnerContext : string.Empty;
            SetStatus(page == tabHome ? "Choose how you want to use Future Path." : "Step " + tcMain.SelectedIndex + " of 6",
                      DeepNavy, Color.White);

            if (page == tabHome)
            {
                AcceptButton = null;
                CancelButton = null;
                HomeShown();
            }
            else if (page == tabProfile)
            {
                AcceptButton = btnProfileSave;
                CancelButton = btnProfileCancel;
                ProfileShown();
            }
            else if (page == tabSubjects)
            {
                AcceptButton = btnSubjectsSave;
                CancelButton = btnSubjectsCancel;
                SubjectsShown();
            }
            else if (page == tabInterests)
            {
                AcceptButton = btnInterestsSave;
                CancelButton = btnInterestsCancel;
                InterestsShown();
            }
            else if (page == tabFaculty)
            {
                AcceptButton = btnFacultyCheck;
                CancelButton = btnFacultyCancel;
                FacultyShown();
            }
            else if (page == tabOutcome)
            {
                AcceptButton = btnOutcomeSummary;
                CancelButton = null;
                OutcomeShown();
            }
            else if (page == tabAlternatives)
            {
                AcceptButton = btnAlternativesSummary;
                CancelButton = null;
                AlternativesShown();
            }
            else if (page == tabFunding)
            {
                AcceptButton = btnFundingAdd;
                CancelButton = null;
                FundingShown();
            }
            else if (page == tabSummary)
            {
                AcceptButton = btnSummaryExport;
                CancelButton = btnSummaryCancel;
                SummaryShown();
            }
        }

        #endregion

        #region Menu

        // Return to the homepage using the menu
        private void mnuHome_Click(object sender, EventArgs e)
        {
            GoHome();
        }

        // Close the application using the menu
        private void mnuExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region Shared helpers (used by every screen's partial file)

        // Display a message in the status bar
        private void SetStatus(string message, Color back, Color fore)
        {
            pnlStatus.BackColor = back;
            lblStatus.ForeColor = fore;
            lblLearnerContext.ForeColor = fore;
            lblStatus.Text = message;
        }

        // Display a coloured notice message on a screen
        private static void ShowNotice(Label notice, string message, Color back, Color fore)
        {
            notice.Text = message;
            notice.BackColor = back;
            notice.ForeColor = fore;
        }

        // Enable or disable a screen's main action button
        private static void SetPrimaryEnabled(Button button, bool enabled)
        {
            button.Enabled = enabled;
            button.BackColor = enabled ? NavyColour : RuleColour;
        }

        // Show validation feedback and highlight invalid input fields
        private static bool CheckField(bool valid, Control input, Label fieldLabel, Label messageLabel, string message)
        {
            input.BackColor = valid ? SystemColors.Window : RedTint;
            fieldLabel.ForeColor = valid ? InkColour : RedColour;
            messageLabel.Text = valid ? string.Empty : "⚠ " + message;
            messageLabel.Visible = !valid;
            input.AccessibleDescription = valid ? null : message;
            return valid;
        }

        // Accept either a dot or comma as the decimal separator
        // Convert it to the separator used by the current Windows region
        private void DecimalBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '.' || e.KeyChar == ',')
            {
                e.KeyChar = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
            }
        }

        // Show a confirmation dialog and return whether the user confirmed
        private bool Confirm(string title, string message, string confirmText)
        {
            using (var dialog = new ConfirmationDialog(title, message, confirmText))
            {
                return dialog.ShowDialog(this) == DialogResult.OK;
            }
        }

        // Show validation errors and return focus to the invalid control
        private void ShowValidation(string title, IList<string> lines, Control focus)
        {
            using (var dialog = new ValidationDialog(title, lines))
            {
                dialog.ShowDialog(this);
            }

            if (focus != null && focus.CanFocus)
            {
                focus.Focus();
            }
        }

        #endregion

        #region Closing

        // Confirm before closing the application
        // Warn the learner if their session has not been completed
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing)
            {
                return;
            }

            string message = IsLearnerPage(CurrentPage) && !summaryExported
                ? "Close Future Path? Anything already saved stays on record as an \"In progress\" session."
                : "Close Future Path?";
            if (!Confirm("Close Future Path", message, "Close"))
            {
                e.Cancel = true;
            }
        }

        #endregion
    }
}
