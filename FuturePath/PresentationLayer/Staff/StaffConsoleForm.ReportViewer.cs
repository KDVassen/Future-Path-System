using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using FuturePath.ApplicationLayer.Reports;
using Microsoft.Reporting.WinForms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 11.1a / 11.2b / 11.3c — the report viewer (spec 2.1, Section 6; reports.md "Technology").
    /// FP-LSR, FP-DIE or FP-LER is shown in the WinForms ReportViewer from the typed dataset the Business
    /// Logic Layer fills (<see cref="FpLsrData"/>, <see cref="FpDieData"/>, <see cref="FpLerData"/>). The .rdlc
    /// layouts hold no queries and no business rules: this screen only binds each DataTable to the report
    /// data source of the same name, shows it in print layout and renders it to a file.
    /// <para>
    /// The viewer's own export button is hidden. <b>Save report</b> exports only the formats reports.md allows
    /// for each report (FP-LSR and FP-LER: PDF or Word; FP-DIE: PDF or Excel), and every saved file is recorded
    /// in ExportReport through the BLL. Opened from UI 11 (Preview / Generate report) and from UI 10.2b
    /// (Open advisory summary).
    /// </para>
    /// </summary>
    public partial class StaffConsoleForm
    {
        private const string LsrCode = "FP-LSR";
        private const string DieCode = "FP-DIE";
        private const string LerCode = "FP-LER";

        /// <summary>The report on show: FP-LSR, FP-DIE or FP-LER.</summary>
        private string viewerReportCode = DieCode;

        /// <summary>What the report covers, for the subtitle (a session, or a period and its filters).</summary>
        private string viewerScope = string.Empty;

        /// <summary>The screen Back returns to (UI 11 or UI 10.2b).</summary>
        private TabPage viewerReturnPage;

        /// <summary>The typed dataset the report is rendered from.</summary>
        private DataSet viewerData;

        /// <summary>File extension chosen on UI 11 (".pdf", ".docx", ".xlsx"), or null to ask on save.</summary>
        private string viewerExtension;

        /// <summary>Folder chosen on UI 11, or null to ask on save.</summary>
        private string viewerFolder;

        /// <summary>Proposed file name without extension.</summary>
        private string viewerFileStem;

        /// <summary>AdvisorySession.SessionID for an FP-LSR, otherwise null (ExportReport.SessionID).</summary>
        private int? viewerSessionId;

        /// <summary>ExportReport.ParametersUsed for FP-DIE / FP-LER, otherwise null.</summary>
        private string viewerParameters;

        /// <summary>The full path of the file already saved for this report, or null.</summary>
        private string viewerSavedPath;

        #region Opening the viewer

        /// <summary>
        /// Shows a report in the viewer (UI 11.1a / 11.2b / 11.3c).
        /// </summary>
        /// <param name="reportCode">FP-LSR, FP-DIE or FP-LER.</param>
        /// <param name="data">The typed dataset from the BLL; null shows the empty layout.</param>
        /// <param name="scope">What the report covers, for the subtitle.</param>
        /// <param name="sessionId">The session of an FP-LSR, or null.</param>
        /// <param name="parameters">The selection of an FP-DIE / FP-LER (ExportReport.ParametersUsed), or null.</param>
        /// <param name="extension">The output format chosen on UI 11, or null to ask on save.</param>
        /// <param name="folder">The folder chosen on UI 11, or null to ask on save.</param>
        /// <param name="fileStem">The proposed file name without extension.</param>
        /// <param name="returnPage">The screen Back returns to.</param>
        private void OpenReportViewer(string reportCode, DataSet data, string scope, int? sessionId, string parameters,
                                      string extension, string folder, string fileStem, TabPage returnPage)
        {
            viewerReportCode = reportCode;
            viewerData = data ?? EmptyReportData(reportCode);
            viewerScope = scope ?? string.Empty;
            viewerSessionId = sessionId;
            viewerParameters = parameters;
            viewerExtension = extension;
            viewerFolder = folder;
            viewerFileStem = fileStem;
            viewerReturnPage = returnPage;
            viewerSavedPath = null;

            // viewerData is set above, so tcConsole_Selecting lets the Report viewer tab open.
            SelectPage(tabReportViewer);
            LoadReport(rvReport.LocalReport, viewerReportCode, viewerData);
            UseReportPageSettings(rvReport);
            rvReport.SetDisplayMode(DisplayMode.PrintLayout);
            rvReport.ZoomMode = ZoomMode.PageWidth;
            rvReport.RefreshReport();
        }

        /// <summary>Called by <see cref="PageShown"/> each time the viewer appears; the caption names the report on show.</summary>
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

        /// <summary>Back returns to the screen that opened the viewer.</summary>
        private void btnReportViewerBack_Click(object sender, EventArgs e)
        {
            SelectPage(viewerReturnPage ?? tabReports);
        }

        /// <summary>
        /// Save report → asks for the file when UI 11 did not set one, confirms, renders the report on show
        /// to the chosen format and records the export (ExportReport) through the BLL.
        /// </summary>
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

        /// <summary>The file to save to: UI 11's format and folder when set, otherwise a Save dialog.</summary>
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

        /// <summary>
        /// Renders a report to a file and records it. Used by the viewer, by UI 11 Generate report and by
        /// UI 10.2b Export advisory summary.
        /// </summary>
        /// <param name="reportCode">FP-LSR, FP-DIE or FP-LER.</param>
        /// <param name="data">The typed dataset from the BLL.</param>
        /// <param name="path">The file to write; its extension chooses the format.</param>
        /// <param name="sessionId">ExportReport.SessionID (FP-LSR), or null.</param>
        /// <param name="parameters">ExportReport.ParametersUsed (FP-DIE / FP-LER), or null.</param>
        private void SaveReport(string reportCode, DataSet data, string path, int? sessionId, string parameters)
        {
            byte[] file;
            using (var report = new LocalReport())
            {
                LoadReport(report, reportCode, data ?? EmptyReportData(reportCode));
                file = report.Render(RenderFormatOf(Path.GetExtension(path)));
            }

            File.WriteAllBytes(path, file);

            // TODO BLL: ReportController.RecordExport(UserId, reportCode, sessionId, parameters,
            //               Path.GetExtension(path).TrimStart('.').ToUpperInvariant(), DateTime.Now);
            //           Writes the ExportReport row (ReportCode, SessionID or ParametersUsed, GeneratedByUserID = UserId,
            //           DateGenerated, ExportFormat). The BLL checks the role before recording (spec Section 8).
        }

        #endregion

        #region Report plumbing (display only — no queries, no rules)

        /// <summary>Points a LocalReport at the embedded .rdlc and binds every DataTable as the data source of the same name.</summary>
        private static void LoadReport(LocalReport report, string reportCode, DataSet data)
        {
            report.ReportEmbeddedResource = "FuturePath.PresentationLayer.Reports." + ReportFileOf(reportCode) + ".rdlc";
            report.DataSources.Clear();
            foreach (DataTable table in data.Tables)
            {
                report.DataSources.Add(new ReportDataSource(table.TableName, table));
            }
        }

        /// <summary>
        /// Gives the viewer the report's own page size, orientation and margins. Without this, print layout and the
        /// toolbar's Print use the default printer's portrait page, so the landscape FP-DIE and FP-LER are cut off on
        /// the right and split across twice as many pages.
        /// </summary>
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

        /// <summary>An empty typed dataset for a report, so the layout still shows before the BLL supplies data.</summary>
        private static DataSet EmptyReportData(string reportCode)
        {
            if (reportCode == LsrCode) return new FpLsrData();
            if (reportCode == LerCode) return new FpLerData();
            return new FpDieData();
        }

        private static string ReportFileOf(string reportCode)
        {
            if (reportCode == LsrCode) return "FpLsr";
            if (reportCode == LerCode) return "FpLer";
            return "FpDie";
        }

        /// <summary>The report's name, for the window caption and the status messages.</summary>
        private static string ReportNameOf(string reportCode)
        {
            if (reportCode == LsrCode) return "Learner Advisory Summary";
            if (reportCode == LerCode) return "Learners Not Currently Eligible";
            return "Degree Interest and Eligibility Report";
        }

        /// <summary>The report's screen ID in the spec inventory (UI 11.1a / 11.2b / 11.3c).</summary>
        private static string ReportScreenOf(string reportCode)
        {
            if (reportCode == LsrCode) return "UI 11.1a";
            if (reportCode == LerCode) return "UI 11.3c";
            return "UI 11.2b";
        }

        /// <summary>The Save dialog filter: the export formats reports.md allows for each report.</summary>
        private static string ExportFilterOf(string reportCode)
        {
            return reportCode == DieCode
                ? "PDF document (*.pdf)|*.pdf|Excel workbook (*.xlsx)|*.xlsx"
                : "PDF document (*.pdf)|*.pdf|Word document (*.docx)|*.docx";
        }

        /// <summary>The ReportViewer rendering extension for a file extension.</summary>
        private static string RenderFormatOf(string extension)
        {
            switch ((extension ?? string.Empty).ToLowerInvariant())
            {
                case ".docx": return "WORDOPENXML";
                case ".xlsx": return "EXCELOPENXML";
                default: return "PDF";
            }
        }

        #endregion
    }
}
