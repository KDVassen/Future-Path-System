<#
.SYNOPSIS
    Runs the learner journey through the real classes against a copy of the database: profile
    (UI 2), subjects (UI 3), answers (UI 4), programme choice (UI 5), Sequence 1 (CheckEligibility),
    UI 6 grid and tiles, Sequence 2 (GenerateAlternatives), the More Information Needed paths,
    the staff learner search, and the programme / requirement maintenance used by the admin slice.
    The database copy holds the empty schema; this script inserts its own fixture rows.
    Run: powershell -NoProfile -ExecutionPolicy Bypass -File tests\Learner-Smoke.ps1
#>
. "$PSScriptRoot\TestHelpers.ps1"

$ctx = Initialize-FuturePathTest -SchemaOnly   # empty schema: this script inserts its own fixture rows

function Get-Scalar($sql) { return (Invoke-TestQuery $ctx $sql).Rows[0][0] }

# --- Fixture rows (fixed IDs so the checks below can name them) -----------------------------
$parameterRows = @()
$settings = Get-ProvisionalSettings
foreach ($code in $settings.Keys) { $parameterRows += "('$code', '$($settings[$code])')" }
$provinces = @('Eastern Cape', 'Free State', 'Gauteng', 'KwaZulu-Natal', 'Limpopo', 'Mpumalanga', 'North West', 'Northern Cape', 'Western Cape')
for ($i = 0; $i -lt $provinces.Count; $i++) { $parameterRows += "('PROVINCE_$($i + 1)', '$($provinces[$i])')" }

[void](Invoke-TestQuery $ctx (
    "INSERT INTO GradeLevel (GradeLevelID, GradeDescription, MarkBasis) VALUES (1, 'Grade 11', 'Projected'), (2, 'Grade 12', 'Final'); " +
    "INSERT INTO SystemParameter (ParameterCode, ParameterValue) VALUES " + ($parameterRows -join ', ') + "; " +
    "SET IDENTITY_INSERT Faculty ON; " +
    "INSERT INTO Faculty (FacultyID, FacultyName, IsActive) VALUES (1, 'Commerce', 1), (2, 'Science', 1), (3, 'Humanities', 0); " +
    "SET IDENTITY_INSERT Faculty OFF; " +
    "SET IDENTITY_INSERT Subject ON; " +
    "INSERT INTO Subject (SubjectID, SubjectName, SubjectLevel, IsActive) VALUES " +
    "(1, 'English Home Language', 'Home Language', 1), (2, 'Mathematics', 'Core', 1), (3, 'Mathematical Literacy', 'Core', 1), " +
    "(4, 'Physical Sciences', 'Elective', 1), (5, 'Accounting', 'Elective', 1), (6, 'Business Studies', 'Elective', 1), " +
    "(7, 'Economics', 'Elective', 1), (8, 'Life Orientation', 'Core', 1), (9, 'Old Subject', 'Elective', 0); " +
    "SET IDENTITY_INSERT Subject OFF; " +
    "SET IDENTITY_INSERT DegreeProgramme ON; " +
    "INSERT INTO DegreeProgramme (ProgrammeID, FacultyID, ProgrammeName, MinimumOverallAverage, IsActive) VALUES " +
    "(1, 1, 'BCom Information Systems', NULL, 1), (2, 2, 'BSc Computer Science', NULL, 1), (3, 2, 'BSc Biological Sciences', NULL, 1), " +
    "(4, 1, 'BCom Old Programme', NULL, 0), (5, 3, 'BA Humanities', NULL, 1), (6, 1, 'BCom Accounting', 65, 1); " +
    "SET IDENTITY_INSERT DegreeProgramme OFF; " +
    "INSERT INTO ProgrammeRequirement (ProgrammeID, SubjectID, RequirementType, MinimumMark, AlternativeSubjectID, AlternativeMinimumMark, AdvisoryNote, IsActive) VALUES " +
    "(1, 1, 'Required', 50, NULL, NULL, NULL, 1), (1, 2, 'Required', 60, NULL, NULL, 'Mathematics is needed for the quantitative courses.', 1), " +
    "(1, 5, 'Recommended', 60, NULL, NULL, NULL, 1), " +
    "(2, 1, 'Required', 50, NULL, NULL, NULL, 1), (2, 2, 'Required', 70, NULL, NULL, NULL, 1), (2, 4, 'Required', 50, NULL, NULL, NULL, 0), " +
    "(3, 1, 'Required', 50, NULL, NULL, NULL, 1), (3, 2, 'Required', 60, NULL, NULL, NULL, 1), (3, 4, 'Required', 50, NULL, NULL, NULL, 1), " +
    "(4, 1, 'Required', 50, NULL, NULL, NULL, 1), (5, 1, 'Required', 50, NULL, NULL, NULL, 1), " +
    "(6, 1, 'Required', 50, NULL, NULL, NULL, 1), (6, 2, 'Required', 60, 3, 75, NULL, 1); " +
    "SET IDENTITY_INSERT InterestCategory ON; " +
    "INSERT INTO InterestCategory (CategoryID, CategoryName, IsActive) VALUES (1, 'Business and finance', 1), (2, 'Technology', 1), (3, 'Science', 1), (4, 'Old Category', 0); " +
    "SET IDENTITY_INSERT InterestCategory OFF; " +
    "SET IDENTITY_INSERT InterestQuestion ON; " +
    "INSERT INTO InterestQuestion (QuestionID, CategoryID, QuestionText, DisplayOrder, IsActive) VALUES " +
    "(1, 1, 'I enjoy business and finance.', 1, 1), (2, 2, 'I enjoy working with technology.', 2, 1), " +
    "(3, 3, 'I enjoy science experiments.', 3, 1), (4, 4, 'An old statement.', 4, 1); " +
    "SET IDENTITY_INSERT InterestQuestion OFF; " +
    "INSERT INTO DegreeInterestMapping (ProgrammeID, CategoryID, RelevanceWeight) VALUES " +
    "(1, 1, 1.00), (1, 2, 0.80), (1, 4, 0.50), (2, 2, 1.00), (3, 3, 1.00), (4, 1, 1.00), (5, 1, 1.00), (6, 1, 1.00);"))
$advisorHash = [FuturePath.ApplicationLayer.UserAccount]::HashPassword('Advisor-Test-1')
[void](Invoke-TestQuery $ctx "INSERT INTO UserAccount (Username, PasswordHash, FullName, Role, IsActive) VALUES ('smoke.advisor', '$advisorHash', 'Smoke Advisor', 'Career Advisor', 1)")
$advisorId = Get-Scalar "SELECT UserID FROM UserAccount WHERE Username = 'smoke.advisor'"
[FuturePath.ApplicationLayer.SystemSettings]::Reload()

$english = 1; $maths = 2; $mathsLit = 3; $physics = 4; $accounting = 5; $business = 6; $economics = 7; $lifeOrientation = 8; $oldSubject = 9
$commerce = 1; $science = 2; $humanities = 3
$bcomIS = 1; $bscCS = 2; $bscBio = 3; $oldProgramme = 4; $humanitiesProgramme = 5; $bcomAccounting = 6

function New-MarkList([object[]]$pairs) {
    $list = New-Object 'System.Collections.Generic.List[System.Collections.Generic.KeyValuePair[int,decimal]]'
    foreach ($pair in $pairs) {
        $list.Add((New-Object 'System.Collections.Generic.KeyValuePair[int,decimal]' -ArgumentList ([int]$pair[0]), ([decimal]$pair[1])))
    }
    return ,$list
}

function New-Responses($sessionId, [hashtable]$answers) {
    $responses = New-TypedCollection 'FuturePath.ApplicationLayer.LearnerInterestResponse'
    foreach ($questionId in $answers.Keys) {
        $responses.Add((New-Object FuturePath.ApplicationLayer.LearnerInterestResponse(0, $sessionId, $questionId, 0, $answers[$questionId])))
    }
    return ,$responses
}

$learnerController = New-Object FuturePath.ApplicationLayer.LearnerController
$eligibility = New-Object FuturePath.ApplicationLayer.EligibilityController
$recommendation = New-Object FuturePath.ApplicationLayer.RecommendationController
$learnerRepository = New-Object FuturePath.DatabaseLayer.LearnerRepository
$sessionRepository = New-Object FuturePath.DatabaseLayer.AdvisorySessionRepository
$programmeRepository = New-Object FuturePath.DatabaseLayer.DegreeProgrammeRepository
$year = [int16](Get-Date).Year

# --- UI 2: ValidateProfile ----------------------------------------------------------------------
Assert-Equal 0 $learnerController.ValidateProfile(2, 'Rondebosch High', 'Western Cape', $year, 'gadija@example.com', '082 123 4567').Count 'A valid profile has no messages'
Assert-Equal 0 $learnerController.ValidateProfile(1, 'Rondebosch High', 'western cape', ($year - 1), '', '').Count 'Province matches without case; last year is allowed; contact details are optional'
Assert-Equal 0 $learnerController.ValidateProfile(2, 'Rondebosch High', 'Gauteng', ($year + 1), '', '082-123-4567').Count 'Next year is allowed; dashes in the mobile number are removed'
Assert-Equal 1 $learnerController.ValidateProfile(3, 'Rondebosch High', 'Gauteng', $year, '', '').Count 'Grade 3 is refused'
Assert-Equal 1 $learnerController.ValidateProfile(2, '   ', 'Gauteng', $year, '', '').Count 'A blank school is refused'
Assert-Equal 1 $learnerController.ValidateProfile(2, ('S' * 101), 'Gauteng', $year, '', '').Count 'A school name over 100 characters is refused'
Assert-Equal 1 $learnerController.ValidateProfile(2, 'Rondebosch High', 'Atlantis', $year, '', '').Count 'A province that is not a PROVINCE_n value is refused'
Assert-Equal 1 $learnerController.ValidateProfile(2, 'Rondebosch High', 'Gauteng', ($year + 2), '', '').Count 'An advising year two years ahead is refused'
Assert-Equal 1 $learnerController.ValidateProfile(2, 'Rondebosch High', 'Gauteng', $year, 'not-an-email', '').Count 'A badly formed email is refused'
Assert-Equal 1 $learnerController.ValidateProfile(2, 'Rondebosch High', 'Gauteng', $year, '', '12345').Count 'A badly formed mobile number is refused'
$messages = $learnerController.ValidateProfile(0, '', '', 1990, 'x@', '99')
Assert-Equal 6 $messages.Count 'Every problem gets its own message'
Assert-True ($messages[0] -like 'Choose*grade*') ('Messages name the field and the fix: ' + $messages[0])

# --- UI 2: SaveProfile (create, then update) ------------------------------------------------------
$session = $learnerController.SaveProfile($null, 'Gadija', 'Jacobs', 2, 'Rondebosch High', 'western cape', $year, 'gadija@example.com', '082-123-4567')
$sessionId = $session.SessionID
$learnerId = $session.LearnerID
Assert-True ($sessionId -gt 0 -and $learnerId -gt 0) 'SaveProfile creates the learner and the session'
Assert-True ($session.SessionNumber -match '^\d{4}-\d{4}$') ('Session number yyyy-NNNN: ' + $session.SessionNumber)
$row = (Invoke-TestQuery $ctx "SELECT l.Province, l.ContactNumber, s.SessionStatus, s.GradeLevelID FROM Learner l JOIN AdvisorySession s ON s.LearnerID = l.LearnerID WHERE s.SessionID = $sessionId").Rows[0]
Assert-Equal 'Western Cape' $row.Province 'The province is stored as listed in SystemParameter'
Assert-Equal '082 123 4567' $row.ContactNumber 'The mobile number is stored as 0XX XXX XXXX'
Assert-Equal 'In progress' $row.SessionStatus 'A new session is In progress'

$updated = $learnerController.SaveProfile($sessionId, 'Gadija', 'Jacobs', 1, 'Wynberg Girls High', 'Western Cape', $year, '', '')
Assert-Equal $sessionId $updated.SessionID 'Updating the profile keeps the session'
Assert-Equal 1 (Get-Scalar 'SELECT COUNT(*) FROM Learner') 'Updating the profile does not add a learner'
Assert-Equal 1 (Get-Scalar 'SELECT COUNT(*) FROM AdvisorySession') 'Updating the profile does not add a session'
$row = (Invoke-TestQuery $ctx "SELECT l.SchoolName, l.ContactEmail, s.GradeLevelID FROM Learner l JOIN AdvisorySession s ON s.LearnerID = l.LearnerID WHERE s.SessionID = $sessionId").Rows[0]
Assert-Equal 'Wynberg Girls High' $row.SchoolName 'The school is updated'
Assert-True ($row.ContactEmail -is [DBNull]) 'A cleared email is stored as NULL'
Assert-Equal 1 $row.GradeLevelID 'The session grade is updated'
[void]$learnerController.SaveProfile($sessionId, 'Gadija', 'Jacobs', 2, 'Wynberg Girls High', 'Western Cape', $year, '', '')

Assert-Throws { $learnerController.SaveProfile($sessionId, ('N' * 51), 'Jacobs', 2, 'Wynberg Girls High', 'Western Cape', $year, '', '') } 'ArgumentException' 'A first name over 50 characters is refused'
Assert-Throws { $learnerController.SaveProfile($sessionId, 'Gadija', 'Jacobs', 2, '', 'Western Cape', $year, '', '') } 'ArgumentException' 'SaveProfile validates again'
Assert-Throws { $learnerController.SaveProfile(99999, 'Gadija', 'Jacobs', 2, 'School', 'Western Cape', $year, '', '') } 'ArgumentException' 'An unknown session is refused'
$done = $learnerController.SaveProfile($null, 'Done', 'Learner', 2, 'Done High', 'Gauteng', $year, '', '')
$sessionRepository.MarkCompleted($done.SessionID, (Get-Date))
Assert-Throws { $learnerController.SaveProfile($done.SessionID, 'Done', 'Learner', 2, 'Other High', 'Gauteng', $year, '', '') } 'ArgumentException' 'A completed session cannot be changed'
Assert-Equal 'Completed' (Get-Scalar "SELECT SessionStatus FROM AdvisorySession WHERE SessionID = $($done.SessionID)") 'MarkCompleted sets the status'

# --- UI 3: ValidateSubjectEntry boundaries ------------------------------------------------------
$captured = New-Object 'System.Collections.Generic.List[int]'
$captured.Add($maths)
Assert-Equal 1 $learnerController.ValidateSubjectEntry($english, '-1', $captured).Count 'Mark -1 is refused'
Assert-Equal 0 $learnerController.ValidateSubjectEntry($english, '0', $captured).Count 'Mark 0 is accepted'
Assert-Equal 0 $learnerController.ValidateSubjectEntry($english, '100', $captured).Count 'Mark 100 is accepted'
Assert-Equal 1 $learnerController.ValidateSubjectEntry($english, '101', $captured).Count 'Mark 101 is refused'
Assert-Equal 1 $learnerController.ValidateSubjectEntry($english, 'abc', $captured).Count 'Letters are refused'
Assert-Equal 0 $learnerController.ValidateSubjectEntry($english, '72.5', $captured).Count 'One decimal place is accepted'
Assert-Equal 1 $learnerController.ValidateSubjectEntry($english, '72.55', $captured).Count 'Two decimal places are refused'
$messages = $learnerController.ValidateSubjectEntry($maths, '70', $captured)
Assert-True ($messages.Count -eq 1 -and $messages[0] -eq 'Mathematics has already been captured in this session.') 'A duplicate subject is refused by name'
Assert-Equal 1 $learnerController.ValidateSubjectEntry($oldSubject, '70', $captured).Count 'An inactive subject is refused'
Assert-Equal 2 $learnerController.ValidateSubjectEntry(0, '', $captured).Count 'No subject and no mark gives two messages'

# --- UI 3: SaveSubjects -------------------------------------------------------------------------
$gadijaMarks = @(@($english, 72), @($maths, 70), @($accounting, 74), @($business, 79), @($economics, 70), @($lifeOrientation, 80))
$learnerController.SaveSubjects($sessionId, (New-MarkList $gadijaMarks))
Assert-Equal 6 (Get-Scalar "SELECT COUNT(*) FROM LearnerSubject WHERE SessionID = $sessionId") 'SaveSubjects stores six subjects'
Assert-Throws { $learnerController.SaveSubjects($sessionId, (New-MarkList @(@($english, 72), @($english, 75)))) } 'ArgumentException' 'A duplicate subject in the list is refused'
Assert-Throws { $learnerController.SaveSubjects($sessionId, (New-MarkList @())) } 'ArgumentException' 'At least one subject is needed'
Assert-Throws { $learnerController.SaveSubjects($sessionId, (New-MarkList @(@($oldSubject, 70)))) } 'ArgumentException' 'An inactive subject is refused on save'
Assert-Throws { $learnerController.SaveSubjects($sessionId, (New-MarkList @(@($english, 101)))) } 'ArgumentException' 'Mark 101 is refused on save'
Assert-Throws { $learnerController.SaveSubjects(99999, (New-MarkList @(@($english, 70)))) } 'ArgumentException' 'An unknown session is refused on save'
Assert-Equal 6 (Get-Scalar "SELECT COUNT(*) FROM LearnerSubject WHERE SessionID = $sessionId") 'Refused saves leave the stored subjects unchanged'
$gadijaMarks[0] = @($english, '72.5')
$learnerController.SaveSubjects($sessionId, (New-MarkList $gadijaMarks))
Assert-Equal 6 (Get-Scalar "SELECT COUNT(*) FROM LearnerSubject WHERE SessionID = $sessionId") 'Saving again replaces the subjects'
Assert-Equal '72.5' (Get-Scalar "SELECT Mark FROM LearnerSubject WHERE SessionID = $sessionId AND SubjectID = $english") 'A one-decimal mark is stored exactly'

# --- UI 4: responses (InterestController belongs to another slice, so the repository is used) ------
$learnerRepository.SaveResponses($sessionId, (New-Responses $sessionId @{ 1 = 5; 2 = 4; 3 = 2; 4 = 3 }))
Assert-Equal 4 (Get-Scalar "SELECT COUNT(*) FROM LearnerInterestResponse WHERE SessionID = $sessionId") 'SaveResponses stores the answers'
$learnerRepository.SaveResponses($sessionId, (New-Responses $sessionId @{ 1 = 5; 2 = 4; 3 = 2; 4 = 3 }))
Assert-Equal 4 (Get-Scalar "SELECT COUNT(*) FROM LearnerInterestResponse WHERE SessionID = $sessionId") 'Saving the answers again replaces them'

$learner = $learnerRepository.FindById($learnerId)
Assert-Equal $sessionId $learner.CurrentSessionID 'FindById loads the current session'
Assert-Equal 'Grade12' $learner.Grade 'FindById sets the grade from the session'
Assert-Equal 6 $learner.GetSubjects().Count 'FindById loads the subjects'
Assert-Equal 'Accounting' $learner.GetSubjects()[0].SubjectName 'Subjects carry their names (ordered by name)'
Assert-Equal 'Home Language' ($learner.GetSubjects() | Where-Object { $_.SubjectID -eq $english }).SubjectLevel 'Subjects carry their levels'
Assert-Equal 4 $learner.GetResponses().Count 'FindById loads the responses'
Assert-Equal 2 ($learner.GetResponses() | Where-Object { $_.QuestionID -eq 2 }).CategoryID 'Responses carry the question category'
Assert-True ($learnerRepository.FindById(99999) -eq $null) 'An unknown learner gives null'
Assert-True ($learnerRepository.FindBySessionId(99999) -eq $null) 'An unknown session gives null'

# --- UI 5: SaveProgrammeSelection ---------------------------------------------------------------
$learnerController.SaveProgrammeSelection($sessionId, $science, $bcomIS)
$row = (Invoke-TestQuery $ctx "SELECT PreferredFacultyID, SelectedProgrammeID FROM AdvisorySession WHERE SessionID = $sessionId").Rows[0]
Assert-True ($row.PreferredFacultyID -eq $science -and $row.SelectedProgrammeID -eq $bcomIS) 'The preferred faculty may differ from the programme faculty'
$learnerController.SaveProgrammeSelection($sessionId, $commerce, $bcomIS)
Assert-Equal $commerce (Get-Scalar "SELECT PreferredFacultyID FROM AdvisorySession WHERE SessionID = $sessionId") 'The selection is stored'
Assert-Throws { $learnerController.SaveProgrammeSelection($sessionId, $commerce, $oldProgramme) } 'ArgumentException' 'An inactive programme is refused'
Assert-Throws { $learnerController.SaveProgrammeSelection($sessionId, $commerce, $humanitiesProgramme) } 'ArgumentException' 'A programme under an inactive faculty is refused'
Assert-Throws { $learnerController.SaveProgrammeSelection($sessionId, $humanities, $bcomIS) } 'ArgumentException' 'An inactive preferred faculty is refused'
Assert-Throws { $learnerController.SaveProgrammeSelection($sessionId, $commerce, 99999) } 'ArgumentException' 'An unknown programme is refused'
Assert-Throws { $learnerController.SaveProgrammeSelection(99999, $commerce, $bcomIS) } 'ArgumentException' 'An unknown session is refused'

# --- Sequence 1: CheckEligibility ---------------------------------------------------------------
$outcome = $eligibility.CheckEligibility($learnerId, $bcomIS)
Assert-Equal 'STRONG' $outcome.Code ('Gadija -> BCom IS is a Strong Match: ' + $outcome.Explanation)
Assert-True ($outcome.Explanation.Length -gt 0) 'The outcome states why'
$outcome = $eligibility.CheckEligibility($learnerId, $bcomIS)
Assert-Equal 1 (Get-Scalar "SELECT COUNT(*) FROM RecommendationResult WHERE SessionID = $sessionId AND IsSelectedProgramme = 1") 'Checking twice keeps one selected row'
Assert-Equal 1 (Get-Scalar "SELECT COUNT(*) FROM RecommendationResult WHERE SessionID = $sessionId") 'Checking twice keeps one row in all'
$stored = (Invoke-TestQuery $ctx "SELECT OutcomeCode, OverallScore, FacultyBonus, InterestMatchScore FROM RecommendationResult WHERE SessionID = $sessionId").Rows[0]
Assert-Equal 'STRONG' $stored.OutcomeCode 'The outcome code is stored'
Assert-True ($stored.FacultyBonus -eq $true) 'The faculty bonus is stored'
Assert-True ($stored.InterestMatchScore -eq 88.89) ('The inactive Old Category is ignored in the interest score: (1x100 + 0.8x75) / 1.8 = 88.89 (got ' + $stored.InterestMatchScore + ')')

$outcome = $eligibility.CheckEligibility($learnerId, $bscCS)
Assert-Equal 'STRONG' $outcome.Code ('Inactive requirements are ignored (BSc CS Physical Sciences): ' + $outcome.Explanation)
Assert-Equal 1 (Get-Scalar "SELECT COUNT(*) FROM RecommendationResult WHERE SessionID = $sessionId AND IsSelectedProgramme = 1") 'Switching programme still keeps exactly one selected row'
Assert-Equal $bscCS (Get-Scalar "SELECT ProgrammeID FROM RecommendationResult WHERE SessionID = $sessionId AND IsSelectedProgramme = 1") 'The selected row is the new programme'
Assert-Equal $bscCS (Get-Scalar "SELECT SelectedProgrammeID FROM AdvisorySession WHERE SessionID = $sessionId") 'save(session) records the new selected programme'
Assert-Equal $commerce (Get-Scalar "SELECT PreferredFacultyID FROM AdvisorySession WHERE SessionID = $sessionId") 'The preferred faculty is kept'
$outcome = $eligibility.CheckEligibility($learnerId, $bcomIS)
Assert-Equal 'STRONG' $sessionRepository.FindById($sessionId).Outcome 'AdvisorySession.Outcome is the selected row''s OutcomeCode'

# --- UI 6: requirement grid and tiles -----------------------------------------------------------
$checks = $eligibility.GetCheckedRequirements($sessionId, $bcomIS)
Assert-Equal 3 $checks.Count 'Every active requirement is checked (two Required, one Recommended)'
Assert-Equal 'English Home Language' $checks[0].SubjectName 'Checks are in requirement order'
Assert-True ($checks[0].IsMet) 'English 72.5 meets 50'
Assert-Equal 2 $eligibility.GetCheckedRequirements($sessionId, $bscCS).Count 'Inactive requirements are not checked'
Assert-Equal 0 $eligibility.GetCheckedRequirements(99999, $bcomIS).Count 'An unknown session gives no checks'
$tiles = $eligibility.GetSelectedResult($sessionId)
Assert-Equal 1 $tiles.Rows.Count 'GetSelectedResult returns one row'
Assert-Equal 'SubjectMatchScore,MarkMatchScore,InterestMatchScore,OverallScore,OutcomeCode,OutcomeCategory,OutcomeReason,SubjectNote,MarkNote,StrongestCategory' (($tiles.Columns | ForEach-Object { $_.ColumnName }) -join ',') 'GetSelectedResult has the contract columns'
Assert-Equal 'Strong Match' $tiles.Rows[0].OutcomeCategory 'OutcomeCategory is the outcome in words'
Assert-Equal '2 of 2 required subjects captured' $tiles.Rows[0].SubjectNote 'SubjectNote'
Assert-Equal '2 of 2 minimum marks met' $tiles.Rows[0].MarkNote 'MarkNote'
Assert-Equal 'Business and finance' $tiles.Rows[0].StrongestCategory 'StrongestCategory is the best mapped category'
Assert-Equal 0 $eligibility.GetSelectedResult(99999).Rows.Count 'No stored result gives an empty table'

# --- Sequence 2: GenerateAlternatives -----------------------------------------------------------
$list = $recommendation.GenerateAlternatives($learnerId, $bcomIS)
Assert-True ($recommendation.MissingInformation -eq $null) 'No information is missing'
Assert-True ($list[0].IsSelectedProgramme -and $list[0].ProgrammeID -eq $bcomIS -and $list[0].AlternativeRank -eq $null) 'The stored selected row comes first'
$alternatives = @($list | Where-Object { -not $_.IsSelectedProgramme })
Assert-Equal 'BCom Accounting,BSc Computer Science,BSc Biological Sciences' (($alternatives | ForEach-Object { $_.ProgrammeName }) -join ',') 'Alternatives: active programmes of active faculties, Strong before NCE, then by score'
Assert-Equal '1,2,3' (($alternatives | ForEach-Object { $_.AlternativeRank }) -join ',') 'Ranks run 1..n'
Assert-True (@($alternatives | Where-Object { $_.ProgrammeID -eq $oldProgramme -or $_.ProgrammeID -eq $humanitiesProgramme }).Count -eq 0) 'The inactive programme and the programme under the inactive faculty are absent'
Assert-True (@($alternatives | Where-Object { $_.OutcomeReason.Length -eq 0 -or $_.FacultyName.Length -eq 0 }).Count -eq 0) 'Every alternative states its reason and faculty'
Assert-Equal 'NCE' $alternatives[2].OutcomeCode ('BSc Biological Sciences needs Physical Sciences: ' + $alternatives[2].OutcomeReason)
Assert-Equal 3 (Get-Scalar "SELECT COUNT(*) FROM RecommendationResult WHERE SessionID = $sessionId AND IsSelectedProgramme = 0 AND AlternativeRank IS NOT NULL") 'The alternatives are stored with their ranks'
$list = $recommendation.GenerateAlternatives($learnerId, $bcomIS)
Assert-Equal 4 $list.Count 'A second run is safe'
Assert-Equal 4 (Get-Scalar "SELECT COUNT(*) FROM RecommendationResult WHERE SessionID = $sessionId") 'A second run replaces the alternatives'
$results = $sessionRepository.FindResults($sessionId)
Assert-True ($results[0].IsSelectedProgramme -and $results[1].AlternativeRank -eq 1 -and $results[3].AlternativeRank -eq 3) 'FindResults: selected first, then by rank'
$list = $recommendation.GenerateAlternatives($learnerId, $bscCS)
Assert-Equal 3 $list.Count 'Excluding a different programme never repeats the selected programme as an alternative'
Assert-Equal 1 (Get-Scalar "SELECT COUNT(*) FROM RecommendationResult WHERE SessionID = $sessionId AND IsSelectedProgramme = 1") 'The selected row is untouched by Sequence 2'
[void]$recommendation.GenerateAlternatives($learnerId, $bcomIS)

Assert-True (-not $recommendation.GetAlternativesInSummary($sessionId)) 'Alternatives are not in the summary by default'
$recommendation.SetAlternativesInSummary($sessionId, $true)
Assert-True ($recommendation.GetAlternativesInSummary($sessionId)) 'The include-alternatives choice is stored'
$sessionRepository.SetFundingInfoRequested($sessionId, $true)
Assert-True ($sessionRepository.FindById($sessionId).FundingInfoRequested) 'The funding choice is stored'

# --- More Information Needed paths --------------------------------------------------------------
$before = Get-Scalar 'SELECT COUNT(*) FROM RecommendationResult'
Assert-Equal 'MOREINFO' $eligibility.CheckEligibility(99999, $bcomIS).Code 'An unknown learner is More Information Needed'
Assert-Equal 'MOREINFO' $eligibility.CheckEligibility($learnerId, 99999).Code 'An unknown programme is More Information Needed'
Assert-Equal 'MOREINFO' $eligibility.CheckEligibility($learnerId, $oldProgramme).Code 'An inactive programme is More Information Needed'
Assert-Equal $before (Get-Scalar 'SELECT COUNT(*) FROM RecommendationResult') 'Nothing is stored when the learner or programme is missing'

$second = $learnerController.SaveProfile($null, 'Labeeqah', 'Rademeyer', 1, 'Bishops Diocesan', 'Western Cape', $year, '', '')
$outcome = $eligibility.CheckEligibility($second.LearnerID, $bcomIS)
Assert-Equal 'MOREINFO' $outcome.Code 'No subjects is More Information Needed'
Assert-True ($outcome.Explanation -like '*subject*') ('It says what is missing: ' + $outcome.Explanation)
$stored = (Invoke-TestQuery $ctx "SELECT OutcomeCode, OverallScore FROM RecommendationResult WHERE SessionID = $($second.SessionID) AND IsSelectedProgramme = 1").Rows[0]
Assert-True ($stored.OutcomeCode -eq 'MOREINFO' -and $stored.OverallScore -is [DBNull]) 'The selected row is stored for More Information Needed, with no score'
Assert-Equal $commerce (Get-Scalar "SELECT PreferredFacultyID FROM AdvisorySession WHERE SessionID = $($second.SessionID)") 'With no preferred faculty, the programme faculty is stored'
$list = $recommendation.GenerateAlternatives($second.LearnerID, $bcomIS)
Assert-True ($list.Count -eq 0 -and $recommendation.MissingInformation.Code -eq 'MOREINFO') 'Sequence 2 with no subjects returns nothing and sets MissingInformation'

$learnerController.SaveSubjects($second.SessionID, (New-MarkList @(@($english, 80), @($maths, 58), @($physics, 62))))
Assert-Equal 0 (Get-Scalar "SELECT COUNT(*) FROM RecommendationResult WHERE SessionID = $($second.SessionID)") 'SaveSubjects clears the stale results'
$outcome = $eligibility.CheckEligibility($second.LearnerID, $bscBio)
Assert-Equal 'MOREINFO' $outcome.Code 'No interest answers is More Information Needed'
Assert-True ($outcome.Explanation -like '*interest questionnaire*') ('It names the questionnaire: ' + $outcome.Explanation)
$list = $recommendation.GenerateAlternatives($second.LearnerID, $bscBio)
Assert-True ($list.Count -eq 0 -and $recommendation.MissingInformation.Explanation -like '*interest questionnaire*') 'Sequence 2 with no answers sets MissingInformation'
$list = $recommendation.GenerateAlternatives(99999, $bcomIS)
Assert-True ($list.Count -eq 0 -and $recommendation.MissingInformation -ne $null) 'Sequence 2 for an unknown learner sets MissingInformation'

$learnerRepository.SaveResponses($second.SessionID, (New-Responses $second.SessionID @{ 1 = 2; 2 = 3; 3 = 5 }))
Assert-Equal 0 (Get-Scalar "SELECT COUNT(*) FROM RecommendationResult WHERE SessionID = $($second.SessionID)") 'SaveResponses clears the stale results'
$outcome = $eligibility.CheckEligibility($second.LearnerID, $bscBio)
Assert-Equal 'POSSIBLE' $outcome.Code ('Labeeqah -> BSc Biological Sciences is a Possible Match: ' + $outcome.Explanation)
[void]$recommendation.GenerateAlternatives($second.LearnerID, $bscBio)
Assert-True ($recommendation.MissingInformation -eq $null) 'MissingInformation is reset on the next call'

# FindById prefers the In-progress session over a later completed one
[void](Invoke-TestQuery $ctx "INSERT INTO AdvisorySession (LearnerID, GradeLevelID, SessionDate, AdvisingYear, SessionStatus, DateCompleted) VALUES ($learnerId, 2, GETDATE(), $year, 'Completed', GETDATE())")
Assert-Equal $sessionId $learnerRepository.FindById($learnerId).CurrentSessionID 'FindById picks the In-progress session first'

# --- UI 3 achievement level ---------------------------------------------------------------------
$dash = [string][char]0x2013
Assert-Equal ('6  70' + $dash + '79%') $learnerController.GetAchievementLevel(72) 'Mark 72 is level 6, 70-79%'
Assert-Equal ('7  80' + $dash + '100%') $learnerController.GetAchievementLevel(85) 'Mark 85 is level 7, 80-100%'
Assert-Equal ('1  0' + $dash + '29%') $learnerController.GetAchievementLevel(0) 'Mark 0 is level 1, 0-29%'
Assert-Equal '' $learnerController.GetAchievementLevel(101) 'An invalid mark has no level'

# --- UI 10.1a: FindLearners ---------------------------------------------------------------------
$found = $learnerController.FindLearners($advisorId, 'Jac', $null, $null, $null, $null)
Assert-Equal 1 $found.Rows.Count 'Surname prefix search finds Gadija'
Assert-Equal 'LearnerID,LearnerName,GradeDescription,SchoolName,Province,SessionCount,LastSessionDate' (($found.Columns | ForEach-Object { $_.ColumnName }) -join ',') 'Only the contract columns (no contact details)'
Assert-Equal 'Gadija Jacobs' $found.Rows[0].LearnerName 'LearnerName is first name and surname'
Assert-Equal 'Grade 12' $found.Rows[0].GradeDescription 'GradeDescription comes from the latest session'
Assert-Equal 2 $found.Rows[0].SessionCount 'SessionCount counts every session'
Assert-Equal 3 $learnerController.FindLearners($advisorId, '', '', 0, '', '').Rows.Count 'Blank filters and grade 0 return every learner'
Assert-Equal 0 $learnerController.FindLearners($advisorId, 'acobs', $null, $null, $null, $null).Rows.Count 'Names match from the start only'
Assert-Equal 1 $learnerController.FindLearners($advisorId, $null, $null, $null, 'irls', $null).Rows.Count 'The school matches anywhere in its name'
Assert-Equal 1 $learnerController.FindLearners($advisorId, $null, $null, 1, $null, 'Western Cape').Rows.Count 'Grade and province filters (Labeeqah, Grade 11)'
Assert-Equal 0 $learnerController.FindLearners($advisorId, '%', $null, $null, $null, $null).Rows.Count 'A typed % is not a wildcard'
Assert-Throws { $learnerController.FindLearners(99999, 'Jac', $null, $null, $null, $null) } 'UnauthorizedAccessException' 'An unknown user cannot search learners'

# --- Programme and requirement maintenance (used by ReferenceDataController) -----------------------
$newProgramme = New-Object FuturePath.ApplicationLayer.DegreeProgramme(0, $science, 'BSc Statistics', 60, $true)
$programmeRepository.DataSetChange($newProgramme, [FuturePath.DatabaseLayer.DB+DBOperation]::Add)
$newId = $programmeRepository.UpdateDataSource($newProgramme, [FuturePath.DatabaseLayer.DB+DBOperation]::Add)
Assert-True ($newId -gt 6 -and $newProgramme.ProgrammeID -eq $newId) 'Adding a programme returns its new key'
$newProgramme.ProgrammeName = 'BSc Mathematical Statistics'; $newProgramme.MinimumOverallAverage = $null
$programmeRepository.DataSetChange($newProgramme, [FuturePath.DatabaseLayer.DB+DBOperation]::Edit)
Assert-Equal $newId $programmeRepository.UpdateDataSource($newProgramme, [FuturePath.DatabaseLayer.DB+DBOperation]::Edit) 'Editing a programme returns its key'
$row = (Invoke-TestQuery $ctx "SELECT ProgrammeName, MinimumOverallAverage FROM DegreeProgramme WHERE ProgrammeID = $newId").Rows[0]
Assert-True ($row.ProgrammeName -eq 'BSc Mathematical Statistics' -and $row.MinimumOverallAverage -is [DBNull]) 'The programme edit is stored'
Assert-True ($programmeRepository.ProgrammeNameExists('bsc mathematical statistics', $null)) 'ProgrammeNameExists ignores case'
Assert-True (-not $programmeRepository.ProgrammeNameExists('BSc Mathematical Statistics', $newId)) 'ProgrammeNameExists excludes the programme itself'

$requirement = New-Object FuturePath.ApplicationLayer.ProgrammeRequirement(0, $newId, $maths, '', 'Required', 65, $mathsLit, '', 80, 'Statistics needs Mathematics.', $true)
$programmeRepository.DataSetChange($requirement, [FuturePath.DatabaseLayer.DB+DBOperation]::Add)
$requirementId = $programmeRepository.UpdateDataSource($requirement, [FuturePath.DatabaseLayer.DB+DBOperation]::Add)
Assert-True ($requirementId -gt 0) 'Adding a requirement returns its new key'
$loaded = $programmeRepository.FindRequirementById($requirementId)
Assert-True ($loaded.SubjectName -eq 'Mathematics' -and $loaded.AlternativeSubjectName -eq 'Mathematical Literacy' -and $loaded.AlternativeMinimumMark -eq 80) 'The requirement reads back with its subject names'
Assert-True ($programmeRepository.RequirementExists($newId, $maths, $null)) 'RequirementExists finds the subject'
$loaded.IsActive = $false; $loaded.AlternativeSubjectID = 0
$programmeRepository.DataSetChange($loaded, [FuturePath.DatabaseLayer.DB+DBOperation]::Edit)
[void]$programmeRepository.UpdateDataSource($loaded, [FuturePath.DatabaseLayer.DB+DBOperation]::Edit)
Assert-True ((Get-Scalar "SELECT AlternativeSubjectID FROM ProgrammeRequirement WHERE RequirementID = $requirementId") -is [DBNull]) 'An alternative of 0 is stored as no alternative'
Assert-Equal 0 $programmeRepository.FindByIdWithRequirements($newId).GetRequirements().Count 'A deactivated requirement is not loaded for the engine'
Assert-Equal 1 $programmeRepository.GetRequirements($newId, $true).Count 'GetRequirements includes inactive ones on request'
Assert-Throws { $programmeRepository.DataSetChange((New-Object FuturePath.ApplicationLayer.DegreeProgramme(99999, $science, 'Nope', $null, $true)), [FuturePath.DatabaseLayer.DB+DBOperation]::Edit) } 'ArgumentException' 'Editing an unknown programme is refused'

Assert-Equal 5 $programmeRepository.GetProgrammes($null, $false).Count 'GetProgrammes leaves out inactive programmes and inactive faculties'
Assert-Equal 7 $programmeRepository.GetProgrammes($null, $true).Count 'GetProgrammes includes them on request'
Assert-Equal 'BCom Accounting,BCom Information Systems' (($programmeRepository.SearchActiveProgrammes('bcom') | ForEach-Object { $_.ProgrammeName }) -join ',') 'Search finds active programmes only'
Assert-Equal 0 $programmeRepository.SearchActiveProgrammes('  ').Count 'Blank search text finds nothing'
$withMappings = $programmeRepository.FindByIdWithRequirements($bcomIS)
Assert-Equal 'Commerce' $withMappings.FacultyName 'FindByIdWithRequirements reads the faculty name'
Assert-Equal 2 $withMappings.InterestMappings.Count 'Mappings to inactive categories are left out'

Complete-FuturePathTest $ctx
