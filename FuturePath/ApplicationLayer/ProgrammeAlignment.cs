
namespace FuturePath.ApplicationLayer
{
    // Stores how well a learner's interests match a degree programme
    // Includes the alignment percentage and strongest interest categories
    public class ProgrammeAlignment
    {
        #region Data Members
        private decimal alignmentPercent;
        private string strongestCategories;
        #endregion

        #region Properties
        // Store the learner's interest alignment score from 0 to 100
        public decimal AlignmentPercent
        {
            get { return alignmentPercent; }
            set { alignmentPercent = value; }
        }

        // Store up to three of the learner's strongest matching categories
        public string StrongestCategories
        {
            get { return strongestCategories; }
            set { strongestCategories = value; }
        }
        #endregion

        #region Constructors
        // Create a new alignment with default values
        public ProgrammeAlignment()
        {
            alignmentPercent = 0;
            strongestCategories = "";
        }

        // Create an alignment using the calculated score and category names
        public ProgrammeAlignment(decimal pAlignmentPercent, string pStrongestCategories)
        {
            alignmentPercent = pAlignmentPercent;
            strongestCategories = pStrongestCategories;
        }
        #endregion
    }
}
