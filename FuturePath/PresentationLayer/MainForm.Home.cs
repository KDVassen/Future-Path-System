using System;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 1 Homepage: the shared entry point that splits into the Learner Advisory System (no login) and
    /// the staff area behind UI 8 (spec 2.1). The main window stays open, hidden, while the staff screens
    /// are in use, and every learner session returns here.
    /// </summary>
    public partial class MainForm
    {
        /// <summary>
        /// Called by ShowPage each time UI 1 is shown (ShowPage has already set the status band). Puts the
        /// focus on <b>Start advisory session</b>, so Enter starts a session as it did on the old homepage.
        /// </summary>
        private void HomeShown()
        {
            ActiveControl = btnStart;
        }

        /// <summary>Start advisory session → UI 2 with a fresh, empty session.</summary>
        private void btnStart_Click(object sender, EventArgs e)
        {
            StartLearnerSession();
        }

        /// <summary>Staff login → UI 8. The main window is hidden until the staff member returns.</summary>
        private void btnStaffLogin_Click(object sender, EventArgs e)
        {
            var loginForm = new StaffLoginForm(this);
            loginForm.Show();
            Hide();
        }
    }
}
