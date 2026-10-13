using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 10.1a — Learner profile tab of the Sessions dashboard (read-only).
    /// The advisor searches learners on surname, first name, grade, school and province (none is
    /// required) and sees one row per learner with their session count and last session date.
    /// Only what an advisor needs to find a learner is shown: contact details are never exposed.
    /// <b>Open learner profile</b> switches to the Advisory session tab (10.2b) filtered to the
    /// selected learner's sessions.
    /// <para>
    /// This file also holds the console's single <see cref="ProcessDialogKey"/> override, which gives
    /// Enter inside any of the three Sessions search cards (10.1a, 10.2b, 10.3c) to that tab's Find.
    /// </para>
    /// </summary>
    public partial class StaffConsoleForm
    {
        /// <summary>
        /// Called by <see cref="PageShown"/> each time UI 10.1a appears. The first time, it prepares the
        /// grid and binds the grade and province lists, then shows the search guidance.
        /// </summary>
        /// <param name="firstTime">True the first time the tab is shown in this console.</param>
        private void LearnerSearchShown(bool firstTime)
        {
            if (!firstTime)
            {
                return;
            }

            grdLearners.AutoGenerateColumns = false;
            lblLrnResultsCount.Text = "Search to list learners";
            LearnerSearchLoadLists();
            ReportStatus("Search on any field, or leave them all blank to list every learner.", false);
        }

        #region Button click events

        /// <summary>
        /// Open learner profile: moves to the Advisory session tab (10.2b) and lists the selected
        /// learner's sessions there.
        /// </summary>
        private void btnLearnerSearchOpen_Click(object sender, EventArgs e)
        {
            if (grdLearners.CurrentRow == null)
            {
                ReportStatus("Select a learner in the list first, then choose Open learner profile.", true);
                return;
            }

            DataGridViewRow row = grdLearners.CurrentRow;
            int learnerId;
            if (!int.TryParse(Convert.ToString(row.Cells[colLearnerId.Index].Value, CultureInfo.InvariantCulture), out learnerId))
            {
                ReportStatus("This learner's sessions could not be opened. Please search again.", true);
                return;
            }

            string learnerName = Convert.ToString(row.Cells[colLrnLearner.Index].Value, CultureInfo.CurrentCulture);
            SelectPage(tabSessionSearch);
            SessionSearchShowLearnerSessions(learnerId, learnerName);
        }

        #endregion

        #region Business Logic Layer hooks

        /// <summary>Binds the Grade and Province lists to stored data (never literal Items).</summary>
        private void LearnerSearchLoadLists()
        {
            try
            {
                // TODO BLL: cboLrnGrade.DataSource = ReferenceDataController.GetGradeLevels();
                //           cboLrnGrade.DisplayMember = "GradeDescription"; cboLrnGrade.ValueMember = "GradeLevelID";
                // TODO BLL: cboProvince.DataSource = ReferenceDataController.GetProvinces();
                //           (Province has no table yet: proposed SystemParameter PROVINCE_1..9, as on UI 2.)
                cboLrnGrade.SelectedIndex = -1;
                cboProvince.SelectedIndex = -1;
            }
            catch (Exception)
            {
                ReportStatus("The grade and province lists could not be loaded. You can still search by name or school.", true);
            }
        }

        /// <summary>
        /// Asks the BLL for matching learners. The result carries only LearnerID, LearnerName,
        /// GradeDescription, SchoolName, Province, SessionCount and LastSessionDate — no contact details.
        /// </summary>
        private DataTable LearnerSearchFindLearners(string surname, string firstName, int? gradeLevelId, string school, string province)
        {
            // TODO BLL: return LearnerController.FindLearners(surname, firstName, gradeLevelId, school, province, UserId);
            //           (the BLL checks the role before returning learner data — Section 8)
            return null;
        }

        #endregion

        #region Search

        /// <summary>Runs the search and reports the result in the status bar.</summary>
        private void LearnerSearchFind()
        {
            int? gradeLevelId = cboLrnGrade.SelectedValue is int ? (int?)(int)cboLrnGrade.SelectedValue : null;
            string province = cboProvince.SelectedIndex >= 0 ? cboProvince.Text : null;

            DataTable results;
            try
            {
                results = LearnerSearchFindLearners(txtLrnSurname.Text.Trim(), txtFirstName.Text.Trim(), gradeLevelId,
                    txtSchool.Text.Trim(), province);
            }
            catch (Exception)
            {
                ReportStatus("The learner list could not be loaded. Please try again.", true);
                return;
            }

            grdLearners.DataSource = results;
            int count = results == null ? 0 : results.Rows.Count;
            lblLrnResultsCount.Text = count == 1 ? "1 learner matches" : count + " learners match";
            if (count == 0)
            {
                ReportStatus("No learners match these criteria. Clear a field to widen the search.", true);
            }
            else
            {
                ReportStatus(count + (count == 1 ? " learner" : " learners") + " found. Select one and choose Open learner profile.", false);
            }
        }

        /// <summary>Find runs the learner search.</summary>
        private void btnLrnFind_Click(object sender, EventArgs e)
        {
            LearnerSearchFind();
        }

        /// <summary>Clears every criterion and the result list.</summary>
        private void btnLrnClear_Click(object sender, EventArgs e)
        {
            txtLrnSurname.Clear();
            txtFirstName.Clear();
            txtSchool.Clear();
            cboLrnGrade.SelectedIndex = -1;
            cboProvince.SelectedIndex = -1;
            grdLearners.DataSource = null;
            lblLrnResultsCount.Text = "Search to list learners";
            ReportStatus("Search criteria cleared.", false);
            txtLrnSurname.Focus();
        }

        /// <summary>Shows the last session date as dd/MM/yyyy (display formatting only).</summary>
        private void grdLearners_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == colLastSession.Index && e.Value is DateTime)
            {
                e.Value = ((DateTime)e.Value).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                e.FormattingApplied = true;
            }
            else if (e.ColumnIndex == colLastSession.Index && (e.Value == null || e.Value == DBNull.Value))
            {
                e.Value = "—";
                e.CellStyle.ForeColor = MutedColour;
                e.FormattingApplied = true;
            }
            else
            {
                e.CellStyle.ForeColor = InkColour;
            }
        }

        /// <summary>Double-clicking a learner is the same as Open learner profile.</summary>
        private void grdLearners_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                btnLearnerSearchOpen_Click(sender, e);
            }
        }

        #endregion

        #region Enter in a Sessions search card (UI 10.1a, 10.2b, 10.3c)

        /// <summary>
        /// Enter inside a Sessions search card runs that tab's Find instead of the primary (Accept)
        /// button: Learner profile (10.1a), Advisory session (10.2b) or Advisory reports (10.3c). Only
        /// the card on show can hold the focus, so at most one test is true. Every other key, and
        /// Enter anywhere else, is handled by the form as usual.
        /// </summary>
        /// <param name="keyData">The key pressed, with any modifier keys.</param>
        /// <returns>True when the key was handled here.</returns>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Return)
            {
                if (pnlLrnSearchCard.ContainsFocus)
                {
                    LearnerSearchFind();
                    return true;
                }

                if (pnlSesSearchCard.ContainsFocus)
                {
                    SessionSearchFind();
                    return true;
                }

                if (pnlRscSearchCard.ContainsFocus)
                {
                    ReportShortcutsFind();
                    return true;
                }
            }

            return base.ProcessDialogKey(keyData);
        }

        #endregion
    }
}
