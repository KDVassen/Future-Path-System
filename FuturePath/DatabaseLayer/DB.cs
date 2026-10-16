using System;
using System.Data;
using System.Data.SqlClient;
using FuturePath.Properties;

namespace FuturePath.DatabaseLayer
{
    /// <summary>
    /// The base class of every repository, following the Waitering System DB class. It holds one
    /// connection (cnMain), one DataSet (dsMain) and one DataAdapter (daMain).
    /// - Adding or editing one entity: fill a table into the DataSet, change a row
    ///   (DataSetChange), then send the change with the adapter (UpdateDataSource).
    /// - Replacing a set of child rows (a session's subjects, a funding option's links): use
    ///   direct parameterised commands inside one transaction.
    /// Every query is parameterised. Departures from the Waitering version (decisions.md):
    /// no MessageBox here (errors are thrown and the forms show a friendly message), no database
    /// work in the constructor, and FillDataSet clears the table first so rows are never duplicated.
    /// </summary>
    public class DB
    {
        #region Data Members
        /// <summary>The change a repository makes to the DataSet. Reference data is never deleted, only deactivated.</summary>
        public enum DBOperation
        {
            /// <summary>Insert a new row.</summary>
            Add = 0,
            /// <summary>Update an existing row.</summary>
            Edit = 1
        }

        /// <summary>
        /// The connection string, from Settings (App.config: AttachDbFilename=|DataDirectory|\FuturePath.mdf
        /// on (LocalDB)\MSSQLLocalDB), declared as in the Waitering System DB class.
        /// </summary>
        private string strConn = Settings.Default.FuturePathDatabaseConnectionString;
        /// <summary>The connection every command uses.</summary>
        protected SqlConnection cnMain;
        /// <summary>The in-memory copy of the tables this repository has filled.</summary>
        protected DataSet dsMain;
        /// <summary>The adapter used to fill tables and send changes back.</summary>
        protected SqlDataAdapter daMain;
        #endregion

        #region Constructor
        /// <summary>Creates the connection, DataSet and adapter objects. Does not open the connection.</summary>
        public DB()
        {
            cnMain = new SqlConnection(strConn);
            dsMain = new DataSet();
            daMain = new SqlDataAdapter();
        }
        #endregion

        #region Fill the DataSet
        /// <summary>
        /// Fills one table of the DataSet fresh from the database with a parameterised query.
        /// The table is cleared first. Do not call this while a transaction is open.
        /// </summary>
        protected void FillDataSet(string sqlLocal, string table, params SqlParameter[] parameters)
        {
            daMain = new SqlDataAdapter(sqlLocal, cnMain);
            if (parameters != null)
            {
                daMain.SelectCommand.Parameters.AddRange(parameters);
            }
            if (dsMain.Tables.Contains(table))
            {
                dsMain.Tables[table].Clear();
            }
            try
            {
                OpenConnection();
                daMain.Fill(dsMain, table);
            }
            finally
            {
                CloseConnection();
            }
        }

        /// <summary>Returns a table that FillDataSet has filled.</summary>
        protected DataTable GetTable(string table)
        {
            return dsMain.Tables[table];
        }

        /// <summary>Returns the whole DataSet (as in the Waitering System).</summary>
        public DataSet GetDataSet()
        {
            return dsMain;
        }

        /// <summary>Finds the row with this key in a filled table, or returns null (used for edits).</summary>
        protected DataRow FindRow(string table, string keyColumn, int keyValue)
        {
            foreach (DataRow row in dsMain.Tables[table].Rows)
            {
                if (row.RowState != DataRowState.Deleted && Convert.ToInt32(row[keyColumn]) == keyValue)
                {
                    return row;
                }
            }
            return null;
        }
        #endregion

        #region Update the data source
        /// <summary>
        /// Sends the DataSet changes for one table to the database through daMain's Insert or
        /// Update command (the repository builds that command first). If the database rejects
        /// the change, the change is undone in the DataSet and the error is passed on.
        /// For an insert, end the INSERT with "; SELECT CAST(SCOPE_IDENTITY() AS INT) AS KeyColumn"
        /// and set UpdatedRowSource = FirstReturnedRecord; the new key is then in the row.
        /// </summary>
        protected void UpdateDataSource(string table)
        {
            try
            {
                OpenConnection();
                daMain.Update(dsMain, table);
            }
            catch
            {
                dsMain.Tables[table].RejectChanges();
                throw;
            }
            finally
            {
                CloseConnection();
            }
        }
        #endregion

        #region Direct commands and transactions
        /// <summary>Runs an INSERT, UPDATE or DELETE and returns the number of rows changed.</summary>
        protected int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
        {
            using (SqlCommand command = new SqlCommand(sql, cnMain))
            {
                AddParameters(command, parameters);
                try
                {
                    OpenConnection();
                    return command.ExecuteNonQuery();
                }
                finally
                {
                    CloseConnection();
                }
            }
        }

        /// <summary>Runs a command inside an open transaction (see BeginTransaction).</summary>
        protected int ExecuteNonQuery(SqlTransaction transaction, string sql, params SqlParameter[] parameters)
        {
            using (SqlCommand command = new SqlCommand(sql, cnMain, transaction))
            {
                AddParameters(command, parameters);
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>Runs a query that returns one value (e.g. a COUNT or a new key). DBNull is returned as null.</summary>
        protected object ExecuteScalar(string sql, params SqlParameter[] parameters)
        {
            using (SqlCommand command = new SqlCommand(sql, cnMain))
            {
                AddParameters(command, parameters);
                try
                {
                    OpenConnection();
                    object value = command.ExecuteScalar();
                    if (value == DBNull.Value)
                    {
                        return null;
                    }
                    return value;
                }
                finally
                {
                    CloseConnection();
                }
            }
        }

        /// <summary>Runs a one-value query inside an open transaction. DBNull is returned as null.</summary>
        protected object ExecuteScalar(SqlTransaction transaction, string sql, params SqlParameter[] parameters)
        {
            using (SqlCommand command = new SqlCommand(sql, cnMain, transaction))
            {
                AddParameters(command, parameters);
                object value = command.ExecuteScalar();
                if (value == DBNull.Value)
                {
                    return null;
                }
                return value;
            }
        }

        /// <summary>
        /// Opens the connection and starts a transaction. Use it like this:
        /// <code>
        /// SqlTransaction transaction = BeginTransaction();
        /// try { ExecuteNonQuery(transaction, ...); transaction.Commit(); }
        /// catch { transaction.Rollback(); throw; }
        /// finally { CloseConnection(); }
        /// </code>
        /// </summary>
        protected SqlTransaction BeginTransaction()
        {
            OpenConnection();
            return cnMain.BeginTransaction();
        }

        /// <summary>Closes the connection if it is open.</summary>
        protected void CloseConnection()
        {
            if (cnMain.State != ConnectionState.Closed)
            {
                cnMain.Close();
            }
        }

        /// <summary>Opens the connection if it is closed.</summary>
        private void OpenConnection()
        {
            if (cnMain.State == ConnectionState.Closed)
            {
                cnMain.Open();
            }
        }

        /// <summary>Adds the parameters to a command.</summary>
        private static void AddParameters(SqlCommand command, SqlParameter[] parameters)
        {
            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }
        }
        #endregion

        #region Parameter and value helpers
        /// <summary>Makes a parameter with a value. A null value is sent as SQL NULL.</summary>
        protected static SqlParameter Param(string name, SqlDbType type, object value)
        {
            SqlParameter parameter = new SqlParameter(name, type);
            parameter.Value = DbValue(value);
            return parameter;
        }

        /// <summary>Makes an adapter parameter whose value is read from a DataTable column (Waitering Build_INSERT_Parameters).</summary>
        protected static SqlParameter SourceParam(string name, SqlDbType type, string sourceColumn)
        {
            return new SqlParameter(name, type, 0, sourceColumn);
        }

        /// <summary>The value to put in a DataRow: null becomes DBNull.</summary>
        protected static object DbValue(object value)
        {
            if (value == null)
            {
                return DBNull.Value;
            }
            return value;
        }

        /// <summary>Text with the spaces at the ends removed; null becomes "".</summary>
        protected static string TextOrEmpty(string value)
        {
            if (value == null)
            {
                return "";
            }
            return value.Trim();
        }

        /// <summary>The value to put in a DataRow for optional text: blank becomes DBNull, otherwise trimmed.</summary>
        protected static object DbText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return DBNull.Value;
            }
            return value.Trim();
        }

        /// <summary>Reads text from a row; DBNull becomes "".</summary>
        protected static string ReadString(DataRow row, string column)
        {
            if (row[column] is DBNull)
            {
                return "";
            }
            return Convert.ToString(row[column]).TrimEnd();
        }

        /// <summary>Reads a whole number that is never NULL (e.g. a key) from a row.</summary>
        protected static int ReadInt(DataRow row, string column)
        {
            return Convert.ToInt32(row[column]);
        }

        /// <summary>Reads a whole number from a column that allows NULL; DBNull becomes null.</summary>
        protected static int? ReadNullableInt(DataRow row, string column)
        {
            if (row[column] is DBNull)
            {
                return null;
            }
            return Convert.ToInt32(row[column]);
        }

        /// <summary>Reads a decimal number from a column that allows NULL; DBNull becomes null.</summary>
        protected static decimal? ReadDecimal(DataRow row, string column)
        {
            if (row[column] is DBNull)
            {
                return null;
            }
            return Convert.ToDecimal(row[column]);
        }

        /// <summary>Reads a date from a column that allows NULL; DBNull becomes null.</summary>
        protected static DateTime? ReadDate(DataRow row, string column)
        {
            if (row[column] is DBNull)
            {
                return null;
            }
            return Convert.ToDateTime(row[column]);
        }

        /// <summary>Reads a BIT from a row; DBNull becomes false.</summary>
        protected static bool ReadBool(DataRow row, string column)
        {
            if (row[column] is DBNull)
            {
                return false;
            }
            return Convert.ToBoolean(row[column]);
        }
        #endregion
    }
}
