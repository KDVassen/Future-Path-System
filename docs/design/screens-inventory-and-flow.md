# Future Path — screen inventory and navigation (spec Section 2.1)

The flow diagram is `docs/figures/fig-2-1-interface-flow.png`, and its source is
`interface-flow-v14.dot`. **Do not renumber screens.** Other sections refer to these IDs.

## Inventory

| UI # | Screen | Lane | Detailed in spec 2.3 |
|---|---|---|---|
| 1 | Future Path Homepage | shared entry | — |
| 2 | Learner Profile Capture | Learner, step 1 of 6 | ✔ 2.3.2 |
| 2.1a | Invalid Profile Data | modal validation dialog | — |
| 3 | Subjects and Marks Capture | Learner, step 2 | ✔ 2.3.3 |
| 3.1a | Invalid / Duplicate Subjects | modal validation dialog | — |
| 4 | Interest Questionnaire | Learner, step 3 | ✔ 2.3.4 |
| 5 | Faculty & Department Selection | Learner, step 4 | ✔ 2.3.5 |
| 6 | Eligibility Results | Learner, step 5 | ✔ 2.3.6 |
| 6.1a | Alternative Programme Recommendations | Learner | ✔ 2.3.7 |
| 6.2b | Funding Options | Learner | ✔ 2.3.8 |
| 7 | Advisory Summary | Learner, step 6 | ✔ 2.3.9 |
| 8 | Staff Login | shared gate | ✔ 2.3.1 |
| 8.1a | Invalid Login | modal validation dialog | — |
| 9 | Advisor Dashboard | Career Advisor | — |
| 10 | Sessions Dashboard (three tabs) | Career Advisor | — |
| 10.1a | Learner Profile tab | Career Advisor | — |
| 10.2b | Advisory Session tab | Career Advisor | ✔ 2.3.10 |
| 10.3c | Advisory Reports tab | Career Advisor | — |
| 11 | Reports Selection & Generation | Advisor + Admin | ✔ 2.3.12 |
| 11.1a / 11.2b / 11.3c | FP-LSR / FP-DIE / FP-LER in the report viewer | Advisor + Admin | Section 6 |
| 12 | Administrator Dashboard | Administrator | — |
| 13 | Reference Data (TabControl) | Administrator | — |
| 13.1 | University Information (faculties and programmes) tab | Administrator | — |
| 13.2 | Degree Requirements tab | Administrator | ✔ 2.3.11 |
| 13.3 | Funding Information tab | Administrator | — |
| 13.4 | Interest Categories tab | Administrator | — |
| 13.5 | Questionnaire Questions tab | Administrator | — |
| **13.6** | **User Accounts tab** — added 29 Sep (`decisions.md` D5) | Administrator | — |

Screens without a spec wireframe are built to the same standard (`screens-standards.md`),
using the archetype of their nearest drawn sibling. UI 13.1 and 13.3–13.6 follow 13.2's
maintenance-tab pattern. UI 10.1a and 10.3c follow 10.2b's search-and-grid pattern.

## Navigation

- **UI 1 splits into two areas**: the Learner Advisory System (no login) and the staff area
  behind UI 8. UI 8 branches by `UserAccount.Role` into UI 9 (Career Advisor) or UI 12
  (Administrator). There is one login form, not two.
- **The learner lane runs UI 2 → 3 → 4 → 5 → 6 → 7** in a fixed order ("Step n of 6", where
  Home is a rail item, not a step). UI 6 branches to 6.1a (**Alternative programmes**) and
  6.2b (**Funding options**). Both carry forward into UI 7: 6.1a by its tick box,
  6.2b by **Add to Summary**. The case study asks whether the learner wants funding options
  after the outcome is shown (§6.7), and the **Funding options** button is that choice.
- **Validation dialogs 2.1a, 3.1a and 8.1a are modals** raised by the screen that owns the data.
  Their single **Re-attempt correction** button returns focus to the first offending control,
  and nothing the user typed is lost.
- **UI 5 and UI 4 have no validation dialog.** They are selection-only.
- **UI 10 is a hub, not a sequence.** The advisor searches once and moves between the three
  tabs. UI 10.3c's **Generate report** opens UI 11 already filtered.
- **UI 13.1–13.6 are TabPages inside UI 13.** The navigation rail is built at runtime from the
  role, so a Career Advisor's rail has no Reference data item.
- **Learners have no login**, and nothing in the learner lane shows another learner's data.
- **No dead ends.** Every screen offers Back, Cancel or a rail destination, and Back keeps
  captured data.

## UI 10 tabs (design notes; consistent with 2.3.10)

The tab strip is at the top. Each tab has its own search criteria, results grid and export.
The whole screen is read-only apart from the search fields.

| Tab | Searches on | Grid | Primary action |
|---|---|---|---|
| 10.1a Learner profile | surname, first name, grade, school, province | learner, grade, school, province, sessions, last session | Open learner profile |
| 10.2b Advisory session | session number, surname, date range, outcome | session, date, learner, grade, programme, outcome, exported | Open / export advisory summary (FP-LSR) |
| 10.3c Advisory reports | date range, faculty, grade, outcome | report, scope, rows in scope, last generated | Generate report (opens UI 11 filtered) |
