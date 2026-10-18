
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FuturePath.ApplicationLayer;

namespace FuturePath.PresentationLayer
{
    // Handles learner profile capture and validation
    public partial class MainForm
    {
        private const string ProfileGuidanceText =
            "Only your grade, school, province and advising year are required. Names and contact details are optional — " +
            "leave them blank and the session is stored without identifying you.";

        private const string ProfileStepStatus = "Step 1 of 6 · Complete the required fields marked *.";

        // Tracks whether the grade and province lists have been loaded
        private bool profileListsLoaded;

        // Keeps the last validation error visible when returning to the page
        private string profileNoticeError;

        #region Showing and resetting

        // Load profile details and display guidance or validation errors
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

        // Clear the profile fields for a new learner session
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

        // Show the current advisory session details
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

        // Load the available grades and provinces
        private void ProfileLoadLists()
        {
            try
            {
                ReferenceDataController referenceDataController = new ReferenceDataController();
                cboGrade.DisplayMember = "GradeDescription";
                cboGrade.ValueMember = "GradeLevelID";
                cboGrade.DataSource = referenceDataController.GetGradeLevels(false);

                // Load province options from reference data
                cboProvince.DisplayMember = "ParameterValue";
                cboProvince.ValueMember = "ParameterValue";
                cboProvince.DataSource = referenceDataController.GetProvinces();

                cboGrade.SelectedIndex = -1;
                cboProvince.SelectedIndex = -1;
                profileListsLoaded = true;
            }
            catch (Exception)
            {
                SetStatus("The grade and province lists could not be loaded. Please close and try again.", RedColour, Color.White);
            }
        }

        // Save the learner profile and advisory session
        private void ProfileSave()
        {
            int sessionId = 0;
            if (SessionId.HasValue)
            {
                sessionId = SessionId.Value;
            }

            LearnerController learnerController = new LearnerController();
            AdvisorySession session = learnerController.SaveProfile(sessionId, txtFirstName.Text.Trim(), txtSurname.Text.Trim(),
                (int)cboGrade.SelectedValue, txtSchool.Text.Trim(), cboProvince.Text,
                (short)numAdvisingYear.Value, txtEmail.Text.Trim(), txtMobile.Text.Trim());

            SessionId = session.SessionID;
            LearnerId = session.LearnerID;
            SessionNumber = session.SessionNumber;
            txtSessionNumber.Text = SessionNumber;
        }

        // Build the learner details shown in the status bar
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

        // Check that a grade is selected
        private bool ProfileCheckGrade()
        {
            bool valid = CheckField(cboGrade.SelectedIndex >= 0, cboGrade, lblGrade, lblGradeError,
                "Current grade: choose Grade 11 or Grade 12.");
            lblGradeHint.Visible = valid;
            return valid;
        }

        // Check that a school name is entered
        private bool ProfileCheckSchool()
        {
            return CheckField(txtSchool.Text.Trim().Length > 0, txtSchool, lblSchool, lblSchoolError,
                "School name: enter the name of your school.");
        }

        // Check that a province is selected
        private bool ProfileCheckProvince()
        {
            return CheckField(cboProvince.SelectedIndex >= 0, cboProvince, lblProvince, lblProvinceError,
                "Province: choose the province of your school.");
        }

        // Check that the advising year is within the allowed range
        private bool ProfileCheckAdvisingYear()
        {
            int year;
            bool valid = int.TryParse(numAdvisingYear.Text.Trim(), out year)
                         && year >= numAdvisingYear.Minimum && year <= numAdvisingYear.Maximum;
            return CheckField(valid, numAdvisingYear, lblAdvisingYear, lblAdvisingYearError,
                "Advising year: enter a year from " + numAdvisingYear.Minimum + " to " + numAdvisingYear.Maximum + ".");
        }

        // Check the optional email address format
        private bool ProfileCheckEmail()
        {
            string text = txtEmail.Text.Trim();
            bool valid = text.Length == 0 || Regex.IsMatch(text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return CheckField(valid, txtEmail, lblEmail, lblEmailError,
                "Email address: enter an address such as name@example.co.za, or leave it blank.");
        }

        // Check the optional mobile number format
        private bool ProfileCheckMobile()
        {
            string digits = Regex.Replace(txtMobile.Text, @"[\s\-]", string.Empty);
            bool valid = digits.Length == 0 || Regex.IsMatch(digits, @"^0\d{9}$");
            return CheckField(valid, txtMobile, lblMobile, lblMobileError,
                "Mobile number: use the format 0XX XXX XXXX, or leave it blank.");
        }

        // Validate the grade when leaving the field
        private void cboGrade_Leave(object sender, EventArgs e) { ProfileCheckGrade(); }

        // Validate the school name when leaving the field
        private void txtSchool_Leave(object sender, EventArgs e) { ProfileCheckSchool(); }

        // Validate the province when leaving the field
        private void cboProvince_Leave(object sender, EventArgs e) { ProfileCheckProvince(); }

        // Validate the advising year when leaving the field
        private void numAdvisingYear_Leave(object sender, EventArgs e) { ProfileCheckAdvisingYear(); }

        // Validate the email address when leaving the field
        private void txtEmail_Leave(object sender, EventArgs e) { ProfileCheckEmail(); }

        // Validate the mobile number when leaving the field
        private void txtMobile_Leave(object sender, EventArgs e) { ProfileCheckMobile(); }

        #endregion

        #region Notice

        // Display the default profile guidance
        private void ProfileShowGuidance()
        {
            ShowNotice(lblProfileNotice, ProfileGuidanceText, BlueTint, InkColour);
        }

        // Display the profile validation error
        private void ProfileShowNoticeError(string message)
        {
            ShowNotice(lblProfileNotice, message, RedTint, RedColour);
        }

        #endregion

        #region Button click events

        // Validate and save the profile before continuing
        private void btnProfileSave_Click(object sender, EventArgs e)
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
            catch (ArgumentException ex)
            {
                // Display validation errors returned by the controller
                ProfileShowNoticeError(ex.Message);
                SetStatus(ex.Message, RedColour, Color.White);
                return;
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

        // Return to the home screen
        private void btnProfileBack_Click(object sender, EventArgs e)
        {
            GoHome();
        }

        // Cancel the session and return home
        private void btnProfileCancel_Click(object sender, EventArgs e)
        {
            GoHome();
        }

        #endregion
    }
}
