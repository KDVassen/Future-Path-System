namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// Links a degree programme to an interest category with a relevance weight (0.00 to 1.00).
    /// Maps to DegreeInterestMapping, whose key is (ProgrammeID, CategoryID). The class diagram's
    /// mappingId is that composite key, so no separate ID column is stored.
    /// </summary>
    public class DegreeInterestMapping
    {
        #region Data Members
        private int programmeID;
        private int categoryID;
        private string categoryName;
        private decimal relevanceWeight;
        #endregion

        #region Properties
        /// <summary>The programme (part of the key).</summary>
        public int ProgrammeID
        {
            get { return programmeID; }
            set { programmeID = value; }
        }

        /// <summary>The interest category (part of the key).</summary>
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

        /// <summary>How relevant the category is to the programme, 0.00 to 1.00 (class diagram "weight").</summary>
        public decimal RelevanceWeight
        {
            get { return relevanceWeight; }
            set { relevanceWeight = value; }
        }
        #endregion

        #region Constructors
        /// <summary>Creates an empty mapping with weight 1.</summary>
        public DegreeInterestMapping()
        {
            programmeID = 0;
            categoryID = 0;
            categoryName = "";
            relevanceWeight = 1;
        }

        /// <summary>Creates a mapping with all its values.</summary>
        public DegreeInterestMapping(int pProgrammeID, int pCategoryID, string pCategoryName, decimal pRelevanceWeight)
        {
            programmeID = pProgrammeID;
            categoryID = pCategoryID;
            categoryName = pCategoryName;
            relevanceWeight = pRelevanceWeight;
        }
        #endregion

        #region Methods
        /// <summary>Turns the mapping into an InterestCategory that carries this programme's weight.</summary>
        public InterestCategory ToInterestCategory()
        {
            InterestCategory category = new InterestCategory(categoryID, categoryName, true);
            category.RelevanceWeight = relevanceWeight;
            return category;
        }
        #endregion
    }
}
