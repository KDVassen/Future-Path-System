using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 13 — Reference Data (wide, Administrators only; spec 2.3.11 and design notes UI 13.1–13.6).
    /// A TabControl fills the content area with six tab pages, each hosting one maintenance tab:
    /// <see cref="UniversityInfoTab"/> (13.1), <see cref="DegreeRequirementsTab"/> (13.2),
    /// <see cref="FundingTab"/> (13.3), <see cref="InterestCategoriesTab"/> (13.4),
    /// <see cref="QuestionnaireTab"/> (13.5) and <see cref="UserAccountsTab"/> (13.6).
    /// The action band acts on the active tab: <b>Add &lt;item&gt;</b> (primary), <b>Update &lt;item&gt;</b>,
    /// <b>Deactivate</b> and <b>Cancel</b>. The status band reminds the administrator that changes apply
    /// to new advisory sessions only, and relays each tab's messages.
    /// <para>
    /// Role check (defence in depth, Section 8): if the role passed in is not "Administrator", nothing
    /// editable is shown and the screen returns to the user's own dashboard. The BLL checks the role
    /// again on every save.
    /// </para>
    /// </summary>
    public partial class ReferenceDataForm : Form
    {
        // Colours used at runtime (spec 2.2.6). Everything else is set in the Designer.
        private static readonly Color InkColour = Color.FromArgb(27, 31, 36);
        private static readonly Color BlueColour = Color.FromArgb(31, 95, 168);
        private static readonly Color AmberColour = Color.FromArgb(200, 137, 43);
        private static readonly Color DeepNavy = Color.FromArgb(11, 31, 58);

        private const string AdministratorRole = "Administrator";
        private const string CareerAdvisorRole = "Career Advisor";
        private const string ChangesNote = "Changes apply to new advisory sessions only";

        private readonly Form homeForm;
        private readonly int userId;
        private readonly string fullName;
        private readonly string role;

        /// <summary>The operations of each tab page, so the action band can call the active tab.</summary>
        private readonly Dictionary<TabPage, TabHooks> hooks = new Dictionary<TabPage, TabHooks>();

        /// <summary>Set when this screen closes to open another one, so FormClosing does not ask to quit.</summary>
        private bool navigating;

        /// <summary>Creates the reference data screen for a signed-in staff member.</summary>
        /// <param name="homeForm">UI 1, kept alive (hidden) and shown again on sign out.</param>
        /// <param name="userId">UserAccount.UserID of the signed-in user.</param>
        /// <param name="fullName">UserAccount.FullName, shown in the rail footer.</param>
        /// <param name="role">UserAccount.Role: "Administrator" or "Career Advisor".</param>
        public ReferenceDataForm(Form homeForm, int userId, string fullName, string role)
        {
            InitializeComponent();
            this.homeForm = homeForm;
            this.userId = userId;
            this.fullName = fullName;
            this.role = role;

            // Rail built from the role: Sessions for Career Advisors, Reference data for Administrators.
            btnNavSessions.Visible = !IsAdministrator;
            btnNavReferenceData.Visible = IsAdministrator;
            btnNavReferenceData.BringToFront();
            lblRailFooter.Text = "Signed in" + Environment.NewLine + fullName + Environment.NewLine + role;
            lblBadge.Text = IsAdministrator ? "ADMINISTRATOR" : "CAREER ADVISOR";

            RegisterTab(tabUniversity, ucUniversityInfo.LoadData, () => ucUniversityInfo.ItemNoun, ucUniversityInfo.AddNew,
                ucUniversityInfo.UpdateSelected, ucUniversityInfo.DeactivateSelected, ucUniversityInfo.CancelEdit);
            RegisterTab(tabRequirements, ucDegreeRequirements.LoadData, () => ucDegreeRequirements.ItemNoun, ucDegreeRequirements.AddNew,
                ucDegreeRequirements.UpdateSelected, ucDegreeRequirements.DeactivateSelected, ucDegreeRequirements.CancelEdit);
            RegisterTab(tabFunding, ucFunding.LoadData, () => ucFunding.ItemNoun, ucFunding.AddNew,
                ucFunding.UpdateSelected, ucFunding.DeactivateSelected, ucFunding.CancelEdit);
            RegisterTab(tabInterests, ucInterestCategories.LoadData, () => ucInterestCategories.ItemNoun, ucInterestCategories.AddNew,
                ucInterestCategories.UpdateSelected, ucInterestCategories.DeactivateSelected, ucInterestCategories.CancelEdit);
            RegisterTab(tabQuestionnaire, ucQuestionnaire.LoadData, () => ucQuestionnaire.ItemNoun, ucQuestionnaire.AddNew,
                ucQuestionnaire.UpdateSelected, ucQuestionnaire.DeactivateSelected, ucQuestionnaire.CancelEdit);
            RegisterTab(tabUserAccounts, ucUserAccounts.LoadData, () => ucUserAccounts.ItemNoun, ucUserAccounts.AddNew,
                ucUserAccounts.UpdateSelected, ucUserAccounts.DeactivateSelected, ucUserAccounts.CancelEdit);

            ucUniversityInfo.UserId = userId; ucUniversityInfo.Role = role;
            ucDegreeRequirements.UserId = userId; ucDegreeRequirements.Role = role;
            ucFunding.UserId = userId; ucFunding.Role = role;
            ucInterestCategories.UserId = userId; ucInterestCategories.Role = role;
            ucQuestionnaire.UserId = userId; ucQuestionnaire.Role = role;
            ucUserAccounts.UserId = userId; ucUserAccounts.Role = role;

            ucUniversityInfo.StatusChanged += Tab_StatusChanged;
            ucDegreeRequirements.StatusChanged += Tab_StatusChanged;
            ucFunding.StatusChanged += Tab_StatusChanged;
            ucInterestCategories.StatusChanged += Tab_StatusChanged;
            ucQuestionnaire.StatusChanged += Tab_StatusChanged;
            ucUserAccounts.StatusChanged += Tab_StatusChanged;

            if (IsAdministrator)
            {
                UpdateActionButtons();
                SetStatus("Signed in as " + fullName + " (" + role + ") · " + ChangesNote, DeepNavy, Color.White);
            }
            else
            {
                // Defence in depth: nothing editable is ever rendered for anyone but an Administrator.
                tabReference.Visible = false;
                btnAdd.Enabled = false;
                btnUpdate.Enabled = false;
                btnDeactivate.Enabled = false;
                btnCancel.Enabled = false;
                SetStatus("Reference data is maintained by Administrators only. Returning to your dashboard.", AmberColour, InkColour);
            }
        }

        private bool IsAdministrator
        {
            get { return string.Equals(role, AdministratorRole, StringComparison.Ordinal); }
        }

        #region Tabs and action band

        /// <summary>A tab page's operations, captured once so the action band can call whichever tab is active.</summary>
        private sealed class TabHooks
        {
            public Action LoadData;
            public Func<string> ItemNoun;
            public Action AddNew;
            public Action UpdateSelected;
            public Action DeactivateSelected;
            public Action CancelEdit;
            public bool Loaded;
        }

        private void RegisterTab(TabPage page, Action loadData, Func<string> itemNoun, Action addNew,
            Action updateSelected, Action deactivateSelected, Action cancelEdit)
        {
            hooks[page] = new TabHooks
            {
                LoadData = loadData, ItemNoun = itemNoun, AddNew = addNew, UpdateSelected = updateSelected,
                DeactivateSelected = deactivateSelected, CancelEdit = cancelEdit
            };
        }

        /// <summary>The operations of the active tab, or null when none is selected.</summary>
        private TabHooks ActiveTab
        {
            get
            {
                TabHooks active;
                return tabReference.SelectedTab != null && hooks.TryGetValue(tabReference.SelectedTab, out active) ? active : null;
            }
        }

        /// <summary>Loads the active tab's data the first time it is shown.</summary>
        private void EnsureActiveTabLoaded()
        {
            TabHooks active = ActiveTab;
            if (active == null || active.Loaded)
            {
                return;
            }

            active.Loaded = true;
            try
            {
                active.LoadData();
            }
            catch (Exception)
            {
                active.Loaded = false;
                SetStatus("This tab's data could not be loaded. Please try again. · " + ChangesNote, AmberColour, InkColour);
            }
        }

        /// <summary>Puts the active tab's item noun on the Add and Update buttons.</summary>
        private void UpdateActionButtons()
        {
            TabHooks active = ActiveTab;
            string noun = active == null ? "item" : active.ItemNoun();
            btnAdd.Text = "Add " + noun;
            btnAdd.AccessibleName = btnAdd.Text;
            btnUpdate.Text = "Update " + noun;
            btnUpdate.AccessibleName = btnUpdate.Text;
        }

        private void tabReference_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsAdministrator)
            {
                return;
            }

            UpdateActionButtons();
            SetStatus("Signed in as " + fullName + " (" + role + ") · " + ChangesNote, DeepNavy, Color.White);
            EnsureActiveTabLoaded();
        }

        /// <summary>A tab's status message: amber for a warning, blue otherwise, always with the changes note.</summary>
        private void Tab_StatusChanged(string message, bool isWarning)
        {
            SetStatus(message + " · " + ChangesNote, isWarning ? AmberColour : BlueColour, isWarning ? InkColour : Color.White);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            RunOnActiveTab(delegate(TabHooks t) { t.AddNew(); });
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            RunOnActiveTab(delegate(TabHooks t) { t.UpdateSelected(); });
        }

        private void btnDeactivate_Click(object sender, EventArgs e)
        {
            RunOnActiveTab(delegate(TabHooks t) { t.DeactivateSelected(); });
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            RunOnActiveTab(delegate(TabHooks t) { t.CancelEdit(); });
        }

        /// <summary>Runs an action-band command on the active tab (Administrators only).</summary>
        private void RunOnActiveTab(Action<TabHooks> command)
        {
            TabHooks active = ActiveTab;
            if (!IsAdministrator || active == null)
            {
                return;
            }

            EnsureActiveTabLoaded();
            command(active);
        }

        private void SetStatus(string message, Color back, Color fore)
        {
            pnlStatus.BackColor = back;
            lblStatus.ForeColor = fore;
            lblStatus.Text = message;
        }

        #endregion

        #region Rail navigation

        private void btnNavDashboard_Click(object sender, EventArgs e)
        {
            OpenRoleDashboard();
        }

        /// <summary>Only shown to Career Advisors; opens UI 10.</summary>
        private void btnNavSessions_Click(object sender, EventArgs e)
        {
            OpenScreen(new SessionsDashboardForm(homeForm, userId, fullName, role));
        }

        /// <summary>Already here: returns to the first tab.</summary>
        private void btnNavReferenceData_Click(object sender, EventArgs e)
        {
            if (IsAdministrator)
            {
                tabReference.SelectedIndex = 0;
            }
        }

        private void btnNavReports_Click(object sender, EventArgs e)
        {
            OpenScreen(new ReportGenerationForm(homeForm, userId, fullName, role));
        }

        /// <summary>Sign out: confirm, show UI 1 and close this screen.</summary>
        private void btnNavSignOut_Click(object sender, EventArgs e)
        {
            using (var dialog = new ConfirmationDialog("Sign out",
                "Sign out of Future Path? Anything you have not saved on this screen is discarded.", "Sign out"))
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

        /// <summary>UI 12 for Administrators, UI 9 for Career Advisors; UI 1 for any other role.</summary>
        private void OpenRoleDashboard()
        {
            if (IsAdministrator)
            {
                OpenScreen(new AdminDashboardForm(homeForm, userId, fullName, role));
            }
            else if (string.Equals(role, CareerAdvisorRole, StringComparison.Ordinal))
            {
                OpenScreen(new AdvisorDashboardForm(homeForm, userId, fullName, role));
            }
            else
            {
                homeForm.Show();
                navigating = true;
                Close();
            }
        }

        private void OpenScreen(Form next)
        {
            next.Show();
            navigating = true;
            Close();
        }

        #endregion

        #region Showing and closing

        /// <summary>Administrators get the first tab loaded; anyone else goes back to their own dashboard.</summary>
        private void ReferenceDataForm_Shown(object sender, EventArgs e)
        {
            if (!IsAdministrator)
            {
                OpenRoleDashboard();
                return;
            }

            // The TabControl only reports its selected page once its handle exists, so the button
            // texts are set again here.
            UpdateActionButtons();
            EnsureActiveTabLoaded();
        }

        /// <summary>The caption-bar X asks first, because it closes Future Path.</summary>
        private void ReferenceDataForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing || navigating || homeForm.Visible)
            {
                return;
            }

            using (var dialog = new ConfirmationDialog("Close Future Path",
                "Close Future Path? Anything you have not saved on this screen is discarded.", "Close"))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>Ends the application when this screen was closed with the caption-bar X.</summary>
        private void ReferenceDataForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!navigating && !homeForm.Visible)
            {
                Application.Exit();
            }
        }

        #endregion
    }
}
