// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using AutoMapper;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApp.Web.Helpers;
using EdFi.Ods.AdminApp.Web.Models.ViewModels;

namespace EdFi.Ods.AdminApp.Web.Infrastructure.AutoMapper
{
    public class EducationOrganizationNameResolver : IValueResolver<ApplicationEducationOrganization, EducationOrganizationModel, string>
    {
        public string Resolve(ApplicationEducationOrganization source, EducationOrganizationModel destination, string destMember, ResolutionContext context)
        {
            var edOrgs = context.GetEducationOrganizations();
            var edOrg = edOrgs.FirstOrDefault(o => o.EducationOrganizationId == source.EducationOrganizationId);

            return edOrg != null ? edOrg.Name : "";
        }
    }
}