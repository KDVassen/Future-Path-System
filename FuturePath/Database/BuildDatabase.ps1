<#
.SYNOPSIS
    Rebuilds FuturePath\FuturePath.mdf (and FuturePath_log.ldf) from the SQL scripts.

.DESCRIPTION
    Creates a fresh database on (LocalDB)\MSSQLLocalDB with its files in the FuturePath
    project folder, runs CreateSchema.sql and then SeedData.sql, and detaches the database
    so the .mdf can be attached by the application (|DataDirectory|\FuturePath.mdf).

    Use it when the committed .mdf will not open on your machine (an older LocalDB
    version), or after changing CreateSchema.sql / SeedData.sql.
    Only needs LocalDB (installed with Visual Studio's data workload). No sqlcmd needed.

    Close Visual Studio's Server Explorer connection and stop the app first: LocalDB keeps
    an attached .mdf locked. The script runs "sqllocaldb stop" to release old locks.

.PARAMETER SchemaOnly
    Build the empty schema without running SeedData.sql.

.PARAMETER KeepLocalDbRunning
    Skip the "sqllocaldb stop" step. Use it when other programs (tests) are using LocalDB at the
    same time and the .mdf is not attached anywhere.

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File FuturePath\Database\BuildDatabase.ps1
#>
param(
    [switch]$SchemaOnly,
    [switch]$KeepLocalDbRunning
)

$ErrorActionPreference = 'Stop'

$databaseFolder = $PSScriptRoot
$projectFolder  = Split-Path -Parent $databaseFolder
$mdfPath        = Join-Path $projectFolder 'FuturePath.mdf'
$ldfPath        = Join-Path $projectFolder 'FuturePath_log.ldf'
$server         = '(localdb)\MSSQLLocalDB'
$buildName      = 'FuturePathBuild_' + [guid]::NewGuid().ToString('N').Substring(0, 8)

# Runs one .sql file. Batches are separated by lines that contain only GO.
function Invoke-SqlFile([System.Data.SqlClient.SqlConnection]$connection, [string]$path) {
    $script  = [System.IO.File]::ReadAllText($path)
    $batches = [regex]::Split($script, '^\s*GO\s*$', [System.Text.RegularExpressions.RegexOptions]::Multiline)
    foreach ($batch in $batches) {
        if ($batch.Trim().Length -eq 0) { continue }
        $command = $connection.CreateCommand()
        $command.CommandText = $batch
        $command.CommandTimeout = 120
        [void]$command.ExecuteNonQuery()
    }
}

function Invoke-Sql([System.Data.SqlClient.SqlConnection]$connection, [string]$sql) {
    $command = $connection.CreateCommand()
    $command.CommandText = $sql
    $command.CommandTimeout = 120
    [void]$command.ExecuteNonQuery()
}

& sqllocaldb start MSSQLLocalDB | Out-Null
if (-not $KeepLocalDbRunning) {
    Write-Host "Releasing old LocalDB locks..."
    & sqllocaldb stop MSSQLLocalDB -k | Out-Null
    & sqllocaldb start MSSQLLocalDB | Out-Null
}

foreach ($file in @($mdfPath, $ldfPath)) {
    if (Test-Path $file) { Remove-Item $file -Force }
}

$master = New-Object System.Data.SqlClient.SqlConnection("Data Source=$server;Initial Catalog=master;Integrated Security=True")
$master.Open()
try {
    Write-Host "Creating database files in $projectFolder"
    Invoke-Sql $master ("CREATE DATABASE [$buildName] ON (NAME = N'FuturePath', FILENAME = N'" + $mdfPath.Replace("'", "''") +
                        "') LOG ON (NAME = N'FuturePath_log', FILENAME = N'" + $ldfPath.Replace("'", "''") + "')")

    $database = New-Object System.Data.SqlClient.SqlConnection("Data Source=$server;Initial Catalog=$buildName;Integrated Security=True")
    $database.Open()
    try {
        Write-Host "Running CreateSchema.sql"
        Invoke-SqlFile $database (Join-Path $databaseFolder 'CreateSchema.sql')

        $seedPath = Join-Path $databaseFolder 'SeedData.sql'
        if (-not $SchemaOnly -and (Test-Path $seedPath)) {
            Write-Host "Running SeedData.sql"
            Invoke-SqlFile $database $seedPath
        }
    }
    finally {
        $database.Close()
        [System.Data.SqlClient.SqlConnection]::ClearAllPools()
    }

    Write-Host "Detaching the build database"
    Invoke-Sql $master "ALTER DATABASE [$buildName] SET SINGLE_USER WITH ROLLBACK IMMEDIATE"
    Invoke-Sql $master "EXEC master.dbo.sp_detach_db @dbname = N'$buildName'"
}
finally {
    $master.Close()
}

Write-Host "Done: $mdfPath"

# The seed holds only the sessions' inputs; their results come from the real engine.
if (-not $SchemaOnly) {
    $exePath = Join-Path $projectFolder 'bin\Debug\FuturePath.exe'
    if (Test-Path $exePath) {
        Write-Host "Storing the seeded sessions' results with the engine (GenerateSeedResults.ps1)"
        & powershell.exe -NoProfile -ExecutionPolicy Bypass -File (Join-Path $databaseFolder 'GenerateSeedResults.ps1')
        if ($LASTEXITCODE -ne 0) { throw "GenerateSeedResults.ps1 failed." }
    } else {
        Write-Host "Build the project, then run Database\GenerateSeedResults.ps1 to store the seeded sessions' results." -ForegroundColor Yellow
    }
}
