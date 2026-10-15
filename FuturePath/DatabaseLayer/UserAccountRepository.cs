using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
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
            if (accounts.Count == 0)
            {
                return null;
            }
            return accounts[0];
        }

        /// <summary>One account, or null.</summary>
        public UserAccount FindById(int userID)
        {
            FillDataSet(sqlLocal + " WHERE UserID = @UserID", table, Param("@UserID", SqlDbType.Int, userID));
            Add2Collection();
            if (accounts.Count == 0)
            {
                return null;
            }
            return accounts[0];
        }

        /// <summary>Every account, ordered by FullName.</summary>
        public Collection<UserAccount> GetAccounts()
        {
            FillDataSet(sqlLocal + " ORDER BY FullName", table);
            Add2Collection();
            return accounts;
        }

        /// <summary>True if another account already has this username.</summary>
        public bool UsernameExists(string username, int excludeUserID)
        {
            object count = ExecuteScalar("SELECT COUNT(*) FROM UserAccount WHERE Username = @Username AND UserID <> @ExcludeID",
                                         Param("@Username", SqlDbType.VarChar, TextOrEmpty(username)),
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
                accounts.Add(new UserAccount(ReadInt(row, "UserID"), ReadString(row, "Username"), ReadString(row, "PasswordHash"),
                                             ReadString(row, "FullName"), UserAccount.RoleFromText(ReadString(row, "Role")),
                                             ReadBool(row, "IsActive")));
            }
        }
        #endregion

        #region Maintenance - Waitering pattern
        /// <summary>Adds a new account row to the DataSet, or changes the existing row (Edit).</summary>
        public void DataSetChange(UserAccount account, DB.DBOperation operation)
        {
            DataRow row;
            if (operation == DB.DBOperation.Add)
            {
                // An empty fill gives the table its columns; the new row is added to it.
                FillDataSet(sqlLocal + " WHERE 1 = 0", table);
                row = GetTable(table).NewRow();
                FillRow(row, account);
                GetTable(table).Rows.Add(row);
            }
            else
            {
                FillDataSet(sqlLocal + " WHERE UserID = @UserID", table, Param("@UserID", SqlDbType.Int, account.UserID));
                row = FindRow(table, "UserID", account.UserID);
                if (row == null)
                {
                    throw new InvalidOperationException("The user account to update was not found.");
                }
                FillRow(row, account);
            }
        }

        /// <summary>Sends the account change to the database. Returns the UserID (the new key for Add).</summary>
        public int UpdateDataSource(UserAccount account, DB.DBOperation operation)
        {
            if (operation == DB.DBOperation.Add)
            {
                Create_INSERT_Command(account);
            }
            else
            {
                Create_UPDATE_Command(account);
            }
            UpdateDataSource(table);
            if (operation == DB.DBOperation.Add)
            {
                // The INSERT returned the new key into the row (FirstReturnedRecord).
                DataTable accountTable = GetTable(table);
                account.UserID = ReadInt(accountTable.Rows[accountTable.Rows.Count - 1], "UserID");
            }
            return account.UserID;
        }

        /// <summary>
        /// Copies the account's values into a row (Waitering FillRow). The key is never changed
        /// here. Only the salted hash is stored, never a password.
        /// </summary>
        private void FillRow(DataRow row, UserAccount account)
        {
            row["Username"] = account.Username.Trim();
            row["PasswordHash"] = account.PasswordHash;
            row["FullName"] = account.FullName.Trim();
            row["Role"] = account.RoleName;
            row["IsActive"] = account.IsActive;
        }
        #endregion

        #region Build Parameters, Create Commands
        /// <summary>The INSERT parameters, each read from a column of the new row (Waitering Build_INSERT_Parameters).</summary>
        private void Build_INSERT_Parameters(UserAccount account)
        {
            daMain.InsertCommand.Parameters.Add(SourceParam("@Username", SqlDbType.VarChar, "Username"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@PasswordHash", SqlDbType.VarChar, "PasswordHash"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@FullName", SqlDbType.VarChar, "FullName"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@Role", SqlDbType.VarChar, "Role"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@IsActive", SqlDbType.Bit, "IsActive"));
        }

        /// <summary>The INSERT command. It returns the new UserID into the row.</summary>
        private void Create_INSERT_Command(UserAccount account)
        {
            daMain.InsertCommand = new SqlCommand("INSERT INTO UserAccount (Username, PasswordHash, FullName, Role, IsActive) " +
                                                  "VALUES (@Username, @PasswordHash, @FullName, @Role, @IsActive); " +
                                                  "SELECT CAST(SCOPE_IDENTITY() AS INT) AS UserID", cnMain);
            daMain.InsertCommand.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;
            Build_INSERT_Parameters(account);
        }

        /// <summary>The UPDATE parameters, each read from a column of the changed row.</summary>
        private void Build_UPDATE_Parameters(UserAccount account)
        {
            daMain.UpdateCommand.Parameters.Add(SourceParam("@Username", SqlDbType.VarChar, "Username"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@PasswordHash", SqlDbType.VarChar, "PasswordHash"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@FullName", SqlDbType.VarChar, "FullName"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@Role", SqlDbType.VarChar, "Role"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@IsActive", SqlDbType.Bit, "IsActive"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@UserID", SqlDbType.Int, "UserID"));
        }

        /// <summary>The UPDATE command for one account.</summary>
        private void Create_UPDATE_Command(UserAccount account)
        {
            daMain.UpdateCommand = new SqlCommand("UPDATE UserAccount SET Username = @Username, PasswordHash = @PasswordHash, FullName = @FullName, " +
                                                  "Role = @Role, IsActive = @IsActive WHERE UserID = @UserID", cnMain);
            Build_UPDATE_Parameters(account);
        }
        #endregion
    }
}
