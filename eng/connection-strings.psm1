# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

#requires -version 5

function Build-PostgresConnectionString {
    param (
        [string]
        $Server,

        [string]
        $DatabaseName,

        [int]
        $Port = 5432,

        [switch]
        $UseIntegratedSecurity,

        [string]
        $Username,

        [string]
        $Password
    )

    $connectionString = "host=$Server;port=$Port;database=$DatabaseName;"

    if ($UseIntegratedSecurity) {
        return $connectionString + "Integrated Security=true"
    }

    return $connectionString + "User Id=$Username;Password=$Password"
}

function Build-SqlServerConnectionString {
    param (
        [string]
        $Server,

        [string]
        $DatabaseName,

        [int]
        $Port = 1433,

        [switch]
        $UseIntegratedSecurity,

        [string]
        $Username,

        [string]
        $Password
    )

    # Only use port if non-standard, so that developers don't need
    # to manually enable TCP/IP on their local SQL Server instances
    if (1433 -ne $Port) {
        $connectionString = "Data Source=$Server,$Port;Initial Catalog=$DatabaseName;"
    }
    else {
        $connectionString = "Data Source=$Server;Initial Catalog=$DatabaseName;"
    }

    if ($UseIntegratedSecurity) {
        return $connectionString + "Integrated Security=true;Encrypt=False;"
    }

    return $connectionString + "User Id=$Username;Password=$Password;Encrypt=False;"
}

function New-ConnectionString {
    <#
        .SYNOPSIS
            Builds a database connection string for either SQL Server or PostgreSQL.

        .EXAMPLE
            PS > $parameters = @{
                ForPostgreSQL = $true
                Server = "myPgServer"
                DatabaseName = "EdFi_ODS"
                Port = 2345,
                UseIntegratedSecurity = $true
            }

            PS > New-ConnectionString @parameters

            Creates a connection string for connecting to PostgreSQL on a non-standard port,
            using integrated security instead of username and password.
            
        .EXAMPLE
            PS > $parameters = @{
                Server = "mySqlServer"
                DatabaseName = "EdFi_ODS"
                Username = "myuser"
                Password = "mypassword"
            }

            PS > New-ConnectionString @parameters

            Creates a connection string for connecting to SQL Server on a standard port,
            using user name and password.
    #>
    param (
        # True if connection string is for a PostgreSQL database. Otherwise for SQL Server.
        [switch]
        $ForPostgreSQL,

        # Server or host name.
        [string]
        [Parameter(Mandatory=$true)]
        $Server,

        # Database name.
        [string]
        [Parameter(Mandatory=$true)]
        $DatabaseName,

        # Port number, optional.
        [int]
        $Port,

        # Indicates that integrated security should be used instead of username and password.
        [switch]
        $UseIntegratedSecurity,

        # Database username if not using integrated security.
        [string]
        $Username,

        # Database password if not using integrated security.
        [string]
        $Password
    )

    if (-not $Port){
        if ($ForPostgreSQL) {
            $Port = 5432
        }
        else {
            $Port = 1433
        }
    }

    $params = @{
        Server = $Server
        databaseName = $DatabaseName
        Port = $Port
        UseIntegratedSecurity = $UseIntegratedSecurity
        Username = $Username
        password = $password
    }

    if ($ForPostgreSQL) {
        return Build-PostgresConnectionString @params
    }

    return Build-SqlServerConnectionString @params
}

function Get-MaskedConnectionString {
    param (
        [string]
        $ConnectionString
    )

    if ($ConnectionString.ToLower().Contains("password")) {
        return $ConnectionString -Replace "Password\=[^\;]+", "Password=*********"
    }

    return $ConnectionString
}

$functions = @(
    "New-ConnectionString",
    "Get-MaskedConnectionString"
)

Export-ModuleMember -Function $functions
