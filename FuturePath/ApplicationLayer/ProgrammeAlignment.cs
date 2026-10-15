namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// The UI 5 interest-alignment strip for one programme: how well the learner's interests match
    /// it, and the learner's strongest matching categories. Returned by
    /// InterestController.GetProgrammeAlignment; nothing is stored.
    /// </summary>
    public class ProgrammeAlignment
    {
        #region Data Members
        private decimal alignmentPercent;
        private string strongestCategories;
        #endregion

        #region Properties
        /// <summary>The learner's interest score for the programme, 0 to 100.</summary>
        public decimal AlignmentPercent
        {
            get { return alignmentPercent; }
            set { alignmentPercent = value; }
        }

        /// <summary>Up to three category names, best first, separated by commas (or "").</summary>
        public string StrongestCategories
        {
            get { return strongestCategories; }
            set { strongestCategories = value; }
        }
        #endregion

        #region Constructors
        /// <summary>Creates an empty alignment (0%, no categories).</summary>
        public ProgrammeAlignment()
        {
            alignmentPercent = 0;
            strongestCategories = "";
        }

        /// <summary>Creates an alignment with its values.</summary>
        public ProgrammeAlignment(decimal pAlignmentPercent, string pStrongestCategories)
        {
            alignmentPercent = pAlignmentPercent;
            strongestCategories = pStrongestCategories;
        }
        #endregion
    }
}
