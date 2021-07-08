// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Management.Helpers;
using EdFi.Ods.AdminApp.Management.Settings;
using EdFi.Ods.AdminApp.Web.ActionFilters;
using EdFi.Ods.AdminApp.Web.Display.TabEnumeration;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using EdFi.Ods.AdminApp.Web.Models.ViewModels;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.OdsInstanceSettings;
using Microsoft.Extensions.Options;
using log4net;

namespace EdFi.Ods.AdminApp.Web.Controllers
{
    public class OdsInstanceSettingsController : ControllerBase
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(OdsInstanceSettingsController));

        private readonly ICloudOdsSettingsService _cloudOdsSettingsService;
        private readonly ITabDisplayService _tabDisplayService;
        private readonly InstanceContext _instanceContext;
        private readonly AppSettings _appSettings;
        private readonly IInferExtensionDetails _inferExtensionDetails;

        public OdsInstanceSettingsController(
             ICloudOdsSettingsService cloudOdsSettingsService
            , ITabDisplayService tabDisplayService 
            , InstanceContext instanceContext
            , IOptions<AppSettings> appSettingsAccessor
            , IInferExtensionDetails inferExtensionDetails
            )
        {
            _cloudOdsSettingsService = cloudOdsSettingsService;
            _tabDisplayService = tabDisplayService;
            _instanceContext = instanceContext;
            _appSettings = appSettingsAccessor.Value;
            _inferExtensionDetails = inferExtensionDetails;
        }

        [AddTelemetry("ODS Instance Settings > Logging", TelemetryType.View)]
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

        public ActionResult OdsVersionMetadata()
        {
            var model = new OdsVersionMetadataViewModel();

            try
            {
                model.OdsApiVersion = InMemoryCache.Instance
                    .GetOrSet("OdsApiVersion", () => new InferOdsApiVersion().Version(CloudOdsAdminAppSettings.Instance.ProductionApiUrl));

                model.TpdmVersion = InMemoryCache.Instance.GetOrSet(
                    "TpdmExtensionVersion", () =>
                        _inferExtensionDetails.TpdmExtensionVersion(
                            CloudOdsAdminAppSettings.Instance.ProductionApiUrl));
            }
            catch (Exception exception)
            {
                _logger.Error("Failed to infer the version of the ODS / API and its extension. This can happen when the ODS / API is unreachable.", exception);
            }

            return PartialView(model);
        }
    }
}
