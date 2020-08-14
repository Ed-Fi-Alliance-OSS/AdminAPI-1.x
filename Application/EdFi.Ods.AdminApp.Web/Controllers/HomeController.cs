// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
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
        private readonly CloudOdsUpdateService _cloudOdsUpdateService;
        private readonly IHomeScreenDisplayService _homeScreenDisplayService;
        private readonly IGetOdsInstanceRegistrationsQuery _getOdsInstanceRegistrationsQuery;


        public HomeController(CloudOdsUpdateService cloudOdsUpdateService, IHomeScreenDisplayService homeScreenDisplayService, IGetOdsInstanceRegistrationsQuery getOdsInstanceRegistrationsQuery)
        {
            _cloudOdsUpdateService = cloudOdsUpdateService;
            _homeScreenDisplayService = homeScreenDisplayService;
            _getOdsInstanceRegistrationsQuery = getOdsInstanceRegistrationsQuery;
        }

        public async Task<ActionResult> Index(bool setupCompleted = false)
        {
            if (setupCompleted && ZeroOdsInstanceRegistrations())
                return RedirectToAction("RegisterOdsInstance", "OdsInstances");

            var updateInfo = await _cloudOdsUpdateService.GetUpdateInfo();
            var model = new IndexModel
            {
                SetupJustCompleted = setupCompleted,
                UpdateAvailable = updateInfo.UpdateAvailable,
                HomeScreenDisplays = _homeScreenDisplayService.GetHomeScreenDisplays()
            };

            return View(model);
        }

        public ActionResult Error(string message)
        {
            var model = new HandleErrorInfo(new Exception(message), "Home", "Index");
            return View(model);
        }

        private bool ZeroOdsInstanceRegistrations()
        {
            return CloudOdsAdminAppSettings.Instance.Mode.SupportsMultipleInstances &&
                   !_getOdsInstanceRegistrationsQuery.Execute().Any();
        }
    }
}