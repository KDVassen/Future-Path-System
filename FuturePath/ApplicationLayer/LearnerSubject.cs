
using System.Globalization;

namespace FuturePath.ApplicationLayer
{
    // Stores a subject and mark captured during a learner's advisory session
    // Marks belong to the session so returning learners can enter updated marks
    public class LearnerSubject
    {
        #region Data Members
        // Minimum and maximum marks allowed
        public const decimal MinimumMark = 0m;
        public const decimal MaximumMark = 100m;

        private int learnerSubjectID;
        private int sessionID;
        private int subjectID;
        private string subjectName;
        private string subjectLevel;
        private decimal mark;
        #endregion

        #region Properties
        public int LearnerSubjectID
        {
            get { return learnerSubjectID; }
            set { learnerSubjectID = value; }
        }

        public int SessionID
        {
            get { return sessionID; }
            set { sessionID = value; }
        }

        public int SubjectID
        {
            get { return subjectID; }
            set { subjectID = value; }
        }

        public string SubjectName
        {
            get { return subjectName; }
            set { subjectName = value; }
        }

        public string SubjectLevel
        {
            get { return subjectLevel; }
            set { subjectLevel = value; }
        }

        public decimal Mark
        {
            get { return mark; }
            set { mark = value; }
        }
        #endregion

        #region Constructors
        // Create a new subject mark with default values
        public LearnerSubject()
        {
            learnerSubjectID = 0;
            sessionID = 0;
            subjectID = 0;
            subjectName = "";
            subjectLevel = "";
            mark = 0;
        }

        // Create a subject mark using its stored details
        public LearnerSubject(int pLearnerSubjectID, int pSessionID, int pSubjectID, string pSubjectName, string pSubjectLevel, decimal pMark)
        {
            learnerSubjectID = pLearnerSubjectID;
            sessionID = pSessionID;
            subjectID = pSubjectID;
            subjectName = pSubjectName;
            subjectLevel = pSubjectLevel;
            mark = pMark;
        }
        #endregion

        #region Methods
        // Check if the mark is between 0 and 100 with at most one decimal place
        public bool ValidateMarkRange()
        {
            return IsValidMark(mark);
        }

        // Check whether a given mark is within the allowed range
        public static bool IsValidMark(decimal value)
        {
            return value >= MinimumMark && value <= MaximumMark && decimal.Round(value, 1) == value;
        }

        // Convert the entered text into a valid decimal mark
        // Accept both a full stop and a comma as the decimal separator
        // Return -1 if the mark is invalid
        public static decimal ParseMark(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return -1;
            }
            string cleaned = text.Trim().Replace(',', '.');
            decimal value;
            if (!decimal.TryParse(cleaned, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out value))
            {
                return -1;
            }
            if (!IsValidMark(value))
            {
                return -1;
            }
            return value;
        }

        // Return the subject name and mark as a string
        public override string ToString()
        {
            return subjectName + ": " + mark.ToString("0.#", CultureInfo.InvariantCulture) + "%";
        }
        #endregion
    }
}
