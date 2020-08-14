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
    public class LocalEducationAgencyReferenceResolverTests
    {
        private const int LocalEducationAgencyId = 123;
        private const string Name = "TestSchool";

        [Test]
        public void When_resolve_destination_local_education_agency_reference_from_source()
        {
            // Arrange
            var school = new School
            {
                LocalEducationAgencyId = LocalEducationAgencyId
            };

            var edfiSchool = new EdFiSchool("id", Name, 1234, new List<EdFiEducationOrganizationAddress>(),
                new List<EdFiEducationOrganizationCategory>(), new List<EdFiSchoolGradeLevel>());

            Mapper.Initialize(cfg =>
                cfg.CreateMap<School, EdFiSchool>()
                    .ForMember(dst => dst.LocalEducationAgencyReference,
                        opt => opt.ResolveUsing<LocalEducationAgencyReferenceResolver>()));

            // Act
            var result = Mapper.Map(school, edfiSchool);

            // Assert
            result.ShouldNotBeNull();
            result.LocalEducationAgencyReference.ShouldNotBeNull();
            result.LocalEducationAgencyReference.LocalEducationAgencyId.ShouldBe(LocalEducationAgencyId);
        }

        [Test]
        public void When_source_value_isnull_resolve_destination_local_education_agency_reference_from_destination()
        {
            // Arrange
            var edfiSchool = new EdFiSchool("id", Name, 1234,
                new List<EdFiEducationOrganizationAddress>(),
                new List<EdFiEducationOrganizationCategory>(), new List<EdFiSchoolGradeLevel>(),
                new EdFiLocalEducationAgencyReference(LocalEducationAgencyId));

            Mapper.Initialize(cfg =>
                cfg.CreateMap<School, EdFiSchool>()
                    .ForMember(dst => dst.LocalEducationAgencyReference,
                        opt => opt.ResolveUsing<LocalEducationAgencyReferenceResolver>()));

            // Act
            var result = Mapper.Map(new School(), edfiSchool);

            // Assert
            result.ShouldNotBeNull();
            result.LocalEducationAgencyReference.ShouldNotBeNull();
            result.LocalEducationAgencyReference.LocalEducationAgencyId.ShouldBe(LocalEducationAgencyId);
        }
    }
}
