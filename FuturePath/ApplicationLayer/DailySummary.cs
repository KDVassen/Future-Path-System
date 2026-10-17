
namespace FuturePath.ApplicationLayer
{
    // Stores the daily session counts shown on the staff dashboard
    // Includes completed, in-progress and exception sessions
    public class DailySummary
    {
        #region Data Members
        private int completedCount;
        private int inProgressCount;
        private int exceptionCount;
        #endregion

        #region Properties
        public int CompletedCount
        {
            get { return completedCount; }
            set { completedCount = value; }
        }

        public int InProgressCount
        {
            get { return inProgressCount; }
            set { inProgressCount = value; }
        }

        // Count completed sessions that need follow-up
        // Includes Not Currently Eligible and More Information Needed outcomes
        public int ExceptionCount
        {
            get { return exceptionCount; }
            set { exceptionCount = value; }
        }
        #endregion

        #region Constructors
        // Create a daily summary with all counts set to zero
        public DailySummary()
        {
            completedCount = 0;
            inProgressCount = 0;
            exceptionCount = 0;
        }

        // Create a daily summary using the three session counts
        public DailySummary(int pCompletedCount, int pInProgressCount, int pExceptionCount)
        {
            completedCount = pCompletedCount;
            inProgressCount = pInProgressCount;
            exceptionCount = pExceptionCount;
        }
        #endregion
    }
}
