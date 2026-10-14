namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// A school subject a learner can capture (e.g. Mathematics). Maps to the Subject table.
    /// The subject level (Home Language, First Additional Language, Core, Elective) is stored
    /// with the subject, so UI 3 fills it in read-only (decisions.md D4).
    /// </summary>
    public class Subject
    {
        #region Data Members
        private int subjectID;
        private string subjectName;
        private string subjectLevel;
        private bool isActive;
        #endregion

        #region Properties
        /// <summary>Primary key (Subject.SubjectID).</summary>
        public int SubjectID
        {
            get { return subjectID; }
            set { subjectID = value; }
        }

        /// <summary>The subject name, e.g. "Mathematics".</summary>
        public string SubjectName
        {
            get { return subjectName; }
            set { subjectName = value; }
        }

        /// <summary>Home Language, First Additional Language, Core or Elective.</summary>
        public string SubjectLevel
        {
            get { return subjectLevel; }
            set { subjectLevel = value; }
        }

        /// <summary>Inactive subjects are not offered on UI 3.</summary>
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }
        #endregion

        #region Constructors
        /// <summary>Creates an empty, active subject.</summary>
        public Subject()
        {
            subjectID = 0;
            subjectName = "";
            subjectLevel = "";
            isActive = true;
        }

        /// <summary>Creates a subject with all its values.</summary>
        public Subject(int pSubjectID, string pSubjectName, string pSubjectLevel, bool pIsActive)
        {
            subjectID = pSubjectID;
            subjectName = pSubjectName;
            subjectLevel = pSubjectLevel;
            isActive = pIsActive;
        }
        #endregion

        #region Methods
        /// <summary>Returns the subject name, so a ComboBox shows it.</summary>
        public override string ToString()
        {
            return subjectName;
        }
        #endregion
    }
}
