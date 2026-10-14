using System.Collections.ObjectModel;

namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// One subject requirement of a degree programme, e.g. "Mathematics, Required, minimum 60%,
    /// no alternative". It can accept one alternative subject (Mathematics / Mathematical
    /// Literacy), with its own minimum. Maps to ProgrammeRequirement; one requirement per subject
    /// per programme (UNIQUE(ProgrammeID, SubjectID)).
    /// </summary>
    public class ProgrammeRequirement
    {
        #region Data Members
        /// <summary>A requirement that decides eligibility.</summary>
        public const string TypeRequired = "Required";
        /// <summary>A requirement that only adds advice; it never decides the outcome.</summary>
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
        /// <summary>Primary key (ProgrammeRequirement.RequirementID).</summary>
        public int RequirementID
        {
            get { return requirementID; }
            set { requirementID = value; }
        }

        /// <summary>The programme this requirement belongs to.</summary>
        public int ProgrammeID
        {
            get { return programmeID; }
            set { programmeID = value; }
        }

        /// <summary>The subject required (FK to Subject).</summary>
        public int SubjectID
        {
            get { return subjectID; }
            set { subjectID = value; }
        }

        /// <summary>The subject name (read from Subject, for display).</summary>
        public string SubjectName
        {
            get { return subjectName; }
            set { subjectName = value; }
        }

        /// <summary>Required or Recommended.</summary>
        public string RequirementType
        {
            get { return requirementType; }
            set { requirementType = value; }
        }

        /// <summary>The minimum mark (%), or null if only the subject itself is needed.</summary>
        public decimal? MinimumMark
        {
            get { return minimumMark; }
            set { minimumMark = value; }
        }

        /// <summary>The accepted alternative subject, or null if there is none.</summary>
        public int? AlternativeSubjectID
        {
            get { return alternativeSubjectID; }
            set { alternativeSubjectID = value; }
        }

        /// <summary>The alternative subject's name (for display), or "".</summary>
        public string AlternativeSubjectName
        {
            get { return alternativeSubjectName; }
            set { alternativeSubjectName = value; }
        }

        /// <summary>The minimum for the alternative subject; when null, MinimumMark applies.</summary>
        public decimal? AlternativeMinimumMark
        {
            get { return alternativeMinimumMark; }
            set { alternativeMinimumMark = value; }
        }

        /// <summary>The note shown to the learner (UI 6, FP-LSR), or "".</summary>
        public string AdvisoryNote
        {
            get { return advisoryNote; }
            set { advisoryNote = value; }
        }

        /// <summary>Inactive requirements are ignored by the eligibility check.</summary>
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        /// <summary>True for a Required requirement.</summary>
        public bool IsRequired
        {
            get { return requirementType == TypeRequired; }
        }
        #endregion

        #region Constructors
        /// <summary>Creates an empty, active Required requirement.</summary>
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

        /// <summary>Creates a requirement with all its stored values.</summary>
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
        /// <summary>Class diagram isSatisfiedBy(learnerSubjects): true if this requirement is met.</summary>
        public bool IsSatisfiedBy(Collection<LearnerSubject> learnerSubjects)
        {
            return CheckAgainst(learnerSubjects).IsMet;
        }

        /// <summary>
        /// Checks this requirement against the learner's subjects:
        /// 1. the required subject meets the minimum: MET;
        /// 2. otherwise the accepted alternative meets its minimum: ALT;
        /// 3. otherwise a captured subject below its minimum: GAP, with the shortfall;
        /// 4. otherwise: MISSING.
        /// </summary>
        public RequirementCheck CheckAgainst(Collection<LearnerSubject> learnerSubjects)
        {
            LearnerSubject direct = FindSubject(learnerSubjects, subjectID);
            LearnerSubject alternative = null;
            if (alternativeSubjectID.HasValue)
            {
                alternative = FindSubject(learnerSubjects, alternativeSubjectID.Value);
            }
            decimal? alternativeMinimum = alternativeMinimumMark ?? minimumMark;

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
            return new RequirementCheck(requirementID, subjectName, "", alternativeSubjectName ?? "", requirementType,
                                        minimumMark, null, RequirementCheck.ResultMissing, null, advisoryNote ?? "");
        }

        /// <summary>True if Required or Recommended.</summary>
        public static bool IsValidType(string type)
        {
            return type == TypeRequired || type == TypeRecommended;
        }

        /// <summary>True if there is no minimum, or the mark reaches it.</summary>
        private static bool Meets(decimal mark, decimal? minimum)
        {
            return !minimum.HasValue || mark >= minimum.Value;
        }

        /// <summary>Builds a check for a captured subject.</summary>
        private RequirementCheck MakeCheck(LearnerSubject taken, decimal? minimum, string resultCode, decimal? shortfall)
        {
            return new RequirementCheck(requirementID, subjectName, taken.SubjectName, alternativeSubjectName ?? "", requirementType,
                                        minimum, taken.Mark, resultCode, shortfall, advisoryNote ?? "");
        }

        /// <summary>Finds a subject in the learner's captured subjects, or returns null.</summary>
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

        /// <summary>Returns e.g. "Mathematics (Required, 60%)".</summary>
        public override string ToString()
        {
            return subjectName + " (" + requirementType + (minimumMark.HasValue ? ", " + minimumMark.Value.ToString("0.#") + "%" : "") + ")";
        }
        #endregion
    }
}
