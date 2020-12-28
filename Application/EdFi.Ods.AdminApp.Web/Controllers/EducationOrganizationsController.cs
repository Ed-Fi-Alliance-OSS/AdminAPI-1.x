// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Threading.Tasks;
using AutoMapper;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Management.Api.Models;
using EdFi.Ods.AdminApp.Web.Models.ViewModels;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.EducationOrganizations;
using Microsoft.AspNetCore.Mvc;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Helpers;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Ods.AdminApp.Web.Display.TabEnumeration;
using EdFi.Ods.AdminApp.Web.Infrastructure;

namespace EdFi.Ods.AdminApp.Web.Controllers
{
    public class EducationOrganizationsController : ControllerBase
    {
        private readonly IOdsApiFacadeFactory _odsApiFacadeFactory;
        private readonly IMapper _mapper;
        private readonly InstanceContext _instanceContext;
        private readonly ITabDisplayService _tabDisplayService;

        public EducationOrganizationsController(IOdsApiFacadeFactory odsApiFacadeFactory
            , IMapper mapper, InstanceContext instanceContext, ITabDisplayService tabDisplayService)
        {
            _odsApiFacadeFactory = odsApiFacadeFactory;
            _mapper = mapper;
            _instanceContext = instanceContext;
            _tabDisplayService = tabDisplayService;
        }

        public ActionResult Index()
        {
            var model = new EducationOrganizationsIndexModel
            {
                OdsInstanceSettingsTabEnumerations =
                    _tabDisplayService.GetOdsInstanceSettingsTabDisplay(OdsInstanceSettingsTabEnumeration
                        .EducationOrganizations),
                OdsInstance = _instanceContext
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> AddLocalEducationAgency(AddLocalEducationAgencyModel viewModel)
        {
            var model = _mapper.Map<LocalEducationAgency>(viewModel);
            model.Id = Guid.Empty.ToString();
            var addResult = (await _odsApiFacadeFactory.Create()).AddLocalEducationAgency(model);
            return addResult.Success ? JsonSuccess("Organization Added") : JsonError(addResult.ErrorMessage);
        }

        [HttpPost]
        public async Task<ActionResult> AddSchool(AddSchoolModel viewModel)
        {
            var model = _mapper.Map<School>(viewModel);
            model.Id = Guid.Empty.ToString();
            var addResult = (await _odsApiFacadeFactory.Create()).AddSchool(model);
            return addResult.Success ? JsonSuccess("School Added") : JsonError(addResult.ErrorMessage);
        }

        public async Task<ActionResult> EditLocalEducationAgencyModal(string id)
        {
            var api = await _odsApiFacadeFactory.Create();
            var educationAgency = api.GetLocalEducationAgencyById(id);
            var localEducationAgencyCategoryOptions = api.GetLocalEducationAgencyCategories();
            var stateOptions = api.GetAllStateAbbreviations();

            var model = _mapper.Map<EditLocalEducationAgencyModel>(educationAgency);
            model.LocalEducationAgencyCategoryTypeOptions = localEducationAgencyCategoryOptions;
            model.StateOptions = stateOptions;

            return PartialView("_EditLocalEducationAgencyModal", model);
        }

        [HttpPost]
        public async Task<ActionResult> EditLocalEducationAgency(EditLocalEducationAgencyModel model)
        {
            var editResult = (await _odsApiFacadeFactory.Create()).EditLocalEducationAgency(_mapper.Map<LocalEducationAgency>(model));
            return editResult.Success ? JsonSuccess("Organization Updated") : JsonError(editResult.ErrorMessage);
        }

        public async Task<ActionResult> EditSchoolModal(string id)
        {
            var api = await _odsApiFacadeFactory.Create();
            var school = api.GetSchoolById(id);
            var gradeLevelOptions = api.GetAllGradeLevels();
            var stateOptions = api.GetAllStateAbbreviations();

            var model = _mapper.Map<EditSchoolModel>(school);
            model.GradeLevelOptions = gradeLevelOptions;
            model.StateOptions = stateOptions;

            return PartialView("_EditSchoolModal", model);
        }

        [HttpPost]
        public async Task<ActionResult> EditSchool(EditSchoolModel model)
        {
            var editResult = (await _odsApiFacadeFactory.Create()).EditSchool(_mapper.Map<School>(model));
            return editResult.Success ? JsonSuccess("School Updated") : JsonError(editResult.ErrorMessage);
        }

        public async Task<ActionResult> EducationOrganizationList()
        {
            var api = await _odsApiFacadeFactory.Create();
            var schools = api.GetAllSchools();
            var localEducationAgencies = api.GetAllLocalEducationAgencies();

            var requiredApiDataExist = (await _odsApiFacadeFactory.Create()).DoesApiDataExist();

            var model = new EducationOrganizationViewModel
            {
                Schools = schools,
                LocalEducationAgencies = localEducationAgencies,
                ShouldAllowMultipleDistricts = CloudOdsAdminAppSettings.Instance.Mode != ApiMode.DistrictSpecific,
                AddSchoolModel = new AddSchoolModel
                {
                    GradeLevelOptions = api.GetAllGradeLevels(),
                    StateOptions = api.GetAllStateAbbreviations(),
                    RequiredApiDataExist = requiredApiDataExist
                },
                AddLocalEducationAgencyModel = new AddLocalEducationAgencyModel
                {
                    LocalEducationAgencyCategoryTypeOptions = api.GetLocalEducationAgencyCategories(),
                    StateOptions = api.GetAllStateAbbreviations(),
                    RequiredApiDataExist = requiredApiDataExist
                }
            };

            if (CloudOdsAdminAppSettings.Instance.Mode == ApiMode.DistrictSpecific)
            {
                model.AddLocalEducationAgencyModel.LocalEducationAgencyId = OdsInstanceIdentityHelper.GetIdentityValue(_instanceContext.Name);
            }

            return PartialView("_EducationOrganizations", model);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteLocalEducationAgency(DeleteEducationOrganizationModel model)
        {
            var deletionResult = (await _odsApiFacadeFactory.Create()).DeleteLocalEducationAgency(model.Id);
            return deletionResult.Success ? JsonSuccess("Organization Removed") : JsonError(deletionResult.ErrorMessage);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteSchool(DeleteEducationOrganizationModel model)
        {
            var deletionResult = (await _odsApiFacadeFactory.Create()).DeleteSchool(model.Id);
            return deletionResult.Success ? JsonSuccess("School Removed") : JsonError(deletionResult.ErrorMessage);
        }
    }
}
