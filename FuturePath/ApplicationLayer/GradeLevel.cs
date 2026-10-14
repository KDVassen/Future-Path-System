namespace FuturePath.ApplicationLayer
{
    /// <summary>
    /// The learner's school grade (class diagram «enumeration» GradeLevel: GRADE_11, GRADE_12).
    /// The numbers match GradeLevel.GradeLevelID in the database, so the enum and the table
    /// always agree. Grade 11 marks are treated as projected, Grade 12 marks as final.
    /// </summary>
    public enum GradeLevel
    {
        /// <summary>Grade 11 (marks are projected results). GradeLevelID 1.</summary>
        Grade11 = 1,
        /// <summary>Grade 12 (marks are final results). GradeLevelID 2.</summary>
        Grade12 = 2
    }
}
