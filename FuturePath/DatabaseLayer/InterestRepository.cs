using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
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
        private string categoryListTable = "CategoryList";
        private string weightTable = "RelevanceWeight";
        private string sqlCategories = "SELECT CategoryID, CategoryName, IsActive FROM InterestCategory";
        private string sqlQuestions = "SELECT q.QuestionID, q.CategoryID, c.CategoryName, q.QuestionText, q.DisplayOrder, q.IsActive " +
                                      "FROM InterestQuestion q INNER JOIN InterestCategory c ON c.CategoryID = q.CategoryID";
        private Collection<InterestCategory> categories;
        private Collection<InterestQuestion> questions;
        #endregion

        #region Constructor
        /// <summary>Creates the repository (no database work until a method is called).</summary>
        public InterestRepository() : base()
        {
            categories = new Collection<InterestCategory>();
            questions = new Collection<InterestQuestion>();
        }
        #endregion

        #region Categories
        /// <summary>Categories ordered by name; inactive ones only when includeInactive is true.</summary>
        public Collection<InterestCategory> GetCategories(bool includeInactive)
        {
            FillDataSet(sqlCategories + " WHERE (@IncludeInactive = 1 OR IsActive = 1) ORDER BY CategoryName", categoryTable,
                        Param("@IncludeInactive", SqlDbType.Bit, includeInactive));
            AddCategories2Collection();
            return categories;
        }

        /// <summary>
        /// UI 13.4 grid: CategoryID, CategoryName, QuestionCount (the category's active questions),
        /// MappedProgrammeCount (its DegreeInterestMapping rows), IsActive. A new table every call.
        /// </summary>
        public DataTable GetCategoryTable(bool includeInactive)
        {
            string sql = "SELECT c.CategoryID, c.CategoryName, " +
                         "(SELECT COUNT(*) FROM InterestQuestion q WHERE q.CategoryID = c.CategoryID AND q.IsActive = 1) AS QuestionCount, " +
                         "(SELECT COUNT(*) FROM DegreeInterestMapping m WHERE m.CategoryID = c.CategoryID) AS MappedProgrammeCount, " +
                         "c.IsActive " +
                         "FROM InterestCategory c WHERE (@IncludeInactive = 1 OR c.IsActive = 1) ORDER BY c.CategoryName";
            FillDataSet(sql, categoryListTable, Param("@IncludeInactive", SqlDbType.Bit, includeInactive));
            return GetTable(categoryListTable).Copy();
        }

        /// <summary>One category, or null.</summary>
        public InterestCategory FindCategoryById(int categoryID)
        {
            FillDataSet(sqlCategories + " WHERE CategoryID = @CategoryID", categoryTable, Param("@CategoryID", SqlDbType.Int, categoryID));
            AddCategories2Collection();
            return categories.Count > 0 ? categories[0] : null;
        }

        /// <summary>True if another category already has this name.</summary>
        public bool CategoryNameExists(string categoryName, int? excludeCategoryID)
        {
            object count = ExecuteScalar("SELECT COUNT(*) FROM InterestCategory WHERE CategoryName = @CategoryName AND (@ExcludeID IS NULL OR CategoryID <> @ExcludeID)",
                                         Param("@CategoryName", SqlDbType.VarChar, (categoryName ?? "").Trim()),
                                         Param("@ExcludeID", SqlDbType.Int, excludeCategoryID));
            return Convert.ToInt32(count) > 0;
        }

        /// <summary>Adds a new category row to the DataSet, or changes the existing row (Edit).</summary>
        public void DataSetChange(InterestCategory category, DB.DBOperation operation)
        {
            DataRow row;
            if (operation == DB.DBOperation.Add)
            {
                // An empty fill gives the table its columns; the new row is added to it.
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

        /// <summary>Sends the category change to the database. Returns the CategoryID (the new key for Add).</summary>
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
                // The INSERT returned the new key into the row (FirstReturnedRecord).
                DataTable rows = GetTable(categoryTable);
                category.CategoryID = ReadInt(rows.Rows[rows.Rows.Count - 1], "CategoryID").Value;
            }
            return category.CategoryID;
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
            FillDataSet(sqlQuestions + " WHERE (@CategoryID IS NULL OR q.CategoryID = @CategoryID)" +
                        " AND (@IncludeInactive = 1 OR (q.IsActive = 1 AND c.IsActive = 1))" +
                        " ORDER BY q.DisplayOrder, q.QuestionID", questionTable,
                        Param("@CategoryID", SqlDbType.Int, categoryID),
                        Param("@IncludeInactive", SqlDbType.Bit, includeInactive));
            AddQuestions2Collection();
            return questions;
        }

        /// <summary>One question with CategoryName, or null.</summary>
        public InterestQuestion FindQuestionById(int questionID)
        {
            FillDataSet(sqlQuestions + " WHERE q.QuestionID = @QuestionID", questionTable, Param("@QuestionID", SqlDbType.Int, questionID));
            AddQuestions2Collection();
            return questions.Count > 0 ? questions[0] : null;
        }

        /// <summary>Adds a new question row to the DataSet, or changes the existing row (Edit).</summary>
        public void DataSetChange(InterestQuestion question, DB.DBOperation operation)
        {
            DataRow row;
            if (operation == DB.DBOperation.Add)
            {
                // An empty fill gives the table its columns; the new row is added to it.
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

        /// <summary>Sends the question change to the database. Returns the QuestionID (the new key for Add).</summary>
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
                // The INSERT returned the new key into the row (FirstReturnedRecord).
                DataTable rows = GetTable(questionTable);
                question.QuestionID = ReadInt(rows.Rows[rows.Rows.Count - 1], "QuestionID").Value;
            }
            return question.QuestionID;
        }
        #endregion

        #region Relevance weights
        /// <summary>
        /// UI 13.4 weights grid for a category: ProgrammeID, ProgrammeName, RelevanceWeight,
        /// ordered by programme name. A new table every call.
        /// </summary>
        public DataTable GetRelevanceWeights(int categoryID)
        {
            FillDataSet("SELECT m.ProgrammeID, p.ProgrammeName, m.RelevanceWeight FROM DegreeInterestMapping m " +
                        "INNER JOIN DegreeProgramme p ON p.ProgrammeID = m.ProgrammeID " +
                        "WHERE m.CategoryID = @CategoryID ORDER BY p.ProgrammeName", weightTable,
                        Param("@CategoryID", SqlDbType.Int, categoryID));
            return GetTable(weightTable).Copy();
        }

        /// <summary>Inserts or updates the DegreeInterestMapping row for (programme, category).</summary>
        public void SaveRelevanceWeight(int programmeID, int categoryID, decimal relevanceWeight)
        {
            // Update the mapping if it exists; if no row was updated, insert it.
            ExecuteNonQuery("UPDATE DegreeInterestMapping SET RelevanceWeight = @Weight WHERE ProgrammeID = @ProgrammeID AND CategoryID = @CategoryID; " +
                            "IF @@ROWCOUNT = 0 " +
                            "INSERT INTO DegreeInterestMapping (ProgrammeID, CategoryID, RelevanceWeight) VALUES (@ProgrammeID, @CategoryID, @Weight)",
                            Param("@Weight", SqlDbType.Decimal, relevanceWeight),
                            Param("@ProgrammeID", SqlDbType.Int, programmeID),
                            Param("@CategoryID", SqlDbType.Int, categoryID));
        }
        #endregion

        #region Utility Methods
        /// <summary>Turns the filled InterestCategory rows into InterestCategory objects (Waitering Add2Collection).</summary>
        private void AddCategories2Collection()
        {
            categories = new Collection<InterestCategory>();
            foreach (DataRow row in GetTable(categoryTable).Rows)
            {
                categories.Add(new InterestCategory(ReadInt(row, "CategoryID").Value, ReadString(row, "CategoryName"), ReadBool(row, "IsActive")));
            }
        }

        /// <summary>Turns the filled InterestQuestion rows into InterestQuestion objects (Waitering Add2Collection).</summary>
        private void AddQuestions2Collection()
        {
            questions = new Collection<InterestQuestion>();
            foreach (DataRow row in GetTable(questionTable).Rows)
            {
                InterestQuestion question = new InterestQuestion(ReadInt(row, "QuestionID").Value, ReadInt(row, "CategoryID").Value,
                                                                 ReadString(row, "QuestionText"), ReadInt(row, "DisplayOrder").Value,
                                                                 ReadBool(row, "IsActive"));
                question.CategoryName = ReadString(row, "CategoryName");
                questions.Add(question);
            }
        }

        /// <summary>Copies the category's values into a row (Waitering FillRow). The key is never changed here.</summary>
        private void FillRow(DataRow row, InterestCategory category)
        {
            row["CategoryName"] = category.CategoryName.Trim();
            row["IsActive"] = category.IsActive;
        }

        /// <summary>Copies the question's values into a row (Waitering FillRow). The key is never changed here.</summary>
        private void FillRow(DataRow row, InterestQuestion question)
        {
            row["CategoryID"] = question.CategoryID;
            row["QuestionText"] = question.QuestionText.Trim();
            row["DisplayOrder"] = question.DisplayOrder;
            row["IsActive"] = question.IsActive;
        }
        #endregion

        #region Build Parameters, Create Commands
        /// <summary>The category INSERT parameters, each read from a column of the new row.</summary>
        private void Build_INSERT_Parameters(InterestCategory category)
        {
            daMain.InsertCommand.Parameters.Add(SourceParam("@CategoryName", SqlDbType.VarChar, "CategoryName"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@IsActive", SqlDbType.Bit, "IsActive"));
        }

        /// <summary>The category INSERT command. It returns the new CategoryID into the row.</summary>
        private void Create_INSERT_Command(InterestCategory category)
        {
            daMain.InsertCommand = new SqlCommand("INSERT INTO InterestCategory (CategoryName, IsActive) VALUES (@CategoryName, @IsActive); " +
                                                  "SELECT CAST(SCOPE_IDENTITY() AS INT) AS CategoryID", cnMain);
            daMain.InsertCommand.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;
            Build_INSERT_Parameters(category);
        }

        /// <summary>The category UPDATE parameters, each read from a column of the changed row.</summary>
        private void Build_UPDATE_Parameters(InterestCategory category)
        {
            daMain.UpdateCommand.Parameters.Add(SourceParam("@CategoryName", SqlDbType.VarChar, "CategoryName"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@IsActive", SqlDbType.Bit, "IsActive"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@CategoryID", SqlDbType.Int, "CategoryID"));
        }

        /// <summary>The category UPDATE command.</summary>
        private void Create_UPDATE_Command(InterestCategory category)
        {
            daMain.UpdateCommand = new SqlCommand("UPDATE InterestCategory SET CategoryName = @CategoryName, IsActive = @IsActive WHERE CategoryID = @CategoryID", cnMain);
            Build_UPDATE_Parameters(category);
        }

        /// <summary>The question INSERT parameters, each read from a column of the new row.</summary>
        private void Build_INSERT_Parameters(InterestQuestion question)
        {
            daMain.InsertCommand.Parameters.Add(SourceParam("@CategoryID", SqlDbType.Int, "CategoryID"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@QuestionText", SqlDbType.VarChar, "QuestionText"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@DisplayOrder", SqlDbType.Int, "DisplayOrder"));
            daMain.InsertCommand.Parameters.Add(SourceParam("@IsActive", SqlDbType.Bit, "IsActive"));
        }

        /// <summary>The question INSERT command. It returns the new QuestionID into the row.</summary>
        private void Create_INSERT_Command(InterestQuestion question)
        {
            daMain.InsertCommand = new SqlCommand("INSERT INTO InterestQuestion (CategoryID, QuestionText, DisplayOrder, IsActive) " +
                                                  "VALUES (@CategoryID, @QuestionText, @DisplayOrder, @IsActive); " +
                                                  "SELECT CAST(SCOPE_IDENTITY() AS INT) AS QuestionID", cnMain);
            daMain.InsertCommand.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;
            Build_INSERT_Parameters(question);
        }

        /// <summary>The question UPDATE parameters, each read from a column of the changed row.</summary>
        private void Build_UPDATE_Parameters(InterestQuestion question)
        {
            daMain.UpdateCommand.Parameters.Add(SourceParam("@CategoryID", SqlDbType.Int, "CategoryID"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@QuestionText", SqlDbType.VarChar, "QuestionText"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@DisplayOrder", SqlDbType.Int, "DisplayOrder"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@IsActive", SqlDbType.Bit, "IsActive"));
            daMain.UpdateCommand.Parameters.Add(SourceParam("@QuestionID", SqlDbType.Int, "QuestionID"));
        }

        /// <summary>The question UPDATE command.</summary>
        private void Create_UPDATE_Command(InterestQuestion question)
        {
            daMain.UpdateCommand = new SqlCommand("UPDATE InterestQuestion SET CategoryID = @CategoryID, QuestionText = @QuestionText, " +
                                                  "DisplayOrder = @DisplayOrder, IsActive = @IsActive WHERE QuestionID = @QuestionID", cnMain);
            Build_UPDATE_Parameters(question);
        }
        #endregion
    }
}
