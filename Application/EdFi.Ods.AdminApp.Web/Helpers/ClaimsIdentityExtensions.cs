// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Security.Claims;

namespace EdFi.Ods.AdminApp.Web.Helpers
{
    public static class ClaimsIdentityExtensions
    {
        public static string GetSignedInUserName(this ClaimsIdentity claimsIdentity)
        {
            var usernameParts = claimsIdentity.FindFirst(ClaimTypes.Name).Value.Split('#');
            return usernameParts[usernameParts.Length - 1];
        }

        public static string GetTenantId(this ClaimsIdentity claimsIdentity)
        {
            return claimsIdentity.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
        }
    }
}
