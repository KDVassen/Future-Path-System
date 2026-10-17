
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using FuturePath.DatabaseLayer;

namespace FuturePath.ApplicationLayer
{
    // Manages reference data used throughout the system
    // Allows administrators to maintain faculties, programmes and requirements
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
        // Return the faculties loaded by the controller
        public Collection<Faculty> AllFaculties
        {
            get { return faculties; }
        }

        // Return the programmes loaded by the controller
        public Collection<DegreeProgramme> AllProgrammes
        {
            get { return programmes; }
        }
        #endregion

        #region Constructor
        // Initialise the repositories and collections
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
        // Get the available grade levels
        // Optionally include an All grades choice for filtering
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

        // Get the province options from the system settings
        public DataTable GetProvinces()
        {
            return SystemSettings.GetNumberedParameters(SystemSettings.ProvincePrefix);
        }

        // Get the four possible advisory outcomes
        // Optionally include an All outcomes choice
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

            // Use the outcome types and labels defined in AdvisoryOutcome
            foreach (AdvisoryOutcome.OutcomeType type in AdvisoryOutcome.GetOutcomeTypes())
            {
                table.Rows.Add(AdvisoryOutcome.GetCode(type), AdvisoryOutcome.GetCategory(type), AdvisoryOutcome.GetDisplayOrder(type));
            }
            return table;
        }

        // Get the available programme requirement types
        public DataTable GetRequirementTypes()
        {
            DataTable table = new DataTable("RequirementTypes");
            table.Columns.Add("RequirementType", typeof(string));
            table.Columns.Add("Description", typeof(string));
            table.Rows.Add(ProgrammeRequirement.TypeRequired, ProgrammeRequirement.TypeRequired + ": decides eligibility");
            table.Rows.Add(ProgrammeRequirement.TypeRecommended, ProgrammeRequirement.TypeRecommended + ": advice only");
            return table;
        }

        // Read a whole-number system parameter
        // Return zero if the parameter is missing or invalid
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

        // Load faculties, including inactive ones when requested
        // Optionally add an All faculties choice
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

        // Return only active faculties
        public Collection<Faculty> GetActiveFaculties()
        {
            return GetFaculties(false, false);
        }

        // Load degree programmes for a selected faculty
        // A faculty ID of zero includes programmes from all faculties
        public Collection<DegreeProgramme> GetProgrammes(int facultyID, bool includeInactive)
        {
            programmes = programmeRepository.GetProgrammes(facultyID, includeInactive);
            return programmes;
        }

        // Return active programmes from active faculties
        public Collection<DegreeProgramme> GetActiveProgrammes(int facultyID)
        {
            return GetProgrammes(facultyID, false);
        }

        // Search active programmes by name
        // Return an empty collection if no search text is entered
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

        // Get the available school subjects
        public Collection<Subject> GetSubjects(bool includeInactive)
        {
            return subjectRepository.GetSubjects(includeInactive);
        }

        // Return only active school subjects
        public Collection<Subject> GetActiveSubjects()
        {
            return GetSubjects(false);
        }

        // Get active subjects for selecting an alternative requirement
        // Include None when no alternative subject is required
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

        // Load programme requirements into a table for administrator maintenance
        // Include the programme's minimum overall average as a separate row
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

            // Add the minimum average if the programme has one
            DegreeProgramme programme = programmeRepository.FindById(programmeID);
            if (programme != null && programme.MinimumOverallAverage.HasValue)
            {
                table.Rows.Add(DBNull.Value, ProgrammeRequirement.TypeRequired, DBNull.Value, "Minimum average",
                               programme.MinimumOverallAverage.Value, DBNull.Value, "", DBNull.Value, "", true);
            }
            return table;
        }

        // Get active requirements for the learner's selected programme
        // Include the overall average requirement when applicable
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
        // Validate the faculty name and check for duplicates
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

        // Add or update a faculty after checking administrator access
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

        // Deactivate a faculty without deleting it
        // Its programmes will no longer appear in learner lists
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
        // Validate the programme's faculty, name and minimum average
        // Check for duplicate names and marks outside the valid range
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

        // Add or update a programme after checking administrator access
        // A minimum average of zero means no minimum is required
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

        // Deactivate a programme so it no longer appears in recommendations
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
        // Validate the programme requirement and any accepted alternative
        // Check for duplicate subjects, valid marks and advisory note length
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

        // Add or update a subject requirement for a degree programme
        // Zero values are treated as missing minimums or no alternative
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

            // Store no alternative as null
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

        // Deactivate a programme requirement without deleting it
        public void DeactivateRequirement(int actorUserID, int requirementID)
        {
            loginController.CheckAdministrator(actorUserID);
            ProgrammeRequirement requirement = FindRequirement(requirementID);
            requirement.IsActive = false;
            DataMaintenance(requirement, DB.DBOperation.Edit);
            FinalizeChanges(requirement, DB.DBOperation.Edit);
        }

        // Save the programme's minimum overall average
        // A value of zero removes the minimum average requirement
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

        #region Database communication
        // Apply faculty changes to the repository DataSet
        private void DataMaintenance(Faculty faculty, DB.DBOperation operation)
        {
            facultyRepository.DataSetChange(faculty, operation);
        }

        // Save faculty changes to the database
        private int FinalizeChanges(Faculty faculty, DB.DBOperation operation)
        {
            return facultyRepository.UpdateDataSource(faculty, operation);
        }

        // Apply programme changes to the repository DataSet
        private void DataMaintenance(DegreeProgramme programme, DB.DBOperation operation)
        {
            programmeRepository.DataSetChange(programme, operation);
        }

        // Save programme changes to the database
        private int FinalizeChanges(DegreeProgramme programme, DB.DBOperation operation)
        {
            return programmeRepository.UpdateDataSource(programme, operation);
        }

        // Apply requirement changes to the repository DataSet
        private void DataMaintenance(ProgrammeRequirement requirement, DB.DBOperation operation)
        {
            programmeRepository.DataSetChange(requirement, operation);
        }

        // Save requirement changes to the database
        private int FinalizeChanges(ProgrammeRequirement requirement, DB.DBOperation operation)
        {
            return programmeRepository.UpdateDataSource(requirement, operation);
        }
        #endregion

        #region Helpers
        // Find a faculty for editing or throw an error if it no longer exists
        private Faculty FindFaculty(int facultyID)
        {
            Faculty faculty = facultyRepository.FindById(facultyID);
            if (faculty == null)
            {
                throw new ArgumentException("The selected faculty no longer exists. Refresh the list and try again.");
            }
            return faculty;
        }

        // Find a programme for editing or throw an error if it no longer exists
        private DegreeProgramme FindProgramme(int programmeID)
        {
            DegreeProgramme programme = programmeRepository.FindById(programmeID);
            if (programme == null)
            {
                throw new ArgumentException("The selected degree programme no longer exists. Refresh the list and try again.");
            }
            return programme;
        }

        // Find a requirement for editing or throw an error if it no longer exists
        private ProgrammeRequirement FindRequirement(int requirementID)
        {
            ProgrammeRequirement requirement = programmeRepository.FindRequirementById(requirementID);
            if (requirement == null)
            {
                throw new ArgumentException("The selected requirement no longer exists. Refresh the list and try again.");
            }
            return requirement;
        }

        // Check that a percentage is between 0 and 100 with two decimal places at most
        private static bool IsValidPercentage(decimal value)
        {
            return value >= 0 && value <= 100 && decimal.Round(value, 2) == value;
        }

        // Return null when no minimum mark has been set
        private static decimal? NoMinimumIfZero(decimal value)
        {
            if (value == 0)
            {
                return null;
            }
            return value;
        }

        // Convert null values to DBNull for DataTable cells
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
