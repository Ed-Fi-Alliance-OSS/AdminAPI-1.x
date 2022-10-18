// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Database.Ods.SchoolYears;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace EdFi.Ods.AdminApp.Web.Controllers
{
    public class SingleInstanceCurrentYearViewComponent : ViewComponent
    {
        public class Model
        {
            public string InstanceName { get; set; }
            public string SchoolYearDescription { get; set; }
        }

        private readonly InstanceContext _instanceContext;
        private readonly GetCurrentSchoolYearQuery _getCurrentSchoolYear;

        public SingleInstanceCurrentYearViewComponent(
            InstanceContext instanceContext,
            GetCurrentSchoolYearQuery getCurrentSchoolYear)
        {
            _instanceContext = instanceContext;
            _getCurrentSchoolYear = getCurrentSchoolYear;
        }

        public IViewComponentResult Invoke()
        {
            var apiMode = CloudOdsAdminAppSettings.Instance.Mode;

            if (apiMode.SupportsMultipleInstances)
                throw new InvalidOperationException(
                    $"Attempted to display {nameof(SingleInstanceCurrentYearViewComponent)}, but the " +
                    $"ODS is configured with multi-instance API mode '{apiMode}'.");

            var schoolYear = _getCurrentSchoolYear.Execute(_instanceContext.Name, apiMode);

            return View(
                new Model
                {
                    InstanceName = _instanceContext.Name,
                    SchoolYearDescription = schoolYear?.SchoolYearDescription
                });
        }
    }
}
