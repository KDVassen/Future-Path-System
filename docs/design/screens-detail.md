# Future Path — detailed screen behaviour (spec Section 2.3)

Per-screen build notes, taken from the submitted 2.3 text and wireframes, with the settled
changes from `decisions.md`. The wireframe images are `docs/figures/ui-*.png` and are the
visual reference. The fields and rules below are the contract.

**Every learner screen:** Cancel and Back exit without losing data. Cancel asks for
confirmation. From UI 2 onward the session is saved as it goes, with `SessionStatus = 'In progress'`
(`decisions.md` D3).

## UI 8 — Staff login (standard dialog)
- Enter **Username\*** and **Password\*** (masked). Two buttons in the action band.
- On **Login**: look up `UserAccount` by username. Verify the salted hash and check `IsActive`.
  Route by `Role` to UI 9 or UI 12.
- A blank or unrecognised entry turns the label and text box red, and the advisory panel
  changes from blue guidance to a red message naming the field and the fix (UI 8.1a). Never
  say which of username or password was wrong.

## UI 2 — Learner profile (wide, step 1 of 6)
- **Optional:** First name, Surname, Email address, Mobile number (0XX XXX XXXX). Contact
  details are used only for advisor follow-up and are **never printed on FP-LSR**.
- **Required\*:** Current grade (ComboBox → `GradeLevel`, with the hint "Grade 11 marks are
  treated as projected results"), School name (TextBox), Province (ComboBox, value-bound),
  Advising year (NumericUpDown, defaults to the current year).
- **System-supplied, read-only "Session details":** session number (`yyyy-NNNN`), date started
  (system date), captured by ("Self-service"), status ("In progress").
- Validation fires on **Save and continue**: a missing required value turns the label and
  control red, and the advisory panel turns red and names the field.
- Email and mobile number are format-checked if entered.

## UI 3 — Subjects and marks (wide, step 2) — the graded rich dialog
- Add a subject: **Subject\*** (ComboBox → active `Subject`), **Subject level** (read-only,
  filled from `Subject.SubjectLevel`), **Mark % (0–100)\*** (NumericUpDown, 0–100, one decimal).
  **Add subject** (amber) commits a row to the grid.
- Grid: Subject · Level · Mark · Achievement level · bin icon (removes the row). The
  achievement level comes from the `NSC_LEVEL_n` parameters. The footer shows the captured
  count and average, recalculated on every add or delete.
- **Validation on Add subject:** a duplicate subject in this session, or a mark out of range,
  turns the controls red, and the **validation summary** lists every entry to fix (UI 3.1a on
  leaving with invalid data). The BLL checks for duplicates too (field-combination control).
- **Amber notice:** "n more subjects needed" against `MIN_SUBJECT_COUNT`. This is a **warning,
  not a block.** Save and continue needs at least one subject. There is no maximum, and nothing
  truncates the list.

## UI 4 — Interest questionnaire (wide, step 3)
- A statement grid with one five-point RadioButton group per active `InterestQuestion`, ordered
  by `DisplayOrder`. The category name shows under each statement. Responses are
  Strongly disagree (1) … Strongly agree (5).
- Four running-figure tiles show category scores calculated from the answers so far.
- Unanswered statements are shaded amber and counted in the advisory panel. **Save and
  continue stays disabled until every question is answered.** Red never appears on this screen.

## UI 5 — Faculty and department selection (standard, step 4)
- A search-mode radio pair, a **Faculty\*** ComboBox (→ active `Faculty`), and a programme grid
  (active `DegreeProgramme` for that faculty) with an in-grid **Select** button column.
- A requirements panel read from `ProgrammeRequirement` for the selected programme, and an
  interest-alignment strip (alignment % and strongest categories, calculated from UI 4).
- No free text, so no validation dialog. **Check eligibility** runs Sequence 1 and stays
  active for other programmes. Preferred faculty and selected programme are saved on the session.

## UI 6 — Eligibility results (standard, step 5) — outcome screen
- A learner context strip, an **outcome band** with a status chip, a requirement grid, and four
  running-figure tiles.
- The band **always states why** (never the label alone). Green = requirement met, amber = gap.
  Every state has its own words and marker. The advisory disclaimer
  "Preliminary guidance only — not an admission decision" is shown here, and requirement data
  is labelled as locally stored sample data.
- Buttons: **Back**, **Funding options** (→ 6.2b), **Alternative programmes** (→ 6.1a, runs
  Sequence 2), **Continue to summary** (→ UI 7). None of them changes the stored outcome.

## UI 6.1a — Alternative programmes (standard)
- A learner context strip, a blue advisory panel, and a ranked grid (rank, programme, outcome
  chip, fit %) of **active** programmes only. Selecting a row rewrites the **reasons panel**, and
  every recommendation states its reason.
- An **"Include these alternatives in my summary"** tick box, **unticked by default**, saved to
  `AdvisorySession.AlternativesInSummary`.
- Buttons: Back, **Compare with my selection**, **Continue to summary**. There is no validation.

## UI 6.2b — Funding options (standard)
- Filters: Faculty (defaults to the UI 5 faculty) and Funding type (the `FUNDING_TYPE_n`
  parameters), plus an "open options only" tick box.
- The grid of **active** funding linked to the faculty or programme has a Status chip, derived
  by comparing `ClosingDate` with today. Selecting a row fills a read-only panel with
  eligibility, required documents and contact details. Funding data is labelled as sample data
  to be checked against the official source.
- Buttons: **Back** (→ UI 6) and **Add to Summary**, which sets `FundingInfoRequested = 1` and
  carries funding into UI 7.

## UI 7 — Advisory summary and export (wide, step 6)
- **Export format\*** (PDF / Word) and **Save to\*** folder (TextBox + Browse). Both are required.
- The preview pane is assembled from the stored session (FP-LSR). The file name is proposed as
  `<Surname>_<SessionNumber>`. Generation date and time are stamped at export, the page count
  is derived, and the disclaimer is printed automatically.
- The document has no section tick boxes. Alternatives print only if ticked on 6.1a, funding
  only if added on 6.2b, and everything else always.
- **Save and export**: ask for confirmation, write the file, set `SessionStatus = 'Completed'`
  and `DateCompleted`, and write an `ExportReport` row (`FP-LSR`, `GeneratedByUserID = null`).
  An unwritable folder raises a dialog naming the folder, not a technical error. Success
  shows in the status band.

## UI 10.2b — Advisory session tab (wide, Career Advisor)
- Search on session number, surname, date range and outcome. **None are required.**
- A read-only DataGridView joins `AdvisorySession`, `Learner`, `DegreeProgramme` and the selected
  `RecommendationResult`. The outcome is **read back, never recalculated**, and "Exported"
  means an `ExportReport` row exists.
- **Find**: an invalid session number, or a from-date after the to-date, turns red. The status
  band turns amber when nothing matches and blue when results show.

## UI 11 — Report selection and generation (standard, Advisor + Admin)
- A RadioButton group of FP-LSR / FP-DIE / FP-LER. **Sessions from\*** and **Sessions to\***
  (DateTimePickers), Faculty, Grade and Advisory outcome (bound ComboBoxes), an "Include
  inactive programmes" tick box, **Output format\***, and **Save to\*** + Browse.
- **Parameters that don't apply to the chosen report are disabled, with the reason stated,
  never hidden.** For example, the Advisory outcome filter is disabled for FP-LER, which
  filters on outcome automatically. "Include inactive programmes" is never available for
  learner-facing output.
- A readiness panel shows the report, sessions in scope, faculties, programmes and last
  generated (from `ExportReport`), and turns green when everything is valid.
- Validation on **Generate report**: a missing required field, or from-date after to-date,
  turns red. Buttons: Cancel, Preview, Generate report.
- **The BLL checks the role before generating** (Section 8).

## UI 13.2 — Reference data, Degree requirements tab (wide, Administrator)
- Scope: a Faculty ComboBox above a Degree programme ListBox. Inactive programmes stay listed
  for administrators.
- A read-only grid of stored requirements (Requirement type · Subject · Min. mark · Accepted
  alternative · Status chip, green Active / grey Inactive) with a count line.
- Form: **Requirement type\***, **Subject\***, **Accepted alternative\*** (ComboBoxes),
  **Min. mark %\*** (NumericUpDown, 0–100), an Active tick box, and a multi-line **Advisory
  note shown to the learner**.
- Validation on **Update requirement** / **Add requirement**. Buttons: Cancel, Deactivate
  (with confirmation), Update requirement, Add requirement. The status band says "Changes apply
  to new advisory sessions only".
- **Every save calls a BLL method that checks `Role = Administrator` before running.**

## Screens built to the pattern, without a spec wireframe
- **UI 1**: two entry buttons, **Start advisory session** and **Staff login**.
- **UI 9 / UI 12**: hub dashboards with buttons to UI 10 / UI 11, and to UI 13 / UI 11.
- **UI 13.1, 13.3, 13.4, 13.5, 13.6**: the same layout as 13.2 (scope list, read-only grid,
  maintenance form, Deactivate / Update / Add). 13.6 maintains `UserAccount`: username, full
  name, role, active, and a password reset that stores a new salted hash.
- **UI 10.1a / 10.3c**: see the tab table in `screens-inventory-and-flow.md`.
