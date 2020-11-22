// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading.Tasks;

namespace EdFi.Ods.AdminApp.Management.Settings
{
    public interface ICloudOdsSettingsService
    {
        Task<CloudOdsApiWebsiteSettings> GetSettings(string instanceName);

        Task UpdateSettings(string instanceName, CloudOdsApiWebsiteSettings newSettings);
    }

    public class CloudOdsSettingsService : ICloudOdsSettingsService
    {
        private readonly IGetCloudOdsApiWebsiteSettingsQuery _getCloudOdsApiWebsiteSettingsQuery;
        private readonly IGetCloudOdsInstanceQuery _getCloudOdsInstanceQuery;
        private readonly IUpdateCloudOdsApiWebsiteSettingsCommand _updateCloudOdsApiWebsiteSettingsCommand;

        public CloudOdsSettingsService(IGetCloudOdsInstanceQuery getCloudOdsInstanceQuery,
            IGetCloudOdsApiWebsiteSettingsQuery getCloudOdsApiWebsiteSettingsQuery,
            IUpdateCloudOdsApiWebsiteSettingsCommand updateCloudOdsApiWebsiteSettingsCommand)
        {
            _getCloudOdsInstanceQuery = getCloudOdsInstanceQuery;
            _getCloudOdsApiWebsiteSettingsQuery = getCloudOdsApiWebsiteSettingsQuery;
            _updateCloudOdsApiWebsiteSettingsCommand = updateCloudOdsApiWebsiteSettingsCommand;
        }

        public async Task<CloudOdsApiWebsiteSettings> GetSettings(string instanceName)
        {
            var instance = await GetCloudOdsInstance(instanceName);
            var context = new CloudOdsApiOperationContext(instance);

            return await _getCloudOdsApiWebsiteSettingsQuery.Execute(context);
        }

        public async Task UpdateSettings(string instanceName, CloudOdsApiWebsiteSettings newSettings)
        {
            var instance = await GetCloudOdsInstance(instanceName);
            var context = new CloudOdsApiOperationContext(instance);

            await _updateCloudOdsApiWebsiteSettingsCommand.Execute(context, newSettings);
        }

        private async Task<CloudOdsInstance> GetCloudOdsInstance(string cloudOdsInstanceName)
        {
            var defaultInstance = await _getCloudOdsInstanceQuery.Execute(cloudOdsInstanceName);
            return defaultInstance;
        }
    }
}
