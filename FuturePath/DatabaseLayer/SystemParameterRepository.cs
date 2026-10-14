using System;
using System.Collections.Generic;
using System.Data;

namespace FuturePath.DatabaseLayer
{
    /// <summary>
    /// Reads the SystemParameter table for the SystemSettings helper. Parameters are only read
    /// here; no screen in the specification edits them.
    /// </summary>
    public class SystemParameterRepository : DB
    {
        #region Data Members
        private string table = "SystemParameter";
        private string sqlLocal = "SELECT ParameterCode, ParameterValue FROM SystemParameter";
        #endregion

        #region Constructor
        /// <summary>Creates the repository (no database work until a method is called).</summary>
        public SystemParameterRepository() : base()
        {
        }
        #endregion

        #region Read
        /// <summary>Returns every parameter as code to value (codes are not case-sensitive).</summary>
        public Dictionary<string, string> GetParameters()
        {
            FillDataSet(sqlLocal, table);
            Dictionary<string, string> parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (DataRow row in GetTable(table).Rows)
            {
                parameters[ReadString(row, "ParameterCode")] = ReadString(row, "ParameterValue");
            }
            return parameters;
        }
        #endregion
    }
}
