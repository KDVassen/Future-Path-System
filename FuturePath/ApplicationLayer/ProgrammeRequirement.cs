
using System.Collections.ObjectModel;
using System.Globalization;

namespace FuturePath.ApplicationLayer
{
    // Stores the subject and mark requirements for a degree programme
    // Also checks whether a learner meets the requirements or an accepted alternative
    public class ProgrammeRequirement
    {
        #region Data Members
        // Types of programme requirements
        public const string TypeRequired = "Required";
        public const string TypeRecommended = "Recommended";

        private int requirementID;
        private int programmeID;
        private int subjectID;
        private string subjectName;
        private string requirementType;
        private decimal? minimumMark;
        private int? alternativeSubjectID;
        private string alternativeSubjectName;
        private decimal? alternativeMinimumMark;
        private string advisoryNote;
        private bool isActive;
        #endregion

        #region Properties
        public int RequirementID
        {
            get { return requirementID; }
            set { requirementID = value; }
        }

        public int ProgrammeID
        {
            get { return programmeID; }
            set { programmeID = value; }
        }

        public int SubjectID
        {
            get { return subjectID; }
            set { subjectID = value; }
        }

        public string SubjectName
        {
            get { return subjectName; }
            set { subjectName = value; }
        }

        public string RequirementType
        {
            get { return requirementType; }
            set { requirementType = value; }
        }

        public decimal? MinimumMark
        {
            get { return minimumMark; }
            set { minimumMark = value; }
        }

        public int? AlternativeSubjectID
        {
            get { return alternativeSubjectID; }
            set { alternativeSubjectID = value; }
        }

        public string AlternativeSubjectName
        {
            get { return alternativeSubjectName; }
            set { alternativeSubjectName = value; }
        }

        public decimal? AlternativeMinimumMark
        {
            get { return alternativeMinimumMark; }
            set { alternativeMinimumMark = value; }
        }

        public string AdvisoryNote
        {
            get { return advisoryNote; }
            set { advisoryNote = value; }
        }

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        // Check whether this is a required subject
        public bool IsRequired
        {
            get { return requirementType == TypeRequired; }
        }
        #endregion

        #region Constructors
        // Create an active requirement with default values
        public ProgrammeRequirement()
        {
            requirementID = 0;
            programmeID = 0;
            subjectID = 0;
            subjectName = "";
            requirementType = TypeRequired;
            minimumMark = null;
            alternativeSubjectID = null;
            alternativeSubjectName = "";
            alternativeMinimumMark = null;
            advisoryNote = "";
            isActive = true;
        }

        // Create a requirement using its stored details
        public ProgrammeRequirement(int pRequirementID, int pProgrammeID, int pSubjectID, string pSubjectName, string pRequirementType,
                                    decimal? pMinimumMark, int? pAlternativeSubjectID, string pAlternativeSubjectName,
                                    decimal? pAlternativeMinimumMark, string pAdvisoryNote, bool pIsActive)
        {
            requirementID = pRequirementID;
            programmeID = pProgrammeID;
            subjectID = pSubjectID;
            subjectName = pSubjectName;
            requirementType = pRequirementType;
            minimumMark = pMinimumMark;
            alternativeSubjectID = pAlternativeSubjectID;
            alternativeSubjectName = pAlternativeSubjectName;
            alternativeMinimumMark = pAlternativeMinimumMark;
            advisoryNote = pAdvisoryNote;
            isActive = pIsActive;
        }
        #endregion

        #region Methods
        // Check if the learner meets this programme requirement
        public bool IsSatisfiedBy(Collection<LearnerSubject> learnerSubjects)
        {
            return CheckAgainst(learnerSubjects).IsMet;
        }

        // Compare the learner's subjects and marks with the requirements
        // Check the main subject first, then any accepted alternative
        // Record whether the requirement is met, missing or below the minimum
        public RequirementCheck CheckAgainst(Collection<LearnerSubject> learnerSubjects)
        {
            LearnerSubject direct = FindSubject(learnerSubjects, subjectID);
            LearnerSubject alternative = null;
            if (alternativeSubjectID.HasValue)
            {
                alternative = FindSubject(learnerSubjects, alternativeSubjectID.Value);
            }
            // Use the alternative minimum if one is provided
            // Otherwise use the main subject's minimum
            decimal? alternativeMinimum = minimumMark;
            if (alternativeMinimumMark.HasValue)
            {
                alternativeMinimum = alternativeMinimumMark;
            }

            if (direct != null && Meets(direct.Mark, minimumMark))
            {
                return MakeCheck(direct, minimumMark, RequirementCheck.ResultMet, null);
            }
            if (alternative != null && Meets(alternative.Mark, alternativeMinimum))
            {
                return MakeCheck(alternative, alternativeMinimum, RequirementCheck.ResultAlternative, null);
            }
            if (direct != null)
            {
                return MakeCheck(direct, minimumMark, RequirementCheck.ResultGap, minimumMark.Value - direct.Mark);
            }
            if (alternative != null)
            {
                return MakeCheck(alternative, alternativeMinimum, RequirementCheck.ResultGap, alternativeMinimum.Value - alternative.Mark);
            }
            return new RequirementCheck(requirementID, subjectName, "", alternativeSubjectName, requirementType,
                                        minimumMark, null, RequirementCheck.ResultMissing, null, advisoryNote);
        }

        // Check if the requirement type is valid
        public static bool IsValidType(string type)
        {
            return type == TypeRequired || type == TypeRecommended;
        }

        // Check if the learner's mark meets the minimum
        // A subject without a minimum mark is automatically accepted
        private static bool Meets(decimal mark, decimal? minimum)
        {
            return !minimum.HasValue || mark >= minimum.Value;
        }

        // Create the result for a captured subject
        private RequirementCheck MakeCheck(LearnerSubject taken, decimal? minimum, string resultCode, decimal? shortfall)
        {
            return new RequirementCheck(requirementID, subjectName, taken.SubjectName, alternativeSubjectName, requirementType,
                                        minimum, taken.Mark, resultCode, shortfall, advisoryNote);
        }

        // Find the subject in the learner's captured subjects
        // Return null if it has not been added
        private static LearnerSubject FindSubject(Collection<LearnerSubject> learnerSubjects, int id)
        {
            if (learnerSubjects == null)
            {
                return null;
            }
            foreach (LearnerSubject learnerSubject in learnerSubjects)
            {
                if (learnerSubject.SubjectID == id)
                {
                    return learnerSubject;
                }
            }
            return null;
        }

        // Return a description of the requirement and its minimum mark
        public override string ToString()
        {
            string text = subjectName + " (" + requirementType;
            if (minimumMark.HasValue)
            {
                text += ", " + minimumMark.Value.ToString("0.#", CultureInfo.InvariantCulture) + "%";
            }
            return text + ")";
        }
        #endregion
    }
}
