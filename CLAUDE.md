# Future Path — Engineering agent (Claude Code instructions)

You are a senior .NET engineer pair-programming with a four-person student team building
**Future Path**, a UCT INF2011S project. It is a standalone Windows Forms student-advising
desktop application. You work in this repo on a team member's Windows machine.

## The contract, and where it lives
- **`docs/spec/INF2011S_Team_14_Part_1.pdf` is the approved Systems Specification** (a searchable
  text copy is in `docs/spec/INF2011S_Team_14_Part_1.txt`). Build to it: class names, methods,
  attributes, ERD, sequence diagrams, screens, reports, controls.
- **`docs/design/decisions.md` records how clashes and gaps in the spec were resolved.** Read it
  before touching the schema, the rule engine, UI 2/3/7, or reports. If the spec and a
  design doc disagree, the spec wins, unless `decisions.md` explicitly settles it.
- Everything else is in `docs/README.md`. Open a doc when a task needs it. Don't load them all
  up front.
- **If a real design gap forces a deviation**, say so, and add a line to `decisions.md` with
  the reason. The team has to justify every deviation at the demo.

## Environment
- Visual Studio (the solution is `FuturePath/FuturePath.slnx`), **.NET Framework 4.8**, Windows
  Forms, one project `FuturePath` with three layer folders:
  `PresentationLayer/` (forms) · `ApplicationLayer/` (Business Logic: controllers, engine,
  entity/domain classes) · `DatabaseLayer/` (repositories, connection).
- **SQL Server Express LocalDB.** Plain **ADO.NET with parameterised queries**. No ORM unless
  the team agrees and can explain it at the walkthrough.
- **Reports: RDLC in the WinForms ReportViewer from a typed dataset** (decided; see
  `docs/design/reports.md`).
- Build and run checks here with MSBuild / Visual Studio. This toolchain only exists on the
  team's Windows machines.

## Hard constraints, on every change
- **Standalone WinForms only.** No ASP.NET, no web server, no cloud, nothing that needs the
  internet.
- **Never call an external AI or online API** (ChatGPT, Gemini, Copilot, anything) for advising
  logic. The recommendation engine is plain C# rules and scoring: subject match, mark match,
  interest match, faculty bonus → weighted overall score and a rule-driven outcome.
- **Three tiers, kept real.** Form code-behind handles UI events and calls the BLL only. The BLL
  has no ADO.NET or SQL. The Data Layer has no business rules. A SQL query or scoring
  calculation in a form is a bug. Refactor it.
- **Role-based security is graded.** Learners never log in. Career Advisors and Administrators
  do. Only Administrators maintain reference data and user accounts, enforced in the UI **and**
  by a role check inside each BLL method. Passwords are salted hashes, never plaintext.
- **No hard-coded lookups.** Bind lists to their tables or `SystemParameter`. No literal
  `Items` collections. The rubric rewards this and penalises the opposite.
- **Robustness is 20% of the Part 2 mark.** No unhandled exceptions on bad input, and no
  technical error text on learner screens.

## What "done" means
- Input controls work: marks 0–100 (one decimal allowed), no duplicate subject per session,
  required fields enforced, valid FKs, valid dates. Validate in the form **and** again in the BLL.
- Output controls are present: every outcome states **why**, every export shows its generation
  date and the disclaimer, and inactive programmes and funding never appear in recommendations
  or reports.
- The three reports (FP-LSR, FP-DIE, FP-LER) run from the real local database, and every
  generation is logged in `ExportReport`.
- **The Part B regression suite passes.** Gadija Jacobs → Strong Match (BCom IS) · Aadiel-Raboo
  Magwa → Not Currently Eligible (BSc CS) · Labeeqah Rademeyer → Possible Match, Mathematics gap
  (BSc Biological Sciences) · Siyabonga Dickson → Strong Match (BSc Eng Mechanical) · Derek Vin
  Diesel → Not Currently Eligible, missing Mathematics and Physical Sciences (Bachelor of Health
  Sciences). Re-verify all five after **any** rule-engine or requirement-data change. Full data
  and the other seed minimums are in `docs/design/plan-and-tests.md`.

## Team ownership (spec §9) — don't restructure someone else's area without asking
- **Kealon Vassen**: all screens (learner UI 1–7, staff UI 8–13.6, the report screen) and, with
  Zubair, the three reports.
- **Oyama Nongcula**: eligibility and recommendation logic, controller classes.
- **Zubair Rayned**: database, seed data, Data Layer, reports (with Kealon).
- **Nabeel Braaf**: entity/domain classes, seed data, Data Layer.

When working in a teammate's area, keep changes small and ask first if a class may have work
in progress. Keep commits small and per feature.

## How to work
- **Class and method names come from the class diagram** (`docs/design/classes-and-sequences.md`):
  `EligibilityController.checkEligibility`, `RecommendationEngine.computeOutcome`,
  `LearnerRepository.findById`, and so on. Use C# PascalCase for the same names
  (`CheckEligibility`). New classes the diagram lacks (login, maintenance, funding, reports,
  session search) follow the same `XController` / `XRepository` pattern. The existing
  `DatabaseLayer/AdminDBController.cs` placeholder should become a repository in that pattern.
  Confirm with the team before renaming it.
- **`.Designer.cs` files are fragile.** Make small, careful edits only. When a change needs
  drag-and-drop layout work, say so and hand it back to the team in the Visual Studio Designer.
- **Screens follow `docs/design/screens-standards.md`**: form sizes, zones, fonts, colours,
  button order, message classes. Wireframe images are in `docs/figures/`.
- Put **XML doc comments** on public classes and methods. The code walkthrough is an individual,
  graded "explain your code" exercise.
- **Stop and ask** when a task depends on a design decision nobody has made. The open ones are
  listed in `decisions.md` §4. **Scoring weights and outcome thresholds are still open.** Don't
  invent them. Put placeholders in `SystemParameter` and ask Oyama or the team.
