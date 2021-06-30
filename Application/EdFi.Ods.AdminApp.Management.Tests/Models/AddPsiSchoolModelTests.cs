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
    public class AddPsiSchoolModelTests
    {
        private Mock<IOdsApiFacade> _mockOdsApiFacade;
        private Mock<IOdsApiFacadeFactory> _mockOdsApiFacadeFactory;
        private AddPsiSchoolModel _addPsiSchoolModel;
        private const int Id = 1;

        [SetUp]
        public void Init()
        {
            _mockOdsApiFacadeFactory = new Mock<IOdsApiFacadeFactory>();
            _mockOdsApiFacade = new Mock<IOdsApiFacade>();
           
            _addPsiSchoolModel = new AddPsiSchoolModel
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
            _addPsiSchoolModel.SchoolId = null;
            var validator = new AddSchoolModelValidator<AddSchoolModel>(_mockOdsApiFacadeFactory.Object);
            validator.ShouldNotValidate(_addPsiSchoolModel, "'School ID' must not be empty.");
        }

        [Test]
        public void ShouldNotValidateAddSchoolModelIfGradeLevelsListIsEmpty()
        {
            // Arrange
            _addPsiSchoolModel.GradeLevels = new List<string>();

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

            var existingPsiWithDifferentId = new PostSecondaryInstitution
            {
                EducationOrganizationId = 4
            };

            _mockOdsApiFacade.Setup(x => x.GetAllPostSecondaryInstitutions()).Returns(new List<PostSecondaryInstitution>
            {
                existingPsiWithDifferentId
            });

            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));

            var validator = new AddSchoolModelValidator<AddSchoolModel>(_mockOdsApiFacadeFactory.Object);
            validator.ShouldNotValidate(_addPsiSchoolModel, "You must choose at least one grade level");
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

            var existingPsiWithDifferentId = new PostSecondaryInstitution
            {
                EducationOrganizationId = 4
            };

            _mockOdsApiFacade.Setup(x => x.GetAllPostSecondaryInstitutions()).Returns(new List<PostSecondaryInstitution>
            {
                existingPsiWithDifferentId
            });

            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));

            var validator = new AddSchoolModelValidator<AddSchoolModel>(_mockOdsApiFacadeFactory.Object);
            validator.ShouldValidate(_addPsiSchoolModel);
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

            var existingPsiWithDifferentId = new PostSecondaryInstitution
            {
                EducationOrganizationId = 4
            };

            _mockOdsApiFacade.Setup(x => x.GetAllPostSecondaryInstitutions()).Returns(new List<PostSecondaryInstitution>
            {
                existingPsiWithDifferentId
            });

            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));

            var validator = new AddSchoolModelValidator<AddSchoolModel>(_mockOdsApiFacadeFactory.Object);
            validator.ShouldNotValidate(_addPsiSchoolModel, "This 'School ID' is already associated with another Education Organization. Please provide a unique value.");
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

            var existingPsiWithDifferentId = new PostSecondaryInstitution
            {
                EducationOrganizationId = 4
            };

            _mockOdsApiFacade.Setup(x => x.GetAllPostSecondaryInstitutions()).Returns(new List<PostSecondaryInstitution>
            {
                existingPsiWithDifferentId
            });

            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));

            var validator = new AddSchoolModelValidator<AddSchoolModel>(_mockOdsApiFacadeFactory.Object);
            validator.ShouldNotValidate(_addPsiSchoolModel, "This 'School ID' is already associated with another Education Organization. Please provide a unique value.");
        }

        [Test]
        public void ShouldNotValidateAddSchoolModelIfPassedWithExistingPsiEdOrgId()
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

            var existingPsiWithSameId = new PostSecondaryInstitution
            {
                EducationOrganizationId = Id
            };

            _mockOdsApiFacade.Setup(x => x.GetAllPostSecondaryInstitutions()).Returns(new List<PostSecondaryInstitution>
            {
                existingPsiWithSameId
            });

            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));

            var validator = new AddSchoolModelValidator<AddSchoolModel>(_mockOdsApiFacadeFactory.Object);
            validator.ShouldNotValidate(_addPsiSchoolModel, "This 'School ID' is already associated with another Education Organization. Please provide a unique value.");
        }
    }
}
