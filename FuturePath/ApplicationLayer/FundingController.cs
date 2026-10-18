
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using FuturePath.DatabaseLayer;

namespace FuturePath.ApplicationLayer
{
    // Handles funding information for learners and administrators
    // Allows learners to view funding and administrators to manage funding options
    public class FundingController
    {
        #region Data Members
        private const int MaximumFundingNameLength = 100;
        private const int MaximumEligibilityLength = 500;
        private const int MaximumDocumentsLength = 255;
        private const int MaximumContactLength = 150;

        private FundingRepository fundingRepository;
        private AdvisorySessionRepository sessionRepository;
        private LoginController loginController;
        private Collection<FundingOption> fundingOptions;
        #endregion

        #region Properties
        // Return the funding options loaded by the controller
        public Collection<FundingOption> AllFundingOptions
        {
            get { return fundingOptions; }
        }
        #endregion

        #region Constructor
        // Initialise the repositories and login controller
        public FundingController()
        {
            fundingRepository = new FundingRepository();
            sessionRepository = new AdvisorySessionRepository();
            loginController = new LoginController();
            fundingOptions = new Collection<FundingOption>();
        }
        #endregion

        #region Learner view (UI 6.2b)
        // Get the available funding types from the system settings
        // Optionally add an All funding types choice for filtering
        public DataTable GetFundingTypes(bool includeAllRow)
        {
            DataTable table = SystemSettings.GetNumberedParameters(SystemSettings.FundingTypePrefix);
            table.Columns.Add("FundingType", typeof(string));
            foreach (DataRow row in table.Rows)
            {
                row["FundingType"] = row["ParameterValue"];
            }
            if (includeAllRow)
            {
                DataRow allRow = table.NewRow();
                allRow["ParameterCode"] = "";
                allRow["ParameterValue"] = "All funding types";
                allRow["FundingType"] = "";
                table.Rows.InsertAt(allRow, 0);
            }
            return table;
        }

        // Get active funding linked to the selected faculty or programme
        // Filter by funding type and optionally show only open applications
        public Collection<FundingOption> GetActiveFunding(int facultyID, int programmeID, string fundingType, bool openOnly)
        {
            fundingOptions = new Collection<FundingOption>();
            foreach (FundingOption option in fundingRepository.GetActiveFundingFor(facultyID, programmeID))
            {
                if (!string.IsNullOrWhiteSpace(fundingType)
                    && !string.Equals(option.FundingType, fundingType.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                if (openOnly && !option.IsOpen)
                {
                    continue;
                }
                fundingOptions.Add(option);
            }
            return fundingOptions;
        }

        // Record that the learner wants funding information in their summary
        public void RequestFundingInfo(int sessionID)
        {
            sessionRepository.SetFundingInfoRequested(sessionID, true);
        }
        #endregion

        #region Maintenance (UI 13.3)
        // Load funding options for the administrator's list
        // Include inactive options when requested
        public Collection<FundingOption> GetFundingOptions(bool includeInactive)
        {
            fundingOptions = fundingRepository.GetFundingOptions(includeInactive);
            return fundingOptions;
        }

        // Find a funding option using its ID
        public FundingOption GetFunding(int fundingID)
        {
            return fundingRepository.FindById(fundingID);
        }

        // Check the funding name, type and required information
        // Return a message for each validation problem found
        public Collection<string> ValidateFunding(int fundingID, string fundingName, string fundingType, string eligibilityDescription,
                                                  string requiredDocuments, DateTime? closingDate, string contactInformation)
        {
            Collection<string> problems = new Collection<string>();
            if (string.IsNullOrWhiteSpace(fundingName))
            {
                problems.Add("Enter the funding option's name.");
            }
            else if (fundingName.Trim().Length > MaximumFundingNameLength)
            {
                problems.Add("Shorten the funding name to " + MaximumFundingNameLength + " characters or fewer.");
            }
            else if (fundingRepository.FundingNameExists(fundingName, fundingID))
            {
                problems.Add("A funding option called " + fundingName.Trim() + " already exists. Enter a different name.");
            }

            if (FindFundingType(fundingType) == null)
            {
                problems.Add("Choose a funding type from the list.");
            }

            if (string.IsNullOrWhiteSpace(eligibilityDescription))
            {
                problems.Add("Enter who is eligible for this funding.");
            }
            else if (eligibilityDescription.Trim().Length > MaximumEligibilityLength)
            {
                problems.Add("Shorten the eligibility description to " + MaximumEligibilityLength + " characters or fewer.");
            }

            if (requiredDocuments != null && requiredDocuments.Trim().Length > MaximumDocumentsLength)
            {
                problems.Add("Shorten the required documents to " + MaximumDocumentsLength + " characters or fewer.");
            }
            if (contactInformation != null && contactInformation.Trim().Length > MaximumContactLength)
            {
                problems.Add("Shorten the contact information to " + MaximumContactLength + " characters or fewer.");
            }

            // The closing date is optional
            return problems;
        }

        // Add or update a funding option and its faculty and programme links
        // Only administrators can save changes, and invalid details are rejected
        public int SaveFunding(int actorUserID, int fundingID, string fundingName, string fundingType, string eligibilityDescription,
                               string requiredDocuments, DateTime? closingDate, string contactInformation, bool isMeritBased,
                               bool isNeedBased, bool isActive, IList<int> facultyIDs, IList<int> programmeIDs)
        {
            loginController.CheckAdministrator(actorUserID);
            Collection<string> problems = ValidateFunding(fundingID, fundingName, fundingType, eligibilityDescription,
                                                          requiredDocuments, closingDate, contactInformation);
            if (problems.Count > 0)
            {
                throw new ArgumentException(problems[0]);
            }
            if (fundingID > 0 && fundingRepository.FindById(fundingID) == null)
            {
                throw new ArgumentException("The selected funding option no longer exists. Refresh the list and try again.");
            }

            // Build the funding option before saving it with all its links
            FundingOption option = new FundingOption();
            option.FundingID = fundingID;
            option.FundingName = fundingName.Trim();
            option.FundingType = FindFundingType(fundingType);
            option.EligibilityDescription = eligibilityDescription.Trim();
            option.RequiredDocuments = TextOrEmpty(requiredDocuments);
            if (closingDate.HasValue)
            {
                option.ClosingDate = closingDate.Value.Date;
            }
            option.ContactInformation = TextOrEmpty(contactInformation);
            option.IsMeritBased = isMeritBased;
            option.IsNeedBased = isNeedBased;
            option.IsActive = isActive;
            if (facultyIDs != null)
            {
                foreach (int facultyID in facultyIDs)
                {
                    if (!option.FacultyIds.Contains(facultyID))
                    {
                        option.FacultyLinks.Add(new FundingFaculty(option.FundingID, facultyID, ""));
                    }
                }
            }
            if (programmeIDs != null)
            {
                foreach (int programmeID in programmeIDs)
                {
                    if (!option.ProgrammeIds.Contains(programmeID))
                    {
                        option.ProgrammeLinks.Add(new FundingProgramme(option.FundingID, programmeID, ""));
                    }
                }
            }
            return fundingRepository.Save(option);
        }

        // Deactivate a funding option without deleting it
        // Only administrators can perform this action
        public void DeactivateFunding(int actorUserID, int fundingID)
        {
            loginController.CheckAdministrator(actorUserID);
            if (fundingRepository.FindById(fundingID) == null)
            {
                throw new ArgumentException("The selected funding option no longer exists. Refresh the list and try again.");
            }
            fundingRepository.SetActive(fundingID, false);
        }
        #endregion

        #region Helpers
        // Remove extra spaces and return an empty string for null values
        private static string TextOrEmpty(string text)
        {
            if (text == null)
            {
                return "";
            }
            return text.Trim();
        }

        // Find a valid funding type from the system settings
        // Compare without case sensitivity and return null if not found
        private static string FindFundingType(string fundingType)
        {
            if (string.IsNullOrWhiteSpace(fundingType))
            {
                return null;
            }
            foreach (string type in SystemSettings.GetNumberedValues(SystemSettings.FundingTypePrefix))
            {
                if (string.Equals(type, fundingType.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return type;
                }
            }
            return null;
        }
        #endregion
    }
}
