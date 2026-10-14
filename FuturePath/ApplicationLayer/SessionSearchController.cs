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
    /// Dates are whole days: a filter "from 1 June to 3 June" covers SessionDate from 1 June
    /// 00:00 up to, but not including, 4 June 00:00.
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
            loginController.CheckStaff(actorUserID);
            DateTime dayStart = day.Date;
            DateTime dayEnd = day.Date.AddDays(1);
            completedCount = searchRepository.CountCompleted(dayStart, dayEnd);
            inProgressCount = searchRepository.CountStartedInProgress(dayStart, dayEnd);
            exceptionCount = searchRepository.CountExceptionsCompleted(dayStart, dayEnd);
        }

        /// <summary>
        /// Checks the UI 10.2b search fields: a session number, if given, must be yyyy-NNNN; the
        /// from-date must not be after the to-date. Returns one message per problem.
        /// </summary>
        public Collection<string> ValidateSearch(string sessionNumber, DateTime? fromDate, DateTime? toDate)
        {
            Collection<string> messages = new Collection<string>();
            int year;
            int sessionID;
            if (!string.IsNullOrWhiteSpace(sessionNumber) && !AdvisorySession.TryParseSessionNumber(sessionNumber, out year, out sessionID))
            {
                messages.Add("Session number: use the form yyyy-NNNN, e.g. 2026-0417, or leave it blank.");
            }
            if (fromDate.HasValue && toDate.HasValue && fromDate.Value.Date > toDate.Value.Date)
            {
                messages.Add("Session from must be on or before Session to.");
            }
            return messages;
        }

        /// <summary>
        /// UI 10.2b search. Every filter is optional (blank or null = no filter). Returns SessionID,
        /// SessionStatus, SessionNumber, SessionDate, LearnerName, Grade, ProgrammeName, OutcomeCode,
        /// IsExported (an ExportReport row exists), newest first.
        /// Throws ArgumentException (with the first validation message) for a badly formed
        /// session number, reversed dates or an unknown outcome code.
        /// </summary>
        public DataTable FindSessions(int actorUserID, string sessionNumber, string surname, DateTime? fromDate, DateTime? toDate, string outcomeCode, int? learnerID)
        {
            loginController.CheckStaff(actorUserID);

            // Validate again in the BLL (the form validates first).
            Collection<string> messages = ValidateSearch(sessionNumber, fromDate, toDate);
            if (messages.Count > 0)
            {
                throw new ArgumentException(messages[0]);
            }

            // A session number yyyy-NNNN becomes the SessionID and the year it was started.
            int? sessionIDFilter = null;
            int? yearFilter = null;
            int year;
            int sessionID;
            if (AdvisorySession.TryParseSessionNumber(sessionNumber, out year, out sessionID))
            {
                sessionIDFilter = sessionID;
                yearFilter = year;
            }

            // Whole days: from the start of fromDate up to the start of the day after toDate.
            DateTime? fromFilter = null;
            DateTime? beforeFilter = null;
            if (fromDate.HasValue)
            {
                fromFilter = fromDate.Value.Date;
            }
            if (toDate.HasValue)
            {
                beforeFilter = toDate.Value.Date.AddDays(1);
            }

            // "All" outcomes is blank; anything else must be one of the four stored codes.
            string outcomeFilter = null;
            if (!string.IsNullOrWhiteSpace(outcomeCode))
            {
                if (!AdvisoryOutcome.IsValidCode(outcomeCode))
                {
                    throw new ArgumentException("Choose an advisory outcome from the list.");
                }
                outcomeFilter = outcomeCode.Trim().ToUpperInvariant();
            }

            int? learnerFilter = null;
            if (learnerID.HasValue && learnerID.Value > 0)
            {
                learnerFilter = learnerID;
            }

            DataTable sessions = searchRepository.FindSessions(sessionIDFilter, yearFilter, surname, fromFilter, beforeFilter,
                                                               outcomeFilter, learnerFilter);
            AddSessionNumbers(sessions);
            return sessions;
        }

        /// <summary>The total number of advisory sessions (for "n of N sessions match").</summary>
        public int CountSessions(int actorUserID)
        {
            loginController.CheckStaff(actorUserID);
            return searchRepository.CountSessions();
        }

        /// <summary>The SessionID for a session number (yyyy-NNNN), or null if it is invalid or does not exist.</summary>
        public int? FindSessionId(string sessionNumber)
        {
            int year;
            int sessionID;
            if (!AdvisorySession.TryParseSessionNumber(sessionNumber, out year, out sessionID))
            {
                return null;
            }
            if (!searchRepository.SessionExists(sessionID, year))
            {
                return null;
            }
            return sessionID;
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// Adds the SessionNumber column (yyyy-NNNN, built from SessionDate and SessionID; it is
        /// not stored) and fills it for every row. It is placed third, after SessionStatus.
        /// </summary>
        private static void AddSessionNumbers(DataTable sessions)
        {
            DataColumn column = sessions.Columns.Add("SessionNumber", typeof(string));
            column.SetOrdinal(2);
            foreach (DataRow row in sessions.Rows)
            {
                DateTime sessionDate = Convert.ToDateTime(row["SessionDate"]);
                int sessionID = Convert.ToInt32(row["SessionID"]);
                row["SessionNumber"] = AdvisorySession.FormatSessionNumber(sessionDate, sessionID);
            }
        }
        #endregion
    }
}
