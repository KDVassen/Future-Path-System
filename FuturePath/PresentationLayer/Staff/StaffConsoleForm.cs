using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using FuturePath.ApplicationLayer;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// The staff console: every screen a Career Advisor or Administrator reaches after UI 8 Staff login —
    /// UI 9 Advisor dashboard, UI 12 Administrator dashboard, UI 10 Sessions (10.1a / 10.2b / 10.3c),
    /// UI 13 Reference data (13.1–13.6), UI 11 Reports and the report viewer (UI 11.1a / 11.2b / 11.3c).
    /// Each screen is a TabPage of <c>tcConsole</c>, whose tab headers are visible; Sessions and Reference
    /// data hold their screens in the nested <c>tcSessions</c> and <c>tcReference</c> tab controls.
    /// Each tab has its own row of buttons, with one <c>btnX_Click</c> handler per button in that screen's
    /// file. The menu bar offers Sign out and Exit, and the status bar shows the latest message with the
    /// signed-in user on the right. The window is about 990 × 800 (decisions.md D6).
    /// <para>
    /// Role-based security (spec Section 8, decisions.md D11): the constructor removes every page the
    /// signed-in role may not reach, so Administrator-only screens are never rendered for a Career Advisor.
    /// The Business Logic Layer checks the role again inside every method that changes data.
    /// </para>
    /// <para>
    /// This file holds the shared parts (palette, tab navigation, status bar, menu, sign out, closing).
    /// Each screen's controls and code live in a partial-class file named after it
    /// (StaffConsoleForm.Reports.cs, StaffConsoleForm.Requirements.cs, …).
    /// </para>
    /// </summary>
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
            tcSessions.SelectedTab = tabSessionSearch;      // UI 10 opens on 10.2b, as in its wireframe
            tcReference.SelectedTab = tabRequirements;      // UI 13 opens on 13.2, as in its wireframe
            numMinimumAverage.KeyPress += DecimalBox_KeyPress;
            numMinimumMark.KeyPress += DecimalBox_KeyPress;
            numRelevanceWeight.KeyPress += DecimalBox_KeyPress;
            ApplyRole();
            OpenDashboard();
            PageShown(CurrentPage);   // no selection event fires before the window exists, so show the first page here
        }

        #region The signed-in staff member

        /// <summary>UserAccount.UserID — passed to every BLL call so the BLL can check the role itself.</summary>
        private int UserId { get; set; }

        /// <summary>UserAccount.FullName, shown in the status bar.</summary>
        private string FullName { get; set; }

        /// <summary>UserAccount.Role.</summary>
        private string Role { get; set; }

        /// <summary>True when an Administrator is signed in.</summary>
        private bool IsAdministrator
        {
            get { return Role == UserAccount.RoleAdministratorText; }
        }

        /// <summary>
        /// Removes the pages this role may not reach (decisions.md D11) and shows who is signed in:
        /// Career Advisors get UI 9, 10 and 11; Administrators get UI 12, 10, 13 and 11.
        /// </summary>
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

        /// <summary>The screen on show (the selected sub-tab when Sessions or Reference data is selected).</summary>
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

        /// <summary>True for the three Sessions sub-tabs (10.1a, 10.2b, 10.3c).</summary>
        private bool IsSessionsPage(TabPage page)
        {
            return page == tabLearnerSearch || page == tabSessionSearch || page == tabReportShortcuts;
        }

        /// <summary>True for the six Reference data sub-tabs (13.1–13.6).</summary>
        private bool IsReferencePage(TabPage page)
        {
            return page == tabUniversity || page == tabRequirements || page == tabFundingAdmin
                   || page == tabCategories || page == tabQuestionnaire || page == tabAccounts;
        }

        /// <summary>Opens the signed-in role's dashboard: UI 12 for Administrators, UI 9 for Career Advisors.</summary>
        private void OpenDashboard()
        {
            tcConsole.SelectedTab = IsAdministrator ? tabAdminDashboard : tabAdvisorDashboard;
        }

        /// <summary>
        /// Selects a screen from code. A Sessions or Reference sub-tab is selected first, then its parent tab
        /// (selecting tabSessions or tabReference itself keeps the sub-tab used last). A page removed for this
        /// role (<see cref="ApplyRole"/>) is never selected.
        /// </summary>
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

        /// <summary>UI 10.3c "Generate report" → UI 11, already filtered (spec 2.1).</summary>
        private void OpenReports(string reportCode, DateTime from, DateTime to, int? facultyId, int? gradeLevelId, string outcome)
        {
            tcConsole.SelectedTab = tabReports;
            ReportsApplyFilter(reportCode, from, to, facultyId, gradeLevelId, outcome);
        }

        /// <summary>The Report viewer tab is refused until a report has been opened from Reports or Sessions.</summary>
        private void tcConsole_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage == tabReportViewer && viewerData == null)
            {
                e.Cancel = true;
                ReportStatus("Open a report from Reports or Sessions first.", true);
            }
        }

        /// <summary>A new top-level tab is on show.</summary>
        private void tcConsole_SelectedIndexChanged(object sender, EventArgs e)
        {
            PageShown(CurrentPage);
        }

        /// <summary>A new Sessions sub-tab (10.1a / 10.2b / 10.3c) is on show.</summary>
        private void tcSessions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tcConsole.SelectedTab == tabSessions)
            {
                PageShown(CurrentPage);
            }
        }

        /// <summary>A new Reference data sub-tab (13.1–13.6) is on show.</summary>
        private void tcReference_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tcConsole.SelectedTab == tabReference)
            {
                PageShown(CurrentPage);
            }
        }

        /// <summary>
        /// Runs each time a screen comes into view: sets the caption, the status bar and the page's Enter / Esc
        /// buttons, then lets the screen refresh through its <c>…Shown(firstTime)</c> method.
        /// </summary>
        private void PageShown(TabPage page)
        {
            if (page == null)
            {
                return;
            }

            Text = "Future Path — " + page.Text;
            SetStatus(IsReferencePage(page) ? ReferenceNote : "Ready", DeepNavy, Color.White);

            // Enter and Esc run this page's own buttons (the dashboards have none).
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

        /// <summary>Sets the buttons that Enter (accept) and Esc (cancel) press; null for none.</summary>
        private void SetPageButtons(Button accept, Button cancel)
        {
            AcceptButton = accept;
            CancelButton = cancel;
        }

        #endregion

        #region Menu

        /// <summary>Sign out: asks for confirmation, then returns to UI 1.</summary>
        private void mnuSignOut_Click(object sender, EventArgs e)
        {
            SignOut();
        }

        /// <summary>Exit: closes Future Path (the closing confirmation still asks first).</summary>
        private void mnuExit_Click(object sender, EventArgs e)
        {
            Close();
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

        #region Shared helpers (used by every screen's partial file)

        /// <summary>Shows a message in the status bar (never a modal; spec 2.2.7).</summary>
        private void SetStatus(string message, Color back, Color fore)
        {
            pnlStatus.BackColor = back;
            lblStatus.ForeColor = fore;
            lblSignedIn.ForeColor = fore;
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
        /// Shows a UI 13 form's validation summary — "⚠ n entries to correct: A, B." — with the matching
        /// status message, then moves focus to the first invalid control.
        /// </summary>
        /// <param name="summaryPanel">The form's red summary panel.</param>
        /// <param name="summaryLabel">The label inside it.</param>
        /// <param name="fields">The names of the fields to correct, in screen order.</param>
        /// <param name="firstInvalid">The first control to correct (receives the focus).</param>
        /// <param name="savedItem">What cannot be saved yet, e.g. "the programme".</param>
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

        /// <summary>Shows an IsActive cell as a status chip (13.1–13.6): "✓ Active" in green or "Inactive" in grey.</summary>
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

        /// <summary>
        /// The ID chosen in a bound list, or 0 when nothing is chosen. 0 means "all" or "none" to every
        /// BLL method (application-layer.md §3), so a blank filter is passed straight through.
        /// </summary>
        private static int SelectedId(ComboBox list)
        {
            if (list.SelectedIndex < 0 || !(list.SelectedValue is int))
            {
                return 0;
            }

            return (int)list.SelectedValue;
        }

        /// <summary>The code chosen in a bound list (e.g. an OutcomeCode), or "" when nothing is chosen ("all").</summary>
        private static string SelectedCode(ComboBox list)
        {
            if (list.SelectedIndex < 0 || list.SelectedValue == null)
            {
                return string.Empty;
            }

            return Convert.ToString(list.SelectedValue, System.Globalization.CultureInfo.InvariantCulture);
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

        /// <summary>The caption-bar X and Exit close Future Path, so they ask first (not when signing out).</summary>
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

        /// <summary>Ends the application when the console was closed with the caption-bar X or Exit.</summary>
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
