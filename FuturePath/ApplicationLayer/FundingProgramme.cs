
namespace FuturePath.ApplicationLayer
{
    // Links a funding option to a degree programme
    // Stores the programme details needed when displaying funding information
    public class FundingProgramme
    {
        #region Data Members
        private int fundingID;
        private int programmeID;
        private string programmeName;
        #endregion

        #region Properties
        public int FundingID
        {
            get { return fundingID; }
            set { fundingID = value; }
        }

        public int ProgrammeID
        {
            get { return programmeID; }
            set { programmeID = value; }
        }

        public string ProgrammeName
        {
            get { return programmeName; }
            set { programmeName = value; }
        }
        #endregion

        #region Constructors
        // Create a new funding and programme link with default values
        public FundingProgramme()
        {
            fundingID = 0;
            programmeID = 0;
            programmeName = "";
        }

        // Create a link between a funding option and a degree programme
        public FundingProgramme(int pFundingID, int pProgrammeID, string pProgrammeName)
        {
            fundingID = pFundingID;
            programmeID = pProgrammeID;
            programmeName = pProgrammeName;
        }
        #endregion
    }
}
