
using System;
using System.Data;
using System.Data.SqlClient;
using FuturePath.ApplicationLayer;

namespace FuturePath.DatabaseLayer
{
    // Handles database operations for report export records
    // Saves export details and retrieves the last generated date
    public class ExportReportRepository : DB
    {
        #region Data Members
        private string table = "ExportReport";
        private string sqlLocal = "SELECT ExportID, ReportCode, SessionID, ParametersUsed, GeneratedByUserID, DateGenerated, ExportFormat FROM ExportReport";
        #endregion

        #region Constructor
        // Initialise the export report repository
        public ExportReportRepository() : base()
        {
        }
        #endregion

        #region Methods
        // Save a new report export record to the database
        // Return the generated export ID
        public int Insert(ExportReport report)
        {
            // Load the table structure without retrieving existing records
            FillDataSet(sqlLocal + " WHERE 1 = 0", table);

            // Add the report details to a new DataSet row
            DataRow row = GetTable(table).NewRow();
            FillRow(row, report);
            GetTable(table).Rows.Add(row);

            // Insert the new record and retrieve its generated ID
            Create_INSERT_Command();
            UpdateDataSource(table);
            report.ExportID = Convert.ToInt32(row["ExportID"]);
            return report.ExportID;
        }

        // Get the date a report was last generated
        // Return null if the report has never been generated
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
        // Copy report export details into a DataRow
        // Convert missing values to database NULL values
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
        // Add the required parameters to the INSERT command
        private void Build_INSERT_Parameters()
        {
            daMain.InsertCommand.Parameters.Add(SourceParam("@ReportCode", SqlDbType.VarChar, "ReportCode"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@SessionID", SqlDbType.Int, "SessionID"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@ParametersUsed", SqlDbType.VarChar, "ParametersUsed"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@GeneratedByUserID", SqlDbType.Int, "GeneratedByUserID"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@DateGenerated", SqlDbType.DateTime, "DateGenerated"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@ExportFormat", SqlDbType.VarChar, "ExportFormat"));
        }

        // Create the SQL command for inserting a report export record
        // Return the new ExportID to the DataSet after insertion
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
