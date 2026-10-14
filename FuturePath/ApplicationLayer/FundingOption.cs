using System;
using System.Collections.ObjectModel;

namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// A bursary, scholarship or other funding option, linked to faculties and/or programmes
    /// through FundingFaculty and FundingProgramme. Inactive funding never appears to learners
    /// or in reports. Maps to FundingOption. Funding data is locally stored sample data, to be
    /// checked against the official source.
    /// </summary>
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
        /// <summary>Primary key (FundingOption.FundingID).</summary>
        public int FundingID
        {
            get { return fundingID; }
            set { fundingID = value; }
        }

        /// <summary>The funding option's name.</summary>
        public string FundingName
        {
            get { return fundingName; }
            set { fundingName = value; }
        }

        /// <summary>Scholarship, Bursary, Financial Aid or External Funding (the FUNDING_TYPE_n parameters).</summary>
        public string FundingType
        {
            get { return fundingType; }
            set { fundingType = value; }
        }

        /// <summary>Who may apply.</summary>
        public string EligibilityDescription
        {
            get { return eligibilityDescription; }
            set { eligibilityDescription = value; }
        }

        /// <summary>Documents needed to apply, or "".</summary>
        public string RequiredDocuments
        {
            get { return requiredDocuments; }
            set { requiredDocuments = value; }
        }

        /// <summary>The closing date, or null when applications are always open.</summary>
        public DateTime? ClosingDate
        {
            get { return closingDate; }
            set { closingDate = value; }
        }

        /// <summary>Who to contact, or "" (FundingOption.ContactInformation; class diagram contactInfo).</summary>
        public string ContactInformation
        {
            get { return contactInformation; }
            set { contactInformation = value; }
        }

        /// <summary>True if awarded on academic merit.</summary>
        public bool IsMeritBased
        {
            get { return isMeritBased; }
            set { isMeritBased = value; }
        }

        /// <summary>True if awarded on financial need.</summary>
        public bool IsNeedBased
        {
            get { return isNeedBased; }
            set { isNeedBased = value; }
        }

        /// <summary>Inactive funding is never shown to learners or printed in reports.</summary>
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        /// <summary>The faculties this funding is offered for.</summary>
        public Collection<FundingFaculty> FacultyLinks
        {
            get { return facultyLinks; }
        }

        /// <summary>The programmes this funding is offered for.</summary>
        public Collection<FundingProgramme> ProgrammeLinks
        {
            get { return programmeLinks; }
        }

        /// <summary>The linked faculty IDs (for the UI 13.3 tick lists).</summary>
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

        /// <summary>The linked programme IDs (for the UI 13.3 tick lists).</summary>
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

        /// <summary>The linked faculty and programme names, e.g. "Commerce, BCom Information Systems".</summary>
        public string LinkedTo
        {
            get
            {
                string text = "";
                foreach (FundingFaculty link in facultyLinks)
                {
                    text += (text.Length > 0 ? ", " : "") + link.FacultyName;
                }
                foreach (FundingProgramme link in programmeLinks)
                {
                    text += (text.Length > 0 ? ", " : "") + link.ProgrammeName;
                }
                return text;
            }
        }

        /// <summary>True if applications are open today (no closing date, or a closing date not yet passed).</summary>
        public bool IsOpen
        {
            get { return IsOpenOn(DateTime.Today); }
        }

        /// <summary>"Open" or "Closed" (the UI 6.2b status chip).</summary>
        public string Status
        {
            get { return IsOpen ? "Open" : "Closed"; }
        }
        #endregion

        #region Constructors
        /// <summary>Creates an empty, active funding option with no links.</summary>
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

        /// <summary>Creates a funding option with all its stored values (no links yet).</summary>
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
        /// <summary>True if applications are open on the given day.</summary>
        public bool IsOpenOn(DateTime day)
        {
            return !closingDate.HasValue || closingDate.Value.Date >= day.Date;
        }

        /// <summary>
        /// True if the funding is linked to the faculty or to the programme. With no faculty and
        /// no programme given, every funding option matches.
        /// </summary>
        public bool LinksTo(int? facultyID, int? programmeID)
        {
            if (!facultyID.HasValue && !programmeID.HasValue)
            {
                return true;
            }
            foreach (FundingFaculty link in facultyLinks)
            {
                if (facultyID.HasValue && link.FacultyID == facultyID.Value)
                {
                    return true;
                }
            }
            foreach (FundingProgramme link in programmeLinks)
            {
                if (programmeID.HasValue && link.ProgrammeID == programmeID.Value)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>Returns the funding name.</summary>
        public override string ToString()
        {
            return fundingName;
        }
        #endregion
    }
}
