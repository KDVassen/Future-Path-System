
namespace FuturePath.ApplicationLayer
{
    // Stores information about a university faculty
    // Faculties offer degree programmes and can be deactivated
    public class Faculty
    {
        #region Data Members
        private int facultyID;
        private string facultyName;
        private bool isActive;
        #endregion

        #region Properties
        public int FacultyID
        {
            get { return facultyID; }
            set { facultyID = value; }
        }

        public string FacultyName
        {
            get { return facultyName; }
            set { facultyName = value; }
        }

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }
        #endregion

        #region Constructors
        // Create a new active faculty with default values
        public Faculty()
        {
            facultyID = 0;
            facultyName = "";
            isActive = true;
        }

        // Create a faculty using its stored details
        public Faculty(int pFacultyID, string pFacultyName, bool pIsActive)
        {
            facultyID = pFacultyID;
            facultyName = pFacultyName;
            isActive = pIsActive;
        }
        #endregion

        #region Methods
        // Return the faculty name for display
        public override string ToString()
        {
            return facultyName;
        }
        #endregion
    }
}
