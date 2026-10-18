
namespace FuturePath.ApplicationLayer
{
    // Stores a learner's answer to an interest questionnaire question
    // Each response belongs to an advisory session and an interest category
    public class LearnerInterestResponse
    {
        #region Data Members
        // Valid response values range from 1 to 5
        public const int MinimumResponse = 1;
        public const int MaximumResponse = 5;

        private int responseID;
        private int sessionID;
        private int questionID;
        private int categoryID;
        private int responseValue;
        #endregion

        #region Properties
        public int ResponseID
        {
            get { return responseID; }
            set { responseID = value; }
        }

        public int SessionID
        {
            get { return sessionID; }
            set { sessionID = value; }
        }

        public int QuestionID
        {
            get { return questionID; }
            set { questionID = value; }
        }

        public int CategoryID
        {
            get { return categoryID; }
            set { categoryID = value; }
        }

        public int ResponseValue
        {
            get { return responseValue; }
            set { responseValue = value; }
        }
        #endregion

        #region Constructors
        // Create a new response with default values
        public LearnerInterestResponse()
        {
            responseID = 0;
            sessionID = 0;
            questionID = 0;
            categoryID = 0;
            responseValue = MinimumResponse;
        }

        // Create a response using its saved values
        public LearnerInterestResponse(int pResponseID, int pSessionID, int pQuestionID, int pCategoryID, int pResponseValue)
        {
            responseID = pResponseID;
            sessionID = pSessionID;
            questionID = pQuestionID;
            categoryID = pCategoryID;
            responseValue = pResponseValue;
        }
        #endregion

        #region Methods
        // Check whether the learner's response is between 1 and 5
        public bool ValidateResponseRange()
        {
            return IsValidResponse(responseValue);
        }

        // Check if the given response value is valid
        public static bool IsValidResponse(int value)
        {
            return value >= MinimumResponse && value <= MaximumResponse;
        }

        // Convert the answer into a percentage score
        // Values from 1 to 5 become 0, 25, 50, 75 and 100
        public decimal GetScore()
        {
            return (responseValue - MinimumResponse) * 100m / (MaximumResponse - MinimumResponse);
        }
        #endregion
    }
}
