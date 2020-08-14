// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Castle.Components.DictionaryAdapter;
using EdFi.Ods.AdminApp.Management.Api.Automapper;
using EdFi.Ods.AdminApp.Management.Api.DomainModels;
using EdFi.Ods.AdminApp.Management.Api.Models;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.UnitTests.Automapper
{
    [TestFixture]
    public class AdminManagementMappingProfileTests
    {
        public class When_Auto_Mapper_Configuration_Is_Valid
        {
            [Test]
            public void Should_have_valid_configuration()
            {
                // Arrange
                var profile = new AdminManagementMappingProfile();
                var configuration = new MapperConfiguration(cfg => cfg.AddProfile(profile));

                // Assert
                configuration.AssertConfigurationIsValid();
            }
        }

        // Tests for verifying the mapped properties from source to destination, which excludes ignored properties and property value resolved with custom resolver 

        public class When_Mapping_Values_From_Source_To_Destination
        {
            private AdminManagementMappingProfile _profile;
            private MapperConfiguration _configuration;
            private IMapper _mapper;

            private const string Id = "id";
            private const string Name = "name";
            private const int SchoolId = 1000;
            private const int LeaId = 1234;
            private const string AddressTypeDescriptor = "Namespace#Physical";
            private const string StreetNumberName = "StreetNumberName";
            private const string ApartmentRoomSuiteNumber = "206";
            private const string StateAbbreviationDescriptor = "Namespace#TX";
            private const string State = "TX";
            private const string City = "Austin";
            private const string PostalCode = "78989";
            private const string GradeLevelDescriptor = "Namespace#Kinder";
            private const string LocalEducationAgencyCategoryDescriptor = "NameSpace#Private";
            private const string LocalEducationAgencyCategoryType = "Public";
            private const int StateEducationAgencyId = 999;
            private EditableList<EdFiEducationOrganizationAddress> _addresses;
            private EditableList<EdFiSchoolGradeLevel> _gradeLevels;
            private EditableList<EdFiEducationOrganizationCategory> _edOrgCategories;
            private EdFiLocalEducationAgencyReference _leaReference;
            private EdFiStateEducationAgencyReference _stateEducationAgencyReference;

            [SetUp]
            public void Init()
            {
                _profile = new AdminManagementMappingProfile();
                _configuration = new MapperConfiguration(cfg => cfg.AddProfile(_profile));
                _mapper = _configuration.CreateMapper();
                _configuration.AssertConfigurationIsValid();

                _addresses = new EditableList<EdFiEducationOrganizationAddress>
                {
                    new EdFiEducationOrganizationAddress(AddressTypeDescriptor, stateAbbreviationDescriptor: StateAbbreviationDescriptor,
                         apartmentRoomSuiteNumber: ApartmentRoomSuiteNumber, city:City, postalCode:PostalCode, streetNumberName:StreetNumberName)
                };
                _gradeLevels = new EditableList<EdFiSchoolGradeLevel>
                    {new EdFiSchoolGradeLevel(GradeLevelDescriptor)};
                _edOrgCategories = new EditableList<EdFiEducationOrganizationCategory>();
                _leaReference = new EdFiLocalEducationAgencyReference(LeaId);
                _stateEducationAgencyReference = new EdFiStateEducationAgencyReference(StateEducationAgencyId);
            }

            [Test]
            public void When_Mapping_EdFiSchool_to_School_Should_Have_Expected_Values_On_Destination()
            {
                // Arrange
                var edfiSchool = new EdFiSchool(Id, Name, SchoolId, _addresses, _edOrgCategories, _gradeLevels, _leaReference);

                // Act
                 var result = _mapper.Map<School>(edfiSchool);

                // Assert
                result.ShouldNotBeNull();
                result.Id.ShouldBe(Id);
                result.Name.ShouldBe(Name);
                result.EducationOrganizationId.ShouldBe(SchoolId);
                result.LocalEducationAgencyId.ShouldBe(LeaId);
                result.StreetNumberName.ShouldBe(StreetNumberName);
                result.ApartmentRoomSuiteNumber.ShouldBe(ApartmentRoomSuiteNumber);
                result.State.ShouldBe(StateAbbreviationDescriptor);
                result.City.ShouldBe(City);
                result.ZipCode.ShouldBe(PostalCode);
                result.GradeLevels.Count.ShouldBeGreaterThan(0);
                result.GradeLevels.First().ShouldBe(GradeLevelDescriptor);
            }

            [Test]
            public void When_Mapping_EdFiLocalEducationAgency_to_LocalEducationAgency_Should_Have_Expected_Values_On_Destination()
            {
                // Arrange
                var edfiSchool = new EdFiLocalEducationAgency(Id, _addresses, _edOrgCategories,
                    localEducationAgencyCategoryDescriptor: LocalEducationAgencyCategoryDescriptor,
                    nameOfInstitution: Name, localEducationAgencyId: LeaId, stateEducationAgencyReference: _stateEducationAgencyReference);

                // Act
                var result = _mapper.Map<LocalEducationAgency>(edfiSchool);

                // Assert
                result.ShouldNotBeNull();
                result.Id.ShouldBe(Id);
                result.Name.ShouldBe(Name);
                result.EducationOrganizationId.ShouldBe(LeaId);
                result.LocalEducationAgencyId.ShouldBe(LeaId);
                result.StreetNumberName.ShouldBe(StreetNumberName);
                result.ApartmentRoomSuiteNumber.ShouldBe(ApartmentRoomSuiteNumber);
                result.State.ShouldBe(StateAbbreviationDescriptor);
                result.City.ShouldBe(City);
                result.ZipCode.ShouldBe(PostalCode);
                result.LocalEducationAgencyCategoryType.ShouldBe(LocalEducationAgencyCategoryDescriptor);
                result.StateOrganizationId.ShouldBe(StateEducationAgencyId);
            }

            [Test]
            public void When_Mapping_School_to_EdFiSchool_Should_Have_Expected_Values_On_Destination()
            {
                // Arrange
                var school = new School
                {
                    Id = Id,
                    EducationOrganizationId = SchoolId,
                    Name = Name,
                    State = State,
                    ApartmentRoomSuiteNumber = ApartmentRoomSuiteNumber,
                    StreetNumberName = StreetNumberName,
                    City = City,
                    ZipCode = PostalCode,
                    GradeLevels = new List<string> { GradeLevelDescriptor},
                    LocalEducationAgencyId = LeaId,
                    EducationOrganizationCategory = LocalEducationAgencyCategoryType
                };

                // Act
                var result = _mapper.Map<EdFiSchool>(school);

                // Assert
                result.ShouldNotBeNull();
                result.Id.ShouldBe(Id);
                result.SchoolId.ShouldBe(SchoolId);
                result.NameOfInstitution.ShouldBe(Name);
                result.Addresses.Count.ShouldBe(1);
                result.Addresses.First().StreetNumberName.ShouldBe(StreetNumberName);
                result.Addresses.First().ApartmentRoomSuiteNumber.ShouldBe(ApartmentRoomSuiteNumber);
                result.Addresses.First().City.ShouldBe(City);
                result.Addresses.First().StateAbbreviationDescriptor.ShouldContain(State);
                result.Addresses.First().PostalCode.ShouldBe(PostalCode);
                result.EducationOrganizationCategories.Count.ShouldBe(1);
                result.EducationOrganizationCategories.First().EducationOrganizationCategoryDescriptor.ShouldContain(LocalEducationAgencyCategoryType);
                result.GradeLevels.Count.ShouldBe(1);
                result.GradeLevels.First().GradeLevelDescriptor.ShouldBe(GradeLevelDescriptor);
                result.LocalEducationAgencyReference.LocalEducationAgencyId.ShouldBe(LeaId);
            }

            [Test]
            public void When_Mapping_LocalEducationAgency_to_EdFiLocalEducationAgency_Should_Have_Expected_Values_On_Destination()
            {
                // Arrange
                var lea = new LocalEducationAgency
                {
                    Id = Id,
                    LocalEducationAgencyId = LeaId,
                    Name = Name,
                    LocalEducationAgencyCategoryType = LocalEducationAgencyCategoryType,
                    State = State,
                    ApartmentRoomSuiteNumber = ApartmentRoomSuiteNumber,
                    StreetNumberName = StreetNumberName,
                    City = City,
                    ZipCode = PostalCode,
                    StateOrganizationId = StateEducationAgencyId,
                    EducationOrganizationCategory = LocalEducationAgencyCategoryType
                };

                // Act
                var result = _mapper.Map<EdFiLocalEducationAgency>(lea);

                // Assert
                result.ShouldNotBeNull();
                result.Id.ShouldBe(Id);
                result.LocalEducationAgencyId.ShouldBe(LeaId);
                result.NameOfInstitution.ShouldBe(Name);
                result.LocalEducationAgencyCategoryDescriptor.ShouldBe(LocalEducationAgencyCategoryType);
                result.Addresses.Count.ShouldBe(1);
                result.Addresses.First().StreetNumberName.ShouldBe(StreetNumberName);
                result.Addresses.First().City.ShouldBe(City);
                result.Addresses.First().StateAbbreviationDescriptor.ShouldContain(State);
                result.Addresses.First().ApartmentRoomSuiteNumber.ShouldBe(ApartmentRoomSuiteNumber);
                result.Addresses.First().PostalCode.ShouldBe(PostalCode);
                result.StateEducationAgencyReference.StateEducationAgencyId.ShouldBe(StateEducationAgencyId);
                result.Categories.Count.ShouldBe(1);
                result.Categories.First().EducationOrganizationCategoryDescriptor.ShouldContain(LocalEducationAgencyCategoryType);
            }
        }
    }
}
