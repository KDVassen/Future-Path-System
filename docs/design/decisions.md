# Future Path — design decisions and clarifications

_Updated 29 September 2026, after reviewing the submitted specification
(`docs/spec/INF2011S_Team_14_Part_1.pdf`) against the design notes._

**Precedence.** The submitted specification is the contract. Where a design note disagrees with
it, the spec wins, and this file records how each clash was resolved. Where the spec is silent,
the design notes fill the gap.

## 1. Settled by Kealon, 29 September

| # | Question | Decision |
|---|---|---|
| D1 | The submitted ERD (Figure 5) leaves out `UserAccount` and `SystemParameter`. | **Both tables are built.** Section 8, the class diagram, `ExportReport.GeneratedByUserID` and the "stored minimum" on UI 3 all depend on them. |
| D2 | Which reports are logged in `ExportReport`? | **All three.** `SessionID` is nullable, and `ReportCode` and `ParametersUsed` are added. |
| D3 | When is an advisory session written? | **As it goes.** Rows are written from UI 2 onward with `SessionStatus = 'In progress'`. **Save and export** on UI 7 sets it to `'Completed'`. Reports count completed sessions only. This matches the two sequence diagrams. The UI 2 status-band text "nothing is saved until the advisory summary" changes to "your session is completed when you export the summary". |
| D4 | Where do Subject level and NSC achievement level live? | **`Subject.SubjectLevel`**, so the level follows from the subject and UI 3 fills it in (read-only). NSC achievement bands 1–7 are `SystemParameter` rows. |
| D5 | Where do Administrators maintain user accounts (Section 8, Stage 7)? | **A new TabPage, UI 13.6 "User accounts"**, in UI 13 next to 13.1–13.5. |

## 2. Clashes resolved in favour of the spec

| Topic | Design notes said | Spec says, and what is built |
|---|---|---|
| Red for "Not Currently Eligible" | Proposed a palette amendment. | **Rejected.** Red is only for invalid input (2.2.6). An unmet requirement is **amber**, and every state carries its own words and marker (2.3.6). |
| Who generates FP-LSR | Advisor/Admin only; never given to a learner. | **The learner exports it on UI 7.** An Advisor or Admin can regenerate it later. The audience is the learner, and parents, teachers and advisors are the secondary audience (6.1). For a learner export, `GeneratedByUserID` is **null**. |
| FP-LSR subject order | As captured. | **By subject name, ascending** (6.1 Sequence). |
| FP-LSR content | Page-by-page block list. | **Three tables and two charts** across two A4 portrait pages (6.1 Layout). |
| Screen fonts | Title 15.75 pt, label 8.25 pt, and so on. | **Spec sizes**: title 20 pt Semibold, section 12 pt Semibold, body 10 pt, field label 12 pt Semibold, caption 8 pt, figures Consolas 10 pt. |
| Spacing | 4 px ramp. | **About 20 px between control groups** (2.2.1). |
| Message classes | Six classes. | The **four in 2.2.7** (field message, validation summary, validation dialog, status message), plus **confirmation requests** before submitting a session, exporting, or deactivating (Section 7). Success is reported in the status band, never in a modal (2.3.9). |
| Lockout after five failed logins | Proposed. | **Not built.** It isn't in the spec. |
| UI 10 export format | Open. | Not specified. UI 10.2b opens and exports the **FP-LSR** for a session. The other tabs use the Reports hub (UI 11). |
| "Alternate" vs "Alternative" | Undecided. | The spec uses both. **"Alternative"** is used in all code and UI labels. |

## 3. Gaps filled where the spec is silent (review these)

- **The class diagram and the ERD model the learner differently.** The class diagram gives
  `Learner` the grade, preferred faculty and subjects, and gives `AdvisorySession` an `outcome`.
  The ERD stores all of these per session. **The database follows the ERD. The classes follow the
  class diagram as the in-memory model of "the learner in this session"**, and the repositories
  map between the two. `AdvisorySession.outcome` is read from the selected programme's
  `RecommendationResult.OutcomeCode`. Defence: marks belong to the day they were captured, and a
  returning Grade 12 learner has new ones.
- **Class diagram attributes the ERD figure doesn't show** are added as columns:
  - `RecommendationResult.FacultyBonus BIT`, replacing a decimal faculty score (the sequence
    diagram's `matchesPreferredFaculty()` returns a boolean)
  - `RecommendationResult.AlternativeRank`
  - `ProgrammeRequirement.AdvisoryNote` (renamed from `RequirementNote`)
- **UI 13.2 needs `ProgrammeRequirement.RequirementType` and `IsActive`.** The screen sets a
  requirement type and shows active/inactive chips. A "Minimum average" row on UI 13.2 is
  read from and written to `DegreeProgramme.MinimumOverallAverage`.
- **UI 4 needs `InterestQuestion.DisplayOrder`.** Statements are shown in a stored order.
- **UI 2 "Advising year"** is stored as `AdvisorySession.AdvisingYear`. **"Captured by"** always
  displays "Self-service", because learners complete their own sessions. **The session number**
  is shown as `yyyy-NNNN`, built from `SessionDate` and `SessionID`, and is not stored.
- **UI 6.1a's "include alternatives" tick box** is stored as
  `AdvisorySession.AlternativesInSummary`, so an advisor regenerating FP-LSR gets the same
  document. Funding uses the existing `FundingInfoRequested` flag, which **Add to Summary** sets.
- **School name is free text.** 2.3.2 calls it value-bound, but the ERD stores `SchoolName` as
  text and the wireframe draws a text box. Province is value-bound. School name and province
  are NOT NULL because UI 2 marks them required.
- **Marks accept one decimal place.** Section 7 says "whole or one-decimal". The UI 3 wireframe's
  "whole number" message is updated to match. Stored as `DECIMAL(5,1)`.
- **FP-LER exports PDF and Word.** 6.3 Media says pdf/docx, and 6.3 Distribution says PDF or
  Excel. The Media row is used, which also matches the design notes.
- **FP-DIE keeps its six key figures on page 1** (sessions, learners, and the count and % of
  each outcome). The 6.2.1 Content row doesn't list them, but the submitted 6.2.2 layout
  shows them.
- **The FP-LER listing keeps a Reason column.** 6.3 Content leaves it out of the column list,
  but 6.3 Objectives require the specific reason per learner.
- **UI 6.2b feeds UI 7 through Add to Summary.** The 2.3.8 text and the 2.1 narrative say
  so, even though the flow figure still draws 6.2b returning by Back only.
- **Funding types** (Scholarship, Bursary, Financial Aid, External Funding; Section 7) are
  `SystemParameter` rows `FUNDING_TYPE_1…4`, and the UI 13.3 combo is bound to them. This keeps
  them out of the code.
- **The minimum subject count (7) is a warning, not a block.** Every Part B learner has 5 or 6
  subjects, and Siyabonga Dickson (5 subjects) must still reach a Strong Match. UI 3 shows the
  amber notice. **Save and continue** is enabled once at least one subject is captured
  (Section 7: "at least one subject and mark").
- **Modal dialogs** (UI 2.1a, 3.1a, 8.1a, confirmations) use a 520 × 300 dialog size. The
  2.2.1 text announces "three sizes" but lists only two.
- **Only two controllers are in the class diagram.** Login, reference-data maintenance,
  funding, session search and reports need their own controller and repository classes. Build
  them in the same pattern (`<<controller>> XController`, `<<data access>> XRepository`) and
  record each as an extension of Section 4, not a deviation.

## 4. Still open — ask before building

- **Scoring weights and outcome thresholds** (owner: Oyama, Stage 5). The spec defines the four
  components and the four outcomes but gives no numbers. They go in `SystemParameter`
  (`WEIGHT_SUBJECT`, `WEIGHT_MARK`, `WEIGHT_INTEREST`, `WEIGHT_FACULTY`, plus threshold keys).
  Whatever is chosen must reproduce the five Part B outcomes. It also has to decide how far
  below a minimum a mark can be and still be a Possible Match rather than Not Currently
  Eligible. Labeeqah is 2 points short and must be Possible. The designed FP-LER shows
  "Mathematics 46% below 50% minimum" as Not Currently Eligible.
