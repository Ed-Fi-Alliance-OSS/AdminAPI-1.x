// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using AutoMapper;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Management.Database.Queries;
using EdFi.Ods.AdminApp.Management.Settings;
using EdFi.Ods.AdminApp.Web.Display.TabEnumeration;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using EdFi.Ods.AdminApp.Web.Infrastructure.IO;
using EdFi.Ods.AdminApp.Web.Infrastructure.Jobs;
using Moq;
using NUnit.Framework;

namespace EdFi.Ods.AdminApp.Web.Tests.Controllers.OdsInstanceSettingsController
{
    public class OdsInstanceSettingsControllerFixture
    {
        protected virtual void AdditionalSetup()
        {
        }

        [SetUp]
        public void SetUp()
        {
            Mapper = new Mock<IMapper>();
            GetVendorsQuery = new Mock<IGetVendorsQuery>();
            OdsApiFacadeFactory = new Mock<IOdsApiFacadeFactory>();
            GetProductionApiProvisioningWarningsQuery = new Mock<IGetProductionApiProvisioningWarningsQuery>();
            CachedItems = new Mock<ICachedItems>();
            ProductionSetupJob = new Mock<IProductionSetupJob>();
            CloudOdsSettingsService = new Mock<ICloudOdsSettingsService>();
            TabDisplayService = new Mock<ITabDisplayService>();

            LearningStandardsJob = new Mock<IProductionLearningStandardsJob>();
            LearningStandardsSetupCommand = new Mock<IEnableLearningStandardsSetupCommand>();
            OdsSecretConfigurationProvider = new Mock<IOdsSecretConfigurationProvider>();
            ResetLearningStandards = new Mock<IResetLearningStandards>();
            ApiConnectionInformationProvider = new Mock<IOdsApiConnectionInformationProvider>();
            FileUploadHandler = new Mock<IFileUploadHandler>();
            BulkUploadJob = new Mock<IBulkUploadJob>();
            InstanceContext = new InstanceContext {Id = OdsInstanceId, Name = "TestOdsInstance", Description = "TestOdsInstance Description" };
            ApiModeProvider = new Mock<ICloudOdsAdminAppSettingsApiModeProvider>();
            InferOdsApiVersion = new Mock<IInferOdsApiVersion>();

            SystemUnderTest = new Web.Controllers.OdsInstanceSettingsController(
                Mapper.Object,
                GetVendorsQuery.Object,
                OdsApiFacadeFactory.Object,
                GetProductionApiProvisioningWarningsQuery.Object,
                CachedItems.Object,
                ProductionSetupJob.Object,
                CloudOdsSettingsService.Object,
                TabDisplayService.Object,
                LearningStandardsJob.Object,
                LearningStandardsSetupCommand.Object,
                OdsSecretConfigurationProvider.Object,
                ResetLearningStandards.Object,
                ApiConnectionInformationProvider.Object,
                FileUploadHandler.Object,
                BulkUploadJob.Object,
                InstanceContext,
                ApiModeProvider.Object,
                InferOdsApiVersion.Object
            );

            AdditionalSetup();
        }

        protected Mock<IMapper> Mapper;
        protected Mock<IGetVendorsQuery> GetVendorsQuery;
        protected Mock<IOdsApiFacadeFactory> OdsApiFacadeFactory;
        protected Mock<IGetProductionApiProvisioningWarningsQuery> GetProductionApiProvisioningWarningsQuery;
        protected Mock<ICachedItems> CachedItems;
        protected Mock<IProductionSetupJob> ProductionSetupJob;
        protected Mock<ICloudOdsSettingsService> CloudOdsSettingsService;
        protected Mock<ITabDisplayService> TabDisplayService;
        protected Mock<IProductionLearningStandardsJob> LearningStandardsJob;
        protected Mock<IEnableLearningStandardsSetupCommand> LearningStandardsSetupCommand;
        protected Mock<IOdsSecretConfigurationProvider> OdsSecretConfigurationProvider;
        protected Mock<IResetLearningStandards> ResetLearningStandards;
        protected Mock<IOdsApiConnectionInformationProvider> ApiConnectionInformationProvider;
        protected Mock<IFileUploadHandler> FileUploadHandler;
        protected Mock<IBulkUploadJob> BulkUploadJob;
        protected InstanceContext InstanceContext;
        protected Mock<ICloudOdsAdminAppSettingsApiModeProvider> ApiModeProvider;
        protected Web.Controllers.OdsInstanceSettingsController SystemUnderTest;
        protected int OdsInstanceId = 1234;
        protected Mock<IInferOdsApiVersion> InferOdsApiVersion;
    }
}
