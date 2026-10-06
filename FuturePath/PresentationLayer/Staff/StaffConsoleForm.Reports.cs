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
    /// UI 11 — Reports selection and generation (Career Advisor and Administrator; spec 2.3.12). The user
    /// picks one of the three reports (FP-LSR, FP-DIE, FP-LER), sets its parameters, and chooses an output
    /// format and a folder. Parameters that do not apply to the chosen report are disabled with the reason
    /// stated beneath them, never hidden. A readiness panel names the report and its scope and turns green
    /// once every requirement is met. Validation fires on Generate report; the BLL checks the role again
    /// before generating and logs the export in <c>ExportReport</c> (Section 8).
    /// <para>
    /// Generate report, Preview and Cancel are the console's action-band buttons: the host calls
    /// <see cref="ReportsGenerate"/> and <see cref="ReportsPreview"/>, and Cancel returns to the dashboard.
    /// </para>
    /// </summary>
    public partial class StaffConsoleForm
    {
        private const string ReportsLearnerSummaryCode = "FP-LSR";
        private const string ReportsDegreeInterestCode = "FP-DIE";
        private const string ReportsNotEligibleCode = "FP-LER";

        /// <summary>True while code (not the user) sets the parameters, so change events are ignored.</summary>
        private bool reportsLoading = true;

        /// <summary>
        /// Called by <see cref="ShowPage"/> each time UI 11 appears. The first time, sets the default period
        /// (1 January to today) and folder, binds the lists and applies the rules of the default report
        /// (FP-DIE). Later visits keep what the user entered (the console keeps the screen alive) and only
        /// refresh the readiness panel, so its scope and "last generated" line are current.
        /// </summary>
        /// <param name="firstTime">True the first time UI 11 is shown in this console.</param>
        private void ReportsShown(bool firstTime)
        {
            if (!firstTime)
            {
                ReportsUpdateReadiness();
                return;
            }

            reportsLoading = true;
            dtpRepFrom.Value = new DateTime(DateTime.Today.Year, 1, 1);
            dtpRepTo.Value = DateTime.Today;
            txtSaveTo.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "FuturePath", "Reports");

            ReportsLoadLists();
            reportsLoading = false;
            ReportsApplyRules();
        }

        #region Report choice

        /// <summary>The report code of the selected radio button.</summary>
        private string ReportsSelectedCode
        {
            get
            {
                if (rdoLearnerSummary.Checked) return ReportsLearnerSummaryCode;
                if (rdoNotEligible.Checked) return ReportsNotEligibleCode;
                return ReportsDegreeInterestCode;
            }
        }

        /// <summary>The report name shown in the readiness panel and the confirmation.</summary>
        private string ReportsSelectedName
        {
            get
            {
                if (rdoLearnerSummary.Checked) return "Learner Advisory Summary (FP-LSR)";
                if (rdoNotEligible.Checked) return "Learners Not Currently Eligible (FP-LER)";
                return "Degree Interest and Eligibility Report (FP-DIE)";
            }
        }

        /// <summary>
        /// Fills this screen with the filters of UI 10.3c, whose Generate report opens UI 11 already filtered
        /// (the host's <c>OpenReports</c> shows this page first, so the lists are bound and the IDs can be
        /// selected straight away).
        /// </summary>
        /// <param name="reportCode">"FP-LSR", "FP-DIE" or "FP-LER".</param>
        /// <param name="from">First session date in scope.</param>
        /// <param name="to">Last session date in scope.</param>
        /// <param name="facultyId">Faculty.FacultyID, or null for all faculties.</param>
        /// <param name="gradeLevelId">GradeLevel.GradeLevelID, or null for all grades.</param>
        /// <param name="outcome">RecommendationResult.OutcomeCode, or null for all outcomes.</param>
        private void ReportsApplyFilter(string reportCode, DateTime from, DateTime to, int? facultyId, int? gradeLevelId, string outcome)
        {
            reportsLoading = true;
            rdoLearnerSummary.Checked = reportCode == ReportsLearnerSummaryCode;
            rdoNotEligible.Checked = reportCode == ReportsNotEligibleCode;
            rdoDegreeInterest.Checked = reportCode != ReportsLearnerSummaryCode && reportCode != ReportsNotEligibleCode;

            if (from > to)
            {
                DateTime swap = from;
                from = to;
                to = swap;
            }

            dtpRepFrom.Value = ReportsClamp(from.Date, dtpRepFrom);
            dtpRepTo.Value = ReportsClamp(to.Date, dtpRepTo);
            ReportsSelectValue(cboRepFaculty, facultyId);
            ReportsSelectValue(cboRepGrade, gradeLevelId);
            ReportsSelectValue(cboRepOutcome, outcome);
            reportsLoading = false;

            ReportsApplyRules();
            SetStatus("Filters copied from Advisory reports (UI 10.3c). Check them, then generate the report.", BlueColour, Color.White);
        }

        /// <summary>Keeps a date inside the range a DateTimePicker accepts.</summary>
        private static DateTime ReportsClamp(DateTime value, DateTimePicker picker)
        {
            if (value < picker.MinDate) return picker.MinDate;
            if (value > picker.MaxDate) return picker.MaxDate;
            return value;
        }

        /// <summary>Selects a value in a bound list; an unbound list or a null value selects nothing.</summary>
        private static void ReportsSelectValue(ComboBox combo, object value)
        {
            if (value == null || combo.DataSource == null || string.IsNullOrEmpty(combo.ValueMember))
            {
                combo.SelectedIndex = -1;
                return;
            }

            combo.SelectedValue = value;
        }

        /// <summary>A different report was chosen: re-apply the parameter rules for it.</summary>
        private void ReportChoice_CheckedChanged(object sender, EventArgs e)
        {
            var choice = sender as RadioButton;
            if (reportsLoading || choice == null || !choice.Checked)
            {
                return;
            }

            ReportsApplyRules();
        }

        /// <summary>
        /// Enables the parameters the chosen report uses and disables the rest, stating the reason
        /// under each disabled one (spec 2.3.12: disabled, never hidden). FP-LSR is regenerated per
        /// session, so it takes a session number instead of the period and filters.
        /// </summary>
        private void ReportsApplyRules()
        {
            string code = ReportsSelectedCode;
            bool summary = code == ReportsLearnerSummaryCode;
            bool exceptions = code == ReportsNotEligibleCode;

            ReportsSetParameter(dtpRepFrom, lblRepFrom, null, !summary, null);
            ReportsSetParameter(dtpRepTo, lblRepTo, null, !summary, null);
            lblRepFromStar.Visible = !summary;
            lblRepToStar.Visible = !summary;
            lblPeriodNote.Text = summary ? "FP-LSR covers one session: enter its session number instead." : string.Empty;
            lblPeriodNote.Visible = summary;

            ReportsSetParameter(cboRepFaculty, lblRepFaculty, lblFacultyNote, !summary,
                summary ? "Not used: FP-LSR covers one session" : "Optional filter");
            ReportsSetParameter(cboRepGrade, lblRepGrade, lblGradeNote, !summary,
                summary ? "Not used for FP-LSR" : "Optional filter");
            ReportsSetParameter(cboRepOutcome, lblRepOutcome, lblOutcomeNote, !summary && !exceptions,
                summary ? "Not used: FP-LSR covers one session"
                        : exceptions ? "Exception report filters on outcome automatically" : "Optional filter");
            ReportsSetParameter(txtRepSessionNumber, lblRepSessionNumber, lblSessionNumberNote, summary,
                summary ? "Required, e.g. 2026-0042" : "Only for FP-LSR");
            lblSessionNumberStar.Visible = summary;

            bool inactiveApplies = code == ReportsDegreeInterestCode;
            if (!inactiveApplies)
            {
                chkIncludeInactive.Checked = false;
            }

            ReportsSetParameter(chkIncludeInactive, null, lblIncludeInactiveNote, inactiveApplies,
                summary ? "Never for learner-facing output"
                        : exceptions ? "FP-LER lists active programmes only" : "Staff-only report; off by default");

            // A disabled field cannot stay red.
            ReportsClearError(dtpRepFrom, lblRepFrom, lblFromError);
            lblRepTo.ForeColor = dtpRepTo.Enabled ? InkColour : MutedColour;
            ReportsClearError(txtRepSessionNumber, lblRepSessionNumber, lblRepSessionNumberError);

            ReportsLoadExportFormats(code);
            ReportsUpdateReadiness();
        }

        /// <summary>Enables or disables one parameter and writes its caption (muted, never red).</summary>
        private static void ReportsSetParameter(Control input, Label fieldLabel, Label noteLabel, bool enabled, string note)
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
        private void ReportsLoadLists()
        {
            try
            {
                // TODO BLL: cboRepFaculty.DataSource = ReferenceDataController.GetFaculties(includeAllRow: true);
                //           cboRepFaculty.DisplayMember = "FacultyName"; cboRepFaculty.ValueMember = "FacultyID";
                //           // the BLL puts an "All faculties" row (FacultyID null) first
                // TODO BLL: cboRepGrade.DataSource = ReferenceDataController.GetGradeLevels(includeAllRow: true);
                //           cboRepGrade.DisplayMember = "GradeDescription"; cboRepGrade.ValueMember = "GradeLevelID";
                // TODO BLL: cboRepOutcome.DataSource = ReferenceDataController.GetOutcomes(includeAllRow: true);
                //           cboRepOutcome.DisplayMember = "OutcomeName"; cboRepOutcome.ValueMember = "OutcomeCode";
                //           // no Outcome table exists: proposed SystemParameter OUTCOME_1..4 (RecommendationResult.OutcomeCode)
                cboRepFaculty.SelectedIndex = -1;
                cboRepGrade.SelectedIndex = -1;
                cboRepOutcome.SelectedIndex = -1;
            }
            catch (Exception)
            {
                SetStatus("The faculty, grade and outcome lists could not be loaded. Please close and try again.", RedColour, Color.White);
            }
        }

        /// <summary>Binds the output formats allowed for the chosen report (reports.md: LSR PDF/Word, DIE PDF/Excel, LER PDF/Word).</summary>
        private void ReportsLoadExportFormats(string reportCode)
        {
            try
            {
                // TODO BLL: cboOutputFormat.DataSource = ReportController.GetExportFormats(reportCode);
                //           cboOutputFormat.DisplayMember = "FormatName"; cboOutputFormat.ValueMember = "ExportFormat";
                cboOutputFormat.SelectedIndex = -1;
            }
            catch (Exception)
            {
                SetStatus("The output formats could not be loaded. Please try again.", RedColour, Color.White);
            }
        }

        /// <summary>
        /// Refreshes the scope line of the readiness panel: sessions in scope, faculties,
        /// programmes and the last generated date (from <c>ExportReport</c>).
        /// </summary>
        private void ReportsLoadScope()
        {
            string code = ReportsSelectedCode;
            string scope = code == ReportsLearnerSummaryCode
                ? "One completed advisory session" + (txtRepSessionNumber.Text.Trim().Length > 0 ? " · " + txtRepSessionNumber.Text.Trim() : string.Empty)
                : "Sessions " + dtpRepFrom.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) + " to " +
                  dtpRepTo.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) +
                  " · " + (cboRepFaculty.SelectedIndex >= 0 ? cboRepFaculty.Text : "all faculties") +
                  " · " + (cboRepGrade.SelectedIndex >= 0 ? cboRepGrade.Text : "all grades");
            try
            {
                // TODO BLL: var s = ReportController.GetScope(code, ReportsSessionNumberOrNull(), dtpRepFrom.Value.Date, dtpRepTo.Value.Date,
                //               ReportsSelectedId(cboRepFaculty), ReportsSelectedId(cboRepGrade), ReportsSelectedOutcome(), chkIncludeInactive.Checked);
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
        /// (the screen's role is never trusted), builds the typed dataset, writes the file and logs an
        /// <c>ExportReport</c> row (ReportCode, SessionID or ParametersUsed, GeneratedByUserID,
        /// DateGenerated, ExportFormat).
        /// </summary>
        private void ReportsBuildAndSave()
        {
            // TODO BLL: var report = ReportController.Generate(UserId, ReportsSelectedCode, ReportsSessionNumberOrNull(),
            //               dtpRepFrom.Value.Date, dtpRepTo.Value.Date, ReportsSelectedId(cboRepFaculty), ReportsSelectedId(cboRepGrade),
            //               ReportsSelectedOutcome(), chkIncludeInactive.Checked, (string)cboOutputFormat.SelectedValue, txtSaveTo.Text.Trim());
            // TODO Stage 10: open the report viewer (UI 11.1a / 11.2b / 11.3c) for report — the RDLC ReportViewer is deferred.
        }

        /// <summary>Builds the preview of the chosen report without saving or logging anything.</summary>
        private void ReportsBuildPreview()
        {
            // TODO BLL: var data = ReportController.BuildPreview(UserId, ReportsSelectedCode, ReportsSessionNumberOrNull(),
            //               dtpRepFrom.Value.Date, dtpRepTo.Value.Date, ReportsSelectedId(cboRepFaculty), ReportsSelectedId(cboRepGrade),
            //               ReportsSelectedOutcome(), chkIncludeInactive.Checked);
            // TODO Stage 10: show data in the report viewer (UI 11.1a / 11.2b / 11.3c) — nothing is written or logged.
        }

        /// <summary>The selected ID of an enabled, bound list, or null for "all".</summary>
        private static int? ReportsSelectedId(ComboBox combo)
        {
            if (!combo.Enabled || combo.SelectedIndex < 0 || !(combo.SelectedValue is int))
            {
                return null;
            }

            return (int)combo.SelectedValue;
        }

        /// <summary>The selected outcome code, or null when the filter is off or not used.</summary>
        private string ReportsSelectedOutcome()
        {
            return cboRepOutcome.Enabled && cboRepOutcome.SelectedIndex >= 0 ? cboRepOutcome.SelectedValue as string : null;
        }

        /// <summary>The session number for FP-LSR, otherwise null.</summary>
        private string ReportsSessionNumberOrNull()
        {
            return txtRepSessionNumber.Enabled ? txtRepSessionNumber.Text.Trim() : null;
        }

        #endregion

        #region Validation (validated again in the BLL)

        /// <summary>True when the period is not used (FP-LSR) or Sessions from is on or before Sessions to.</summary>
        private bool ReportsPeriodValid
        {
            get { return !dtpRepFrom.Enabled || dtpRepFrom.Value.Date <= dtpRepTo.Value.Date; }
        }

        /// <summary>True when the session number is not used, or is in the format yyyy-NNNN.</summary>
        private bool ReportsSessionNumberValid
        {
            get { return !txtRepSessionNumber.Enabled || Regex.IsMatch(txtRepSessionNumber.Text.Trim(), @"^\d{4}-\d{4}$"); }
        }

        /// <summary>True when an output format is chosen.</summary>
        private bool ReportsOutputFormatValid
        {
            get { return cboOutputFormat.SelectedIndex >= 0; }
        }

        /// <summary>True when Save to holds a full folder path without invalid characters.</summary>
        private bool ReportsSaveToValid
        {
            get
            {
                string path = txtSaveTo.Text.Trim();
                return path.Length > 0 && path.IndexOfAny(Path.GetInvalidPathChars()) < 0 && Path.IsPathRooted(path);
            }
        }

        /// <summary>
        /// Checks the period. One message shows under both pickers and both labels turn red (a
        /// DateTimePicker cannot be tinted). When the period is not used (FP-LSR) its labels stay muted.
        /// </summary>
        private bool ReportsCheckPeriod()
        {
            if (!dtpRepFrom.Enabled)
            {
                ReportsClearError(dtpRepFrom, lblRepFrom, lblFromError);
                lblRepTo.ForeColor = MutedColour;
                return true;
            }

            const string message = "Sessions from must be on or before Sessions to.";
            bool valid = ReportsPeriodValid;
            CheckField(valid, dtpRepFrom, lblRepFrom, lblFromError, message);
            lblRepTo.ForeColor = valid ? InkColour : RedColour;
            dtpRepTo.AccessibleDescription = valid ? null : message;
            return valid;
        }

        /// <summary>Checks the FP-LSR session number; when it is not used, its label stays muted.</summary>
        private bool ReportsCheckSessionNumber()
        {
            if (!txtRepSessionNumber.Enabled)
            {
                ReportsClearError(txtRepSessionNumber, lblRepSessionNumber, lblRepSessionNumberError);
                return true;
            }

            bool valid = CheckField(ReportsSessionNumberValid, txtRepSessionNumber, lblRepSessionNumber, lblRepSessionNumberError,
                "Use the format yyyy-NNNN.");
            lblSessionNumberNote.Visible = valid && lblSessionNumberNote.Text.Length > 0;
            return valid;
        }

        /// <summary>Checks that an output format is chosen.</summary>
        private bool ReportsCheckOutputFormat()
        {
            return CheckField(ReportsOutputFormatValid, cboOutputFormat, lblOutputFormat, lblOutputFormatError,
                "Choose an output format for this report.");
        }

        /// <summary>Checks the Save to folder.</summary>
        private bool ReportsCheckSaveTo()
        {
            return CheckField(ReportsSaveToValid, txtSaveTo, lblSaveTo, lblSaveToError,
                "Save to: choose a folder with Browse, or type a full folder path.");
        }

        /// <summary>Clears a field message and gives the label its normal colour (ink, or muted when disabled).</summary>
        private static void ReportsClearError(Control input, Label fieldLabel, Label messageLabel)
        {
            if (!(input is DateTimePicker))
            {
                input.BackColor = SystemColors.Window;
            }

            fieldLabel.ForeColor = input.Enabled ? InkColour : MutedColour;
            messageLabel.Visible = false;
        }

        /// <summary>Session number: checked when the user leaves it with something typed.</summary>
        private void txtRepSessionNumber_Leave(object sender, EventArgs e)
        {
            if (txtRepSessionNumber.Enabled && txtRepSessionNumber.Text.Trim().Length > 0) ReportsCheckSessionNumber();
        }

        /// <summary>Output format: re-checked on leave once its message is showing.</summary>
        private void cboOutputFormat_Leave(object sender, EventArgs e)
        {
            if (lblOutputFormatError.Visible) ReportsCheckOutputFormat();
        }

        /// <summary>Save to: checked on leave.</summary>
        private void txtSaveTo_Leave(object sender, EventArgs e)
        {
            ReportsCheckSaveTo();
        }

        /// <summary>Any parameter change re-checks readiness (and clears a period error once fixed).</summary>
        private void Parameter_Changed(object sender, EventArgs e)
        {
            if (reportsLoading)
            {
                return;
            }

            if ((sender == dtpRepFrom || sender == dtpRepTo) && lblFromError.Visible)
            {
                ReportsCheckPeriod();
            }

            ReportsUpdateReadiness();
        }

        /// <summary>
        /// Updates the readiness panel: amber with the count of outstanding entries, or green with
        /// "Ready to generate" once every requirement for the chosen report is met.
        /// </summary>
        private void ReportsUpdateReadiness()
        {
            if (reportsLoading)
            {
                return;
            }

            int outstanding = 0;
            if (!ReportsPeriodValid) outstanding++;
            if (!ReportsSessionNumberValid) outstanding++;
            if (!ReportsOutputFormatValid) outstanding++;
            if (!ReportsSaveToValid) outstanding++;

            lblReadyReport.Text = ReportsSelectedName;
            ReportsLoadScope();
            bool ready = outstanding == 0;
            pnlReadiness.BackColor = ready ? GreenTint : Color.White;
            pnlReadinessBar.BackColor = ready ? GreenColour : AmberColour;
            lblReadyChip.BackColor = ready ? GreenTint : AmberTint;
            lblReadyChip.ForeColor = ready ? GreenColour : InkColour;
            lblReadyChip.Text = ready ? "✓ Ready to generate"
                                      : "! " + outstanding + (outstanding == 1 ? " entry" : " entries") + " to complete";
        }

        /// <summary>Checks every required field for the chosen report and returns the messages.</summary>
        private List<string> ReportsValidateAll()
        {
            var problems = new List<string>();
            if (!ReportsCheckPeriod()) problems.Add(lblFromError.Text);
            if (!ReportsCheckSessionNumber()) problems.Add(lblRepSessionNumberError.Text);
            if (!ReportsCheckOutputFormat()) problems.Add(lblOutputFormatError.Text);
            if (!ReportsCheckSaveTo()) problems.Add(lblSaveToError.Text);
            ReportsUpdateReadiness();
            if (problems.Count > 0)
            {
                SetStatus(problems.Count + (problems.Count == 1 ? " entry" : " entries") +
                          " to correct before the report can be generated. See the fields marked in red.", RedColour, Color.White);
            }

            return problems;
        }

        /// <summary>Moves focus to the first field with a message showing.</summary>
        private void ReportsFocusFirstError()
        {
            if (lblFromError.Visible) dtpRepFrom.Focus();
            else if (lblRepSessionNumberError.Visible) txtRepSessionNumber.Focus();
            else if (lblOutputFormatError.Visible) cboOutputFormat.Focus();
            else if (lblSaveToError.Visible) txtSaveTo.Focus();
        }

        #endregion

        #region Buttons

        /// <summary>
        /// Generate report (action band) → validate, confirm, generate. The BLL checks the role and logs
        /// the export in ExportReport.
        /// </summary>
        private void ReportsGenerate()
        {
            if (ReportsValidateAll().Count > 0)
            {
                ReportsFocusFirstError();
                return;
            }

            string message = "Generate the " + ReportsSelectedName + " as " + cboOutputFormat.Text + " and save it to " +
                             txtSaveTo.Text.Trim() + "? The export is recorded against your account.";
            if (!Confirm("Generate report", message, "Generate report"))
            {
                return;
            }

            try
            {
                ReportsBuildAndSave();
            }
            catch (UnauthorizedAccessException)
            {
                SetStatus("The report could not be saved to " + txtSaveTo.Text.Trim() + ". Choose another folder.", RedColour, Color.White);
                return;
            }
            catch (Exception)
            {
                SetStatus("The report could not be generated. Please try again.", RedColour, Color.White);
                return;
            }

            SetStatus(ReportsSelectedName + " generated and saved to " + txtSaveTo.Text.Trim() + ".", GreenColour, Color.White);
        }

        /// <summary>Preview (action band) → validate the parameters, then show the report without saving or logging it.</summary>
        private void ReportsPreview()
        {
            bool valid = ReportsCheckPeriod() & ReportsCheckSessionNumber();
            ReportsUpdateReadiness();
            if (!valid)
            {
                SetStatus("Correct the fields marked in red before previewing.", RedColour, Color.White);
                ReportsFocusFirstError();
                return;
            }

            try
            {
                ReportsBuildPreview();
                SetStatus("Preview of the " + ReportsSelectedName + ": nothing is saved or recorded.", BlueColour, Color.White);
            }
            catch (Exception)
            {
                SetStatus("The preview could not be built. Please try again.", RedColour, Color.White);
            }
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
                    ReportsCheckSaveTo();
                }
            }
        }

        #endregion
    }
}
