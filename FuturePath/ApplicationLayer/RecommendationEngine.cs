using System;
using System.Collections.ObjectModel;

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
        /// + WEIGHT_FACULTY x (100 if facultyBonus, else 0), rounded to two decimals.
        /// </summary>
        public decimal CalculateOverallScore(MatchResult matchResult, decimal interestScore, bool facultyBonus)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Class diagram computeOutcome(matchResult, interestScore, facultyBonus). Rules in order:
        /// 1. no subjects captured: More Information Needed;
        /// 2. a Required subject missing (no accepted alternative): Not Currently Eligible, naming every one;
        /// 3. a Required mark more than MARK_TOLERANCE below its minimum: Not Currently Eligible;
        /// 4. a shortfall within the tolerance, or the average below the programme minimum: Possible Match, naming the gaps;
        /// 5. otherwise Strong Match if the overall score is at least THRESHOLD_STRONG and the
        ///    interest score at least INTEREST_STRONG_MIN, else Possible Match (weaker interest alignment).
        /// The explanation always says why.
        /// </summary>
        public AdvisoryOutcome ComputeOutcome(MatchResult matchResult, decimal interestScore, bool facultyBonus)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Class diagram rankAlternatives(candidateOutcomes). Orders the candidates by outcome
        /// (Strong, Possible, Not Currently Eligible, More Information Needed), then by overall
        /// score (highest first), then by programme name, and numbers AlternativeRank from 1.
        /// </summary>
        public Collection<RecommendationResult> RankAlternatives(Collection<RecommendationResult> candidateOutcomes)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
