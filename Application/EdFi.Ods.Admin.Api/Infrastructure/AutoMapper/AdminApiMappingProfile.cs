// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Admin.DataAccess.Models;
using Profile = AutoMapper.Profile;
using EdFi.Ods.Admin.Api.Features.Vendors;
using EdFi.Ods.Admin.Api.Features.Applications;
using EdFi.Ods.AdminApp.Management.Database.Commands;
using EdFi.Security.DataAccess.Models;
using EdFi.Ods.Admin.Api.Features.ClaimSets;

namespace EdFi.Ods.Admin.Api.Infrastructure
{
    public class AdminApiMappingProfile : Profile
    {
        public AdminApiMappingProfile()
        {

            CreateMap<Vendor, EditVendor.Request>()
                .ForMember(dst => dst.Company, opt => opt.MapFrom(src => src.VendorName))
                .ForMember(dst => dst.ContactName, opt => opt.MapFrom(src => src.ContactName()))
                .ForMember(dst => dst.ContactEmailAddress, opt => opt.MapFrom(src => src.ContactEmail()))
                .ForMember(dst => dst.NamespacePrefixes, opt => opt.MapFrom(src => ToCommaSeparated(src.VendorNamespacePrefixes)));

            CreateMap<Vendor, VendorModel>()
                .ForMember(dst => dst.Company, opt => opt.MapFrom(src => src.VendorName))
                .ForMember(dst => dst.ContactName, opt => opt.MapFrom(src => src.ContactName()))
                .ForMember(dst => dst.ContactEmailAddress, opt => opt.MapFrom(src => src.ContactEmail()))
                .ForMember(dst => dst.NamespacePrefixes, opt => opt.MapFrom(src => ToCommaSeparated(src.VendorNamespacePrefixes)));

            CreateMap<EdFi.Admin.DataAccess.Models.Application, ApplicationModel>()
                .ForMember(dst => dst.EducationOrganizationId, opt => opt.MapFrom(src => src.ApplicationEducationOrganizations.First().EducationOrganizationId))
                .ForMember(dst => dst.ProfileName, opt => opt.MapFrom(src => src.ProfileName()));

            CreateMap<AddApplicationResult, ApplicationResult>()
                .ForMember(dst => dst.ApplicationId, opt => opt.MapFrom(src => src.ApplicationId))
                .ForMember(dst => dst.Key, opt => opt.MapFrom(src => src.Key))
                .ForMember(dst => dst.Secret, opt => opt.MapFrom(src => src.Secret));

            CreateMap<RegenerateApiClientSecretResult, ApplicationResult>()
                .ForMember(dst => dst.ApplicationId, opt => opt.MapFrom(src => src.Application.ApplicationId))
                .ForMember(dst => dst.Key, opt => opt.MapFrom(src => src.Key))
                .ForMember(dst => dst.Secret, opt => opt.MapFrom(src => src.Secret));


            CreateMap<AuthorizationStrategy, AdminApp.Management.ClaimSetEditor.AuthorizationStrategy>()
                .ForMember(dst => dst.AuthStrategyName, opt => opt.MapFrom(src => src.AuthorizationStrategyName))
                .ForMember(dst => dst.AuthStrategyId, opt => opt.MapFrom(src => src.AuthorizationStrategyId))
                .ForMember(dst => dst.IsInheritedFromParent, opt => opt.Ignore());

            CreateMap<AdminApp.Management.ClaimSetEditor.ClaimSet, ClaimSetDetailsModel>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dst => dst.IsSystemReserved, opt => opt.MapFrom(src => !src.IsEditable));

            CreateMap<ClaimSet, ClaimSetModel>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.ClaimSetId))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.ClaimSetName));

            CreateMap<AdminApp.Management.ClaimSetEditor.ResourceClaim, ResourceClaimModel>()
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dst => dst.Read, opt => opt.MapFrom(src => src.Read))
                .ForMember(dst => dst.Update, opt => opt.MapFrom(src => src.Update))
                .ForMember(dst => dst.Create, opt => opt.MapFrom(src => src.Create))
                .ForMember(dst => dst.Delete, opt => opt.MapFrom(src => src.Delete))
                .ForMember(dst => dst.AuthStrategyOverridesForCRUD, opt => opt.MapFrom(src => src.AuthStrategyOverridesForCRUD))
                .ForMember(dst => dst.DefaultAuthStrategiesForCRUD, opt => opt.MapFrom(src => src.DefaultAuthStrategiesForCRUD))
                .ForMember(dst => dst.Children, opt => opt.MapFrom(src => src.Children));

            CreateMap<AdminApp.Management.ClaimSetEditor.AuthorizationStrategy, AuthorizationStrategyModel>()
                .ForMember(dst => dst.AuthStrategyId, opt => opt.MapFrom(src => src.AuthStrategyId))
                .ForMember(dst => dst.AuthStrategyName, opt => opt.MapFrom(src => src.AuthStrategyName))
                .ForMember(dst => dst.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
                .ForMember(dst => dst.IsInheritedFromParent, opt => opt.MapFrom(src => src.IsInheritedFromParent));

            CreateMap<AuthorizationStrategyModel, AdminApp.Management.ClaimSetEditor.AuthorizationStrategy>()
                .ForMember(dst => dst.AuthStrategyId, opt => opt.MapFrom(src => src.AuthStrategyId))
                .ForMember(dst => dst.AuthStrategyName, opt => opt.MapFrom(src => src.AuthStrategyName))
                .ForMember(dst => dst.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
                .ForMember(dst => dst.IsInheritedFromParent, opt => opt.MapFrom(src => src.IsInheritedFromParent));

            CreateMap<ResourceClaimModel, AdminApp.Management.ClaimSetEditor.ResourceClaim>()
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dst => dst.Read, opt => opt.MapFrom(src => src.Read))
                .ForMember(dst => dst.Update, opt => opt.MapFrom(src => src.Update))
                .ForMember(dst => dst.Create, opt => opt.MapFrom(src => src.Create))
                .ForMember(dst => dst.Delete, opt => opt.MapFrom(src => src.Delete))
                .ForMember(dst => dst.AuthStrategyOverridesForCRUD, opt => opt.MapFrom(src => src.AuthStrategyOverridesForCRUD))
                .ForMember(dst => dst.DefaultAuthStrategiesForCRUD, opt => opt.MapFrom(src => src.DefaultAuthStrategiesForCRUD))
                .ForMember(dst => dst.Children, opt => opt.MapFrom(src => src.Children));


        }

        private string ToCommaSeparated(ICollection<VendorNamespacePrefix> vendorNamespacePrefixes)
        {
            return vendorNamespacePrefixes != null && vendorNamespacePrefixes.Any()
                            ? ToDelimiterSeparated(vendorNamespacePrefixes.Select(x => x.NamespacePrefix))
                            : string.Empty;
        }

        public string ToDelimiterSeparated(IEnumerable<string> inputStrings, string separator = ",")
        {
            var listOfStrings = inputStrings.ToList();

            return listOfStrings.Any()
                ? string.Join(separator, listOfStrings)
                : string.Empty;
        }
    }
}
