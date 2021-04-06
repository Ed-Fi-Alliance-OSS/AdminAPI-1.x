// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Helpers;
using EdFi.Ods.AdminApp.Management.Settings;
using EdFi.Ods.AdminApp.Web.ActionFilters;
using EdFi.Ods.AdminApp.Web.Display.TabEnumeration;
using EdFi.Ods.AdminApp.Web.Models.ViewModels;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.OdsInstanceSettings;
using Microsoft.Extensions.Options;

namespace EdFi.Ods.AdminApp.Web.Controllers
{
    public class OdsInstanceSettingsController : ControllerBase
    {
        private readonly ICloudOdsSettingsService _cloudOdsSettingsService;
        private readonly ITabDisplayService _tabDisplayService;
        private readonly InstanceContext _instanceContext;
        private readonly AppSettings _appSettings;

        public OdsInstanceSettingsController(
             ICloudOdsSettingsService cloudOdsSettingsService
            , ITabDisplayService tabDisplayService 
            , InstanceContext instanceContext
            , IOptions<AppSettings> appSettingsAccessor
            )
        {
            _cloudOdsSettingsService = cloudOdsSettingsService;
            _tabDisplayService = tabDisplayService;
            _instanceContext = instanceContext;
            _appSettings = appSettingsAccessor.Value;
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
    }
}
