// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Helpers;
using Microsoft.AspNetCore.Mvc.Filters;
using EdFi.Ods.AdminApp.Web.Controllers;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using log4net;
using Microsoft.Extensions.Options;

namespace EdFi.Ods.AdminApp.Web.ActionFilters
{
    public class AddTelemetryFilter : IAsyncActionFilter
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(AddTelemetryFilter));

        private readonly string _action;
        private readonly TelemetryType _type;
        private readonly ITelemetry _telemetry;
        private readonly AppSettings _appSettings;

        public AddTelemetryFilter(string action, TelemetryType type, ITelemetry telemetry, IOptions<AppSettings> appSettingsAccessor)
        {
            _action = action;
            _type = type;
            _telemetry = telemetry;
            _appSettings = appSettingsAccessor.Value;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
        {
            if (_appSettings.EnableProductImprovement && !string.IsNullOrEmpty(_appSettings.GoogleAnalyticsMeasurementId))
            {
                try
                {
                    if (_type == TelemetryType.Event)
                        await _telemetry.Event(_action);
                    else
                        await _telemetry.View(_action);
                }
                catch (Exception e)
                {
                    _logger.Error("Google Analytics Add Telemetry Filter failed", e);
                }
            }

            await next();
        }
    }
}
