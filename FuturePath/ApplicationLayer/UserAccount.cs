using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// A staff login: a Career Advisor or an Administrator. Learners never log in, so this class
    /// has no associations (class diagram). Passwords are never stored: only a salted SHA-256
    /// hash is kept, in the form "salt:hash" (spec Section 8). Maps to UserAccount.
    /// </summary>
    public class UserAccount
    {
        #region Data Members
        /// <summary>The two staff roles (spec Section 8).</summary>
        public enum RoleType
        {
            /// <summary>Views sessions and generates reports.</summary>
            CareerAdvisor = 1,
            /// <summary>Also maintains reference data and user accounts.</summary>
            Administrator = 2
        }

        /// <summary>Stored text for the Administrator role.</summary>
        public const string RoleAdministratorText = "Administrator";
        /// <summary>Stored text for the Career Advisor role.</summary>
        public const string RoleCareerAdvisorText = "Career Advisor";
        /// <summary>The shortest password accepted.</summary>
        public const int MinimumPasswordLength = 8;

        // Bytes of random salt added to each password before hashing
        private const int SaltSize = 16;

        private int userID;
        private string username;
        private string passwordHash;
        private string fullName;
        private RoleType role;
        private bool isActive;
        #endregion

        #region Properties
        /// <summary>Primary key (UserAccount.UserID).</summary>
        public int UserID
        {
            get { return userID; }
            set { userID = value; }
        }

        /// <summary>The login name (unique; letters, digits, '.', '_' and '-').</summary>
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        /// <summary>The salted password hash. Never shown on a screen or returned to a form.</summary>
        public string PasswordHash
        {
            get { return passwordHash; }
            set { passwordHash = value; }
        }

        /// <summary>The staff member's full name (status bar and dashboards).</summary>
        public string FullName
        {
            get { return fullName; }
            set { fullName = value; }
        }

        /// <summary>Career Advisor or Administrator.</summary>
        public RoleType Role
        {
            get { return role; }
            set { role = value; }
        }

        /// <summary>The role as stored and shown: "Administrator" or "Career Advisor".</summary>
        public string RoleName
        {
            get { return RoleToText(role); }
        }

        /// <summary>Inactive accounts cannot log in.</summary>
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        /// <summary>True for an Administrator.</summary>
        public bool IsAdministrator
        {
            get { return role == RoleType.Administrator; }
        }
        #endregion

        #region Constructors
        /// <summary>Creates an empty, active Career Advisor account.</summary>
        public UserAccount()
        {
            userID = 0;
            username = "";
            passwordHash = "";
            fullName = "";
            role = RoleType.CareerAdvisor;
            isActive = true;
        }

        /// <summary>Creates an account with all its stored values.</summary>
        public UserAccount(int pUserID, string pUsername, string pPasswordHash, string pFullName, RoleType pRole, bool pIsActive)
        {
            userID = pUserID;
            username = pUsername;
            passwordHash = pPasswordHash;
            fullName = pFullName;
            role = pRole;
            isActive = pIsActive;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Class diagram authenticate(username, password). True only if the account is active,
        /// the username matches (not case-sensitive) and the password matches the stored hash.
        /// </summary>
        public bool Authenticate(string pUsername, string pPassword)
        {
            if (!isActive || pUsername == null)
            {
                return false;
            }
            if (!string.Equals(username, pUsername.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return VerifyPassword(pPassword, passwordHash);
        }

        /// <summary>Replaces the stored hash with a new salted hash of the password.</summary>
        public void SetPassword(string password)
        {
            passwordHash = HashPassword(password);
        }

        /// <summary>
        /// Makes a salted hash of a password, stored as "salt:hash" (both in Base64). The salt is
        /// 16 random bytes, new every time, so two equal passwords get different hashes.
        /// </summary>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Enter a password.");
            }
            byte[] saltBytes = new byte[SaltSize];
            RandomNumberGenerator random = RandomNumberGenerator.Create();
            random.GetBytes(saltBytes);
            string salt = Convert.ToBase64String(saltBytes);
            return salt + ":" + HashWithSalt(password, salt);
        }

        /// <summary>
        /// True if the password gives the stored hash: the password is hashed again with the
        /// stored salt and the two hashes are compared. A missing or damaged hash returns false.
        /// </summary>
        public static bool VerifyPassword(string password, string storedHash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash))
            {
                return false;
            }
            string[] parts = storedHash.Split(':');
            if (parts.Length != 2)
            {
                return false;
            }
            string salt = parts[0];
            string hash = parts[1];
            return HashWithSalt(password, salt) == hash;
        }

        /// <summary>Turns stored role text into a RoleType. Throws ArgumentException for unknown text.</summary>
        public static RoleType RoleFromText(string text)
        {
            if (IsSameText(text, RoleAdministratorText))
            {
                return RoleType.Administrator;
            }
            if (IsSameText(text, RoleCareerAdvisorText))
            {
                return RoleType.CareerAdvisor;
            }
            throw new ArgumentException("Choose a role: " + RoleAdministratorText + " or " + RoleCareerAdvisorText + ".");
        }

        /// <summary>Turns a RoleType into its stored text.</summary>
        public static string RoleToText(RoleType value)
        {
            if (value == RoleType.Administrator)
            {
                return RoleAdministratorText;
            }
            return RoleCareerAdvisorText;
        }

        /// <summary>True if the role text is Administrator or Career Advisor.</summary>
        public static bool IsValidRoleText(string text)
        {
            return IsSameText(text, RoleAdministratorText) || IsSameText(text, RoleCareerAdvisorText);
        }

        /// <summary>True if a username is 1 to 50 letters, digits, '.', '_' or '-'.</summary>
        public static bool IsValidUsername(string text)
        {
            return !string.IsNullOrWhiteSpace(text) && text.Trim().Length <= 50 && Regex.IsMatch(text.Trim(), @"^[A-Za-z0-9._\-]+$");
        }

        /// <summary>The SHA-256 hash (Base64) of the salt followed by the password.</summary>
        private static string HashWithSalt(string password, string salt)
        {
            SHA256 sha = SHA256.Create();
            byte[] hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(salt + password));
            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>True if the text (ignoring spaces at the ends and capitals) equals the expected text.</summary>
        private static bool IsSameText(string text, string expected)
        {
            if (text == null)
            {
                return false;
            }
            return string.Equals(text.Trim(), expected, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>Returns "Full name (Role)". Never includes the hash.</summary>
        public override string ToString()
        {
            return fullName + " (" + RoleName + ")";
        }
        #endregion
    }
}
