// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Ods.AdminApp.Web.ActionFilters;
using EdFi.Ods.AdminApp.Web.Display.HomeScreen;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.Home;

namespace EdFi.Ods.AdminApp.Web.Controllers
{
    [AllowAnonymous, BypassInstanceContextFilter]
    public class HomeController : ControllerBase
    {
        private readonly IHomeScreenDisplayService _homeScreenDisplayService;
        private readonly IGetOdsInstanceRegistrationsQuery _getOdsInstanceRegistrationsQuery;


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
        public ActionResult PostSetup(bool setupCompleted = false, bool isRestartRequired = false)
        {
            if (setupCompleted && isRestartRequired)
            {
                return View();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Error()
        {
            return View();
        }

        private bool ZeroOdsInstanceRegistrations()
        {
            return CloudOdsAdminAppSettings.Instance.Mode.SupportsMultipleInstances &&
                   !_getOdsInstanceRegistrationsQuery.Execute().Any();
        }
    }
}
