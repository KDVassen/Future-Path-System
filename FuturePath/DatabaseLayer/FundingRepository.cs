using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using FuturePath.ApplicationLayer;

namespace FuturePath.DatabaseLayer
{
    /// <summary>
    /// Data access for funding options and their faculty / programme links (UI 6.2b, UI 13.3,
    /// FP-LSR Table 3). An option and its links are saved together in one transaction (a set of
    /// link rows is replaced with direct commands, application-layer.md §2 "Replace a set").
    /// </summary>
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
        /// <summary>Creates the repository (no database work until a method is called).</summary>
        public FundingRepository() : base()
        {
            fundingOptions = new Collection<FundingOption>();
        }
        #endregion

        #region Read
        /// <summary>Funding options with their links (and link names), ordered by FundingName; inactive ones only when includeInactive is true.</summary>
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

        /// <summary>One funding option with its links, or null.</summary>
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

        /// <summary>
        /// Active funding linked to the faculty OR the programme, with links, ordered by ClosingDate
        /// (nearest first, no closing date last). 0 means "not given"; with both 0, every active
        /// funding option. Links to inactive faculties or programmes are not loaded, so they do not count.
        /// </summary>
        public Collection<FundingOption> GetActiveFundingFor(int facultyID, int programmeID)
        {
            // 1. Every active option, nearest closing date first (no closing date sorts as the last date)
            FillDataSet(sqlLocal + " WHERE IsActive = 1 ORDER BY ISNULL(ClosingDate, '9999-12-31'), FundingName", table);
            Add2Collection();

            // 2. Their links to active faculties and active programmes
            LoadLinks(0, true);

            // 3. Keep only the options linked to the faculty or to the programme
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

        /// <summary>True if another funding option already has this name.</summary>
        public bool FundingNameExists(string fundingName, int excludeFundingID)
        {
            object count = ExecuteScalar("SELECT COUNT(*) FROM FundingOption WHERE FundingName = @FundingName AND FundingID <> @ExcludeID",
                                         Param("@FundingName", SqlDbType.VarChar, TextOrEmpty(fundingName)),
                                         Param("@ExcludeID", SqlDbType.Int, excludeFundingID));
            return Convert.ToInt32(count) > 0;
        }
        #endregion

        #region Write
        /// <summary>
        /// Inserts (FundingID 0) or updates the option and replaces its FundingFaculty and
        /// FundingProgramme rows with option.FacultyLinks / ProgrammeLinks, in one transaction.
        /// Sets option.FundingID and returns it.
        /// </summary>
        public int Save(FundingOption option)
        {
            int fundingID = option.FundingID;
            SqlTransaction transaction = BeginTransaction();
            try
            {
                // 1. The option itself: insert (and read the new key) or update.
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

                // 2. Remove the old links.
                ExecuteNonQuery(transaction, "DELETE FROM FundingFaculty WHERE FundingID = @FundingID", Param("@FundingID", SqlDbType.Int, fundingID));
                ExecuteNonQuery(transaction, "DELETE FROM FundingProgramme WHERE FundingID = @FundingID", Param("@FundingID", SqlDbType.Int, fundingID));

                // 3. Insert the new links (a new parameter object for every command).
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

            // Only now that the save is committed does the option (and its links) get the key.
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

        /// <summary>Sets IsActive (deactivation; funding is never deleted).</summary>
        public void SetActive(int fundingID, bool isActive)
        {
            ExecuteNonQuery("UPDATE FundingOption SET IsActive = @IsActive WHERE FundingID = @FundingID",
                            Param("@IsActive", SqlDbType.Bit, isActive), Param("@FundingID", SqlDbType.Int, fundingID));
        }
        #endregion

        #region Utility Methods
        /// <summary>Turns the filled FundingOption rows into FundingOption objects (Waitering Add2Collection), without links.</summary>
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

        /// <summary>
        /// Reads the faculty and programme links (with their names) and attaches each one to its
        /// option in fundingOptions. A fundingID of 0 reads the links of every option. With
        /// activeOnly, links to inactive faculties, inactive programmes and programmes under an
        /// inactive faculty are left out.
        /// </summary>
        private void LoadLinks(int fundingID, bool activeOnly)
        {
            // The faculty links
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

            // The programme links (a new parameter list: one parameter object cannot be used by two commands)
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

        /// <summary>The option with this ID in fundingOptions, or null (a link to an option that was not read is skipped).</summary>
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

        /// <summary>
        /// The parameters of the FundingOption INSERT / UPDATE, made new for every command.
        /// Blank optional text is stored as NULL.
        /// </summary>
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
