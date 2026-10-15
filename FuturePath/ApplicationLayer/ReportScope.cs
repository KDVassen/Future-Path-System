namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// What a report request covers, for the UI 11 readiness panel: the number of sessions,
    /// faculties and programmes in scope. Returned by ReportController.GetScope; nothing is stored.
    /// </summary>
    public class ReportScope
    {
        #region Data Members
        private int sessionCount;
        private int facultyCount;
        private int programmeCount;
        #endregion

        #region Properties
        /// <summary>Completed sessions in the period after the filters.</summary>
        public int SessionCount
        {
            get { return sessionCount; }
            set { sessionCount = value; }
        }

        /// <summary>Different faculties among those sessions.</summary>
        public int FacultyCount
        {
            get { return facultyCount; }
            set { facultyCount = value; }
        }

        /// <summary>Different programmes among those sessions.</summary>
        public int ProgrammeCount
        {
            get { return programmeCount; }
            set { programmeCount = value; }
        }
        #endregion

        #region Constructors
        /// <summary>Creates an empty scope (all counts 0).</summary>
        public ReportScope()
        {
            sessionCount = 0;
            facultyCount = 0;
            programmeCount = 0;
        }

        /// <summary>Creates a scope with the three counts.</summary>
        public ReportScope(int pSessionCount, int pFacultyCount, int pProgrammeCount)
        {
            sessionCount = pSessionCount;
            facultyCount = pFacultyCount;
            programmeCount = pProgrammeCount;
        }
        #endregion
    }
}
