
namespace FuturePath.ApplicationLayer
{
    // Stores the number of sessions, faculties and programmes covered by a report
    // Used to show the report scope before generating it
    public class ReportScope
    {
        #region Data Members
        private int sessionCount;
        private int facultyCount;
        private int programmeCount;
        #endregion

        #region Properties
        public int SessionCount
        {
            get { return sessionCount; }
            set { sessionCount = value; }
        }

        public int FacultyCount
        {
            get { return facultyCount; }
            set { facultyCount = value; }
        }

        public int ProgrammeCount
        {
            get { return programmeCount; }
            set { programmeCount = value; }
        }
        #endregion

        #region Constructors
        // Create an empty report scope with all counts set to zero
        public ReportScope()
        {
            sessionCount = 0;
            facultyCount = 0;
            programmeCount = 0;
        }

        // Create a report scope using the provided counts
        public ReportScope(int pSessionCount, int pFacultyCount, int pProgrammeCount)
        {
            sessionCount = pSessionCount;
            facultyCount = pFacultyCount;
            programmeCount = pProgrammeCount;
        }
        #endregion
    }
}
