using System;
using System.Data;

namespace FuturePath.DatabaseLayer
{
    /// <summary>Data access for the GradeLevel lookup table (IDs match the GradeLevel enum).</summary>
    public class GradeLevelRepository : DB
    {
        #region Data Members
        private string table = "GradeLevel";
        #endregion

        #region Constructor
        /// <summary>Creates the repository (no database work until a method is called).</summary>
        public GradeLevelRepository() : base()
        {
        }
        #endregion

        #region Read
        /// <summary>Columns GradeLevelID, GradeDescription, MarkBasis, ordered by GradeLevelID.</summary>
        public DataTable GetGradeLevels()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
