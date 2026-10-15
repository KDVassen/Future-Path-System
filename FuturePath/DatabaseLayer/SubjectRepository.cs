using System.Collections.ObjectModel;
using System.Data;
using FuturePath.ApplicationLayer;

namespace FuturePath.DatabaseLayer
{
    /// <summary>Data access for school subjects (read-only: the specification has no subject maintenance screen).</summary>
    public class SubjectRepository : DB
    {
        #region Data Members
        private string table = "Subject";
        private string sqlLocal = "SELECT SubjectID, SubjectName, SubjectLevel, IsActive FROM Subject";
        private Collection<Subject> subjects;
        #endregion

        #region Constructor
        /// <summary>Creates the repository (no database work until a method is called).</summary>
        public SubjectRepository() : base()
        {
            subjects = new Collection<Subject>();
        }
        #endregion

        #region Read
        /// <summary>Subjects ordered by name; inactive ones only when includeInactive is true.</summary>
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

        /// <summary>One subject, or null.</summary>
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
        /// <summary>Turns the filled Subject rows into Subject objects (Waitering Add2Collection).</summary>
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
