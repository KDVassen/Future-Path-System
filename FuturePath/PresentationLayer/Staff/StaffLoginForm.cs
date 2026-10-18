
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using FuturePath.ApplicationLayer;

namespace FuturePath.PresentationLayer
{
    // Handles staff login for Career Advisors and System Administrators
    // Validates credentials and opens the dashboard for the signed-in role
    public partial class StaffLoginForm : Form
    {
        // Colours used for validation messages and guidance
        private static readonly Color InkColour = Color.FromArgb(27, 31, 36);
        private static readonly Color RedColour = Color.FromArgb(164, 38, 44);
        private static readonly Color RedTint = Color.FromArgb(250, 234, 235);
        private static readonly Color BlueColour = Color.FromArgb(31, 95, 168);
        private static readonly Color BlueTint = Color.FromArgb(232, 238, 246);
        private static readonly Color DeepNavy = Color.FromArgb(11, 31, 58);

        private const string GuidanceText =
            "Learners do not log in. Only Career Advisors and System Administrators hold accounts.";

        private const string ReadyStatus = "Enter your username and password, then choose Log in.";

        private readonly Form homeForm;
        private bool navigating;

        // Initialise the login screen and display the initial guidance
        public StaffLoginForm(Form homeForm)
        {
            InitializeComponent();
            this.homeForm = homeForm;
            ShowGuidance();
            SetStatus(ReadyStatus, DeepNavy);
        }

        #region Business Logic Layer hook

        // Authenticate staff credentials through the login controller
        // Return the account if valid, otherwise null
        private UserAccount Authenticate(string username, string password)
        {
            LoginController loginController = new LoginController();
            return loginController.Authenticate(username, password);
        }

        #endregion

        #region Field validation

        // Check whether a username has been entered
        private bool CheckUsername()
        {
            return CheckField(txtUsername.Text.Trim().Length > 0, txtUsername, lblUsername, lblUsernameError,
                "Username: enter your staff username.");
        }

        // Check whether a password has been entered
        private bool CheckPassword()
        {
            return CheckField(txtPassword.Text.Length > 0, txtPassword, lblPassword, lblPasswordError,
                "Password: enter your password.");
        }

        // Highlight invalid fields and display their validation messages
        private static bool CheckField(bool valid, Control input, Label fieldLabel, Label messageLabel, string message)
        {
            input.BackColor = valid ? SystemColors.Window : RedTint;
            fieldLabel.ForeColor = valid ? InkColour : RedColour;
            messageLabel.Text = valid ? string.Empty : "⚠ " + message;
            messageLabel.Visible = !valid;
            input.AccessibleDescription = valid ? null : message;
            return valid;
        }

        // Clear the username error once a value has been entered
        private void txtUsername_Leave(object sender, EventArgs e)
        {
            if (txtUsername.Text.Trim().Length > 0)
            {
                CheckUsername();
            }
        }

        // Clear the password error once a value has been entered
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

        // Validate login details and authenticate the staff member
        // Open the correct dashboard or display validation errors
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

            UserAccount account;
            try
            {
                account = Authenticate(txtUsername.Text.Trim(), txtPassword.Text);
            }
            catch (Exception)
            {
                txtPassword.Clear();
                SetStatus("Log in is not available right now. Please try again, or contact your Administrator.", RedColour);
                return;
            }

            if (account == null)
            {
                // Do not reveal whether the username or password was incorrect
                const string message = "Username or password not recognised: check both and try again.";
                CheckField(false, txtUsername, lblUsername, lblUsernameError, message);
                CheckField(false, txtPassword, lblPassword, lblPasswordError, "Re-enter your password.");
                RejectLogin("The username or password is not recognised. Check both and try again. If you have forgotten " +
                            "your password, ask an Administrator to reset it.",
                    new List<string> { message, "If you have forgotten your password, an Administrator can reset it." },
                    txtUsername);
                return;
            }

            // Open the staff console with access based on the account's role
            StaffConsoleForm console = new StaffConsoleForm(homeForm, account.UserID, account.FullName, account.RoleName);
            txtPassword.Clear();
            console.Show();
            navigating = true;
            Close();
        }

        // Display the invalid login dialog and return focus to the input field
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

        // Explain that password resets must be handled by an Administrator
        private void lnkForgotPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SetStatus("Passwords are reset by a System Administrator on UI 13.6 User accounts. Ask an Administrator " +
                      "to reset yours.", BlueColour);
        }

        // Cancel login and return to the homepage
        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtPassword.Clear();
            homeForm.Show();
            navigating = true;
            Close();
        }

        #endregion

        #region Closing

        // Ask for confirmation before closing the application from the login screen
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

        // Exit the application when the login screen is closed directly
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
