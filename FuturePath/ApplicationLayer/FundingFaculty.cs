namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// Links a funding option to a faculty it is offered for (FundingFaculty junction table).
    /// Loaded as a collection on FundingOption.
    /// </summary>
    public class FundingFaculty
    {
        #region Data Members
        private int fundingID;
        private int facultyID;
        private string facultyName;
        #endregion

        #region Properties
        /// <summary>The funding option (part of the key).</summary>
        public int FundingID
        {
            get { return fundingID; }
            set { fundingID = value; }
        }

        /// <summary>The faculty (part of the key).</summary>
        public int FacultyID
        {
            get { return facultyID; }
            set { facultyID = value; }
        }

        /// <summary>The faculty name (for display).</summary>
        public string FacultyName
        {
            get { return facultyName; }
            set { facultyName = value; }
        }
        #endregion

        #region Constructors
        /// <summary>Creates an empty link.</summary>
        public FundingFaculty()
        {
            fundingID = 0;
            facultyID = 0;
            facultyName = "";
        }

        /// <summary>Creates a link between a funding option and a faculty.</summary>
        public FundingFaculty(int pFundingID, int pFacultyID, string pFacultyName)
        {
            fundingID = pFundingID;
            facultyID = pFacultyID;
            facultyName = pFacultyName;
        }
        #endregion
    }
}
