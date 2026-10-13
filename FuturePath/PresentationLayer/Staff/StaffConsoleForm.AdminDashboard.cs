using System;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 12 — Administrator dashboard (Administrator). The System Administrator's hub, built to the hub
    /// pattern (screens-detail.md): a welcome line and one card per task with a button to UI 13 (Reference
    /// data), UI 10 (Sessions — Administrators reach it too, decisions.md D11) and UI 11 (Reports).
    /// Reference data is offered only when the signed-in role is Administrator; the BLL checks the role again
    /// on every change (Section 8).
    /// </summary>
    public partial class StaffConsoleForm
    {
        /// <summary>
        /// Called by <see cref="PageShown"/> each time UI 12 appears. The first time, writes the welcome line
        /// and enables "Maintain reference data" only for an Administrator.
        /// </summary>
        /// <param name="firstTime">True the first time UI 12 is shown in this console.</param>
        private void AdminDashboardShown(bool firstTime)
        {
            if (firstTime)
            {
                lblAdmWelcome.Text = "Welcome, " + FullName;
                btnMaintainReferenceData.Enabled = IsAdministrator;
            }
        }

        #region Hub buttons

        /// <summary>Maintain reference data → UI 13 on the sub-tab used last (Administrators only; a Career Advisor never reaches it).</summary>
        private void btnMaintainReferenceData_Click(object sender, EventArgs e)
        {
            if (!IsAdministrator)
            {
                return;
            }

            SelectPage(tabReference);
        }

        /// <summary>Search advisory sessions → UI 10 Sessions (opens on the Sessions tab used last).</summary>
        private void btnAdmSearchSessions_Click(object sender, EventArgs e)
        {
            SelectPage(tabSessions);
        }

        /// <summary>Generate reports → UI 11 Reports.</summary>
        private void btnAdmGenerateReports_Click(object sender, EventArgs e)
        {
            SelectPage(tabReports);
        }

        #endregion
    }
}
