using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// The recommendation engine: plain C# rules and scoring, no external service (CLAUDE.md).
    /// The outcome is rule-driven first, then score-driven. A missing Required subject can never
    /// be a match, whatever the score. Weights and thresholds are SystemParameter rows read
    /// through SystemSettings (provisional values, decisions.md D14).
    /// </summary>
    public class RecommendationEngine
    {
        #region Data Members
        // The longest explanation that fits RecommendationResult.OutcomeReason (VARCHAR(500))
        private const int MaximumExplanationLength = 500;
        #endregion

        #region Constructor
        /// <summary>Creates the engine.</summary>
        public RecommendationEngine()
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// The weighted overall score, 0-100:
        /// WEIGHT_SUBJECT x SubjectScore + WEIGHT_MARK x MarkScore + WEIGHT_INTEREST x interestScore
        /// + WEIGHT_FACULTY x (100 if facultyBonus, else 0), rounded to two decimals and kept
        /// between 0 and 100. A missing matchResult counts as 0 for the subject and mark parts.
        /// </summary>
        public decimal CalculateOverallScore(MatchResult matchResult, decimal interestScore, bool facultyBonus)
        {
            decimal subjectScore = 0;
            decimal markScore = 0;
            if (matchResult != null)
            {
                subjectScore = matchResult.SubjectScore;
                markScore = matchResult.MarkScore;
            }
            decimal facultyScore = facultyBonus ? 100 : 0;

            // The four weighted parts (application-layer.md §4)
            decimal overall = SystemSettings.WeightSubject * subjectScore
                            + SystemSettings.WeightMark * markScore
                            + SystemSettings.WeightInterest * interestScore
                            + SystemSettings.WeightFaculty * facultyScore;

            overall = Math.Round(overall, 2, MidpointRounding.AwayFromZero);
            if (overall < 0)
            {
                overall = 0;
            }
            if (overall > 100)
            {
                overall = 100;
            }
            return overall;
        }

        /// <summary>
        /// Class diagram computeOutcome(matchResult, interestScore, facultyBonus). Rules in order:
        /// 1. no subjects captured: More Information Needed;
        /// 2. a Required subject missing (no accepted alternative): Not Currently Eligible, naming every one;
        /// 3. a Required mark more than MARK_TOLERANCE below its minimum: Not Currently Eligible;
        /// 4. a shortfall within the tolerance, or the average below the programme minimum: Possible Match, naming the gaps;
        /// 5. otherwise Strong Match if the overall score is at least THRESHOLD_STRONG and the
        ///    interest score at least INTEREST_STRONG_MIN, else Possible Match (weaker interest alignment).
        /// The explanation always says why (at most 500 characters). OverallScore is set on every
        /// outcome except More Information Needed (null).
        /// </summary>
        public AdvisoryOutcome ComputeOutcome(MatchResult matchResult, decimal interestScore, bool facultyBonus)
        {
            // Rule 1: nothing to check without subjects
            if (matchResult == null || matchResult.SubjectsCaptured == 0)
            {
                return AdvisoryOutcome.MoreInformationNeeded("Capture at least one subject and mark (step 2) so your eligibility can be checked.");
            }

            decimal overallScore = CalculateOverallScore(matchResult, interestScore, facultyBonus);
            decimal tolerance = SystemSettings.MarkTolerance;
            AdvisoryOutcome.OutcomeType outcome;
            string explanation;

            if (matchResult.HasMissingRequired)
            {
                // Rule 2: a missing Required subject can never be a match
                outcome = AdvisoryOutcome.OutcomeType.NotCurrentlyEligible;
                explanation = DescribeMissingSubjects(matchResult);
            }
            else if (matchResult.LargestShortfall > tolerance)
            {
                // Rule 3: a mark too far below its minimum
                outcome = AdvisoryOutcome.OutcomeType.NotCurrentlyEligible;
                explanation = JoinSentences(matchResult.GapList) + " The largest shortfall (" + FormatNumber(matchResult.LargestShortfall)
                            + " points) is more than the " + FormatNumber(tolerance) + " points allowed for a possible match.";
            }
            else if (matchResult.LargestShortfall > 0 || !matchResult.AverageMet)
            {
                // Rule 4: a small gap, or the average below the programme minimum
                outcome = AdvisoryOutcome.OutcomeType.PossibleMatch;
                explanation = "Possible match: " + JoinSentences(matchResult.GapList);
                if (matchResult.LargestShortfall > 0)
                {
                    explanation += " This is within " + FormatNumber(tolerance) + " points of the minimum.";
                }
            }
            else if (overallScore >= SystemSettings.ThresholdStrong && interestScore >= SystemSettings.InterestStrongMinimum)
            {
                // Rule 5: requirements met and a strong score
                outcome = AdvisoryOutcome.OutcomeType.StrongMatch;
                explanation = DescribeRequirementsMet(matchResult) + ", and your interests align strongly ("
                            + FormatNumber(interestScore) + "%).";
            }
            else if (interestScore < SystemSettings.InterestStrongMinimum)
            {
                // Rule 5: requirements met, but the interests are weaker
                outcome = AdvisoryOutcome.OutcomeType.PossibleMatch;
                explanation = "You meet the minimum requirements, but your interest alignment (" + FormatNumber(interestScore)
                            + "%) is weaker than a strong match needs (" + FormatNumber(SystemSettings.InterestStrongMinimum) + "%).";
            }
            else
            {
                // Rule 5: requirements met, but the overall score is below the Strong threshold
                outcome = AdvisoryOutcome.OutcomeType.PossibleMatch;
                explanation = "You meet the minimum requirements, but your overall score (" + FormatNumber(overallScore)
                            + "%) is below the " + FormatNumber(SystemSettings.ThresholdStrong) + "% a strong match needs.";
            }

            // Recommended subjects only add advice; they never change the outcome
            if (matchResult.Notes.Count > 0)
            {
                explanation += " Advice: " + JoinSentences(matchResult.Notes);
            }
            return new AdvisoryOutcome(outcome, LimitLength(explanation), overallScore);
        }

        /// <summary>
        /// Class diagram rankAlternatives(candidateOutcomes). Orders the candidates by outcome
        /// (Strong, Possible, Not Currently Eligible, More Information Needed), then by overall
        /// score (highest first, no score last), then by programme name, and numbers
        /// AlternativeRank from 1. Returns a new collection; an empty one when there are no candidates.
        /// </summary>
        public Collection<RecommendationResult> RankAlternatives(Collection<RecommendationResult> candidateOutcomes)
        {
            List<RecommendationResult> sorted = new List<RecommendationResult>();
            if (candidateOutcomes != null)
            {
                foreach (RecommendationResult candidate in candidateOutcomes)
                {
                    sorted.Add(candidate);
                }
            }
            sorted.Sort(CompareCandidates);

            Collection<RecommendationResult> ranked = new Collection<RecommendationResult>();
            int rank = 1;
            foreach (RecommendationResult candidate in sorted)
            {
                candidate.AlternativeRank = rank;
                ranked.Add(candidate);
                rank++;
            }
            return ranked;
        }
        #endregion

        #region Helpers
        /// <summary>
        /// The ranking order used by RankAlternatives. A negative number puts a before b.
        /// </summary>
        private static int CompareCandidates(RecommendationResult a, RecommendationResult b)
        {
            // 1. Outcome: the enum numbers go from weakest (0) to strongest (3), so the higher number comes first
            int outcomeA = (int)AdvisoryOutcome.FromCode(a.OutcomeCode);
            int outcomeB = (int)AdvisoryOutcome.FromCode(b.OutcomeCode);
            if (outcomeA != outcomeB)
            {
                return outcomeB.CompareTo(outcomeA);
            }

            // 2. Overall score, highest first; a result without a score goes last
            if (a.OverallScore.HasValue && !b.OverallScore.HasValue)
            {
                return -1;
            }
            if (!a.OverallScore.HasValue && b.OverallScore.HasValue)
            {
                return 1;
            }
            if (a.OverallScore.HasValue && b.OverallScore.HasValue && a.OverallScore.Value != b.OverallScore.Value)
            {
                return b.OverallScore.Value.CompareTo(a.OverallScore.Value);
            }

            // 3. Programme name, A to Z
            return string.Compare(a.ProgrammeName, b.ProgrammeName, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Rule 2's explanation: every missing Required subject in one sentence, e.g.
        /// "Mathematics and Physical Sciences are required but were not captured.", followed by
        /// any other gaps (marks below a minimum, an average below the programme minimum).
        /// </summary>
        private static string DescribeMissingSubjects(MatchResult matchResult)
        {
            string text = JoinNames(matchResult.MissingRequired);
            if (matchResult.MissingRequired.Count == 1)
            {
                text += " is required but was not captured.";
            }
            else
            {
                text += " are required but were not captured.";
            }

            // The GapList already has one sentence per missing subject; skip those and keep the rest
            Collection<string> missingSentences = new Collection<string>();
            foreach (RequirementCheck check in matchResult.Checks)
            {
                if (check.IsRequired && check.ResultCode == RequirementCheck.ResultMissing)
                {
                    missingSentences.Add(check.Describe());
                }
            }
            foreach (string sentence in matchResult.GapList)
            {
                if (!missingSentences.Contains(sentence))
                {
                    text += " " + sentence;
                }
            }
            return text;
        }

        /// <summary>The start of the Strong Match explanation, e.g. "You meet all 3 required subjects and minimum marks".</summary>
        private static string DescribeRequirementsMet(MatchResult matchResult)
        {
            int required = matchResult.RequirementsChecked;
            if (required == 0)
            {
                return "You meet the minimum requirements";
            }
            if (required == 1)
            {
                return "You meet the 1 required subject and its minimum mark";
            }
            return "You meet all " + required.ToString(CultureInfo.InvariantCulture) + " required subjects and minimum marks";
        }

        /// <summary>Joins names as "A", "A and B" or "A, B and C".</summary>
        private static string JoinNames(Collection<string> names)
        {
            string text = "";
            for (int i = 0; i < names.Count; i++)
            {
                if (i > 0 && i == names.Count - 1)
                {
                    text += " and ";
                }
                else if (i > 0)
                {
                    text += ", ";
                }
                text += names[i];
            }
            return text;
        }

        /// <summary>Joins full sentences with a space between them.</summary>
        private static string JoinSentences(Collection<string> sentences)
        {
            string text = "";
            foreach (string sentence in sentences)
            {
                if (text.Length > 0)
                {
                    text += " ";
                }
                text += sentence;
            }
            return text;
        }

        /// <summary>Cuts an explanation to 500 characters (ending in "...") so it always fits OutcomeReason.</summary>
        private static string LimitLength(string text)
        {
            if (text.Length <= MaximumExplanationLength)
            {
                return text;
            }
            return text.Substring(0, MaximumExplanationLength - 3) + "...";
        }

        /// <summary>Formats a number without trailing zeros (58, 57.5), the same on any regional setting.</summary>
        private static string FormatNumber(decimal value)
        {
            return value.ToString("0.#", CultureInfo.InvariantCulture);
        }
        #endregion
    }
}
