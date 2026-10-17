
using System;
using System.Collections.ObjectModel;
using System.Data;
using FuturePath.DatabaseLayer;

namespace FuturePath.ApplicationLayer
{
    // Handles staff account management for administrators
    // Allows accounts to be created, updated and deactivated
    public class UserAccountController
    {
        #region Data Members
        private const int MaximumFullNameLength = 100;

        private UserAccountRepository userAccountRepository;
        private LoginController loginController;
        private Collection<UserAccount> accounts;
        #endregion

        #region Properties
        // Return the accounts loaded for display without their password hashes
        public Collection<UserAccount> AllAccounts
        {
            get { return accounts; }
        }
        #endregion

        #region Constructor
        // Initialise the user account repository and login controller
        public UserAccountController()
        {
            userAccountRepository = new UserAccountRepository();
            loginController = new LoginController();
            accounts = new Collection<UserAccount>();
        }
        #endregion

        #region Read
        // Get the available staff roles
        public Collection<string> GetRoles()
        {
            Collection<string> roles = new Collection<string>();
            roles.Add(UserAccount.RoleAdministratorText);
            roles.Add(UserAccount.RoleCareerAdvisorText);
            return roles;
        }

        // Load staff accounts for administrator maintenance
        // Remove password hashes from the accounts used for display
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

                // Clear the hash so it is not exposed through AllAccounts
                account.PasswordHash = "";
            }
            return table;
        }
        #endregion

        #region Maintenance
        // Validate usernames, names, roles and passwords before saving
        // Prevent administrators from deactivating or demoting themselves
        public Collection<string> ValidateAccount(int actorUserID, int userID, string username, string fullName, string role, bool isActive, string newPassword)
        {
            Collection<string> problems = new Collection<string>();

            // Check the username format and ensure it is unique
            if (!UserAccount.IsValidUsername(username))
            {
                problems.Add("Enter a username of up to 50 letters, digits, full stops, underscores or hyphens, with no spaces.");
            }
            else if (userAccountRepository.UsernameExists(username, userID))
            {
                problems.Add("The username " + username.Trim() + " is already taken. Choose a different username.");
            }

            // Check that a valid full name has been entered
            if (string.IsNullOrWhiteSpace(fullName))
            {
                problems.Add("Enter the staff member's full name.");
            }
            else if (fullName.Trim().Length > MaximumFullNameLength)
            {
                problems.Add("Shorten the full name to " + MaximumFullNameLength + " characters or fewer.");
            }

            // Check that the selected role is recognised
            if (!UserAccount.IsValidRoleText(role))
            {
                problems.Add("Choose a role: " + UserAccount.RoleAdministratorText + " or " + UserAccount.RoleCareerAdvisorText + ".");
            }

            // Require a password for new accounts and validate password changes
            if (userID == 0)
            {
                if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < UserAccount.MinimumPasswordLength)
                {
                    problems.Add("Enter a password of at least " + UserAccount.MinimumPasswordLength + " characters.");
                }
            }
            else if (!string.IsNullOrWhiteSpace(newPassword) && newPassword.Length < UserAccount.MinimumPasswordLength)
            {
                problems.Add("Enter a new password of at least " + UserAccount.MinimumPasswordLength + " characters, or leave it blank to keep the current one.");
            }

            // Prevent an administrator from removing their own access
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

        // Add or update a staff account after checking administrator access
        // Hash new passwords and keep the existing password when no change is given
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
                    // Generate a new salted hash when the password is changed
                    account.SetPassword(newPassword);
                }
                operation = DB.DBOperation.Edit;
            }

            DataMaintenance(account, operation);
            return FinalizeChanges(account, operation);
        }

        // Deactivate a staff account without deleting it
        // Prevent administrators from deactivating their own accounts
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

        // Apply account changes to the repository DataSet
        private void DataMaintenance(UserAccount account, DB.DBOperation operation)
        {
            userAccountRepository.DataSetChange(account, operation);
        }

        // Save account changes to the database
        private int FinalizeChanges(UserAccount account, DB.DBOperation operation)
        {
            return userAccountRepository.UpdateDataSource(account, operation);
        }
        #endregion
    }
}
