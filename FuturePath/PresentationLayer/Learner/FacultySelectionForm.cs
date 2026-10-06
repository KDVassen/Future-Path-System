using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 5 — Faculty &amp; Department Selection (standard, step 4 of 6; spec 2.3.5).
    /// The learner picks a search mode, a faculty and one programme (with its in-grid
    /// <b>Select</b> button). The requirements panel shows the stored ProgrammeRequirement rows for
    /// that programme, and the interest-alignment strip shows how the questionnaire answers from
    /// UI 4 align with it. Everything shown is system-supplied, so there is no validation dialog:
    /// a missing faculty or programme gets an amber field message or status instead.
    /// <b>Check eligibility</b> runs Sequence 1, saves the preferred faculty and selected programme
    /// on the session, and opens UI 6. It stays active, so the learner can come back and check
    /// other programmes.
    /// </summary>
    public partial class FacultySelectionForm : Form
    {
        // Colours used at runtime (spec 2.2.6). Everything else is set in the Designer.
        private static readonly Color InkColour = Color.FromArgb(27, 31, 36);
        private static readonly Color AmberColour = Color.FromArgb(200, 137, 43);
        private static readonly Color AmberText = Color.FromArgb(138, 93, 24);
        private static readonly Color AmberTint = Color.FromArgb(252, 243, 224);
        private static readonly Color NavyColour = Color.FromArgb(19, 41, 75);
        private static readonly Color GreenColour = Color.FromArgb(31, 107, 58);
        private static readonly Color DeepNavy = Color.FromArgb(11, 31, 58);

        private const string ReadyStatus = "Step 4 of 6 · Choose a faculty, select a programme, then check eligibility.";

        private readonly Form homeForm;
        private readonly Form previousForm;
        private EligibilityResultsForm resultsForm;

        /// <summary>Creates the faculty and department screen. Only stores its arguments.</summary>
        /// <param name="homeForm">UI 1, shown again when the session is left.</param>
        /// <param name="previousForm">UI 4, shown again by Back.</param>
        public FacultySelectionForm(Form homeForm, Form previousForm)
        {
            InitializeComponent();
            this.homeForm = homeForm;
            this.previousForm = previousForm;
            SetStatus(ReadyStatus, DeepNavy, Color.White);
            LoadFaculties();
        }

        #region Session values handed to the next screens

        /// <summary>AdvisorySession.SessionID of the in-progress session.</summary>
        public int? SessionId { get; set; }

        /// <summary>Learner.LearnerID of the learner in this session.</summary>
        public int? LearnerId { get; set; }

        /// <summary>The learner context strip text built on UI 2.</summary>
        public string LearnerContext { get; set; }

        /// <summary>AdvisorySession.PreferredFacultyID — the faculty chosen in the Faculty combo.</summary>
        public int? PreferredFacultyId { get; set; }

        /// <summary>AdvisorySession.SelectedProgrammeID — the programme chosen with its Select button.</summary>
        public int? SelectedProgrammeId { get; set; }

        /// <summary>DegreeProgramme.ProgrammeName of the selected programme, used in UI 6 headlines.</summary>
        public string SelectedProgrammeName { get; set; }

        #endregion

        #region Business Logic Layer hooks

        /// <summary>Binds the Faculty combo to the active faculties (never literal Items).</summary>
        private void LoadFaculties()
        {
            try
            {
                // TODO BLL: cboFaculty.DataSource = ReferenceDataController.GetActiveFaculties();
                //           cboFaculty.DisplayMember = "FacultyName"; cboFaculty.ValueMember = "FacultyID";
                cboFaculty.SelectedIndex = -1;
            }
            catch (Exception)
            {
                SetStatus("The faculty list could not be loaded. Please close and try again.", AmberColour, InkColour);
            }
        }

        /// <summary>
        /// Fills the programme grid: the active programmes of the chosen faculty while browsing,
        /// or the active programmes whose name contains the search text while searching.
        /// </summary>
        private void LoadProgrammes()
        {
            try
            {
                if (rbBrowse.Checked)
                {
                    // TODO BLL: dgvProgrammes.DataSource = cboFaculty.SelectedIndex < 0 ? null
                    //               : ReferenceDataController.GetActiveProgrammes((int)cboFaculty.SelectedValue);
                    lblProgrammeCount.Text = cboFaculty.SelectedIndex < 0
                        ? "Choose a faculty to list its active programmes."
                        : dgvProgrammes.Rows.Count + " active programmes in " + cboFaculty.Text;
                }
                else
                {
                    string text = txtProgrammeSearch.Text.Trim();
                    // TODO BLL: dgvProgrammes.DataSource = text.Length == 0 ? null
                    //               : ReferenceDataController.SearchActiveProgrammes(text);
                    lblProgrammeCount.Text = text.Length == 0
                        ? "Type part of a programme name to search every faculty."
                        : dgvProgrammes.Rows.Count + " active programmes matching \"" + text + "\"";
                }

                dgvProgrammes.Invalidate();
            }
            catch (Exception)
            {
                SetStatus("The programme list could not be loaded. Please try again.", AmberColour, InkColour);
            }
        }

        /// <summary>
        /// Fills the requirements panel from ProgrammeRequirement (and the programme's minimum
        /// overall average) and the interest-alignment strip for the selected programme.
        /// </summary>
        private void LoadProgrammeDetails()
        {
            lblRequirementsProgramme.Text = SelectedProgrammeName ?? "No programme selected yet";
            lblAlignmentValue.Text = "—";
            lblStrongestCategories.Text = "Select a programme to see how your questionnaire answers align with it.";
            if (!SelectedProgrammeId.HasValue)
            {
                dgvRequirements.DataSource = null;
                return;
            }

            try
            {
                // TODO BLL: dgvRequirements.DataSource =
                //               ReferenceDataController.GetActiveRequirements(SelectedProgrammeId.Value);
                //           (rows: SubjectName, MinimumMark, RequirementType; plus an "Overall average" row
                //            from DegreeProgramme.MinimumOverallAverage)
                // TODO BLL: var alignment = InterestController.GetProgrammeAlignment(SessionId.Value, SelectedProgrammeId.Value);
                //           lblAlignmentValue.Text = alignment.AlignmentPercent.ToString("0") + "%";
                //           lblStrongestCategories.Text = "Strongest categories: " + string.Join(", ", alignment.StrongestCategories);
            }
            catch (Exception)
            {
                SetStatus("The requirements for this programme could not be loaded. Please try again.", AmberColour, InkColour);
            }
        }

        /// <summary>
        /// Saves the preferred faculty and the selected programme on the in-progress session
        /// (AdvisorySession.PreferredFacultyID and SelectedProgrammeID).
        /// </summary>
        private void SaveSelection()
        {
            // TODO BLL: LearnerController.SaveProgrammeSelection(SessionId.Value, PreferredFacultyId.Value, SelectedProgrammeId.Value);
        }

        /// <summary>
        /// Sequence 1 — Check degree eligibility. The controller loads the learner and the
        /// programme, runs the RecommendationEngine, stores the RecommendationResult row
        /// (IsSelectedProgramme = 1) and returns the AdvisoryOutcome, which UI 6 displays with its
        /// explanation.
        /// </summary>
        /// <param name="results">The UI 6 screen that displays the outcome.</param>
        private void RunEligibilityCheck(EligibilityResultsForm results)
        {
            // TODO BLL: AdvisoryOutcome outcome = EligibilityController.CheckEligibility(LearnerId.Value, SelectedProgrammeId.Value);
            //           results.DisplayOutcome(outcome.Category, outcome.Explanation, outcome.OverallScore);
        }

        #endregion

        #region Selections

        /// <summary>Switches between browsing by faculty and searching by programme name.</summary>
        private void rbSearchMode_CheckedChanged(object sender, EventArgs e)
        {
            if (!((RadioButton)sender).Checked)
            {
                return;
            }

            bool searching = rbSearch.Checked;
            txtProgrammeSearch.Enabled = searching;
            lblSearchHint.Text = searching
                ? "Searches all faculties"
                : "Disabled while browsing by faculty";
            ClearSelectedProgramme();
            LoadProgrammes();
            if (searching)
            {
                txtProgrammeSearch.Focus();
            }
        }

        /// <summary>A new faculty becomes the preferred faculty and, while browsing, relists the programmes.</summary>
        private void cboFaculty_SelectionChangeCommitted(object sender, EventArgs e)
        {
            PreferredFacultyId = cboFaculty.SelectedValue is int ? (int?)(int)cboFaculty.SelectedValue : null;
            CheckFaculty();
            if (rbBrowse.Checked)
            {
                ClearSelectedProgramme();
                LoadProgrammes();
            }
        }

        private void cboFaculty_Leave(object sender, EventArgs e)
        {
            CheckFaculty();
        }

        /// <summary>Relists the programmes as the search text changes (search mode only).</summary>
        private void txtProgrammeSearch_TextChanged(object sender, EventArgs e)
        {
            if (rbSearch.Checked)
            {
                ClearSelectedProgramme();
                LoadProgrammes();
            }
        }

        /// <summary>The in-grid Select button chooses that programme and loads its requirements.</summary>
        private void dgvProgrammes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != colSelect.Index)
            {
                return;
            }

            DataGridViewRow row = dgvProgrammes.Rows[e.RowIndex];
            object id = row.Cells[colProgrammeId.Index].Value;
            if (!(id is int))
            {
                return;
            }

            SelectedProgrammeId = (int)id;
            SelectedProgrammeName = Convert.ToString(row.Cells[colProgrammeName.Index].Value);
            dgvProgrammes.Invalidate();
            LoadProgrammeDetails();
            SetStatus("Selected " + SelectedProgrammeName + ". Check eligibility when you are ready.", DeepNavy, Color.White);
        }

        /// <summary>Shows "Selected" on the chosen programme's button and "Select" on every other row.</summary>
        private void dgvProgrammes_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != colSelect.Index)
            {
                return;
            }

            object id = dgvProgrammes.Rows[e.RowIndex].Cells[colProgrammeId.Index].Value;
            bool chosen = SelectedProgrammeId.HasValue && id is int && (int)id == SelectedProgrammeId.Value;
            e.Value = chosen ? "✓ Selected" : "Select";
            e.CellStyle.BackColor = chosen ? GreenColour : NavyColour;
            e.CellStyle.SelectionBackColor = e.CellStyle.BackColor;
            e.FormattingApplied = true;
        }

        /// <summary>
        /// Clears the default first-row highlight after binding, so the blue selection never hides
        /// a coloured cell or suggests a choice the learner did not make.
        /// </summary>
        private void Grid_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            ((DataGridView)sender).ClearSelection();
        }

        /// <summary>Forgets the selected programme when the list changes.</summary>
        private void ClearSelectedProgramme()
        {
            SelectedProgrammeId = null;
            SelectedProgrammeName = null;
            LoadProgrammeDetails();
        }

        /// <summary>
        /// Faculty is required, but no selection can be invalid (2.3.5), so a missing faculty is an
        /// outstanding requirement: amber label and message, never red.
        /// </summary>
        private bool CheckFaculty()
        {
            bool chosen = cboFaculty.SelectedIndex >= 0;
            const string message = "Choose your preferred faculty.";
            lblFaculty.ForeColor = chosen ? InkColour : AmberText;
            cboFaculty.BackColor = chosen ? SystemColors.Window : AmberTint;
            lblFacultyError.Text = chosen ? string.Empty : "! " + message;
            lblFacultyError.Visible = !chosen;
            cboFaculty.AccessibleDescription = chosen ? null : message;
            return chosen;
        }

        #endregion

        #region Status band

        private void SetStatus(string message, Color back, Color fore)
        {
            pnlStatus.BackColor = back;
            lblStatus.ForeColor = fore;
            lblStatus.Text = message;
        }

        #endregion

        #region Buttons and navigation

        /// <summary>
        /// Check eligibility → saves the selection, runs Sequence 1 and shows the outcome on UI 6.
        /// The button stays active, so another programme can be checked after coming Back.
        /// </summary>
        private void btnCheckEligibility_Click(object sender, EventArgs e)
        {
            if (!CheckFaculty())
            {
                SetStatus("Choose a faculty before you check eligibility.", AmberColour, InkColour);
                cboFaculty.Focus();
                return;
            }

            if (!SelectedProgrammeId.HasValue)
            {
                SetStatus("Choose a programme with its Select button before you check eligibility.", AmberColour, InkColour);
                dgvProgrammes.Focus();
                return;
            }

            if (resultsForm == null || resultsForm.IsDisposed)
            {
                resultsForm = new EligibilityResultsForm(homeForm, this);
            }

            resultsForm.SessionId = SessionId;
            resultsForm.LearnerId = LearnerId;
            resultsForm.LearnerContext = LearnerContext;
            resultsForm.PreferredFacultyId = PreferredFacultyId;
            resultsForm.SelectedProgrammeId = SelectedProgrammeId;
            resultsForm.SelectedProgrammeName = SelectedProgrammeName;

            try
            {
                SaveSelection();
                RunEligibilityCheck(resultsForm);
            }
            catch (Exception)
            {
                SetStatus("Eligibility could not be checked for this programme. Please try again.", AmberColour, InkColour);
                return;
            }

            resultsForm.Show();
            Hide();
        }

        /// <summary>Back → UI 4, without writing anything.</summary>
        private void btnBack_Click(object sender, EventArgs e)
        {
            previousForm.Show();
            Hide();
        }

        /// <summary>Cancel leaves the session after confirmation.</summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            LeaveSession();
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

        /// <summary>Refreshes the context strip and restores the preferred faculty whenever the screen is shown.</summary>
        private void FacultySelectionForm_VisibleChanged(object sender, EventArgs e)
        {
            if (!Visible)
            {
                return;
            }

            lblContext.Text = LearnerContext ?? string.Empty;
            if (PreferredFacultyId.HasValue && cboFaculty.DataSource != null && cboFaculty.SelectedIndex < 0)
            {
                cboFaculty.SelectedValue = PreferredFacultyId.Value;
                LoadProgrammes();
            }

            SetStatus(ReadyStatus, DeepNavy, Color.White);
        }

        /// <summary>The caption-bar X asks first, because it closes Future Path.</summary>
        private void FacultySelectionForm_FormClosing(object sender, FormClosingEventArgs e)
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
        private void FacultySelectionForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!homeForm.Visible)
            {
                Application.Exit();
            }
        }

        #endregion
    }
}
