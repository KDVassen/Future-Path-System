using System;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 1 — Future Path Homepage. The shared entry point that splits into the Learner Advisory
    /// System (no login) and the staff area behind UI 8 (spec 2.1). It is the application's main
    /// form: it stays open, hidden, while the learner or staff screens are in use, and every other
    /// screen returns here.
    /// </summary>
    public partial class HomeForm : Form
    {
        /// <summary>Creates the homepage.</summary>
        public HomeForm()
        {
            InitializeComponent();
            lblStatus.Text = "Choose how you want to use Future Path.";
        }

        /// <summary>Start advisory session → UI 2 with a fresh, empty session.</summary>
        private void btnStart_Click(object sender, EventArgs e)
        {
            var profileForm = new LearnerProfileForm(this);
            profileForm.Show();
            Hide();
        }

        /// <summary>Staff login → UI 8.</summary>
        private void btnStaffLogin_Click(object sender, EventArgs e)
        {
            var loginForm = new StaffLoginForm(this);
            loginForm.Show();
            Hide();
        }

        /// <summary>Resets the status band whenever the homepage comes back into view.</summary>
        private void HomeForm_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                lblStatus.Text = "Choose how you want to use Future Path.";
            }
        }

        /// <summary>Closing the homepage closes Future Path, so ask first.</summary>
        private void HomeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing)
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

        private void HomeForm_Load(object sender, EventArgs e)
        {

        }
    }
}
