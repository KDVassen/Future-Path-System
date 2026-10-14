# Future Path — project documents

**Order of authority:** `spec/` (the submitted specification) → `design/decisions.md` (how
clashes and gaps were settled) → the other `design/` notes → `brief/` (the case study that
both are built on).

| Folder / file | What it is | Open it when |
|---|---|---|
| `spec/INF2011S_Team_14_Part_1.pdf` (+ `.txt`) | The submitted Part 1 Systems Specification, Team 14 | Always the final word. Search the `.txt`. |
| `design/decisions.md` | Clashes between spec and design notes, how each was resolved, what is still open | Before schema, rule-engine, UI 2/3/7 or report work |
| `design/classes-and-sequences.md` | Class diagram and both sequence diagrams as text, plus the class ↔ table mapping | Writing controllers, entities, repositories, the engine |
| `design/application-layer.md` | How the BLL and Data Layer are built: Waitering pattern, conventions, outcome rules, database build, tests | Changing or calling any controller, repository or entity |
| `design/database.md` | All 19 tables, constraints, SystemParameter keys, seed minimums | Creating or changing the database or queries |
| `design/screens-inventory-and-flow.md` | Screen IDs, navigation, UI 10 tabs | Building navigation or a new screen |
| `design/screens-standards.md` | Form sizes, zones, fonts, colours, buttons, messages | Building any form |
| `design/screens-detail.md` | Field-by-field behaviour of each screen | Building a specific screen |
| `design/reports.md` | FP-LSR / FP-DIE / FP-LER content, selection, sorting, export, RDLC | Report work |
| `design/controls-and-security.md` | Input/output controls, validation rules, the role matrix | Validation, login, access checks |
| `design/plan-and-tests.md` | Who builds what by when, Part B regression data, TC01–TC05 | Seeding data, testing, planning |
| `design/interface-flow-v14.dot` | Graphviz source of Figure 2-1 | Only if the flow diagram is redrawn |
| `figures/` | Diagrams, wireframes and report layouts cut from the submitted spec | You need to see a screen or diagram |
| `brief/` | The case study (+ `.txt`) and the course's overall guidelines (Part 2 marking) | Checking an original requirement |

## figures/
`fig-1-architecture.png` · `fig-2-1-interface-flow.png` · `fig-3-1-sequence-eligibility.png` ·
`fig-3-2-sequence-alternatives.png` · `fig-4-class-diagram.png` · `fig-5-erd.png` ·
`ui-08-staff-login.png` · `ui-02-learner-profile.png` · `ui-03-subjects-marks.png` ·
`ui-04-interest-questionnaire.png` · `ui-05-faculty-selection.png` ·
`ui-06-eligibility-results.png` · `ui-06-1a-alternatives.png` · `ui-06-2b-funding.png` ·
`ui-07-advisory-summary.png` · `ui-10-2b-advisory-session-tab.png` ·
`ui-13-2-degree-requirements.png` · `ui-11-report-generation.png` · `rpt-*.png` (report
layouts as submitted — build with the FP-LSR / FP-DIE / FP-LER IDs even where an image still
shows an old one).
