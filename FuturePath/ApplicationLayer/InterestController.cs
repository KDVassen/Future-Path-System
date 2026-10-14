using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using FuturePath.DatabaseLayer;

namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// The interest questionnaire: UI 4 (questions, running category scores, saving answers),
    /// UI 5's interest-alignment strip, and the Administrator maintenance of categories,
    /// relevance weights (UI 13.4) and questions (UI 13.5). Maintenance methods check the
    /// Administrator role first.
    /// </summary>
    public class InterestController
    {
        #region Data Members
        private InterestRepository interestRepository;
        private LearnerRepository learnerRepository;
        private DegreeProgrammeRepository programmeRepository;
        private LoginController loginController;
        private Collection<InterestQuestion> questions;
        #endregion

        #region Properties
        /// <summary>The questions read by the last GetActiveQuestions call.</summary>
        public Collection<InterestQuestion> AllQuestions
        {
            get { return questions; }
        }
        #endregion

        #region Constructor
        /// <summary>Creates the controller and its repositories (no database work yet).</summary>
        public InterestController()
        {
            interestRepository = new InterestRepository();
            learnerRepository = new LearnerRepository();
            programmeRepository = new DegreeProgrammeRepository();
            loginController = new LoginController();
            questions = new Collection<InterestQuestion>();
        }
        #endregion

        #region Learner questionnaire (UI 4, UI 5)
        /// <summary>UI 4 statements: QuestionID, QuestionText, CategoryID, CategoryName, DisplayOrder (active only, in display order).</summary>
        public DataTable GetActiveQuestions()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The running category scores on UI 4 from the answers so far (question ID to 1-5), as
        /// (category name, 0-100) pairs, highest first. Pure calculation: nothing is saved.
        /// </summary>
        public List<KeyValuePair<string, decimal>> CalculateCategoryScores(IDictionary<int, int> responses)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Saves the session's answers (question ID to 1-5), replacing earlier ones. Every value
        /// must be 1-5 and every question an active one. Throws ArgumentException if not.
        /// </summary>
        public void SaveResponses(int sessionID, IDictionary<int, int> responses)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// UI 5 interest-alignment strip: the learner's interest score for the programme
        /// (Learner.CalculateInterestScore over the programme's categories), and the names of the
        /// learner's strongest mapped categories (comma-separated, best first, at most three).
        /// </summary>
        public decimal GetProgrammeAlignment(int sessionID, int programmeID, out string strongestCategories)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Categories and weights (UI 13.4)
        /// <summary>CategoryID, CategoryName, QuestionCount, MappedProgrammeCount, IsActive (a new table every call, safe for two combos).</summary>
        public DataTable GetInterestCategories(bool includeInactive)
        {
            throw new NotImplementedException();
        }

        /// <summary>Checks a category: name required, at most 60 characters, not already used.</summary>
        public Collection<string> ValidateCategory(int? categoryID, string categoryName)
        {
            throw new NotImplementedException();
        }

        /// <summary>Adds (categoryID null) or updates a category (Administrator only). Returns the CategoryID.</summary>
        public int SaveInterestCategory(int actorUserID, int? categoryID, string categoryName, bool isActive)
        {
            throw new NotImplementedException();
        }

        /// <summary>Deactivates a category (Administrator only). Its questions are then no longer asked.</summary>
        public void DeactivateInterestCategory(int actorUserID, int categoryID)
        {
            throw new NotImplementedException();
        }

        /// <summary>The programmes mapped to a category: ProgrammeID, ProgrammeName, RelevanceWeight.</summary>
        public DataTable GetRelevanceWeights(int categoryID)
        {
            throw new NotImplementedException();
        }

        /// <summary>Sets a programme's relevance weight for a category, 0.00-1.00 with two decimals (Administrator only).</summary>
        public void SaveRelevanceWeight(int actorUserID, int programmeID, int categoryID, decimal relevanceWeight)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Questions (UI 13.5)
        /// <summary>QuestionID, CategoryID, DisplayOrder, QuestionText, CategoryName, IsActive; categoryID null means all.</summary>
        public DataTable GetQuestions(int? categoryID, bool includeInactive)
        {
            throw new NotImplementedException();
        }

        /// <summary>Checks a question: text required (max 255), existing active category, display order 1-99.</summary>
        public Collection<string> ValidateQuestion(string questionText, int categoryID, int displayOrder)
        {
            throw new NotImplementedException();
        }

        /// <summary>Adds (questionID null) or updates a question (Administrator only). Returns the QuestionID.</summary>
        public int SaveQuestion(int actorUserID, int? questionID, string questionText, int categoryID, int displayOrder, bool isActive)
        {
            throw new NotImplementedException();
        }

        /// <summary>Deactivates a question (Administrator only).</summary>
        public void DeactivateQuestion(int actorUserID, int questionID)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
