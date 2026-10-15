using System.Data;

namespace FuturePath.DatabaseLayer
{
    /// <summary>Data access for the GradeLevel lookup table (IDs match the GradeLevel enum).</summary>
    public class GradeLevelRepository : DB
    {
        #region Data Members
        private string table = "GradeLevel";
        private string sqlLocal = "SELECT GradeLevelID, GradeDescription, MarkBasis FROM GradeLevel";
        #endregion

        #region Constructor
        /// <summary>Creates the repository (no database work until a method is called).</summary>
        public GradeLevelRepository() : base()
        {
        }
        #endregion

        #region Read
        /// <summary>
        /// Columns GradeLevelID, GradeDescription, MarkBasis, ordered by GradeLevelID. A new table
        /// is returned every call, so the caller may add rows to it and bind it to several combos.
        /// </summary>
        public DataTable GetGradeLevels()
        {
            FillDataSet(sqlLocal + " ORDER BY GradeLevelID", table);
            return GetTable(table).Copy();
        }
        #endregion
    }
}
