
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using FuturePath.DatabaseLayer;

namespace FuturePath.ApplicationLayer
{
    // Handles learner profiles, subjects, marks and programme selections
    // Validates learner information before saving it to the database
    public class LearnerController
    {
        #region Data Members
        // Maximum lengths allowed for learner details
        private const int MaximumNameLength = 50;
        private const int MaximumSchoolLength = 100;
        private const int MaximumEmailLength = 100;

        // Patterns used to validate email addresses and South African mobile numbers
        private const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        private const string MobilePattern = @"^0\d{9}$";

        private LearnerRepository learnerRepository;
        private AdvisorySessionRepository sessionRepository;
        private SubjectRepository subjectRepository;
        private DegreeProgrammeRepository programmeRepository;
        private FacultyRepository facultyRepository;
        private LoginController loginController;
        #endregion

        #region Constructor
        // Initialise the repositories and login controller
        public LearnerController()
        {
            learnerRepository = new LearnerRepository();
            sessionRepository = new AdvisorySessionRepository();
            subjectRepository = new SubjectRepository();
            programmeRepository = new DegreeProgrammeRepository();
            facultyRepository = new FacultyRepository();
            loginController = new LoginController();
        }
        #endregion

        #region Profile (UI 2)
        // Validate the learner's grade, school, province and advising year
        // Check optional email and mobile details when provided
        public Collection<string> ValidateProfile(int gradeLevelID, string schoolName, string province, short advisingYear, string contactEmail, string contactNumber)
        {
            Collection<string> messages = new Collection<string>();

            // Check that the grade is Grade 11 or Grade 12
            if (!Enum.IsDefined(typeof(GradeLevel), gradeLevelID))
            {
                messages.Add("Choose the learner's current grade (Grade 11 or Grade 12).");
            }

            // Check that a valid school name was entered
            if (string.IsNullOrWhiteSpace(schoolName))
            {
                messages.Add("Enter the school name.");
            }
            else if (schoolName.Trim().Length > MaximumSchoolLength)
            {
                messages.Add("Enter a school name of at most " + MaximumSchoolLength + " characters.");
            }

            // Check that the province exists in the system settings
            if (FindProvince(province) == null)
            {
                messages.Add("Choose a province from the list.");
            }

            // Allow the current advising year or one year before or after
            int thisYear = DateTime.Today.Year;
            if (advisingYear < thisYear - 1 || advisingYear > thisYear + 1)
            {
                messages.Add("Choose an advising year from " + (thisYear - 1) + " to " + (thisYear + 1) + ".");
            }

            // Validate contact details only when they have been entered
            if (!string.IsNullOrWhiteSpace(contactEmail))
            {
                string email = contactEmail.Trim();
                if (email.Length > MaximumEmailLength || !Regex.IsMatch(email, EmailPattern))
                {
                    messages.Add("Enter an email address like name@example.com, or leave it blank.");
                }
            }
            if (!string.IsNullOrWhiteSpace(contactNumber))
            {
                if (!Regex.IsMatch(DigitsOnly(contactNumber), MobilePattern))
                {
                    messages.Add("Enter a mobile number in the format 0XX XXX XXXX, or leave it blank.");
                }
            }
            return messages;
        }

        // Save a learner's profile and advisory session
        // Create new records for the first save or update existing records
        public AdvisorySession SaveProfile(int sessionID, string firstName, string lastName, int gradeLevelID, string schoolName,
                                           string province, short advisingYear, string contactEmail, string contactNumber)
        {
            // Validate the profile and check the name lengths before saving
            Collection<string> messages = ValidateProfile(gradeLevelID, schoolName, province, advisingYear, contactEmail, contactNumber);
            if (!string.IsNullOrWhiteSpace(firstName) && firstName.Trim().Length > MaximumNameLength)
            {
                messages.Add("Enter a first name of at most " + MaximumNameLength + " characters.");
            }
            if (!string.IsNullOrWhiteSpace(lastName) && lastName.Trim().Length > MaximumNameLength)
            {
                messages.Add("Enter a surname of at most " + MaximumNameLength + " characters.");
            }
            if (messages.Count > 0)
            {
                throw new ArgumentException(messages[0]);
            }

            GradeLevel grade = (GradeLevel)gradeLevelID;
            string storedProvince = FindProvince(province);
            string storedNumber = FormatMobile(contactNumber);
            string storedEmail = Clean(contactEmail);

            if (sessionID == 0)
            {
                // Create a new learner and advisory session
                Learner newLearner = new Learner(Clean(firstName), Clean(lastName), grade, schoolName.Trim(), storedProvince, storedEmail, storedNumber);
                learnerRepository.Save(newLearner);
                AdvisorySession newSession = new AdvisorySession(newLearner.LearnerID, grade, advisingYear);
                sessionRepository.Save(newSession);
                return newSession;
            }

            // Update the learner and session if they have not been completed
            AdvisorySession session = sessionRepository.FindById(sessionID);
            if (session == null)
            {
                throw new ArgumentException("We could not find your advisory session. Start again from step 1.");
            }
            if (session.IsCompleted)
            {
                throw new ArgumentException("This advisory session has been completed, so its profile can no longer be changed.");
            }
            Learner learner = learnerRepository.FindBySessionId(sessionID);
            if (learner == null)
            {
                throw new ArgumentException("We could not find your learner profile. Start again from step 1.");
            }
            learner.FirstName = Clean(firstName);
            learner.LastName = Clean(lastName);
            learner.SchoolName = schoolName.Trim();
            learner.Province = storedProvince;
            learner.ContactEmail = storedEmail;
            learner.ContactNumber = storedNumber;
            learnerRepository.Save(learner);

            session.Grade = grade;
            session.AdvisingYear = advisingYear;
            sessionRepository.Save(session);
            return session;
        }
        #endregion

        #region Subjects and marks (UI 3)
        // Validate a selected subject and its mark
        // Prevent duplicate subjects and marks outside the allowed range
        public Collection<string> ValidateSubjectEntry(int subjectID, string markText, IList<int> capturedSubjectIDs)
        {
            Collection<string> messages = new Collection<string>();
            string subjectName = "the subject";

            // Check that the selected subject exists and is active
            Subject subject = null;
            if (subjectID > 0)
            {
                subject = subjectRepository.FindById(subjectID);
            }
            if (subject == null || !subject.IsActive)
            {
                messages.Add("Choose a subject from the list.");
            }
            else
            {
                subjectName = subject.SubjectName;

                // Prevent the same subject from being captured twice
                if (capturedSubjectIDs != null && capturedSubjectIDs.Contains(subjectID))
                {
                    messages.Add(subject.SubjectName + " has already been captured in this session.");
                }
            }

            // Check that the mark is between 0 and 100 with one decimal place at most
            if (LearnerSubject.ParseMark(markText) < 0)
            {
                messages.Add("Enter a mark for " + subjectName + " from 0 to 100, with at most one decimal place.");
            }
            return messages;
        }

        // Calculate the NSC achievement level for a subject mark
        // Use the level boundaries stored in the system settings
        public string GetAchievementLevel(decimal mark)
        {
            if (!LearnerSubject.IsValidMark(mark))
            {
                return "";
            }

            // Load the lower mark boundary for each achievement level
            Collection<string> values = SystemSettings.GetNumberedValues(SystemSettings.NscLevelPrefix);
            List<decimal> lowerBounds = new List<decimal>();
            foreach (string value in values)
            {
                decimal bound;
                if (!decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out bound))
                {
                    return "";
                }
                lowerBounds.Add(bound);
            }

            // Find the highest achievement level reached by the learner's mark
            for (int i = lowerBounds.Count - 1; i >= 0; i--)
            {
                if (mark >= lowerBounds[i])
                {
                    int level = i + 1;

                    // Use the next level's boundary to calculate the upper mark limit
                    decimal upperBound = LearnerSubject.MaximumMark;
                    if (i < lowerBounds.Count - 1)
                    {
                        upperBound = lowerBounds[i + 1] - 1;
                    }
                    return level.ToString(CultureInfo.InvariantCulture) + "  " + FormatNumber(lowerBounds[i]) + "–" + FormatNumber(upperBound) + "%";
                }
            }
            return "";
        }

        // Validate and save all subjects and marks for an advisory session
        // Replace previously captured subjects with the updated list
        public void SaveSubjects(int sessionID, IList<KeyValuePair<int, decimal>> subjectMarks)
        {
            if (sessionRepository.FindById(sessionID) == null)
            {
                throw new ArgumentException("We could not find your advisory session. Start again from step 1.");
            }
            if (subjectMarks == null || subjectMarks.Count == 0)
            {
                throw new ArgumentException("Capture at least one subject and mark before you continue.");
            }

            // Use the Learner class to validate subjects, marks and duplicates
            Learner learner = new Learner();
            learner.CurrentSessionID = sessionID;
            foreach (KeyValuePair<int, decimal> entry in subjectMarks)
            {
                Subject subject = subjectRepository.FindById(entry.Key);
                if (subject == null || !subject.IsActive)
                {
                    throw new ArgumentException("Choose each subject from the list. One of the subjects is not offered.");
                }
                learner.AddSubject(subject, entry.Value);
            }
            learnerRepository.SaveSubjects(sessionID, learner.GetSubjects());
        }
        #endregion

        #region Faculty and programme (UI 5)
        // Save the learner's preferred faculty and selected degree programme
        // Check that both are valid and active before saving
        public void SaveProgrammeSelection(int sessionID, int preferredFacultyID, int selectedProgrammeID)
        {
            if (sessionRepository.FindById(sessionID) == null)
            {
                throw new ArgumentException("We could not find your advisory session. Start again from step 1.");
            }

            // Check that the programme and its faculty are active
            DegreeProgramme programme = programmeRepository.FindById(selectedProgrammeID);
            if (programme == null || !programme.IsActive || !IsActiveFaculty(programme.FacultyID))
            {
                throw new ArgumentException("Choose an active degree programme from the list.");
            }

            // Check that the preferred faculty exists and is active
            if (!IsActiveFaculty(preferredFacultyID))
            {
                throw new ArgumentException("Choose a faculty from the list.");
            }
            sessionRepository.SaveProgrammeSelection(sessionID, preferredFacultyID, selectedProgrammeID);
        }
        #endregion

        #region Staff search (UI 10.1a)
        // Search for learner profiles using the selected filters
        // Only authorised staff members can perform this search
        public DataTable FindLearners(int actorUserID, string surname, string firstName, int gradeLevelID, string schoolName, string province)
        {
            loginController.CheckStaff(actorUserID);
            return learnerRepository.FindLearners(surname, firstName, gradeLevelID, schoolName, province);
        }
        #endregion

        #region Helpers
        // Find a valid province from the system settings
        // Return null if the province is not recognised
        private static string FindProvince(string province)
        {
            if (string.IsNullOrWhiteSpace(province))
            {
                return null;
            }
            foreach (string value in SystemSettings.GetNumberedValues(SystemSettings.ProvincePrefix))
            {
                if (string.Equals(value, province.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return value;
                }
            }
            return null;
        }

        // Check that a faculty exists and is active
        private bool IsActiveFaculty(int facultyID)
        {
            Faculty faculty = facultyRepository.FindById(facultyID);
            return faculty != null && faculty.IsActive;
        }

        // Remove spaces and dashes from a mobile number
        private static string DigitsOnly(string number)
        {
            return Clean(number).Replace(" ", "").Replace("-", "");
        }

        // Format a valid mobile number as 0XX XXX XXXX
        // Return an empty string if the number is not ten digits long
        private static string FormatMobile(string number)
        {
            string digits = DigitsOnly(number);
            if (digits.Length != 10)
            {
                return "";
            }
            return digits.Substring(0, 3) + " " + digits.Substring(3, 3) + " " + digits.Substring(6);
        }

        // Remove extra spaces or return an empty string if the text is null
        private static string Clean(string text)
        {
            if (text == null)
            {
                return "";
            }
            return text.Trim();
        }

        // Format a number without unnecessary decimal zeros
        private static string FormatNumber(decimal value)
        {
            return value.ToString("0.#", CultureInfo.InvariantCulture);
        }
        #endregion
    }
}
