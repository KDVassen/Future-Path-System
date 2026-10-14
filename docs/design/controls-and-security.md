# Future Path — input/output controls and security (spec Sections 7 and 8)

## Input formats (7 — formalised inputs)

| Data | Convention |
|---|---|
| Learner grade | Chosen from a fixed list (Grade 11 / Grade 12 → `GradeLevel`), never free text |
| School subjects | Chosen from the stored `Subject` list |
| Marks | Numeric percentage, whole or one decimal, one per subject |
| Faculty / degree programme | Chosen from stored `Faculty` / `DegreeProgramme` lists |
| Interest questionnaire | A fixed set of stored questions on a 1–5 response scale |
| Funding preference | A yes/no choice to view funding (the **Funding options** button on UI 6) |
| Name, school, province, contact | Names and contact details are optional. School and province are required on UI 2 (school is free text, province is value-bound). |

## Validation — applied twice (Presentation for feedback, BLL so it can't be bypassed)
- Marks between **0 and 100**. Boundary tests: −1 ✗, 0 ✓, 100 ✓, 101 ✗, letters ✗.
- Required fields can't be blank: grade, school, province, advising year, at least one subject
  and mark, and a selected faculty and programme before the eligibility check.
- **The same subject can't be captured twice in one session.** The BLL checks existing
  `LearnerSubject` rows for the session, backed by `UNIQUE(SessionID, SubjectID)`.
- Dates come from DateTimePickers only (e.g. funding closing date). From-date ≤ to-date.
- A programme must belong to an existing faculty (FK, and only existing faculties are offered).
- Funding type comes only from the predefined set (`FUNDING_TYPE_n`).

## Input integrity controls
- **Value limits** on numeric fields.
- **Data validation**: formats checked before accepting, and reference values picked from lists.
- **Completeness**: the learner can't reach the eligibility check, and an administrator can't
  save a record, while mandatory fields are empty.
- **Field combination**: no second entry for a subject in the same session.
- **Referential integrity**: every FK is enforced in the database.
- **Confirmation**: before submitting a session, exporting a report, or deactivating a degree
  or funding option.

## Output integrity controls
- **Accuracy**: every outcome is shown with its specific reason (which subject or mark
  requirement wasn't met), never the label alone.
- **Currency**: recommendation and report queries **exclude inactive** programmes and funding.
- **Proper destination**: a learner sees only their own session's FP-LSR. FP-DIE and FP-LER
  are for Career Advisors and Administrators only, enforced by the menu **and** by a role check
  in the BLL before generating.
- **Completeness and source labelling**: the standard advisory disclaimer appears on screen and
  on every export. Every export shows its generation date. Funding and requirement data is
  marked as **locally stored sample data** to be checked against the official source.

## Roles (8)

| Capability | Learner | Career Advisor | Administrator |
|---|---|---|---|
| Complete an advisory session (subjects, interests, eligibility, funding view, export) | Yes (no login) | — | — |
| View advisory sessions and outcomes across learners | — | Yes | Yes |
| Generate FP-DIE and FP-LER (and regenerate FP-LSR) | — | Yes | Yes |
| Maintain faculties, programmes, requirements | — | No | Yes |
| Maintain funding options | — | No | Yes |
| Maintain interest categories and questions | — | No | Yes |
| Maintain user accounts (UI 13.6) | — | No | Yes |

- **Interface**: the menu and rail are built from the session type. Administrator-only forms
  are never rendered or enabled for a Career Advisor.
- **Business Logic Layer**: every method that changes reference data or user accounts checks
  `Role == Administrator` itself before running.
- **Data**: `UserAccount` holds username, **salted password hash** (e.g. PBKDF2 via
  `Rfc2898DeriveBytes`, never plaintext), role and active status. It is the single source of
  truth for login. Seed at least one active Administrator and one active Career Advisor.
