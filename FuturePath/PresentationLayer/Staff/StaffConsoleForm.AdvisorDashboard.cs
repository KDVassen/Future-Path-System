
using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using FuturePath.ApplicationLayer;

namespace FuturePath.PresentationLayer
{
    // Handles the Career Advisor dashboard and daily session summary
    public partial class StaffConsoleForm
    {
        // Display the welcome message and refresh today's statistics
        private void AdvisorDashboardShown(bool firstTime)
        {
            if (firstTime)
            {
                lblAdvWelcome.Text = "Welcome, " + FullName;
            }

            AdvisorDashboardLoadToday();
        }

        #region Business Logic Layer hook

        // Load today's session statistics from the Business Logic Layer
        private void AdvisorDashboardLoadToday()
        {
            pnlTodayCard.Visible = false;
            try
            {
                // Retrieve saved session counts
                SessionSearchController sessionSearchController = new SessionSearchController();
                DailySummary today = sessionSearchController.GetDailySummary(UserId, DateTime.Today);
                AdvisorDashboardShowFigure(lblCompletedTileValue, today.CompletedCount);
                AdvisorDashboardShowFigure(lblInProgressTileValue, today.InProgressCount);
                AdvisorDashboardShowFigure(lblFollowUpTileValue, today.ExceptionCount);
                pnlTodayCard.Visible = true;
            }
            catch (Exception)
            {
                SetStatus("Today's figures could not be loaded. Sessions and reports are still available.", RedColour, Color.White);
            }
        }

        // Display a session count in its dashboard tile
        private static void AdvisorDashboardShowFigure(Label valueLabel, int value)
        {
            valueLabel.Text = value.ToString("N0", CultureInfo.CurrentCulture);
        }

        #endregion

        #region Hub buttons

        // Open the Sessions section
        private void btnAdvSearchSessions_Click(object sender, EventArgs e)
        {
            SelectPage(tabSessions);
        }

        // Open the Reports section
        private void btnAdvGenerateReports_Click(object sender, EventArgs e)
        {
            SelectPage(tabReports);
        }

        #endregion
    }
}
