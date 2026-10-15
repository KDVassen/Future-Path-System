# Future Path — classes and sequence diagrams (spec Sections 3 and 4)

A transcription of the submitted design class diagram and the two design sequence diagrams, so
code can be written against them. Figures are in `docs/figures/` (`fig-4-class-diagram.png`,
`fig-3-1-sequence-eligibility.png`, `fig-3-2-sequence-alternatives.png`). Where the class
diagram and the ERD differ, see `decisions.md` §3.

## Architecture (spec §1 package diagram)

| Layer | Packages | Project folder |
|---|---|---|
| Presentation | Learner-facing forms, advisor console | `FuturePath/PresentationLayer` |
| Business Logic | Eligibility engine, Report generation, Authorization & access | `FuturePath/ApplicationLayer` |
| Data | Local relational database (SQL Server Express LocalDB) | `FuturePath/DatabaseLayer` |

Dependencies point downward only. The eligibility and scoring engine is the critical-path
component, so it is built and tested first, against the domain classes and Part B data.

## Classes in the submitted diagram

### Boundary (Presentation)
- **EligibilityCheckView** (UI 5/6) — `+displayOutcome(outcome): void`
- **AlternativeProgrammesView** (UI 6.1a) — `+displayOutcome(outcome): void`,
  `+displayAlternatives(rankedAlternatives): void`

### Controller (Business Logic)
- **EligibilityController** —
  `+checkEligibility(learnerId, programmeId): AdvisoryOutcome`.
  Uses LearnerRepository, DegreeProgrammeRepository, AdvisorySessionRepository and
  RecommendationEngine.
- **RecommendationController** —
  `+generateAlternatives(learnerId, excludedProgrammeId): RecommendationResult[]`.
  Uses the same four.
- **RecommendationEngine** (no stereotype) —
  `+computeOutcome(matchResult, interestScore, facultyBonus): AdvisoryOutcome`,
  `+rankAlternatives(candidateOutcomes): RecommendationResult[]`

### Result classes (transient, not stored)
- **MatchResult** — `subjectScore: double`, `markScore: double`, `gapList: string[]`
- **AdvisoryOutcome** — `category: string`, `explanation: string`, `overallScore: double`

### Data access
- **LearnerRepository** — `+findById(learnerId): Learner`, `+save(learner): void`
- **DegreeProgrammeRepository** —
  `+findByIdWithRequirements(programmeId): DegreeProgramme`,
  `+findActiveProgrammes(excludedProgrammeId): DegreeProgramme[]`
- **AdvisorySessionRepository** — `+save(session): AdvisorySession`,
  `+save(result): RecommendationResult`, `+saveAll(results): boolean`

### Entity
| Class | Attributes | Methods |
|---|---|---|
| Learner | learnerId, name, grade: GradeLevel, school, province, contactDetails, preferredFaculty: int | `Learner(name, grade, school, province, contactDetails)`, `addSubject(subject, mark)`, `getSubjects(): LearnerSubject[]`, `getPreferredFacultyId(): int`, `calculateInterestScore(categories): double` |
| LearnerSubject | learnerSubjectId, mark: int | `validateMarkRange(): boolean` |
| LearnerInterestResponse | responseId, responseValue: int | — |
| AdvisorySession | sessionId, dateCompleted: date, outcome: string | `recordOutcome(outcome): void` |
| RecommendationResult | resultId, subjectScore, markScore, interestScore, facultyBonus: boolean, overallScore, category, explanation, rank: int | — |
| DegreeProgramme | programmeId, programmeName, isActive | `evaluateSubjectAndMarkMatch(learnerSubjects): MatchResult`, `getInterestCategories(): InterestCategory[]`, `matchesPreferredFaculty(facultyId): boolean`, `getRequirements(): ProgrammeRequirement[]` |
| ProgrammeRequirement | requirementId, minimumMark: int, advisoryNote | `isSatisfiedBy(learnerSubjects): boolean` |
| Subject | subjectId, subjectName | — |
| Faculty | facultyId, facultyName | — |
| InterestCategory | categoryId, categoryName | — |
| InterestQuestion | questionId, questionText | — |
| DegreeInterestMapping | mappingId, weight: double | — |
| FundingOption | fundingId, fundingName, fundingType, eligibilityDescription, requiredDocuments, closingDate, contactInfo, isActive | — |
| FundingFaculty / FundingProgramme | fundingId + facultyId / programmeId | — |
| ExportReport | reportId, dateGenerated, format | — |
| UserAccount | userId, username, passwordHash, role | `authenticate(username, password): boolean` |
| GradeLevel «enumeration» | GRADE_11, GRADE_12 | — |

**Associations.** Faculty 1 — offers — 0..* DegreeProgramme · DegreeProgramme 1 ◆ has 0..*
ProgrammeRequirement · ProgrammeRequirement 0..* requires 0..1 Subject and 0..* accepts
alternative 0..1 Subject · Learner 1 ◆ captures 0..* LearnerSubject, each for 1 Subject ·
Learner 1 answers 0..* LearnerInterestResponse, each responding to 1 InterestQuestion ·
InterestCategory 1 groups 0..* InterestQuestion · DegreeInterestMapping 0..* maps 1
InterestCategory and 1 DegreeProgramme · Learner 1 completes 0..* AdvisorySession ·
AdvisorySession 1 ◆ produces 0..* RecommendationResult, and 0..* evaluates 1 DegreeProgramme ·
RecommendationResult 0..* recommends 1 DegreeProgramme · AdvisorySession 1 exports 0..*
ExportReport · FundingFaculty and FundingProgramme link FundingOption to Faculty and
DegreeProgramme · Learner 0..* classifies as 1 GradeLevel · UserAccount has no associations
(learners never log in).

## Mapping the classes to storage

- The in-memory `Learner` holds the **current session's** grade, preferred faculty, subjects and
  responses. `LearnerRepository.findById` hydrates it from `Learner` plus the in-progress
  `AdvisorySession`, `LearnerSubject` and `LearnerInterestResponse` rows.
- `AdvisorySession.outcome` = `OutcomeCode` of the `RecommendationResult` row where
  `IsSelectedProgramme = 1`. `dateCompleted` is set when UI 7 completes the session.
- `RecommendationResult.category` ↔ `OutcomeCode`, `explanation` ↔ `OutcomeReason`,
  `rank` ↔ `AlternativeRank`.
- `GradeLevel` enum ↔ the `GradeLevel` table (IDs seeded to match).
- `DegreeInterestMapping.mappingId` is the class's own identity. The table key is
  (ProgrammeID, CategoryID).
- `FundingFaculty` / `FundingProgramme` may be loaded as collections on `FundingOption`.

## Sequence 1 — Check degree eligibility and generate an advisory outcome

Trigger: the learner clicks **Check eligibility** on UI 5.

1. `EligibilityCheckView → EligibilityController.checkEligibility(learnerId, programmeId)`
2. `LearnerRepository.findById(learnerId)` → Learner
3. `DegreeProgrammeRepository.findByIdWithRequirements(programmeId)` → DegreeProgramme
4. **alt [required learner or programme information missing]** →
   `displayOutcome("More information needed")`
5. **else:** `learner.getSubjects()` → LearnerSubject[]; `learner.getPreferredFacultyId()` → int
6. `programme.evaluateSubjectAndMarkMatch(learnerSubjects)` → MatchResult
7. `programme.getInterestCategories()` → InterestCategory[]
8. `learner.calculateInterestScore(categories)` → double
9. `programme.matchesPreferredFaculty(facultyId)` → facultyBonus: boolean
10. `RecommendationEngine.computeOutcome(matchResult, interestScore, facultyBonus)` → AdvisoryOutcome
11. `AdvisorySessionRepository.save(newAdvisorySession)`, then `save(newRecommendationResult)`
    (the selected programme's row, `IsSelectedProgramme = 1`)
12. `displayOutcome(outcome)`. The view shows the outcome **with its explanation**.

## Sequence 2 — Generate alternative degree recommendations

Trigger: the learner clicks **Alternative programmes** on UI 6.

1. `AlternativeProgrammesView → RecommendationController.generateAlternatives(learnerId, excludedProgrammeId)`
2. `LearnerRepository.findById(learnerId)`
3. **alt [learner has no captured subjects or interests]** → `displayOutcome("More information needed")`
4. **else:** `getSubjects()`, `getPreferredFacultyId()`,
   `DegreeProgrammeRepository.findActiveProgrammes(excludedProgrammeId)` → candidates (active only)
5. **loop for each candidate:** `evaluateSubjectAndMarkMatch` → `getInterestCategories` →
   `calculateInterestScore` → `matchesPreferredFaculty` → `computeOutcome` → candidateOutcome
6. `RecommendationEngine.rankAlternatives(candidateOutcomes)` → RecommendationResult[]
7. `AdvisorySessionRepository.saveAll(rankedAlternatives)` → confirmation
8. `displayAlternatives(rankedAlternatives)`. Each row states its reason, not just its rank and score.

## Outcome definitions (case study §6.5)

| Outcome | Meaning |
|---|---|
| Strong Match | Appears to meet the minimum requirements and has strong interest alignment. |
| Possible Match | Meets some requirements but has one or more gaps or weaker alignment. |
| Not Currently Eligible | Does not currently meet one or more important requirements. |
| More Information Needed | The check cannot be completed because required information is missing. |

The outcome is **rule-driven first, then score-driven**. A missing required subject with no
accepted alternative can never produce a match, whatever the score. Weights and thresholds are
still open (`decisions.md` §4).
