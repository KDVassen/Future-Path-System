
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace FuturePath.ApplicationLayer
{
    // Stores staff account details and handles password authentication
    // Supports Career Advisor and Administrator roles
    public class UserAccount
    {
        #region Data Members
        // Staff roles used to control system access
        public enum RoleType
        {
            CareerAdvisor = 1,
            Administrator = 2
        }

        // Role names and minimum password length
        public const string RoleAdministratorText = "Administrator";
        public const string RoleCareerAdvisorText = "Career Advisor";
        public const int MinimumPasswordLength = 8;

        // Number of random bytes used when creating a password salt
        private const int SaltSize = 16;

        private int userID;
        private string username;
        private string passwordHash;
        private string fullName;
        private RoleType role;
        private bool isActive;
        #endregion

        #region Properties
        public int UserID
        {
            get { return userID; }
            set { userID = value; }
        }

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        // Store the salted password hash instead of the actual password
        public string PasswordHash
        {
            get { return passwordHash; }
            set { passwordHash = value; }
        }

        public string FullName
        {
            get { return fullName; }
            set { fullName = value; }
        }

        public RoleType Role
        {
            get { return role; }
            set { role = value; }
        }

        // Get the role name used by the system
        public string RoleName
        {
            get { return RoleToText(role); }
        }

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        // Check whether the account has Administrator access
        public bool IsAdministrator
        {
            get { return role == RoleType.Administrator; }
        }
        #endregion

        #region Constructors
        // Create an active Career Advisor account with default values
        public UserAccount()
        {
            userID = 0;
            username = "";
            passwordHash = "";
            fullName = "";
            role = RoleType.CareerAdvisor;
            isActive = true;
        }

        // Create a user account using the provided details
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
        // Check that the account is active and the username matches
        // Verify the password against the stored hash
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

        // Generate and store a new salted hash for the password
        public void SetPassword(string password)
        {
            passwordHash = HashPassword(password);
        }

        // Generate a random salt and use it to hash the password
        // Store the salt and hash together as Base64 text
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

        // Hash the supplied password using the stored salt
        // Compare it with the saved hash to verify the password
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

        // Convert stored role text into the matching RoleType
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

        // Convert a RoleType into its stored role name
        public static string RoleToText(RoleType value)
        {
            if (value == RoleType.Administrator)
            {
                return RoleAdministratorText;
            }
            return RoleCareerAdvisorText;
        }

        // Check whether the supplied text is a valid staff role
        public static bool IsValidRoleText(string text)
        {
            return IsSameText(text, RoleAdministratorText) || IsSameText(text, RoleCareerAdvisorText);
        }

        // Check that the username contains only allowed characters
        // Usernames must be between 1 and 50 characters long
        public static bool IsValidUsername(string text)
        {
            return !string.IsNullOrWhiteSpace(text) && text.Trim().Length <= 50 && Regex.IsMatch(text.Trim(), @"^[A-Za-z0-9._-]+$");
        }

        // Generate a SHA-256 hash from the salt and password
        private static string HashWithSalt(string password, string salt)
        {
            SHA256 sha = SHA256.Create();
            byte[] hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(salt + password));
            return Convert.ToBase64String(hashBytes);
        }

        // Compare two text values without case sensitivity or extra spaces
        private static bool IsSameText(string text, string expected)
        {
            if (text == null)
            {
                return false;
            }
            return string.Equals(text.Trim(), expected, StringComparison.OrdinalIgnoreCase);
        }

        // Display the staff member's name and role
        public override string ToString()
        {
            return fullName + " (" + RoleName + ")";
        }
        #endregion
    }
}
