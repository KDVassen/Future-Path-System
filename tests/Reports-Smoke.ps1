<#
.SYNOPSIS
    Reports and staff session search: SessionSearchController, ExportReportRepository and the
    ReportController catalogue, formats, parameters text, validation, scope counts and export log.

.DESCRIPTION
    Section 1 (PRE-MERGE) uses only this slice's own repositories and must pass now.
    Section 2 (POST-MERGE) fills FP-LSR, FP-DIE and FP-LER. It needs LearnerRepository,
    AdvisorySessionRepository, DegreeProgrammeRepository, FundingRepository and InterestRepository,
    which are stubs (NotImplementedException) until the other slices are merged, so it is
    expected to fail before the merge. Use -SkipPostMerge to run Section 1 only.

    Run: powershell -NoProfile -ExecutionPolicy Bypass -File tests\Reports-Smoke.ps1
#>
param([switch]$SkipPostMerge)

. "$PSScriptRoot\TestHelpers.ps1"

$ctx = Initialize-FuturePathTest

# =====================================================================================
# Fixture rows (the database copy starts empty)
# =====================================================================================
[void](Invoke-TestQuery $ctx "INSERT INTO GradeLevel (GradeLevelID, GradeDescription, MarkBasis) VALUES (1, 'Grade 11', 'Projected'), (2, 'Grade 12', 'Final')")

$settings = Get-ProvisionalSettings
$settings['FUNDING_TYPE_1'] = 'Scholarship'
$settings['FUNDING_TYPE_2'] = 'Bursary'
$settings['FUNDING_TYPE_3'] = 'Financial Aid'
$settings['FUNDING_TYPE_4'] = 'External Funding'
$parameterRows = @()
foreach ($key in $settings.Keys) { $parameterRows += "('$key', '$($settings[$key])')" }
[void](Invoke-TestQuery $ctx ("INSERT INTO SystemParameter (ParameterCode, ParameterValue) VALUES " + ($parameterRows -join ', ')))

$hash = [FuturePath.ApplicationLayer.UserAccount]::HashPassword('Report-Test-1')
[void](Invoke-TestQuery $ctx ("INSERT INTO UserAccount (Username, PasswordHash, FullName, Role, IsActive) VALUES " +
    "('rpt.advisor', '$hash', 'Rita Advisor', 'Career Advisor', 1), " +
    "('rpt.admin', '$hash', 'Andile Admin', 'Administrator', 1), " +
    "('rpt.gone', '$hash', 'Gone Advisor', 'Career Advisor', 0)"))
$users = Invoke-TestQuery $ctx "SELECT UserID, Username FROM UserAccount"
$advisorId = ($users.Rows | Where-Object { $_.Username -eq 'rpt.advisor' }).UserID
$adminId = ($users.Rows | Where-Object { $_.Username -eq 'rpt.admin' }).UserID
$inactiveId = ($users.Rows | Where-Object { $_.Username -eq 'rpt.gone' }).UserID
$unknownId = 9999

[void](Invoke-TestQuery $ctx @"
SET IDENTITY_INSERT Faculty ON;
INSERT INTO Faculty (FacultyID, FacultyName, IsActive) VALUES (1, 'Commerce', 1), (2, 'Science', 1), (3, 'Closed Faculty', 0);
SET IDENTITY_INSERT Faculty OFF;
SET IDENTITY_INSERT Subject ON;
INSERT INTO Subject (SubjectID, SubjectName, SubjectLevel, IsActive) VALUES
  (1, 'Mathematics', 'Core', 1), (2, 'Mathematical Literacy', 'Core', 1), (3, 'Physical Sciences', 'Elective', 1),
  (4, 'English Home Language', 'Home Language', 1), (5, 'Accounting', 'Elective', 1), (6, 'Life Sciences', 'Elective', 1);
SET IDENTITY_INSERT Subject OFF;
SET IDENTITY_INSERT DegreeProgramme ON;
INSERT INTO DegreeProgramme (ProgrammeID, FacultyID, ProgrammeName, MinimumOverallAverage, IsActive) VALUES
  (1, 1, 'BCom Information Systems', 60, 1), (2, 2, 'BSc Computer Science', 65, 1),
  (3, 2, 'BSc Old Programme', NULL, 0), (4, 3, 'BA Closed Programme', NULL, 1);
SET IDENTITY_INSERT DegreeProgramme OFF;
INSERT INTO ProgrammeRequirement (ProgrammeID, SubjectID, RequirementType, MinimumMark, AdvisoryNote, IsActive) VALUES
  (1, 1, 'Required', 50, NULL, 1), (1, 4, 'Required', 50, NULL, 1), (1, 5, 'Recommended', 50, 'Recommended, not compulsory', 1),
  (2, 1, 'Required', 65, 'Mathematical Literacy not accepted', 1), (2, 3, 'Required', 60, NULL, 1);
SET IDENTITY_INSERT InterestCategory ON;
INSERT INTO InterestCategory (CategoryID, CategoryName, IsActive) VALUES (1, 'Business and finance', 1), (2, 'Technology and computing', 1), (3, 'Old category', 0);
SET IDENTITY_INSERT InterestCategory OFF;
SET IDENTITY_INSERT InterestQuestion ON;
INSERT INTO InterestQuestion (QuestionID, CategoryID, QuestionText, DisplayOrder, IsActive) VALUES
  (1, 1, 'I enjoy working with money and business ideas.', 1, 1), (2, 2, 'I enjoy working with computers.', 2, 1), (3, 3, 'Old statement.', 3, 1);
SET IDENTITY_INSERT InterestQuestion OFF;
"@)

[void](Invoke-TestQuery $ctx @"
SET IDENTITY_INSERT Learner ON;
INSERT INTO Learner (LearnerID, FirstName, LastName, SchoolName, Province, ContactEmail, ContactNumber) VALUES
  (1, 'Gadija', 'Jacobs', 'Rondebosch High', 'Western Cape', 'gadija@example.com', '082 000 0001'),
  (2, 'Aadiel-Raboo', 'Magwa', 'Athlone High School', 'Western Cape', NULL, NULL),
  (3, 'Labeeqah', 'Rademeyer', 'Wynberg Girls High', 'Western Cape', NULL, NULL),
  (4, 'Thandi', 'Mokoena', 'Soweto Secondary', 'Gauteng', NULL, NULL),
  (5, 'Jacob', 'Smith', 'Paarl Boys High', 'Western Cape', NULL, NULL);
SET IDENTITY_INSERT Learner OFF;
SET IDENTITY_INSERT AdvisorySession ON;
INSERT INTO AdvisorySession (SessionID, LearnerID, GradeLevelID, PreferredFacultyID, SelectedProgrammeID, SessionDate, AdvisingYear,
                             FundingInfoRequested, AlternativesInSummary, SessionStatus, DateCompleted) VALUES
  (1, 1, 2, 1, 1, '2026-03-10T09:00:00', 2026, 1, 1, 'Completed', '2026-03-10T10:00:00'),
  (2, 2, 2, 2, 2, '2026-04-05T09:00:00', 2026, 1, 0, 'Completed', '2026-04-05T10:00:00'),
  (3, 3, 1, 2, 2, '2026-05-20T09:00:00', 2026, 0, 0, 'Completed', '2026-05-20T10:00:00'),
  (4, 4, 2, 1, 1, '2026-09-01T09:00:00', 2026, 0, 0, 'In progress', NULL),
  (5, 5, 2, 1, 1, '2026-06-15T08:00:00', 2026, 0, 0, 'Completed', '2026-06-15T09:00:00'),
  (6, 1, 2, 1, 1, '2026-06-15T10:00:00', 2026, 0, 0, 'In progress', NULL),
  (7, 2, 2, 1, 1, '2026-06-14T15:00:00', 2026, 0, 0, 'Completed', '2026-06-15T08:30:00'),
  (8, 3, 1, 2, 2, '2026-06-15T11:00:00', 2026, 0, 0, 'Completed', '2026-06-16T09:00:00'),
  (9, 4, 2, 3, 4, '2026-07-01T09:00:00', 2026, 0, 0, 'Completed', '2026-07-01T10:00:00'),
  (10, 5, 2, 2, 3, '2026-07-02T09:00:00', 2026, 0, 0, 'Completed', '2026-07-02T10:00:00'),
  (11, 1, 2, 1, 1, '2025-12-31T15:00:00', 2025, 0, 0, 'Completed', '2025-12-31T16:00:00');
SET IDENTITY_INSERT AdvisorySession OFF;
"@)

[void](Invoke-TestQuery $ctx @"
INSERT INTO RecommendationResult (SessionID, ProgrammeID, SubjectMatchScore, MarkMatchScore, InterestMatchScore, FacultyBonus,
                                  OverallScore, OutcomeCode, OutcomeReason, AlternativeRank, IsSelectedProgramme) VALUES
  (1, 1, 100, 100, 80, 1, 94, 'STRONG', 'Meets every stored requirement for BCom Information Systems.', NULL, 1),
  (1, 2, 50, 0, 60, 0, 42, 'NCE', 'Physical Sciences is required but was not captured.', 1, 0),
  (1, 3, 100, 100, 50, 0, 80, 'STRONG', 'Withdrawn programme.', 2, 0),
  (2, 2, 50, 0, 70, 1, 45.5, 'NCE', 'Physical Sciences is required but was not captured. Mathematics 55% is 10 points below the 65% minimum.', NULL, 1),
  (2, 3, 100, 100, 60, 0, 81, 'STRONG', 'Withdrawn programme.', 1, 0),
  (2, 1, 100, 100, 60, 0, 78, 'STRONG', 'Meets every stored requirement.', 2, 0),
  (3, 2, 100, 100, NULL, 1, NULL, 'MOREINFO', 'The interest questionnaire was not completed.', NULL, 1),
  (5, 1, 50, 50, 60, 1, 54, 'NCE', 'Mathematics is required but was not captured.', NULL, 1),
  (6, 1, 100, 100, 50, 1, 85, 'POSSIBLE', 'Meets the requirements, but interest alignment is weaker.', NULL, 1),
  (7, 1, 100, 100, 80, 1, 94, 'STRONG', 'Meets every stored requirement.', NULL, 1),
  (8, 2, 100, 50, 70, 1, 76, 'POSSIBLE', 'Mathematics 64% is 1 points below the 65% minimum.', NULL, 1),
  (9, 4, 100, 100, 70, 1, 91, 'POSSIBLE', 'Closed faculty.', NULL, 1),
  (10, 3, 0, 0, 40, 1, 22, 'NCE', 'Withdrawn programme.', NULL, 1),
  (11, 1, 100, 100, 80, 1, 94, 'STRONG', 'Meets every stored requirement.', NULL, 1);
INSERT INTO LearnerSubject (SessionID, SubjectID, Mark) VALUES
  (1, 1, 80), (1, 4, 75), (1, 5, 70), (1, 6, 67),
  (2, 1, 55), (2, 4, 60),
  (3, 1, 70), (3, 3, 62),
  (5, 4, 70), (5, 5, 65),
  (6, 1, 70), (6, 4, 70),
  (7, 1, 75), (7, 4, 80),
  (8, 1, 64), (8, 3, 70);
INSERT INTO LearnerInterestResponse (SessionID, QuestionID, ResponseValue) VALUES (1, 1, 5), (1, 2, 3), (1, 3, 4);
SET IDENTITY_INSERT FundingOption ON;
INSERT INTO FundingOption (FundingID, FundingName, FundingType, EligibilityDescription, RequiredDocuments, ClosingDate, IsActive) VALUES
  (1, 'Commerce Bursary', 'Bursary', 'Commerce applicants with financial need.', 'ID copy; results', '2099-12-31', 1),
  (2, 'Science Scholarship', 'Scholarship', 'Science applicants with strong marks.', NULL, NULL, 1),
  (3, 'Old Bursary', 'Bursary', 'Withdrawn.', NULL, NULL, 0),
  (4, 'Expired Aid', 'Financial Aid', 'Closed in 2020.', NULL, '2020-01-01', 1);
SET IDENTITY_INSERT FundingOption OFF;
INSERT INTO FundingFaculty (FundingID, FacultyID) VALUES (1, 1), (3, 1), (4, 1);
INSERT INTO FundingProgramme (FundingID, ProgrammeID) VALUES (2, 2);
INSERT INTO ExportReport (ReportCode, SessionID, ParametersUsed, GeneratedByUserID, DateGenerated, ExportFormat) VALUES
  ('FP-LSR', 1, NULL, NULL, '2026-03-10T10:00:00', 'PDF');
"@)

function Count-Rows($Sql) { return (Invoke-TestQuery $ctx $Sql).Rows[0][0] }
function N($Value) { return ([decimal]$Value).ToString('0.#', [System.Globalization.CultureInfo]::InvariantCulture) }   # 55.0 and 55 compare equal
$from2026 = [datetime]'2026-01-01'
$to2026 = [datetime]'2026-12-31'

# =====================================================================================
# SECTION 1 - PRE-MERGE (this slice's own repositories only)
# =====================================================================================
Write-Host ''
Write-Host '--- Section 1: pre-merge ---' -ForegroundColor Cyan

# --- SessionSearchController: FindSessions ---
$search = New-Object FuturePath.ApplicationLayer.SessionSearchController
$all = $search.FindSessions($advisorId, '', '', $null, $null, '', $null)
Assert-Equal 11 $all.Rows.Count 'Search with no filters lists every session'
Assert-Equal 'SessionID,SessionStatus,SessionNumber,SessionDate,LearnerName,Grade,ProgrammeName,OutcomeCode,IsExported' (($all.Columns | ForEach-Object { $_.ColumnName }) -join ',') 'Search columns match the UI 10.2b grid'
Assert-Equal 4 $all.Rows[0].SessionID 'Newest session first'
Assert-Equal 11 $all.Rows[10].SessionID 'Oldest session last'
$s2 = $all.Select('SessionID = 2')[0]
Assert-Equal '2026-0002' $s2.SessionNumber 'SessionNumber is built as yyyy-NNNN'
Assert-Equal 'Aadiel-Raboo Magwa' $s2.LearnerName 'LearnerName is first name and surname'
Assert-Equal 'Grade 12' $s2.Grade 'Grade is the grade description'
Assert-Equal 'BSc Computer Science' $s2.ProgrammeName 'ProgrammeName is the selected programme'
Assert-Equal 'NCE' $s2.OutcomeCode 'OutcomeCode is read back from the selected result'
Assert-Equal $false $s2.IsExported 'Session without an export is not exported'
Assert-Equal $true ($all.Select('SessionID = 1')[0]).IsExported 'Session with an ExportReport row is exported'
Assert-Equal '' ($all.Select('SessionID = 4')[0]).OutcomeCode 'Session without a result has OutcomeCode ""'
Assert-Equal 'In progress' ($all.Select('SessionID = 4')[0]).SessionStatus 'SessionStatus is returned'
$again = $search.FindSessions($advisorId, '', '', $null, $null, '', $null)
Assert-Equal 11 $again.Rows.Count 'A second search on the same controller works (new table each time)'

Assert-Equal 1 $search.FindSessions($advisorId, '2026-0002', '', $null, $null, '', $null).Rows.Count 'Session number 2026-0002 finds one session'
Assert-Equal 0 $search.FindSessions($advisorId, '2025-0002', '', $null, $null, '', $null).Rows.Count 'Session number with the wrong year finds none'
Assert-Equal 11 $search.FindSessions($advisorId, '2025-0011', '', $null, $null, '', $null).Rows[0].SessionID 'Session number 2025-0011 finds the 2025 session'
Assert-Equal 2 $search.FindSessions($advisorId, '', 'Mag', $null, $null, '', $null).Rows.Count 'Surname is a prefix match'
Assert-Equal 2 $search.FindSessions($advisorId, '', 'mag', $null, $null, '', $null).Rows.Count 'Surname match ignores case'
Assert-Equal 0 $search.FindSessions($advisorId, '', 'agwa', $null, $null, '', $null).Rows.Count 'Surname does not match in the middle'
Assert-Equal 0 $search.FindSessions($advisorId, '', '%', $null, $null, '', $null).Rows.Count 'A typed % is not a wildcard'
Assert-Equal 3 $search.FindSessions($advisorId, '', '', [datetime]'2026-06-15', [datetime]'2026-06-15', '', $null).Rows.Count 'Date range of one day includes the whole day'
Assert-Equal 6 $search.FindSessions($advisorId, '', '', [datetime]'2026-06-15', $null, '', $null).Rows.Count 'From-date only'
Assert-Equal 2 $search.FindSessions($advisorId, '', '', $null, [datetime]'2026-03-10', '', $null).Rows.Count 'To-date only'
Assert-Equal 3 $search.FindSessions($advisorId, '', '', $null, $null, 'NCE', $null).Rows.Count 'Outcome filter NCE'
Assert-Equal 3 $search.FindSessions($advisorId, '', '', $null, $null, 'nce', $null).Rows.Count 'Outcome filter ignores case'
Assert-Equal 2 $search.FindSessions($advisorId, '', '', $null, $null, '', 2).Rows.Count 'Learner filter'
Assert-Equal 11 $search.FindSessions($advisorId, '', '', $null, $null, '', 0).Rows.Count 'Learner 0 means no learner filter'
Assert-Equal 7 $search.FindSessions($advisorId, '', 'Magwa', $null, $null, 'STRONG', $null).Rows[0].SessionID 'Filters combine (surname and outcome)'
Assert-Throws { $search.FindSessions($advisorId, '2026-12', '', $null, $null, '', $null) } 'ArgumentException' 'Badly formed session number is refused'
Assert-Throws { $search.FindSessions($advisorId, '', '', [datetime]'2026-02-01', [datetime]'2026-01-01', '', $null) } 'ArgumentException' 'From-date after to-date is refused'
Assert-Throws { $search.FindSessions($advisorId, '', '', $null, $null, 'XYZ', $null) } 'ArgumentException' 'Unknown outcome code is refused'
Assert-Throws { $search.FindSessions($inactiveId, '', '', $null, $null, '', $null) } 'UnauthorizedAccessException' 'Inactive account cannot search sessions'
Assert-Throws { $search.FindSessions($unknownId, '', '', $null, $null, '', $null) } 'UnauthorizedAccessException' 'Unknown user cannot search sessions'

# --- ValidateSearch, CountSessions, FindSessionId, GetDailySummary ---
Assert-Equal 0 $search.ValidateSearch('2026-0001', [datetime]'2026-01-01', [datetime]'2026-02-01').Count 'Valid search has no messages'
Assert-Equal 0 $search.ValidateSearch('', $null, $null).Count 'Blank search has no messages'
Assert-Equal 1 $search.ValidateSearch('2026-12', $null, $null).Count 'Bad session number gives one message'
Assert-Equal 2 $search.ValidateSearch('abc', [datetime]'2026-02-01', [datetime]'2026-01-01').Count 'Bad number and reversed dates give two messages'
Assert-Equal 11 $search.CountSessions($advisorId) 'CountSessions counts every session'
Assert-Throws { $search.CountSessions($inactiveId) } 'UnauthorizedAccessException' 'Inactive account cannot count sessions'
Assert-Equal 2 $search.FindSessionId('2026-0002') 'FindSessionId hit'
Assert-Equal 2 $search.FindSessionId(' 2026-0002 ') 'FindSessionId ignores surrounding spaces'
Assert-True ($search.FindSessionId('2025-0002') -eq $null) 'FindSessionId with the wrong year is null'
Assert-True ($search.FindSessionId('2026-0099') -eq $null) 'FindSessionId for a missing session is null'
Assert-True ($search.FindSessionId('abc') -eq $null) 'FindSessionId with a bad format is null'
Assert-True ($search.FindSessionId('') -eq $null) 'FindSessionId with blank text is null'

$completed = 0; $inProgress = 0; $exceptions = 0
$search.GetDailySummary($advisorId, [datetime]'2026-06-15T14:00:00', [ref]$completed, [ref]$inProgress, [ref]$exceptions)
Assert-Equal 2 $completed 'Daily summary: completed that day (by DateCompleted)'
Assert-Equal 1 $inProgress 'Daily summary: started that day and still in progress'
Assert-Equal 1 $exceptions 'Daily summary: completed that day as NCE or MOREINFO'
Assert-Throws { $search.GetDailySummary($inactiveId, [datetime]'2026-06-15', [ref]$completed, [ref]$inProgress, [ref]$exceptions) } 'UnauthorizedAccessException' 'Inactive account cannot read the daily summary'

# --- ExportReportRepository ---
$exportRepository = New-Object FuturePath.DatabaseLayer.ExportReportRepository
Assert-True ($exportRepository.GetLastGenerated('FP-DIE') -eq $null) 'GetLastGenerated is null for a report never generated'
Assert-Equal '2026-03-10 10:00' $exportRepository.GetLastGenerated('FP-LSR').ToString('yyyy-MM-dd HH:mm') 'GetLastGenerated reads the latest FP-LSR row'
$entry = New-Object FuturePath.ApplicationLayer.ExportReport('FP-DIE', $null, 'Period test', $advisorId, [datetime]'2026-08-01T12:00:00', 'XLSX')
$newId = $exportRepository.Insert($entry)
Assert-True ($newId -gt 0) 'Insert returns the new ExportID'
Assert-Equal $newId $entry.ExportID 'Insert sets ExportID on the entity'
$stored = (Invoke-TestQuery $ctx "SELECT * FROM ExportReport WHERE ExportID = $newId").Rows[0]
Assert-True ($stored.SessionID -is [DBNull]) 'FP-DIE row stores no SessionID'
Assert-Equal 'Period test' $stored.ParametersUsed 'ParametersUsed is stored'
Assert-Equal $advisorId $stored.GeneratedByUserID 'GeneratedByUserID is stored'
Assert-Equal 'XLSX' $stored.ExportFormat 'ExportFormat is stored'
Assert-Equal '2026-08-01 12:00' $exportRepository.GetLastGenerated('FP-DIE').ToString('yyyy-MM-dd HH:mm') 'GetLastGenerated sees the new row'
$learnerEntry = New-Object FuturePath.ApplicationLayer.ExportReport('FP-LSR', 1, $null, $null, [datetime]'2026-03-11T08:00:00', 'DOCX')
$secondId = $exportRepository.Insert($learnerEntry)
Assert-True ($secondId -gt $newId) 'A second Insert on the same repository gets a new key'
$stored = (Invoke-TestQuery $ctx "SELECT * FROM ExportReport WHERE ExportID = $secondId").Rows[0]
Assert-True ($stored.GeneratedByUserID -is [DBNull]) 'Learner export stores GeneratedByUserID NULL'
Assert-True ($stored.ParametersUsed -is [DBNull]) 'Learner export stores ParametersUsed NULL'

# --- ReportController: names and formats ---
$reports = New-Object FuturePath.ApplicationLayer.ReportController
Assert-Equal 'Learners Not Currently Eligible' $reports.GetReportName('FP-LER') 'GetReportName FP-LER'
Assert-Equal 'Degree Interest and Eligibility' $reports.GetReportName('fp-die') 'GetReportName ignores case'
Assert-Equal '' $reports.GetReportName('FP-XYZ') 'GetReportName of an unknown code is ""'
$formats = $reports.GetExportFormats('FP-LSR')
Assert-Equal 'ExportFormat,FormatName,FormatCode,RenderFormat' (($formats.Columns | ForEach-Object { $_.ColumnName }) -join ',') 'GetExportFormats columns'
Assert-Equal '.pdf|.docx' (($formats.Rows | ForEach-Object { $_.ExportFormat }) -join '|') 'FP-LSR: PDF or Word'
Assert-Equal 'PDF|DOCX' (($formats.Rows | ForEach-Object { $_.FormatCode }) -join '|') 'FP-LSR format codes'
Assert-Equal 'PDF|WORDOPENXML' (($formats.Rows | ForEach-Object { $_.RenderFormat }) -join '|') 'FP-LSR render formats'
$formats = $reports.GetExportFormats('FP-DIE')
Assert-Equal '.pdf|.xlsx' (($formats.Rows | ForEach-Object { $_.ExportFormat }) -join '|') 'FP-DIE: PDF or Excel'
Assert-Equal 'PDF|EXCELOPENXML' (($formats.Rows | ForEach-Object { $_.RenderFormat }) -join '|') 'FP-DIE render formats'
Assert-Equal 'Excel workbook (.xlsx)' $formats.Rows[1].FormatName 'Format names are written for people'
$formats = $reports.GetExportFormats('FP-LER')
Assert-Equal '.pdf|.docx' (($formats.Rows | ForEach-Object { $_.ExportFormat }) -join '|') 'FP-LER: PDF or Word'
Assert-Equal 0 $reports.GetExportFormats('nope').Rows.Count 'Unknown report has no formats'
Assert-Equal '2026-08-01 12:00' $reports.GetLastGenerated('FP-DIE').ToString('yyyy-MM-dd HH:mm') 'ReportController.GetLastGenerated'
Assert-True ($reports.GetLastGenerated('nope') -eq $null) 'GetLastGenerated of an unknown code is null'

# --- BuildParametersUsed ---
$text = $reports.BuildParametersUsed('FP-DIE', [datetime]'2026-01-01', [datetime]'2026-09-15', 1, $null, '', $false)
Assert-Equal 'Period 2026-01-01..2026-09-15; Faculty=Commerce; Grade=All; Outcome=All; Inactive=Excluded' $text 'Parameters text uses names (faculty filter)'
$text = $reports.BuildParametersUsed('FP-DIE', [datetime]'2026-01-01', [datetime]'2026-09-15', 0, 2, 'NCE', $true)
Assert-Equal 'Period 2026-01-01..2026-09-15; Faculty=All; Grade=Grade 12; Outcome=Not Currently Eligible; Inactive=Included' $text 'Parameters text: grade, outcome and inactive included'
$text = $reports.BuildParametersUsed('FP-LER', [datetime]'2026-01-01', [datetime]'2026-09-15', 2, 1, 'STRONG', $true)
Assert-Equal 'Period 2026-01-01..2026-09-15; Faculty=Science; Grade=Grade 11; Outcome=Not Currently Eligible or More Information Needed; Inactive=Excluded' $text 'FP-LER parameters always show its two outcomes and Inactive=Excluded'
Assert-True ($text.Length -le 255) 'Parameters text is at most 255 characters'
Assert-True ($reports.BuildParametersUsed('FP-LSR', $from2026, $to2026, $null, $null, '', $false) -eq $null) 'FP-LSR has no parameters text'

# --- ValidateReportRequest ---
Assert-Equal 0 $reports.ValidateReportRequest('FP-DIE', [datetime]'2026-01-01', [datetime]'2026-09-15', 'XLSX').Count 'Valid FP-DIE request'
Assert-Equal 0 $reports.ValidateReportRequest('FP-DIE', [datetime]'2026-01-01', [datetime]'2026-09-15', '.xlsx').Count 'Format may be given as the file extension'
Assert-Equal 0 $reports.ValidateReportRequest('FP-LSR', [datetime]'2026-09-15', [datetime]'2026-09-15', 'PDF').Count 'One-day period is valid'
$messages = $reports.ValidateReportRequest('FP-DIE', [datetime]'2026-01-01', [datetime]'2026-09-15', 'DOCX')
Assert-Equal 1 $messages.Count 'FP-DIE cannot be exported as Word'
Assert-True ($messages[0] -like '*Excel*') 'The message names the formats FP-DIE allows'
Assert-Equal 1 $reports.ValidateReportRequest('FP-LER', [datetime]'2026-09-15', [datetime]'2026-01-01', 'PDF').Count 'Reversed dates give one message'
Assert-Equal 3 $reports.ValidateReportRequest('FP-XYZ', [datetime]'2026-09-15', [datetime]'2026-01-01', '').Count 'Unknown report, reversed dates and no format give three messages'

# --- GetReportCatalogue and GetScope ---
$catalogue = $reports.GetReportCatalogue($advisorId, $from2026, $to2026, $null, $null, '')
Assert-Equal 'ReportCode,ReportName,Scope,RowsInScope,LastGenerated' (($catalogue.Columns | ForEach-Object { $_.ColumnName }) -join ',') 'Catalogue columns match the UI 10.3c grid'
Assert-Equal 'FP-LSR|FP-DIE|FP-LER' (($catalogue.Rows | ForEach-Object { $_.ReportCode }) -join '|') 'Catalogue lists the three reports'
Assert-Equal '6|6|3' (($catalogue.Rows | ForEach-Object { $_.RowsInScope }) -join '|') 'Rows in scope: completed, active sessions; FP-LER only NCE and MOREINFO'
Assert-True (-not ($catalogue.Rows[0].LastGenerated -is [DBNull])) 'FP-LSR last generated is shown'
Assert-True ($catalogue.Rows[2].LastGenerated -is [DBNull]) 'FP-LER never generated is DBNull'
$catalogue = $reports.GetReportCatalogue($advisorId, $from2026, $to2026, 1, $null, '')
Assert-Equal '3|3|1' (($catalogue.Rows | ForEach-Object { $_.RowsInScope }) -join '|') 'Catalogue faculty filter'
$catalogue = $reports.GetReportCatalogue($advisorId, $from2026, $to2026, $null, 1, '')
Assert-Equal '2|2|1' (($catalogue.Rows | ForEach-Object { $_.RowsInScope }) -join '|') 'Catalogue grade filter'
$catalogue = $reports.GetReportCatalogue($advisorId, $from2026, $to2026, 0, 0, 'STRONG')
Assert-Equal '2|2|3' (($catalogue.Rows | ForEach-Object { $_.RowsInScope }) -join '|') 'Catalogue outcome filter (FP-LER ignores it)'
$catalogue = $reports.GetReportCatalogue($advisorId, $from2026, [datetime]'2026-06-14', $null, $null, '')
Assert-Equal '4|4|2' (($catalogue.Rows | ForEach-Object { $_.RowsInScope }) -join '|') 'Period ends with the to-date'
$catalogue = $reports.GetReportCatalogue($advisorId, $from2026, [datetime]'2026-06-15', $null, $null, '')
Assert-Equal '6|6|3' (($catalogue.Rows | ForEach-Object { $_.RowsInScope }) -join '|') 'The whole to-date is in the period'
$catalogue = $reports.GetReportCatalogue($advisorId, [datetime]'2025-12-31', [datetime]'2025-12-31', $null, $null, '')
Assert-Equal '1|1|0' (($catalogue.Rows | ForEach-Object { $_.RowsInScope }) -join '|') 'One-day period in 2025'
Assert-Throws { $reports.GetReportCatalogue($inactiveId, $from2026, $to2026, $null, $null, '') } 'UnauthorizedAccessException' 'Inactive account cannot read the catalogue'
Assert-Throws { $reports.GetReportCatalogue($advisorId, $to2026, $from2026, $null, $null, '') } 'ArgumentException' 'Catalogue refuses reversed dates'
Assert-Throws { $reports.GetReportCatalogue($advisorId, $from2026, $to2026, $null, $null, 'BAD') } 'ArgumentException' 'Catalogue refuses an unknown outcome'

$sessions = 0; $faculties = 0; $programmes = 0
$reports.GetScope($advisorId, 'FP-DIE', $from2026, $to2026, $null, $null, '', $false, [ref]$sessions, [ref]$faculties, [ref]$programmes)
Assert-Equal '6/2/2' "$sessions/$faculties/$programmes" 'FP-DIE scope: inactive programmes and faculties excluded'
$reports.GetScope($advisorId, 'FP-DIE', $from2026, $to2026, $null, $null, '', $true, [ref]$sessions, [ref]$faculties, [ref]$programmes)
Assert-Equal '8/3/4' "$sessions/$faculties/$programmes" 'FP-DIE scope with inactive included'
$reports.GetScope($advisorId, 'FP-LER', $from2026, $to2026, $null, $null, 'STRONG', $true, [ref]$sessions, [ref]$faculties, [ref]$programmes)
Assert-Equal '3/2/2' "$sessions/$faculties/$programmes" 'FP-LER scope ignores the outcome filter and never includes inactive'
$reports.GetScope($adminId, 'FP-LSR', $from2026, $to2026, 2, $null, '', $true, [ref]$sessions, [ref]$faculties, [ref]$programmes)
Assert-Equal '3/1/1' "$sessions/$faculties/$programmes" 'FP-LSR scope with a faculty filter (inactive never included)'
Assert-Throws { $reports.GetScope($advisorId, 'FP-XYZ', $from2026, $to2026, $null, $null, '', $false, [ref]$sessions, [ref]$faculties, [ref]$programmes) } 'ArgumentException' 'GetScope refuses an unknown report'
Assert-Throws { $reports.GetScope($inactiveId, 'FP-DIE', $from2026, $to2026, $null, $null, '', $false, [ref]$sessions, [ref]$faculties, [ref]$programmes) } 'UnauthorizedAccessException' 'Inactive account cannot read the scope'

# --- RecordExport ---
$before = Count-Rows 'SELECT COUNT(*) FROM ExportReport'
$reports.RecordExport($advisorId, 'FP-LER', $null, 'Period 2026-01-01..2026-12-31; Faculty=All', 'PDF', [datetime]'2026-08-02T09:00:00')
$row = (Invoke-TestQuery $ctx 'SELECT TOP 1 * FROM ExportReport ORDER BY ExportID DESC').Rows[0]
Assert-Equal 'FP-LER' $row.ReportCode 'RecordExport writes an FP-LER row'
Assert-Equal $advisorId $row.GeneratedByUserID 'RecordExport stores the staff member'
Assert-Equal 'Period 2026-01-01..2026-12-31; Faculty=All' $row.ParametersUsed 'RecordExport stores the parameters'
Assert-True ($row.SessionID -is [DBNull]) 'FP-LER row has no SessionID'
$reports.RecordExport($adminId, 'fp-die', 5, 'Period x', '.xlsx', [datetime]'2026-08-03T09:00:00')
$row = (Invoke-TestQuery $ctx 'SELECT TOP 1 * FROM ExportReport ORDER BY ExportID DESC').Rows[0]
Assert-Equal 'FP-DIE|XLSX' ($row.ReportCode + '|' + $row.ExportFormat) 'RecordExport normalises the code and the extension'
Assert-True ($row.SessionID -is [DBNull]) 'FP-DIE never stores a SessionID'
$reports.RecordExport($advisorId, 'FP-LSR', 1, 'ignored', 'DOCX', [datetime]'2026-08-04T09:00:00')
$row = (Invoke-TestQuery $ctx 'SELECT TOP 1 * FROM ExportReport ORDER BY ExportID DESC').Rows[0]
Assert-Equal 1 $row.SessionID 'FP-LSR export stores the session'
Assert-True ($row.ParametersUsed -is [DBNull]) 'FP-LSR export stores no parameters'
$reports.RecordExport($advisorId, 'FP-DIE', $null, ('x' * 300), 'PDF', [datetime]'2026-08-05T09:00:00')
Assert-Equal 255 (Count-Rows 'SELECT TOP 1 LEN(ParametersUsed) FROM ExportReport ORDER BY ExportID DESC') 'Long parameters are cut to 255 characters'
$before = Count-Rows 'SELECT COUNT(*) FROM ExportReport'
Assert-Throws { $reports.RecordExport($advisorId, 'FP-DIE', $null, 'p', 'DOCX', [datetime]'2026-08-06') } 'ArgumentException' 'FP-DIE cannot be recorded as Word'
Assert-Throws { $reports.RecordExport($advisorId, 'FP-LER', $null, 'p', 'XLSX', [datetime]'2026-08-06') } 'ArgumentException' 'FP-LER cannot be recorded as Excel'
Assert-Throws { $reports.RecordExport($advisorId, 'FP-LER', $null, 'p', 'TXT', [datetime]'2026-08-06') } 'ArgumentException' 'Unknown format is refused'
Assert-Throws { $reports.RecordExport($advisorId, 'FP-XYZ', $null, 'p', 'PDF', [datetime]'2026-08-06') } 'ArgumentException' 'Unknown report is refused'
Assert-Throws { $reports.RecordExport($advisorId, 'FP-LSR', $null, $null, 'PDF', [datetime]'2026-08-06') } 'ArgumentException' 'FP-LSR without a session is refused'
Assert-Throws { $reports.RecordExport($advisorId, 'FP-LSR', 999, $null, 'PDF', [datetime]'2026-08-06') } 'ArgumentException' 'FP-LSR for a missing session is refused'
Assert-Throws { $reports.RecordExport($advisorId, 'FP-LSR', 4, $null, 'PDF', [datetime]'2026-08-06') } 'InvalidOperationException' 'FP-LSR for an in-progress session is refused'
Assert-Throws { $reports.RecordExport($inactiveId, 'FP-DIE', $null, 'p', 'PDF', [datetime]'2026-08-06') } 'UnauthorizedAccessException' 'Inactive account cannot record an export'
Assert-Throws { $reports.RecordExport($unknownId, 'FP-DIE', $null, 'p', 'PDF', [datetime]'2026-08-06') } 'UnauthorizedAccessException' 'Unknown user cannot record an export'
Assert-Equal $before (Count-Rows 'SELECT COUNT(*) FROM ExportReport') 'Refused exports write no row'

# --- Checks that run before any other slice's repository is used ---
Assert-Throws { $reports.CompleteLearnerSession(6, 'XLSX', [datetime]'2026-06-15T12:00:00') } 'ArgumentException' 'Learner summary cannot be exported as Excel'
Assert-Throws { $reports.CompleteLearnerSession(6, '', [datetime]'2026-06-15T12:00:00') } 'ArgumentException' 'Learner summary needs a format'
Assert-Throws { $reports.GetLearnerSummary($inactiveId, 1) } 'UnauthorizedAccessException' 'Inactive account cannot regenerate FP-LSR'
Assert-Throws { $reports.BuildDegreeInterestReport($inactiveId, $from2026, $to2026, $null, $null, '', $false) } 'UnauthorizedAccessException' 'Inactive account cannot build FP-DIE'
Assert-Throws { $reports.BuildDegreeInterestReport($unknownId, $from2026, $to2026, $null, $null, '', $false) } 'UnauthorizedAccessException' 'Unknown user cannot build FP-DIE'
Assert-Throws { $reports.BuildDegreeInterestReport($advisorId, $to2026, $from2026, $null, $null, '', $false) } 'ArgumentException' 'FP-DIE refuses reversed dates'
Assert-Throws { $reports.BuildDegreeInterestReport($advisorId, $from2026, $to2026, $null, $null, 'BAD', $false) } 'ArgumentException' 'FP-DIE refuses an unknown outcome'
Assert-Throws { $reports.BuildNotEligibleReport($inactiveId, $from2026, $to2026, $null, $null) } 'UnauthorizedAccessException' 'Inactive account cannot build FP-LER'
Assert-Throws { $reports.BuildNotEligibleReport($advisorId, $to2026, $from2026, $null, $null) } 'ArgumentException' 'FP-LER refuses reversed dates'

Write-Host ''
Write-Host ("Section 1 (pre-merge): {0} passed, {1} failed" -f $script:TestsPassed, $script:TestsFailed) -ForegroundColor $(if ($script:TestsFailed -eq 0) { 'Green' } else { 'Red' })

# =====================================================================================
# SECTION 2 - POST-MERGE: FP-LSR, FP-DIE and FP-LER filling
# Needs LearnerRepository, AdvisorySessionRepository, DegreeProgrammeRepository,
# FundingRepository and InterestRepository (stubs until the other slices are merged).
# =====================================================================================
if ($SkipPostMerge) {
    Complete-FuturePathTest $ctx
    return
}
Write-Host ''
Write-Host '--- Section 2: POST-MERGE (expected to fail with NotImplementedException before the merge) ---' -ForegroundColor Cyan
$prePassed = $script:TestsPassed
$preFailed = $script:TestsFailed

function Invoke-PostMerge {
    <# Runs one group of post-merge checks; an exception fails the group with its message. #>
    param([string]$Label, [scriptblock]$Block)
    try {
        & $Block
    } catch {
        $exception = $_.Exception
        while ($exception.InnerException -ne $null) { $exception = $exception.InnerException }
        $script:TestsFailed++
        $message = "FAIL  POST-MERGE $Label  ($($exception.GetType().Name): $($exception.Message))"
        $script:Failures.Add($message)
        Write-Host $message -ForegroundColor Red
    }
}

Invoke-PostMerge 'FP-LSR learner summary (session 1)' {
    $lsr = $reports.GetLearnerSummary(1)
    $header = $lsr.ReportHeader.Rows[0]
    Assert-Equal 1 $lsr.ReportHeader.Rows.Count 'FP-LSR has one header row'
    Assert-Equal '2026-0001' $header.SessionNumber 'FP-LSR session number'
    Assert-Equal 'STRONG' $header.OutcomeCode 'FP-LSR outcome is read back from the stored result'
    Assert-Equal 'Meets every stored requirement for BCom Information Systems.' $header.OutcomeReason 'FP-LSR reason is the stored reason'
    Assert-Equal 'Grade 12|Final' ($header.GradeDescription + '|' + $header.MarkBasis) 'FP-LSR grade and mark basis'
    Assert-Equal 4 $header.SubjectCount 'FP-LSR subject count'
    Assert-Equal 73 (N $header.OverallAverage) 'FP-LSR overall average'
    Assert-Equal 'Met' $header.AverageResultCode 'FP-LSR average meets the minimum'
    Assert-Equal '4/4' ("{0}/{1}" -f $header.RequirementsMet, $header.RequirementsChecked) 'FP-LSR requirements met (average included)'
    Assert-Equal 3 $header.QuestionsAnswered 'FP-LSR questions answered'
    Assert-Equal 'Commerce' $header.FacultyName 'FP-LSR faculty of the selected programme'
    Assert-Equal $true $header.IncludeAlternatives 'FP-LSR prints alternatives when ticked'
    Assert-True ($header.OverallGuidance.Length -gt 0) 'FP-LSR has overall guidance'
    Assert-Equal 'Accounting|English Home Language|Life Sciences|Mathematics' (($lsr.SubjectRequirements.Rows | ForEach-Object { $_.SubjectName }) -join '|') 'Table 1 subjects by name'
    Assert-Equal 'NotAssessed' ($lsr.SubjectRequirements.Select("SubjectName = 'Life Sciences'")[0]).ResultCode 'A subject that is not a requirement is NotAssessed'
    Assert-Equal 4 $lsr.MarkComparison.Rows.Count 'Chart 1: three subjects with a minimum plus the overall average'
    Assert-Equal 'Business and finance' $lsr.InterestProfile.Rows[0].CategoryName 'Interest profile highest first, active categories only'
    Assert-Equal 2 $lsr.InterestProfile.Rows.Count 'Inactive interest category left out'
    Assert-Equal 'BSc Computer Science' (($lsr.Alternatives.Rows | ForEach-Object { $_.ProgrammeName }) -join '|') 'Alternatives: inactive programme left out'
    Assert-Equal 42 (N $lsr.Alternatives.Rows[0].OverallScore) 'Alternative fit is the stored OverallScore'
    Assert-Equal 'Commerce Bursary' (($lsr.FundingOptions.Rows | ForEach-Object { $_.FundingName }) -join '|') 'Funding: inactive and expired options left out'
}

Invoke-PostMerge 'FP-LSR staff regeneration (session 2, missing subject)' {
    $lsr = $reports.GetLearnerSummary($advisorId, 2)
    $missing = $lsr.SubjectRequirements.Select("SubjectName = 'Physical Sciences'")[0]
    Assert-True ($missing.IsMarkNull()) 'Required subject not taken has no mark'
    Assert-Equal 'Gap' $missing.ResultCode 'Required subject not taken is a Gap'
    Assert-Equal 10 (N ($lsr.SubjectRequirements.Select("SubjectName = 'Mathematics'")[0]).Shortfall) 'Mark gap shortfall in points'
    Assert-Equal 'Gap|7.5' ($lsr.ReportHeader.Rows[0].AverageResultCode + '|' + (N $lsr.ReportHeader.Rows[0].AverageShortfall)) 'Average below the minimum'
    Assert-Equal 0 $lsr.Alternatives.Rows.Count 'Alternatives not ticked: none printed'
    Assert-Equal 'Science Scholarship' (($lsr.FundingOptions.Rows | ForEach-Object { $_.FundingName }) -join '|') 'Funding linked to the selected programme'
}

Invoke-PostMerge 'FP-LSR refusals' {
    Assert-Throws { $reports.GetLearnerSummary($advisorId, 4) } 'InvalidOperationException' 'Staff cannot regenerate an in-progress session'
    Assert-Throws { $reports.GetLearnerSummary(4) } 'InvalidOperationException' 'Learner summary needs the eligibility check first'
    Assert-Throws { $reports.CompleteLearnerSession(4, 'PDF', [datetime]'2026-09-01T12:00:00') } 'InvalidOperationException' 'Completing needs the eligibility check first'
}

Invoke-PostMerge 'FP-DIE (2026, no filters)' {
    $die = $reports.BuildDegreeInterestReport($adminId, $from2026, $to2026, $null, $null, '', $false)
    $header = $die.ReportHeader.Rows[0]
    Assert-Equal 6 $header.SessionsInScope 'FP-DIE sessions in scope'
    Assert-Equal 4 $header.LearnerCount 'FP-DIE distinct learners'
    Assert-Equal 50 (N $header.EligibilityRate) 'FP-DIE eligibility rate'
    Assert-Equal 'Andile Admin, Administrator' $header.GeneratedByName 'FP-DIE generated by'
    Assert-Equal 'All faculties|All grades|All outcomes' ($header.FacultyFilter + '|' + $header.GradeFilter + '|' + $header.OutcomeFilter) 'FP-DIE filter text'
    Assert-Equal 'STRONG:2|POSSIBLE:1|NCE:2|MOREINFO:1' (($die.OutcomeSummary.Rows | ForEach-Object { $_.OutcomeCode + ':' + $_.SessionCount }) -join '|') 'FP-DIE outcome summary, fixed order'
    Assert-Equal 2 $die.FacultyOutcomes.Rows.Count 'FP-DIE faculties'
    Assert-Equal 8 $die.FacultyOutcomeSlices.Rows.Count 'FP-DIE four slices per faculty'
    Assert-Equal 2 $die.ProgrammeOutcomes.Rows.Count 'FP-DIE programmes'
    Assert-Equal 'Mathematics:1|Physical Sciences:1' (($die.SubjectGaps.Rows | ForEach-Object { $_.SubjectName + ':' + $_.GapCount }) -join '|') 'FP-DIE required subjects not taken'
    Assert-Equal 'Mathematics:2:5.5:55' (($die.MarkShortfalls.Rows | ForEach-Object { "{0}:{1}:{2}:{3}" -f $_.SubjectName, $_.SessionCount, (N $_.AverageShortfall), (N $_.LowestMark) }) -join '|') 'FP-DIE marks below the minimum'
    Assert-Equal '2/2/5.5' ("{0}/{1}/{2}" -f $header.SubjectGapSessions, $header.MarkGapSessions, (N $header.AverageShortfall)) 'FP-DIE gap totals'
    Assert-Equal 4 $die.FundingCategories.Rows.Count 'FP-DIE one row per funding type'
    Assert-Equal 'Bursary:3:1:Commerce' ("{0}:{1}:{2}:{3}" -f $die.FundingCategories.Rows[0].FundingType, $die.FundingCategories.Rows[0].OfferedCount, $die.FundingCategories.Rows[0].AddedToSummaryCount, $die.FundingCategories.Rows[0].MostLinkedFaculty) 'FP-DIE funding category'
    $inactive = $reports.BuildDegreeInterestReport($adminId, $from2026, $to2026, $null, $null, '', $true)
    Assert-Equal 8 $inactive.ReportHeader.Rows[0].SessionsInScope 'FP-DIE with inactive programmes included'
    $nce = $reports.BuildDegreeInterestReport($advisorId, $from2026, $to2026, $null, $null, 'NCE', $false)
    Assert-Equal 2 $nce.ReportHeader.Rows[0].SessionsInScope 'FP-DIE outcome filter'
}

Invoke-PostMerge 'FP-LER (2026, no filters)' {
    $ler = $reports.BuildNotEligibleReport($advisorId, $from2026, $to2026, $null, $null)
    $header = $ler.ReportHeader.Rows[0]
    Assert-Equal '6/3/50' ("{0}/{1}/{2}" -f $header.SessionsInScope, $header.ExceptionCount, (N $header.ExceptionPercent)) 'FP-LER scope, exceptions and percentage'
    Assert-Equal '2/1/3' ("{0}/{1}/{2}" -f $header.NotEligibleCount, $header.MoreInfoNeededCount, $header.LearnersAffected) 'FP-LER split and learners affected'
    Assert-Equal '5|2|3' (($ler.ExceptionListing.Rows | ForEach-Object { $_.SessionID }) -join '|') 'FP-LER order: faculty, NCE before MOREINFO, newest first'
    Assert-Equal 'BCom Information Systems' $ler.ExceptionListing.Rows[1].AlternativeProgramme 'FP-LER alternative skips an inactive rank-1 programme'
    Assert-Equal 'Physical Sciences is required but was not captured.' $ler.ExceptionListing.Rows[1].OutcomeReason 'FP-LER reason is the first sentence of the stored reason'
    Assert-Equal 11 $ler.ExceptionListing.Rows[2].Grade 'FP-LER grade as 11 or 12'
    Assert-Equal 5 $ler.ReasonSummary.Rows.Count 'FP-LER reason categories'
    Assert-Equal 'Science' $ler.FacultySummary.Rows[0].FacultyName 'FP-LER faculty with most follow-ups first'
    $commerce = $reports.BuildNotEligibleReport($advisorId, $from2026, $to2026, 1, $null)
    Assert-Equal '3/1' ("{0}/{1}" -f $commerce.ReportHeader.Rows[0].SessionsInScope, $commerce.ReportHeader.Rows[0].ExceptionCount) 'FP-LER faculty filter'
}

Invoke-PostMerge 'CompleteLearnerSession (session 6)' {
    $reports.CompleteLearnerSession(6, 'DOCX', [datetime]'2026-06-15T12:00:00')
    $session = (Invoke-TestQuery $ctx 'SELECT SessionStatus, DateCompleted FROM AdvisorySession WHERE SessionID = 6').Rows[0]
    Assert-Equal 'Completed' $session.SessionStatus 'Save and export completes the session'
    $row = (Invoke-TestQuery $ctx 'SELECT TOP 1 * FROM ExportReport ORDER BY ExportID DESC').Rows[0]
    Assert-Equal 'FP-LSR|6|DOCX' ("{0}|{1}|{2}" -f $row.ReportCode, $row.SessionID, $row.ExportFormat) 'Save and export logs FP-LSR'
    Assert-True ($row.GeneratedByUserID -is [DBNull]) 'A learner export has no GeneratedByUserID'
}

Write-Host ''
Write-Host ("Section 2 (post-merge): {0} passed, {1} failed" -f ($script:TestsPassed - $prePassed), ($script:TestsFailed - $preFailed)) -ForegroundColor $(if ($script:TestsFailed -eq $preFailed) { 'Green' } else { 'Yellow' })

Complete-FuturePathTest $ctx
