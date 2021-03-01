// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using EdFi.Ods.AdminApp.Management.Helpers;
using EdFi.Ods.AdminApp.Management.Instances;
using Microsoft.Extensions.Options;

namespace EdFi.Ods.AdminApp.Management.Services
{
    public class ConnectionStringService: IConnectionStringService
    {
        private readonly ConnectionStrings _connectionStrings;

        public ConnectionStringService(IOptions<ConnectionStrings> connectionStrings)
        {
            _connectionStrings = connectionStrings.Value;
        }

        public string GetConnectionString(string odsInstanceName, ApiMode apiMode)
        {
            var connectionString = _connectionStrings.ProductionOds;

            if (apiMode.SupportsMultipleInstances)
            {
                if (IsTemplate(connectionString))
                {
                    connectionString = GetModifiedConnectionString(
                        connectionString, $"Ods_{odsInstanceName.ExtractNumericInstanceSuffix()}");
                }
                else
                {
                    throw new InvalidOperationException(
                        "The connection string must contain a placeholder {0} for the multi-instance modes to work.");
                }
            }
            else if (apiMode == ApiMode.SharedInstance)
            {
                if (IsTemplate(connectionString))
                {
                    connectionString = GetModifiedConnectionString(connectionString, "Ods");
                }
            }

            return connectionString;
        }

        private static bool IsTemplate(string connectionString)
            => connectionString.Contains("{0}");

        private static string GetModifiedConnectionString(string connectionString, string replacement)
            => connectionString.Replace("{0}", replacement);
    }
}
