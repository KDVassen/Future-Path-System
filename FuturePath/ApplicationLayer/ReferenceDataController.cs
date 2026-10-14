using System;
using System.Collections.ObjectModel;
using System.Data;
using FuturePath.DatabaseLayer;

namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// Reference data: the lists every screen binds to (grades, provinces, outcomes, faculties,
    /// programmes, subjects, requirements), so no lookup is hard-coded in a form, and the
    /// Administrator maintenance of faculties, programmes and requirements (UI 13.1, 13.2).
    /// Every maintenance method checks the Administrator role first. "All" rows (for report
    /// filters) use 0 or "" as their value; forms treat that as "no filter".
    /// </summary>
    public class ReferenceDataController
    {
        #region Data Members
        private FacultyRepository facultyRepository;
        private DegreeProgrammeRepository programmeRepository;
        private SubjectRepository subjectRepository;
        private GradeLevelRepository gradeLevelRepository;
        private LoginController loginController;
        private Collection<Faculty> faculties;
        private Collection<DegreeProgramme> programmes;
        #endregion

        #region Properties
        /// <summary>The faculties read by the last GetFaculties call.</summary>
        public Collection<Faculty> AllFaculties
        {
            get { return faculties; }
        }

        /// <summary>The programmes read by the last GetProgrammes call.</summary>
        public Collection<DegreeProgramme> AllProgrammes
        {
            get { return programmes; }
        }
        #endregion

        #region Constructor
        /// <summary>Creates the controller and its repositories (no database work yet).</summary>
        public ReferenceDataController()
        {
            facultyRepository = new FacultyRepository();
            programmeRepository = new DegreeProgrammeRepository();
            subjectRepository = new SubjectRepository();
            gradeLevelRepository = new GradeLevelRepository();
            loginController = new LoginController();
            faculties = new Collection<Faculty>();
            programmes = new Collection<DegreeProgramme>();
        }
        #endregion

        #region Lookups
        /// <summary>GradeLevelID, GradeDescription, MarkBasis; with includeAllRow an "All grades" row (GradeLevelID 0) first.</summary>
        public DataTable GetGradeLevels(bool includeAllRow)
        {
            throw new NotImplementedException();
        }

        /// <summary>The provinces from PROVINCE_n parameters: ParameterCode, ParameterValue (display ParameterValue).</summary>
        public DataTable GetProvinces()
        {
            throw new NotImplementedException();
        }

        /// <summary>OutcomeCode, OutcomeName, OutcomeOrder for the four outcomes; with includeAllRow an "All outcomes" row (OutcomeCode "") first.</summary>
        public DataTable GetOutcomes(bool includeAllRow)
        {
            throw new NotImplementedException();
        }

        /// <summary>RequirementType ("Required" / "Recommended") and Description, for the UI 13.2 combo.</summary>
        public DataTable GetRequirementTypes()
        {
            throw new NotImplementedException();
        }

        /// <summary>A whole-number system parameter (e.g. MIN_SUBJECT_COUNT), or null if it is missing or not a number.</summary>
        public int? GetParameterInt(string code)
        {
            throw new NotImplementedException();
        }

        /// <summary>Faculties ordered by name; with includeAllRow an "All faculties" item (FacultyID 0) first.</summary>
        public Collection<Faculty> GetFaculties(bool includeInactive, bool includeAllRow)
        {
            throw new NotImplementedException();
        }

        /// <summary>Active faculties (UI 5, UI 6.2b).</summary>
        public Collection<Faculty> GetActiveFaculties()
        {
            throw new NotImplementedException();
        }

        /// <summary>Programmes with FacultyName; facultyID null means every faculty.</summary>
        public Collection<DegreeProgramme> GetProgrammes(int? facultyID, bool includeInactive)
        {
            throw new NotImplementedException();
        }

        /// <summary>Active programmes of an active faculty (UI 5 browse mode).</summary>
        public Collection<DegreeProgramme> GetActiveProgrammes(int facultyID)
        {
            throw new NotImplementedException();
        }

        /// <summary>Active programmes whose name contains the text (UI 5 search mode). Blank text returns none.</summary>
        public Collection<DegreeProgramme> SearchActiveProgrammes(string text)
        {
            throw new NotImplementedException();
        }

        /// <summary>Subjects ordered by name.</summary>
        public Collection<Subject> GetSubjects(bool includeInactive)
        {
            throw new NotImplementedException();
        }

        /// <summary>Active subjects (UI 3).</summary>
        public Collection<Subject> GetActiveSubjects()
        {
            throw new NotImplementedException();
        }

        /// <summary>Active subjects with a "None" item (SubjectID 0) first, for the UI 13.2 accepted-alternative combo.</summary>
        public Collection<Subject> GetAlternativeSubjects()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// UI 13.2 grid: RequirementID (DBNull on the synthetic row), RequirementType, SubjectID,
        /// SubjectName, MinimumMark, AlternativeSubjectID, AlternativeSubjectName,
        /// AlternativeMinimumMark, AdvisoryNote, IsActive, plus one "Minimum average" row read
        /// from DegreeProgramme.MinimumOverallAverage (only if the programme sets one).
        /// </summary>
        public DataTable GetRequirements(int programmeID, bool includeInactive)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// UI 5 requirements panel: SubjectName, MinimumMark, RequirementType for the active
        /// requirements, plus an "Overall average" row if the programme sets a minimum average.
        /// </summary>
        public DataTable GetActiveRequirements(int programmeID)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Faculty maintenance (UI 13.1)
        /// <summary>Checks a faculty: name required, at most 60 characters, not already used. Returns one message per problem.</summary>
        public Collection<string> ValidateFaculty(int? facultyID, string facultyName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds (facultyID null) or renames a faculty (Administrator only). isActive null keeps the
        /// current value. Returns the FacultyID.
        /// </summary>
        public int SaveFaculty(int actorUserID, int? facultyID, string facultyName, bool? isActive)
        {
            throw new NotImplementedException();
        }

        /// <summary>Deactivates a faculty (Administrator only). Its programmes then drop out of learner lists.</summary>
        public void DeactivateFaculty(int actorUserID, int facultyID)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Programme maintenance (UI 13.1)
        /// <summary>
        /// Checks a programme: existing faculty, name required (max 100) and not already used,
        /// minimum average 0-100 when given. Returns one message per problem.
        /// </summary>
        public Collection<string> ValidateProgramme(int? programmeID, int facultyID, string programmeName, decimal? minimumOverallAverage)
        {
            throw new NotImplementedException();
        }

        /// <summary>Adds (programmeID null) or updates a programme (Administrator only). Returns the ProgrammeID.</summary>
        public int SaveProgramme(int actorUserID, int? programmeID, int facultyID, string programmeName, decimal? minimumOverallAverage, bool isActive)
        {
            throw new NotImplementedException();
        }

        /// <summary>Deactivates a programme (Administrator only). It then never appears in recommendations or reports.</summary>
        public void DeactivateProgramme(int actorUserID, int programmeID)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Requirement maintenance (UI 13.2)
        /// <summary>
        /// Checks a requirement: valid type, existing subject, one requirement per subject per
        /// programme, alternative different from the subject, marks 0-100, note at most 255
        /// characters. Returns one message per problem.
        /// </summary>
        public Collection<string> ValidateRequirement(int? requirementID, int programmeID, string requirementType, int subjectID,
                                                      int? alternativeSubjectID, decimal? minimumMark, decimal? alternativeMinimumMark, string advisoryNote)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds (requirementID null) or updates a requirement (Administrator only). An
        /// alternativeSubjectID of null or 0 means no alternative. Returns the RequirementID.
        /// Changes apply to new advisory sessions only.
        /// </summary>
        public int SaveRequirement(int actorUserID, int? requirementID, int programmeID, string requirementType, int subjectID,
                                   int? alternativeSubjectID, decimal? minimumMark, decimal? alternativeMinimumMark, bool isActive, string advisoryNote)
        {
            throw new NotImplementedException();
        }

        /// <summary>Deactivates a requirement (Administrator only).</summary>
        public void DeactivateRequirement(int actorUserID, int requirementID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Saves the "Minimum average" row of UI 13.2 into DegreeProgramme.MinimumOverallAverage
        /// (Administrator only). null or 0 clears it.
        /// </summary>
        public void SaveMinimumAverage(int actorUserID, int programmeID, decimal? minimumOverallAverage)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
