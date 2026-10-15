namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// One statement on the interest questionnaire (UI 4), e.g. "I enjoy working with numbers
    /// and data". Each question belongs to one interest category and is answered on a 1-5 scale.
    /// Maps to InterestQuestion; DisplayOrder fixes the order the statements appear in.
    /// </summary>
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
        /// <summary>Primary key (InterestQuestion.QuestionID).</summary>
        public int QuestionID
        {
            get { return questionID; }
            set { questionID = value; }
        }

        /// <summary>The category this question scores (FK to InterestCategory).</summary>
        public int CategoryID
        {
            get { return categoryID; }
            set { categoryID = value; }
        }

        /// <summary>The category name (read from InterestCategory, for display).</summary>
        public string CategoryName
        {
            get { return categoryName; }
            set { categoryName = value; }
        }

        /// <summary>The statement the learner agrees or disagrees with.</summary>
        public string QuestionText
        {
            get { return questionText; }
            set { questionText = value; }
        }

        /// <summary>Position on UI 4 (1 to 99, lowest first).</summary>
        public int DisplayOrder
        {
            get { return displayOrder; }
            set { displayOrder = value; }
        }

        /// <summary>Inactive questions are not asked.</summary>
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }
        #endregion

        #region Constructors
        /// <summary>Creates an empty, active question.</summary>
        public InterestQuestion()
        {
            questionID = 0;
            categoryID = 0;
            categoryName = "";
            questionText = "";
            displayOrder = 1;
            isActive = true;
        }

        /// <summary>Creates a question with all its stored values.</summary>
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
        /// <summary>Returns the statement text.</summary>
        public override string ToString()
        {
            return questionText;
        }
        #endregion
    }
}
