// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Configuration;
using EdFi.Ods.AdminApp.Management.Helpers;
using EdFi.Ods.AdminApp.Management.Instances;

namespace EdFi.Ods.AdminApp.Management.Services
{
    public class ConnectionStringService: IConnectionStringService
    {
        public string GetConnectionString(string odsInstanceName, ApiMode apiMode)
        {
            var connectionString = ConfigurationHelper.GetConnectionStrings().ProductionOds;
            if (apiMode.SupportsMultipleInstances)
            {
                connectionString = GetModifiedConnectionString(connectionString, odsInstanceName);
            }

            return connectionString;
        }

        private string GetModifiedConnectionString(string connectionString, string odsInstanceName)
        {
            if(!connectionString.Contains("{0}"))
                throw new InvalidOperationException(
                    "The connection string must contain a placeholder {0} for the multi-instance modes to work.");
            var modifiedConnectionString = connectionString.Replace("{0}", $"Ods_{odsInstanceName.ExtractNumericInstanceSuffix()}");
            return modifiedConnectionString;
        }
    }
}