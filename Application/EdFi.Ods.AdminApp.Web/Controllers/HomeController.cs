// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Ods.AdminApp.Web.ActionFilters;
using EdFi.Ods.AdminApp.Web.Display.HomeScreen;
using EdFi.Ods.AdminApp.Web.Helpers;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.Home;
using log4net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Data.SqlClient;

namespace EdFi.Ods.AdminApp.Web.Controllers
{
    [AllowAnonymous, BypassInstanceContextFilter]
    public class HomeController : ControllerBase
    {
        private readonly IHomeScreenDisplayService _homeScreenDisplayService;
        private readonly IGetOdsInstanceRegistrationsQuery _getOdsInstanceRegistrationsQuery;
        private readonly ILog _logger = LogManager.GetLogger(typeof(HomeController));


        public HomeController(IHomeScreenDisplayService homeScreenDisplayService, IGetOdsInstanceRegistrationsQuery getOdsInstanceRegistrationsQuery)
        {
            _homeScreenDisplayService = homeScreenDisplayService;
            _getOdsInstanceRegistrationsQuery = getOdsInstanceRegistrationsQuery;
        }

        [AddTelemetry("Home Index", TelemetryType.View)]
        public ActionResult Index(bool setupCompleted = false)
        {
            if (setupCompleted && ZeroOdsInstanceRegistrations())
                return RedirectToAction("RegisterOdsInstance", "OdsInstances");

            var model = new IndexModel
            {
                SetupJustCompleted = setupCompleted,
                HomeScreenDisplays = _homeScreenDisplayService.GetHomeScreenDisplays()
            };

            return View(model);
        }

        [AddTelemetry("Post Setup", TelemetryType.View)]
        public ActionResult PostSetup(bool setupCompleted = false)
        {
            bool.TryParse(Request.Cookies["RestartRequired"], out var isRestartRequired);
            Response.Cookies.Delete("RestartRequired");

            if (setupCompleted && isRestartRequired)
            {
                return View();
            }

            return RedirectToAction("Index", new { setupCompleted });
        }

        public ActionResult Error()
        {
            var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var exception = exceptionHandlerFeature.Error;

            _logger.Error(exception);

            if (HttpContext.Request.IsAjaxRequest())
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, exception.Message);
            }

            return View();
        }

        private bool IsReportsController(string controllerName) => controllerName.ToLower().Equals("reports");


        private bool ZeroOdsInstanceRegistrations()
        {
            return CloudOdsAdminAppSettings.Instance.Mode.SupportsMultipleInstances &&
                   !_getOdsInstanceRegistrationsQuery.Execute().Any();
        }
    }
}
