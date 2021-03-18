// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Management.Helpers;
using EdFi.Ods.AdminApp.Web.Display.TabEnumeration;
using EdFi.Ods.AdminApp.Web.Infrastructure.IO;
using EdFi.Ods.AdminApp.Web.Infrastructure.Jobs;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.BulkUpload;
using FluentValidation;
using log4net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace EdFi.Ods.AdminApp.Web.Controllers
{
    public class BulkUploadController : ControllerBase
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(BulkUploadController));
        private readonly IOdsSecretConfigurationProvider _odsSecretConfigurationProvider;
        private readonly IOdsApiConnectionInformationProvider _apiConnectionInformationProvider;
        private readonly ITabDisplayService _tabDisplayService;
        private readonly string _missingOdsSecretConfig = "ODS secret configuration can not be null";
        private readonly IFileUploadHandler _fileUploadHandler;
        private readonly IBulkUploadJob _bulkUploadJob;
        private readonly InstanceContext _instanceContext;
        private readonly IInferOdsApiVersion _inferOdsApiVersion;
        private readonly IValidator<BulkFileUploadModel> _bulkLoadValidator;
        private readonly AppSettings _appSettings;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BulkUploadController(
            ITabDisplayService tabDisplayService
            , IOdsSecretConfigurationProvider odsSecretConfigurationProvider
            , IOdsApiConnectionInformationProvider apiConnectionInformationProvider
            , IFileUploadHandler fileUploadHandler
            , IBulkUploadJob bulkUploadJob
            , InstanceContext instanceContext
            , IInferOdsApiVersion inferOdsApiVersion
            , IValidator<BulkFileUploadModel> bulkLoadValidator
            , IOptions<AppSettings> appSettingsAccessor
            , IWebHostEnvironment webHostEnvironment
            )
        {
            _tabDisplayService = tabDisplayService;
            _odsSecretConfigurationProvider = odsSecretConfigurationProvider;
            _apiConnectionInformationProvider = apiConnectionInformationProvider;
            _fileUploadHandler = fileUploadHandler;
            _bulkUploadJob = bulkUploadJob;
            _instanceContext = instanceContext;
            _inferOdsApiVersion = inferOdsApiVersion;
            _bulkLoadValidator = bulkLoadValidator;
            _appSettings = appSettingsAccessor.Value;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<ActionResult> Index()
        {
            var config = await _odsSecretConfigurationProvider.GetSecretConfiguration(_instanceContext.Id);
            if (config == null)
            {
                _logger.Error(_missingOdsSecretConfig);
                return JsonError(_missingOdsSecretConfig);
            }
            var model = new BulkFileUploadIndexModel
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

            ViewData["PathBase"] = Request != null ? Request.PathBase.ToString() : "";

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> BulkFileUpload(BulkFileUploadModel model)
        {
            var bulkFiles = model.BulkFiles.Where(file => file != null && file.Length > 0).ToArray();

            if (!bulkFiles.Any())
            {
                throw new Exception("The given file is empty. Please upload a file compatible with the Ed-Fi Data Standard.");
            }

            if (bulkFiles.Sum(f => f.Length) > BulkFileUploadModel.MaxFileSize)
            {
                throw new Exception($"Upload exceeds maximum limit of {BulkFileUploadModel.MaxFileSize} bytes");
            }

            if (bulkFiles.Length > 1)
            {
                throw new Exception("Currently, the bulk import process only supports a single file at a time");
            }


            InterchangeFileType bulkFileUploadType = null;
            if (model.BulkFileType != null)
                bulkFileUploadType = InterchangeFileType.FromInt32(model.BulkFileType.Value);
            var uploadedFiles = _fileUploadHandler.SaveFilesToUploadDirectory(bulkFiles,
                fileName => InterchangeFileHelpers.BuildFileNameForImport(bulkFileUploadType, fileName), _webHostEnvironment);

            var connectionInformation = await GetConnectionInformationProvider();

            var config = await _odsSecretConfigurationProvider.GetSecretConfiguration(_instanceContext.Id);
            if (config == null)
            {
                throw new InvalidOperationException("ODS secret configuration can not be null.");
            }

            if (model != null)
            {
                model.ApiKey = config.BulkUploadCredential?.ApiKey;
                var validationResult = await _bulkLoadValidator.ValidateAsync(model);
                if (!validationResult.IsValid)
                {
                    var errorMessage = string.Join(",", validationResult.Errors.Select(x => x.ErrorMessage));
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { Result = new { Errors = new[] { new { ErrorMessage = errorMessage } } } });
                }
            }

            var schemaBasePath = Path.Combine(_webHostEnvironment.ContentRootPath, _appSettings.XsdFolder);
            var standardVersion = _inferOdsApiVersion.EdFiStandardVersion(connectionInformation.ApiServerUrl);
            var odsApiVersion = _inferOdsApiVersion.Version(connectionInformation.ApiServerUrl);

            const int IdealSimultaneousRequests = 20;
            const int PessimisticSimultaneousRequests = 1;

            var maxSimultaneousRequests = odsApiVersion.StartsWith("3.")
                ? PessimisticSimultaneousRequests
                : IdealSimultaneousRequests;

            var jobContext = new BulkUploadJobContext
            {
                ApiServerUrl = connectionInformation.ApiServerUrl,
                DataDirectoryFullPath = uploadedFiles.Directory,
                OdsInstanceId = _instanceContext.Id,
                OdsInstanceName = _instanceContext.Name,
                ApiBaseUrl = connectionInformation.ApiBaseUrl,
                OauthUrl = connectionInformation.OAuthUrl,
                MetadataUrl = connectionInformation.MetadataUrl,
                DependenciesUrl = connectionInformation.DependenciesUrl,
                ClientKey = config.BulkUploadCredential?.ApiKey ?? string.Empty,
                ClientSecret = config.BulkUploadCredential?.ApiSecret ?? string.Empty,
                SchemaPath = Path.Combine(schemaBasePath,standardVersion),
                MaxSimultaneousRequests = maxSimultaneousRequests
            };

            if (!_bulkUploadJob.IsJobRunning())
            {
                _bulkUploadJob.EnqueueJob(jobContext);
            }

            var updatedValue = model;
            updatedValue.IsJobRunning = _bulkUploadJob.IsJobRunning();
            updatedValue.IsSameOdsInstance = _bulkUploadJob.IsSameOdsInstance(_instanceContext.Id, typeof(BulkUploadJobContext));
            return PartialView("_SignalRStatus_BulkLoad", updatedValue);
        }

        [HttpPost]
        public async Task<ActionResult> SaveBulkLoadCredentials(SaveBulkUploadCredentialsModel model)
        {
            var config = await _odsSecretConfigurationProvider.GetSecretConfiguration(_instanceContext.Id);
            if (config == null) return JsonError(_missingOdsSecretConfig);
            config.BulkUploadCredential = new BulkUploadCredential
            {
                ApiKey = model.ApiKey,
                ApiSecret = model.ApiSecret
            };
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

        private async Task<OdsApiConnectionInformation> GetConnectionInformationProvider()
        {
            return await _apiConnectionInformationProvider.GetConnectionInformationForEnvironment();
        }
    }
}
