using System;
using System.Collections.ObjectModel;
using FuturePath.ApplicationLayer;

namespace FuturePath.DatabaseLayer
{
    /// <summary>
    /// Data access for advisory sessions and their recommendation results (class diagram
    /// AdvisorySessionRepository: save(session), save(result), saveAll(results)).
    /// Re-running a check is safe: UNIQUE(SessionID, ProgrammeID) is respected by replacing rows.
    /// </summary>
    public class AdvisorySessionRepository : DB
    {
        #region Data Members
        private string table = "AdvisorySession";
        #endregion

        #region Constructor
        /// <summary>Creates the repository (no database work until a method is called).</summary>
        public AdvisorySessionRepository() : base()
        {
        }
        #endregion

        #region Read
        /// <summary>
        /// Loads one session, with Outcome set to the selected programme's OutcomeCode ("" if none).
        /// Results are not loaded. Returns null if it does not exist.
        /// </summary>
        public AdvisorySession FindById(int sessionID)
        {
            throw new NotImplementedException();
        }

        /// <summary>The selected programme's result (IsSelectedProgramme = 1) with ProgrammeName and FacultyName, or null.</summary>
        public RecommendationResult FindSelectedResult(int sessionID)
        {
            throw new NotImplementedException();
        }

        /// <summary>All results of a session with ProgrammeName and FacultyName: the selected one first, then alternatives by rank.</summary>
        public Collection<RecommendationResult> FindResults(int sessionID)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Write
        /// <summary>
        /// Class diagram save(session). Inserts the session (sets SessionID) when SessionID is 0,
        /// otherwise updates every column. Returns the same session.
        /// </summary>
        public AdvisorySession Save(AdvisorySession session)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Class diagram save(result): stores the selected programme's result. In one transaction
        /// it deletes the session's previous selected row and any row for the same programme,
        /// then inserts this row with IsSelectedProgramme = 1 (sets ResultID). Returns the result.
        /// </summary>
        public RecommendationResult Save(RecommendationResult result)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Class diagram saveAll(results): stores the ranked alternatives. In one transaction it
        /// deletes the session's previous alternative rows (IsSelectedProgramme = 0) and inserts
        /// these (sets each ResultID). All results must belong to one session. Returns true.
        /// </summary>
        public bool SaveAll(Collection<RecommendationResult> results)
        {
            throw new NotImplementedException();
        }

        /// <summary>Stores the preferred faculty and selected programme chosen on UI 5.</summary>
        public void SaveProgrammeSelection(int sessionID, int preferredFacultyID, int selectedProgrammeID)
        {
            throw new NotImplementedException();
        }

        /// <summary>Stores the UI 6.1a "include these alternatives in my summary" choice.</summary>
        public void SetAlternativesInSummary(int sessionID, bool include)
        {
            throw new NotImplementedException();
        }

        /// <summary>Stores the UI 6.2b "Add to Summary" choice (FundingInfoRequested).</summary>
        public void SetFundingInfoRequested(int sessionID, bool requested)
        {
            throw new NotImplementedException();
        }

        /// <summary>Sets SessionStatus = 'Completed' and DateCompleted (UI 7 Save and export).</summary>
        public void MarkCompleted(int sessionID, DateTime dateCompleted)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
