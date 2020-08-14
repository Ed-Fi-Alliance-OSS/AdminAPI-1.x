// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace EdFi.Ods.AdminApp.Web.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetSignedInUserId(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        public static string GetSignedInUserName(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirst(ClaimTypes.Name).Value.Split('#')[claimsPrincipal.FindFirst(ClaimTypes.Name).Value.Split('#').Length - 1];
        }

        public static string GetTenantId(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
        }

        public static IEnumerable<string> GetTenantIds(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindAll("http://schemas.microsoft.com/identity/claims/tenantid").Select(c => c.Value);
        } 
    }
}