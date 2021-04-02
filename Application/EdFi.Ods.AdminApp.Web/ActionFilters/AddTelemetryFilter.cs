// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using EdFi.Ods.AdminApp.Web.Controllers;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using log4net;

namespace EdFi.Ods.AdminApp.Web.ActionFilters
{
    public class AddTelemetryFilter : IAsyncActionFilter
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(SetupController));

        private readonly string _action;
        private readonly TelemetryType _type;
        private readonly ITelemetry _telemetry;

        public AddTelemetryFilter(string action, TelemetryType type, ITelemetry telemetry)
        {
            _action = action;
            _type = type;
            _telemetry = telemetry;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
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

            await next();
        }
    }
}
