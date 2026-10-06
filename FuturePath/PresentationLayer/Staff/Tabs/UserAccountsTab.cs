using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 13.6 — Reference data, User accounts tab (Administrators only; decisions.md D5; hosted by
    /// <c>ReferenceDataForm</c>). Built to the UI 13.2 maintenance pattern: a read-only grid of
    /// <c>UserAccount</c> rows (Username · Full name · Role · Status chip) with a count line, and a
    /// maintenance form below it (Username*, Full name*, Role*, Active, and a password group).
    /// Password hashes are never loaded into or shown on this screen. A password is required when an
    /// account is added and optional on update (entering one resets it); the BLL stores only a salted
    /// PBKDF2 hash. Both password boxes are cleared after every save attempt. An Administrator cannot
    /// deactivate their own account. Nothing is ever deleted.
    /// </summary>
    public partial class UserAccountsTab : UserControl
    {
        // Colours used at runtime (spec 2.2.6). Everything else is set in the Designer.
        private static readonly Color InkColour = Color.FromArgb(27, 31, 36);
        private static readonly Color MutedColour = Color.FromArgb(88, 96, 108);
        private static readonly Color RedColour = Color.FromArgb(164, 38, 44);
        private static readonly Color RedTint = Color.FromArgb(250, 234, 235);
        private static readonly Color GreenColour = Color.FromArgb(31, 107, 58);
        private static readonly Color GreenTint = Color.FromArgb(230, 242, 234);
        private static readonly Color GreyTint = Color.FromArgb(238, 240, 243);
        private static readonly Font ChipFont = new Font("Segoe UI Semibold", 9F);

        /// <summary>Minimum password length checked in the form (the BLL checks it again).</summary>
        private const int MinimumPasswordLength = 8;

        private const string OwnAccountMessage = "Active: you cannot deactivate your own account. Ask another administrator.";

        /// <summary>UserID of the account being edited; null while adding a new account.</summary>
        private int? selectedUserId;

        /// <summary>IsActive of the account being edited, so Deactivate can say it is already inactive.</summary>
        private bool selectedIsActive;

        /// <summary>True while the grid is being rebound, so selection events are ignored.</summary>
        private bool loading;

        /// <summary>Creates the tab. The host sets <see cref="UserId"/> and <see cref="Role"/>, then calls <see cref="LoadData"/>.</summary>
        public UserAccountsTab()
        {
            InitializeComponent();
            grdAccounts.AutoGenerateColumns = false;
            UpdateCountLine();
        }

        #region Tab contract (ReferenceDataForm)

        /// <summary>UserAccount.UserID of the signed-in Administrator; used for the own-account check and every BLL save.</summary>
        public int UserId { get; set; }

        /// <summary>UserAccount.Role of the signed-in user ("Administrator").</summary>
        public string Role { get; set; }

        /// <summary>Noun used by the host's action band: "Add user account", "Update user account".</summary>
        public string ItemNoun
        {
            get { return "user account"; }
        }

        /// <summary>Raised with a status-band message; isWarning = true shows amber, otherwise blue.</summary>
        public event Action<string, bool> StatusChanged;

        /// <summary>Loads the role list and the accounts. Called by the host when the tab is first shown.</summary>
        public void LoadData()
        {
            LoadRoles();
            LoadAccounts();
            ClearForm();
        }

        /// <summary>Adds a new account. A password is required; the boxes are cleared afterwards whatever happens.</summary>
        public void AddNew()
        {
            try
            {
                if (!ValidateForm(true))
                {
                    return;
                }

                string username = txtUsername.Text.Trim();
                try
                {
                    SaveAccount(null, txtNewPassword.Text);
                }
                catch (Exception)
                {
                    RaiseStatus("The user account could not be added. The username may already be in use. Please try again.", true);
                    return;
                }

                LoadAccounts();
                ClearForm();
                RaiseStatus("User account \"" + username + "\" added.", false);
            }
            finally
            {
                ClearPasswords();
            }
        }

        /// <summary>
        /// Saves the form over the selected account. Leaving both password boxes blank keeps the current
        /// password; entering one resets it. The boxes are cleared afterwards whatever happens.
        /// </summary>
        public void UpdateSelected()
        {
            try
            {
                if (!selectedUserId.HasValue)
                {
                    RaiseStatus("Select a user account in the grid before you update it.", true);
                    return;
                }

                if (!ValidateForm(false))
                {
                    return;
                }

                string username = txtUsername.Text.Trim();
                bool reset = txtNewPassword.Text.Length > 0;
                try
                {
                    SaveAccount(selectedUserId, reset ? txtNewPassword.Text : null);
                }
                catch (Exception)
                {
                    RaiseStatus("The user account could not be updated. The username may already be in use. Please try again.", true);
                    return;
                }

                LoadAccounts();
                ClearForm();
                RaiseStatus("User account \"" + username + "\" updated" + (reset ? " and its password reset." : "."), false);
            }
            finally
            {
                ClearPasswords();
            }
        }

        /// <summary>Deactivates the selected account after confirmation (never deletes it, never your own).</summary>
        public void DeactivateSelected()
        {
            ClearPasswords();
            if (!selectedUserId.HasValue)
            {
                RaiseStatus("Select a user account in the grid before you deactivate it.", true);
                return;
            }

            if (selectedUserId.Value == UserId)
            {
                RaiseStatus("You cannot deactivate your own account. Ask another administrator.", true);
                return;
            }

            if (!selectedIsActive)
            {
                RaiseStatus("This account is already inactive. Tick Active and update it to let the user sign in again.", true);
                return;
            }

            string username = txtUsername.Text.Trim();
            using (var dialog = new ConfirmationDialog("Deactivate user account",
                "Deactivate the account \"" + username + "\" (" + txtFullName.Text.Trim() + ")? They can no longer sign in. " +
                "The account and the reports it generated stay on record — nothing is deleted.", "Deactivate"))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }
            }

            try
            {
                DeactivateAccount(selectedUserId.Value);
            }
            catch (Exception)
            {
                RaiseStatus("The user account could not be deactivated. Please try again.", true);
                return;
            }

            LoadAccounts();
            ClearForm();
            RaiseStatus("User account \"" + username + "\" deactivated.", false);
        }

        /// <summary>Discards the edit, clears the form, both password boxes and the grid selection.</summary>
        public void CancelEdit()
        {
            ClearForm();
            RaiseStatus("Edit cancelled. Nothing was saved.", false);
        }

        #endregion

        #region Business Logic Layer hooks

        /// <summary>Binds the role list supplied by the BLL (never literal Items).</summary>
        private void LoadRoles()
        {
            try
            {
                // TODO BLL: cboRole.DataSource = UserAccountController.GetRoles();   // "Administrator", "Career Advisor"
                cboRole.SelectedIndex = -1;
            }
            catch (Exception)
            {
                RaiseStatus("The role list could not be loaded. Please close the tab and try again.", true);
            }
        }

        /// <summary>Binds the account grid (active and inactive). PasswordHash is never part of the list.</summary>
        private void LoadAccounts()
        {
            loading = true;
            try
            {
                // TODO BLL: grdAccounts.DataSource = UserAccountController.GetAccounts(UserId);
                //           Columns: UserID, Username, FullName, Role, IsActive — the BLL never returns PasswordHash.
                //           The BLL checks that UserId's Role = Administrator before returning the list.
                grdAccounts.ClearSelection();
            }
            catch (Exception)
            {
                RaiseStatus("The user accounts could not be loaded. Please close the tab and try again.", true);
            }
            finally
            {
                loading = false;
            }

            UpdateCountLine();
        }

        /// <summary>
        /// Adds (userId null) or updates an account. <paramref name="newPassword"/> is null on an update
        /// that keeps the current password. The password is passed straight to the BLL and never kept here.
        /// </summary>
        private void SaveAccount(int? userId, string newPassword)
        {
            // TODO BLL: UserAccountController.SaveAccount(UserId, userId, txtUsername.Text.Trim(), txtFullName.Text.Trim(),
            //               (string)cboRole.SelectedValue, chkAccountActive.Checked, newPassword);
            //           The BLL checks that UserId's Role = Administrator before running; re-checks every field and
            //           that Username is unique; refuses to deactivate the caller's own account (userId == UserId).
            //           When newPassword is not null it stores a salted hash only — PBKDF2 via Rfc2898DeriveBytes
            //           with a random per-user salt into UserAccount.PasswordHash — never the plaintext.
        }

        /// <summary>Sets IsActive = 0 on an account so it can no longer sign in. Nothing is deleted.</summary>
        private void DeactivateAccount(int userId)
        {
            // TODO BLL: UserAccountController.DeactivateAccount(UserId, userId);
            //           The BLL checks that UserId's Role = Administrator before running and refuses userId == UserId.
        }

        #endregion

        #region Grid display and selection

        /// <summary>Writes "N accounts · M active" from the bound rows.</summary>
        private void UpdateCountLine()
        {
            int total = grdAccounts.Rows.Count;
            int active = 0;
            foreach (DataGridViewRow row in grdAccounts.Rows)
            {
                if (IsTrue(row.Cells[colAccountStatus.Index].Value))
                {
                    active++;
                }
            }

            lblAccountCount.Text = total + (total == 1 ? " account" : " accounts") + " · " + active + " active";
        }

        /// <summary>Shows IsActive as a status chip: words plus colour, green Active / grey Inactive.</summary>
        private void grdAccounts_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != colAccountStatus.Index)
            {
                return;
            }

            bool active = IsTrue(e.Value);
            e.Value = active ? "✓ Active" : "Inactive";
            e.CellStyle.ForeColor = active ? GreenColour : MutedColour;
            e.CellStyle.BackColor = active ? GreenTint : GreyTint;
            e.CellStyle.Font = ChipFont;
            e.FormattingApplied = true;
        }

        /// <summary>
        /// After (re)binding, starts with no row selected, so the form stays ready for a new
        /// account until the administrator picks one, and refreshes the count line.
        /// </summary>
        private void grdAccounts_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            bool wasLoading = loading;
            loading = true;
            grdAccounts.ClearSelection();
            grdAccounts.CurrentCell = null;
            loading = wasLoading;
            UpdateCountLine();
        }

        /// <summary>
        /// Copies the account the administrator selects into the form; password boxes always start empty.
        /// Only a selection made in the focused grid counts, not the automatic first-row selection on binding.
        /// </summary>
        private void grdAccounts_SelectionChanged(object sender, EventArgs e)
        {
            if (loading || !grdAccounts.Focused || grdAccounts.SelectedRows.Count == 0)
            {
                return;
            }

            DataGridViewRow row = grdAccounts.SelectedRows[0];
            object id = row.Cells[colUserId.Index].Value;
            if (id == null || id == DBNull.Value)
            {
                return;
            }

            ClearValidation();
            ClearPasswords();
            selectedUserId = Convert.ToInt32(id, CultureInfo.InvariantCulture);
            selectedIsActive = IsTrue(row.Cells[colAccountStatus.Index].Value);
            txtUsername.Text = Convert.ToString(row.Cells[colUsername.Index].Value, CultureInfo.CurrentCulture);
            txtFullName.Text = Convert.ToString(row.Cells[colFullName.Index].Value, CultureInfo.CurrentCulture);
            cboRole.SelectedIndex = cboRole.FindStringExact(Convert.ToString(row.Cells[colRole.Index].Value, CultureInfo.CurrentCulture));
            chkAccountActive.Checked = selectedIsActive;

            if (selectedUserId.Value == UserId)
            {
                RaiseStatus("This is your own account. You can change it, but you cannot deactivate it.", false);
            }
        }

        #endregion

        #region Maintenance form validation (validated again in the BLL on save)

        /// <summary>
        /// Validates the whole form; shows field messages and the validation summary.
        /// </summary>
        /// <param name="isNew">True on Add: the password is then required.</param>
        private bool ValidateForm(bool isNew)
        {
            var problems = new List<KeyValuePair<Control, string>>();
            if (!CheckUsername()) problems.Add(Problem(txtUsername, lblUsernameError));
            if (!CheckFullName()) problems.Add(Problem(txtFullName, lblFullNameError));
            if (!CheckRole()) problems.Add(Problem(cboRole, lblRoleError));
            if (!CheckNewPassword(isNew)) problems.Add(Problem(txtNewPassword, lblNewPasswordError));
            if (!CheckConfirmPassword(isNew)) problems.Add(Problem(txtConfirmPassword, lblConfirmPasswordError));

            bool ownAccountOff = !isNew && selectedUserId.HasValue && selectedUserId.Value == UserId && !chkAccountActive.Checked;
            chkAccountActive.ForeColor = ownAccountOff ? RedColour : InkColour;
            if (ownAccountOff)
            {
                problems.Add(new KeyValuePair<Control, string>(chkAccountActive, OwnAccountMessage));
            }

            if (problems.Count == 0)
            {
                pnlAccountSummary.Visible = false;
                return true;
            }

            // The summary names each field; the full correction is in the field message under it.
            var lines = new List<string>();
            foreach (KeyValuePair<Control, string> problem in problems)
            {
                int colon = problem.Value.IndexOf(':');
                lines.Add("• " + (colon > 0 ? problem.Value.Substring(0, colon) : problem.Value));
            }

            lblAccountSummary.Text = "⚠ " + problems.Count + (problems.Count == 1 ? " entry" : " entries") + " to correct (see the message under each):" +
                Environment.NewLine + string.Join(Environment.NewLine, lines);
            pnlAccountSummary.Visible = true;
            RaiseStatus(problems.Count + (problems.Count == 1 ? " entry" : " entries") +
                " to correct before the account can be saved. Re-enter any password.", true);
            problems[0].Key.Focus();
            return false;
        }

        private static KeyValuePair<Control, string> Problem(Control input, Label messageLabel)
        {
            return new KeyValuePair<Control, string>(input, messageLabel.Text.TrimStart('⚠', ' '));
        }

        private bool CheckUsername()
        {
            string text = txtUsername.Text.Trim();
            bool valid = text.Length > 0 && text.Length <= 50 && Regex.IsMatch(text, @"^[A-Za-z0-9._\-]+$");
            return CheckField(valid, txtUsername, lblUsername, lblUsernameError,
                "Username: letters, digits, . _ - only.");
        }

        private bool CheckFullName()
        {
            string text = txtFullName.Text.Trim();
            return CheckField(text.Length > 0 && text.Length <= 100, txtFullName, lblFullName, lblFullNameError,
                "Full name: enter the user's name (max 100 characters).");
        }

        private bool CheckRole()
        {
            return CheckField(cboRole.SelectedIndex >= 0, cboRole, lblRole, lblRoleError, "Role: choose a role.");
        }

        /// <summary>Required on Add; on Update only checked when something was typed (a reset).</summary>
        private bool CheckNewPassword(bool isNew)
        {
            bool resetting = isNew || txtNewPassword.Text.Length > 0 || txtConfirmPassword.Text.Length > 0;
            bool valid = !resetting || txtNewPassword.Text.Length >= MinimumPasswordLength;
            return CheckField(valid, txtNewPassword, lblNewPassword, lblNewPasswordError,
                "New password: at least " + MinimumPasswordLength + " characters.");
        }

        /// <summary>The confirmation must match the new password whenever a password is being set.</summary>
        private bool CheckConfirmPassword(bool isNew)
        {
            bool resetting = isNew || txtNewPassword.Text.Length > 0 || txtConfirmPassword.Text.Length > 0;
            bool valid = !resetting || (txtConfirmPassword.Text.Length > 0 && txtConfirmPassword.Text == txtNewPassword.Text);
            return CheckField(valid, txtConfirmPassword, lblConfirmPassword, lblConfirmPasswordError,
                "Confirm password: must match the new password.");
        }

        private void txtUsername_Leave(object sender, EventArgs e) { CheckUsername(); }

        private void txtFullName_Leave(object sender, EventArgs e) { CheckFullName(); }

        private void cboRole_Leave(object sender, EventArgs e) { CheckRole(); }

        private void txtNewPassword_Leave(object sender, EventArgs e)
        {
            if (txtNewPassword.Text.Length > 0) CheckNewPassword(!selectedUserId.HasValue);
        }

        private void txtConfirmPassword_Leave(object sender, EventArgs e)
        {
            if (txtConfirmPassword.Text.Length > 0) CheckConfirmPassword(!selectedUserId.HasValue);
        }

        /// <summary>
        /// Shows or clears a field message (spec 2.2.7): red text with a warning glyph under the
        /// control, a red tint on the control and a red label.
        /// </summary>
        private static bool CheckField(bool valid, Control input, Label fieldLabel, Label messageLabel, string message)
        {
            input.BackColor = valid ? SystemColors.Window : RedTint;
            fieldLabel.ForeColor = valid ? InkColour : RedColour;
            messageLabel.Text = valid ? string.Empty : "⚠ " + message;
            messageLabel.Visible = !valid;
            input.AccessibleDescription = valid ? null : message;
            return valid;
        }

        private void ClearValidation()
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
        private void ClearPasswords()
        {
            txtNewPassword.Clear();
            txtConfirmPassword.Clear();
        }

        /// <summary>Empties the form and the selection, ready to add a new account.</summary>
        private void ClearForm()
        {
            loading = true;
            grdAccounts.ClearSelection();
            loading = false;

            selectedUserId = null;
            selectedIsActive = false;
            txtUsername.Clear();
            txtFullName.Clear();
            cboRole.SelectedIndex = -1;
            chkAccountActive.Checked = true;
            ClearPasswords();
            ClearValidation();
        }

        /// <summary>True for a bound BIT value of 1.</summary>
        private static bool IsTrue(object value)
        {
            return value is bool && (bool)value;
        }

        private void RaiseStatus(string message, bool isWarning)
        {
            Action<string, bool> handler = StatusChanged;
            if (handler != null)
            {
                handler(message, isWarning);
            }
        }

        #endregion
    }
}
