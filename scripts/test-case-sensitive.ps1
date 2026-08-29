# 'Elite - The Sharp Kind' - Andy Hawkins 2023-2026.

<#
.SYNOPSIS
    Runs the tests with a case-sensitive temporary directory.

.DESCRIPTION
    CI runs on Linux, where two files whose names differ only in case are two
    files. Windows says they are one, so a test that saves "Broken" and reads
    "BROKEN" passes here and fails there - which is a slow way to find out.

    Windows can mark a single directory case-sensitive, and the tests take
    their working directories from the TEMP variable, so pointing TEMP at such
    a directory reproduces the Linux behaviour without leaving Windows.

    Needs the Windows Subsystem for Linux optional component installed, which
    is what provides the per-directory case-sensitivity flag. No elevation.

.EXAMPLE
    ./scripts/test-case-sensitive.ps1
#>

[CmdletBinding()]
param(
    # The test project or solution to run. Defaults to the whole solution.
    [string] $Target = (Join-Path $PSScriptRoot '..' 'TheSharpKind.slnx')
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$caseSensitiveRoot = Join-Path ([System.IO.Path]::GetTempPath()) "SharpKindCaseSensitive_$([guid]::NewGuid().ToString('N'))"
New-Item -ItemType Directory -Path $caseSensitiveRoot | Out-Null

try {
    # Subdirectories created inside inherit the flag, so the per-test folders
    # are case-sensitive too.
    $result = fsutil.exe file setCaseSensitiveInfo $caseSensitiveRoot enable 2>&1
    if ($LASTEXITCODE -ne 0) {
        throw "Could not mark $caseSensitiveRoot case-sensitive. Is the Windows Subsystem for Linux component installed?`n$result"
    }

    Write-Host "Running tests with TEMP -> $caseSensitiveRoot" -ForegroundColor Cyan

    $originalTemp = $env:TEMP
    $originalTmp = $env:TMP
    try {
        $env:TEMP = $caseSensitiveRoot
        $env:TMP = $caseSensitiveRoot
        dotnet test $Target --nologo
        exit $LASTEXITCODE
    }
    finally {
        $env:TEMP = $originalTemp
        $env:TMP = $originalTmp
    }
}
finally {
    Remove-Item -Recurse -Force $caseSensitiveRoot -ErrorAction SilentlyContinue
}
