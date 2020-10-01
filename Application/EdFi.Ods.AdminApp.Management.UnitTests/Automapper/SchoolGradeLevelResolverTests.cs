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
    public class SchoolGradeLevelResolverTests
    {
        public class When_resolving_value_with_grade_level_resolver
        {
            private const string Kinder = "Kinder";

            [Test]
            public void Should_resolve_destination_grade_levels()
            {
                // Arrange
                var school = new School
                {
                    GradeLevels = new List<string> {Kinder, "FirstGrade", "SecondGrade"}
                };

                var edfiSchool = new EdFiSchool(
                    "id", "TestSchool", 1234, new List<EdFiEducationOrganizationAddress>(),
                    new List<EdFiEducationOrganizationCategory>(), new List<EdFiSchoolGradeLevel>());

                var mapperConfiguration = new MapperConfiguration(
                    cfg =>
                        cfg.CreateMap<School, EdFiSchool>()
                            .ForMember(
                                dst => dst.GradeLevels, opt => opt.MapFrom<SchoolGradeLevelResolver>()));
                var mapper = mapperConfiguration.CreateMapper();

                // Act
                var result = mapper.Map(school, edfiSchool);

                // Assert
                result.ShouldNotBeNull();
                result.GradeLevels.ShouldNotBeNull();
                result.GradeLevels.Count.ShouldBe(3);
                result.GradeLevels.First().GradeLevelDescriptor.ShouldBe(Kinder);
            }
        }
    }
}
