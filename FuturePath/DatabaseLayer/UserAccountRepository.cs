using System;
using System.Collections.ObjectModel;
using System.Data;
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
        private string sqlLocal = "SELECT UserID, Username, PasswordHash, FullName, Role, IsActive FROM UserAccount";
        private Collection<UserAccount> accounts;
        #endregion

        #region Constructor
        /// <summary>Creates the repository (no database work until a method is called).</summary>
        public UserAccountRepository() : base()
        {
            accounts = new Collection<UserAccount>();
        }
        #endregion

        #region Read
        /// <summary>The account with this username (not case-sensitive), or null.</summary>
        public UserAccount FindByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return null;
            }
            FillDataSet(sqlLocal + " WHERE Username = @Username", table, Param("@Username", SqlDbType.VarChar, username.Trim()));
            Add2Collection();
            return accounts.Count > 0 ? accounts[0] : null;
        }

        /// <summary>One account, or null.</summary>
        public UserAccount FindById(int userID)
        {
            FillDataSet(sqlLocal + " WHERE UserID = @UserID", table, Param("@UserID", SqlDbType.Int, userID));
            Add2Collection();
            return accounts.Count > 0 ? accounts[0] : null;
        }

        /// <summary>Every account, ordered by FullName.</summary>
        public Collection<UserAccount> GetAccounts()
        {
            FillDataSet(sqlLocal + " ORDER BY FullName", table);
            Add2Collection();
            return accounts;
        }

        /// <summary>True if another account already has this username.</summary>
        public bool UsernameExists(string username, int? excludeUserID)
        {
            object count = ExecuteScalar("SELECT COUNT(*) FROM UserAccount WHERE Username = @Username AND (@ExcludeID IS NULL OR UserID <> @ExcludeID)",
                                         Param("@Username", SqlDbType.VarChar, (username ?? "").Trim()),
                                         Param("@ExcludeID", SqlDbType.Int, excludeUserID));
            return Convert.ToInt32(count) > 0;
        }
        #endregion

        #region Utility Methods
        /// <summary>Turns the filled UserAccount rows into UserAccount objects (Waitering Add2Collection).</summary>
        private void Add2Collection()
        {
            accounts = new Collection<UserAccount>();
            foreach (DataRow row in GetTable(table).Rows)
            {
                accounts.Add(new UserAccount(ReadInt(row, "UserID").Value, ReadString(row, "Username"), ReadString(row, "PasswordHash"),
                                             ReadString(row, "FullName"), UserAccount.RoleFromText(ReadString(row, "Role")),
                                             ReadBool(row, "IsActive")));
            }
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
