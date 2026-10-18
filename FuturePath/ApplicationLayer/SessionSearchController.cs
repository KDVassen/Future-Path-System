
using System;
using System.Collections.ObjectModel;
using System.Data;
using FuturePath.DatabaseLayer;

namespace FuturePath.ApplicationLayer
{
    // Handles staff dashboard summaries and advisory session searches
    // Uses saved session information and checks staff access
    public class SessionSearchController
    {
        #region Data Members
        private SessionSearchRepository searchRepository;
        private LoginController loginController;
        #endregion

        #region Constructor
        // Initialise the session search repository and login controller
        public SessionSearchController()
        {
            searchRepository = new SessionSearchRepository();
            loginController = new LoginController();
        }
        #endregion

        #region Methods
        // Get the daily counts of completed, in-progress and exception sessions
        // Only authorised staff can view these figures
        public DailySummary GetDailySummary(int actorUserID, DateTime day)
        {
            loginController.CheckStaff(actorUserID);
            DateTime dayStart = day.Date;
            DateTime dayEnd = day.Date.AddDays(1);
            DailySummary summary = new DailySummary();
            summary.CompletedCount = searchRepository.CountCompleted(dayStart, dayEnd);
            summary.InProgressCount = searchRepository.CountStartedInProgress(dayStart, dayEnd);
            summary.ExceptionCount = searchRepository.CountExceptionsCompleted(dayStart, dayEnd);
            return summary;
        }

        // Validate the session number and selected date range
        // Return a message for each problem found
        public Collection<string> ValidateSearch(string sessionNumber, DateTime? fromDate, DateTime? toDate)
        {
            Collection<string> messages = new Collection<string>();
            if (!string.IsNullOrWhiteSpace(sessionNumber) && !AdvisorySession.IsValidSessionNumber(sessionNumber))
            {
                messages.Add("Session number: use the form yyyy-NNNN, e.g. 2026-0417, or leave it blank.");
            }
            if (fromDate.HasValue && toDate.HasValue && fromDate.Value.Date > toDate.Value.Date)
            {
                messages.Add("Session from must be on or before Session to.");
            }
            return messages;
        }

        // Search advisory sessions using the selected filters
        // Validate the inputs and return matching sessions
        public DataTable FindSessions(int actorUserID, string sessionNumber, string surname, DateTime? fromDate, DateTime? toDate, string outcomeCode, int learnerID)
        {
            loginController.CheckStaff(actorUserID);

            // Check the session number and date range before searching
            Collection<string> messages = ValidateSearch(sessionNumber, fromDate, toDate);
            if (messages.Count > 0)
            {
                throw new ArgumentException(messages[0]);
            }

            // Extract the session ID and year from the session number
            int sessionIDFilter = AdvisorySession.GetSessionID(sessionNumber);
            int yearFilter = AdvisorySession.GetSessionYear(sessionNumber);

            // Include complete days when applying the date filters
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

            // Check the selected outcome or leave the filter blank for all outcomes
            string outcomeFilter = "";
            if (!string.IsNullOrWhiteSpace(outcomeCode))
            {
                if (!AdvisoryOutcome.IsValidCode(outcomeCode))
                {
                    throw new ArgumentException("Choose an advisory outcome from the list.");
                }
                outcomeFilter = outcomeCode.Trim().ToUpperInvariant();
            }

            DataTable sessions = searchRepository.FindSessions(sessionIDFilter, yearFilter, surname, fromFilter, beforeFilter,
                                                               outcomeFilter, learnerID);
            AddSessionNumbers(sessions);
            return sessions;
        }

        // Count all advisory sessions for the staff search screen
        public int CountSessions(int actorUserID)
        {
            loginController.CheckStaff(actorUserID);
            return searchRepository.CountSessions();
        }

        // Find the session ID using its formatted session number
        // Return zero if the session is invalid or does not exist
        public int FindSessionId(string sessionNumber)
        {
            int sessionID = AdvisorySession.GetSessionID(sessionNumber);
            int year = AdvisorySession.GetSessionYear(sessionNumber);
            if (sessionID == 0 || !searchRepository.SessionExists(sessionID, year))
            {
                return 0;
            }
            return sessionID;
        }
        #endregion

        #region Utility Methods
        // Add a formatted session number to each search result
        // Place the new column after the session status column
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
