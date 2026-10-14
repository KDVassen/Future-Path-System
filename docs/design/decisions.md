# Future Path — design decisions

_Updated 14 October 2026._

The submitted specification (`docs/spec/INF2011S_Team_14_Part_1.pdf`) is the contract. This file
records only what the team decided where the spec is silent, unclear or deliberately changed, so the
team can defend each choice at the demo. Code comments cite these entries as "decisions.md D3",
"decisions.md §2" and so on, so keep the numbers stable when you edit.

## 1. Decisions

### Data and sessions

| # | Decision | Why |
|---|---|---|
| D1 | **`UserAccount` and `SystemParameter` are built**, although the submitted ERD (Figure 5) leaves them out. | Section 8, the class diagram, `ExportReport.GeneratedByUserID` and the stored minimum on UI 3 all depend on them. |
| D2 | **All three reports are logged in `ExportReport`.** `SessionID` is nullable, and `ReportCode` and `ParametersUsed` are added. | FP-DIE and FP-LER cover many sessions, so they have no single session to point at. |
| D3 | **A session is written as it goes.** Rows are written from UI 2 onward with `SessionStatus = 'In progress'`; **Save and export** on UI 7 sets `'Completed'`. Reports count completed sessions only. UI 2 says "your session is completed when you export the summary". | This matches both sequence diagrams. |
| D4 | **Subject level lives in `Subject.SubjectLevel`**, so UI 3 fills it in (read-only). NSC achievement bands 1–7 are `SystemParameter` rows. | The level follows from the subject; no hard-coded bands. |
| D5 | **Administrators maintain user accounts on UI 13.6 "User accounts"**, a tab beside 13.1–13.5. | Section 8, Stage 7 needs a place for it. |

### Screens and navigation

| # | Decision | Why |
|---|---|---|
| D6 | **One window size for both main windows: about 990 × 800** (client 974 × 761 at 100% scaling). Modal dialogs are 520 × 300. | The rail is gone (D7), so every card keeps its position in a narrower window. One size is simpler than the spec's "wide 1200 × 800" and "standard 1000 × 720", and it still fits 1400 × 800. |
| D7 | **Visible tabs replace the navigation rail** (spec 2.2.1 / 2.2.2). Each window is one `TabControl`: the learner window has Home and steps 1–6; the staff console has the dashboard, Sessions, Reference data, Reports and Report viewer. UI 6/6.1a/6.2b, UI 10.1a–10.3c and UI 13.1–13.6 sit in a `TabControl` inside their tab. The learner can only open a step already reached (`tcMain_Selecting`), which keeps the fixed order of 2.2.2. The Report viewer tab opens only after a report is previewed. | The custom rail, hidden tab strip and central page switcher were hard to explain at the code walkthrough. A standard `TabControl` does the same job with far less code. |
| D8 | **No title band.** The tab header and the window caption ("Future Path — <tab>") name the screen. The learner's name and grade, and the signed-in staff member and role, are shown on the right of the status bar. The LEARNER / ADMINISTRATOR badge is dropped. | Removes a zone that repeated the tab name. |
| D9 | **Each tab has its own buttons**, right-aligned along its bottom edge in the spec's order (primary furthest right, then Back, then Cancel), each with its own `_Click` handler. Each learner tab that needs one has its own notice label at the top. The staff "Close" button on UI 10 and "Cancel" on UI 11 are dropped. | Replaces the shared action band and its central dispatcher. Close and Cancel only returned to the dashboard, which the Dashboard tab now does. |
| D10 | **A `MenuStrip`** gives the learner **Home** and **Exit**, and staff **Sign out** and **Exit**. Home asks before leaving a session; Exit and the caption-bar X ask before closing. | Sign out and Home were on the rail. A menu keeps them one click away on every tab. |
| D11 | **Role-based security in the UI and the BLL.** The staff console removes the tabs a role may not reach (`TabPages.Remove`): Career Advisors get the Advisor dashboard, Sessions and Reports; Administrators also get Reference data and the Administrator dashboard. Every BLL method that changes data checks the role again. | Section 8. Administrator-only screens are never shown to a Career Advisor. |
| D12 | **A missing faculty on UI 5 is shown in red**, like every other required field (`CheckField`). This **deliberately departs from spec 2.3.5**, which says no value on UI 5 can be invalid and shows the message in amber. | One rule for every required field is easier to use and to explain than a special amber case. |
| D13 | **Future Path is DPI-aware** (`app.manifest`, `dpiAware = true`). | Without it, on a 125% display Windows capped the window at its unscaled height, so the bottom of every tab was hidden. With it, both forms keep their designed size whatever DPI Visual Studio saved them at. Per-monitor awareness was tried and dropped, because the staff grids left stale pixels. |

### Business logic, data layer and database (application layer, October 2026)

How the classes are built is described in `application-layer.md`.

| # | Decision | Why |
|---|---|---|
| D14 | **Provisional scoring values** (SystemParameter): WEIGHT_SUBJECT 0.30, WEIGHT_MARK 0.30, WEIGHT_INTEREST 0.30, WEIGHT_FACULTY 0.10, THRESHOLD_STRONG 75, INTEREST_STRONG_MIN 60, MARK_TOLERANCE 2. The outcome is **rules first**: a missing Required subject → Not Currently Eligible; a Required mark more than 2 points short → Not Currently Eligible; within 2 points, or an average below the programme minimum → Possible Match; all met with Overall ≥ 75 and Interest ≥ 60 → Strong Match, otherwise Possible Match. **Oyama and the team must confirm or change these values**; every change re-runs `tests/PartB-Regression.ps1`. | The spec defines the four components and outcomes but no numbers (§4 below). These values reproduce all five Part B outcomes. Labeeqah (2 points short) is Possible, and the designed FP-LER example (46% against 50%) is Not Currently Eligible. |
| D15 | **Outcome codes** stored in `RecommendationResult.OutcomeCode` are STRONG / POSSIBLE / NCE / MOREINFO. The words shown to people ("Not Currently Eligible", …) come from `AdvisoryOutcome.GetCategory`. | OutcomeCode is VARCHAR(10), so the full words don't fit. One list in the BLL replaces the different spellings the forms were guessing at. |
| D16 | **Provinces are SystemParameter rows** PROVINCE_1 … PROVINCE_9. | UI 2 and UI 10.1a need a value-bound province list. There is no Province table in the ERD, and the rubric penalises hard-coded lookups. |
| D17 | **The overall average includes every captured subject**, Life Orientation too. No subject name is hard-coded in the rules. | It's the simplest rule to explain, and it changes no Part B outcome. |
| D18 | **The Data Layer follows the Waitering System `DB` pattern with five changes**: (1) no database work and no `MessageBox` in constructors or repositories; errors are thrown and the forms show a friendly message; (2) `FillDataSet` takes parameters and clears the table first; (3) replacing a set of child rows (a session's subjects or answers, funding links, re-run results) uses direct commands in one transaction; (4) no delete, only deactivation; (5) `DataMaintenance` / `FinalizeChanges` are private, so a form can't skip the BLL role check. | The Waitering version shows technical text, duplicates rows on refill, and can't run in a transaction. Robustness is 20% of the Part 2 mark. |
| D19 | **Classes added to the class diagram** (an extension of Section 4, see §3): `LoginController` is the "Authorization & access" package (Authenticate, CheckStaff, CheckAdministrator). `RequirementCheck` is the one place that decides whether a requirement is met; UI 6, FP-LSR, FP-DIE and FP-LER all use it, so no gap rule lives in SQL. `SystemSettings` is the parameter helper. One controller per screen group follows the `XController` / `XRepository` pattern. The empty `User.cs` and `AdminDBController.cs` placeholders are replaced by `UserAccount` and the repositories. | The class diagram covers the two sequence diagrams only. Section 8, the reports and the maintenance screens need these. |
| D20 | **Checking again replaces the earlier result.** Saving the selected result replaces the session's previous selected row. Generating alternatives replaces the earlier alternatives. Saving subjects or answers again clears the session's results. | `UNIQUE(SessionID, ProgrammeID)` would otherwise make a second "Check eligibility" fail, and a stored result must always match the marks it was made from. |
| D21 | **The database ships as `FuturePath/FuturePath.mdf`**, attached through `|DataDirectory|` like the Waitering System's `EmployeeDatabase.mdf`. `Database/CreateSchema.sql`, `SeedData.sql` and `BuildDatabase.ps1` rebuild it. The `.mdf` is built with SQL Server 2025 LocalDB; on an older LocalDB, run `BuildDatabase.ps1`. | It works straight after a clone, and the scripts keep it explainable and rebuildable. |

## 2. Clashes resolved in favour of the spec

| Topic | What is built |
|---|---|
| Red for "Not Currently Eligible" | **Not used.** Red is for invalid input only (2.2.6). An unmet requirement is **amber**, and every outcome carries its own words and marker (2.3.6). |
| Who generates FP-LSR | **The learner exports it on UI 7.** An Advisor or Administrator can regenerate it later. For a learner export, `GeneratedByUserID` is **null** (6.1). |
| FP-LSR subject order | **By subject name, ascending** (6.1 Sequence). |
| FP-LSR content | **Three tables and two charts** across two A4 portrait pages (6.1 Layout). |
| Screen fonts | **Spec sizes**: title 20 pt Semibold, section heading 12 pt Semibold, body 10 pt, field label 12 pt Semibold, caption 8 pt, figures Consolas 10 pt. |
| Spacing | **About 20 px between groups of controls** (2.2.1). |
| Message classes | The **four in 2.2.7** (field message, validation summary, validation dialog, status message), plus **confirmation requests** before leaving or submitting a session, exporting, or deactivating (Section 7). Success is reported in the status bar, never in a modal (2.3.9). |
| Lockout after five failed logins | **Not built.** It is not in the spec. |
| UI 10 export | UI 10.2b opens and exports the **FP-LSR** for a session. The other reports are made on UI 11. |
| "Alternate" vs "Alternative" | **"Alternative"** in all code and UI labels. |

## 3. Gaps filled where the spec is silent

- **Learner model.** The class diagram gives `Learner` the grade, preferred faculty and subjects,
  and gives `AdvisorySession` an `outcome`; the ERD stores all of these per session. **The database
  follows the ERD; the classes follow the class diagram** as the in-memory "learner in this
  session", and the repositories map between them. `AdvisorySession.outcome` is read from the
  selected programme's `RecommendationResult.OutcomeCode`. Marks belong to the day they were
  captured, and a returning Grade 12 learner has new ones.
- **Extra columns from the class diagram:** `RecommendationResult.FacultyBonus BIT` (the sequence
  diagram's `matchesPreferredFaculty()` returns a boolean), `RecommendationResult.AlternativeRank`,
  and `ProgrammeRequirement.AdvisoryNote`.
- **UI 13.2** needs `ProgrammeRequirement.RequirementType` and `IsActive`. Its "Minimum average"
  row reads and writes `DegreeProgramme.MinimumOverallAverage`.
- **UI 4** needs `InterestQuestion.DisplayOrder`, so statements appear in a stored order.
- **UI 2:** "Advising year" is `AdvisorySession.AdvisingYear`; "Captured by" always shows
  "Self-service"; the session number `yyyy-NNNN` is built from `SessionDate` and `SessionID` and is
  not stored.
- **UI 6.1a's "include alternatives" tick box** is `AdvisorySession.AlternativesInSummary`, so a
  regenerated FP-LSR matches. Funding uses `FundingInfoRequested`, set by **Add to Summary** on
  UI 6.2b, which then opens UI 7.
- **School name is free text** (the ERD stores text and the wireframe draws a text box). Province
  is value-bound. Both are required.
- **Marks accept one decimal place** and are stored as `DECIMAL(5,1)`.
- **FP-LER exports PDF and Word** (6.3 Media).
- **FP-DIE keeps its six key figures on page 1**, as in the submitted 6.2.2 layout.
- **FP-LER keeps a Reason column**, because 6.3 requires the reason per learner.
- **Funding types** (Scholarship, Bursary, Financial Aid, External Funding) are `SystemParameter`
  rows `FUNDING_TYPE_1…4`, and the UI 13.3 combo is bound to them.
- **The minimum subject count (7) is a warning, not a block.** Every Part B learner has 5 or 6
  subjects, and Siyabonga Dickson (5) must still reach a Strong Match. UI 3 shows the amber notice;
  **Save and continue** opens once one subject is captured.
- **Controllers beyond the class diagram** (login, reference-data maintenance, funding, session
  search, reports) follow the same `XController` / `XRepository` pattern and are recorded as an
  extension of Section 4, not a deviation.

## 4. Still open — ask before building

- **Scoring weights and outcome thresholds: confirm the provisional values in D14** (owner: Oyama,
  Stage 5). The spec defines the four components and the four outcomes but gives no numbers.
  Provisional values are now in `SystemParameter` and reproduce the five Part B outcomes (D14).
  The team should confirm them, or agree on different numbers, before the demo. Any change must
  still pass `tests/PartB-Regression.ps1`. Example: Labeeqah is 2 points short and must be
  Possible, while the designed FP-LER shows "Mathematics 46% below 50% minimum" as Not Currently
  Eligible.
