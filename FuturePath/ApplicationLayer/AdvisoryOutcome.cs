using System;

namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// The result of one eligibility check (class diagram AdvisoryOutcome: category, explanation,
    /// overallScore). A transient value: it is stored through RecommendationResult
    /// (OutcomeCode and OutcomeReason). Every outcome carries its reason; the label is never
    /// shown on its own (spec Section 7, output controls).
    /// Code is the short value stored in RecommendationResult.OutcomeCode (VARCHAR(10)).
    /// Category is the wording shown to people.
    /// </summary>
    public class AdvisoryOutcome
    {
        #region Data Members
        /// <summary>
        /// The four outcomes (case study §6.5). The numbers order them from weakest to strongest,
        /// which the engine uses to rank alternatives.
        /// </summary>
        public enum OutcomeType
        {
            /// <summary>The check cannot be completed because required information is missing.</summary>
            MoreInformationNeeded = 0,
            /// <summary>Does not currently meet one or more important requirements.</summary>
            NotCurrentlyEligible = 1,
            /// <summary>Meets some requirements but has one or more gaps or weaker alignment.</summary>
            PossibleMatch = 2,
            /// <summary>Appears to meet the minimum requirements and has strong interest alignment.</summary>
            StrongMatch = 3
        }

        /// <summary>Stored code for Strong Match.</summary>
        public const string CodeStrong = "STRONG";
        /// <summary>Stored code for Possible Match.</summary>
        public const string CodePossible = "POSSIBLE";
        /// <summary>Stored code for Not Currently Eligible.</summary>
        public const string CodeNotEligible = "NCE";
        /// <summary>Stored code for More Information Needed.</summary>
        public const string CodeMoreInformation = "MOREINFO";

        private OutcomeType outcome;
        private string explanation;
        private decimal? overallScore;
        #endregion

        #region Properties
        /// <summary>Which of the four outcomes this is.</summary>
        public OutcomeType Outcome
        {
            get { return outcome; }
            set { outcome = value; }
        }

        /// <summary>The stored code: STRONG, POSSIBLE, NCE or MOREINFO.</summary>
        public string Code
        {
            get { return GetCode(outcome); }
        }

        /// <summary>The words shown to people, e.g. "Not Currently Eligible" (class diagram "category").</summary>
        public string Category
        {
            get { return GetCategory(outcome); }
        }

        /// <summary>Why this outcome was given, naming the subjects or marks involved.</summary>
        public string Explanation
        {
            get { return explanation; }
            set { explanation = value; }
        }

        /// <summary>The weighted overall score (0 to 100), or null when it could not be calculated.</summary>
        public decimal? OverallScore
        {
            get { return overallScore; }
            set { overallScore = value; }
        }

        /// <summary>The case study definition of this outcome.</summary>
        public string Definition
        {
            get { return GetDefinition(outcome); }
        }
        #endregion

        #region Constructors
        /// <summary>Creates a More Information Needed outcome with no explanation yet.</summary>
        public AdvisoryOutcome()
        {
            outcome = OutcomeType.MoreInformationNeeded;
            explanation = "";
            overallScore = null;
        }

        /// <summary>Creates an outcome with its explanation and overall score.</summary>
        public AdvisoryOutcome(OutcomeType pOutcome, string pExplanation, decimal? pOverallScore)
        {
            outcome = pOutcome;
            explanation = pExplanation;
            overallScore = pOverallScore;
        }
        #endregion

        #region Methods
        /// <summary>Creates a More Information Needed outcome that says what is missing.</summary>
        public static AdvisoryOutcome MoreInformationNeeded(string reason)
        {
            return new AdvisoryOutcome(OutcomeType.MoreInformationNeeded, reason, null);
        }

        /// <summary>Returns the stored code for an outcome.</summary>
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

        /// <summary>Returns the display words for an outcome.</summary>
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

        /// <summary>Returns the display words for a stored code, e.g. "NCE" gives "Not Currently Eligible".</summary>
        public static string GetCategory(string code)
        {
            return GetCategory(FromCode(code));
        }

        /// <summary>Returns the case study definition of an outcome (§6.5).</summary>
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

        /// <summary>
        /// Turns a stored code back into an outcome. Throws ArgumentException for an unknown code,
        /// so a bad value is never silently shown as a different outcome.
        /// </summary>
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

        /// <summary>True if the code is one of the four stored outcome codes.</summary>
        public static bool IsValidCode(string code)
        {
            if (code == null)
            {
                return false;
            }
            string value = code.Trim().ToUpperInvariant();
            return value == CodeStrong || value == CodePossible || value == CodeNotEligible || value == CodeMoreInformation;
        }

        /// <summary>
        /// The order outcomes are listed in on screens and reports:
        /// 1 Strong, 2 Possible, 3 Not Currently Eligible, 4 More Information Needed.
        /// </summary>
        public static int GetDisplayOrder(OutcomeType type)
        {
            return 4 - (int)type;
        }

        /// <summary>The four outcomes in display order (Strong first).</summary>
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

        /// <summary>Returns "Category: explanation".</summary>
        public override string ToString()
        {
            return Category + ": " + explanation;
        }
        #endregion
    }
}
