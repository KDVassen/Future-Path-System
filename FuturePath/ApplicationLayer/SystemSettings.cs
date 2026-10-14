using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using FuturePath.DatabaseLayer;

namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// The settings helper for the SystemParameter table (database.md "SystemParameter keys").
    /// It is not a domain class. Parameters are read once, the first time one is needed, and
    /// kept in memory. They hold every value that must not be hard-coded: the scoring weights
    /// and thresholds, the minimum subject count, the NSC achievement bands, the funding types
    /// and the provinces. Numbers are stored with a '.' decimal point and read with the
    /// invariant culture, so they parse the same on any Windows regional setting.
    /// </summary>
    public static class SystemSettings
    {
        #region Data Members
        /// <summary>Subjects a learner should capture (UI 3 amber notice; a warning, not a block).</summary>
        public const string MinSubjectCountCode = "MIN_SUBJECT_COUNT";
        /// <summary>Weight of the subject score in the overall score (provisional, decisions.md D14).</summary>
        public const string WeightSubjectCode = "WEIGHT_SUBJECT";
        /// <summary>Weight of the mark score in the overall score (provisional, decisions.md D14).</summary>
        public const string WeightMarkCode = "WEIGHT_MARK";
        /// <summary>Weight of the interest score in the overall score (provisional, decisions.md D14).</summary>
        public const string WeightInterestCode = "WEIGHT_INTEREST";
        /// <summary>Weight of the faculty bonus in the overall score (provisional, decisions.md D14).</summary>
        public const string WeightFacultyCode = "WEIGHT_FACULTY";
        /// <summary>Lowest overall score for a Strong Match (provisional, decisions.md D14).</summary>
        public const string ThresholdStrongCode = "THRESHOLD_STRONG";
        /// <summary>Lowest interest score for a Strong Match (provisional, decisions.md D14).</summary>
        public const string InterestStrongMinCode = "INTEREST_STRONG_MIN";
        /// <summary>Largest mark shortfall that is still a Possible Match (provisional, decisions.md D14).</summary>
        public const string MarkToleranceCode = "MARK_TOLERANCE";
        /// <summary>Prefix of the NSC achievement level lower bounds NSC_LEVEL_1 to NSC_LEVEL_7.</summary>
        public const string NscLevelPrefix = "NSC_LEVEL_";
        /// <summary>Prefix of the funding types FUNDING_TYPE_1 to FUNDING_TYPE_4.</summary>
        public const string FundingTypePrefix = "FUNDING_TYPE_";
        /// <summary>Prefix of the provinces PROVINCE_1 to PROVINCE_9.</summary>
        public const string ProvincePrefix = "PROVINCE_";

        private static Dictionary<string, string> parameters;
        #endregion

        #region Properties
        /// <summary>WEIGHT_SUBJECT as a number.</summary>
        public static decimal WeightSubject
        {
            get { return GetDecimal(WeightSubjectCode); }
        }

        /// <summary>WEIGHT_MARK as a number.</summary>
        public static decimal WeightMark
        {
            get { return GetDecimal(WeightMarkCode); }
        }

        /// <summary>WEIGHT_INTEREST as a number.</summary>
        public static decimal WeightInterest
        {
            get { return GetDecimal(WeightInterestCode); }
        }

        /// <summary>WEIGHT_FACULTY as a number.</summary>
        public static decimal WeightFaculty
        {
            get { return GetDecimal(WeightFacultyCode); }
        }

        /// <summary>THRESHOLD_STRONG as a number.</summary>
        public static decimal ThresholdStrong
        {
            get { return GetDecimal(ThresholdStrongCode); }
        }

        /// <summary>INTEREST_STRONG_MIN as a number.</summary>
        public static decimal InterestStrongMinimum
        {
            get { return GetDecimal(InterestStrongMinCode); }
        }

        /// <summary>MARK_TOLERANCE as a number.</summary>
        public static decimal MarkTolerance
        {
            get { return GetDecimal(MarkToleranceCode); }
        }

        /// <summary>MIN_SUBJECT_COUNT as a whole number.</summary>
        public static int MinimumSubjectCount
        {
            get { return GetInt(MinSubjectCountCode); }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Uses the given parameter values instead of reading the database. Used by tests that run
        /// the engine without a database, and by Reload.
        /// </summary>
        public static void Load(IDictionary<string, string> values)
        {
            parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<string, string> pair in values)
            {
                parameters[pair.Key] = pair.Value;
            }
        }

        /// <summary>Reads the parameters from the database again (after an Administrator changes them).</summary>
        public static void Reload()
        {
            SystemParameterRepository repository = new SystemParameterRepository();
            Load(repository.GetParameters());
        }

        /// <summary>True if the parameter exists.</summary>
        public static bool Contains(string code)
        {
            EnsureLoaded();
            return parameters.ContainsKey(code);
        }

        /// <summary>
        /// Returns a parameter's text. Throws InvalidOperationException naming the missing code,
        /// so a gap in the SystemParameter table is found rather than replaced by a hidden default.
        /// </summary>
        public static string GetText(string code)
        {
            EnsureLoaded();
            string value;
            if (!parameters.TryGetValue(code, out value))
            {
                throw new InvalidOperationException("The system setting " + code + " is missing. Ask an Administrator to check the SystemParameter table.");
            }
            return value;
        }

        /// <summary>Returns a parameter as a whole number.</summary>
        public static int GetInt(string code)
        {
            int value;
            if (!int.TryParse(GetText(code), NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
            {
                throw new InvalidOperationException("The system setting " + code + " is not a whole number.");
            }
            return value;
        }

        /// <summary>Returns a parameter as a decimal number (stored with a '.' decimal point).</summary>
        public static decimal GetDecimal(string code)
        {
            decimal value;
            if (!decimal.TryParse(GetText(code), NumberStyles.Number, CultureInfo.InvariantCulture, out value))
            {
                throw new InvalidOperationException("The system setting " + code + " is not a number.");
            }
            return value;
        }

        /// <summary>
        /// Returns the values of a numbered group in number order, e.g. FUNDING_TYPE_1,
        /// FUNDING_TYPE_2 and so on, stopping at the first missing number.
        /// </summary>
        public static Collection<string> GetNumberedValues(string prefix)
        {
            Collection<string> values = new Collection<string>();
            foreach (DataRow row in GetNumberedParameters(prefix).Rows)
            {
                values.Add(Convert.ToString(row["ParameterValue"]));
            }
            return values;
        }

        /// <summary>
        /// Returns a numbered group as a table with ParameterCode and ParameterValue columns,
        /// ready to bind to a ComboBox (DisplayMember ParameterValue).
        /// </summary>
        public static DataTable GetNumberedParameters(string prefix)
        {
            EnsureLoaded();
            DataTable table = new DataTable("Parameters");
            table.Columns.Add("ParameterCode", typeof(string));
            table.Columns.Add("ParameterValue", typeof(string));
            int number = 1;
            string value;
            while (parameters.TryGetValue(prefix + number.ToString(CultureInfo.InvariantCulture), out value))
            {
                table.Rows.Add(prefix + number.ToString(CultureInfo.InvariantCulture), value);
                number++;
            }
            return table;
        }

        /// <summary>Reads the parameters from the database the first time they are needed.</summary>
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
