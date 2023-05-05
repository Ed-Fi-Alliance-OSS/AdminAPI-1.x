// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Data;
using System.IO.Compression;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using NuGet.Common;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace EdFi.Ods.Admin.Api.DBTests
{
    public class SecurityTestDatabaseSetup
    {
        private static SqlConnectionStringBuilder ConnectionStringBuilder
        {
            get
            {
                return new SqlConnectionStringBuilder() { ConnectionString = Testing.SecurityV53ConnectionString };
            }
        }

        public void EnsureSecurityDatabase(string downloadPath,
            string version = "5.3.1146",
            string nugetSource = "https://pkgs.dev.azure.com/ed-fi-alliance/Ed-Fi-Alliance-OSS/_packaging/EdFi/nuget/v3/index.json",
            string packageName = "EdFi.Suite3.RestApi.Databases")
        {
            if (!CheckSecurityDbExists())
            {
                var task = Task.Run(async () => await DownloadDbPackage(packageName, version, nugetSource, downloadPath));
                var scriptsPath = task.GetAwaiter().GetResult();
                ExecuteSqlScripts(scriptsPath);
            }
        }

        private async Task<string> DownloadDbPackage(string packageName, string version, string nugetSource, string downloadPath)
        {
            var logger = NullLogger.Instance;
            var cancellationToken = CancellationToken.None;

            var cache = new SourceCacheContext();
            var repository = Repository.Factory.GetCoreV3(nugetSource);
            var resource = await repository.GetResourceAsync<FindPackageByIdResource>();

            var packageVersion = new NuGetVersion(version);
            var packagePath = Path.Combine(downloadPath, $"{packageName}.{packageVersion}.nupkg");
            if (!File.Exists(packagePath))
            {
                using var packageStream = File.OpenWrite(packagePath);

                await resource.CopyNupkgToStreamAsync(
                    packageName,
                    packageVersion,
                    packageStream,
                    cache,
                    logger,
                    cancellationToken);
            }

            var packageContentDir = Path.Combine(downloadPath, $"{packageName}.{packageVersion}");
            if (!Directory.Exists(packageContentDir))
            {
                var result = Path.ChangeExtension(packagePath, ".zip");
                File.Move(packagePath, result);

                if (!Directory.Exists(packageContentDir))
                {
                    Directory.CreateDirectory(packageContentDir);
                }
                ZipFile.ExtractToDirectory(result, packageContentDir);
            }
            return packageContentDir;
        }

        private static SqlConnectionStringBuilder MasterConnection
        {
            get
            {
                var csb = new SqlConnectionStringBuilder
                {
                    ConnectionString = ConnectionStringBuilder.ConnectionString, InitialCatalog = "master"
                };
                return csb;
            }
        }

        private void ExecuteSqlScripts(string scriptsPath)
        {
            using (var connection = new SqlConnection(MasterConnection.ConnectionString))
            {
                connection.Open();
                try
                {
                    var sql = @"declare @database varchar(max) = quotename(@databaseName)
                                EXEC('CREATE DATABASE ' + @database + '')";
                    using var command = new SqlCommand(sql, connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@databaseName", ConnectionStringBuilder.InitialCatalog);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            
            using var conn = new SqlConnection(ConnectionStringBuilder.ConnectionString);
            var server = new Server(new ServerConnection(conn));
            var scriptFilesPath = Path.Combine(scriptsPath, @"Ed-Fi-ODS\Artifacts\MsSql\Structure\Security");

            foreach (var file in Directory.EnumerateFiles(scriptFilesPath, "*.sql"))
            {
                var script = File.ReadAllText(file);
                server.ConnectionContext.ExecuteNonQuery(script);
            }
        }

        private bool CheckSecurityDbExists()
        {
            try
            {
                var sqlCreateDBQuery = $"SELECT database_id FROM sys.databases WHERE Name = '{ConnectionStringBuilder.InitialCatalog}'";
                using var connection = new SqlConnection(MasterConnection.ConnectionString);
                using var sqlCmd = new SqlCommand(sqlCreateDBQuery, connection);
                connection.Open();
                var result = sqlCmd.ExecuteScalar();
                if (result != null)
                {
                    _ = int.TryParse(result.ToString(), out var databaseID);
                    return databaseID > 0;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
