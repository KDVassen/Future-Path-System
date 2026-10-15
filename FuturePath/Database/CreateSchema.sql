/* =====================================================================
   Future Path - database schema (SQL Server Express LocalDB)
   Source of truth: docs/design/database.md (19 tables, 3NF).
   Run by Database/BuildDatabase.ps1 against an empty database.

   Conventions
   - Reference data is never deleted: it is deactivated with IsActive = 0.
   - Subjects, marks and interest responses belong to the advisory
     session, not the learner (database.md "Rules the schema encodes").
   - OutcomeCode values: STRONG / POSSIBLE / NCE / MOREINFO
     (ApplicationLayer/AdvisoryOutcome.cs).
   ===================================================================== */

/* ---------- Reference data (maintained by Administrators) ---------- */

CREATE TABLE Faculty (
    FacultyID            INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Faculty PRIMARY KEY,
    FacultyName          VARCHAR(60)  NOT NULL CONSTRAINT UQ_Faculty_Name UNIQUE,
    IsActive             BIT          NOT NULL CONSTRAINT DF_Faculty_IsActive DEFAULT (1)
);

CREATE TABLE DegreeProgramme (
    ProgrammeID          INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_DegreeProgramme PRIMARY KEY,
    FacultyID            INT          NOT NULL CONSTRAINT FK_DegreeProgramme_Faculty REFERENCES Faculty (FacultyID),
    ProgrammeName        VARCHAR(100) NOT NULL CONSTRAINT UQ_DegreeProgramme_Name UNIQUE,
    MinimumOverallAverage DECIMAL(5,2) NULL CONSTRAINT CK_DegreeProgramme_MinAverage CHECK (MinimumOverallAverage BETWEEN 0 AND 100),
    IsActive             BIT          NOT NULL CONSTRAINT DF_DegreeProgramme_IsActive DEFAULT (1)
);

CREATE TABLE Subject (
    SubjectID            INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Subject PRIMARY KEY,
    SubjectName          VARCHAR(60)  NOT NULL CONSTRAINT UQ_Subject_Name UNIQUE,
    SubjectLevel         VARCHAR(30)  NOT NULL CONSTRAINT CK_Subject_Level CHECK (SubjectLevel IN ('Home Language', 'First Additional Language', 'Core', 'Elective')),
    IsActive             BIT          NOT NULL CONSTRAINT DF_Subject_IsActive DEFAULT (1)
);

CREATE TABLE ProgrammeRequirement (
    RequirementID        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ProgrammeRequirement PRIMARY KEY,
    ProgrammeID          INT          NOT NULL CONSTRAINT FK_ProgrammeRequirement_Programme REFERENCES DegreeProgramme (ProgrammeID),
    SubjectID            INT          NOT NULL CONSTRAINT FK_ProgrammeRequirement_Subject REFERENCES Subject (SubjectID),
    RequirementType      VARCHAR(20)  NOT NULL CONSTRAINT CK_ProgrammeRequirement_Type CHECK (RequirementType IN ('Required', 'Recommended')),
    MinimumMark          DECIMAL(5,2) NULL CONSTRAINT CK_ProgrammeRequirement_MinMark CHECK (MinimumMark BETWEEN 0 AND 100),
    AlternativeSubjectID INT          NULL CONSTRAINT FK_ProgrammeRequirement_Alternative REFERENCES Subject (SubjectID),
    AlternativeMinimumMark DECIMAL(5,2) NULL CONSTRAINT CK_ProgrammeRequirement_AltMinMark CHECK (AlternativeMinimumMark BETWEEN 0 AND 100),
    AdvisoryNote         VARCHAR(255) NULL,
    IsActive             BIT          NOT NULL CONSTRAINT DF_ProgrammeRequirement_IsActive DEFAULT (1),
    CONSTRAINT UQ_ProgrammeRequirement_ProgrammeSubject UNIQUE (ProgrammeID, SubjectID),
    CONSTRAINT CK_ProgrammeRequirement_AltNotSelf CHECK (AlternativeSubjectID IS NULL OR AlternativeSubjectID <> SubjectID)
);

CREATE TABLE InterestCategory (
    CategoryID           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_InterestCategory PRIMARY KEY,
    CategoryName         VARCHAR(60)  NOT NULL CONSTRAINT UQ_InterestCategory_Name UNIQUE,
    IsActive             BIT          NOT NULL CONSTRAINT DF_InterestCategory_IsActive DEFAULT (1)
);

CREATE TABLE InterestQuestion (
    QuestionID           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_InterestQuestion PRIMARY KEY,
    CategoryID           INT          NOT NULL CONSTRAINT FK_InterestQuestion_Category REFERENCES InterestCategory (CategoryID),
    QuestionText         VARCHAR(255) NOT NULL,
    DisplayOrder         INT          NOT NULL CONSTRAINT CK_InterestQuestion_DisplayOrder CHECK (DisplayOrder BETWEEN 1 AND 99),
    IsActive             BIT          NOT NULL CONSTRAINT DF_InterestQuestion_IsActive DEFAULT (1)
);

CREATE TABLE DegreeInterestMapping (
    ProgrammeID          INT          NOT NULL CONSTRAINT FK_DegreeInterestMapping_Programme REFERENCES DegreeProgramme (ProgrammeID),
    CategoryID           INT          NOT NULL CONSTRAINT FK_DegreeInterestMapping_Category REFERENCES InterestCategory (CategoryID),
    RelevanceWeight      DECIMAL(4,2) NOT NULL CONSTRAINT CK_DegreeInterestMapping_Weight CHECK (RelevanceWeight BETWEEN 0 AND 1),
    CONSTRAINT PK_DegreeInterestMapping PRIMARY KEY (ProgrammeID, CategoryID)
);

/* GradeLevelID values match the GradeLevel enum: 1 = Grade 11, 2 = Grade 12. */
CREATE TABLE GradeLevel (
    GradeLevelID         INT          NOT NULL CONSTRAINT PK_GradeLevel PRIMARY KEY,
    GradeDescription     VARCHAR(50)  NOT NULL,
    MarkBasis            VARCHAR(30)  NOT NULL
);

CREATE TABLE FundingOption (
    FundingID            INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_FundingOption PRIMARY KEY,
    FundingName          VARCHAR(100) NOT NULL CONSTRAINT UQ_FundingOption_Name UNIQUE,
    FundingType          VARCHAR(40)  NOT NULL,
    EligibilityDescription VARCHAR(500) NOT NULL,
    RequiredDocuments    VARCHAR(255) NULL,
    ClosingDate          DATE         NULL,
    ContactInformation   VARCHAR(150) NULL,
    IsMeritBased         BIT          NOT NULL CONSTRAINT DF_FundingOption_IsMeritBased DEFAULT (0),
    IsNeedBased          BIT          NOT NULL CONSTRAINT DF_FundingOption_IsNeedBased DEFAULT (0),
    IsActive             BIT          NOT NULL CONSTRAINT DF_FundingOption_IsActive DEFAULT (1)
);

CREATE TABLE FundingFaculty (
    FundingID            INT          NOT NULL CONSTRAINT FK_FundingFaculty_Funding REFERENCES FundingOption (FundingID),
    FacultyID            INT          NOT NULL CONSTRAINT FK_FundingFaculty_Faculty REFERENCES Faculty (FacultyID),
    CONSTRAINT PK_FundingFaculty PRIMARY KEY (FundingID, FacultyID)
);

CREATE TABLE FundingProgramme (
    FundingID            INT          NOT NULL CONSTRAINT FK_FundingProgramme_Funding REFERENCES FundingOption (FundingID),
    ProgrammeID          INT          NOT NULL CONSTRAINT FK_FundingProgramme_Programme REFERENCES DegreeProgramme (ProgrammeID),
    CONSTRAINT PK_FundingProgramme PRIMARY KEY (FundingID, ProgrammeID)
);

/* ---------- Transaction data (written by an advisory session) ---------- */

CREATE TABLE Learner (
    LearnerID            INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Learner PRIMARY KEY,
    FirstName            VARCHAR(50)  NULL,
    LastName             VARCHAR(50)  NULL,
    SchoolName           VARCHAR(100) NOT NULL,
    Province             VARCHAR(50)  NOT NULL,
    ContactEmail         VARCHAR(100) NULL,
    ContactNumber        VARCHAR(20)  NULL
);

CREATE TABLE AdvisorySession (
    SessionID            INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_AdvisorySession PRIMARY KEY,
    LearnerID            INT          NOT NULL CONSTRAINT FK_AdvisorySession_Learner REFERENCES Learner (LearnerID),
    GradeLevelID         INT          NOT NULL CONSTRAINT FK_AdvisorySession_GradeLevel REFERENCES GradeLevel (GradeLevelID),
    PreferredFacultyID   INT          NULL CONSTRAINT FK_AdvisorySession_Faculty REFERENCES Faculty (FacultyID),
    SelectedProgrammeID  INT          NULL CONSTRAINT FK_AdvisorySession_Programme REFERENCES DegreeProgramme (ProgrammeID),
    SessionDate          DATETIME     NOT NULL CONSTRAINT DF_AdvisorySession_SessionDate DEFAULT (GETDATE()),
    AdvisingYear         SMALLINT     NOT NULL,
    FundingInfoRequested BIT          NOT NULL CONSTRAINT DF_AdvisorySession_Funding DEFAULT (0),
    AlternativesInSummary BIT         NOT NULL CONSTRAINT DF_AdvisorySession_Alternatives DEFAULT (0),
    SessionStatus        VARCHAR(20)  NOT NULL CONSTRAINT DF_AdvisorySession_Status DEFAULT ('In progress')
                                      CONSTRAINT CK_AdvisorySession_Status CHECK (SessionStatus IN ('In progress', 'Completed')),
    DateCompleted        DATETIME     NULL
);

CREATE TABLE LearnerSubject (
    LearnerSubjectID     INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_LearnerSubject PRIMARY KEY,
    SessionID            INT          NOT NULL CONSTRAINT FK_LearnerSubject_Session REFERENCES AdvisorySession (SessionID),
    SubjectID            INT          NOT NULL CONSTRAINT FK_LearnerSubject_Subject REFERENCES Subject (SubjectID),
    Mark                 DECIMAL(5,1) NOT NULL CONSTRAINT CK_LearnerSubject_Mark CHECK (Mark BETWEEN 0 AND 100),
    CONSTRAINT UQ_LearnerSubject_SessionSubject UNIQUE (SessionID, SubjectID)
);

CREATE TABLE LearnerInterestResponse (
    ResponseID           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_LearnerInterestResponse PRIMARY KEY,
    SessionID            INT          NOT NULL CONSTRAINT FK_LearnerInterestResponse_Session REFERENCES AdvisorySession (SessionID),
    QuestionID           INT          NOT NULL CONSTRAINT FK_LearnerInterestResponse_Question REFERENCES InterestQuestion (QuestionID),
    ResponseValue        INT          NOT NULL CONSTRAINT CK_LearnerInterestResponse_Value CHECK (ResponseValue BETWEEN 1 AND 5),
    CONSTRAINT UQ_LearnerInterestResponse_SessionQuestion UNIQUE (SessionID, QuestionID)
);

CREATE TABLE RecommendationResult (
    ResultID             INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_RecommendationResult PRIMARY KEY,
    SessionID            INT          NOT NULL CONSTRAINT FK_RecommendationResult_Session REFERENCES AdvisorySession (SessionID),
    ProgrammeID          INT          NOT NULL CONSTRAINT FK_RecommendationResult_Programme REFERENCES DegreeProgramme (ProgrammeID),
    SubjectMatchScore    DECIMAL(5,2) NULL,
    MarkMatchScore       DECIMAL(5,2) NULL,
    InterestMatchScore   DECIMAL(5,2) NULL,
    FacultyBonus         BIT          NOT NULL CONSTRAINT DF_RecommendationResult_FacultyBonus DEFAULT (0),
    OverallScore         DECIMAL(5,2) NULL,
    OutcomeCode          VARCHAR(10)  NOT NULL CONSTRAINT CK_RecommendationResult_Outcome CHECK (OutcomeCode IN ('STRONG', 'POSSIBLE', 'NCE', 'MOREINFO')),
    OutcomeReason        VARCHAR(500) NOT NULL,
    AlternativeRank      INT          NULL,
    IsSelectedProgramme  BIT          NOT NULL CONSTRAINT DF_RecommendationResult_IsSelected DEFAULT (0),
    CONSTRAINT UQ_RecommendationResult_SessionProgramme UNIQUE (SessionID, ProgrammeID)
);

/* ---------- Security and system ---------- */

/* PasswordHash format: "saltBase64:hashBase64" (salted SHA-256, UserAccount.HashPassword). */
CREATE TABLE UserAccount (
    UserID               INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_UserAccount PRIMARY KEY,
    Username             VARCHAR(50)  NOT NULL CONSTRAINT UQ_UserAccount_Username UNIQUE,
    PasswordHash         VARCHAR(128) NOT NULL,
    FullName             VARCHAR(100) NOT NULL,
    Role                 VARCHAR(30)  NOT NULL CONSTRAINT CK_UserAccount_Role CHECK (Role IN ('Administrator', 'Career Advisor')),
    IsActive             BIT          NOT NULL CONSTRAINT DF_UserAccount_IsActive DEFAULT (1)
);

CREATE TABLE ExportReport (
    ExportID             INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ExportReport PRIMARY KEY,
    ReportCode           VARCHAR(10)  NOT NULL CONSTRAINT CK_ExportReport_Code CHECK (ReportCode IN ('FP-LSR', 'FP-DIE', 'FP-LER')),
    SessionID            INT          NULL CONSTRAINT FK_ExportReport_Session REFERENCES AdvisorySession (SessionID),
    ParametersUsed       VARCHAR(255) NULL,
    GeneratedByUserID    INT          NULL CONSTRAINT FK_ExportReport_User REFERENCES UserAccount (UserID),
    DateGenerated        DATETIME     NOT NULL CONSTRAINT DF_ExportReport_DateGenerated DEFAULT (GETDATE()),
    ExportFormat         VARCHAR(10)  NOT NULL CONSTRAINT CK_ExportReport_Format CHECK (ExportFormat IN ('PDF', 'DOCX', 'XLSX'))
);

CREATE TABLE SystemParameter (
    ParameterID          INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_SystemParameter PRIMARY KEY,
    ParameterCode        VARCHAR(30)  NOT NULL CONSTRAINT UQ_SystemParameter_Code UNIQUE,
    ParameterValue       VARCHAR(50)  NOT NULL,
    ParameterDescription VARCHAR(255) NULL
);
