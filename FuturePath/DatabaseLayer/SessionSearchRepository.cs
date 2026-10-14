namespace FuturePath.DatabaseLayer
{
    /// <summary>
    /// Read-only queries for the staff session screens (UI 9 dashboard counts, UI 10.2b session
    /// search). Outcomes are read back from the stored RecommendationResult, never recalculated.
    /// Only SessionSearchController uses this class; its methods are defined with it.
    /// </summary>
    public class SessionSearchRepository : DB
    {
        #region Constructor
        /// <summary>Creates the repository (no database work until a method is called).</summary>
        public SessionSearchRepository() : base()
        {
        }
        #endregion
    }
}
