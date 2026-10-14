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
