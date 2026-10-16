<#
.SYNOPSIS
    Checks the seed data (FuturePath\Database\SeedData.sql) against the seed rules: row counts, the
    Part B rows, the requirement constraints, interest data, funding coverage, system parameters,
    sessions and staff accounts. It finishes by applying the outcome rules of
    application-layer.md section 4 to every seeded session (with the shared entity methods
    EvaluateSubjectAndMarkMatch and CalculateInterestScore), so the intended scenario of each
    session is proved by the data. The real engine is tested by PartB-Regression.ps1.
    Last, it rebuilds the database from Database\CreateSchema.sql and SeedData.sql and checks that
    every table matches the shipped FuturePath.mdf row for row.

    Uses only the shared contract classes, so it runs before the learner-journey code is merged.
    Run: powershell -NoProfile -ExecutionPolicy Bypass -File tests\Seed-Check.ps1
#>
. "$PSScriptRoot\TestHelpers.ps1"

$ctx = Initialize-FuturePathTest
try {
    $seedPath = Join-Path $PSScriptRoot '..\FuturePath\Database\SeedData.sql'
    $seedToday = '2026-10-14'   # "today" when the seed was written; fixed so the checks do not age

    # ---------- helpers ----------
    function Get-Scalar([string]$Sql) {
        $table = Invoke-TestQuery $ctx $Sql
        return $table.Rows[0][0]
    }

    function Assert-Number($Expected, $Actual, [string]$Label) {
        # Compares numbers by value, so 60 and 60.00 (DECIMAL(5,2)) are equal.
        if ($Actual -is [DBNull] -or $Actual -eq $null) { Assert-Equal $Expected 'NULL' $Label; return }
        Assert-Equal ([double]$Expected) ([double]$Actual) $Label
    }

    function Assert-AtLeast([double]$Minimum, $Actual, [string]$Label) {
        if ([double]$Actual -ge $Minimum) { Assert-True $true $Label }
        else { Assert-Equal ">= $Minimum" $Actual $Label }
    }

    function Get-SeedPassword([string]$Username) {
        # The test passwords live only in the SeedData.sql header comment.
        $text = [System.IO.File]::ReadAllText($seedPath)
        $match = [regex]::Match($text, '(?m)^\s+' + [regex]::Escape($Username) + '\s+(\S+)\s*$')
        if (-not $match.Success) { throw "No test password for $Username in the SeedData.sql header comment." }
        return $match.Groups[1].Value
    }

    function Get-Nullable($Value) { if ($Value -is [DBNull]) { return $null } return $Value }

    # ---------- 1. Row counts ----------
    Write-Host "`n--- Row counts ---"
    $expectedCounts = [ordered]@{
        GradeLevel = 2; SystemParameter = 28; Faculty = 5; Subject = 16; DegreeProgramme = 11
        ProgrammeRequirement = 39; InterestCategory = 9; InterestQuestion = 13; DegreeInterestMapping = 29
        FundingOption = 10; FundingFaculty = 15; FundingProgramme = 14; Learner = 13; AdvisorySession = 13
        LearnerSubject = 77; LearnerInterestResponse = 144; UserAccount = 5
    }
    # RecommendationResult and ExportReport rows (written by the real engine, stored in SeedData.sql) are checked in section 11.
    foreach ($tableName in $expectedCounts.Keys) {
        Assert-Equal $expectedCounts[$tableName] (Get-Scalar "SELECT COUNT(*) FROM $tableName") "$tableName has $($expectedCounts[$tableName]) rows"
    }
    # Case-study minimums (database.md "Seed data minimums" and the seed brief).
    Assert-AtLeast 10 (Get-Scalar 'SELECT COUNT(*) FROM Learner') 'At least 10 learners'
    Assert-AtLeast 16 (Get-Scalar 'SELECT COUNT(*) FROM Subject WHERE IsActive = 1') 'At least 16 active subjects'
    Assert-AtLeast 5  (Get-Scalar 'SELECT COUNT(*) FROM Faculty WHERE IsActive = 1') 'At least 5 active faculties'
    Assert-AtLeast 10 (Get-Scalar 'SELECT COUNT(*) FROM DegreeProgramme WHERE IsActive = 1') 'At least 10 active programmes'
    Assert-AtLeast 8  (Get-Scalar 'SELECT COUNT(*) FROM InterestCategory WHERE IsActive = 1') 'At least 8 active interest categories'
    Assert-AtLeast 10 (Get-Scalar 'SELECT COUNT(*) FROM InterestQuestion WHERE IsActive = 1') 'At least 10 active interest questions'
    Assert-AtLeast 8  (Get-Scalar 'SELECT COUNT(*) FROM FundingOption WHERE IsActive = 1') 'At least 8 active funding options'
    Assert-AtLeast 12 (Get-Scalar 'SELECT COUNT(*) FROM AdvisorySession') 'At least 12 advisory sessions'

    # ---------- 2. Grade levels and system parameters ----------
    Write-Host "`n--- Grade levels and system parameters ---"
    $grades = Invoke-TestQuery $ctx 'SELECT GradeLevelID, GradeDescription, MarkBasis FROM GradeLevel ORDER BY GradeLevelID'
    Assert-Equal '1|Grade 11|Projected' ("{0}|{1}|{2}" -f $grades.Rows[0].GradeLevelID, $grades.Rows[0].GradeDescription, $grades.Rows[0].MarkBasis) 'GradeLevel 1 = Grade 11, Projected'
    Assert-Equal '2|Grade 12|Final' ("{0}|{1}|{2}" -f $grades.Rows[1].GradeLevelID, $grades.Rows[1].GradeDescription, $grades.Rows[1].MarkBasis) 'GradeLevel 2 = Grade 12, Final'

    $expectedParameters = [ordered]@{
        MIN_SUBJECT_COUNT = '7'
        NSC_LEVEL_1 = '0'; NSC_LEVEL_2 = '30'; NSC_LEVEL_3 = '40'; NSC_LEVEL_4 = '50'; NSC_LEVEL_5 = '60'; NSC_LEVEL_6 = '70'; NSC_LEVEL_7 = '80'
        FUNDING_TYPE_1 = 'Scholarship'; FUNDING_TYPE_2 = 'Bursary'; FUNDING_TYPE_3 = 'Financial Aid'; FUNDING_TYPE_4 = 'External Funding'
        PROVINCE_1 = 'Eastern Cape'; PROVINCE_2 = 'Free State'; PROVINCE_3 = 'Gauteng'; PROVINCE_4 = 'KwaZulu-Natal'; PROVINCE_5 = 'Limpopo'
        PROVINCE_6 = 'Mpumalanga'; PROVINCE_7 = 'North West'; PROVINCE_8 = 'Northern Cape'; PROVINCE_9 = 'Western Cape'
        WEIGHT_SUBJECT = '0.30'; WEIGHT_MARK = '0.30'; WEIGHT_INTEREST = '0.30'; WEIGHT_FACULTY = '0.10'
        THRESHOLD_STRONG = '75'; INTEREST_STRONG_MIN = '60'; MARK_TOLERANCE = '2'
    }
    $parameterRows = Invoke-TestQuery $ctx 'SELECT ParameterCode, ParameterValue, ParameterDescription FROM SystemParameter'
    $parameters = @{}
    foreach ($row in $parameterRows.Rows) { $parameters[[string]$row.ParameterCode] = $row }
    foreach ($code in $expectedParameters.Keys) {
        if ($parameters.ContainsKey($code)) {
            Assert-Equal $expectedParameters[$code] $parameters[$code].ParameterValue "SystemParameter $code = $($expectedParameters[$code])"
            Assert-True (-not ($parameters[$code].ParameterDescription -is [DBNull]) -and $parameters[$code].ParameterDescription.Length -gt 0) "SystemParameter $code has a description"
        } else {
            Assert-Equal $code 'missing' "SystemParameter $code is present"
        }
    }
    foreach ($code in @('WEIGHT_SUBJECT', 'WEIGHT_MARK', 'WEIGHT_INTEREST', 'WEIGHT_FACULTY', 'THRESHOLD_STRONG', 'INTEREST_STRONG_MIN', 'MARK_TOLERANCE')) {
        Assert-True ([string]$parameters[$code].ParameterDescription -like '*Provisional (decisions.md D14)*') "$code is described as provisional (D14)"
    }
    # The contract's settings helper reads them (invariant culture) from the database copy.
    [FuturePath.ApplicationLayer.SystemSettings]::Reload()
    Assert-Number 0.30 ([FuturePath.ApplicationLayer.SystemSettings]::WeightSubject) 'SystemSettings reads WEIGHT_SUBJECT'
    Assert-Number 2 ([FuturePath.ApplicationLayer.SystemSettings]::MarkTolerance) 'SystemSettings reads MARK_TOLERANCE'
    Assert-Equal 7 ([FuturePath.ApplicationLayer.SystemSettings]::MinimumSubjectCount) 'SystemSettings reads MIN_SUBJECT_COUNT'
    Assert-Equal 9 ([FuturePath.ApplicationLayer.SystemSettings]::GetNumberedValues('PROVINCE_')).Count 'Nine provinces, numbered without gaps'
    Assert-Equal 4 ([FuturePath.ApplicationLayer.SystemSettings]::GetNumberedValues('FUNDING_TYPE_')).Count 'Four funding types, numbered without gaps'
    Assert-Equal 7 ([FuturePath.ApplicationLayer.SystemSettings]::GetNumberedValues('NSC_LEVEL_')).Count 'Seven NSC levels, numbered without gaps'

    # ---------- 3. Faculties, subjects, programmes ----------
    Write-Host "`n--- Faculties, subjects and programmes ---"
    $expectedFaculties = @{ 1 = 'Commerce'; 2 = 'Science'; 3 = 'Engineering and the Built Environment'; 4 = 'Health Sciences'; 5 = 'Humanities' }
    foreach ($id in ($expectedFaculties.Keys | Sort-Object)) {
        Assert-Equal $expectedFaculties[$id] (Get-Scalar "SELECT FacultyName FROM Faculty WHERE FacultyID = $id AND IsActive = 1") "Faculty $id is $($expectedFaculties[$id]) (active)"
    }

    $expectedSubjects = @{
        1 = 'English Home Language|Home Language'; 2 = 'Afrikaans First Additional Language|First Additional Language'
        3 = 'Mathematics|Core'; 4 = 'Mathematical Literacy|Core'; 5 = 'Life Orientation|Core'
        6 = 'Physical Sciences|Elective'; 7 = 'Life Sciences|Elective'; 8 = 'Accounting|Elective'; 9 = 'Business Studies|Elective'
        10 = 'Economics|Elective'; 11 = 'Geography|Elective'; 12 = 'History|Elective'; 13 = 'Information Technology|Elective'
        14 = 'Engineering Graphics and Design|Elective'; 15 = 'Tourism|Elective'; 16 = 'Visual Arts|Elective'
    }
    $subjectRows = Invoke-TestQuery $ctx 'SELECT SubjectID, SubjectName, SubjectLevel, IsActive FROM Subject'
    $subjectById = @{}
    foreach ($row in $subjectRows.Rows) { $subjectById[[int]$row.SubjectID] = $row }
    foreach ($id in ($expectedSubjects.Keys | Sort-Object)) {
        $row = $subjectById[$id]
        $actual = if ($row) { "{0}|{1}" -f $row.SubjectName, $row.SubjectLevel } else { 'missing' }
        Assert-Equal $expectedSubjects[$id] $actual "Subject $id is $($expectedSubjects[$id])"
    }

    $expectedProgrammes = @{
        1 = 'BCom Information Systems|1|60|True'; 2 = 'BCom Accounting|1|60|True'; 3 = 'BSc Computer Science|2|65|True'
        4 = 'BSc Biological Sciences|2|60|True'; 5 = 'BSc Engineering (Mechanical Engineering)|3|65|True'
        6 = 'BSc Engineering (Civil Engineering)|3|65|True'; 7 = 'Bachelor of Health Sciences|4|65|True'
        8 = 'BSc Physiotherapy|4|60|True'; 9 = 'Bachelor of Social Science|5|55|True'; 10 = 'Bachelor of Arts|5||True'
        11 = 'BCom Tourism Management|1|50|False'
    }
    $programmeRows = Invoke-TestQuery $ctx 'SELECT ProgrammeID, FacultyID, ProgrammeName, MinimumOverallAverage, IsActive FROM DegreeProgramme'
    $programmeById = @{}
    foreach ($row in $programmeRows.Rows) { $programmeById[[int]$row.ProgrammeID] = $row }
    foreach ($id in ($expectedProgrammes.Keys | Sort-Object)) {
        $row = $programmeById[$id]
        $minimum = if ($row.MinimumOverallAverage -is [DBNull]) { '' } else { ([double]$row.MinimumOverallAverage).ToString([System.Globalization.CultureInfo]::InvariantCulture) }
        Assert-Equal $expectedProgrammes[$id] ("{0}|{1}|{2}|{3}" -f $row.ProgrammeName, $row.FacultyID, $minimum, [bool]$row.IsActive) "Programme $id is $($expectedProgrammes[$id])"
    }

    # ---------- 4. Programme requirements ----------
    Write-Host "`n--- Programme requirements ---"
    $requirementRows = Invoke-TestQuery $ctx ('SELECT r.RequirementID, r.ProgrammeID, s.SubjectName, r.RequirementType, r.MinimumMark, ' +
        'a.SubjectName AS AlternativeName, r.AlternativeMinimumMark, r.AdvisoryNote, r.IsActive ' +
        'FROM ProgrammeRequirement r JOIN Subject s ON s.SubjectID = r.SubjectID LEFT JOIN Subject a ON a.SubjectID = r.AlternativeSubjectID')
    function Assert-Requirement([int]$ProgrammeID, [string]$Subject, [string]$Type, $Minimum, [string]$Alternative = '', $AlternativeMinimum = $null) {
        $row = $requirementRows.Rows | Where-Object { $_.ProgrammeID -eq $ProgrammeID -and $_.SubjectName -eq $Subject }
        $label = "Programme $ProgrammeID requires $Subject ($Type $Minimum" + $(if ($Alternative) { ", or $Alternative $AlternativeMinimum" } else { ', no alternative' }) + ')'
        if ($row -eq $null) { Assert-Equal 'a requirement row' 'missing' $label; return }
        $actualAlternative = if ($row.AlternativeName -is [DBNull]) { '' } else { $row.AlternativeName }
        $actualAlternativeMinimum = if ($row.AlternativeMinimumMark -is [DBNull]) { '' } else { [double]$row.AlternativeMinimumMark }
        $expected = "$Type|$([double]$Minimum)|$Alternative|$(if ($AlternativeMinimum -ne $null) { [double]$AlternativeMinimum } else { '' })|True"
        $actual = "$($row.RequirementType)|$([double]$row.MinimumMark)|$actualAlternative|$actualAlternativeMinimum|$([bool]$row.IsActive)"
        Assert-Equal $expected $actual $label
    }
    # Part B rows (English = English Home Language).
    Assert-Requirement 1 'English Home Language' 'Required' 50
    Assert-Requirement 1 'Mathematics' 'Required' 60
    Assert-Requirement 1 'Accounting' 'Recommended' 50
    Assert-Requirement 3 'English Home Language' 'Required' 50
    Assert-Requirement 3 'Mathematics' 'Required' 70
    Assert-Requirement 3 'Information Technology' 'Recommended' 60
    Assert-Requirement 4 'English Home Language' 'Required' 50
    Assert-Requirement 4 'Mathematics' 'Required' 60
    Assert-Requirement 4 'Physical Sciences' 'Required' 50
    Assert-Requirement 4 'Life Sciences' 'Recommended' 60
    Assert-Requirement 5 'English Home Language' 'Required' 50
    Assert-Requirement 5 'Mathematics' 'Required' 70
    Assert-Requirement 5 'Physical Sciences' 'Required' 70
    Assert-Requirement 5 'Engineering Graphics and Design' 'Recommended' 60
    Assert-Requirement 7 'English Home Language' 'Required' 50
    Assert-Requirement 7 'Mathematics' 'Required' 60
    Assert-Requirement 7 'Physical Sciences' 'Required' 60
    Assert-Requirement 7 'Life Sciences' 'Recommended' 60
    Assert-Requirement 9 'English Home Language' 'Required' 60
    Assert-Requirement 9 'Mathematics' 'Required' 50 'Mathematical Literacy' 60
    Assert-Requirement 10 'English Home Language' 'Required' 60
    Assert-Requirement 10 'Mathematics' 'Required' 50 'Mathematical Literacy' 60

    # Case-study constraints, counted over active requirements of active programmes.
    $activeRequirement = 'FROM ProgrammeRequirement r JOIN DegreeProgramme p ON p.ProgrammeID = r.ProgrammeID AND p.IsActive = 1 JOIN Subject s ON s.SubjectID = r.SubjectID WHERE r.IsActive = 1'
    Assert-AtLeast 3 (Get-Scalar "SELECT COUNT(DISTINCT r.ProgrammeID) $activeRequirement AND r.RequirementType = 'Required' AND s.SubjectName = 'Mathematics'") 'At least 3 programmes require Mathematics'
    Assert-AtLeast 2 (Get-Scalar "SELECT COUNT(DISTINCT r.ProgrammeID) $activeRequirement AND r.RequirementType = 'Required' AND s.SubjectName = 'Physical Sciences'") 'At least 2 programmes require Physical Sciences'
    Assert-AtLeast 2 (Get-Scalar "SELECT COUNT(DISTINCT r.ProgrammeID) $activeRequirement AND r.AlternativeSubjectID = (SELECT SubjectID FROM Subject WHERE SubjectName = 'Mathematical Literacy')") 'At least 2 programmes accept Mathematical Literacy'
    Assert-Equal 0 (Get-Scalar "SELECT COUNT(*) FROM ProgrammeRequirement r WHERE r.ProgrammeID = 3 AND r.IsActive = 1 AND (r.SubjectID = 4 OR r.AlternativeSubjectID = 4)") 'BSc Computer Science does not accept Mathematical Literacy'
    Assert-Equal 0 (Get-Scalar 'SELECT COUNT(*) FROM DegreeProgramme p WHERE NOT EXISTS (SELECT 1 FROM ProgrammeRequirement r WHERE r.ProgrammeID = p.ProgrammeID AND r.IsActive = 1)') 'Every programme has at least one active requirement'
    Assert-Equal 0 (Get-Scalar 'SELECT COUNT(*) FROM DegreeProgramme p WHERE NOT EXISTS (SELECT 1 FROM DegreeInterestMapping m WHERE m.ProgrammeID = p.ProgrammeID)') 'Every programme has at least one interest mapping'
    Assert-Equal 'Information Technology|1|Required|False' ((Invoke-TestQuery $ctx "SELECT s.SubjectName + '|' + CAST(r.ProgrammeID AS VARCHAR) + '|' + r.RequirementType + '|' + CASE r.IsActive WHEN 1 THEN 'True' ELSE 'False' END AS V FROM ProgrammeRequirement r JOIN Subject s ON s.SubjectID = r.SubjectID WHERE r.RequirementID = 4").Rows[0].V) 'Requirement 4 (BCom IS, Information Technology) is the inactive requirement'
    Assert-Equal 1 (Get-Scalar 'SELECT COUNT(*) FROM ProgrammeRequirement WHERE IsActive = 0') 'Exactly one inactive requirement'
    Assert-AtLeast 3 (Get-Scalar 'SELECT COUNT(*) FROM ProgrammeRequirement WHERE AdvisoryNote IS NOT NULL AND IsActive = 1') 'Some active requirements carry an advisory note'

    # ---------- 5. Interest categories, questions and mappings ----------
    Write-Host "`n--- Interest data ---"
    foreach ($name in @('Numbers and data', 'Helping people and communities', 'Technology', 'Science experiments and investigation',
                        'Reading, writing and analysing ideas', 'Business, entrepreneurship and finance', 'Designing and building', 'Society, law and politics')) {
        Assert-Equal 1 (Get-Scalar "SELECT COUNT(*) FROM InterestCategory WHERE CategoryName = '$name' AND IsActive = 1") "Active category: $name"
    }
    Assert-Equal 1 (Get-Scalar 'SELECT COUNT(*) FROM InterestCategory WHERE IsActive = 0') 'One inactive interest category'
    Assert-Equal 0 (Get-Scalar 'SELECT COUNT(*) FROM InterestCategory c WHERE c.IsActive = 1 AND NOT EXISTS (SELECT 1 FROM InterestQuestion q WHERE q.CategoryID = c.CategoryID AND q.IsActive = 1)') 'Every active category has an active question'
    Assert-Equal 1 (Get-Scalar 'SELECT COUNT(*) FROM InterestQuestion WHERE IsActive = 0') 'One inactive interest question'
    Assert-Equal 0 (Get-Scalar 'SELECT COUNT(*) FROM InterestQuestion q JOIN InterestCategory c ON c.CategoryID = q.CategoryID WHERE q.IsActive = 1 AND c.IsActive = 0') 'No active question sits under an inactive category'
    $activeQuestionCount = [int](Get-Scalar 'SELECT COUNT(*) FROM InterestQuestion WHERE IsActive = 1')
    Assert-Equal 1 (Get-Scalar 'SELECT CASE WHEN MIN(DisplayOrder) = 1 AND MAX(DisplayOrder) = COUNT(*) AND COUNT(DISTINCT DisplayOrder) = COUNT(*) THEN 1 ELSE 0 END FROM InterestQuestion WHERE IsActive = 1') 'Active questions are numbered 1..n without gaps (DisplayOrder)'
    $caseStudyQuestions = @('I enjoy solving problems using numbers or data.', 'I enjoy helping people and communities.', 'I enjoy working with technology.',
        'I enjoy science experiments and research.', 'I enjoy reading, writing and analysing ideas.', 'I enjoy business, entrepreneurship and finance.',
        'I enjoy designing or building practical solutions.', 'I enjoy understanding society, law or politics.')
    for ($i = 0; $i -lt $caseStudyQuestions.Count; $i++) {
        Assert-Equal $caseStudyQuestions[$i] (Get-Scalar "SELECT QuestionText FROM InterestQuestion WHERE QuestionID = $($i + 1) AND IsActive = 1") "Question $($i + 1) uses the case-study wording"
    }
    Assert-Equal 0 (Get-Scalar 'SELECT COUNT(*) FROM (SELECT ProgrammeID FROM DegreeInterestMapping GROUP BY ProgrammeID HAVING COUNT(*) NOT BETWEEN 2 AND 4) x') 'Every programme maps to 2-4 categories'
    Assert-Equal 0 (Get-Scalar 'SELECT COUNT(*) FROM DegreeInterestMapping WHERE RelevanceWeight NOT BETWEEN 0.40 AND 1.00') 'Every relevance weight is 0.40-1.00'
    Assert-Equal 0 (Get-Scalar 'SELECT COUNT(*) FROM DegreeInterestMapping m JOIN InterestCategory c ON c.CategoryID = m.CategoryID WHERE c.IsActive = 0') 'Mappings use active categories only'

    # ---------- 6. Funding ----------
    Write-Host "`n--- Funding ---"
    $activeFunding = 'FROM FundingOption f WHERE f.IsActive = 1'
    foreach ($facultyID in 1..4) {
        Assert-AtLeast 1 (Get-Scalar "SELECT COUNT(*) $activeFunding AND EXISTS (SELECT 1 FROM FundingFaculty ff WHERE ff.FundingID = f.FundingID AND ff.FacultyID = $facultyID)") "Active funding is linked to faculty $facultyID ($($expectedFaculties[$facultyID]))"
    }
    Assert-AtLeast 1 (Get-Scalar "SELECT COUNT(*) $activeFunding AND f.IsNeedBased = 1") 'At least one active need-based option'
    Assert-AtLeast 1 (Get-Scalar "SELECT COUNT(*) $activeFunding AND f.IsMeritBased = 1") 'At least one active merit-based option'
    Assert-AtLeast 1 (Get-Scalar 'SELECT COUNT(DISTINCT FundingID) FROM FundingProgramme') 'Some options are linked to programmes'
    Assert-Equal 0 (Get-Scalar "SELECT COUNT(*) FROM FundingOption f WHERE f.FundingType NOT IN (SELECT ParameterValue FROM SystemParameter WHERE ParameterCode LIKE 'FUNDING[_]TYPE[_]%')") 'Every FundingType is a FUNDING_TYPE_n value'
    Assert-Equal 4 (Get-Scalar 'SELECT COUNT(DISTINCT FundingType) FROM FundingOption WHERE IsActive = 1') 'Active options use all four funding types'
    Assert-Equal 'Marula Leaf Commerce Bursary' (Get-Scalar 'SELECT FundingName FROM FundingOption WHERE FundingID = 9 AND IsActive = 0') 'Funding 9 is the inactive option'
    Assert-Equal 1 (Get-Scalar 'SELECT COUNT(*) FROM FundingFaculty WHERE FundingID = 9 AND FacultyID = 1') 'The inactive option is linked to Commerce'
    Assert-Equal 1 (Get-Scalar 'SELECT COUNT(*) FROM FundingProgramme WHERE FundingID = 9 AND ProgrammeID = 1') 'The inactive option is linked to BCom IS'
    Assert-Equal 'Harbourlight Early Application Bursary' (Get-Scalar "SELECT FundingName FROM FundingOption WHERE FundingID = 10 AND IsActive = 1 AND ClosingDate < '$seedToday'") 'Funding 10 is active but its closing date has passed'
    Assert-Equal 1 (Get-Scalar "SELECT COUNT(*) $activeFunding AND f.ClosingDate < '$seedToday'") 'Exactly one active option is closed'
    Assert-Equal 1 (Get-Scalar 'SELECT COUNT(*) FROM FundingFaculty WHERE FundingID = 10 AND FacultyID = 1') 'The closed option is linked to Commerce (TC04 can see it disappear)'
    Assert-True ([int](Get-Scalar "SELECT COUNT(*) $activeFunding AND f.ClosingDate BETWEEN '2026-11-01' AND '2027-12-31'") * 2 -gt [int](Get-Scalar "SELECT COUNT(*) $activeFunding")) 'Most active closing dates fall between 2026-11 and 2027'
    Assert-Equal 0 (Get-Scalar "SELECT COUNT(*) FROM FundingOption WHERE EligibilityDescription = '' OR RequiredDocuments IS NULL OR ClosingDate IS NULL OR ContactInformation IS NULL") 'Every option has eligibility, documents, closing date and contact details'
    Assert-Equal 0 (Get-Scalar 'SELECT COUNT(*) FROM FundingOption f WHERE NOT EXISTS (SELECT 1 FROM FundingFaculty x WHERE x.FundingID = f.FundingID) AND NOT EXISTS (SELECT 1 FROM FundingProgramme y WHERE y.FundingID = f.FundingID)') 'Every option is linked to a faculty or programme'

    # ---------- 7. Learners ----------
    Write-Host "`n--- Learners ---"
    $expectedLearners = @{ 1 = 'Gadija|Jacobs'; 2 = 'Aadiel-Raboo|Magwa'; 3 = 'Labeeqah|Rademeyer'; 4 = 'Siyabonga|Dickson'; 5 = 'Derek|Vin Diesel' }
    foreach ($id in ($expectedLearners.Keys | Sort-Object)) {
        Assert-Equal $expectedLearners[$id] (Get-Scalar "SELECT FirstName + '|' + LastName FROM Learner WHERE LearnerID = $id") "Learner $id is $($expectedLearners[$id])"
    }
    Assert-Equal 0 (Get-Scalar "SELECT COUNT(*) FROM Learner WHERE Province NOT IN (SELECT ParameterValue FROM SystemParameter WHERE ParameterCode LIKE 'PROVINCE[_]%')") 'Every learner province is a PROVINCE_n value'
    Assert-Equal 0 (Get-Scalar "SELECT COUNT(*) FROM Learner WHERE LTRIM(SchoolName) = ''") 'Every learner has a school'
    Assert-AtLeast 1 (Get-Scalar 'SELECT COUNT(*) FROM Learner WHERE ContactEmail IS NOT NULL OR ContactNumber IS NOT NULL') 'Some learners give contact details'
    Assert-AtLeast 1 (Get-Scalar 'SELECT COUNT(*) FROM Learner WHERE ContactEmail IS NULL AND ContactNumber IS NULL') 'Some learners give no contact details'
    Assert-Equal 0 (Get-Scalar "SELECT COUNT(*) FROM Learner WHERE ContactNumber IS NOT NULL AND ContactNumber NOT LIKE '0[0-9][0-9] [0-9][0-9][0-9] [0-9][0-9][0-9][0-9]'") 'Contact numbers use the 0XX XXX XXXX format'

    # ---------- 8. Sessions, subjects and responses ----------
    Write-Host "`n--- Sessions ---"
    $expectedPartB = @{
        1 = @{ Grade = 2; Programme = 1; Subjects = 'Accounting=74;Business Studies=79;Economics=70;English Home Language=72;Life Orientation=80;Mathematics=70' }
        2 = @{ Grade = 2; Programme = 3; Subjects = 'Business Studies=71;English Home Language=65;History=69;Life Orientation=82;Mathematical Literacy=78;Tourism=75' }
        3 = @{ Grade = 1; Programme = 4; Subjects = 'English Home Language=80;Geography=66;Life Orientation=85;Life Sciences=70;Mathematics=58;Physical Sciences=62' }
        4 = @{ Grade = 2; Programme = 5; Subjects = 'Engineering Graphics and Design=73;English Home Language=70;Life Orientation=80;Mathematics=75;Physical Sciences=78' }
        5 = @{ Grade = 2; Programme = 7; Subjects = 'English Home Language=82;History=79;Life Orientation=90;Life Sciences=68;Mathematical Literacy=76;Visual Arts=85' }
    }
    foreach ($id in ($expectedPartB.Keys | Sort-Object)) {
        $e = $expectedPartB[$id]
        $session = (Invoke-TestQuery $ctx "SELECT s.LearnerID, s.GradeLevelID, s.PreferredFacultyID, s.SelectedProgrammeID, p.FacultyID AS ProgrammeFaculty FROM AdvisorySession s JOIN DegreeProgramme p ON p.ProgrammeID = s.SelectedProgrammeID WHERE s.SessionID = $id").Rows[0]
        Assert-Equal "$id|$($e.Grade)|$($e.Programme)" ("{0}|{1}|{2}" -f $session.LearnerID, $session.GradeLevelID, $session.SelectedProgrammeID) "Session $id belongs to learner $id, grade $($e.Grade), programme $($e.Programme)"
        Assert-Equal $session.ProgrammeFaculty $session.PreferredFacultyID "Session $id preferred faculty is the programme's faculty"
        $marks = Invoke-TestQuery $ctx "SELECT s.SubjectName, ls.Mark FROM LearnerSubject ls JOIN Subject s ON s.SubjectID = ls.SubjectID WHERE ls.SessionID = $id ORDER BY s.SubjectName"
        $actualMarks = ($marks.Rows | ForEach-Object { '{0}={1}' -f $_.SubjectName, ([double]$_.Mark).ToString([cultureinfo]::InvariantCulture) }) -join ';'
        Assert-Equal $e.Subjects $actualMarks "Session $id subjects and marks are exactly Part B"
        Assert-Equal $activeQuestionCount (Get-Scalar "SELECT COUNT(*) FROM LearnerInterestResponse r JOIN InterestQuestion q ON q.QuestionID = r.QuestionID WHERE r.SessionID = $id AND q.IsActive = 1") "Session $id answers every active question"
    }
    # Sessions 6-12 were completed by their learner's FP-LSR export; 1-5 (Part B) and 13 stay In progress.
    Assert-Equal '6,7,8,9,10,11,12' (((Invoke-TestQuery $ctx "SELECT SessionID FROM AdvisorySession WHERE SessionStatus = 'Completed' AND DateCompleted IS NOT NULL ORDER BY SessionID").Rows | ForEach-Object { $_.SessionID }) -join ',') 'Sessions 6-12 are Completed with a DateCompleted'
    Assert-Equal 0 (Get-Scalar "SELECT COUNT(*) FROM AdvisorySession WHERE SessionID IN (1, 2, 3, 4, 5, 13) AND (SessionStatus <> 'In progress' OR DateCompleted IS NOT NULL)") 'Part B sessions and session 13 are still In progress'
    Assert-Equal 0 (Get-Scalar 'SELECT COUNT(*) FROM AdvisorySession WHERE SessionID <> LearnerID') 'SessionID = LearnerID for every session'
    Assert-Equal 0 (Get-Scalar 'SELECT COUNT(*) FROM (SELECT LearnerID FROM AdvisorySession GROUP BY LearnerID HAVING COUNT(*) > 1) x') 'One session per learner (CheckEligibility reaches it by learner ID)'
    Assert-Equal 2 (Get-Scalar 'SELECT COUNT(DISTINCT GradeLevelID) FROM AdvisorySession') 'Sessions cover both grades'
    Assert-Equal 0 (Get-Scalar "SELECT COUNT(*) FROM AdvisorySession WHERE SessionDate < '2026-01-01' OR SessionDate >= '2026-10-11' OR AdvisingYear <> 2026") 'Session dates fall in 2026-01-01..2026-10-10, advising year 2026'
    Assert-AtLeast 6 (Get-Scalar 'SELECT COUNT(DISTINCT MONTH(SessionDate)) FROM AdvisorySession') 'Session dates are spread over the year'
    Assert-AtLeast 1 (Get-Scalar 'SELECT COUNT(*) FROM AdvisorySession WHERE FundingInfoRequested = 1') 'A session requests funding information'
    Assert-AtLeast 1 (Get-Scalar 'SELECT COUNT(*) FROM AdvisorySession WHERE AlternativesInSummary = 1') 'A session includes alternatives in its summary'
    Assert-Equal 0 (Get-Scalar 'SELECT COUNT(*) FROM AdvisorySession s WHERE s.PreferredFacultyID IS NULL OR s.SelectedProgrammeID IS NULL') 'Every session has a preferred faculty and a selected programme'
    Assert-Equal 0 (Get-Scalar 'SELECT COUNT(*) FROM AdvisorySession s JOIN DegreeProgramme p ON p.ProgrammeID = s.SelectedProgrammeID WHERE p.IsActive = 0') 'No session selects the inactive programme'
    Assert-Equal 1 (Get-Scalar 'SELECT COUNT(*) FROM AdvisorySession s WHERE s.SessionID = 12 AND NOT EXISTS (SELECT 1 FROM LearnerSubject x WHERE x.SessionID = s.SessionID) AND NOT EXISTS (SELECT 1 FROM LearnerInterestResponse y WHERE y.SessionID = s.SessionID)') 'Session 12 has a programme but no subjects and no answers (More Information Needed)'
    Assert-Equal 0 (Get-Scalar "SELECT COUNT(*) FROM AdvisorySession s WHERE s.SessionID <> 12 AND (SELECT COUNT(*) FROM LearnerInterestResponse r WHERE r.SessionID = s.SessionID) <> $activeQuestionCount") 'Every other session answers every active question'
    Assert-Equal 0 (Get-Scalar 'SELECT COUNT(*) FROM LearnerInterestResponse r JOIN InterestQuestion q ON q.QuestionID = r.QuestionID WHERE q.IsActive = 0') 'No answers to inactive questions'
    Assert-Equal 0 (Get-Scalar 'SELECT COUNT(*) FROM LearnerSubject WHERE Mark < 0 OR Mark > 100') 'Every mark is 0-100'

    # ---------- 9. Staff accounts ----------
    Write-Host "`n--- Staff accounts ---"
    $users = Invoke-TestQuery $ctx 'SELECT UserID, Username, PasswordHash, Role, IsActive FROM UserAccount ORDER BY UserID'
    Assert-Equal '1|admin|Administrator|True' ("{0}|{1}|{2}|{3}" -f $users.Rows[0].UserID, $users.Rows[0].Username, $users.Rows[0].Role, [bool]$users.Rows[0].IsActive) 'UserID 1 is the active Administrator admin'
    Assert-Equal '2|advisor|Career Advisor|True' ("{0}|{1}|{2}|{3}" -f $users.Rows[1].UserID, $users.Rows[1].Username, $users.Rows[1].Role, [bool]$users.Rows[1].IsActive) 'UserID 2 is the active Career Advisor advisor'
    Assert-Equal '3|Career Advisor|False' ("{0}|{1}|{2}" -f $users.Rows[2].UserID, $users.Rows[2].Role, [bool]$users.Rows[2].IsActive) 'UserID 3 is an inactive Career Advisor'
    Assert-Equal '4|senior.advisor|Career Advisor|True' ("{0}|{1}|{2}|{3}" -f $users.Rows[3].UserID, $users.Rows[3].Username, $users.Rows[3].Role, [bool]$users.Rows[3].IsActive) 'UserID 4 is the active Career Advisor senior.advisor'
    Assert-Equal '5|data.admin|Administrator|True' ("{0}|{1}|{2}|{3}" -f $users.Rows[4].UserID, $users.Rows[4].Username, $users.Rows[4].Role, [bool]$users.Rows[4].IsActive) 'UserID 5 is the active Administrator data.admin'
    $login = New-Object FuturePath.ApplicationLayer.LoginController
    foreach ($row in $users.Rows) {
        $password = Get-SeedPassword ([string]$row.Username)
        Assert-True (([string]$row.PasswordHash).Split(':').Count -eq 2 -and -not ([string]$row.PasswordHash).Contains($password)) "$($row.Username): password is stored as a salted hash (salt:hash), not plaintext"
        Assert-True ([FuturePath.ApplicationLayer.UserAccount]::VerifyPassword($password, [string]$row.PasswordHash)) "$($row.Username): the recorded test password matches the stored hash"
    }
    $admin = $login.Authenticate('admin', (Get-SeedPassword 'admin'))
    Assert-Equal 'Administrator' $(if ($admin) { $admin.RoleName } else { 'null' }) 'admin signs in as Administrator'
    $advisor = $login.Authenticate('advisor', (Get-SeedPassword 'advisor'))
    Assert-Equal 'Career Advisor' $(if ($advisor) { $advisor.RoleName } else { 'null' }) 'advisor signs in as Career Advisor'
    Assert-True ($login.Authenticate('former.advisor', (Get-SeedPassword 'former.advisor')) -eq $null) 'The inactive account cannot sign in with its correct password'
    Assert-Equal 'Administrator' $login.CheckAdministrator(1).RoleName 'UserID 1 passes CheckAdministrator'
    Assert-Throws { $login.CheckAdministrator(2) } 'UnauthorizedAccessException' 'UserID 2 fails CheckAdministrator'
    Assert-Throws { $login.CheckStaff(3) } 'UnauthorizedAccessException' 'UserID 3 (inactive) fails CheckStaff'

    # ---------- 10. Intended outcomes (application-layer.md section 4 applied to the seed) ----------
    Write-Host "`n--- Intended outcomes from the seed (shared entity methods + section 4 rules) ---"
    $setting = @{}
    foreach ($code in $expectedParameters.Keys) { $setting[$code] = [string]$parameters[$code].ParameterValue }
    $invariant = [System.Globalization.CultureInfo]::InvariantCulture
    function Get-Setting([string]$Code) { return [decimal]::Parse($setting[$Code], $invariant) }

    function New-SeedProgramme([int]$ProgrammeID) {
        # Mirrors DegreeProgrammeRepository.FindByIdWithRequirements: ACTIVE requirements, mappings to ACTIVE categories.
        $p = $programmeById[$ProgrammeID]
        $programme = New-Object FuturePath.ApplicationLayer.DegreeProgramme($ProgrammeID, [int]$p.FacultyID, [string]$p.ProgrammeName, (Get-Nullable $p.MinimumOverallAverage), [bool]$p.IsActive)
        $rows = Invoke-TestQuery $ctx "SELECT r.RequirementID, r.SubjectID, s.SubjectName, r.RequirementType, r.MinimumMark, r.AlternativeSubjectID, ISNULL(a.SubjectName, '') AS AlternativeName, r.AlternativeMinimumMark, ISNULL(r.AdvisoryNote, '') AS AdvisoryNote FROM ProgrammeRequirement r JOIN Subject s ON s.SubjectID = r.SubjectID LEFT JOIN Subject a ON a.SubjectID = r.AlternativeSubjectID WHERE r.ProgrammeID = $ProgrammeID AND r.IsActive = 1 ORDER BY r.RequirementID"
        foreach ($r in $rows.Rows) {
            $requirement = New-Object FuturePath.ApplicationLayer.ProgrammeRequirement([int]$r.RequirementID, $ProgrammeID, [int]$r.SubjectID, [string]$r.SubjectName, [string]$r.RequirementType,
                (Get-Nullable $r.MinimumMark), (Get-Nullable $r.AlternativeSubjectID), [string]$r.AlternativeName, (Get-Nullable $r.AlternativeMinimumMark), [string]$r.AdvisoryNote, $true)
            $programme.GetRequirements().Add($requirement)
        }
        $mappings = Invoke-TestQuery $ctx "SELECT m.CategoryID, c.CategoryName, m.RelevanceWeight FROM DegreeInterestMapping m JOIN InterestCategory c ON c.CategoryID = m.CategoryID WHERE m.ProgrammeID = $ProgrammeID AND c.IsActive = 1"
        foreach ($m in $mappings.Rows) {
            $programme.InterestMappings.Add((New-Object FuturePath.ApplicationLayer.DegreeInterestMapping($ProgrammeID, [int]$m.CategoryID, [string]$m.CategoryName, [decimal]$m.RelevanceWeight)))
        }
        return $programme
    }

    function New-SeedLearner([int]$SessionID) {
        # Mirrors LearnerRepository.FindBySessionId: the session's grade, preferred faculty, subjects and responses.
        $s = (Invoke-TestQuery $ctx "SELECT PreferredFacultyID FROM AdvisorySession WHERE SessionID = $SessionID").Rows[0]
        $learner = New-Object FuturePath.ApplicationLayer.Learner
        $learner.CurrentSessionID = $SessionID
        $learner.PreferredFacultyID = [int]$s.PreferredFacultyID
        foreach ($m in (Invoke-TestQuery $ctx "SELECT ls.SubjectID, x.SubjectName, x.SubjectLevel, ls.Mark FROM LearnerSubject ls JOIN Subject x ON x.SubjectID = ls.SubjectID WHERE ls.SessionID = $SessionID").Rows) {
            [void]$learner.AddSubject((New-Object FuturePath.ApplicationLayer.Subject([int]$m.SubjectID, [string]$m.SubjectName, [string]$m.SubjectLevel, $true)), [decimal]$m.Mark)
        }
        foreach ($r in (Invoke-TestQuery $ctx "SELECT r.QuestionID, q.CategoryID, r.ResponseValue FROM LearnerInterestResponse r JOIN InterestQuestion q ON q.QuestionID = r.QuestionID WHERE r.SessionID = $SessionID AND q.IsActive = 1").Rows) {
            $learner.SetResponse([int]$r.QuestionID, [int]$r.CategoryID, [int]$r.ResponseValue)
        }
        return $learner
    }

    function Get-IntendedOutcome($Learner, $Programme) {
        # The rules of application-layer.md section 4, in order, with the seeded SystemParameter values.
        if (-not $Learner.HasSubjects -or -not $Learner.HasInterestResponses) { return @{ Code = 'MOREINFO'; Interest = $null; Match = $null } }
        $match = $Programme.EvaluateSubjectAndMarkMatch($Learner.GetSubjects())
        $interest = $Learner.CalculateInterestScore($Programme.GetInterestCategories())
        $bonus = $Programme.MatchesPreferredFaculty($Learner.GetPreferredFacultyId())
        $result = @{ Interest = $interest; Match = $match }
        if ($match.HasMissingRequired) { $result.Code = 'NCE'; return $result }
        if ($match.LargestShortfall -gt (Get-Setting 'MARK_TOLERANCE')) { $result.Code = 'NCE'; return $result }
        if ($match.LargestShortfall -gt 0 -or -not $match.AverageMet) { $result.Code = 'POSSIBLE'; return $result }
        $overall = (Get-Setting 'WEIGHT_SUBJECT') * $match.SubjectScore + (Get-Setting 'WEIGHT_MARK') * $match.MarkScore +
                   (Get-Setting 'WEIGHT_INTEREST') * $interest + (Get-Setting 'WEIGHT_FACULTY') * $(if ($bonus) { 100 } else { 0 })
        $result.Overall = $overall
        if ($overall -ge (Get-Setting 'THRESHOLD_STRONG') -and $interest -ge (Get-Setting 'INTEREST_STRONG_MIN')) { $result.Code = 'STRONG' } else { $result.Code = 'POSSIBLE' }
        return $result
    }

    # SessionID -> intended outcome, and a word that the outcome's reason must be about.
    $scenarios = @{
        1 = @('STRONG', ''); 2 = @('NCE', 'Mathematics'); 3 = @('POSSIBLE', 'Mathematics'); 4 = @('STRONG', ''); 5 = @('NCE', 'Physical Sciences')
        6 = @('STRONG', ''); 7 = @('NCE', 'Physical Sciences'); 8 = @('NCE', 'Mathematics'); 9 = @('POSSIBLE', ''); 10 = @('STRONG', '')
        11 = @('POSSIBLE', 'average'); 12 = @('MOREINFO', ''); 13 = @('STRONG', '')
    }
    $sessionProgramme = @{}
    foreach ($row in (Invoke-TestQuery $ctx 'SELECT SessionID, SelectedProgrammeID FROM AdvisorySession').Rows) { $sessionProgramme[[int]$row.SessionID] = [int]$row.SelectedProgrammeID }
    $intended = @{}
    foreach ($sessionID in ($scenarios.Keys | Sort-Object)) {
        $learner = New-SeedLearner $sessionID
        $programme = New-SeedProgramme $sessionProgramme[$sessionID]
        $outcome = Get-IntendedOutcome $learner $programme
        $intended[$sessionID] = $outcome
        $gaps = if ($outcome.Match) { (@($outcome.Match.MissingRequired) + @($outcome.Match.GapList)) -join ' ' } else { '' }
        Assert-Equal $scenarios[$sessionID][0] $outcome.Code ("Session {0} ({1}) gives {2}" -f $sessionID, $programme.ProgrammeName, $scenarios[$sessionID][0])
        if ($scenarios[$sessionID][1]) {
            Assert-True ($gaps -like "*$($scenarios[$sessionID][1])*") "Session $sessionID gap names $($scenarios[$sessionID][1]): $gaps"
        }
    }
    # Specific facts the scenarios rely on.
    Assert-Equal 'Mathematics' ($intended[2].Match.MissingRequired -join ',') 'Aadiel (BSc CS): Mathematics is missing; Mathematical Literacy is not accepted'
    Assert-Equal 'Mathematics,Physical Sciences' ($intended[5].Match.MissingRequired -join ',') 'Derek (BHealthSc): Mathematics and Physical Sciences are missing'
    Assert-Number 2 $intended[3].Match.LargestShortfall 'Labeeqah: Mathematics shortfall is 2 (within MARK_TOLERANCE)'
    Assert-Number 4 $intended[8].Match.LargestShortfall 'Session 8: Mathematics shortfall is 4 (beyond MARK_TOLERANCE)'
    Assert-True (-not $intended[11].Match.AverageMet) 'Session 11: overall average is below the programme minimum'
    Assert-True ($intended[9].Interest -lt (Get-Setting 'INTEREST_STRONG_MIN')) "Session 9: interest in BCom IS is weak ($($intended[9].Interest))"
    $healthInterest = (New-SeedLearner 9).CalculateInterestScore((New-SeedProgramme 7).GetInterestCategories())
    Assert-True ($healthInterest -ge 75) "Session 9: interests align with Health Sciences instead (BHealthSc interest $healthInterest)"
    Assert-Equal 'ALT' (($intended[13].Match.Checks | Where-Object { $_.SubjectName -eq 'Mathematics' }).ResultCode) 'Session 13: Mathematical Literacy is accepted in place of Mathematics'

    # Gadija and Siyabonga need strong interest alignment with their programme (at least 75 in every mapped category).
    foreach ($pair in @(@(1, 1, 'Gadija'), @(4, 5, 'Siyabonga'))) {
        $learner = New-SeedLearner $pair[0]
        $programme = New-SeedProgramme $pair[1]
        Assert-True ($intended[$pair[0]].Interest -ge 75) "$($pair[2]): interest score $($intended[$pair[0]].Interest) is at least 75"
        foreach ($category in $programme.GetInterestCategories()) {
            $score = $learner.CalculateCategoryScore($category.CategoryID)
            Assert-True ($score -ne $null -and $score -ge 75) "$($pair[2]): '$($category.CategoryName)' scores $score (at least 75)"
        }
    }

    # Aadiel's alternatives (TC03): BSocSc and BA accept his Mathematical Literacy, and he meets them.
    foreach ($programmeID in @(9, 10)) {
        $outcome = Get-IntendedOutcome (New-SeedLearner 2) (New-SeedProgramme $programmeID)
        Assert-True ($outcome.Code -eq 'STRONG' -or $outcome.Code -eq 'POSSIBLE') "Aadiel would be $($outcome.Code) for programme $programmeID (a suitable alternative)"
    }

    # Labeeqah boundary rows used by PartB-Regression.ps1 (Mathematics 57.9 / 58.0 / 60, interest 80, bonus).
    foreach ($case in @(@(57.9, 'NCE'), @(58.0, 'POSSIBLE'), @(60, 'STRONG'))) {
        $learner = New-SeedLearner 3
        [void]$learner.RemoveSubject(3)
        [void]$learner.AddSubject((New-Object FuturePath.ApplicationLayer.Subject(3, 'Mathematics', 'Core', $true)), [decimal]$case[0])
        $match = (New-SeedProgramme 4).EvaluateSubjectAndMarkMatch($learner.GetSubjects())
        $code = if ($match.HasMissingRequired -or $match.LargestShortfall -gt 2) { 'NCE' } elseif ($match.LargestShortfall -gt 0 -or -not $match.AverageMet) { 'POSSIBLE' } elseif ((0.3 * $match.SubjectScore + 0.3 * $match.MarkScore + 0.3 * 80 + 10) -ge 75) { 'STRONG' } else { 'POSSIBLE' }
        Assert-Equal $case[1] $code "Labeeqah with Mathematics $($case[0]) is $($case[1]) (boundary)"
    }

    # ---------- 11. Stored results (written by the real engine, stored in SeedData.sql) ----------
    Write-Host "`n--- Stored results ---"
    foreach ($sessionID in ($scenarios.Keys | Sort-Object)) {
        $selected = Invoke-TestQuery $ctx "SELECT OutcomeCode, OutcomeReason FROM RecommendationResult WHERE SessionID = $sessionID AND IsSelectedProgramme = 1"
        Assert-Equal 1 $selected.Rows.Count "Session $sessionID has exactly one stored selected result"
        if ($selected.Rows.Count -eq 1) {
            Assert-Equal $scenarios[$sessionID][0] $selected.Rows[0].OutcomeCode "Session $sessionID stored outcome is $($scenarios[$sessionID][0])"
            Assert-True (([string]$selected.Rows[0].OutcomeReason).Length -gt 0) "Session $sessionID stored outcome has a reason"
        }
    }
    Assert-AtLeast 1 (Get-Scalar 'SELECT COUNT(*) FROM RecommendationResult WHERE IsSelectedProgramme = 0 AND AlternativeRank = 1') 'Ranked alternatives are stored'
    Assert-Equal 0 (Get-Scalar 'SELECT COUNT(*) FROM RecommendationResult r JOIN DegreeProgramme p ON p.ProgrammeID = r.ProgrammeID WHERE p.IsActive = 0') 'No stored result is for an inactive programme'
    Assert-Equal '6,7,8,9,10,11,12' (((Invoke-TestQuery $ctx "SELECT SessionID FROM ExportReport WHERE ReportCode = 'FP-LSR' AND GeneratedByUserID IS NULL AND ParametersUsed IS NULL ORDER BY SessionID").Rows | ForEach-Object { $_.SessionID }) -join ',') 'Each completed session has its learner FP-LSR export logged'
    Assert-Equal 7 (Get-Scalar 'SELECT COUNT(*) FROM ExportReport') 'Seven exports logged in total'

    # ---------- 12. The Database folder scripts rebuild exactly the shipped database ----------
    Write-Host "`n--- CreateSchema.sql + SeedData.sql match FuturePath.mdf ---"
    function Get-RowText($Context, [string]$TableName) {
        # Every row as one line of text (columns joined with |), sorted, so two copies of a table can be compared.
        $rows = Invoke-TestQuery $Context "SELECT * FROM [$TableName]"
        $lines = New-Object System.Collections.Generic.List[string]
        foreach ($row in $rows.Rows) {
            $values = foreach ($value in $row.ItemArray) {
                if ($value -is [DBNull]) { 'NULL' }
                elseif ($value -is [datetime]) { $value.ToString('o') }
                else { [System.Convert]::ToString($value, [cultureinfo]::InvariantCulture) }
            }
            $lines.Add(($values -join '|'))
        }
        $sorted = $lines.ToArray()
        [Array]::Sort($sorted, [StringComparer]::Ordinal)
        return ,$sorted
    }

    $scripted = $null
    try {
        $scripted = Initialize-FuturePathTest -FromScripts
        $tableNames = @((Invoke-TestQuery $ctx 'SELECT name FROM sys.tables ORDER BY name').Rows | ForEach-Object { $_.name })
        Assert-Equal 19 $tableNames.Count 'FuturePath.mdf has 19 tables'
        Assert-Equal 19 (Invoke-TestQuery $scripted 'SELECT COUNT(*) AS N FROM sys.tables').Rows[0].N 'The scripts create 19 tables'
        foreach ($tableName in $tableNames) {
            $shipped = Get-RowText $ctx $tableName
            $rebuilt = Get-RowText $scripted $tableName
            $firstDifference = 'none'
            if ($shipped.Count -ne $rebuilt.Count) {
                $firstDifference = '{0} rows in FuturePath.mdf, {1} from the scripts' -f $shipped.Count, $rebuilt.Count
            } else {
                for ($i = 0; $i -lt $shipped.Count; $i++) {
                    if ($shipped[$i] -cne $rebuilt[$i]) { $firstDifference = 'FuturePath.mdf has ' + $shipped[$i]; break }
                }
            }
            Assert-Equal 'none' $firstDifference ('{0}: the scripts rebuild its {1} rows exactly' -f $tableName, $shipped.Count)
        }
    } finally {
        Remove-TestDatabaseCopy $scripted
    }

} catch {
    # An unexpected error is a failure, never a silent stop; the database copy is always detached.
    Assert-Equal 'no exception' $_.Exception.Message 'Seed-Check ran to the end'
} finally {
    Complete-FuturePathTest $ctx
}
