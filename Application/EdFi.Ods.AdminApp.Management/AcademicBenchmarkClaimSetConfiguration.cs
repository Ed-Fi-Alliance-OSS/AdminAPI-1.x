// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Configuration.Claims;

namespace EdFi.Ods.AdminApp.Management
{
    public static class AcademicBenchmarkClaimSetConfiguration
    {
        public static CloudOdsClaimSet Default = new CloudOdsClaimSet
        {
            ApplicationName = CloudOdsAdminApp.SecurityContextApplicationName,
            ClaimSetName = CloudsOdsAcademicBenchmarksConnectApp.DefaultClaimSet,
            Claims = new[]
            {
                new CloudOdsEntityClaim
                {
                    EntityName = "gradeLevelDescriptor",
                    Actions = new[] { CloudOdsClaimAction.Create, CloudOdsClaimAction.Read, CloudOdsClaimAction.Update, CloudOdsClaimAction.Delete }
                },
                new CloudOdsEntityClaim
                {
                    EntityName = "publicationStatusDescriptor",
                    Actions = new[] { CloudOdsClaimAction.Create, CloudOdsClaimAction.Read, CloudOdsClaimAction.Update, CloudOdsClaimAction.Delete }
                },
                new CloudOdsEntityClaim
                {
                    EntityName = "academicSubjectDescriptor",
                    Actions = new[] { CloudOdsClaimAction.Create, CloudOdsClaimAction.Read, CloudOdsClaimAction.Update, CloudOdsClaimAction.Delete }
                },
                new CloudOdsEntityClaim
                {
                    EntityName = "educationStandards",
                    Actions = new[] { CloudOdsClaimAction.Create, CloudOdsClaimAction.Read, CloudOdsClaimAction.Update, CloudOdsClaimAction.Delete }
                }
            }
        };
    }
}