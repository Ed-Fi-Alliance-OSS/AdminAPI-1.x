// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Ods.AdminApp.Web.Display.TabEnumeration;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.Application;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests.Controllers.OdsInstanceSettingsController
{
    [TestFixture]
    public class WhenGettingTheApplicationsScreenContents : OdsInstanceSettingsControllerFixture
    {
        [Test]
        public async Task ThenModelIsInitializedWithOdsInstanceSettings()
        {
            var productionTab = new List<TabDisplay<OdsInstanceSettingsTabEnumeration>>();

            TabDisplayService.Setup(x => x.GetOdsInstanceSettingsTabDisplay(OdsInstanceSettingsTabEnumeration.Applications))
                .Returns(productionTab);

            const string baseUrl = "http://example.com";

            ApiConnectionInformationProvider.Setup(x => x.GetConnectionInformationForEnvironment(CloudOdsEnvironment.Production))
                .ReturnsAsync(new OdsApiConnectionInformation ("Ods Instance", ApiMode.Sandbox) {ApiServerUrl = baseUrl});

            var result = await SystemUnderTest.Applications();

            result.ShouldNotBeNull();
            result.ShouldBeOfType<ViewResult>();
            var viewResult = (ViewResult)result;

            viewResult.Model.ShouldBeOfType<ApplicationsIndexModel>();
            var model = (ApplicationsIndexModel)viewResult.Model;

            model.ShouldSatisfyAllConditions(
                () => model.OdsInstanceSettingsTabEnumerations.ShouldBeSameAs(productionTab),
                () => model.ProductionApiUrl.ShouldContain(baseUrl)
            );
        }
    }
}
