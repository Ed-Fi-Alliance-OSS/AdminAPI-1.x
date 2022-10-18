// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.Admin.Api.Infrastructure.Security;

public static class SecurityConstants
{
    public const string TokenEndpointUri = "/connect/token";

    public static class Scopes
    {
        public const string AdminApiFullAccess = "edfi_admin_api/full_access";
    }
}
