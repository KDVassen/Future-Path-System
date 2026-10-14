namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// The learner's answer to one interest statement on UI 4: 1 = Strongly disagree up to
    /// 5 = Strongly agree. Responses belong to the session. Maps to LearnerInterestResponse;
    /// CategoryID is read from the question so the answer can be scored per category.
    /// </summary>
    public class LearnerInterestResponse
    {
        #region Data Members
        /// <summary>The lowest response value (Strongly disagree).</summary>
        public const int MinimumResponse = 1;
        /// <summary>The highest response value (Strongly agree).</summary>
        public const int MaximumResponse = 5;

        private int responseID;
        private int sessionID;
        private int questionID;
        private int categoryID;
        private int responseValue;
        #endregion

        #region Properties
        /// <summary>Primary key (LearnerInterestResponse.ResponseID).</summary>
        public int ResponseID
        {
            get { return responseID; }
            set { responseID = value; }
        }

        /// <summary>The advisory session the answer was given in.</summary>
        public int SessionID
        {
            get { return sessionID; }
            set { sessionID = value; }
        }

        /// <summary>The question answered (FK to InterestQuestion).</summary>
        public int QuestionID
        {
            get { return questionID; }
            set { questionID = value; }
        }

        /// <summary>The question's category (read from InterestQuestion, not stored on this row).</summary>
        public int CategoryID
        {
            get { return categoryID; }
            set { categoryID = value; }
        }

        /// <summary>The answer, 1 to 5.</summary>
        public int ResponseValue
        {
            get { return responseValue; }
            set { responseValue = value; }
        }
        #endregion

        #region Constructors
        /// <summary>Creates an empty response.</summary>
        public LearnerInterestResponse()
        {
            responseID = 0;
            sessionID = 0;
            questionID = 0;
            categoryID = 0;
            responseValue = MinimumResponse;
        }

        /// <summary>Creates a response with all its values.</summary>
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
        /// <summary>True if the answer is from 1 to 5.</summary>
        public bool ValidateResponseRange()
        {
            return IsValidResponse(responseValue);
        }

        /// <summary>True if the value is from 1 to 5.</summary>
        public static bool IsValidResponse(int value)
        {
            return value >= MinimumResponse && value <= MaximumResponse;
        }

        /// <summary>
        /// The answer as a percentage: 1 = 0, 2 = 25, 3 = 50, 4 = 75, 5 = 100.
        /// </summary>
        public decimal GetScore()
        {
            return (responseValue - MinimumResponse) * 100m / (MaximumResponse - MinimumResponse);
        }
        #endregion
    }
}
