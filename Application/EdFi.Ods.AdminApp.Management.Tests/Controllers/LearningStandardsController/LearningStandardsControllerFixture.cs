// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Web.Display.TabEnumeration;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using EdFi.Ods.AdminApp.Web.Infrastructure.Jobs;
using Moq;
using NUnit.Framework;

namespace EdFi.Ods.AdminApp.Management.Tests.Controllers.LearningStandardsController
{
    public class LearningStandardsControllerFixture
    {
        protected virtual void AdditionalSetup()
        {
        }

        [SetUp]
        public void SetUp()
        {
            OdsApiFacadeFactory = new Mock<IOdsApiFacadeFactory>();
            TabDisplayService = new Mock<ITabDisplayService>();

            LearningStandardsJob = new Mock<IProductionLearningStandardsJob>();
            LearningStandardsSetupCommand = new Mock<IEnableLearningStandardsSetupCommand>();
            OdsSecretConfigurationProvider = new Mock<IOdsSecretConfigurationProvider>();
            ResetLearningStandards = new Mock<IResetLearningStandards>();
            ApiConnectionInformationProvider = new Mock<IOdsApiConnectionInformationProvider>();
            InstanceContext = new InstanceContext {Id = OdsInstanceId, Name = "TestOdsInstance", Description = "TestOdsInstance Description" };
            ApiModeProvider = new Mock<ICloudOdsAdminAppSettingsApiModeProvider>();

            SystemUnderTest = new Web.Controllers.LearningStandardsController(
                OdsApiFacadeFactory.Object,
                TabDisplayService.Object,
                LearningStandardsJob.Object,
                LearningStandardsSetupCommand.Object,
                OdsSecretConfigurationProvider.Object,
                ResetLearningStandards.Object,
                ApiConnectionInformationProvider.Object,
                InstanceContext,
                ApiModeProvider.Object
            );

            AdditionalSetup();
        }

        protected Mock<IOdsApiFacadeFactory> OdsApiFacadeFactory;
        protected Mock<ITabDisplayService> TabDisplayService;
        protected Mock<IProductionLearningStandardsJob> LearningStandardsJob;
        protected Mock<IEnableLearningStandardsSetupCommand> LearningStandardsSetupCommand;
        protected Mock<IOdsSecretConfigurationProvider> OdsSecretConfigurationProvider;
        protected Mock<IResetLearningStandards> ResetLearningStandards;
        protected Mock<IOdsApiConnectionInformationProvider> ApiConnectionInformationProvider;
        protected InstanceContext InstanceContext;
        protected Mock<ICloudOdsAdminAppSettingsApiModeProvider> ApiModeProvider;
        protected Web.Controllers.LearningStandardsController SystemUnderTest;
        protected int OdsInstanceId = 1234;
    }
}
