// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.Admin.Api.Features.Applications
{
    public class ApplicationModel
    {
        public int ApplicationId { get; set; }
        public string? ApplicationName { get; set; }
        public string? ClaimSetName { get; set; }
        public string? ProfileName { get; set; }
        public int EducationOrganizationId { get; set; }
        public string? OdsInstanceName { get; set; }
    }

    public class ApplicationResult
    {
        public int ApplicationId { get; set; }
        public string? Key { get; set; }
        public string? Secret { get; set; }
    }
}
