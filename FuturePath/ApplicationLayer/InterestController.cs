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
        private const int MaximumCategoryNameLength = 60;
        private const int MaximumQuestionTextLength = 255;
        private const int MinimumDisplayOrder = 1;
        private const int MaximumDisplayOrder = 99;
        private const int StrongestCategoryCount = 3;

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
            questions = interestRepository.GetQuestions(null, false);

            DataTable table = new DataTable("Questions");
            table.Columns.Add("QuestionID", typeof(int));
            table.Columns.Add("QuestionText", typeof(string));
            table.Columns.Add("CategoryID", typeof(int));
            table.Columns.Add("CategoryName", typeof(string));
            table.Columns.Add("DisplayOrder", typeof(int));
            foreach (InterestQuestion question in questions)
            {
                table.Rows.Add(question.QuestionID, question.QuestionText, question.CategoryID, question.CategoryName, question.DisplayOrder);
            }
            return table;
        }

        /// <summary>
        /// The running category scores on UI 4 from the answers so far (question ID to 1-5), as
        /// (category name, 0-100) pairs, highest first, ties by name. Pure calculation: nothing is
        /// saved. Answers to unknown or inactive questions are ignored; a value outside 1-5 throws
        /// ArgumentException.
        /// </summary>
        public List<KeyValuePair<string, decimal>> CalculateCategoryScores(IDictionary<int, int> responses)
        {
            List<KeyValuePair<string, decimal>> scores = new List<KeyValuePair<string, decimal>>();
            if (responses == null || responses.Count == 0)
            {
                return scores;
            }

            // 1. An in-memory learner holds the answers, so the scoring rule lives in one place (Learner).
            Collection<InterestQuestion> activeQuestions = interestRepository.GetQuestions(null, false);
            Learner learner = new Learner();
            foreach (KeyValuePair<int, int> answer in responses)
            {
                InterestQuestion question = FindQuestion(activeQuestions, answer.Key);
                if (question != null)
                {
                    learner.SetResponse(question.QuestionID, question.CategoryID, answer.Value);
                }
            }

            // 2. One score for every category that has at least one answer.
            List<int> scoredCategories = new List<int>();
            foreach (InterestQuestion question in activeQuestions)
            {
                if (scoredCategories.Contains(question.CategoryID))
                {
                    continue;
                }
                scoredCategories.Add(question.CategoryID);
                decimal? score = learner.CalculateCategoryScore(question.CategoryID);
                if (score.HasValue)
                {
                    scores.Add(new KeyValuePair<string, decimal>(question.CategoryName, score.Value));
                }
            }

            // 3. Highest first.
            scores.Sort(CompareScores);
            return scores;
        }

        /// <summary>
        /// Saves the session's answers (question ID to 1-5), replacing earlier ones. There must be
        /// at least one answer, every value must be 1-5 and every question an active one. Throws
        /// ArgumentException if not.
        /// </summary>
        public void SaveResponses(int sessionID, IDictionary<int, int> responses)
        {
            if (responses == null || responses.Count == 0)
            {
                throw new ArgumentException("Answer the interest statements before you continue.");
            }

            Collection<InterestQuestion> activeQuestions = interestRepository.GetQuestions(null, false);
            Collection<LearnerInterestResponse> answers = new Collection<LearnerInterestResponse>();
            foreach (KeyValuePair<int, int> answer in responses)
            {
                InterestQuestion question = FindQuestion(activeQuestions, answer.Key);
                if (question == null)
                {
                    throw new ArgumentException("One of the statements is no longer part of the questionnaire. Please start the questionnaire again.");
                }
                if (!LearnerInterestResponse.IsValidResponse(answer.Value))
                {
                    throw new ArgumentException("Choose an answer from Strongly disagree to Strongly agree for every statement.");
                }
                answers.Add(new LearnerInterestResponse(0, sessionID, question.QuestionID, question.CategoryID, answer.Value));
            }
            learnerRepository.SaveResponses(sessionID, answers);
        }

        /// <summary>
        /// UI 5 interest-alignment strip: the learner's interest score for the programme
        /// (Learner.CalculateInterestScore over the programme's categories), and the names of the
        /// learner's strongest mapped categories (comma-separated, best first, at most three;
        /// categories with relevance weight 0 do not count). Returns 0 and "" if the session or
        /// the programme does not exist.
        /// </summary>
        public decimal GetProgrammeAlignment(int sessionID, int programmeID, out string strongestCategories)
        {
            strongestCategories = "";
            Learner learner = learnerRepository.FindBySessionId(sessionID);
            DegreeProgramme programme = programmeRepository.FindByIdWithRequirements(programmeID);
            if (learner == null || programme == null)
            {
                return 0;
            }

            Collection<InterestCategory> categories = programme.GetInterestCategories();
            decimal alignment = learner.CalculateInterestScore(categories);

            // The learner's own score in each mapped category, highest first.
            List<KeyValuePair<string, decimal>> ranked = new List<KeyValuePair<string, decimal>>();
            foreach (InterestCategory category in categories)
            {
                decimal? score = learner.CalculateCategoryScore(category.CategoryID);
                if (score.HasValue && category.RelevanceWeight > 0)
                {
                    ranked.Add(new KeyValuePair<string, decimal>(category.CategoryName, score.Value));
                }
            }
            ranked.Sort(CompareScores);

            for (int i = 0; i < ranked.Count && i < StrongestCategoryCount; i++)
            {
                strongestCategories += (i > 0 ? ", " : "") + ranked[i].Key;
            }
            return alignment;
        }
        #endregion

        #region Categories and weights (UI 13.4)
        /// <summary>CategoryID, CategoryName, QuestionCount, MappedProgrammeCount, IsActive (a new table every call, safe for two combos).</summary>
        public DataTable GetInterestCategories(bool includeInactive)
        {
            return interestRepository.GetCategoryTable(includeInactive);
        }

        /// <summary>Checks a category: name required, at most 60 characters, not already used.</summary>
        public Collection<string> ValidateCategory(int? categoryID, string categoryName)
        {
            Collection<string> problems = new Collection<string>();
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                problems.Add("Enter a category name.");
            }
            else if (categoryName.Trim().Length > MaximumCategoryNameLength)
            {
                problems.Add("Shorten the category name to " + MaximumCategoryNameLength + " characters or fewer.");
            }
            else if (interestRepository.CategoryNameExists(categoryName, categoryID))
            {
                problems.Add("A category called " + categoryName.Trim() + " already exists. Enter a different name.");
            }
            return problems;
        }

        /// <summary>
        /// Adds (categoryID null) or updates a category (Administrator only). Returns the CategoryID.
        /// Throws ArgumentException if invalid, UnauthorizedAccessException if not an Administrator.
        /// </summary>
        public int SaveInterestCategory(int actorUserID, int? categoryID, string categoryName, bool isActive)
        {
            loginController.CheckAdministrator(actorUserID);
            Collection<string> problems = ValidateCategory(categoryID, categoryName);
            if (problems.Count > 0)
            {
                throw new ArgumentException(problems[0]);
            }

            InterestCategory category;
            DB.DBOperation operation;
            if (!categoryID.HasValue)
            {
                category = new InterestCategory(0, categoryName.Trim(), isActive);
                operation = DB.DBOperation.Add;
            }
            else
            {
                category = FindCategory(categoryID.Value);
                category.CategoryName = categoryName.Trim();
                category.IsActive = isActive;
                operation = DB.DBOperation.Edit;
            }

            DataMaintenance(category, operation);
            return FinalizeChanges(category, operation);
        }

        /// <summary>Deactivates a category (Administrator only). Its questions are then no longer asked.</summary>
        public void DeactivateInterestCategory(int actorUserID, int categoryID)
        {
            loginController.CheckAdministrator(actorUserID);
            InterestCategory category = FindCategory(categoryID);
            category.IsActive = false;
            DataMaintenance(category, DB.DBOperation.Edit);
            FinalizeChanges(category, DB.DBOperation.Edit);
        }

        /// <summary>The programmes mapped to a category: ProgrammeID, ProgrammeName, RelevanceWeight.</summary>
        public DataTable GetRelevanceWeights(int categoryID)
        {
            return interestRepository.GetRelevanceWeights(categoryID);
        }

        /// <summary>
        /// Sets a programme's relevance weight for a category, 0.00-1.00 with at most two decimals
        /// (Administrator only). The category and the programme must exist. Adds the mapping if
        /// there is none yet, otherwise changes its weight.
        /// </summary>
        public void SaveRelevanceWeight(int actorUserID, int programmeID, int categoryID, decimal relevanceWeight)
        {
            loginController.CheckAdministrator(actorUserID);
            if (relevanceWeight < 0 || relevanceWeight > 1 || decimal.Round(relevanceWeight, 2) != relevanceWeight)
            {
                throw new ArgumentException("Enter a relevance weight from 0.00 to 1.00, with at most two decimal places.");
            }
            if (interestRepository.FindCategoryById(categoryID) == null)
            {
                throw new ArgumentException("Choose an interest category from the list.");
            }
            if (programmeRepository.FindById(programmeID) == null)
            {
                throw new ArgumentException("Choose a degree programme from the list.");
            }
            interestRepository.SaveRelevanceWeight(programmeID, categoryID, relevanceWeight);
        }
        #endregion

        #region Questions (UI 13.5)
        /// <summary>
        /// QuestionID, CategoryID, DisplayOrder, QuestionText, CategoryName, IsActive, ordered by
        /// DisplayOrder; categoryID null means all.
        /// </summary>
        public DataTable GetQuestions(int? categoryID, bool includeInactive)
        {
            DataTable table = new DataTable("Questions");
            table.Columns.Add("QuestionID", typeof(int));
            table.Columns.Add("CategoryID", typeof(int));
            table.Columns.Add("DisplayOrder", typeof(int));
            table.Columns.Add("QuestionText", typeof(string));
            table.Columns.Add("CategoryName", typeof(string));
            table.Columns.Add("IsActive", typeof(bool));
            foreach (InterestQuestion question in interestRepository.GetQuestions(categoryID, includeInactive))
            {
                table.Rows.Add(question.QuestionID, question.CategoryID, question.DisplayOrder, question.QuestionText,
                               question.CategoryName, question.IsActive);
            }
            return table;
        }

        /// <summary>Checks a question: text required (max 255), existing active category, display order 1-99.</summary>
        public Collection<string> ValidateQuestion(string questionText, int categoryID, int displayOrder)
        {
            Collection<string> problems = new Collection<string>();
            if (string.IsNullOrWhiteSpace(questionText))
            {
                problems.Add("Enter the statement the learner will agree or disagree with.");
            }
            else if (questionText.Trim().Length > MaximumQuestionTextLength)
            {
                problems.Add("Shorten the statement to " + MaximumQuestionTextLength + " characters or fewer.");
            }

            InterestCategory category = interestRepository.FindCategoryById(categoryID);
            if (category == null || !category.IsActive)
            {
                problems.Add("Choose an active interest category for the statement.");
            }

            if (displayOrder < MinimumDisplayOrder || displayOrder > MaximumDisplayOrder)
            {
                problems.Add("Enter a display order from " + MinimumDisplayOrder + " to " + MaximumDisplayOrder + ".");
            }
            return problems;
        }

        /// <summary>
        /// Adds (questionID null) or updates a question (Administrator only). Returns the QuestionID.
        /// Throws ArgumentException if invalid, UnauthorizedAccessException if not an Administrator.
        /// </summary>
        public int SaveQuestion(int actorUserID, int? questionID, string questionText, int categoryID, int displayOrder, bool isActive)
        {
            loginController.CheckAdministrator(actorUserID);
            Collection<string> problems = ValidateQuestion(questionText, categoryID, displayOrder);
            if (problems.Count > 0)
            {
                throw new ArgumentException(problems[0]);
            }

            InterestQuestion question;
            DB.DBOperation operation;
            if (!questionID.HasValue)
            {
                question = new InterestQuestion(0, categoryID, questionText.Trim(), displayOrder, isActive);
                operation = DB.DBOperation.Add;
            }
            else
            {
                question = FindQuestion(questionID.Value);
                question.CategoryID = categoryID;
                question.QuestionText = questionText.Trim();
                question.DisplayOrder = displayOrder;
                question.IsActive = isActive;
                operation = DB.DBOperation.Edit;
            }

            DataMaintenance(question, operation);
            return FinalizeChanges(question, operation);
        }

        /// <summary>Deactivates a question (Administrator only).</summary>
        public void DeactivateQuestion(int actorUserID, int questionID)
        {
            loginController.CheckAdministrator(actorUserID);
            InterestQuestion question = FindQuestion(questionID);
            question.IsActive = false;
            DataMaintenance(question, DB.DBOperation.Edit);
            FinalizeChanges(question, DB.DBOperation.Edit);
        }
        #endregion

        #region Database communication (Waitering DataMaintenance / FinalizeChanges)
        /// <summary>Waitering DataMaintenance: makes the category change in the repository's DataSet. Private, so the role check cannot be skipped.</summary>
        private void DataMaintenance(InterestCategory category, DB.DBOperation operation)
        {
            interestRepository.DataSetChange(category, operation);
        }

        /// <summary>Waitering FinalizeChanges: commits the category change to the database and returns the CategoryID.</summary>
        private int FinalizeChanges(InterestCategory category, DB.DBOperation operation)
        {
            return interestRepository.UpdateDataSource(category, operation);
        }

        /// <summary>Waitering DataMaintenance: makes the question change in the repository's DataSet.</summary>
        private void DataMaintenance(InterestQuestion question, DB.DBOperation operation)
        {
            interestRepository.DataSetChange(question, operation);
        }

        /// <summary>Waitering FinalizeChanges: commits the question change to the database and returns the QuestionID.</summary>
        private int FinalizeChanges(InterestQuestion question, DB.DBOperation operation)
        {
            return interestRepository.UpdateDataSource(question, operation);
        }
        #endregion

        #region Helpers
        /// <summary>Loads a category for an edit, or throws ArgumentException if it no longer exists.</summary>
        private InterestCategory FindCategory(int categoryID)
        {
            InterestCategory category = interestRepository.FindCategoryById(categoryID);
            if (category == null)
            {
                throw new ArgumentException("The selected interest category no longer exists. Refresh the list and try again.");
            }
            return category;
        }

        /// <summary>Loads a question for an edit, or throws ArgumentException if it no longer exists.</summary>
        private InterestQuestion FindQuestion(int questionID)
        {
            InterestQuestion question = interestRepository.FindQuestionById(questionID);
            if (question == null)
            {
                throw new ArgumentException("The selected statement no longer exists. Refresh the list and try again.");
            }
            return question;
        }

        /// <summary>The question with this ID in a list, or null.</summary>
        private static InterestQuestion FindQuestion(Collection<InterestQuestion> list, int questionID)
        {
            foreach (InterestQuestion question in list)
            {
                if (question.QuestionID == questionID)
                {
                    return question;
                }
            }
            return null;
        }

        /// <summary>Orders (name, score) pairs by score, highest first, then by name.</summary>
        private static int CompareScores(KeyValuePair<string, decimal> first, KeyValuePair<string, decimal> second)
        {
            int byScore = second.Value.CompareTo(first.Value);
            if (byScore != 0)
            {
                return byScore;
            }
            return string.Compare(first.Key, second.Key, StringComparison.CurrentCulture);
        }
        #endregion
    }
}
