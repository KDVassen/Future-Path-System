<#
.SYNOPSIS
    The demonstration test cases TC01-TC05 (spec Section 10, plan-and-tests.md), plus the
    security and inactive-data checks, run through the real controllers on a copy of the seeded
    database. Each block catches its own errors, so one failure never stops the rest.

    Seed IDs used (plan-and-tests.md, "Seed data as built"): learners / sessions 1-3, programmes
    1, 3, 4 and 11 (inactive), faculty 1 (Commerce), funding 9 (inactive) and 10 (closed),
    UserID 2 (Career Advisor) and the inactive account former.advisor.
    Run: powershell -NoProfile -ExecutionPolicy Bypass -File tests\TC01-TC05.ps1
#>
. "$PSScriptRoot\TestHelpers.ps1"

$ctx = Initialize-FuturePathTest
try {
    $seedPath = Join-Path $PSScriptRoot '..\FuturePath\Database\SeedData.sql'

    function Get-Count([string]$Sql) {
        return [int](Invoke-TestQuery $ctx $Sql).Rows[0][0]
    }

    function Get-SubjectId([string]$Name) {
        return [int](Invoke-TestQuery $ctx "SELECT SubjectID FROM Subject WHERE SubjectName = '$Name'").Rows[0].SubjectID
    }

    # Builds the IList<KeyValuePair<int, decimal>> that LearnerController.SaveSubjects takes, from (name, mark) pairs.
    function New-MarkList($Pairs) {
        $list = New-Object 'System.Collections.Generic.List[System.Collections.Generic.KeyValuePair[int,decimal]]'
        foreach ($pair in $Pairs) {
            $list.Add((New-Object 'System.Collections.Generic.KeyValuePair[int,decimal]' -ArgumentList (Get-SubjectId $pair[0]), ([decimal]$pair[1])))
        }
        return ,$list
    }

    function Get-SeedPassword([string]$Username) {
        # The test passwords live only in the SeedData.sql header comment.
        $text = [System.IO.File]::ReadAllText($seedPath)
        $match = [regex]::Match($text, '(?m)^\s+' + [regex]::Escape($Username) + '\s+(\S+)\s*$')
        if (-not $match.Success) { throw "No test password for $Username in the SeedData.sql header comment." }
        return $match.Groups[1].Value
    }

    # ---------- TC01 ----------
    Write-Host "`n--- TC01 Create a learner profile and capture subjects and marks (Gadija Jacobs, Grade 12) ---"
    try {
        $learnerController = New-Object FuturePath.ApplicationLayer.LearnerController
        $session = $learnerController.SaveProfile($null, 'Gadija', 'Jacobs', 2, 'Silvertree Secondary School', 'Western Cape', [int16]2026, '', '')
        Assert-True ($session -ne $null -and $session.SessionID -gt 0) 'TC01: SaveProfile returns a new session'
        $sessionID = $session.SessionID
        $stored = (Invoke-TestQuery $ctx "SELECT l.FirstName, l.LastName, l.SchoolName, l.Province, s.GradeLevelID, s.SessionStatus FROM AdvisorySession s JOIN Learner l ON l.LearnerID = s.LearnerID WHERE s.SessionID = $sessionID").Rows[0]
        Assert-Equal 'Gadija|Jacobs|Silvertree Secondary School|Western Cape|2|In progress' ("{0}|{1}|{2}|{3}|{4}|{5}" -f $stored.FirstName, $stored.LastName, $stored.SchoolName, $stored.Province, $stored.GradeLevelID, $stored.SessionStatus) 'TC01: the learner and an In-progress Grade 12 session are stored'

        $gadijaMarks = New-MarkList @(@('English Home Language', 72), @('Mathematics', 70), @('Accounting', 74),
                                      @('Business Studies', 79), @('Economics', 70), @('Life Orientation', 80))
        $learnerController.SaveSubjects($sessionID, $gadijaMarks)
        Assert-Equal 6 (Get-Count "SELECT COUNT(*) FROM LearnerSubject WHERE SessionID = $sessionID") 'TC01: six LearnerSubject rows are saved'
        Assert-Equal 0 (Get-Count "SELECT COUNT(*) FROM LearnerSubject WHERE SessionID = $sessionID AND (Mark < 0 OR Mark > 100)") 'TC01: every saved mark is within 0-100'
        $saved = Invoke-TestQuery $ctx "SELECT s.SubjectName, ls.Mark FROM LearnerSubject ls JOIN Subject s ON s.SubjectID = ls.SubjectID WHERE ls.SessionID = $sessionID ORDER BY s.SubjectName"
        Assert-Equal 'Accounting=74;Business Studies=79;Economics=70;English Home Language=72;Life Orientation=80;Mathematics=70' (($saved.Rows | ForEach-Object { '{0}={1}' -f $_.SubjectName, ([double]$_.Mark).ToString([cultureinfo]::InvariantCulture) }) -join ';') 'TC01: the saved marks are exactly hers'

        # Invalid and boundary input: the BLL validates again, whatever the form did.
        $captured = New-Object 'System.Collections.Generic.List[int]'
        $captured.Add((Get-SubjectId 'English Home Language'))
        $physics = Get-SubjectId 'Physical Sciences'
        foreach ($bad in @('-1', '101', 'abc')) {
            Assert-True ($learnerController.ValidateSubjectEntry($physics, $bad, $captured).Count -ge 1) "TC01: mark '$bad' is refused with a message"
        }
        foreach ($good in @('0', '100')) {
            Assert-Equal 0 ($learnerController.ValidateSubjectEntry($physics, $good, $captured).Count) "TC01: mark '$good' is accepted (boundary)"
        }
        Assert-True ($learnerController.ValidateSubjectEntry((Get-SubjectId 'English Home Language'), '70', $captured).Count -ge 1) 'TC01: a subject already captured in the session is refused'
        Assert-Throws { $learnerController.SaveSubjects($sessionID, (New-MarkList @(@('Mathematics', 70), @('Mathematics', 75)))) } 'ArgumentException' 'TC01: SaveSubjects refuses a duplicate subject'
        Assert-Throws { $learnerController.SaveSubjects($sessionID, (New-MarkList @(,@('Mathematics', 101)))) } 'ArgumentException' 'TC01: SaveSubjects refuses a mark of 101'
        Assert-Equal 6 (Get-Count "SELECT COUNT(*) FROM LearnerSubject WHERE SessionID = $sessionID") 'TC01: the refused saves left her six marks unchanged'
    } catch {
        Assert-Equal 'no exception' $_.Exception.Message 'TC01'
    }

    # ---------- TC02 ----------
    Write-Host "`n--- TC02 Check eligibility for a selected degree (Gadija -> BCom Information Systems) ---"
    try {
        $outcome = (New-Object FuturePath.ApplicationLayer.EligibilityController).CheckEligibility(1, 1)
        Assert-Equal 'STRONG' $outcome.Code 'TC02: Gadija Jacobs -> BCom Information Systems is a Strong Match'
        Assert-True (-not [string]::IsNullOrWhiteSpace($outcome.Explanation)) 'TC02: the Strong Match states its reason'
        Write-Host ("      {0} | {1}" -f $outcome.Category, $outcome.Explanation) -ForegroundColor Gray
    } catch {
        Assert-Equal 'no exception' $_.Exception.Message 'TC02'
    }

    # ---------- TC03 ----------
    Write-Host "`n--- TC03 Generate alternatives (Aadiel-Raboo Magwa -> BSc Computer Science) ---"
    try {
        $outcome = (New-Object FuturePath.ApplicationLayer.EligibilityController).CheckEligibility(2, 3)
        Assert-Equal 'NCE' $outcome.Code 'TC03: Aadiel-Raboo Magwa -> BSc Computer Science is Not Currently Eligible'
        Assert-True ($outcome.Explanation -like '*Mathematics*') 'TC03: the reason names Mathematics (Mathematical Literacy is not accepted)'
        Write-Host ("      {0} | {1}" -f $outcome.Category, $outcome.Explanation) -ForegroundColor Gray

        $rc = New-Object FuturePath.ApplicationLayer.RecommendationController
        $results = @($rc.GenerateAlternatives(2, 3))
        Assert-True ($rc.MissingInformation -eq $null) 'TC03: no information is missing'
        Assert-True ($results.Count -ge 3) "TC03: the selected result and at least two alternatives are returned ($($results.Count) rows)"
        if ($results.Count -ge 1) {
            $first = $results[0]
            Assert-Equal '3|True|' ("{0}|{1}|{2}" -f $first.ProgrammeID, $first.IsSelectedProgramme, $first.AlternativeRank) 'TC03: the selected programme comes first, without a rank'
            Assert-Equal 'NCE' $first.OutcomeCode 'TC03: the selected row keeps its Not Currently Eligible outcome'
        }
        $alternatives = @($results | Select-Object -Skip 1)
        $expectedAlternatives = Get-Count 'SELECT COUNT(*) FROM DegreeProgramme p JOIN Faculty f ON f.FacultyID = p.FacultyID WHERE p.IsActive = 1 AND f.IsActive = 1 AND p.ProgrammeID <> 3'
        Assert-Equal $expectedAlternatives $alternatives.Count 'TC03: every other active programme is scored as an alternative'
        foreach ($result in $results) {
            Assert-True (-not [string]::IsNullOrWhiteSpace($result.OutcomeReason)) "TC03: $($result.ProgrammeName) has an OutcomeReason"
        }
        Assert-True (-not ($alternatives | Where-Object { $_.ProgrammeID -eq 3 })) 'TC03: the selected programme is not repeated as an alternative'
        Assert-True (-not ($alternatives | Where-Object { $_.ProgrammeID -eq 11 })) 'TC03: the inactive programme (11) is never an alternative'
        Assert-Equal ((1..[Math]::Max(1, $alternatives.Count)) -join ',') (($alternatives | ForEach-Object { $_.AlternativeRank }) -join ',') 'TC03: AlternativeRank runs 1..n'
        Assert-True (@($alternatives | Where-Object { $_.OutcomeCode -eq 'STRONG' -or $_.OutcomeCode -eq 'POSSIBLE' }).Count -ge 1) 'TC03: at least one alternative is a Strong or Possible Match (BSocSc / BA accept Mathematical Literacy)'
        foreach ($result in $alternatives) {
            Write-Host ("      {0,2}. {1} ({2}): {3} | {4}" -f $result.AlternativeRank, $result.ProgrammeName, $result.FacultyName, $result.OutcomeCode, $result.OutcomeReason) -ForegroundColor Gray
        }
        Assert-Equal $results.Count (Get-Count 'SELECT COUNT(*) FROM RecommendationResult WHERE SessionID = 2') 'TC03: the selected result and the alternatives are stored'
    } catch {
        Assert-Equal 'no exception' $_.Exception.Message 'TC03'
    }

    # ---------- TC04 ----------
    Write-Host "`n--- TC04 View funding (Gadija -> Commerce / BCom Information Systems) ---"
    try {
        $fundingController = New-Object FuturePath.ApplicationLayer.FundingController
        $funding = @($fundingController.GetActiveFunding(1, 1, '', $false))
        Assert-True ($funding.Count -ge 1) "TC04: active funding linked to Commerce or BCom IS is shown ($($funding.Count) options)"
        Assert-Equal 0 (@($funding | Where-Object { -not $_.IsActive }).Count) 'TC04: no inactive option is shown'
        Assert-True (-not ($funding | Where-Object { $_.FundingID -eq 9 })) 'TC04: the inactive Commerce option (9) never appears'
        Assert-True (@($funding | Where-Object { $_.FundingID -eq 10 }).Count -eq 1) 'TC04: the closed but active option (10) is listed when openOnly is false'
        foreach ($option in $funding) {
            $links = Get-Count "SELECT (SELECT COUNT(*) FROM FundingFaculty WHERE FundingID = $($option.FundingID) AND FacultyID = 1) + (SELECT COUNT(*) FROM FundingProgramme WHERE FundingID = $($option.FundingID) AND ProgrammeID = 1)"
            Assert-True ($links -ge 1) "TC04: $($option.FundingName) is linked to Commerce or BCom IS"
            Write-Host ("      {0} ({1}), closes {2:yyyy-MM-dd}" -f $option.FundingName, $option.FundingType, $option.ClosingDate) -ForegroundColor Gray
        }
        $open = @($fundingController.GetActiveFunding(1, 1, '', $true))
        Assert-True (-not ($open | Where-Object { $_.FundingID -eq 10 })) 'TC04: with openOnly, the closed option (10) is excluded'
        Assert-True (-not ($open | Where-Object { $_.FundingID -eq 9 })) 'TC04: with openOnly, the inactive option (9) is still excluded'
        Assert-Equal 0 (@($open | Where-Object { -not $_.IsOpenOn((Get-Date)) }).Count) 'TC04: with openOnly, every option is still open today'
    } catch {
        Assert-Equal 'no exception' $_.Exception.Message 'TC04'
    }

    # ---------- TC05 ----------
    Write-Host "`n--- TC05 Export the learner advisory summary (Labeeqah -> BSc Biological Sciences) ---"
    try {
        $outcome = (New-Object FuturePath.ApplicationLayer.EligibilityController).CheckEligibility(3, 4)
        Assert-Equal 'POSSIBLE' $outcome.Code 'TC05: Labeeqah Rademeyer -> BSc Biological Sciences is a Possible Match'
        $rc = New-Object FuturePath.ApplicationLayer.RecommendationController
        [void]$rc.GenerateAlternatives(3, 4)              # UI 6.1a: she looks at the alternatives ...
        $rc.SetAlternativesInSummary(3, $true)            # ... and ticks "include these alternatives in my summary"
        (New-Object FuturePath.ApplicationLayer.FundingController).RequestFundingInfo(3)   # UI 6.2b Add to Summary
        Assert-Equal '1|1' ((Invoke-TestQuery $ctx "SELECT CAST(AlternativesInSummary AS VARCHAR) + '|' + CAST(FundingInfoRequested AS VARCHAR) AS V FROM AdvisorySession WHERE SessionID = 3").Rows[0].V) 'TC05: the alternatives and funding choices are stored on the session'

        $reportController = New-Object FuturePath.ApplicationLayer.ReportController
        $exportsBefore = Get-Count 'SELECT COUNT(*) FROM ExportReport'   # the seed already logs the completed sessions' exports
        $data = $reportController.GetLearnerSummary(3)
        $header = $data.Tables['ReportHeader']
        Assert-Equal 1 $header.Rows.Count 'TC05: FP-LSR has one header row'
        if ($header.Rows.Count -eq 1) {
            $h = $header.Rows[0]
            Assert-Equal 'POSSIBLE' $h['OutcomeCode'] 'TC05: FP-LSR outcome is POSSIBLE'
            Assert-Equal 'BSc Biological Sciences' $h['ProgrammeName'] 'TC05: FP-LSR programme is BSc Biological Sciences'
            Assert-True (-not ($h['DateGenerated'] -is [DBNull])) 'TC05: FP-LSR shows its generation date'
            Assert-Equal 'Labeeqah|Rademeyer' ("{0}|{1}" -f $h['FirstName'], $h['LastName']) 'TC05: FP-LSR names the learner'
            Assert-True (-not [string]::IsNullOrWhiteSpace([string]$h['OutcomeReason'])) 'TC05: FP-LSR states the reason for the outcome'
        }
        $subjects = $data.Tables['SubjectRequirements']
        Assert-Equal 6 $subjects.Rows.Count 'TC05: FP-LSR lists her six subjects'
        Assert-Equal 'English Home Language;Geography;Life Orientation;Life Sciences;Mathematics;Physical Sciences' (($subjects.Rows | ForEach-Object { [string]$_['SubjectName'] } | Sort-Object) -join ';') 'TC05: FP-LSR subjects are exactly hers'
        $mathsRow = $subjects.Rows | Where-Object { $_['SubjectName'] -eq 'Mathematics' }
        if ($mathsRow) { Assert-Equal 'GAP' $mathsRow['ResultCode'] 'TC05: FP-LSR shows Mathematics as the gap' }
        Assert-True ($data.Tables['Alternatives'].Rows.Count -ge 1) 'TC05: FP-LSR includes the chosen alternatives'
        Assert-True ($data.Tables['FundingOptions'].Rows.Count -ge 1) 'TC05: FP-LSR includes funding (requested)'
        Assert-True (-not ($data.Tables['FundingOptions'].Rows | Where-Object { $_['FundingName'] -eq 'Marula Leaf Commerce Bursary' })) 'TC05: FP-LSR never lists inactive funding'
        Assert-Equal $exportsBefore (Get-Count 'SELECT COUNT(*) FROM ExportReport') 'TC05: building the summary logs nothing'

        $reportController.CompleteLearnerSession(3, 'PDF', (Get-Date))
        $session = (Invoke-TestQuery $ctx 'SELECT SessionStatus, DateCompleted FROM AdvisorySession WHERE SessionID = 3').Rows[0]
        Assert-Equal 'Completed' $session.SessionStatus 'TC05: session 3 is Completed'
        Assert-True (-not ($session.DateCompleted -is [DBNull])) 'TC05: session 3 has a DateCompleted'
        Assert-Equal 1 (Get-Count "SELECT COUNT(*) FROM ExportReport WHERE ReportCode = 'FP-LSR' AND SessionID = 3 AND GeneratedByUserID IS NULL AND ExportFormat = 'PDF' AND ParametersUsed IS NULL") 'TC05: one FP-LSR export is logged for session 3 (learner export: GeneratedByUserID NULL)'
        Assert-Equal ($exportsBefore + 1) (Get-Count 'SELECT COUNT(*) FROM ExportReport') 'TC05: completing the session adds exactly one export row'
    } catch {
        Assert-Equal 'no exception' $_.Exception.Message 'TC05'
    }

    # ---------- Security ----------
    Write-Host "`n--- Security (role checks in the BLL) ---"
    try {
        $referenceController = New-Object FuturePath.ApplicationLayer.ReferenceDataController
        Assert-Throws { $referenceController.SaveFaculty(2, $null, 'Test Faculty', $true) } 'UnauthorizedAccessException' 'Security: a Career Advisor (UserID 2) cannot add a faculty'
        Assert-Equal 0 (Get-Count "SELECT COUNT(*) FROM Faculty WHERE FacultyName = 'Test Faculty'") 'Security: no faculty was added'
        $login = New-Object FuturePath.ApplicationLayer.LoginController
        Assert-True ($login.Authenticate('admin', 'wrong') -eq $null) 'Security: a wrong password signs nobody in'
        Assert-True ($login.Authenticate('former.advisor', (Get-SeedPassword 'former.advisor')) -eq $null) 'Security: the inactive account cannot sign in with its correct password'
    } catch {
        Assert-Equal 'no exception' $_.Exception.Message 'Security'
    }

    # ---------- Inactive data ----------
    Write-Host "`n--- Inactive data never reaches a learner ---"
    try {
        $facultyOf11 = Get-Count 'SELECT FacultyID FROM DegreeProgramme WHERE ProgrammeID = 11'
        $programmes = @((New-Object FuturePath.ApplicationLayer.ReferenceDataController).GetActiveProgrammes($facultyOf11))
        Assert-True ($programmes.Count -ge 1) "Inactive data: faculty $facultyOf11 still lists its active programmes"
        Assert-True (-not ($programmes | Where-Object { $_.ProgrammeID -eq 11 })) 'Inactive data: inactive programme 11 is not listed'
    } catch {
        Assert-Equal 'no exception' $_.Exception.Message 'Inactive data'
    }
} catch {
    # An unexpected error is a failure, never a silent stop; the database copy is always detached.
    Assert-Equal 'no exception' $_.Exception.Message 'TC01-TC05 ran to the end'
} finally {
    Complete-FuturePathTest $ctx
}
