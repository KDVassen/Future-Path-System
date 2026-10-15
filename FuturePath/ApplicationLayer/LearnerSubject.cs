using System.Globalization;

namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// One subject and mark captured in an advisory session (UI 3). Marks belong to the session,
    /// not the learner, because a returning learner has new marks (database.md). Maps to the
    /// LearnerSubject table; UNIQUE(SessionID, SubjectID) stops a subject being captured twice.
    /// </summary>
    public class LearnerSubject
    {
        #region Data Members
        /// <summary>The lowest mark allowed.</summary>
        public const decimal MinimumMark = 0m;
        /// <summary>The highest mark allowed.</summary>
        public const decimal MaximumMark = 100m;

        private int learnerSubjectID;
        private int sessionID;
        private int subjectID;
        private string subjectName;
        private string subjectLevel;
        private decimal mark;
        #endregion

        #region Properties
        /// <summary>Primary key (LearnerSubject.LearnerSubjectID).</summary>
        public int LearnerSubjectID
        {
            get { return learnerSubjectID; }
            set { learnerSubjectID = value; }
        }

        /// <summary>The advisory session the mark was captured in.</summary>
        public int SessionID
        {
            get { return sessionID; }
            set { sessionID = value; }
        }

        /// <summary>The subject (FK to Subject).</summary>
        public int SubjectID
        {
            get { return subjectID; }
            set { subjectID = value; }
        }

        /// <summary>The subject name (read from Subject, for display and explanations).</summary>
        public string SubjectName
        {
            get { return subjectName; }
            set { subjectName = value; }
        }

        /// <summary>The subject level (read from Subject, for display).</summary>
        public string SubjectLevel
        {
            get { return subjectLevel; }
            set { subjectLevel = value; }
        }

        /// <summary>The percentage mark, 0 to 100 with at most one decimal place.</summary>
        public decimal Mark
        {
            get { return mark; }
            set { mark = value; }
        }
        #endregion

        #region Constructors
        /// <summary>Creates an empty subject mark.</summary>
        public LearnerSubject()
        {
            learnerSubjectID = 0;
            sessionID = 0;
            subjectID = 0;
            subjectName = "";
            subjectLevel = "";
            mark = 0;
        }

        /// <summary>Creates a subject mark with all its values.</summary>
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
        /// <summary>
        /// Class diagram validateMarkRange(): true if this mark is from 0 to 100 with at most
        /// one decimal place. Boundary tests: -1 fails, 0 passes, 100 passes, 101 fails.
        /// </summary>
        public bool ValidateMarkRange()
        {
            return IsValidMark(mark);
        }

        /// <summary>True if the mark is from 0 to 100 with at most one decimal place.</summary>
        public static bool IsValidMark(decimal value)
        {
            return value >= MinimumMark && value <= MaximumMark && decimal.Round(value, 1) == value;
        }

        /// <summary>
        /// Reads a mark typed by the user ("75", "72.5" or "72,5"). Returns -1 for letters,
        /// signs, more than one decimal place, or a value outside 0 to 100, so bad input never
        /// throws an exception.
        /// </summary>
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

        /// <summary>Returns "Subject: mark%".</summary>
        public override string ToString()
        {
            return subjectName + ": " + mark.ToString("0.#", CultureInfo.InvariantCulture) + "%";
        }
        #endregion
    }
}
