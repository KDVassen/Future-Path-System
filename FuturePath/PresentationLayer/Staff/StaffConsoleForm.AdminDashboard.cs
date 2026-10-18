
using System;

namespace FuturePath.PresentationLayer
{
    // Handles the Administrator dashboard and its navigation buttons
    public partial class StaffConsoleForm
    {
        // Display the welcome message and enable Administrator features
        private void AdminDashboardShown(bool firstTime)
        {
            if (firstTime)
            {
                lblAdmWelcome.Text = "Welcome, " + FullName;
                btnMaintainReferenceData.Enabled = IsAdministrator;
            }
        }

        #region Hub buttons

        // Open the Reference Data section for Administrators
        private void btnMaintainReferenceData_Click(object sender, EventArgs e)
        {
            if (!IsAdministrator)
            {
                return;
            }

            SelectPage(tabReference);
        }

        // Open the Sessions section
        private void btnAdmSearchSessions_Click(object sender, EventArgs e)
        {
            SelectPage(tabSessions);
        }

        // Open the Reports section
        private void btnAdmGenerateReports_Click(object sender, EventArgs e)
        {
            SelectPage(tabReports);
        }

        #endregion
    }
}
