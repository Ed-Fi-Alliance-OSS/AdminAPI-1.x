// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management;
using Microsoft.AspNetCore.Mvc;
using EdFi.Ods.AdminApp.Management.Configuration.Application;
using EdFi.Ods.AdminApp.Management.Helpers;
using EdFi.Ods.AdminApp.Web.ActionFilters;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using EdFi.Ods.AdminApp.Web.Models.ViewModels;
using Microsoft.Extensions.Options;

namespace EdFi.Ods.AdminApp.Web.Controllers
{
    [BypassSetupRequiredFilter, BypassInstanceContextFilter]
    public class ProductImprovementController : ControllerBase
    {
        private readonly ApplicationConfigurationService _applicationConfigurationService;
        private readonly IProductRegistration _productRegistration;
        private readonly AdminAppUserContext _userContext;
        private readonly AppSettings _appSettings;

        public ProductImprovementController(
            ApplicationConfigurationService applicationConfigurationService,
            IProductRegistration productRegistration,
            IOptions<AppSettings> appSettings,
            AdminAppUserContext userContext)
        {
            _applicationConfigurationService = applicationConfigurationService;
            _productRegistration = productRegistration;
            _appSettings = appSettings.Value;
            _userContext = userContext;
        }

        public ActionResult EditConfiguration()
        {
            GuardForDisabledConfig();
            return View(GetProductImprovementModel());
        }

        [HttpPost]
        public async Task<ActionResult> EditConfiguration(ProductImprovementModel model)
        {
            GuardForDisabledConfig();
            SaveProductImprovementModel(model);
            await _productRegistration.NotifyWhenEnabled(_userContext.User);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult EnableProductImprovementFirstTimeSetup()
        {
            GuardForDisabledConfig();
            return View(GetProductImprovementModel());
        }

        [HttpPost]
        public async Task<ActionResult> EnableProductImprovementFirstTimeSetup(ProductImprovementModel model)
        {
            GuardForDisabledConfig();
            SaveProductImprovementModel(model);
            await _productRegistration.NotifyWhenEnabled(_userContext.User);
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

        private void GuardForDisabledConfig()
        {
            if (!_appSettings.EnableProductImprovementSettings)
            {
                var message = "Product Improvement Configuration is disabled." +
                              "Re-Enable in application settings to make adjustments here.";
                throw new Exception(message);
            }
        }
    }
}
