using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// The staff console: every screen a Career Advisor or Administrator reaches after UI 8 Staff login —
    /// UI 9 Advisor dashboard, UI 12 Administrator dashboard, UI 10 Sessions (10.1a / 10.2b / 10.3c under
    /// visible tabs), UI 11 Reports and UI 13 Reference data (13.1–13.6 under visible tabs). Each screen is
    /// a TabPage of <c>tabConsole</c>, whose own tab strip is hidden; the rail moves between areas
    /// (spec 2.2.2). The window is always 1200 × 800 (decisions.md D6).
    /// <para>
    /// Role-based security (spec Section 8, decisions.md D11): the constructor removes every page the
    /// signed-in role may not reach, so Administrator-only screens are never rendered for a Career Advisor.
    /// The Business Logic Layer checks the role again inside every method that changes data.
    /// </para>
    /// <para>
    /// This file holds the shared shell (palette, page switching, rail, action band, status band, sign out,
    /// closing). Each screen's controls and code live in a partial-class file named after it
    /// (StaffConsoleForm.Reports.cs, StaffConsoleForm.Requirements.cs, …).
    /// </para>
    /// </summary>
    public partial class StaffConsoleForm : Form
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
        private static readonly Font RailFont = new Font("Segoe UI", 10F);
        private static readonly Font RailActiveFont = new Font("Segoe UI Semibold", 10F);
        private static readonly Font ChipFont = new Font("Segoe UI Semibold", 9F);

        #endregion

        private const string AdministratorRole = "Administrator";
        private const string ReferenceNote = "Changes apply to new advisory sessions only";

        private readonly Form mainForm;
        private readonly HashSet<TabPage> shownPages = new HashSet<TabPage>();
        private bool changingPage;
        private bool navigating;
        private TabPage lastSessionsPage;
        private TabPage lastReferencePage;

        /// <summary>Opens the console for the staff member returned by the login controller.</summary>
        /// <param name="mainForm">The main window (UI 1), shown again on sign out.</param>
        /// <param name="userId">UserAccount.UserID.</param>
        /// <param name="fullName">UserAccount.FullName.</param>
        /// <param name="role">UserAccount.Role: "Administrator" or "Career Advisor".</param>
        public StaffConsoleForm(Form mainForm, int userId, string fullName, string role)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            UserId = userId;
            FullName = fullName;
            Role = role;
            lastSessionsPage = tabSessionSearch;      // UI 10 opens on 10.2b, as in its wireframe
            lastReferencePage = tabRequirements;      // UI 13 opens on 13.2, as in its wireframe
            numMinimumAverage.KeyPress += DecimalBox_KeyPress;
            numRelevanceWeight.KeyPress += DecimalBox_KeyPress;
            ApplyRole();
            OpenDashboard();
        }

        #region The signed-in staff member

        /// <summary>UserAccount.UserID — passed to every BLL call so the BLL can check the role itself.</summary>
        private int UserId { get; set; }

        /// <summary>UserAccount.FullName, shown in the rail footer and the status band.</summary>
        private string FullName { get; set; }

        /// <summary>UserAccount.Role.</summary>
        private string Role { get; set; }

        /// <summary>True when an Administrator is signed in.</summary>
        private bool IsAdministrator
        {
            get { return Role == AdministratorRole; }
        }

        /// <summary>
        /// Removes the pages this role may not reach and lays out the rail (decisions.md D11): Career
        /// Advisors get UI 9, 10 and 11; Administrators get UI 12, 10, 11 and 13.
        /// </summary>
        private void ApplyRole()
        {
            if (IsAdministrator)
            {
                tabConsole.TabPages.Remove(tabAdvisorDashboard);
            }
            else
            {
                tabConsole.TabPages.Remove(tabAdminDashboard);
                tabConsole.TabPages.Remove(tabReference);
            }

            btnNavReferenceData.Visible = IsAdministrator;
            int y = btnNavSessions.Bottom;
            if (IsAdministrator)
            {
                btnNavReferenceData.Top = y;
                y = btnNavReferenceData.Bottom;
            }

            btnNavReports.Top = y;
            btnNavSignOut.Top = btnNavReports.Bottom;

            lblBadge.Text = IsAdministrator ? "ADMINISTRATOR" : "CAREER ADVISOR";
            lblRailFooter.Text = "Signed in" + Environment.NewLine + FullName + Environment.NewLine + Role;
        }

        #endregion

        #region Page switching

        /// <summary>The screen on show (a sub-tab when a group is showing).</summary>
        private TabPage CurrentPage
        {
            get
            {
                TabPage top = tabConsole.SelectedTab;
                if (top == tabSessions) return tabSessionsGroup.SelectedTab;
                if (top == tabReference) return tabReferenceGroup.SelectedTab;
                return top;
            }
        }

        private bool IsSessionsPage(TabPage page)
        {
            return page == tabLearnerSearch || page == tabSessionSearch || page == tabReportShortcuts;
        }

        private bool IsReferencePage(TabPage page)
        {
            return page == tabUniversity || page == tabRequirements || page == tabFundingAdmin
                   || page == tabCategories || page == tabQuestionnaire || page == tabAccounts;
        }

        /// <summary>Opens the signed-in role's dashboard: UI 12 for Administrators, UI 9 for Career Advisors.</summary>
        private void OpenDashboard()
        {
            ShowPage(IsAdministrator ? tabAdminDashboard : tabAdvisorDashboard);
        }

        /// <summary>
        /// Shows one screen: selects its page (and group tab), sets the caption, title band, rail highlight,
        /// action-band buttons and status band, then lets the screen refresh through its <c>…Shown(firstTime)</c>
        /// method. Pages the role may not reach are refused (they were removed in <see cref="ApplyRole"/>).
        /// </summary>
        private void ShowPage(TabPage page)
        {
            if (page == tabSessions) page = lastSessionsPage;
            if (page == tabReference) page = lastReferencePage;
            if (!IsAdministrator && (page == tabAdminDashboard || IsReferencePage(page))) return;
            if (IsAdministrator && page == tabAdvisorDashboard) return;
            if (IsSessionsPage(page)) lastSessionsPage = page;
            if (IsReferencePage(page)) lastReferencePage = page;

            changingPage = true;
            if (IsSessionsPage(page))
            {
                tabConsole.SelectedTab = tabSessions;
                tabSessionsGroup.SelectedTab = page;
            }
            else if (IsReferencePage(page))
            {
                tabConsole.SelectedTab = tabReference;
                tabReferenceGroup.SelectedTab = page;
            }
            else
            {
                tabConsole.SelectedTab = page;
            }

            changingPage = false;

            string title, subtitle, hint;
            Button railButton;
            if (page == tabAdvisorDashboard || page == tabAdminDashboard)
            {
                title = page == tabAdminDashboard ? "Administrator dashboard" : "Advisor dashboard";
                subtitle = page == tabAdminDashboard ? "Maintain reference data and generate reports"
                                                     : "Review advisory sessions and generate reports";
                hint = "Sign out from the rail when you finish. Learners never log in.";
                railButton = btnNavDashboard;
                ShowButtons();
            }
            else if (IsSessionsPage(page))
            {
                title = "Sessions dashboard";
                subtitle = "Find an advisory session and open or export its summary";
                hint = "Read-only · search on any field, none are required.";
                railButton = btnNavSessions;
                btnPrimary.Text = page == tabLearnerSearch ? LearnerSearchPrimaryText
                                : page == tabSessionSearch ? SessionSearchPrimaryText
                                : ReportShortcutsPrimaryText;
                btnCancel.Text = "Close";
                ShowButtons(btnPrimary, btnCancel);
            }
            else if (page == tabReportViewer)
            {
                title = ReportNameOf(viewerReportCode);
                subtitle = ReportScreenOf(viewerReportCode) + " · " + viewerReportCode + " · " + viewerScope;
                hint = "Preview: nothing is saved or recorded until you save the report.";
                railButton = viewerReturnPage == tabSessionSearch ? btnNavSessions : btnNavReports;
                btnPrimary.Text = "Save report";
                btnCancel.Text = "Back";
                ShowButtons(btnPrimary, btnCancel);
            }
            else if (page == tabReports)
            {
                title = "Reports";
                subtitle = "Choose a report, set its parameters, then preview or generate it";
                hint = "Career Advisors reach these three reports from UI 10.3c";
                railButton = btnNavReports;
                btnPrimary.Text = "Generate report";
                btnCancel.Text = "Cancel";
                ShowButtons(btnPrimary, btnPreview, btnCancel);
            }
            else
            {
                title = "Reference data";
                subtitle = "Maintain the data the recommendation engine reads — Administrators only";
                hint = ReferenceNote;
                railButton = btnNavReferenceData;
                string noun = ItemNounOf(page);
                btnPrimary.Text = "Add " + noun;
                btnUpdate.Text = "Update " + noun;
                btnCancel.Text = "Cancel";
                ShowButtons(btnPrimary, btnUpdate, btnDeactivate, btnCancel);
            }

            btnPrimary.AccessibleName = btnPrimary.Text;
            btnUpdate.AccessibleName = btnUpdate.Text;
            btnCancel.AccessibleName = btnCancel.Text;
            Text = "Future Path — " + title;
            lblTitle.Text = title;
            lblSubtitle.Text = subtitle;
            lblActionHint.Text = hint;
            UpdateRail(railButton);
            SetStatus("Signed in as " + FullName + " (" + Role + ")" + (IsReferencePage(page) ? " · " + ReferenceNote : string.Empty),
                      DeepNavy, Color.White);

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

        /// <summary>The thing a UI 13 tab maintains, for its Add / Update button labels.</summary>
        private string ItemNounOf(TabPage page)
        {
            if (page == tabUniversity) return UniversityItemNoun;
            if (page == tabRequirements) return RequirementsItemNoun;
            if (page == tabFundingAdmin) return FundingAdminItemNoun;
            if (page == tabCategories) return CategoriesItemNoun;
            if (page == tabQuestionnaire) return QuestionnaireItemNoun;
            return AccountsItemNoun;
        }

        /// <summary>UI 10.3c "Generate report" → UI 11, already filtered (spec 2.1).</summary>
        private void OpenReports(string reportCode, DateTime from, DateTime to, int? facultyId, int? gradeLevelId, string outcome)
        {
            ShowPage(tabReports);
            ReportsApplyFilter(reportCode, from, to, facultyId, gradeLevelId, outcome);
        }

        /// <summary>The hidden top-level tab strip never changes by keyboard; only the rail and code move areas.</summary>
        private void tabConsole_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (!changingPage)
            {
                e.Cancel = true;
            }
        }

        /// <summary>A click on a visible Sessions tab (10.1a / 10.2b / 10.3c) refreshes the shell.</summary>
        private void tabSessionsGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!changingPage && tabConsole.SelectedTab == tabSessions && tabSessionsGroup.SelectedTab != null)
            {
                ShowPage(tabSessionsGroup.SelectedTab);
            }
        }

        /// <summary>A click on a visible Reference data tab (13.1–13.6) refreshes the shell.</summary>
        private void tabReferenceGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!changingPage && tabConsole.SelectedTab == tabReference && tabReferenceGroup.SelectedTab != null)
            {
                ShowPage(tabReferenceGroup.SelectedTab);
            }
        }

        #endregion

        #region Rail

        /// <summary>Highlights the rail item of the area on show.</summary>
        private void UpdateRail(Button active)
        {
            foreach (Button item in new[] { btnNavDashboard, btnNavSessions, btnNavReferenceData, btnNavReports, btnNavSignOut })
            {
                bool on = item == active;
                item.BackColor = on ? RailActive : NavyColour;
                item.ForeColor = on ? Color.White : RailText;
                item.Font = on ? RailActiveFont : RailFont;
            }
        }

        private void btnNavDashboard_Click(object sender, EventArgs e)
        {
            OpenDashboard();
        }

        private void btnNavSessions_Click(object sender, EventArgs e)
        {
            ShowPage(tabSessions);
        }

        private void btnNavReferenceData_Click(object sender, EventArgs e)
        {
            if (IsAdministrator)
            {
                ShowPage(tabReference);
            }
        }

        private void btnNavReports_Click(object sender, EventArgs e)
        {
            ShowPage(tabReports);
        }

        private void btnNavSignOut_Click(object sender, EventArgs e)
        {
            SignOut();
        }

        /// <summary>Asks for confirmation, then closes the console and returns to UI 1.</summary>
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

        #region Action band

        /// <summary>Shows only the given action-band buttons and gives the hint label the width they leave.</summary>
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

        /// <summary>The primary (navy, AcceptButton) action of the screen on show.</summary>
        private void btnPrimary_Click(object sender, EventArgs e)
        {
            TabPage page = CurrentPage;
            if (page == tabLearnerSearch) LearnerSearchPrimary();
            else if (page == tabSessionSearch) SessionSearchPrimary();
            else if (page == tabReportShortcuts) ReportShortcutsPrimary();
            else if (page == tabReports) ReportsGenerate();
            else if (page == tabReportViewer) ReportViewerSave();
            else if (page == tabUniversity) UniversityAddNew();
            else if (page == tabRequirements) RequirementsAddNew();
            else if (page == tabFundingAdmin) FundingAdminAddNew();
            else if (page == tabCategories) CategoriesAddNew();
            else if (page == tabQuestionnaire) QuestionnaireAddNew();
            else if (page == tabAccounts) AccountsAddNew();
        }

        /// <summary>UI 13 "Update &lt;item&gt;".</summary>
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            TabPage page = CurrentPage;
            if (page == tabUniversity) UniversityUpdateSelected();
            else if (page == tabRequirements) RequirementsUpdateSelected();
            else if (page == tabFundingAdmin) FundingAdminUpdateSelected();
            else if (page == tabCategories) CategoriesUpdateSelected();
            else if (page == tabQuestionnaire) QuestionnaireUpdateSelected();
            else if (page == tabAccounts) AccountsUpdateSelected();
        }

        /// <summary>UI 13 "Deactivate" (each screen confirms first; nothing is ever deleted).</summary>
        private void btnDeactivate_Click(object sender, EventArgs e)
        {
            TabPage page = CurrentPage;
            if (page == tabUniversity) UniversityDeactivateSelected();
            else if (page == tabRequirements) RequirementsDeactivateSelected();
            else if (page == tabFundingAdmin) FundingAdminDeactivateSelected();
            else if (page == tabCategories) CategoriesDeactivateSelected();
            else if (page == tabQuestionnaire) QuestionnaireDeactivateSelected();
            else if (page == tabAccounts) AccountsDeactivateSelected();
        }

        /// <summary>UI 11 "Preview".</summary>
        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (CurrentPage == tabReports)
            {
                ReportsPreview();
            }
        }

        /// <summary>
        /// Close (UI 10) and Cancel (UI 11) return to the dashboard; Cancel on UI 13 abandons the edit; Back on the
        /// report viewer returns to the screen that opened it.
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            TabPage page = CurrentPage;
            if (!btnCancel.Visible) return;
            if (page == tabUniversity) UniversityCancelEdit();
            else if (page == tabRequirements) RequirementsCancelEdit();
            else if (page == tabFundingAdmin) FundingAdminCancelEdit();
            else if (page == tabCategories) CategoriesCancelEdit();
            else if (page == tabQuestionnaire) QuestionnaireCancelEdit();
            else if (page == tabAccounts) AccountsCancelEdit();
            else if (page == tabReportViewer) ReportViewerBack();
            else OpenDashboard();
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

        /// <summary>
        /// A screen's status message: amber with ink text when <paramref name="isWarning"/> (nothing matched,
        /// something outstanding), blue with white text otherwise. UI 13 screens add the reference-data note.
        /// </summary>
        private void ReportStatus(string message, bool isWarning)
        {
            if (IsReferencePage(CurrentPage) && message.IndexOf(ReferenceNote, StringComparison.OrdinalIgnoreCase) < 0)
            {
                message += " · " + ReferenceNote;
            }

            SetStatus(message, isWarning ? AmberColour : BlueColour, isWarning ? InkColour : Color.White);
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

        /// <summary>Raises the validation dialog, then returns focus to the first offending control.</summary>
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

        /// <summary>The caption-bar X closes Future Path, so it asks first (not when signing out).</summary>
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

        /// <summary>Ends the application when the console was closed with the caption-bar X.</summary>
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
