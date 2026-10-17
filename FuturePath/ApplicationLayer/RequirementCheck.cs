
using System.Globalization;

namespace FuturePath.ApplicationLayer
{
    // Stores the result of checking a learner's subject against a programme requirement
    // Records whether the requirement was met, missing or below the minimum mark
    public class RequirementCheck
    {
        #region Data Members
        // Possible results when checking a subject requirement
        public const string ResultMet = "MET";
        public const string ResultAlternative = "ALT";
        public const string ResultGap = "GAP";
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
        public int RequirementID
        {
            get { return requirementID; }
            set { requirementID = value; }
        }

        public string SubjectName
        {
            get { return subjectName; }
            set { subjectName = value; }
        }

        public string SubjectTakenName
        {
            get { return subjectTakenName; }
            set { subjectTakenName = value; }
        }

        public string AlternativeSubjectName
        {
            get { return alternativeSubjectName; }
            set { alternativeSubjectName = value; }
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

        public decimal? Mark
        {
            get { return mark; }
            set { mark = value; }
        }

        public string ResultCode
        {
            get { return resultCode; }
            set { resultCode = value; }
        }

        public decimal? Shortfall
        {
            get { return shortfall; }
            set { shortfall = value; }
        }

        public string AdvisoryNote
        {
            get { return advisoryNote; }
            set { advisoryNote = value; }
        }

        // Check if the requirement was met using the main or alternative subject
        public bool IsMet
        {
            get { return resultCode == ResultMet || resultCode == ResultAlternative; }
        }

        // Check whether the requirement is compulsory
        public bool IsRequired
        {
            get { return requirementType == ProgrammeRequirement.TypeRequired; }
        }

        // Return a short description of the requirement result
        // Used when displaying the result in a table
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
        // Create a new requirement check with default values
        // Set the initial result to Missing
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

        // Create a requirement check using its stored values
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
        // Create a full explanation of the requirement result
        // Include the subject, mark, minimum and any missing requirements
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

        // Format marks without unnecessary decimal zeros
        private static string FormatNumber(decimal value)
        {
            return value.ToString("0.#", CultureInfo.InvariantCulture);
        }

        // Return the full requirement explanation
        public override string ToString()
        {
            return Describe();
        }
        #endregion
    }
}
