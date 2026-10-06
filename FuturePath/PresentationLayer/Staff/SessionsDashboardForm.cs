using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 10 — Sessions dashboard (wide, Career Advisor; spec 2.3.10).
    /// A hub, not a sequence: the advisor searches once and moves between three tabs —
    /// Learner profile (10.1a), Advisory session (10.2b, shown first) and Advisory reports (10.3c).
    /// The primary button always shows the active tab's action and runs it; Close returns to the
    /// role dashboard. Every tab reports to the status band (amber for a warning, blue otherwise),
    /// and Generate report on 10.3c opens Report generation (UI 11) already filtered.
    /// </summary>
    public partial class SessionsDashboardForm : Form
    {
        // Colours used at runtime (spec 2.2.6). Everything else is set in the Designer.
        private static readonly Color InkColour = Color.FromArgb(27, 31, 36);
        private static readonly Color DeepNavy = Color.FromArgb(11, 31, 58);
        private static readonly Color BlueColour = Color.FromArgb(31, 95, 168);
        private static readonly Color AmberColour = Color.FromArgb(200, 137, 43);

        private const string AdministratorRole = "Administrator";

        private readonly Form homeForm;
        private readonly int userId;
        private readonly string fullName;
        private readonly string role;
        private readonly HashSet<TabPage> loadedTabs = new HashSet<TabPage>();

        // True while moving to another staff screen, so closing this one does not end the application.
        private bool navigating;

        // True while the first tab loads, so its guidance does not replace "Signed in as ..." (warnings still show).
        private bool opening;

        /// <summary>Creates the Sessions dashboard for the signed-in staff member.</summary>
        /// <param name="homeForm">UI 1, shown again on sign out.</param>
        /// <param name="userId">UserAccount.UserID of the signed-in user.</param>
        /// <param name="fullName">UserAccount.FullName, shown on the rail and in the status band.</param>
        /// <param name="role">UserAccount.Role: "Administrator" or "Career Advisor".</param>
        public SessionsDashboardForm(Form homeForm, int userId, string fullName, string role)
        {
            InitializeComponent();
            this.homeForm = homeForm;
            this.userId = userId;
            this.fullName = fullName;
            this.role = role;

            bool isAdministrator = IsAdministrator;
            lblBadge.Text = isAdministrator ? "ADMINISTRATOR" : "CAREER ADVISOR";
            btnNavSessions.Visible = !isAdministrator;
            btnNavReferenceData.Visible = isAdministrator;
            lblRailFooter.Text = "Signed in" + Environment.NewLine + fullName + Environment.NewLine + role;

            learnerProfileTab.UserId = userId;
            learnerProfileTab.Role = role;
            advisorySessionTab.UserId = userId;
            advisorySessionTab.Role = role;
            advisoryReportsTab.UserId = userId;
            advisoryReportsTab.Role = role;

            learnerProfileTab.StatusChanged += Tab_StatusChanged;
            advisorySessionTab.StatusChanged += Tab_StatusChanged;
            advisoryReportsTab.StatusChanged += Tab_StatusChanged;
            learnerProfileTab.LearnerSessionsRequested += LearnerProfileTab_LearnerSessionsRequested;
            advisoryReportsTab.GenerateReportRequested += AdvisoryReportsTab_GenerateReportRequested;

            ShowSignedIn();
        }

        private bool IsAdministrator
        {
            get { return string.Equals(role, AdministratorRole, StringComparison.OrdinalIgnoreCase); }
        }

        #region Tabs

        /// <summary>Opens on the Advisory session tab (10.2b), as in the wireframe.</summary>
        private void SessionsDashboardForm_Load(object sender, EventArgs e)
        {
            opening = true;
            if (tabSessions.SelectedTab == tpAdvisorySession)
            {
                ActivateTab();
            }
            else
            {
                tabSessions.SelectedTab = tpAdvisorySession; // raises SelectedIndexChanged → ActivateTab
            }

            opening = false;
        }

        private void tabSessions_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActivateTab();
        }

        /// <summary>
        /// Loads the active tab the first time it is shown and puts its action on the primary button.
        /// </summary>
        private void ActivateTab()
        {
            TabPage page = tabSessions.SelectedTab;
            if (page == null)
            {
                return;
            }

            string actionText = PrimaryActionText(page);
            btnPrimaryAction.Text = actionText;
            btnPrimaryAction.AccessibleName = actionText;

            if (loadedTabs.Add(page))
            {
                if (page == tpLearnerProfile) learnerProfileTab.LoadData();
                else if (page == tpAdvisorySession) advisorySessionTab.LoadData();
                else if (page == tpAdvisoryReports) advisoryReportsTab.LoadData();
            }
        }

        private string PrimaryActionText(TabPage page)
        {
            if (page == tpLearnerProfile) return learnerProfileTab.PrimaryActionText;
            if (page == tpAdvisoryReports) return advisoryReportsTab.PrimaryActionText;
            return advisorySessionTab.PrimaryActionText;
        }

        /// <summary>The primary button runs the active tab's action.</summary>
        private void btnPrimaryAction_Click(object sender, EventArgs e)
        {
            TabPage page = tabSessions.SelectedTab;
            if (page == tpLearnerProfile) learnerProfileTab.RunPrimaryAction();
            else if (page == tpAdvisoryReports) advisoryReportsTab.RunPrimaryAction();
            else advisorySessionTab.RunPrimaryAction();
        }

        /// <summary>A tab's status message: amber (ink text) for a warning, blue (white text) otherwise.</summary>
        private void Tab_StatusChanged(string message, bool isWarning)
        {
            if (opening && !isWarning)
            {
                return;
            }

            SetStatus(message, isWarning ? AmberColour : BlueColour, isWarning ? InkColour : Color.White);
        }

        /// <summary>Open learner profile (10.1a) → the Advisory session tab, filtered to that learner.</summary>
        private void LearnerProfileTab_LearnerSessionsRequested(int learnerId, string learnerName)
        {
            tabSessions.SelectedTab = tpAdvisorySession;
            advisorySessionTab.ShowLearnerSessions(learnerId, learnerName);
        }

        /// <summary>Generate report (10.3c) → Report generation (UI 11) with the tab's criteria applied.</summary>
        private void AdvisoryReportsTab_GenerateReportRequested(string reportCode, DateTime from, DateTime to,
            int? facultyId, int? gradeLevelId, string outcome)
        {
            var reports = new ReportGenerationForm(homeForm, userId, fullName, role);
            reports.ApplyFilter(reportCode, from, to, facultyId, gradeLevelId, outcome);
            reports.Show();
            navigating = true;
            Close();
        }

        #endregion

        #region Status band

        private void ShowSignedIn()
        {
            SetStatus("Signed in as " + fullName + " (" + role + ")", DeepNavy, Color.White);
        }

        private void SetStatus(string message, Color back, Color fore)
        {
            pnlStatus.BackColor = back;
            lblStatus.ForeColor = fore;
            lblStatus.Text = message;
        }

        #endregion

        #region Rail and navigation

        /// <summary>Close returns to the role dashboard.</summary>
        private void btnClose_Click(object sender, EventArgs e)
        {
            OpenDashboard();
        }

        private void btnNavDashboard_Click(object sender, EventArgs e)
        {
            OpenDashboard();
        }

        /// <summary>Already on the Sessions dashboard: return to its first view.</summary>
        private void btnNavSessions_Click(object sender, EventArgs e)
        {
            tabSessions.SelectedTab = tpAdvisorySession;
        }

        /// <summary>Reference data (UI 13) is for Administrators only.</summary>
        private void btnNavReferenceData_Click(object sender, EventArgs e)
        {
            if (!IsAdministrator)
            {
                return;
            }

            NavigateTo(new ReferenceDataForm(homeForm, userId, fullName, role));
        }

        private void btnNavReports_Click(object sender, EventArgs e)
        {
            NavigateTo(new ReportGenerationForm(homeForm, userId, fullName, role));
        }

        /// <summary>Sign out asks first, then returns to UI 1.</summary>
        private void btnNavSignOut_Click(object sender, EventArgs e)
        {
            using (var dialog = new ConfirmationDialog("Sign out", "Sign out of Future Path and return to the homepage?", "Sign out"))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }
            }

            homeForm.Show();
            navigating = true;
            Close();
        }

        /// <summary>Opens the dashboard for this role: UI 12 for Administrators, UI 9 for Career Advisors.</summary>
        private void OpenDashboard()
        {
            if (IsAdministrator)
            {
                NavigateTo(new AdminDashboardForm(homeForm, userId, fullName, role));
            }
            else
            {
                NavigateTo(new AdvisorDashboardForm(homeForm, userId, fullName, role));
            }
        }

        private void NavigateTo(Form next)
        {
            next.Show();
            navigating = true;
            Close();
        }

        #endregion

        #region Closing

        /// <summary>The caption-bar X asks first, because it closes Future Path.</summary>
        private void SessionsDashboardForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing || navigating || homeForm.Visible)
            {
                return;
            }

            using (var dialog = new ConfirmationDialog("Close Future Path", "Close Future Path? You will be signed out.", "Close"))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>Ends the application when this screen was closed with the caption-bar X.</summary>
        private void SessionsDashboardForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!navigating && !homeForm.Visible)
            {
                Application.Exit();
            }
        }

        #endregion
    }
}
