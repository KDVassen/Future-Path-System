
using System;
using System.Collections.Generic;
using System.Data;

namespace FuturePath.DatabaseLayer
{
    // Handles read-only database operations for system parameters
    public class SystemParameterRepository : DB
    {
        #region Data Members
        private string table = "SystemParameter";
        private string sqlLocal = "SELECT ParameterCode, ParameterValue FROM SystemParameter";
        #endregion

        #region Constructor
        // Initialise the system parameter repository
        public SystemParameterRepository() : base()
        {
        }
        #endregion

        #region Read
        // Load all system parameters into a case-insensitive dictionary
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
