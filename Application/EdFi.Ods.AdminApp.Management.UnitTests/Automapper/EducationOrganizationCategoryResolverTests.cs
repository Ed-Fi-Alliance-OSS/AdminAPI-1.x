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
    public class EducationOrganizationCategoryResolverTests
    {
        public class When_resolving_value_with_education_organization_category_resolver
        {
            private const string EdOrgCategory = "category";

            private const string EdOrgCategoryDescriptor =
                "uri://ed-fi.org/EducationOrganizationCategoryDescriptor#" + EdOrgCategory;

            [Test]
            public void Should_resolve_destination_edorg_categories()
            {
                // Arrange
                var school = new School
                {
                    EducationOrganizationCategory = EdOrgCategory
                };

                var edfiSchool = new EdFiSchool("id", "TestSchool", 1234, new List<EdFiEducationOrganizationAddress>(),
                    new List<EdFiEducationOrganizationCategory>(), new List<EdFiSchoolGradeLevel>());

                var mapperConfiguration = new MapperConfiguration(
                    cfg =>
                        cfg.CreateMap<School, EdFiSchool>()
                            .ForMember(
                                dst => dst.EducationOrganizationCategories,
                                opt => opt.MapFrom<SchoolCategoryResolver>()));
                var mapper = mapperConfiguration.CreateMapper();

                // Act
                var result = mapper.Map(school, edfiSchool);

                // Assert
                result.ShouldNotBeNull();
                result.EducationOrganizationCategories.ShouldNotBeNull();
                result.EducationOrganizationCategories.Count.ShouldBeGreaterThan(0);
                result.EducationOrganizationCategories.First().EducationOrganizationCategoryDescriptor.ShouldBe(EdOrgCategoryDescriptor);
            }
        }
    }
}
