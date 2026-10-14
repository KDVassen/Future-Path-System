namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// A group of interest questions, e.g. "Business and finance". Maps to InterestCategory.
    /// When a category is loaded through a degree programme (DegreeProgramme.GetInterestCategories)
    /// it also carries that programme's relevance weight from DegreeInterestMapping, so the
    /// learner's interest score can weight each category (Learner.CalculateInterestScore).
    /// </summary>
    public class InterestCategory
    {
        #region Data Members
        private int categoryID;
        private string categoryName;
        private bool isActive;
        private decimal relevanceWeight;
        #endregion

        #region Properties
        /// <summary>Primary key (InterestCategory.CategoryID).</summary>
        public int CategoryID
        {
            get { return categoryID; }
            set { categoryID = value; }
        }

        /// <summary>The category name shown on UI 4 and in the reports.</summary>
        public string CategoryName
        {
            get { return categoryName; }
            set { categoryName = value; }
        }

        /// <summary>Inactive categories and their questions are not asked.</summary>
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        /// <summary>
        /// How relevant this category is to the programme it was loaded for (0.00 to 1.00,
        /// from DegreeInterestMapping.RelevanceWeight). It is 1 when the category was not
        /// loaded through a programme.
        /// </summary>
        public decimal RelevanceWeight
        {
            get { return relevanceWeight; }
            set { relevanceWeight = value; }
        }
        #endregion

        #region Constructors
        /// <summary>Creates an empty, active category with weight 1.</summary>
        public InterestCategory()
        {
            categoryID = 0;
            categoryName = "";
            isActive = true;
            relevanceWeight = 1;
        }

        /// <summary>Creates a category with all its stored values (weight 1).</summary>
        public InterestCategory(int pCategoryID, string pCategoryName, bool pIsActive)
        {
            categoryID = pCategoryID;
            categoryName = pCategoryName;
            isActive = pIsActive;
            relevanceWeight = 1;
        }
        #endregion

        #region Methods
        /// <summary>Returns the category name, so a ComboBox shows it.</summary>
        public override string ToString()
        {
            return categoryName;
        }
        #endregion
    }
}
