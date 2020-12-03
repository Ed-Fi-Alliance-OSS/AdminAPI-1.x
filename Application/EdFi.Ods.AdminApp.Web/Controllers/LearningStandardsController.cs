// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Ods.AdminApp.Web.Display.TabEnumeration;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using EdFi.Ods.AdminApp.Web.Infrastructure.Jobs;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.LearningStandards;
using log4net;

namespace EdFi.Ods.AdminApp.Web.Controllers
{
    public class LearningStandardsController : ControllerBase
    {
        private readonly IOdsApiConnectionInformationProvider _apiConnectionInformationProvider;
        private readonly IProductionLearningStandardsJob _learningStandardsJob;
        private readonly IEnableLearningStandardsSetupCommand _learningStandardsSetupCommand;
        private readonly ILog _logger = LogManager.GetLogger(typeof(LearningStandardsController));
        private readonly IOdsApiFacadeFactory _odsApiFacadeFactory;
        private readonly IOdsSecretConfigurationProvider _odsSecretConfigurationProvider;
        private readonly IResetLearningStandards _resetLearningStandards;
        private readonly ITabDisplayService _tabDisplayService;
        private readonly InstanceContext _instanceContext;
        private readonly ICloudOdsAdminAppSettingsApiModeProvider _cloudOdsAdminAppSettingsApiModeProvider;

        public LearningStandardsController(IOdsApiFacadeFactory odsApiFacadeFactory
            , ITabDisplayService tabDisplayService
            , IProductionLearningStandardsJob learningStandardsJob
            , IEnableLearningStandardsSetupCommand learningStandardsSetupCommand
            , IOdsSecretConfigurationProvider odsSecretConfigurationProvider
            , IResetLearningStandards resetLearningStandards
            , IOdsApiConnectionInformationProvider apiConnectionInformationProvider
            , InstanceContext instanceContext
            , ICloudOdsAdminAppSettingsApiModeProvider cloudOdsAdminAppSettingsApiModeProvider
            )
        {
            _odsApiFacadeFactory = odsApiFacadeFactory;
            _tabDisplayService = tabDisplayService;
            _learningStandardsJob = learningStandardsJob;
            _learningStandardsSetupCommand = learningStandardsSetupCommand;
            _odsSecretConfigurationProvider = odsSecretConfigurationProvider;
            _resetLearningStandards = resetLearningStandards;
            _apiConnectionInformationProvider = apiConnectionInformationProvider;
            _instanceContext = instanceContext;
            _cloudOdsAdminAppSettingsApiModeProvider = cloudOdsAdminAppSettingsApiModeProvider;
        }

        public async Task<ActionResult> Index()
        {
            var odsApiFacade = await _odsApiFacadeFactory.Create();
            var config = await _odsSecretConfigurationProvider.GetSecretConfiguration(_instanceContext.Id);

            if (config == null)
            {
                _logger.Error("ODS secret configuration is null.");
                throw new InvalidOperationException("ODS secret configuration can not be null.");
            }

            var model = new LearningStandardsIndexModel
            {
                OdsInstanceSettingsTabEnumerations =
                    _tabDisplayService.GetOdsInstanceSettingsTabDisplay(OdsInstanceSettingsTabEnumeration
                        .LearningStandards),
                LearningStandardsModel = new LearningStandardsModel
                {
                    ApiKey = config.LearningStandardsCredential?.ApiKey ?? string.Empty,
                    ApiSecret = config.LearningStandardsCredential?.ApiSecret ?? string.Empty,
                    HasApiData = odsApiFacade.DoesApiDataExist(),
                    SynchronizationWasSuccessful =
                        config.LearningStandardsCredential?.SynchronizationWasSuccessful ?? false,
                    IsJobRunning = _learningStandardsJob.IsJobRunning(),
                    IsSameOdsInstance = _learningStandardsJob.IsSameOdsInstance(_instanceContext.Id, typeof(LearningStandardsJobContext)),
                    LastUpdatedDate = config.LearningStandardsCredential?.LastUpdatedDate ?? DateTime.MinValue
                },
                OdsInstance = _instanceContext
            };

            ViewData["PathBase"] = Request != null ? Request.PathBase.ToString() : "";

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> LearningStandards(LearningStandardsModel model)
        {
            await _learningStandardsSetupCommand.Execute(
                new AcademicBenchmarkConfig
                {
                    ApiKey = model.ApiKey,
                    ApiSecret = model.ApiSecret,
                    OdsApiMode = CloudOdsAdminAppSettings.Instance.Mode
                });

            await RunLearningStandardsJob();

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> UpdateLearningStandards()
        {
            await RunLearningStandardsJob();

            return Ok();
        }

        private async Task RunLearningStandardsJob()
        {
            var connectionInformation = await GetConnectionInformationProvider();

            int? schoolYear = null;

            if (_cloudOdsAdminAppSettingsApiModeProvider.GetApiMode() == ApiMode.YearSpecific)
            {
                schoolYear = _instanceContext.Name.ExtractNumericInstanceSuffix();
            }

            var jobContext = new LearningStandardsJobContext
            {
                ApiUrl = connectionInformation.ApiServerUrl,
                SchoolYear = schoolYear,
                OdsInstanceId = _instanceContext.Id
            };

            _learningStandardsJob.EnqueueJob(jobContext);
        }

        [HttpPost]
        public async Task<ActionResult> ResetLearningStandards()
        {
            await _resetLearningStandards.Execute();

            return RedirectToActionJson<LearningStandardsController>(
                x => x.Index(),
                "Credentials successfully reset");
        }

        private async Task<OdsApiConnectionInformation> GetConnectionInformationProvider()
        {
            return await _apiConnectionInformationProvider.GetConnectionInformationForEnvironment();
        }
    }
}
