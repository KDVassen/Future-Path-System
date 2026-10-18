
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using FuturePath.DatabaseLayer;

namespace FuturePath.ApplicationLayer
{
    // Manages system settings stored in the database
    // Loads parameters when needed and keeps them in memory for later use
    public static class SystemSettings
    {
        #region Data Members
        // Codes used to retrieve system settings from the database
        public const string MinSubjectCountCode = "MIN_SUBJECT_COUNT";
        public const string WeightSubjectCode = "WEIGHT_SUBJECT";
        public const string WeightMarkCode = "WEIGHT_MARK";
        public const string WeightInterestCode = "WEIGHT_INTEREST";
        public const string WeightFacultyCode = "WEIGHT_FACULTY";
        public const string ThresholdStrongCode = "THRESHOLD_STRONG";
        public const string InterestStrongMinCode = "INTEREST_STRONG_MIN";
        public const string MarkToleranceCode = "MARK_TOLERANCE";

        // Prefixes used for numbered settings such as levels, funding types and provinces
        public const string NscLevelPrefix = "NSC_LEVEL_";
        public const string FundingTypePrefix = "FUNDING_TYPE_";
        public const string ProvincePrefix = "PROVINCE_";

        private static Dictionary<string, string> parameters;
        #endregion

        #region Properties
        // Get the weights used to calculate recommendation scores
        public static decimal WeightSubject
        {
            get { return GetDecimal(WeightSubjectCode); }
        }

        public static decimal WeightMark
        {
            get { return GetDecimal(WeightMarkCode); }
        }

        public static decimal WeightInterest
        {
            get { return GetDecimal(WeightInterestCode); }
        }

        public static decimal WeightFaculty
        {
            get { return GetDecimal(WeightFacultyCode); }
        }

        // Get the thresholds used to determine recommendation outcomes
        public static decimal ThresholdStrong
        {
            get { return GetDecimal(ThresholdStrongCode); }
        }

        public static decimal InterestStrongMinimum
        {
            get { return GetDecimal(InterestStrongMinCode); }
        }

        public static decimal MarkTolerance
        {
            get { return GetDecimal(MarkToleranceCode); }
        }

        // Get the recommended number of subjects to capture
        public static int MinimumSubjectCount
        {
            get { return GetInt(MinSubjectCountCode); }
        }
        #endregion

        #region Methods
        // Load parameter values into memory
        // Also allows tests to supply settings without using the database
        public static void Load(IDictionary<string, string> values)
        {
            parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<string, string> pair in values)
            {
                parameters[pair.Key] = pair.Value;
            }
        }

        // Reload the latest parameter values from the database
        public static void Reload()
        {
            SystemParameterRepository repository = new SystemParameterRepository();
            Load(repository.GetParameters());
        }

        // Check whether a system parameter exists
        public static bool Contains(string code)
        {
            EnsureLoaded();
            return parameters.ContainsKey(code);
        }

        // Get the stored text for a system parameter
        // Throw an error if the requested parameter does not exist
        public static string GetText(string code)
        {
            EnsureLoaded();
            if (!parameters.ContainsKey(code))
            {
                throw new InvalidOperationException("The system setting " + code + " is missing. Ask an Administrator to check the SystemParameter table.");
            }
            return parameters[code];
        }

        // Read a system parameter as a whole number
        public static int GetInt(string code)
        {
            int value;
            if (!int.TryParse(GetText(code), NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
            {
                throw new InvalidOperationException("The system setting " + code + " is not a whole number.");
            }
            return value;
        }

        // Read a system parameter as a decimal number
        // Use invariant culture so decimal values are read consistently
        public static decimal GetDecimal(string code)
        {
            decimal value;
            if (!decimal.TryParse(GetText(code), NumberStyles.Number, CultureInfo.InvariantCulture, out value))
            {
                throw new InvalidOperationException("The system setting " + code + " is not a number.");
            }
            return value;
        }

        // Get the values of numbered system parameters in order
        // Stop when the next numbered parameter does not exist
        public static Collection<string> GetNumberedValues(string prefix)
        {
            Collection<string> values = new Collection<string>();
            foreach (DataRow row in GetNumberedParameters(prefix).Rows)
            {
                values.Add(Convert.ToString(row["ParameterValue"]));
            }
            return values;
        }

        // Create a table of numbered parameters for dropdown lists
        public static DataTable GetNumberedParameters(string prefix)
        {
            EnsureLoaded();
            DataTable table = new DataTable("Parameters");
            table.Columns.Add("ParameterCode", typeof(string));
            table.Columns.Add("ParameterValue", typeof(string));

            // Read each numbered parameter until one is missing
            int number = 1;
            string code = prefix + number.ToString(CultureInfo.InvariantCulture);
            while (parameters.ContainsKey(code))
            {
                table.Rows.Add(code, parameters[code]);
                number++;
                code = prefix + number.ToString(CultureInfo.InvariantCulture);
            }
            return table;
        }

        // Load parameters from the database if they have not been loaded yet
        private static void EnsureLoaded()
        {
            if (parameters == null)
            {
                Reload();
            }
        }
        #endregion
    }
}
