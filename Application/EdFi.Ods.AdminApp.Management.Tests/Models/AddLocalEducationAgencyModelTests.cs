// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Management.Api.Models;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.EducationOrganizations;
using Moq;
using NUnit.Framework;

namespace EdFi.Ods.AdminApp.Management.Tests.Models
{
    [TestFixture]
    public class AddLocalEducationAgencyModelTests
    {
        private Mock<IOdsApiFacade> _mockOdsApiFacade;
        private Mock<IOdsApiFacadeFactory> _mockOdsApiFacadeFactory;
        private AddLocalEducationAgencyModel _addLocalEducationAgencyModel;
        private const int Id = 1;

        [SetUp]
        public void Init()
        {
            _mockOdsApiFacadeFactory = new Mock<IOdsApiFacadeFactory>();
            _mockOdsApiFacade = new Mock<IOdsApiFacade>();

            _addLocalEducationAgencyModel = new AddLocalEducationAgencyModel
            {
                LocalEducationAgencyId = Id,
                Name = "TestSchool",
                StreetNumberName = "1209",
                City = "City",
                State = "State",
                ZipCode = "78979"
            };
        }

        [Test]
        public void ShouldNotValidateAddLocalEducationAgencyModelIfLocalEducationAgencyIdIsEmpty()
        {
            _addLocalEducationAgencyModel.LocalEducationAgencyId = null;
            var validator = new AddLocalEducationAgencyModelValidator(_mockOdsApiFacadeFactory.Object);
            validator.ShouldNotValidate(_addLocalEducationAgencyModel, "'Local Education Organization ID' must not be empty.");
        }

        [Test]
        public void ShouldValidateAddLocalEducationAgencyModelWithValidValues()
        {
            // Arrange
            var existingSchoolWithSameId = new School
            {
                EducationOrganizationId = 2
            };

            _mockOdsApiFacade.Setup(x => x.GetAllSchools()).Returns(new List<School>
            {
                existingSchoolWithSameId
            });

            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));

            var validator = new AddLocalEducationAgencyModelValidator(_mockOdsApiFacadeFactory.Object);
            validator.ShouldValidate(_addLocalEducationAgencyModel);
        }

        [Test]
        public void ShouldNotValidateAddLocalEducationAgencyModelIfPassedWithExistingEdOrgId()
        {
            // Arrange
            var existingSchoolWithSameId = new School
            {
                EducationOrganizationId = Id
            };

            _mockOdsApiFacade.Setup(x => x.GetAllSchools()).Returns(new List<School>
            {
                existingSchoolWithSameId
            });

            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));

            var validator = new AddLocalEducationAgencyModelValidator(_mockOdsApiFacadeFactory.Object);
            validator.ShouldNotValidate(_addLocalEducationAgencyModel, "Local Education Agency Id is already associated with different Education Organization. Please provide different value.");
        }
    }
}
