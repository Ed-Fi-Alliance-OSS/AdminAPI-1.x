// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EdFi.Ods.AdminApp.Management.Configuration.Application;
using EdFi.Ods.AdminApp.Web.ActionFilters;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using EdFi.Ods.AdminApp.Web.Models.ViewModels;

namespace EdFi.Ods.AdminApp.Web.Controllers
{
    [BypassSetupRequiredFilter, BypassInstanceContextFilter]
    public class ProductImprovementController : ControllerBase
    {
        private readonly ApplicationConfigurationService _applicationConfigurationService;
        private readonly IProductRegistration _productRegistration;

        public ProductImprovementController(
            ApplicationConfigurationService applicationConfigurationService,
            IProductRegistration productRegistration
            )
        {
            _applicationConfigurationService = applicationConfigurationService;
            _productRegistration = productRegistration;
        }

        public ActionResult EditConfiguration()
        {
            return View(GetProductImprovementModel());
        }

        [HttpPost]
        public async Task<ActionResult> EditConfiguration(ProductImprovementModel model)
        {
            SaveProductImprovementModel(model);
            await _productRegistration.NotifyWhenEnabled();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult EnableProductImprovementFirstTimeSetup()
        {
            return View(GetProductImprovementModel());
        }

        [HttpPost]
        public async Task<ActionResult> EnableProductImprovementFirstTimeSetup(ProductImprovementModel model)
        {
            SaveProductImprovementModel(model);
            await _productRegistration.NotifyWhenEnabled();
            return RedirectToAction("PostSetup", "Home", new {setupCompleted = true});
        }

        private ProductImprovementModel GetProductImprovementModel()
        {
            var enableProductImprovement =
                _applicationConfigurationService.IsProductImprovementEnabled(out var productRegistrationId);

            return new ProductImprovementModel
            {
                EnableProductImprovement = enableProductImprovement,
                ProductRegistrationId = productRegistrationId
            };
        }

        private void SaveProductImprovementModel(ProductImprovementModel model)
        {
            _applicationConfigurationService.EnableProductImprovement(model.EnableProductImprovement, model.ProductRegistrationId);
        }
    }
}
