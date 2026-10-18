
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using FuturePath.ApplicationLayer;

namespace FuturePath.PresentationLayer
{
    // Main window for staff dashboards, sessions, reports and reference data
    // Handles role-based access, navigation and shared UI behaviour
    public partial class StaffConsoleForm : Form
    {
        #region Palette (spec 2.2.6) — runtime colours and fonts; everything else is set in the Designer

        private static readonly Color DeepNavy = Color.FromArgb(11, 31, 58);
        private static readonly Color InkColour = Color.FromArgb(27, 31, 36);
        private static readonly Color MutedColour = Color.FromArgb(88, 96, 108);
        private static readonly Color GreyTint = Color.FromArgb(236, 238, 241);
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

        private const string ReferenceNote = "Changes apply to new advisory sessions only";

        private readonly Form mainForm;
        private readonly HashSet<TabPage> shownPages = new HashSet<TabPage>();
        private bool navigating;

        // Initialise the staff console with the signed-in user's details
        public StaffConsoleForm(Form mainForm, int userId, string fullName, string role)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            UserId = userId;
            FullName = fullName;
            Role = role;
            tcSessions.SelectedTab = tabSessionSearch;      // UI 10 opens on 10.2b, as in its wireframe
            tcReference.SelectedTab = tabRequirements;      // UI 13 opens on 13.2, as in its wireframe
            numMinimumAverage.KeyPress += DecimalBox_KeyPress;
            numMinimumMark.KeyPress += DecimalBox_KeyPress;
            numRelevanceWeight.KeyPress += DecimalBox_KeyPress;
            ApplyRole();
            OpenDashboard();
            PageShown(CurrentPage);   // Show the initial page before tab events begin
        }

        #region The signed-in staff member

        // ID of the signed-in staff member
        private int UserId { get; set; }

        // Staff member's name displayed in the status bar
        private string FullName { get; set; }

        // Role of the signed-in staff member
        private string Role { get; set; }

        // Check whether the signed-in user is an administrator
        private bool IsAdministrator
        {
            get { return Role == UserAccount.RoleAdministratorText; }
        }

        // Remove pages that the signed-in role is not allowed to access
        // Display the staff member's name and role
        private void ApplyRole()
        {
            if (IsAdministrator)
            {
                tcConsole.TabPages.Remove(tabAdvisorDashboard);
            }
            else
            {
                tcConsole.TabPages.Remove(tabAdminDashboard);
                tcConsole.TabPages.Remove(tabReference);
            }

            lblSignedIn.Text = "Signed in as " + FullName + " (" + Role + ")";
        }

        #endregion

        #region Tab navigation

        // Get the current screen, including nested session and reference tabs
        private TabPage CurrentPage
        {
            get
            {
                TabPage top = tcConsole.SelectedTab;
                if (top == tabSessions) return tcSessions.SelectedTab;
                if (top == tabReference) return tcReference.SelectedTab;
                return top;
            }
        }

        // Check whether a page belongs to the Sessions section
        private bool IsSessionsPage(TabPage page)
        {
            return page == tabLearnerSearch || page == tabSessionSearch || page == tabReportShortcuts;
        }

        // Check whether a page belongs to the Reference Data section
        private bool IsReferencePage(TabPage page)
        {
            return page == tabUniversity || page == tabRequirements || page == tabFundingAdmin
                   || page == tabCategories || page == tabQuestionnaire || page == tabAccounts;
        }

        // Open the dashboard for the signed-in user's role
        private void OpenDashboard()
        {
            tcConsole.SelectedTab = IsAdministrator ? tabAdminDashboard : tabAdvisorDashboard;
        }

        // Navigate to a page or a nested Sessions or Reference Data tab
        // Ignore pages that are unavailable to the signed-in role
        private void SelectPage(TabPage page)
        {
            TabPage parent = IsSessionsPage(page) ? tabSessions : IsReferencePage(page) ? tabReference : page;
            if (!tcConsole.TabPages.Contains(parent))
            {
                return;
            }

            if (IsSessionsPage(page)) tcSessions.SelectedTab = page;
            else if (IsReferencePage(page)) tcReference.SelectedTab = page;
            tcConsole.SelectedTab = parent;
        }

        // Open the Reports screen with the selected filters applied
        private void OpenReports(string reportCode, DateTime from, DateTime to, int facultyId, int gradeLevelId, string outcome)
        {
            tcConsole.SelectedTab = tabReports;
            ReportsApplyFilter(reportCode, from, to, facultyId, gradeLevelId, outcome);
        }

        // Prevent opening the report viewer before a report has been selected
        private void tcConsole_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage == tabReportViewer && viewerData == null)
            {
                e.Cancel = true;
                ReportStatus("Open a report from Reports or Sessions first.", true);
            }
        }

        // Refresh the screen when the main tab changes
        private void tcConsole_SelectedIndexChanged(object sender, EventArgs e)
        {
            PageShown(CurrentPage);
        }

        // Refresh the selected Sessions sub-tab when it is visible
        private void tcSessions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tcConsole.SelectedTab == tabSessions)
            {
                PageShown(CurrentPage);
            }
        }

        // Refresh the selected Reference Data sub-tab when it is visible
        private void tcReference_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tcConsole.SelectedTab == tabReference)
            {
                PageShown(CurrentPage);
            }
        }

        // Update the window title, status bar and buttons for the selected screen
        // Refresh the screen and track whether it has been opened before
        private void PageShown(TabPage page)
        {
            if (page == null)
            {
                return;
            }

            Text = "Future Path — " + page.Text;
            SetStatus(IsReferencePage(page) ? ReferenceNote : "Ready", DeepNavy, Color.White);

            // Set the Enter and Escape buttons for the current page
            if (page == tabLearnerSearch) SetPageButtons(btnLearnerSearchOpen, null);
            else if (page == tabSessionSearch) SetPageButtons(btnSessionSearchOpen, null);
            else if (page == tabReportShortcuts) SetPageButtons(btnReportShortcutsGenerate, null);
            else if (page == tabReports) SetPageButtons(btnReportsGenerate, null);
            else if (page == tabReportViewer) SetPageButtons(btnReportViewerSave, btnReportViewerBack);
            else if (page == tabUniversity) SetPageButtons(btnUniversityAdd, btnUniversityCancel);
            else if (page == tabRequirements) SetPageButtons(btnRequirementsAdd, btnRequirementsCancel);
            else if (page == tabFundingAdmin) SetPageButtons(btnFundingAdminAdd, btnFundingAdminCancel);
            else if (page == tabCategories) SetPageButtons(btnCategoriesAdd, btnCategoriesCancel);
            else if (page == tabQuestionnaire) SetPageButtons(btnQuestionnaireAdd, btnQuestionnaireCancel);
            else if (page == tabAccounts) SetPageButtons(btnAccountsAdd, btnAccountsCancel);
            else SetPageButtons(null, null);

            bool firstTime = shownPages.Add(page);
            if (page == tabAdvisorDashboard) AdvisorDashboardShown(firstTime);
            else if (page == tabAdminDashboard) AdminDashboardShown(firstTime);
            else if (page == tabLearnerSearch) LearnerSearchShown(firstTime);
            else if (page == tabSessionSearch) SessionSearchShown(firstTime);
            else if (page == tabReportShortcuts) ReportShortcutsShown(firstTime);
            else if (page == tabReports) ReportsShown(firstTime);
            else if (page == tabReportViewer) ReportViewerShown(firstTime);
            else if (page == tabUniversity) UniversityShown(firstTime);
            else if (page == tabRequirements) RequirementsShown(firstTime);
            else if (page == tabFundingAdmin) FundingAdminShown(firstTime);
            else if (page == tabCategories) CategoriesShown(firstTime);
            else if (page == tabQuestionnaire) QuestionnaireShown(firstTime);
            else if (page == tabAccounts) AccountsShown(firstTime);
        }

        // Set the buttons triggered by Enter and Escape
        private void SetPageButtons(Button accept, Button cancel)
        {
            AcceptButton = accept;
            CancelButton = cancel;
        }

        #endregion

        #region Menu

        // Sign out and return to the homepage
        private void mnuSignOut_Click(object sender, EventArgs e)
        {
            SignOut();
        }

        // Close the application from the menu
        private void mnuExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        // Confirm sign out before closing the console
        private void SignOut()
        {
            if (!Confirm("Sign out", "Sign out of Future Path and return to the homepage?", "Sign out"))
            {
                return;
            }

            mainForm.Show();
            navigating = true;
            Close();
        }

        #endregion

        #region Shared helpers (used by every screen's partial file)

        // Display a message in the status bar
        private void SetStatus(string message, Color back, Color fore)
        {
            pnlStatus.BackColor = back;
            lblStatus.ForeColor = fore;
            lblSignedIn.ForeColor = fore;
            lblStatus.Text = message;
        }

        // Show a status message with warning or normal colours
        // Add the reference-data note when appropriate
        private void ReportStatus(string message, bool isWarning)
        {
            if (IsReferencePage(CurrentPage) && message.IndexOf(ReferenceNote, StringComparison.OrdinalIgnoreCase) < 0)
            {
                message += " · " + ReferenceNote;
            }

            SetStatus(message, isWarning ? AmberColour : BlueColour, isWarning ? InkColour : Color.White);
        }

        // Highlight invalid input fields and display their validation messages
        private static bool CheckField(bool valid, Control input, Label fieldLabel, Label messageLabel, string message)
        {
            input.BackColor = valid ? SystemColors.Window : RedTint;
            fieldLabel.ForeColor = valid ? InkColour : RedColour;
            messageLabel.Text = valid ? string.Empty : "⚠ " + message;
            messageLabel.Visible = !valid;
            input.AccessibleDescription = valid ? null : message;
            return valid;
        }

        // Display a summary of validation errors and focus on the first invalid field
        private void ShowFormSummary(Panel summaryPanel, Label summaryLabel, List<string> fields, Control firstInvalid, string savedItem)
        {
            string count = fields.Count + (fields.Count == 1 ? " entry" : " entries");
            summaryLabel.Text = "⚠ " + count + " to correct: " + string.Join(", ", fields) + ".";
            summaryPanel.Visible = true;
            ReportStatus(count + " to correct before " + savedItem + " can be saved.", true);
            if (firstInvalid != null && firstInvalid.CanFocus)
            {
                firstInvalid.Focus();
            }
        }

        // Format active and inactive records as coloured status labels
        private static void FormatStatusChip(DataGridViewCellFormattingEventArgs e, bool active)
        {
            e.Value = active ? "✓ Active" : "Inactive";
            e.CellStyle.ForeColor = active ? GreenColour : MutedColour;
            e.CellStyle.BackColor = active ? GreenTint : GreyTint;
            e.CellStyle.SelectionForeColor = e.CellStyle.ForeColor;
            e.CellStyle.SelectionBackColor = e.CellStyle.BackColor;
            e.CellStyle.Font = ChipFont;
            e.FormattingApplied = true;
        }

        // Accept either a dot or comma as a decimal separator
        // Use the separator required by the current Windows region
        private void DecimalBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '.' || e.KeyChar == ',')
            {
                e.KeyChar = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
            }
        }

        // Get the selected ID from a dropdown or return 0 if none is selected
        private static int SelectedId(ComboBox list)
        {
            if (list.SelectedIndex < 0 || !(list.SelectedValue is int))
            {
                return 0;
            }

            return (int)list.SelectedValue;
        }

        // Get the selected code from a dropdown or return an empty string
        private static string SelectedCode(ComboBox list)
        {
            if (list.SelectedIndex < 0 || list.SelectedValue == null)
            {
                return string.Empty;
            }

            return Convert.ToString(list.SelectedValue, System.Globalization.CultureInfo.InvariantCulture);
        }

        // Show a confirmation dialog and return whether the user confirmed
        private bool Confirm(string title, string message, string confirmText)
        {
            using (var dialog = new ConfirmationDialog(title, message, confirmText))
            {
                return dialog.ShowDialog(this) == DialogResult.OK;
            }
        }

        // Display validation errors and return focus to the invalid control
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

        // Confirm before closing the staff console, except when signing out
        private void StaffConsoleForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing || navigating || mainForm.Visible)
            {
                return;
            }

            if (!Confirm("Close Future Path", "Close Future Path? You will be signed out.", "Close"))
            {
                e.Cancel = true;
            }
        }

        // Exit the application when the console is closed without signing out
        private void StaffConsoleForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!navigating && !mainForm.Visible)
            {
                Application.Exit();
            }
        }

        #endregion
    }
}
