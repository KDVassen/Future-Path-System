using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// A degree programme a learner can check, e.g. "BCom Information Systems". It belongs to
    /// one faculty, has requirements, and maps to interest categories. Maps to DegreeProgramme.
    /// The class diagram gives it the matching methods used in both sequence diagrams.
    /// </summary>
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
        /// <summary>Primary key (DegreeProgramme.ProgrammeID).</summary>
        public int ProgrammeID
        {
            get { return programmeID; }
            set { programmeID = value; }
        }

        /// <summary>The faculty that offers the programme.</summary>
        public int FacultyID
        {
            get { return facultyID; }
            set { facultyID = value; }
        }

        /// <summary>The faculty name (read from Faculty, for display).</summary>
        public string FacultyName
        {
            get { return facultyName; }
            set { facultyName = value; }
        }

        /// <summary>The programme name.</summary>
        public string ProgrammeName
        {
            get { return programmeName; }
            set { programmeName = value; }
        }

        /// <summary>The minimum overall average (%), or null if the programme sets none.</summary>
        public decimal? MinimumOverallAverage
        {
            get { return minimumOverallAverage; }
            set { minimumOverallAverage = value; }
        }

        /// <summary>Inactive programmes never appear in recommendations or reports.</summary>
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        /// <summary>The programme's interest mappings (loaded with FindByIdWithRequirements).</summary>
        public Collection<DegreeInterestMapping> InterestMappings
        {
            get { return interestMappings; }
        }
        #endregion

        #region Constructors
        /// <summary>Creates an empty, active programme.</summary>
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

        /// <summary>Creates a programme with its stored values (no requirements loaded yet).</summary>
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
        /// <summary>
        /// Class diagram evaluateSubjectAndMarkMatch(learnerSubjects). Checks every active
        /// requirement against the learner's subjects and works out:
        /// - SubjectScore: % of Required subjects captured (or accepted alternatives);
        /// - MarkScore: % of Required minimum marks met;
        /// - the gaps, the missing Required subjects and the biggest shortfall;
        /// - the overall average against the programme's minimum average.
        /// Recommended requirements only add notes.
        /// </summary>
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

            result.SubjectScore = requiredCount == 0 ? 100 : Math.Round(100m * requiredCaptured / requiredCount, 2);
            result.MarkScore = markCount == 0 ? 100 : Math.Round(100m * marksMet / markCount, 2);

            // The overall average includes every captured subject (decisions.md).
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
                result.GapList.Add("Overall average " + FormatNumber(result.OverallAverage ?? 0) + "% is below the "
                                   + FormatNumber(minimumOverallAverage.Value) + "% minimum average for " + programmeName + ".");
            }
            return result;
        }

        /// <summary>
        /// Class diagram getInterestCategories(). The interest categories this programme maps to,
        /// each carrying its relevance weight.
        /// </summary>
        public Collection<InterestCategory> GetInterestCategories()
        {
            Collection<InterestCategory> categories = new Collection<InterestCategory>();
            foreach (DegreeInterestMapping mapping in interestMappings)
            {
                categories.Add(mapping.ToInterestCategory());
            }
            return categories;
        }

        /// <summary>
        /// Class diagram matchesPreferredFaculty(facultyId). True if the programme belongs to the
        /// learner's preferred faculty; this is the faculty bonus.
        /// </summary>
        public bool MatchesPreferredFaculty(int preferredFacultyID)
        {
            return preferredFacultyID > 0 && facultyID == preferredFacultyID;
        }

        /// <summary>Class diagram getRequirements(). The programme's requirements.</summary>
        public Collection<ProgrammeRequirement> GetRequirements()
        {
            return requirements;
        }

        /// <summary>Formats a number without trailing zeros.</summary>
        private static string FormatNumber(decimal value)
        {
            return value.ToString("0.#", CultureInfo.InvariantCulture);
        }

        /// <summary>Returns the programme name.</summary>
        public override string ToString()
        {
            return programmeName;
        }
        #endregion
    }
}
