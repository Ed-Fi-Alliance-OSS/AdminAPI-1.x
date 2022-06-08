// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Admin.DataAccess.Models;
using Profile = AutoMapper.Profile;
using EdFi.Ods.Admin.Api.Features.Vendors;

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
