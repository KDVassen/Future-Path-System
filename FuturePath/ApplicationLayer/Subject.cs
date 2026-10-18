
namespace FuturePath.ApplicationLayer
{
    // Stores the school subjects available for learners to select
    // Includes the subject name, level and whether it is active
    public class Subject
    {
        #region Data Members
        private int subjectID;
        private string subjectName;
        private string subjectLevel;
        private bool isActive;
        #endregion

        #region Properties
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

        // Store the subject level, such as Core or Elective
        public string SubjectLevel
        {
            get { return subjectLevel; }
            set { subjectLevel = value; }
        }

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }
        #endregion

        #region Constructors
        // Create a new active subject with default values
        public Subject()
        {
            subjectID = 0;
            subjectName = "";
            subjectLevel = "";
            isActive = true;
        }

        // Create a subject using its stored details
        public Subject(int pSubjectID, string pSubjectName, string pSubjectLevel, bool pIsActive)
        {
            subjectID = pSubjectID;
            subjectName = pSubjectName;
            subjectLevel = pSubjectLevel;
            isActive = pIsActive;
        }
        #endregion

        #region Methods
        // Return the subject name for display
        public override string ToString()
        {
            return subjectName;
        }
        #endregion
    }
}
