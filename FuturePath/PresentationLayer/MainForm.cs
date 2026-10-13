using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// The Future Path main window: UI 1 Homepage and the whole self-service learner session
    /// (UI 2–7, with UI 6, 6.1a and 6.2b grouped under visible tabs). Every screen is a TabPage of
    /// <c>tabMain</c>, whose own tab strip is hidden, so each step still looks like its own screen and
    /// the learner can only move in the fixed order of spec 2.2.2. The window is always 1200 × 800
    /// (decisions.md D6).
    /// <para>
    /// This file holds the screen shell that every page shares: the colour palette, page switching,
    /// the navigation rail, the context strip, notice panel, status band, the action band and the
    /// session values. Each screen's own controls and code live in a partial-class file named after
    /// it (MainForm.Profile.cs, MainForm.Subjects.cs, …).
    /// </para>
    /// </summary>
    public partial class MainForm : Form
    {
        #region Palette (spec 2.2.6) — runtime colours and fonts; everything else is set in the Designer

        private static readonly Color NavyColour = Color.FromArgb(19, 41, 75);
        private static readonly Color DeepNavy = Color.FromArgb(11, 31, 58);
        private static readonly Color InkColour = Color.FromArgb(27, 31, 36);
        private static readonly Color MutedColour = Color.FromArgb(88, 96, 108);
        private static readonly Color GreyColour = Color.FromArgb(88, 96, 108);
        private static readonly Color GreyTint = Color.FromArgb(236, 238, 241);
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
        private static readonly Color RailActive = Color.FromArgb(38, 64, 104);
        private static readonly Color RailText = Color.FromArgb(214, 222, 235);
        private static readonly Color RailLocked = Color.FromArgb(112, 130, 158);
        private static readonly Font RailFont = new Font("Segoe UI", 10F);
        private static readonly Font RailActiveFont = new Font("Segoe UI Semibold", 10F);
        private static readonly Font ChipFont = new Font("Segoe UI Semibold", 9F);

        #endregion

        private readonly Label[] railSteps;
        private int highestStepReached;
        private TabPage summaryReturnPage;
        private bool changingPage;

        /// <summary>Creates the main window on the homepage (UI 1).</summary>
        public MainForm()
        {
            InitializeComponent();
            railSteps = new[] { lblStep1, lblStep2, lblStep3, lblStep4, lblStep5, lblStep6 };
            numMark.KeyPress += DecimalBox_KeyPress;
            ShowPage(tabHome);
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

        /// <summary>The learner context strip text, set when UI 2 is saved.</summary>
        private string LearnerContext { get; set; }

        /// <summary>
        /// Clears every captured value so the next learner starts empty. Called on Leave, Cancel, Home and
        /// after a completed export: nothing in the learner lane may show another learner's data (spec 2.1).
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
        /// Home, Cancel, or Back from UI 2: asks first, then returns to UI 1 with the session cleared.
        /// Anything already saved stays on record as an "In progress" session (decisions.md D3).
        /// </summary>
        private void LeaveSession()
        {
            if (summaryExported)
            {
                FinishSession();       // the session is already complete: nothing is left "In progress"
                return;
            }

            if (!Confirm("Leave this advisory session",
                "Return to the homepage? Anything already saved stays on record as an \"In progress\" session. " +
                "Your session is only completed when you export the advisory summary.", "Leave session"))
            {
                return;
            }

            ResetLearnerSession();
            ShowPage(tabHome);
        }

        /// <summary>After a completed export: back to UI 1 with the session cleared, without asking.</summary>
        private void FinishSession()
        {
            ResetLearnerSession();
            ShowPage(tabHome);
        }

        /// <summary>UI 6, 6.1a or 6.2b → UI 7. Back on UI 7 returns to the screen that opened it.</summary>
        private void OpenSummary(TabPage from)
        {
            summaryReturnPage = from;
            GoToStep(tabSummary, 6);
        }

        #endregion

        #region Page switching

        /// <summary>True for every page that belongs to the learner session (everything but UI 1).</summary>
        private bool IsLearnerPage(TabPage page)
        {
            return page != null && page != tabHome;
        }

        /// <summary>The page currently on screen (a results sub-tab when the results group is showing).</summary>
        private TabPage CurrentPage
        {
            get { return tabMain.SelectedTab == tabResults ? tabResultsGroup.SelectedTab : tabMain.SelectedTab; }
        }

        /// <summary>Moves forward to a step and records it as reached, which unlocks it on the rail.</summary>
        private void GoToStep(TabPage page, int step)
        {
            if (step > highestStepReached)
            {
                highestStepReached = step;
            }

            ShowPage(page);
        }

        /// <summary>
        /// Shows one screen: selects its TabPage, then sets the caption, title band, rail progress,
        /// context strip, notice panel, action-band buttons and status band (spec 2.2.1–2.2.3), and finally
        /// lets the screen refresh itself through its <c>…Shown()</c> method.
        /// </summary>
        private void ShowPage(TabPage page)
        {
            changingPage = true;
            if (page == tabOutcome || page == tabAlternatives || page == tabFunding)
            {
                tabMain.SelectedTab = tabResults;
                tabResultsGroup.SelectedTab = page;
            }
            else
            {
                tabMain.SelectedTab = page;
            }

            changingPage = false;

            string title, subtitle, hint;
            int railStep;
            if (page == tabHome)
            {
                title = "Welcome to Future Path";
                subtitle = "Degree, alternative programme and funding guidance for Grade 11 and 12 learners";
                hint = "Preliminary guidance only — not an admission decision.";
                railStep = 0;
                ShowButtons();
            }
            else if (page == tabProfile)
            {
                title = "Learner profile";
                subtitle = "Step 1 of 6 · Tell us who this advisory session is for";
                hint = "Step 1 of 6 · Your session is completed when you export the advisory summary.";
                railStep = 1;
                PrimaryText("Save and continue");
                ShowButtons(btnPrimary, btnBack, btnCancel);
            }
            else if (page == tabSubjects)
            {
                title = "Subjects and marks";
                subtitle = "Step 2 of 6 · Enter every subject on your latest report, then continue";
                hint = "Step 2 of 6 · Add each subject, then save and continue.";
                railStep = 2;
                PrimaryText("Save and continue");
                ShowButtons(btnPrimary, btnBack, btnCancel);
            }
            else if (page == tabInterests)
            {
                title = "Interest questionnaire";
                subtitle = "Step 3 of 6 · Answer all statements to build your interest profile";
                hint = "Step 3 of 6 · Save and continue opens once every statement is answered.";
                railStep = 3;
                PrimaryText("Save and continue");
                ShowButtons(btnPrimary, btnBack, btnCancel);
            }
            else if (page == tabFaculty)
            {
                title = "Faculty and department";
                subtitle = "Step 4 of 6 · Choose the qualification you want checked";
                hint = "Step 4 of 6 · Select a programme, then check eligibility.";
                railStep = 4;
                PrimaryText("Check eligibility");
                ShowButtons(btnPrimary, btnBack, btnCancel);
            }
            else if (page == tabOutcome)
            {
                title = "Eligibility results";
                subtitle = "Step 5 of 6 · Your advisory outcome for the selected qualification";
                hint = "Step 5 of 6";
                railStep = 5;
                PrimaryText("Continue to summary");
                ShowButtons(btnPrimary, btnAlternatives, btnFunding, btnBack);
            }
            else if (page == tabAlternatives)
            {
                title = "Alternative programmes";
                subtitle = "Step 5 of 6 · Programmes that may suit your current subjects and marks better";
                hint = "Step 5 of 6";
                railStep = 5;
                PrimaryText("Continue to summary");
                ShowButtons(btnPrimary, btnCompare, btnBack);
            }
            else if (page == tabFunding)
            {
                title = "Funding options";
                subtitle = "Step 5 of 6 · Funding linked to your selected faculty and qualification";
                hint = "Step 5 of 6";
                railStep = 5;
                PrimaryText("Add to Summary");
                ShowButtons(btnPrimary, btnBack);
            }
            else
            {
                title = "Advisory summary";
                subtitle = "Step 6 of 6 · Check the summary, then save or print it";
                hint = "Step 6 of 6 · You may export your report.";
                railStep = 6;
                PrimaryText("Save and export");
                ShowButtons(btnPrimary, btnBack, btnCancel);
            }

            Text = "Future Path — " + (page == tabHome ? "Homepage" : title);
            lblTitle.Text = title;
            lblSubtitle.Text = subtitle;
            lblActionHint.Text = hint;
            lblBadge.Visible = IsLearnerPage(page);
            UpdateRail(railStep);

            RefreshContextStrip();
            HideNotice();
            SetStatus(page == tabHome ? "Choose how you want to use Future Path." : "Step " + railStep + " of 6",
                      DeepNavy, Color.White);

            if (page == tabHome) HomeShown();
            else if (page == tabProfile) ProfileShown();
            else if (page == tabSubjects) SubjectsShown();
            else if (page == tabInterests) InterestsShown();
            else if (page == tabFaculty) FacultyShown();
            else if (page == tabOutcome) OutcomeShown();
            else if (page == tabAlternatives) AlternativesShown();
            else if (page == tabFunding) FundingShown();
            else SummaryShown();
        }

        /// <summary>The hidden top-level tab strip must never change by keyboard (Ctrl+Tab): only code moves steps.</summary>
        private void tabMain_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (!changingPage)
            {
                e.Cancel = true;
            }
        }

        /// <summary>The visible results tabs (UI 6 / 6.1a / 6.2b) refresh the shell when the learner clicks one.</summary>
        private void tabResultsGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!changingPage && tabMain.SelectedTab == tabResults && tabResultsGroup.SelectedTab != null)
            {
                ShowPage(tabResultsGroup.SelectedTab);
            }
        }

        #endregion

        #region Rail

        /// <summary>
        /// Shows the six steps on learner pages (hidden on UI 1), highlights the active one and unlocks
        /// every step reached so far (spec 2.2.2: the rail shows progress).
        /// </summary>
        private void UpdateRail(int activeStep)
        {
            bool learner = activeStep > 0;
            btnNavHome.Visible = learner;
            lblRailFooter.Text = learner ? "Session" + Environment.NewLine + "Self-service"
                                         : "Works offline" + Environment.NewLine + "Local data only";
            for (int i = 0; i < railSteps.Length; i++)
            {
                int step = i + 1;
                bool active = step == activeStep;
                bool reached = step <= highestStepReached && !summaryExported;   // a completed session is locked
                railSteps[i].Visible = learner;
                railSteps[i].BackColor = active ? RailActive : NavyColour;
                railSteps[i].ForeColor = active ? Color.White : (reached ? RailText : RailLocked);
                railSteps[i].Font = active ? RailActiveFont : RailFont;
                railSteps[i].Cursor = reached && !active ? Cursors.Hand : Cursors.Default;
            }
        }

        /// <summary>A step already reached can be clicked on the rail to go straight back to it.</summary>
        private void lblStep_Click(object sender, EventArgs e)
        {
            int step = Array.IndexOf(railSteps, sender) + 1;
            if (summaryExported || step < 1 || step > highestStepReached || step == RailStepOf(CurrentPage))
            {
                return;
            }

            TabPage[] pages = { tabProfile, tabSubjects, tabInterests, tabFaculty, tabOutcome, tabSummary };
            ShowPage(pages[step - 1]);
        }

        /// <summary>The rail step (1–6) a page belongs to; 0 for the homepage.</summary>
        private int RailStepOf(TabPage page)
        {
            if (page == tabProfile) return 1;
            if (page == tabSubjects) return 2;
            if (page == tabInterests) return 3;
            if (page == tabFaculty) return 4;
            if (page == tabSummary) return 6;
            return page == tabHome ? 0 : 5;
        }

        /// <summary>Home on the rail leaves the learner session after confirmation.</summary>
        private void btnNavHome_Click(object sender, EventArgs e)
        {
            LeaveSession();
        }

        #endregion

        #region Action band

        /// <summary>
        /// Shows only the given action-band buttons (they flow right to left: primary, then Back, then
        /// Cancel) and gives the hint label whatever width they leave.
        /// </summary>
        private void ShowButtons(params Button[] visibleButtons)
        {
            int used = flpActions.Padding.Horizontal;
            foreach (Control control in flpActions.Controls)
            {
                bool show = Array.IndexOf(visibleButtons, control) >= 0;
                control.Visible = show;
                if (show)
                {
                    used += control.Width + control.Margin.Horizontal;
                }
            }

            lblActionHint.Width = Math.Max(0, pnlActions.ClientSize.Width - used);
        }

        /// <summary>Sets the primary button's label and re-enables it (a screen may disable it again in its Shown method).</summary>
        private void PrimaryText(string text)
        {
            btnPrimary.Text = text;
            btnPrimary.AccessibleName = text;
            SetPrimaryEnabled(true);
        }

        /// <summary>Enables or disables the primary button; disabled shows the rule grey (spec 2.2.3).</summary>
        private void SetPrimaryEnabled(bool enabled)
        {
            btnPrimary.Enabled = enabled;
            btnPrimary.BackColor = enabled ? NavyColour : RuleColour;
        }

        /// <summary>The primary (navy, AcceptButton) action of whichever screen is showing.</summary>
        private void btnPrimary_Click(object sender, EventArgs e)
        {
            TabPage page = CurrentPage;
            if (page == tabProfile) ProfilePrimary();
            else if (page == tabSubjects) SubjectsPrimary();
            else if (page == tabInterests) InterestsPrimary();
            else if (page == tabFaculty) FacultyPrimary();
            else if (page == tabOutcome) OutcomePrimary();
            else if (page == tabAlternatives) AlternativesPrimary();
            else if (page == tabFunding) FundingPrimary();
            else if (page == tabSummary) SummaryPrimary();
        }

        /// <summary>Back returns to the previous screen and keeps everything captured (spec 2.2.2).</summary>
        private void btnBack_Click(object sender, EventArgs e)
        {
            TabPage page = CurrentPage;
            if (page == tabProfile) LeaveSession();
            else if (page == tabSubjects) ShowPage(tabProfile);
            else if (page == tabInterests) ShowPage(tabSubjects);
            else if (page == tabFaculty) ShowPage(tabInterests);
            else if (page == tabOutcome) ShowPage(tabFaculty);
            else if (page == tabAlternatives || page == tabFunding) ShowPage(tabOutcome);
            else if (page == tabSummary) ShowPage(summaryReturnPage ?? tabOutcome);
        }

        /// <summary>Cancel abandons the session after confirmation.</summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (IsLearnerPage(CurrentPage) && btnCancel.Visible)
            {
                LeaveSession();
            }
        }

        /// <summary>UI 6 → UI 6.1a (Sequence 2 runs when the page is shown).</summary>
        private void btnAlternatives_Click(object sender, EventArgs e)
        {
            ShowPage(tabAlternatives);
        }

        /// <summary>UI 6 → UI 6.2b, the learner's yes/no choice to view funding (spec Section 7).</summary>
        private void btnFunding_Click(object sender, EventArgs e)
        {
            ShowPage(tabFunding);
        }

        /// <summary>UI 6.1a "Compare with my selection".</summary>
        private void btnCompare_Click(object sender, EventArgs e)
        {
            AlternativesCompare();
        }

        #endregion

        #region Shared zone helpers (used by every screen's partial file)

        /// <summary>Shows a message in the status band (never a modal; spec 2.2.7).</summary>
        private void SetStatus(string message, Color back, Color fore)
        {
            pnlStatus.BackColor = back;
            lblStatus.ForeColor = fore;
            lblStatus.Text = message;
        }

        /// <summary>Shows the notice panel: blue guidance, amber outstanding, red invalid, green go-on.</summary>
        private void ShowNotice(string message, Color back, Color bar, Color fore)
        {
            lblNotice.Text = message;
            pnlNotice.BackColor = back;
            pnlNoticeBar.BackColor = bar;
            lblNotice.ForeColor = fore;
            pnlNotice.Visible = pnlNoticeGap.Visible = true;
        }

        /// <summary>Hides the notice panel (conditional zones are hidden, never shown empty).</summary>
        private void HideNotice()
        {
            pnlNotice.Visible = pnlNoticeGap.Visible = false;
        }

        /// <summary>Shows the learner context strip on UI 3 onward, once UI 2 has been saved.</summary>
        private void RefreshContextStrip()
        {
            TabPage page = CurrentPage;
            bool show = IsLearnerPage(page) && page != tabProfile && !string.IsNullOrEmpty(LearnerContext);
            pnlContext.Visible = pnlContextGap.Visible = show;
            lblContext.Text = LearnerContext;
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

        /// <summary>The caption-bar X closes Future Path, so it asks first.</summary>
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
