using System;
using System.Data;
using System.Data.SqlClient;
using FuturePath.ApplicationLayer;

namespace FuturePath.DatabaseLayer
{
    /// <summary>
    /// Data access for the ExportReport log. Insert is the one place an ExportReport row is
    /// written, both for a learner's own FP-LSR (UI 7) and for staff exports (decisions.md D2).
    /// Insert follows the Waitering pattern: fill an empty copy of the table, add a row, then
    /// send it with the adapter's INSERT command.
    /// </summary>
    public class ExportReportRepository : DB
    {
        #region Data Members
        private string table = "ExportReport";
        private string sqlLocal = "SELECT ExportID, ReportCode, SessionID, ParametersUsed, GeneratedByUserID, DateGenerated, ExportFormat FROM ExportReport";
        #endregion

        #region Constructor
        /// <summary>Creates the repository (no database work until a method is called).</summary>
        public ExportReportRepository() : base()
        {
        }
        #endregion

        #region Methods
        /// <summary>Writes one ExportReport row and returns its ExportID (also stored in report.ExportID).</summary>
        public int Insert(ExportReport report)
        {
            // 1. Fill an empty copy of the table so the DataSet knows its columns.
            FillDataSet(sqlLocal + " WHERE 1 = 0", table);

            // 2. Add the new row to the DataSet (Waitering DataSetChange).
            DataRow row = GetTable(table).NewRow();
            FillRow(row, report);
            GetTable(table).Rows.Add(row);

            // 3. Send the row to the database. The INSERT returns the new key into the row.
            Create_INSERT_Command();
            UpdateDataSource(table);
            report.ExportID = Convert.ToInt32(row["ExportID"]);
            return report.ExportID;
        }

        /// <summary>When the report was last generated, or null if never.</summary>
        public DateTime? GetLastGenerated(string reportCode)
        {
            object value = ExecuteScalar("SELECT MAX(DateGenerated) FROM ExportReport WHERE ReportCode = @ReportCode",
                                         Param("@ReportCode", SqlDbType.VarChar, reportCode));
            if (value == null)
            {
                return null;
            }
            return Convert.ToDateTime(value);
        }
        #endregion

        #region Utility Methods
        /// <summary>Copies the log entry into a DataRow (Waitering FillRow). Nulls become DBNull.</summary>
        private void FillRow(DataRow row, ExportReport report)
        {
            row["ReportCode"] = report.ReportCode;
            row["SessionID"] = DbValue(report.SessionID);
            row["ParametersUsed"] = DbText(report.ParametersUsed);
            row["GeneratedByUserID"] = DbValue(report.GeneratedByUserID);
            row["DateGenerated"] = report.DateGenerated;
            row["ExportFormat"] = report.ExportFormat;
        }
        #endregion

        #region Build Parameters and Create Commands
        /// <summary>One adapter parameter per column; each reads its value from the new row (Waitering Build_INSERT_Parameters).</summary>
        private void Build_INSERT_Parameters()
        {
            daMain.InsertCommand.Parameters.Add(SourceParam("@ReportCode", SqlDbType.VarChar, "ReportCode"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@SessionID", SqlDbType.Int, "SessionID"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@ParametersUsed", SqlDbType.VarChar, "ParametersUsed"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@GeneratedByUserID", SqlDbType.Int, "GeneratedByUserID"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@DateGenerated", SqlDbType.DateTime, "DateGenerated"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@ExportFormat", SqlDbType.VarChar, "ExportFormat"));
        }

        /// <summary>
        /// The adapter's INSERT command (Waitering Create_INSERT_Command). It ends by selecting the
        /// new ExportID, which the adapter copies back into the row (FirstReturnedRecord).
        /// </summary>
        private void Create_INSERT_Command()
        {
            daMain.InsertCommand = new SqlCommand(
                "INSERT INTO ExportReport (ReportCode, SessionID, ParametersUsed, GeneratedByUserID, DateGenerated, ExportFormat) " +
                "VALUES (@ReportCode, @SessionID, @ParametersUsed, @GeneratedByUserID, @DateGenerated, @ExportFormat); " +
                "SELECT CAST(SCOPE_IDENTITY() AS INT) AS ExportID", cnMain);
            daMain.InsertCommand.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;
            Build_INSERT_Parameters();
        }
        #endregion
    }
}
