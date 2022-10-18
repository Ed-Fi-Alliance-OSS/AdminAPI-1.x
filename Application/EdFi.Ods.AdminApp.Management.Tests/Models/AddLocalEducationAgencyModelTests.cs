// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Management.Api.Models;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.EducationOrganizations;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests.Models
{
    [TestFixture]
    public class AddLocalEducationAgencyModelTests
    {
        private Mock<IOdsApiFacade> _mockOdsApiFacade;
        private AddLocalEducationAgencyModel _addLocalEducationAgencyModel;
        private const int Id = 1;

        [SetUp]
        public void Init()
        {
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
            var validator = new AddLocalEducationAgencyModelValidator();
            validator.ShouldNotValidate(_addLocalEducationAgencyModel, "'Local Education Organization ID' must not be empty.");
        }

        [Test]
        public void ShouldValidateAddLocalEducationAgencyModelWithValidValues()
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

            var validator = new AddLocalEducationAgencyModelValidator();
            validator.ShouldValidate(_addLocalEducationAgencyModel);

            EducationOrganizationValidationHelper
                .ProposedEducationOrganizationIdIsInUse(_addLocalEducationAgencyModel.LocalEducationAgencyId.Value, _mockOdsApiFacade.Object)
                .ShouldBeFalse();
        }

        [Test]
        public void ShouldNotValidateAddLocalEducationAgencyModelIfPassedWithExistingLeaEdOrgId()
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

            EducationOrganizationValidationHelper
                .ProposedEducationOrganizationIdIsInUse(_addLocalEducationAgencyModel.LocalEducationAgencyId.Value, _mockOdsApiFacade.Object)
                .ShouldBeTrue();
        }

        [Test]
        public void ShouldNotValidateAddLocalEducationAgencyModelIfPassedWithExistingSchoolEdOrgId()
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

            EducationOrganizationValidationHelper
                .ProposedEducationOrganizationIdIsInUse(_addLocalEducationAgencyModel.LocalEducationAgencyId.Value, _mockOdsApiFacade.Object)
                .ShouldBeTrue();
        }

        [Test]
        public void ShouldNotValidateAddLocalEducationAgencyModelIfPassedWithExistingPsiEdOrgId()
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

            EducationOrganizationValidationHelper
                .ProposedEducationOrganizationIdIsInUse(_addLocalEducationAgencyModel.LocalEducationAgencyId.Value, _mockOdsApiFacade.Object)
                .ShouldBeTrue();
        }
    }
}
