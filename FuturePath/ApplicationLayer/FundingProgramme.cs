namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// Links a funding option to a degree programme it is offered for (FundingProgramme
    /// junction table). Loaded as a collection on FundingOption.
    /// </summary>
    public class FundingProgramme
    {
        #region Data Members
        private int fundingID;
        private int programmeID;
        private string programmeName;
        #endregion

        #region Properties
        /// <summary>The funding option (part of the key).</summary>
        public int FundingID
        {
            get { return fundingID; }
            set { fundingID = value; }
        }

        /// <summary>The degree programme (part of the key).</summary>
        public int ProgrammeID
        {
            get { return programmeID; }
            set { programmeID = value; }
        }

        /// <summary>The programme name (for display).</summary>
        public string ProgrammeName
        {
            get { return programmeName; }
            set { programmeName = value; }
        }
        #endregion

        #region Constructors
        /// <summary>Creates an empty link.</summary>
        public FundingProgramme()
        {
            fundingID = 0;
            programmeID = 0;
            programmeName = "";
        }

        /// <summary>Creates a link between a funding option and a programme.</summary>
        public FundingProgramme(int pFundingID, int pProgrammeID, string pProgrammeName)
        {
            fundingID = pFundingID;
            programmeID = pProgrammeID;
            programmeName = pProgrammeName;
        }
        #endregion
    }
}
