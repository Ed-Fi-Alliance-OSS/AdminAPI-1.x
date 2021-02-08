// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Web.Helpers;

namespace EdFi.Ods.AdminApp.Web.Infrastructure
{
    public class CloudOdsUpdateService
    {
        private readonly ICachedItems _cachedItems;
        private readonly CloudOdsUpdateCheckService _cloudOdsUpdateCheckService;

        public CloudOdsUpdateService(ICachedItems cachedItems, CloudOdsUpdateCheckService cloudOdsUpdateCheckService)
        {
            _cachedItems = cachedItems;
            _cloudOdsUpdateCheckService = cloudOdsUpdateCheckService;
        }

        public async Task<CloudOdsUpdateInfo> GetUpdateInfo()
        {
            var instance = await _cachedItems.GetDefaultCloudOdsInstance();

            var cloudOdsUpdateInfo = new CloudOdsUpdateInfo
            {
                Instance = instance,
                LatestPublishedVersion = _cachedItems.LatestPublishedOdsVersion?.ToVersion(),
                CurrentInstanceVersion = instance?.Version?.ToVersion(),
            };

            cloudOdsUpdateInfo.VersionInformationIsValid =
                _cloudOdsUpdateCheckService.VersionInformationIsValid(cloudOdsUpdateInfo);

            cloudOdsUpdateInfo.UpdateIsCompatible = _cloudOdsUpdateCheckService.UpdateIsCompatible(cloudOdsUpdateInfo);

            return cloudOdsUpdateInfo;
        }
    }
}
