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

`ReportController` is one class split into three files, like the forms' partial classes:
- `ReportController.cs`: the catalogue, the export log and the shared helpers;
- `ReportController.LearnerSummary.cs`: FP-LSR;
- `ReportController.PeriodReports.cs`: FP-DIE and FP-LER.

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
- **0 means "not given".** No public method takes `int?`, `bool?` or `decimal?`.
  - An ID of 0 means "new" in SaveX methods (e.g. `SaveFaculty(actor, 0, "Law", true)` adds) and "all" in filters (e.g. `GetProgrammes(0, false)`).
  - A minimum mark or average of 0 means "no minimum".
  - Nullable types remain only on entity properties whose database column allows NULL (e.g. `DegreeProgramme.MinimumOverallAverage`), so the class mirrors its table.
- **Results are returned, never passed back through `out` parameters.**
  - Several values come back in a small result class: `DailySummary`, `ReportScope`, `ProgrammeAlignment`.
  - Our own parse methods return a value: `LearnerSubject.ParseMark` gives -1 for an invalid mark, and `AdvisorySession.GetSessionID` / `GetSessionYear` give 0 for an invalid number.
  - .NET's own `int.TryParse` and `decimal.TryParse` are still used inside them.
- **Plain `if` / `else`.** The code has no `??` and no `? :`.
- **SQL WHERE clauses are built with `if` statements.** Only the filters that were given are added, each as a parameter:
  ```csharp
  string sql = sqlLocal + " WHERE 1 = 1";
  List<SqlParameter> parameters = new List<SqlParameter>();
  if (facultyID > 0)
  {
      sql += " AND p.FacultyID = @FacultyID";
      parameters.Add(Param("@FacultyID", SqlDbType.Int, facultyID));
  }
  FillDataSet(sql, table, parameters.ToArray());
  ```
  Parameters are the only defence against SQL injection, and they are always used. A typed `%` in a search box is passed as a parameter value, so at worst it acts as the LIKE wildcard; it never becomes SQL.
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
- Seed: `SeedData.sql` holds every row of the shipped database: the sessions' inputs (subjects, marks, answers, chosen programme) and the results the real engine wrote for them (RecommendationResult, the ExportReport log, Completed for sessions 6–12). The results were made by CheckEligibility, GenerateAlternatives and CompleteLearnerSession when the database was first built, never by hand arithmetic, and are stored exactly as written (decisions.md D25).
- The database ships as `FuturePath/FuturePath.mdf` / `FuturePath_log.ldf`, copied to `bin` on build (PreserveNewest). The two scripts recreate exactly its data; `tests\Seed-Check.ps1` checks every table row for row.
- Rebuild it in Visual Studio, with no PowerShell:
  1. Make an empty database: Add → New Item → Service-based Database, or Add New Database under `(localdb)\MSSQLLocalDB` in SQL Server Object Explorer.
  2. In Server Explorer, right-click that database → New Query. Paste `CreateSchema.sql` and Execute. It stops if the window is connected to `master` or another system database.
  3. Open another New Query on the same database. Paste `SeedData.sql` and Execute.
  - Re-run `tests\Seed-Check.ps1` and `tests\PartB-Regression.ps1` after changing the engine, the scoring parameters or the seed.
- Connection: `DB` reads `private string strConn = Settings.Default.FuturePathDatabaseConnectionString;` (as in the Waitering System), which is `AttachDbFilename=|DataDirectory|\FuturePath.mdf` on `(LocalDB)\MSSQLLocalDB`. Tests point that setting at a copy of the database (`tests\TestHelpers.ps1`, `Set-AppConnectionString`).
- **The committed `.mdf` is built on SQL Server 2025 LocalDB.** If it won't open on your machine (`sqllocaldb versions` shows an older version), rebuild it from the two scripts as above.
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
powershell -NoProfile -ExecutionPolicy Bypass -File tests\PartB-Regression.ps1
```

| Script | Database | Checks |
|---|---|---|
| `Contract-Smoke.ps1` | empty schema (`-SchemaOnly`) | entity rules, hashing, login and role checks (31) |
| `Engine-PartB.ps1` | none | the engine on the five Part B learners in memory, boundaries, ranking (39) |
| `Learner-Smoke.ps1` | empty schema | UI 2-6.1a flow through the real controllers (157) |
| `Admin-Smoke.ps1` | empty schema | lookups and every UI 13.x maintenance method, role refusals (172) |
| `Reports-Smoke.ps1` | empty schema | session search, export log, FP-LSR / FP-DIE / FP-LER datasets (194) |
| `Seed-Check.ps1` | seeded copy | every seed rule, and each session's stored outcome against its scenario (344) |
| `PartB-Regression.ps1` | seeded copy | the five Part B outcomes through the real controllers, re-run safety, boundaries (75) |
| `TC01-TC05.ps1` | seeded copy | the spec Section 10 demonstration cases, security and inactive-data checks (74) |

`PartB-Regression.ps1` must pass after **any** change to the rule engine, the requirements, or the scoring parameters.

## 7. Who built which file (October 2026 round; now merged)

| Area | Files |
|---|---|
| Shared contract (coordinator; change only by agreement) | `DB.cs`, every entity / result class, `SystemSettings`, `SystemParameterRepository`, `SubjectRepository`, `LoginController`, the read methods of `UserAccountRepository`, `CreateSchema.sql`, `FuturePath.csproj`, `App.config`, `Properties/Settings.*`, `tests/TestHelpers.ps1`, `tests/Contract-Smoke.ps1`, this file, `decisions.md` |
| Seed and tests | `SeedData.sql`, `tests/PartB-Regression.ps1`, `tests/TC01-TC05.ps1`, the Part B section of `plan-and-tests.md` |
| Learner journey | `LearnerRepository`, `AdvisorySessionRepository`, `DegreeProgrammeRepository`, `RecommendationEngine`, `LearnerController`, `EligibilityController`, `RecommendationController`, `tests/Engine-PartB.ps1`, `tests/Learner-Smoke.ps1` |
| Admin and reference data | `FacultyRepository`, `GradeLevelRepository`, `InterestRepository`, `FundingRepository`, `UserAccountRepository` (maintenance methods), `UserAccountController`, `ReferenceDataController`, `InterestController`, `FundingController`, `tests/Admin-Smoke.ps1` |
| Reports and search | `ReportRepository`, `ExportReportRepository`, `SessionSearchRepository`, `ReportController`, `SessionSearchController`, `tests/Reports-Smoke.ps1` |

## 8. Notes for the forms round (done)

The forms now call these methods; no `// TODO BLL:` placeholder is left (decisions.md D23). The notes
below stay as the record of how the forms' guessed calls were matched to the real signatures.

**How a form calls the BLL** (the same in every screen file):
- The controller is a local variable created inside the existing `try`, e.g.
  `ReferenceDataController referenceDataController = new ReferenceDataController();`.
- A list sets `DisplayMember` and `ValueMember` **before** `DataSource`, then `SelectedIndex = -1` (or 0 for
  an "All" row). Binding fires SelectedIndexChanged, and only then is `SelectedValue` already the ID.
- "Nothing chosen" is passed as 0 or "" (`StaffConsoleForm.SelectedId` / `SelectedCode`).
- `catch (ArgumentException ex)` shows `ex.Message` (the BLL's own words for a refused value) before the
  screen's generic `catch (Exception)` message. UI 11 also shows ReportController's `InvalidOperationException`
  messages ("Session … is still in progress").
- Bound items are read as the BLL types (`(Faculty)lstFaculties.SelectedItem`, `DataRowView`), not by reflection.


- The synthetic **"Minimum average"** row (UI 13.2) and **"Overall average"** row (UI 5) come last. Their values:
  - `RequirementID` DBNull;
  - `RequirementType` "Required";
  - `SubjectID`, `AlternativeSubjectID` and `AlternativeMinimumMark` DBNull;
  - `AlternativeSubjectName` "";
  - `IsActive` true;
  - `MinimumMark` = the programme minimum.
- A minimum average or minimum mark of 0 on UI 13.1 / 13.2 means "no minimum" (stored as NULL). Minimums accept two decimals.
- Form TODO calls that change:

  | Form TODO | Use instead |
  |---|---|
  | `GetFaculties(includeInactive)` | `GetFaculties(includeInactive, includeAllRow)` |
  | `GetGradeLevels()` | `GetGradeLevels(includeAllRow)` |
  | `GetAllProgrammes` / `GetDegreeProgrammes` | `GetProgrammes(0, false)` |
  | `GetSubjects()` | `GetSubjects(includeInactive)` |
  | `SaveRequirement` | also passes `alternativeMinimumMark` |
  | `CalculateCategoryScores(SessionId, …)` | `CalculateCategoryScores(responses)` |
  | `GetProgrammeAlignment` | returns a `ProgrammeAlignment` (AlignmentPercent, StrongestCategories) |
  | `GetDailySummary` | returns a `DailySummary` (CompletedCount, InProgressCount, ExceptionCount) |
  | `ReportController.GetScope` | returns a `ReportScope` (SessionCount, FacultyCount, ProgrammeCount) |
  | Any "null = new" or "null = all" argument | pass 0 instead |
  | Funding type combos | ValueMember `FundingType` |
- Create controllers inside the forms' existing `try` blocks, never as field initialisers.
- `MainForm.Outcome.cs` must map `NCE` and `MOREINFO` (use `AdvisoryOutcome.GetCategory(code)`).
- Align the TODO calls with these signatures:
  - `GetFaculties(includeInactive, includeAllRow)`;
  - `GetProgrammes(facultyID, includeInactive)`;
  - the funding type filter uses the type text (ParameterValue).
- Call `ReportController.CompleteLearnerSession` **before** writing the FP-LSR file on UI 7, so a failure never leaves a written file for an incomplete session.
- The "All" rows already carry 0 / "", which the controllers read as "no filter", so pass them straight through.
- **Known limitation:** `CompleteLearnerSession` makes two writes, the session status and then the ExportReport row, through two repositories with no shared transaction.
  - If the second write fails, the session is Completed but has no export row.
  - Both values are validated first, so this needs a database failure between the two statements. On a local single-user LocalDB that is unlikely.
  - Fix if the team wants it: one repository method that does both in one transaction.
