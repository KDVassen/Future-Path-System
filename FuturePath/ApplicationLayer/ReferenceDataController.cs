using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
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
        private const int MaximumFacultyNameLength = 60;
        private const int MaximumProgrammeNameLength = 100;
        private const int MaximumAdvisoryNoteLength = 255;

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
            DataTable table = gradeLevelRepository.GetGradeLevels();
            if (includeAllRow)
            {
                DataRow allRow = table.NewRow();
                allRow["GradeLevelID"] = 0;
                allRow["GradeDescription"] = "All grades";
                allRow["MarkBasis"] = "";
                table.Rows.InsertAt(allRow, 0);
            }
            return table;
        }

        /// <summary>The provinces from PROVINCE_n parameters: ParameterCode, ParameterValue (display ParameterValue).</summary>
        public DataTable GetProvinces()
        {
            return SystemSettings.GetNumberedParameters(SystemSettings.ProvincePrefix);
        }

        /// <summary>OutcomeCode, OutcomeName, OutcomeOrder for the four outcomes; with includeAllRow an "All outcomes" row (OutcomeCode "") first.</summary>
        public DataTable GetOutcomes(bool includeAllRow)
        {
            DataTable table = new DataTable("Outcomes");
            table.Columns.Add("OutcomeCode", typeof(string));
            table.Columns.Add("OutcomeName", typeof(string));
            table.Columns.Add("OutcomeOrder", typeof(int));
            if (includeAllRow)
            {
                table.Rows.Add("", "All outcomes", 0);
            }
            // The four outcomes come from AdvisoryOutcome, so codes and wording are never typed twice.
            foreach (AdvisoryOutcome.OutcomeType type in AdvisoryOutcome.GetOutcomeTypes())
            {
                table.Rows.Add(AdvisoryOutcome.GetCode(type), AdvisoryOutcome.GetCategory(type), AdvisoryOutcome.GetDisplayOrder(type));
            }
            return table;
        }

        /// <summary>RequirementType ("Required" / "Recommended") and Description, for the UI 13.2 combo.</summary>
        public DataTable GetRequirementTypes()
        {
            DataTable table = new DataTable("RequirementTypes");
            table.Columns.Add("RequirementType", typeof(string));
            table.Columns.Add("Description", typeof(string));
            table.Rows.Add(ProgrammeRequirement.TypeRequired, ProgrammeRequirement.TypeRequired + ": decides eligibility");
            table.Rows.Add(ProgrammeRequirement.TypeRecommended, ProgrammeRequirement.TypeRecommended + ": advice only");
            return table;
        }

        /// <summary>A whole-number system parameter (e.g. MIN_SUBJECT_COUNT), or 0 if it is missing or not a number.</summary>
        public int GetParameterInt(string code)
        {
            if (string.IsNullOrWhiteSpace(code) || !SystemSettings.Contains(code.Trim()))
            {
                return 0;
            }
            int value;
            if (!int.TryParse(SystemSettings.GetText(code.Trim()), NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
            {
                return 0;
            }
            return value;
        }

        /// <summary>Faculties ordered by name; with includeAllRow an "All faculties" item (FacultyID 0) first.</summary>
        public Collection<Faculty> GetFaculties(bool includeInactive, bool includeAllRow)
        {
            faculties = new Collection<Faculty>();
            if (includeAllRow)
            {
                faculties.Add(new Faculty(0, "All faculties", true));
            }
            foreach (Faculty faculty in facultyRepository.GetFaculties(includeInactive))
            {
                faculties.Add(faculty);
            }
            return faculties;
        }

        /// <summary>Active faculties (UI 5, UI 6.2b).</summary>
        public Collection<Faculty> GetActiveFaculties()
        {
            return GetFaculties(false, false);
        }

        /// <summary>
        /// Programmes with FacultyName, ordered by name; a facultyID of 0 means every faculty. Without
        /// includeInactive, inactive programmes and programmes under an inactive faculty are left out.
        /// </summary>
        public Collection<DegreeProgramme> GetProgrammes(int facultyID, bool includeInactive)
        {
            programmes = programmeRepository.GetProgrammes(facultyID, includeInactive);
            return programmes;
        }

        /// <summary>Active programmes of an active faculty (UI 5 browse mode).</summary>
        public Collection<DegreeProgramme> GetActiveProgrammes(int facultyID)
        {
            return GetProgrammes(facultyID, false);
        }

        /// <summary>Active programmes whose name contains the text (UI 5 search mode). Blank text returns none.</summary>
        public Collection<DegreeProgramme> SearchActiveProgrammes(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                programmes = new Collection<DegreeProgramme>();
            }
            else
            {
                programmes = programmeRepository.SearchActiveProgrammes(text.Trim());
            }
            return programmes;
        }

        /// <summary>Subjects ordered by name.</summary>
        public Collection<Subject> GetSubjects(bool includeInactive)
        {
            return subjectRepository.GetSubjects(includeInactive);
        }

        /// <summary>Active subjects (UI 3).</summary>
        public Collection<Subject> GetActiveSubjects()
        {
            return GetSubjects(false);
        }

        /// <summary>Active subjects with a "None" item (SubjectID 0) first, for the UI 13.2 accepted-alternative combo.</summary>
        public Collection<Subject> GetAlternativeSubjects()
        {
            Collection<Subject> subjects = new Collection<Subject>();
            subjects.Add(new Subject(0, "None", "", true));
            foreach (Subject subject in GetSubjects(false))
            {
                subjects.Add(subject);
            }
            return subjects;
        }

        /// <summary>
        /// UI 13.2 grid: RequirementID (DBNull on the synthetic row), RequirementType, SubjectID,
        /// SubjectName, MinimumMark, AlternativeSubjectID, AlternativeSubjectName,
        /// AlternativeMinimumMark, AdvisoryNote, IsActive, plus one "Minimum average" row (last)
        /// read from DegreeProgramme.MinimumOverallAverage (only if the programme sets one).
        /// </summary>
        public DataTable GetRequirements(int programmeID, bool includeInactive)
        {
            DataTable table = new DataTable("Requirements");
            table.Columns.Add("RequirementID", typeof(int));
            table.Columns.Add("RequirementType", typeof(string));
            table.Columns.Add("SubjectID", typeof(int));
            table.Columns.Add("SubjectName", typeof(string));
            table.Columns.Add("MinimumMark", typeof(decimal));
            table.Columns.Add("AlternativeSubjectID", typeof(int));
            table.Columns.Add("AlternativeSubjectName", typeof(string));
            table.Columns.Add("AlternativeMinimumMark", typeof(decimal));
            table.Columns.Add("AdvisoryNote", typeof(string));
            table.Columns.Add("IsActive", typeof(bool));

            foreach (ProgrammeRequirement requirement in programmeRepository.GetRequirements(programmeID, includeInactive))
            {
                table.Rows.Add(requirement.RequirementID, requirement.RequirementType, requirement.SubjectID, requirement.SubjectName,
                               ValueOrDBNull(requirement.MinimumMark), ValueOrDBNull(requirement.AlternativeSubjectID),
                               requirement.AlternativeSubjectName, ValueOrDBNull(requirement.AlternativeMinimumMark),
                               requirement.AdvisoryNote, requirement.IsActive);
            }

            // The "Minimum average" row is DegreeProgramme.MinimumOverallAverage (decisions.md §3), not a requirement row.
            DegreeProgramme programme = programmeRepository.FindById(programmeID);
            if (programme != null && programme.MinimumOverallAverage.HasValue)
            {
                table.Rows.Add(DBNull.Value, ProgrammeRequirement.TypeRequired, DBNull.Value, "Minimum average",
                               programme.MinimumOverallAverage.Value, DBNull.Value, "", DBNull.Value, "", true);
            }
            return table;
        }

        /// <summary>
        /// UI 5 requirements panel: SubjectName, MinimumMark, RequirementType for the active
        /// requirements, plus an "Overall average" row (last) if the programme sets a minimum average.
        /// </summary>
        public DataTable GetActiveRequirements(int programmeID)
        {
            DataTable table = new DataTable("ActiveRequirements");
            table.Columns.Add("SubjectName", typeof(string));
            table.Columns.Add("MinimumMark", typeof(decimal));
            table.Columns.Add("RequirementType", typeof(string));

            foreach (ProgrammeRequirement requirement in programmeRepository.GetRequirements(programmeID, false))
            {
                table.Rows.Add(requirement.SubjectName, ValueOrDBNull(requirement.MinimumMark), requirement.RequirementType);
            }

            DegreeProgramme programme = programmeRepository.FindById(programmeID);
            if (programme != null && programme.MinimumOverallAverage.HasValue)
            {
                table.Rows.Add("Overall average", programme.MinimumOverallAverage.Value, ProgrammeRequirement.TypeRequired);
            }
            return table;
        }
        #endregion

        #region Faculty maintenance (UI 13.1)
        /// <summary>Checks a faculty: name required, at most 60 characters, not already used. Returns one message per problem.</summary>
        public Collection<string> ValidateFaculty(int facultyID, string facultyName)
        {
            Collection<string> problems = new Collection<string>();
            if (string.IsNullOrWhiteSpace(facultyName))
            {
                problems.Add("Enter a faculty name.");
            }
            else if (facultyName.Trim().Length > MaximumFacultyNameLength)
            {
                problems.Add("Shorten the faculty name to " + MaximumFacultyNameLength + " characters or fewer.");
            }
            else if (facultyRepository.FacultyNameExists(facultyName, facultyID))
            {
                problems.Add("A faculty called " + facultyName.Trim() + " already exists. Enter a different name.");
            }
            return problems;
        }

        /// <summary>
        /// Adds (facultyID 0) or updates a faculty's name and active flag (Administrator only).
        /// Returns the FacultyID. Throws ArgumentException if invalid, UnauthorizedAccessException
        /// if not an Administrator.
        /// </summary>
        public int SaveFaculty(int actorUserID, int facultyID, string facultyName, bool isActive)
        {
            loginController.CheckAdministrator(actorUserID);
            Collection<string> problems = ValidateFaculty(facultyID, facultyName);
            if (problems.Count > 0)
            {
                throw new ArgumentException(problems[0]);
            }

            Faculty faculty;
            DB.DBOperation operation;
            if (facultyID == 0)
            {
                faculty = new Faculty(0, facultyName.Trim(), isActive);
                operation = DB.DBOperation.Add;
            }
            else
            {
                faculty = FindFaculty(facultyID);
                faculty.FacultyName = facultyName.Trim();
                faculty.IsActive = isActive;
                operation = DB.DBOperation.Edit;
            }

            DataMaintenance(faculty, operation);
            return FinalizeChanges(faculty, operation);
        }

        /// <summary>Deactivates a faculty (Administrator only). Its programmes then drop out of learner lists.</summary>
        public void DeactivateFaculty(int actorUserID, int facultyID)
        {
            loginController.CheckAdministrator(actorUserID);
            Faculty faculty = FindFaculty(facultyID);
            faculty.IsActive = false;
            DataMaintenance(faculty, DB.DBOperation.Edit);
            FinalizeChanges(faculty, DB.DBOperation.Edit);
        }
        #endregion

        #region Programme maintenance (UI 13.1)
        /// <summary>
        /// Checks a programme: existing faculty, name required (max 100) and not already used,
        /// minimum average 0-100 with at most two decimals (0 = no minimum). Returns one message per problem.
        /// </summary>
        public Collection<string> ValidateProgramme(int programmeID, int facultyID, string programmeName, decimal minimumOverallAverage)
        {
            Collection<string> problems = new Collection<string>();
            if (facultyID <= 0 || facultyRepository.FindById(facultyID) == null)
            {
                problems.Add("Choose the programme's faculty from the list.");
            }

            if (string.IsNullOrWhiteSpace(programmeName))
            {
                problems.Add("Enter the programme name, e.g. BSc Computer Science.");
            }
            else if (programmeName.Trim().Length > MaximumProgrammeNameLength)
            {
                problems.Add("Shorten the programme name to " + MaximumProgrammeNameLength + " characters or fewer.");
            }
            else if (programmeRepository.ProgrammeNameExists(programmeName, programmeID))
            {
                problems.Add("A programme called " + programmeName.Trim() + " already exists. Enter a different name.");
            }

            if (!IsValidPercentage(minimumOverallAverage))
            {
                problems.Add("Enter a minimum overall average from 0 to 100, with at most two decimal places.");
            }
            return problems;
        }

        /// <summary>
        /// Adds (programmeID 0) or updates a programme (Administrator only). A minimum average of 0
        /// means the programme sets none. Returns the ProgrammeID.
        /// </summary>
        public int SaveProgramme(int actorUserID, int programmeID, int facultyID, string programmeName, decimal minimumOverallAverage, bool isActive)
        {
            loginController.CheckAdministrator(actorUserID);
            Collection<string> problems = ValidateProgramme(programmeID, facultyID, programmeName, minimumOverallAverage);
            if (problems.Count > 0)
            {
                throw new ArgumentException(problems[0]);
            }

            DegreeProgramme programme;
            DB.DBOperation operation;
            if (programmeID == 0)
            {
                programme = new DegreeProgramme();
                operation = DB.DBOperation.Add;
            }
            else
            {
                programme = FindProgramme(programmeID);
                operation = DB.DBOperation.Edit;
            }
            programme.FacultyID = facultyID;
            programme.ProgrammeName = programmeName.Trim();
            programme.MinimumOverallAverage = NoMinimumIfZero(minimumOverallAverage);
            programme.IsActive = isActive;

            DataMaintenance(programme, operation);
            return FinalizeChanges(programme, operation);
        }

        /// <summary>Deactivates a programme (Administrator only). It then never appears in recommendations or reports.</summary>
        public void DeactivateProgramme(int actorUserID, int programmeID)
        {
            loginController.CheckAdministrator(actorUserID);
            DegreeProgramme programme = FindProgramme(programmeID);
            programme.IsActive = false;
            DataMaintenance(programme, DB.DBOperation.Edit);
            FinalizeChanges(programme, DB.DBOperation.Edit);
        }
        #endregion

        #region Requirement maintenance (UI 13.2)
        /// <summary>
        /// Checks a requirement: existing programme, valid type, existing subject, one requirement
        /// per subject per programme, alternative (0 means none) existing and different from the
        /// subject, marks 0-100 with at most two decimals, note at most 255 characters.
        /// Returns one message per problem.
        /// </summary>
        public Collection<string> ValidateRequirement(int requirementID, int programmeID, string requirementType, int subjectID,
                                                      int alternativeSubjectID, decimal minimumMark, decimal alternativeMinimumMark, string advisoryNote)
        {
            Collection<string> problems = new Collection<string>();
            if (programmeID <= 0 || programmeRepository.FindById(programmeID) == null)
            {
                problems.Add("Choose a degree programme before you save a requirement.");
            }

            if (!ProgrammeRequirement.IsValidType(requirementType))
            {
                problems.Add("Choose a requirement type: " + ProgrammeRequirement.TypeRequired + " or " + ProgrammeRequirement.TypeRecommended + ".");
            }

            Subject subject = null;
            if (subjectID > 0)
            {
                subject = subjectRepository.FindById(subjectID);
            }
            if (subject == null)
            {
                problems.Add("Choose a subject from the list.");
            }
            else if (programmeID > 0 && programmeRepository.RequirementExists(programmeID, subjectID, requirementID))
            {
                problems.Add("This programme already has a requirement for " + subject.SubjectName + ". Edit that requirement instead.");
            }

            if (alternativeSubjectID > 0)
            {
                if (alternativeSubjectID == subjectID)
                {
                    problems.Add("Choose an accepted alternative that is different from the subject, or None.");
                }
                else if (subjectRepository.FindById(alternativeSubjectID) == null)
                {
                    problems.Add("Choose an accepted alternative from the list, or None.");
                }
            }

            if (!IsValidPercentage(minimumMark))
            {
                problems.Add("Enter a minimum mark from 0 to 100, with at most two decimal places.");
            }
            if (!IsValidPercentage(alternativeMinimumMark))
            {
                problems.Add("Enter the alternative's minimum mark from 0 to 100, with at most two decimal places.");
            }

            if (advisoryNote != null && advisoryNote.Trim().Length > MaximumAdvisoryNoteLength)
            {
                problems.Add("Shorten the advisory note to " + MaximumAdvisoryNoteLength + " characters or fewer.");
            }
            return problems;
        }

        /// <summary>
        /// Adds (requirementID 0) or updates a requirement (Administrator only). 0 means "not set":
        /// an alternativeSubjectID of 0 is no alternative, a minimum mark of 0 is no minimum, and an
        /// alternative minimum of 0 means the main minimum applies. Returns the RequirementID.
        /// Changes apply to new advisory sessions only.
        /// </summary>
        public int SaveRequirement(int actorUserID, int requirementID, int programmeID, string requirementType, int subjectID,
                                   int alternativeSubjectID, decimal minimumMark, decimal alternativeMinimumMark, bool isActive, string advisoryNote)
        {
            loginController.CheckAdministrator(actorUserID);
            Collection<string> problems = ValidateRequirement(requirementID, programmeID, requirementType, subjectID, alternativeSubjectID,
                                                              minimumMark, alternativeMinimumMark, advisoryNote);
            if (problems.Count > 0)
            {
                throw new ArgumentException(problems[0]);
            }

            ProgrammeRequirement requirement;
            DB.DBOperation operation;
            if (requirementID == 0)
            {
                requirement = new ProgrammeRequirement();
                operation = DB.DBOperation.Add;
            }
            else
            {
                requirement = FindRequirement(requirementID);
                operation = DB.DBOperation.Edit;
            }
            requirement.ProgrammeID = programmeID;
            requirement.RequirementType = requirementType;
            requirement.SubjectID = subjectID;
            requirement.MinimumMark = NoMinimumIfZero(minimumMark);
            // No alternative (0) is stored as NULL, and then there is no alternative minimum either
            requirement.AlternativeSubjectID = null;
            requirement.AlternativeMinimumMark = null;
            if (alternativeSubjectID > 0)
            {
                requirement.AlternativeSubjectID = alternativeSubjectID;
                requirement.AlternativeMinimumMark = NoMinimumIfZero(alternativeMinimumMark);
            }
            requirement.AdvisoryNote = "";
            if (advisoryNote != null)
            {
                requirement.AdvisoryNote = advisoryNote.Trim();
            }
            requirement.IsActive = isActive;

            DataMaintenance(requirement, operation);
            return FinalizeChanges(requirement, operation);
        }

        /// <summary>Deactivates a requirement (Administrator only).</summary>
        public void DeactivateRequirement(int actorUserID, int requirementID)
        {
            loginController.CheckAdministrator(actorUserID);
            ProgrammeRequirement requirement = FindRequirement(requirementID);
            requirement.IsActive = false;
            DataMaintenance(requirement, DB.DBOperation.Edit);
            FinalizeChanges(requirement, DB.DBOperation.Edit);
        }

        /// <summary>
        /// Saves the "Minimum average" row of UI 13.2 into DegreeProgramme.MinimumOverallAverage
        /// (Administrator only). 0 clears it.
        /// </summary>
        public void SaveMinimumAverage(int actorUserID, int programmeID, decimal minimumOverallAverage)
        {
            loginController.CheckAdministrator(actorUserID);
            if (!IsValidPercentage(minimumOverallAverage))
            {
                throw new ArgumentException("Enter a minimum average from 0 to 100, with at most two decimal places.");
            }

            DegreeProgramme programme = FindProgramme(programmeID);
            programme.MinimumOverallAverage = NoMinimumIfZero(minimumOverallAverage);
            DataMaintenance(programme, DB.DBOperation.Edit);
            FinalizeChanges(programme, DB.DBOperation.Edit);
        }
        #endregion

        #region Database communication (Waitering DataMaintenance / FinalizeChanges)
        /// <summary>Waitering DataMaintenance: makes the faculty change in the repository's DataSet. Private, so the role check cannot be skipped.</summary>
        private void DataMaintenance(Faculty faculty, DB.DBOperation operation)
        {
            facultyRepository.DataSetChange(faculty, operation);
        }

        /// <summary>Waitering FinalizeChanges: commits the faculty change to the database and returns the FacultyID.</summary>
        private int FinalizeChanges(Faculty faculty, DB.DBOperation operation)
        {
            return facultyRepository.UpdateDataSource(faculty, operation);
        }

        /// <summary>Waitering DataMaintenance: makes the programme change in the repository's DataSet.</summary>
        private void DataMaintenance(DegreeProgramme programme, DB.DBOperation operation)
        {
            programmeRepository.DataSetChange(programme, operation);
        }

        /// <summary>Waitering FinalizeChanges: commits the programme change to the database and returns the ProgrammeID.</summary>
        private int FinalizeChanges(DegreeProgramme programme, DB.DBOperation operation)
        {
            return programmeRepository.UpdateDataSource(programme, operation);
        }

        /// <summary>Waitering DataMaintenance: makes the requirement change in the repository's DataSet.</summary>
        private void DataMaintenance(ProgrammeRequirement requirement, DB.DBOperation operation)
        {
            programmeRepository.DataSetChange(requirement, operation);
        }

        /// <summary>Waitering FinalizeChanges: commits the requirement change to the database and returns the RequirementID.</summary>
        private int FinalizeChanges(ProgrammeRequirement requirement, DB.DBOperation operation)
        {
            return programmeRepository.UpdateDataSource(requirement, operation);
        }
        #endregion

        #region Helpers
        /// <summary>Loads a faculty for an edit, or throws ArgumentException if it no longer exists.</summary>
        private Faculty FindFaculty(int facultyID)
        {
            Faculty faculty = facultyRepository.FindById(facultyID);
            if (faculty == null)
            {
                throw new ArgumentException("The selected faculty no longer exists. Refresh the list and try again.");
            }
            return faculty;
        }

        /// <summary>Loads a programme for an edit, or throws ArgumentException if it no longer exists.</summary>
        private DegreeProgramme FindProgramme(int programmeID)
        {
            DegreeProgramme programme = programmeRepository.FindById(programmeID);
            if (programme == null)
            {
                throw new ArgumentException("The selected degree programme no longer exists. Refresh the list and try again.");
            }
            return programme;
        }

        /// <summary>Loads a requirement for an edit, or throws ArgumentException if it no longer exists.</summary>
        private ProgrammeRequirement FindRequirement(int requirementID)
        {
            ProgrammeRequirement requirement = programmeRepository.FindRequirementById(requirementID);
            if (requirement == null)
            {
                throw new ArgumentException("The selected requirement no longer exists. Refresh the list and try again.");
            }
            return requirement;
        }

        /// <summary>True for a percentage from 0 to 100 with at most two decimal places (DECIMAL(5,2)).</summary>
        private static bool IsValidPercentage(decimal value)
        {
            return value >= 0 && value <= 100 && decimal.Round(value, 2) == value;
        }

        /// <summary>A minimum of 0 means "no minimum", which is stored as NULL.</summary>
        private static decimal? NoMinimumIfZero(decimal value)
        {
            if (value == 0)
            {
                return null;
            }
            return value;
        }

        /// <summary>The value for a DataTable cell: null becomes DBNull.</summary>
        private static object ValueOrDBNull(object value)
        {
            if (value == null)
            {
                return DBNull.Value;
            }
            return value;
        }
        #endregion
    }
}
