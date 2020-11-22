// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Configuration.Claims;
using static EdFi.Ods.AdminApp.Management.Configuration.Claims.CloudOdsClaimAction;
using static EdFi.Ods.AdminApp.Management.Configuration.Claims.CloudOdsClaimAuthorizationStrategy;

namespace EdFi.Ods.AdminApp.Web.Infrastructure
{
    public static class CloudOdsAdminAppClaimSetConfiguration
    {
        public static CloudOdsClaimSet Default = new CloudOdsClaimSet
        {
            ApplicationName = CloudOdsAdminApp.SecurityContextApplicationName,
            ClaimSetName = CloudOdsAdminApp.InternalAdminAppClaimSet,
            Claims = new[]
            {
                new CloudOdsEntityClaim
                {
                    EntityName = "educationOrganizations",
                    Actions = new[] { Create, Read, Update, Delete },
                    AuthorizationStrategy = NoFurtherAuthorizationRequired
                },
                new CloudOdsEntityClaim
                {
                    EntityName = "systemDescriptors",
                    Actions = new[] { Create, Read, Update, Delete },
                    AuthorizationStrategy = NoFurtherAuthorizationRequired
                },
                new CloudOdsEntityClaim
                {
                    EntityName = "managedDescriptors",
                    Actions = new[] { Create, Read, Update, Delete },
                    AuthorizationStrategy = NoFurtherAuthorizationRequired
                },
                new CloudOdsEntityClaim
                {
                    EntityName = "types",
                    Actions = new[] { Read },
                    AuthorizationStrategy = NoFurtherAuthorizationRequired
                }
            }
        };
    }
}