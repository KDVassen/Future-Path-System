
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FuturePath.ApplicationLayer
{
    // Calculates eligibility outcomes and recommendation scores
    // Uses programme requirements, interest scores and system settings
    public class RecommendationEngine
    {
        #region Data Members
        // Maximum length allowed for an outcome explanation
        private const int MaximumExplanationLength = 500;
        #endregion

        #region Constructor
        // Create the recommendation engine
        public RecommendationEngine()
        {
        }
        #endregion

        #region Methods
        // Calculate the weighted overall score using subjects, marks, interests and faculty
        // Keep the final score between 0 and 100
        public decimal CalculateOverallScore(MatchResult matchResult, decimal interestScore, bool facultyBonus)
        {
            decimal subjectScore = 0;
            decimal markScore = 0;
            if (matchResult != null)
            {
                subjectScore = matchResult.SubjectScore;
                markScore = matchResult.MarkScore;
            }
            decimal facultyScore = 0;
            if (facultyBonus)
            {
                facultyScore = 100;
            }

            // Combine the four scores using weights from the system settings
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

        // Determine the learner's eligibility outcome using the matching rules
        // Check missing subjects and mark gaps before using the overall score
        public AdvisoryOutcome ComputeOutcome(MatchResult matchResult, decimal interestScore, bool facultyBonus)
        {
            // Return More Information Needed when no subjects have been captured
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
                // Missing required subjects make the learner not currently eligible
                outcome = AdvisoryOutcome.OutcomeType.NotCurrentlyEligible;
                explanation = DescribeMissingSubjects(matchResult);
            }
            else if (matchResult.LargestShortfall > tolerance)
            {
                // A mark gap above the allowed tolerance makes the learner ineligible
                outcome = AdvisoryOutcome.OutcomeType.NotCurrentlyEligible;
                explanation = JoinSentences(matchResult.GapList) + " The largest shortfall (" + FormatNumber(matchResult.LargestShortfall)
                            + " points) is more than the " + FormatNumber(tolerance) + " points allowed for a possible match.";
            }
            else if (matchResult.LargestShortfall > 0 || !matchResult.AverageMet)
            {
                // A small mark gap or low average gives a possible match
                outcome = AdvisoryOutcome.OutcomeType.PossibleMatch;
                explanation = "Possible match: " + JoinSentences(matchResult.GapList);
                if (matchResult.LargestShortfall > 0)
                {
                    explanation += " This is within " + FormatNumber(tolerance) + " points of the minimum.";
                }
            }
            else if (overallScore >= SystemSettings.ThresholdStrong && interestScore >= SystemSettings.InterestStrongMinimum)
            {
                // Give a strong match when requirements and score thresholds are met
                outcome = AdvisoryOutcome.OutcomeType.StrongMatch;
                explanation = DescribeRequirementsMet(matchResult) + ", and your interests align strongly ("
                            + FormatNumber(interestScore) + "%).";
            }
            else if (interestScore < SystemSettings.InterestStrongMinimum)
            {
                // Give a possible match if the interest alignment is below the strong threshold
                outcome = AdvisoryOutcome.OutcomeType.PossibleMatch;
                explanation = "You meet the minimum requirements, but your interest alignment (" + FormatNumber(interestScore)
                            + "%) is weaker than a strong match needs (" + FormatNumber(SystemSettings.InterestStrongMinimum) + "%).";
            }
            else
            {
                // Give a possible match if the overall score is below the strong threshold
                outcome = AdvisoryOutcome.OutcomeType.PossibleMatch;
                explanation = "You meet the minimum requirements, but your overall score (" + FormatNumber(overallScore)
                            + "%) is below the " + FormatNumber(SystemSettings.ThresholdStrong) + "% a strong match needs.";
            }

            // Shorten explanations that exceed the database character limit
            if (explanation.Length > MaximumExplanationLength)
            {
                explanation = explanation.Substring(0, MaximumExplanationLength);
            }
            return new AdvisoryOutcome(outcome, explanation, overallScore);
        }

        // Rank alternative programmes by outcome, score and programme name
        // Assign each programme a rank starting from 1
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
        // Compare programmes by outcome first, then overall score and name
        private static int CompareCandidates(RecommendationResult a, RecommendationResult b)
        {
            // Put stronger outcomes before weaker outcomes
            int outcomeA = (int)AdvisoryOutcome.FromCode(a.OutcomeCode);
            int outcomeB = (int)AdvisoryOutcome.FromCode(b.OutcomeCode);
            if (outcomeA != outcomeB)
            {
                return outcomeB.CompareTo(outcomeA);
            }

            // Sort by overall score, with higher scores first
            // Programmes without scores appear last
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

            // Sort alphabetically when outcomes and scores are equal
            return string.Compare(a.ProgrammeName, b.ProgrammeName, StringComparison.OrdinalIgnoreCase);
        }

        // Create an explanation listing all missing required subjects
        // Include any other mark or average gaps
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

            // Add other gaps without repeating the missing subject messages
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

        // Create the beginning of the Strong Match explanation
        // Adjust the wording based on the number of required subjects
        private static string DescribeRequirementsMet(MatchResult matchResult)
        {
            int required = matchResult.RequirementsChecked;
            if (required == 0)
            {
                return "You meet the minimum requirements";
            }
            if (required == 1)
            {
                return "You meet the required subject and its minimum mark";
            }
            if (required == 2)
            {
                return "You meet both required subjects and their minimum marks";
            }
            return "You meet all " + required.ToString(CultureInfo.InvariantCulture) + " required subjects and their minimum marks";
        }

        // Join names using commas and 'and' before the last name
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

        // Combine multiple explanation sentences with spaces
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

        // Format numbers without unnecessary decimal zeros
        private static string FormatNumber(decimal value)
        {
            return value.ToString("0.#", CultureInfo.InvariantCulture);
        }
        #endregion
    }
}
