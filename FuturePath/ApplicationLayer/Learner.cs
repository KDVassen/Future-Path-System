using System;
using System.Collections.ObjectModel;

namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// A learner using Future Path. Learners never log in.
    /// As in the class diagram, the in-memory Learner holds the grade, preferred faculty,
    /// subjects and interest responses of the learner's current advisory session. The database
    /// stores those on the session (decisions.md §3 "Learner model");
    /// LearnerRepository.FindById / FindBySessionId fill them in from the session rows.
    /// </summary>
    public class Learner
    {
        #region Data Members
        private int learnerID;
        private string firstName;
        private string lastName;
        private string schoolName;
        private string province;
        private string contactEmail;
        private string contactNumber;
        private GradeLevel grade;
        private int? preferredFacultyID;
        private int? currentSessionID;
        private Collection<LearnerSubject> subjects;
        private Collection<LearnerInterestResponse> responses;
        #endregion

        #region Properties
        /// <summary>Primary key (Learner.LearnerID).</summary>
        public int LearnerID
        {
            get { return learnerID; }
            set { learnerID = value; }
        }

        /// <summary>Optional first name.</summary>
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        /// <summary>Optional surname (Learner.LastName).</summary>
        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        /// <summary>School name (required, free text).</summary>
        public string SchoolName
        {
            get { return schoolName; }
            set { schoolName = value; }
        }

        /// <summary>Province (required, chosen from the PROVINCE_n system parameters).</summary>
        public string Province
        {
            get { return province; }
            set { province = value; }
        }

        /// <summary>Optional email address, used only for advisor follow-up.</summary>
        public string ContactEmail
        {
            get { return contactEmail; }
            set { contactEmail = value; }
        }

        /// <summary>Optional mobile number (0XX XXX XXXX), used only for advisor follow-up.</summary>
        public string ContactNumber
        {
            get { return contactNumber; }
            set { contactNumber = value; }
        }

        /// <summary>The grade in the current session (class diagram grade: GradeLevel).</summary>
        public GradeLevel Grade
        {
            get { return grade; }
            set { grade = value; }
        }

        /// <summary>The preferred faculty chosen on UI 5 in the current session, or null.</summary>
        public int? PreferredFacultyID
        {
            get { return preferredFacultyID; }
            set { preferredFacultyID = value; }
        }

        /// <summary>The advisory session this learner object was loaded for, or null.</summary>
        public int? CurrentSessionID
        {
            get { return currentSessionID; }
            set { currentSessionID = value; }
        }

        /// <summary>First name and surname, or "Learner" when neither was given.</summary>
        public string FullName
        {
            get
            {
                string name = ((firstName ?? "") + " " + (lastName ?? "")).Trim();
                return name.Length == 0 ? "Learner" : name;
            }
        }

        /// <summary>True once at least one subject has been captured.</summary>
        public bool HasSubjects
        {
            get { return subjects.Count > 0; }
        }

        /// <summary>True once at least one interest statement has been answered.</summary>
        public bool HasInterestResponses
        {
            get { return responses.Count > 0; }
        }
        #endregion

        #region Constructors
        /// <summary>Creates an empty Grade 12 learner with no subjects or responses.</summary>
        public Learner()
        {
            learnerID = 0;
            firstName = "";
            lastName = "";
            schoolName = "";
            province = "";
            contactEmail = "";
            contactNumber = "";
            grade = GradeLevel.Grade12;
            preferredFacultyID = null;
            currentSessionID = null;
            subjects = new Collection<LearnerSubject>();
            responses = new Collection<LearnerInterestResponse>();
        }

        /// <summary>
        /// Class diagram constructor Learner(name, grade, school, province, contactDetails),
        /// with the name and contact details split into their stored parts.
        /// </summary>
        public Learner(string pFirstName, string pLastName, GradeLevel pGrade, string pSchoolName, string pProvince, string pContactEmail, string pContactNumber)
            : this()
        {
            firstName = pFirstName;
            lastName = pLastName;
            grade = pGrade;
            schoolName = pSchoolName;
            province = pProvince;
            contactEmail = pContactEmail;
            contactNumber = pContactNumber;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Class diagram addSubject(subject, mark). Adds a subject mark to the current session.
        /// Enforces the field-combination control (no subject twice in one session) and the
        /// 0-100 mark range, and throws ArgumentException with a message the learner can act on.
        /// </summary>
        public LearnerSubject AddSubject(Subject subject, decimal mark)
        {
            if (subject == null || subject.SubjectID <= 0)
            {
                throw new ArgumentException("Choose a subject from the list.");
            }
            if (HasSubject(subject.SubjectID))
            {
                throw new ArgumentException(subject.SubjectName + " has already been captured in this session.");
            }
            if (!LearnerSubject.IsValidMark(mark))
            {
                throw new ArgumentException("Enter a mark for " + subject.SubjectName + " from 0 to 100, with at most one decimal place.");
            }
            LearnerSubject learnerSubject = new LearnerSubject(0, currentSessionID ?? 0, subject.SubjectID, subject.SubjectName, subject.SubjectLevel, mark);
            subjects.Add(learnerSubject);
            return learnerSubject;
        }

        /// <summary>Removes a captured subject. Returns false if it was not captured.</summary>
        public bool RemoveSubject(int subjectID)
        {
            LearnerSubject found = FindSubject(subjectID);
            if (found == null)
            {
                return false;
            }
            return subjects.Remove(found);
        }

        /// <summary>True if the subject is already captured in this session.</summary>
        public bool HasSubject(int subjectID)
        {
            return FindSubject(subjectID) != null;
        }

        /// <summary>Returns the captured mark for a subject, or null if it was not captured.</summary>
        public LearnerSubject FindSubject(int subjectID)
        {
            foreach (LearnerSubject learnerSubject in subjects)
            {
                if (learnerSubject.SubjectID == subjectID)
                {
                    return learnerSubject;
                }
            }
            return null;
        }

        /// <summary>
        /// Class diagram getSubjects(). Returns the session's captured subjects. The repository
        /// also adds stored rows to this collection when it loads the learner.
        /// </summary>
        public Collection<LearnerSubject> GetSubjects()
        {
            return subjects;
        }

        /// <summary>Returns the session's interest responses.</summary>
        public Collection<LearnerInterestResponse> GetResponses()
        {
            return responses;
        }

        /// <summary>Records (or replaces) the answer to one interest question.</summary>
        public void SetResponse(int questionID, int categoryID, int responseValue)
        {
            if (!LearnerInterestResponse.IsValidResponse(responseValue))
            {
                throw new ArgumentException("Choose an answer from Strongly disagree to Strongly agree for every statement.");
            }
            foreach (LearnerInterestResponse response in responses)
            {
                if (response.QuestionID == questionID)
                {
                    response.CategoryID = categoryID;
                    response.ResponseValue = responseValue;
                    return;
                }
            }
            responses.Add(new LearnerInterestResponse(0, currentSessionID ?? 0, questionID, categoryID, responseValue));
        }

        /// <summary>
        /// Class diagram getPreferredFacultyId(). Returns the preferred faculty, or 0 when none
        /// has been chosen yet (0 never matches a real faculty, so it earns no faculty bonus).
        /// </summary>
        public int GetPreferredFacultyId()
        {
            return preferredFacultyID ?? 0;
        }

        /// <summary>
        /// The learner's score for one interest category (0 to 100): the average of
        /// LearnerInterestResponse.GetScore() over the answered questions in that category.
        /// Returns null if no question in the category was answered.
        /// </summary>
        public decimal? CalculateCategoryScore(int categoryID)
        {
            decimal total = 0;
            int answered = 0;
            foreach (LearnerInterestResponse response in responses)
            {
                if (response.CategoryID == categoryID)
                {
                    total += response.GetScore();
                    answered++;
                }
            }
            if (answered == 0)
            {
                return null;
            }
            return Math.Round(total / answered, 2);
        }

        /// <summary>
        /// Class diagram calculateInterestScore(categories). The interest score (0 to 100) for a
        /// programme, from the categories the programme maps to: the relevance-weighted average
        /// of the learner's category scores, sum(weight x score) / sum(weight). Categories the
        /// learner did not answer are skipped. Returns 0 if none was answered.
        /// </summary>
        public decimal CalculateInterestScore(Collection<InterestCategory> categories)
        {
            decimal weightedTotal = 0;
            decimal weightTotal = 0;
            if (categories != null)
            {
                foreach (InterestCategory category in categories)
                {
                    decimal? score = CalculateCategoryScore(category.CategoryID);
                    if (score.HasValue && category.RelevanceWeight > 0)
                    {
                        weightedTotal += category.RelevanceWeight * score.Value;
                        weightTotal += category.RelevanceWeight;
                    }
                }
            }
            if (weightTotal == 0)
            {
                return 0;
            }
            return Math.Round(weightedTotal / weightTotal, 2);
        }

        /// <summary>Returns the learner's full name.</summary>
        public override string ToString()
        {
            return FullName;
        }
        #endregion
    }
}
