
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FuturePath.ApplicationLayer;

namespace FuturePath.PresentationLayer
{
    // Handles report selection, generation and previews
    // Supports learner summaries, degree interest and eligibility reports
    public partial class StaffConsoleForm
    {
        private const string ReportsLearnerSummaryCode = "FP-LSR";
        private const string ReportsDegreeInterestCode = "FP-DIE";
        private const string ReportsNotEligibleCode = "FP-LER";

        // Prevent change events while setting report parameters
        private bool reportsLoading = true;

        // Initialise report settings or refresh the readiness panel
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

        // Get the code of the selected report
        private string ReportsSelectedCode
        {
            get
            {
                if (rdoLearnerSummary.Checked) return ReportsLearnerSummaryCode;
                if (rdoNotEligible.Checked) return ReportsNotEligibleCode;
                return ReportsDegreeInterestCode;
            }
        }

        // Get the name of the selected report
        private string ReportsSelectedName
        {
            get
            {
                if (rdoLearnerSummary.Checked) return "Learner Advisory Summary (FP-LSR)";
                if (rdoNotEligible.Checked) return "Learners Not Currently Eligible (FP-LER)";
                return "Degree Interest and Eligibility Report (FP-DIE)";
            }
        }

        // Apply report filters passed from the Advisory reports screen
        private void ReportsApplyFilter(string reportCode, DateTime from, DateTime to, int facultyId, int gradeLevelId, string outcome)
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

        // Keep a date within the allowed picker range
        private static DateTime ReportsClamp(DateTime value, DateTimePicker picker)
        {
            if (value < picker.MinDate) return picker.MinDate;
            if (value > picker.MaxDate) return picker.MaxDate;
            return value;
        }

        // Select a value in a bound dropdown
        private static void ReportsSelectValue(ComboBox combo, object value)
        {
            if (combo.DataSource == null)
            {
                combo.SelectedIndex = -1;
                return;
            }

            combo.SelectedValue = value;
        }

        // Update parameters when the selected report changes
        private void ReportChoice_CheckedChanged(object sender, EventArgs e)
        {
            var choice = sender as RadioButton;
            if (reportsLoading || choice == null || !choice.Checked)
            {
                return;
            }

            ReportsApplyRules();
        }

        // Enable or disable parameters based on the selected report
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

            // Clear validation colours from disabled fields
            ReportsClearError(dtpRepFrom, lblRepFrom, lblFromError);
            lblRepTo.ForeColor = dtpRepTo.Enabled ? InkColour : MutedColour;
            ReportsClearError(txtRepSessionNumber, lblRepSessionNumber, lblRepSessionNumberError);

            ReportsLoadExportFormats(code);
            ReportsUpdateReadiness();
        }

        // Update a parameter's enabled state and help text
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

        // Load faculty, grade and outcome filter lists
        private void ReportsLoadLists()
        {
            try
            {
                // Each list includes an option for all records
                ReferenceDataController referenceDataController = new ReferenceDataController();
                cboRepFaculty.DisplayMember = "FacultyName";
                cboRepFaculty.ValueMember = "FacultyID";
                cboRepFaculty.DataSource = referenceDataController.GetFaculties(false, true);
                cboRepGrade.DisplayMember = "GradeDescription";
                cboRepGrade.ValueMember = "GradeLevelID";
                cboRepGrade.DataSource = referenceDataController.GetGradeLevels(true);
                cboRepOutcome.DisplayMember = "OutcomeName";
                cboRepOutcome.ValueMember = "OutcomeCode";
                cboRepOutcome.DataSource = referenceDataController.GetOutcomes(true);
                cboRepFaculty.SelectedIndex = 0;
                cboRepGrade.SelectedIndex = 0;
                cboRepOutcome.SelectedIndex = 0;
            }
            catch (Exception)
            {
                SetStatus("The faculty, grade and outcome lists could not be loaded. Please close and try again.", RedColour, Color.White);
            }
        }

        // Load the export formats available for the selected report
        private void ReportsLoadExportFormats(string reportCode)
        {
            try
            {
                ReportController reportController = new ReportController();
                cboOutputFormat.DisplayMember = "FormatName";
                cboOutputFormat.ValueMember = "ExportFormat";
                cboOutputFormat.DataSource = reportController.GetExportFormats(reportCode);
                cboOutputFormat.SelectedIndex = -1;
            }
            catch (Exception)
            {
                SetStatus("The output formats could not be loaded. Please try again.", RedColour, Color.White);
            }
        }

        // Update the report scope and last generated date
        private void ReportsLoadScope()
        {
            string code = ReportsSelectedCode;
            string scope;
            if (code == ReportsLearnerSummaryCode)
            {
                scope = "One completed advisory session";
                if (ReportsSessionNumber().Length > 0)
                {
                    scope += " · " + ReportsSessionNumber();
                }
            }
            else
            {
                scope = "Sessions " + dtpRepFrom.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) + " to " +
                        dtpRepTo.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            }

            try
            {
                ReportController reportController = new ReportController();

                // Count the sessions, faculties and programmes in scope
                if (code != ReportsLearnerSummaryCode && ReportsPeriodValid)
                {
                    ReportScope counts = reportController.GetScope(UserId, code, dtpRepFrom.Value.Date, dtpRepTo.Value.Date,
                        ReportsSelectedId(cboRepFaculty), ReportsSelectedId(cboRepGrade), ReportsSelectedOutcome(), chkIncludeInactive.Checked);
                    scope = counts.SessionCount + " advisory sessions · " + counts.FacultyCount + " faculties · " +
                            counts.ProgrammeCount + " programmes";
                }

                // Retrieve the most recent export date
                DateTime? last = reportController.GetLastGenerated(code);
                if (last.HasValue)
                {
                    scope += " · last generated " + last.Value.ToString("d MMMM yyyy 'at' HH:mm", CultureInfo.CurrentCulture);
                }
                else
                {
                    scope += " · not generated before";
                }
            }
            catch (Exception)
            {
                scope += " · scope could not be counted";
            }

            lblReadyScope.Text = scope;
        }

        // Find the session ID for a learner advisory summary
        private int ReportsFindSessionId()
        {
            if (ReportsSelectedCode != ReportsLearnerSummaryCode)
            {
                return 0;
            }

            SessionSearchController sessionSearchController = new SessionSearchController();
            int sessionId = sessionSearchController.FindSessionId(ReportsSessionNumber());
            if (sessionId == 0)
            {
                throw new ArgumentException("No advisory session has the number " + ReportsSessionNumber() +
                                            ". Check it on Sessions, then try again.");
            }

            return sessionId;
        }

        // Build the selected report's dataset using the controller
        private DataSet ReportsBuildData(int sessionId)
        {
            ReportController reportController = new ReportController();
            if (ReportsSelectedCode == ReportsLearnerSummaryCode)
            {
                return reportController.GetLearnerSummary(UserId, sessionId);
            }

            if (ReportsSelectedCode == ReportsNotEligibleCode)
            {
                return reportController.BuildNotEligibleReport(UserId, dtpRepFrom.Value.Date, dtpRepTo.Value.Date,
                    ReportsSelectedId(cboRepFaculty), ReportsSelectedId(cboRepGrade));
            }

            return reportController.BuildDegreeInterestReport(UserId, dtpRepFrom.Value.Date, dtpRepTo.Value.Date,
                ReportsSelectedId(cboRepFaculty), ReportsSelectedId(cboRepGrade), ReportsSelectedOutcome(), chkIncludeInactive.Checked);
        }

        // Generate, save and open the selected report
        private void ReportsBuildAndSave(int sessionId)
        {
            DataSet data = ReportsBuildData(sessionId);
            string parameters = ReportsParametersUsed();
            string folder = txtSaveTo.Text.Trim();
            string extension = SelectedCode(cboOutputFormat);
            string path = Path.Combine(folder, ReportsFileStem() + extension);
            Directory.CreateDirectory(folder);
            SaveReport(ReportsSelectedCode, data, path, sessionId, parameters);

            OpenReportViewer(ReportsSelectedCode, data, lblReadyScope.Text, sessionId, parameters,
                             extension, folder, ReportsFileStem(), tabReports);
            viewerSavedPath = path;
        }

        // Preview the selected report without saving or logging it
        private void ReportsBuildPreview(int sessionId)
        {
            DataSet data = ReportsBuildData(sessionId);

            // Use the chosen folder if it already exists
            string folder = string.Empty;
            if (Directory.Exists(txtSaveTo.Text.Trim()))
            {
                folder = txtSaveTo.Text.Trim();
            }

            OpenReportViewer(ReportsSelectedCode, data, lblReadyScope.Text, sessionId, ReportsParametersUsed(),
                             SelectedCode(cboOutputFormat), folder, ReportsFileStem(), tabReports);
        }

        // Build the report's suggested filename
        private string ReportsFileStem()
        {
            if (ReportsSelectedCode == ReportsLearnerSummaryCode)
            {
                return ReportsSelectedCode + "_" + ReportsSessionNumber();
            }

            return ReportsSelectedCode + "_" + dtpRepFrom.Value.ToString("yyyyMMdd", CultureInfo.InvariantCulture) +
                   "-" + dtpRepTo.Value.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        }

        // Build the report parameters for the export record
        private string ReportsParametersUsed()
        {
            ReportController reportController = new ReportController();
            return reportController.BuildParametersUsed(ReportsSelectedCode, dtpRepFrom.Value.Date, dtpRepTo.Value.Date,
                ReportsSelectedId(cboRepFaculty), ReportsSelectedId(cboRepGrade), ReportsSelectedOutcome(), chkIncludeInactive.Checked);
        }

        // Get the selected filter ID or zero when disabled
        private static int ReportsSelectedId(ComboBox combo)
        {
            if (!combo.Enabled)
            {
                return 0;
            }

            return SelectedId(combo);
        }

        // Get the selected outcome or an empty value when disabled
        private string ReportsSelectedOutcome()
        {
            if (!cboRepOutcome.Enabled)
            {
                return string.Empty;
            }

            return SelectedCode(cboRepOutcome);
        }

        // Get the session number entered for a learner summary
        private string ReportsSessionNumber()
        {
            if (!txtRepSessionNumber.Enabled)
            {
                return string.Empty;
            }

            return txtRepSessionNumber.Text.Trim();
        }

        #endregion

        #region Validation (validated again in the BLL)

        // Check that the selected date range is valid
        private bool ReportsPeriodValid
        {
            get { return !dtpRepFrom.Enabled || dtpRepFrom.Value.Date <= dtpRepTo.Value.Date; }
        }

        // Check the learner summary session number format
        private bool ReportsSessionNumberValid
        {
            get { return !txtRepSessionNumber.Enabled || Regex.IsMatch(txtRepSessionNumber.Text.Trim(), @"^\d{4}-\d{4}$"); }
        }

        // Check whether an export format has been selected
        private bool ReportsOutputFormatValid
        {
            get { return cboOutputFormat.SelectedIndex >= 0; }
        }

        // Check whether the output folder path is valid
        private bool ReportsSaveToValid
        {
            get
            {
                string path = txtSaveTo.Text.Trim();
                return path.Length > 0 && path.IndexOfAny(Path.GetInvalidPathChars()) < 0 && Path.IsPathRooted(path);
            }
        }

        // Validate the selected report date range
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

        // Validate the learner summary session number
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

        // Validate the selected export format
        private bool ReportsCheckOutputFormat()
        {
            return CheckField(ReportsOutputFormatValid, cboOutputFormat, lblOutputFormat, lblOutputFormatError,
                "Choose an output format for this report.");
        }

        // Validate the selected output folder
        private bool ReportsCheckSaveTo()
        {
            return CheckField(ReportsSaveToValid, txtSaveTo, lblSaveTo, lblSaveToError,
                "Save to: choose a folder with Browse, or type a full folder path.");
        }

        // Clear a field's validation error and restore its colour
        private static void ReportsClearError(Control input, Label fieldLabel, Label messageLabel)
        {
            if (!(input is DateTimePicker))
            {
                input.BackColor = SystemColors.Window;
            }

            fieldLabel.ForeColor = input.Enabled ? InkColour : MutedColour;
            messageLabel.Visible = false;
        }

        // Validate the session number when leaving the field
        private void txtRepSessionNumber_Leave(object sender, EventArgs e)
        {
            if (txtRepSessionNumber.Enabled && txtRepSessionNumber.Text.Trim().Length > 0) ReportsCheckSessionNumber();
        }

        // Recheck the output format when leaving the dropdown
        private void cboOutputFormat_Leave(object sender, EventArgs e)
        {
            if (lblOutputFormatError.Visible) ReportsCheckOutputFormat();
        }

        // Validate the output folder when leaving its field
        private void txtSaveTo_Leave(object sender, EventArgs e)
        {
            ReportsCheckSaveTo();
        }

        // Refresh report readiness when a parameter changes
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

        // Update the readiness panel based on missing or invalid entries
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

        // Validate all required report fields
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

        // Focus the first report field with an error
        private void ReportsFocusFirstError()
        {
            if (lblFromError.Visible) dtpRepFrom.Focus();
            else if (lblRepSessionNumberError.Visible) txtRepSessionNumber.Focus();
            else if (lblOutputFormatError.Visible) cboOutputFormat.Focus();
            else if (lblSaveToError.Visible) txtSaveTo.Focus();
        }

        #endregion

        #region Button click events

        // Validate, confirm and generate the selected report
        private void btnReportsGenerate_Click(object sender, EventArgs e)
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
                ReportsBuildAndSave(ReportsFindSessionId());
            }
            catch (UnauthorizedAccessException)
            {
                SetStatus("The report could not be saved to " + txtSaveTo.Text.Trim() + ". Choose another folder.", RedColour, Color.White);
                return;
            }
            catch (ArgumentException ex)
            {
                // Display validation errors returned by the controller
                SetStatus(ex.Message, RedColour, Color.White);
                return;
            }
            catch (InvalidOperationException ex)
            {
                // Display errors when the report cannot be generated
                SetStatus(ex.Message, RedColour, Color.White);
                return;
            }
            catch (Exception)
            {
                SetStatus("The report could not be generated. Please try again.", RedColour, Color.White);
                return;
            }

            SetStatus(ReportsSelectedName + " generated and saved to " + txtSaveTo.Text.Trim() + ".", GreenColour, Color.White);
        }

        // Preview the selected report without saving it
        private void btnReportsPreview_Click(object sender, EventArgs e)
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
                ReportsBuildPreview(ReportsFindSessionId());
                SetStatus("Preview of the " + ReportsSelectedName + ": nothing is saved or recorded.", BlueColour, Color.White);
            }
            catch (ArgumentException ex)
            {
                SetStatus(ex.Message, RedColour, Color.White);
            }
            catch (InvalidOperationException ex)
            {
                SetStatus(ex.Message, RedColour, Color.White);
            }
            catch (Exception)
            {
                SetStatus("The preview could not be built. Please try again.", RedColour, Color.White);
            }
        }

        // Open a dialog to select the report output folder
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
