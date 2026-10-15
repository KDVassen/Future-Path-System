namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// The three counts on the UI 9 "Today" card, returned by SessionSearchController.GetDailySummary.
    /// A transient result: nothing is stored.
    /// </summary>
    public class DailySummary
    {
        #region Data Members
        private int completedCount;
        private int inProgressCount;
        private int exceptionCount;
        #endregion

        #region Properties
        /// <summary>Sessions completed (exported on UI 7) that day.</summary>
        public int CompletedCount
        {
            get { return completedCount; }
            set { completedCount = value; }
        }

        /// <summary>Sessions started that day that are still in progress.</summary>
        public int InProgressCount
        {
            get { return inProgressCount; }
            set { inProgressCount = value; }
        }

        /// <summary>Sessions completed that day with Not Currently Eligible or More Information Needed (follow-up list).</summary>
        public int ExceptionCount
        {
            get { return exceptionCount; }
            set { exceptionCount = value; }
        }
        #endregion

        #region Constructors
        /// <summary>Creates a summary with all counts 0.</summary>
        public DailySummary()
        {
            completedCount = 0;
            inProgressCount = 0;
            exceptionCount = 0;
        }

        /// <summary>Creates a summary with the three counts.</summary>
        public DailySummary(int pCompletedCount, int pInProgressCount, int pExceptionCount)
        {
            completedCount = pCompletedCount;
            inProgressCount = pInProgressCount;
            exceptionCount = pExceptionCount;
        }
        #endregion
    }
}
