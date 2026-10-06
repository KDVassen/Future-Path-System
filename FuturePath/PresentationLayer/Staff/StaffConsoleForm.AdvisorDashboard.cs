using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 9 — Advisor dashboard (Career Advisor). The Career Advisor's hub, built to the hub pattern
    /// (screens-detail.md): a welcome line, one card per task with a button to UI 10 (Sessions) and UI 11
    /// (Reports), and a read-only "Today" summary whose figures come from the BLL. The summary stays hidden
    /// until figures arrive, so it is never shown empty.
    /// </summary>
    public partial class StaffConsoleForm
    {
        /// <summary>
        /// Called by <see cref="ShowPage"/> each time UI 9 appears. The welcome line is written once; the
        /// "Today" figures are refreshed on every visit, because they change during the day.
        /// </summary>
        /// <param name="firstTime">True the first time UI 9 is shown in this console.</param>
        private void AdvisorDashboardShown(bool firstTime)
        {
            if (firstTime)
            {
                lblAdvWelcome.Text = "Welcome, " + FullName;
            }

            AdvisorDashboardLoadToday();
        }

        #region Business Logic Layer hook

        /// <summary>
        /// Fills the read-only "Today" figures. The card stays hidden unless the BLL returns them.
        /// </summary>
        private void AdvisorDashboardLoadToday()
        {
            pnlTodayCard.Visible = false;
            try
            {
                // TODO BLL: var today = SessionSearchController.GetDailySummary(DateTime.Today);
                //           // counts from AdvisorySession (SessionStatus, SessionDate, DateCompleted) and the selected
                //           // RecommendationResult (OutcomeCode NCE / MIN) — read back, never recalculated
                //           if (today == null) return;
                //           AdvisorDashboardShowFigure(tilCompletedValue, today.CompletedCount);
                //           AdvisorDashboardShowFigure(tilInProgressValue, today.InProgressCount);
                //           AdvisorDashboardShowFigure(tilFollowUpValue, today.ExceptionCount);
                //           pnlTodayCard.Visible = true;
            }
            catch (Exception)
            {
                SetStatus("Today's figures could not be loaded. Sessions and reports are still available.", RedColour, Color.White);
            }
        }

        /// <summary>Writes one whole-number figure into its tile.</summary>
        /// <param name="valueLabel">The tile's value label (tilCompletedValue, tilInProgressValue or tilFollowUpValue).</param>
        /// <param name="value">The count returned by the BLL.</param>
        private static void AdvisorDashboardShowFigure(Label valueLabel, int value)
        {
            valueLabel.Text = value.ToString("N0", CultureInfo.CurrentCulture);
        }

        #endregion

        #region Hub buttons

        /// <summary>Search advisory sessions → UI 10 Sessions (opens on the Sessions tab used last).</summary>
        private void btnAdvSearchSessions_Click(object sender, EventArgs e)
        {
            ShowPage(tabSessions);
        }

        /// <summary>Generate reports → UI 11 Reports.</summary>
        private void btnAdvGenerateReports_Click(object sender, EventArgs e)
        {
            ShowPage(tabReports);
        }

        #endregion
    }
}
