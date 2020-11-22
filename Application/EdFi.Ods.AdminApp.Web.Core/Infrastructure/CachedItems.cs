// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management;

namespace EdFi.Ods.AdminApp.Web.Infrastructure
{
    public interface ICachedItems
    {
        string LatestPublishedOdsVersion { get; }

        Task<CloudOdsInstance> GetDefaultCloudOdsInstance();
    }

    public class CachedItems : ICachedItems
    {
        private readonly IGetCloudOdsInstanceQuery _getCloudOdsInstanceQuery;
        private readonly IGetLatestPublishedOdsVersionQuery _getLatestPublishedOdsVersionQuery;

        public CachedItems(IGetLatestPublishedOdsVersionQuery getLatestPublishedOdsVersionQuery,
            IGetCloudOdsInstanceQuery getCloudOdsInstanceQuery)
        {
            _getLatestPublishedOdsVersionQuery = getLatestPublishedOdsVersionQuery;
            _getCloudOdsInstanceQuery = getCloudOdsInstanceQuery;
        }

        public string LatestPublishedOdsVersion
        {
            get
            {
                return InMemoryCache.Instance.GetOrSet(
                    "LatestPublishedOdsVersion",
                    () => _getLatestPublishedOdsVersionQuery.Execute());
            }
        }

        public async Task<CloudOdsInstance> GetDefaultCloudOdsInstance()
        {
            return await InMemoryCache.Instance.GetOrSet(
                "DefaultCloudOdsInstance",
                async () =>
                {
                    var instance = await _getCloudOdsInstanceQuery.Execute(CloudOdsAdminAppSettings.Instance.OdsInstanceName);

                    if (instance == null)
                    {
                        throw new Exception(
                            "Error retrieving Default ODS Instance.  Confirm that the DefaultOdsInstance configuration setting is correct.");
                    }

                    return instance;
                });
        }
    }
}
