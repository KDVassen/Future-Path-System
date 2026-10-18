
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using FuturePath.DatabaseLayer;

namespace FuturePath.ApplicationLayer
{
    // Handles interest questionnaires and programme interest matching
    // Also allows administrators to manage categories, questions and weights
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
        // Return the questions loaded by the last questionnaire request
        public Collection<InterestQuestion> AllQuestions
        {
            get { return questions; }
        }
        #endregion

        #region Constructor
        // Initialise the repositories and questionnaire collection
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
        // Load active questions in display order for the learner questionnaire
        public DataTable GetActiveQuestions()
        {
            questions = interestRepository.GetQuestions(0, false);

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

        // Calculate category scores from the answers given so far
        // Ignore unknown questions and sort scores from highest to lowest
        public List<KeyValuePair<string, decimal>> CalculateCategoryScores(IDictionary<int, int> responses)
        {
            List<KeyValuePair<string, decimal>> scores = new List<KeyValuePair<string, decimal>>();
            if (responses == null || responses.Count == 0)
            {
                return scores;
            }

            // Use the learner class to calculate scores from the answers
            Collection<InterestQuestion> activeQuestions = interestRepository.GetQuestions(0, false);
            Learner learner = new Learner();
            foreach (KeyValuePair<int, int> answer in responses)
            {
                InterestQuestion question = FindQuestion(activeQuestions, answer.Key);
                if (question != null)
                {
                    learner.SetResponse(question.QuestionID, question.CategoryID, answer.Value);
                }
            }

            // Calculate a score for each category with at least one answer
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

            // Sort the category scores from highest to lowest
            scores.Sort(CompareScores);
            return scores;
        }

        // Validate and save the learner's questionnaire answers
        // Replace any previously saved answers for this session
        public void SaveResponses(int sessionID, IDictionary<int, int> responses)
        {
            if (responses == null || responses.Count == 0)
            {
                throw new ArgumentException("Answer the interest statements before you continue.");
            }

            Collection<InterestQuestion> activeQuestions = interestRepository.GetQuestions(0, false);
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

        // Calculate interest alignment for the selected programme
        // Return the three highest-scoring matching categories
        public ProgrammeAlignment GetProgrammeAlignment(int sessionID, int programmeID)
        {
            ProgrammeAlignment alignment = new ProgrammeAlignment();
            Learner learner = learnerRepository.FindBySessionId(sessionID);
            DegreeProgramme programme = programmeRepository.FindByIdWithRequirements(programmeID);
            if (learner == null || programme == null)
            {
                return alignment;
            }

            Collection<InterestCategory> categories = programme.GetInterestCategories();
            alignment.AlignmentPercent = learner.CalculateInterestScore(categories);

            // Rank the learner's mapped interest categories by score
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

            string strongest = "";
            for (int i = 0; i < ranked.Count && i < StrongestCategoryCount; i++)
            {
                if (i > 0)
                {
                    strongest += ", ";
                }
                strongest += ranked[i].Key;
            }
            alignment.StrongestCategories = strongest;
            return alignment;
        }
        #endregion

        #region Categories and weights (UI 13.4)
        // Load the interest categories for administrator maintenance
        public DataTable GetInterestCategories(bool includeInactive)
        {
            return interestRepository.GetCategoryTable(includeInactive);
        }

        // Check that the category name is provided, short enough and unique
        public Collection<string> ValidateCategory(int categoryID, string categoryName)
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

        // Add or update an interest category after checking administrator access
        public int SaveInterestCategory(int actorUserID, int categoryID, string categoryName, bool isActive)
        {
            loginController.CheckAdministrator(actorUserID);
            Collection<string> problems = ValidateCategory(categoryID, categoryName);
            if (problems.Count > 0)
            {
                throw new ArgumentException(problems[0]);
            }

            InterestCategory category;
            DB.DBOperation operation;
            if (categoryID == 0)
            {
                category = new InterestCategory(0, categoryName.Trim(), isActive);
                operation = DB.DBOperation.Add;
            }
            else
            {
                category = FindCategory(categoryID);
                category.CategoryName = categoryName.Trim();
                category.IsActive = isActive;
                operation = DB.DBOperation.Edit;
            }

            DataMaintenance(category, operation);
            return FinalizeChanges(category, operation);
        }

        // Deactivate a category so its questions are no longer asked
        public void DeactivateInterestCategory(int actorUserID, int categoryID)
        {
            loginController.CheckAdministrator(actorUserID);
            InterestCategory category = FindCategory(categoryID);
            category.IsActive = false;
            DataMaintenance(category, DB.DBOperation.Edit);
            FinalizeChanges(category, DB.DBOperation.Edit);
        }

        // Load programme relevance weights for the selected category
        public DataTable GetRelevanceWeights(int categoryID)
        {
            return interestRepository.GetRelevanceWeights(categoryID);
        }

        // Save a programme's relevance weight for a category
        // Accept values from 0 to 1 with at most two decimal places
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
        // Load questions for one category, or all categories when the ID is zero
        public DataTable GetQuestions(int categoryID, bool includeInactive)
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

        // Validate the question text, active category and display order
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

        // Add or update a question after checking administrator access
        public int SaveQuestion(int actorUserID, int questionID, string questionText, int categoryID, int displayOrder, bool isActive)
        {
            loginController.CheckAdministrator(actorUserID);
            Collection<string> problems = ValidateQuestion(questionText, categoryID, displayOrder);
            if (problems.Count > 0)
            {
                throw new ArgumentException(problems[0]);
            }

            InterestQuestion question;
            DB.DBOperation operation;
            if (questionID == 0)
            {
                question = new InterestQuestion(0, categoryID, questionText.Trim(), displayOrder, isActive);
                operation = DB.DBOperation.Add;
            }
            else
            {
                question = FindQuestion(questionID);
                question.CategoryID = categoryID;
                question.QuestionText = questionText.Trim();
                question.DisplayOrder = displayOrder;
                question.IsActive = isActive;
                operation = DB.DBOperation.Edit;
            }

            DataMaintenance(question, operation);
            return FinalizeChanges(question, operation);
        }

        // Deactivate a question so it no longer appears to learners
        public void DeactivateQuestion(int actorUserID, int questionID)
        {
            loginController.CheckAdministrator(actorUserID);
            InterestQuestion question = FindQuestion(questionID);
            question.IsActive = false;
            DataMaintenance(question, DB.DBOperation.Edit);
            FinalizeChanges(question, DB.DBOperation.Edit);
        }
        #endregion

        #region Database communication
        // Apply a category change to the repository DataSet
        private void DataMaintenance(InterestCategory category, DB.DBOperation operation)
        {
            interestRepository.DataSetChange(category, operation);
        }

        // Save the category changes to the database
        private int FinalizeChanges(InterestCategory category, DB.DBOperation operation)
        {
            return interestRepository.UpdateDataSource(category, operation);
        }

        // Apply a question change to the repository DataSet
        private void DataMaintenance(InterestQuestion question, DB.DBOperation operation)
        {
            interestRepository.DataSetChange(question, operation);
        }

        // Save the question changes to the database
        private int FinalizeChanges(InterestQuestion question, DB.DBOperation operation)
        {
            return interestRepository.UpdateDataSource(question, operation);
        }
        #endregion

        #region Helpers
        // Find a category or report if it no longer exists
        private InterestCategory FindCategory(int categoryID)
        {
            InterestCategory category = interestRepository.FindCategoryById(categoryID);
            if (category == null)
            {
                throw new ArgumentException("The selected interest category no longer exists. Refresh the list and try again.");
            }
            return category;
        }

        // Find a question or report if it no longer exists
        private InterestQuestion FindQuestion(int questionID)
        {
            InterestQuestion question = interestRepository.FindQuestionById(questionID);
            if (question == null)
            {
                throw new ArgumentException("The selected statement no longer exists. Refresh the list and try again.");
            }
            return question;
        }

        // Search a question collection by ID
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

        // Sort scores highest first, then alphabetically for equal scores
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
