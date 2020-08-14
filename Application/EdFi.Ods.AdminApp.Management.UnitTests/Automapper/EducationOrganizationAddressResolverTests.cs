// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EdFi.Ods.AdminApp.Management.Api.Automapper;
using EdFi.Ods.AdminApp.Management.Api.DomainModels;
using EdFi.Ods.AdminApp.Management.Api.Models;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.UnitTests.Automapper
{
    [TestFixture]
    public class EducationOrganizationAddressResolverTests
    {
        private const string City = "Austin";
        private const string StreetNumberName = "street";
        private const string State = "Tx";
        private const string ZipCode = "12309";

        [Test]
        public void Should_resolve_destination_addresses_with_school_address_resolver()
        {
            // Arrange
            var school = new School
            {
                StreetNumberName = StreetNumberName,
                State = State,
                City = City,
                ZipCode = ZipCode
            };
            var edfiSchool = new EdFiSchool("id", "TestSchool", 1234, new List<EdFiEducationOrganizationAddress>(),
                new List<EdFiEducationOrganizationCategory>(), new List<EdFiSchoolGradeLevel>());

            Mapper.Initialize(cfg =>
                cfg.CreateMap<School, EdFiSchool>()
                    .ForMember(dst => dst.Addresses, opt => opt.ResolveUsing<SchoolAddressResolver>()));

            // Act
            var result = Mapper.Map(school, edfiSchool);

            // Assert
            result.ShouldNotBeNull();
            result.Addresses.ShouldNotBeNull();
            result.Addresses.Count.ShouldBeGreaterThan(0);
            result.Addresses.First().City.ShouldBe(City);
            result.Addresses.First().StreetNumberName.ShouldBe(StreetNumberName);
            result.Addresses.First().StateAbbreviationDescriptor.ShouldBe(State);
            result.Addresses.First().PostalCode.ShouldBe(ZipCode);
        }

        [Test]
        public void Should_resolve_destination_addresses_with_local_education_agency_address_resolver()
        {
            // Arrange
            var lea = new LocalEducationAgency
            {
                StreetNumberName = StreetNumberName,
                State = State,
                City = City,
                ZipCode = ZipCode
            };

            var edfiLea = new EdFiLocalEducationAgency("id", new List<EdFiEducationOrganizationAddress>(),
                new List<EdFiEducationOrganizationCategory>(), 1234, localEducationAgencyCategoryDescriptor: "leaCategory",
                nameOfInstitution: "TestLea");

            Mapper.Initialize(cfg =>
                cfg.CreateMap<LocalEducationAgency, EdFiLocalEducationAgency>()
                    .ForMember(dst => dst.Addresses, opt => opt.ResolveUsing<LocalEducationAgencyAddressResolver>()));

            // Act
            var result = Mapper.Map(lea, edfiLea);

            // Assert
            result.ShouldNotBeNull();
            result.Addresses.ShouldNotBeNull();
            result.Addresses.Count.ShouldBeGreaterThan(0);
            result.Addresses.First().City.ShouldBe(City);
            result.Addresses.First().StreetNumberName.ShouldBe(StreetNumberName);
            result.Addresses.First().StateAbbreviationDescriptor.ShouldBe(State);
            result.Addresses.First().PostalCode.ShouldBe(ZipCode);
        }
    }
}
