// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Management.Settings;
using EdFi.Ods.AdminApp.Web.Display.TabEnumeration;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using EdFi.Ods.AdminApp.Web.Infrastructure.IO;
using EdFi.Ods.AdminApp.Web.Infrastructure.Jobs;
using Moq;
using NUnit.Framework;
using EdFi.Ods.AdminApp.Web.Models.ViewModels;
using FluentValidation;
using FluentValidation.Results;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Helpers;
using Microsoft.Extensions.Options;
using System.IO;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.BulkUpload;
using Microsoft.AspNetCore.Hosting;
using static EdFi.Ods.AdminApp.Management.Tests.Testing;

namespace EdFi.Ods.AdminApp.Management.Tests.Controllers.OdsInstanceSettingsController
{
    public class OdsInstanceSettingsControllerFixture
    {
        protected virtual void AdditionalSetup()
        {
        }

        [SetUp]
        public void SetUp()
        {
            OdsApiFacadeFactory = new Mock<IOdsApiFacadeFactory>();
            GetProductionApiProvisioningWarningsQuery = new Mock<IGetProductionApiProvisioningWarningsQuery>();
            CachedItems = new Mock<ICachedItems>();
            ProductionSetupJob = new Mock<IProductionSetupJob>();
            CloudOdsSettingsService = new Mock<ICloudOdsSettingsService>();
            TabDisplayService = new Mock<ITabDisplayService>();

            InstanceContext = new InstanceContext { Id = OdsInstanceId, Name = "TestOdsInstance", Description = "TestOdsInstance Description" };
            AppSettings = new Mock<IOptions<AppSettings>>();
            Scoped<IOptions<AppSettings>>(appSettings =>
            {
                AppSettings.Setup(x => x.Value).Returns(appSettings.Value);
            });
           
            SystemUnderTest = new Web.Controllers.OdsInstanceSettingsController(
                OdsApiFacadeFactory.Object,
                GetProductionApiProvisioningWarningsQuery.Object,
                CachedItems.Object,
                ProductionSetupJob.Object,
                CloudOdsSettingsService.Object,
                TabDisplayService.Object,
                InstanceContext,
                AppSettings.Object
            );

            AdditionalSetup();
        }

        protected Mock<IOdsApiFacadeFactory> OdsApiFacadeFactory;
        protected Mock<IGetProductionApiProvisioningWarningsQuery> GetProductionApiProvisioningWarningsQuery;
        protected Mock<ICachedItems> CachedItems;
        protected Mock<IProductionSetupJob> ProductionSetupJob;
        protected Mock<ICloudOdsSettingsService> CloudOdsSettingsService;
        protected Mock<ITabDisplayService> TabDisplayService;

        protected InstanceContext InstanceContext;
        protected Web.Controllers.OdsInstanceSettingsController SystemUnderTest;
        protected int OdsInstanceId = 1234;
        protected Mock<IOptions<AppSettings>> AppSettings;
    }
}
