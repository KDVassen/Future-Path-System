# Future Path — reports (spec Section 6)

The three reports are **FP-LSR**, **FP-DIE** and **FP-LER**. Layout images are
`docs/figures/rpt-*.png`. Some submitted layout images still print the old IDs
(e.g. "FP-R1"). **Build with the FP- IDs.**

## Technology
- **RDLC in the WinForms ReportViewer**, rendered from a **typed dataset** that the Business
  Logic Layer fills. The `.rdlc` holds no queries and no business rules, which keeps the three
  tiers separate on the printed page. Package: `Microsoft.ReportingServices.ReportViewerControl.Winforms`
  (it supports .NET Framework 4.8 and exports to PDF, Word and Excel).
- Every generation writes an **`ExportReport` row** (`ReportCode`, `SessionID` or
  `ParametersUsed`, user, timestamp, format). See `database.md`.
- Every report prints its **generation date** and the **advisory disclaimer**, and never
  includes inactive programmes or funding.

## House style (the "Two-Tone Band" direction in the submitted layouts)
One navy masthead band (report name, period and parameters, generated date and time, report
ID), with ink on white below it. **Horizontal rules only**, no vertical rules, no boxes, **no
zebra striping**. Text columns are left-aligned. Numeric columns are right-aligned. Status is
shown with a **marker character** (✓ / ! / –) as well as colour. Charts are **bar and pie
only**, each followed by a one-line plain-language note. The footer shows report name,
generation timestamp and "page n of m".

## FP-LSR — Learner Advisory Summary Report (6.1)
- **Type:** a detailed report, one per advisory session. **2 pages, A4 portrait.**
- **Who generates it:** the **learner, on UI 7**, at the end of a completed session. A Career
  Advisor or Administrator can regenerate it later (UI 10.2b / UI 11). It is never scheduled or
  batched, and never produced for an incomplete session.
- **Audience:** the learner, plus whoever they share it with (parents, teachers, advisors).
- **Content:** masthead (title, learner name, session number and date, report ID) · identity
  line (grade, school, province, subjects captured, overall average — **no contact details**)
  · the outcome statement with its reason · **Table 1** subjects and marks with the requirement
  check merged in · **Chart 1** grouped bar of the learner's mark against the stored minimum
  per subject, plus the overall average · the interest profile by category (**Chart 2**) ·
  **Table 2** alternative programmes (only if ticked on 6.1a) · **Table 3** funding options
  (only if added on 6.2b) · overall guidance · full disclaimer.
- **Sequence:** subjects by **name, ascending** · requirements in requirement order · interest
  categories by score, descending · alternatives by overall fit, descending · funding by closing
  date, ascending (nearest deadline first).
- **Summaries:** overall average against the programme's minimum overall average · count of
  requirements met · shortfall in percentage points for each unmet requirement · interest %
  per category. Fit percentages on the alternatives table are **read from the stored
  `OverallScore`, not recalculated.**
- **Export:** PDF or Word (.docx), to a folder the user chooses. The report is never emailed
  or transmitted.
- **Privacy:** a learner can only reach the summary of the session in progress. There is no
  screen for opening another learner's report. Staff access needs a login and the right role.

## FP-DIE — Degree Interest and Eligibility Report (6.2)
- **Type:** a summary report across many sessions, generated once per requested period. **3
  pages, A4 landscape.** On demand only.
- **Audience:** Career Advisors and Administrators only. It contains **no learner names or
  profile data**.
- **Selection:** start and end date (required). Optional faculty, grade and outcome filters.
  **Completed** sessions in the period only. Inactive programmes and funding are excluded by
  default. Every parameter used is printed on page 1.
- **Content:** page 1 has the masthead, parameter line, a strip of six key figures (sessions,
  learners, and the count and % of each outcome, as in the 6.2.2 layout), §1 advisory outcomes by faculty (four
  outcome counts, eligibility rate, totals row) and §2 a pie of the four outcomes for the whole
  period. Page 2 has the degree programme table (selections and four outcome counts, totals row)
  and **five faculty pies** as small multiples (shared legend, fixed slice order, same order as
  the faculty table). Page 3 has a bar of required subjects not taken, a table of subjects taken
  but below the minimum with the average shortfall, a funding categories table, and the
  disclaimer. Sections are numbered 1–8 continuously.
- **Eligibility rate** = (Strong + Possible) ÷ sessions in the group × 100.
- **Sequence:** the faculty table by sessions, descending · the programme table by selections,
  descending · gap bar, shortfall table and funding table by frequency, descending.
- **Grouping:** by faculty, then programme. The subject-gap totals deliberately add up to more
  than the session count, and a note beneath says so. Gaps are **calculated at report time**
  (`LearnerSubject` joined with `ProgrammeRequirement`).
- **Export:** PDF or **Excel** (the one report of figures people may want to chart or compare).

## FP-LER — Learners Not Currently Eligible (6.3) — exception report
- **Type:** exception report. **2 pages, A4 landscape.** On demand, usually at the start of a
  follow-up cycle.
- **Audience:** Career Advisors (their follow-up worklist) and Administrators. **Never** given
  to a learner. A restriction line on page 1 says it is not for general circulation.
- **Selection:** start and end date. Optional faculty and grade. **Always and only** sessions
  whose outcome is **Not Currently Eligible** or **More Information Needed**.
- **Content:** page 1 has the masthead, a fact line (sessions in scope, number and % of
  exceptions, the split between the two outcomes, learners affected), then a **bar chart of
  reasons** and a **pie of exceptions by faculty** side by side, then the listing. The listing
  columns are session number · date · learner name · grade · school · qualification selected ·
  outcome · **reason** · recommended alternative. Page 2 continues the listing and ends with the
  report total row and the disclaimer.
- **Sequence:** faculty first, then outcome (Not Currently Eligible before More Information
  Needed), then session date, newest first. Charts are sorted by frequency, descending.
- **Grouping:** by faculty. Each group heading gives its count and outcome split, and a report
  total row closes the listing.
- **Export:** PDF or Word (`decisions.md` §3).
- **Disclaimer:** Not Currently Eligible is not an admission decision, not a refusal, and not a
  statement about any other qualification. More Information Needed only means the session was
  incomplete. Grade 11 marks are projected and will change.

## Scope decision to be ready to defend
FP-LER has **no follow-up owner or status column**. An exception report often carries one, but
it would need schema and screen work that the spec doesn't include.
