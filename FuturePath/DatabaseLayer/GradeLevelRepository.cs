
using System.Data;

namespace FuturePath.DatabaseLayer
{
    // Handles database access for grade levels
    public class GradeLevelRepository : DB
    {
        #region Data Members
        private string table = "GradeLevel";
        private string sqlLocal = "SELECT GradeLevelID, GradeDescription, MarkBasis FROM GradeLevel";
        #endregion

        #region Constructor
        // Initialise the grade level repository
        public GradeLevelRepository() : base()
        {
        }
        #endregion

        #region Read
        // Load grade levels ordered by ID and return a copy of the table
        public DataTable GetGradeLevels()
        {
            FillDataSet(sqlLocal + " ORDER BY GradeLevelID", table);
            return GetTable(table).Copy();
        }
        #endregion
    }
}
