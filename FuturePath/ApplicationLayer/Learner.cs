
using System;
using System.Collections.ObjectModel;

namespace FuturePath.ApplicationLayer
{
    // Stores the learner's details, subjects and interest responses
    // Learners can use the system without logging in
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
        public int LearnerID
        {
            get { return learnerID; }
            set { learnerID = value; }
        }

        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        public string SchoolName
        {
            get { return schoolName; }
            set { schoolName = value; }
        }

        public string Province
        {
            get { return province; }
            set { province = value; }
        }

        public string ContactEmail
        {
            get { return contactEmail; }
            set { contactEmail = value; }
        }

        public string ContactNumber
        {
            get { return contactNumber; }
            set { contactNumber = value; }
        }

        public GradeLevel Grade
        {
            get { return grade; }
            set { grade = value; }
        }

        public int? PreferredFacultyID
        {
            get { return preferredFacultyID; }
            set { preferredFacultyID = value; }
        }

        public int? CurrentSessionID
        {
            get { return currentSessionID; }
            set { currentSessionID = value; }
        }

        // Combine the learner's first name and surname
        // Use "Learner" if neither name was entered
        public string FullName
        {
            get
            {
                string name = "";
                if (firstName != null)
                {
                    name = firstName;
                }
                if (lastName != null)
                {
                    name = name + " " + lastName;
                }
                name = name.Trim();
                if (name.Length == 0)
                {
                    return "Learner";
                }
                return name;
            }
        }

        // Check whether subjects or interest responses have been captured
        public bool HasSubjects
        {
            get { return subjects.Count > 0; }
        }

        public bool HasInterestResponses
        {
            get { return responses.Count > 0; }
        }
        #endregion

        #region Constructors
        // Create a new learner with default values
        // Initialise the subject and interest response collections
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

        // Create a learner using the provided personal and school details
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
        // Add a subject and its mark to the learner's session
        // Check for duplicate subjects and marks outside the valid range
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
            LearnerSubject learnerSubject = new LearnerSubject(0, SessionIDOrZero(), subject.SubjectID, subject.SubjectName, subject.SubjectLevel, mark);
            subjects.Add(learnerSubject);
            return learnerSubject;
        }

        // Remove a subject from the learner's captured subjects
        public bool RemoveSubject(int subjectID)
        {
            LearnerSubject found = FindSubject(subjectID);
            if (found == null)
            {
                return false;
            }
            return subjects.Remove(found);
        }

        // Check if a subject has already been added
        public bool HasSubject(int subjectID)
        {
            return FindSubject(subjectID) != null;
        }

        // Find a captured subject using its ID
        // Return null if the subject is not found
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

        // Return the learner's captured subjects
        public Collection<LearnerSubject> GetSubjects()
        {
            return subjects;
        }

        // Return the learner's interest questionnaire responses
        public Collection<LearnerInterestResponse> GetResponses()
        {
            return responses;
        }

        // Save the response to an interest question
        // Update the existing response if the question was already answered
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
            responses.Add(new LearnerInterestResponse(0, SessionIDOrZero(), questionID, categoryID, responseValue));
        }

        // Return the learner's preferred faculty ID
        // Return 0 if no faculty has been selected
        public int GetPreferredFacultyId()
        {
            if (preferredFacultyID.HasValue)
            {
                return preferredFacultyID.Value;
            }
            return 0;
        }

        // Return the current advisory session ID
        // Return 0 if a session has not been created
        private int SessionIDOrZero()
        {
            if (currentSessionID.HasValue)
            {
                return currentSessionID.Value;
            }
            return 0;
        }

        // Calculate the learner's average score for an interest category
        // Return null if no questions in that category were answered
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

        // Calculate the weighted interest score for a programme
        // Use the category scores and their relevance weights
        // Return 0 if there are no valid scores
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

        // Return the learner's full name as a string
        public override string ToString()
        {
            return FullName;
        }
        #endregion
    }
}
