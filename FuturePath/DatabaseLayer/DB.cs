
using System;
using System.Data;
using System.Data.SqlClient;
using FuturePath.Properties;

namespace FuturePath.DatabaseLayer
{
    // Base class for database repositories
    // Handles database connections, DataSets, commands and transactions
    public class DB
    {
        #region Data Members
        // Types of changes that can be made to database records
        public enum DBOperation
        {
            Add = 0,
            Edit = 1
        }

        // Get the database connection string from the project settings
        private string strConn = Settings.Default.FuturePathDatabaseConnectionString;

        // Shared connection, DataSet and adapter used by repositories
        protected SqlConnection cnMain;
        protected DataSet dsMain;
        protected SqlDataAdapter daMain;
        #endregion

        #region Constructor
        // Initialise the connection, DataSet and adapter without opening the database
        public DB()
        {
            cnMain = new SqlConnection(strConn);
            dsMain = new DataSet();
            daMain = new SqlDataAdapter();
        }
        #endregion

        #region Fill the DataSet
        // Load database records into a DataSet table using a parameterised query
        // Clear existing rows first to prevent duplicates
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

        // Return a previously loaded table from the DataSet
        protected DataTable GetTable(string table)
        {
            return dsMain.Tables[table];
        }

        // Return the repository's DataSet
        public DataSet GetDataSet()
        {
            return dsMain;
        }

        // Find a record in a loaded table using its key
        // Return null if the record cannot be found
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
        // Save DataSet changes to the database using the adapter
        // Reject the changes if the database operation fails
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
        // Execute an INSERT, UPDATE or DELETE command
        // Return the number of affected rows
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

        // Execute a database command within an existing transaction
        protected int ExecuteNonQuery(SqlTransaction transaction, string sql, params SqlParameter[] parameters)
        {
            using (SqlCommand command = new SqlCommand(sql, cnMain, transaction))
            {
                AddParameters(command, parameters);
                return command.ExecuteNonQuery();
            }
        }

        // Execute a query that returns a single value
        // Convert a database NULL result to null
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

        // Execute a single-value query inside an existing transaction
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

        // Open a connection and begin a database transaction
        protected SqlTransaction BeginTransaction()
        {
            OpenConnection();
            return cnMain.BeginTransaction();
        }

        // Close the database connection if it is open
        protected void CloseConnection()
        {
            if (cnMain.State != ConnectionState.Closed)
            {
                cnMain.Close();
            }
        }

        // Open the database connection if it is closed
        private void OpenConnection()
        {
            if (cnMain.State == ConnectionState.Closed)
            {
                cnMain.Open();
            }
        }

        // Add SQL parameters to a database command
        private static void AddParameters(SqlCommand command, SqlParameter[] parameters)
        {
            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }
        }
        #endregion

        #region Parameter and value helpers
        // Create a SQL parameter and convert null values to DBNull
        protected static SqlParameter Param(string name, SqlDbType type, object value)
        {
            SqlParameter parameter = new SqlParameter(name, type);
            parameter.Value = DbValue(value);
            return parameter;
        }

        // Create a SQL parameter linked to a DataTable column
        protected static SqlParameter SourceParam(string name, SqlDbType type, string sourceColumn)
        {
            return new SqlParameter(name, type, 0, sourceColumn);
        }

        // Convert null values to DBNull for database operations
        protected static object DbValue(object value)
        {
            if (value == null)
            {
                return DBNull.Value;
            }
            return value;
        }

        // Remove extra spaces or return an empty string for null
        protected static string TextOrEmpty(string value)
        {
            if (value == null)
            {
                return "";
            }
            return value.Trim();
        }

        // Convert blank text to DBNull or trim the supplied value
        protected static object DbText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return DBNull.Value;
            }
            return value.Trim();
        }

        // Read text from a database row
        // Return an empty string for DBNull
        protected static string ReadString(DataRow row, string column)
        {
            if (row[column] is DBNull)
            {
                return "";
            }
            return Convert.ToString(row[column]).TrimEnd();
        }

        // Read an integer value from a database row
        protected static int ReadInt(DataRow row, string column)
        {
            return Convert.ToInt32(row[column]);
        }

        // Read an optional integer and return null for DBNull
        protected static int? ReadNullableInt(DataRow row, string column)
        {
            if (row[column] is DBNull)
            {
                return null;
            }
            return Convert.ToInt32(row[column]);
        }

        // Read an optional decimal value and return null for DBNull
        protected static decimal? ReadDecimal(DataRow row, string column)
        {
            if (row[column] is DBNull)
            {
                return null;
            }
            return Convert.ToDecimal(row[column]);
        }

        // Read an optional date and return null for DBNull
        protected static DateTime? ReadDate(DataRow row, string column)
        {
            if (row[column] is DBNull)
            {
                return null;
            }
            return Convert.ToDateTime(row[column]);
        }

        // Read a boolean value and return false for DBNull
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
