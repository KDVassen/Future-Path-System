using System;
using System.Collections.ObjectModel;
using FuturePath.ApplicationLayer;

namespace FuturePath.DatabaseLayer
{
    /// <summary>
    /// Data access for degree programmes and their requirements (class diagram
    /// DegreeProgrammeRepository: findByIdWithRequirements, findActiveProgrammes), plus the
    /// Administrator maintenance of programmes (UI 13.1) and requirements (UI 13.2) in the
    /// Waitering DataSetChange / UpdateDataSource pattern.
    /// </summary>
    public class DegreeProgrammeRepository : DB
    {
        #region Data Members
        private string programmeTable = "DegreeProgramme";
        private string requirementTable = "ProgrammeRequirement";
        #endregion

        #region Constructor
        /// <summary>Creates the repository (no database work until a method is called).</summary>
        public DegreeProgrammeRepository() : base()
        {
        }
        #endregion

        #region Read - eligibility engine
        /// <summary>
        /// Class diagram findByIdWithRequirements(programmeId). Loads the programme (with
        /// FacultyName), its ACTIVE requirements (with subject and alternative names, in
        /// RequirementID order) and its interest mappings to ACTIVE categories (with category
        /// names and weights). Returns null if it does not exist (active or not).
        /// </summary>
        public DegreeProgramme FindByIdWithRequirements(int programmeID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Class diagram findActiveProgrammes(excludedProgrammeId). Every active programme under
        /// an active faculty except the excluded one, each loaded as FindByIdWithRequirements does.
        /// </summary>
        public Collection<DegreeProgramme> FindActiveProgrammes(int excludedProgrammeID)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Read - lists
        /// <summary>One programme with FacultyName, without requirements, or null.</summary>
        public DegreeProgramme FindById(int programmeID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Programmes with FacultyName, ordered by name. facultyID null means every faculty.
        /// Without includeInactive, inactive programmes and programmes under an inactive faculty are left out.
        /// </summary>
        public Collection<DegreeProgramme> GetProgrammes(int? facultyID, bool includeInactive)
        {
            throw new NotImplementedException();
        }

        /// <summary>Active programmes under active faculties whose name contains the text (UI 5 search mode).</summary>
        public Collection<DegreeProgramme> SearchActiveProgrammes(string text)
        {
            throw new NotImplementedException();
        }

        /// <summary>True if another programme already has this name.</summary>
        public bool ProgrammeNameExists(string programmeName, int? excludeProgrammeID)
        {
            throw new NotImplementedException();
        }

        /// <summary>A programme's requirements with SubjectName and AlternativeSubjectName, in RequirementID order.</summary>
        public Collection<ProgrammeRequirement> GetRequirements(int programmeID, bool includeInactive)
        {
            throw new NotImplementedException();
        }

        /// <summary>One requirement with its subject names, or null.</summary>
        public ProgrammeRequirement FindRequirementById(int requirementID)
        {
            throw new NotImplementedException();
        }

        /// <summary>True if the programme already has a requirement for this subject (UNIQUE(ProgrammeID, SubjectID)).</summary>
        public bool RequirementExists(int programmeID, int subjectID, int? excludeRequirementID)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Maintenance - Waitering pattern
        /// <summary>Adds a new programme row to the DataSet, or changes the existing row (Edit).</summary>
        public void DataSetChange(DegreeProgramme programme, DB.DBOperation operation)
        {
            throw new NotImplementedException();
        }

        /// <summary>Sends the programme change to the database. Returns the ProgrammeID (the new key for Add).</summary>
        public int UpdateDataSource(DegreeProgramme programme, DB.DBOperation operation)
        {
            throw new NotImplementedException();
        }

        /// <summary>Adds a new requirement row to the DataSet, or changes the existing row (Edit).</summary>
        public void DataSetChange(ProgrammeRequirement requirement, DB.DBOperation operation)
        {
            throw new NotImplementedException();
        }

        /// <summary>Sends the requirement change to the database. Returns the RequirementID (the new key for Add).</summary>
        public int UpdateDataSource(ProgrammeRequirement requirement, DB.DBOperation operation)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
