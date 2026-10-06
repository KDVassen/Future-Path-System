using System;
using System.Drawing;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 12 — Administrator Dashboard (standard size, Administrator). The System Administrator's
    /// hub, built to the hub pattern (screens-detail.md): a welcome line and one card per task with
    /// a button to UI 13 (Reference data) and UI 11 (Reports). Reference data is offered only when
    /// the signed-in role is Administrator; the BLL checks the role again on every change (Section 8).
    /// </summary>
    public partial class AdminDashboardForm : Form
    {
        // Colours used at runtime (spec 2.2.6). Everything else is set in the Designer.
        private static readonly Color DeepNavy = Color.FromArgb(11, 31, 58);

        private const string AdministratorRole = "Administrator";
        private const string CareerAdvisorRole = "Career Advisor";

        private readonly Form homeForm;
        private readonly int userId;
        private readonly string fullName;
        private readonly string role;
        private bool navigating;

        /// <summary>Creates the Administrator dashboard for the signed-in staff member.</summary>
        /// <param name="homeForm">UI 1, shown again on sign out.</param>
        /// <param name="userId">UserAccount.UserID of the signed-in user.</param>
        /// <param name="fullName">UserAccount.FullName, shown on the rail and in the status band.</param>
        /// <param name="role">UserAccount.Role: "Administrator" or "Career Advisor".</param>
        public AdminDashboardForm(Form homeForm, int userId, string fullName, string role)
        {
            InitializeComponent();
            this.homeForm = homeForm;
            this.userId = userId;
            this.fullName = fullName;
            this.role = role;

            // The rail is built from the role (Section 8): Sessions for Career Advisors, Reference data for Administrators.
            btnNavSessions.Visible = role == CareerAdvisorRole;
            btnNavReferenceData.Visible = role == AdministratorRole;
            btnMaintainReferenceData.Enabled = role == AdministratorRole;
            lblRailFooter.Text = "Signed in" + Environment.NewLine + fullName + Environment.NewLine + role;
            lblWelcome.Text = "Welcome, " + fullName;
            SetStatus("Signed in as " + fullName + " (" + role + ")");
        }

        #region Hub buttons

        /// <summary>Maintain reference data → UI 13 (Administrators only).</summary>
        private void btnMaintainReferenceData_Click(object sender, EventArgs e)
        {
            OpenReferenceData();
        }

        /// <summary>Generate reports → UI 11 Reports.</summary>
        private void btnGenerateReports_Click(object sender, EventArgs e)
        {
            OpenScreen(new ReportGenerationForm(homeForm, userId, fullName, role));
        }

        /// <summary>Opens UI 13 only for an Administrator; a Career Advisor never reaches it.</summary>
        private void OpenReferenceData()
        {
            if (role != AdministratorRole)
            {
                return;
            }

            OpenScreen(new ReferenceDataForm(homeForm, userId, fullName, role));
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
            SetStatus("Signed in as " + fullName + " (" + role + ")");
        }

        /// <summary>Sessions → UI 10, for Career Advisors only (the button is hidden otherwise).</summary>
        private void btnNavSessions_Click(object sender, EventArgs e)
        {
            if (role != CareerAdvisorRole)
            {
                return;
            }

            OpenScreen(new SessionsDashboardForm(homeForm, userId, fullName, role));
        }

        /// <summary>Reference data → UI 13.</summary>
        private void btnNavReferenceData_Click(object sender, EventArgs e)
        {
            OpenReferenceData();
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

        private void SetStatus(string message)
        {
            pnlStatus.BackColor = DeepNavy;
            lblStatus.ForeColor = Color.White;
            lblStatus.Text = message;
        }

        /// <summary>The caption-bar X asks first, because it closes Future Path.</summary>
        private void AdminDashboardForm_FormClosing(object sender, FormClosingEventArgs e)
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
        private void AdminDashboardForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!navigating && !homeForm.Visible)
            {
                Application.Exit();
            }
        }

        #endregion
    }
}
