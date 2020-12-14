// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Api.Models;
using EdFi.Ods.AdminApp.Web.Helpers;
using EdFi.Ods.AdminApp.Web.Models.ViewModels;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.Descriptors;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.EducationOrganizations;
using System.Linq;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.Global;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.OdsInstances;
using EdFi.Security.DataAccess.Models;
using Application = EdFi.Admin.DataAccess.Models.Application;
using Profile = AutoMapper.Profile;

namespace EdFi.Ods.AdminApp.Web.Infrastructure.AutoMapper
{
    public class AdminWebMappingProfile : Profile
    {
        public AdminWebMappingProfile()
        {
            CreateMap<Application, ApplicationModel>()
                .ForMember(dst => dst.EducationOrganizations, opt => opt.MapFrom(src => src.ApplicationEducationOrganizations))
                .ForMember(dst => dst.ProfileName, opt => opt.MapFrom(src => src.ProfileName()));
                //.ForMember(dst => dst.DisplayName, opt => opt.MapFrom(src => src.DisplayName()));

            CreateMap<ApplicationEducationOrganization, EducationOrganizationModel>()
                .ForMember(dst => dst.EducationOrganizationId, opt => opt.MapFrom(src => src.EducationOrganizationId))
                .ForMember(dst => dst.Name, opt => opt.MapFrom<EducationOrganizationNameResolver>());

            CreateMap<Admin.DataAccess.Models.Profile, ProfileModel>();

            CreateMap<Vendor, EditVendorModel>()
                .ForMember(dst => dst.Company, opt => opt.MapFrom(src => src.VendorName))
                .ForMember(dst => dst.ContactName, opt => opt.MapFrom(src => src.ContactName()))
                .ForMember(dst => dst.ContactEmailAddress, opt => opt.MapFrom(src => src.ContactEmail()))
                .ForMember(dst => dst.NamespacePrefix, opt => opt.MapFrom(src =>
                    src.VendorNamespacePrefixes != null && src.VendorNamespacePrefixes.Any()
                        ? src.VendorNamespacePrefixes.First().NamespacePrefix
                        : string.Empty));

            //CreateMap<Vendor, VendorApplicationsModel>()
            //    .ForMember(dst => dst.Applications,
            //        opt => opt.MapFrom(src => src.Applications == null ? null : src.Applications.Where(app => app.IsProductionApplication())));
            CreateMap<Vendor, VendorApplicationsModel>()
                .ForMember(dst => dst.Applications, opt => opt.MapFrom(src => src.Applications));

            CreateMap<Vendor, VendorOverviewModel>()
                .ForMember(dst => dst.NamespacePrefix, opt => opt.MapFrom(src =>
                        src.VendorNamespacePrefixes != null && src.VendorNamespacePrefixes.Any()
                            ? src.VendorNamespacePrefixes.First().NamespacePrefix
                            : string.Empty));

            CreateMap<EducationOrganization, EducationOrganizationModel>();
            CreateMap<LocalEducationAgency, EducationOrganizationModel>();
            CreateMap<School, EducationOrganizationModel>();

            CreateMap<AddLocalEducationAgencyModel, LocalEducationAgency>()
                .ForMember(dst => dst.Id, opt => opt.Ignore())
                .ForMember(dst => dst.EducationOrganizationId, opt => opt.MapFrom(src => src.LocalEducationAgencyId))
                .ForMember(dst => dst.EducationOrganizationCategory, opt => opt.MapFrom(src => EducationOrganizationTypes.Instance.LocalEducationAgency));

            CreateMap<AddSchoolModel, School>()
                .ForMember(dst => dst.Id, opt => opt.Ignore())
                .ForMember(dst => dst.EducationOrganizationId, opt => opt.MapFrom(src => src.SchoolId))
                .ForMember(dst => dst.EducationOrganizationCategory, opt => opt.MapFrom(src => EducationOrganizationTypes.Instance.SchoolType));

            CreateMap<LocalEducationAgency, EditLocalEducationAgencyModel>()
                .ForMember(dst => dst.LocalEducationAgencyCategoryTypeOptions, opt => opt.Ignore())
                .ForMember(dst => dst.StateOptions, opt => opt.Ignore());

            CreateMap<EditLocalEducationAgencyModel, LocalEducationAgency>()
                .ForMember(dst => dst.EducationOrganizationId, opt => opt.MapFrom(src => src.LocalEducationAgencyId))
                .ForMember(dst => dst.EducationOrganizationCategory, opt => opt.MapFrom(src => EducationOrganizationTypes.Instance.LocalEducationAgency));

            CreateMap<School, EditSchoolModel>()
                .ForMember(dst => dst.SchoolId, opt => opt.MapFrom(src => src.EducationOrganizationId))
                .ForMember(dst => dst.GradeLevelOptions, opt => opt.Ignore())
                .ForMember(dst => dst.StateOptions, opt => opt.Ignore());

            CreateMap<EditSchoolModel, School>()
                .ForMember(dst => dst.EducationOrganizationId, opt => opt.MapFrom(src => src.SchoolId))
                .ForMember(dst => dst.EducationOrganizationCategory, opt => opt.MapFrom(src => EducationOrganizationTypes.Instance.SchoolType));
                
            CreateMap<Descriptor, DescriptorModel>();

            CreateMap<AuthorizationStrategy, Management.ClaimSetEditor.AuthorizationStrategy>()
                .ForMember(dst => dst.AuthStrategyName, opt => opt.MapFrom(src => src.AuthorizationStrategyName))
                .ForMember(dst => dst.AuthStrategyId, opt => opt.MapFrom(src => src.AuthorizationStrategyId))
                .ForMember(dst => dst.IsInheritedFromParent, opt => opt.Ignore());

            CreateMap<AdminAppUser, UserModel>()
                .ForMember(dst => dst.UserId, opt => opt.MapFrom(src => src.Id));

            CreateMap<OdsInstanceRegistration, OdsInstanceModel>();
        }
    }
}
