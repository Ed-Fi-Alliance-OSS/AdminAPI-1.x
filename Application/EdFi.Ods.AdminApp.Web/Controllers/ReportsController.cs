// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Database.Ods;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.Reports;
using System.Linq;
using EdFi.Ods.AdminApp.Web.Display.TabEnumeration;
#if NET48
using System.Web.Mvc;
#else
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
#endif
using EdFi.Ods.AdminApp.Web.Infrastructure;

namespace EdFi.Ods.AdminApp.Web.Controllers
{
    public class ReportsController : ControllerBase
    {
        private readonly GetAllLocalEducationAgenciesQuery _getAllLocalEducationAgenciesQuery;
        private readonly StudentsByProgramQuery _studentsByProgramQuery;
        private readonly StudentEconomicSituationReportQuery _studentEconomicSituationReportQuery;
        private readonly TotalEnrollmentQuery _totalEnrollmentQuery;
        private readonly GetSchoolsBySchoolTypeQuery _getSchoolsBySchoolTypeQuery;
        private readonly StudentEnrollmentByGenderQuery _studentEnrollmentByGenderQuery;
        private readonly StudentEnrollmentByRaceQuery _studentEnrollmentByRaceQuery;
        private readonly StudentEnrollmentByEthnicityQuery _studentEnrollmentByEthnicityQuery;
        private readonly InstanceContext _instanceContext;
        private readonly ITabDisplayService _tabDisplayService;

        public ReportsController(GetAllLocalEducationAgenciesQuery getAllLocalEducationAgenciesQuery
            , StudentsByProgramQuery studentsByProgramQuery
            , StudentEconomicSituationReportQuery studentEconomicSituationReportQuery
            , TotalEnrollmentQuery totalEnrollmentQuery
            , GetSchoolsBySchoolTypeQuery getSchoolsBySchoolTypeQuery
            , StudentEnrollmentByGenderQuery studentEnrollmentByGenderQuery
            , StudentEnrollmentByRaceQuery studentEnrollmentByRaceQuery
            , StudentEnrollmentByEthnicityQuery studentEnrollmentByEthnicityQuery
            , InstanceContext instanceContext
            , ITabDisplayService tabDisplayService)
        {
            _getAllLocalEducationAgenciesQuery = getAllLocalEducationAgenciesQuery;
            _studentsByProgramQuery = studentsByProgramQuery;
            _studentEconomicSituationReportQuery = studentEconomicSituationReportQuery;
            _totalEnrollmentQuery = totalEnrollmentQuery;
            _getSchoolsBySchoolTypeQuery = getSchoolsBySchoolTypeQuery;
            _studentEnrollmentByGenderQuery = studentEnrollmentByGenderQuery;
            _studentEnrollmentByRaceQuery = studentEnrollmentByRaceQuery;
            _studentEnrollmentByEthnicityQuery = studentEnrollmentByEthnicityQuery;
            _instanceContext = instanceContext;
            _tabDisplayService = tabDisplayService;
        }

        public ActionResult Index()
        {
            return RedirectToAction("SelectDistrict", "OdsInstanceSettings");
        }

        public ActionResult SelectDistrict(int localEducationAgencyId)
        {
            var districtOptions = _getAllLocalEducationAgenciesQuery
                .Execute(_instanceContext.Name, CloudOdsAdminAppSettings.Instance.Mode)
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.LocalEducationAgencyId.ToString(),
                    Selected = x.LocalEducationAgencyId == localEducationAgencyId
                }).ToArray();

            var model = new SelectDistrictModel
            {
                LocalEducationAgencyId = localEducationAgencyId,
                DistrictOptions = districtOptions
            };

            return PartialView("_SelectDistrict", model);
        }

        public ActionResult TotalEnrollment(int id)
        {
            var reportsModel = GetReportModel(id);
            reportsModel.TotalEnrollmentReport = _totalEnrollmentQuery.Execute(_instanceContext.Name, CloudOdsAdminAppSettings.Instance.Mode, id);

            return PartialView("_TotalEnrollment", reportsModel);
        }

        public ActionResult SchoolsBySchoolType(int id)
        {
            var reportsModel = GetReportModel(id);
            reportsModel.SchoolTypeReport= _getSchoolsBySchoolTypeQuery.Execute(_instanceContext.Name, CloudOdsAdminAppSettings.Instance.Mode, id);

            return PartialView("_SchoolsBySchoolType", reportsModel);
        }

        public ActionResult EnrollmentByGender(int id)
        {
            var reportsModel = GetReportModel(id);
            reportsModel.StudentGenderReport = _studentEnrollmentByGenderQuery.Execute(_instanceContext.Name, CloudOdsAdminAppSettings.Instance.Mode, id);

            return PartialView("_EnrollmentByGender", reportsModel);
        }

        public ActionResult EnrollmentByRace(int id)
        {
            var raceReport = _studentEnrollmentByRaceQuery.Execute(_instanceContext.Name, CloudOdsAdminAppSettings.Instance.Mode, id);
            return PartialView("_EnrollmentByRace", raceReport);
        }

        public ActionResult EnrollmentByEthnicity(int id)
        {
            var ethnicityReport = _studentEnrollmentByEthnicityQuery.Execute(_instanceContext.Name, CloudOdsAdminAppSettings.Instance.Mode, id);
            return PartialView("_EnrollmentByEthnicity", ethnicityReport);
        }

        public ActionResult StudentsByProgram(int id)
        {
            var model = _studentsByProgramQuery.Execute(_instanceContext.Name, CloudOdsAdminAppSettings.Instance.Mode, id);
            return PartialView("_StudentsByProgram",model);
        }

        public ActionResult StudentsByAttribute(int id)
        {
            var model = _studentEconomicSituationReportQuery.Execute(_instanceContext.Name, CloudOdsAdminAppSettings.Instance.Mode, id);
            return PartialView("_StudentsByAttribute", model);
        }

        ReportsIndexModel GetReportModel(int id)
        {
            var model = new ReportsIndexModel
            {
                ReportsModel = new ReportsModel
                {
                    LocalEducationAgencyId = id
                },
                OdsInstanceSettingsTabEnumerations =
                    _tabDisplayService.GetOdsInstanceSettingsTabDisplay(OdsInstanceSettingsTabEnumeration.Reports),
                OdsInstance = _instanceContext
            };
            return model;
        }
    }
}
