
namespace FuturePath.ApplicationLayer
{
    // Stores an interest category used in the learner questionnaire
    // Includes its relevance weight when matching interests to a programme
    public class InterestCategory
    {
        #region Data Members
        private int categoryID;
        private string categoryName;
        private bool isActive;
        private decimal relevanceWeight;
        #endregion

        #region Properties
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

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        // Store how relevant this category is to a degree programme
        // The default weight is 1 when no programme weight is provided
        public decimal RelevanceWeight
        {
            get { return relevanceWeight; }
            set { relevanceWeight = value; }
        }
        #endregion

        #region Constructors
        // Create a new active interest category with default values
        public InterestCategory()
        {
            categoryID = 0;
            categoryName = "";
            isActive = true;
            relevanceWeight = 1;
        }

        // Create an interest category using its stored details
        // Set the default relevance weight to 1
        public InterestCategory(int pCategoryID, string pCategoryName, bool pIsActive)
        {
            categoryID = pCategoryID;
            categoryName = pCategoryName;
            isActive = pIsActive;
            relevanceWeight = 1;
        }
        #endregion

        #region Methods
        // Return the category name for display
        public override string ToString()
        {
            return categoryName;
        }
        #endregion
    }
}
