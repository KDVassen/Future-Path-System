
using System;
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
    // Handles displaying, saving and exporting reports
    public partial class StaffConsoleForm
    {
        private const string LsrCode = "FP-LSR";
        private const string DieCode = "FP-DIE";
        private const string LerCode = "FP-LER";

        // Code of the report currently displayed
        private string viewerReportCode = DieCode;

        // Description of the report's scope
        private string viewerScope = string.Empty;

        // Page to return to when Back is clicked
        private TabPage viewerReturnPage;

        // Dataset used to display the report
        private DataSet viewerData;

        // Selected export file extension
        private string viewerExtension;

        // Selected export folder
        private string viewerFolder;

        // Proposed filename without the extension
        private string viewerFileStem;

        // Session ID for learner advisory summaries
        private int viewerSessionId;

        // Parameters used when generating the report
        private string viewerParameters;

        // Full path of the saved report
        private string viewerSavedPath;

        #region Opening the viewer

        // Open the selected report in the ReportViewer
        private void OpenReportViewer(string reportCode, DataSet data, string scope, int sessionId, string parameters,
                                      string extension, string folder, string fileStem, TabPage returnPage)
        {
            viewerReportCode = reportCode;
            viewerData = data;
            viewerScope = scope ?? string.Empty;
            viewerSessionId = sessionId;
            viewerParameters = parameters;
            viewerExtension = extension;
            viewerFolder = folder;
            viewerFileStem = fileStem;
            viewerReturnPage = returnPage;
            viewerSavedPath = null;

            // Set up the report before opening the viewer
            SelectPage(tabReportViewer);
            LoadReport(rvReport.LocalReport, viewerReportCode, viewerData);
            UseReportPageSettings(rvReport);
            rvReport.SetDisplayMode(DisplayMode.PrintLayout);
            rvReport.ZoomMode = ZoomMode.PageWidth;
            rvReport.RefreshReport();
        }

        // Update the window title and status when the viewer opens
        private void ReportViewerShown(bool firstTime)
        {
            Text = "Future Path — " + ReportNameOf(viewerReportCode) + " (" + ReportScreenOf(viewerReportCode) + ")";
            string scope = viewerScope.Length > 0 ? " · " + viewerScope : string.Empty;
            ReportStatus(viewerSavedPath == null
                ? "Preview of the " + ReportNameOf(viewerReportCode) + scope + ". Nothing is saved or recorded until you save it."
                : ReportNameOf(viewerReportCode) + " saved as " + viewerSavedPath + ".", false);
        }

        #endregion

        #region Button click events

        // Return to the page that opened the report viewer
        private void btnReportViewerBack_Click(object sender, EventArgs e)
        {
            SelectPage(viewerReturnPage ?? tabReports);
        }

        // Save the displayed report and record the export
        private void btnReportViewerSave_Click(object sender, EventArgs e)
        {
            string path = ReportViewerChoosePath();
            if (path == null)
            {
                return;
            }

            if (!Confirm("Save report", "Save the " + ReportNameOf(viewerReportCode) + " as \"" + path +
                         "\"? The export is recorded against your account.", "Save report"))
            {
                return;
            }

            try
            {
                SaveReport(viewerReportCode, viewerData, path, viewerSessionId, viewerParameters);
            }
            catch (UnauthorizedAccessException)
            {
                SetStatus("The report could not be saved to " + Path.GetDirectoryName(path) + ". Choose another folder.", RedColour, Color.White);
                return;
            }
            catch (IOException)
            {
                SetStatus("The report could not be saved as " + path + ". Close the file if it is open, then try again.", RedColour, Color.White);
                return;
            }
            catch (ArgumentException ex)
            {
                // Display the reason when an export is rejected
                SetStatus(ex.Message, RedColour, Color.White);
                return;
            }
            catch (Exception)
            {
                SetStatus("The report could not be saved. Please try again.", RedColour, Color.White);
                return;
            }

            viewerSavedPath = path;
            SetStatus(ReportNameOf(viewerReportCode) + " saved as " + path + ".", GreenColour, Color.White);
        }

        #endregion

        #region Saving

        // Get the selected save path or open a Save File dialog
        private string ReportViewerChoosePath()
        {
            if (!string.IsNullOrEmpty(viewerExtension) && !string.IsNullOrEmpty(viewerFolder))
            {
                return Path.Combine(viewerFolder, viewerFileStem + viewerExtension);
            }

            using (var save = new SaveFileDialog())
            {
                save.Title = "Save " + ReportNameOf(viewerReportCode);
                save.Filter = ExportFilterOf(viewerReportCode);
                save.FileName = viewerFileStem;
                return save.ShowDialog(this) == DialogResult.OK ? save.FileName : null;
            }
        }

        // Render the report to a file and record the export
        private void SaveReport(string reportCode, DataSet data, string path, int sessionId, string parameters)
        {
            ReportController reportController = new ReportController();
            string extension = Path.GetExtension(path).ToLowerInvariant();
            string renderFormat = string.Empty;
            foreach (DataRow format in reportController.GetExportFormats(reportCode).Rows)
            {
                if (Convert.ToString(format["ExportFormat"], CultureInfo.InvariantCulture) == extension)
                {
                    renderFormat = Convert.ToString(format["RenderFormat"], CultureInfo.InvariantCulture);
                }
            }

            if (renderFormat.Length == 0)
            {
                throw new ArgumentException("Save the " + ReportNameOf(reportCode) + " as one of the file types offered for it.");
            }

            byte[] file;
            using (var report = new LocalReport())
            {
                LoadReport(report, reportCode, data);
                file = report.Render(renderFormat);
            }

            File.WriteAllBytes(path, file);

            // Record the report export through the controller
            reportController.RecordExport(UserId, reportCode, sessionId, parameters, extension, DateTime.Now);
        }

        #endregion

        #region Report plumbing (display only — no queries, no rules)

        // Load the report layout and bind its dataset tables
        private static void LoadReport(LocalReport report, string reportCode, DataSet data)
        {
            report.ReportEmbeddedResource = "FuturePath.PresentationLayer.Reports." + ReportFileOf(reportCode) + ".rdlc";
            report.DataSources.Clear();
            foreach (DataTable table in data.Tables)
            {
                report.DataSources.Add(new ReportDataSource(table.TableName, table));
            }
        }

        // Apply the report's page size, margins and orientation
        private static void UseReportPageSettings(ReportViewer viewer)
        {
            ReportPageSettings page = viewer.LocalReport.GetDefaultPageSettings();
            viewer.SetPageSettings(new System.Drawing.Printing.PageSettings
            {
                PaperSize = page.PaperSize,
                Margins = page.Margins,
                Landscape = page.IsLandscape
            });
        }

        private static string ReportFileOf(string reportCode)
        {
            if (reportCode == LsrCode) return "FpLsr";
            if (reportCode == LerCode) return "FpLer";
            return "FpDie";
        }

        // Get the display name of the selected report
        private static string ReportNameOf(string reportCode)
        {
            if (reportCode == LsrCode) return "Learner Advisory Summary";
            if (reportCode == LerCode) return "Learners Not Currently Eligible";
            return "Degree Interest and Eligibility Report";
        }

        // Get the screen ID for the selected report
        private static string ReportScreenOf(string reportCode)
        {
            if (reportCode == LsrCode) return "UI 11.1a";
            if (reportCode == LerCode) return "UI 11.3c";
            return "UI 11.2b";
        }

        // Build the Save dialog filter using supported export formats
        private static string ExportFilterOf(string reportCode)
        {
            ReportController reportController = new ReportController();
            string filter = string.Empty;
            foreach (DataRow format in reportController.GetExportFormats(reportCode).Rows)
            {
                if (filter.Length > 0)
                {
                    filter += "|";
                }

                filter += format["FormatName"] + "|*" + format["ExportFormat"];
            }

            return filter;
        }

        #endregion
    }
}
