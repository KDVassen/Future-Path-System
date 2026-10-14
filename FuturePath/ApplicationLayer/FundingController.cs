using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using FuturePath.DatabaseLayer;

namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// Funding options: the learner's UI 6.2b view (active funding linked to their faculty or
    /// programme) and the Administrator maintenance on UI 13.3. Inactive funding never reaches a
    /// learner. Maintenance methods check the Administrator role first.
    /// </summary>
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
        /// <summary>The funding options read by the last GetFundingOptions or GetActiveFunding call.</summary>
        public Collection<FundingOption> AllFundingOptions
        {
            get { return fundingOptions; }
        }
        #endregion

        #region Constructor
        /// <summary>Creates the controller and its repositories (no database work yet).</summary>
        public FundingController()
        {
            fundingRepository = new FundingRepository();
            sessionRepository = new AdvisorySessionRepository();
            loginController = new LoginController();
            fundingOptions = new Collection<FundingOption>();
        }
        #endregion

        #region Learner view (UI 6.2b)
        /// <summary>
        /// The funding types from FUNDING_TYPE_n with columns ParameterCode, ParameterValue (the
        /// text to show) and FundingType (the value to filter or store: the type text kept in
        /// FundingOption.FundingType). With includeAllRow an "All funding types" row comes first,
        /// with ParameterCode "" and FundingType "" (no filter). Bind DisplayMember = ParameterValue,
        /// ValueMember = FundingType.
        /// </summary>
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

        /// <summary>
        /// Active funding linked to the faculty or programme (both null: all active funding),
        /// filtered by funding type ("" or null: all) and, with openOnly, to options still open
        /// today. Ordered by closing date, nearest first (no closing date last). A faculty or
        /// programme ID of 0 (an "All" row) is treated as no filter.
        /// </summary>
        public Collection<FundingOption> GetActiveFunding(int? facultyID, int? programmeID, string fundingType, bool openOnly)
        {
            int? faculty = (facultyID.HasValue && facultyID.Value > 0) ? facultyID : null;
            int? programme = (programmeID.HasValue && programmeID.Value > 0) ? programmeID : null;
            string type = (fundingType ?? "").Trim();

            fundingOptions = new Collection<FundingOption>();
            foreach (FundingOption option in fundingRepository.GetActiveFundingFor(faculty, programme))
            {
                if (type.Length > 0 && !string.Equals(option.FundingType, type, StringComparison.OrdinalIgnoreCase))
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

        /// <summary>UI 6.2b "Add to Summary": sets FundingInfoRequested so funding prints on the FP-LSR.</summary>
        public void RequestFundingInfo(int sessionID)
        {
            sessionRepository.SetFundingInfoRequested(sessionID, true);
        }
        #endregion

        #region Maintenance (UI 13.3)
        /// <summary>Funding options with their links, for the UI 13.3 grid.</summary>
        public Collection<FundingOption> GetFundingOptions(bool includeInactive)
        {
            fundingOptions = fundingRepository.GetFundingOptions(includeInactive);
            return fundingOptions;
        }

        /// <summary>One funding option with FacultyIds and ProgrammeIds, or null.</summary>
        public FundingOption GetFunding(int fundingID)
        {
            return fundingRepository.FindById(fundingID);
        }

        /// <summary>
        /// Checks a funding option: name required (max 100) and not already used, type one of the
        /// FUNDING_TYPE_n values, eligibility required (max 500), documents max 255, contact max
        /// 150. Returns one message per problem.
        /// </summary>
        public Collection<string> ValidateFunding(int? fundingID, string fundingName, string fundingType, string eligibilityDescription,
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
            // closingDate comes from a DateTimePicker and is optional, so any date is accepted.
            return problems;
        }

        /// <summary>
        /// Adds (fundingID null) or updates a funding option and its faculty and programme links in
        /// one transaction (Administrator only). Repeated IDs in the lists are saved once. Returns
        /// the FundingID. Throws ArgumentException if invalid, UnauthorizedAccessException if not
        /// an Administrator.
        /// </summary>
        public int SaveFunding(int actorUserID, int? fundingID, string fundingName, string fundingType, string eligibilityDescription,
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
            if (fundingID.HasValue && fundingRepository.FindById(fundingID.Value) == null)
            {
                throw new ArgumentException("The selected funding option no longer exists. Refresh the list and try again.");
            }

            // The option is built whole: Save replaces every column and every link in one transaction.
            FundingOption option = new FundingOption(fundingID ?? 0, fundingName.Trim(), FindFundingType(fundingType),
                                                     eligibilityDescription.Trim(), (requiredDocuments ?? "").Trim(),
                                                     closingDate.HasValue ? closingDate.Value.Date : (DateTime?)null,
                                                     (contactInformation ?? "").Trim(), isMeritBased, isNeedBased, isActive);
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

        /// <summary>Deactivates a funding option (Administrator only). It is never deleted.</summary>
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
        /// <summary>
        /// The funding type as stored in its FUNDING_TYPE_n parameter, or null if the text is not
        /// one of them (not case-sensitive). Funding types are never typed into the code.
        /// </summary>
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
