using System;
using System.Collections.ObjectModel;
using System.Data;
using FuturePath.ApplicationLayer;

namespace FuturePath.DatabaseLayer
{
    /// <summary>
    /// Data access for learners (class diagram LearnerRepository: findById, save).
    /// The in-memory Learner carries its current session's grade, preferred faculty, subjects and
    /// interest responses, so this repository reads Learner together with the AdvisorySession,
    /// LearnerSubject and LearnerInterestResponse rows (classes-and-sequences.md "Mapping the
    /// classes to storage").
    /// </summary>
    public class LearnerRepository : DB
    {
        #region Data Members
        private string table = "Learner";
        #endregion

        #region Constructor
        /// <summary>Creates the repository (no database work until a method is called).</summary>
        public LearnerRepository() : base()
        {
        }
        #endregion

        #region Read
        /// <summary>
        /// Class diagram findById(learnerId). Loads the learner with their latest advisory session
        /// (the In-progress one if there is one, otherwise the most recent): grade, preferred
        /// faculty, CurrentSessionID, subjects (with names and levels) and responses (with
        /// CategoryID). Returns null if the learner does not exist.
        /// </summary>
        public Learner FindById(int learnerID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads the learner of one session, with that session's grade, preferred faculty,
        /// subjects and responses (CurrentSessionID = sessionID). Returns null if the session
        /// does not exist.
        /// </summary>
        public Learner FindBySessionId(int sessionID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// UI 10.1a learner search. Blank text or null means "no filter". Returns columns
        /// LearnerID, LearnerName, GradeDescription (latest session), SchoolName, Province,
        /// SessionCount, LastSessionDate (DateTime or DBNull). Never returns contact details.
        /// </summary>
        public DataTable FindLearners(string surname, string firstName, int? gradeLevelID, string schoolName, string province)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Write
        /// <summary>
        /// Class diagram save(learner). Inserts the Learner row (and sets learner.LearnerID) when
        /// LearnerID is 0, otherwise updates it. Only the Learner table is written; sessions,
        /// subjects and responses have their own methods.
        /// </summary>
        public void Save(Learner learner)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Replaces the session's LearnerSubject rows with these subjects, in one transaction.
        /// Also deletes the session's RecommendationResult rows, because they no longer match the marks.
        /// </summary>
        public void SaveSubjects(int sessionID, Collection<LearnerSubject> subjects)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Replaces the session's LearnerInterestResponse rows with these responses, in one
        /// transaction. Also deletes the session's RecommendationResult rows (stale interest scores).
        /// </summary>
        public void SaveResponses(int sessionID, Collection<LearnerInterestResponse> responses)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
