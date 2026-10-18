
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using FuturePath.ApplicationLayer;

namespace FuturePath.DatabaseLayer
{
    // Handles database operations for staff user accounts
    // Loads account details and saves new or updated accounts
    public class UserAccountRepository : DB
    {
        #region Data Members
        private string table = "UserAccount";
        private string sqlLocal = "SELECT UserID, Username, PasswordHash, FullName, Role, IsActive FROM UserAccount";
        private Collection<UserAccount> accounts;
        #endregion

        #region Constructor
        // Initialise the user account collection
        public UserAccountRepository() : base()
        {
            accounts = new Collection<UserAccount>();
        }
        #endregion

        #region Read
        // Find a staff account using its username
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

        // Find a user account by ID or return null if it does not exist
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

        // Load all user accounts ordered by full name
        public Collection<UserAccount> GetAccounts()
        {
            FillDataSet(sqlLocal + " ORDER BY FullName", table);
            Add2Collection();
            return accounts;
        }

        // Check whether another account already has the same username
        public bool UsernameExists(string username, int excludeUserID)
        {
            object count = ExecuteScalar("SELECT COUNT(*) FROM UserAccount WHERE Username = @Username AND UserID <> @ExcludeID",
                                         Param("@Username", SqlDbType.VarChar, TextOrEmpty(username)),
                                         Param("@ExcludeID", SqlDbType.Int, excludeUserID));
            return Convert.ToInt32(count) > 0;
        }
        #endregion

        #region Utility Methods
        // Convert database rows into UserAccount objects
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
        // Add a new account or update an existing one in the DataSet
        public void DataSetChange(UserAccount account, DB.DBOperation operation)
        {
            DataRow row;
            if (operation == DB.DBOperation.Add)
            {
                // Load the table structure before adding a new account
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

        // Save account changes to the database and return the account ID
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
                // Read the new account ID from the inserted row
                DataTable accountTable = GetTable(table);
                account.UserID = ReadInt(accountTable.Rows[accountTable.Rows.Count - 1], "UserID");
            }
            return account.UserID;
        }

        // Copy account details into a DataRow
        // Store only the password hash, never the actual password
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
        // Add the required parameters to the INSERT command
        private void Build_INSERT_Parameters(UserAccount account)
        {
            daMain.InsertCommand.Parameters.Add(SourceParam("@Username", SqlDbType.VarChar, "Username"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@PasswordHash", SqlDbType.VarChar, "PasswordHash"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@FullName", SqlDbType.VarChar, "FullName"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@Role", SqlDbType.VarChar, "Role"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@IsActive", SqlDbType.Bit, "IsActive"));
        }

        // Create the SQL command for inserting a new account
        private void Create_INSERT_Command(UserAccount account)
        {
            daMain.InsertCommand = new SqlCommand("INSERT INTO UserAccount (Username, PasswordHash, FullName, Role, IsActive) " +
                                                  "VALUES (@Username, @PasswordHash, @FullName, @Role, @IsActive); " +
                                                  "SELECT CAST(SCOPE_IDENTITY() AS INT) AS UserID", cnMain);
            daMain.InsertCommand.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;
            Build_INSERT_Parameters(account);
        }

        // Add the required parameters to the UPDATE command
        private void Build_UPDATE_Parameters(UserAccount account)
        {
            daMain.UpdateCommand.Parameters.Add(SourceParam("@Username", SqlDbType.VarChar, "Username"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@PasswordHash", SqlDbType.VarChar, "PasswordHash"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@FullName", SqlDbType.VarChar, "FullName"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@Role", SqlDbType.VarChar, "Role"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@IsActive", SqlDbType.Bit, "IsActive"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@UserID", SqlDbType.Int, "UserID"));
        }

        // Create the SQL command for updating an existing account
        private void Create_UPDATE_Command(UserAccount account)
        {
            daMain.UpdateCommand = new SqlCommand("UPDATE UserAccount SET Username = @Username, PasswordHash = @PasswordHash, FullName = @FullName, " +
                                                  "Role = @Role, IsActive = @IsActive WHERE UserID = @UserID", cnMain);
            Build_UPDATE_Parameters(account);
        }
        #endregion
    }
}
