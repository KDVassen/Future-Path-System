using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 9 — Advisor Dashboard (standard size, Career Advisor). The Career Advisor's hub, built to
    /// the hub pattern (screens-detail.md): a welcome line, one card per task with a button to
    /// UI 10 (Sessions dashboard) and UI 11 (Reports), and a read-only "Today" summary whose
    /// figures come from the BLL. The summary stays hidden until figures arrive, so it is never
    /// shown empty.
    /// </summary>
    public partial class AdvisorDashboardForm : Form
    {
        // Colours used at runtime (spec 2.2.6). Everything else is set in the Designer.
        private static readonly Color DeepNavy = Color.FromArgb(11, 31, 58);
        private static readonly Color RedColour = Color.FromArgb(164, 38, 44);

        private const string AdministratorRole = "Administrator";
        private const string CareerAdvisorRole = "Career Advisor";

        private readonly Form homeForm;
        private readonly int userId;
        private readonly string fullName;
        private readonly string role;
        private bool navigating;

        /// <summary>Creates the Career Advisor dashboard for the signed-in staff member.</summary>
        /// <param name="homeForm">UI 1, shown again on sign out.</param>
        /// <param name="userId">UserAccount.UserID of the signed-in user.</param>
        /// <param name="fullName">UserAccount.FullName, shown on the rail and in the status band.</param>
        /// <param name="role">UserAccount.Role: "Career Advisor" or "Administrator".</param>
        public AdvisorDashboardForm(Form homeForm, int userId, string fullName, string role)
        {
            InitializeComponent();
            this.homeForm = homeForm;
            this.userId = userId;
            this.fullName = fullName;
            this.role = role;

            // The rail is built from the role (Section 8): Sessions for Career Advisors, Reference data for Administrators.
            btnNavSessions.Visible = role == CareerAdvisorRole;
            btnNavReferenceData.Visible = role == AdministratorRole;
            lblRailFooter.Text = "Signed in" + Environment.NewLine + fullName + Environment.NewLine + role;
            lblWelcome.Text = "Welcome, " + fullName;
            SetStatus("Signed in as " + fullName + " (" + role + ")", DeepNavy);
            LoadTodaySummary();
        }

        #region Business Logic Layer hook

        /// <summary>
        /// Fills the read-only "Today" figures. The card stays hidden unless the BLL returns them.
        /// </summary>
        private void LoadTodaySummary()
        {
            pnlTodayCard.Visible = false;
            try
            {
                // TODO BLL: var today = SessionSearchController.GetDailySummary(DateTime.Today);
                //           // counts from AdvisorySession (SessionStatus, SessionDate, DateCompleted) and the selected
                //           // RecommendationResult (OutcomeCode NCE / MIN) — read back, never recalculated
                //           if (today == null) return;
                //           ShowFigure(tilCompletedValue, today.CompletedCount);
                //           ShowFigure(tilInProgressValue, today.InProgressCount);
                //           ShowFigure(tilFollowUpValue, today.ExceptionCount);
                //           pnlTodayCard.Visible = true;
            }
            catch (Exception)
            {
                SetStatus("Today's figures could not be loaded. Sessions and reports are still available.", RedColour);
            }
        }

        /// <summary>Writes one whole-number figure into its tile.</summary>
        private static void ShowFigure(Label valueLabel, int value)
        {
            valueLabel.Text = value.ToString("N0", CultureInfo.CurrentCulture);
        }

        #endregion

        #region Hub buttons

        /// <summary>Search advisory sessions → UI 10 Sessions dashboard.</summary>
        private void btnSearchSessions_Click(object sender, EventArgs e)
        {
            OpenSessions();
        }

        /// <summary>Generate reports → UI 11 Reports.</summary>
        private void btnGenerateReports_Click(object sender, EventArgs e)
        {
            OpenScreen(new ReportGenerationForm(homeForm, userId, fullName, role));
        }

        private void OpenSessions()
        {
            OpenScreen(new SessionsDashboardForm(homeForm, userId, fullName, role));
        }

        /// <summary>Shows the next staff screen and closes this one (staff navigation pattern).</summary>
        private void OpenScreen(Form next)
        {
            next.Show();
            navigating = true;
            Close();
        }

        #endregion

        #region Navigation rail

        /// <summary>Dashboard: this screen is already the dashboard.</summary>
        private void btnNavDashboard_Click(object sender, EventArgs e)
        {
            SetStatus("Signed in as " + fullName + " (" + role + ")", DeepNavy);
        }

        /// <summary>Sessions → UI 10.</summary>
        private void btnNavSessions_Click(object sender, EventArgs e)
        {
            OpenSessions();
        }

        /// <summary>Reference data → UI 13, for Administrators only (the button is hidden otherwise).</summary>
        private void btnNavReferenceData_Click(object sender, EventArgs e)
        {
            if (role != AdministratorRole)
            {
                return;
            }

            OpenScreen(new ReferenceDataForm(homeForm, userId, fullName, role));
        }

        /// <summary>Reports → UI 11.</summary>
        private void btnNavReports_Click(object sender, EventArgs e)
        {
            OpenScreen(new ReportGenerationForm(homeForm, userId, fullName, role));
        }

        /// <summary>Sign out → confirm, then back to UI 1.</summary>
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

        #endregion

        #region Status band and closing

        private void SetStatus(string message, Color back)
        {
            pnlStatus.BackColor = back;
            lblStatus.ForeColor = Color.White;
            lblStatus.Text = message;
        }

        /// <summary>The caption-bar X asks first, because it closes Future Path.</summary>
        private void AdvisorDashboardForm_FormClosing(object sender, FormClosingEventArgs e)
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
        private void AdvisorDashboardForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!navigating && !homeForm.Visible)
            {
                Application.Exit();
            }
        }

        #endregion
    }
}
