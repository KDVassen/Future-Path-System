# Future-Path-System
Standalone Windows Forms student advising system for Grade 11/12 learners. Matches subjects, marks and interests against degree requirements using a local rule-based scoring engine, then generates advisory outcomes and reports. C# .NET Framework, three-tier architecture, SQL Server LocalDB. UCT INF2011S project.

Future Path is a desktop student advising application for Grade 11 and 12 South African learners, built for UCT's INF2011S Systems Design & Development course. Career Advisors record a learner's subjects, marks, interests and faculty preferences. The system then checks eligibility against each degree programme's requirements and gives an outcome: Strong Match, Possible Match or Not Currently Eligible. Every outcome states the reason, such as a missing subject or a mark below the required level. The system also suggests alternative degrees and relevant funding options.

All recommendations come from local, rule-based scoring in the business logic layer. The app needs no internet connection and uses no external AI services. Administrators maintain the reference data (faculties, programmes, requirements, funding and interest questions) through role-based access. The system produces three reports: a Learner Advisory Summary, a Degree Interest & Eligibility Report, and a Learners Not Currently Eligible exception report.

Stack: C# · Windows Forms · .NET Framework · SQL Server LocalDB · three-tier architecture (Presentation / Business Logic / Data Access)
