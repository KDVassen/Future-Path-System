using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 7 — Advisory Summary and export (wide, step 6 of 6; spec 2.3.9, report FP-LSR).
    /// The learner sets only the export format and the destination folder (both required). The file
    /// name, generation date and the preview are system-supplied: every preview line is assembled
    /// from the stored session, alternatives appear only if ticked on UI 6.1a and funding only if
    /// added on UI 6.2b. <b>Save and export</b> validates, asks for confirmation, writes the file,
    /// completes the session and logs the export. An unusable folder raises a dialog that names the
    /// folder; success is shown in the status band, never in a modal. Back and Cancel write nothing.
    /// </summary>
    public partial class AdvisorySummaryForm : Form
    {
        // Colours and preview fonts used at runtime (spec 2.2.6). Everything else is set in the Designer.
        private static readonly Color InkColour = Color.FromArgb(27, 31, 36);
        private static readonly Color MutedColour = Color.FromArgb(88, 96, 108);
        private static readonly Color RedColour = Color.FromArgb(164, 38, 44);
        private static readonly Color RedTint = Color.FromArgb(250, 234, 235);
        private static readonly Color BlueColour = Color.FromArgb(31, 95, 168);
        private static readonly Color BlueTint = Color.FromArgb(232, 238, 246);
        private static readonly Color AmberColour = Color.FromArgb(200, 137, 43);
        private static readonly Color GreenColour = Color.FromArgb(31, 107, 58);
        private static readonly Color DeepNavy = Color.FromArgb(11, 31, 58);
        private static readonly Font PreviewTitleFont = new Font("Segoe UI Semibold", 12F);
        private static readonly Font PreviewSectionFont = new Font("Segoe UI Semibold", 10F);
        private static readonly Font PreviewBodyFont = new Font("Segoe UI", 9F);
        private static readonly Font PreviewCaptionFont = new Font("Segoe UI", 8F);

        private const string ReadyStatus = "Step 6 of 6 · Choose an export format and a folder, then save and export.";
        private const string GuidanceText =
            "Saving the advisory summary completes this session: your profile, subjects, marks, questionnaire " +
            "responses and advisory outcome are kept on record as a completed session.";
        private const string Disclaimer =
            "Preliminary guidance only: Future Path does not make admission decisions, and the requirements used are " +
            "sample data held in this system. Confirm every requirement with the institution before applying.";

        private readonly Form homeForm;

        /// <summary>True once the summary has been saved; the primary button then becomes Finish.</summary>
        private bool exported;
        private DateTime? generatedOn;

        // Values read from the stored session for the file name and the preview (filled by the BLL).
        private string surname;
        private string sessionNumber;
        private string outcomeCategory;
        private string outcomeReason;
        private string subjectsLine;
        private string interestsLine;
        private string alternativesLine;
        private string fundingLine;
        private bool includeAlternatives;
        private bool includeFunding;
        private int? pageCount;

        /// <summary>Creates the Advisory Summary screen. Only stores its arguments.</summary>
        /// <param name="homeForm">UI 1, shown again when the session ends or is left.</param>
        /// <param name="previousForm">The screen that opened this one (UI 6, 6.1a or 6.2b).</param>
        public AdvisorySummaryForm(Form homeForm, Form previousForm)
        {
            InitializeComponent();
            this.homeForm = homeForm;
            PreviousForm = previousForm;

            txtGeneratedOn.Text = DateTime.Today.ToString("dd MMMM yyyy", CultureInfo.CurrentCulture);
            ShowGuidance();
            SetStatus(ReadyStatus, DeepNavy, Color.White);
            LoadExportFormats();
        }

        #region Session values handed on by the previous screen

        /// <summary>The screen Back returns to (set by <c>EligibilityResultsForm.OpenSummary</c>).</summary>
        public Form PreviousForm { get; set; }

        /// <summary>AdvisorySession.SessionID of the session in progress.</summary>
        public int? SessionId { get; set; }

        /// <summary>Learner.LearnerID of the learner in this session.</summary>
        public int? LearnerId { get; set; }

        /// <summary>The learner context strip text.</summary>
        public string LearnerContext { get; set; }

        /// <summary>AdvisorySession.PreferredFacultyID chosen on UI 5.</summary>
        public int? PreferredFacultyId { get; set; }

        /// <summary>AdvisorySession.SelectedProgrammeID chosen on UI 5.</summary>
        public int? SelectedProgrammeId { get; set; }

        /// <summary>DegreeProgramme.ProgrammeName of the learner's selection.</summary>
        public string SelectedProgrammeName { get; set; }

        #endregion

        #region Business Logic Layer hooks

        /// <summary>Binds the export formats offered for FP-LSR (PDF / Word) — never literal Items.</summary>
        private void LoadExportFormats()
        {
            try
            {
                // TODO BLL: cboExportFormat.DataSource = ReportController.GetExportFormats("FP-LSR");
                //           cboExportFormat.DisplayMember = "FormatName";      // e.g. "PDF document (.pdf)"
                //           cboExportFormat.ValueMember = "FileExtension";     // e.g. ".pdf"
                cboExportFormat.SelectedIndex = -1;
            }
            catch (Exception)
            {
                SetStatus("The export formats could not be loaded. Please go back and try again.", AmberColour, InkColour);
            }
        }

        /// <summary>
        /// Reads the stored session that the preview and the file name are built from. The outcome is
        /// read back, never recalculated; alternatives and funding flags come from the session row.
        /// </summary>
        private void LoadSummary()
        {
            // Cleared first, so a failed read never shows the previous values.
            surname = sessionNumber = outcomeCategory = outcomeReason = null;
            subjectsLine = interestsLine = alternativesLine = fundingLine = null;
            includeAlternatives = includeFunding = false;
            pageCount = null;
            try
            {
                // TODO BLL: var summary = ReportController.GetLearnerSummary(SessionId.Value);   // FP-LSR data set
                //           surname = summary.LastName; sessionNumber = summary.SessionNumber;
                //           outcomeCategory = summary.OutcomeCode; outcomeReason = summary.OutcomeReason;
                //           subjectsLine = summary.SubjectsAndMarks;     // by subject name, ascending
                //           interestsLine = summary.InterestResults;     // by score, descending
                //           includeAlternatives = summary.AlternativesInSummary;
                //           alternativesLine = summary.Alternatives;     // by OverallScore, descending
                //           includeFunding = summary.FundingInfoRequested;
                //           fundingLine = summary.Funding;               // by ClosingDate, ascending
                //           pageCount = summary.PageCount;
            }
            catch (Exception)
            {
                SetStatus("Your stored session could not be read for the preview. Please go back and try again.", AmberColour, InkColour);
            }

            UpdateFileName();
            BuildPreview();
        }

        /// <summary>
        /// Writes the FP-LSR file, sets SessionStatus = 'Completed' and DateCompleted, and writes an
        /// ExportReport row (ReportCode FP-LSR, GeneratedByUserID null — a learner's own export).
        /// </summary>
        private void ExportSummary(string folder, string fileName)
        {
            // TODO BLL: ReportController.ExportLearnerSummary(SessionId.Value, (string)cboExportFormat.SelectedValue,
            //               Path.Combine(folder, fileName), generatedOn.Value, null /* GeneratedByUserID */);
        }

        #endregion

        #region File name and preview (assembled from the stored session)

        /// <summary>Proposes the file name &lt;Surname&gt;_&lt;SessionNumber&gt; with the chosen format's extension.</summary>
        private void UpdateFileName()
        {
            string name = string.IsNullOrEmpty(surname) ? "Learner" : surname.Trim().Replace(' ', '-');
            string number = string.IsNullOrEmpty(sessionNumber) ? "session" : sessionNumber;
            string extension = cboExportFormat.SelectedValue as string ?? string.Empty;
            txtFileName.Text = name + "_" + number + extension;

            string format = cboExportFormat.SelectedIndex >= 0 ? cboExportFormat.Text : "Choose an export format";
            lblPreviewMeta.Text = pageCount.HasValue
                ? format + " · " + pageCount.Value + (pageCount.Value == 1 ? " page" : " pages")
                : format;
        }

        /// <summary>
        /// Builds the read-only preview. There are no section tick boxes: alternatives print only if
        /// ticked on UI 6.1a, funding only if added on UI 6.2b, and everything else always.
        /// </summary>
        private void BuildPreview()
        {
            rtbPreview.Clear();
            AppendPreview("Future Path — Learner Advisory Summary", PreviewTitleFont, InkColour);
            string stamp = generatedOn.HasValue
                ? "Generated " + generatedOn.Value.ToString("dd MMMM yyyy 'at' HH:mm", CultureInfo.CurrentCulture)
                : "Generated when you save and export";
            AppendPreview("Session " + Dash(sessionNumber) + " · " + stamp, PreviewCaptionFont, MutedColour);

            AppendSection("Learner profile", Dash(LearnerContext));
            AppendSection("Selected qualification", Dash(SelectedProgrammeName));
            AppendSection("Subjects and marks", Dash(subjectsLine));
            AppendSection("Interest category results", Dash(interestsLine));
            string outcome = string.IsNullOrEmpty(outcomeCategory) ? "Advisory outcome" : "Advisory outcome — " + outcomeCategory;
            AppendSection(outcome, Dash(outcomeReason));
            if (includeAlternatives)
            {
                AppendSection("Suggested alternative programmes", Dash(alternativesLine));
            }

            if (includeFunding)
            {
                AppendSection("Funding options", Dash(fundingLine));
            }

            AppendPreview(string.Empty, PreviewCaptionFont, MutedColour);
            AppendPreview(Disclaimer, PreviewCaptionFont, MutedColour);
            rtbPreview.SelectionStart = 0;
            rtbPreview.ScrollToCaret();
        }

        private void AppendSection(string heading, string body)
        {
            AppendPreview(string.Empty, PreviewCaptionFont, InkColour);
            AppendPreview(heading, PreviewSectionFont, InkColour);
            AppendPreview(body, PreviewBodyFont, InkColour);
        }

        private void AppendPreview(string text, Font font, Color colour)
        {
            rtbPreview.SelectionStart = rtbPreview.TextLength;
            rtbPreview.SelectionFont = font;
            rtbPreview.SelectionColor = colour;
            rtbPreview.AppendText(text + Environment.NewLine);
        }

        private static string Dash(string value)
        {
            return string.IsNullOrEmpty(value) ? "—" : value;
        }

        #endregion

        #region Field validation (validated again in the BLL on export)

        private bool CheckFormat()
        {
            return CheckField(cboExportFormat.SelectedIndex >= 0, cboExportFormat, lblExportFormat, lblExportFormatError,
                "Export format: choose PDF or Word.");
        }

        private bool CheckFolder()
        {
            return CheckField(txtSaveTo.Text.Trim().Length > 0, txtSaveTo, lblSaveTo, lblSaveToError,
                "Save to: choose the folder to save your summary in.");
        }

        /// <summary>
        /// True when the folder exists and a file can be written there. Checked here so the learner
        /// gets a plain message naming the folder instead of a technical error.
        /// </summary>
        private static bool FolderIsWritable(string folder)
        {
            try
            {
                if (!Path.IsPathRooted(folder) || !Directory.Exists(folder))
                {
                    return false;
                }

                string probe = Path.Combine(folder, "futurepath-" + Guid.NewGuid().ToString("N") + ".tmp");
                File.WriteAllText(probe, string.Empty);
                File.Delete(probe);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>Raises the dialog that names an unusable folder in plain words.</summary>
        private void ShowFolderProblem(string folder)
        {
            CheckField(false, txtSaveTo, lblSaveTo, lblSaveToError, "Save to: this folder cannot be used. Choose another folder.");
            SetStatus("Your summary was not saved. Choose a folder you can save files in.", RedColour, Color.White);
            using (var dialog = new ValidationDialog("Folder cannot be used", new List<string>
            {
                "Future Path could not save your summary in the folder \"" + folder + "\".",
                "Check that the folder exists and that you are allowed to save files there, or use Browse to choose another folder."
            }))
            {
                dialog.ShowDialog(this);
            }

            txtSaveTo.Focus();
        }

        /// <summary>
        /// Shows or clears the field message (spec 2.2.7): red text with a warning glyph under the
        /// control, a red tint on the control and a red label.
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

        private void cboExportFormat_Leave(object sender, EventArgs e) { CheckFormat(); }

        private void txtSaveTo_Leave(object sender, EventArgs e) { CheckFolder(); }

        /// <summary>A new format changes the proposed file extension and the preview line.</summary>
        private void cboExportFormat_SelectionChangeCommitted(object sender, EventArgs e)
        {
            CheckFormat();
            UpdateFileName();
        }

        #endregion

        #region Notice panel and status band

        private void ShowGuidance()
        {
            pnlNotice.BackColor = BlueTint;
            pnlNoticeBar.BackColor = BlueColour;
            lblNotice.ForeColor = InkColour;
            lblNotice.Text = GuidanceText;
        }

        private void ShowNoticeError(string message)
        {
            pnlNotice.BackColor = RedTint;
            pnlNoticeBar.BackColor = RedColour;
            lblNotice.ForeColor = RedColour;
            lblNotice.Text = message;
        }

        private void SetStatus(string message, Color back, Color fore)
        {
            pnlStatus.BackColor = back;
            lblStatus.ForeColor = fore;
            lblStatus.Text = message;
        }

        #endregion

        #region Buttons and navigation

        /// <summary>Browse (amber group button) picks the destination folder.</summary>
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Choose the folder to save your advisory summary in.";
                dialog.ShowNewFolderButton = true;
                if (Directory.Exists(txtSaveTo.Text.Trim()))
                {
                    dialog.SelectedPath = txtSaveTo.Text.Trim();
                }

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    txtSaveTo.Text = dialog.SelectedPath;
                    CheckFolder();
                }
            }
        }

        /// <summary>
        /// Save and export → validate both required fields, check the folder, confirm, export, then
        /// report success in the status band. After success the button reads Finish and returns home.
        /// </summary>
        private void btnSaveExport_Click(object sender, EventArgs e)
        {
            if (exported)
            {
                ReturnHome();
                return;
            }

            var problems = new List<KeyValuePair<Control, string>>();
            if (!CheckFormat()) problems.Add(new KeyValuePair<Control, string>(cboExportFormat, lblExportFormatError.Text.TrimStart('⚠', ' ')));
            if (!CheckFolder()) problems.Add(new KeyValuePair<Control, string>(txtSaveTo, lblSaveToError.Text.TrimStart('⚠', ' ')));
            if (problems.Count > 0)
            {
                ShowNoticeError(problems[0].Value + (problems.Count > 1 ? "  (" + problems.Count + " fields to correct)" : string.Empty));
                SetStatus(problems.Count + (problems.Count == 1 ? " entry" : " entries") + " to correct before you can export.", RedColour, Color.White);
                problems[0].Key.Focus();
                return;
            }

            ShowGuidance();
            string folder = txtSaveTo.Text.Trim();
            if (!FolderIsWritable(folder))
            {
                ShowFolderProblem(folder);
                return;
            }

            using (var dialog = new ConfirmationDialog("Save and export",
                "Save your advisory summary as " + txtFileName.Text + " in \"" + folder + "\"? This completes your advisory " +
                "session, and it can no longer be changed.", "Save and export"))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }
            }

            generatedOn = DateTime.Now;
            try
            {
                ExportSummary(folder, txtFileName.Text);
            }
            catch (UnauthorizedAccessException)
            {
                ShowFolderProblem(folder);
                return;
            }
            catch (IOException)
            {
                ShowFolderProblem(folder);
                return;
            }
            catch (Exception)
            {
                generatedOn = null;
                SetStatus("Your advisory summary could not be saved. Please try again.", AmberColour, InkColour);
                return;
            }

            ShowExported(folder);
        }

        /// <summary>Success is shown in the status band (never a modal) and the screen switches to Finish.</summary>
        private void ShowExported(string folder)
        {
            exported = true;
            txtGeneratedOn.Text = generatedOn.Value.ToString("dd MMMM yyyy HH:mm", CultureInfo.CurrentCulture);
            BuildPreview();
            cboExportFormat.Enabled = false;
            txtSaveTo.ReadOnly = true;
            btnBrowse.Enabled = false;
            btnBack.Visible = false;
            btnCancel.Visible = false;
            btnSaveExport.Text = "Finish";
            btnSaveExport.AccessibleName = "Finish and return to the homepage";
            lblActionHint.Text = "Session completed · Finish returns to the homepage.";
            SetStatus("✓ Saved " + txtFileName.Text + " in " + folder + ". Your advisory session is complete.", GreenColour, Color.White);
        }

        /// <summary>Back → the screen that opened the summary. Nothing is written.</summary>
        private void btnBack_Click(object sender, EventArgs e)
        {
            if (PreviousForm != null)
            {
                PreviousForm.Show();
            }

            Hide();
        }

        /// <summary>Cancel leaves the session without exporting, after confirmation.</summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            LeaveSession();
        }

        /// <summary>Home on the rail: straight home once exported, otherwise after confirmation.</summary>
        private void btnNavHome_Click(object sender, EventArgs e)
        {
            if (exported)
            {
                ReturnHome();
            }
            else
            {
                LeaveSession();
            }
        }

        /// <summary>
        /// Asks for confirmation, then returns to UI 1. The session stays "In progress" because it was
        /// not exported (decisions.md D3).
        /// </summary>
        private void LeaveSession()
        {
            using (var dialog = new ConfirmationDialog("Leave this advisory session",
                "Return to the homepage without exporting? Anything already saved stays on record as an \"In progress\" " +
                "session. Your session is only completed when you export the advisory summary.", "Leave session"))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }
            }

            ReturnHome();
        }

        /// <summary>Shows UI 1 and closes every learner screen without asking.</summary>
        private void ReturnHome()
        {
            homeForm.Show();
            foreach (Form screen in Application.OpenForms.Cast<Form>().ToList())
            {
                if (screen != homeForm)
                {
                    screen.Close();
                }
            }
        }

        #endregion

        #region Showing and closing

        /// <summary>Shows the learner context and rebuilds the preview from the stored session each time.</summary>
        private void AdvisorySummaryForm_VisibleChanged(object sender, EventArgs e)
        {
            if (!Visible || exported)
            {
                return;
            }

            lblContext.Text = LearnerContext;
            ShowGuidance();
            SetStatus(ReadyStatus, DeepNavy, Color.White);
            LoadSummary();
        }

        /// <summary>The caption-bar X asks first, because it closes Future Path.</summary>
        private void AdvisorySummaryForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing || homeForm.Visible)
            {
                return;
            }

            string message = exported
                ? "Close Future Path? Your advisory summary has been saved and your session is complete."
                : "Close Future Path? Anything already saved stays on record as an \"In progress\" session.";
            using (var dialog = new ConfirmationDialog("Close Future Path", message, "Close"))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>Ends the application when this screen was closed with the caption-bar X.</summary>
        private void AdvisorySummaryForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!homeForm.Visible)
            {
                Application.Exit();
            }
        }

        #endregion
    }
}
