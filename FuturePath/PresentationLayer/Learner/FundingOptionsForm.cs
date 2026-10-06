using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 6.2b — Funding Options (standard, rail step 5; spec 2.3.8).
    /// Nothing is typed. The learner sets the Faculty and Funding type filters (both bound to stored
    /// reference data), may limit the list to options that are still open, and selects a row to fill
    /// the read-only detail card. Only <b>active</b> funding linked to the faculty or programme is
    /// listed. Funding is labelled as sample information to verify with the funder. There is no
    /// validation. <b>Add to Summary</b> sets <c>FundingInfoRequested = 1</c> and opens UI 7.
    /// </summary>
    public partial class FundingOptionsForm : Form
    {
        // Colours used at runtime (spec 2.2.6). Everything else is set in the Designer.
        private static readonly Color InkColour = Color.FromArgb(27, 31, 36);
        private static readonly Color MutedColour = Color.FromArgb(88, 96, 108);
        private static readonly Color RuleColour = Color.FromArgb(215, 220, 227);
        private static readonly Color AmberColour = Color.FromArgb(200, 137, 43);
        private static readonly Color GreenColour = Color.FromArgb(31, 107, 58);
        private static readonly Color GreenTint = Color.FromArgb(230, 242, 234);
        private static readonly Color DeepNavy = Color.FromArgb(11, 31, 58);
        private static readonly Font ChipFont = new Font("Segoe UI Semibold", 9F);

        private const string ReadyStatus = "Step 5 of 6 · Select a funding option to see its details.";

        private readonly Form homeForm;
        private readonly EligibilityResultsForm resultsForm;

        /// <summary>The UI 5 faculty the Faculty filter was last defaulted to.</summary>
        private int? defaultedFacultyId;
        private bool listsLoaded;

        /// <summary>Creates the Funding Options screen. Only stores its arguments.</summary>
        /// <param name="homeForm">UI 1, shown again when the session is left.</param>
        /// <param name="resultsForm">UI 6, which opened this screen and opens UI 7.</param>
        public FundingOptionsForm(Form homeForm, EligibilityResultsForm resultsForm)
        {
            InitializeComponent();
            this.homeForm = homeForm;
            this.resultsForm = resultsForm;

            dgvFunding.AutoGenerateColumns = false;
            ClearDetail();
            SetStatus(ReadyStatus, false);
        }

        #region Session values handed on by the previous screen

        /// <summary>AdvisorySession.SessionID of the session in progress.</summary>
        public int? SessionId { get; set; }

        /// <summary>Learner.LearnerID of the learner in this session.</summary>
        public int? LearnerId { get; set; }

        /// <summary>The learner context strip text.</summary>
        public string LearnerContext { get; set; }

        /// <summary>AdvisorySession.PreferredFacultyID chosen on UI 5 (the Faculty filter's default).</summary>
        public int? PreferredFacultyId { get; set; }

        /// <summary>AdvisorySession.SelectedProgrammeID chosen on UI 5 (programme-linked funding is included).</summary>
        public int? SelectedProgrammeId { get; set; }

        /// <summary>DegreeProgramme.ProgrammeName of the learner's selection.</summary>
        public string SelectedProgrammeName { get; set; }

        #endregion

        #region Business Logic Layer hooks

        /// <summary>Binds both filters to stored reference data (never literal Items).</summary>
        private void LoadLists()
        {
            try
            {
                // TODO BLL: cboFaculty.DataSource = ReferenceDataController.GetActiveFaculties();
                //           cboFaculty.DisplayMember = "FacultyName"; cboFaculty.ValueMember = "FacultyID";
                // TODO BLL: cboFundingType.DataSource = FundingController.GetFundingTypes();
                //           SystemParameter rows FUNDING_TYPE_1..4, preceded by the "All funding types" row the BLL supplies.
                //           cboFundingType.DisplayMember = "ParameterValue"; cboFundingType.ValueMember = "ParameterCode";
                cboFundingType.SelectedIndex = cboFundingType.Items.Count > 0 ? 0 : -1;
                listsLoaded = true;
            }
            catch (Exception)
            {
                SetStatus("The faculty and funding type lists could not be loaded. Go back and try again.", true);
            }
        }

        /// <summary>Lists the active funding for the chosen faculty (and the selected programme) using the filters.</summary>
        private void LoadFunding()
        {
            ClearDetail();
            UpdateHeading();
            try
            {
                // TODO BLL: dgvFunding.DataSource = FundingController.GetActiveFunding(
                //               cboFaculty.SelectedValue as int?, SelectedProgrammeId,
                //               cboFundingType.SelectedValue as string, chkOpenOnly.Checked);
                //           Active FundingOption rows linked through FundingFaculty or FundingProgramme, ordered by
                //           ClosingDate: FundingID, FundingName, FundingType, LinkedTo, ClosingDate,
                //           EligibilityDescription, RequiredDocuments, ContactInformation.
                int count = dgvFunding.Rows.Count;
                lblFundingCount.Text = count == 1 ? "1 funding option shown" : count + " funding options shown";
            }
            catch (Exception)
            {
                SetStatus("Funding options could not be loaded. Please try again.", true);
            }
        }

        /// <summary>Records that the learner wants funding in the summary (FundingInfoRequested = 1).</summary>
        private void RequestFundingInSummary()
        {
            // TODO BLL: FundingController.RequestFundingInfo(SessionId.Value);   // AdvisorySession.FundingInfoRequested = 1
        }

        #endregion

        #region Filters

        /// <summary>Defaults the Faculty filter to the UI 5 faculty whenever that choice has changed.</summary>
        private void ApplyDefaultFaculty()
        {
            if (defaultedFacultyId == PreferredFacultyId && dgvFunding.DataSource != null)
            {
                return;
            }

            if (PreferredFacultyId.HasValue && cboFaculty.Items.Count > 0)
            {
                cboFaculty.SelectedValue = PreferredFacultyId.Value;
            }

            defaultedFacultyId = PreferredFacultyId;
            LoadFunding();
        }

        private void cboFaculty_SelectionChangeCommitted(object sender, EventArgs e) { LoadFunding(); }

        private void cboFundingType_SelectionChangeCommitted(object sender, EventArgs e) { LoadFunding(); }

        private void chkOpenOnly_CheckedChanged(object sender, EventArgs e) { LoadFunding(); }

        /// <summary>"Funding linked to the Faculty of Science" — follows the Faculty filter.</summary>
        private void UpdateHeading()
        {
            string faculty = cboFaculty.SelectedIndex >= 0 ? cboFaculty.Text : string.Empty;
            if (faculty.Length == 0)
            {
                lblFundingHeading.Text = "Funding linked to your faculty";
            }
            else if (faculty.StartsWith("Faculty", StringComparison.OrdinalIgnoreCase))
            {
                lblFundingHeading.Text = "Funding linked to the " + faculty;
            }
            else
            {
                lblFundingHeading.Text = "Funding linked to the Faculty of " + faculty;
            }
        }

        #endregion

        #region Grid and detail card (display only)

        /// <summary>Selecting a row fills the read-only detail card.</summary>
        private void dgvFunding_SelectionChanged(object sender, EventArgs e)
        {
            DataGridViewRow row = dgvFunding.CurrentRow;
            if (row == null)
            {
                ClearDetail();
                return;
            }

            lblDetailHeading.Text = CellText(row, colFundingName);
            txtEligibility.Text = CellText(row, colEligibility);
            txtDocuments.Text = CellText(row, colDocuments);
            txtContact.Text = CellText(row, colContact);
        }

        /// <summary>Shows the first option's details as soon as the list is filled.</summary>
        private void dgvFunding_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            int count = dgvFunding.Rows.Count;
            lblFundingCount.Text = count == 1 ? "1 funding option shown" : count + " funding options shown";
            if (count > 0)
            {
                dgvFunding.CurrentCell = dgvFunding.Rows[0].Cells[colFundingName.Index];
            }
            else
            {
                ClearDetail();
            }
        }

        /// <summary>
        /// Status chip derived from the stored ClosingDate against today's date (display only):
        /// "✓ Open" on green, or "– Closed" in grey. No closing date counts as open.
        /// </summary>
        private void dgvFunding_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != colStatus.Index)
            {
                return;
            }

            object closing = dgvFunding.Rows[e.RowIndex].Cells[colClosingDate.Index].Value;
            bool open = closing == null || closing == DBNull.Value
                        || Convert.ToDateTime(closing, CultureInfo.InvariantCulture).Date >= DateTime.Today;
            e.Value = open ? "✓ Open" : "– Closed";
            e.CellStyle.Font = ChipFont;
            e.CellStyle.ForeColor = open ? GreenColour : MutedColour;
            e.CellStyle.BackColor = open ? GreenTint : RuleColour;
            e.FormattingApplied = true;
        }

        private static string CellText(DataGridViewRow row, DataGridViewColumn column)
        {
            object value = row.Cells[column.Index].Value;
            string text = value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value, CultureInfo.CurrentCulture);
            return text.Length == 0 ? "—" : text;
        }

        private void ClearDetail()
        {
            lblDetailHeading.Text = "Select a funding option to see its details";
            txtEligibility.Text = string.Empty;
            txtDocuments.Text = string.Empty;
            txtContact.Text = string.Empty;
        }

        #endregion

        #region Status band

        /// <summary>Status band: deep navy for normal messages, amber for a problem (never exception text).</summary>
        private void SetStatus(string message, bool warning)
        {
            pnlStatus.BackColor = warning ? AmberColour : DeepNavy;
            lblStatus.ForeColor = warning ? InkColour : Color.White;
            lblStatus.Text = message;
        }

        #endregion

        #region Buttons and navigation

        /// <summary>Add to Summary → sets FundingInfoRequested = 1, then opens UI 7 through UI 6.</summary>
        private void btnAddToSummary_Click(object sender, EventArgs e)
        {
            try
            {
                RequestFundingInSummary();
            }
            catch (Exception)
            {
                SetStatus("Funding could not be added to your summary. Please try again.", true);
                return;
            }

            resultsForm.OpenSummary(this);
        }

        /// <summary>Back → UI 6 (the advisory outcome). Nothing changes.</summary>
        private void btnBack_Click(object sender, EventArgs e)
        {
            resultsForm.Show();
            Hide();
        }

        /// <summary>Home on the rail leaves the session after confirmation.</summary>
        private void btnNavHome_Click(object sender, EventArgs e)
        {
            LeaveSession();
        }

        /// <summary>
        /// Asks for confirmation, returns to UI 1 and closes every learner screen. Anything already
        /// saved stays on record as an "In progress" session (decisions.md D3).
        /// </summary>
        private void LeaveSession()
        {
            using (var dialog = new ConfirmationDialog("Leave this advisory session",
                "Return to the homepage? Anything already saved stays on record as an \"In progress\" session. " +
                "Your session is only completed when you export the advisory summary.", "Leave session"))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }
            }

            homeForm.Show();
            foreach (Form screen in Application.OpenForms.Cast<Form>().ToList())
            {
                if (screen != homeForm)
                {
                    screen.Close();
                }
            }
        }

        #endregion

        #region Showing and closing

        /// <summary>Shows the learner context, loads the filters once and lists the funding.</summary>
        private void FundingOptionsForm_VisibleChanged(object sender, EventArgs e)
        {
            if (!Visible)
            {
                return;
            }

            lblContext.Text = LearnerContext;
            SetStatus(ReadyStatus, false);
            if (!listsLoaded)
            {
                LoadLists();
            }

            ApplyDefaultFaculty();
        }

        /// <summary>The caption-bar X asks first, because it closes Future Path.</summary>
        private void FundingOptionsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing || homeForm.Visible)
            {
                return;
            }

            using (var dialog = new ConfirmationDialog("Close Future Path",
                "Close Future Path? Anything already saved stays on record as an \"In progress\" session.", "Close"))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>Ends the application when this screen was closed with the caption-bar X.</summary>
        private void FundingOptionsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!homeForm.Visible)
            {
                Application.Exit();
            }
        }

        #endregion
    }
}
