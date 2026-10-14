namespace FuturePath.DatabaseLayer
{
    /// <summary>
    /// Raw rows for the three reports (FP-LSR, FP-DIE, FP-LER). This class only reads; every
    /// count, rate, shortfall and chart note is worked out by ReportController, and subject gaps
    /// come from DegreeProgramme.EvaluateSubjectAndMarkMatch, never from SQL.
    /// Only ReportController uses this class; its methods are defined with it.
    /// </summary>
    public class ReportRepository : DB
    {
        #region Constructor
        /// <summary>Creates the repository (no database work until a method is called).</summary>
        public ReportRepository() : base()
        {
        }
        #endregion
    }
}
