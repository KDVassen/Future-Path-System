<#
.SYNOPSIS
    The Part B regression suite (CLAUDE.md "What done means", plan-and-tests.md "Part B").
    Runs the five case-study learners through the real EligibilityController on a copy of the
    seeded database, checks that the stored result is safe to re-run, and checks the Mathematics
    tolerance boundary for Labeeqah Rademeyer with the real RecommendationEngine.

    Must pass after ANY change to the rule engine, the requirement rows or the scoring
    parameters (decisions.md D14). The seed IDs it relies on are listed in plan-and-tests.md,
    "Seed data as built": LearnerID = SessionID 1-5, programmes 1, 3, 4, 5, 7.
    Run: powershell -NoProfile -ExecutionPolicy Bypass -File tests\PartB-Regression.ps1
#>
. "$PSScriptRoot\TestHelpers.ps1"

$ctx = Initialize-FuturePathTest
try {
    # LearnerID (= SessionID), the Part B programme, the expected outcome and the subjects its reason must name.
    $cases = @(
        @{ Learner = 1; Name = 'Gadija Jacobs';      Programme = 1; Code = 'STRONG';   Mentions = @() },
        @{ Learner = 2; Name = 'Aadiel-Raboo Magwa'; Programme = 3; Code = 'NCE';      Mentions = @('Mathematics') },
        @{ Learner = 3; Name = 'Labeeqah Rademeyer'; Programme = 4; Code = 'POSSIBLE'; Mentions = @('Mathematics') },
        @{ Learner = 4; Name = 'Siyabonga Dickson';  Programme = 5; Code = 'STRONG';   Mentions = @() },
        @{ Learner = 5; Name = 'Derek Vin Diesel';   Programme = 7; Code = 'NCE';      Mentions = @('Mathematics', 'Physical Sciences') }
    )

    function Get-Count([string]$Sql) {
        return [int](Invoke-TestQuery $ctx $Sql).Rows[0][0]
    }

    # Runs CheckEligibility for one learner and checks the outcome and its reason.
    function Test-PartBCase($Case, [string]$Run) {
        try {
            $controller = New-Object FuturePath.ApplicationLayer.EligibilityController
            $outcome = $controller.CheckEligibility($Case.Learner, $Case.Programme)
            Assert-Equal $Case.Code $outcome.Code "$Run $($Case.Name) -> $($Case.Code)"
            Assert-True (-not [string]::IsNullOrWhiteSpace($outcome.Explanation)) "$Run $($Case.Name): the outcome states its reason"
            foreach ($word in $Case.Mentions) {
                Assert-True ($outcome.Explanation -like "*$word*") "$Run $($Case.Name): the reason names $word"
            }
            Write-Host ("      {0} | {1}" -f $outcome.Category, $outcome.Explanation) -ForegroundColor Gray
        } catch {
            Assert-Equal 'no exception' $_.Exception.Message "$Run $($Case.Name): CheckEligibility runs"
        }
    }

    # Checks that the session holds exactly one selected result, for the right programme and outcome.
    function Assert-OneSelectedRow($Case, [string]$Run) {
        $session = $Case.Learner
        $rows = Invoke-TestQuery $ctx "SELECT ProgrammeID, OutcomeCode, OutcomeReason FROM RecommendationResult WHERE SessionID = $session AND IsSelectedProgramme = 1"
        Assert-Equal 1 $rows.Rows.Count "$Run session ${session}: exactly one RecommendationResult row with IsSelectedProgramme = 1"
        if ($rows.Rows.Count -eq 1) {
            $row = $rows.Rows[0]
            Assert-Equal "$($Case.Programme)|$($Case.Code)" ("{0}|{1}" -f $row.ProgrammeID, $row.OutcomeCode) "$Run session ${session}: the stored row is programme $($Case.Programme) with $($Case.Code)"
            Assert-True (([string]$row.OutcomeReason).Length -gt 0) "$Run session ${session}: the stored row keeps its reason"
        }
        Assert-Equal 1 (Get-Count "SELECT COUNT(*) FROM RecommendationResult WHERE SessionID = $session AND ProgrammeID = $($Case.Programme)") "$Run session ${session}: one row for programme $($Case.Programme) (UNIQUE(SessionID, ProgrammeID))"
    }

    Write-Host "`n--- Part B learners (first run) ---"
    foreach ($case in $cases) { Test-PartBCase $case 'Run 1:' }
    foreach ($case in $cases) { Assert-OneSelectedRow $case 'Run 1:' }

    Write-Host "`n--- Re-run safety (second run on the same sessions) ---"
    foreach ($case in $cases) { Test-PartBCase $case 'Run 2:' }
    foreach ($case in $cases) { Assert-OneSelectedRow $case 'Run 2:' }

    Write-Host "`n--- Requirements are loaded from the database (active rows only) ---"
    try {
        $repository = New-Object FuturePath.DatabaseLayer.DegreeProgrammeRepository
        $bcomIS = $repository.FindByIdWithRequirements(1)
        Assert-True ($bcomIS -ne $null) 'Programme 1 (BCom Information Systems) loads'
        Assert-True (-not ($bcomIS.GetRequirements() | Where-Object { $_.RequirementID -eq 4 })) 'Programme 1 loads without its inactive requirement 4 (Information Technology)'
    } catch {
        Assert-Equal 'no exception' $_.Exception.Message 'FindByIdWithRequirements(1) runs'
    }

    Write-Host "`n--- Mathematics tolerance boundary (Labeeqah, BSc Biological Sciences 60% minimum, MARK_TOLERANCE 2) ---"
    try {
        $resultsBefore = Get-Count 'SELECT COUNT(*) FROM RecommendationResult'
        $programme = (New-Object FuturePath.DatabaseLayer.DegreeProgrammeRepository).FindByIdWithRequirements(4)
        Assert-Equal 'BSc Biological Sciences' $programme.ProgrammeName 'Programme 4 loads with its requirements'
        $engine = New-Object FuturePath.ApplicationLayer.RecommendationEngine
        $labeeqah = Invoke-TestQuery $ctx 'SELECT ls.SubjectID, s.SubjectName, s.SubjectLevel, ls.Mark FROM LearnerSubject ls JOIN Subject s ON s.SubjectID = ls.SubjectID WHERE ls.SessionID = 3'
        $invariant = [System.Globalization.CultureInfo]::InvariantCulture

        foreach ($boundary in @(@('57.9', 'NCE'), @('58.0', 'POSSIBLE'), @('60', 'STRONG'))) {
            # An in-memory learner with Labeeqah's subjects, Mathematics replaced by the boundary mark.
            $learner = New-Object FuturePath.ApplicationLayer.Learner
            foreach ($row in $labeeqah.Rows) {
                $mark = if ($row.SubjectName -eq 'Mathematics') { [decimal]::Parse($boundary[0], $invariant) } else { [decimal]$row.Mark }
                $subject = New-Object FuturePath.ApplicationLayer.Subject([int]$row.SubjectID, [string]$row.SubjectName, [string]$row.SubjectLevel, $true)
                [void]$learner.AddSubject($subject, $mark)
            }
            $match = $programme.EvaluateSubjectAndMarkMatch($learner.GetSubjects())
            $outcome = $engine.ComputeOutcome($match, [decimal]80, $true)
            Assert-Equal $boundary[1] $outcome.Code "Mathematics $($boundary[0]) (interest 80, faculty bonus) -> $($boundary[1])"
            Write-Host ("      {0} | {1}" -f $outcome.Category, $outcome.Explanation) -ForegroundColor Gray
        }
        Assert-Equal $resultsBefore (Get-Count 'SELECT COUNT(*) FROM RecommendationResult') 'The boundary checks wrote nothing to the database'
    } catch {
        Assert-Equal 'no exception' $_.Exception.Message 'Boundary checks run'
    }
} catch {
    # An unexpected error is a failure, never a silent stop; the database copy is always detached.
    Assert-Equal 'no exception' $_.Exception.Message 'PartB-Regression ran to the end'
} finally {
    Complete-FuturePathTest $ctx
}
