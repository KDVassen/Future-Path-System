<#
.SYNOPSIS
    Checks the shared contract: the harness loads FuturePath.exe, a database copy attaches, and the
    entity rule methods behave (requirement checks, match scores, interest score, password hash).
    Run: powershell -NoProfile -ExecutionPolicy Bypass -File tests\Contract-Smoke.ps1
#>
. "$PSScriptRoot\TestHelpers.ps1"

$ctx = Initialize-FuturePathTest -SchemaOnly   # empty schema: this script inserts its own fixture rows

# Database copy attaches and SystemParameter can be read (empty schema gives 0 rows).
$parameters = (New-Object FuturePath.DatabaseLayer.SystemParameterRepository).GetParameters()
Assert-True ($parameters -ne $null) 'SystemParameterRepository reads the database copy'

# --- Entity rule methods (no database) ---
function New-Subject($id, $name) { New-Object FuturePath.ApplicationLayer.Subject($id, $name, 'Core', $true) }
$maths = New-Subject 2 'Mathematics'; $mathsLit = New-Subject 3 'Mathematical Literacy'; $physics = New-Subject 4 'Physical Sciences'

$learner = New-Object FuturePath.ApplicationLayer.Learner
[void]$learner.AddSubject($maths, 58)
[void]$learner.AddSubject($physics, 62)
Assert-Throws { $learner.AddSubject($maths, 70) } 'ArgumentException' 'Duplicate subject in a session is refused'
Assert-Throws { $learner.AddSubject($mathsLit, 101) } 'ArgumentException' 'Mark 101 is refused'
Assert-True ([FuturePath.ApplicationLayer.LearnerSubject]::IsValidMark(0)) 'Mark 0 is valid'
Assert-True ([FuturePath.ApplicationLayer.LearnerSubject]::IsValidMark(100)) 'Mark 100 is valid'
Assert-True (-not [FuturePath.ApplicationLayer.LearnerSubject]::IsValidMark(-1)) 'Mark -1 is invalid'
Assert-True (-not [FuturePath.ApplicationLayer.LearnerSubject]::IsValidMark(72.55)) 'Mark with two decimals is invalid'
$parsed = [decimal]0
Assert-True (-not [FuturePath.ApplicationLayer.LearnerSubject]::TryParseMark('abc', [ref]$parsed)) 'Letters are not a mark'
Assert-True ([FuturePath.ApplicationLayer.LearnerSubject]::TryParseMark('72,5', [ref]$parsed)) 'Comma decimal is accepted'

$programme = New-Object FuturePath.ApplicationLayer.DegreeProgramme(1, 2, 'BSc Biological Sciences', 60, $true)
$programme.GetRequirements().Add((New-Object FuturePath.ApplicationLayer.ProgrammeRequirement(1, 1, 2, 'Mathematics', 'Required', 60, $null, '', $null, '', $true)))
$programme.GetRequirements().Add((New-Object FuturePath.ApplicationLayer.ProgrammeRequirement(2, 1, 4, 'Physical Sciences', 'Required', 50, $null, '', $null, '', $true)))
$match = $programme.EvaluateSubjectAndMarkMatch($learner.GetSubjects())
Assert-Equal 100 $match.SubjectScore 'Both required subjects captured: SubjectScore 100'
Assert-Equal 50 $match.MarkScore 'One of two minimums met: MarkScore 50'
Assert-Equal 2 $match.LargestShortfall 'Mathematics 58 vs 60: shortfall 2'
Assert-Equal 'Mathematics 58% is 2 points below the 60% minimum.' $match.GapList[0] 'Gap sentence names the subject and shortfall'

$alt = New-Object FuturePath.ApplicationLayer.ProgrammeRequirement(3, 1, 2, 'Mathematics', 'Required', 50, 3, 'Mathematical Literacy', 60, '', $true)
$litLearner = New-Object FuturePath.ApplicationLayer.Learner
[void]$litLearner.AddSubject($mathsLit, 78)
Assert-Equal 'ALT' $alt.CheckAgainst($litLearner.GetSubjects()).ResultCode 'Mathematical Literacy 78 accepted as the alternative'
$noAlt = New-Object FuturePath.ApplicationLayer.ProgrammeRequirement(4, 1, 2, 'Mathematics', 'Required', 70, $null, '', $null, '', $true)
Assert-Equal 'MISSING' $noAlt.CheckAgainst($litLearner.GetSubjects()).ResultCode 'No alternative: Mathematics is missing'

# Interest score: weighted mean of category scores.
$learner.SetResponse(1, 10, 5)   # category 10 -> 100
$learner.SetResponse(2, 20, 3)   # category 20 -> 50
$categories = New-TypedCollection 'FuturePath.ApplicationLayer.InterestCategory'
$c1 = New-Object FuturePath.ApplicationLayer.InterestCategory(10, 'Science', $true); $c1.RelevanceWeight = 1.0
$c2 = New-Object FuturePath.ApplicationLayer.InterestCategory(20, 'Business', $true); $c2.RelevanceWeight = 0.5
$categories.Add($c1); $categories.Add($c2)
Assert-Equal 83.33 $learner.CalculateInterestScore($categories) 'Weighted interest score (1x100 + 0.5x50) / 1.5'

# Password hashing.
$hash = [FuturePath.ApplicationLayer.UserAccount]::HashPassword('Correct-Horse-1')
Assert-True ([FuturePath.ApplicationLayer.UserAccount]::VerifyPassword('Correct-Horse-1', $hash)) 'Hash verifies the right password'
Assert-True (-not [FuturePath.ApplicationLayer.UserAccount]::VerifyPassword('wrong', $hash)) 'Hash rejects a wrong password'
Assert-True (-not ($hash -like '*Correct-Horse-1*')) 'Hash does not contain the password'

# --- Shared repositories and the role checks (database copy, own fixture rows) ---
$adminHash = [FuturePath.ApplicationLayer.UserAccount]::HashPassword('Admin-Test-1')
$advisorHash = [FuturePath.ApplicationLayer.UserAccount]::HashPassword('Advisor-Test-1')
[void](Invoke-TestQuery $ctx ("INSERT INTO UserAccount (Username, PasswordHash, FullName, Role, IsActive) VALUES " +
    "('smoke.admin', '$adminHash', 'Smoke Admin', 'Administrator', 1), " +
    "('smoke.advisor', '$advisorHash', 'Smoke Advisor', 'Career Advisor', 1), " +
    "('smoke.gone', '$advisorHash', 'Smoke Inactive', 'Career Advisor', 0); " +
    "INSERT INTO Subject (SubjectName, SubjectLevel, IsActive) VALUES ('Mathematics', 'Core', 1), ('Old Subject', 'Elective', 0)"))
$ids = Invoke-TestQuery $ctx "SELECT UserID, Username FROM UserAccount"
$adminId = ($ids.Rows | Where-Object { $_.Username -eq 'smoke.admin' }).UserID
$advisorId = ($ids.Rows | Where-Object { $_.Username -eq 'smoke.advisor' }).UserID
$inactiveId = ($ids.Rows | Where-Object { $_.Username -eq 'smoke.gone' }).UserID

$login = New-Object FuturePath.ApplicationLayer.LoginController
Assert-Equal 'Smoke Admin' $login.Authenticate('SMOKE.ADMIN', 'Admin-Test-1').FullName 'Login works and the username is not case-sensitive'
Assert-True ($login.Authenticate('smoke.admin', 'wrong') -eq $null) 'Wrong password returns null'
Assert-True ($login.Authenticate('nobody', 'Admin-Test-1') -eq $null) 'Unknown username returns null'
Assert-True ($login.Authenticate('smoke.gone', 'Advisor-Test-1') -eq $null) 'Inactive account cannot log in'
Assert-Equal 'Administrator' $login.CheckAdministrator($adminId).RoleName 'Administrator passes CheckAdministrator'
Assert-Throws { $login.CheckAdministrator($advisorId) } 'UnauthorizedAccessException' 'Career Advisor fails CheckAdministrator'
Assert-Equal 'Career Advisor' $login.CheckStaff($advisorId).RoleName 'Career Advisor passes CheckStaff'
Assert-Throws { $login.CheckStaff($inactiveId) } 'UnauthorizedAccessException' 'Inactive account fails CheckStaff'

$subjectRepository = New-Object FuturePath.DatabaseLayer.SubjectRepository
Assert-Equal 1 $subjectRepository.GetSubjects($false).Count 'Active subjects only'
Assert-Equal 2 $subjectRepository.GetSubjects($true).Count 'Inactive subjects included on request'

# Session numbers.
Assert-Equal '2026-0007' ([FuturePath.ApplicationLayer.AdvisorySession]::FormatSessionNumber([datetime]'2026-03-01', 7)) 'Session number format'
Assert-Equal 'Not Currently Eligible' ([FuturePath.ApplicationLayer.AdvisoryOutcome]::GetCategory('NCE')) 'NCE shows as Not Currently Eligible'

Complete-FuturePathTest $ctx
