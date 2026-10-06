using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 8 — Staff Login, with UI 8.1a Invalid Login (spec 2.3.1). The single gate into the staff
    /// area: only Career Advisors and System Administrators hold accounts, learners never log in.
    /// The user enters a Username and a masked Password (both required). A blank or unrecognised
    /// entry turns the offending labels and text boxes red, the guidance panel turns from blue to a
    /// red message naming the fix, and the Invalid Login dialog is raised. For an unrecognised entry
    /// the screen never says whether the username or the password was wrong. A recognised account is
    /// routed by <c>UserAccount.Role</c> to UI 9 (Career Advisor) or UI 12 (Administrator).
    /// </summary>
    public partial class StaffLoginForm : Form
    {
        // Colours used at runtime (spec 2.2.6). Everything else is set in the Designer.
        private static readonly Color InkColour = Color.FromArgb(27, 31, 36);
        private static readonly Color RedColour = Color.FromArgb(164, 38, 44);
        private static readonly Color RedTint = Color.FromArgb(250, 234, 235);
        private static readonly Color BlueColour = Color.FromArgb(31, 95, 168);
        private static readonly Color BlueTint = Color.FromArgb(232, 238, 246);
        private static readonly Color DeepNavy = Color.FromArgb(11, 31, 58);

        private const string AdministratorRole = "Administrator";
        private const string CareerAdvisorRole = "Career Advisor";

        private const string GuidanceText =
            "Learners do not log in. Only Career Advisors and System Administrators hold accounts.";

        private const string ReadyStatus = "Enter your username and password, then choose Log in.";

        private readonly Form homeForm;
        private bool navigating;

        /// <summary>Creates the staff login screen.</summary>
        /// <param name="homeForm">UI 1, shown again on Cancel.</param>
        public StaffLoginForm(Form homeForm)
        {
            InitializeComponent();
            this.homeForm = homeForm;
            ShowGuidance();
            SetStatus(ReadyStatus, DeepNavy);
        }

        #region Business Logic Layer hook

        /// <summary>
        /// Asks the BLL to check the credentials. The BLL looks up <c>UserAccount</c> by username,
        /// verifies the salted password hash (PBKDF2, never plaintext) and checks <c>IsActive</c>.
        /// It returns nothing about which part failed, so this screen cannot leak it.
        /// </summary>
        /// <returns>True and the account details when the credentials are recognised.</returns>
        private bool Authenticate(string username, string password, out int userId, out string fullName, out string role)
        {
            userId = 0;
            fullName = null;
            role = null;

            // TODO BLL: var account = LoginController.Authenticate(username, password);
            //           // returns (UserID, FullName, Role) for an active account whose salted hash matches, otherwise null
            //           if (account == null) return false;
            //           userId = account.UserId; fullName = account.FullName; role = account.Role;
            //           return true;
            // Until the BLL exists every entry is treated as unrecognised. There is deliberately no bypass.
            return false;
        }

        #endregion

        #region Field validation

        /// <summary>A blank username is named, because the user can already see it is blank.</summary>
        private bool CheckUsername()
        {
            return CheckField(txtUsername.Text.Trim().Length > 0, txtUsername, lblUsername, lblUsernameError,
                "Username: enter your staff username.");
        }

        /// <summary>A blank password is named, because the user can already see it is blank.</summary>
        private bool CheckPassword()
        {
            return CheckField(txtPassword.Text.Length > 0, txtPassword, lblPassword, lblPasswordError,
                "Password: enter your password.");
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

        /// <summary>Clears a red field once the user has typed something into it.</summary>
        private void txtUsername_Leave(object sender, EventArgs e)
        {
            if (txtUsername.Text.Trim().Length > 0)
            {
                CheckUsername();
            }
        }

        /// <summary>Clears a red field once the user has typed something into it.</summary>
        private void txtPassword_Leave(object sender, EventArgs e)
        {
            if (txtPassword.Text.Length > 0)
            {
                CheckPassword();
            }
        }

        #endregion

        #region Guidance panel and status band

        private void ShowGuidance()
        {
            pnlNotice.BackColor = BlueTint;
            pnlNoticeBar.BackColor = BlueColour;
            lblNotice.ForeColor = InkColour;
            lblNotice.Text = GuidanceText;
        }

        private void ShowNoticeError(string message)
        {
            pnlNotice.BackColor = RedTint;
            pnlNoticeBar.BackColor = RedColour;
            lblNotice.ForeColor = RedColour;
            lblNotice.Text = message;
        }

        private void SetStatus(string message, Color back)
        {
            pnlStatus.BackColor = back;
            lblStatus.ForeColor = Color.White;
            lblStatus.Text = message;
        }

        #endregion

        #region Buttons and navigation

        /// <summary>
        /// Log in → validate that both fields are filled, authenticate through the BLL, then open
        /// the dashboard for the account's role. Any failure raises UI 8.1a and clears the password.
        /// </summary>
        private void btnLogin_Click(object sender, EventArgs e)
        {
            bool usernameGiven = CheckUsername();
            bool passwordGiven = CheckPassword();
            if (!usernameGiven || !passwordGiven)
            {
                var lines = new List<string>();
                if (!usernameGiven) lines.Add("Username: enter your staff username.");
                if (!passwordGiven) lines.Add("Password: enter your password.");
                RejectLogin("Enter both your username and your password, then choose Log in again.", lines,
                    usernameGiven ? (Control)txtPassword : txtUsername);
                return;
            }

            int userId;
            string fullName;
            string role;
            bool recognised;
            try
            {
                recognised = Authenticate(txtUsername.Text.Trim(), txtPassword.Text, out userId, out fullName, out role);
            }
            catch (Exception)
            {
                txtPassword.Clear();
                SetStatus("Log in is not available right now. Please try again, or contact your Administrator.", RedColour);
                return;
            }

            if (!recognised || (role != AdministratorRole && role != CareerAdvisorRole))
            {
                // Unrecognised: both fields turn red and nothing says which one was wrong.
                const string message = "Username or password not recognised: check both and try again.";
                CheckField(false, txtUsername, lblUsername, lblUsernameError, message);
                CheckField(false, txtPassword, lblPassword, lblPasswordError, "Re-enter your password.");
                RejectLogin("The username or password is not recognised. Check both and try again. If you have forgotten " +
                            "your password, ask an Administrator to reset it.",
                    new List<string> { message, "If you have forgotten your password, an Administrator can reset it." },
                    txtUsername);
                return;
            }

            // One console for both roles: it removes the pages this role may not reach (decisions.md D11)
            // and opens on the role's own dashboard (UI 9 or UI 12).
            var console = new StaffConsoleForm(homeForm, userId, fullName, role);
            txtPassword.Clear();
            console.Show();
            navigating = true;
            Close();
        }

        /// <summary>Shows UI 8.1a Invalid Login, clears the password and returns focus.</summary>
        private void RejectLogin(string panelMessage, IList<string> dialogLines, Control focusTarget)
        {
            txtPassword.Clear();
            ShowNoticeError(panelMessage);
            SetStatus("Log in failed. Correct the fields marked in red.", RedColour);
            using (var dialog = new ValidationDialog("Invalid login", dialogLines))
            {
                dialog.ShowDialog(this);
            }

            focusTarget.Focus();
        }

        /// <summary>Forgot password only explains the process: nothing is reset from this screen.</summary>
        private void lnkForgotPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SetStatus("Passwords are reset by a System Administrator on UI 13.6 User accounts. Ask an Administrator " +
                      "to reset yours.", BlueColour);
        }

        /// <summary>Cancel returns to the homepage without logging in.</summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtPassword.Clear();
            homeForm.Show();
            navigating = true;
            Close();
        }

        #endregion

        #region Closing

        /// <summary>The caption-bar X asks first, because it closes Future Path.</summary>
        private void StaffLoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing || navigating || homeForm.Visible)
            {
                return;
            }

            using (var dialog = new ConfirmationDialog("Close Future Path", "Close Future Path?", "Close"))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>Ends the application when this screen was closed with the caption-bar X.</summary>
        private void StaffLoginForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!navigating && !homeForm.Visible)
            {
                Application.Exit();
            }
        }

        #endregion
    }
}
