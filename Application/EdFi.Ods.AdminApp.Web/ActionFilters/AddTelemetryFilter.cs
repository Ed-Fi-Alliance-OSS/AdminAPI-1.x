// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Database.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using EdFi.Ods.AdminApp.Web.Controllers;
using EdFi.Ods.AdminApp.Web.Helpers;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace EdFi.Ods.AdminApp.Web.ActionFilters
{
    public class AddTelemetryFilter : IAsyncActionFilter
    {
        private readonly string _action;
        private readonly bool _isView;
        private readonly ITelemetry _telemetry;

        public AddTelemetryFilter(string action, bool isView, ITelemetry telemetry)
        {
            _action = action;
            _isView = isView;
            _telemetry = telemetry;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
        {
            if(!_isView)
                await _telemetry.Event(_action);
            else
                await _telemetry.View(_action);

            await next();
        }
    }
}
