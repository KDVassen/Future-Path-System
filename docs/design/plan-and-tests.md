# Future Path — implementation plan and test plan (spec Sections 9 and 10)

Part 2 (the working system) is due **16 October 2026**. The demonstration and the individual
code walkthrough are in the **week of 19 October**.

## Team and stages (spec §9)

| # | Activity | Owner | Dates (2026) |
|---|---|---|---|
| 1 | Create the database (tables, keys, relationships from the ERD) | Zubair Rayned | 22–24 Sep |
| 2 | Build the entity/domain classes (class diagram) | Nabeel Braaf | 22–24 Sep |
| 3 | Learner screens UI 2–7 and 6.1a/6.2b | **Kealon Vassen** | 22–29 Sep |
| 4 | Seed Part B and extra test data | Zubair Rayned & Nabeel Braaf | 25–27 Sep |
| 5 | Eligibility and recommendation logic (rules, interest alignment, gaps, outcomes, ranking) | Oyama Nongcula | 25 Sep – 2 Oct |
| 6 | Data Layer classes (repositories) | Zubair Rayned & Nabeel Braaf | 28 Sep – 2 Oct |
| 7 | Staff login, Career Advisor screens, Administrator maintenance (incl. user accounts) and the report generation screen | **Kealon Vassen** | 30 Sep – 5 Oct |
| 8 | Controller classes (requests, processing, staff access) | Oyama Nongcula | 3–5 Oct |
| 9 | Integrate the three tiers | Whole team | 6–9 Oct |
| 10 | Build and connect the three reports | **Kealon Vassen** & Zubair Rayned | 10–11 Oct |
| — | Test during development | Whole team | 25 Sep – 11 Oct |
| 11 | Full system testing (valid, invalid, boundary, exception, Part B) | Whole team | 12–14 Oct |
| 12 | Final fixes and submission checks | Whole team | 15–16 Oct |
| 13 | Final submission | Whole team | 16 Oct |
| 14 | Prepare the demonstration | Whole team | week of 19 Oct |

The rule engine is developed and tested against the domain classes and Part B data while the
Data Layer is still being built, and connected to the database during integration.

## Test environment
Windows, Visual Studio, C# on .NET Framework 4.8, Windows Forms, SQL Server Express LocalDB,
three tiers, fully offline.

## Test approach
**Unit** (a mark range check, an interest score, a requirement check), **integration** (form →
BLL → DAL → database), **system** (end to end, all roles and reports). Scenario types:
**valid** (mark 75), **invalid** (letters for a mark, a duplicate subject), **boundary**
(−1, 0, 100, 101), **exception** (missing information → More Information Needed, never a
crash). When a test fails, log it, fix it, re-test, and re-run related tests that passed before.

## Part B — the regression suite
**Every change to the rule engine must still reproduce all five.**

| Learner | Grade | Subjects and marks | Programme | Expected outcome |
|---|---|---|---|---|
| Gadija Jacobs | 12 | English 72, Mathematics 70, Accounting 74, Business Studies 79, Economics 70, Life Orientation 80 | BCom Information Systems | Strong Match |
| Aadiel-Raboo Magwa | 12 | English 65, Mathematical Literacy 78, Business Studies 71, History 69, Tourism 75, Life Orientation 82 | BSc Computer Science | Not Currently Eligible |
| Labeeqah Rademeyer | 11 | English 80, Mathematics 58, Physical Sciences 62, Life Sciences 70, Geography 66, Life Orientation 85 | BSc Biological Sciences | Possible Match — Mathematics is the gap |
| Siyabonga Dickson | 12 | English 70, Mathematics 75, Physical Sciences 78, Engineering Graphics and Design 73, Life Orientation 80 | BSc Engineering (Mechanical Engineering) | Strong Match |
| Derek Vin Diesel | 12 | English 82, Mathematical Literacy 76, History 79, Life Sciences 68, Visual Arts 85, Life Orientation 90 | Bachelor of Health Sciences | Not Currently Eligible — Mathematics and Physical Sciences required |

**Requirement data must also satisfy (case study Part B §6):** at least 3 programmes require
Mathematics · at least 2 require Physical Sciences · at least 2 accept Mathematical Literacy ·
BSc Computer Science must **not** accept Mathematical Literacy for Mathematics · BSc Biological
Sciences' Mathematics minimum sits just above 58 (60 in the design data) · Bachelor of Health
Sciences requires Mathematics **and** Physical Sciences · every programme has at least one
requirement and at least one interest-category mapping.

**Seed sessions (at least 10) must show:** a strong match · a missing required subject · a mark
below the minimum · interests that align with a different faculty · a funding request · an
exported report · a More Information Needed outcome.

**Funding (at least 8):** at least one linked to each of Commerce, Science, Engineering and
Health Sciences, at least one need-based and at least one merit-based.

**Interest questions (at least 8)** — the case study examples: numbers/data · helping people ·
technology · science experiments · reading/writing/analysing · business/finance ·
designing/building · society/law/politics.

## Test cases for the demonstration (spec §10)

| ID | Scenario | Data | Expected |
|---|---|---|---|
| TC01 | Create a learner profile and capture subjects and marks | Gadija Jacobs, Grade 12, her six marks | Profile, subjects and marks saved. All marks are within 0–100. |
| TC02 | Check eligibility for a selected degree | Gadija → BCom Information Systems | Strong Match, with its reason |
| TC03 | Generate alternative recommendations | Aadiel-Raboo Magwa → BSc Computer Science | Not Currently Eligible, the unmet requirement explained, and suitable alternatives shown |
| TC04 | View funding information | Gadija → BCom IS | Active funding linked to Commerce or the programme is shown |
| TC05 | Export the learner advisory summary | Labeeqah → BSc Biological Sciences | FP-LSR exported with learner info, subjects and marks, programme, interest result, outcome, gaps, alternatives if chosen, funding, date generated and disclaimer |

## Seed data as built

`FuturePath/Database/SeedData.sql` builds this data. `BuildDatabase.ps1` runs it after
`CreateSchema.sql`, and `tests/Seed-Check.ps1` checks every rule below. Every ID is fixed with
`IDENTITY_INSERT`, so tests can refer to it.

**The scoring is provisional** (`decisions.md` D14: weights 0.30 / 0.30 / 0.30 / 0.10,
`THRESHOLD_STRONG` 75, `INTEREST_STRONG_MIN` 60, `MARK_TOLERANCE` 2). **The five Part B outcomes
depend on the requirement rows, the interest mappings and the invented interest answers below.**
After changing any of them, or a scoring parameter, run `Seed-Check.ps1` and `PartB-Regression.ps1`.

All programme names and rules are sample data, not official university requirements. The funding
options are locally stored sample data. Every person, school and funding name outside Part B is
fictional, and the contact details use the reserved `.example` domain and `555` numbers.

### IDs the tests rely on

| What | IDs |
|---|---|
| Part B learners = their sessions | 1 Gadija Jacobs · 2 Aadiel-Raboo Magwa · 3 Labeeqah Rademeyer · 4 Siyabonga Dickson · 5 Derek Vin Diesel (`SessionID = LearnerID` for every seeded session; one session per learner) |
| Part B programmes | 1 BCom Information Systems · 3 BSc Computer Science · 4 BSc Biological Sciences · 5 BSc Engineering (Mechanical Engineering) · 7 Bachelor of Health Sciences |
| Inactive rows | Programme 11 (Commerce) · requirement 4 (BCom IS, Information Technology) · interest category 9 · question 13 · funding 9 (linked to Commerce and BCom IS) |
| Closed funding | Funding 10: active, closed 2026-09-30, linked to Commerce, Science and BCom IS |
| Faculties | 1 Commerce · 2 Science · 3 Engineering and the Built Environment · 4 Health Sciences · 5 Humanities |
| Grades | `GradeLevelID` 1 = Grade 11 (Projected), 2 = Grade 12 (Final) |
| Subjects | 1 English Home Language · 2 Afrikaans First Additional Language · 3 Mathematics · 4 Mathematical Literacy · 5 Life Orientation · 6 Physical Sciences · 7 Life Sciences · 8 Accounting · 9 Business Studies · 10 Economics · 11 Geography · 12 History · 13 Information Technology · 14 Engineering Graphics and Design · 15 Tourism · 16 Visual Arts |
| Staff | UserID 1 `admin` (Administrator, active) · 2 `advisor` (Career Advisor, active) · 3 `former.advisor` (Career Advisor, **inactive**) · 4 `senior.advisor` (Career Advisor, active) · 5 `data.admin` (Administrator, active). The test passwords are only in the header comment of `SeedData.sql`; the tests read them from there. |

Part B's "English" is English Home Language. The case study's subject list names Computer
Applications Technology; the seed uses Information Technology instead, as the seed brief asked.
The team should confirm this, or add Computer Applications Technology as subject 17.

### Programmes and requirements

Mathematics and Physical Sciences requirements have **no** alternative unless one is shown.
"Min avg" is `MinimumOverallAverage`, taken over every captured subject.

| ID | Programme | Faculty | Min avg | Required (minimum %) | Recommended |
|---|---|---|---|---|---|
| 1 | BCom Information Systems | Commerce | 60 | English HL 50 · Mathematics 60 · *(inactive: Information Technology 50)* | Accounting 50 |
| 2 | BCom Accounting | Commerce | 60 | English HL 50 · Mathematics 60 | Accounting 60 |
| 3 | BSc Computer Science | Science | 65 | English HL 50 · Mathematics 70 (Mathematical Literacy **not** accepted) | Information Technology 60 |
| 4 | BSc Biological Sciences | Science | 60 | English HL 50 · Mathematics 60 · Physical Sciences 50 | Life Sciences 60 |
| 5 | BSc Engineering (Mechanical Engineering) | Engineering | 65 | English HL 50 · Mathematics 70 · Physical Sciences 70 | Engineering Graphics and Design 60 |
| 6 | BSc Engineering (Civil Engineering) | Engineering | 65 | English HL 50 · Mathematics 70 · Physical Sciences 65 | Geography 60 |
| 7 | Bachelor of Health Sciences | Health Sciences | 65 | English HL 50 · Mathematics 60 · Physical Sciences 60 | Life Sciences 60 |
| 8 | BSc Physiotherapy | Health Sciences | 60 | English HL 50 · Mathematics 50 · Life Sciences 60 | Physical Sciences 50 |
| 9 | Bachelor of Social Science | Humanities | 55 | English HL 60 · Mathematics 50, **or Mathematical Literacy 60** | History 60 |
| 10 | Bachelor of Arts | Humanities | none | English HL 60 · Mathematics 50, **or Mathematical Literacy 60** | Afrikaans FAL 50 |
| 11 | BCom Tourism Management (**inactive**) | Commerce | 50 | English HL 50 · Mathematics 50, or Mathematical Literacy 50 | Tourism 60 |

Case-study checks: every programme requires Mathematics (10 of them are active). Four require
Physical Sciences (4, 5, 6, 7). Two active programmes accept Mathematical Literacy (9, 10), and
BSc CS does not. Every programme has at least one requirement and two to four interest mappings.
Advisory notes are on requirements 2, 3, 4, 7, 9, 12, 18, 25, 32, 35 and 36.

Requirement 4 is inactive on purpose. If it were active, Gadija would have to take Information
Technology, so her Strong Match proves inactive requirements are ignored. Programme 11 would suit
Aadiel (Mathematical Literacy, Tourism), so TC03 proves inactive programmes never become
alternatives.

### Interest categories, questions and mappings

| Category | Questions (DisplayOrder = QuestionID) |
|---|---|
| 1 Numbers and data | Q1 I enjoy solving problems using numbers or data. |
| 2 Helping people and communities | Q2 I enjoy helping people and communities. · Q11 I would like a career caring for people's health and wellbeing. |
| 3 Technology | Q3 I enjoy working with technology. · Q9 I like finding out how computers, apps and websites work. |
| 4 Science experiments and investigation | Q4 I enjoy science experiments and research. · Q10 I am curious about how living things and the natural world work. |
| 5 Reading, writing and analysing ideas | Q5 I enjoy reading, writing and analysing ideas. |
| 6 Business, entrepreneurship and finance | Q6 I enjoy business, entrepreneurship and finance. |
| 7 Designing and building | Q7 I enjoy designing or building practical solutions. · Q12 I like working out how machines, buildings or structures are put together. |
| 8 Society, law and politics | Q8 I enjoy understanding society, law or politics. |
| 9 Outdoor adventure and sport (**inactive**) | Q13 I enjoy outdoor adventure and sport. (**inactive**) |

Q1–Q8 use the case study's wording. The relevance weights are:

| Programme | Categories (weight) |
|---|---|
| 1 BCom IS | Technology 1.00 · Business 0.80 · Numbers 0.70 |
| 2 BCom Accounting | Numbers 1.00 · Business 0.90 |
| 3 BSc CS | Technology 1.00 · Numbers 0.80 · Designing 0.50 |
| 4 BSc Bio Sci | Science 1.00 · Numbers 0.50 · Helping 0.40 |
| 5 BSc Eng Mech | Designing 1.00 · Numbers 0.70 · Science 0.60 |
| 6 BSc Eng Civil | Designing 1.00 · Numbers 0.70 · Society 0.40 |
| 7 BHealthSc | Helping 1.00 · Science 0.90 · Reading 0.40 |
| 8 BSc Physiotherapy | Helping 1.00 · Science 0.70 |
| 9 BSocSc | Society 1.00 · Helping 0.80 · Reading 0.70 |
| 10 BA | Reading 1.00 · Society 0.70 |
| 11 BCom Tourism (inactive) | Business 1.00 · Helping 0.60 |

### Part B learners' interest answers (invented)

The case study gives the Part B learners' subjects and marks but no questionnaire answers. A
Strong Match needs an interest score of at least `INTEREST_STRONG_MIN` (60) and an overall score
of at least 75, so the answers below are invented. Gadija and Siyabonga answer 4s and 5s on their
programme's categories, so each mapped category scores at least 75. The other three give varied,
plausible answers. Their outcomes are decided by the requirement rules, whatever they answer.
Answers are 1 (Strongly disagree) to 5 (Strongly agree), scored 0 / 25 / 50 / 75 / 100.

| Session | Q1 | Q2 | Q3 | Q4 | Q5 | Q6 | Q7 | Q8 | Q9 | Q10 | Q11 | Q12 | Interest score for the selected programme |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 1 Gadija | 4 | 3 | 4 | 2 | 3 | 5 | 3 | 2 | 5 | 2 | 2 | 3 | 88.0 (BCom IS) |
| 2 Aadiel | 3 | 4 | 3 | 2 | 3 | 5 | 2 | 4 | 3 | 1 | 3 | 2 | not used: NCE (BSocSc 64, BA 60.3 as alternatives) |
| 3 Labeeqah | 3 | 4 | 2 | 5 | 4 | 1 | 2 | 3 | 2 | 5 | 4 | 2 | 81.6, but the Mathematics gap decides: POSSIBLE |
| 4 Siyabonga | 4 | 3 | 4 | 4 | 2 | 2 | 5 | 2 | 4 | 4 | 2 | 5 | 85.9 (BSc Eng Mech) |
| 5 Derek | 2 | 5 | 3 | 3 | 4 | 2 | 3 | 4 | 2 | 4 | 5 | 3 | not used: NCE |

With these answers the provisional formula gives Gadija 96.4 overall and Siyabonga 95.8, both
Strong. Labeeqah's Mathematics 58 is 2 points under 60, which is within `MARK_TOLERANCE`, so she
is a Possible Match. The boundary rows in `PartB-Regression.ps1` show that 57.9 gives NCE, 58.0
gives Possible and 60 gives Strong.

### Session scenarios

Every session is seeded **In progress**, with no `RecommendationResult` and no `ExportReport` rows.
After the merge, the coordinator runs `CheckEligibility` and then `GenerateAlternatives` for
sessions 6–13. Session 12 returns no alternatives. For sessions 6–12 the coordinator then calls
`ReportController.CompleteLearnerSession`, which marks the session Completed and logs its FP-LSR
(D3: a session completes when its summary is exported). Sessions 1–5 stay In progress, so the
Part B regression can re-run them.

| Session | Learner (grade) | Date | Programme (preferred faculty) | Intended outcome and why | Flags | Complete / export |
|---|---|---|---|---|---|---|
| 1 | Gadija Jacobs (12) | 2026-09-28 | 1 BCom IS (Commerce) | STRONG: requirements met, interest 88 | | No / No |
| 2 | Aadiel-Raboo Magwa (12) | 2026-09-29 | 3 BSc CS (Science) | NCE: Mathematics missing; Mathematical Literacy not accepted | | No / No |
| 3 | Labeeqah Rademeyer (11) | 2026-09-30 | 4 BSc Bio Sci (Science) | POSSIBLE: Mathematics 58 is 2 below 60 | | No / No |
| 4 | Siyabonga Dickson (12) | 2026-10-01 | 5 BSc Eng Mech (Engineering) | STRONG: requirements met, interest 85.9 | | No / No |
| 5 | Derek Vin Diesel (12) | 2026-10-02 | 7 BHealthSc (Health Sciences) | NCE: Mathematics and Physical Sciences missing | | No / No |
| 6 | Nomvula Khumalo (12) | 2026-01-20 | 2 BCom Accounting (Commerce) | STRONG: **strong match**, interest 100 | funding requested | Yes / Yes (PDF) |
| 7 | Pieter van Wyk (11) | 2026-02-17 | 6 BSc Eng Civil (Engineering) | NCE: **missing required subject** (Physical Sciences) | | Yes / Yes (DOCX) |
| 8 | Ayesha Patel (12) | 2026-03-24 | 8 BSc Physiotherapy (Health Sciences) | NCE: **mark below the minimum**, Mathematics 46 vs 50 (4 points, beyond the tolerance) | | Yes / Yes (PDF) |
| 9 | Lerato Mokoena (12) | 2026-05-12 | 1 BCom IS (Commerce) | POSSIBLE: requirements met but interest 20; **her interests align with Health Sciences** (BHealthSc interest 91.3, a Strong alternative) | alternatives in summary | Yes / Yes (PDF) |
| 10 | Thabo Ndlovu (11) | 2026-06-23 | 3 BSc CS (Science) | STRONG: interest 83.2 | **funding requested** | Yes / Yes (PDF) |
| 11 | Chloe Adams (12) | 2026-08-04 | 3 BSc CS (Science) | POSSIBLE: overall average 60.5 below the 65 minimum (one mark has a decimal: Geography 58.5) | **alternatives in summary** (BCom IS is Strong) | Yes / Yes (DOCX) |
| 12 | (no name given) (11) | 2026-09-08 | 9 BSocSc (Humanities) | MOREINFO: **no subjects and no interest answers** | | Yes / Yes (PDF) |
| 13 | Sipho Mahlangu (11) | 2026-10-10 | 9 BSocSc (Humanities) | STRONG: Mathematical Literacy 66 accepted, interest 81 | | No (In progress) / No |

Once the coordinator has completed them, sessions 6–12 give the **exported reports** scenario,
and FP-DIE and FP-LER get completed sessions in all five faculties. Learner 12 has no first name
or surname, because both are optional. Learners 6–13 give a mix of contact details: email only,
number only, both, or none.

### Funding (sample data, closing dates as at 2026-10-14)

| ID | Name | Type | Merit / need | Closes | Faculties | Programmes |
|---|---|---|---|---|---|---|
| 1 | Kalahari Lantern Commerce Bursary | Bursary | need | 2026-11-30 | Commerce | 1, 2 |
| 2 | Bluegum Science Excellence Scholarship | Scholarship | merit | 2027-01-31 | Science | 3, 4 |
| 3 | Ironvale Engineering Bursary | Bursary | merit, need | 2026-12-15 | Engineering | 5, 6 |
| 4 | Sunhaven Health Sciences Bursary | Bursary | need | 2027-02-26 | Health Sciences | 7, 8 |
| 5 | Silverleaf Academic Merit Award | Scholarship | merit | 2027-03-31 | all five | — |
| 6 | Riverstone Community Financial Aid Fund | Financial Aid | need | 2027-01-15 | Health Sciences, Humanities | — |
| 7 | Bytebridge Digital Futures Bursary | External Funding | merit | 2026-11-20 | — | 1, 3 |
| 8 | Khanyisa Social Impact Scholarship | Scholarship | need | 2027-04-30 | Humanities | 9, 10 |
| 9 | Marula Leaf Commerce Bursary (**inactive**) | Bursary | need | 2027-01-31 | Commerce | 1 |
| 10 | Harbourlight Early Application Bursary (**closed**) | Bursary | merit | 2026-09-30 | Commerce, Science | 1 |

### The test scripts

| Script | Needs | Checks |
|---|---|---|
| `tests/Seed-Check.ps1` | the shared contract only | Every rule above. It also proves each session's intended outcome by applying the §4 rules (`application-layer.md`) to the seed, using the shared entity methods. |
| `tests/PartB-Regression.ps1` | learner-journey code | The five Part B outcomes through `EligibilityController`, one selected row per session, re-run safety, and the 57.9 / 58.0 / 60 boundary through `RecommendationEngine` |
| `tests/TC01-TC05.ps1` | every slice | TC01–TC05 through the real controllers, plus the role checks and inactive-programme filtering |

TC04 never counts the open funding options, so it doesn't go stale as closing dates pass. It
checks that the inactive option (9) never appears, and that the closed option (10) is listed
until "open only" is ticked and excluded after.
