
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
    // Handles the advisory summary preview, export and session completion
    public partial class MainForm
    {
        private const string SummaryReadyStatus = "Step 6 of 6 · Choose an export format and a folder, then save and export.";
        private const string SummaryGuidanceText =
            "Saving the advisory summary completes this session: your profile, subjects, marks, questionnaire " +
            "responses and advisory outcome are kept on record as a completed session.";

        // Tracks whether export formats have been loaded
        private bool summaryFormatsLoaded;

        // Tracks whether the summary has been exported
        private bool summaryExported;

        // Success message shown after exporting
        private string summaryExportedMessage;

        private DateTime? summaryGeneratedOn;

        // Report data used for both the preview and export
        private FpLsrData summaryData;

        // Details used to generate the file name
        private string summarySurname;
        private string summarySessionNumber;

        // Number of pages in the rendered report
        private int? summaryPageCount;

        // Tracks whether the preview event handler is attached
        private bool summaryViewerReady;

        #region Shown, button clicks and reset

        // Load the summary preview or restore the completed state
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

        // Validate and export the advisory summary
        private void btnSummaryExport_Click(object sender, EventArgs e)
        {
            if (summaryExported)
            {
                GoHome();   // Return home after the session is completed
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

        // Reset the summary screen for a new learner
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

        // Return to the page that opened the summary
        private void btnSummaryBack_Click(object sender, EventArgs e)
        {
            SelectPage(summaryReturnPage ?? tabOutcome);
        }

        // Cancel the session and return home
        private void btnSummaryCancel_Click(object sender, EventArgs e)
        {
            GoHome();
        }

        #endregion

        #region Business Logic Layer hooks

        // Load the available PDF and Word export formats
        private void SummaryLoadExportFormats()
        {
            try
            {
                // Retrieve export formats from the report controller
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

        // Load the stored session data for the report preview
        private void SummaryLoad()
        {
            // Clear previous data before loading the current session
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
                summaryData = new FpLsrData();   // Use an empty report instead of showing previous data
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

        // Render the report, complete the session and write the exported file
        private void SummaryExport(string folder, string fileName)
        {
            if (summaryData.ReportHeader.Count > 0)
            {
                summaryData.ReportHeader[0].DateGenerated = summaryGeneratedOn.Value;   // Set the report's export date
            }

            // Use the selected format to render the report
            DataRowView format = (DataRowView)cboExportFormat.SelectedItem;
            byte[] file = rvSummary.LocalReport.Render(Convert.ToString(format["RenderFormat"], CultureInfo.InvariantCulture));

            ReportController reportController = new ReportController();
            reportController.CompleteLearnerSession(SessionId.Value, Convert.ToString(format["FormatCode"], CultureInfo.InvariantCulture),
                                                    summaryGeneratedOn.Value);
            File.WriteAllBytes(Path.Combine(folder, fileName), file);
        }

        #endregion

        #region File name and preview (assembled from the stored session)

        // Generate the summary file name using the surname and session number
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

        // Build the report preview from the stored session data
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

            ReportPageSettings page = report.GetDefaultPageSettings();   // Use the report's page settings
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

        // Update the page count once the preview has rendered
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

        // Check that an export format is selected
        private bool SummaryCheckFormat()
        {
            return CheckField(cboExportFormat.SelectedIndex >= 0, cboExportFormat, lblExportFormat, lblExportFormatError,
                "Export format: choose PDF or Word.");
        }

        // Check that a destination folder is entered
        private bool SummaryCheckFolder()
        {
            return CheckField(txtSaveTo.Text.Trim().Length > 0, txtSaveTo, lblSaveTo, lblSaveToError,
                "Save to: choose the folder to save your summary in.");
        }

        // Check whether the selected folder can be written to
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

        // Show a validation error when the folder cannot be used
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

        // Validate the export format when leaving the field
        private void cboExportFormat_Leave(object sender, EventArgs e) { SummaryCheckFormat(); }

        // Validate the folder when leaving the field
        private void txtSaveTo_Leave(object sender, EventArgs e) { SummaryCheckFolder(); }

        // Update the file name when the export format changes
        private void cboExportFormat_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SummaryCheckFormat();
            SummaryUpdateFileName();
        }

        #endregion

        #region Browse, guidance and the completed state

        // Open a dialog to choose the destination folder
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

        // Display guidance about saving the advisory summary
        private void SummaryShowGuidance()
        {
            ShowNotice(lblSummaryNotice, SummaryGuidanceText, BlueTint, InkColour);
        }

        // Show export success and switch to the Finish state
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

        // Update the screen after a successful export
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
