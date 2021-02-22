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
    public class AddSchoolModelTests
    {
        private Mock<IOdsApiFacade> _mockOdsApiFacade;
        private Mock<IOdsApiFacadeFactory> _mockOdsApiFacadeFactory;
        private AddSchoolModel _addSchoolModel;
        private const int Id = 1;

        [SetUp]
        public void Init()
        {
            _mockOdsApiFacadeFactory = new Mock<IOdsApiFacadeFactory>();
            _mockOdsApiFacade = new Mock<IOdsApiFacade>();
           
            _addSchoolModel = new AddSchoolModel
            {
                SchoolId = Id,
                Name = "TestSchool",
                StreetNumberName = "1209",
                City = "City",
                State = "State",
                ZipCode = "78979",
                GradeLevels = new List<string>
                    { "First Grade" }
            };
        }

        [Test]
        public void ShouldNotValidateAddSchoolModelIfSchoolIdIsEmpty()
        {
            _addSchoolModel.SchoolId = null;
            var validator = new AddSchoolModelValidator(_mockOdsApiFacadeFactory.Object);
            validator.ShouldNotValidate(_addSchoolModel, "'School ID' must not be empty.");
        }

        [Test]
        public void ShouldNotValidateAddSchoolModelIfGradeLevelsListIsEmpty()
        {
            // Arrange
            _addSchoolModel.GradeLevels = new List<string>();

            var existingLeaWithDifferentId = new LocalEducationAgency
            {
                EducationOrganizationId = 2
            };

            _mockOdsApiFacade.Setup(x => x.GetAllLocalEducationAgencies()).Returns(new List<LocalEducationAgency>
            {
                existingLeaWithDifferentId
            });

            var existingSchoolWithDifferentId = new School
            {
                EducationOrganizationId = 3
            };

            _mockOdsApiFacade.Setup(x => x.GetAllSchools()).Returns(new List<School>
            {
                existingSchoolWithDifferentId
            });

            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));

            var validator = new AddSchoolModelValidator(_mockOdsApiFacadeFactory.Object);
            validator.ShouldNotValidate(_addSchoolModel, "You must choose at least one grade level");
        }

        [Test]
        public void ShouldValidateAddSchoolModelWithValidValues()
        {
            // Arrange
            var existingLeaWithDifferentId = new LocalEducationAgency
            {
                EducationOrganizationId = 2
            };

            _mockOdsApiFacade.Setup(x => x.GetAllLocalEducationAgencies()).Returns(new List<LocalEducationAgency>
            {
                existingLeaWithDifferentId
            });

            var existingSchoolWithDifferentId = new School
            {
                EducationOrganizationId = 3
            };

            _mockOdsApiFacade.Setup(x => x.GetAllSchools()).Returns(new List<School>
            {
                existingSchoolWithDifferentId
            });

            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));

            var validator = new AddSchoolModelValidator(_mockOdsApiFacadeFactory.Object);
            validator.ShouldValidate(_addSchoolModel);
        }

        [Test]
        public void ShouldNotValidateAddSchoolModelIfPassedWithExistingLeaEdOrgId()
        {
            // Arrange
            var existingLeaWithSameId = new LocalEducationAgency
            {
                EducationOrganizationId = Id
            };

            _mockOdsApiFacade.Setup(x => x.GetAllLocalEducationAgencies()).Returns(new List<LocalEducationAgency>
            {
                existingLeaWithSameId
            });

            var existingSchoolWithDifferentId = new School
            {
                EducationOrganizationId = 2
            };

            _mockOdsApiFacade.Setup(x => x.GetAllSchools()).Returns(new List<School>
            {
                existingSchoolWithDifferentId
            });

            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));

            var validator = new AddSchoolModelValidator(_mockOdsApiFacadeFactory.Object);
            validator.ShouldNotValidate(_addSchoolModel, "This 'School ID' is already associated with another Education Organization. Please provide a unique value.");
        }

        [Test]
        public void ShouldNotValidateAddSchoolModelIfPassedWithExistingSchoolEdOrgId()
        {
            // Arrange
            var existingLeaWithDifferentId = new LocalEducationAgency
            {
                EducationOrganizationId = 2
            };

            _mockOdsApiFacade.Setup(x => x.GetAllLocalEducationAgencies()).Returns(new List<LocalEducationAgency>
            {
                existingLeaWithDifferentId
            });

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

            var validator = new AddSchoolModelValidator(_mockOdsApiFacadeFactory.Object);
            validator.ShouldNotValidate(_addSchoolModel, "This 'School ID' is already associated with another Education Organization. Please provide a unique value.");
        }
    }
}
