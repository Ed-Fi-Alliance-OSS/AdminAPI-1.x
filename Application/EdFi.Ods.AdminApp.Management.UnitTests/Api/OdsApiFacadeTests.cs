// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using AutoMapper;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Management.Api.Automapper;
using EdFi.Ods.AdminApp.Management.Api.Common;
using EdFi.Ods.AdminApp.Management.Api.DomainModels;
using EdFi.Ods.AdminApp.Management.Api.Models;
using EdFi.Ods.AdminApp.Management.ErrorHandling;
using EdFi.Ods.AdminApp.Management.Instances;
using NUnit.Framework;
using Moq;
using RestSharp;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.UnitTests.Api
{
    [TestFixture]
    public class OdsApiFacadeTests
    {
        private AdminManagementMappingProfile _profile;
        private MapperConfiguration _configuration;
        private IMapper _mapper;
        private OdsApiFacade _facade;
        private OdsApiConnectionInformation _connectionInformation;
        private School _school;
        private const string SchoolId = "id";
        private const string SchoolName = "TestSchool";
        private const int LocalEducationAgencyId = 123;

        [SetUp]
        public void Init()
        {
            _profile = new AdminManagementMappingProfile();
            _configuration = new MapperConfiguration(cfg => cfg.AddProfile(_profile));
            _mapper = _configuration.CreateMapper();
            _connectionInformation = new OdsApiConnectionInformation ("Ods Instance", ApiMode.Sandbox)
            {
                ApiServerUrl = "http://server"
            };
            _school = new School
            {
                Id = SchoolId,
                Name = SchoolName,
                StreetNumberName = "Test street",
                City = "Austin",
                State = "TX",
                ZipCode = "98989",
                GradeLevels = new List<string> { "Kinder" }
            };
        }

        [Test]
        public void Should_GetAllSchools_returns_expected_schools_list()
        {
            // Arrange
            var edfiSchool = new EdFiSchool("id", "TestSchool", 1234, new List<EdFiEducationOrganizationAddress>(),
                new List<EdFiEducationOrganizationCategory>(), new List<EdFiSchoolGradeLevel>());
            var mockOdsRestClient = new Mock<IOdsRestClient>();
            mockOdsRestClient.Setup(x => x.GetAll<EdFiSchool>(ResourcePaths.Schools)).Returns(new List<EdFiSchool>
            {
                edfiSchool
            });

            _facade = new OdsApiFacade(_mapper, mockOdsRestClient.Object);

            //Act
            var result = _facade.GetAllSchools();

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBeGreaterThan(0);
            result.First().Name.ShouldBe("TestSchool");
        }

        [Test]
        public void Should_GetAllSchools_returns_exception_when_api_returns_not_ok_response()
        {
            // Arrange
            const string errorMessage = "not found error";

            var mockRestClient = new Mock<IRestClient>();
            mockRestClient.Setup(x => x.BaseUrl).Returns(new Uri(_connectionInformation.ApiBaseUrl));
            mockRestClient.Setup(x => x.Execute(It.IsAny<RestRequest>())).Returns(new RestResponse
                {StatusCode = HttpStatusCode.NotFound, ErrorMessage = errorMessage});

            var mockTokenRetriever = new Mock<ITokenRetriever>();
            mockTokenRetriever.Setup(x => x.ObtainNewBearerToken()).Returns("Token");

            var mockOdsRestClient =
                new OdsRestClient(_connectionInformation, mockRestClient.Object, mockTokenRetriever.Object);

            _facade = new OdsApiFacade(_mapper, mockOdsRestClient);

            //Act
            var ex = Assert.Throws<OdsApiConnectionException>(() => _facade.GetAllSchools());

            // Assert
            Assert.AreEqual(errorMessage, ex.Message);
        }

        [Test]
        public void Should_GetSchoolsByParentEdOrgId_returns_expected_schools_list()
        {
            // Arrange
            var edfiSchool = new EdFiSchool("id", "TestSchool", 1234, new List<EdFiEducationOrganizationAddress>(),
                new List<EdFiEducationOrganizationCategory>(), new List<EdFiSchoolGradeLevel>(),
                new EdFiLocalEducationAgencyReference(LocalEducationAgencyId));

            var mockOdsRestClient = new Mock<IOdsRestClient>();
            var filters = new Dictionary<string, object>
            {
                {"localEducationAgencyId", LocalEducationAgencyId}
            };
            mockOdsRestClient.Setup(x => x.GetAll<EdFiSchool>(ResourcePaths.Schools, filters)).Returns(new List<EdFiSchool>
            {
                edfiSchool
            });

            _facade = new OdsApiFacade(_mapper, mockOdsRestClient.Object);

            //Act
            var result = _facade.GetSchoolsByLeaIds(new List<int> { LocalEducationAgencyId });

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBeGreaterThan(0);
            result.First().Name.ShouldBe("TestSchool");
        }

        [Test]
        public void Should_GetSchoolsByParentEdOrgId_returns_exception_when_api_returns_not_ok_response()
        {
            // Arrange
            const string errorMessage = "not found error";

            var mockRestClient = new Mock<IRestClient>();
            mockRestClient.Setup(x => x.BaseUrl).Returns(new Uri(_connectionInformation.ApiBaseUrl));
            mockRestClient.Setup(x => x.Execute(It.IsAny<RestRequest>())).Returns(new RestResponse
            { StatusCode = HttpStatusCode.NotFound, ErrorMessage = errorMessage });

            var mockTokenRetriever = new Mock<ITokenRetriever>();
            mockTokenRetriever.Setup(x => x.ObtainNewBearerToken()).Returns("Token");

            var mockOdsRestClient =
                new OdsRestClient(_connectionInformation, mockRestClient.Object, mockTokenRetriever.Object);

            _facade = new OdsApiFacade(_mapper, mockOdsRestClient);

            //Act
            var ex = Assert.Throws<OdsApiConnectionException>(() => _facade.GetSchoolsByLeaIds(new List<int> { LocalEducationAgencyId }));

            // Assert
            Assert.AreEqual(errorMessage, ex.Message);
        }

        [Test]
        public void Should_GetSchoolById_returns_expected_school()
        {
            // Arrange
            var edfiSchool = new EdFiSchool(SchoolId, SchoolName, 1234, new List<EdFiEducationOrganizationAddress>(),
                new List<EdFiEducationOrganizationCategory>(), new List<EdFiSchoolGradeLevel>());
            var mockOdsRestClient = new Mock<IOdsRestClient>();
            mockOdsRestClient.Setup(x => x.GetById<EdFiSchool>(ResourcePaths.SchoolById, SchoolId)).Returns(edfiSchool);

            _facade = new OdsApiFacade(_mapper, mockOdsRestClient.Object);

            // Act
            var result = _facade.GetSchoolById(SchoolId);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(SchoolId);
            result.Name.ShouldBe(SchoolName);
        }

        [Test]
        public void Should_GetSchoolById_returns_exception_when_api_returns_not_ok_response()
        {
            // Arrange
            const string errorMessage = "not found error";

            var mockRestClient = new Mock<IRestClient>();
            mockRestClient.Setup(x => x.BaseUrl).Returns(new Uri(_connectionInformation.ApiBaseUrl));
            mockRestClient.Setup(x => x.Execute(It.IsAny<RestRequest>())).Returns(new RestResponse
                { StatusCode = HttpStatusCode.NotFound, ErrorMessage = errorMessage });

            var mockTokenRetriever = new Mock<ITokenRetriever>();
            mockTokenRetriever.Setup(x => x.ObtainNewBearerToken()).Returns("Token");

            var mockOdsRestClient =
                new OdsRestClient(_connectionInformation, mockRestClient.Object, mockTokenRetriever.Object);

            _facade = new OdsApiFacade(_mapper, mockOdsRestClient);

            //Act
            var ex = Assert.Throws<OdsApiConnectionException>(() => _facade.GetSchoolById(SchoolId));

            // Assert
            Assert.AreEqual(errorMessage, ex.Message);
        }

        [Test]
        public void Should_AddSchool_successfully_add_school()
        {
            // Arrange
            var mockOdsRestClient = new Mock<IOdsRestClient>();
            mockOdsRestClient
                .Setup(x => x.PostResource(It.IsAny<EdFiSchool>(), ResourcePaths.Schools, It.IsAny<bool>()))
                .Returns(new OdsApiResult());

            _facade = new OdsApiFacade(_mapper, mockOdsRestClient.Object);

            // Act
            var result = _facade.AddSchool(_school);

            // Assert
            result.Success.ShouldBe(true);
        }

        [Test]
        public void Should_AddSchool_returns_not_success_result_when_api_throws_exception()
        {
            // Arrange
            const string errorMsg = "exception";
            var mockRestClient = new Mock<IRestClient>();
            mockRestClient.Setup(x => x.BaseUrl).Returns(new Uri(_connectionInformation.ApiBaseUrl));
            mockRestClient.Setup(x => x.Execute(It.IsAny<RestRequest>())).Throws(new Exception(errorMsg));

            var mockTokenRetriever = new Mock<ITokenRetriever>();
            mockTokenRetriever.Setup(x => x.ObtainNewBearerToken()).Returns("Token");

            var mockOdsRestClient =
                new OdsRestClient(_connectionInformation, mockRestClient.Object, mockTokenRetriever.Object);

            _facade = new OdsApiFacade(_mapper, mockOdsRestClient);

            // Act
            var result = _facade.AddSchool(_school);

            // Assert
            result.Success.ShouldBe(false);
            result.ErrorMessage.ShouldContain(errorMsg);
        }

        [Test]
        public void Should_DeleteSchool_successfully_delete_school()
        {
            // Arrange
            var mockOdsRestClient = new Mock<IOdsRestClient>();
            mockOdsRestClient
                .Setup(x => x.DeleteResource(ResourcePaths.SchoolById, SchoolId, It.IsAny<bool>()))
                .Returns(new OdsApiResult());

            _facade = new OdsApiFacade(_mapper, mockOdsRestClient.Object);

            // Act
            var result = _facade.DeleteSchool(SchoolId);

            // Assert
            result.Success.ShouldBe(true);
        }

        [Test]
        public void Should_DeleteSchool_returns_not_success_result_when_api_throws_exception()
        {
            // Arrange
            const string errorMsg = "exception";
            var mockRestClient = new Mock<IRestClient>();
            mockRestClient.Setup(x => x.BaseUrl).Returns(new Uri(_connectionInformation.ApiBaseUrl));
            mockRestClient.Setup(x => x.Execute(It.IsAny<RestRequest>())).Throws(new Exception(errorMsg));

            var mockTokenRetriever = new Mock<ITokenRetriever>();
            mockTokenRetriever.Setup(x => x.ObtainNewBearerToken()).Returns("Token");

            var mockOdsRestClient =
                new OdsRestClient(_connectionInformation, mockRestClient.Object, mockTokenRetriever.Object);

            _facade = new OdsApiFacade(_mapper, mockOdsRestClient);

            // Act
            var result = _facade.DeleteSchool(SchoolId);

            // Assert
            result.Success.ShouldBe(false);
            result.ErrorMessage.ShouldContain(errorMsg);
        }

        [Test]
        public void Should_DeleteSchool_returns_not_success_result_when_api_returns_not_ok_response()
        {
            // Arrange
            const string errorMsg = "exception";
            var mockRestClient = new Mock<IRestClient>();
            mockRestClient.Setup(x => x.BaseUrl).Returns(new Uri(_connectionInformation.ApiBaseUrl));
            mockRestClient.Setup(x => x.Execute(It.IsAny<RestRequest>())).Returns(new RestResponse
                { StatusCode = HttpStatusCode.NotFound, ErrorMessage = errorMsg });

            var mockTokenRetriever = new Mock<ITokenRetriever>();
            mockTokenRetriever.Setup(x => x.ObtainNewBearerToken()).Returns("Token");

            var mockOdsRestClient =
                new OdsRestClient(_connectionInformation, mockRestClient.Object, mockTokenRetriever.Object);

            _facade = new OdsApiFacade(_mapper, mockOdsRestClient);

            // Act
            var result = _facade.DeleteSchool(SchoolId);

            // Assert
            result.Success.ShouldBe(false);
            result.ErrorMessage.ShouldBe(errorMsg);
        }

        [Test]
        public void Should_GetAllDescriptors_returns_expected_descriptors_list()
        {
            // Arrange
            const string content =
                "{\r\n  \"swagger\": \"2.0\",\r\n  \"basePath\": \"/v3.1.1/api/data/v3\",\r\n  \"consumes\": [\r\n    \"application/json\"\r\n  ],\r\n  \"definitions\": {\r\n    \"edFi_test1Descriptor\": {\r\n    },\r\n    \"edFi_test2Descriptor\": {\r\n    }\r\n  }, \"paths\": {\r\n    \"/ed-fi/testCategory1Descriptors\": {\r\n    },\r\n   \"/ed-fi/testCategory1Descriptors/{id}\": {\r\n    },\r\n   \"/ed-fi/testCategory1Descriptors/deletes\": {\r\n    },\r\n    \"/ed-fi/testCategory2Descriptors\": {\r\n    },\r\n   \"/ed-fi/testCategory2Descriptors/{id}\": {\r\n    },\r\n   \"/ed-fi/testCategory2Descriptors/deletes\": {\r\n    }\r\n  }\r\n}";
            var mockRestClient = new Mock<IRestClient>();
            mockRestClient.Setup(x => x.BaseUrl).Returns(new Uri(_connectionInformation.ApiBaseUrl));
            mockRestClient.Setup(x => x.Execute(It.IsAny<RestRequest>())).Returns(new RestResponse
            {
                ResponseStatus = ResponseStatus.Completed,
                StatusCode = HttpStatusCode.OK,
                Content = content
            });

            var mockTokenRetriever = new Mock<ITokenRetriever>();
            mockTokenRetriever.Setup(x => x.ObtainNewBearerToken()).Returns("Token");

            var mockOdsRestClient =
                new OdsRestClient(_connectionInformation, mockRestClient.Object, mockTokenRetriever.Object);

            _facade = new OdsApiFacade(_mapper, mockOdsRestClient);

            // Act
            var result = _facade.GetAllDescriptors();

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);
            result[0].ShouldBe("/ed-fi/testCategory1Descriptors");
            result[1].ShouldBe("/ed-fi/testCategory2Descriptors");
        }

        [Test]
        public void Should_GetAllDescriptors_returns_exception_when_swagger_end_point_returns_not_ok_response()
        {
            // Arrange
            const string errorMessage = "not found error";

            var mockRestClient = new Mock<IRestClient>();
            mockRestClient.Setup(x => x.BaseUrl).Returns(new Uri(_connectionInformation.ApiBaseUrl));
            mockRestClient.Setup(x => x.Execute(It.IsAny<RestRequest>())).Returns(new RestResponse
                { StatusCode = HttpStatusCode.NotFound, ErrorMessage = errorMessage });

            var mockTokenRetriever = new Mock<ITokenRetriever>();
            mockTokenRetriever.Setup(x => x.ObtainNewBearerToken()).Returns("Token");

            var mockOdsRestClient =
                new OdsRestClient(_connectionInformation, mockRestClient.Object, mockTokenRetriever.Object);

            _facade = new OdsApiFacade(_mapper, mockOdsRestClient);

            //Act
            var ex = Assert.Throws<OdsApiConnectionException>(() => _facade.GetAllDescriptors());

            // Assert
            Assert.AreEqual(errorMessage, ex.Message);
        }

        [Test]
        public void Should_EditSchool_successfully_edit_school()
        {
            // Arrange
            var mockOdsRestClient = new Mock<IOdsRestClient>();
            mockOdsRestClient
                .Setup(x => x.PutResource(It.IsAny<EdFiSchool>(), ResourcePaths.Schools, It.IsAny<string>(),
                    It.IsAny<bool>()))
                .Returns(new OdsApiResult());

            _facade = new OdsApiFacade(_mapper, mockOdsRestClient.Object);

            // Act
            var result = _facade.EditSchool(_school);

            // Assert
            result.Success.ShouldBe(true);
        }

        [Test]
        public void Should_EditSchool_returns_not_success_result_when_api_throws_exception()
        {
            // Arrange
            const string errorMsg = "exception";
            var mockRestClient = new Mock<IRestClient>();
            mockRestClient.Setup(x => x.BaseUrl).Returns(new Uri(_connectionInformation.ApiBaseUrl));
            mockRestClient.Setup(x => x.Execute(It.IsAny<RestRequest>())).Throws(new Exception(errorMsg));

            var mockTokenRetriever = new Mock<ITokenRetriever>();
            mockTokenRetriever.Setup(x => x.ObtainNewBearerToken()).Returns("Token");

            var mockOdsRestClient =
                new OdsRestClient(_connectionInformation, mockRestClient.Object, mockTokenRetriever.Object);

            _facade = new OdsApiFacade(_mapper, mockOdsRestClient);

            // Act
            var result = _facade.EditSchool(_school);

            // Assert
            result.Success.ShouldBe(false);
            result.ErrorMessage.ShouldContain(errorMsg);
        }

        [Test]
        public void Should_AddSchool_returns_not_success_result_when_api_not_returns_not_expected_response()
        {
            // Arrange
            const string errorMsg = "exception";
            var mockRestClient = new Mock<IRestClient>();
            mockRestClient.Setup(x => x.BaseUrl).Returns(new Uri(_connectionInformation.ApiBaseUrl));
            mockRestClient.Setup(x => x.Execute(It.IsAny<RestRequest>())).Returns(new RestResponse
            {
                StatusCode = HttpStatusCode.Conflict,
                ErrorMessage = errorMsg
            });

            var mockTokenRetriever = new Mock<ITokenRetriever>();
            mockTokenRetriever.Setup(x => x.ObtainNewBearerToken()).Returns("Token");

            var mockOdsRestClient =
                new OdsRestClient(_connectionInformation, mockRestClient.Object, mockTokenRetriever.Object);

            _facade = new OdsApiFacade(_mapper, mockOdsRestClient);

            // Act
            var result = _facade.AddSchool(_school);

            // Assert
            result.Success.ShouldBe(false);
            result.ErrorMessage.ShouldBe(errorMsg);
        }

        [Test]
        public void Should_AddSchool_returns_success_result_when_api_returns_expected_response()
        {
            // Arrange
            var mockRestClient = new Mock<IRestClient>();
            mockRestClient.Setup(x => x.BaseUrl).Returns(new Uri(_connectionInformation.ApiBaseUrl));
            mockRestClient.Setup(x => x.Execute(It.IsAny<RestRequest>())).Returns(new RestResponse
            {
                ResponseStatus = ResponseStatus.Completed,
                StatusCode = HttpStatusCode.Created
            });

            var mockTokenRetriever = new Mock<ITokenRetriever>();
            mockTokenRetriever.Setup(x => x.ObtainNewBearerToken()).Returns("Token");

            var mockOdsRestClient =
                new OdsRestClient(_connectionInformation, mockRestClient.Object, mockTokenRetriever.Object);

            _facade = new OdsApiFacade(_mapper, mockOdsRestClient);

            // Act
            var result = _facade.AddSchool(_school);

            // Assert
            result.Success.ShouldBe(true);
        }

        [Test]
        public void Should_EditSchool_returns_not_success_result_when_api_not_returns_not_expected_response()
        {
            // Arrange
            const string errorMsg = "exception";
            var mockRestClient = new Mock<IRestClient>();
            mockRestClient.Setup(x => x.BaseUrl).Returns(new Uri(_connectionInformation.ApiBaseUrl));
            mockRestClient.Setup(x => x.Execute(It.IsAny<RestRequest>())).Returns(new RestResponse
            {
                StatusCode = HttpStatusCode.Conflict,
                ErrorMessage = errorMsg
            });

            var mockTokenRetriever = new Mock<ITokenRetriever>();
            mockTokenRetriever.Setup(x => x.ObtainNewBearerToken()).Returns("Token");

            var mockOdsRestClient =
                new OdsRestClient(_connectionInformation, mockRestClient.Object, mockTokenRetriever.Object);

            _facade = new OdsApiFacade(_mapper, mockOdsRestClient);

            // Act
            var result = _facade.EditSchool(_school);

            // Assert
            result.Success.ShouldBe(false);
            result.ErrorMessage.ShouldBe(errorMsg);
        }

        [Test]
        public void Should_EditSchool_returns_success_result_when_api_returns_expected_response()
        {
            // Arrange
            var mockRestClient = new Mock<IRestClient>();
            mockRestClient.Setup(x => x.BaseUrl).Returns(new Uri(_connectionInformation.ApiBaseUrl));
            mockRestClient.Setup(x => x.Execute(It.IsAny<RestRequest>())).Returns(new RestResponse
            {
                StatusCode = HttpStatusCode.NoContent,
                ResponseStatus = ResponseStatus.Completed,
            });

            var mockTokenRetriever = new Mock<ITokenRetriever>();
            mockTokenRetriever.Setup(x => x.ObtainNewBearerToken()).Returns("Token");

            var mockOdsRestClient =
                new OdsRestClient(_connectionInformation, mockRestClient.Object, mockTokenRetriever.Object);

            _facade = new OdsApiFacade(_mapper, mockOdsRestClient);

            // Act
            var result = _facade.EditSchool(_school);

            // Assert
            result.Success.ShouldBe(true);
        }
    }
}
