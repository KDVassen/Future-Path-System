
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FuturePath.ApplicationLayer;

namespace FuturePath.PresentationLayer
{
    // Handles user account management for Administrators
    // Passwords are cleared after use and are never displayed in the account list
    public partial class StaffConsoleForm
    {
        // Minimum length required for account passwords
        private const int AccountsMinimumPasswordLength = 8;

        // ID of the selected user, or null for a new account
        private int? accountsSelectedUserId;

        // Active status of the selected account
        private bool accountsSelectedIsActive;

        // Prevent selection events while refreshing the grid
        private bool accountsLoading;

        #region Page shown

        // Initialise the account screen and clear any entered passwords
        private void AccountsShown(bool firstTime)
        {
            if (!firstTime)
            {
                AccountsClearPasswords();
                return;
            }

            dgvAccounts.AutoGenerateColumns = false;
            AccountsLoadRoles();
            AccountsLoadAccounts();
            AccountsClearForm();
        }

        #endregion

        #region Button click events

        // Validate and add a new user account
        private void btnAccountsAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (!AccountsValidateForm(true))
                {
                    return;
                }

                string username = txtUsername.Text.Trim();
                try
                {
                    AccountsSaveAccount(0, txtNewPassword.Text);
                }
                catch (ArgumentException ex)
                {
                    // Display validation errors returned by the controller
                    ReportStatus(ex.Message, true);
                    return;
                }
                catch (Exception)
                {
                    ReportStatus("The user account could not be added. Please try again.", true);
                    return;
                }

                AccountsLoadAccounts();
                AccountsClearForm();
                ReportStatus("User account \"" + username + "\" added.", false);
            }
            finally
            {
                AccountsClearPasswords();
            }
        }

        // Update the selected account and optionally reset its password
        private void btnAccountsUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (!accountsSelectedUserId.HasValue)
                {
                    ReportStatus("Select a user account in the grid before you update it.", true);
                    return;
                }

                if (!AccountsValidateForm(false))
                {
                    return;
                }

                string username = txtUsername.Text.Trim();
                bool reset = txtNewPassword.Text.Length > 0;
                try
                {
                    // Leave the password unchanged when the field is empty
                    AccountsSaveAccount(accountsSelectedUserId.Value, txtNewPassword.Text);
                }
                catch (ArgumentException ex)
                {
                    ReportStatus(ex.Message, true);
                    return;
                }
                catch (Exception)
                {
                    ReportStatus("The user account could not be updated. Please try again.", true);
                    return;
                }

                AccountsLoadAccounts();
                AccountsClearForm();
                ReportStatus("User account \"" + username + "\" updated" + (reset ? " and its password reset." : "."), false);
            }
            finally
            {
                AccountsClearPasswords();
            }
        }

        // Deactivate the selected account after confirmation
        private void btnAccountsDeactivate_Click(object sender, EventArgs e)
        {
            AccountsClearPasswords();
            if (!accountsSelectedUserId.HasValue)
            {
                ReportStatus("Select a user account in the grid before you deactivate it.", true);
                return;
            }

            if (accountsSelectedUserId.Value == UserId)
            {
                ReportStatus("You cannot deactivate your own account. Ask another administrator.", true);
                return;
            }

            if (!accountsSelectedIsActive)
            {
                ReportStatus("This account is already inactive. Tick Active and update it to let the user sign in again.", true);
                return;
            }

            string username = txtUsername.Text.Trim();
            if (!Confirm("Deactivate user account",
                "Deactivate the account \"" + username + "\" (" + txtFullName.Text.Trim() + ")? They can no longer sign in. " +
                "The account and the reports it generated stay on record — nothing is deleted.", "Deactivate"))
            {
                return;
            }

            try
            {
                AccountsDeactivateAccount(accountsSelectedUserId.Value);
            }
            catch (ArgumentException ex)
            {
                ReportStatus(ex.Message, true);
                return;
            }
            catch (Exception)
            {
                ReportStatus("The user account could not be deactivated. Please try again.", true);
                return;
            }

            AccountsLoadAccounts();
            AccountsClearForm();
            ReportStatus("User account \"" + username + "\" deactivated.", false);
        }

        // Cancel editing and clear the account form
        private void btnAccountsCancel_Click(object sender, EventArgs e)
        {
            AccountsClearForm();
            ReportStatus("Edit cancelled. Nothing was saved.", false);
        }

        #endregion

        #region Business Logic Layer hooks

        // Load the available user account roles
        private void AccountsLoadRoles()
        {
            try
            {
                // Retrieve Administrator and Career Advisor roles
                UserAccountController userAccountController = new UserAccountController();
                cboRole.DataSource = userAccountController.GetRoles();
                cboRole.SelectedIndex = -1;
            }
            catch (Exception)
            {
                ReportStatus("The role list could not be loaded. Please try again, or sign out and back in.", true);
            }
        }

        // Load active and inactive accounts without password hashes
        private void AccountsLoadAccounts()
        {
            accountsLoading = true;
            try
            {
                // Retrieve account details for the grid
                UserAccountController userAccountController = new UserAccountController();
                dgvAccounts.DataSource = userAccountController.GetAccounts(UserId);
                dgvAccounts.ClearSelection();
            }
            catch (Exception)
            {
                ReportStatus("The user accounts could not be loaded. Please try again, or sign out and back in.", true);
            }
            finally
            {
                accountsLoading = false;
            }

            AccountsUpdateCountLine();
        }

        // Add or update an account through the controller
        private void AccountsSaveAccount(int userId, string newPassword)
        {
            UserAccountController userAccountController = new UserAccountController();
            userAccountController.SaveAccount(UserId, userId, txtUsername.Text.Trim(), txtFullName.Text.Trim(),
                (string)cboRole.SelectedItem, chkAccountActive.Checked, newPassword);
        }

        // Deactivate a user account without deleting it
        private void AccountsDeactivateAccount(int userId)
        {
            UserAccountController userAccountController = new UserAccountController();
            userAccountController.DeactivateAccount(UserId, userId);
        }

        #endregion

        #region Grid display and selection

        // Update the total and active account counts
        private void AccountsUpdateCountLine()
        {
            int total = dgvAccounts.Rows.Count;
            int active = 0;
            foreach (DataGridViewRow row in dgvAccounts.Rows)
            {
                if (AccountsIsTrue(row.Cells[colAccountsStatus.Index].Value))
                {
                    active++;
                }
            }

            lblAccountCount.Text = total + (total == 1 ? " account" : " accounts") + " · " + active + " active";
        }

        // Format account status as active or inactive
        private void dgvAccounts_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == colAccountsStatus.Index)
            {
                FormatStatusChip(e, AccountsIsTrue(e.Value));
            }
        }

        // Clear automatic selection after the account grid is loaded
        private void dgvAccounts_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            bool wasLoading = accountsLoading;
            accountsLoading = true;
            dgvAccounts.ClearSelection();
            dgvAccounts.CurrentCell = null;
            accountsLoading = wasLoading;
            AccountsUpdateCountLine();
        }

        // Load the selected account into the form for editing
        private void dgvAccounts_SelectionChanged(object sender, EventArgs e)
        {
            if (accountsLoading || !dgvAccounts.Focused || dgvAccounts.SelectedRows.Count == 0)
            {
                return;
            }

            DataGridViewRow row = dgvAccounts.SelectedRows[0];
            object id = row.Cells[colAccountsUserId.Index].Value;
            if (id == null || id == DBNull.Value)
            {
                return;
            }

            AccountsClearValidation();
            AccountsClearPasswords();
            accountsSelectedUserId = Convert.ToInt32(id, CultureInfo.InvariantCulture);
            accountsSelectedIsActive = AccountsIsTrue(row.Cells[colAccountsStatus.Index].Value);
            txtUsername.Text = Convert.ToString(row.Cells[colAccountsUsername.Index].Value, CultureInfo.CurrentCulture);
            txtFullName.Text = Convert.ToString(row.Cells[colAccountsFullName.Index].Value, CultureInfo.CurrentCulture);
            cboRole.SelectedIndex = cboRole.FindStringExact(Convert.ToString(row.Cells[colAccountsRole.Index].Value, CultureInfo.CurrentCulture));
            chkAccountActive.Checked = accountsSelectedIsActive;

            if (accountsSelectedUserId.Value == UserId)
            {
                ReportStatus("This is your own account. You can change it, but you cannot deactivate it.", false);
            }
        }

        #endregion

        #region Maintenance form validation (validated again in the BLL on save)

        // Validate the account form and show any errors
        private bool AccountsValidateForm(bool isNew)
        {
            var problems = new List<string>();
            var invalid = new List<Control>();
            if (!AccountsCheckUsername()) { problems.Add("Username"); invalid.Add(txtUsername); }
            if (!AccountsCheckFullName()) { problems.Add("Full name"); invalid.Add(txtFullName); }
            if (!AccountsCheckRole()) { problems.Add("Role"); invalid.Add(cboRole); }
            if (!AccountsCheckNewPassword(isNew)) { problems.Add("New password"); invalid.Add(txtNewPassword); }
            if (!AccountsCheckConfirmPassword(isNew)) { problems.Add("Confirm password"); invalid.Add(txtConfirmPassword); }

            // Prevent administrators from deactivating their own accounts
            bool ownAccountOff = !isNew && accountsSelectedUserId.HasValue && accountsSelectedUserId.Value == UserId && !chkAccountActive.Checked;
            chkAccountActive.ForeColor = ownAccountOff ? RedColour : InkColour;
            if (ownAccountOff) { problems.Add("Active"); invalid.Add(chkAccountActive); }

            if (problems.Count == 0)
            {
                pnlAccountSummary.Visible = false;
                return true;
            }

            ShowFormSummary(pnlAccountSummary, lblAccountSummary, problems, invalid[0], "the account");
            return false;
        }

        // Check that the username has a valid format and length
        private bool AccountsCheckUsername()
        {
            string text = txtUsername.Text.Trim();
            bool valid = text.Length > 0 && text.Length <= 50 && Regex.IsMatch(text, @"^[A-Za-z0-9._\-]+$");
            return CheckField(valid, txtUsername, lblUsername, lblUsernameError,
                "Username: letters, digits, . _ - only.");
        }

        // Check that the full name is entered and within 100 characters
        private bool AccountsCheckFullName()
        {
            string text = txtFullName.Text.Trim();
            return CheckField(text.Length > 0 && text.Length <= 100, txtFullName, lblFullName, lblFullNameError,
                "Full name: enter the user's name (max 100 characters).");
        }

        // Check that a user role has been selected
        private bool AccountsCheckRole()
        {
            return CheckField(cboRole.SelectedIndex >= 0, cboRole, lblRole, lblRoleError, "Role: choose a role.");
        }

        // Check password length when adding or resetting an account
        private bool AccountsCheckNewPassword(bool isNew)
        {
            bool resetting = isNew || txtNewPassword.Text.Length > 0 || txtConfirmPassword.Text.Length > 0;
            bool valid = !resetting || txtNewPassword.Text.Length >= AccountsMinimumPasswordLength;
            return CheckField(valid, txtNewPassword, lblNewPassword, lblNewPasswordError,
                "New password: at least " + AccountsMinimumPasswordLength + " characters.");
        }

        // Check that the password confirmation matches
        private bool AccountsCheckConfirmPassword(bool isNew)
        {
            bool resetting = isNew || txtNewPassword.Text.Length > 0 || txtConfirmPassword.Text.Length > 0;
            bool valid = !resetting || (txtConfirmPassword.Text.Length > 0 && txtConfirmPassword.Text == txtNewPassword.Text);
            return CheckField(valid, txtConfirmPassword, lblConfirmPassword, lblConfirmPasswordError,
                "Confirm password: must match the new password.");
        }

        // Validate the username when leaving the field
        private void txtUsername_Leave(object sender, EventArgs e) { AccountsCheckUsername(); }

        // Validate the full name when leaving the field
        private void txtFullName_Leave(object sender, EventArgs e) { AccountsCheckFullName(); }

        // Validate the role when leaving the dropdown
        private void cboRole_Leave(object sender, EventArgs e) { AccountsCheckRole(); }

        // Validate the new password when leaving the field
        private void txtNewPassword_Leave(object sender, EventArgs e)
        {
            if (txtNewPassword.Text.Length > 0) AccountsCheckNewPassword(!accountsSelectedUserId.HasValue);
        }

        // Validate the confirmation when leaving the field
        private void txtConfirmPassword_Leave(object sender, EventArgs e)
        {
            if (txtConfirmPassword.Text.Length > 0) AccountsCheckConfirmPassword(!accountsSelectedUserId.HasValue);
        }

        // Clear validation messages and restore field colours
        private void AccountsClearValidation()
        {
            CheckField(true, txtUsername, lblUsername, lblUsernameError, string.Empty);
            CheckField(true, txtFullName, lblFullName, lblFullNameError, string.Empty);
            CheckField(true, cboRole, lblRole, lblRoleError, string.Empty);
            CheckField(true, txtNewPassword, lblNewPassword, lblNewPasswordError, string.Empty);
            CheckField(true, txtConfirmPassword, lblConfirmPassword, lblConfirmPasswordError, string.Empty);
            chkAccountActive.ForeColor = InkColour;
            pnlAccountSummary.Visible = false;
        }

        #endregion

        #region Helpers

        // Clear both password fields after an account action
        private void AccountsClearPasswords()
        {
            txtNewPassword.Clear();
            txtConfirmPassword.Clear();
        }

        // Reset the form for a new user account
        private void AccountsClearForm()
        {
            accountsLoading = true;
            dgvAccounts.ClearSelection();
            accountsLoading = false;

            accountsSelectedUserId = null;
            accountsSelectedIsActive = false;
            txtUsername.Clear();
            txtFullName.Clear();
            cboRole.SelectedIndex = -1;
            chkAccountActive.Checked = true;
            AccountsClearPasswords();
            AccountsClearValidation();
        }

        // Check whether an account's stored active flag is true
        private static bool AccountsIsTrue(object value)
        {
            return value is bool && (bool)value;
        }

        #endregion
    }
}
