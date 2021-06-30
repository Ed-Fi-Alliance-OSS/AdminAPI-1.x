// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApp.Management.Api.Enumerations;
using EdFi.Ods.AdminApp.Management.Api.Models;
using System.Collections.Generic;
using System.Linq;
using EdFi.Ods.AdminApp.Management.Api.DomainModels;
using LocalEducationAgency = EdFi.Ods.AdminApp.Management.Api.Models.LocalEducationAgency;
using School = EdFi.Ods.AdminApp.Management.Api.Models.School;

namespace EdFi.Ods.AdminApp.Management.Api.Automapper
{
    public class SchoolAddressResolver : IValueResolver<School, EdFiSchool, List<EdFiEducationOrganizationAddress>>
    {
        public List<EdFiEducationOrganizationAddress> Resolve(School source, EdFiSchool destination, List<EdFiEducationOrganizationAddress> destMember, ResolutionContext context)
        {
            var addressList = EducationOrganizationAddressResolver.ReplaceFirstAddressInList(source, destMember);
            destMember?.Clear(); // by default, automapper combines the source list with the destination list. we want to replace the destination list instead
            return addressList;
        }
    }

    public class LocalEducationAgencyAddressResolver : IValueResolver<LocalEducationAgency, EdFiLocalEducationAgency, List<EdFiEducationOrganizationAddress>>
    {
        public List<EdFiEducationOrganizationAddress> Resolve(LocalEducationAgency source,
            EdFiLocalEducationAgency destination, List<EdFiEducationOrganizationAddress> destMember,
            ResolutionContext context)
        {
            var addressList = EducationOrganizationAddressResolver.ReplaceFirstAddressInList(source, destMember);
            destMember?.Clear(); // by default, automapper combines the source list with the destination list. we want to replace the destination list instead
            return addressList;
        }
    }

    public class PostSecondaryInstitutionAddressResolver : IValueResolver<PostSecondaryInstitution, EdFiPostSecondaryInstitution, List<EdFiEducationOrganizationAddress>>
    {
        public List<EdFiEducationOrganizationAddress> Resolve(PostSecondaryInstitution source,
            EdFiPostSecondaryInstitution destination, List<EdFiEducationOrganizationAddress> destMember,
            ResolutionContext context)
        {
            var addressList = EducationOrganizationAddressResolver.ReplaceFirstAddressInList(source, destMember);
            destMember?.Clear(); // by default, automapper combines the source list with the destination list. we want to replace the destination list instead
            return addressList;
        }
    }

    public static class EducationOrganizationAddressResolver
    {
        public static List<EdFiEducationOrganizationAddress> ReplaceFirstAddressInList(EducationOrganization source,
            ICollection<EdFiEducationOrganizationAddress> originalList)
        {
            var addressList = originalList?.ToList() ?? new List<EdFiEducationOrganizationAddress>();
            originalList?.Clear();

            var address = Resolve(source)[0];

            if (addressList.Any())
            {
                addressList[0] = address;
            }
            else
            {
                addressList.Add(address);
            }

            return addressList;
        }

        public static List<EdFiEducationOrganizationAddress> Resolve(EducationOrganization source)
        {
            var address = new EdFiEducationOrganizationAddress(
                "uri://ed-fi.org/AddressTypeDescriptor#" + AddressType.Physical,
                stateAbbreviationDescriptor: source.State,
                apartmentRoomSuiteNumber: source.ApartmentRoomSuiteNumber,
                streetNumberName: source.StreetNumberName,
                city: source.City,
                postalCode: source.ZipCode);

            return new List<EdFiEducationOrganizationAddress>
            {
                address
            };
        }
    }
}
