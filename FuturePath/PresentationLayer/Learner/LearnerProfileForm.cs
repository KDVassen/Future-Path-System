using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FuturePath.PresentationLayer
{
    /// <summary>
    /// UI 2 — Learner Profile Capture (wide, step 1 of 6; spec 2.3.2).
    /// Names and contact details are optional. Grade, school, province and advising year are
    /// required. Session details are system-supplied and read-only. On <b>Save and continue</b>,
    /// a missing or invalid value turns its label and control red, the advisory panel turns red and
    /// names the field, and UI 2.1a (Invalid Profile Data) is raised.
    /// </summary>
    public partial class LearnerProfileForm : Form
    {
        // Colours used at runtime (spec 2.2.6). Everything else is set in the Designer.
        private static readonly Color InkColour = Color.FromArgb(27, 31, 36);
        private static readonly Color RedColour = Color.FromArgb(164, 38, 44);
        private static readonly Color RedTint = Color.FromArgb(250, 234, 235);
        private static readonly Color BlueColour = Color.FromArgb(31, 95, 168);
        private static readonly Color BlueTint = Color.FromArgb(232, 238, 246);
        private static readonly Color DeepNavy = Color.FromArgb(11, 31, 58);

        private const string GuidanceText =
            "Only your grade, school, province and advising year are required. Names and contact details are optional — " +
            "leave them blank and the session is stored without identifying you.";

        private readonly Form homeForm;
        private SubjectsMarksForm subjectsForm;

        /// <summary>Creates the learner profile screen for a new self-service session.</summary>
        /// <param name="homeForm">UI 1, shown again when the session is left.</param>
        public LearnerProfileForm(Form homeForm)
        {
            InitializeComponent();
            this.homeForm = homeForm;

            numAdvisingYear.Minimum = DateTime.Today.Year - 1;
            numAdvisingYear.Maximum = DateTime.Today.Year + 1;
            numAdvisingYear.Value = DateTime.Today.Year;
            txtDateStarted.Text = DateTime.Today.ToString("dd MMMM yyyy", CultureInfo.CurrentCulture);
            txtCapturedBy.Text = "Self-service";
            txtSessionStatus.Text = "In progress";
            txtSessionNumber.Text = "Assigned on save";

            ShowGuidance();
            SetStatus("Step 1 of 6 · Complete the required fields marked *.", DeepNavy, Color.White);
            LoadLists();
        }

        #region Session values handed to the next screens

        /// <summary>AdvisorySession.SessionID, returned by the BLL on the first save.</summary>
        public int? SessionId { get; private set; }

        /// <summary>Learner.LearnerID, returned by the BLL on the first save.</summary>
        public int? LearnerId { get; private set; }

        /// <summary>The display session number yyyy-NNNN, built by the BLL (not stored).</summary>
        public string SessionNumber { get; private set; }

        /// <summary>The learner context strip text used by every later step.</summary>
        public string LearnerContext
        {
            get
            {
                string name = (txtFirstName.Text.Trim() + " " + txtSurname.Text.Trim()).Trim();
                string line = name.Length == 0 ? "Learner (name not given)" : name;
                line += " · " + cboGrade.Text + " · " + txtSchool.Text.Trim() + " · " + cboProvince.Text;
                if (!string.IsNullOrEmpty(SessionNumber))
                {
                    line += "     Session " + SessionNumber;
                }

                return line;
            }
        }

        #endregion

        #region Business Logic Layer hooks

        /// <summary>Binds the grade and province lists to stored data (never literal Items).</summary>
        private void LoadLists()
        {
            try
            {
                // TODO BLL: cboGrade.DataSource = ReferenceDataController.GetGradeLevels();
                //           cboGrade.DisplayMember = "GradeDescription"; cboGrade.ValueMember = "GradeLevelID";
                // TODO BLL: cboProvince.DataSource = ReferenceDataController.GetProvinces();
                //           Province is value-bound (2.3.2) but has no table yet: proposed SystemParameter PROVINCE_1..9.
                cboGrade.SelectedIndex = -1;
                cboProvince.SelectedIndex = -1;
            }
            catch (Exception)
            {
                SetStatus("The grade and province lists could not be loaded. Please close and try again.", RedColour, Color.White);
            }
        }

        /// <summary>
        /// Saves the learner and opens (or updates) the "In progress" advisory session
        /// (decisions.md D3). The BLL validates again and returns the IDs and the session number.
        /// </summary>
        private void SaveProfile()
        {
            // TODO BLL: var saved = LearnerController.SaveProfile(SessionId, txtFirstName.Text.Trim(), txtSurname.Text.Trim(),
            //               (int)cboGrade.SelectedValue, txtSchool.Text.Trim(), cboProvince.Text,
            //               (short)numAdvisingYear.Value, txtEmail.Text.Trim(), txtMobile.Text.Trim());
            //           SessionId = saved.SessionId; LearnerId = saved.LearnerId; SessionNumber = saved.SessionNumber;
            if (!string.IsNullOrEmpty(SessionNumber))
            {
                txtSessionNumber.Text = SessionNumber;
            }
        }

        #endregion

        #region Field validation (validated again in the BLL on save)

        private bool CheckGrade()
        {
            return CheckField(cboGrade.SelectedIndex >= 0, cboGrade, lblGrade, lblGradeError,
                "Current grade: choose Grade 11 or Grade 12.");
        }

        private bool CheckSchool()
        {
            return CheckField(txtSchool.Text.Trim().Length > 0, txtSchool, lblSchool, lblSchoolError,
                "School name: enter the name of your school.");
        }

        private bool CheckProvince()
        {
            return CheckField(cboProvince.SelectedIndex >= 0, cboProvince, lblProvince, lblProvinceError,
                "Province: choose the province of your school.");
        }

        private bool CheckAdvisingYear()
        {
            // Read the typed text before the NumericUpDown clamps it on losing focus.
            int year;
            bool valid = int.TryParse(numAdvisingYear.Text.Trim(), out year)
                         && year >= numAdvisingYear.Minimum && year <= numAdvisingYear.Maximum;
            return CheckField(valid, numAdvisingYear, lblAdvisingYear, lblAdvisingYearError,
                "Advising year: enter a year from " + numAdvisingYear.Minimum + " to " + numAdvisingYear.Maximum + ".");
        }

        private bool CheckEmail()
        {
            string text = txtEmail.Text.Trim();
            bool valid = text.Length == 0 || Regex.IsMatch(text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return CheckField(valid, txtEmail, lblEmail, lblEmailError,
                "Email address: enter an address such as name@example.co.za, or leave it blank.");
        }

        private bool CheckMobile()
        {
            string digits = Regex.Replace(txtMobile.Text, @"[\s\-]", string.Empty);
            bool valid = digits.Length == 0 || Regex.IsMatch(digits, @"^0\d{9}$");
            return CheckField(valid, txtMobile, lblMobile, lblMobileError,
                "Mobile number: use the format 0XX XXX XXXX, or leave it blank.");
        }

        /// <summary>
        /// Shows or clears the field message (spec 2.2.7): red text with a warning glyph under the
        /// control, a red tint on the control and a red label.
        /// </summary>
        private static bool CheckField(bool valid, Control input, Label fieldLabel, Label messageLabel, string message)
        {
            input.BackColor = valid ? SystemColors.Window : RedTint;
            fieldLabel.ForeColor = valid ? InkColour : RedColour;
            messageLabel.Text = valid ? string.Empty : "⚠ " + message;
            messageLabel.Visible = !valid;
            input.AccessibleDescription = valid ? null : message;
            return valid;
        }

        private void cboGrade_Leave(object sender, EventArgs e) { CheckGrade(); }

        private void txtSchool_Leave(object sender, EventArgs e) { CheckSchool(); }

        private void cboProvince_Leave(object sender, EventArgs e) { CheckProvince(); }

        private void numAdvisingYear_Leave(object sender, EventArgs e) { CheckAdvisingYear(); }

        private void txtEmail_Leave(object sender, EventArgs e) { CheckEmail(); }

        private void txtMobile_Leave(object sender, EventArgs e) { CheckMobile(); }

        #endregion

        #region Notice panel and status band

        private void ShowGuidance()
        {
            pnlNotice.BackColor = BlueTint;
            pnlNoticeBar.BackColor = BlueColour;
            lblNotice.ForeColor = InkColour;
            lblNotice.Text = GuidanceText;
        }

        private void ShowNoticeError(string message)
        {
            pnlNotice.BackColor = RedTint;
            pnlNoticeBar.BackColor = RedColour;
            lblNotice.ForeColor = RedColour;
            lblNotice.Text = message;
        }

        private void SetStatus(string message, Color back, Color fore)
        {
            pnlStatus.BackColor = back;
            lblStatus.ForeColor = fore;
            lblStatus.Text = message;
        }

        #endregion

        #region Buttons and navigation

        /// <summary>Save and continue → validate, save, then UI 3; or UI 2.1a if anything is invalid.</summary>
        private void btnSaveContinue_Click(object sender, EventArgs e)
        {
            var problems = new List<KeyValuePair<Control, string>>();
            if (!CheckGrade()) problems.Add(new KeyValuePair<Control, string>(cboGrade, lblGradeError.Text.TrimStart('⚠', ' ')));
            if (!CheckSchool()) problems.Add(new KeyValuePair<Control, string>(txtSchool, lblSchoolError.Text.TrimStart('⚠', ' ')));
            if (!CheckProvince()) problems.Add(new KeyValuePair<Control, string>(cboProvince, lblProvinceError.Text.TrimStart('⚠', ' ')));
            if (!CheckAdvisingYear()) problems.Add(new KeyValuePair<Control, string>(numAdvisingYear, lblAdvisingYearError.Text.TrimStart('⚠', ' ')));
            if (!CheckEmail()) problems.Add(new KeyValuePair<Control, string>(txtEmail, lblEmailError.Text.TrimStart('⚠', ' ')));
            if (!CheckMobile()) problems.Add(new KeyValuePair<Control, string>(txtMobile, lblMobileError.Text.TrimStart('⚠', ' ')));

            if (problems.Count > 0)
            {
                ShowNoticeError(problems[0].Value + (problems.Count > 1 ? "  (" + problems.Count + " fields to correct)" : string.Empty));
                SetStatus(problems.Count + (problems.Count == 1 ? " entry" : " entries") + " to correct before you can continue.", RedColour, Color.White);
                using (var dialog = new ValidationDialog("Invalid profile data", problems.Select(p => p.Value).ToList()))
                {
                    dialog.ShowDialog(this);
                }

                problems[0].Key.Focus();
                return;
            }

            try
            {
                SaveProfile();
            }
            catch (Exception)
            {
                SetStatus("Your profile could not be saved. Please try again.", RedColour, Color.White);
                return;
            }

            ShowGuidance();
            if (subjectsForm == null || subjectsForm.IsDisposed)
            {
                subjectsForm = new SubjectsMarksForm(homeForm, this);
            }

            subjectsForm.SessionId = SessionId;
            subjectsForm.LearnerId = LearnerId;
            subjectsForm.LearnerContext = LearnerContext;
            subjectsForm.Show();
            Hide();
        }

        /// <summary>Back from step 1 goes to UI 1, after confirmation.</summary>
        private void btnBack_Click(object sender, EventArgs e)
        {
            LeaveSession();
        }

        /// <summary>Cancel abandons the session after confirmation.</summary>
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

        private void LearnerProfileForm_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                SetStatus("Step 1 of 6 · Complete the required fields marked *.", DeepNavy, Color.White);
            }
        }

        /// <summary>The caption-bar X asks first, because it closes Future Path.</summary>
        private void LearnerProfileForm_FormClosing(object sender, FormClosingEventArgs e)
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
        private void LearnerProfileForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!homeForm.Visible)
            {
                Application.Exit();
            }
        }

        #endregion
    }
}
