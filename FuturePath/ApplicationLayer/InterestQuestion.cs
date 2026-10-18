
namespace FuturePath.ApplicationLayer
{
    // Stores a question used in the learner's interest questionnaire
    // Each question belongs to an interest category and has a display order
    public class InterestQuestion
    {
        #region Data Members
        private int questionID;
        private int categoryID;
        private string categoryName;
        private string questionText;
        private int displayOrder;
        private bool isActive;
        #endregion

        #region Properties
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

        public string CategoryName
        {
            get { return categoryName; }
            set { categoryName = value; }
        }

        public string QuestionText
        {
            get { return questionText; }
            set { questionText = value; }
        }

        // Set the order in which the question appears in the questionnaire
        public int DisplayOrder
        {
            get { return displayOrder; }
            set { displayOrder = value; }
        }

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }
        #endregion

        #region Constructors
        // Create a new active question with default values
        public InterestQuestion()
        {
            questionID = 0;
            categoryID = 0;
            categoryName = "";
            questionText = "";
            displayOrder = 1;
            isActive = true;
        }

        // Create a question using its stored details
        public InterestQuestion(int pQuestionID, int pCategoryID, string pQuestionText, int pDisplayOrder, bool pIsActive)
        {
            questionID = pQuestionID;
            categoryID = pCategoryID;
            categoryName = "";
            questionText = pQuestionText;
            displayOrder = pDisplayOrder;
            isActive = pIsActive;
        }
        #endregion

        #region Methods
        // Return the question text for display
        public override string ToString()
        {
            return questionText;
        }
        #endregion
    }
}
