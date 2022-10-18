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
using EdFi.Ods.AdminApp.Management.Services;
using EdFi.Ods.AdminApp.Web.Controllers;
using EdFi.Ods.AdminApp.Web.Display.Pagination;
using EdFi.Ods.AdminApp.Web.Display.TabEnumeration;
using EdFi.Ods.AdminApp.Web.Models.ViewModels;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.EducationOrganizations;
using Moq;
using NUnit.Framework;
using Shouldly;
using System.Runtime.Caching;

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
        private IInferExtensionDetails _inferExtensionDetails;
        private Mock<IInferExtensionDetails> _mockInferExtensionDetails;
        private Mock<IOdsApiValidator> _mockOdsApiValidator;

        [SetUp]
        public void Init()
        {
            _mockMapper = new Mock<IMapper>();
            _mockOdsApiFacadeFactory = new Mock<IOdsApiFacadeFactory>();
            _mockOdsApiFacade = new Mock<IOdsApiFacade>();
            _mockInstanceContext = new Mock<InstanceContext>();
            _tabDisplayService = new Mock<ITabDisplayService>();
            _mockInferExtensionDetails = new Mock<IInferExtensionDetails>();
            _mockOdsApiValidator = new Mock<IOdsApiValidator>();
            _inferExtensionDetails = new InferExtensionDetails(new Mock<ISimpleGetRequest>().Object);
            MemoryCache.Default.Remove("TpdmExtensionVersion");
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
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object, _inferExtensionDetails, _mockOdsApiValidator.Object);

            // Act
            var result = _controller.AddLocalEducationAgency(addLocalEducationAgencyModel).Result as ContentResult;

            // Assert
            result.ShouldNotBeNull();
            result.Content.ShouldContain("Organization Added");
        }

        [Test]
        public void When_Perform_Post_Request_To_AddPostSecondaryInstitution_Return_Expected_Success_response()
        {
            // Arrange
            var addPostSecondaryInstitutionModel = new AddPostSecondaryInstitutionModel
            {
                City = "city"
            };

            _mockMapper.Setup(x => x.Map<PostSecondaryInstitution>(It.IsAny<AddPostSecondaryInstitutionModel>()))
                .Returns(new PostSecondaryInstitution());
            _mockOdsApiFacade.Setup(x => x.AddPostSecondaryInstitution(It.IsAny<PostSecondaryInstitution>())).Returns(new OdsApiResult());
            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));
            _controller =
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object, _inferExtensionDetails, _mockOdsApiValidator.Object);

            // Act
            var result = _controller.AddPostSecondaryInstitution(addPostSecondaryInstitutionModel).Result as ContentResult;

            // Assert
            result.ShouldNotBeNull();
            result.Content.ShouldContain("Post-Secondary Institution Added");
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
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object, _inferExtensionDetails, _mockOdsApiValidator.Object);

            // Act
            var result = _controller.AddSchool(addSchoolModel).Result as ContentResult;

            // Assert
            result.ShouldNotBeNull();
            result.Content.ShouldContain("School Added");
        }

        [Test]
        public void When_Perform_Post_Request_To_AddPsiSchool_Return_Expected_Success_response()
        {
            // Arrange
            var addPsiSchoolModel = new AddPsiSchoolModel
            {
                City = "city"
            };

            _mockMapper.Setup(x => x.Map<PsiSchool>(It.IsAny<AddPsiSchoolModel>()))
                .Returns(new PsiSchool());
            _mockOdsApiFacade.Setup(x => x.AddPsiSchool(It.IsAny<PsiSchool>())).Returns(new OdsApiResult());
            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));
            _controller =
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object, _inferExtensionDetails, _mockOdsApiValidator.Object);

            // Act
            var result = _controller.AddPsiSchool(addPsiSchoolModel).Result as ContentResult;

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
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object, _inferExtensionDetails, _mockOdsApiValidator.Object);

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
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object, _inferExtensionDetails, _mockOdsApiValidator.Object);

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
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object, _inferExtensionDetails, _mockOdsApiValidator.Object);

            // Act
            var result = _controller.EditLocalEducationAgency(editLocalEducationAgencyModel).Result as ContentResult;

            // Assert
            result.ShouldNotBeNull();
            result.Content.ShouldContain(error);
        }

        [Test]
        public void When_Perform_Post_Request_To_EditPostSecondaryInstitution_Return_Success_Response()
        {
            // Arrange
            var editPostSecondaryInstitutionModel = new EditPostSecondaryInstitutionModel
            {
                City = "city"
            };

            _mockMapper.Setup(x => x.Map<PostSecondaryInstitution>(It.IsAny<EditPostSecondaryInstitutionModel>()))
                .Returns(new PostSecondaryInstitution());
            _mockOdsApiFacade.Setup(x => x.EditPostSecondaryInstitution(It.IsAny<PostSecondaryInstitution>()))
                .Returns(new OdsApiResult());
            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));
            _controller =
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object, _inferExtensionDetails, _mockOdsApiValidator.Object);

            // Act
            var result = _controller.EditPostSecondaryInstitution(editPostSecondaryInstitutionModel).Result as ContentResult;

            // Assert
            result.ShouldNotBeNull();
            result.Content.ShouldContain("Post-Secondary Institution Updated");
        }

        [Test]
        public void When_Perform_Post_Request_To_EditPostSecondaryInstitution_Return_Error_Response()
        {
            // Arrange
            const string error = "error";
            var editPostSecondaryInstitutionModel = new EditPostSecondaryInstitutionModel
            {
                City = "city"
            };

            var apiResult = new OdsApiResult { ErrorMessage = error };

            _mockMapper.Setup(x => x.Map<PostSecondaryInstitution>(It.IsAny<EditPostSecondaryInstitutionModel>()))
                .Returns(new PostSecondaryInstitution());
            _mockOdsApiFacade.Setup(x => x.EditPostSecondaryInstitution(It.IsAny<PostSecondaryInstitution>()))
                .Returns(apiResult);
            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));
            _controller =
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object, _inferExtensionDetails, _mockOdsApiValidator.Object);

            // Act
            var result = _controller.EditPostSecondaryInstitution(editPostSecondaryInstitutionModel).Result as ContentResult;

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
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object, _inferExtensionDetails, _mockOdsApiValidator.Object);

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
        public void When_Perform_Get_Request_To_EditPsiSchoolModal_Return_PartialView_With_Expected_Model()
        {
            // Arrange
            const string gradeLevel = "FirstGrade";
            var value = "Namespace#FirstGrade";
            var schoolId = "id";
            var name = "school";
            const string accreditationStatus = "Test Accreditation Status";
            const string accreditationStatusValue = "Namespace#Test Accreditation Status";
            const string federalLocaleCode = "Test Federal Locale Code";
            const string federalLocaleCodeValue = "Namespace#Test Federal Locale Code";

            var editPsiSchoolModel = new EditPsiSchoolModel
            {
                Name = name
            };

            var tpdmVersionDetails = new TpdmExtensionDetails
            {
                TpdmVersion = "1.1.0", IsTpdmCommunityVersion = true
            };            
            _mockInferExtensionDetails.Setup(x => x.TpdmExtensionVersion(It.IsAny<string>())).Returns(Task.FromResult(tpdmVersionDetails));

            _mockOdsApiFacade.Setup(x => x.GetPsiSchoolById(schoolId))
                .Returns(new PsiSchool());
            _mockMapper.Setup(x => x.Map<EditPsiSchoolModel>(It.IsAny<PsiSchool>()))
                .Returns(editPsiSchoolModel);
            _mockOdsApiFacade.Setup(x => x.GetAllGradeLevels())
                .Returns(new List<SelectOptionModel> { new SelectOptionModel { DisplayText = gradeLevel, Value = value } });
            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));
            _mockOdsApiFacade.Setup(x => x.GetAccreditationStatusOptions())
                .Returns(new List<SelectOptionModel>
                    {new SelectOptionModel {DisplayText = accreditationStatus, Value = accreditationStatusValue}});
            _mockOdsApiFacade.Setup(x => x.GetFederalLocaleCodes())
                .Returns(new List<SelectOptionModel>
                    {new SelectOptionModel {DisplayText = federalLocaleCode, Value = federalLocaleCodeValue}});
            _controller =
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object, _mockInferExtensionDetails.Object, _mockOdsApiValidator.Object);

            // Act
            var result = _controller.EditPsiSchoolModal(schoolId).Result as PartialViewResult;

            // Assert
            result.ShouldNotBeNull();
            var model = (EditPsiSchoolModel)result.ViewData.Model;
            model.ShouldNotBeNull();
            model.GradeLevelOptions.Count.ShouldBeGreaterThan(0);
            model.GradeLevelOptions.First().DisplayText.ShouldBe(gradeLevel);
            model.GradeLevelOptions.First().Value.ShouldBe(value);
            model.Name.ShouldMatch(name);

            model.AccreditationStatusOptions.Count.ShouldBeGreaterThan(0);
            model.FederalLocaleCodeOptions.Count.ShouldBeGreaterThan(0);
            var accreditationStatusOption = model.AccreditationStatusOptions.Single(x => x.Value != null);
            accreditationStatusOption.DisplayText.ShouldBe(accreditationStatus);
            accreditationStatusOption.Value.ShouldBe(accreditationStatusValue);
            var federalLocaleCodeOption = model.FederalLocaleCodeOptions.Single(x => x.Value != null);
            federalLocaleCodeOption.DisplayText.ShouldBe(federalLocaleCode);
            federalLocaleCodeOption.Value.ShouldBe(federalLocaleCodeValue);
        }

        [Test]
        public void When_Perform_Get_Request_To_EditPsiSchoolModal_Return_PartialView_With_Expected_Model_On_TpdmCore()
        {
            // Arrange
            const string gradeLevel = "FirstGrade";
            var value = "Namespace#FirstGrade";
            var schoolId = "id";
            var name = "school";       

            var editPsiSchoolModel = new EditPsiSchoolModel
            {
                Name = name
            };

            var tpdmVersionDetails = new TpdmExtensionDetails()
            {
                TpdmVersion = "1.1.0", IsTpdmCommunityVersion = false
            };
            _mockInferExtensionDetails.Setup(x => x.TpdmExtensionVersion(It.IsAny<string>())).Returns(Task.FromResult(tpdmVersionDetails));

            _mockOdsApiFacade.Setup(x => x.GetPsiSchoolById(schoolId))
                .Returns(new PsiSchool());
            _mockMapper.Setup(x => x.Map<EditPsiSchoolModel>(It.IsAny<PsiSchool>()))
                .Returns(editPsiSchoolModel);
            _mockOdsApiFacade.Setup(x => x.GetAllGradeLevels())
                .Returns(new List<SelectOptionModel> { new SelectOptionModel { DisplayText = gradeLevel, Value = value } });
            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));
            _mockOdsApiFacade.Setup(x => x.GetAccreditationStatusOptions()).Returns<List<SelectOptionModel>>(null);
            _mockOdsApiFacade.Setup(x => x.GetFederalLocaleCodes()).Returns<List<SelectOptionModel>>(null);
            _controller =
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object, _mockInferExtensionDetails.Object, _mockOdsApiValidator.Object);

            // Act
            var result = _controller.EditPsiSchoolModal(schoolId).Result as PartialViewResult;

            // Assert
            result.ShouldNotBeNull();
            var model = (EditPsiSchoolModel)result.ViewData.Model;
            model.ShouldNotBeNull();
            model.GradeLevelOptions.Count.ShouldBeGreaterThan(0);
            model.GradeLevelOptions.First().DisplayText.ShouldBe(gradeLevel);
            model.GradeLevelOptions.First().Value.ShouldBe(value);
            model.Name.ShouldMatch(name);

            model.AccreditationStatusOptions.ShouldBeNull();
            model.FederalLocaleCodeOptions.ShouldBeNull();        
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
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object, _inferExtensionDetails, _mockOdsApiValidator.Object);

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
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object, _inferExtensionDetails, _mockOdsApiValidator.Object);

            // Act
            var result = _controller.EditSchool(editSchoolModel).Result as ContentResult;

            // Assert
            result.ShouldNotBeNull();
            result.Content.ShouldContain(error);
        }

        [Test]
        public void When_Perform_Post_Request_To_EditPsiSchool_Return_Success_Response()
        {
            // Arrange
            var editPsiSchoolModel = new EditPsiSchoolModel
            {
                City = "city"
            };

            _mockMapper.Setup(x => x.Map<PsiSchool>(It.IsAny<EditPsiSchoolModel>()))
                .Returns(new PsiSchool());
            _mockOdsApiFacade.Setup(x => x.EditPsiSchool(It.IsAny<PsiSchool>())).Returns(new OdsApiResult());
            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));
            _controller =
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object, _inferExtensionDetails, _mockOdsApiValidator.Object);

            // Act
            var result = _controller.EditPsiSchool(editPsiSchoolModel).Result as ContentResult;

            // Assert
            result.ShouldNotBeNull();
            result.Content.ShouldContain("School Updated");
        }

        [Test]
        public void When_Perform_Post_Request_To_EditPsiSchool_Return_Error_Response()
        {
            // Arrange
            var error = "error";
            var editPsiSchoolModel = new EditPsiSchoolModel
            {
                City = "city"
            };
            var apiResult = new OdsApiResult { ErrorMessage = error };

            _mockMapper.Setup(x => x.Map<PsiSchool>(It.IsAny<EditPsiSchoolModel>()))
                .Returns(new PsiSchool());
            _mockOdsApiFacade.Setup(x => x.EditPsiSchool(It.IsAny<PsiSchool>())).Returns(apiResult);
            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));
            _controller =
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object, _inferExtensionDetails, _mockOdsApiValidator.Object);

            // Act
            var result = _controller.EditPsiSchool(editPsiSchoolModel).Result as ContentResult;

            // Assert
            result.ShouldNotBeNull();
            result.Content.ShouldContain(error);
        }

        [Test]
        public void When_Perform_Get_Request_To_LocalEducationAgencyList_Return_Education_Organization_List()
        {
            // Arrange
            var lea = new LocalEducationAgency();

            var schools = new List<School>
            {
                new School()
            };

            var leas = new List<LocalEducationAgency>
            {
                lea
            };

            var psis = new List<PostSecondaryInstitution>
            {
                new PostSecondaryInstitution()
            };
            const string gradeLevel = "FirstGrade";
            const string value = "Namespace#FirstGrade";
            const string localEducationAgencyCategory = "School";
            const string localEducationAgencyCategoryValue = "Namespace#School";

            _mockOdsApiFacade.Setup(x => x.GetSchoolsByLeaIds(new List<int>(){lea.EducationOrganizationId})).Returns(schools);
            _mockOdsApiFacade.Setup(x => x.GetLocalEducationAgenciesByPage(0, Page<LocalEducationAgency>.DefaultPageSize + 1)).Returns(leas);
            _mockOdsApiFacade.Setup(x => x.GetPostSecondaryInstitutionsByPage(0, Page<PostSecondaryInstitution>.DefaultPageSize + 1)).Returns(psis);
            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));
            _mockOdsApiFacade.Setup(x => x.GetAllGradeLevels())
                .Returns(new List<SelectOptionModel> { new SelectOptionModel { DisplayText = gradeLevel, Value = value } });
            _mockOdsApiFacade.Setup(x => x.GetLocalEducationAgencyCategories())
                .Returns(new List<SelectOptionModel>
                    {new SelectOptionModel {DisplayText = localEducationAgencyCategory, Value = localEducationAgencyCategoryValue}});
            _controller =
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object, _inferExtensionDetails, _mockOdsApiValidator.Object);

            // Act
            var result = _controller.LocalEducationAgencyList(1).Result as PartialViewResult;

            // Assert
            result.ShouldNotBeNull();
            var model = (LocalEducationAgencyViewModel)result.ViewData.Model;
            model.ShouldNotBeNull();
            model.Schools.Count.ShouldBeGreaterThan(0);
            model.LocalEducationAgencies.Items.Count().ShouldBeGreaterThan(0);

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
        public void When_Perform_Get_Request_To_PostSecondaryInstitutionsList_Return_Post_Secondary_Institutions_List()
        {
            // Arrange
            var schools = new List<PsiSchool>
            {
                new PsiSchool()
            };

            var leas = new List<LocalEducationAgency>
            {
                new LocalEducationAgency()
            };

            var psis = new List<PostSecondaryInstitution>
            {
                new PostSecondaryInstitution()
            };
            const string gradeLevel = "FirstGrade";
            const string value = "Namespace#FirstGrade";
            const string postSecondaryInstitutionLevel = "Four or more years";
            const string postSecondaryInstitutionLevelValue = "Namespace#Four or more years";
            const string administrativeFundingControl = "Private School";
            const string administrativeFundingControlValue = "Namespace#Private School";
            const string accreditationStatus = "Test Accreditation Status";
            const string accreditationStatusValue = "Namespace#Test Accreditation Status";
            const string federalLocaleCode = "Test Federal Locale Code";
            const string federalLocaleCodeValue = "Namespace#Test Federal Locale Code";

            var tpdmVersionDetails = new TpdmExtensionDetails()
            {
                TpdmVersion = "1.1.0", IsTpdmCommunityVersion = true
            };          
            _mockInferExtensionDetails.Setup(x => x.TpdmExtensionVersion(It.IsAny<string>())).Returns(Task.FromResult(tpdmVersionDetails));

            _mockOdsApiFacade.Setup(x => x.GetAllPsiSchools()).Returns(schools);
            _mockOdsApiFacade.Setup(x => x.GetLocalEducationAgenciesByPage(0, Page<LocalEducationAgency>.DefaultPageSize + 1)).Returns(leas);
            _mockOdsApiFacade.Setup(x => x.GetPostSecondaryInstitutionsByPage(0, Page<PostSecondaryInstitution>.DefaultPageSize + 1)).Returns(psis);
            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));
            _mockOdsApiFacade.Setup(x => x.GetAllGradeLevels())
                .Returns(new List<SelectOptionModel> { new SelectOptionModel { DisplayText = gradeLevel, Value = value } });
            _mockOdsApiFacade.Setup(x => x.GetPostSecondaryInstitutionLevels())
                .Returns(new List<SelectOptionModel>
                    {new SelectOptionModel {DisplayText = postSecondaryInstitutionLevel, Value = postSecondaryInstitutionLevelValue}});
            _mockOdsApiFacade.Setup(x => x.GetAdministrativeFundingControls())
                .Returns(new List<SelectOptionModel>
                    {new SelectOptionModel {DisplayText = administrativeFundingControl, Value = administrativeFundingControlValue}});
            _mockOdsApiFacade.Setup(x => x.GetAccreditationStatusOptions())
                .Returns(new List<SelectOptionModel>
                    {new SelectOptionModel {DisplayText = accreditationStatus, Value = accreditationStatusValue}});
            _mockOdsApiFacade.Setup(x => x.GetFederalLocaleCodes())
                .Returns(new List<SelectOptionModel>
                    {new SelectOptionModel {DisplayText = federalLocaleCode, Value = federalLocaleCodeValue}});
            _controller =
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object, _mockInferExtensionDetails.Object, _mockOdsApiValidator.Object);

            // Act
            var result = _controller.PostSecondaryInstitutionsList(1).Result as PartialViewResult;

            // Assert
            result.ShouldNotBeNull();
            var model = (PostSecondaryInstitutionViewModel)result.ViewData.Model;
            model.ShouldNotBeNull();
            model.Schools.Count.ShouldBeGreaterThan(0);
            model.PostSecondaryInstitutions.Items.Count().ShouldBeGreaterThan(0);

            var addSchoolModel = model.AddPsiSchoolModel;
            addSchoolModel.ShouldNotBeNull();
            addSchoolModel.GradeLevelOptions.Count.ShouldBe(1);
            addSchoolModel.GradeLevelOptions.Single().DisplayText.ShouldBe(gradeLevel);
            addSchoolModel.GradeLevelOptions.Single().Value.ShouldBe(value);

            var addPostSecondaryInstitutionModel = model.AddPostSecondaryInstitutionModel;
            addPostSecondaryInstitutionModel.ShouldNotBeNull();
            addPostSecondaryInstitutionModel.PostSecondaryInstitutionLevelOptions.Count.ShouldBeGreaterThan(0);
            addPostSecondaryInstitutionModel.AdministrativeFundingControlOptions.Count.ShouldBeGreaterThan(0);
            var postSecondaryInstitutionLevelOption = addPostSecondaryInstitutionModel.PostSecondaryInstitutionLevelOptions.Single(x => x.Value != null);
            postSecondaryInstitutionLevelOption.DisplayText.ShouldBe(postSecondaryInstitutionLevel);
            postSecondaryInstitutionLevelOption.Value.ShouldBe(postSecondaryInstitutionLevelValue);
            var administrativeFundingControlOption = addPostSecondaryInstitutionModel.AdministrativeFundingControlOptions.Single(x => x.Value != null);
            administrativeFundingControlOption.DisplayText.ShouldBe(administrativeFundingControl);
            administrativeFundingControlOption.Value.ShouldBe(administrativeFundingControlValue);
        }

        [Test]
        public void When_Perform_Get_Request_To_PostSecondaryInstitutionsList_Return_Post_Secondary_Institutions_List_On_TpdmCore()
        {
            // Arrange
            var schools = new List<PsiSchool>
            {
                new PsiSchool()
            };

            var leas = new List<LocalEducationAgency>
            {
                new LocalEducationAgency()
            };

            var psis = new List<PostSecondaryInstitution>
            {
                new PostSecondaryInstitution()
            };
            const string gradeLevel = "FirstGrade";
            const string value = "Namespace#FirstGrade";
            const string postSecondaryInstitutionLevel = "Four or more years";
            const string postSecondaryInstitutionLevelValue = "Namespace#Four or more years";
            const string administrativeFundingControl = "Private School";
            const string administrativeFundingControlValue = "Namespace#Private School";          

            var tpdmVersionDetails = new TpdmExtensionDetails()
            {
                TpdmVersion = "1.1.0", IsTpdmCommunityVersion = false
            };
            _mockInferExtensionDetails.Setup(x => x.TpdmExtensionVersion(It.IsAny<string>())).Returns(Task.FromResult(tpdmVersionDetails));

            _mockOdsApiFacade.Setup(x => x.GetAllPsiSchools()).Returns(schools);
            _mockOdsApiFacade.Setup(x => x.GetLocalEducationAgenciesByPage(0, Page<LocalEducationAgency>.DefaultPageSize + 1)).Returns(leas);
            _mockOdsApiFacade.Setup(x => x.GetPostSecondaryInstitutionsByPage(0, Page<PostSecondaryInstitution>.DefaultPageSize + 1)).Returns(psis);
            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));
            _mockOdsApiFacade.Setup(x => x.GetAllGradeLevels())
                .Returns(new List<SelectOptionModel> { new SelectOptionModel { DisplayText = gradeLevel, Value = value } });
            _mockOdsApiFacade.Setup(x => x.GetPostSecondaryInstitutionLevels())
                .Returns(new List<SelectOptionModel>
                    {new SelectOptionModel {DisplayText = postSecondaryInstitutionLevel, Value = postSecondaryInstitutionLevelValue}});
            _mockOdsApiFacade.Setup(x => x.GetAdministrativeFundingControls())
                .Returns(new List<SelectOptionModel>
                    {new SelectOptionModel {DisplayText = administrativeFundingControl, Value = administrativeFundingControlValue}});         
            _controller =
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object, _mockInferExtensionDetails.Object, _mockOdsApiValidator.Object);

            // Act
            var result = _controller.PostSecondaryInstitutionsList(1).Result as PartialViewResult;

            // Assert
            result.ShouldNotBeNull();
            var model = (PostSecondaryInstitutionViewModel)result.ViewData.Model;
            model.ShouldNotBeNull();
            model.Schools.Count.ShouldBeGreaterThan(0);
            model.PostSecondaryInstitutions.Items.Count().ShouldBeGreaterThan(0);

            var addSchoolModel = model.AddPsiSchoolModel;
            addSchoolModel.ShouldNotBeNull();
            addSchoolModel.GradeLevelOptions.Count.ShouldBe(1);
            addSchoolModel.GradeLevelOptions.Single().DisplayText.ShouldBe(gradeLevel);
            addSchoolModel.GradeLevelOptions.Single().Value.ShouldBe(value);

            var addPostSecondaryInstitutionModel = model.AddPostSecondaryInstitutionModel;
            addPostSecondaryInstitutionModel.ShouldNotBeNull();
            addPostSecondaryInstitutionModel.PostSecondaryInstitutionLevelOptions.Count.ShouldBeGreaterThan(0);
            addPostSecondaryInstitutionModel.AdministrativeFundingControlOptions.Count.ShouldBeGreaterThan(0);
            var postSecondaryInstitutionLevelOption = addPostSecondaryInstitutionModel.PostSecondaryInstitutionLevelOptions.Single(x => x.Value != null);
            postSecondaryInstitutionLevelOption.DisplayText.ShouldBe(postSecondaryInstitutionLevel);
            postSecondaryInstitutionLevelOption.Value.ShouldBe(postSecondaryInstitutionLevelValue);
            var administrativeFundingControlOption = addPostSecondaryInstitutionModel.AdministrativeFundingControlOptions.Single(x => x.Value != null);
            administrativeFundingControlOption.DisplayText.ShouldBe(administrativeFundingControl);
            administrativeFundingControlOption.Value.ShouldBe(administrativeFundingControlValue);
        }

        [Test]
        public void When_Perform_Post_Request_To_DeletePostSecondaryInstitution_Return_Success_Response()
        {
            // Arrange
            var deletePostSecondaryInstitutionModel = new DeleteEducationOrganizationModel
            {
                Id = "id"
            };
        
            _mockOdsApiFacade.Setup(x => x.DeletePostSecondaryInstitution(It.IsAny<string>()))
                .Returns(new OdsApiResult());
            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));
            _controller =
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object, _inferExtensionDetails, _mockOdsApiValidator.Object);

            // Act
            var result = _controller.DeletePostSecondaryInstitution(deletePostSecondaryInstitutionModel).Result as ContentResult;

            // Assert
            result.ShouldNotBeNull();
            result.Content.ShouldContain("Post-Secondary Institution Removed");
        }

        [Test]
        public void When_Perform_Post_Request_To_DeletePostSecondaryInstitution_Return_Error_Response()
        {
            // Arrange
            var error = "error";
            var deletePostSecondaryInstitutionModel = new DeleteEducationOrganizationModel
            {
                Id = "id"
            };
            var apiResult = new OdsApiResult { ErrorMessage = error };
         
            _mockOdsApiFacade.Setup(x => x.DeletePostSecondaryInstitution(It.IsAny<string>()))
                .Returns(apiResult);
            _mockOdsApiFacadeFactory.Setup(x => x.Create())
                .Returns(Task.FromResult(_mockOdsApiFacade.Object));
            _controller =
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object, _inferExtensionDetails, _mockOdsApiValidator.Object);

            // Act
            var result = _controller.DeletePostSecondaryInstitution(deletePostSecondaryInstitutionModel).Result as ContentResult;

            // Assert
            result.ShouldNotBeNull();
            result.Content.ShouldContain(error);
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
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object, _inferExtensionDetails, _mockOdsApiValidator.Object);

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
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object, _inferExtensionDetails, _mockOdsApiValidator.Object);

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
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object, _inferExtensionDetails, _mockOdsApiValidator.Object);

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
                new EducationOrganizationsController(_mockOdsApiFacadeFactory.Object, _mockMapper.Object, _mockInstanceContext.Object, _tabDisplayService.Object, _inferExtensionDetails, _mockOdsApiValidator.Object);

            // Act
            var result = _controller.DeleteSchool(deleteLocalEducationAgencyModel).Result as ContentResult;

            // Assert
            result.ShouldNotBeNull();
            result.Content.ShouldContain(error);
        }
    }
}
