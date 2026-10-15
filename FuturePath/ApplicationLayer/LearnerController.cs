using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using FuturePath.DatabaseLayer;

namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// The learner's steps before the eligibility check: the profile (UI 2), subjects and marks
    /// (UI 3) and the faculty / programme choice (UI 5), plus the staff learner search (UI 10.1a).
    /// The session is written as it goes (decisions.md D3). Every input is validated here again,
    /// even though the form validated it, so bad data cannot reach the database.
    /// </summary>
    public class LearnerController
    {
        #region Data Members
        // Longest values the Learner table accepts (database.md)
        private const int MaximumNameLength = 50;
        private const int MaximumSchoolLength = 100;
        private const int MaximumEmailLength = 100;

        // name@domain.tld: no spaces, one @, and a dot after the @
        private const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        // A South African mobile number: 0 and nine more digits (spaces and dashes removed first)
        private const string MobilePattern = @"^0\d{9}$";

        private LearnerRepository learnerRepository;
        private AdvisorySessionRepository sessionRepository;
        private SubjectRepository subjectRepository;
        private DegreeProgrammeRepository programmeRepository;
        private FacultyRepository facultyRepository;
        private LoginController loginController;
        #endregion

        #region Constructor
        /// <summary>Creates the controller and its repositories (no database work yet).</summary>
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
        /// <summary>
        /// Checks the UI 2 profile and returns one message per problem (empty when valid): grade
        /// 1 or 2; school required (max 100); province one of the PROVINCE_n values; advising year
        /// within one year of this year; email and mobile (0XX XXX XXXX) format-checked only if given.
        /// </summary>
        public Collection<string> ValidateProfile(int gradeLevelID, string schoolName, string province, short advisingYear, string contactEmail, string contactNumber)
        {
            Collection<string> messages = new Collection<string>();

            // Grade: the GradeLevel enum matches the GradeLevel table (1 = Grade 11, 2 = Grade 12)
            if (!Enum.IsDefined(typeof(GradeLevel), gradeLevelID))
            {
                messages.Add("Choose the learner's current grade (Grade 11 or Grade 12).");
            }

            // School: required free text
            if (string.IsNullOrWhiteSpace(schoolName))
            {
                messages.Add("Enter the school name.");
            }
            else if (schoolName.Trim().Length > MaximumSchoolLength)
            {
                messages.Add("Enter a school name of at most " + MaximumSchoolLength + " characters.");
            }

            // Province: one of the PROVINCE_n system parameters
            if (FindProvince(province) == null)
            {
                messages.Add("Choose a province from the list.");
            }

            // Advising year: this year, or one year either side
            int thisYear = DateTime.Today.Year;
            if (advisingYear < thisYear - 1 || advisingYear > thisYear + 1)
            {
                messages.Add("Choose an advising year from " + (thisYear - 1) + " to " + (thisYear + 1) + ".");
            }

            // Optional contact details: format-checked only when given
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

        /// <summary>
        /// Saves the UI 2 profile. The first call (sessionID 0) creates the Learner and an
        /// In-progress AdvisorySession; later calls update both. Returns the session (SessionID,
        /// LearnerID, SessionNumber). Throws ArgumentException if the profile is invalid, if the
        /// session does not exist, or if the session has already been completed.
        /// </summary>
        public AdvisorySession SaveProfile(int sessionID, string firstName, string lastName, int gradeLevelID, string schoolName,
                                           string province, short advisingYear, string contactEmail, string contactNumber)
        {
            // Validate again (the form already did), including the name lengths
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
                // First save: a new learner (class diagram constructor) and a new In-progress session
                Learner newLearner = new Learner(Clean(firstName), Clean(lastName), grade, schoolName.Trim(), storedProvince, storedEmail, storedNumber);
                learnerRepository.Save(newLearner);
                AdvisorySession newSession = new AdvisorySession(newLearner.LearnerID, grade, advisingYear);
                sessionRepository.Save(newSession);
                return newSession;
            }

            // Later saves: update the learner and the session
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
        /// <summary>
        /// Checks one subject before it is added to the UI 3 grid: a subject chosen and active,
        /// not already captured (capturedSubjectIDs), and a mark 0-100 with at most one decimal
        /// (letters fail). Returns one message per problem.
        /// </summary>
        public Collection<string> ValidateSubjectEntry(int subjectID, string markText, IList<int> capturedSubjectIDs)
        {
            Collection<string> messages = new Collection<string>();
            string subjectName = "the subject";

            // Subject: chosen from the list and still offered
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
                // Field-combination control: no subject twice in one session
                if (capturedSubjectIDs != null && capturedSubjectIDs.Contains(subjectID))
                {
                    messages.Add(subject.SubjectName + " has already been captured in this session.");
                }
            }

            // Mark: 0 to 100 with at most one decimal place (letters and signs give -1)
            if (LearnerSubject.ParseMark(markText) < 0)
            {
                messages.Add("Enter a mark for " + subjectName + " from 0 to 100, with at most one decimal place.");
            }
            return messages;
        }

        /// <summary>
        /// The NSC achievement level for a mark from the NSC_LEVEL_1..7 lower bounds,
        /// e.g. 72 gives "6  70-79%" (with an en dash) and 85 gives "7  80-100%". Returns "" for
        /// an invalid mark, or if the NSC_LEVEL parameters are missing or not numbers.
        /// </summary>
        public string GetAchievementLevel(decimal mark)
        {
            if (!LearnerSubject.IsValidMark(mark))
            {
                return "";
            }

            // The lower bound of each level, from NSC_LEVEL_1 upwards
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

            // The highest level whose lower bound the mark reaches
            for (int i = lowerBounds.Count - 1; i >= 0; i--)
            {
                if (mark >= lowerBounds[i])
                {
                    int level = i + 1;
                    // The band ends just below the next level (the top level ends at 100)
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

        /// <summary>
        /// Replaces the session's subjects and marks (UI 3 Save and continue). Validates every
        /// entry again: at least one subject, active subjects, marks 0-100 with one decimal, no
        /// duplicates. Earlier results for the session are cleared. Throws ArgumentException if invalid.
        /// </summary>
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

            // Build the session's subjects through Learner.AddSubject (class diagram addSubject),
            // which refuses a duplicate subject and a mark outside 0-100 with a friendly message
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
        /// <summary>
        /// Stores the preferred faculty and selected programme. The programme must exist, be active
        /// and belong to an active faculty; the preferred faculty must be an active faculty. The
        /// preferred faculty may differ from the programme's faculty (that difference is what the
        /// faculty bonus measures). Throws ArgumentException if not valid.
        /// </summary>
        public void SaveProgrammeSelection(int sessionID, int preferredFacultyID, int selectedProgrammeID)
        {
            if (sessionRepository.FindById(sessionID) == null)
            {
                throw new ArgumentException("We could not find your advisory session. Start again from step 1.");
            }

            // The programme: exists, active, and under an active faculty
            DegreeProgramme programme = programmeRepository.FindById(selectedProgrammeID);
            if (programme == null || !programme.IsActive || !IsActiveFaculty(programme.FacultyID))
            {
                throw new ArgumentException("Choose an active degree programme from the list.");
            }

            // The preferred faculty: exists and active
            if (!IsActiveFaculty(preferredFacultyID))
            {
                throw new ArgumentException("Choose a faculty from the list.");
            }
            sessionRepository.SaveProgrammeSelection(sessionID, preferredFacultyID, selectedProgrammeID);
        }
        #endregion

        #region Staff search (UI 10.1a)
        /// <summary>
        /// UI 10.1a learner search (Career Advisor or Administrator; throws
        /// UnauthorizedAccessException otherwise). A grade of 0 means every grade.
        /// See LearnerRepository.FindLearners for the columns.
        /// </summary>
        public DataTable FindLearners(int actorUserID, string surname, string firstName, int gradeLevelID, string schoolName, string province)
        {
            loginController.CheckStaff(actorUserID);
            return learnerRepository.FindLearners(surname, firstName, gradeLevelID, schoolName, province);
        }
        #endregion

        #region Helpers
        /// <summary>The province as stored in the PROVINCE_n parameters (matched without case), or null if it is not one of them.</summary>
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

        /// <summary>True if the faculty exists and is active.</summary>
        private bool IsActiveFaculty(int facultyID)
        {
            Faculty faculty = facultyRepository.FindById(facultyID);
            return faculty != null && faculty.IsActive;
        }

        /// <summary>A mobile number without its spaces and dashes.</summary>
        private static string DigitsOnly(string number)
        {
            return Clean(number).Replace(" ", "").Replace("-", "");
        }

        /// <summary>A valid mobile number stored as "0XX XXX XXXX", or "" when none was given.</summary>
        private static string FormatMobile(string number)
        {
            string digits = DigitsOnly(number);
            if (digits.Length != 10)
            {
                return "";
            }
            return digits.Substring(0, 3) + " " + digits.Substring(3, 3) + " " + digits.Substring(6);
        }

        /// <summary>Trimmed text, or "" for null.</summary>
        private static string Clean(string text)
        {
            if (text == null)
            {
                return "";
            }
            return text.Trim();
        }

        /// <summary>Formats a number without trailing zeros, the same on any regional setting.</summary>
        private static string FormatNumber(decimal value)
        {
            return value.ToString("0.#", CultureInfo.InvariantCulture);
        }
        #endregion
    }
}
