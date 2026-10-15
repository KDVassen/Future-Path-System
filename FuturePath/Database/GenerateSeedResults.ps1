<#
.SYNOPSIS
    Stores the seeded sessions' results using the real recommendation engine.

.DESCRIPTION
    SeedData.sql seeds only the INPUTS of each advisory session (subjects, marks, interest
    answers, chosen programme). This script runs the application's own controllers over every
    seeded session, so every stored RecommendationResult is produced by the engine and not
    typed in by hand:
      1. EligibilityController.CheckEligibility   (the selected programme's result)
      2. RecommendationController.GenerateAlternatives (the ranked alternatives)
      3. ReportController.CompleteLearnerSession  (sessions 6-12 only: Completed + FP-LSR export row)
    Sessions 1-5 (Part B) and 13 stay In progress, as planned in SeedData.sql.

    BuildDatabase.ps1 runs this automatically when the project has been built. Run it on its
    own after changing the engine, the scoring parameters or the seed rows:
        powershell -NoProfile -ExecutionPolicy Bypass -File FuturePath\Database\GenerateSeedResults.ps1
    It changes FuturePath\FuturePath.mdf (the project copy), so stop the app and close any
    Visual Studio connection to the database first.
#>
$ErrorActionPreference = 'Stop'

$projectFolder = Split-Path -Parent $PSScriptRoot
$exePath = Join-Path $projectFolder 'bin\Debug\FuturePath.exe'
$mdfPath = Join-Path $projectFolder 'FuturePath.mdf'
if (-not (Test-Path $exePath)) {
    throw "Build the project first: $exePath was not found."
}
[void][System.Reflection.Assembly]::LoadFrom($exePath)
$connectionString = "Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=$mdfPath;Integrated Security=True;Connect Timeout=60"
[FuturePath.DatabaseLayer.DB]::ConnectionString = $connectionString

# Sessions completed and exported by the learner on UI 7, with the format they chose (SeedData.sql session plan).
$completedSessions = @{ 6 = 'PDF'; 7 = 'DOCX'; 8 = 'PDF'; 9 = 'PDF'; 10 = 'PDF'; 11 = 'DOCX'; 12 = 'PDF' }

function Get-Rows([string]$sql) {
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $adapter = New-Object System.Data.SqlClient.SqlDataAdapter($sql, $connection)
    $table = New-Object System.Data.DataTable
    try { [void]$adapter.Fill($table) } finally { $connection.Close() }
    return ,$table
}

try {
    $sessions = Get-Rows 'SELECT SessionID, LearnerID, SelectedProgrammeID, SessionDate, SessionStatus FROM AdvisorySession ORDER BY SessionID'
    $eligibility = New-Object FuturePath.ApplicationLayer.EligibilityController
    $recommendation = New-Object FuturePath.ApplicationLayer.RecommendationController
    $reports = New-Object FuturePath.ApplicationLayer.ReportController

    foreach ($session in $sessions.Rows) {
        $sessionID = [int]$session.SessionID
        $learnerID = [int]$session.LearnerID
        if ([string]$session.SessionStatus -eq 'Completed') {
            # Already completed by an earlier run: its results and export row are final.
            Write-Host ("Session {0}: already Completed, left unchanged" -f $sessionID)
            continue
        }
        if ($session.SelectedProgrammeID -is [DBNull]) {
            Write-Host ("Session {0}: no programme selected, skipped" -f $sessionID)
            continue
        }
        $programmeID = [int]$session.SelectedProgrammeID

        $outcome = $eligibility.CheckEligibility($learnerID, $programmeID)
        $alternatives = $recommendation.GenerateAlternatives($learnerID, $programmeID)
        $line = "Session {0}: {1} - {2}" -f $sessionID, $outcome.Category, $outcome.Explanation
        Write-Host $line
        Write-Host ("            {0} rows shown on UI 6.1a" -f $alternatives.Count)

        if ($completedSessions.ContainsKey($sessionID)) {
            # Completed half an hour after the session started (the learner's own UI 7 export).
            $completedOn = ([datetime]$session.SessionDate).AddMinutes(30)
            $reports.CompleteLearnerSession($sessionID, $completedSessions[$sessionID], $completedOn)
            Write-Host ("            Completed and exported ({0})" -f $completedSessions[$sessionID])
        }
    }
}
finally {
    # Release the project .mdf so it can be committed and attached by the app.
    [System.Data.SqlClient.SqlConnection]::ClearAllPools()
    $master = New-Object System.Data.SqlClient.SqlConnection('Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True')
    $master.Open()
    try {
        $find = $master.CreateCommand()
        $find.CommandText = 'SELECT DB_NAME(database_id) FROM sys.master_files WHERE physical_name = @path'
        [void]$find.Parameters.AddWithValue('@path', $mdfPath)
        $name = $find.ExecuteScalar()
        if ($name -ne $null -and $name -ne [DBNull]::Value) {
            $detach = $master.CreateCommand()
            $detach.CommandText = 'ALTER DATABASE [' + ([string]$name).Replace(']', ']]') + '] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; EXEC master.dbo.sp_detach_db @dbname = @name'
            [void]$detach.Parameters.AddWithValue('@name', [string]$name)
            [void]$detach.ExecuteNonQuery()
        }
    } finally {
        $master.Close()
    }
}
Write-Host "Done: results stored in $mdfPath"
