# Future-Path-System
A standalone Windows Forms student advising system for Grade 11/12 learners. It matches
subjects, marks and interests against degree requirements using a local rule-based scoring
engine, then produces advisory outcomes and reports. C# on .NET Framework 4.8, three-tier
architecture, SQL Server LocalDB. UCT INF2011S project, Team 14.

Future Path is a desktop advising application for Grade 11 and 12 South African learners,
built for UCT's INF2011S Systems Design & Development course. Learners run their own advisory
session without logging in. They capture their grade, subjects and marks, answer an interest
questionnaire, and choose a faculty and degree. The system checks eligibility against the
programme's stored requirements and gives one of four outcomes: Strong Match, Possible Match,
Not Currently Eligible, or More Information Needed. Every outcome states the reason, such as a
missing subject or a mark below the required minimum. The system also suggests alternative
degrees, shows relevant funding options, and lets the learner export an advisory summary.

All recommendations come from local, rule-based scoring in the business logic layer. The app
needs no internet connection and uses no external AI services. Career Advisors and
Administrators log in. Advisors review advisory sessions and run reports. Administrators also
maintain the reference data (faculties, programmes, requirements, funding, interest questions)
and user accounts. The system produces three reports: the Learner Advisory Summary (FP-LSR), the
Degree Interest & Eligibility Report (FP-DIE), and the Learners Not Currently Eligible exception
report (FP-LER).

Stack: C# · Windows Forms · .NET Framework 4.8 · SQL Server LocalDB · RDLC ReportViewer ·
three-tier architecture (Presentation / Business Logic / Data Access)

The design documents, including the submitted Systems Specification, are in [`docs/`](docs/README.md).
