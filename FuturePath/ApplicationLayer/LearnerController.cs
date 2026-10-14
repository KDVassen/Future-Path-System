using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
        private LearnerRepository learnerRepository;
        private AdvisorySessionRepository sessionRepository;
        private SubjectRepository subjectRepository;
        private LoginController loginController;
        #endregion

        #region Constructor
        /// <summary>Creates the controller and its repositories (no database work yet).</summary>
        public LearnerController()
        {
            learnerRepository = new LearnerRepository();
            sessionRepository = new AdvisorySessionRepository();
            subjectRepository = new SubjectRepository();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Saves the UI 2 profile. The first call (sessionID null) creates the Learner and an
        /// In-progress AdvisorySession; later calls update both. Returns the session (SessionID,
        /// LearnerID, SessionNumber). Throws ArgumentException if the profile is invalid.
        /// </summary>
        public AdvisorySession SaveProfile(int? sessionID, string firstName, string lastName, int gradeLevelID, string schoolName,
                                           string province, short advisingYear, string contactEmail, string contactNumber)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// The NSC achievement level for a mark from the NSC_LEVEL_1..7 lower bounds,
        /// e.g. 72 gives "6  70-79%". Returns "" for an invalid mark.
        /// </summary>
        public string GetAchievementLevel(decimal mark)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Replaces the session's subjects and marks (UI 3 Save and continue). Validates every
        /// entry again: at least one subject, active subjects, marks 0-100 with one decimal, no
        /// duplicates. Earlier results for the session are cleared. Throws ArgumentException if invalid.
        /// </summary>
        public void SaveSubjects(int sessionID, IList<KeyValuePair<int, decimal>> subjectMarks)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Faculty and programme (UI 5)
        /// <summary>Stores the preferred faculty and selected programme (both must exist and be active).</summary>
        public void SaveProgrammeSelection(int sessionID, int preferredFacultyID, int selectedProgrammeID)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Staff search (UI 10.1a)
        /// <summary>UI 10.1a learner search (Career Advisor or Administrator). See LearnerRepository.FindLearners for the columns.</summary>
        public DataTable FindLearners(int actorUserID, string surname, string firstName, int? gradeLevelID, string schoolName, string province)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
