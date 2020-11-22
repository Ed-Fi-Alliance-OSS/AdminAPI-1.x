// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
#if NET48
using System.Web.Mvc;
using System.Web.Hosting;
#else
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
#endif
using AutoMapper;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Management.Database.Queries;
using EdFi.Ods.AdminApp.Management.Helpers;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Ods.AdminApp.Management.Settings;
using EdFi.Ods.AdminApp.Web.Display.TabEnumeration;
using EdFi.Ods.AdminApp.Web.Helpers;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using EdFi.Ods.AdminApp.Web.Infrastructure.IO;
using EdFi.Ods.AdminApp.Web.Infrastructure.Jobs;
using EdFi.Ods.AdminApp.Web.Models.ViewModels;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.OdsInstanceSettings;
using FluentValidation;
using log4net;
using Microsoft.Extensions.Options;

namespace EdFi.Ods.AdminApp.Web.Controllers
{
    public class OdsInstanceSettingsController : ControllerBase
    {
        private readonly IOdsApiConnectionInformationProvider _apiConnectionInformationProvider;
        private readonly ICachedItems _cachedItems;
        private readonly ICloudOdsSettingsService _cloudOdsSettingsService;
        private readonly IGetProductionApiProvisioningWarningsQuery _getProductionApiProvisioningWarningsQuery;
        private readonly IProductionLearningStandardsJob _learningStandardsJob;
        private readonly IEnableLearningStandardsSetupCommand _learningStandardsSetupCommand;
        private readonly ILog _logger = LogManager.GetLogger(typeof(OdsInstanceSettingsController));
        private readonly IOdsApiFacadeFactory _odsApiFacadeFactory;
        private readonly IOdsSecretConfigurationProvider _odsSecretConfigurationProvider;
        private readonly IProductionSetupJob _productionSetupJob;
        private readonly IResetLearningStandards _resetLearningStandards;
        private readonly ITabDisplayService _tabDisplayService;
        private readonly string _missingOdsSecretConfig = "ODS secret configuration can not be null";
        private readonly IFileUploadHandler _fileUploadHandler;
        private readonly IBulkUploadJob _bulkUploadJob;
        private readonly InstanceContext _instanceContext;
        private readonly ICloudOdsAdminAppSettingsApiModeProvider _cloudOdsAdminAppSettingsApiModeProvider;
        private readonly IInferOdsApiVersion _inferOdsApiVersion;   
        private readonly IValidator<BulkFileUploadModel> _bulkLoadValidator;
        private readonly AppSettings _appSettings;
#if !NET48
        private readonly IWebHostEnvironment _webHostEnvironment;
#endif

        public OdsInstanceSettingsController(
              IOdsApiFacadeFactory odsApiFacadeFactory
            , IGetProductionApiProvisioningWarningsQuery getProductionApiProvisioningWarningsQuery
            , ICachedItems cachedItems
            , IProductionSetupJob productionSetupJob
            , ICloudOdsSettingsService cloudOdsSettingsService
            , ITabDisplayService tabDisplayService 
            , IProductionLearningStandardsJob learningStandardsJob
            , IEnableLearningStandardsSetupCommand learningStandardsSetupCommand
            , IOdsSecretConfigurationProvider odsSecretConfigurationProvider
            , IResetLearningStandards resetLearningStandards
            , IOdsApiConnectionInformationProvider apiConnectionInformationProvider
            , IFileUploadHandler fileUploadHandler
            , IBulkUploadJob bulkUploadJob
            , InstanceContext instanceContext
            , ICloudOdsAdminAppSettingsApiModeProvider cloudOdsAdminAppSettingsApiModeProvider
            , IInferOdsApiVersion inferOdsApiVersion          
            , IValidator<BulkFileUploadModel> bulkLoadValidator
            , IOptions<AppSettings> appSettingsAccessor
#if !NET48
            , IWebHostEnvironment webHostEnvironment
#endif
            )
        {
            _odsApiFacadeFactory = odsApiFacadeFactory;
            _getProductionApiProvisioningWarningsQuery = getProductionApiProvisioningWarningsQuery;
            _cachedItems = cachedItems;
            _productionSetupJob = productionSetupJob;
            _cloudOdsSettingsService = cloudOdsSettingsService;
            _tabDisplayService = tabDisplayService;
            _learningStandardsJob = learningStandardsJob;
            _learningStandardsSetupCommand = learningStandardsSetupCommand;
            _odsSecretConfigurationProvider = odsSecretConfigurationProvider;
            _resetLearningStandards = resetLearningStandards;
            _apiConnectionInformationProvider = apiConnectionInformationProvider;
            _fileUploadHandler = fileUploadHandler;
            _bulkUploadJob = bulkUploadJob;
            _instanceContext = instanceContext;
            _cloudOdsAdminAppSettingsApiModeProvider = cloudOdsAdminAppSettingsApiModeProvider;
            _inferOdsApiVersion = inferOdsApiVersion;
            _bulkLoadValidator = bulkLoadValidator;
            _appSettings = appSettingsAccessor.Value;

#if !NET48
            _webHostEnvironment = webHostEnvironment;
#endif
        }

        public async Task<ActionResult> Setup()
        {
            var setupCompleted = (await _odsApiFacadeFactory.Create()).DoesApiDataExist();

            if (setupCompleted) return RedirectToAction("SetupComplete");

            var model = new OdsInstanceSettingsModel
            {
                OdsInstanceSettingsTabEnumerations =
                    _tabDisplayService.GetOdsInstanceSettingsTabDisplay(OdsInstanceSettingsTabEnumeration.Setup)
            };

            return View(model);
        }

        public async Task<ActionResult> SetupComplete()
        {
            var defaultOdsInstance = await _cachedItems.GetDefaultCloudOdsInstance();

            var model = new OdsInstanceSettingsModel
            {
                ProductionSetupCompletedModel = new OdsInstanceSetupCompletedModel
                {
                    ProvisioningWarnings = await _getProductionApiProvisioningWarningsQuery.Execute(defaultOdsInstance)
                },
                OdsInstanceSettingsTabEnumerations =
                    _tabDisplayService.GetOdsInstanceSettingsTabDisplay(OdsInstanceSettingsTabEnumeration.Setup)
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Setup(OdsInstanceSettingsModel model)
        {
            if (CloudOdsAdminAppSettings.Instance.SystemManagedSqlServer)
                _productionSetupJob.EnqueueJob(1);

#if NET48
            return new HttpStatusCodeResult(200);
#else
            return Ok();
#endif
        }

        public async Task<ActionResult> Logging()
        {
            var settings = await _cloudOdsSettingsService.GetSettings(_appSettings.DefaultOdsInstance);

            var model = new OdsInstanceSettingsModel
            {
                LogSettingsModel = new LogSettingsModel
                {
                    LogLevel = settings.LogLevel
                },
                OdsInstanceSettingsTabEnumerations =
                    _tabDisplayService.GetOdsInstanceSettingsTabDisplay(OdsInstanceSettingsTabEnumeration.Logging),
                OdsInstance = _instanceContext
            };

            return View(model);
        }

        public async Task<ActionResult> LearningStandards()
        {
            var odsApiFacade = await _odsApiFacadeFactory.Create();
            var config = await _odsSecretConfigurationProvider.GetSecretConfiguration(_instanceContext.Id);

            if (config == null)
            {
                _logger.Error("ODS secret configuration is null.");
                throw new InvalidOperationException("ODS secret configuration can not be null.");
            }

            var model = new OdsInstanceSettingsModel
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

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> LearningStandards(OdsInstanceSettingsModel model)
        {
            await _learningStandardsSetupCommand.Execute(
                new AcademicBenchmarkConfig
                {
                    ApiKey = model.LearningStandardsModel.ApiKey,
                    ApiSecret = model.LearningStandardsModel.ApiSecret,
                    OdsApiMode = CloudOdsAdminAppSettings.Instance.Mode
                });

            await RunLearningStandardsJob();

#if NET48
            return new HttpStatusCodeResult(200);
#else
            return Ok();
#endif
        }

        [HttpPost]
        public async Task<ActionResult> UpdateLearningStandards()
        {
            await RunLearningStandardsJob();

#if NET48
            return new HttpStatusCodeResult(200);
#else
            return Ok();
#endif
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

            return RedirectToActionJson<OdsInstanceSettingsController>(
                x => x.LearningStandards(),
                "Credentials successfully reset");
        }

        private async Task<OdsApiConnectionInformation> GetConnectionInformationProvider()
        {
            return await _apiConnectionInformationProvider.GetConnectionInformationForEnvironment();
        }

        public async Task<ActionResult> BulkLoad()
        {
            var config = await _odsSecretConfigurationProvider.GetSecretConfiguration(_instanceContext.Id);
            if (config == null)
            {
                _logger.Error(_missingOdsSecretConfig);
                return JsonError(_missingOdsSecretConfig);
            }
            var model = new OdsInstanceSettingsModel
            {
                OdsInstanceSettingsTabEnumerations =
                    _tabDisplayService.GetOdsInstanceSettingsTabDisplay(OdsInstanceSettingsTabEnumeration
                        .BulkLoad),
                BulkFileUploadModel = new BulkFileUploadModel
                {
                    ApiKey = config.BulkUploadCredential?.ApiKey ?? string.Empty,
                    ApiSecret = config.BulkUploadCredential?.ApiSecret ?? string.Empty
                },
                OdsInstance = _instanceContext
            };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> BulkFileUpload(OdsInstanceSettingsModel model)
        {
#if NET48
            var bulkFiles = model.BulkFileUploadModel.BulkFiles.Where(file => file != null && file.ContentLength > 0).ToArray();
#else
            var bulkFiles = model.BulkFileUploadModel.BulkFiles.Where(file => file != null && file.Length > 0).ToArray();
#endif

            if (!bulkFiles.Any())
            {
#if NET48
                return new HttpStatusCodeResult(HttpStatusCode.NoContent);
#else
                return NoContent();
#endif
            }

#if NET48
            if (bulkFiles.Sum(f => f.ContentLength) > BulkFileUploadModel.MaxFileSize)
#else
            if (bulkFiles.Sum(f => f.Length) > BulkFileUploadModel.MaxFileSize)
#endif
            {
                throw new Exception($"Upload exceeds maximum limit of {BulkFileUploadModel.MaxFileSize} bytes");
            }

            if (bulkFiles.Length > 1)
            {
                throw new Exception("Currently, the bulk import process only supports a single file at a time");
            }

            var uploadedFiles = _fileUploadHandler.SaveFilesToUploadDirectory(bulkFiles,
                fileName => InterchangeFileHelpers.BuildFileNameForImport(model.BulkFileUploadModel.BulkFileType, fileName));

            var connectionInformation = await GetConnectionInformationProvider();

            var config = await _odsSecretConfigurationProvider.GetSecretConfiguration(_instanceContext.Id);
            if (config == null)
            {
                throw new InvalidOperationException("ODS secret configuration can not be null.");
            }

            if (model.BulkFileUploadModel != null)
            {               
                model.BulkFileUploadModel.ApiKey = config.BulkUploadCredential?.ApiKey;
                var validationResult = await _bulkLoadValidator.ValidateAsync(model.BulkFileUploadModel);
                if (!validationResult.IsValid)
                {
                    var errorMessage = string.Join(",", validationResult.Errors.Select(x => x.ErrorMessage));
                    Response.StatusCode = (int) HttpStatusCode.BadRequest;
#if NET48
                    Response.TrySkipIisCustomErrors = true;
#endif
                    return Json(new {Result = new {Errors = new[] {new {ErrorMessage = errorMessage}}}});
                }
            }

#if NET48
            var schemaBasePath = HostingEnvironment.MapPath(_appSettings.XsdFolder);
#else
            var schemaBasePath = Path.Combine(_webHostEnvironment.ContentRootPath, _appSettings.XsdFolder);
#endif
            var standardVersion = _inferOdsApiVersion.EdFiStandardVersion(connectionInformation.ApiServerUrl);
            var odsApiVersion = _inferOdsApiVersion.Version(connectionInformation.ApiServerUrl);

            const int IdealSimultaneousRequests = 20;
            const int PessimisticSimultaneousRequests = 1;

            var maxSimultaneousRequests = odsApiVersion.StartsWith("3.")
                ? PessimisticSimultaneousRequests
                : IdealSimultaneousRequests;

            var jobContext = new BulkUploadJobContext
            {
                DataDirectoryFullPath = uploadedFiles.Directory,
                OdsInstanceId = _instanceContext.Id,
                OdsInstanceName = _instanceContext.Name,
                ApiBaseUrl = connectionInformation.ApiBaseUrl,
                OauthUrl = connectionInformation.OAuthUrl,
                MetadataUrl = connectionInformation.MetadataUrl,
                DependenciesUrl = connectionInformation.DependenciesUrl,
                ClientKey = config.BulkUploadCredential?.ApiKey ?? string.Empty,
                ClientSecret = config.BulkUploadCredential?.ApiSecret ?? string.Empty,
                SchemaPath = $"{schemaBasePath}\\{standardVersion}",
                MaxSimultaneousRequests = maxSimultaneousRequests
            };

            if (!_bulkUploadJob.IsJobRunning())
            {
                _bulkUploadJob.EnqueueJob(jobContext);
            }

            var updatedValue = model;
            updatedValue.BulkFileUploadModel.IsJobRunning = _bulkUploadJob.IsJobRunning();
            updatedValue.BulkFileUploadModel.IsSameOdsInstance = _bulkUploadJob.IsSameOdsInstance(_instanceContext.Id, typeof(BulkUploadJobContext));
            return PartialView("_SignalRStatus_BulkLoad", updatedValue);
        }

        [HttpPost]
        public async Task<ActionResult> SaveBulkLoadCredentials(SaveBulkUploadCredentialsModel model)
        {
            var config = await _odsSecretConfigurationProvider.GetSecretConfiguration(_instanceContext.Id);
            if (config == null) return JsonError(_missingOdsSecretConfig);
            config.BulkUploadCredential = new BulkUploadCredential { ApiKey = model.ApiKey,
                ApiSecret = model.ApiSecret };
            await _odsSecretConfigurationProvider.SetSecretConfiguration(config, _instanceContext.Id);
            return JsonSuccess("Credentials successfully saved");
        }

        [HttpPost]
        public async Task<ActionResult> ResetCredentials()
        {
            var config = await _odsSecretConfigurationProvider.GetSecretConfiguration(_instanceContext.Id);
            if (config == null) return JsonError(_missingOdsSecretConfig);
            if (config.BulkUploadCredential == null) return JsonError("Missing bulk load credentials");
            config.BulkUploadCredential.ApiKey = string.Empty;
            config.BulkUploadCredential.ApiSecret = string.Empty;
            await _odsSecretConfigurationProvider.SetSecretConfiguration(config, _instanceContext.Id);
            return JsonSuccess("Credentials successfully reset");
        }
    }
}
