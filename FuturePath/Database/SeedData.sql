/* =====================================================================
   Future Path - seed data (case study Part B and extra test data).
   Run by Database/BuildDatabase.ps1 after CreateSchema.sql, on an empty
   database. Every ID is fixed with IDENTITY_INSERT so the tests can rely
   on it (docs/design/plan-and-tests.md, "Seed data as built").

   - The five Part B learners are LearnerID / SessionID 1-5, exactly as
     the case study gives them. Their outcomes depend on the requirement
     rows below and on the provisional scoring parameters
     (decisions.md D14). Re-run tests\PartB-Regression.ps1 after changing
     either.
   - Every other person, school and funding option is fictional. The
     funding options are locally stored sample data, not real offers, and
     the requirements are sample rules, not official university rules.
   - Every session is 'In progress' and there are no RecommendationResult
     or ExportReport rows: results are generated with the real engine
     after the merge (see the session section for the plan).

   Test passwords (sample accounts only; never use them on a real system):
       admin           FuturePath-Admin1
       advisor         FuturePath-Advisor1
       former.advisor  FuturePath-Former1
       senior.advisor  FuturePath-Senior1
       data.admin      FuturePath-DataAdmin1
   The stored values are salted SHA-256 hashes (salt:hash) made with UserAccount.HashPassword.
   ===================================================================== */

SET NOCOUNT ON;
GO

/* ---------- Grade levels (no IDENTITY: the IDs match the GradeLevel enum) ---------- */

INSERT INTO GradeLevel (GradeLevelID, GradeDescription, MarkBasis) VALUES
    (1, 'Grade 11', 'Projected'),
    (2, 'Grade 12', 'Final');
GO

/* ---------- System parameters (application-layer.md section 5) ---------- */

SET IDENTITY_INSERT SystemParameter ON;
INSERT INTO SystemParameter (ParameterID, ParameterCode, ParameterValue, ParameterDescription) VALUES
    ( 1, 'MIN_SUBJECT_COUNT',   '7',    'Subjects a learner should capture on UI 3. A warning (amber notice), not a block.'),
    ( 2, 'NSC_LEVEL_1',         '0',    'NSC achievement level 1 (Not achieved): lower bound of the band, in %.'),
    ( 3, 'NSC_LEVEL_2',         '30',   'NSC achievement level 2 (Elementary achievement): lower bound of the band, in %.'),
    ( 4, 'NSC_LEVEL_3',         '40',   'NSC achievement level 3 (Moderate achievement): lower bound of the band, in %.'),
    ( 5, 'NSC_LEVEL_4',         '50',   'NSC achievement level 4 (Adequate achievement): lower bound of the band, in %.'),
    ( 6, 'NSC_LEVEL_5',         '60',   'NSC achievement level 5 (Substantial achievement): lower bound of the band, in %.'),
    ( 7, 'NSC_LEVEL_6',         '70',   'NSC achievement level 6 (Meritorious achievement): lower bound of the band, in %.'),
    ( 8, 'NSC_LEVEL_7',         '80',   'NSC achievement level 7 (Outstanding achievement): lower bound of the band, in %.'),
    ( 9, 'FUNDING_TYPE_1',      'Scholarship',      'Funding type for UI 13.3 and the UI 6.2b filter.'),
    (10, 'FUNDING_TYPE_2',      'Bursary',          'Funding type for UI 13.3 and the UI 6.2b filter.'),
    (11, 'FUNDING_TYPE_3',      'Financial Aid',    'Funding type for UI 13.3 and the UI 6.2b filter.'),
    (12, 'FUNDING_TYPE_4',      'External Funding', 'Funding type for UI 13.3 and the UI 6.2b filter.'),
    (13, 'PROVINCE_1',          'Eastern Cape',     'Province for the learner profile (UI 2) and the staff search.'),
    (14, 'PROVINCE_2',          'Free State',       'Province for the learner profile (UI 2) and the staff search.'),
    (15, 'PROVINCE_3',          'Gauteng',          'Province for the learner profile (UI 2) and the staff search.'),
    (16, 'PROVINCE_4',          'KwaZulu-Natal',    'Province for the learner profile (UI 2) and the staff search.'),
    (17, 'PROVINCE_5',          'Limpopo',          'Province for the learner profile (UI 2) and the staff search.'),
    (18, 'PROVINCE_6',          'Mpumalanga',       'Province for the learner profile (UI 2) and the staff search.'),
    (19, 'PROVINCE_7',          'North West',       'Province for the learner profile (UI 2) and the staff search.'),
    (20, 'PROVINCE_8',          'Northern Cape',    'Province for the learner profile (UI 2) and the staff search.'),
    (21, 'PROVINCE_9',          'Western Cape',     'Province for the learner profile (UI 2) and the staff search.'),
    (22, 'WEIGHT_SUBJECT',      '0.30', 'Weight of the subject match score in the overall score. Provisional (decisions.md D14).'),
    (23, 'WEIGHT_MARK',         '0.30', 'Weight of the mark match score in the overall score. Provisional (decisions.md D14).'),
    (24, 'WEIGHT_INTEREST',     '0.30', 'Weight of the interest score in the overall score. Provisional (decisions.md D14).'),
    (25, 'WEIGHT_FACULTY',      '0.10', 'Weight of the preferred-faculty bonus in the overall score. Provisional (decisions.md D14).'),
    (26, 'THRESHOLD_STRONG',    '75',   'Lowest overall score (0-100) for a Strong Match. Provisional (decisions.md D14).'),
    (27, 'INTEREST_STRONG_MIN', '60',   'Lowest interest score (0-100) for a Strong Match. Provisional (decisions.md D14).'),
    (28, 'MARK_TOLERANCE',      '2',    'Largest shortfall (points) below a Required minimum that is still a Possible Match. Provisional (decisions.md D14).');
SET IDENTITY_INSERT SystemParameter OFF;
GO

/* ---------- Faculties ---------- */

SET IDENTITY_INSERT Faculty ON;
INSERT INTO Faculty (FacultyID, FacultyName, IsActive) VALUES
    (1, 'Commerce', 1),
    (2, 'Science', 1),
    (3, 'Engineering and the Built Environment', 1),
    (4, 'Health Sciences', 1),
    (5, 'Humanities', 1);
SET IDENTITY_INSERT Faculty OFF;
GO

/* ---------- Subjects (16; Part B "English" is English Home Language) ---------- */

SET IDENTITY_INSERT Subject ON;
INSERT INTO Subject (SubjectID, SubjectName, SubjectLevel, IsActive) VALUES
    ( 1, 'English Home Language',               'Home Language', 1),
    ( 2, 'Afrikaans First Additional Language', 'First Additional Language', 1),
    ( 3, 'Mathematics',                         'Core', 1),
    ( 4, 'Mathematical Literacy',               'Core', 1),
    ( 5, 'Life Orientation',                    'Core', 1),
    ( 6, 'Physical Sciences',                   'Elective', 1),
    ( 7, 'Life Sciences',                       'Elective', 1),
    ( 8, 'Accounting',                          'Elective', 1),
    ( 9, 'Business Studies',                    'Elective', 1),
    (10, 'Economics',                           'Elective', 1),
    (11, 'Geography',                           'Elective', 1),
    (12, 'History',                             'Elective', 1),
    (13, 'Information Technology',              'Elective', 1),
    (14, 'Engineering Graphics and Design',     'Elective', 1),
    (15, 'Tourism',                             'Elective', 1),
    (16, 'Visual Arts',                         'Elective', 1);
SET IDENTITY_INSERT Subject OFF;
GO

/* ---------- Degree programmes (sample names and rules, not official requirements) ----------
   11 is inactive on purpose: it would suit Aadiel-Raboo Magwa (Mathematical Literacy,
   Tourism), so it proves inactive programmes never appear in alternatives or lists. */

SET IDENTITY_INSERT DegreeProgramme ON;
INSERT INTO DegreeProgramme (ProgrammeID, FacultyID, ProgrammeName, MinimumOverallAverage, IsActive) VALUES
    ( 1, 1, 'BCom Information Systems',                 60, 1),
    ( 2, 1, 'BCom Accounting',                          60, 1),
    ( 3, 2, 'BSc Computer Science',                     65, 1),
    ( 4, 2, 'BSc Biological Sciences',                  60, 1),
    ( 5, 3, 'BSc Engineering (Mechanical Engineering)', 65, 1),
    ( 6, 3, 'BSc Engineering (Civil Engineering)',      65, 1),
    ( 7, 4, 'Bachelor of Health Sciences',              65, 1),
    ( 8, 4, 'BSc Physiotherapy',                        60, 1),
    ( 9, 5, 'Bachelor of Social Science',               55, 1),
    (10, 5, 'Bachelor of Arts',                         NULL, 1),
    (11, 1, 'BCom Tourism Management',                  50, 0);
SET IDENTITY_INSERT DegreeProgramme OFF;
GO

/* ---------- Programme requirements ----------
   Subjects are written by name for readability and joined to Subject; the row-count check
   at the end of this batch fails the build if a name is misspelt.
   Part B depends on these rows (plan-and-tests.md):
   - 1 BCom IS: English 50, Mathematics 60 (no alternative)          -> Gadija meets them.
   - 3 BSc CS: Mathematics 70, Mathematical Literacy NOT accepted    -> Aadiel is missing Mathematics.
   - 4 BSc Bio Sci: Mathematics 60 (no alternative)                  -> Labeeqah's 58 is 2 short (tolerance 2).
   - 5 BSc Eng Mech: Mathematics 70, Physical Sciences 70            -> Siyabonga meets them.
   - 7 BHealthSc: Mathematics 60 and Physical Sciences 60, no alts   -> Derek is missing both.
   Requirement 4 is INACTIVE: if it were active, Gadija (no Information Technology) would
   not be eligible, so it proves inactive requirements are ignored. */

DECLARE @Requirement TABLE (
    RequirementID INT, ProgrammeID INT, SubjectName VARCHAR(60), RequirementType VARCHAR(20),
    MinimumMark DECIMAL(5,2), AlternativeName VARCHAR(60), AlternativeMinimumMark DECIMAL(5,2),
    AdvisoryNote VARCHAR(255), IsActive BIT);

INSERT INTO @Requirement VALUES
    -- 1 BCom Information Systems
    ( 1,  1, 'English Home Language',  'Required',    50, NULL, NULL, NULL, 1),
    ( 2,  1, 'Mathematics',            'Required',    60, NULL, NULL, 'Mathematical Literacy is not accepted in place of Mathematics for this programme.', 1),
    ( 3,  1, 'Accounting',             'Recommended', 50, NULL, NULL, 'Not required, but Accounting makes the first-year accounting course easier.', 1),
    ( 4,  1, 'Information Technology', 'Required',    50, NULL, NULL, 'Retired sample rule kept for history: Information Technology is no longer required.', 0),
    -- 2 BCom Accounting
    ( 5,  2, 'English Home Language',  'Required',    50, NULL, NULL, NULL, 1),
    ( 6,  2, 'Mathematics',            'Required',    60, NULL, NULL, NULL, 1),
    ( 7,  2, 'Accounting',             'Recommended', 60, NULL, NULL, 'Accounting is strongly recommended for this programme.', 1),
    -- 3 BSc Computer Science (Mathematical Literacy NOT accepted)
    ( 8,  3, 'English Home Language',  'Required',    50, NULL, NULL, NULL, 1),
    ( 9,  3, 'Mathematics',            'Required',    70, NULL, NULL, 'Mathematical Literacy is not accepted in place of Mathematics for this programme.', 1),
    (10,  3, 'Information Technology', 'Recommended', 60, NULL, NULL, NULL, 1),
    -- 4 BSc Biological Sciences
    (11,  4, 'English Home Language',  'Required',    50, NULL, NULL, NULL, 1),
    (12,  4, 'Mathematics',            'Required',    60, NULL, NULL, 'Mathematics is needed for first-year chemistry and statistics. A Grade 11 learner still has time to improve this mark.', 1),
    (13,  4, 'Physical Sciences',      'Required',    50, NULL, NULL, NULL, 1),
    (14,  4, 'Life Sciences',          'Recommended', 60, NULL, NULL, NULL, 1),
    -- 5 BSc Engineering (Mechanical Engineering)
    (15,  5, 'English Home Language',  'Required',    50, NULL, NULL, NULL, 1),
    (16,  5, 'Mathematics',            'Required',    70, NULL, NULL, NULL, 1),
    (17,  5, 'Physical Sciences',      'Required',    70, NULL, NULL, NULL, 1),
    (18,  5, 'Engineering Graphics and Design', 'Recommended', 60, NULL, NULL, 'Engineering Graphics and Design helps with first-year engineering drawing.', 1),
    -- 6 BSc Engineering (Civil Engineering)
    (19,  6, 'English Home Language',  'Required',    50, NULL, NULL, NULL, 1),
    (20,  6, 'Mathematics',            'Required',    70, NULL, NULL, NULL, 1),
    (21,  6, 'Physical Sciences',      'Required',    65, NULL, NULL, NULL, 1),
    (22,  6, 'Geography',              'Recommended', 60, NULL, NULL, NULL, 1),
    -- 7 Bachelor of Health Sciences
    (23,  7, 'English Home Language',  'Required',    50, NULL, NULL, NULL, 1),
    (24,  7, 'Mathematics',            'Required',    60, NULL, NULL, NULL, 1),
    (25,  7, 'Physical Sciences',      'Required',    60, NULL, NULL, 'Physical Sciences is needed for first-year chemistry and physics.', 1),
    (26,  7, 'Life Sciences',          'Recommended', 60, NULL, NULL, NULL, 1),
    -- 8 BSc Physiotherapy
    (27,  8, 'English Home Language',  'Required',    50, NULL, NULL, NULL, 1),
    (28,  8, 'Mathematics',            'Required',    50, NULL, NULL, NULL, 1),
    (29,  8, 'Life Sciences',          'Required',    60, NULL, NULL, NULL, 1),
    (30,  8, 'Physical Sciences',      'Recommended', 50, NULL, NULL, NULL, 1),
    -- 9 Bachelor of Social Science (accepts Mathematical Literacy)
    (31,  9, 'English Home Language',  'Required',    60, NULL, NULL, NULL, 1),
    (32,  9, 'Mathematics',            'Required',    50, 'Mathematical Literacy', 60, 'Mathematical Literacy of 60% or more is accepted in place of Mathematics.', 1),
    (33,  9, 'History',                'Recommended', 60, NULL, NULL, NULL, 1),
    -- 10 Bachelor of Arts (accepts Mathematical Literacy)
    (34, 10, 'English Home Language',  'Required',    60, NULL, NULL, NULL, 1),
    (35, 10, 'Mathematics',            'Required',    50, 'Mathematical Literacy', 60, 'Mathematical Literacy of 60% or more is accepted in place of Mathematics.', 1),
    (36, 10, 'Afrikaans First Additional Language', 'Recommended', 50, NULL, NULL, 'A second language is useful for language and literature majors.', 1),
    -- 11 BCom Tourism Management (inactive programme)
    (37, 11, 'English Home Language',  'Required',    50, NULL, NULL, NULL, 1),
    (38, 11, 'Mathematics',            'Required',    50, 'Mathematical Literacy', 50, NULL, 1),
    (39, 11, 'Tourism',                'Recommended', 60, NULL, NULL, NULL, 1);

SET IDENTITY_INSERT ProgrammeRequirement ON;
INSERT INTO ProgrammeRequirement (RequirementID, ProgrammeID, SubjectID, RequirementType, MinimumMark,
                                  AlternativeSubjectID, AlternativeMinimumMark, AdvisoryNote, IsActive)
SELECT r.RequirementID, r.ProgrammeID, s.SubjectID, r.RequirementType, r.MinimumMark,
       a.SubjectID, r.AlternativeMinimumMark, r.AdvisoryNote, r.IsActive
FROM @Requirement r
     INNER JOIN Subject s ON s.SubjectName = r.SubjectName
     LEFT JOIN Subject a ON a.SubjectName = r.AlternativeName;
SET IDENTITY_INSERT ProgrammeRequirement OFF;

IF (SELECT COUNT(*) FROM ProgrammeRequirement) <> (SELECT COUNT(*) FROM @Requirement)
   OR EXISTS (SELECT 1 FROM @Requirement r WHERE r.AlternativeName IS NOT NULL
              AND NOT EXISTS (SELECT 1 FROM Subject s WHERE s.SubjectName = r.AlternativeName))
    THROW 50001, 'SeedData: a ProgrammeRequirement subject name does not match the Subject table.', 1;
GO

/* ---------- Interest categories and questions ----------
   Categories 1-8 are the case-study themes; 9 is inactive. Questions 1-8 use the case-study
   wording; 9-12 add a second statement to four categories. Question 13 is inactive. */

SET IDENTITY_INSERT InterestCategory ON;
INSERT INTO InterestCategory (CategoryID, CategoryName, IsActive) VALUES
    (1, 'Numbers and data', 1),
    (2, 'Helping people and communities', 1),
    (3, 'Technology', 1),
    (4, 'Science experiments and investigation', 1),
    (5, 'Reading, writing and analysing ideas', 1),
    (6, 'Business, entrepreneurship and finance', 1),
    (7, 'Designing and building', 1),
    (8, 'Society, law and politics', 1),
    (9, 'Outdoor adventure and sport', 0);
SET IDENTITY_INSERT InterestCategory OFF;

SET IDENTITY_INSERT InterestQuestion ON;
INSERT INTO InterestQuestion (QuestionID, CategoryID, QuestionText, DisplayOrder, IsActive) VALUES
    ( 1, 1, 'I enjoy solving problems using numbers or data.', 1, 1),
    ( 2, 2, 'I enjoy helping people and communities.', 2, 1),
    ( 3, 3, 'I enjoy working with technology.', 3, 1),
    ( 4, 4, 'I enjoy science experiments and research.', 4, 1),
    ( 5, 5, 'I enjoy reading, writing and analysing ideas.', 5, 1),
    ( 6, 6, 'I enjoy business, entrepreneurship and finance.', 6, 1),
    ( 7, 7, 'I enjoy designing or building practical solutions.', 7, 1),
    ( 8, 8, 'I enjoy understanding society, law or politics.', 8, 1),
    ( 9, 3, 'I like finding out how computers, apps and websites work.', 9, 1),
    (10, 4, 'I am curious about how living things and the natural world work.', 10, 1),
    (11, 2, 'I would like a career caring for people''s health and wellbeing.', 11, 1),
    (12, 7, 'I like working out how machines, buildings or structures are put together.', 12, 1),
    (13, 9, 'I enjoy outdoor adventure and sport.', 13, 0);
SET IDENTITY_INSERT InterestQuestion OFF;
GO

/* ---------- Degree interest mappings (2-4 categories per programme, weight 0.40-1.00) ---------- */

INSERT INTO DegreeInterestMapping (ProgrammeID, CategoryID, RelevanceWeight) VALUES
    ( 1, 3, 1.00), ( 1, 6, 0.80), ( 1, 1, 0.70),                -- BCom IS: technology, business, numbers
    ( 2, 1, 1.00), ( 2, 6, 0.90),                               -- BCom Accounting: numbers, business
    ( 3, 3, 1.00), ( 3, 1, 0.80), ( 3, 7, 0.50),                -- BSc CS: technology, numbers, designing
    ( 4, 4, 1.00), ( 4, 1, 0.50), ( 4, 2, 0.40),                -- BSc Bio Sci: science, numbers, helping
    ( 5, 7, 1.00), ( 5, 1, 0.70), ( 5, 4, 0.60),                -- BSc Eng Mech: designing, numbers, science
    ( 6, 7, 1.00), ( 6, 1, 0.70), ( 6, 8, 0.40),                -- BSc Eng Civil: designing, numbers, society
    ( 7, 2, 1.00), ( 7, 4, 0.90), ( 7, 5, 0.40),                -- BHealthSc: helping, science, reading
    ( 8, 2, 1.00), ( 8, 4, 0.70),                               -- BSc Physiotherapy: helping, science
    ( 9, 8, 1.00), ( 9, 2, 0.80), ( 9, 5, 0.70),                -- BSocSc: society, helping, reading
    (10, 5, 1.00), (10, 8, 0.70),                               -- BA: reading, society
    (11, 6, 1.00), (11, 2, 0.60);                               -- BCom Tourism (inactive): business, helping
GO

/* ---------- Funding options (locally stored sample data; every name is fictional) ----------
   Today is 2026-10-14. 9 is INACTIVE and linked to Commerce and BCom IS, so the tests prove
   it is excluded. 10 is active but CLOSED (2026-09-30), so "open only" can be tested. */

SET IDENTITY_INSERT FundingOption ON;
INSERT INTO FundingOption (FundingID, FundingName, FundingType, EligibilityDescription, RequiredDocuments,
                           ClosingDate, ContactInformation, IsMeritBased, IsNeedBased, IsActive) VALUES
    ( 1, 'Kalahari Lantern Commerce Bursary', 'Bursary',
         'For South African learners entering a Commerce degree whose household income is below the sample threshold. Covers tuition and books for the first year.',
         'Certified ID copy; latest school report; proof of household income', '2026-11-30',
         'bursaries@kalaharilantern.example | 021 555 0110', 0, 1, 1),
    ( 2, 'Bluegum Science Excellence Scholarship', 'Scholarship',
         'For learners entering a Science degree with an average of 75% or more, including Mathematics.',
         'Latest school report; one-page motivation letter', '2027-01-31',
         'scholarships@bluegumfoundation.example | 012 555 0120', 1, 0, 1),
    ( 3, 'Ironvale Engineering Bursary', 'Bursary',
         'For learners entering an Engineering degree with Mathematics and Physical Sciences of 70% or more who need financial support. Includes vacation work.',
         'Certified ID copy; latest school report; proof of household income', '2026-12-15',
         'careers@ironvale.example | 011 555 0130', 1, 1, 1),
    ( 4, 'Sunhaven Health Sciences Bursary', 'Bursary',
         'For learners entering a Health Sciences degree who need financial support and plan to work in a community clinic after graduating.',
         'Certified ID copy; proof of household income; letter of motivation', '2027-02-26',
         'health.bursary@sunhaven.example | 031 555 0140', 0, 1, 1),
    ( 5, 'Silverleaf Academic Merit Award', 'Scholarship',
         'Merit award for learners with an average of 80% or more, in any faculty.',
         'Latest school report', '2027-03-31',
         'awards@silverleaf.example | 021 555 0150', 1, 0, 1),
    ( 6, 'Riverstone Community Financial Aid Fund', 'Financial Aid',
         'Need-based financial aid for learners entering Health Sciences or Humanities degrees. Covers registration and residence fees.',
         'Certified ID copy; proof of household income; affidavit of dependants', '2027-01-15',
         'aid@riverstonefund.example | 041 555 0160', 0, 1, 1),
    ( 7, 'Bytebridge Digital Futures Bursary', 'External Funding',
         'Funded by a fictional software company for learners entering BCom Information Systems or BSc Computer Science with Mathematics of 65% or more.',
         'Latest school report; short coding or technology motivation', '2026-11-20',
         'futures@bytebridge.example | 010 555 0170', 1, 0, 1),
    ( 8, 'Khanyisa Social Impact Scholarship', 'Scholarship',
         'For learners entering a Humanities degree who need financial support and volunteer in their community.',
         'Certified ID copy; proof of household income; reference from a community organisation', '2027-04-30',
         'apply@khanyisatrust.example | 015 555 0180', 0, 1, 1),
    ( 9, 'Marula Leaf Commerce Bursary', 'Bursary',
         'Discontinued sample bursary for Commerce learners. Kept inactive for history.',
         'Certified ID copy', '2027-01-31',
         'info@marulaleaf.example | 021 555 0190', 0, 1, 0),
    (10, 'Harbourlight Early Application Bursary', 'Bursary',
         'Early-application bursary for Commerce and Science learners with an average of 70% or more. Applications have closed for this year.',
         'Latest school report', '2026-09-30',
         'early@harbourlight.example | 021 555 0200', 1, 0, 1);
SET IDENTITY_INSERT FundingOption OFF;

INSERT INTO FundingFaculty (FundingID, FacultyID) VALUES
    ( 1, 1),
    ( 2, 2),
    ( 3, 3),
    ( 4, 4),
    ( 5, 1), ( 5, 2), ( 5, 3), ( 5, 4), ( 5, 5),
    ( 6, 4), ( 6, 5),
    ( 8, 5),
    ( 9, 1),
    (10, 1), (10, 2);

INSERT INTO FundingProgramme (FundingID, ProgrammeID) VALUES
    ( 1, 1), ( 1, 2),
    ( 2, 3), ( 2, 4),
    ( 3, 5), ( 3, 6),
    ( 4, 7), ( 4, 8),
    ( 7, 1), ( 7, 3),
    ( 8, 9), ( 8, 10),
    ( 9, 1),
    (10, 1);
GO

/* ---------- Learners (1-5 are the Part B learners; everyone else is fictional) ----------
   Learner 12 gave no name (FirstName and LastName are optional). */

SET IDENTITY_INSERT Learner ON;
INSERT INTO Learner (LearnerID, FirstName, LastName, SchoolName, Province, ContactEmail, ContactNumber) VALUES
    ( 1, 'Gadija',       'Jacobs',     'Silvertree Secondary School',     'Western Cape',  'gadija.jacobs@mail.example',      '082 555 0101'),
    ( 2, 'Aadiel-Raboo', 'Magwa',      'Kingfisher Bay High School',      'Eastern Cape',  NULL,                              '073 555 0102'),
    ( 3, 'Labeeqah',     'Rademeyer',  'Fynbos Valley High School',       'Western Cape',  'labeeqah.rademeyer@mail.example', NULL),
    ( 4, 'Siyabonga',    'Dickson',    'Sunbird Valley High School',      'KwaZulu-Natal', NULL,                              NULL),
    ( 5, 'Derek',        'Vin Diesel', 'Ironwood Ridge High School',      'Gauteng',       'derek.vindiesel@mail.example',    '061 555 0105'),
    ( 6, 'Nomvula',      'Khumalo',    'Jacaranda Crescent High School',  'Gauteng',       'nomvula.khumalo@mail.example',    '072 555 0106'),
    ( 7, 'Pieter',       'van Wyk',    'Golden Plains High School',       'Free State',    NULL,                              '083 555 0107'),
    ( 8, 'Ayesha',       'Patel',      'Sunbird Valley High School',      'KwaZulu-Natal', 'ayesha.patel@mail.example',       NULL),
    ( 9, 'Lerato',       'Mokoena',    'Baobab Crest Secondary School',   'Limpopo',       NULL,                              '079 555 0109'),
    (10, 'Thabo',        'Ndlovu',     'Lowveld Horizon Secondary School','Mpumalanga',    'thabo.ndlovu@mail.example',       '084 555 0110'),
    (11, 'Chloe',        'Adams',      'Karoo Star Combined School',      'Northern Cape', NULL,                              NULL),
    (12, NULL,           NULL,         'Marula Grove Secondary School',   'North West',    NULL,                              NULL),
    (13, 'Sipho',        'Mahlangu',   'Kingfisher Bay High School',      'Eastern Cape',  'sipho.mahlangu@mail.example',     '076 555 0113');
SET IDENTITY_INSERT Learner OFF;
GO

/* ---------- Advisory sessions ----------
   SessionID = LearnerID (one session per learner, so CheckEligibility(learnerID, ...) always
   reaches the session below). Every session is seeded 'In progress' with no results and no
   exports. Intended outcomes follow application-layer.md section 4 with the provisional
   parameters (decisions.md D14). Interest scores are for the selected programme.

   Session  Learner              Gr  Programme (preferred faculty)        Intended outcome and why                                      Coordinator: complete?  export?
   -------  -------------------  --  -----------------------------------  ------------------------------------------------------------  ----------------------  -------
    1       Gadija Jacobs        12  1 BCom Information Systems (Comm)    STRONG: requirements met, interest 88                         No (stays In progress)  No
    2       Aadiel-Raboo Magwa   12  3 BSc Computer Science (Sci)         NCE: Mathematics missing (Maths Literacy not accepted)         No (stays In progress)  No
    3       Labeeqah Rademeyer   11  4 BSc Biological Sciences (Sci)      POSSIBLE: Mathematics 58 is 2 below 60                        No (stays In progress)  No
    4       Siyabonga Dickson    12  5 BSc Eng (Mechanical) (Eng)         STRONG: requirements met, interest 85.9                       No (stays In progress)  No
    5       Derek Vin Diesel     12  7 Bachelor of Health Sciences (HS)   NCE: Mathematics and Physical Sciences missing                No (stays In progress)  No
    6       Nomvula Khumalo      12  2 BCom Accounting (Comm)             STRONG: requirements met, interest 100; funding requested     Yes                     Yes (PDF)
    7       Pieter van Wyk       11  6 BSc Eng (Civil) (Eng)              NCE: Physical Sciences (required) not captured                Yes                     Yes (DOCX)
    8       Ayesha Patel         12  8 BSc Physiotherapy (HS)             NCE: Mathematics 46 is 4 below the 50 minimum                 Yes                     Yes (PDF)
    9       Lerato Mokoena       12  1 BCom Information Systems (Comm)    POSSIBLE: requirements met but interest 20 (< 60); her        Yes                     Yes (PDF)
                                                                          interests align with Health Sciences; alternatives in summary
   10       Thabo Ndlovu         11  3 BSc Computer Science (Sci)         STRONG: requirements met, interest 83.2; funding requested    Yes                     Yes (PDF)
   11       Chloe Adams          12  3 BSc Computer Science (Sci)         POSSIBLE: average 60.5 below the 65 minimum; alternatives     Yes                     Yes (DOCX)
                                                                          in summary (BCom IS is a Strong alternative)
   12       (no name given)      11  9 Bachelor of Social Science (Hum)   MOREINFO: no subjects and no interest answers                 Yes                     Yes (PDF)
   13       Sipho Mahlangu       11  9 Bachelor of Social Science (Hum)   STRONG: Mathematical Literacy 66 accepted, interest 81        No (stays In progress)  No

   Coordinator, after the merge: for sessions 6-13 run CheckEligibility(learnerID, programme),
   then RecommendationController.GenerateAlternatives(learnerID, programme) (12 returns none),
   then for 6-12 ReportController.CompleteLearnerSession(sessionID, format, SessionDate), which
   marks the session Completed and logs its FP-LSR export (decisions.md D3: a session completes
   when its summary is exported). Sessions 1-5 stay In progress so PartB-Regression.ps1 can
   re-run them. */

SET IDENTITY_INSERT AdvisorySession ON;
INSERT INTO AdvisorySession (SessionID, LearnerID, GradeLevelID, PreferredFacultyID, SelectedProgrammeID, SessionDate,
                             AdvisingYear, FundingInfoRequested, AlternativesInSummary, SessionStatus, DateCompleted) VALUES
    ( 1,  1, 2, 1,  1, '2026-09-28T10:00:00', 2026, 0, 0, 'In progress', NULL),
    ( 2,  2, 2, 2,  3, '2026-09-29T11:30:00', 2026, 0, 0, 'In progress', NULL),
    ( 3,  3, 1, 2,  4, '2026-09-30T09:00:00', 2026, 0, 0, 'In progress', NULL),
    ( 4,  4, 2, 3,  5, '2026-10-01T14:00:00', 2026, 0, 0, 'In progress', NULL),
    ( 5,  5, 2, 4,  7, '2026-10-02T10:30:00', 2026, 0, 0, 'In progress', NULL),
    ( 6,  6, 2, 1,  2, '2026-01-20T09:15:00', 2026, 1, 0, 'In progress', NULL),
    ( 7,  7, 1, 3,  6, '2026-02-17T13:40:00', 2026, 0, 0, 'In progress', NULL),
    ( 8,  8, 2, 4,  8, '2026-03-24T10:05:00', 2026, 0, 0, 'In progress', NULL),
    ( 9,  9, 2, 1,  1, '2026-05-12T15:20:00', 2026, 0, 1, 'In progress', NULL),
    (10, 10, 1, 2,  3, '2026-06-23T11:00:00', 2026, 1, 0, 'In progress', NULL),
    (11, 11, 2, 2,  3, '2026-08-04T09:45:00', 2026, 0, 1, 'In progress', NULL),
    (12, 12, 1, 5,  9, '2026-09-08T12:00:00', 2026, 0, 0, 'In progress', NULL),
    (13, 13, 1, 5,  9, '2026-10-10T10:00:00', 2026, 0, 0, 'In progress', NULL);
SET IDENTITY_INSERT AdvisorySession OFF;
GO

/* ---------- Subjects and marks per session (Part B rows exactly as the case study) ---------- */

DECLARE @Mark TABLE (LearnerSubjectID INT, SessionID INT, SubjectName VARCHAR(60), Mark DECIMAL(5,1));

INSERT INTO @Mark VALUES
    -- 1 Gadija Jacobs
    ( 1,  1, 'English Home Language', 72), ( 2,  1, 'Mathematics', 70), ( 3,  1, 'Accounting', 74),
    ( 4,  1, 'Business Studies', 79), ( 5,  1, 'Economics', 70), ( 6,  1, 'Life Orientation', 80),
    -- 2 Aadiel-Raboo Magwa
    ( 7,  2, 'English Home Language', 65), ( 8,  2, 'Mathematical Literacy', 78), ( 9,  2, 'Business Studies', 71),
    (10,  2, 'History', 69), (11,  2, 'Tourism', 75), (12,  2, 'Life Orientation', 82),
    -- 3 Labeeqah Rademeyer
    (13,  3, 'English Home Language', 80), (14,  3, 'Mathematics', 58), (15,  3, 'Physical Sciences', 62),
    (16,  3, 'Life Sciences', 70), (17,  3, 'Geography', 66), (18,  3, 'Life Orientation', 85),
    -- 4 Siyabonga Dickson
    (19,  4, 'English Home Language', 70), (20,  4, 'Mathematics', 75), (21,  4, 'Physical Sciences', 78),
    (22,  4, 'Engineering Graphics and Design', 73), (23,  4, 'Life Orientation', 80),
    -- 5 Derek Vin Diesel
    (24,  5, 'English Home Language', 82), (25,  5, 'Mathematical Literacy', 76), (26,  5, 'History', 79),
    (27,  5, 'Life Sciences', 68), (28,  5, 'Visual Arts', 85), (29,  5, 'Life Orientation', 90),
    -- 6 Nomvula Khumalo
    (30,  6, 'English Home Language', 78), (31,  6, 'Mathematics', 74), (32,  6, 'Accounting', 81),
    (33,  6, 'Business Studies', 76), (34,  6, 'Economics', 72), (35,  6, 'Afrikaans First Additional Language', 68),
    (36,  6, 'Life Orientation', 84),
    -- 7 Pieter van Wyk (no Physical Sciences)
    (37,  7, 'English Home Language', 68), (38,  7, 'Mathematics', 72), (39,  7, 'Geography', 74),
    (40,  7, 'Engineering Graphics and Design', 70), (41,  7, 'Information Technology', 66),
    (42,  7, 'Afrikaans First Additional Language', 75), (43,  7, 'Life Orientation', 80),
    -- 8 Ayesha Patel (Mathematics 46 vs the 50 minimum)
    (44,  8, 'English Home Language', 74), (45,  8, 'Mathematics', 46), (46,  8, 'Life Sciences', 77),
    (47,  8, 'Physical Sciences', 58), (48,  8, 'Geography', 70), (49,  8, 'Afrikaans First Additional Language', 63),
    (50,  8, 'Life Orientation', 82),
    -- 9 Lerato Mokoena
    (51,  9, 'English Home Language', 76), (52,  9, 'Mathematics', 68), (53,  9, 'Life Sciences', 82),
    (54,  9, 'Physical Sciences', 71), (55,  9, 'Business Studies', 64), (56,  9, 'Afrikaans First Additional Language', 66),
    (57,  9, 'Life Orientation', 85),
    -- 10 Thabo Ndlovu
    (58, 10, 'English Home Language', 70), (59, 10, 'Mathematics', 82), (60, 10, 'Physical Sciences', 75),
    (61, 10, 'Information Technology', 88), (62, 10, 'Geography', 65), (63, 10, 'Afrikaans First Additional Language', 62),
    (64, 10, 'Life Orientation', 78),
    -- 11 Chloe Adams (average 60.5, below BSc CS's 65; one mark with a decimal)
    (65, 11, 'English Home Language', 55), (66, 11, 'Mathematics', 72), (67, 11, 'Information Technology', 70),
    (68, 11, 'Physical Sciences', 52), (69, 11, 'Geography', 58.5), (70, 11, 'Afrikaans First Additional Language', 50),
    (71, 11, 'Life Orientation', 66),
    -- 12 (no name): no subjects captured -> More Information Needed
    -- 13 Sipho Mahlangu (Mathematical Literacy accepted for BSocSc)
    (72, 13, 'English Home Language', 71), (73, 13, 'Mathematical Literacy', 66), (74, 13, 'History', 74),
    (75, 13, 'Geography', 69), (76, 13, 'Business Studies', 60), (77, 13, 'Life Orientation', 77);

SET IDENTITY_INSERT LearnerSubject ON;
INSERT INTO LearnerSubject (LearnerSubjectID, SessionID, SubjectID, Mark)
SELECT m.LearnerSubjectID, m.SessionID, s.SubjectID, m.Mark
FROM @Mark m INNER JOIN Subject s ON s.SubjectName = m.SubjectName;
SET IDENTITY_INSERT LearnerSubject OFF;

IF (SELECT COUNT(*) FROM LearnerSubject) <> (SELECT COUNT(*) FROM @Mark)
    THROW 50002, 'SeedData: a LearnerSubject subject name does not match the Subject table.', 1;
GO

/* ---------- Interest responses (1 = Strongly disagree ... 5 = Strongly agree) ----------
   One row per session, one column per active question (Q1-Q12); unpivoted below.
   The case study gives no Part B answers, so these are invented (plan-and-tests.md explains
   why): Gadija and Siyabonga answer 4s and 5s on their programme's categories (interest 88
   and 85.9, at least 75 in every mapped category), which a Strong Match needs
   (INTEREST_STRONG_MIN 60). The others give varied, plausible answers. Session 12 has none.

   Questions: Q1 numbers, Q2 helping, Q3 technology, Q4 science, Q5 reading, Q6 business,
   Q7 designing, Q8 society, Q9 technology, Q10 science, Q11 helping, Q12 designing. */

DECLARE @Answer TABLE (SessionID INT, Q1 INT, Q2 INT, Q3 INT, Q4 INT, Q5 INT, Q6 INT,
                       Q7 INT, Q8 INT, Q9 INT, Q10 INT, Q11 INT, Q12 INT);

INSERT INTO @Answer VALUES
    --       Q1 Q2 Q3 Q4 Q5 Q6 Q7 Q8 Q9 Q10 Q11 Q12
    ( 1,     4, 3, 4, 2, 3, 5, 3, 2, 5, 2,  2,  3),   -- Gadija: technology, business, numbers
    ( 2,     3, 4, 3, 2, 3, 5, 2, 4, 3, 1,  3,  2),   -- Aadiel: business and society
    ( 3,     3, 4, 2, 5, 4, 1, 2, 3, 2, 5,  4,  2),   -- Labeeqah: science and helping
    ( 4,     4, 3, 4, 4, 2, 2, 5, 2, 4, 4,  2,  5),   -- Siyabonga: designing, numbers, science
    ( 5,     2, 5, 3, 3, 4, 2, 3, 4, 2, 4,  5,  3),   -- Derek: helping and reading
    ( 6,     5, 3, 4, 2, 3, 5, 3, 3, 3, 2,  2,  2),   -- Nomvula: numbers and business
    ( 7,     4, 2, 3, 3, 2, 3, 5, 3, 3, 2,  2,  5),   -- Pieter: designing
    ( 8,     3, 5, 2, 4, 4, 2, 2, 3, 2, 5,  5,  2),   -- Ayesha: helping and science
    ( 9,     2, 5, 2, 5, 3, 2, 2, 3, 1, 5,  5,  2),   -- Lerato: helping and science (Health Sciences), not BCom IS
    (10,     4, 2, 5, 3, 2, 3, 4, 2, 5, 3,  2,  3),   -- Thabo: technology
    (11,     4, 2, 4, 3, 3, 3, 3, 2, 4, 2,  2,  3),   -- Chloe: technology and numbers
    (13,     2, 4, 2, 2, 4, 3, 2, 5, 2, 2,  3,  2);   -- Sipho: society, reading, helping

SET IDENTITY_INSERT LearnerInterestResponse ON;
INSERT INTO LearnerInterestResponse (ResponseID, SessionID, QuestionID, ResponseValue)
SELECT ROW_NUMBER() OVER (ORDER BY a.SessionID, q.QuestionID), a.SessionID, q.QuestionID, q.ResponseValue
FROM @Answer a
     CROSS APPLY (VALUES (1, a.Q1), (2, a.Q2), (3, a.Q3), (4, a.Q4), (5, a.Q5), (6, a.Q6),
                         (7, a.Q7), (8, a.Q8), (9, a.Q9), (10, a.Q10), (11, a.Q11), (12, a.Q12)) AS q (QuestionID, ResponseValue);
SET IDENTITY_INSERT LearnerInterestResponse OFF;
GO

/* ---------- Staff accounts (passwords are in the header comment; hashes are salted SHA-256) ---------- */

SET IDENTITY_INSERT UserAccount ON;
INSERT INTO UserAccount (UserID, Username, PasswordHash, FullName, Role, IsActive) VALUES
    (1, 'admin',          '/QCkBFNWPwPdjsUJ5R+K4w==:I3CKALC3mFpc/TQPVOrpi8q6VvTTRXnwpJz/SR19mOc=', 'Ruth Arendse',   'Administrator',  1),
    (2, 'advisor',        'INyXI8Q7EIgy5UEskrseUg==:ZnNIAz5avWiLdV42Les8g8dX4E7jb4gAIFmFh/DVZRI=', 'Kagiso Molefe',  'Career Advisor', 1),
    (3, 'former.advisor', 'm+CgljWcfRwYLdum/W1M4w==:OgYdspxAx1W1hii+LZ4XYxUodu9rDEAeOn0ZO3lKetE=', 'Megan Pillay',   'Career Advisor', 0),
    (4, 'senior.advisor', 'Wg+3JsOdpoxNtn4zGGvVyw==:k6rumnM+zXJ6/LDRVflFHkvqvGUtO3IIlvN6enhceUQ=', 'Zanele Dlamini', 'Career Advisor', 1),
    (5, 'data.admin',     'fVhSUHGUaf26a8fZ4jw8Tg==:RgRsovzW3RZN71WmGpSjZTkFL/Q6DGBtUIkk5iRD6JE=', 'Riaan Peters',   'Administrator',  1);
SET IDENTITY_INSERT UserAccount OFF;
GO
