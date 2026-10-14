using System;
using System.Collections.ObjectModel;
using FuturePath.ApplicationLayer;

namespace FuturePath.DatabaseLayer
{
    /// <summary>
    /// Data access for staff accounts (UI 8 login, UI 13.6 maintenance), in the Waitering
    /// DataSetChange / UpdateDataSource pattern. UserAccount is the single source of truth for login.
    /// </summary>
    public class UserAccountRepository : DB
    {
        #region Data Members
        private string table = "UserAccount";
        #endregion

        #region Constructor
        /// <summary>Creates the repository (no database work until a method is called).</summary>
        public UserAccountRepository() : base()
        {
        }
        #endregion

        #region Read
        /// <summary>The account with this username (not case-sensitive), or null.</summary>
        public UserAccount FindByUsername(string username)
        {
            throw new NotImplementedException();
        }

        /// <summary>One account, or null.</summary>
        public UserAccount FindById(int userID)
        {
            throw new NotImplementedException();
        }

        /// <summary>Every account, ordered by FullName.</summary>
        public Collection<UserAccount> GetAccounts()
        {
            throw new NotImplementedException();
        }

        /// <summary>True if another account already has this username.</summary>
        public bool UsernameExists(string username, int? excludeUserID)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Maintenance - Waitering pattern
        /// <summary>Adds a new account row to the DataSet, or changes the existing row (Edit).</summary>
        public void DataSetChange(UserAccount account, DB.DBOperation operation)
        {
            throw new NotImplementedException();
        }

        /// <summary>Sends the account change to the database. Returns the UserID (the new key for Add).</summary>
        public int UpdateDataSource(UserAccount account, DB.DBOperation operation)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
