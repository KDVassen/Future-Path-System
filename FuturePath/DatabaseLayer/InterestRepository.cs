using System;
using System.Collections.ObjectModel;
using System.Data;
using FuturePath.ApplicationLayer;

namespace FuturePath.DatabaseLayer
{
    /// <summary>
    /// Data access for interest categories, questions and programme relevance weights
    /// (UI 4 questionnaire, UI 13.4 categories and weights, UI 13.5 questionnaire), in the
    /// Waitering DataSetChange / UpdateDataSource pattern.
    /// </summary>
    public class InterestRepository : DB
    {
        #region Data Members
        private string categoryTable = "InterestCategory";
        private string questionTable = "InterestQuestion";
        #endregion

        #region Constructor
        /// <summary>Creates the repository (no database work until a method is called).</summary>
        public InterestRepository() : base()
        {
        }
        #endregion

        #region Categories
        /// <summary>Categories ordered by name; inactive ones only when includeInactive is true.</summary>
        public Collection<InterestCategory> GetCategories(bool includeInactive)
        {
            throw new NotImplementedException();
        }

        /// <summary>UI 13.4 grid: CategoryID, CategoryName, QuestionCount, MappedProgrammeCount, IsActive (a new table every call).</summary>
        public DataTable GetCategoryTable(bool includeInactive)
        {
            throw new NotImplementedException();
        }

        /// <summary>One category, or null.</summary>
        public InterestCategory FindCategoryById(int categoryID)
        {
            throw new NotImplementedException();
        }

        /// <summary>True if another category already has this name.</summary>
        public bool CategoryNameExists(string categoryName, int? excludeCategoryID)
        {
            throw new NotImplementedException();
        }

        /// <summary>Adds a new category row to the DataSet, or changes the existing row (Edit).</summary>
        public void DataSetChange(InterestCategory category, DB.DBOperation operation)
        {
            throw new NotImplementedException();
        }

        /// <summary>Sends the category change to the database. Returns the CategoryID (the new key for Add).</summary>
        public int UpdateDataSource(InterestCategory category, DB.DBOperation operation)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Questions
        /// <summary>
        /// Questions with CategoryName, ordered by DisplayOrder then QuestionID. categoryID null
        /// means every category. Without includeInactive, inactive questions and questions in
        /// inactive categories are left out.
        /// </summary>
        public Collection<InterestQuestion> GetQuestions(int? categoryID, bool includeInactive)
        {
            throw new NotImplementedException();
        }

        /// <summary>One question with CategoryName, or null.</summary>
        public InterestQuestion FindQuestionById(int questionID)
        {
            throw new NotImplementedException();
        }

        /// <summary>Adds a new question row to the DataSet, or changes the existing row (Edit).</summary>
        public void DataSetChange(InterestQuestion question, DB.DBOperation operation)
        {
            throw new NotImplementedException();
        }

        /// <summary>Sends the question change to the database. Returns the QuestionID (the new key for Add).</summary>
        public int UpdateDataSource(InterestQuestion question, DB.DBOperation operation)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Relevance weights
        /// <summary>UI 13.4 weights grid for a category: ProgrammeID, ProgrammeName, RelevanceWeight.</summary>
        public DataTable GetRelevanceWeights(int categoryID)
        {
            throw new NotImplementedException();
        }

        /// <summary>Inserts or updates the DegreeInterestMapping row for (programme, category).</summary>
        public void SaveRelevanceWeight(int programmeID, int categoryID, decimal relevanceWeight)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
