// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using EdFi.Ods.AdminApp.Management.Helpers;
using EdFi.Ods.AdminApp.Management.Instances;
using Microsoft.Extensions.Options;
using ApiMode = EdFi.Ods.AdminApp.Management.Instances.ApiMode;

namespace EdFi.Ods.AdminApp.Management.Services
{
    public class ConnectionStringService: IConnectionStringService
    {
        private readonly ConnectionStrings _connectionStrings;

        private readonly IConnectionStringBuilderAdapterFactory _connectionStringBuilderAdapterFactory;

        public ConnectionStringService(IOptions<ConnectionStrings> connectionStrings,
            IConnectionStringBuilderAdapterFactory connectionStringBuilderAdapterFactory)
        {
            _connectionStrings = connectionStrings.Value;
            _connectionStringBuilderAdapterFactory = connectionStringBuilderAdapterFactory;
        }

        public string GetConnectionString(string odsInstanceName, ApiMode apiMode)
        {
            var connectionStringBuilder = _connectionStringBuilderAdapterFactory.Get();
            connectionStringBuilder.ConnectionString = _connectionStrings.ProductionOds;

            if (apiMode.SupportsMultipleInstances && !IsTemplate(connectionStringBuilder.DatabaseName))
            {
                throw new InvalidOperationException(
                             "The database name on the connection string must contain a placeholder {0} for the multi-instance modes to work.");
            }

            connectionStringBuilder.DatabaseName = GetUpdatedName(connectionStringBuilder.DatabaseName);
            connectionStringBuilder.ServerName = GetUpdatedName(connectionStringBuilder.ServerName);

            return connectionStringBuilder.ConnectionString;

            string GetUpdatedName(string input)
            {
                if(!IsTemplate(input))                
                    return input;
                
                var updatedValue = input;
                if (apiMode.SupportsMultipleInstances)
                {
                    updatedValue = string.Format(input, $"Ods_{odsInstanceName.ExtractNumericInstanceSuffix()}");
                }
                else if (apiMode == ApiMode.SharedInstance)
                {
                    updatedValue = string.Format(input, "Ods");
                }

                return updatedValue;
            }
        }

        private static bool IsTemplate(string input)
            => input.Contains("{0}");
    }  
}
