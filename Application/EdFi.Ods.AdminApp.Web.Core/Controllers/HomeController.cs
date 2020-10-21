// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EdFi.Ods.AdminApp.Web.Models;
using log4net;

namespace EdFi.Ods.AdminApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(HomeController));

        public IActionResult Index()
        {
            _logger.Info("TEST INFO LOGGING");
            _logger.Error("TEST ERROR LOGGING");
            _logger.Debug("TEST DEBUG LOGGING");
            _logger.Warn("TEST WARN LOGGING");

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
