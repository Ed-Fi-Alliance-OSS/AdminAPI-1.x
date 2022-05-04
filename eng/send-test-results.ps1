# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

[CmdLetBinding()]
<#
    .SYNOPSIS
        Publish JUnit test result to Zephyr for Jira

    .DESCRIPTION
        Publishes a JUnit test result fila to Zephyr for Jira. If the test case already exists, it will add the execution to the test, if not, it
        will create a new test case.
        If a cycle and folder id is specified, it will add the execution to the existing data, if not, it will create a new cycle or folder.


    .EXAMPLE
        $parameters = @{
            cycleId = 39
            taskName = "Automation Task 1"
            cycleName = "Test Cycle 1"
            folderName = "New Folder"
        }

        .\SendTestResults.ps1 -JiraURL https://tracker-stage.ed-fi.org -PersonalAccessToken ** -ProjectId 1 -ResultsFile "C:/results.xml" -ConfigParams $parameters

        Sends the results to the staging Jira server
    .EXAMPLE
        $parameters = @{
            cycleId = 39
            folderId = 1
            taskName = "Automation Task 1"
            cycleName = "Test Cycle 1"
            folderName = "New Folder"
        }

        .\SendTestResults.ps1 -PersonalAccessToken ** -ProjectId 1 -AdminAppVersion 2.3.0 -ResultsFile "C:/results.xml" -ConfigParams $parameters

        Sends the results to the main Jira server
#>
param(
    # Ed-Fi's Jira URL
    [string]
    $JiraURL = "https://tracker.ed-fi.org",

    # Access Token to upload the results
    [string]
    $PersonalAccessToken,

    # Id of the project in Jira https://support.smartbear.com/zephyr-squad-server/docs/api/how-to/get-ids.html
    [string]
    $ProjectId,

    # Numeric part of the fix version field on Jira
    [string]
    $AdminAppVersion,

    # Full path of an XML file with JUnit format to upload the results
    [string]
    $ResultsFilePath,

    # Configuration parameters. See examples
    [hashtable]
    $ConfigParams = @{},

    [boolean]
    $IncludeDateOnFolder = $true
)

$headers = @{Authorization = "Bearer $PersonalAccessToken"}

function ObtainAdminAppVersionId {
    param (
        [string]
        $AdminAppVersion
    )

    if(-not $AdminAppVersion) {
        throw "Please specify a valid Admin App version defined in Jira."
    }

    $getVersionURL = "$JiraURL/rest/zapi/latest/util/versionBoard-list?projectId=$ProjectId"

    $response = Invoke-RestMethod -Uri $getVersionURL -Headers $headers
    if($response.errorDesc) {
        throw $response.errorDesc
    }

    $unreleasedVersions = $response.unreleasedVersions | where { $_.label -like "*$AdminAppVersion*"}
    if($unreleasedVersions) {
        Write-Host "Unreleased Admin App version found: $unreleasedVersions.value"
        return $unreleasedVersions.value
    }

    $releasedVersions = $response.releasedVersions | where { $_.label -like "*$AdminAppVersion*"}
    if($releasedVersions) {
        Write-Host "Released Admin App version found: $releasedVersions.value"
        return $releasedVersions.value
    }

    throw "Please specify a valid Admin App version defined in Jira."

}

function SetCycleId {
    param (
        [string]
        $VersionId
    )

    # Get only if there's no cycleId specified
    if($ConfigParams.cycleId) {
        return
    }

    if(!$ConfigParams.cycleName) {
      throw "Specify test cycle name to get ID"
    }

    $getCycleURL = "$JiraURL/rest/zapi/latest/cycle?projectId=$ProjectId&versionId=$VersionId"

    $response = Invoke-RestMethod -Uri $getCycleURL -Headers $headers
    if($response.errorDesc) {
        throw $response.errorDesc
    }

    # Remove entries that do not contain valuable information
    $response.psobject.properties.remove('recordsCount')
    $response.psobject.properties.remove('-1')
    $result = $response.psobject.properties.value | where { $_.name -eq $ConfigParams.cycleName}

    if($result) {
        $cycle = $response.psobject.properties.name[0]
        $ConfigParams.Add("cycleId", $cycle)
        Write-Host "Found cycle: $cycle for name: $ConfigParams.cycleName"
    } else {
        Write-Host "Could not find an existing cycle with the given name. Will create a new one"
    }
}

function CreateAutomationJob {
    param (
        [string]
        $VersionId
    )

    $createJobURL = "$JiraURL/rest/zapi/latest/automation/job/create"

    if($ConfigParams.folderName -and $IncludeDateOnFolder) {
        $folder = $ConfigParams.folderName
        $ConfigParams.Remove("folderName")
        $date = Get-Date -DisplayHint Date
        $ConfigParams.Add("folderName", "$folder - $date")
    }

    $ConfigParams.Add("projectId", $ProjectId)
    $ConfigParams.Add("versionId", $VersionId)
    $ConfigParams.Add("automationType", "UPLOAD")
    $ConfigParams.Add("automationTool", "JUnit")

    $body = $ConfigParams | ConvertTo-Json

    $response = Invoke-RestMethod -Method 'Post' -Uri $createJobURL -Headers $headers -Body $body -ContentType "application/json"
    if($response.errorDesc) {
        throw $response.errorDesc
    }

    if($response.status -eq 200) {
        return $response.JOB_ID
    }
}

function UploadResultsFile {
    param (
        [string]
        $JobId
    )

    $uploadJobUrl = "$JiraURL/rest/zapi/latest/automation/upload/$JobId"

    try {
        $fileBytes = [System.IO.File]::ReadAllBytes($ResultsFilePath);
    } catch {
        throw "Results file not found. Verify that file is located in path: $ResultsFilePath"
    }

    $fileEnc = [System.Text.Encoding]::GetEncoding('UTF-8').GetString($fileBytes);
    $boundary = [System.Guid]::NewGuid().ToString();
    $LF = "`r`n";
    $filename = Split-Path $ResultsFilePath -leaf;

    $bodyLines = (
        "--$boundary",
        "Content-Disposition: form-data; name=`"file`"; filename=`"$filename`"",
        "Content-Type: application/octet-stream$LF",
        $fileEnc,
        "--$boundary--$LF"
    ) -join $LF

    $response = Invoke-RestMethod -Uri $uploadJobUrl -Method Post -Headers $headers -ContentType "multipart/form-data; boundary=`"$boundary`"" -Body $bodyLines
    if($response.errorDesc) {
        throw $response.errorDesc
    }

    if($response.status -eq 200) {
        Write-Host $response.message
    }
}

function ExecuteJob {
    param (
        [string]
        $JobId
    )

    $executeJobUrl = "$JiraURL/rest/zapi/latest/automation/job/execute/$JobId"

    $response = Invoke-RestMethod -Uri $executeJobUrl -Method Post -Headers $headers -ContentType "application/json"
    if($response.errorDesc) {
        throw $response.errorDesc
    }

    if($response.status -eq 200) {
        Write-Host $response.message
    }
}

function GetJobStatus {
    param (
        [string]
        $JobId
    )

    $jobStatusUrl = "$JiraURL/rest/zapi/latest/automation/job/status/$JobId"

    $response = Invoke-RestMethod -Uri $jobStatusUrl -Headers $headers
    if($response.errorDesc) {
        throw $response.errorDesc
    }

    Write-Host $response.Status
}

try {
    $versionId = ObtainAdminAppVersionId -AdminAppVersion $AdminAppVersion
    SetCycleId -VersionId $versionId
    $jobId = CreateAutomationJob -VersionId $versionId
    Write-Host "Created Zephyr run for Admin App version: $versionId with job: $jobId"
    UploadResultsFile -JobId $jobId
    ExecuteJob -JobId $jobId
    GetJobStatus -JobId $jobId
} catch {
    Write-Host "Error sending test results: " -NoNewLine -ForegroundColor Red
    Write-Host $_.Exception.Message
    Write-Host "Please check info and try again. See full exception below`n"
    throw $_
}
