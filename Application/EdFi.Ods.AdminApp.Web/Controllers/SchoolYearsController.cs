// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Database.Ods.SchoolYears;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.SchoolYears;
using Microsoft.AspNetCore.Mvc;

namespace EdFi.Ods.AdminApp.Web.Controllers
{
    public class SchoolYearsController : ControllerBase
    {
        private readonly InstanceContext _instanceContext;
        private readonly GetSchoolYearsQuery _getSchoolYears;
        private readonly GetCurrentSchoolYearQuery _getCurrentSchoolYear;
        private readonly SetCurrentSchoolYearCommand _setCurrentSchoolYear;

        public SchoolYearsController(
            InstanceContext instanceContext,
            GetSchoolYearsQuery getSchoolYears,
            GetCurrentSchoolYearQuery getCurrentSchoolYear,
            SetCurrentSchoolYearCommand setCurrentSchoolYear)
        {
            _instanceContext = instanceContext;
            _getSchoolYears = getSchoolYears;
            _getCurrentSchoolYear = getCurrentSchoolYear;
            _setCurrentSchoolYear = setCurrentSchoolYear;
        }

        public ActionResult Index()
        {
            return Json(_getSchoolYears.Execute(_instanceContext.Name, ApiMode));
        }

        public ActionResult Edit()
        {
            var currentSchoolYear = _getCurrentSchoolYear.Execute(_instanceContext.Name, ApiMode)?.SchoolYear;
            var schoolYears = _getSchoolYears.Execute(_instanceContext.Name, ApiMode);

            return PartialView(
                new EditSchoolYearModel
                {
                    SchoolYear = currentSchoolYear,
                    SchoolYears = schoolYears
                        .ToSelectListItems(
                            "Select School Year",
                            x => x.SchoolYear.ToString(),
                            x => x.SchoolYearDescription)
                });
        }

        [HttpPost]
        public ActionResult Edit(EditSchoolYearModel model)
        {
            _setCurrentSchoolYear.Execute(_instanceContext.Name, ApiMode, model.SchoolYear.Value);

            return JsonSuccess("School Year Saved");
        }

        private static ApiMode ApiMode => CloudOdsAdminAppSettings.Instance.Mode;
    }
}
