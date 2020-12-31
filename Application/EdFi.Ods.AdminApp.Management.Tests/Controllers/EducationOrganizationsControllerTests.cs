// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Management.Api.Models;
using EdFi.Ods.AdminApp.Web.Controllers;
using EdFi.Ods.AdminApp.Web.Display.TabEnumeration;
using EdFi.Ods.AdminApp.Web.Models.ViewModels;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.EducationOrganizations;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests.Controllers
{
    [TestFixture]
    public class EducationOrganizationsControllerTests
    {
        private Mock<IOdsApiFacadeFactory> _mockOdsApiFacadeFactory;
        private Mock<IMapper> _mockMapper;
        private EducationOrganizationsController _controller;
        private Mock<IOdsApiFacade> _mockOdsApiFacade;
        private Mock<InstanceContext> _mockInstanceContext;
        private Mock<ITabDisplayService> _tabDisplayService;

        [SetUp]
        public void Init()
        {
            _mockMapper = new Mock<IMapper>();
            _mockOdsApiFacadeFactory = new Mock<IOdsApiFacadeFactory>();
            _mockOdsApiFacade = new Mock<IOdsApiFacade>();
            _mockInstanceContext = new Mock<InstanceContext>();
            _tabDisplayService = new Mock<ITabDisplayService>();
        }

        [Test]
        public void When_Perform_Post_Request_To_AddLocalEducationAgency_Return_Expected_Success_response()
        {
            // Arrange
            var addLocalEducationAgencyModel = new AddLocalEducationAgencyModel
            {
                City = "city"
            };
           
            _mockMapper.Setup(x => x.Map<LocalEducationAgency>(It.IsAny<AddLocalEducationAgencyModel>()))
                .Returns(new LocalEducationAgency());
            _mockOdsApiFacade.Setup(x => x.AddLocalEducationAgency(It.IsAny<LocalEducationAgency>())).Returns(new OdsApiResult());
            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));
            _controller =
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object);

            // Act
            var result = _controller.AddLocalEducationAgency(addLocalEducationAgencyModel).Result as ContentResult;

            // Assert
            result.ShouldNotBeNull();
            result.Content.ShouldContain("Organization Added");
        }

        [Test]
        public void When_Perform_Post_Request_To_AddSchool_Return_Expected_Success_response()
        {
            // Arrange
            var addSchoolModel = new AddSchoolModel
            {
                City = "city"
            };
         
            _mockMapper.Setup(x => x.Map<School>(It.IsAny<AddSchoolModel>()))
                .Returns(new School());
            _mockOdsApiFacade.Setup(x => x.AddSchool(It.IsAny<School>())).Returns(new OdsApiResult());
            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));
            _controller =
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object);

            // Act
            var result = _controller.AddSchool(addSchoolModel).Result as ContentResult;

            // Assert
            result.ShouldNotBeNull();
            result.Content.ShouldContain("School Added");
        }

        [Test]
        public void When_Perform_Get_Request_To_EditLocalEducationAgencyModal_Return_PartialView_With_Expected_Model()
        {
            // Arrange
            const string localEducationAgencyCategory = "School";
            const string localEducationAgencyCategoryValue = "Namespace#School";
            const string localEducationAgencyId = "id";
            const string name = "testSchool";

            var editLocalEducationAgencyModel = new EditLocalEducationAgencyModel
            {
                Name = name
            };

            _mockOdsApiFacade.Setup(x => x.GetLocalEducationAgencyById(localEducationAgencyId))
                .Returns(new LocalEducationAgency());
            _mockOdsApiFacade.Setup(x => x.GetLocalEducationAgencyCategories())
                .Returns(new List<SelectOptionModel>
                    {new SelectOptionModel {DisplayText = localEducationAgencyCategory, Value = localEducationAgencyCategoryValue}});
            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));
            _mockMapper.Setup(x => x.Map<EditLocalEducationAgencyModel>(It.IsAny<LocalEducationAgency>()))
                .Returns(editLocalEducationAgencyModel);
            _controller =
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object);

            // Act
            var result =
                _controller.EditLocalEducationAgencyModal(localEducationAgencyId).Result as
                    PartialViewResult;

            // Assert
            result.ShouldNotBeNull();
            var model = (EditLocalEducationAgencyModel) result.ViewData.Model;
            model.ShouldNotBeNull();
            model.LocalEducationAgencyCategoryTypeOptions.Count.ShouldBeGreaterThan(0);
            model.LocalEducationAgencyCategoryTypeOptions.First().DisplayText.ShouldBe(localEducationAgencyCategory);
            model.LocalEducationAgencyCategoryTypeOptions.First().Value.ShouldBe(localEducationAgencyCategoryValue);
            model.Name.ShouldMatch(name);
        }

        [Test]
        public void When_Perform_Post_Request_To_EditLocalEducationAgency_Return_Success_Response()
        {
            // Arrange
            var editLocalEducationAgencyModel = new EditLocalEducationAgencyModel
            {
                City = "city"
            };
          
            _mockMapper.Setup(x => x.Map<LocalEducationAgency>(It.IsAny<EditLocalEducationAgencyModel>()))
                .Returns(new LocalEducationAgency());
            _mockOdsApiFacade.Setup(x => x.EditLocalEducationAgency(It.IsAny<LocalEducationAgency>()))
                .Returns(new OdsApiResult());
            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));
            _controller =
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object);

            // Act
            var result = _controller.EditLocalEducationAgency(editLocalEducationAgencyModel).Result as ContentResult;

            // Assert
            result.ShouldNotBeNull();
            result.Content.ShouldContain("Organization Updated");
        }

        [Test]
        public void When_Perform_Post_Request_To_EditLocalEducationAgency_Return_Error_Response()
        {
            // Arrange
            const string error = "error";
            var editLocalEducationAgencyModel = new EditLocalEducationAgencyModel
            {
                City = "city"
            };

            var apiResult = new OdsApiResult {ErrorMessage = error};
            
            _mockMapper.Setup(x => x.Map<LocalEducationAgency>(It.IsAny<EditLocalEducationAgencyModel>()))
                .Returns(new LocalEducationAgency());
            _mockOdsApiFacade.Setup(x => x.EditLocalEducationAgency(It.IsAny<LocalEducationAgency>()))
                .Returns(apiResult);
            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));
            _controller =
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object);

            // Act
            var result = _controller.EditLocalEducationAgency(editLocalEducationAgencyModel).Result as ContentResult;

            // Assert
            result.ShouldNotBeNull();
            result.Content.ShouldContain(error);
        }

        [Test]
        public void When_Perform_Get_Request_To_EditSchoolModal_Return_PartialView_With_Expected_Model()
        {
            // Arrange
            const string gradeLevel = "FirstGrade";
            var value = "Namespace#FirstGrade";
            var schoolId = "id";
            var name = "school";

            var editSchoolModel = new EditSchoolModel
            {
                Name = name
            };
         
            _mockOdsApiFacade.Setup(x => x.GetSchoolById(schoolId))
                .Returns(new School());
            _mockMapper.Setup(x => x.Map<EditSchoolModel>(It.IsAny<School>()))
                .Returns(editSchoolModel);
            _mockOdsApiFacade.Setup(x => x.GetAllGradeLevels())
                .Returns(new List<SelectOptionModel> { new SelectOptionModel { DisplayText = gradeLevel, Value = value } });
            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));
            _controller =
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object);

            // Act
            var result = _controller.EditSchoolModal(schoolId).Result as PartialViewResult;

            // Assert
            result.ShouldNotBeNull();
            var model = (EditSchoolModel) result.ViewData.Model;
            model.ShouldNotBeNull();
            model.GradeLevelOptions.Count.ShouldBeGreaterThan(0);
            model.GradeLevelOptions.First().DisplayText.ShouldBe(gradeLevel);
            model.GradeLevelOptions.First().Value.ShouldBe(value);
            model.Name.ShouldMatch(name);
        }

        [Test]
        public void When_Perform_Post_Request_To_EditSchool_Return_Success_Response()
        {
            // Arrange
            var editSchoolModel = new EditSchoolModel
            {
                City = "city"
            };
         
            _mockMapper.Setup(x => x.Map<School>(It.IsAny<EditSchoolModel>()))
                .Returns(new School());
            _mockOdsApiFacade.Setup(x => x.EditSchool(It.IsAny<School>())).Returns(new OdsApiResult());
            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));
            _controller =
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object);

            // Act
            var result = _controller.EditSchool(editSchoolModel).Result as ContentResult;

            // Assert
            result.ShouldNotBeNull();
            result.Content.ShouldContain("School Updated");
        }

        [Test]
        public void When_Perform_Post_Request_To_EditSchool_Return_Error_Response()
        {
            // Arrange
            var error = "error";
            var editSchoolModel = new EditSchoolModel
            {
                City = "city"
            };
            var apiResult = new OdsApiResult {ErrorMessage = error};
         
            _mockMapper.Setup(x => x.Map<School>(It.IsAny<EditSchoolModel>()))
                .Returns(new School());
            _mockOdsApiFacade.Setup(x => x.EditSchool(It.IsAny<School>())).Returns(apiResult);
            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));
            _controller =
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object);

            // Act
            var result = _controller.EditSchool(editSchoolModel).Result as ContentResult;

            // Assert
            result.ShouldNotBeNull();
            result.Content.ShouldContain(error);
        }

        [Test]
        public void When_Perform_Get_Request_To_EducationOrganizationList_Return_Education_Organization_List()
        {
            // Arrange
            var schools = new List<School>
            {
                new School()
            };

            var leas = new List<LocalEducationAgency>
            {
                new LocalEducationAgency()
            };
            const string gradeLevel = "FirstGrade";
            const string value = "Namespace#FirstGrade";
            const string localEducationAgencyCategory = "School";
            const string localEducationAgencyCategoryValue = "Namespace#School";

            _mockOdsApiFacade.Setup(x => x.GetAllSchools()).Returns(schools);
            _mockOdsApiFacade.Setup(x => x.GetAllLocalEducationAgencies()).Returns(leas);
            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));
            _mockOdsApiFacade.Setup(x => x.GetAllGradeLevels())
                .Returns(new List<SelectOptionModel> { new SelectOptionModel { DisplayText = gradeLevel, Value = value } });
            _mockOdsApiFacade.Setup(x => x.GetLocalEducationAgencyCategories())
                .Returns(new List<SelectOptionModel>
                    {new SelectOptionModel {DisplayText = localEducationAgencyCategory, Value = localEducationAgencyCategoryValue}});
            _controller =
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object);

            // Act
            var result = _controller.EducationOrganizationList().Result as PartialViewResult;

            // Assert
            result.ShouldNotBeNull();
            var model = (EducationOrganizationViewModel)result.ViewData.Model;
            model.ShouldNotBeNull();
            model.Schools.Count.ShouldBeGreaterThan(0);
            model.LocalEducationAgencies.Count.ShouldBeGreaterThan(0);

            var addSchoolModel = model.AddSchoolModel;
            addSchoolModel.ShouldNotBeNull();
            addSchoolModel.GradeLevelOptions.Count.ShouldBe(1);
            addSchoolModel.GradeLevelOptions.Single().DisplayText.ShouldBe(gradeLevel);
            addSchoolModel.GradeLevelOptions.Single().Value.ShouldBe(value);

            var addLocalEducationAgencyModel = model.AddLocalEducationAgencyModel;
            addLocalEducationAgencyModel.ShouldNotBeNull();
            addLocalEducationAgencyModel.LocalEducationAgencyCategoryTypeOptions.Count.ShouldBe(1);
            addLocalEducationAgencyModel.LocalEducationAgencyCategoryTypeOptions.Single().DisplayText.ShouldBe(localEducationAgencyCategory);
            addLocalEducationAgencyModel.LocalEducationAgencyCategoryTypeOptions.Single().Value.ShouldBe(localEducationAgencyCategoryValue);
        }

        [Test]
        public void When_Perform_Post_Request_To_DeleteLocalEducationAgency_Return_Success_Response()
        {
            // Arrange
            var deleteLocalEducationAgencyModel = new DeleteEducationOrganizationModel
            {
                Id = "id"
            };
        
            _mockOdsApiFacade.Setup(x => x.DeleteLocalEducationAgency(It.IsAny<string>()))
                .Returns(new OdsApiResult());
            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));
            _controller =
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object);

            // Act
            var result = _controller.DeleteLocalEducationAgency(deleteLocalEducationAgencyModel).Result as ContentResult;

            // Assert
            result.ShouldNotBeNull();
            result.Content.ShouldContain("Organization Removed");
        }

        [Test]
        public void When_Perform_Post_Request_To_DeleteLocalEducationAgency_Return_Error_Response()
        {
            // Arrange
            var error = "error";
            var deleteLocalEducationAgencyModel = new DeleteEducationOrganizationModel
            {
                Id = "id"
            };
            var apiResult = new OdsApiResult { ErrorMessage = error };
         
            _mockOdsApiFacade.Setup(x => x.DeleteLocalEducationAgency(It.IsAny<string>()))
                .Returns(apiResult);
            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));
            _controller =
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object);

            // Act
            var result = _controller.DeleteLocalEducationAgency(deleteLocalEducationAgencyModel).Result as ContentResult;

            // Assert
            result.ShouldNotBeNull();
            result.Content.ShouldContain(error);
        }

        [Test]
        public void When_Perform_Post_Request_To_DeleteSchool_Return_Success_Response()
        {
            // Arrange
            var deleteLocalEducationAgencyModel = new DeleteEducationOrganizationModel
            {
                Id = "id"
            };

            _mockOdsApiFacade.Setup(x => x.DeleteSchool(It.IsAny<string>()))
                .Returns(new OdsApiResult());
            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));
            _controller =
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object);

            // Act
            var result = _controller.DeleteSchool(deleteLocalEducationAgencyModel).Result as ContentResult;

            // Assert
            result.ShouldNotBeNull();
            result.Content.ShouldContain("School Removed");
        }

        [Test]
        public void When_Perform_Post_Request_To_DeleteSchool_Return_Error_Response()
        {
            // Arrange
            var error = "error";
            var deleteLocalEducationAgencyModel = new DeleteEducationOrganizationModel
            {
                Id = "id"
            };
            var apiResult = new OdsApiResult { ErrorMessage = error };

            _mockOdsApiFacade.Setup(x => x.DeleteSchool(It.IsAny<string>()))
                .Returns(apiResult);
            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));
            _controller =
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object);

            // Act
            var result = _controller.DeleteSchool(deleteLocalEducationAgencyModel).Result as ContentResult;

            // Assert
            result.ShouldNotBeNull();
            result.Content.ShouldContain(error);
        }
    }
}
