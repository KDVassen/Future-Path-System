using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 10.1a — Learner profile tab of the Sessions dashboard (read-only).
    /// The advisor searches learners on surname, first name, grade, school and province (none is
    /// required) and sees one row per learner with their session count and last session date.
    /// Only what an advisor needs to find a learner is shown: contact details are never exposed.
    /// <b>Open learner profile</b> asks the host to switch to the Advisory session tab (10.2b)
    /// filtered to the selected learner's sessions.
    /// </summary>
    public partial class LearnerProfileTab : UserControl
    {
        // Colours used at runtime (spec 2.2.6). Everything else is set in the Designer.
        private static readonly Color InkColour = Color.FromArgb(27, 31, 36);
        private static readonly Color MutedColour = Color.FromArgb(88, 96, 108);

        /// <summary>Creates the tab. The host sets <see cref="UserId"/> and <see cref="Role"/> before <see cref="LoadData"/>.</summary>
        public LearnerProfileTab()
        {
            InitializeComponent();
            grdLearners.AutoGenerateColumns = false;
            lblResultsCount.Text = "Search to list learners";
        }

        #region Tab contract

        /// <summary>UserAccount.UserID of the signed-in staff member (the BLL checks access with it).</summary>
        public int UserId { get; set; }

        /// <summary>UserAccount.Role of the signed-in staff member ("Career Advisor" or "Administrator").</summary>
        public string Role { get; set; }

        /// <summary>Raised with a status message; isWarning = true shows amber, false shows blue.</summary>
        public event Action<string, bool> StatusChanged;

        /// <summary>
        /// Raised by <b>Open learner profile</b> with Learner.LearnerID and the learner's display name.
        /// The host switches to the Advisory session tab and lists that learner's sessions.
        /// </summary>
        public event Action<int, string> LearnerSessionsRequested;

        /// <summary>Caption of the host's primary button while this tab is active.</summary>
        public string PrimaryActionText
        {
            get { return "Open learner profile"; }
        }

        /// <summary>Binds the grade and province lists. The host calls this when the tab is first shown.</summary>
        public void LoadData()
        {
            LoadLists();
            RaiseStatus("Search on any field, or leave them all blank to list every learner.", false);
        }

        /// <summary>Opens the selected learner's sessions (the tab's primary action).</summary>
        public void RunPrimaryAction()
        {
            if (grdLearners.CurrentRow == null)
            {
                RaiseStatus("Select a learner in the list first, then choose Open learner profile.", true);
                return;
            }

            DataGridViewRow row = grdLearners.CurrentRow;
            int learnerId;
            if (!int.TryParse(Convert.ToString(row.Cells[colLearnerId.Index].Value, CultureInfo.InvariantCulture), out learnerId))
            {
                RaiseStatus("This learner's sessions could not be opened. Please search again.", true);
                return;
            }

            if (LearnerSessionsRequested != null)
            {
                LearnerSessionsRequested(learnerId, Convert.ToString(row.Cells[colLearner.Index].Value, CultureInfo.CurrentCulture));
            }
        }

        #endregion

        #region Business Logic Layer hooks

        /// <summary>Binds the Grade and Province lists to stored data (never literal Items).</summary>
        private void LoadLists()
        {
            try
            {
                // TODO BLL: cboGrade.DataSource = ReferenceDataController.GetGradeLevels();
                //           cboGrade.DisplayMember = "GradeDescription"; cboGrade.ValueMember = "GradeLevelID";
                // TODO BLL: cboProvince.DataSource = ReferenceDataController.GetProvinces();
                //           (Province has no table yet: proposed SystemParameter PROVINCE_1..9, as on UI 2.)
                cboGrade.SelectedIndex = -1;
                cboProvince.SelectedIndex = -1;
            }
            catch (Exception)
            {
                RaiseStatus("The grade and province lists could not be loaded. You can still search by name or school.", true);
            }
        }

        /// <summary>
        /// Asks the BLL for matching learners. The result carries only LearnerID, LearnerName,
        /// GradeDescription, SchoolName, Province, SessionCount and LastSessionDate — no contact details.
        /// </summary>
        private DataTable FindLearners(string surname, string firstName, int? gradeLevelId, string school, string province)
        {
            // TODO BLL: return LearnerController.FindLearners(surname, firstName, gradeLevelId, school, province, UserId);
            //           (the BLL checks the role before returning learner data — Section 8)
            return null;
        }

        #endregion

        #region Search

        /// <summary>Runs the search and reports the result in the host's status band.</summary>
        private void Find()
        {
            int? gradeLevelId = cboGrade.SelectedValue is int ? (int?)(int)cboGrade.SelectedValue : null;
            string province = cboProvince.SelectedIndex >= 0 ? cboProvince.Text : null;

            DataTable results;
            try
            {
                results = FindLearners(txtSurname.Text.Trim(), txtFirstName.Text.Trim(), gradeLevelId, txtSchool.Text.Trim(), province);
            }
            catch (Exception)
            {
                RaiseStatus("The learner list could not be loaded. Please try again.", true);
                return;
            }

            grdLearners.DataSource = results;
            int count = results == null ? 0 : results.Rows.Count;
            lblResultsCount.Text = count == 1 ? "1 learner matches" : count + " learners match";
            if (count == 0)
            {
                RaiseStatus("No learners match these criteria. Clear a field to widen the search.", true);
            }
            else
            {
                RaiseStatus(count + (count == 1 ? " learner" : " learners") + " found. Select one and choose Open learner profile.", false);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            Find();
        }

        /// <summary>Clears every criterion and the result list.</summary>
        private void btnClear_Click(object sender, EventArgs e)
        {
            txtSurname.Clear();
            txtFirstName.Clear();
            txtSchool.Clear();
            cboGrade.SelectedIndex = -1;
            cboProvince.SelectedIndex = -1;
            grdLearners.DataSource = null;
            lblResultsCount.Text = "Search to list learners";
            RaiseStatus("Search criteria cleared.", false);
            txtSurname.Focus();
        }

        /// <summary>Enter in the search card runs Find instead of the host's primary button.</summary>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Return && pnlSearchCard.ContainsFocus)
            {
                Find();
                return true;
            }

            return base.ProcessDialogKey(keyData);
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
                RunPrimaryAction();
            }
        }

        #endregion

        private void RaiseStatus(string message, bool isWarning)
        {
            if (StatusChanged != null)
            {
                StatusChanged(message, isWarning);
            }
        }
    }
}
