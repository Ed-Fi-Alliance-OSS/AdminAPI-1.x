// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Helpers;
using Microsoft.Extensions.Options;

namespace EdFi.Ods.AdminApp.Management.OnPrem
{
    public class GetOnPremOdsInstanceQuery : IGetCloudOdsInstanceQuery
    {
        private readonly AppSettings _appSettings;

        public GetOnPremOdsInstanceQuery(IOptions<AppSettings> appSettingsAccessor)
        {
            _appSettings = appSettingsAccessor.Value;
        }

        public Task<CloudOdsInstance> Execute(string instanceName)
        {
            return Task.FromResult(new CloudOdsInstance
            {
                FriendlyName = instanceName,
                Version = _appSettings.AwsCurrentVersion //leaving "AWS" reference for config file compatibility with AWS distribution
            });
        }
    }
}