
namespace FuturePath.ApplicationLayer
{
    // Links a degree programme to an interest category
    // Stores how relevant the category is to the programme
    public class DegreeInterestMapping
    {
        #region Data Members
        private int programmeID;
        private int categoryID;
        private string categoryName;
        private decimal relevanceWeight;
        #endregion

        #region Properties
        public int ProgrammeID
        {
            get { return programmeID; }
            set { programmeID = value; }
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

        // Weight used when calculating a learner's interest score
        public decimal RelevanceWeight
        {
            get { return relevanceWeight; }
            set { relevanceWeight = value; }
        }
        #endregion

        #region Constructors
        // Create a new mapping with default values
        // Set the relevance weight to 1
        public DegreeInterestMapping()
        {
            programmeID = 0;
            categoryID = 0;
            categoryName = "";
            relevanceWeight = 1;
        }

        // Create a mapping using its stored values
        public DegreeInterestMapping(int pProgrammeID, int pCategoryID, string pCategoryName, decimal pRelevanceWeight)
        {
            programmeID = pProgrammeID;
            categoryID = pCategoryID;
            categoryName = pCategoryName;
            relevanceWeight = pRelevanceWeight;
        }
        #endregion

        #region Methods
        // Create an InterestCategory using this mapping's category details
        // Include the relevance weight for programme matching
        public InterestCategory ToInterestCategory()
        {
            InterestCategory category = new InterestCategory(categoryID, categoryName, true);
            category.RelevanceWeight = relevanceWeight;
            return category;
        }
        #endregion
    }
}
