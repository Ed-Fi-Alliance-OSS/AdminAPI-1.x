// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Helpers;
using Microsoft.AspNetCore.Mvc;
using EdFi.Ods.AdminApp.Web.Models;
using Microsoft.Extensions.Options;

namespace EdFi.Ods.AdminApp.Web.Controllers
{
    public class TestAppSettingsController : Controller
    {
        private readonly AppSettings _appSettings;

        public TestAppSettingsController(IOptions<AppSettings> appSettingsAccessor)
        {
            _appSettings = appSettingsAccessor.Value;
        }

        public IActionResult Index()
        {
            var model = new TestAppSettingsModel
            {
                Settings1 = _appSettings.XsdFolder,
                Settings2 = _appSettings.DefaultOdsInstance,
                Settings3 = _appSettings.ApiStartupType,
                Settings4 = _appSettings.OptionalEntropy
            };

            return View(model);
        }
    }
}
