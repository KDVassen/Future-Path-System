using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// The Future Path main window: UI 1 Homepage and the whole self-service learner session (UI 2–7).
    /// Every screen is a TabPage of the TabControl <c>tcMain</c>, in step order: Home, then steps 1–6.
    /// Step 5 holds a second TabControl, <c>tcResults</c>, with UI 6, 6.1a and 6.2b. The learner can only
    /// open a step that has already been reached (spec 2.2.2), and each tab has its own buttons along
    /// its bottom edge. The window is a fixed size (decisions.md D6).
    /// <para>
    /// This file holds what every tab shares: the colour palette, the learner session, moving between
    /// tabs, the menu, and the status bar. Each screen's own controls and code live in a partial-class
    /// file named after it (MainForm.Profile.cs, MainForm.Subjects.cs, …).
    /// </para>
    /// </summary>
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

        /// <summary>The highest step (1–6) the learner has reached; tabs after it stay locked. 0 = no session.</summary>
        private int highestStepReached;

        /// <summary>The tab that opened UI 7, so Back on UI 7 returns to it.</summary>
        private TabPage summaryReturnPage;

        /// <summary>Creates the main window on the homepage (UI 1).</summary>
        public MainForm()
        {
            InitializeComponent();
            numMark.KeyPress += DecimalBox_KeyPress;

            // No tab events fire before the window exists, so show the homepage by hand.
            PageShown(tabHome);
        }

        #region The learner session (held once for every step — nothing is copied between screens)

        /// <summary>AdvisorySession.SessionID, returned by the BLL when UI 2 is first saved.</summary>
        private int? SessionId { get; set; }

        /// <summary>Learner.LearnerID, returned by the BLL when UI 2 is first saved.</summary>
        private int? LearnerId { get; set; }

        /// <summary>The display session number yyyy-NNNN, built by the BLL (not stored).</summary>
        private string SessionNumber { get; set; }

        /// <summary>The preferred faculty chosen on UI 5.</summary>
        private int? PreferredFacultyId { get; set; }

        /// <summary>The programme checked on UI 5.</summary>
        private int? SelectedProgrammeId { get; set; }

        /// <summary>The checked programme's name, for headings on UI 6, 6.1a and 7.</summary>
        private string SelectedProgrammeName { get; set; }

        /// <summary>The learner's name, grade and school for the status bar, set when UI 2 is saved.</summary>
        private string LearnerContext { get; set; }

        /// <summary>
        /// Clears every captured value so the next learner starts empty. Called whenever the learner goes
        /// back to the homepage: nothing in the learner lane may show another learner's data (spec 2.1).
        /// </summary>
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

        /// <summary>UI 1 → Start advisory session: a fresh, empty session on UI 2.</summary>
        private void StartLearnerSession()
        {
            ResetLearnerSession();
            GoToStep(tabProfile, 1);
        }

        /// <summary>
        /// Home, Cancel, or Back from UI 2. Selecting the Home tab asks the learner to confirm first
        /// (see <see cref="tcMain_Selecting"/>), so every way home goes through the same check.
        /// </summary>
        private void GoHome()
        {
            tcMain.SelectedTab = tabHome;
        }

        /// <summary>UI 6, 6.1a or 6.2b → UI 7. Back on UI 7 returns to the tab that opened it.</summary>
        private void OpenSummary(TabPage from)
        {
            summaryReturnPage = from;
            GoToStep(tabSummary, 6);
        }

        #endregion

        #region Moving between tabs

        /// <summary>True for every tab that belongs to the learner session (everything but UI 1).</summary>
        private bool IsLearnerPage(TabPage page)
        {
            return page != null && page != tabHome;
        }

        /// <summary>The tab on screen (the results sub-tab when step 5 is showing).</summary>
        private TabPage CurrentPage
        {
            get { return tcMain.SelectedTab == tabResults ? tcResults.SelectedTab : tcMain.SelectedTab; }
        }

        /// <summary>Moves forward to a step: records it as reached (which unlocks its tab), then opens it.</summary>
        private void GoToStep(TabPage page, int step)
        {
            if (step > highestStepReached)
            {
                highestStepReached = step;
            }

            SelectPage(page);
        }

        /// <summary>Opens a tab. A results tab (UI 6, 6.1a, 6.2b) is chosen inside tcResults first, then step 5 is opened.</summary>
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

        /// <summary>
        /// Runs before a tab opens, whether the learner clicked it or the code selected it.
        /// The tab's position in tcMain is its step number (Home = 0), so a step not yet reached stays locked.
        /// Going Home asks first, unless the session is already complete.
        /// </summary>
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

        /// <summary>A new tab is showing: refresh it.</summary>
        private void tcMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            PageShown(CurrentPage);
        }

        /// <summary>A results tab (UI 6 / 6.1a / 6.2b) was chosen: refresh it, if step 5 is the tab on screen.</summary>
        private void tcResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tcMain.SelectedTab == tabResults)
            {
                PageShown(CurrentPage);
            }
        }

        /// <summary>
        /// Sets up the window for the tab on screen: the caption, the status bar, which buttons Enter and
        /// Esc press, and then lets the screen refresh itself through its <c>…Shown()</c> method.
        /// </summary>
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

        /// <summary>Menu → Home: back to UI 1 (the Selecting check asks first during a session).</summary>
        private void mnuHome_Click(object sender, EventArgs e)
        {
            GoHome();
        }

        /// <summary>Menu → Exit: closes Future Path (FormClosing asks first).</summary>
        private void mnuExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region Shared helpers (used by every screen's partial file)

        /// <summary>Shows a message in the status bar (never a modal; spec 2.2.7).</summary>
        private void SetStatus(string message, Color back, Color fore)
        {
            pnlStatus.BackColor = back;
            lblStatus.ForeColor = fore;
            lblLearnerContext.ForeColor = fore;
            lblStatus.Text = message;
        }

        /// <summary>Fills a tab's notice label: blue guidance, amber outstanding, red invalid, green go-on.</summary>
        private static void ShowNotice(Label notice, string message, Color back, Color fore)
        {
            notice.Text = message;
            notice.BackColor = back;
            notice.ForeColor = fore;
        }

        /// <summary>Enables or disables a tab's primary button; disabled shows the rule grey (spec 2.2.3).</summary>
        private static void SetPrimaryEnabled(Button button, bool enabled)
        {
            button.Enabled = enabled;
            button.BackColor = enabled ? NavyColour : RuleColour;
        }

        /// <summary>
        /// Shows or clears a field message (spec 2.2.7): red text with a warning glyph under the control,
        /// a red tint on the control and a red label. Returns <paramref name="valid"/>.
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

        /// <summary>
        /// Lets a decimal box accept either "." or "," as the decimal point. A NumericUpDown only accepts the
        /// separator of the Windows regional setting (a comma on many South African machines), so "67.5"
        /// would otherwise be refused; the typed key is swapped for the regional separator before the box sees it.
        /// </summary>
        private void DecimalBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '.' || e.KeyChar == ',')
            {
                e.KeyChar = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
            }
        }

        /// <summary>Asks for confirmation (spec Section 7). Returns true when the user confirms.</summary>
        private bool Confirm(string title, string message, string confirmText)
        {
            using (var dialog = new ConfirmationDialog(title, message, confirmText))
            {
                return dialog.ShowDialog(this) == DialogResult.OK;
            }
        }

        /// <summary>Raises the validation dialog (UI 2.1a / 3.1a), then returns focus to the first offending control.</summary>
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

        /// <summary>The caption-bar X (or Menu → Exit) closes Future Path, so it asks first.</summary>
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
