using System;
using System.Collections.ObjectModel;
using System.Data;
using FuturePath.DatabaseLayer;

namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// Staff session screens: the UI 9 dashboard counts and the UI 10.2b session search. Every
    /// method checks the Career Advisor or Administrator role first. Outcomes are read back from
    /// the stored result, never recalculated.
    /// </summary>
    public class SessionSearchController
    {
        #region Data Members
        private SessionSearchRepository searchRepository;
        private LoginController loginController;
        #endregion

        #region Constructor
        /// <summary>Creates the controller and its repository (no database work yet).</summary>
        public SessionSearchController()
        {
            searchRepository = new SessionSearchRepository();
            loginController = new LoginController();
        }
        #endregion

        #region Methods
        /// <summary>
        /// UI 9 "Today" card: sessions completed that day, sessions started that day and still in
        /// progress, and completed that day with outcome Not Currently Eligible or More Information Needed.
        /// </summary>
        public void GetDailySummary(int actorUserID, DateTime day, out int completedCount, out int inProgressCount, out int exceptionCount)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks the UI 10.2b search fields: a session number, if given, must be yyyy-NNNN; the
        /// from-date must not be after the to-date. Returns one message per problem.
        /// </summary>
        public Collection<string> ValidateSearch(string sessionNumber, DateTime? fromDate, DateTime? toDate)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// UI 10.2b search. Every filter is optional (blank or null = no filter). Returns SessionID,
        /// SessionStatus, SessionNumber, SessionDate, LearnerName, Grade, ProgrammeName, OutcomeCode,
        /// IsExported (an ExportReport row exists), newest first.
        /// </summary>
        public DataTable FindSessions(int actorUserID, string sessionNumber, string surname, DateTime? fromDate, DateTime? toDate, string outcomeCode, int? learnerID)
        {
            throw new NotImplementedException();
        }

        /// <summary>The total number of advisory sessions (for "n of N sessions match").</summary>
        public int CountSessions(int actorUserID)
        {
            throw new NotImplementedException();
        }

        /// <summary>The SessionID for a session number (yyyy-NNNN), or null if it is invalid or does not exist.</summary>
        public int? FindSessionId(string sessionNumber)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
