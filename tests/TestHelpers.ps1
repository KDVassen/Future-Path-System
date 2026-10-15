<#
.SYNOPSIS
    Shared helpers for the Future Path test scripts (Windows PowerShell 5.1).

.DESCRIPTION
    Dot-source this file from a test script:
        . "$PSScriptRoot\TestHelpers.ps1"
        $ctx = Initialize-FuturePathTest            # loads FuturePath.exe, attaches a COPY of the database
        ... call controllers, e.g. (New-Object FuturePath.ApplicationLayer.EligibilityController).CheckEligibility(1, 1)
        Assert-Equal 'STRONG' $outcome.Code 'Gadija -> Strong Match'
        Complete-FuturePathTest $ctx                # detaches and deletes the copy, prints the summary

    Rules
    - Build first (MSBuild or Visual Studio); tests load FuturePath\bin\Debug\FuturePath.exe.
    - Tests never touch FuturePath.mdf or bin\Debug\FuturePath.mdf: each run copies the project's
      FuturePath.mdf / FuturePath_log.ldf to a new temp folder and points DB.ConnectionString at it.
    - Run every test script in its own process (powershell.exe -File ...), because a loaded
      assembly stays locked until the process ends:
        powershell -NoProfile -ExecutionPolicy Bypass -File tests\PartB-Regression.ps1
    - Tests that insert their own fixture rows use Initialize-FuturePathTest -SchemaOnly (an empty
      database built from CreateSchema.sql); the others get a copy of the seeded FuturePath.mdf.
    - Engine-only tests need no database: call Initialize-FuturePathTest -NoDatabase and
      Set-TestSettings with the SystemParameter values to use.
#>

$ErrorActionPreference = 'Stop'

$script:RepoRoot     = Split-Path -Parent $PSScriptRoot
$script:ProjectDir   = Join-Path $script:RepoRoot 'FuturePath'
$script:ExePath      = Join-Path $script:ProjectDir 'bin\Debug\FuturePath.exe'
$script:TestsPassed  = 0
$script:TestsFailed  = 0
$script:Failures     = New-Object System.Collections.Generic.List[string]

function New-SchemaOnlyDatabase {
    <#
    .SYNOPSIS Builds an EMPTY database (CreateSchema.sql only, no seed rows) as FuturePath.mdf in a folder.
    #>
    param([string]$Folder)
    $name = 'FuturePathSchema_' + [guid]::NewGuid().ToString('N').Substring(0, 8)
    $mdf = Join-Path $Folder 'FuturePath.mdf'
    $ldf = Join-Path $Folder 'FuturePath_log.ldf'
    $master = New-Object System.Data.SqlClient.SqlConnection('Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=60')
    $master.Open()
    try {
        $create = $master.CreateCommand()
        $create.CommandText = "CREATE DATABASE [$name] ON (NAME = N'FuturePath', FILENAME = N'" + $mdf.Replace("'", "''") +
                              "') LOG ON (NAME = N'FuturePath_log', FILENAME = N'" + $ldf.Replace("'", "''") + "')"
        [void]$create.ExecuteNonQuery()

        $database = New-Object System.Data.SqlClient.SqlConnection("Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=$name;Integrated Security=True;Connect Timeout=60")
        $database.Open()
        try {
            $script = [System.IO.File]::ReadAllText((Join-Path $script:ProjectDir 'Database\CreateSchema.sql'))
            foreach ($batch in [regex]::Split($script, '^\s*GO\s*$', [System.Text.RegularExpressions.RegexOptions]::Multiline)) {
                if ($batch.Trim().Length -eq 0) { continue }
                $command = $database.CreateCommand()
                $command.CommandText = $batch
                [void]$command.ExecuteNonQuery()
            }
        } finally {
            $database.Close()
            [System.Data.SqlClient.SqlConnection]::ClearAllPools()
        }

        $detach = $master.CreateCommand()
        $detach.CommandText = "ALTER DATABASE [$name] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; EXEC master.dbo.sp_detach_db @dbname = N'$name'"
        [void]$detach.ExecuteNonQuery()
    } finally {
        $master.Close()
    }
}

function Initialize-FuturePathTest {
    <#
    .SYNOPSIS Loads FuturePath.exe and (unless -NoDatabase) attaches a fresh copy of the database.
    .PARAMETER MdfPath Database to copy (default: FuturePath\FuturePath.mdf, the seeded database built by Database\BuildDatabase.ps1).
    .PARAMETER SchemaOnly Attach an EMPTY database (schema only) instead, for tests that insert their own fixture rows.
    #>
    param(
        [switch]$NoDatabase,
        [switch]$SchemaOnly,
        [string]$MdfPath = (Join-Path $script:ProjectDir 'FuturePath.mdf')
    )
    if (-not (Test-Path $script:ExePath)) {
        throw "Build the project first: $script:ExePath was not found."
    }
    [void][System.Reflection.Assembly]::LoadFrom($script:ExePath)

    $context = [pscustomobject]@{ TempFolder = $null; MdfPath = $null; ConnectionString = $null }
    if ($NoDatabase) {
        return $context
    }

    $temp = Join-Path ([System.IO.Path]::GetTempPath()) ('FuturePathTest_' + [guid]::NewGuid().ToString('N').Substring(0, 8))
    New-Item -ItemType Directory -Path $temp | Out-Null
    if ($SchemaOnly) {
        New-SchemaOnlyDatabase $temp
    } else {
        $ldfSource = [System.IO.Path]::Combine([System.IO.Path]::GetDirectoryName($MdfPath), [System.IO.Path]::GetFileNameWithoutExtension($MdfPath) + '_log.ldf')
        Copy-Item $MdfPath (Join-Path $temp 'FuturePath.mdf')
        if (Test-Path $ldfSource) {
            Copy-Item $ldfSource (Join-Path $temp 'FuturePath_log.ldf')
        }
    }

    $context.TempFolder = $temp
    $context.MdfPath = Join-Path $temp 'FuturePath.mdf'
    $context.ConnectionString = "Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=$($context.MdfPath);Integrated Security=True;Connect Timeout=60"
    [FuturePath.DatabaseLayer.DB]::ConnectionString = $context.ConnectionString
    return $context
}

function Set-TestSettings {
    <#
    .SYNOPSIS Gives SystemSettings fixed parameter values (engine-only tests without a database).
    .EXAMPLE Set-TestSettings @{ WEIGHT_SUBJECT = '0.30'; WEIGHT_MARK = '0.30'; ... }
    #>
    param([hashtable]$Values)
    $dictionary = New-Object 'System.Collections.Generic.Dictionary[string,string]'
    foreach ($key in $Values.Keys) { $dictionary[[string]$key] = [string]$Values[$key] }
    [FuturePath.ApplicationLayer.SystemSettings]::Load($dictionary)
}

function Get-ProvisionalSettings {
    <# .SYNOPSIS The provisional scoring parameters (decisions.md D14), as a hashtable for Set-TestSettings. #>
    return @{
        MIN_SUBJECT_COUNT   = '7'
        WEIGHT_SUBJECT      = '0.30'
        WEIGHT_MARK         = '0.30'
        WEIGHT_INTEREST     = '0.30'
        WEIGHT_FACULTY      = '0.10'
        THRESHOLD_STRONG    = '75'
        INTEREST_STRONG_MIN = '60'
        MARK_TOLERANCE      = '2'
        NSC_LEVEL_1 = '0'; NSC_LEVEL_2 = '30'; NSC_LEVEL_3 = '40'; NSC_LEVEL_4 = '50'
        NSC_LEVEL_5 = '60'; NSC_LEVEL_6 = '70'; NSC_LEVEL_7 = '80'
    }
}

function Invoke-TestQuery {
    <#
    .SYNOPSIS Runs SQL against the test copy and returns a DataTable (for checking stored rows).
    .EXAMPLE (Invoke-TestQuery $ctx 'SELECT COUNT(*) AS N FROM ExportReport').Rows[0].N
    #>
    param($Context, [string]$Sql)
    $connection = New-Object System.Data.SqlClient.SqlConnection($Context.ConnectionString)
    $adapter = New-Object System.Data.SqlClient.SqlDataAdapter($Sql, $connection)
    $table = New-Object System.Data.DataTable
    try { [void]$adapter.Fill($table) } finally { $connection.Close() }
    return ,$table
}

function Assert-Equal {
    param($Expected, $Actual, [string]$Label)
    if ("$Expected" -eq "$Actual") {
        $script:TestsPassed++
        Write-Host ("PASS  " + $Label) -ForegroundColor Green
    } else {
        $script:TestsFailed++
        $message = "FAIL  $Label  (expected '$Expected', got '$Actual')"
        $script:Failures.Add($message)
        Write-Host $message -ForegroundColor Red
    }
}

function Assert-True {
    param([bool]$Condition, [string]$Label)
    Assert-Equal $true $Condition $Label
}

function Assert-Throws {
    <#
    .SYNOPSIS Passes if the script block throws an exception of the given type (inner exceptions included).
    .EXAMPLE Assert-Throws { $c.SaveFaculty(2, $null, 'X', $true) } 'UnauthorizedAccessException' 'Advisor cannot add a faculty'
    #>
    param([scriptblock]$Block, [string]$ExceptionType, [string]$Label)
    try {
        & $Block | Out-Null
        Assert-Equal $ExceptionType 'no exception' $Label
    } catch {
        $exception = $_.Exception
        $names = @()
        while ($exception -ne $null) { $names += $exception.GetType().Name; $exception = $exception.InnerException }
        if ($names -contains $ExceptionType) {
            Assert-Equal $ExceptionType $ExceptionType $Label
        } else {
            Assert-Equal $ExceptionType ($names -join ' > ') $Label
        }
    }
}

function New-TypedCollection {
    <# .SYNOPSIS Creates System.Collections.ObjectModel.Collection[T], e.g. New-TypedCollection 'FuturePath.ApplicationLayer.LearnerSubject'. #>
    param([string]$TypeName)
    return ,(New-Object ("System.Collections.ObjectModel.Collection[$TypeName]"))
}

function Complete-FuturePathTest {
    <# .SYNOPSIS Detaches and deletes the database copy, prints the summary, and sets the exit code. #>
    param($Context)
    if ($Context -ne $null -and $Context.TempFolder) {
        [System.Data.SqlClient.SqlConnection]::ClearAllPools()
        $master = New-Object System.Data.SqlClient.SqlConnection('Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True')
        try {
            $master.Open()
            $find = $master.CreateCommand()
            $find.CommandText = 'SELECT DB_NAME(database_id) FROM sys.master_files WHERE physical_name = @path'
            [void]$find.Parameters.AddWithValue('@path', $Context.MdfPath)
            $name = $find.ExecuteScalar()
            if ($name -ne $null -and $name -ne [DBNull]::Value) {
                $quoted = '[' + ([string]$name).Replace(']', ']]') + ']'
                $detach = $master.CreateCommand()
                $detach.CommandText = "ALTER DATABASE $quoted SET SINGLE_USER WITH ROLLBACK IMMEDIATE; EXEC master.dbo.sp_detach_db @dbname = @name"
                [void]$detach.Parameters.AddWithValue('@name', [string]$name)
                [void]$detach.ExecuteNonQuery()
            }
        } catch {
            Write-Host ("(cleanup) could not detach the test database: " + $_.Exception.Message) -ForegroundColor Yellow
        } finally {
            $master.Close()
        }
        Remove-Item $Context.TempFolder -Recurse -Force -ErrorAction SilentlyContinue
    }

    Write-Host ''
    Write-Host ("{0} passed, {1} failed" -f $script:TestsPassed, $script:TestsFailed) -ForegroundColor $(if ($script:TestsFailed -eq 0) { 'Green' } else { 'Red' })
    foreach ($failure in $script:Failures) { Write-Host $failure -ForegroundColor Red }
    if ($script:TestsFailed -gt 0) { exit 1 }
}
