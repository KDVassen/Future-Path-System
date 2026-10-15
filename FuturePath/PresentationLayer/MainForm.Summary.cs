using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using FuturePath.ApplicationLayer;
using FuturePath.ApplicationLayer.Reports;
using Microsoft.Reporting.WinForms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 7 — Advisory Summary and export (step 6 of 6; spec 2.3.9, report FP-LSR).
    /// The learner sets only the export format and the destination folder (both required). The file
    /// name, generation date and the preview are system-supplied: the preview is the FP-LSR report itself
    /// (FpLsr.rdlc in the ReportViewer), rendered from the stored session, so the learner sees exactly what is
    /// exported; alternatives appear only if ticked on UI 6.1a and funding only if added on UI 6.2b. <b>Save and export</b> validates, asks for confirmation, writes the file,
    /// completes the session and logs the export. An unusable folder raises a dialog that names the
    /// folder; success is shown in the status bar, never in a modal, and the primary button becomes
    /// <b>Finish</b>. Back returns to the tab that opened UI 7; Back and Cancel write nothing.
    /// </summary>
    public partial class MainForm
    {
        private const string SummaryReadyStatus = "Step 6 of 6 · Choose an export format and a folder, then save and export.";
        private const string SummaryGuidanceText =
            "Saving the advisory summary completes this session: your profile, subjects, marks, questionnaire " +
            "responses and advisory outcome are kept on record as a completed session.";
        /// <summary>True once the export formats have been bound for this session.</summary>
        private bool summaryFormatsLoaded;

        /// <summary>True once the summary has been saved; the primary button then becomes Finish.</summary>
        private bool summaryExported;

        /// <summary>The success message shown in the status bar after the export.</summary>
        private string summaryExportedMessage;

        private DateTime? summaryGeneratedOn;

        /// <summary>The stored session as the FP-LSR typed dataset (filled by the BLL): the preview and the export are rendered from it.</summary>
        private FpLsrData summaryData;

        // From the report header, for the proposed file name Surname_SessionNumber.
        private string summarySurname;
        private string summarySessionNumber;

        /// <summary>Pages of the rendered summary, shown beside the export format.</summary>
        private int? summaryPageCount;

        /// <summary>True once the preview's RenderingComplete handler is attached.</summary>
        private bool summaryViewerReady;

        #region Shown, button clicks and reset

        /// <summary>
        /// Called by PageShown each time UI 7 appears: binds the export formats once per session, shows
        /// the guidance notice and rebuilds the preview from the stored session. After the export it only
        /// restores the completed (Finish) state.
        /// </summary>
        private void SummaryShown()
        {
            SummaryShowGuidance();
            if (summaryExported)
            {
                SummaryApplyFinishState();
                return;
            }

            if (!summaryFormatsLoaded)
            {
                SummaryLoadExportFormats();
            }

            txtGeneratedOn.Text = DateTime.Today.ToString("dd MMMM yyyy", CultureInfo.CurrentCulture);
            SetStatus(SummaryReadyStatus, DeepNavy, Color.White);
            SummaryLoad();
        }

        /// <summary>
        /// Save and export → validate both required fields, check the folder, confirm, export, then
        /// report success in the status bar. After success the button reads Finish and returns home.
        /// </summary>
        private void btnSummaryExport_Click(object sender, EventArgs e)
        {
            if (summaryExported)
            {
                GoHome();   // the session is complete, so the Home tab does not ask
                return;
            }

            var problems = new List<KeyValuePair<Control, string>>();
            if (!SummaryCheckFormat()) problems.Add(new KeyValuePair<Control, string>(cboExportFormat, lblExportFormatError.Text.TrimStart('⚠', ' ')));
            if (!SummaryCheckFolder()) problems.Add(new KeyValuePair<Control, string>(txtSaveTo, lblSaveToError.Text.TrimStart('⚠', ' ')));
            if (problems.Count > 0)
            {
                ShowNotice(lblSummaryNotice, problems[0].Value + (problems.Count > 1 ? "  (" + problems.Count + " fields to correct)" : string.Empty),
                           RedTint, RedColour);
                SetStatus(problems.Count + (problems.Count == 1 ? " entry" : " entries") + " to correct before you can export.",
                          RedColour, Color.White);
                problems[0].Key.Focus();
                return;
            }

            SummaryShowGuidance();
            string folder = txtSaveTo.Text.Trim();
            if (!SummaryFolderIsWritable(folder))
            {
                SummaryShowFolderProblem(folder);
                return;
            }

            if (!Confirm("Save and export",
                "Save your advisory summary as " + txtFileName.Text + " in \"" + folder + "\"? This completes your advisory " +
                "session, and it can no longer be changed.", "Save and export"))
            {
                return;
            }

            summaryGeneratedOn = DateTime.Now;
            try
            {
                SummaryExport(folder, txtFileName.Text);
            }
            catch (UnauthorizedAccessException)
            {
                summaryGeneratedOn = null;
                SummaryShowFolderProblem(folder);
                return;
            }
            catch (IOException)
            {
                summaryGeneratedOn = null;
                SummaryShowFolderProblem(folder);
                return;
            }
            catch (Exception)
            {
                summaryGeneratedOn = null;
                SetStatus("Your advisory summary could not be saved. Please try again.", AmberColour, InkColour);
                return;
            }

            SummaryShowExported(folder);
        }

        /// <summary>
        /// Returns UI 7 to a brand-new state for the next learner (spec 2.1): no export format, folder,
        /// file name, generation date, preview or field messages; the fields are editable again; the
        /// exported (Finish) state is cleared; and the formats are bound again the next time it is shown.
        /// </summary>
        private void SummaryReset()
        {
            summaryFormatsLoaded = false;
            summaryExported = false;
            summaryExportedMessage = null;
            summaryGeneratedOn = null;
            summaryData = null;
            summarySurname = summarySessionNumber = null;
            summaryPageCount = null;

            cboExportFormat.Enabled = true;
            cboExportFormat.SelectedIndex = -1;
            txtSaveTo.ReadOnly = false;
            txtSaveTo.Text = string.Empty;
            btnBrowse.Enabled = true;
            txtFileName.Text = string.Empty;
            txtGeneratedOn.Text = string.Empty;
            lblPreviewMeta.Text = string.Empty;
            rvSummary.Reset();
            CheckField(true, cboExportFormat, lblExportFormat, lblExportFormatError, string.Empty);
            CheckField(true, txtSaveTo, lblSaveTo, lblSaveToError, string.Empty);

            btnSummaryExport.Text = "Save and export";
            btnSummaryExport.AccessibleName = btnSummaryExport.Text;
            btnSummaryBack.Visible = true;
            btnSummaryCancel.Visible = true;
        }

        /// <summary>Back → the tab that opened UI 7 (UI 6, 6.1a or 6.2b). Nothing is written.</summary>
        private void btnSummaryBack_Click(object sender, EventArgs e)
        {
            SelectPage(summaryReturnPage ?? tabOutcome);
        }

        /// <summary>Cancel leaves the session (the Home tab asks first). Nothing is written.</summary>
        private void btnSummaryCancel_Click(object sender, EventArgs e)
        {
            GoHome();
        }

        #endregion

        #region Business Logic Layer hooks

        /// <summary>Binds the export formats offered for FP-LSR (PDF / Word) — never literal Items.</summary>
        private void SummaryLoadExportFormats()
        {
            try
            {
                // ExportFormat (".pdf"), FormatName ("PDF document (.pdf)"), FormatCode (PDF) and RenderFormat.
                ReportController reportController = new ReportController();
                cboExportFormat.DisplayMember = "FormatName";
                cboExportFormat.ValueMember = "ExportFormat";
                cboExportFormat.DataSource = reportController.GetExportFormats(ExportReport.CodeLearnerSummary);
                cboExportFormat.SelectedIndex = -1;
                summaryFormatsLoaded = true;
            }
            catch (Exception)
            {
                SetStatus("The export formats could not be loaded. Please go back and try again.", AmberColour, InkColour);
            }
        }

        /// <summary>
        /// Reads the stored session as the FP-LSR typed dataset that the preview and the file name are built
        /// from. The outcome is read back, never recalculated; the alternatives and funding flags come from the
        /// session row (AlternativesInSummary, FundingInfoRequested).
        /// </summary>
        private void SummaryLoad()
        {
            // Cleared first, so a failed read never shows the previous values.
            summaryData = null;
            summarySurname = summarySessionNumber = null;
            summaryPageCount = null;
            try
            {
                ReportController reportController = new ReportController();
                summaryData = reportController.GetLearnerSummary(SessionId.Value);
            }
            catch (Exception)
            {
                SetStatus("Your stored session could not be read for the preview. Please go back and try again.", AmberColour, InkColour);
            }

            if (summaryData == null)
            {
                summaryData = new FpLsrData();   // the empty layout, so the preview never shows another learner's values
            }

            summarySessionNumber = SessionNumber;
            if (summaryData.ReportHeader.Count > 0)
            {
                FpLsrData.ReportHeaderRow header = summaryData.ReportHeader[0];
                summarySessionNumber = header.SessionNumber;
                if (!header.IsLastNameNull())
                {
                    summarySurname = header.LastName;
                }
            }

            SummaryUpdateFileName();
            SummaryBuildPreview();
        }

        /// <summary>
        /// Renders the FP-LSR from the same report the preview shows, stamped with the export date and time.
        /// The BLL then sets SessionStatus = 'Completed' and DateCompleted and writes the ExportReport row
        /// (ReportCode FP-LSR, GeneratedByUserID null — a learner's own export), and only then is the file
        /// written, so a failure never leaves a file for a session that is not completed (application-layer.md §8).
        /// </summary>
        private void SummaryExport(string folder, string fileName)
        {
            if (summaryData.ReportHeader.Count > 0)
            {
                summaryData.ReportHeader[0].DateGenerated = summaryGeneratedOn.Value;   // "stamped at export" (2.3.9)
            }

            // The chosen row of GetExportFormats: FormatCode (PDF / DOCX) is stored, RenderFormat drives the ReportViewer.
            DataRowView format = (DataRowView)cboExportFormat.SelectedItem;
            byte[] file = rvSummary.LocalReport.Render(Convert.ToString(format["RenderFormat"], CultureInfo.InvariantCulture));

            ReportController reportController = new ReportController();
            reportController.CompleteLearnerSession(SessionId.Value, Convert.ToString(format["FormatCode"], CultureInfo.InvariantCulture),
                                                    summaryGeneratedOn.Value);
            File.WriteAllBytes(Path.Combine(folder, fileName), file);
        }

        #endregion

        #region File name and preview (assembled from the stored session)

        /// <summary>Proposes the file name &lt;Surname&gt;_&lt;SessionNumber&gt; with the chosen format's extension.</summary>
        private void SummaryUpdateFileName()
        {
            string name = string.IsNullOrEmpty(summarySurname) ? "Learner" : summarySurname.Trim().Replace(' ', '-');
            string number = string.IsNullOrEmpty(summarySessionNumber) ? "session" : summarySessionNumber;
            string extension = cboExportFormat.SelectedValue as string ?? string.Empty;
            txtFileName.Text = name + "_" + number + extension;

            string format = cboExportFormat.SelectedIndex >= 0 ? cboExportFormat.Text : "Choose an export format";
            lblPreviewMeta.Text = summaryPageCount.HasValue
                ? format + " · " + summaryPageCount.Value + (summaryPageCount.Value == 1 ? " page" : " pages")
                : format;
        }

        /// <summary>
        /// Shows the FP-LSR in the preview pane, in print layout so the learner sees the exported pages. There are
        /// no section tick boxes: alternatives print only if ticked on UI 6.1a, funding only if added on UI 6.2b,
        /// and everything else always — the report reads those flags from the stored session.
        /// </summary>
        private void SummaryBuildPreview()
        {
            if (!summaryViewerReady)
            {
                rvSummary.RenderingComplete += SummaryPreview_RenderingComplete;
                summaryViewerReady = true;
            }

            LocalReport report = rvSummary.LocalReport;
            report.ReportEmbeddedResource = "FuturePath.PresentationLayer.Reports.FpLsr.rdlc";
            report.DataSources.Clear();
            foreach (System.Data.DataTable table in summaryData.Tables)
            {
                report.DataSources.Add(new ReportDataSource(table.TableName, table));
            }

            ReportPageSettings page = report.GetDefaultPageSettings();   // the report's own A4 portrait page, not the printer's
            rvSummary.SetPageSettings(new System.Drawing.Printing.PageSettings
            {
                PaperSize = page.PaperSize,
                Margins = page.Margins,
                Landscape = page.IsLandscape
            });
            rvSummary.SetDisplayMode(DisplayMode.PrintLayout);
            rvSummary.ZoomMode = ZoomMode.PageWidth;
            rvSummary.RefreshReport();
        }

        /// <summary>Once the preview has rendered, shows its page count beside the export format.</summary>
        private void SummaryPreview_RenderingComplete(object sender, RenderingCompleteEventArgs e)
        {
            try
            {
                summaryPageCount = rvSummary.GetTotalPages();
            }
            catch (Exception)
            {
                summaryPageCount = null;
            }

            SummaryUpdateFileName();
        }

        #endregion

        #region Field validation (validated again in the BLL on export)

        /// <summary>Export format is required.</summary>
        private bool SummaryCheckFormat()
        {
            return CheckField(cboExportFormat.SelectedIndex >= 0, cboExportFormat, lblExportFormat, lblExportFormatError,
                "Export format: choose PDF or Word.");
        }

        /// <summary>Save to (the destination folder) is required.</summary>
        private bool SummaryCheckFolder()
        {
            return CheckField(txtSaveTo.Text.Trim().Length > 0, txtSaveTo, lblSaveTo, lblSaveToError,
                "Save to: choose the folder to save your summary in.");
        }

        /// <summary>
        /// True when the folder exists and a file can be written there. Checked here so the learner
        /// gets a plain message naming the folder instead of a technical error.
        /// </summary>
        private static bool SummaryFolderIsWritable(string folder)
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
        private void SummaryShowFolderProblem(string folder)
        {
            CheckField(false, txtSaveTo, lblSaveTo, lblSaveToError, "Save to: this folder cannot be used. Choose another folder.");
            SetStatus("Your summary was not saved. Choose a folder you can save files in.", RedColour, Color.White);
            ShowValidation("Folder cannot be used", new List<string>
            {
                "Future Path could not save your summary in the folder \"" + folder + "\".",
                "Check that the folder exists and that you are allowed to save files there, or use Browse to choose another folder."
            }, txtSaveTo);
        }

        /// <summary>Checks the format when the learner leaves the combo.</summary>
        private void cboExportFormat_Leave(object sender, EventArgs e) { SummaryCheckFormat(); }

        /// <summary>Checks the folder when the learner leaves the box.</summary>
        private void txtSaveTo_Leave(object sender, EventArgs e) { SummaryCheckFolder(); }

        /// <summary>A new format changes the proposed file extension and the preview line.</summary>
        private void cboExportFormat_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SummaryCheckFormat();
            SummaryUpdateFileName();
        }

        #endregion

        #region Browse, guidance and the completed state

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
                    SummaryCheckFolder();
                }
            }
        }

        /// <summary>Shows the blue guidance notice: saving completes the session.</summary>
        private void SummaryShowGuidance()
        {
            ShowNotice(lblSummaryNotice, SummaryGuidanceText, BlueTint, InkColour);
        }

        /// <summary>Success is shown in the status bar (never a modal) and the screen switches to Finish.</summary>
        private void SummaryShowExported(string folder)
        {
            summaryExported = true;
            txtGeneratedOn.Text = summaryGeneratedOn.Value.ToString("dd MMMM yyyy HH:mm", CultureInfo.CurrentCulture);
            SummaryBuildPreview();
            cboExportFormat.Enabled = false;
            txtSaveTo.ReadOnly = true;
            btnBrowse.Enabled = false;
            summaryExportedMessage = "✓ Saved " + txtFileName.Text + " in " + folder + ". Your advisory session is complete.";
            SummaryApplyFinishState();
        }

        /// <summary>
        /// The completed state of UI 7: only the primary button, reading Finish (it returns to the
        /// homepage), a green notice, and the success message in the status bar.
        /// </summary>
        private void SummaryApplyFinishState()
        {
            btnSummaryExport.Text = "Finish";
            btnSummaryExport.AccessibleName = "Finish and return to the homepage";
            SetPrimaryEnabled(btnSummaryExport, true);
            btnSummaryBack.Visible = false;
            btnSummaryCancel.Visible = false;
            ShowNotice(lblSummaryNotice, "✓ Session completed. Finish returns to the homepage.", GreenTint, GreenColour);
            SetStatus(summaryExportedMessage, GreenColour, Color.White);
        }

        #endregion
    }
}
