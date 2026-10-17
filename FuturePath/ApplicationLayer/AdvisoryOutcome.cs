
using System;

namespace FuturePath.ApplicationLayer
{
    // Stores the result of a learner's programme eligibility check
    // Includes the outcome category, explanation and overall score
    public class AdvisoryOutcome
    {
        #region Data Members
        // The four possible advisory outcomes
        // Values are ordered from weakest to strongest for ranking
        public enum OutcomeType
        {
            MoreInformationNeeded = 0,
            NotCurrentlyEligible = 1,
            PossibleMatch = 2,
            StrongMatch = 3
        }

        // Codes used when saving advisory outcomes
        public const string CodeStrong = "STRONG";
        public const string CodePossible = "POSSIBLE";
        public const string CodeNotEligible = "NCE";
        public const string CodeMoreInformation = "MOREINFO";

        private OutcomeType outcome;
        private string explanation;
        private decimal? overallScore;
        #endregion

        #region Properties
        public OutcomeType Outcome
        {
            get { return outcome; }
            set { outcome = value; }
        }

        // Get the stored code for the selected outcome
        public string Code
        {
            get { return GetCode(outcome); }
        }

        // Get the outcome category shown to the learner
        public string Category
        {
            get { return GetCategory(outcome); }
        }

        public string Explanation
        {
            get { return explanation; }
            set { explanation = value; }
        }

        public decimal? OverallScore
        {
            get { return overallScore; }
            set { overallScore = value; }
        }

        // Get the description of the advisory outcome
        public string Definition
        {
            get { return GetDefinition(outcome); }
        }
        #endregion

        #region Constructors
        // Create a new outcome with default values
        public AdvisoryOutcome()
        {
            outcome = OutcomeType.MoreInformationNeeded;
            explanation = "";
            overallScore = null;
        }

        // Create an outcome using its category, explanation and score
        public AdvisoryOutcome(OutcomeType pOutcome, string pExplanation, decimal? pOverallScore)
        {
            outcome = pOutcome;
            explanation = pExplanation;
            overallScore = pOverallScore;
        }
        #endregion

        #region Methods
        // Create an outcome when the learner has not provided enough information
        public static AdvisoryOutcome MoreInformationNeeded(string reason)
        {
            return new AdvisoryOutcome(OutcomeType.MoreInformationNeeded, reason, null);
        }

        // Return the stored code for an outcome type
        public static string GetCode(OutcomeType type)
        {
            switch (type)
            {
                case OutcomeType.StrongMatch:
                    return CodeStrong;
                case OutcomeType.PossibleMatch:
                    return CodePossible;
                case OutcomeType.NotCurrentlyEligible:
                    return CodeNotEligible;
                default:
                    return CodeMoreInformation;
            }
        }

        // Return the category name displayed to the learner
        public static string GetCategory(OutcomeType type)
        {
            switch (type)
            {
                case OutcomeType.StrongMatch:
                    return "Strong Match";
                case OutcomeType.PossibleMatch:
                    return "Possible Match";
                case OutcomeType.NotCurrentlyEligible:
                    return "Not Currently Eligible";
                default:
                    return "More Information Needed";
            }
        }

        // Convert a stored outcome code into its display name
        public static string GetCategory(string code)
        {
            return GetCategory(FromCode(code));
        }

        // Return the explanation of what each outcome category means
        public static string GetDefinition(OutcomeType type)
        {
            switch (type)
            {
                case OutcomeType.StrongMatch:
                    return "Appears to meet the minimum requirements and has strong interest alignment.";
                case OutcomeType.PossibleMatch:
                    return "Meets some requirements but has one or more gaps or weaker alignment.";
                case OutcomeType.NotCurrentlyEligible:
                    return "Does not currently meet one or more important requirements.";
                default:
                    return "The check cannot be completed because required information is missing.";
            }
        }

        // Convert a stored code back into its outcome type
        // Throw an error if the code is not recognised
        public static OutcomeType FromCode(string code)
        {
            string value = "";
            if (code != null)
            {
                value = code.Trim().ToUpperInvariant();
            }
            switch (value)
            {
                case CodeStrong:
                    return OutcomeType.StrongMatch;
                case CodePossible:
                    return OutcomeType.PossibleMatch;
                case CodeNotEligible:
                    return OutcomeType.NotCurrentlyEligible;
                case CodeMoreInformation:
                    return OutcomeType.MoreInformationNeeded;
                default:
                    throw new ArgumentException("Unknown outcome code: " + code);
            }
        }

        // Check whether the code matches one of the four valid outcomes
        public static bool IsValidCode(string code)
        {
            if (code == null)
            {
                return false;
            }
            string value = code.Trim().ToUpperInvariant();
            return value == CodeStrong || value == CodePossible || value == CodeNotEligible || value == CodeMoreInformation;
        }

        // Set the display order for outcomes on screens and reports
        // Strong Match appears first and More Information Needed appears last
        public static int GetDisplayOrder(OutcomeType type)
        {
            return 4 - (int)type;
        }

        // Return all four outcome types in display order
        public static OutcomeType[] GetOutcomeTypes()
        {
            return new OutcomeType[]
            {
                OutcomeType.StrongMatch,
                OutcomeType.PossibleMatch,
                OutcomeType.NotCurrentlyEligible,
                OutcomeType.MoreInformationNeeded
            };
        }

        // Return the outcome category followed by its explanation
        public override string ToString()
        {
            return Category + ": " + explanation;
        }
        #endregion
    }
}
