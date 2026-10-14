using System.Collections.ObjectModel;

namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// How well a learner's subjects and marks match one degree programme (class diagram
    /// MatchResult: subjectScore, markScore, gapList). A transient value made by
    /// DegreeProgramme.EvaluateSubjectAndMarkMatch and passed to RecommendationEngine.ComputeOutcome.
    /// It also keeps each requirement's check, the subjects missing and the overall average,
    /// because the rule-driven outcome and FP-LSR need them.
    /// </summary>
    public class MatchResult
    {
        #region Data Members
        private decimal subjectScore;
        private decimal markScore;
        private Collection<string> gapList;
        private Collection<string> notes;
        private Collection<RequirementCheck> checks;
        private Collection<string> missingRequired;
        private decimal largestShortfall;
        private int subjectsCaptured;
        private decimal? overallAverage;
        private decimal? minimumOverallAverage;
        #endregion

        #region Properties
        /// <summary>
        /// % of the programme's active Required requirements whose subject (or accepted
        /// alternative) was captured. 100 when the programme has no Required requirement.
        /// </summary>
        public decimal SubjectScore
        {
            get { return subjectScore; }
            set { subjectScore = value; }
        }

        /// <summary>
        /// % of the Required requirements with a minimum mark that were met.
        /// 100 when no Required requirement has a minimum mark.
        /// </summary>
        public decimal MarkScore
        {
            get { return markScore; }
            set { markScore = value; }
        }

        /// <summary>One sentence per unmet Required requirement (and an average below the minimum).</summary>
        public Collection<string> GapList
        {
            get { return gapList; }
        }

        /// <summary>One sentence per unmet Recommended requirement. Advice only; never decides the outcome.</summary>
        public Collection<string> Notes
        {
            get { return notes; }
        }

        /// <summary>The check of every active requirement, in requirement order.</summary>
        public Collection<RequirementCheck> Checks
        {
            get { return checks; }
        }

        /// <summary>Names of Required subjects missing with no accepted alternative.</summary>
        public Collection<string> MissingRequired
        {
            get { return missingRequired; }
        }

        /// <summary>The biggest shortfall (points) on a Required mark, 0 if none.</summary>
        public decimal LargestShortfall
        {
            get { return largestShortfall; }
            set { largestShortfall = value; }
        }

        /// <summary>How many subjects the learner captured.</summary>
        public int SubjectsCaptured
        {
            get { return subjectsCaptured; }
            set { subjectsCaptured = value; }
        }

        /// <summary>The learner's average over every captured subject (one decimal), or null if none.</summary>
        public decimal? OverallAverage
        {
            get { return overallAverage; }
            set { overallAverage = value; }
        }

        /// <summary>The programme's minimum overall average, or null if it sets none.</summary>
        public decimal? MinimumOverallAverage
        {
            get { return minimumOverallAverage; }
            set { minimumOverallAverage = value; }
        }

        /// <summary>True if the programme sets no minimum average, or the learner's average reaches it.</summary>
        public bool AverageMet
        {
            get { return !minimumOverallAverage.HasValue || (overallAverage.HasValue && overallAverage.Value >= minimumOverallAverage.Value); }
        }

        /// <summary>Points below the minimum average, or null when it was met.</summary>
        public decimal? AverageShortfall
        {
            get
            {
                if (AverageMet)
                {
                    return null;
                }
                return minimumOverallAverage.Value - (overallAverage ?? 0);
            }
        }

        /// <summary>True if any Required subject is missing.</summary>
        public bool HasMissingRequired
        {
            get { return missingRequired.Count > 0; }
        }

        /// <summary>How many Required requirements were checked.</summary>
        public int RequirementsChecked
        {
            get
            {
                int count = 0;
                foreach (RequirementCheck check in checks)
                {
                    if (check.IsRequired)
                    {
                        count++;
                    }
                }
                return count;
            }
        }

        /// <summary>How many Required requirements were met.</summary>
        public int RequirementsMet
        {
            get
            {
                int count = 0;
                foreach (RequirementCheck check in checks)
                {
                    if (check.IsRequired && check.IsMet)
                    {
                        count++;
                    }
                }
                return count;
            }
        }
        #endregion

        #region Constructors
        /// <summary>Creates an empty match result.</summary>
        public MatchResult()
        {
            subjectScore = 0;
            markScore = 0;
            gapList = new Collection<string>();
            notes = new Collection<string>();
            checks = new Collection<RequirementCheck>();
            missingRequired = new Collection<string>();
            largestShortfall = 0;
            subjectsCaptured = 0;
            overallAverage = null;
            minimumOverallAverage = null;
        }
        #endregion
    }
}
