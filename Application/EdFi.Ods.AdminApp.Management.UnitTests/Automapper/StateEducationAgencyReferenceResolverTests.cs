// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using AutoMapper;
using EdFi.Ods.AdminApp.Management.Api.Automapper;
using EdFi.Ods.AdminApp.Management.Api.DomainModels;
using EdFi.Ods.AdminApp.Management.Api.Models;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.UnitTests.Automapper
{
    [TestFixture]
    public class StateEducationAgencyReferenceResolverTests
    {
        private const int StateEducationAgencyId = 123;
        private const string Name = "TestSchool";
        private const string Category = "StateLEA";

        [Test]
        public void When_resolve_destination_state_education_agency_reference_from_source()
        {
            // Arrange
            var lea = new LocalEducationAgency
            {
                StateOrganizationId = StateEducationAgencyId
            };

            var edfiLea= new EdFiLocalEducationAgency("id", new List<EdFiEducationOrganizationAddress>(),
                new List<EdFiEducationOrganizationCategory>(), 1234, 
                nameOfInstitution: Name, localEducationAgencyCategoryDescriptor: Category);

            var mapperConfiguration = new MapperConfiguration(cfg =>
                cfg.CreateMap<LocalEducationAgency, EdFiLocalEducationAgency>()
                    .ForMember(dst => dst.StateEducationAgencyReference,
                        opt => opt.MapFrom<StateEducationAgencyReferenceResolver>()));
            var mapper = mapperConfiguration.CreateMapper();

            // Act
            var result = mapper.Map(lea, edfiLea);

            // Assert
            result.ShouldNotBeNull();
            result.StateEducationAgencyReference.ShouldNotBeNull();
            result.StateEducationAgencyReference.StateEducationAgencyId.ShouldBe(StateEducationAgencyId);
        }

        [Test]
        public void When_source_value_isnull_resolve_destination_state_education_agency_reference_from_destination()
        {
            // Arrange
            var edfiLea = new EdFiLocalEducationAgency("id", new List<EdFiEducationOrganizationAddress>(),
                new List<EdFiEducationOrganizationCategory>(), 1234, stateEducationAgencyReference: new EdFiStateEducationAgencyReference(StateEducationAgencyId),
                nameOfInstitution: Name, localEducationAgencyCategoryDescriptor: Category);

            var mapperConfiguration = new MapperConfiguration(cfg =>
                cfg.CreateMap<LocalEducationAgency, EdFiLocalEducationAgency>()
                    .ForMember(dst => dst.StateEducationAgencyReference,
                        opt => opt.MapFrom<StateEducationAgencyReferenceResolver>()));
            var mapper = mapperConfiguration.CreateMapper();

            // Act
            var result = mapper.Map(new LocalEducationAgency(), edfiLea);

            // Assert
            result.ShouldNotBeNull();
            result.StateEducationAgencyReference.ShouldNotBeNull();
            result.StateEducationAgencyReference.StateEducationAgencyId.ShouldBe(StateEducationAgencyId);
        }
    }
}
