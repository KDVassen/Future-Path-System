using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FuturePath.ApplicationLayer;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 13.6 — Reference data, User accounts (Administrators only; decisions.md D5; page
    /// <c>tabAccounts</c> in the Reference data group). Built to the UI 13.2 maintenance pattern: a
    /// read-only grid of <c>UserAccount</c> rows (Username · Full name · Role · Status chip) with a count
    /// line, and a maintenance form below it (Username*, Full name*, Role*, Active, and a password group).
    /// Password hashes are never loaded into or shown on this screen. A password is required when an
    /// account is added and optional on update (entering one resets it); the BLL stores only a salted
    /// SHA-256 hash (decisions.md D22). Both password boxes are cleared after every save attempt, on Deactivate, Cancel and
    /// selection. An Administrator can never deactivate their own account (compared with the console's
    /// <see cref="UserId"/>). Nothing is ever deleted.
    /// </summary>
    public partial class StaffConsoleForm
    {
        /// <summary>Minimum password length checked in the form (the BLL checks it again).</summary>
        private const int AccountsMinimumPasswordLength = 8;

        /// <summary>UserID of the account being edited; null while adding a new account.</summary>
        private int? accountsSelectedUserId;

        /// <summary>IsActive of the account being edited, so Deactivate can say it is already inactive.</summary>
        private bool accountsSelectedIsActive;

        /// <summary>True while the grid is being rebound, so selection events are ignored.</summary>
        private bool accountsLoading;

        #region Page shown

        /// <summary>
        /// Called by <see cref="PageShown"/> each time UI 13.6 appears. The first time, it prepares the grid
        /// and loads the role list and the accounts. Every time, both password boxes start empty, so a
        /// password typed before moving to another page never waits on screen.
        /// </summary>
        /// <param name="firstTime">True the first time the page is shown in this console.</param>
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

        /// <summary>Add user account: a password is required; the boxes are cleared afterwards whatever happens.</summary>
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
                    // The BLL's own check failed, e.g. "The username … is already taken."
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

        /// <summary>
        /// Update user account: saves the form over the selected account. Leaving both password boxes blank
        /// keeps the current password; entering one resets it. The boxes are cleared afterwards whatever happens.
        /// </summary>
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
                    // A blank password keeps the current one.
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

        /// <summary>Deactivate: deactivates the selected account after confirmation (never deletes it, never your own).</summary>
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

        /// <summary>Cancel: discards the edit, clears the form, both password boxes and the grid selection.</summary>
        private void btnAccountsCancel_Click(object sender, EventArgs e)
        {
            AccountsClearForm();
            ReportStatus("Edit cancelled. Nothing was saved.", false);
        }

        #endregion

        #region Business Logic Layer hooks

        /// <summary>Binds the role list supplied by the BLL (never literal Items).</summary>
        private void AccountsLoadRoles()
        {
            try
            {
                // "Administrator", "Career Advisor" (the UserAccount role constants).
                UserAccountController userAccountController = new UserAccountController();
                cboRole.DataSource = userAccountController.GetRoles();
                cboRole.SelectedIndex = -1;
            }
            catch (Exception)
            {
                ReportStatus("The role list could not be loaded. Please try again, or sign out and back in.", true);
            }
        }

        /// <summary>Binds the account grid (active and inactive). PasswordHash is never part of the list.</summary>
        private void AccountsLoadAccounts()
        {
            accountsLoading = true;
            try
            {
                // Columns: UserID, Username, FullName, Role, IsActive — the BLL never returns PasswordHash.
                // The BLL checks that UserId's Role = Administrator before returning the list.
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

        /// <summary>
        /// Adds (userId 0) or updates an account. A blank <paramref name="newPassword"/> on an update keeps the
        /// current password. The password is passed straight to the BLL and never kept here.
        /// The BLL checks that UserId's Role = Administrator, re-checks every field and that Username is unique,
        /// and refuses to deactivate or demote the caller's own account. It stores only a salted hash
        /// (a random salt plus SHA-256, decisions.md D22), never the plaintext.
        /// </summary>
        /// <param name="userId">The account being updated, or 0 to add a new one.</param>
        /// <param name="newPassword">The new password, or "" to keep the current one.</param>
        private void AccountsSaveAccount(int userId, string newPassword)
        {
            UserAccountController userAccountController = new UserAccountController();
            userAccountController.SaveAccount(UserId, userId, txtUsername.Text.Trim(), txtFullName.Text.Trim(),
                (string)cboRole.SelectedItem, chkAccountActive.Checked, newPassword);
        }

        /// <summary>Sets IsActive = 0 on an account so it can no longer sign in. Nothing is deleted.</summary>
        /// <param name="userId">The account to deactivate (the BLL refuses the signed-in Administrator's own).</param>
        private void AccountsDeactivateAccount(int userId)
        {
            UserAccountController userAccountController = new UserAccountController();
            userAccountController.DeactivateAccount(UserId, userId);
        }

        #endregion

        #region Grid display and selection

        /// <summary>Writes "N accounts · M active" from the bound rows.</summary>
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

        /// <summary>Shows IsActive as a status chip (display formatting only).</summary>
        private void dgvAccounts_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == colAccountsStatus.Index)
            {
                FormatStatusChip(e, AccountsIsTrue(e.Value));
            }
        }

        /// <summary>
        /// After (re)binding, starts with no row selected, so the form stays ready for a new
        /// account until the administrator picks one, and refreshes the count line.
        /// </summary>
        private void dgvAccounts_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            bool wasLoading = accountsLoading;
            accountsLoading = true;
            dgvAccounts.ClearSelection();
            dgvAccounts.CurrentCell = null;
            accountsLoading = wasLoading;
            AccountsUpdateCountLine();
        }

        /// <summary>
        /// Copies the account the administrator selects into the form; password boxes always start empty.
        /// Only a selection made in the focused grid counts, not the automatic first-row selection on binding.
        /// </summary>
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

        /// <summary>
        /// Validates the whole form; shows field messages and the validation summary.
        /// </summary>
        /// <param name="isNew">True on Add: the password is then required.</param>
        private bool AccountsValidateForm(bool isNew)
        {
            var problems = new List<string>();
            var invalid = new List<Control>();
            if (!AccountsCheckUsername()) { problems.Add("Username"); invalid.Add(txtUsername); }
            if (!AccountsCheckFullName()) { problems.Add("Full name"); invalid.Add(txtFullName); }
            if (!AccountsCheckRole()) { problems.Add("Role"); invalid.Add(cboRole); }
            if (!AccountsCheckNewPassword(isNew)) { problems.Add("New password"); invalid.Add(txtNewPassword); }
            if (!AccountsCheckConfirmPassword(isNew)) { problems.Add("Confirm password"); invalid.Add(txtConfirmPassword); }

            // An Administrator may not switch off their own account: Active turns red and is listed in the summary.
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

        /// <summary>Username: required, at most 50 characters of letters, digits, . _ and -.</summary>
        private bool AccountsCheckUsername()
        {
            string text = txtUsername.Text.Trim();
            bool valid = text.Length > 0 && text.Length <= 50 && Regex.IsMatch(text, @"^[A-Za-z0-9._\-]+$");
            return CheckField(valid, txtUsername, lblUsername, lblUsernameError,
                "Username: letters, digits, . _ - only.");
        }

        /// <summary>Full name: required, at most 100 characters.</summary>
        private bool AccountsCheckFullName()
        {
            string text = txtFullName.Text.Trim();
            return CheckField(text.Length > 0 && text.Length <= 100, txtFullName, lblFullName, lblFullNameError,
                "Full name: enter the user's name (max 100 characters).");
        }

        /// <summary>Role: one must be chosen.</summary>
        private bool AccountsCheckRole()
        {
            return CheckField(cboRole.SelectedIndex >= 0, cboRole, lblRole, lblRoleError, "Role: choose a role.");
        }

        /// <summary>Required on Add; on Update only checked when something was typed (a reset).</summary>
        private bool AccountsCheckNewPassword(bool isNew)
        {
            bool resetting = isNew || txtNewPassword.Text.Length > 0 || txtConfirmPassword.Text.Length > 0;
            bool valid = !resetting || txtNewPassword.Text.Length >= AccountsMinimumPasswordLength;
            return CheckField(valid, txtNewPassword, lblNewPassword, lblNewPasswordError,
                "New password: at least " + AccountsMinimumPasswordLength + " characters.");
        }

        /// <summary>The confirmation must match the new password whenever a password is being set.</summary>
        private bool AccountsCheckConfirmPassword(bool isNew)
        {
            bool resetting = isNew || txtNewPassword.Text.Length > 0 || txtConfirmPassword.Text.Length > 0;
            bool valid = !resetting || (txtConfirmPassword.Text.Length > 0 && txtConfirmPassword.Text == txtNewPassword.Text);
            return CheckField(valid, txtConfirmPassword, lblConfirmPassword, lblConfirmPasswordError,
                "Confirm password: must match the new password.");
        }

        /// <summary>Checks the username as the administrator leaves the box.</summary>
        private void txtUsername_Leave(object sender, EventArgs e) { AccountsCheckUsername(); }

        /// <summary>Checks the full name as the administrator leaves the box.</summary>
        private void txtFullName_Leave(object sender, EventArgs e) { AccountsCheckFullName(); }

        /// <summary>Checks the role as the administrator leaves the list.</summary>
        private void cboRole_Leave(object sender, EventArgs e) { AccountsCheckRole(); }

        /// <summary>Checks the new password on leaving the box, only when something was typed.</summary>
        private void txtNewPassword_Leave(object sender, EventArgs e)
        {
            if (txtNewPassword.Text.Length > 0) AccountsCheckNewPassword(!accountsSelectedUserId.HasValue);
        }

        /// <summary>Checks the confirmation on leaving the box, only when something was typed.</summary>
        private void txtConfirmPassword_Leave(object sender, EventArgs e)
        {
            if (txtConfirmPassword.Text.Length > 0) AccountsCheckConfirmPassword(!accountsSelectedUserId.HasValue);
        }

        /// <summary>Clears every field message, the own-account mark and the validation summary.</summary>
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

        /// <summary>Empties both password boxes so a password never stays on screen after an attempt.</summary>
        private void AccountsClearPasswords()
        {
            txtNewPassword.Clear();
            txtConfirmPassword.Clear();
        }

        /// <summary>Empties the form and the selection, ready to add a new account.</summary>
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

        /// <summary>True for a bound BIT value of 1.</summary>
        private static bool AccountsIsTrue(object value)
        {
            return value is bool && (bool)value;
        }

        #endregion
    }
}
