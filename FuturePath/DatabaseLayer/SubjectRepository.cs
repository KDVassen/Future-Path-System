using System;
using System.Collections.ObjectModel;
using FuturePath.ApplicationLayer;

namespace FuturePath.DatabaseLayer
{
    /// <summary>Data access for school subjects (read-only: the specification has no subject maintenance screen).</summary>
    public class SubjectRepository : DB
    {
        #region Data Members
        private string table = "Subject";
        #endregion

        #region Constructor
        /// <summary>Creates the repository (no database work until a method is called).</summary>
        public SubjectRepository() : base()
        {
        }
        #endregion

        #region Read
        /// <summary>Subjects ordered by name; inactive ones only when includeInactive is true.</summary>
        public Collection<Subject> GetSubjects(bool includeInactive)
        {
            throw new NotImplementedException();
        }

        /// <summary>One subject, or null.</summary>
        public Subject FindById(int subjectID)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
