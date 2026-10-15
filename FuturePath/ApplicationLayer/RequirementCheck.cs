using System.Globalization;

namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// The result of checking one programme requirement against a learner's subjects
    /// (built by ProgrammeRequirement.CheckAgainst). A transient value, like MatchResult.
    /// It is the single source of truth for "was this requirement met?": UI 6's requirement
    /// grid, FP-LSR Table 1, and the FP-DIE / FP-LER gap figures all use it, so no gap rule
    /// is repeated in SQL or in a form.
    /// </summary>
    public class RequirementCheck
    {
        #region Data Members
        /// <summary>Met with the required subject.</summary>
        public const string ResultMet = "MET";
        /// <summary>Met with the accepted alternative subject (e.g. Mathematical Literacy).</summary>
        public const string ResultAlternative = "ALT";
        /// <summary>The subject was captured but the mark is below the minimum.</summary>
        public const string ResultGap = "GAP";
        /// <summary>Neither the subject nor an accepted alternative was captured.</summary>
        public const string ResultMissing = "MISSING";

        private int requirementID;
        private string subjectName;
        private string subjectTakenName;
        private string alternativeSubjectName;
        private string requirementType;
        private decimal? minimumMark;
        private decimal? mark;
        private string resultCode;
        private decimal? shortfall;
        private string advisoryNote;
        #endregion

        #region Properties
        /// <summary>The requirement checked.</summary>
        public int RequirementID
        {
            get { return requirementID; }
            set { requirementID = value; }
        }

        /// <summary>The subject the programme asks for, e.g. "Mathematics".</summary>
        public string SubjectName
        {
            get { return subjectName; }
            set { subjectName = value; }
        }

        /// <summary>The subject actually used for the check (the alternative when it was used), or "".</summary>
        public string SubjectTakenName
        {
            get { return subjectTakenName; }
            set { subjectTakenName = value; }
        }

        /// <summary>The accepted alternative subject, or "" if there is none.</summary>
        public string AlternativeSubjectName
        {
            get { return alternativeSubjectName; }
            set { alternativeSubjectName = value; }
        }

        /// <summary>Required or Recommended.</summary>
        public string RequirementType
        {
            get { return requirementType; }
            set { requirementType = value; }
        }

        /// <summary>The minimum mark that applied (the alternative's minimum when the alternative was used).</summary>
        public decimal? MinimumMark
        {
            get { return minimumMark; }
            set { minimumMark = value; }
        }

        /// <summary>The learner's mark for the subject used, or null if it was not captured.</summary>
        public decimal? Mark
        {
            get { return mark; }
            set { mark = value; }
        }

        /// <summary>MET, ALT, GAP or MISSING.</summary>
        public string ResultCode
        {
            get { return resultCode; }
            set { resultCode = value; }
        }

        /// <summary>Points below the minimum (GAP only), otherwise null.</summary>
        public decimal? Shortfall
        {
            get { return shortfall; }
            set { shortfall = value; }
        }

        /// <summary>The Administrator's note shown to the learner, or "".</summary>
        public string AdvisoryNote
        {
            get { return advisoryNote; }
            set { advisoryNote = value; }
        }

        /// <summary>True if the requirement was met (MET or ALT).</summary>
        public bool IsMet
        {
            get { return resultCode == ResultMet || resultCode == ResultAlternative; }
        }

        /// <summary>True for a Required requirement, false for a Recommended one.</summary>
        public bool IsRequired
        {
            get { return requirementType == ProgrammeRequirement.TypeRequired; }
        }

        /// <summary>A short result for a grid column, e.g. "Met" or "2 points below minimum".</summary>
        public string RequirementOutcome
        {
            get
            {
                switch (resultCode)
                {
                    case ResultMet:
                        return "Met";
                    case ResultAlternative:
                        return "Met with " + subjectTakenName;
                    case ResultGap:
                        return FormatNumber(shortfall.Value) + " points below minimum";
                    default:
                        return "Not captured";
                }
            }
        }
        #endregion

        #region Constructors
        /// <summary>Creates an empty check (Missing).</summary>
        public RequirementCheck()
        {
            requirementID = 0;
            subjectName = "";
            subjectTakenName = "";
            alternativeSubjectName = "";
            requirementType = ProgrammeRequirement.TypeRequired;
            minimumMark = null;
            mark = null;
            resultCode = ResultMissing;
            shortfall = null;
            advisoryNote = "";
        }

        /// <summary>Creates a check with all its values.</summary>
        public RequirementCheck(int pRequirementID, string pSubjectName, string pSubjectTakenName, string pAlternativeSubjectName,
                                string pRequirementType, decimal? pMinimumMark, decimal? pMark, string pResultCode,
                                decimal? pShortfall, string pAdvisoryNote)
        {
            requirementID = pRequirementID;
            subjectName = pSubjectName;
            subjectTakenName = pSubjectTakenName;
            alternativeSubjectName = pAlternativeSubjectName;
            requirementType = pRequirementType;
            minimumMark = pMinimumMark;
            mark = pMark;
            resultCode = pResultCode;
            shortfall = pShortfall;
            advisoryNote = pAdvisoryNote;
        }
        #endregion

        #region Methods
        /// <summary>
        /// A full sentence for explanations and reports, e.g.
        /// "Mathematics 58% is 2 points below the 60% minimum." or
        /// "Physical Sciences is required but was not captured."
        /// </summary>
        public string Describe()
        {
            string needed = "recommended";
            if (IsRequired)
            {
                needed = "required";
            }
            switch (resultCode)
            {
                case ResultMet:
                    if (minimumMark.HasValue)
                    {
                        return subjectName + " " + FormatNumber(mark.Value) + "% meets the " + FormatNumber(minimumMark.Value) + "% minimum.";
                    }
                    return subjectName + " was captured.";
                case ResultAlternative:
                    return subjectTakenName + " " + FormatNumber(mark.Value) + "% is accepted in place of " + subjectName + ".";
                case ResultGap:
                    return subjectTakenName + " " + FormatNumber(mark.Value) + "% is " + FormatNumber(shortfall.Value)
                           + " points below the " + FormatNumber(minimumMark.Value) + "% minimum.";
                default:
                    if (!string.IsNullOrEmpty(alternativeSubjectName))
                    {
                        return subjectName + " (or " + alternativeSubjectName + ") is " + needed + " but was not captured.";
                    }
                    return subjectName + " is " + needed + " but was not captured.";
            }
        }

        /// <summary>Formats a mark without trailing zeros: 58, 57.5.</summary>
        private static string FormatNumber(decimal value)
        {
            return value.ToString("0.#", CultureInfo.InvariantCulture);
        }

        /// <summary>Returns the sentence from Describe().</summary>
        public override string ToString()
        {
            return Describe();
        }
        #endregion
    }
}
