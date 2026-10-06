using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 11 — Reports Selection and Generation (standard size, Career Advisor and Administrator;
    /// spec 2.3.12). The user picks one of the three reports (FP-LSR, FP-DIE, FP-LER), sets its
    /// parameters, and chooses an output format and a folder. Parameters that do not apply to the
    /// chosen report are disabled with the reason stated beneath them, never hidden. A readiness
    /// panel names the report and its scope and turns green once every requirement is met.
    /// Validation fires on Generate report; the BLL checks the role again before generating and
    /// logs the export in <c>ExportReport</c> (Section 8).
    /// </summary>
    public partial class ReportGenerationForm : Form
    {
        // Colours used at runtime (spec 2.2.6). Everything else is set in the Designer.
        private static readonly Color InkColour = Color.FromArgb(27, 31, 36);
        private static readonly Color MutedColour = Color.FromArgb(88, 96, 108);
        private static readonly Color RedColour = Color.FromArgb(164, 38, 44);
        private static readonly Color RedTint = Color.FromArgb(250, 234, 235);
        private static readonly Color BlueColour = Color.FromArgb(31, 95, 168);
        private static readonly Color AmberColour = Color.FromArgb(200, 137, 43);
        private static readonly Color AmberTint = Color.FromArgb(252, 243, 224);
        private static readonly Color GreenColour = Color.FromArgb(31, 107, 58);
        private static readonly Color GreenTint = Color.FromArgb(230, 242, 234);
        private static readonly Color DeepNavy = Color.FromArgb(11, 31, 58);

        private const string AdministratorRole = "Administrator";
        private const string CareerAdvisorRole = "Career Advisor";

        private const string LearnerSummaryCode = "FP-LSR";
        private const string DegreeInterestCode = "FP-DIE";
        private const string NotEligibleCode = "FP-LER";

        private readonly Form homeForm;
        private readonly int userId;
        private readonly string fullName;
        private readonly string role;
        private bool navigating;
        private bool loading;

        /// <summary>Creates the report screen for the signed-in staff member.</summary>
        /// <param name="homeForm">UI 1, shown again on sign out.</param>
        /// <param name="userId">UserAccount.UserID, passed to the BLL, which checks the role itself.</param>
        /// <param name="fullName">UserAccount.FullName, shown on the rail and in the status band.</param>
        /// <param name="role">UserAccount.Role: "Career Advisor" or "Administrator".</param>
        public ReportGenerationForm(Form homeForm, int userId, string fullName, string role)
        {
            loading = true;
            InitializeComponent();
            this.homeForm = homeForm;
            this.userId = userId;
            this.fullName = fullName;
            this.role = role;

            // The rail and badge are built from the role (Section 8).
            btnNavSessions.Visible = role == CareerAdvisorRole;
            btnNavReferenceData.Visible = role == AdministratorRole;
            lblBadge.Text = role == AdministratorRole ? "ADMINISTRATOR" : "CAREER ADVISOR";
            lblRailFooter.Text = "Signed in" + Environment.NewLine + fullName + Environment.NewLine + role;

            dtpFrom.Value = new DateTime(DateTime.Today.Year, 1, 1);
            dtpTo.Value = DateTime.Today;
            txtSaveTo.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "FuturePath", "Reports");

            LoadLists();
            loading = false;
            ApplyReportRules();
            SetStatus(SignedInText, DeepNavy);
        }

        private string SignedInText
        {
            get { return "Signed in as " + fullName + " (" + role + ")"; }
        }

        #region Report choice

        /// <summary>The report code of the selected radio button.</summary>
        private string SelectedReportCode
        {
            get
            {
                if (rdoLearnerSummary.Checked) return LearnerSummaryCode;
                if (rdoNotEligible.Checked) return NotEligibleCode;
                return DegreeInterestCode;
            }
        }

        /// <summary>The report name shown in the readiness panel and the confirmation.</summary>
        private string SelectedReportName
        {
            get
            {
                if (rdoLearnerSummary.Checked) return "Learner Advisory Summary (FP-LSR)";
                if (rdoNotEligible.Checked) return "Learners Not Currently Eligible (FP-LER)";
                return "Degree Interest and Eligibility Report (FP-DIE)";
            }
        }

        /// <summary>
        /// Opens this screen already filtered, as UI 10.3c's Generate report does. Lists are bound
        /// in the constructor, so the IDs can be selected straight away.
        /// </summary>
        /// <param name="reportCode">"FP-LSR", "FP-DIE" or "FP-LER".</param>
        /// <param name="from">First session date in scope.</param>
        /// <param name="to">Last session date in scope.</param>
        /// <param name="facultyId">Faculty.FacultyID, or null for all faculties.</param>
        /// <param name="gradeLevelId">GradeLevel.GradeLevelID, or null for all grades.</param>
        /// <param name="outcome">RecommendationResult.OutcomeCode, or null for all outcomes.</param>
        public void ApplyFilter(string reportCode, DateTime from, DateTime to, int? facultyId, int? gradeLevelId, string outcome)
        {
            loading = true;
            rdoLearnerSummary.Checked = reportCode == LearnerSummaryCode;
            rdoNotEligible.Checked = reportCode == NotEligibleCode;
            rdoDegreeInterest.Checked = reportCode != LearnerSummaryCode && reportCode != NotEligibleCode;

            if (from > to)
            {
                DateTime swap = from;
                from = to;
                to = swap;
            }

            dtpFrom.Value = Clamp(from.Date, dtpFrom);
            dtpTo.Value = Clamp(to.Date, dtpTo);
            SelectValue(cboFaculty, facultyId);
            SelectValue(cboGrade, gradeLevelId);
            SelectValue(cboOutcome, outcome);
            loading = false;

            ApplyReportRules();
            SetStatus("Filters copied from Advisory reports (UI 10.3c). Check them, then generate the report.", BlueColour);
        }

        private static DateTime Clamp(DateTime value, DateTimePicker picker)
        {
            if (value < picker.MinDate) return picker.MinDate;
            if (value > picker.MaxDate) return picker.MaxDate;
            return value;
        }

        /// <summary>Selects a value in a bound list; an unbound list or a null value selects nothing.</summary>
        private static void SelectValue(ComboBox combo, object value)
        {
            if (value == null || combo.DataSource == null || string.IsNullOrEmpty(combo.ValueMember))
            {
                combo.SelectedIndex = -1;
                return;
            }

            combo.SelectedValue = value;
        }

        private void ReportChoice_CheckedChanged(object sender, EventArgs e)
        {
            var choice = sender as RadioButton;
            if (loading || choice == null || !choice.Checked)
            {
                return;
            }

            ApplyReportRules();
        }

        /// <summary>
        /// Enables the parameters the chosen report uses and disables the rest, stating the reason
        /// under each disabled one (spec 2.3.12: disabled, never hidden). FP-LSR is regenerated per
        /// session, so it takes a session number instead of the period and filters.
        /// </summary>
        private void ApplyReportRules()
        {
            string code = SelectedReportCode;
            bool summary = code == LearnerSummaryCode;
            bool exceptions = code == NotEligibleCode;

            SetParameter(dtpFrom, lblFrom, null, !summary, null);
            SetParameter(dtpTo, lblTo, null, !summary, null);
            lblFromStar.Visible = !summary;
            lblToStar.Visible = !summary;
            lblPeriodNote.Text = summary ? "FP-LSR covers one session: enter its session number instead." : string.Empty;
            lblPeriodNote.Visible = summary;

            SetParameter(cboFaculty, lblFaculty, lblFacultyNote, !summary,
                summary ? "Not used: FP-LSR covers one session" : "Optional filter");
            SetParameter(cboGrade, lblGrade, lblGradeNote, !summary,
                summary ? "Not used for FP-LSR" : "Optional filter");
            SetParameter(cboOutcome, lblOutcome, lblOutcomeNote, !summary && !exceptions,
                summary ? "Not used: FP-LSR covers one session"
                        : exceptions ? "Exception report filters on outcome automatically" : "Optional filter");
            SetParameter(txtSessionNumber, lblSessionNumber, lblSessionNumberNote, summary,
                summary ? "Required, e.g. 2026-0042" : "Only for FP-LSR");
            lblSessionNumberStar.Visible = summary;

            bool inactiveApplies = code == DegreeInterestCode;
            if (!inactiveApplies)
            {
                chkIncludeInactive.Checked = false;
            }

            SetParameter(chkIncludeInactive, null, lblIncludeInactiveNote, inactiveApplies,
                summary ? "Never for learner-facing output"
                        : exceptions ? "FP-LER lists active programmes only" : "Staff-only report; off by default");

            // A disabled field cannot stay red.
            ClearError(dtpFrom, lblFrom, lblFromError);
            lblTo.ForeColor = dtpTo.Enabled ? InkColour : MutedColour;
            ClearError(txtSessionNumber, lblSessionNumber, lblSessionNumberError);

            LoadExportFormats(code);
            UpdateReadiness();
        }

        /// <summary>Enables or disables one parameter and writes its caption (muted, never red).</summary>
        private static void SetParameter(Control input, Label fieldLabel, Label noteLabel, bool enabled, string note)
        {
            input.Enabled = enabled;
            if (fieldLabel != null)
            {
                fieldLabel.ForeColor = enabled ? InkColour : MutedColour;
            }

            if (noteLabel != null)
            {
                noteLabel.Text = note ?? string.Empty;
                noteLabel.Visible = !string.IsNullOrEmpty(note);
            }

            input.AccessibleDescription = enabled ? null : note;
        }

        #endregion

        #region Business Logic Layer hooks

        /// <summary>Binds the Faculty, Grade and Advisory outcome lists to their reference tables.</summary>
        private void LoadLists()
        {
            try
            {
                // TODO BLL: cboFaculty.DataSource = ReferenceDataController.GetFaculties(includeAllRow: true);
                //           cboFaculty.DisplayMember = "FacultyName"; cboFaculty.ValueMember = "FacultyID";
                //           // the BLL puts an "All faculties" row (FacultyID null) first
                // TODO BLL: cboGrade.DataSource = ReferenceDataController.GetGradeLevels(includeAllRow: true);
                //           cboGrade.DisplayMember = "GradeDescription"; cboGrade.ValueMember = "GradeLevelID";
                // TODO BLL: cboOutcome.DataSource = ReferenceDataController.GetOutcomes(includeAllRow: true);
                //           cboOutcome.DisplayMember = "OutcomeName"; cboOutcome.ValueMember = "OutcomeCode";
                //           // no Outcome table exists: proposed SystemParameter OUTCOME_1..4 (RecommendationResult.OutcomeCode)
                cboFaculty.SelectedIndex = -1;
                cboGrade.SelectedIndex = -1;
                cboOutcome.SelectedIndex = -1;
            }
            catch (Exception)
            {
                SetStatus("The faculty, grade and outcome lists could not be loaded. Please close and try again.", RedColour);
            }
        }

        /// <summary>Binds the output formats allowed for the chosen report (reports.md: LSR PDF/Word, DIE PDF/Excel, LER PDF/Word).</summary>
        private void LoadExportFormats(string reportCode)
        {
            try
            {
                // TODO BLL: cboOutputFormat.DataSource = ReportController.GetExportFormats(reportCode);
                //           cboOutputFormat.DisplayMember = "FormatName"; cboOutputFormat.ValueMember = "ExportFormat";
                cboOutputFormat.SelectedIndex = -1;
            }
            catch (Exception)
            {
                SetStatus("The output formats could not be loaded. Please try again.", RedColour);
            }
        }

        /// <summary>
        /// Refreshes the scope line of the readiness panel: sessions in scope, faculties,
        /// programmes and the last generated date (from <c>ExportReport</c>).
        /// </summary>
        private void LoadScope()
        {
            string code = SelectedReportCode;
            string scope = code == LearnerSummaryCode
                ? "One completed advisory session" + (txtSessionNumber.Text.Trim().Length > 0 ? " · " + txtSessionNumber.Text.Trim() : string.Empty)
                : "Sessions " + dtpFrom.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) + " to " +
                  dtpTo.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) +
                  " · " + (cboFaculty.SelectedIndex >= 0 ? cboFaculty.Text : "all faculties") +
                  " · " + (cboGrade.SelectedIndex >= 0 ? cboGrade.Text : "all grades");
            try
            {
                // TODO BLL: var s = ReportController.GetScope(code, SessionNumberOrNull(), dtpFrom.Value.Date, dtpTo.Value.Date,
                //               SelectedId(cboFaculty), SelectedId(cboGrade), SelectedOutcome(), chkIncludeInactive.Checked);
                //           scope = s.SessionCount + " advisory sessions · " + s.FacultyCount + " faculties · " + s.ProgrammeCount + " programmes";
                // TODO BLL: DateTime? last = ReportController.GetLastGenerated(code);   // MAX(ExportReport.DateGenerated) for this ReportCode
                //           scope += last.HasValue ? " · last generated " + last.Value.ToString("d MMMM yyyy 'at' HH:mm") : " · not generated before";
            }
            catch (Exception)
            {
                scope += " · scope could not be counted";
            }

            lblReadyScope.Text = scope;
        }

        /// <summary>
        /// Generates the report. The BLL checks the signed-in user's role from <c>UserAccount</c>
        /// (the form's role is never trusted), builds the typed dataset, writes the file and logs an
        /// <c>ExportReport</c> row (ReportCode, SessionID or ParametersUsed, GeneratedByUserID,
        /// DateGenerated, ExportFormat).
        /// </summary>
        private void GenerateReport()
        {
            // TODO BLL: var report = ReportController.Generate(userId, SelectedReportCode, SessionNumberOrNull(),
            //               dtpFrom.Value.Date, dtpTo.Value.Date, SelectedId(cboFaculty), SelectedId(cboGrade),
            //               SelectedOutcome(), chkIncludeInactive.Checked, (string)cboOutputFormat.SelectedValue, txtSaveTo.Text.Trim());
            // TODO Stage 10: open the report viewer (UI 11.1a / 11.2b / 11.3c) for report — the RDLC ReportViewer is deferred.
        }

        /// <summary>Opens the preview of the chosen report without saving or logging anything.</summary>
        private void PreviewReport()
        {
            // TODO BLL: var data = ReportController.BuildPreview(userId, SelectedReportCode, SessionNumberOrNull(),
            //               dtpFrom.Value.Date, dtpTo.Value.Date, SelectedId(cboFaculty), SelectedId(cboGrade),
            //               SelectedOutcome(), chkIncludeInactive.Checked);
            // TODO Stage 10: show data in the report viewer (UI 11.1a / 11.2b / 11.3c) — nothing is written or logged.
        }

        /// <summary>The selected ID of an enabled, bound list, or null for "all".</summary>
        private static int? SelectedId(ComboBox combo)
        {
            if (!combo.Enabled || combo.SelectedIndex < 0 || !(combo.SelectedValue is int))
            {
                return null;
            }

            return (int)combo.SelectedValue;
        }

        /// <summary>The selected outcome code, or null when the filter is off or not used.</summary>
        private string SelectedOutcome()
        {
            return cboOutcome.Enabled && cboOutcome.SelectedIndex >= 0 ? cboOutcome.SelectedValue as string : null;
        }

        /// <summary>The session number for FP-LSR, otherwise null.</summary>
        private string SessionNumberOrNull()
        {
            return txtSessionNumber.Enabled ? txtSessionNumber.Text.Trim() : null;
        }

        #endregion

        #region Validation (validated again in the BLL)

        private bool PeriodValid
        {
            get { return !dtpFrom.Enabled || dtpFrom.Value.Date <= dtpTo.Value.Date; }
        }

        private bool SessionNumberValid
        {
            get { return !txtSessionNumber.Enabled || Regex.IsMatch(txtSessionNumber.Text.Trim(), @"^\d{4}-\d{4}$"); }
        }

        private bool OutputFormatValid
        {
            get { return cboOutputFormat.SelectedIndex >= 0; }
        }

        private bool SaveToValid
        {
            get
            {
                string path = txtSaveTo.Text.Trim();
                return path.Length > 0 && path.IndexOfAny(Path.GetInvalidPathChars()) < 0 && Path.IsPathRooted(path);
            }
        }

        private bool CheckPeriod()
        {
            // One message under both pickers; both labels turn red (a DateTimePicker cannot be tinted).
            const string message = "Sessions from must be on or before Sessions to.";
            bool valid = PeriodValid;
            CheckField(valid, dtpFrom, lblFrom, lblFromError, message);
            lblTo.ForeColor = valid ? InkColour : RedColour;
            dtpTo.AccessibleDescription = valid ? null : message;
            return valid;
        }

        private bool CheckSessionNumber()
        {
            bool valid = CheckField(SessionNumberValid, txtSessionNumber, lblSessionNumber, lblSessionNumberError,
                "Use the format yyyy-NNNN.");
            lblSessionNumberNote.Visible = valid && lblSessionNumberNote.Text.Length > 0;
            return valid;
        }

        private bool CheckOutputFormat()
        {
            return CheckField(OutputFormatValid, cboOutputFormat, lblOutputFormat, lblOutputFormatError,
                "Choose an output format for this report.");
        }

        private bool CheckSaveTo()
        {
            return CheckField(SaveToValid, txtSaveTo, lblSaveTo, lblSaveToError,
                "Save to: choose a folder with Browse, or type a full folder path.");
        }

        /// <summary>
        /// Shows or clears a field message (spec 2.2.7): red text with a warning glyph under the
        /// control, a red tint on the control and a red label.
        /// </summary>
        private static bool CheckField(bool valid, Control input, Label fieldLabel, Label messageLabel, string message)
        {
            if (!(input is DateTimePicker))
            {
                input.BackColor = valid ? SystemColors.Window : RedTint;
            }

            fieldLabel.ForeColor = valid ? InkColour : RedColour;
            messageLabel.Text = valid ? string.Empty : "⚠ " + message;
            messageLabel.Visible = !valid;
            input.AccessibleDescription = valid ? null : message;
            return valid;
        }

        private static void ClearError(Control input, Label fieldLabel, Label messageLabel)
        {
            if (!(input is DateTimePicker))
            {
                input.BackColor = SystemColors.Window;
            }

            fieldLabel.ForeColor = input.Enabled ? InkColour : MutedColour;
            messageLabel.Visible = false;
        }

        private void txtSessionNumber_Leave(object sender, EventArgs e)
        {
            if (txtSessionNumber.Enabled && txtSessionNumber.Text.Trim().Length > 0) CheckSessionNumber();
        }

        private void cboOutputFormat_Leave(object sender, EventArgs e)
        {
            if (lblOutputFormatError.Visible) CheckOutputFormat();
        }

        private void txtSaveTo_Leave(object sender, EventArgs e)
        {
            CheckSaveTo();
        }

        /// <summary>Any parameter change re-checks readiness (and clears a period error once fixed).</summary>
        private void Parameter_Changed(object sender, EventArgs e)
        {
            if (loading)
            {
                return;
            }

            if ((sender == dtpFrom || sender == dtpTo) && lblFromError.Visible)
            {
                CheckPeriod();
            }

            UpdateReadiness();
        }

        /// <summary>
        /// Updates the readiness panel: amber with the count of outstanding entries, or green with
        /// "Ready to generate" once every requirement for the chosen report is met.
        /// </summary>
        private void UpdateReadiness()
        {
            if (loading)
            {
                return;
            }

            int outstanding = 0;
            if (!PeriodValid) outstanding++;
            if (!SessionNumberValid) outstanding++;
            if (!OutputFormatValid) outstanding++;
            if (!SaveToValid) outstanding++;

            lblReadyReport.Text = SelectedReportName;
            LoadScope();
            bool ready = outstanding == 0;
            pnlReadiness.BackColor = ready ? GreenTint : Color.White;
            pnlReadinessBar.BackColor = ready ? GreenColour : AmberColour;
            lblReadyChip.BackColor = ready ? GreenTint : AmberTint;
            lblReadyChip.ForeColor = ready ? GreenColour : InkColour;
            lblReadyChip.Text = ready ? "✓ Ready to generate"
                                      : "! " + outstanding + (outstanding == 1 ? " entry" : " entries") + " to complete";
        }

        /// <summary>Checks every required field for the chosen report and returns the messages.</summary>
        private List<string> ValidateAll()
        {
            var problems = new List<string>();
            if (!CheckPeriod()) problems.Add(lblFromError.Text);
            if (!CheckSessionNumber()) problems.Add(lblSessionNumberError.Text);
            if (!CheckOutputFormat()) problems.Add(lblOutputFormatError.Text);
            if (!CheckSaveTo()) problems.Add(lblSaveToError.Text);
            UpdateReadiness();
            if (problems.Count > 0)
            {
                SetStatus(problems.Count + (problems.Count == 1 ? " entry" : " entries") +
                          " to correct before the report can be generated. See the fields marked in red.", RedColour);
            }

            return problems;
        }

        #endregion

        #region Buttons

        /// <summary>Generate report → validate, confirm, generate (BLL checks the role and logs ExportReport).</summary>
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (ValidateAll().Count > 0)
            {
                FocusFirstError();
                return;
            }

            string message = "Generate the " + SelectedReportName + " as " + cboOutputFormat.Text + " and save it to " +
                             txtSaveTo.Text.Trim() + "? The export is recorded against your account.";
            using (var dialog = new ConfirmationDialog("Generate report", message, "Generate report"))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }
            }

            try
            {
                GenerateReport();
            }
            catch (UnauthorizedAccessException)
            {
                SetStatus("The report could not be saved to " + txtSaveTo.Text.Trim() + ". Choose another folder.", RedColour);
                return;
            }
            catch (Exception)
            {
                SetStatus("The report could not be generated. Please try again.", RedColour);
                return;
            }

            SetStatus(SelectedReportName + " generated and saved to " + txtSaveTo.Text.Trim() + ".", GreenColour);
        }

        /// <summary>Preview → validate the parameters, then show the report without saving or logging it.</summary>
        private void btnPreview_Click(object sender, EventArgs e)
        {
            bool valid = CheckPeriod() & CheckSessionNumber();
            UpdateReadiness();
            if (!valid)
            {
                SetStatus("Correct the fields marked in red before previewing.", RedColour);
                FocusFirstError();
                return;
            }

            try
            {
                PreviewReport();
                SetStatus("Preview of the " + SelectedReportName + ": nothing is saved or recorded.", BlueColour);
            }
            catch (Exception)
            {
                SetStatus("The preview could not be built. Please try again.", RedColour);
            }
        }

        private void FocusFirstError()
        {
            if (lblFromError.Visible) dtpFrom.Focus();
            else if (lblSessionNumberError.Visible) txtSessionNumber.Focus();
            else if (lblOutputFormatError.Visible) cboOutputFormat.Focus();
            else if (lblSaveToError.Visible) txtSaveTo.Focus();
        }

        /// <summary>Browse → choose the Save to folder (captures nothing else).</summary>
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Choose the folder to save the report in";
                folderDialog.ShowNewFolderButton = true;
                if (Directory.Exists(txtSaveTo.Text.Trim()))
                {
                    folderDialog.SelectedPath = txtSaveTo.Text.Trim();
                }

                if (folderDialog.ShowDialog(this) == DialogResult.OK)
                {
                    txtSaveTo.Text = folderDialog.SelectedPath;
                    CheckSaveTo();
                }
            }
        }

        /// <summary>Cancel → back to the role's dashboard; nothing is captured.</summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            OpenDashboard();
        }

        #endregion

        #region Navigation rail

        private void btnNavDashboard_Click(object sender, EventArgs e)
        {
            OpenDashboard();
        }

        /// <summary>Opens UI 12 for an Administrator or UI 9 for a Career Advisor.</summary>
        private void OpenDashboard()
        {
            OpenScreen(role == AdministratorRole
                ? (Form)new AdminDashboardForm(homeForm, userId, fullName, role)
                : new AdvisorDashboardForm(homeForm, userId, fullName, role));
        }

        /// <summary>Sessions → UI 10 (Career Advisors only; hidden otherwise).</summary>
        private void btnNavSessions_Click(object sender, EventArgs e)
        {
            if (role == CareerAdvisorRole)
            {
                OpenScreen(new SessionsDashboardForm(homeForm, userId, fullName, role));
            }
        }

        /// <summary>Reference data → UI 13 (Administrators only; hidden otherwise).</summary>
        private void btnNavReferenceData_Click(object sender, EventArgs e)
        {
            if (role == AdministratorRole)
            {
                OpenScreen(new ReferenceDataForm(homeForm, userId, fullName, role));
            }
        }

        /// <summary>Reports: this screen is already Reports.</summary>
        private void btnNavReports_Click(object sender, EventArgs e)
        {
            SetStatus(SignedInText, DeepNavy);
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

        /// <summary>Shows the next staff screen and closes this one (staff navigation pattern).</summary>
        private void OpenScreen(Form next)
        {
            next.Show();
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
        private void ReportGenerationForm_FormClosing(object sender, FormClosingEventArgs e)
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
        private void ReportGenerationForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!navigating && !homeForm.Visible)
            {
                Application.Exit();
            }
        }

        #endregion
    }
}
