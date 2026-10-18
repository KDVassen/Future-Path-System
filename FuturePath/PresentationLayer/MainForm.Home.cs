
using System;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    // Handles the homepage and navigation to learner or staff screens
    public partial class MainForm
    {
        // Focus the Start button when the homepage opens
        private void HomeShown()
        {
            ActiveControl = btnStart;
        }

        // Start a new learner advisory session
        private void btnStart_Click(object sender, EventArgs e)
        {
            StartLearnerSession();
        }

        // Open staff login and hide the main window
        private void btnStaffLogin_Click(object sender, EventArgs e)
        {
            var loginForm = new StaffLoginForm(this);
            loginForm.Show();
            Hide();
        }
    }
}
