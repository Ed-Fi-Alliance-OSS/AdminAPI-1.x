# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

#requires -version 5
$ErrorActionPreference = "Stop" 

## CONSTANTS
$ADMINAPI_ENDPOINT_VENDOR = "/v1/vendors"
$ADMINAPI_ENDPOINT_APPLICATION = "/v1/applications"
$ADMINAPI_ENDPOINT_OAUTH_URL = "/connect/token"

function Bulk-EdFiOdsApplications
{
    <#
    .SYNOPSIS
        EdFi Bulk Application generator.

    .DESCRIPTION
        This PowerShell script creates multiple vendors and applications upon execution and stores the generated keys and secrets in a file.
        Please remember to securely store keys and secrets. The script utilizes a CSV file as input to specify the vendors and applications to be created.
        The script prevents duplicate creation of vendors and applications by skipping existing combinations.
        This Script only works for AdminAPI-1.x

    .EXAMPLE
        PS c:/> $parameters = @{
                                CSVFilePath = "District details.csv"
                                BaseApiUrl = 'https://localhost/AdminApi'
                                namespacePrefixes = "uri://ed-fi.org/"
                                Key = "Your_Admin_Api_User"
                                Secret = "Your_Admin_Api_Password"
                                ClaimsetName = "District Hosted SIS Vendor"
                                LogRootPath = "Logs"
                            }
        PS c:/> Bulk-EdFiOdsApplications @parameters

        Set your apropiate values .
    #>
    [CmdletBinding()]
    param (
        [hashtable]
        [Parameter(Mandatory = $true)]
        $Config
    )
    
  Write-Host "BaseApiUrl $($config.BaseApiUrl)"
    # Execute\Init task
    Init -Config $config   
}

Function Init
{
  [CmdletBinding()]
    param (
        [hashtable]
        [Parameter(Mandatory = $true)]
        $Config
    )
	$error.clear()
    # Enable Logging
    New-Item -ItemType Directory -Force -Path $Config.logRootPath
    $date = Get-Date -Format "yyyy-MM-dd-H-m-s"
    $logPath = Join-Path -Path $Config.logRootPath -ChildPath  "Applications_Log_$date.log"
    Start-Transcript -Path $logPath    

    Write-Host "*** Initializing Ed-Fi >  CSV Processing. ***" 
    Write-Host  $Config.BaseApiUrl
    Get-Token $Config
    Write-Host "Creating a Json Object and Post Applications"
    Create-Applications $Config 
    Stop-Transcript
}
<#
.SYNOPSIS
   function that generates a token
#>
Function Get-Token
{
    [CmdletBinding()]
    param (
        [hashtable]
        $Config
    )
    # extract the requiered parameters from the config file.
    $baseApiUrl = $Config.BaseApiUrl
    $oAuthUrl = "$baseApiUrl" + $ADMINAPI_ENDPOINT_OAUTH_URL
    Write-Host " *** Getting Token ******** "
    $formData = @{
        client_id = $Config.Key
        client_secret = $Config.Secret
        grant_type = 'client_credentials'
        scope = 'edfi_admin_api/full_access'
    }
    Write-Host "token url $oAuthUrl"
    $oAuthResponse = Invoke-RestMethod -Uri "$oAuthUrl" -Method Post -Body $formData
    $Config.Token = $OAuthResponse.access_token
}

Function Set-Headers 
{
    [CmdletBinding()]
    param (
        [hashtable]
        [Parameter(Mandatory = $true)]
        $Config
    )
    $token =  $Config.Token
    $headers = @{
		"Accept" = "application/json"
        "Authorization" = "Bearer $token"
        "Content-Type" = "application/json"
    }
    return  $headers
}
<#
.SYNOPSIS
   function that extract the Vendor Id from the Header
#>
Function Extract-VendorId($Result) 
{   
    try {
        $location = $Result.Headers.Location | ConvertTo-Json 
        return ($location.split("/")[-1]) -replace '\"', ''
    	}
		catch {
			Write-Host " An error occurred: $_"
            Write-Host $_
		} 
}

Function Get-Resource
{
    [CmdletBinding()]
    Param(
        [hashtable]
        [Parameter(Mandatory = $true)]
        $Config,
        [string]
        [Parameter(Mandatory = $true)]
        $EndPoint
    )

    $token = $Config.Token
    $headers = Set-Headers $Config
    $baseApiUrl = $Config.BaseApiUrl
    $uri = "$baseApiUrl" + "$EndPoint" + "?offset=0&limit=1000"
    try {
        $result = Invoke-WebRequest -Method Get -Uri $uri -Headers $headers 
        return $result
    	}
		catch {
			Write-Host "An error occurred: $uri"
            Write-Host $_
		} 
}

<#
.SYNOPSIS
   function that allows us to post a payload
#>
Function Post-Data
{
    [CmdletBinding()]
    Param(
        [hashtable]
        [Parameter(Mandatory = $true)]
        $Config,
        [PSCustomObject]
        [Parameter(Mandatory = $true)]
        $PostPayloadJSON,
        [string]
        [Parameter(Mandatory = $true)]
        $EndPoint
    )
    
    $headers = Set-Headers $Config
    $baseApiUrl = $Config.BaseApiUrl
    $uri = "$baseApiUrl" + "$EndPoint"
    Write-Host "Uri  ***********$uri"	
	$requestSchema = ConvertTo-Json $PostPayloadJSON
    try {
        $result = Invoke-WebRequest  -Uri  $uri -Method Post -Headers $headers -Body $requestSchema
        return $result
    }
    catch {
        Write-Host "An error occurred: $uri"
        Write-Host "$requestSchema"
        Write-Host $_
    }
}
<#
.SYNOPSIS
   function that allows us to create and Post payloads for vendors and  applications
#>
Function Create-Applications
{
    [CmdletBinding()]
    Param(
        [hashtable]
        [Parameter(Mandatory = $true)]
        $Config
    )
      Write-Host "Working file '"  $Config.CSVFilePath "'"
      $vendorsFromOds = @()
      $newApplications = @()
      $vendorId = 0
      $applicationsCount = 0
      $csv = import-csv -path  $Config.CSVFilePath
      $Config.CSVResultFile = ($Config.CSVFilePath -replace '.csv', "$(Get-Date -Format "yyyy-MM-dd-H-m") _key_Secret.csv")
      $namesPace = $Config.NamesPace

      $vendorsFromOds = ((Get-Resource  $Config $ADMINAPI_ENDPOINT_VENDOR).Content | ConvertFrom-Json)."result"
      $applicationsFromOds = ((Get-Resource  $Config $ADMINAPI_ENDPOINT_APPLICATION).Content | ConvertFrom-Json)."result"
       
        Import-Csv $Config.CSVFilePath  -delimiter ","   -Header EducationOrganiztionId,VendorName,ApplicationName,ContactName,ContactEmail,key,secret|Select-Object -Skip 1|
            ForEach-Object {
                $vendor = $_.Vendorname
                $applicationName = $_.ApplicationName
                $vendorId = ($vendorsFromOds | Where-object {$_.company -eq  $vendor }).vendorId  | Select-Object -First 1 
                if ($null -eq $vendorId)
                {                  
                    $objVendors = [PSCustomObject]@{                           
                                            company = [System.Security.SecurityElement]::Escape($_.Vendorname)
                                            namespacePrefixes = [System.Security.SecurityElement]::Escape($Config.namespacePrefixes)
                                            contactName = [System.Security.SecurityElement]::Escape($_.contactName)
                                            contactEmailAddress = [System.Security.SecurityElement]::Escape($_.ContactEmail)                        
                    }
                    $vendorResult = Post-Data  $Config $objVendors $ADMINAPI_ENDPOINT_VENDOR
                    $vendorId = Extract-VendorId $vendorResult
                    Write-Host "A new Vendor was created   $vendorId" 
                }
                $applicationId = ($applicationsFromOds | Where-object {$_.applicationName -eq  $applicationName  -and $_.vendorId -eq  $vendorId }).applicationId  | Select-Object -First 1 
                if ($null -eq $applicationId)
                {                                     
                    $objApplications = [PSCustomObject]@{
                                            applicationName = [System.Security.SecurityElement]::Escape($applicationName)
                                            vendorId = $vendorId
                                            claimSetName = [System.Security.SecurityElement]::Escape($Config.ClaimsetName)
                                            educationOrganizationIds = @([System.Security.SecurityElement]::Escape($_.EducationOrganiztionId))
                                        }
                    
                    $application = (Post-Data  $Config $ObjApplications $ADMINAPI_ENDPOINT_APPLICATION).Content | ConvertFrom-Json                    
                    $applicationResult = $Application."result"
                    Write-Host "The application $applicationName was created"
                    $newApplications += [PSCustomObject]@{
                                            ApplicationName = $_.ApplicationName
                                            key = $applicationResult.key
                                            secret = $applicationResult.secret
                                        }
                    $applicationsCount ++
                }else
                 {                   
                    Write-Host "The application already exist. Skipped Vendor : $vendor, Application : $applicationName"
                }
            }
    if ($applicationsCount -gt 0)
    {                                     
        $newApplications | Export-Csv -Path $Config.CSVResultFile  -NoTypeInformation                
        Write-Host "*********************************************************************************************************"
        Write-Host "*** Find the keys and secrets in the '$($Config.CSVResultFile)'  file                                    "  
        Write-Host "*********************************************************************************************************"
    }else
    {                   
        Write-Host "*********************************************************************************************************"
        Write-Host "*** No applications were created                                                                       "  
        Write-Host "*********************************************************************************************************"
    }          
       
}