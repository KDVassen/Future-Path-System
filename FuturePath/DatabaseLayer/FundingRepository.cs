
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using FuturePath.ApplicationLayer;

namespace FuturePath.DatabaseLayer
{
    // Handles database operations for funding options
    // Manages funding details and their faculty and programme links
    public class FundingRepository : DB
    {
        #region Data Members
        private string table = "FundingOption";
        private string facultyLinkTable = "FundingFaculty";
        private string programmeLinkTable = "FundingProgramme";
        private string sqlLocal = "SELECT FundingID, FundingName, FundingType, EligibilityDescription, RequiredDocuments, ClosingDate, " +
                                  "ContactInformation, IsMeritBased, IsNeedBased, IsActive FROM FundingOption";
        private Collection<FundingOption> fundingOptions;
        #endregion

        #region Constructor
        // Initialise the funding options collection
        public FundingRepository() : base()
        {
            fundingOptions = new Collection<FundingOption>();
        }
        #endregion

        #region Read
        // Load funding options with their faculty and programme links
        // Include inactive options when requested
        public Collection<FundingOption> GetFundingOptions(bool includeInactive)
        {
            string sql = sqlLocal;
            if (!includeInactive)
            {
                sql += " WHERE IsActive = 1";
            }
            FillDataSet(sql + " ORDER BY FundingName", table);
            Add2Collection();
            LoadLinks(0, false);
            return fundingOptions;
        }

        // Find a funding option by ID and load its links
        public FundingOption FindById(int fundingID)
        {
            FillDataSet(sqlLocal + " WHERE FundingID = @FundingID", table, Param("@FundingID", SqlDbType.Int, fundingID));
            Add2Collection();
            if (fundingOptions.Count == 0)
            {
                return null;
            }
            LoadLinks(fundingID, false);
            return fundingOptions[0];
        }

        // Get active funding linked to the selected faculty or programme
        // Order by closing date and exclude links to inactive records
        public Collection<FundingOption> GetActiveFundingFor(int facultyID, int programmeID)
        {
            // Load active options with the nearest closing dates first
            FillDataSet(sqlLocal + " WHERE IsActive = 1 ORDER BY ISNULL(ClosingDate, '9999-12-31'), FundingName", table);
            Add2Collection();

            // Load links to active faculties and programmes
            LoadLinks(0, true);

            // Keep funding linked to the requested faculty or programme
            Collection<FundingOption> linked = new Collection<FundingOption>();
            foreach (FundingOption option in fundingOptions)
            {
                if (option.LinksTo(facultyID, programmeID))
                {
                    linked.Add(option);
                }
            }
            fundingOptions = linked;
            return fundingOptions;
        }

        // Check whether another funding option has the same name
        public bool FundingNameExists(string fundingName, int excludeFundingID)
        {
            object count = ExecuteScalar("SELECT COUNT(*) FROM FundingOption WHERE FundingName = @FundingName AND FundingID <> @ExcludeID",
                                         Param("@FundingName", SqlDbType.VarChar, TextOrEmpty(fundingName)),
                                         Param("@ExcludeID", SqlDbType.Int, excludeFundingID));
            return Convert.ToInt32(count) > 0;
        }
        #endregion

        #region Write
        // Add or update a funding option and replace its links
        // Save everything in one transaction to keep the records consistent
        public int Save(FundingOption option)
        {
            int fundingID = option.FundingID;
            SqlTransaction transaction = BeginTransaction();
            try
            {
                // Insert a new funding option or update an existing one
                if (fundingID == 0)
                {
                    object newID = ExecuteScalar(transaction,
                        "INSERT INTO FundingOption (FundingName, FundingType, EligibilityDescription, RequiredDocuments, ClosingDate, " +
                        "ContactInformation, IsMeritBased, IsNeedBased, IsActive) VALUES (@FundingName, @FundingType, @EligibilityDescription, " +
                        "@RequiredDocuments, @ClosingDate, @ContactInformation, @IsMeritBased, @IsNeedBased, @IsActive); " +
                        "SELECT CAST(SCOPE_IDENTITY() AS INT)",
                        OptionParameters(option, fundingID));
                    fundingID = Convert.ToInt32(newID);
                }
                else
                {
                    ExecuteNonQuery(transaction,
                        "UPDATE FundingOption SET FundingName = @FundingName, FundingType = @FundingType, EligibilityDescription = @EligibilityDescription, " +
                        "RequiredDocuments = @RequiredDocuments, ClosingDate = @ClosingDate, ContactInformation = @ContactInformation, " +
                        "IsMeritBased = @IsMeritBased, IsNeedBased = @IsNeedBased, IsActive = @IsActive WHERE FundingID = @FundingID",
                        OptionParameters(option, fundingID));
                }

                // Remove previous faculty and programme links
                ExecuteNonQuery(transaction, "DELETE FROM FundingFaculty WHERE FundingID = @FundingID", Param("@FundingID", SqlDbType.Int, fundingID));
                ExecuteNonQuery(transaction, "DELETE FROM FundingProgramme WHERE FundingID = @FundingID", Param("@FundingID", SqlDbType.Int, fundingID));

                // Insert the updated faculty links
                foreach (FundingFaculty link in option.FacultyLinks)
                {
                    ExecuteNonQuery(transaction, "INSERT INTO FundingFaculty (FundingID, FacultyID) VALUES (@FundingID, @FacultyID)",
                                    Param("@FundingID", SqlDbType.Int, fundingID), Param("@FacultyID", SqlDbType.Int, link.FacultyID));
                }
                foreach (FundingProgramme link in option.ProgrammeLinks)
                {
                    ExecuteNonQuery(transaction, "INSERT INTO FundingProgramme (FundingID, ProgrammeID) VALUES (@FundingID, @ProgrammeID)",
                                    Param("@FundingID", SqlDbType.Int, fundingID), Param("@ProgrammeID", SqlDbType.Int, link.ProgrammeID));
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                CloseConnection();
            }

            // Update the funding IDs after the transaction has succeeded
            option.FundingID = fundingID;
            foreach (FundingFaculty link in option.FacultyLinks)
            {
                link.FundingID = fundingID;
            }
            foreach (FundingProgramme link in option.ProgrammeLinks)
            {
                link.FundingID = fundingID;
            }
            return fundingID;
        }

        // Activate or deactivate a funding option without deleting it
        public void SetActive(int fundingID, bool isActive)
        {
            ExecuteNonQuery("UPDATE FundingOption SET IsActive = @IsActive WHERE FundingID = @FundingID",
                            Param("@IsActive", SqlDbType.Bit, isActive), Param("@FundingID", SqlDbType.Int, fundingID));
        }
        #endregion

        #region Utility Methods
        // Convert database rows into FundingOption objects
        private void Add2Collection()
        {
            fundingOptions = new Collection<FundingOption>();
            foreach (DataRow row in GetTable(table).Rows)
            {
                fundingOptions.Add(new FundingOption(ReadInt(row, "FundingID"), ReadString(row, "FundingName"), ReadString(row, "FundingType"),
                                                     ReadString(row, "EligibilityDescription"), ReadString(row, "RequiredDocuments"),
                                                     ReadDate(row, "ClosingDate"), ReadString(row, "ContactInformation"),
                                                     ReadBool(row, "IsMeritBased"), ReadBool(row, "IsNeedBased"), ReadBool(row, "IsActive")));
            }
        }

        // Load the faculty and programme links for funding options
        // Optionally exclude links to inactive faculties and programmes
        private void LoadLinks(int fundingID, bool activeOnly)
        {
            // Load faculty links and their names
            string sql = "SELECT ff.FundingID, ff.FacultyID, f.FacultyName FROM FundingFaculty ff " +
                         "INNER JOIN Faculty f ON f.FacultyID = ff.FacultyID WHERE 1 = 1";
            List<SqlParameter> parameters = new List<SqlParameter>();
            if (fundingID > 0)
            {
                sql += " AND ff.FundingID = @FundingID";
                parameters.Add(Param("@FundingID", SqlDbType.Int, fundingID));
            }
            if (activeOnly)
            {
                sql += " AND f.IsActive = 1";
            }
            FillDataSet(sql + " ORDER BY f.FacultyName", facultyLinkTable, parameters.ToArray());
            foreach (DataRow row in GetTable(facultyLinkTable).Rows)
            {
                FundingOption option = FindOption(ReadInt(row, "FundingID"));
                if (option != null)
                {
                    option.FacultyLinks.Add(new FundingFaculty(option.FundingID, ReadInt(row, "FacultyID"), ReadString(row, "FacultyName")));
                }
            }

            // Load programme links using a new list of SQL parameters
            sql = "SELECT fp.FundingID, fp.ProgrammeID, p.ProgrammeName FROM FundingProgramme fp " +
                  "INNER JOIN DegreeProgramme p ON p.ProgrammeID = fp.ProgrammeID " +
                  "INNER JOIN Faculty pf ON pf.FacultyID = p.FacultyID WHERE 1 = 1";
            parameters = new List<SqlParameter>();
            if (fundingID > 0)
            {
                sql += " AND fp.FundingID = @FundingID";
                parameters.Add(Param("@FundingID", SqlDbType.Int, fundingID));
            }
            if (activeOnly)
            {
                sql += " AND p.IsActive = 1 AND pf.IsActive = 1";
            }
            FillDataSet(sql + " ORDER BY p.ProgrammeName", programmeLinkTable, parameters.ToArray());
            foreach (DataRow row in GetTable(programmeLinkTable).Rows)
            {
                FundingOption option = FindOption(ReadInt(row, "FundingID"));
                if (option != null)
                {
                    option.ProgrammeLinks.Add(new FundingProgramme(option.FundingID, ReadInt(row, "ProgrammeID"), ReadString(row, "ProgrammeName")));
                }
            }
        }

        // Find a funding option in the loaded collection by its ID
        private FundingOption FindOption(int fundingID)
        {
            foreach (FundingOption option in fundingOptions)
            {
                if (option.FundingID == fundingID)
                {
                    return option;
                }
            }
            return null;
        }

        // Create SQL parameters for inserting or updating a funding option
        // Store optional blank text as database NULL values
        private static SqlParameter[] OptionParameters(FundingOption option, int fundingID)
        {
            return new SqlParameter[]
            {
                Param("@FundingName", SqlDbType.VarChar, option.FundingName.Trim()),
                Param("@FundingType", SqlDbType.VarChar, option.FundingType.Trim()),
                Param("@EligibilityDescription", SqlDbType.VarChar, option.EligibilityDescription.Trim()),
                Param("@RequiredDocuments", SqlDbType.VarChar, DbText(option.RequiredDocuments)),
                Param("@ClosingDate", SqlDbType.Date, option.ClosingDate),
                Param("@ContactInformation", SqlDbType.VarChar, DbText(option.ContactInformation)),
                Param("@IsMeritBased", SqlDbType.Bit, option.IsMeritBased),
                Param("@IsNeedBased", SqlDbType.Bit, option.IsNeedBased),
                Param("@IsActive", SqlDbType.Bit, option.IsActive),
                Param("@FundingID", SqlDbType.Int, fundingID)
            };
        }
        #endregion
    }
}
