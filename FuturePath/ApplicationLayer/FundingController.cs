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
        /// The funding types from FUNDING_TYPE_n: ParameterCode, ParameterValue. With includeAllRow
        /// an "All funding types" row (ParameterValue "") comes first. The value is the type text
        /// stored in FundingOption.FundingType.
        /// </summary>
        public DataTable GetFundingTypes(bool includeAllRow)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Active funding linked to the faculty or programme (both null: all active funding),
        /// filtered by funding type ("" or null: all) and, with openOnly, to options still open
        /// today. Ordered by closing date, nearest first.
        /// </summary>
        public Collection<FundingOption> GetActiveFunding(int? facultyID, int? programmeID, string fundingType, bool openOnly)
        {
            throw new NotImplementedException();
        }

        /// <summary>UI 6.2b "Add to Summary": sets FundingInfoRequested so funding prints on the FP-LSR.</summary>
        public void RequestFundingInfo(int sessionID)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Maintenance (UI 13.3)
        /// <summary>Funding options with their links, for the UI 13.3 grid.</summary>
        public Collection<FundingOption> GetFundingOptions(bool includeInactive)
        {
            throw new NotImplementedException();
        }

        /// <summary>One funding option with FacultyIds and ProgrammeIds, or null.</summary>
        public FundingOption GetFunding(int fundingID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks a funding option: name required (max 100) and not already used, type one of the
        /// FUNDING_TYPE_n values, eligibility required (max 500), documents max 255, contact max
        /// 150. Returns one message per problem.
        /// </summary>
        public Collection<string> ValidateFunding(int? fundingID, string fundingName, string fundingType, string eligibilityDescription,
                                                  string requiredDocuments, DateTime? closingDate, string contactInformation)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds (fundingID null) or updates a funding option and its faculty and programme links in
        /// one transaction (Administrator only). Returns the FundingID.
        /// </summary>
        public int SaveFunding(int actorUserID, int? fundingID, string fundingName, string fundingType, string eligibilityDescription,
                               string requiredDocuments, DateTime? closingDate, string contactInformation, bool isMeritBased,
                               bool isNeedBased, bool isActive, IList<int> facultyIDs, IList<int> programmeIDs)
        {
            throw new NotImplementedException();
        }

        /// <summary>Deactivates a funding option (Administrator only).</summary>
        public void DeactivateFunding(int actorUserID, int fundingID)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
