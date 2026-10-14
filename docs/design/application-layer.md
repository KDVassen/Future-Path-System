# Future Path: Application and Data Layer contract

How the Business Logic Layer (`FuturePath/ApplicationLayer`) and Data Layer
(`FuturePath/DatabaseLayer`) are built, and what every class promises. Read this before you
change a controller, repository, entity or the database. The XML doc comment on each method is
the detailed contract. This page gives the rules that apply to all of them.

Sources: the spec class diagram (`classes-and-sequences.md`), the 19 tables (`database.md`),
the forms' `// TODO BLL:` calls, and the **Waitering System** coding pattern.

## 1. Layers and who calls whom

```
Form code-behind  ──►  XController (ApplicationLayer)  ──►  XRepository : DB (DatabaseLayer)  ──►  FuturePath.mdf
                         │  entity rule methods
                         └► RecommendationEngine, SystemSettings
```

- **Forms** only handle UI events and call controllers. They contain no SQL and no scoring.
- **Controllers** validate again, check the role, and apply the rules. They contain no SQL and no ADO.NET.
- **Repositories** read and write rows. They contain no business rules. Gap logic, outcomes and
  scores never appear in SQL.

## 2. Coding pattern (from the Waitering System)

| Waitering System | Future Path |
|---|---|
| `Person`, `Employee`, `Role`: `#region Data Members / Properties / Constructors / Methods`, private fields with public properties, a default constructor and a parameterised one, nested enums | Every entity: `Faculty`, `Learner`, `AdvisorySession`, …; nested enums `AdvisoryOutcome.OutcomeType`, `UserAccount.RoleType`, `DB.DBOperation` |
| `EmployeeController` holds `EmployeeDB` and a `Collection<Employee>`; `DataMaintenance` then `FinalizeChanges` | `XController` holds `XRepository` objects and `Collection<T>`; maintenance goes through private `DataMaintenance(entity, DB.DBOperation)` then `FinalizeChanges(entity, op)` |
| `DB`: `cnMain`, `dsMain`, `daMain`, `FillDataSet`, `UpdateDataSource` | `DB`: the same members, plus parameters, transactions and read helpers |
| `EmployeeDB : DB`: table and SQL strings, `Add2Collection`, `FillRow`, `DataSetChange`, `Build_INSERT_Parameters`, `Create_INSERT_Command`, `UpdateDataSource(entity)` | `XRepository : DB` with the same method names, plus the `_UPDATE_` pair for edits |

**Deliberate departures**, each recorded in `decisions.md`:
1. No database work and no `MessageBox` in constructors or in the Data Layer. Errors are thrown and the forms catch them, so a learner never sees technical text.
2. `FillDataSet` clears the table first (the Waitering version duplicates rows) and takes `SqlParameter`s.
3. Replacing a *set* of child rows uses direct commands inside one `SqlTransaction`. This applies to a session's subjects or responses, a funding option's links, and re-run results. A DataAdapter can't join an outer transaction.
4. There is no delete. Reference data is deactivated (`IsActive = 0`).
5. `DataMaintenance` and `FinalizeChanges` are **private**, so a form can't skip the role check.
6. Class-diagram names are the C# names in PascalCase (`checkEligibility` → `CheckEligibility`). Data-access classes are `XRepository` (the spec name), not `XDB`.

### Repository recipe (Waitering pattern)
**Add**
1. `FillDataSet("SELECT ... FROM T WHERE 1 = 0", table)` to get the table's schema.
2. `NewRow`, then `FillRow(row, entity)`, then `Rows.Add`.
3. `Create_INSERT_Command(entity)`. Its text ends with `; SELECT CAST(SCOPE_IDENTITY() AS INT) AS <PK>`, and it sets `UpdatedRowSource = UpdateRowSource.FirstReturnedRecord`.
4. `Build_INSERT_Parameters` adds one `SourceParam` per column.
5. `UpdateDataSource(table)`.
6. Read the new key from the row.

**Edit**
1. `FillDataSet("SELECT ... FROM T WHERE <PK> = @ID", table, Param(...))`.
2. `FindRow`, then `FillRow`.
3. `Create_UPDATE_Command` with `Build_UPDATE_Parameters`.
4. `UpdateDataSource(table)`.

**Replace a set**
```
SqlTransaction transaction = BeginTransaction();
try { ExecuteNonQuery(transaction, "DELETE ...", ...); ExecuteNonQuery(transaction, "INSERT ...", ...); transaction.Commit(); }
catch { transaction.Rollback(); throw; }
finally { CloseConnection(); }
```

**Reads:** `FillDataSet` with parameters, then build entities in an `Add2Collection`-style loop with `ReadString` / `ReadInt` / `ReadDecimal` / `ReadDate` / `ReadBool`. These turn DBNull into "" / null / false. Write values back with `DbValue` / `DbText`, which turn null or blank into DBNull.

**Rules:**
- Never concatenate a value into SQL; always pass it as a parameter.
- Use a new `SqlParameter` object for every command.
- Don't call `FillDataSet` while a transaction is open.

## 3. Conventions every controller follows

- **Return types.**
  - Reference lists return `Collection<Entity>`.
  - Grids that join tables, compute columns, or include a synthetic row return a `DataTable` with exactly the column names given in the method's XML doc. The forms bind by those names.
  - "All" rows use **0** (IDs) or **""** (codes) as their value.
- **Validation runs twice.** The form validates for feedback. `ValidateX(...)` returns a `Collection<string>` of messages for the ValidationDialog, and every `SaveX(...)` validates again, throwing `ArgumentException` with the first message.
- **Messages are written for the user.** Name the field and the fix, for example "Enter a mark for Mathematics from 0 to 100, with at most one decimal place." Never include exception text or SQL.
- **Role checks (spec Section 8, decisions.md D11).**
  - Every Administrator-only method starts with `loginController.CheckAdministrator(actorUserID)`.
  - Every staff read of learner or session data starts with `CheckStaff(actorUserID)`.
  - Both re-read the account from the database and throw `UnauthorizedAccessException`.
  - Learner methods (UI 2–7) take no user ID; learners never log in.
- **Numbers.**
  - Marks, scores, weights and minimums are `decimal`.
  - SystemParameter values are parsed with the invariant culture (`SystemSettings`).
  - Stored explanation text formats numbers with `"0.#"` and the invariant culture.
- **Currency of data.** Recommendations, funding and reports exclude inactive programmes, inactive funding, and **programmes under an inactive faculty**.
- **Re-run safety** (UNIQUE(SessionID, ProgrammeID)):
  - Saving the selected result deletes the session's previous selected row and any row for that programme.
  - `SaveAll` replaces the session's alternative rows.
  - `SaveSubjects` / `SaveResponses` delete the session's results, because they are stale.

## 4. Outcomes and the engine

| Code (stored, `OutcomeCode`) | Category (shown) | Order |
|---|---|---|
| `STRONG` | Strong Match | 1 |
| `POSSIBLE` | Possible Match | 2 |
| `NCE` | Not Currently Eligible | 3 |
| `MOREINFO` | More Information Needed | 4 |

`RecommendationEngine.ComputeOutcome`. Rules are checked first, then the score. Every value is a SystemParameter row (provisional, `decisions.md` D14).
1. No subjects captured → MOREINFO. The *controller* also returns MOREINFO, before the engine runs, when the learner, the session, the programme, or the interest responses are missing (sequence diagrams, "alt").
2. Any Required subject is missing and no accepted alternative was captured → NCE. The explanation names **every** missing subject.
3. Any Required shortfall is greater than `MARK_TOLERANCE` (2) → NCE.
4. A shortfall of at most 2, or an overall average below the programme minimum → POSSIBLE, naming the gaps.
5. Otherwise STRONG if Overall ≥ `THRESHOLD_STRONG` (75) **and** Interest ≥ `INTEREST_STRONG_MIN` (60). If not, POSSIBLE: "meets the requirements, but interest alignment is weaker".

Overall = `WEIGHT_SUBJECT`·S + `WEIGHT_MARK`·M + `WEIGHT_INTEREST`·I + `WEIGHT_FACULTY`·(bonus ? 100 : 0), with weights 0.30 / 0.30 / 0.30 / 0.10.
- S and M come from `DegreeProgramme.EvaluateSubjectAndMarkMatch`.
- I comes from `Learner.CalculateInterestScore` (relevance-weighted).
- The bonus comes from `DegreeProgramme.MatchesPreferredFaculty`.
- Recommended requirements only add notes.

`RankAlternatives` orders by outcome order, then Overall (descending), then name, and sets `AlternativeRank` 1…n.

`RequirementCheck` (MET / ALT / GAP / MISSING) is the single source of "was this requirement met". UI 6, FP-LSR Table 1, and the FP-DIE / FP-LER gaps all use it.

## 5. Database and seed data

- Schema: `FuturePath/Database/CreateSchema.sql` (19 tables, exactly as `database.md`).
- Seed: `SeedData.sql`.
- Build: `powershell -ExecutionPolicy Bypass -File FuturePath\Database\BuildDatabase.ps1`. It creates `FuturePath.mdf` / `FuturePath_log.ldf` in the project folder, which are copied to `bin` (PreserveNewest).
- Connection: `Properties.Settings.Default.FuturePathConnectionString` = `AttachDbFilename=|DataDirectory|\FuturePath.mdf` on `(LocalDB)\MSSQLLocalDB`. Tests override it through `DB.ConnectionString`.
- **The committed `.mdf` is built on SQL Server 2025 LocalDB.** If it won't open on your machine (`sqllocaldb versions` shows an older version), run `BuildDatabase.ps1`.
- `GradeLevelID` 1 = Grade 11 (Projected), 2 = Grade 12 (Final), matching the `GradeLevel` enum.
- Seed IDs are fixed with `IDENTITY_INSERT` so tests can refer to them. The Part B list is in `plan-and-tests.md`.

**SystemParameter rows**

| Code | Value |
|---|---|
| `MIN_SUBJECT_COUNT` | 7 |
| `NSC_LEVEL_1..7` | 0, 30, 40, 50, 60, 70, 80 |
| `FUNDING_TYPE_1..4` | Scholarship, Bursary, Financial Aid, External Funding |
| `PROVINCE_1..9` | Eastern Cape, Free State, Gauteng, KwaZulu-Natal, Limpopo, Mpumalanga, North West, Northern Cape, Western Cape |
| `WEIGHT_SUBJECT` | 0.30 |
| `WEIGHT_MARK` | 0.30 |
| `WEIGHT_INTEREST` | 0.30 |
| `WEIGHT_FACULTY` | 0.10 |
| `THRESHOLD_STRONG` | 75 |
| `INTEREST_STRONG_MIN` | 60 |
| `MARK_TOLERANCE` | 2 |

## 6. Testing

Tests are PowerShell 5.1 scripts in `tests/` that load the built `FuturePath.exe` and call the real classes. See `tests/TestHelpers.ps1`.
- Each run attaches a **copy** of the database in a temp folder, so the project `.mdf` and `bin` are never touched.
- Run each script in its own process:

```
powershell -NoProfile -ExecutionPolicy Bypass -File tests\Contract-Smoke.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tests\PartB-Regression.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tests\TC01-TC05.ps1
```

`PartB-Regression.ps1` must pass after **any** change to the rule engine, the requirements, or the scoring parameters.

## 7. Who owns which file (this round)

| Area | Files |
|---|---|
| Shared contract (coordinator; change only by agreement) | `DB.cs`, every entity / result class, `SystemSettings`, `SystemParameterRepository`, `SubjectRepository`, `LoginController`, the read methods of `UserAccountRepository`, `CreateSchema.sql`, `BuildDatabase.ps1`, `FuturePath.csproj`, `App.config`, `Properties/Settings.*`, `tests/TestHelpers.ps1`, `tests/Contract-Smoke.ps1`, this file, `decisions.md` |
| Seed and tests | `SeedData.sql`, `tests/PartB-Regression.ps1`, `tests/TC01-TC05.ps1`, the Part B section of `plan-and-tests.md` |
| Learner journey | `LearnerRepository`, `AdvisorySessionRepository`, `DegreeProgrammeRepository`, `RecommendationEngine`, `LearnerController`, `EligibilityController`, `RecommendationController`, `tests/Engine-PartB.ps1`, `tests/Learner-Smoke.ps1` |
| Admin and reference data | `FacultyRepository`, `GradeLevelRepository`, `InterestRepository`, `FundingRepository`, `UserAccountRepository` (maintenance methods), `UserAccountController`, `ReferenceDataController`, `InterestController`, `FundingController`, `tests/Admin-Smoke.ps1` |
| Reports and search | `ReportRepository`, `ExportReportRepository`, `SessionSearchRepository`, `ReportController`, `SessionSearchController`, `tests/Reports-Smoke.ps1` |

## 8. Notes for the forms round (next)

- Create controllers inside the forms' existing `try` blocks, never as field initialisers.
- `MainForm.Outcome.cs` must map `NCE` and `MOREINFO` (use `AdvisoryOutcome.GetCategory(code)`).
- Align the TODO calls with these signatures:
  - `GetFaculties(includeInactive, includeAllRow)`;
  - `GetProgrammes(facultyID, includeInactive)`;
  - the funding type filter uses the type text (ParameterValue);
  - `GetDailySummary` uses `out` parameters.
- Call `ReportController.CompleteLearnerSession` **before** writing the FP-LSR file on UI 7, so a failure never leaves a written file for an incomplete session.
- Map the "All" rows: 0 / "" → null filter.
