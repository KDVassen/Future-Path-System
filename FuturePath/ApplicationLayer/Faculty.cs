namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// A university faculty (e.g. Commerce) that offers degree programmes.
    /// Maps to the Faculty table. Faculties are never deleted, only deactivated.
    /// </summary>
    public class Faculty
    {
        #region Data Members
        private int facultyID;
        private string facultyName;
        private bool isActive;
        #endregion

        #region Properties
        /// <summary>Primary key (Faculty.FacultyID).</summary>
        public int FacultyID
        {
            get { return facultyID; }
            set { facultyID = value; }
        }

        /// <summary>The faculty name shown to learners and staff.</summary>
        public string FacultyName
        {
            get { return facultyName; }
            set { facultyName = value; }
        }

        /// <summary>False once an Administrator deactivates the faculty.</summary>
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }
        #endregion

        #region Constructors
        /// <summary>Creates an empty, active faculty.</summary>
        public Faculty()
        {
            facultyID = 0;
            facultyName = "";
            isActive = true;
        }

        /// <summary>Creates a faculty with all its values.</summary>
        public Faculty(int pFacultyID, string pFacultyName, bool pIsActive)
        {
            facultyID = pFacultyID;
            facultyName = pFacultyName;
            isActive = pIsActive;
        }
        #endregion

        #region Methods
        /// <summary>Returns the faculty name, so a ListBox or ComboBox shows it.</summary>
        public override string ToString()
        {
            return facultyName;
        }
        #endregion
    }
}
