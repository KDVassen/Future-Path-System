
using System;
using System.Collections.ObjectModel;

namespace FuturePath.ApplicationLayer
{
    // Stores information about a bursary, scholarship or other funding option
    // Funding can be linked to specific faculties and degree programmes
    public class FundingOption
    {
        #region Data Members
        private int fundingID;
        private string fundingName;
        private string fundingType;
        private string eligibilityDescription;
        private string requiredDocuments;
        private DateTime? closingDate;
        private string contactInformation;
        private bool isMeritBased;
        private bool isNeedBased;
        private bool isActive;
        private Collection<FundingFaculty> facultyLinks;
        private Collection<FundingProgramme> programmeLinks;
        #endregion

        #region Properties
        public int FundingID
        {
            get { return fundingID; }
            set { fundingID = value; }
        }

        public string FundingName
        {
            get { return fundingName; }
            set { fundingName = value; }
        }

        public string FundingType
        {
            get { return fundingType; }
            set { fundingType = value; }
        }

        public string EligibilityDescription
        {
            get { return eligibilityDescription; }
            set { eligibilityDescription = value; }
        }

        public string RequiredDocuments
        {
            get { return requiredDocuments; }
            set { requiredDocuments = value; }
        }

        public DateTime? ClosingDate
        {
            get { return closingDate; }
            set { closingDate = value; }
        }

        public string ContactInformation
        {
            get { return contactInformation; }
            set { contactInformation = value; }
        }

        public bool IsMeritBased
        {
            get { return isMeritBased; }
            set { isMeritBased = value; }
        }

        public bool IsNeedBased
        {
            get { return isNeedBased; }
            set { isNeedBased = value; }
        }

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        // Return the faculties linked to this funding option
        public Collection<FundingFaculty> FacultyLinks
        {
            get { return facultyLinks; }
        }

        // Return the programmes linked to this funding option
        public Collection<FundingProgramme> ProgrammeLinks
        {
            get { return programmeLinks; }
        }

        // Collect the IDs of all linked faculties
        public Collection<int> FacultyIds
        {
            get
            {
                Collection<int> ids = new Collection<int>();
                foreach (FundingFaculty link in facultyLinks)
                {
                    ids.Add(link.FacultyID);
                }
                return ids;
            }
        }

        // Collect the IDs of all linked degree programmes
        public Collection<int> ProgrammeIds
        {
            get
            {
                Collection<int> ids = new Collection<int>();
                foreach (FundingProgramme link in programmeLinks)
                {
                    ids.Add(link.ProgrammeID);
                }
                return ids;
            }
        }

        // Combine the names of linked faculties and programmes
        // Separate multiple names using commas
        public string LinkedTo
        {
            get
            {
                string text = "";
                foreach (FundingFaculty link in facultyLinks)
                {
                    if (text.Length > 0)
                    {
                        text += ", ";
                    }
                    text += link.FacultyName;
                }
                foreach (FundingProgramme link in programmeLinks)
                {
                    if (text.Length > 0)
                    {
                        text += ", ";
                    }
                    text += link.ProgrammeName;
                }
                return text;
            }
        }

        // Check if applications are still open today
        public bool IsOpen
        {
            get { return IsOpenOn(DateTime.Today); }
        }

        // Return the funding application status
        public string Status
        {
            get
            {
                if (IsOpen)
                {
                    return "Open";
                }
                return "Closed";
            }
        }
        #endregion

        #region Constructors
        // Create an active funding option with default values
        // Initialise the faculty and programme collections
        public FundingOption()
        {
            fundingID = 0;
            fundingName = "";
            fundingType = "";
            eligibilityDescription = "";
            requiredDocuments = "";
            closingDate = null;
            contactInformation = "";
            isMeritBased = false;
            isNeedBased = false;
            isActive = true;
            facultyLinks = new Collection<FundingFaculty>();
            programmeLinks = new Collection<FundingProgramme>();
        }

        // Create a funding option using its stored details
        // Faculty and programme links can be loaded separately
        public FundingOption(int pFundingID, string pFundingName, string pFundingType, string pEligibilityDescription,
                             string pRequiredDocuments, DateTime? pClosingDate, string pContactInformation,
                             bool pIsMeritBased, bool pIsNeedBased, bool pIsActive)
            : this()
        {
            fundingID = pFundingID;
            fundingName = pFundingName;
            fundingType = pFundingType;
            eligibilityDescription = pEligibilityDescription;
            requiredDocuments = pRequiredDocuments;
            closingDate = pClosingDate;
            contactInformation = pContactInformation;
            isMeritBased = pIsMeritBased;
            isNeedBased = pIsNeedBased;
            isActive = pIsActive;
        }
        #endregion

        #region Methods
        // Check whether funding applications are open on a given date
        // Funding without a closing date is treated as open
        public bool IsOpenOn(DateTime day)
        {
            return !closingDate.HasValue || closingDate.Value.Date >= day.Date;
        }

        // Check if funding is linked to the selected faculty or programme
        // Return true when neither a faculty nor a programme is specified
        public bool LinksTo(int facultyID, int programmeID)
        {
            if (facultyID == 0 && programmeID == 0)
            {
                return true;
            }
            foreach (FundingFaculty link in facultyLinks)
            {
                if (link.FacultyID == facultyID)
                {
                    return true;
                }
            }
            foreach (FundingProgramme link in programmeLinks)
            {
                if (link.ProgrammeID == programmeID)
                {
                    return true;
                }
            }
            return false;
        }

        // Return the funding option's name for display
        public override string ToString()
        {
            return fundingName;
        }
        #endregion
    }
}
