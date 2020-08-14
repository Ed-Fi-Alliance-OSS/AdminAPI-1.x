// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Ods.AdminApp.Web.Display.TabEnumeration;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.OdsInstanceSettings;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Web.Tests.Controllers.OdsInstanceSettingsController
{
    [TestFixture]
    public class WhenGettingTheApplicationsScreenContents : OdsInstanceSettingsControllerFixture
    {

        // Writing separate tests for each condition, when mock setup is required in all cases, seems
        // impractical and redundant. This test will use a mixture of "bare" asserts and grouped
        // "satisfy all" assertions: the initial assertions are deliberately separate, because
        // the grouped assertions are meaningless if the initial assertions fail. The initial
        // assertions are basically guard clauses for null values.
        [Test]
        public async Task ThenModelIsInitializedWithOdsInstanceSettings()
        {
            // Arrange
            var productionTab = new List<TabDisplay<OdsInstanceSettingsTabEnumeration>>();

            TabDisplayService.Setup(x => x.GetOdsInstanceSettingsTabDisplay(OdsInstanceSettingsTabEnumeration.Applications))
                .Returns(productionTab);

            const string baseUrl = "http://example.com";

            ApiConnectionInformationProvider.Setup(x => x.GetConnectionInformationForEnvironment(CloudOdsEnvironment.Production))
                .ReturnsAsync(new OdsApiConnectionInformation ("Ods Instance", ApiMode.Sandbox) {ApiServerUrl = baseUrl});

            // Act
            var result = await SystemUnderTest.Applications();

            // Assert

            result.ShouldNotBeNull();
            result.ShouldBeOfType<ViewResult>();
            var viewResult = result as ViewResult;

            // ReSharper disable PossibleNullReferenceException - assertions are the guard clauses
            viewResult.Model.ShouldBeOfType<OdsInstanceSettingsModel>();
            var model = viewResult.Model as OdsInstanceSettingsModel;

            model.ShouldSatisfyAllConditions(
                () => model.OdsInstanceSettingsTabEnumerations.ShouldBeSameAs(productionTab),
                () => model.ProductionApiUrl.ShouldContain(baseUrl)
            );

            // ReSharper restore PossibleNullReferenceException
        }
    }
}
