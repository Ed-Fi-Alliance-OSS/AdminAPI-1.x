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
            private const int PsiId = 12345;
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
            private const string PostSecondaryInstitutionCategoryType = "PSI";
            private const string PostSecondaryInstitutionLevel = "Public";
            private const string AdministrativeFundingControl = "Public";
            private const string AccreditationStatusDescriptor = "TestAccreditationStatus";
            private const string FederalLocaleCodeDescriptor = "TestFederalLocaleCode";
            private const bool ImprovingSchool = false;
            private const int StateEducationAgencyId = 999;
            private List<EdFiEducationOrganizationAddress> _addresses;
            private List<EdFiSchoolGradeLevel> _gradeLevels;
            private List<EdFiEducationOrganizationCategory> _edOrgCategories;
            private EdFiLocalEducationAgencyReference _leaReference;
            private SchoolExtensions _schoolExtensions;
            private EdFiStateEducationAgencyReference _stateEducationAgencyReference;

            [SetUp]
            public void Init()
            {
                _profile = new AdminManagementMappingProfile();
                _configuration = new MapperConfiguration(cfg => cfg.AddProfile(_profile));
                _mapper = _configuration.CreateMapper();
                _configuration.AssertConfigurationIsValid();

                _addresses = new List<EdFiEducationOrganizationAddress>
                {
                    new EdFiEducationOrganizationAddress(AddressTypeDescriptor, stateAbbreviationDescriptor: StateAbbreviationDescriptor,
                         apartmentRoomSuiteNumber: ApartmentRoomSuiteNumber, city:City, postalCode:PostalCode, streetNumberName:StreetNumberName)
                };
                _gradeLevels = new List<EdFiSchoolGradeLevel>
                    {new EdFiSchoolGradeLevel(GradeLevelDescriptor)};
                _edOrgCategories = new List<EdFiEducationOrganizationCategory>();
                _leaReference = new EdFiLocalEducationAgencyReference(LeaId);

                var postSecondaryInstitutionReference = new EdFiPostSecondaryInstitutionReference(PsiId);
                var tpdmExtension = new TpdmSchoolExtension(AccreditationStatusDescriptor, FederalLocaleCodeDescriptor, ImprovingSchool, postSecondaryInstitutionReference);
                _schoolExtensions = new SchoolExtensions(tpdmExtension);

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
            public void When_Mapping_EdFiPsiSchool_to_School_Should_Have_Expected_Values_On_Destination()
            {
                // Arrange
                var edfiSchool = new EdFiSchool(Id, Name, SchoolId, _addresses, _edOrgCategories, _gradeLevels, null, _schoolExtensions);

                // Act
                var result = _mapper.Map<PsiSchool>(edfiSchool);

                // Assert
                result.ShouldNotBeNull();
                result.Id.ShouldBe(Id);
                result.Name.ShouldBe(Name);
                result.EducationOrganizationId.ShouldBe(SchoolId);
                result.PostSecondaryInstitutionId.ShouldBe(PsiId);
                result.LocalEducationAgencyId.ShouldBeNull();
                result.StreetNumberName.ShouldBe(StreetNumberName);
                result.ApartmentRoomSuiteNumber.ShouldBe(ApartmentRoomSuiteNumber);
                result.State.ShouldBe(StateAbbreviationDescriptor);
                result.City.ShouldBe(City);
                result.ZipCode.ShouldBe(PostalCode);
                result.GradeLevels.Count.ShouldBeGreaterThan(0);
                result.GradeLevels.First().ShouldBe(GradeLevelDescriptor);
                result.AccreditationStatus.ShouldBe(AccreditationStatusDescriptor);
                result.FederalLocaleCode.ShouldBe(FederalLocaleCodeDescriptor);
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
            public void When_Mapping_EdFiPostSecondaryInstitution_to_PostSecondaryInstitution_Should_Have_Expected_Values_On_Destination()
            {
                // Arrange
                var edfiPsi = new EdFiPostSecondaryInstitution(Id, _addresses, _edOrgCategories,
                    postSecondaryInstitutionLevelDescriptor: PostSecondaryInstitutionLevel,
                    administrativeFundingControlDescriptor: AdministrativeFundingControl,
                    nameOfInstitution: Name, postSecondaryInstitutionId: PsiId);

                // Act
                var result = _mapper.Map<PostSecondaryInstitution>(edfiPsi);

                // Assert
                result.ShouldNotBeNull();
                result.Id.ShouldBe(Id);
                result.Name.ShouldBe(Name);
                result.EducationOrganizationId.ShouldBe(PsiId);
                result.PostSecondaryInstitutionId.ShouldBe(PsiId);
                result.StreetNumberName.ShouldBe(StreetNumberName);
                result.ApartmentRoomSuiteNumber.ShouldBe(ApartmentRoomSuiteNumber);
                result.State.ShouldBe(StateAbbreviationDescriptor);
                result.City.ShouldBe(City);
                result.ZipCode.ShouldBe(PostalCode);
                result.PostSecondaryInstitutionLevel.ShouldBe(PostSecondaryInstitutionLevel);
                result.AdministrativeFundingControl.ShouldBe(AdministrativeFundingControl);
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

                var edfiSchool = new EdFiSchool(Id, Name, SchoolId, new List<EdFiEducationOrganizationAddress>(),
                    new List<EdFiEducationOrganizationCategory>(), new List<EdFiSchoolGradeLevel>());
                // Act
                var result = _mapper.Map(school, edfiSchool);

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
                result._ext.ShouldBeNull();
            }

            [Test]
            public void When_Mapping_PsiSchool_to_EdFiSchool_Should_Have_Expected_Values_On_Destination()
            {
                // Arrange
                var school = new PsiSchool
                {
                    Id = Id,
                    EducationOrganizationId = SchoolId,
                    Name = Name,
                    State = State,
                    ApartmentRoomSuiteNumber = ApartmentRoomSuiteNumber,
                    StreetNumberName = StreetNumberName,
                    City = City,
                    ZipCode = PostalCode,
                    GradeLevels = new List<string> { GradeLevelDescriptor },
                    PostSecondaryInstitutionId = PsiId,
                    AccreditationStatus = AccreditationStatusDescriptor,
                    FederalLocaleCode = FederalLocaleCodeDescriptor,
                    EducationOrganizationCategory = PostSecondaryInstitutionCategoryType
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
                result.EducationOrganizationCategories.First().EducationOrganizationCategoryDescriptor.ShouldContain(PostSecondaryInstitutionCategoryType);
                result.GradeLevels.Count.ShouldBe(1);
                result.GradeLevels.First().GradeLevelDescriptor.ShouldBe(GradeLevelDescriptor);
                result._ext.TPDM.PostSecondaryInstitutionReference.PostSecondaryInstitutionId.ShouldBe(PsiId);
                result._ext.TPDM.AccreditationStatusDescriptor.ShouldBe(AccreditationStatusDescriptor);
                result._ext.TPDM.FederalLocaleCodeDescriptor.ShouldBe(FederalLocaleCodeDescriptor);
                result.LocalEducationAgencyReference.ShouldBeNull();
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

                var edfiLea = new EdFiLocalEducationAgency(Id, new List<EdFiEducationOrganizationAddress>(),
                    new List<EdFiEducationOrganizationCategory>(), LeaId, nameOfInstitution: Name, localEducationAgencyCategoryDescriptor: LocalEducationAgencyCategoryType);
                // Act
                var result = _mapper.Map(lea, edfiLea);

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

            [Test]
            public void When_Mapping_PostSecondaryInstitution_to_EdFiPostSecondaryInstitution_Should_Have_Expected_Values_On_Destination()
            {
                // Arrange
                var psi = new PostSecondaryInstitution
                {
                    Id = Id,
                    PostSecondaryInstitutionId = PsiId,
                    Name = Name,
                    State = State,
                    ApartmentRoomSuiteNumber = ApartmentRoomSuiteNumber,
                    StreetNumberName = StreetNumberName,
                    City = City,
                    ZipCode = PostalCode,
                    EducationOrganizationCategory = PostSecondaryInstitutionCategoryType,
                    PostSecondaryInstitutionLevel = PostSecondaryInstitutionLevel,
                    AdministrativeFundingControl = AdministrativeFundingControl
                };

                // Act
                var result = _mapper.Map<EdFiPostSecondaryInstitution>(psi);

                // Assert
                result.ShouldNotBeNull();
                result.Id.ShouldBe(Id);
                result.PostSecondaryInstitutionId.ShouldBe(PsiId);
                result.NameOfInstitution.ShouldBe(Name);
                result.PostSecondaryInstitutionLevelDescriptor.ShouldBe(PostSecondaryInstitutionLevel);
                result.AdministrativeFundingControlDescriptor.ShouldBe(AdministrativeFundingControl);
                result.Addresses.Count.ShouldBe(1);
                result.Addresses.First().StreetNumberName.ShouldBe(StreetNumberName);
                result.Addresses.First().City.ShouldBe(City);
                result.Addresses.First().StateAbbreviationDescriptor.ShouldContain(State);
                result.Addresses.First().ApartmentRoomSuiteNumber.ShouldBe(ApartmentRoomSuiteNumber);
                result.Addresses.First().PostalCode.ShouldBe(PostalCode);
                result.Categories.Count.ShouldBe(1);
                result.Categories.First().EducationOrganizationCategoryDescriptor.ShouldContain(PostSecondaryInstitutionCategoryType);
            }
        }
    }
}
