<#
.SYNOPSIS
    Tests the recommendation engine with no database: the five Part B learners (plan-and-tests.md)
    against their programmes built in memory, the 2-point tolerance boundary, weak interest,
    missing information, the explanation wording, RankAlternatives and CalculateOverallScore.
    Run: powershell -NoProfile -ExecutionPolicy Bypass -File tests\Engine-PartB.ps1
#>
. "$PSScriptRoot\TestHelpers.ps1"

$ctx = Initialize-FuturePathTest -NoDatabase
Set-TestSettings (Get-ProvisionalSettings)
$engine = New-Object FuturePath.ApplicationLayer.RecommendationEngine

# --- In-memory reference data -------------------------------------------------------------
$subjectNames = @{
    1 = 'English Home Language'; 2 = 'Mathematics'; 3 = 'Mathematical Literacy'; 4 = 'Physical Sciences'
    5 = 'Life Sciences'; 6 = 'Accounting'; 7 = 'Business Studies'; 8 = 'Economics'; 9 = 'Life Orientation'
    10 = 'History'; 11 = 'Tourism'; 12 = 'Geography'; 13 = 'Engineering Graphics and Design'; 14 = 'Visual Arts'
}
$subjects = @{}
foreach ($id in $subjectNames.Keys) {
    $subjects[$subjectNames[$id]] = New-Object FuturePath.ApplicationLayer.Subject($id, $subjectNames[$id], 'Core', $true)
}

# Interest categories: one question per category (QuestionID = CategoryID + 100)
$categoryNames = @{ 10 = 'Business and finance'; 20 = 'Technology'; 30 = 'Science'; 40 = 'Helping people'; 50 = 'Designing and building'; 60 = 'Numbers and data' }

function New-Programme($id, $facultyId, $name, [object[]]$requirements, [hashtable]$mappings) {
    $programme = New-Object FuturePath.ApplicationLayer.DegreeProgramme($id, $facultyId, $name, $null, $true)
    $reqId = $id * 10
    foreach ($r in $requirements) {
        $reqId++
        $subject = $subjects[$r[0]]
        $programme.GetRequirements().Add((New-Object FuturePath.ApplicationLayer.ProgrammeRequirement($reqId, $id, $subject.SubjectID, $subject.SubjectName,
            'Required', [decimal]$r[1], $null, '', $null, '', $true)))
    }
    foreach ($categoryId in $mappings.Keys) {
        $programme.InterestMappings.Add((New-Object FuturePath.ApplicationLayer.DegreeInterestMapping($id, $categoryId, $categoryNames[$categoryId], [decimal]$mappings[$categoryId])))
    }
    return $programme
}

function New-Learner($first, $last, $gradeLevel, [hashtable]$marks, [hashtable]$answers, $preferredFacultyId) {
    $learner = New-Object FuturePath.ApplicationLayer.Learner
    $learner.FirstName = $first; $learner.LastName = $last
    $learner.Grade = [FuturePath.ApplicationLayer.GradeLevel]$gradeLevel
    $learner.PreferredFacultyID = $preferredFacultyId
    foreach ($name in $marks.Keys) { [void]$learner.AddSubject($subjects[$name], [decimal]$marks[$name]) }
    foreach ($categoryId in $answers.Keys) { $learner.SetResponse($categoryId + 100, $categoryId, $answers[$categoryId]) }
    return $learner
}

# Sequence 1 steps 5-10 for an in-memory learner and programme
function Invoke-Check($learner, $programme) {
    $match = $programme.EvaluateSubjectAndMarkMatch($learner.GetSubjects())
    $interest = $learner.CalculateInterestScore($programme.GetInterestCategories())
    $bonus = $programme.MatchesPreferredFaculty($learner.GetPreferredFacultyId())
    return $engine.ComputeOutcome($match, $interest, $bonus)
}

# Faculties: 1 Commerce, 2 Science, 3 Engineering, 4 Health Sciences
$bcomIS  = New-Programme 1 1 'BCom Information Systems' @(@('English Home Language', 50), @('Mathematics', 60)) @{ 10 = 1.0; 20 = 0.8; 60 = 0.6 }
$bscCS   = New-Programme 2 2 'BSc Computer Science' @(@('English Home Language', 50), @('Mathematics', 70)) @{ 20 = 1.0; 60 = 0.8 }
$bscBio  = New-Programme 3 2 'BSc Biological Sciences' @(@('English Home Language', 50), @('Mathematics', 60), @('Physical Sciences', 50)) @{ 30 = 1.0; 40 = 0.5 }
$engMech = New-Programme 4 3 'BSc Engineering (Mechanical Engineering)' @(@('English Home Language', 50), @('Mathematics', 70), @('Physical Sciences', 70)) @{ 50 = 1.0; 30 = 0.7; 60 = 0.6 }
$bhsc    = New-Programme 5 4 'Bachelor of Health Sciences' @(@('English Home Language', 50), @('Mathematics', 60), @('Physical Sciences', 60)) @{ 40 = 1.0; 30 = 0.8 }

# --- The five Part B learners ---------------------------------------------------------------
$gadija = New-Learner 'Gadija' 'Jacobs' 2 @{ 'English Home Language' = 72; 'Mathematics' = 70; 'Accounting' = 74; 'Business Studies' = 79; 'Economics' = 70; 'Life Orientation' = 80 } @{ 10 = 5; 20 = 4; 60 = 4; 30 = 2; 40 = 2; 50 = 2 } 1
$aadiel = New-Learner 'Aadiel-Raboo' 'Magwa' 2 @{ 'English Home Language' = 65; 'Mathematical Literacy' = 78; 'Business Studies' = 71; 'History' = 69; 'Tourism' = 75; 'Life Orientation' = 82 } @{ 20 = 4; 60 = 3; 10 = 4; 30 = 2; 40 = 3; 50 = 2 } 2
$labeeqahMarks = @{ 'English Home Language' = 80; 'Mathematics' = 58; 'Physical Sciences' = 62; 'Life Sciences' = 70; 'Geography' = 66; 'Life Orientation' = 85 }
$labeeqahAnswers = @{ 30 = 4; 40 = 4; 10 = 2; 20 = 3; 50 = 2; 60 = 3 }
$labeeqah = New-Learner 'Labeeqah' 'Rademeyer' 1 $labeeqahMarks $labeeqahAnswers 2
$siyabonga = New-Learner 'Siyabonga' 'Dickson' 2 @{ 'English Home Language' = 70; 'Mathematics' = 75; 'Physical Sciences' = 78; 'Engineering Graphics and Design' = 73; 'Life Orientation' = 80 } @{ 50 = 5; 30 = 4; 60 = 4; 10 = 2; 20 = 3; 40 = 2 } 3
$derek = New-Learner 'Derek' 'Vin Diesel' 2 @{ 'English Home Language' = 82; 'Mathematical Literacy' = 76; 'History' = 79; 'Life Sciences' = 68; 'Visual Arts' = 85; 'Life Orientation' = 90 } @{ 40 = 5; 30 = 3; 10 = 2; 20 = 2; 50 = 4; 60 = 2 } 4

Assert-True ($gadija.CalculateInterestScore($bcomIS.GetInterestCategories()) -ge 75) 'Gadija is strongly aligned with BCom IS (>= 75)'
Assert-True ($siyabonga.CalculateInterestScore($engMech.GetInterestCategories()) -ge 75) 'Siyabonga is strongly aligned with Mechanical Engineering (>= 75)'

$outcome = Invoke-Check $gadija $bcomIS
Assert-Equal 'STRONG' $outcome.Code 'Part B: Gadija Jacobs -> BCom IS is a Strong Match'
Assert-True ($outcome.Explanation -like 'You meet both required subjects and their minimum marks, and your interests align strongly (*%).') ('Strong explanation says why: ' + $outcome.Explanation)
Assert-True ($outcome.OverallScore -ge 75) 'Strong Match has an overall score of at least 75'

$outcome = Invoke-Check $aadiel $bscCS
Assert-Equal 'NCE' $outcome.Code 'Part B: Aadiel-Raboo Magwa -> BSc CS is Not Currently Eligible'
Assert-True ($outcome.Explanation -like '*Mathematics*') ('NCE explanation mentions Mathematics: ' + $outcome.Explanation)
Assert-Equal 'Mathematics is required but was not captured.' $outcome.Explanation 'Mathematical Literacy is not accepted for BSc CS'
Assert-True ($outcome.OverallScore -ne $null) 'NCE still carries an overall score'

$outcome = Invoke-Check $labeeqah $bscBio
Assert-Equal 'POSSIBLE' $outcome.Code 'Part B: Labeeqah Rademeyer -> BSc Biological Sciences is a Possible Match'
Assert-True ($outcome.Explanation -like '*Mathematics*') ('Possible explanation names Mathematics as the gap: ' + $outcome.Explanation)
Assert-Equal 'Possible match: Mathematics 58% is 2 points below the 60% minimum. This is within 2 points of the minimum.' $outcome.Explanation 'Possible explanation wording'

$outcome = Invoke-Check $siyabonga $engMech
Assert-Equal 'STRONG' $outcome.Code 'Part B: Siyabonga Dickson -> Mechanical Engineering is a Strong Match'
Assert-True ($outcome.Explanation -like 'You meet all 3 required subjects*') ('Strong explanation counts 3 required subjects: ' + $outcome.Explanation)

$outcome = Invoke-Check $derek $bhsc
Assert-Equal 'NCE' $outcome.Code 'Part B: Derek Vin Diesel -> Bachelor of Health Sciences is Not Currently Eligible'
Assert-Equal 'Mathematics and Physical Sciences are required but were not captured.' $outcome.Explanation 'NCE names both missing subjects'

# --- The tolerance boundary (MARK_TOLERANCE = 2) ---------------------------------------------
$labeeqahMarks['Mathematics'] = '57.9'
$outcome = Invoke-Check (New-Learner 'Labeeqah' 'Rademeyer' 1 $labeeqahMarks $labeeqahAnswers 2) $bscBio
Assert-Equal 'NCE' $outcome.Code 'Mathematics 57.9 (2.1 below 60) is Not Currently Eligible'
Assert-True ($outcome.Explanation -like '*Mathematics 57.9% is 2.1 points below the 60% minimum.*more than the 2 points*') ('Shortfall explanation: ' + $outcome.Explanation)
$labeeqahMarks['Mathematics'] = '58.0'
$outcome = Invoke-Check (New-Learner 'Labeeqah' 'Rademeyer' 1 $labeeqahMarks $labeeqahAnswers 2) $bscBio
Assert-Equal 'POSSIBLE' $outcome.Code 'Mathematics 58.0 (2 below 60) is a Possible Match'

# --- Requirements met but weak interest (every answer 2 = 25%) ------------------------------
$weak = New-Learner 'Weak' 'Interest' 2 @{ 'English Home Language' = 72; 'Mathematics' = 70; 'Accounting' = 74 } @{ 10 = 2; 20 = 2; 30 = 2; 40 = 2; 50 = 2; 60 = 2 } 1
$outcome = Invoke-Check $weak $bcomIS
Assert-Equal 'POSSIBLE' $outcome.Code 'Requirements met with weak interest is a Possible Match'
Assert-Equal 'You meet the minimum requirements, but your interest alignment (25%) is weaker than a strong match needs (60%).' $outcome.Explanation 'Weak-interest explanation wording'
Assert-True ($outcome.OverallScore -eq 77.5) ('Weak interest: overall 30 + 30 + 7.5 + 10 = 77.5, above 75, so the interest rule decides (got ' + $outcome.OverallScore + ')')

# --- Average below the programme minimum ----------------------------------------------------
$withAverage = New-Programme 6 1 'BCom Accounting' @(@('English Home Language', 50), @('Mathematics', 60)) @{ 10 = 1.0 }
$withAverage.MinimumOverallAverage = 80
$outcome = Invoke-Check $gadija $withAverage
Assert-Equal 'POSSIBLE' $outcome.Code 'Average below the programme minimum is a Possible Match'
Assert-True ($outcome.Explanation -like 'Possible match: Overall average 74.2% is below the 80% minimum average*') ('Average explanation: ' + $outcome.Explanation)

# --- Three missing subjects, and the 500-character limit -------------------------------------
$three = New-Programme 7 4 'BSc Pharmacy' @(@('Mathematics', 60), @('Physical Sciences', 60), @('Accounting', 50)) @{ 40 = 1.0 }
$outcome = Invoke-Check $derek $three
Assert-Equal 'Mathematics, Physical Sciences and Accounting are required but were not captured.' $outcome.Explanation 'Three missing subjects are joined as "A, B and C"'

$long = New-Object FuturePath.ApplicationLayer.DegreeProgramme(8, 1, 'Long Programme', $null, $true)
for ($i = 1; $i -le 30; $i++) {
    $long.GetRequirements().Add((New-Object FuturePath.ApplicationLayer.ProgrammeRequirement((800 + $i), 8, (900 + $i), ('A very long subject name number ' + $i),
        'Required', [decimal]50, $null, '', $null, '', $true)))
}
$outcome = Invoke-Check $gadija $long
Assert-Equal 500 $outcome.Explanation.Length 'A long explanation is cut to the 500 characters OutcomeReason holds'

# --- Missing information --------------------------------------------------------------------
$empty = New-Object FuturePath.ApplicationLayer.Learner
$outcome = $engine.ComputeOutcome($bcomIS.EvaluateSubjectAndMarkMatch($empty.GetSubjects()), 0, $false)
Assert-Equal 'MOREINFO' $outcome.Code 'No subjects captured is More Information Needed'
Assert-True ($outcome.OverallScore -eq $null) 'More Information Needed has no overall score'
Assert-True ($outcome.Explanation.Length -gt 0) ('More Information Needed says why: ' + $outcome.Explanation)
Assert-Equal 'MOREINFO' $engine.ComputeOutcome($null, 0, $false).Code 'A missing match result is More Information Needed'

# --- CalculateOverallScore arithmetic --------------------------------------------------------
$match = New-Object FuturePath.ApplicationLayer.MatchResult
$match.SubjectScore = 100; $match.MarkScore = 50
$score = $engine.CalculateOverallScore($match, [decimal]80, $true)
Assert-True ($score -eq 79) ('Overall = 0.3x100 + 0.3x50 + 0.3x80 + 0.1x100 = 79 (got ' + $score + ')')
$score = $engine.CalculateOverallScore($match, [decimal]80, $false)
Assert-True ($score -eq 69) ('No faculty bonus: 79 - 10 = 69 (got ' + $score + ')')
$score = $engine.CalculateOverallScore($match, [decimal]'83.33', $false)
Assert-True ($score -eq 70) ('Rounded to two decimals: 69.999 -> 70.00 (got ' + $score + ')')
$heavy = Get-ProvisionalSettings
$heavy['WEIGHT_SUBJECT'] = '1'; $heavy['WEIGHT_MARK'] = '1'; $heavy['WEIGHT_INTEREST'] = '1'; $heavy['WEIGHT_FACULTY'] = '1'
Set-TestSettings $heavy
$score = $engine.CalculateOverallScore($match, [decimal]80, $true)
Assert-True ($score -eq 100) ('Overall score is capped at 100 (got ' + $score + ')')
Set-TestSettings (Get-ProvisionalSettings)

# --- RankAlternatives -----------------------------------------------------------------------
function New-Result($name, $code, $score) {
    $result = New-Object FuturePath.ApplicationLayer.RecommendationResult
    $result.ProgrammeName = $name; $result.OutcomeCode = $code; $result.OverallScore = $score
    return $result
}
$candidates = New-TypedCollection 'FuturePath.ApplicationLayer.RecommendationResult'
$candidates.Add((New-Result 'Zoology' 'POSSIBLE' 70))
$candidates.Add((New-Result 'Accounting' 'STRONG' 80))
$candidates.Add((New-Result 'Drama' 'MOREINFO' $null))
$candidates.Add((New-Result 'Chemistry' 'NCE' 85))
$candidates.Add((New-Result 'Biology' 'STRONG' 90))
$candidates.Add((New-Result 'Finance' 'NCE' $null))
$candidates.Add((New-Result 'Economics' 'POSSIBLE' 70))
$ranked = $engine.RankAlternatives($candidates)
Assert-Equal 'Biology,Accounting,Economics,Zoology,Chemistry,Finance,Drama' (($ranked | ForEach-Object { $_.ProgrammeName }) -join ',') 'Ranked by outcome, then score (no score last), then name'
Assert-Equal '1,2,3,4,5,6,7' (($ranked | ForEach-Object { $_.AlternativeRank }) -join ',') 'AlternativeRank runs 1..n'
Assert-True (-not [object]::ReferenceEquals($ranked, $candidates)) 'RankAlternatives returns a new collection'
Assert-Equal 0 $engine.RankAlternatives($null).Count 'No candidates gives an empty ranking'

Complete-FuturePathTest $ctx
