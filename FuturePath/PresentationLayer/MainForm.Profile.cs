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
    /// UI 2 — Learner Profile Capture (step 1 of 6; spec 2.3.2).
    /// Names and contact details are optional. Grade, school, province and advising year are
    /// required. Session details are system-supplied and read-only. On <b>Save and continue</b>,
    /// a missing or invalid value turns its label and control red, the notice panel turns red and
    /// names the field, and UI 2.1a (Invalid Profile Data) is raised.
    /// </summary>
    public partial class MainForm
    {
        private const string ProfileGuidanceText =
            "Only your grade, school, province and advising year are required. Names and contact details are optional — " +
            "leave them blank and the session is stored without identifying you.";

        private const string ProfileStepStatus = "Step 1 of 6 · Complete the required fields marked *.";

        /// <summary>True once the grade and province lists have been bound for this session.</summary>
        private bool profileListsLoaded;

        /// <summary>
        /// The red notice text left standing by the last failed <b>Save and continue</b>, or null when the blue
        /// guidance shows. Kept so the red notice is still there if the learner moves away and comes back.
        /// </summary>
        private string profileNoticeError;

        #region Showing and resetting

        /// <summary>
        /// Called by ShowPage each time UI 2 is shown: binds the lists the first time, shows the system-supplied
        /// session details, and shows the blue guidance notice unless a red error notice is standing.
        /// </summary>
        private void ProfileShown()
        {
            SetStatus(ProfileStepStatus, DeepNavy, Color.White);
            ProfileShowSessionDetails();
            if (profileNoticeError == null)
            {
                ProfileShowGuidance();
            }
            else
            {
                ProfileShowNoticeError(profileNoticeError);
            }

            if (!profileListsLoaded)
            {
                ProfileLoadLists();
            }
        }

        /// <summary>
        /// Returns UI 2 to a brand-new session: every field empty, advising year back to this year, the
        /// system-supplied session details for a new session, no red field states, the lists to be bound again
        /// and the blue guidance notice the next time UI 2 is shown. No learner may see the previous learner's
        /// details (spec 2.1).
        /// </summary>
        private void ProfileReset()
        {
            txtFirstName.Text = string.Empty;
            txtSurname.Text = string.Empty;
            cboGrade.SelectedIndex = -1;
            txtSchool.Text = string.Empty;
            cboProvince.SelectedIndex = -1;

            int year = DateTime.Today.Year;
            numAdvisingYear.Minimum = year - 1;
            numAdvisingYear.Maximum = year + 1;
            numAdvisingYear.Value = year;
            numAdvisingYear.Text = year.ToString(CultureInfo.InvariantCulture);

            txtEmail.Text = string.Empty;
            txtMobile.Text = string.Empty;

            txtDateStarted.Text = string.Empty;
            ProfileShowSessionDetails();

            CheckField(true, cboGrade, lblGrade, lblGradeError, null);
            lblGradeHint.Visible = true;
            CheckField(true, txtSchool, lblSchool, lblSchoolError, null);
            CheckField(true, cboProvince, lblProvince, lblProvinceError, null);
            CheckField(true, numAdvisingYear, lblAdvisingYear, lblAdvisingYearError, null);
            CheckField(true, txtEmail, lblEmail, lblEmailError, null);
            CheckField(true, txtMobile, lblMobile, lblMobileError, null);

            profileNoticeError = null;
            profileListsLoaded = false;
        }

        /// <summary>
        /// Fills the read-only, system-supplied session details: the session number once the BLL has assigned
        /// one ("Assigned on save" until then), the date the session started (today, set when it starts),
        /// "Self-service" as the capturer and "In progress" as the status (decisions.md D3).
        /// </summary>
        private void ProfileShowSessionDetails()
        {
            txtSessionNumber.Text = string.IsNullOrEmpty(SessionNumber) ? "Assigned on save" : SessionNumber;
            if (txtDateStarted.Text.Length == 0)
            {
                txtDateStarted.Text = DateTime.Today.ToString("dd MMMM yyyy", CultureInfo.CurrentCulture);
            }

            txtCapturedBy.Text = "Self-service";
            txtSessionStatus.Text = "In progress";
        }

        #endregion

        #region Business Logic Layer hooks

        /// <summary>Binds the grade and province lists to stored data (never literal Items).</summary>
        private void ProfileLoadLists()
        {
            try
            {
                // TODO BLL: cboGrade.DataSource = ReferenceDataController.GetGradeLevels();
                //           cboGrade.DisplayMember = "GradeDescription"; cboGrade.ValueMember = "GradeLevelID";
                // TODO BLL: cboProvince.DataSource = ReferenceDataController.GetProvinces();
                //           Province is value-bound (2.3.2) but has no table yet: proposed SystemParameter PROVINCE_1..9.
                cboGrade.SelectedIndex = -1;
                cboProvince.SelectedIndex = -1;
                profileListsLoaded = true;
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
        private void ProfileSave()
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

        /// <summary>
        /// The learner context strip text shown on UI 3 onward: name (or "Learner (name not given)") · grade ·
        /// school · province, and the session number once the BLL has assigned one.
        /// </summary>
        private string ProfileContextLine()
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

        #endregion

        #region Field validation (validated again in the BLL on save)

        /// <summary>
        /// Current grade is required: one of the bound grade levels must be chosen. The grade hint shares the
        /// space under the grade box with the field message, so it is hidden while the message shows.
        /// </summary>
        private bool ProfileCheckGrade()
        {
            bool valid = CheckField(cboGrade.SelectedIndex >= 0, cboGrade, lblGrade, lblGradeError,
                "Current grade: choose Grade 11 or Grade 12.");
            lblGradeHint.Visible = valid;
            return valid;
        }

        /// <summary>School name is required.</summary>
        private bool ProfileCheckSchool()
        {
            return CheckField(txtSchool.Text.Trim().Length > 0, txtSchool, lblSchool, lblSchoolError,
                "School name: enter the name of your school.");
        }

        /// <summary>Province is required: one of the bound provinces must be chosen.</summary>
        private bool ProfileCheckProvince()
        {
            return CheckField(cboProvince.SelectedIndex >= 0, cboProvince, lblProvince, lblProvinceError,
                "Province: choose the province of your school.");
        }

        /// <summary>
        /// Advising year is required and must lie in the allowed range. Reads the typed text, because the
        /// NumericUpDown clamps it on losing focus.
        /// </summary>
        private bool ProfileCheckAdvisingYear()
        {
            int year;
            bool valid = int.TryParse(numAdvisingYear.Text.Trim(), out year)
                         && year >= numAdvisingYear.Minimum && year <= numAdvisingYear.Maximum;
            return CheckField(valid, numAdvisingYear, lblAdvisingYear, lblAdvisingYearError,
                "Advising year: enter a year from " + numAdvisingYear.Minimum + " to " + numAdvisingYear.Maximum + ".");
        }

        /// <summary>Email address is optional; when given it must look like name@example.co.za.</summary>
        private bool ProfileCheckEmail()
        {
            string text = txtEmail.Text.Trim();
            bool valid = text.Length == 0 || Regex.IsMatch(text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return CheckField(valid, txtEmail, lblEmail, lblEmailError,
                "Email address: enter an address such as name@example.co.za, or leave it blank.");
        }

        /// <summary>Mobile number is optional; when given it must be ten digits starting with 0 (spaces and dashes allowed).</summary>
        private bool ProfileCheckMobile()
        {
            string digits = Regex.Replace(txtMobile.Text, @"[\s\-]", string.Empty);
            bool valid = digits.Length == 0 || Regex.IsMatch(digits, @"^0\d{9}$");
            return CheckField(valid, txtMobile, lblMobile, lblMobileError,
                "Mobile number: use the format 0XX XXX XXXX, or leave it blank.");
        }

        /// <summary>Re-checks the grade when the learner leaves it.</summary>
        private void cboGrade_Leave(object sender, EventArgs e) { ProfileCheckGrade(); }

        /// <summary>Re-checks the school name when the learner leaves it.</summary>
        private void txtSchool_Leave(object sender, EventArgs e) { ProfileCheckSchool(); }

        /// <summary>Re-checks the province when the learner leaves it.</summary>
        private void cboProvince_Leave(object sender, EventArgs e) { ProfileCheckProvince(); }

        /// <summary>Re-checks the advising year (as typed) when the learner leaves it.</summary>
        private void numAdvisingYear_Leave(object sender, EventArgs e) { ProfileCheckAdvisingYear(); }

        /// <summary>Re-checks the email address when the learner leaves it.</summary>
        private void txtEmail_Leave(object sender, EventArgs e) { ProfileCheckEmail(); }

        /// <summary>Re-checks the mobile number when the learner leaves it.</summary>
        private void txtMobile_Leave(object sender, EventArgs e) { ProfileCheckMobile(); }

        #endregion

        #region Notice panel

        /// <summary>Shows the blue guidance notice (what is required and what is optional).</summary>
        private void ProfileShowGuidance()
        {
            ShowNotice(ProfileGuidanceText, BlueTint, BlueColour, InkColour);
        }

        /// <summary>Shows the red notice naming the first field to correct.</summary>
        private void ProfileShowNoticeError(string message)
        {
            ShowNotice(message, RedTint, RedColour, RedColour);
        }

        #endregion

        #region Save and continue

        /// <summary>
        /// Save and continue → validate, save, then UI 3; or UI 2.1a (Invalid profile data) if anything is
        /// invalid, with the focus returned to the first field to correct.
        /// </summary>
        private void ProfilePrimary()
        {
            var problems = new List<KeyValuePair<Control, string>>();
            if (!ProfileCheckGrade()) problems.Add(new KeyValuePair<Control, string>(cboGrade, lblGradeError.Text.TrimStart('⚠', ' ')));
            if (!ProfileCheckSchool()) problems.Add(new KeyValuePair<Control, string>(txtSchool, lblSchoolError.Text.TrimStart('⚠', ' ')));
            if (!ProfileCheckProvince()) problems.Add(new KeyValuePair<Control, string>(cboProvince, lblProvinceError.Text.TrimStart('⚠', ' ')));
            if (!ProfileCheckAdvisingYear()) problems.Add(new KeyValuePair<Control, string>(numAdvisingYear, lblAdvisingYearError.Text.TrimStart('⚠', ' ')));
            if (!ProfileCheckEmail()) problems.Add(new KeyValuePair<Control, string>(txtEmail, lblEmailError.Text.TrimStart('⚠', ' ')));
            if (!ProfileCheckMobile()) problems.Add(new KeyValuePair<Control, string>(txtMobile, lblMobileError.Text.TrimStart('⚠', ' ')));

            if (problems.Count > 0)
            {
                profileNoticeError = problems[0].Value + (problems.Count > 1 ? "  (" + problems.Count + " fields to correct)" : string.Empty);
                ProfileShowNoticeError(profileNoticeError);
                SetStatus(problems.Count + (problems.Count == 1 ? " entry" : " entries") + " to correct before you can continue.", RedColour, Color.White);
                ShowValidation("Invalid profile data", problems.Select(p => p.Value).ToList(), problems[0].Key);
                return;
            }

            try
            {
                ProfileSave();
            }
            catch (Exception)
            {
                SetStatus("Your profile could not be saved. Please try again.", RedColour, Color.White);
                return;
            }

            profileNoticeError = null;
            ProfileShowGuidance();
            LearnerContext = ProfileContextLine();
            GoToStep(tabSubjects, 2);
        }

        #endregion
    }
}
