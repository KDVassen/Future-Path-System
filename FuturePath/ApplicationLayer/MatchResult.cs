
using System.Collections.ObjectModel;

namespace FuturePath.ApplicationLayer
{
    // Stores the results of comparing a learner's subjects and marks with a degree programme
    // Keeps track of scores, missing requirements and any mark shortfalls
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
        // Percentage of required subjects captured by the learner
        // Returns 100 when the programme has no required subjects
        public decimal SubjectScore
        {
            get { return subjectScore; }
            set { subjectScore = value; }
        }

        // Percentage of required minimum marks that were met
        // Returns 100 when no minimum marks are required
        public decimal MarkScore
        {
            get { return markScore; }
            set { markScore = value; }
        }

        // Store the requirements that the learner has not met
        public Collection<string> GapList
        {
            get { return gapList; }
        }

        // Store advice for recommended requirements that were not met
        public Collection<string> Notes
        {
            get { return notes; }
        }

        // Store the results of checking each active programme requirement
        public Collection<RequirementCheck> Checks
        {
            get { return checks; }
        }

        // Store the names of required subjects that are missing
        public Collection<string> MissingRequired
        {
            get { return missingRequired; }
        }

        // Store the largest difference between a required mark and the learner's mark
        public decimal LargestShortfall
        {
            get { return largestShortfall; }
            set { largestShortfall = value; }
        }

        public int SubjectsCaptured
        {
            get { return subjectsCaptured; }
            set { subjectsCaptured = value; }
        }

        public decimal? OverallAverage
        {
            get { return overallAverage; }
            set { overallAverage = value; }
        }

        public decimal? MinimumOverallAverage
        {
            get { return minimumOverallAverage; }
            set { minimumOverallAverage = value; }
        }

        // Check if the learner meets the programme's minimum overall average
        // Return true if the programme does not require a minimum average
        public bool AverageMet
        {
            get { return !minimumOverallAverage.HasValue || (overallAverage.HasValue && overallAverage.Value >= minimumOverallAverage.Value); }
        }

        // Calculate how far the learner's average is below the required average
        // Return null if the minimum average has been met
        public decimal? AverageShortfall
        {
            get
            {
                if (AverageMet)
                {
                    return null;
                }
                decimal average = 0;
                if (overallAverage.HasValue)
                {
                    average = overallAverage.Value;
                }
                return minimumOverallAverage.Value - average;
            }
        }

        // Check if any required subjects are missing
        public bool HasMissingRequired
        {
            get { return missingRequired.Count > 0; }
        }

        // Count the number of required programme requirements checked
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

        // Count the number of required programme requirements met
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
        // Create an empty match result and initialise the collections
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
