
using System.Collections.ObjectModel;
using System.Data;
using FuturePath.ApplicationLayer;

namespace FuturePath.DatabaseLayer
{
    // Handles read-only database operations for school subjects
    public class SubjectRepository : DB
    {
        #region Data Members
        private string table = "Subject";
        private string sqlLocal = "SELECT SubjectID, SubjectName, SubjectLevel, IsActive FROM Subject";
        private Collection<Subject> subjects;
        #endregion

        #region Constructor
        // Initialise the subject collection
        public SubjectRepository() : base()
        {
            subjects = new Collection<Subject>();
        }
        #endregion

        #region Read
        // Load subjects ordered by name
        // Include inactive subjects when requested
        public Collection<Subject> GetSubjects(bool includeInactive)
        {
            string sql = sqlLocal;
            if (!includeInactive)
            {
                sql += " WHERE IsActive = 1";
            }
            FillDataSet(sql + " ORDER BY SubjectName", table);
            Add2Collection();
            return subjects;
        }

        // Find a subject by its ID or return null if it does not exist
        public Subject FindById(int subjectID)
        {
            FillDataSet(sqlLocal + " WHERE SubjectID = @SubjectID", table, Param("@SubjectID", SqlDbType.Int, subjectID));
            Add2Collection();
            if (subjects.Count == 0)
            {
                return null;
            }
            return subjects[0];
        }
        #endregion

        #region Utility Methods
        // Convert database rows into Subject objects
        private void Add2Collection()
        {
            subjects = new Collection<Subject>();
            foreach (DataRow row in GetTable(table).Rows)
            {
                subjects.Add(new Subject(ReadInt(row, "SubjectID"), ReadString(row, "SubjectName"),
                                         ReadString(row, "SubjectLevel"), ReadBool(row, "IsActive")));
            }
        }
        #endregion
    }
}
