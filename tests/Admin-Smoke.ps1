<#
.SYNOPSIS
    Smoke test for the administration and reference-data slice: lookups, faculty maintenance,
    interest categories / questions / weights, funding options and user accounts, plus the
    Administrator role check on every maintenance method.
    Run: powershell -NoProfile -ExecutionPolicy Bypass -File tests\Admin-Smoke.ps1

.NOTES
    Paths that need DegreeProgrammeRepository, LearnerRepository or AdvisorySessionRepository
    (owned by the learner-journey slice) are not exercised here, except SaveRelevanceWeight,
    which falls back to the repository while DegreeProgrammeRepository.FindById is a stub.
#>
. "$PSScriptRoot\TestHelpers.ps1"

$ctx = Initialize-FuturePathTest

function Get-Value([string]$Sql) {
    # First column of the first row of a query on the test copy.
    return (Invoke-TestQuery $ctx $Sql).Rows[0][0]
}

function Get-Names($Items, [string]$Property) {
    # Joins one property of every item, e.g. "Commerce, Science".
    $names = @()
    foreach ($item in $Items) { $names += [string]$item.$Property }
    return ($names -join ', ')
}

function Test-ThrowsType([scriptblock]$Block, [string]$ExceptionType) {
    # True if the block throws the exception type (inner exceptions included).
    try { & $Block | Out-Null; return $false }
    catch {
        $exception = $_.Exception
        while ($exception -ne $null) {
            if ($exception.GetType().Name -eq $ExceptionType) { return $true }
            $exception = $exception.InnerException
        }
        throw
    }
}

# ---------------------------------------------------------------------------------------------
# Fixture rows
# ---------------------------------------------------------------------------------------------
$parameters = Get-ProvisionalSettings
$parameters['FUNDING_TYPE_1'] = 'Scholarship'; $parameters['FUNDING_TYPE_2'] = 'Bursary'
$parameters['FUNDING_TYPE_3'] = 'Financial Aid'; $parameters['FUNDING_TYPE_4'] = 'External Funding'
$provinces = @('Eastern Cape', 'Free State', 'Gauteng', 'KwaZulu-Natal', 'Limpopo', 'Mpumalanga', 'North West', 'Northern Cape', 'Western Cape')
for ($i = 0; $i -lt $provinces.Count; $i++) { $parameters['PROVINCE_' + ($i + 1)] = $provinces[$i] }
$values = @()
foreach ($key in $parameters.Keys) { $values += "('$key', '$($parameters[$key])')" }
[void](Invoke-TestQuery $ctx ("INSERT INTO SystemParameter (ParameterCode, ParameterValue) VALUES " + ($values -join ', ')))
[FuturePath.ApplicationLayer.SystemSettings]::Reload()

$adminHash = [FuturePath.ApplicationLayer.UserAccount]::HashPassword('Admin-Test-1')
$advisorHash = [FuturePath.ApplicationLayer.UserAccount]::HashPassword('Advisor-Test-1')
[void](Invoke-TestQuery $ctx ("INSERT INTO GradeLevel (GradeLevelID, GradeDescription, MarkBasis) VALUES (1, 'Grade 11', 'Projected'), (2, 'Grade 12', 'Final'); " +
    "INSERT INTO UserAccount (Username, PasswordHash, FullName, Role, IsActive) VALUES " +
    "('smoke.admin', '$adminHash', 'Smoke Admin', 'Administrator', 1), " +
    "('smoke.advisor', '$advisorHash', 'Smoke Advisor', 'Career Advisor', 1); " +
    "INSERT INTO Faculty (FacultyName, IsActive) VALUES ('Commerce', 1), ('Science', 1), ('Humanities', 0); " +
    "INSERT INTO Subject (SubjectName, SubjectLevel, IsActive) VALUES ('Mathematics', 'Core', 1), ('Physical Sciences', 'Elective', 1), " +
    "('Mathematical Literacy', 'Core', 1), ('Old Subject', 'Elective', 0); " +
    "INSERT INTO InterestCategory (CategoryName, IsActive) VALUES ('Business and finance', 1), ('Science and research', 1), ('Old category', 0);"))

$adminId    = Get-Value "SELECT UserID FROM UserAccount WHERE Username = 'smoke.admin'"
$advisorId  = Get-Value "SELECT UserID FROM UserAccount WHERE Username = 'smoke.advisor'"
$commerceId = Get-Value "SELECT FacultyID FROM Faculty WHERE FacultyName = 'Commerce'"
$scienceId  = Get-Value "SELECT FacultyID FROM Faculty WHERE FacultyName = 'Science'"
$humanitiesId = Get-Value "SELECT FacultyID FROM Faculty WHERE FacultyName = 'Humanities'"
$businessId = Get-Value "SELECT CategoryID FROM InterestCategory WHERE CategoryName = 'Business and finance'"
$sciCatId   = Get-Value "SELECT CategoryID FROM InterestCategory WHERE CategoryName = 'Science and research'"
$oldCatId   = Get-Value "SELECT CategoryID FROM InterestCategory WHERE CategoryName = 'Old category'"

[void](Invoke-TestQuery $ctx ("INSERT INTO DegreeProgramme (FacultyID, ProgrammeName, MinimumOverallAverage, IsActive) VALUES " +
    "($commerceId, 'BCom Information Systems', 65, 1), ($scienceId, 'BSc Computer Science', NULL, 1), " +
    "($humanitiesId, 'BA Fine Art', NULL, 1), ($commerceId, 'BCom Old Programme', NULL, 0); " +
    "INSERT INTO InterestQuestion (CategoryID, QuestionText, DisplayOrder, IsActive) VALUES " +
    "($businessId, 'I enjoy working with numbers and data', 2, 1), " +
    "($businessId, 'I like planning how a business could grow', 1, 1), " +
    "($businessId, 'Retired business statement', 3, 0), " +
    "($sciCatId, 'I like finding out how things work', 4, 1), " +
    "($sciCatId, 'I enjoy doing experiments', 5, 1), " +
    "($oldCatId, 'Statement in an inactive category', 6, 1);"))

$progIS  = Get-Value "SELECT ProgrammeID FROM DegreeProgramme WHERE ProgrammeName = 'BCom Information Systems'"
$progCS  = Get-Value "SELECT ProgrammeID FROM DegreeProgramme WHERE ProgrammeName = 'BSc Computer Science'"
$progBA  = Get-Value "SELECT ProgrammeID FROM DegreeProgramme WHERE ProgrammeName = 'BA Fine Art'"
$progOld = Get-Value "SELECT ProgrammeID FROM DegreeProgramme WHERE ProgrammeName = 'BCom Old Programme'"
$qNumbers  = Get-Value "SELECT QuestionID FROM InterestQuestion WHERE QuestionText = 'I enjoy working with numbers and data'"
$qPlanning = Get-Value "SELECT QuestionID FROM InterestQuestion WHERE QuestionText = 'I like planning how a business could grow'"
$qRetired  = Get-Value "SELECT QuestionID FROM InterestQuestion WHERE QuestionText = 'Retired business statement'"
$qHow      = Get-Value "SELECT QuestionID FROM InterestQuestion WHERE QuestionText = 'I like finding out how things work'"
$qLab      = Get-Value "SELECT QuestionID FROM InterestQuestion WHERE QuestionText = 'I enjoy doing experiments'"
$qOldCat   = Get-Value "SELECT QuestionID FROM InterestQuestion WHERE QuestionText = 'Statement in an inactive category'"

[void](Invoke-TestQuery $ctx ("INSERT INTO DegreeInterestMapping (ProgrammeID, CategoryID, RelevanceWeight) VALUES ($progIS, $businessId, 1.00), ($progCS, $businessId, 0.40); " +
    "INSERT INTO FundingOption (FundingName, FundingType, EligibilityDescription, ClosingDate, IsMeritBased, IsNeedBased, IsActive) VALUES " +
    "('Science Scholarship', 'Scholarship', 'Science applicants', DATEADD(day, 30, CAST(GETDATE() AS DATE)), 1, 0, 1), " +
    "('Open Bursary', 'Bursary', 'Any Commerce applicant', NULL, 0, 1, 1), " +
    "('Closed Aid', 'Financial Aid', 'Commerce applicants in need', DATEADD(day, -10, CAST(GETDATE() AS DATE)), 0, 1, 1), " +
    "('Humanities Award', 'External Funding', 'Humanities applicants', NULL, 1, 0, 1), " +
    "('Programme Bursary', 'Bursary', 'BCom IS applicants', DATEADD(day, 60, CAST(GETDATE() AS DATE)), 0, 1, 1), " +
    "('Inactive Grant', 'Scholarship', 'Retired', NULL, 1, 0, 0), " +
    "('Old Programme Grant', 'Scholarship', 'Old programme applicants', NULL, 1, 0, 1), " +
    "('Fine Art Grant', 'Bursary', 'BA Fine Art applicants', NULL, 0, 1, 1);"))
$fScience = Get-Value "SELECT FundingID FROM FundingOption WHERE FundingName = 'Science Scholarship'"
$fOpen    = Get-Value "SELECT FundingID FROM FundingOption WHERE FundingName = 'Open Bursary'"
$fClosed  = Get-Value "SELECT FundingID FROM FundingOption WHERE FundingName = 'Closed Aid'"
$fHum     = Get-Value "SELECT FundingID FROM FundingOption WHERE FundingName = 'Humanities Award'"
$fProg    = Get-Value "SELECT FundingID FROM FundingOption WHERE FundingName = 'Programme Bursary'"
$fInact   = Get-Value "SELECT FundingID FROM FundingOption WHERE FundingName = 'Inactive Grant'"
$fOldProg = Get-Value "SELECT FundingID FROM FundingOption WHERE FundingName = 'Old Programme Grant'"
$fArt     = Get-Value "SELECT FundingID FROM FundingOption WHERE FundingName = 'Fine Art Grant'"
[void](Invoke-TestQuery $ctx ("INSERT INTO FundingFaculty (FundingID, FacultyID) VALUES ($fScience, $scienceId), ($fOpen, $commerceId), ($fOpen, $humanitiesId), " +
    "($fClosed, $commerceId), ($fHum, $humanitiesId), ($fInact, $commerceId); " +
    "INSERT INTO FundingProgramme (FundingID, ProgrammeID) VALUES ($fProg, $progIS), ($fOldProg, $progOld), ($fArt, $progBA);"))

$reference = New-Object FuturePath.ApplicationLayer.ReferenceDataController
$interests = New-Object FuturePath.ApplicationLayer.InterestController
$funding   = New-Object FuturePath.ApplicationLayer.FundingController
$users     = New-Object FuturePath.ApplicationLayer.UserAccountController
$login     = New-Object FuturePath.ApplicationLayer.LoginController

# ---------------------------------------------------------------------------------------------
# Lookups
# ---------------------------------------------------------------------------------------------
$provinceTable = $reference.GetProvinces()
Assert-Equal 9 $provinceTable.Rows.Count 'GetProvinces: nine PROVINCE_n rows'
Assert-Equal 'PROVINCE_1|Eastern Cape' ($provinceTable.Rows[0].ParameterCode + '|' + $provinceTable.Rows[0].ParameterValue) 'GetProvinces: PROVINCE_1 first'
Assert-Equal 'Western Cape' $provinceTable.Rows[8].ParameterValue 'GetProvinces: PROVINCE_9 last'

$outcomes = $reference.GetOutcomes($false)
Assert-Equal 4 $outcomes.Rows.Count 'GetOutcomes: four outcomes'
Assert-Equal 'STRONG|Strong Match|1' ($outcomes.Rows[0].OutcomeCode + '|' + $outcomes.Rows[0].OutcomeName + '|' + $outcomes.Rows[0].OutcomeOrder) 'GetOutcomes: Strong Match first'
Assert-Equal 'MOREINFO|More Information Needed|4' ($outcomes.Rows[3].OutcomeCode + '|' + $outcomes.Rows[3].OutcomeName + '|' + $outcomes.Rows[3].OutcomeOrder) 'GetOutcomes: More Information Needed last'
$outcomesAll = $reference.GetOutcomes($true)
Assert-Equal 5 $outcomesAll.Rows.Count 'GetOutcomes(all): five rows'
Assert-Equal '|All outcomes' ($outcomesAll.Rows[0].OutcomeCode + '|' + $outcomesAll.Rows[0].OutcomeName) 'GetOutcomes(all): "All outcomes" row first with code ""'

$grades = $reference.GetGradeLevels($false)
Assert-Equal 2 $grades.Rows.Count 'GetGradeLevels: two grades'
Assert-Equal '1|Grade 11|Projected' ("{0}|{1}|{2}" -f $grades.Rows[0].GradeLevelID, $grades.Rows[0].GradeDescription, $grades.Rows[0].MarkBasis) 'GetGradeLevels: Grade 11 first'
$gradesAll = $reference.GetGradeLevels($true)
Assert-Equal 3 $gradesAll.Rows.Count 'GetGradeLevels(all): three rows'
Assert-Equal '0|All grades' ("{0}|{1}" -f $gradesAll.Rows[0].GradeLevelID, $gradesAll.Rows[0].GradeDescription) 'GetGradeLevels(all): "All grades" row first with ID 0'
Assert-Equal 2 $reference.GetGradeLevels($false).Rows.Count 'GetGradeLevels: the "All" row never leaks into the next call'

$types = $funding.GetFundingTypes($false)
Assert-Equal 4 $types.Rows.Count 'GetFundingTypes: four FUNDING_TYPE_n rows'
Assert-Equal 'FUNDING_TYPE_1|Scholarship|Scholarship' ($types.Rows[0].ParameterCode + '|' + $types.Rows[0].ParameterValue + '|' + $types.Rows[0].FundingType) 'GetFundingTypes: FundingType column holds the type text'
$typesAll = $funding.GetFundingTypes($true)
Assert-Equal 5 $typesAll.Rows.Count 'GetFundingTypes(all): five rows'
Assert-Equal '|All funding types|' ($typesAll.Rows[0].ParameterCode + '|' + $typesAll.Rows[0].ParameterValue + '|' + $typesAll.Rows[0].FundingType) 'GetFundingTypes(all): "All funding types" row first, code and type ""'

Assert-Equal 'Commerce, Science' (Get-Names $reference.GetFaculties($false, $false) 'FacultyName') 'GetFaculties: active only, by name'
Assert-Equal 'Commerce, Humanities, Science' (Get-Names $reference.GetFaculties($true, $false) 'FacultyName') 'GetFaculties: inactive included on request'
$facultiesAll = $reference.GetFaculties($true, $true)
Assert-Equal 4 $facultiesAll.Count 'GetFaculties(all row): four items'
Assert-Equal '0|All faculties' ("{0}|{1}" -f $facultiesAll[0].FacultyID, $facultiesAll[0].FacultyName) 'GetFaculties(all row): "All faculties" first with ID 0'
Assert-Equal 4 $reference.AllFaculties.Count 'AllFaculties holds the last list'
Assert-Equal 2 $reference.GetActiveFaculties().Count 'GetActiveFaculties: two active faculties'

Assert-Equal 'Mathematical Literacy, Mathematics, Physical Sciences' (Get-Names $reference.GetActiveSubjects() 'SubjectName') 'GetActiveSubjects: active subjects by name'
Assert-Equal 4 $reference.GetSubjects($true).Count 'GetSubjects: inactive included on request'
$alternatives = $reference.GetAlternativeSubjects()
Assert-Equal 4 $alternatives.Count 'GetAlternativeSubjects: None plus three active subjects'
Assert-Equal '0|None' ("{0}|{1}" -f $alternatives[0].SubjectID, $alternatives[0].SubjectName) 'GetAlternativeSubjects: "None" first with ID 0'

$requirementTypes = $reference.GetRequirementTypes()
Assert-Equal 'Required, Recommended' (($requirementTypes.Rows | ForEach-Object { $_.RequirementType }) -join ', ') 'GetRequirementTypes: Required and Recommended'
Assert-True ($requirementTypes.Columns.Contains('Description')) 'GetRequirementTypes: has a Description column to display'

Assert-Equal 7 $reference.GetParameterInt('MIN_SUBJECT_COUNT') 'GetParameterInt: MIN_SUBJECT_COUNT is 7'
Assert-True ($reference.GetParameterInt('NO_SUCH_CODE') -eq $null) 'GetParameterInt: a missing code returns null'
Assert-True ($reference.GetParameterInt('PROVINCE_1') -eq $null) 'GetParameterInt: a non-number returns null'
Assert-True ($reference.GetParameterInt('') -eq $null) 'GetParameterInt: a blank code returns null'
Assert-Equal 0 $reference.SearchActiveProgrammes('  ').Count 'SearchActiveProgrammes: blank text returns none'

# ---------------------------------------------------------------------------------------------
# Faculty maintenance (UI 13.1)
# ---------------------------------------------------------------------------------------------
$engineeringId = $reference.SaveFaculty($adminId, $null, '  Engineering  ', $null)
Assert-True ($engineeringId -gt 0) 'SaveFaculty: Add returns the new FacultyID'
Assert-Equal 'Engineering|True' ((Get-Value "SELECT FacultyName + '|' + CAST(CASE WHEN IsActive = 1 THEN 'True' ELSE 'False' END AS VARCHAR(5)) FROM Faculty WHERE FacultyID = $engineeringId")) 'SaveFaculty: stored trimmed and active'
Assert-Equal $engineeringId ($reference.SaveFaculty($adminId, $engineeringId, 'Engineering and the Built Environment', $null)) 'SaveFaculty: Edit returns the same FacultyID'
Assert-Equal 'Engineering and the Built Environment' (Get-Value "SELECT FacultyName FROM Faculty WHERE FacultyID = $engineeringId") 'SaveFaculty: renamed'
Assert-Equal 0 ($reference.ValidateFaculty($engineeringId, 'Engineering and the Built Environment')).Count 'ValidateFaculty: keeping its own name is not a duplicate'
$duplicate = $reference.ValidateFaculty($null, 'commerce')
Assert-Equal 1 $duplicate.Count 'ValidateFaculty: duplicate name (any case) gives one message'
Assert-True ($duplicate[0] -like '*already exists*') 'ValidateFaculty: duplicate message says the name already exists'
Assert-Throws { $reference.SaveFaculty($adminId, $null, 'Commerce', $true) } 'ArgumentException' 'SaveFaculty: duplicate name refused'
Assert-Throws { $reference.SaveFaculty($adminId, $null, '   ', $true) } 'ArgumentException' 'SaveFaculty: blank name refused'
Assert-True (($reference.ValidateFaculty($null, ('x' * 61)))[0] -like '*60 characters*') 'ValidateFaculty: 61 characters refused, naming the limit'
Assert-Throws { $reference.SaveFaculty($advisorId, $null, 'Law', $true) } 'UnauthorizedAccessException' 'SaveFaculty: Career Advisor refused'
Assert-Throws { $reference.DeactivateFaculty($advisorId, $engineeringId) } 'UnauthorizedAccessException' 'DeactivateFaculty: Career Advisor refused'
Assert-Throws { $reference.SaveFaculty(99999, $null, 'Law', $true) } 'UnauthorizedAccessException' 'SaveFaculty: unknown user refused'
Assert-Equal 0 (Get-Value "SELECT COUNT(*) FROM Faculty WHERE FacultyName = 'Law'") 'SaveFaculty: nothing written when refused'
$reference.DeactivateFaculty($adminId, $engineeringId)
Assert-Equal $false (Get-Value "SELECT IsActive FROM Faculty WHERE FacultyID = $engineeringId") 'DeactivateFaculty: IsActive = 0'
Assert-Equal 2 $reference.GetActiveFaculties().Count 'DeactivateFaculty: drops out of the active list'
[void]$reference.SaveFaculty($adminId, $engineeringId, 'Engineering', $null)
Assert-Equal 'Engineering|False' ((Get-Value "SELECT FacultyName + '|' + CASE WHEN IsActive = 1 THEN 'True' ELSE 'False' END FROM Faculty WHERE FacultyID = $engineeringId")) 'SaveFaculty: rename with isActive null keeps it inactive'
Assert-Throws { $reference.DeactivateFaculty($adminId, 99999) } 'ArgumentException' 'DeactivateFaculty: unknown faculty refused'

# ---------------------------------------------------------------------------------------------
# Interest categories, questions and weights (UI 13.4, 13.5, UI 4)
# ---------------------------------------------------------------------------------------------
$categoryTable = $interests.GetInterestCategories($true)
Assert-Equal 'CategoryID, CategoryName, QuestionCount, MappedProgrammeCount, IsActive' (($categoryTable.Columns | ForEach-Object { $_.ColumnName }) -join ', ') 'GetInterestCategories: grid columns'
$businessRow = $categoryTable.Select("CategoryID = $businessId")[0]
Assert-Equal 2 $businessRow.QuestionCount 'GetInterestCategories: QuestionCount counts active questions only'
Assert-Equal 2 $businessRow.MappedProgrammeCount 'GetInterestCategories: MappedProgrammeCount counts mappings'
Assert-Equal 3 $categoryTable.Rows.Count 'GetInterestCategories(true): inactive category included'
Assert-Equal 2 $interests.GetInterestCategories($false).Rows.Count 'GetInterestCategories(false): inactive category left out'
Assert-True (-not [object]::ReferenceEquals($interests.GetInterestCategories($false), $interests.GetInterestCategories($false))) 'GetInterestCategories: a new table every call'

$artsId = $interests.SaveInterestCategory($adminId, $null, 'Creative arts', $true)
Assert-True ($artsId -gt 0) 'SaveInterestCategory: Add returns the new CategoryID'
Assert-Equal $artsId ($interests.SaveInterestCategory($adminId, $artsId, 'Creative and performing arts', $true)) 'SaveInterestCategory: Edit returns the same ID'
Assert-Equal 'Creative and performing arts' (Get-Value "SELECT CategoryName FROM InterestCategory WHERE CategoryID = $artsId") 'SaveInterestCategory: renamed'
Assert-Throws { $interests.SaveInterestCategory($adminId, $null, 'Business and finance', $true) } 'ArgumentException' 'SaveInterestCategory: duplicate name refused'
Assert-Throws { $interests.SaveInterestCategory($adminId, $null, '', $true) } 'ArgumentException' 'SaveInterestCategory: blank name refused'
Assert-True (($interests.ValidateCategory($null, ('y' * 61)))[0] -like '*60 characters*') 'ValidateCategory: 61 characters refused'
Assert-Throws { $interests.SaveInterestCategory($advisorId, $null, 'Sport', $true) } 'UnauthorizedAccessException' 'SaveInterestCategory: Career Advisor refused'

$drawId = $interests.SaveQuestion($adminId, $null, 'I enjoy drawing and design', $artsId, 7, $true)
Assert-True ($drawId -gt 0) 'SaveQuestion: Add returns the new QuestionID'
Assert-Equal $drawId ($interests.SaveQuestion($adminId, $drawId, 'I enjoy drawing, design and making things', $artsId, 8, $true)) 'SaveQuestion: Edit returns the same ID'
Assert-Equal 'I enjoy drawing, design and making things|8' (Get-Value "SELECT QuestionText + '|' + CAST(DisplayOrder AS VARCHAR(3)) FROM InterestQuestion WHERE QuestionID = $drawId") 'SaveQuestion: text and order updated'
Assert-Throws { $interests.SaveQuestion($adminId, $null, 'Statement', $oldCatId, 9, $true) } 'ArgumentException' 'SaveQuestion: inactive category refused'
Assert-Throws { $interests.SaveQuestion($adminId, $null, 'Statement', 99999, 9, $true) } 'ArgumentException' 'SaveQuestion: unknown category refused'
Assert-Throws { $interests.SaveQuestion($adminId, $null, 'Statement', $artsId, 0, $true) } 'ArgumentException' 'SaveQuestion: display order 0 refused'
Assert-Throws { $interests.SaveQuestion($adminId, $null, 'Statement', $artsId, 100, $true) } 'ArgumentException' 'SaveQuestion: display order 100 refused'
Assert-Throws { $interests.SaveQuestion($adminId, $null, ' ', $artsId, 9, $true) } 'ArgumentException' 'SaveQuestion: blank statement refused'
Assert-True (($interests.ValidateQuestion(('z' * 256), $artsId, 9))[0] -like '*255 characters*') 'ValidateQuestion: 256 characters refused'
Assert-Equal 0 ($interests.ValidateQuestion('Fine statement', $artsId, 99)).Count 'ValidateQuestion: order 99 and an active category are valid'
Assert-Throws { $interests.SaveQuestion($advisorId, $null, 'Statement', $artsId, 9, $true) } 'UnauthorizedAccessException' 'SaveQuestion: Career Advisor refused'
Assert-Equal 1 ($interests.GetInterestCategories($true).Select("CategoryID = $artsId")[0].QuestionCount) 'GetInterestCategories: the new question is counted'

$questionGrid = $interests.GetQuestions($null, $true)
Assert-Equal 'QuestionID, CategoryID, DisplayOrder, QuestionText, CategoryName, IsActive' (($questionGrid.Columns | ForEach-Object { $_.ColumnName }) -join ', ') 'GetQuestions: grid columns'
Assert-Equal 7 $questionGrid.Rows.Count 'GetQuestions(null, true): every question'
Assert-Equal 'I like planning how a business could grow' $questionGrid.Rows[0].QuestionText 'GetQuestions: ordered by DisplayOrder'
Assert-Equal 3 $interests.GetQuestions($businessId, $true).Rows.Count 'GetQuestions(category, true): one category, inactive included'
Assert-Equal 5 $interests.GetQuestions($null, $false).Rows.Count 'GetQuestions(null, false): inactive questions and inactive categories left out'

$active = $interests.GetActiveQuestions()
Assert-Equal 'QuestionID, QuestionText, CategoryID, CategoryName, DisplayOrder' (($active.Columns | ForEach-Object { $_.ColumnName }) -join ', ') 'GetActiveQuestions: UI 4 columns'
Assert-Equal 5 $active.Rows.Count 'GetActiveQuestions: active questions in active categories'
Assert-Equal 'Business and finance' $active.Rows[0].CategoryName 'GetActiveQuestions: carries the category name'
Assert-Equal 5 $interests.AllQuestions.Count 'AllQuestions holds the last list'

$interests.DeactivateQuestion($adminId, $drawId)
Assert-Equal $false (Get-Value "SELECT IsActive FROM InterestQuestion WHERE QuestionID = $drawId") 'DeactivateQuestion: IsActive = 0'
Assert-Throws { $interests.DeactivateQuestion($advisorId, $qNumbers) } 'UnauthorizedAccessException' 'DeactivateQuestion: Career Advisor refused'
$interests.DeactivateInterestCategory($adminId, $artsId)
Assert-Equal $false (Get-Value "SELECT IsActive FROM InterestCategory WHERE CategoryID = $artsId") 'DeactivateInterestCategory: IsActive = 0'
Assert-Throws { $interests.DeactivateInterestCategory($advisorId, $businessId) } 'UnauthorizedAccessException' 'DeactivateInterestCategory: Career Advisor refused'
Assert-Throws { $interests.DeactivateInterestCategory($adminId, 99999) } 'ArgumentException' 'DeactivateInterestCategory: unknown category refused'

# Relevance weights. Role and value checks run before the programme lookup.
Assert-Throws { $interests.SaveRelevanceWeight($advisorId, $progIS, $sciCatId, [decimal]'0.5') } 'UnauthorizedAccessException' 'SaveRelevanceWeight: Career Advisor refused'
Assert-Throws { $interests.SaveRelevanceWeight($adminId, $progIS, $sciCatId, [decimal]'1.5') } 'ArgumentException' 'SaveRelevanceWeight: 1.5 refused'
Assert-Throws { $interests.SaveRelevanceWeight($adminId, $progIS, $sciCatId, [decimal]'-0.1') } 'ArgumentException' 'SaveRelevanceWeight: -0.1 refused'
Assert-Throws { $interests.SaveRelevanceWeight($adminId, $progIS, $sciCatId, [decimal]'0.333') } 'ArgumentException' 'SaveRelevanceWeight: three decimals refused'
Assert-Throws { $interests.SaveRelevanceWeight($adminId, $progIS, 99999, [decimal]'0.5') } 'ArgumentException' 'SaveRelevanceWeight: unknown category refused'

function Save-Weight([int]$ProgrammeId, [int]$CategoryId, [decimal]$Weight) {
    # The controller checks the programme with DegreeProgrammeRepository.FindById. While that is
    # still a stub in this worktree, the repository method is called directly instead.
    $stubbed = Test-ThrowsType { $interests.SaveRelevanceWeight($adminId, $ProgrammeId, $CategoryId, $Weight) } 'NotImplementedException'
    if ($stubbed) {
        Write-Host '      (DegreeProgrammeRepository.FindById is a stub: InterestRepository.SaveRelevanceWeight called directly)' -ForegroundColor Yellow
        (New-Object FuturePath.DatabaseLayer.InterestRepository).SaveRelevanceWeight($ProgrammeId, $CategoryId, $Weight)
    }
}
Save-Weight $progCS $sciCatId ([decimal]'0.75')
$weights = $interests.GetRelevanceWeights($sciCatId)
Assert-Equal 'ProgrammeID, ProgrammeName, RelevanceWeight' (($weights.Columns | ForEach-Object { $_.ColumnName }) -join ', ') 'GetRelevanceWeights: grid columns'
Assert-Equal "1|$progCS|0.75" ("{0}|{1}|{2}" -f $weights.Rows.Count, $weights.Rows[0].ProgrammeID, $weights.Rows[0].RelevanceWeight.ToString([System.Globalization.CultureInfo]::InvariantCulture)) 'SaveRelevanceWeight: mapping inserted'
Save-Weight $progCS $sciCatId ([decimal]'0.5')
$weights = $interests.GetRelevanceWeights($sciCatId)
Assert-Equal '1|0.50' ("{0}|{1}" -f $weights.Rows.Count, $weights.Rows[0].RelevanceWeight.ToString([System.Globalization.CultureInfo]::InvariantCulture)) 'SaveRelevanceWeight: second save updates the same mapping'
$businessWeights = $interests.GetRelevanceWeights($businessId)
Assert-Equal 'BCom Information Systems, BSc Computer Science' (($businessWeights.Rows | ForEach-Object { $_.ProgrammeName }) -join ', ') 'GetRelevanceWeights: ordered by programme name'

# Running category scores (UI 4).
$answers = New-Object 'System.Collections.Generic.Dictionary[int,int]'
$answers[$qNumbers] = 5; $answers[$qPlanning] = 3; $answers[$qHow] = 4
$answers[99999] = 5; $answers[$qRetired] = 1; $answers[$qOldCat] = 1
$scores = $interests.CalculateCategoryScores($answers)
Assert-Equal 2 $scores.Count 'CalculateCategoryScores: one score per answered active category; unknown and inactive questions ignored'
Assert-Equal 'Business and finance=75, Science and research=75' (($scores | ForEach-Object { $_.Key + '=' + $_.Value.ToString('0.##', [System.Globalization.CultureInfo]::InvariantCulture) }) -join ', ') 'CalculateCategoryScores: equal scores ordered by name'
$answers[$qLab] = 5
$scores = $interests.CalculateCategoryScores($answers)
Assert-Equal 'Science and research=87.5, Business and finance=75' (($scores | ForEach-Object { $_.Key + '=' + $_.Value.ToString('0.##', [System.Globalization.CultureInfo]::InvariantCulture) }) -join ', ') 'CalculateCategoryScores: highest first'
Assert-Equal 0 $interests.CalculateCategoryScores((New-Object 'System.Collections.Generic.Dictionary[int,int]')).Count 'CalculateCategoryScores: no answers, no scores'
$badAnswer = New-Object 'System.Collections.Generic.Dictionary[int,int]'; $badAnswer[$qNumbers] = 6
Assert-Throws { $interests.CalculateCategoryScores($badAnswer) } 'ArgumentException' 'CalculateCategoryScores: an answer of 6 refused'

# SaveResponses: only the checks that run before LearnerRepository (a stub here).
Assert-Throws { $interests.SaveResponses(1, (New-Object 'System.Collections.Generic.Dictionary[int,int]')) } 'ArgumentException' 'SaveResponses: no answers refused'
$unknownAnswer = New-Object 'System.Collections.Generic.Dictionary[int,int]'; $unknownAnswer[$qRetired] = 3
Assert-Throws { $interests.SaveResponses(1, $unknownAnswer) } 'ArgumentException' 'SaveResponses: an inactive question refused'
$zeroAnswer = New-Object 'System.Collections.Generic.Dictionary[int,int]'; $zeroAnswer[$qNumbers] = 0
Assert-Throws { $interests.SaveResponses(1, $zeroAnswer) } 'ArgumentException' 'SaveResponses: an answer of 0 refused'

# ---------------------------------------------------------------------------------------------
# Funding (UI 6.2b learner view, UI 13.3 maintenance)
# ---------------------------------------------------------------------------------------------
Assert-Equal 'Closed Aid, Programme Bursary, Open Bursary' (Get-Names $funding.GetActiveFunding($commerceId, $progIS, '', $false) 'FundingName') 'GetActiveFunding: faculty OR programme, nearest closing date first, no date last'
Assert-Equal 'Programme Bursary, Open Bursary' (Get-Names $funding.GetActiveFunding($commerceId, $progIS, 'Bursary', $false) 'FundingName') 'GetActiveFunding: funding type filter'
Assert-Equal 'Programme Bursary, Open Bursary' (Get-Names $funding.GetActiveFunding($commerceId, $progIS, $null, $true) 'FundingName') 'GetActiveFunding: open-only filter drops a closed option'
Assert-Equal 'Closed Aid, Open Bursary' (Get-Names $funding.GetActiveFunding($commerceId, $null, '', $false) 'FundingName') 'GetActiveFunding: faculty only; inactive funding excluded'
Assert-Equal 0 $funding.GetActiveFunding($humanitiesId, $null, '', $false).Count 'GetActiveFunding: a link to an inactive faculty does not count'
Assert-Equal 0 $funding.GetActiveFunding($null, $progOld, '', $false).Count 'GetActiveFunding: a link to an inactive programme does not count'
Assert-Equal 0 $funding.GetActiveFunding($null, $progBA, '', $false).Count 'GetActiveFunding: a link to a programme under an inactive faculty does not count'
$allActive = $funding.GetActiveFunding($null, $null, '', $false)
Assert-Equal 7 $allActive.Count 'GetActiveFunding: no faculty or programme gives every active option'
Assert-True (-not ((Get-Names $allActive 'FundingName') -like '*Inactive Grant*')) 'GetActiveFunding: inactive funding never listed'
Assert-Equal 7 $funding.GetActiveFunding(0, 0, '', $false).Count 'GetActiveFunding: ID 0 ("All") means no filter'
$openBursary = ($funding.GetActiveFunding($commerceId, $null, '', $false) | Where-Object { $_.FundingName -eq 'Open Bursary' })
Assert-Equal 'Commerce' $openBursary.LinkedTo 'GetActiveFunding: LinkedTo leaves out the inactive faculty link'
Assert-Equal 2 $funding.AllFundingOptions.Count 'AllFundingOptions holds the last list'

$closing = [datetime]::Today.AddDays(45)
$meritId = $funding.SaveFunding($adminId, $null, ' Commerce Merit Award ', 'bursary', 'Grade 12 learners with 70% or more', 'Certified ID copy', $closing,
                                'awards@example.ac.za', $true, $false, $true, [int[]]@($commerceId, $commerceId), [int[]]@($progIS))
Assert-True ($meritId -gt 0) 'SaveFunding: Add returns the new FundingID'
$saved = $funding.GetFunding($meritId)
Assert-Equal 'Commerce Merit Award|Bursary' ($saved.FundingName + '|' + $saved.FundingType) 'SaveFunding: name trimmed and type stored as the parameter text'
Assert-Equal "1|$commerceId|1|$progIS" ("{0}|{1}|{2}|{3}" -f $saved.FacultyIds.Count, $saved.FacultyIds[0], $saved.ProgrammeIds.Count, $saved.ProgrammeIds[0]) 'SaveFunding: links saved, duplicate faculty ignored'
Assert-Equal 'Commerce, BCom Information Systems' $saved.LinkedTo 'GetFunding: links carry their names'
Assert-Equal $closing.ToString('yyyy-MM-dd') $saved.ClosingDate.ToString('yyyy-MM-dd') 'SaveFunding: closing date stored'

Assert-Equal $meritId ($funding.SaveFunding($adminId, $meritId, 'Commerce Merit Award', 'Scholarship', 'Grade 12 learners with 75% or more', '', $null,
                                         '', $true, $false, $true, [int[]]@($scienceId), [int[]]@())) 'SaveFunding: Update returns the same FundingID'
$saved = $funding.GetFunding($meritId)
Assert-Equal "Scholarship|1|$scienceId|0" ("{0}|{1}|{2}|{3}" -f $saved.FundingType, $saved.FacultyIds.Count, $saved.FacultyIds[0], $saved.ProgrammeIds.Count) 'SaveFunding: Update replaces the links'
Assert-True ($saved.ClosingDate -eq $null) 'SaveFunding: closing date cleared'
Assert-Equal 1 (Get-Value "SELECT COUNT(*) FROM FundingOption WHERE FundingID = $meritId AND RequiredDocuments IS NULL AND ContactInformation IS NULL") 'SaveFunding: blank optional text stored as NULL'

Assert-Throws { $funding.SaveFunding($adminId, $null, 'Open Bursary', 'Bursary', 'x', '', $null, '', $false, $false, $true, [int[]]@(), [int[]]@()) } 'ArgumentException' 'SaveFunding: duplicate name refused'
Assert-Throws { $funding.SaveFunding($adminId, $null, 'New Loan', 'Loan', 'x', '', $null, '', $false, $false, $true, [int[]]@(), [int[]]@()) } 'ArgumentException' 'SaveFunding: a type outside FUNDING_TYPE_n refused'
Assert-Throws { $funding.SaveFunding($adminId, $null, 'New Grant', 'Bursary', '  ', '', $null, '', $false, $false, $true, [int[]]@(), [int[]]@()) } 'ArgumentException' 'SaveFunding: blank eligibility refused'
Assert-Throws { $funding.SaveFunding($advisorId, $null, 'New Grant', 'Bursary', 'x', '', $null, '', $false, $false, $true, [int[]]@(), [int[]]@()) } 'UnauthorizedAccessException' 'SaveFunding: Career Advisor refused'
$fundingMessages = $funding.ValidateFunding($null, ('n' * 101), '', ('e' * 501), ('d' * 256), $null, ('c' * 151))
Assert-Equal 5 $fundingMessages.Count 'ValidateFunding: one message per problem (name, type, eligibility, documents, contact)'
Assert-True ($fundingMessages[0] -like '*100 characters*') 'ValidateFunding: the name message names the limit'

Assert-Equal 9 $funding.GetFundingOptions($true).Count 'GetFundingOptions(true): every option'
$funding.DeactivateFunding($adminId, $meritId)
Assert-Equal $false (Get-Value "SELECT IsActive FROM FundingOption WHERE FundingID = $meritId") 'DeactivateFunding: IsActive = 0'
Assert-Equal 7 $funding.GetFundingOptions($false).Count 'GetFundingOptions(false): inactive options left out'
Assert-True (-not ((Get-Names $funding.GetActiveFunding($scienceId, $null, '', $false) 'FundingName') -like '*Commerce Merit Award*')) 'DeactivateFunding: no longer offered to learners'
Assert-Throws { $funding.DeactivateFunding($advisorId, $fOpen) } 'UnauthorizedAccessException' 'DeactivateFunding: Career Advisor refused'
Assert-Throws { $funding.DeactivateFunding($adminId, 99999) } 'ArgumentException' 'DeactivateFunding: unknown option refused'
Assert-True ($funding.GetFunding(99999) -eq $null) 'GetFunding: unknown option returns null'

# ---------------------------------------------------------------------------------------------
# User accounts (UI 13.6)
# ---------------------------------------------------------------------------------------------
Assert-Equal 'Administrator, Career Advisor' ($users.GetRoles() -join ', ') 'GetRoles: the two staff roles'

$newId = $users.SaveAccount($adminId, $null, ' new.advisor ', 'New Advisor', 'Career Advisor', $true, 'First-Pass-1')
Assert-True ($newId -gt 0) 'SaveAccount: Add returns the new UserID'
$storedHash = Get-Value "SELECT PasswordHash FROM UserAccount WHERE UserID = $newId"
Assert-True ($storedHash -ne 'First-Pass-1' -and [FuturePath.ApplicationLayer.UserAccount]::VerifyPassword('First-Pass-1', $storedHash)) 'SaveAccount: only a salted hash is stored'
Assert-Equal 'New Advisor' $login.Authenticate('new.advisor', 'First-Pass-1').FullName 'SaveAccount: the new account can sign in'

[void]$users.SaveAccount($adminId, $newId, 'new.advisor', 'New Advisor Renamed', 'Career Advisor', $true, '')
Assert-Equal 'New Advisor Renamed' (Get-Value "SELECT FullName FROM UserAccount WHERE UserID = $newId") 'SaveAccount: Update changes the full name'
Assert-Equal $storedHash (Get-Value "SELECT PasswordHash FROM UserAccount WHERE UserID = $newId") 'SaveAccount: a blank password keeps the hash'

[void]$users.SaveAccount($adminId, $newId, 'new.advisor', 'New Advisor Renamed', 'Administrator', $true, 'Second-Pass-2')
Assert-True ($login.Authenticate('new.advisor', 'Second-Pass-2') -ne $null) 'SaveAccount: password reset, the new password signs in'
Assert-True ($login.Authenticate('new.advisor', 'First-Pass-1') -eq $null) 'SaveAccount: password reset, the old password no longer signs in'
Assert-Equal 'Administrator' (Get-Value "SELECT Role FROM UserAccount WHERE UserID = $newId") 'SaveAccount: role changed'

Assert-Throws { $users.SaveAccount($adminId, $null, 'SMOKE.ADMIN', 'Copy', 'Career Advisor', $true, 'Another-Pass-1') } 'ArgumentException' 'SaveAccount: duplicate username (any case) refused'
Assert-Throws { $users.SaveAccount($adminId, $null, 'has space', 'Someone', 'Career Advisor', $true, 'Another-Pass-1') } 'ArgumentException' 'SaveAccount: username with a space refused'
Assert-Throws { $users.SaveAccount($adminId, $null, 'someone', 'Someone', 'Learner', $true, 'Another-Pass-1') } 'ArgumentException' 'SaveAccount: unknown role refused'
Assert-Throws { $users.SaveAccount($adminId, $null, 'someone', 'Someone', 'Career Advisor', $true, 'short') } 'ArgumentException' 'SaveAccount: password under 8 characters refused'
Assert-Throws { $users.SaveAccount($adminId, $null, 'someone', 'Someone', 'Career Advisor', $true, '') } 'ArgumentException' 'SaveAccount: Add without a password refused'
Assert-Throws { $users.SaveAccount($adminId, $null, 'someone', ' ', 'Career Advisor', $true, 'Another-Pass-1') } 'ArgumentException' 'SaveAccount: blank full name refused'
Assert-Throws { $users.SaveAccount($adminId, $adminId, 'smoke.admin', 'Smoke Admin', 'Administrator', $false, '') } 'ArgumentException' 'SaveAccount: an Administrator cannot deactivate themselves'
Assert-Throws { $users.SaveAccount($adminId, $adminId, 'smoke.admin', 'Smoke Admin', 'Career Advisor', $true, '') } 'ArgumentException' 'SaveAccount: an Administrator cannot remove their own role'
Assert-Throws { $users.DeactivateAccount($adminId, $adminId) } 'ArgumentException' 'DeactivateAccount: refuses your own account'
Assert-Throws { $users.SaveAccount($advisorId, $null, 'someone', 'Someone', 'Career Advisor', $true, 'Another-Pass-1') } 'UnauthorizedAccessException' 'SaveAccount: Career Advisor refused'
Assert-Throws { $users.GetAccounts($advisorId) } 'UnauthorizedAccessException' 'GetAccounts: Career Advisor refused'
Assert-Throws { $users.DeactivateAccount($advisorId, $newId) } 'UnauthorizedAccessException' 'DeactivateAccount: Career Advisor refused'
$validation = $users.ValidateAccount($adminId, $null, '', '', 'Nobody', $true, 'x')
Assert-Equal 4 $validation.Count 'ValidateAccount: one message per problem (username, name, role, password)'

$users.DeactivateAccount($adminId, $newId)
Assert-Equal $false (Get-Value "SELECT IsActive FROM UserAccount WHERE UserID = $newId") 'DeactivateAccount: IsActive = 0'
Assert-True ($login.Authenticate('new.advisor', 'Second-Pass-2') -eq $null) 'DeactivateAccount: the account can no longer sign in'
Assert-Throws { $users.DeactivateAccount($adminId, 99999) } 'ArgumentException' 'DeactivateAccount: unknown account refused'

$accountGrid = $users.GetAccounts($adminId)
Assert-Equal 'UserID, Username, FullName, Role, IsActive' (($accountGrid.Columns | ForEach-Object { $_.ColumnName }) -join ', ') 'GetAccounts: grid columns, no PasswordHash'
Assert-Equal 3 $accountGrid.Rows.Count 'GetAccounts: every account'
$hashes = @((Invoke-TestQuery $ctx 'SELECT PasswordHash FROM UserAccount').Rows | ForEach-Object { $_.PasswordHash })
$leaked = $false
foreach ($row in $accountGrid.Rows) { foreach ($cell in $row.ItemArray) { if ($hashes -contains [string]$cell) { $leaked = $true } } }
foreach ($account in $users.AllAccounts) { if ($account.PasswordHash -ne '') { $leaked = $true } }
Assert-True (-not $leaked) 'GetAccounts: no hash in the grid or in AllAccounts'
Assert-Equal 'Career Advisor' ($accountGrid.Select("Username = 'smoke.advisor'")[0].Role) 'GetAccounts: Role is the role text'
Assert-True ($login.Authenticate('smoke.admin', 'Admin-Test-1') -ne $null) 'GetAccounts: blanking AllAccounts does not touch the stored hash'

Complete-FuturePathTest $ctx
