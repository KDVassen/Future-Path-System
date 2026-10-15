# Future Path — database design (spec Section 5)

_The submitted ERD is `docs/figures/fig-5-erd.png`. It draws 17 entities with keys and the
attributes the advising logic uses. The build adds `UserAccount` and `SystemParameter`
(`decisions.md` D1), giving **19 tables**, plus the columns listed in `decisions.md` §3._

Target: **SQL Server Express LocalDB**, 3NF. `PK` primary key, `FK` foreign key, `*` nullable.
New or changed against the Part 1 notes: **bold**.

## Tables

**Reference data (maintained by Administrators, never deleted — deactivate with `IsActive`)**

- **FACULTY** — FacultyID PK · FacultyName VARCHAR(60) · IsActive BIT
- **DEGREEPROGRAMME** — ProgrammeID PK · FacultyID FK · ProgrammeName VARCHAR(100) · MinimumOverallAverage DECIMAL(5,2)* · IsActive BIT
- **SUBJECT** — SubjectID PK · SubjectName VARCHAR(60) · **SubjectLevel VARCHAR(30)** (Home Language / First Additional Language / Core / Elective) · IsActive BIT
- **PROGRAMMEREQUIREMENT** — RequirementID PK · ProgrammeID FK · SubjectID FK · **RequirementType VARCHAR(20)** (Required / Recommended) · MinimumMark DECIMAL(5,2)* · AlternativeSubjectID FK* · AlternativeMinimumMark DECIMAL(5,2)* · **AdvisoryNote VARCHAR(255)*** · **IsActive BIT** · UNIQUE(ProgrammeID, SubjectID)
- **INTERESTCATEGORY** — CategoryID PK · CategoryName VARCHAR(60) · IsActive BIT
- **INTERESTQUESTION** — QuestionID PK · CategoryID FK · QuestionText VARCHAR(255) · **DisplayOrder INT** · IsActive BIT
- **DEGREEINTERESTMAPPING** — ProgrammeID PK/FK · CategoryID PK/FK · RelevanceWeight DECIMAL(4,2)
- **GRADELEVEL** — GradeLevelID PK · GradeDescription VARCHAR(50) · MarkBasis VARCHAR(30) (Projected for Grade 11, Final for Grade 12)
- **FUNDINGOPTION** — FundingID PK · FundingName VARCHAR(100) · FundingType VARCHAR(40) · EligibilityDescription VARCHAR(500) · RequiredDocuments VARCHAR(255)* · ClosingDate DATE* · ContactInformation VARCHAR(150)* · IsMeritBased BIT · IsNeedBased BIT · IsActive BIT
- **FUNDINGFACULTY** — FundingID PK/FK · FacultyID PK/FK
- **FUNDINGPROGRAMME** — FundingID PK/FK · ProgrammeID PK/FK

**Transaction data (written by an advisory session)**

- **LEARNER** — LearnerID PK · FirstName VARCHAR(50)* · LastName VARCHAR(50)* · **SchoolName VARCHAR(100)** · **Province VARCHAR(50)** · ContactEmail VARCHAR(100)* · ContactNumber VARCHAR(20)*
- **ADVISORYSESSION** — SessionID PK · LearnerID FK · GradeLevelID FK · PreferredFacultyID FK* · SelectedProgrammeID FK* · SessionDate DATETIME · **AdvisingYear SMALLINT** · FundingInfoRequested BIT · **AlternativesInSummary BIT** · SessionStatus VARCHAR(20) ('In progress' / 'Completed') · **DateCompleted DATETIME***
- **LEARNERSUBJECT** — LearnerSubjectID PK · SessionID FK · SubjectID FK · Mark **DECIMAL(5,1)** CHECK 0–100 · UNIQUE(SessionID, SubjectID)
- **LEARNERINTERESTRESPONSE** — ResponseID PK · SessionID FK · QuestionID FK · ResponseValue INT CHECK 1–5 · UNIQUE(SessionID, QuestionID)
- **RECOMMENDATIONRESULT** — ResultID PK · SessionID FK · ProgrammeID FK · SubjectMatchScore* · MarkMatchScore* · InterestMatchScore* DECIMAL(5,2) · **FacultyBonus BIT** · OverallScore DECIMAL(5,2)* · OutcomeCode VARCHAR(10) · OutcomeReason VARCHAR(500) · **AlternativeRank INT*** · IsSelectedProgramme BIT · UNIQUE(SessionID, ProgrammeID)
- **EXPORTREPORT** — ExportID PK · **ReportCode VARCHAR(10)** (FP-LSR / FP-DIE / FP-LER) · SessionID FK* · **ParametersUsed VARCHAR(255)*** · GeneratedByUserID FK* · DateGenerated DATETIME · ExportFormat VARCHAR(10)

**Security and system**

- **USERACCOUNT** — UserID PK · Username VARCHAR(50) UNIQUE · PasswordHash VARCHAR(128) (salted hash, never plaintext) · FullName VARCHAR(100) · Role VARCHAR(30) (Administrator / Career Advisor) · IsActive BIT
- **SYSTEMPARAMETER** — ParameterID PK · ParameterCode VARCHAR(30) UNIQUE · ParameterValue VARCHAR(50) · ParameterDescription VARCHAR(255)*

## Rules the schema encodes

- **Subjects, marks and interest responses belong to the session, not the learner.** The case
  study words the duplicate-subject control as "in one advisory session". A returning Grade 12
  learner has new marks, and an exported summary must reproduce what was captured that day.
- **The outcome lives on `RecommendationResult`.** The selected programme's row has
  `IsSelectedProgramme = 1`, and alternatives get their own rows with `AlternativeRank`. The
  engine always writes a row for the selected programme, even for More Information Needed.
  FP-DIE and FP-LER join through it.
- **`PreferredFacultyID` and `SelectedProgrammeID` are not redundant.** The preferred faculty is
  the learner's stated interest and can differ from the chosen degree's faculty. That difference
  is what `FacultyBonus` measures.
- **Scores are stored on purpose.** Weights and requirements are editable by Administrators, so a
  stored score is a historical record, not a duplicate that could be recalculated.
- **`ExportReport`.** For FP-LSR, `SessionID` is set and `ParametersUsed` is null. For FP-DIE and
  FP-LER, `SessionID` is null and `ParametersUsed` holds the selection, for example
  `Period 2026-01-01..2026-09-15; Faculty=All; Inactive=Excluded`. `GeneratedByUserID` is null
  only for a learner's own FP-LSR export on UI 7. 3NF holds: every non-key column depends only
  on `ExportID`.
- **No School entity.** `SchoolName` is free text, because school names aren't unique across
  provinces and no school register is kept.
- **One accepted alternative subject per requirement.** This covers Mathematics ↔ Mathematical
  Literacy, which is all Part B needs.
- **One interest category per question** (`InterestQuestion.CategoryID`).
- **FP-DIE's subject and mark gaps are calculated at report time** by joining `LearnerSubject`
  against `ProgrammeRequirement`. There is no stored gap table.

## SystemParameter keys

| Code | Value | Used by |
|---|---|---|
| `MIN_SUBJECT_COUNT` | 7 | UI 3 amber notice (a warning, not a block — `decisions.md` §3) |
| `NSC_LEVEL_1` … `NSC_LEVEL_7` | 0, 30, 40, 50, 60, 70, 80 (lower bound, %) | UI 3 achievement level |
| `FUNDING_TYPE_1` … `_4` | Scholarship, Bursary, Financial Aid, External Funding | UI 13.3, UI 6.2b filter |
| `WEIGHT_SUBJECT`, `WEIGHT_MARK`, `WEIGHT_INTEREST`, `WEIGHT_FACULTY`, outcome thresholds | **open — see `decisions.md` §4** | RecommendationEngine |

Load parameters once at startup into a settings helper in the Business Logic Layer. It is not
a domain class.

## Seed data minimums (case study Part B)

At least 10 learners, including the five named ones · 16 subjects · 5 faculties · 10 degree
programmes · at least 5 interest categories and 8 questions · 8 funding options · 10 advisory
sessions · 1 active Administrator and 1 active Career Advisor. The requirement rules are in
`plan-and-tests.md`.

## Where the ERD and class diagram differ on purpose

`FundingFaculty` and `FundingProgramme` are pure link tables. `SystemParameter` is a settings
helper. Controllers and the RecommendationEngine have no table. The class diagram's `Learner`
carries session-scoped data that the database stores on the session. See
`classes-and-sequences.md` → "Mapping the classes to storage".
