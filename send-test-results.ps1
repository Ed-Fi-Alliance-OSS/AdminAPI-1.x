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
        return -1
    }

    $getVersionURL = "$JiraURL/rest/zapi/latest/util/versionBoard-list?projectId=$ProjectId"

    try {
        $response = Invoke-RestMethod -Uri $getVersionURL -Headers $headers
    } catch {
        Write-Host "Error: $_"
    }

    $unreleasedVersions = $response.unreleasedVersions | where { $_.label -like "*$AdminAppVersion*"}
    if($unreleasedVersions) {
        return $unreleasedVersions.value
    }

    $releasedVersions = $response.releasedVersions | where { $_.label -like "*$AdminAppVersion*"}
    if($releasedVersions) {
        return $releasedVersions.value
    }

    return "-1"

}

function GetCycleId {
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

    try {
        $response = Invoke-RestMethod -Uri $getCycleURL -Headers $headers
    } catch {
        Write-Host "Error: $_"
    }

    # Remove entries that do not bring valuable information
    $response.psobject.properties.remove('recordsCount')
    $response.psobject.properties.remove('-1')
    $result = $response.psobject.properties.value | where { $_.name -eq $ConfigParams.cycleName}

    if($result) {
        $ConfigParams.Add("cycleId", $response.psobject.properties.name[0])
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

    try {
        $response = Invoke-RestMethod -Method 'Post' -Uri $createJobURL -Headers $headers -Body $body -ContentType "application/json"
    } catch {
        Write-Host "Error: $_"
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

    $fileBytes = [System.IO.File]::ReadAllBytes($ResultsFilePath);
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

    try {
        $response = Invoke-RestMethod -Uri $uploadJobUrl -Method Post -Headers $headers -ContentType "multipart/form-data; boundary=`"$boundary`"" -Body $bodyLines
    } catch {
        Write-Host "Error: $_"
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

    try {
        $response = Invoke-RestMethod -Uri $executeJobUrl -Method Post -Headers $headers -ContentType "application/json"
    } catch {
        Write-Host "Error: $_"
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

    try {
        $response = Invoke-RestMethod -Uri $jobStatusUrl -Headers $headers
    } catch {
        Write-Host "Error: $_"
    }

    Write-Host $response.Status
}

$versionId = ObtainAdminAppVersionId -AdminAppVersion $AdminAppVersion
GetCycleId -VersionId $versionId
$jobId = CreateAutomationJob -VersionId $versionId
Write-Host "Created Zephyr run for job: $jobId with version: $versionId and parameters $parameters"
UploadResultsFile -JobId $jobId
ExecuteJob -JobId $jobId
GetJobStatus -JobId $jobId
