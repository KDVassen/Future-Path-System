using System;
using System.Collections.ObjectModel;
using FuturePath.ApplicationLayer;

namespace FuturePath.DatabaseLayer
{
    /// <summary>
    /// Data access for faculties (UI 5 list, UI 13.1 maintenance), in the Waitering
    /// DataSetChange / UpdateDataSource pattern. Faculties are never deleted, only deactivated.
    /// </summary>
    public class FacultyRepository : DB
    {
        #region Data Members
        private string table = "Faculty";
        #endregion

        #region Constructor
        /// <summary>Creates the repository (no database work until a method is called).</summary>
        public FacultyRepository() : base()
        {
        }
        #endregion

        #region Read
        /// <summary>Faculties ordered by name; inactive ones only when includeInactive is true.</summary>
        public Collection<Faculty> GetFaculties(bool includeInactive)
        {
            throw new NotImplementedException();
        }

        /// <summary>One faculty, or null.</summary>
        public Faculty FindById(int facultyID)
        {
            throw new NotImplementedException();
        }

        /// <summary>True if another faculty already has this name.</summary>
        public bool FacultyNameExists(string facultyName, int? excludeFacultyID)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Maintenance - Waitering pattern
        /// <summary>Adds a new faculty row to the DataSet, or changes the existing row (Edit).</summary>
        public void DataSetChange(Faculty faculty, DB.DBOperation operation)
        {
            throw new NotImplementedException();
        }

        /// <summary>Sends the faculty change to the database. Returns the FacultyID (the new key for Add).</summary>
        public int UpdateDataSource(Faculty faculty, DB.DBOperation operation)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
