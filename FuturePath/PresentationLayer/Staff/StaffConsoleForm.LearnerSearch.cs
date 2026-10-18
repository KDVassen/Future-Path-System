
using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using FuturePath.ApplicationLayer;

namespace FuturePath.PresentationLayer
{
    // Handles learner profile searches and navigation to advisory sessions
    // Displays learner details without exposing contact information
    public partial class StaffConsoleForm
    {
        // Initialise the learner search screen and load its dropdown lists
        private void LearnerSearchShown(bool firstTime)
        {
            if (!firstTime)
            {
                return;
            }

            dgvLearners.AutoGenerateColumns = false;
            lblLrnResultsCount.Text = "Search to list learners";
            LearnerSearchLoadLists();
            ReportStatus("Search on any field, or leave them all blank to list every learner.", false);
        }

        #region Button click events

        // Open the selected learner's advisory sessions
        private void btnLearnerSearchOpen_Click(object sender, EventArgs e)
        {
            if (dgvLearners.CurrentRow == null)
            {
                ReportStatus("Select a learner in the list first, then choose Open learner profile.", true);
                return;
            }

            DataGridViewRow row = dgvLearners.CurrentRow;
            int learnerId;
            if (!int.TryParse(Convert.ToString(row.Cells[colLearnersId.Index].Value, CultureInfo.InvariantCulture), out learnerId))
            {
                ReportStatus("This learner's sessions could not be opened. Please search again.", true);
                return;
            }

            string learnerName = Convert.ToString(row.Cells[colLearnersName.Index].Value, CultureInfo.CurrentCulture);
            SelectPage(tabSessionSearch);
            SessionSearchShowLearnerSessions(learnerId, learnerName);
        }

        #endregion

        #region Business Logic Layer hooks

        // Load the available grades and provinces from stored reference data
        private void LearnerSearchLoadLists()
        {
            try
            {
                ReferenceDataController referenceDataController = new ReferenceDataController();
                cboLrnGrade.DisplayMember = "GradeDescription";
                cboLrnGrade.ValueMember = "GradeLevelID";
                cboLrnGrade.DataSource = referenceDataController.GetGradeLevels(false);

                // Load provinces from system parameters
                cboProvince.DisplayMember = "ParameterValue";
                cboProvince.ValueMember = "ParameterValue";
                cboProvince.DataSource = referenceDataController.GetProvinces();

                cboLrnGrade.SelectedIndex = -1;
                cboProvince.SelectedIndex = -1;
            }
            catch (Exception)
            {
                ReportStatus("The grade and province lists could not be loaded. You can still search by name or school.", true);
            }
        }

        // Search for matching learners using the selected criteria
        // Return learner details without contact information
        private DataTable LearnerSearchFindLearners(string surname, string firstName, int gradeLevelId, string school, string province)
        {
            LearnerController learnerController = new LearnerController();
            return learnerController.FindLearners(UserId, surname, firstName, gradeLevelId, school, province);
        }

        #endregion

        #region Search

        // Run the learner search and display the number of matches
        private void LearnerSearchFind()
        {
            DataTable results;
            try
            {
                results = LearnerSearchFindLearners(txtLrnSurname.Text.Trim(), txtFirstName.Text.Trim(), SelectedId(cboLrnGrade),
                    txtSchool.Text.Trim(), SelectedCode(cboProvince));
            }
            catch (Exception)
            {
                ReportStatus("The learner list could not be loaded. Please try again.", true);
                return;
            }

            dgvLearners.DataSource = results;
            int count = results.Rows.Count;
            if (count == 1)
            {
                lblLrnResultsCount.Text = "1 learner matches";
            }
            else
            {
                lblLrnResultsCount.Text = count + " learners match";
            }

            if (count == 0)
            {
                ReportStatus("No learners match these criteria. Clear a field to widen the search.", true);
            }
            else
            {
                ReportStatus(count + (count == 1 ? " learner" : " learners") + " found. Select one and choose Open learner profile.", false);
            }
        }

        // Search for learners when Find is clicked
        private void btnLrnFind_Click(object sender, EventArgs e)
        {
            LearnerSearchFind();
        }

        // Clear the search fields and results
        private void btnLrnClear_Click(object sender, EventArgs e)
        {
            txtLrnSurname.Clear();
            txtFirstName.Clear();
            txtSchool.Clear();
            cboLrnGrade.SelectedIndex = -1;
            cboProvince.SelectedIndex = -1;
            dgvLearners.DataSource = null;
            lblLrnResultsCount.Text = "Search to list learners";
            ReportStatus("Search criteria cleared.", false);
            txtLrnSurname.Focus();
        }

        // Format the last session date for display
        private void dgvLearners_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == colLearnersLastSession.Index && e.Value is DateTime)
            {
                e.Value = ((DateTime)e.Value).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                e.FormattingApplied = true;
            }
            else if (e.ColumnIndex == colLearnersLastSession.Index && (e.Value == null || e.Value == DBNull.Value))
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

        // Open a learner's profile when their row is double-clicked
        private void dgvLearners_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                btnLearnerSearchOpen_Click(sender, e);
            }
        }

        #endregion

        #region Enter in a Sessions search card (UI 10.1a, 10.2b, 10.3c)

        // Run the active Sessions search when Enter is pressed
        // Use normal keyboard handling outside the search cards
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
