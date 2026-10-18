
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using FuturePath.ApplicationLayer;

namespace FuturePath.DatabaseLayer
{
    // Handles interest categories, questionnaire questions and programme relevance weights
    // Loads and saves the information used for interest-based recommendations
    public class InterestRepository : DB
    {
        #region Data Members
        private string categoryTable = "InterestCategory";
        private string questionTable = "InterestQuestion";
        private string categoryListTable = "CategoryList";
        private string weightTable = "RelevanceWeight";
        private string sqlCategories = "SELECT CategoryID, CategoryName, IsActive FROM InterestCategory";
        private string sqlQuestions = "SELECT q.QuestionID, q.CategoryID, c.CategoryName, q.QuestionText, q.DisplayOrder, q.IsActive " +
                                      "FROM InterestQuestion q INNER JOIN InterestCategory c ON c.CategoryID = q.CategoryID";
        private Collection<InterestCategory> categories;
        private Collection<InterestQuestion> questions;
        #endregion

        #region Constructor
        // Initialise the interest category and question collections
        public InterestRepository() : base()
        {
            categories = new Collection<InterestCategory>();
            questions = new Collection<InterestQuestion>();
        }
        #endregion

        #region Categories
        // Load interest categories ordered by name
        // Include inactive categories when requested
        public Collection<InterestCategory> GetCategories(bool includeInactive)
        {
            string sql = sqlCategories;
            if (!includeInactive)
            {
                sql += " WHERE IsActive = 1";
            }
            FillDataSet(sql + " ORDER BY CategoryName", categoryTable);
            AddCategories2Collection();
            return categories;
        }

        // Load category details with their question and programme mapping counts
        public DataTable GetCategoryTable(bool includeInactive)
        {
            string sql = "SELECT c.CategoryID, c.CategoryName, " +
                         "(SELECT COUNT(*) FROM InterestQuestion q WHERE q.CategoryID = c.CategoryID AND q.IsActive = 1) AS QuestionCount, " +
                         "(SELECT COUNT(*) FROM DegreeInterestMapping m WHERE m.CategoryID = c.CategoryID) AS MappedProgrammeCount, " +
                         "c.IsActive " +
                         "FROM InterestCategory c";
            if (!includeInactive)
            {
                sql += " WHERE c.IsActive = 1";
            }
            FillDataSet(sql + " ORDER BY c.CategoryName", categoryListTable);
            return GetTable(categoryListTable).Copy();
        }

        // Find an interest category by its ID
        public InterestCategory FindCategoryById(int categoryID)
        {
            FillDataSet(sqlCategories + " WHERE CategoryID = @CategoryID", categoryTable, Param("@CategoryID", SqlDbType.Int, categoryID));
            AddCategories2Collection();
            if (categories.Count == 0)
            {
                return null;
            }
            return categories[0];
        }

        // Check whether another interest category has the same name
        public bool CategoryNameExists(string categoryName, int excludeCategoryID)
        {
            object count = ExecuteScalar("SELECT COUNT(*) FROM InterestCategory WHERE CategoryName = @CategoryName AND CategoryID <> @ExcludeID",
                                         Param("@CategoryName", SqlDbType.VarChar, TextOrEmpty(categoryName)),
                                         Param("@ExcludeID", SqlDbType.Int, excludeCategoryID));
            return Convert.ToInt32(count) > 0;
        }

        // Add a new interest category or update an existing one in the DataSet
        public void DataSetChange(InterestCategory category, DB.DBOperation operation)
        {
            DataRow row;
            if (operation == DB.DBOperation.Add)
            {
                // Load the table structure before adding a new category
                FillDataSet(sqlCategories + " WHERE 1 = 0", categoryTable);
                row = GetTable(categoryTable).NewRow();
                FillRow(row, category);
                GetTable(categoryTable).Rows.Add(row);
            }
            else
            {
                FillDataSet(sqlCategories + " WHERE CategoryID = @CategoryID", categoryTable, Param("@CategoryID", SqlDbType.Int, category.CategoryID));
                row = FindRow(categoryTable, "CategoryID", category.CategoryID);
                if (row == null)
                {
                    throw new InvalidOperationException("The interest category to update was not found.");
                }
                FillRow(row, category);
            }
        }

        // Save category changes to the database and return the category ID
        public int UpdateDataSource(InterestCategory category, DB.DBOperation operation)
        {
            if (operation == DB.DBOperation.Add)
            {
                Create_INSERT_Command(category);
            }
            else
            {
                Create_UPDATE_Command(category);
            }
            UpdateDataSource(categoryTable);
            if (operation == DB.DBOperation.Add)
            {
                // Retrieve the new category ID after insertion
                DataTable rows = GetTable(categoryTable);
                category.CategoryID = ReadInt(rows.Rows[rows.Rows.Count - 1], "CategoryID");
            }
            return category.CategoryID;
        }
        #endregion

        #region Questions
        // Load questions ordered by display order and question ID
        // Optionally filter by category and include inactive questions
        public Collection<InterestQuestion> GetQuestions(int categoryID, bool includeInactive)
        {
            string sql = sqlQuestions + " WHERE 1 = 1";
            List<SqlParameter> parameters = new List<SqlParameter>();
            if (categoryID > 0)
            {
                sql += " AND q.CategoryID = @CategoryID";
                parameters.Add(Param("@CategoryID", SqlDbType.Int, categoryID));
            }
            if (!includeInactive)
            {
                sql += " AND q.IsActive = 1 AND c.IsActive = 1";
            }
            FillDataSet(sql + " ORDER BY q.DisplayOrder, q.QuestionID", questionTable, parameters.ToArray());
            AddQuestions2Collection();
            return questions;
        }

        // Find an interest question by its ID
        public InterestQuestion FindQuestionById(int questionID)
        {
            FillDataSet(sqlQuestions + " WHERE q.QuestionID = @QuestionID", questionTable, Param("@QuestionID", SqlDbType.Int, questionID));
            AddQuestions2Collection();
            if (questions.Count == 0)
            {
                return null;
            }
            return questions[0];
        }

        // Add a new question or update an existing one in the DataSet
        public void DataSetChange(InterestQuestion question, DB.DBOperation operation)
        {
            DataRow row;
            if (operation == DB.DBOperation.Add)
            {
                // Load the table structure before adding a new question
                FillDataSet(sqlQuestions + " WHERE 1 = 0", questionTable);
                row = GetTable(questionTable).NewRow();
                FillRow(row, question);
                GetTable(questionTable).Rows.Add(row);
            }
            else
            {
                FillDataSet(sqlQuestions + " WHERE q.QuestionID = @QuestionID", questionTable, Param("@QuestionID", SqlDbType.Int, question.QuestionID));
                row = FindRow(questionTable, "QuestionID", question.QuestionID);
                if (row == null)
                {
                    throw new InvalidOperationException("The interest question to update was not found.");
                }
                FillRow(row, question);
            }
        }

        // Save question changes to the database and return the question ID
        public int UpdateDataSource(InterestQuestion question, DB.DBOperation operation)
        {
            if (operation == DB.DBOperation.Add)
            {
                Create_INSERT_Command(question);
            }
            else
            {
                Create_UPDATE_Command(question);
            }
            UpdateDataSource(questionTable);
            if (operation == DB.DBOperation.Add)
            {
                // Retrieve the new question ID after insertion
                DataTable rows = GetTable(questionTable);
                question.QuestionID = ReadInt(rows.Rows[rows.Rows.Count - 1], "QuestionID");
            }
            return question.QuestionID;
        }
        #endregion

        #region Relevance weights
        // Load programme relevance weights for the selected interest category
        public DataTable GetRelevanceWeights(int categoryID)
        {
            FillDataSet("SELECT m.ProgrammeID, p.ProgrammeName, m.RelevanceWeight FROM DegreeInterestMapping m " +
                        "INNER JOIN DegreeProgramme p ON p.ProgrammeID = m.ProgrammeID " +
                        "WHERE m.CategoryID = @CategoryID ORDER BY p.ProgrammeName", weightTable,
                        Param("@CategoryID", SqlDbType.Int, categoryID));
            return GetTable(weightTable).Copy();
        }

        // Update a programme's relevance weight or insert a new mapping
        public void SaveRelevanceWeight(int programmeID, int categoryID, decimal relevanceWeight)
        {
            // Insert a mapping only if an existing row was not updated
            ExecuteNonQuery("UPDATE DegreeInterestMapping SET RelevanceWeight = @Weight WHERE ProgrammeID = @ProgrammeID AND CategoryID = @CategoryID; " +
                            "IF @@ROWCOUNT = 0 " +
                            "INSERT INTO DegreeInterestMapping (ProgrammeID, CategoryID, RelevanceWeight) VALUES (@ProgrammeID, @CategoryID, @Weight)",
                            Param("@Weight", SqlDbType.Decimal, relevanceWeight),
                            Param("@ProgrammeID", SqlDbType.Int, programmeID),
                            Param("@CategoryID", SqlDbType.Int, categoryID));
        }
        #endregion

        #region Utility Methods
        // Convert database rows into InterestCategory objects
        private void AddCategories2Collection()
        {
            categories = new Collection<InterestCategory>();
            foreach (DataRow row in GetTable(categoryTable).Rows)
            {
                categories.Add(new InterestCategory(ReadInt(row, "CategoryID"), ReadString(row, "CategoryName"), ReadBool(row, "IsActive")));
            }
        }

        // Convert database rows into InterestQuestion objects
        private void AddQuestions2Collection()
        {
            questions = new Collection<InterestQuestion>();
            foreach (DataRow row in GetTable(questionTable).Rows)
            {
                InterestQuestion question = new InterestQuestion(ReadInt(row, "QuestionID"), ReadInt(row, "CategoryID"),
                                                                 ReadString(row, "QuestionText"), ReadInt(row, "DisplayOrder"),
                                                                 ReadBool(row, "IsActive"));
                question.CategoryName = ReadString(row, "CategoryName");
                questions.Add(question);
            }
        }

        // Copy an interest category's values into a DataRow
        private void FillRow(DataRow row, InterestCategory category)
        {
            row["CategoryName"] = category.CategoryName.Trim();
            row["IsActive"] = category.IsActive;
        }

        // Copy an interest question's values into a DataRow
        private void FillRow(DataRow row, InterestQuestion question)
        {
            row["CategoryID"] = question.CategoryID;
            row["QuestionText"] = question.QuestionText.Trim();
            row["DisplayOrder"] = question.DisplayOrder;
            row["IsActive"] = question.IsActive;
        }
        #endregion

        #region Build Parameters, Create Commands
        // Add the category parameters to the INSERT command
        private void Build_INSERT_Parameters(InterestCategory category)
        {
            daMain.InsertCommand.Parameters.Add(SourceParam("@CategoryName", SqlDbType.VarChar, "CategoryName"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@IsActive", SqlDbType.Bit, "IsActive"));
        }

        // Create the SQL command for inserting a new interest category
        private void Create_INSERT_Command(InterestCategory category)
        {
            daMain.InsertCommand = new SqlCommand("INSERT INTO InterestCategory (CategoryName, IsActive) VALUES (@CategoryName, @IsActive); " +
                                                  "SELECT CAST(SCOPE_IDENTITY() AS INT) AS CategoryID", cnMain);
            daMain.InsertCommand.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;
            Build_INSERT_Parameters(category);
        }

        // Add the category parameters and ID to the UPDATE command
        private void Build_UPDATE_Parameters(InterestCategory category)
        {
            daMain.UpdateCommand.Parameters.Add(SourceParam("@CategoryName", SqlDbType.VarChar, "CategoryName"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@IsActive", SqlDbType.Bit, "IsActive"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@CategoryID", SqlDbType.Int, "CategoryID"));
        }

        // Create the SQL command for updating an interest category
        private void Create_UPDATE_Command(InterestCategory category)
        {
            daMain.UpdateCommand = new SqlCommand("UPDATE InterestCategory SET CategoryName = @CategoryName, IsActive = @IsActive WHERE CategoryID = @CategoryID", cnMain);
            Build_UPDATE_Parameters(category);
        }

        // Add the question parameters to the INSERT command
        private void Build_INSERT_Parameters(InterestQuestion question)
        {
            daMain.InsertCommand.Parameters.Add(SourceParam("@CategoryID", SqlDbType.Int, "CategoryID"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@QuestionText", SqlDbType.VarChar, "QuestionText"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@DisplayOrder", SqlDbType.Int, "DisplayOrder"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@IsActive", SqlDbType.Bit, "IsActive"));
        }

        // Create the SQL command for inserting a new interest question
        private void Create_INSERT_Command(InterestQuestion question)
        {
            daMain.InsertCommand = new SqlCommand("INSERT INTO InterestQuestion (CategoryID, QuestionText, DisplayOrder, IsActive) " +
                                                  "VALUES (@CategoryID, @QuestionText, @DisplayOrder, @IsActive); " +
                                                  "SELECT CAST(SCOPE_IDENTITY() AS INT) AS QuestionID", cnMain);
            daMain.InsertCommand.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;
            Build_INSERT_Parameters(question);
        }

        // Add the question parameters and ID to the UPDATE command
        private void Build_UPDATE_Parameters(InterestQuestion question)
        {
            daMain.UpdateCommand.Parameters.Add(SourceParam("@CategoryID", SqlDbType.Int, "CategoryID"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@QuestionText", SqlDbType.VarChar, "QuestionText"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@DisplayOrder", SqlDbType.Int, "DisplayOrder"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@IsActive", SqlDbType.Bit, "IsActive"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@QuestionID", SqlDbType.Int, "QuestionID"));
        }

        // Create the SQL command for updating an interest question
        private void Create_UPDATE_Command(InterestQuestion question)
        {
            daMain.UpdateCommand = new SqlCommand("UPDATE InterestQuestion SET CategoryID = @CategoryID, QuestionText = @QuestionText, " +
                                                  "DisplayOrder = @DisplayOrder, IsActive = @IsActive WHERE QuestionID = @QuestionID", cnMain);
            Build_UPDATE_Parameters(question);
        }
        #endregion
    }
}
