
using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FuturePath.ApplicationLayer
{
    // Stores information about a degree programme and its requirements
    // Also checks subject marks and interest categories for recommendations
    public class DegreeProgramme
    {
        #region Data Members
        private int programmeID;
        private int facultyID;
        private string facultyName;
        private string programmeName;
        private decimal? minimumOverallAverage;
        private bool isActive;
        private Collection<ProgrammeRequirement> requirements;
        private Collection<DegreeInterestMapping> interestMappings;
        #endregion

        #region Properties
        public int ProgrammeID
        {
            get { return programmeID; }
            set { programmeID = value; }
        }

        public int FacultyID
        {
            get { return facultyID; }
            set { facultyID = value; }
        }

        public string FacultyName
        {
            get { return facultyName; }
            set { facultyName = value; }
        }

        public string ProgrammeName
        {
            get { return programmeName; }
            set { programmeName = value; }
        }

        public decimal? MinimumOverallAverage
        {
            get { return minimumOverallAverage; }
            set { minimumOverallAverage = value; }
        }

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        public Collection<DegreeInterestMapping> InterestMappings
        {
            get { return interestMappings; }
        }
        #endregion

        #region Constructors
        // Create a new active programme with empty requirements and interest mappings
        public DegreeProgramme()
        {
            programmeID = 0;
            facultyID = 0;
            facultyName = "";
            programmeName = "";
            minimumOverallAverage = null;
            isActive = true;
            requirements = new Collection<ProgrammeRequirement>();
            interestMappings = new Collection<DegreeInterestMapping>();
        }

        // Create a programme using its saved details
        // Requirements and interest mappings can be loaded separately
        public DegreeProgramme(int pProgrammeID, int pFacultyID, string pProgrammeName, decimal? pMinimumOverallAverage, bool pIsActive)
            : this()
        {
            programmeID = pProgrammeID;
            facultyID = pFacultyID;
            programmeName = pProgrammeName;
            minimumOverallAverage = pMinimumOverallAverage;
            isActive = pIsActive;
        }
        #endregion

        #region Methods
        // Check the learner's subjects and marks against the programme requirements
        // Calculate the subject and mark scores and record any missing requirements
        public MatchResult EvaluateSubjectAndMarkMatch(Collection<LearnerSubject> learnerSubjects)
        {
            MatchResult result = new MatchResult();
            if (learnerSubjects == null)
            {
                learnerSubjects = new Collection<LearnerSubject>();
            }
            result.SubjectsCaptured = learnerSubjects.Count;

            int requiredCount = 0;
            int requiredCaptured = 0;
            int markCount = 0;
            int marksMet = 0;

            foreach (ProgrammeRequirement requirement in requirements)
            {
                if (!requirement.IsActive)
                {
                    continue;
                }
                RequirementCheck check = requirement.CheckAgainst(learnerSubjects);
                result.Checks.Add(check);

                if (!requirement.IsRequired)
                {
                    if (!check.IsMet)
                    {
                        result.Notes.Add(check.Describe());
                    }
                    continue;
                }

                requiredCount++;
                if (check.ResultCode == RequirementCheck.ResultMissing)
                {
                    result.MissingRequired.Add(requirement.SubjectName);
                }
                else
                {
                    requiredCaptured++;
                }
                if (requirement.MinimumMark.HasValue)
                {
                    markCount++;
                    if (check.IsMet)
                    {
                        marksMet++;
                    }
                }
                if (!check.IsMet)
                {
                    result.GapList.Add(check.Describe());
                    if (check.ResultCode == RequirementCheck.ResultGap && check.Shortfall.HasValue && check.Shortfall.Value > result.LargestShortfall)
                    {
                        result.LargestShortfall = check.Shortfall.Value;
                    }
                }
            }

            // Give a score of 100 if there are no required subjects or minimum marks
            result.SubjectScore = 100;
            if (requiredCount > 0)
            {
                result.SubjectScore = Math.Round(100m * requiredCaptured / requiredCount, 2);
            }
            result.MarkScore = 100;
            if (markCount > 0)
            {
                result.MarkScore = Math.Round(100m * marksMet / markCount, 2);
            }

            // Calculate the overall average using all captured subjects
            if (learnerSubjects.Count > 0)
            {
                decimal total = 0;
                foreach (LearnerSubject learnerSubject in learnerSubjects)
                {
                    total += learnerSubject.Mark;
                }
                result.OverallAverage = Math.Round(total / learnerSubjects.Count, 1);
            }
            result.MinimumOverallAverage = minimumOverallAverage;
            if (!result.AverageMet)
            {
                decimal average = 0;
                if (result.OverallAverage.HasValue)
                {
                    average = result.OverallAverage.Value;
                }
                result.GapList.Add("Overall average " + FormatNumber(average) + "% is below the "
                                   + FormatNumber(minimumOverallAverage.Value) + "% minimum average for " + programmeName + ".");
            }
            return result;
        }

        // Get the interest categories linked to this programme
        // Each category includes a relevance weight used in scoring
        public Collection<InterestCategory> GetInterestCategories()
        {
            Collection<InterestCategory> categories = new Collection<InterestCategory>();
            foreach (DegreeInterestMapping mapping in interestMappings)
            {
                categories.Add(mapping.ToInterestCategory());
            }
            return categories;
        }

        // Check if the programme belongs to the learner's preferred faculty
        public bool MatchesPreferredFaculty(int preferredFacultyID)
        {
            return preferredFacultyID > 0 && facultyID == preferredFacultyID;
        }

        // Return the requirements for this programme
        public Collection<ProgrammeRequirement> GetRequirements()
        {
            return requirements;
        }

        // Format a number without unnecessary decimal zeros
        private static string FormatNumber(decimal value)
        {
            return value.ToString("0.#", CultureInfo.InvariantCulture);
        }

        // Return the programme name as a string
        public override string ToString()
        {
            return programmeName;
        }
        #endregion
    }
}
