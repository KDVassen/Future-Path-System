
using System;
using FuturePath.DatabaseLayer;

namespace FuturePath.ApplicationLayer
{
    // Handles staff login and access permissions
    // Checks whether users are active and have the required role
    public class LoginController
    {
        #region Data Members
        private UserAccountRepository userAccountRepository;
        #endregion

        #region Constructor
        // Initialise the user account repository
        public LoginController()
        {
            userAccountRepository = new UserAccountRepository();
        }
        #endregion

        #region Methods
        // Check the staff member's username and password
        // Return the account if valid, otherwise return null
        public UserAccount Authenticate(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
            {
                return null;
            }
            UserAccount account = userAccountRepository.FindByUsername(username);
            if (account == null || !account.Authenticate(username, password))
            {
                return null;
            }
            return account;
        }

        // Check whether the user is an active staff member
        // Throw an error if the account does not exist or is inactive
        public UserAccount CheckStaff(int actorUserID)
        {
            UserAccount account = userAccountRepository.FindById(actorUserID);
            if (account == null || !account.IsActive)
            {
                throw new UnauthorizedAccessException("Please sign in as a Career Advisor or Administrator to do this.");
            }
            return account;
        }

        // Check whether the staff member has Administrator access
        // Prevent other users from changing reference data and accounts
        public UserAccount CheckAdministrator(int actorUserID)
        {
            UserAccount account = CheckStaff(actorUserID);
            if (!account.IsAdministrator)
            {
                throw new UnauthorizedAccessException("Only an Administrator can change reference data and user accounts.");
            }
            return account;
        }
        #endregion
    }
}
