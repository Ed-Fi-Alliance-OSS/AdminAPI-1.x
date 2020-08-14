// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using Microsoft.Azure.Management.WebSites.Models;

namespace EdFi.Ods.AdminApp.Management.Azure
{
    public static class AzureModelExtensions
    {
        public static bool HasTag(this Site site, string tagName)
        {
            return site.Tags != null && site.Tags.ContainsKey(tagName);
        }

        public static bool HasTagWithValue(this Site site, string tagName, string tagValue)
        {
            return site.HasTag(tagName) &&
                   site.Tags[tagName].Equals(tagValue, StringComparison.InvariantCultureIgnoreCase);
        }

        public static string GetTagValue(this Site site, string tagName)
        {
            return site.HasTag(tagName) ? site.Tags[tagName] : null;
        }

        public static bool MatchesOperationContext(this Site site, ICloudOdsOperationContext context)
        {
            return (context.TargetEnvironment == null || site.IsInEnvironment(context.TargetEnvironment)) &&
                   (context.TargetRole == null || site.IsInRole(context.TargetRole));
        }

        public static bool IsInRole(this Site site, CloudOdsRole role)
        {
            return site.HasTagWithValue(CloudOdsTags.Role, role.DisplayName);
        }

        public static bool IsInEnvironment(this Site site, CloudOdsEnvironment environment)
        {
            return site.HasTagWithValue(CloudOdsTags.Environment, environment.DisplayName);
        }

        public static bool IsProductionApi(this Site site)
        {
            return site.IsInRole(CloudOdsRole.Api) && site.IsInEnvironment(CloudOdsEnvironment.Production);
        }

        public static bool IsAdminApp(this Site site)
        {
            return site.IsInRole(CloudOdsRole.AdminApp);
        }

        public static bool IsSwaggerWebsite(this Site site)
        {
            return site.IsInRole(CloudOdsRole.Swagger);
        }

        public static CloudOdsWebsite ToCloudOdsWebsite(this Site site)
        {
            return new CloudOdsWebsite
            {
                Environment = CloudOdsEnvironment.Parse(site.GetTagValue(CloudOdsTags.Environment)),
                Role = CloudOdsRole.Parse(site.GetTagValue(CloudOdsTags.Role)),
                SystemId = site.Id,
                SystemName = site.Name,
                Url = site.DefaultHostName
            };
        }
    }
}
