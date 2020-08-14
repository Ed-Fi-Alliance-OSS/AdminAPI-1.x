# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

function Get-MsBuildVersionNumber {
    
    $output = Get-Command -Name msbuild.exe -ErrorAction SilentlyContinue

    if ($output) {
        # We only support Visual Studio 2017 or newer
        if (($output.Version.Major -as [int]) -ge 15) {
            return $output.Version.ToString()
        }
    }

    return $null
}

function Initialize-MsBuild {   
    param (
        # Optional path to msbuild.exe
        [string]
        $MsbuildFolder
    )

    $testForMsBuild = Get-MsBuildVersionNumber
    if ($testForMsBuild) {
        Write-Host "Found msbuild.exe version $testForMsBuild" -ForegroundColor DarkGray
        return
    }

    if ([System.String]::IsNullOrEmpty($MsbuildFolder)) {
        $msbuild = $null

        $pfx86 = [System.Environment]::GetEnvironmentVariable("ProgramFiles(x86)")
        $vswhere = $pfx86 + "\Microsoft Visual Studio\Installer\vswhere.exe"
        if (Test-Path $vswhere) {
            $arguments = @(
                "-products", "*",
                "-latest",
                "-find", "MSBuild\**\Bin\MSBuild.exe"
            )
            $msbuild = &$vswhere @arguments | Select-Object -first 1
        } else {
            throw "Failed to find 'vswhere'; cannot lookup msbuild path."
        }

        $MsbuildFolder = Split-Path -Path $msbuild

        if ([System.String]::IsNullOrEmpty($MsbuildFolder) -or !(Test-Path $MsbuildFolder)) {
            throw @"
Failed to find msbuild using 'vswhere'. You may need to update your Visual
Studio Installer to 2.3.2217.1010 or higher. Please visit 
https://visualstudio.microsoft.com/ to do so.
"@
        }
    }

    Write-Host "Adding $MsbuildFolder to PATH."
    $env:PATH = "$MsbuildFolder;$env:PATH"
    
    $testForMsBuild = Get-MsBuildVersionNumber
    Write-Host "Found msbuild.exe version $testForMsBuild" -ForegroundColor DarkGray
}

function Get-Copyright {
    param (
        [string]
        $BirthYear,

        [string]
        $Maintainers
    )

    $date = Get-Date
    $year = $date.Year
    $copyright_span = if ($year -eq $BirthYear) { $year } else { "$BirthYear-$year" }
    return "Copyright (c) $copyright_span $Maintainers"
}

function Invoke-RegenerateFile {
    param (
        [string]
        $Path,

        [string]
        $NewContent
    )

    $oldContent = Get-Content -Path $Path

    if ($new_content -ne $oldContent) {
        $relative_path = Resolve-Path -Relative $Path
        Write-Host "Generating $relative_path"
        [System.IO.File]::WriteAllText($Path, $NewContent, [System.Text.Encoding]::UTF8)
    }
}

function Invoke-Execute {
    param (
        [ScriptBlock]
        $Command
    )

    $global:lastexitcode = 0
    & $Command

    if ($lastexitcode -ne 0) {
        throw "Error executing command: $Command"
    }
}

function Invoke-Step {
    param (
        [ScriptBlock]
        $block
    )

    $command = $block.ToString().Trim()

    Write-Host
    Write-Host $command -fore CYAN

    &$block
}

function Invoke-Main {
    param (
        [ScriptBlock]
        $MainBlock
    )

    try {
        &$MainBlock
        Write-Host
        Write-Host "Build Succeeded" -fore GREEN
        exit 0
    } catch [Exception] {
        Write-Host
        Write-Error $_.Exception.Message
        Write-Host
        Write-Error "Build Failed"
        exit 1
    }
}

Export-ModuleMember -Function *