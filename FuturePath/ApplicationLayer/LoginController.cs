using System;
using FuturePath.DatabaseLayer;

namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// The "Authorization and access" package of the Business Logic Layer (spec Section 1 and 8).
    /// Authenticate signs a staff member in on UI 8; CheckStaff and CheckAdministrator are called
    /// at the start of every BLL method that needs a role, so security does not depend on the
    /// forms hiding tabs (decisions.md D11). The role and active flag are always re-read from the
    /// database, so a deactivated account loses access straight away.
    /// </summary>
    public class LoginController
    {
        #region Data Members
        private UserAccountRepository userAccountRepository;
        #endregion

        #region Constructor
        /// <summary>Creates the controller and its repository (no database work yet).</summary>
        public LoginController()
        {
            userAccountRepository = new UserAccountRepository();
        }
        #endregion

        #region Methods
        /// <summary>
        /// UI 8 login. Returns the account if the username exists, the account is active and the
        /// password matches the salted hash; otherwise null. Never says which part was wrong.
        /// </summary>
        public UserAccount Authenticate(string username, string password)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the active staff account (Career Advisor or Administrator) with this ID, or
        /// throws UnauthorizedAccessException.
        /// </summary>
        public UserAccount CheckStaff(int actorUserID)
        {
            throw new NotImplementedException();
        }

        /// <summary>Returns the active Administrator account with this ID, or throws UnauthorizedAccessException.</summary>
        public UserAccount CheckAdministrator(int actorUserID)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
