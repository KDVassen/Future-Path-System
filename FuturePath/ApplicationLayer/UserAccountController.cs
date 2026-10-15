using System;
using System.Collections.ObjectModel;
using System.Data;
using FuturePath.DatabaseLayer;

namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// UI 13.6 User accounts (decisions.md D5): Administrators create, update and deactivate staff
    /// accounts. Every public method that reads or changes accounts checks the Administrator role
    /// first. Passwords are stored only as salted hashes, and the hash is never returned to a form.
    /// </summary>
    public class UserAccountController
    {
        #region Data Members
        private const int MaximumFullNameLength = 100;

        private UserAccountRepository userAccountRepository;
        private LoginController loginController;
        private Collection<UserAccount> accounts;
        #endregion

        #region Properties
        /// <summary>The accounts read by the last GetAccounts call (their PasswordHash is blanked).</summary>
        public Collection<UserAccount> AllAccounts
        {
            get { return accounts; }
        }
        #endregion

        #region Constructor
        /// <summary>Creates the controller and its repository (no database work yet).</summary>
        public UserAccountController()
        {
            userAccountRepository = new UserAccountRepository();
            loginController = new LoginController();
            accounts = new Collection<UserAccount>();
        }
        #endregion

        #region Read
        /// <summary>The roles an account can have: "Administrator", "Career Advisor" (from the UserAccount constants).</summary>
        public Collection<string> GetRoles()
        {
            Collection<string> roles = new Collection<string>();
            roles.Add(UserAccount.RoleAdministratorText);
            roles.Add(UserAccount.RoleCareerAdvisorText);
            return roles;
        }

        /// <summary>
        /// UI 13.6 grid (Administrator only): UserID, Username, FullName, Role (text), IsActive,
        /// ordered by FullName. Never the hash. Throws UnauthorizedAccessException if not an Administrator.
        /// </summary>
        public DataTable GetAccounts(int actorUserID)
        {
            loginController.CheckAdministrator(actorUserID);
            accounts = userAccountRepository.GetAccounts();

            DataTable table = new DataTable("Accounts");
            table.Columns.Add("UserID", typeof(int));
            table.Columns.Add("Username", typeof(string));
            table.Columns.Add("FullName", typeof(string));
            table.Columns.Add("Role", typeof(string));
            table.Columns.Add("IsActive", typeof(bool));
            foreach (UserAccount account in accounts)
            {
                table.Rows.Add(account.UserID, account.Username, account.FullName, account.RoleName, account.IsActive);
                // The accounts kept for AllAccounts are for display only, so they never carry the hash.
                account.PasswordHash = "";
            }
            return table;
        }
        #endregion

        #region Maintenance
        /// <summary>
        /// Checks an account before saving and returns one message per problem (empty when valid):
        /// username required, format (UserAccount.IsValidUsername) and unique; full name required
        /// (max 100); role valid; a password of at least 8 characters on Add (optional on Update);
        /// an Administrator cannot deactivate their own account or remove their own Administrator role.
        /// </summary>
        public Collection<string> ValidateAccount(int actorUserID, int userID, string username, string fullName, string role, bool isActive, string newPassword)
        {
            Collection<string> problems = new Collection<string>();

            if (!UserAccount.IsValidUsername(username))
            {
                problems.Add("Enter a username of up to 50 letters, digits, full stops, underscores or hyphens, with no spaces.");
            }
            else if (userAccountRepository.UsernameExists(username, userID))
            {
                problems.Add("The username " + username.Trim() + " is already taken. Choose a different username.");
            }

            if (string.IsNullOrWhiteSpace(fullName))
            {
                problems.Add("Enter the staff member's full name.");
            }
            else if (fullName.Trim().Length > MaximumFullNameLength)
            {
                problems.Add("Shorten the full name to " + MaximumFullNameLength + " characters or fewer.");
            }

            if (!UserAccount.IsValidRoleText(role))
            {
                problems.Add("Choose a role: " + UserAccount.RoleAdministratorText + " or " + UserAccount.RoleCareerAdvisorText + ".");
            }

            if (userID == 0)
            {
                // A new account must have a password.
                if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < UserAccount.MinimumPasswordLength)
                {
                    problems.Add("Enter a password of at least " + UserAccount.MinimumPasswordLength + " characters.");
                }
            }
            else if (!string.IsNullOrWhiteSpace(newPassword) && newPassword.Length < UserAccount.MinimumPasswordLength)
            {
                problems.Add("Enter a new password of at least " + UserAccount.MinimumPasswordLength + " characters, or leave it blank to keep the current one.");
            }

            // An Administrator may not lock themselves out.
            if (userID == actorUserID)
            {
                if (!isActive)
                {
                    problems.Add("You cannot deactivate your own account. Ask another Administrator to do it.");
                }
                if (UserAccount.IsValidRoleText(role) && UserAccount.RoleFromText(role) != UserAccount.RoleType.Administrator)
                {
                    problems.Add("You cannot remove your own Administrator role. Ask another Administrator to do it.");
                }
            }
            return problems;
        }

        /// <summary>
        /// Adds (userID 0) or updates an account (Administrator only). A non-blank newPassword
        /// sets a new salted hash; blank keeps the old one on Update. Returns the UserID.
        /// Throws ArgumentException if invalid, UnauthorizedAccessException if not an Administrator.
        /// </summary>
        public int SaveAccount(int actorUserID, int userID, string username, string fullName, string role, bool isActive, string newPassword)
        {
            loginController.CheckAdministrator(actorUserID);
            Collection<string> problems = ValidateAccount(actorUserID, userID, username, fullName, role, isActive, newPassword);
            if (problems.Count > 0)
            {
                throw new ArgumentException(problems[0]);
            }

            UserAccount account;
            DB.DBOperation operation;
            if (userID == 0)
            {
                account = new UserAccount(0, username.Trim(), "", fullName.Trim(), UserAccount.RoleFromText(role), isActive);
                account.SetPassword(newPassword);
                operation = DB.DBOperation.Add;
            }
            else
            {
                account = userAccountRepository.FindById(userID);
                if (account == null)
                {
                    throw new ArgumentException("The selected account no longer exists. Refresh the list and try again.");
                }
                account.Username = username.Trim();
                account.FullName = fullName.Trim();
                account.Role = UserAccount.RoleFromText(role);
                account.IsActive = isActive;
                if (!string.IsNullOrWhiteSpace(newPassword))
                {
                    // Password reset: only the new salted hash is stored.
                    account.SetPassword(newPassword);
                }
                operation = DB.DBOperation.Edit;
            }

            DataMaintenance(account, operation);
            return FinalizeChanges(account, operation);
        }

        /// <summary>Deactivates an account (Administrator only). Refuses the Administrator's own account.</summary>
        public void DeactivateAccount(int actorUserID, int userID)
        {
            loginController.CheckAdministrator(actorUserID);
            if (userID == actorUserID)
            {
                throw new ArgumentException("You cannot deactivate your own account. Ask another Administrator to do it.");
            }

            UserAccount account = userAccountRepository.FindById(userID);
            if (account == null)
            {
                throw new ArgumentException("The selected account no longer exists. Refresh the list and try again.");
            }
            account.IsActive = false;
            DataMaintenance(account, DB.DBOperation.Edit);
            FinalizeChanges(account, DB.DBOperation.Edit);
        }

        /// <summary>Waitering DataMaintenance: makes the change in the repository's DataSet. Private, so the role check cannot be skipped.</summary>
        private void DataMaintenance(UserAccount account, DB.DBOperation operation)
        {
            userAccountRepository.DataSetChange(account, operation);
        }

        /// <summary>Waitering FinalizeChanges: commits the change to the database and returns the UserID.</summary>
        private int FinalizeChanges(UserAccount account, DB.DBOperation operation)
        {
            return userAccountRepository.UpdateDataSource(account, operation);
        }
        #endregion
    }
}
