// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using Microsoft.AspNetCore.Mvc;
using EdFi.Ods.AdminApp.Management.Configuration.Application;
using EdFi.Ods.AdminApp.Web.ActionFilters;
using EdFi.Ods.AdminApp.Web.Models.ViewModels;

namespace EdFi.Ods.AdminApp.Web.Controllers
{
    [BypassSetupRequiredFilter, BypassInstanceContextFilter]
    public class ProductImprovementController : ControllerBase
    {
        private readonly ApplicationConfigurationService _applicationConfigurationService;

        public ProductImprovementController(ApplicationConfigurationService applicationConfigurationService)
        {
            _applicationConfigurationService = applicationConfigurationService;
        }

        public ActionResult EditConfiguration()
        {
            var model = new ProductImprovementModel
            {
                EnableProductImprovement = _applicationConfigurationService.IsProductImprovementEnabled()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult EditConfiguration(ProductImprovementModel model)
        {
            _applicationConfigurationService.EnableProductImprovement(model.EnableProductImprovement);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult EnableProductImprovementFirstTimeSetup()
        {
            var model = new ProductImprovementModel
            {
                EnableProductImprovement = _applicationConfigurationService.IsProductImprovementEnabled()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult EnableProductImprovementFirstTimeSetup(ProductImprovementModel model)
        {
            _applicationConfigurationService.EnableProductImprovement(model.EnableProductImprovement);
            return RedirectToAction("PostSetup", "Home", new {setupCompleted = true});
        }
    }
}
