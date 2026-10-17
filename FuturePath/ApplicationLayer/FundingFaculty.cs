
namespace FuturePath.ApplicationLayer
{
    // Links a funding option to a university faculty
    // Stores the faculty details needed when displaying funding information
    public class FundingFaculty
    {
        #region Data Members
        private int fundingID;
        private int facultyID;
        private string facultyName;
        #endregion

        #region Properties
        public int FundingID
        {
            get { return fundingID; }
            set { fundingID = value; }
        }

        public int FacultyID
        {
            get { return facultyID; }
            set { facultyID = value; }
        }

        public string FacultyName
        {
            get { return facultyName; }
            set { facultyName = value; }
        }
        #endregion

        #region Constructors
        // Create a new funding and faculty link with default values
        public FundingFaculty()
        {
            fundingID = 0;
            facultyID = 0;
            facultyName = "";
        }

        // Create a link between a funding option and a faculty
        public FundingFaculty(int pFundingID, int pFacultyID, string pFacultyName)
        {
            fundingID = pFundingID;
            facultyID = pFacultyID;
            facultyName = pFacultyName;
        }
        #endregion
    }
}
