// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Configuration.Application;
using EdFi.Ods.AdminApp.Management.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EdFi.Ods.AdminApp.Web.Controllers
{
    public class TrackerViewComponent : ViewComponent
    {
        private readonly AdminAppUserContext _adminAppUserContext;
        private readonly ApplicationConfigurationService _applicationConfigurationService;
        private readonly AppSettings _appSettings;

        public class Model
        {
            public bool EnableProductImprovement { get; set; }
            public string MeasurementId { get; set; }
            public string UserId { get; set; }
        }

        public TrackerViewComponent(AdminAppUserContext adminAppUserContext, ApplicationConfigurationService applicationConfigurationService, IOptions<AppSettings> appSettingsAccessor)
        {
            _adminAppUserContext = adminAppUserContext;
            _applicationConfigurationService = applicationConfigurationService;
            _appSettings = appSettingsAccessor.Value;
        }

        public IViewComponentResult Invoke()
        {
            return View(
                new Model
                {
                    EnableProductImprovement = _applicationConfigurationService.IsProductImprovementEnabled(),
                    MeasurementId = _appSettings.GoogleAnalyticsMeasurementId,
                    UserId = _adminAppUserContext?.User?.Id
                });
        }
    }
}
