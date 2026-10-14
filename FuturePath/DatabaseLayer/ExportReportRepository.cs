using System;
using FuturePath.ApplicationLayer;

namespace FuturePath.DatabaseLayer
{
    /// <summary>
    /// Data access for the ExportReport log. Insert is the one place an ExportReport row is
    /// written, both for a learner's own FP-LSR (UI 7) and for staff exports (decisions.md D2).
    /// </summary>
    public class ExportReportRepository : DB
    {
        #region Data Members
        private string table = "ExportReport";
        #endregion

        #region Constructor
        /// <summary>Creates the repository (no database work until a method is called).</summary>
        public ExportReportRepository() : base()
        {
        }
        #endregion

        #region Methods
        /// <summary>Writes one ExportReport row and returns its ExportID.</summary>
        public int Insert(ExportReport report)
        {
            throw new NotImplementedException();
        }

        /// <summary>When the report was last generated, or null if never.</summary>
        public DateTime? GetLastGenerated(string reportCode)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
