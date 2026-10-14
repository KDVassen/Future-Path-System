using System;
using System.Collections.ObjectModel;
using FuturePath.ApplicationLayer;

namespace FuturePath.DatabaseLayer
{
    /// <summary>
    /// Data access for funding options and their faculty / programme links (UI 6.2b, UI 13.3,
    /// FP-LSR Table 3). An option and its links are saved together in one transaction.
    /// </summary>
    public class FundingRepository : DB
    {
        #region Data Members
        private string table = "FundingOption";
        #endregion

        #region Constructor
        /// <summary>Creates the repository (no database work until a method is called).</summary>
        public FundingRepository() : base()
        {
        }
        #endregion

        #region Read
        /// <summary>Funding options with their links (and link names), ordered by FundingName; inactive ones only when includeInactive is true.</summary>
        public Collection<FundingOption> GetFundingOptions(bool includeInactive)
        {
            throw new NotImplementedException();
        }

        /// <summary>One funding option with its links, or null.</summary>
        public FundingOption FindById(int fundingID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Active funding linked to the faculty OR the programme (links to inactive faculties or
        /// programmes do not count), with links, ordered by ClosingDate (nearest first, no date last).
        /// With both null, every active funding option.
        /// </summary>
        public Collection<FundingOption> GetActiveFundingFor(int? facultyID, int? programmeID)
        {
            throw new NotImplementedException();
        }

        /// <summary>True if another funding option already has this name.</summary>
        public bool FundingNameExists(string fundingName, int? excludeFundingID)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Write
        /// <summary>
        /// Inserts (FundingID 0) or updates the option and replaces its FundingFaculty and
        /// FundingProgramme rows with option.FacultyLinks / ProgrammeLinks, in one transaction.
        /// Returns the FundingID.
        /// </summary>
        public int Save(FundingOption option)
        {
            throw new NotImplementedException();
        }

        /// <summary>Sets IsActive (deactivation; funding is never deleted).</summary>
        public void SetActive(int fundingID, bool isActive)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
