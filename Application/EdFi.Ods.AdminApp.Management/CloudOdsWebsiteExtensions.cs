// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace EdFi.Ods.AdminApp.Management
{
    public static class CloudOdsWebsiteExtensions
    {
        public static bool HasTag(this CloudOdsWebsite site, string tagName)
        {
            return site.Tags != null && site.Tags.ContainsKey(tagName);
        }

        public static bool HasTagWithValue(this CloudOdsWebsite site, string tagName, string tagValue)
        {
            return site.HasTag(tagName) &&
                   site.Tags[tagName].Equals(tagValue, StringComparison.InvariantCultureIgnoreCase);
        }

        public static string GetTagValue(this CloudOdsWebsite site, string tagName)
        {
            return site.HasTag(tagName) ? site.Tags[tagName] : null;
        }

        public static bool IsInRole(this CloudOdsWebsite site, CloudOdsRole role)
        {
            return site.Role == role;
        }

        public static bool IsInEnvironment(this CloudOdsWebsite site, CloudOdsEnvironment environment)
        {
            return site.Environment == environment;
        }

        public static bool IsProductionApi(this CloudOdsWebsite site)
        {
            return site.IsInRole(CloudOdsRole.Api) && site.IsInEnvironment(CloudOdsEnvironment.Production);
        }

        public static bool IsAdminApp(this CloudOdsWebsite site)
        {
            return site.IsInRole(CloudOdsRole.AdminApp);
        }

        public static bool IsSwaggerWebsite(this CloudOdsWebsite site)
        {
            return site.IsInRole(CloudOdsRole.Swagger);
        }
    }
}
