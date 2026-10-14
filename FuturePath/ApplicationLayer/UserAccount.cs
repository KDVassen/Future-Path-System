using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// A staff login: a Career Advisor or an Administrator. Learners never log in, so this class
    /// has no associations (class diagram). Passwords are never stored. Only a salted PBKDF2
    /// hash is kept, in the form "iterations:salt:hash" (spec Section 8). Maps to UserAccount.
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

        private const int Iterations = 10000;
        private const int SaltSize = 16;
        private const int HashSize = 32;

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
        /// Makes a salted PBKDF2 (SHA-256) hash of a password: "iterations:salt:hash" in Base64.
        /// A new random salt is used every time, so equal passwords get different hashes.
        /// </summary>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Enter a password.");
            }
            byte[] salt = new byte[SaltSize];
            using (RandomNumberGenerator random = RandomNumberGenerator.Create())
            {
                random.GetBytes(salt);
            }
            byte[] hash = DeriveHash(password, salt, Iterations);
            return Iterations.ToString(CultureInfo.InvariantCulture) + ":" + Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
        }

        /// <summary>
        /// True if the password produces the stored hash. A missing or damaged hash returns
        /// false and never throws.
        /// </summary>
        public static bool VerifyPassword(string password, string storedHash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash))
            {
                return false;
            }
            string[] parts = storedHash.Split(':');
            if (parts.Length != 3)
            {
                return false;
            }
            int iterations;
            if (!int.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out iterations) || iterations <= 0)
            {
                return false;
            }
            byte[] salt;
            byte[] expected;
            try
            {
                salt = Convert.FromBase64String(parts[1]);
                expected = Convert.FromBase64String(parts[2]);
            }
            catch (FormatException)
            {
                return false;
            }
            byte[] actual = DeriveHash(password, salt, iterations);
            return SameBytes(expected, actual);
        }

        /// <summary>Turns stored role text into a RoleType. Throws ArgumentException for unknown text.</summary>
        public static RoleType RoleFromText(string text)
        {
            string value = (text ?? "").Trim();
            if (string.Equals(value, RoleAdministratorText, StringComparison.OrdinalIgnoreCase))
            {
                return RoleType.Administrator;
            }
            if (string.Equals(value, RoleCareerAdvisorText, StringComparison.OrdinalIgnoreCase))
            {
                return RoleType.CareerAdvisor;
            }
            throw new ArgumentException("Choose a role: " + RoleAdministratorText + " or " + RoleCareerAdvisorText + ".");
        }

        /// <summary>Turns a RoleType into its stored text.</summary>
        public static string RoleToText(RoleType value)
        {
            return value == RoleType.Administrator ? RoleAdministratorText : RoleCareerAdvisorText;
        }

        /// <summary>True if the role text is Administrator or Career Advisor.</summary>
        public static bool IsValidRoleText(string text)
        {
            string value = (text ?? "").Trim();
            return string.Equals(value, RoleAdministratorText, StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, RoleCareerAdvisorText, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>True if a username is 1 to 50 letters, digits, '.', '_' or '-'.</summary>
        public static bool IsValidUsername(string text)
        {
            return !string.IsNullOrWhiteSpace(text) && text.Trim().Length <= 50 && Regex.IsMatch(text.Trim(), @"^[A-Za-z0-9._\-]+$");
        }

        /// <summary>PBKDF2 with SHA-256.</summary>
        private static byte[] DeriveHash(string password, byte[] salt, int iterations)
        {
            using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(HashSize);
            }
        }

        /// <summary>Compares two hashes in constant time, so timing does not reveal how much matched.</summary>
        private static bool SameBytes(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }
            int difference = 0;
            for (int i = 0; i < a.Length; i++)
            {
                difference |= a[i] ^ b[i];
            }
            return difference == 0;
        }

        /// <summary>Returns "Full name (Role)". Never includes the hash.</summary>
        public override string ToString()
        {
            return fullName + " (" + RoleName + ")";
        }
        #endregion
    }
}
