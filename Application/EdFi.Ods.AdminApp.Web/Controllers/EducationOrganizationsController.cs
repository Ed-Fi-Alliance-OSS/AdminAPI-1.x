// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
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
using EdFi.Ods.AdminApp.Web.ActionFilters;
using EdFi.Ods.AdminApp.Web.Display.Pagination;
using EdFi.Ods.AdminApp.Web.Display.TabEnumeration;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using Newtonsoft.Json;
using static EdFi.Ods.AdminApp.Web.Models.ViewModels.EducationOrganizations.EducationOrganizationValidationHelper;

namespace EdFi.Ods.AdminApp.Web.Controllers
{
    public class EducationOrganizationsController : ControllerBase
    {
        private readonly IOdsApiFacadeFactory _odsApiFacadeFactory;
        private readonly IMapper _mapper;
        private readonly InstanceContext _instanceContext;
        private readonly ITabDisplayService _tabDisplayService;
        private readonly IInferExtensionDetails _inferExtensionDetails;

        public EducationOrganizationsController(IOdsApiFacadeFactory odsApiFacadeFactory
            , IMapper mapper, InstanceContext instanceContext, ITabDisplayService tabDisplayService
            , IInferExtensionDetails inferExtensionDetails)
        {
            _odsApiFacadeFactory = odsApiFacadeFactory;
            _mapper = mapper;
            _instanceContext = instanceContext;
            _tabDisplayService = tabDisplayService;
            _inferExtensionDetails = inferExtensionDetails;
        }

        [AddTelemetry("Local Education Agencies Index", TelemetryType.View)]
        public async Task<ActionResult> LocalEducationAgencies()
        {
            var model = new EducationOrganizationsIndexModel
            {
                OdsInstanceSettingsTabEnumerations =
                    _tabDisplayService.GetOdsInstanceSettingsTabDisplay(OdsInstanceSettingsTabEnumeration
                        .EducationOrganizations),
                OdsInstance = _instanceContext,
                TpdmEnabled = await TpdmEnabled(),
                Mode = EducationOrganizationsMode.LocalEducationAgencies
            };

            return View("Index", model);
        }

        [AddTelemetry("Post-Secondary Institutions Index", TelemetryType.View)]
        public async Task<ActionResult> PostSecondaryInstitutions()
        {
            var model = new EducationOrganizationsIndexModel
            {
                OdsInstanceSettingsTabEnumerations =
                    _tabDisplayService.GetOdsInstanceSettingsTabDisplay(OdsInstanceSettingsTabEnumeration
                        .EducationOrganizations),
                OdsInstance = _instanceContext,
                TpdmEnabled = await TpdmEnabled(),
                Mode = EducationOrganizationsMode.PostSecondaryInstitutions
            };

            return View("Index", model);
        }

        [HttpPost]
        [AddTelemetry("Add Local Education Agency")]
        public async Task<ActionResult> AddLocalEducationAgency(AddLocalEducationAgencyModel viewModel)
        {
            var apiFacade = await _odsApiFacadeFactory.Create();

            var leaId = viewModel.LocalEducationAgencyId;

            if (leaId != null)
            {
                if (ProposedEducationOrganizationIdIsInUse(leaId.Value, apiFacade))
                    return ValidationFailureResult(
                        "LocalEducationAgencyId",
                        "This 'Local Education Organization ID' is already associated with " +
                        "another Education Organization. Please provide a unique value.");
            }

            var model = _mapper.Map<LocalEducationAgency>(viewModel);
            model.Id = Guid.Empty.ToString();
            var addResult = apiFacade.AddLocalEducationAgency(model);
            return addResult.Success ? JsonSuccess("Organization Added") : JsonError(addResult.ErrorMessage);
        }

        [HttpPost]
        [AddTelemetry("Add Post-Secondary Institution")]
        public async Task<ActionResult> AddPostSecondaryInstitution(AddPostSecondaryInstitutionModel viewModel)
        {
            var apiFacade = await _odsApiFacadeFactory.Create();

            var psiId = viewModel.PostSecondaryInstitutionId;

            if (psiId != null)
            {
                if (ProposedEducationOrganizationIdIsInUse(psiId.Value, apiFacade))
                    return ValidationFailureResult(
                        "PostSecondaryInstitutionId",
                        "This 'Post-Secondary Institution ID' is already associated with " +
                        "another Education Organization. Please provide a unique value.");
            }

            var model = _mapper.Map<PostSecondaryInstitution>(viewModel);
            model.Id = Guid.Empty.ToString();
            var addResult = apiFacade.AddPostSecondaryInstitution(model);
            return addResult.Success ? JsonSuccess("Post-Secondary Institution Added") : JsonError(addResult.ErrorMessage);
        }

        [HttpPost]
        [AddTelemetry("Add School")]
        public async Task<ActionResult> AddSchool(AddSchoolModel viewModel)
        {
            var apiFacade = await _odsApiFacadeFactory.Create();

            var schoolId = viewModel.SchoolId;

            if (schoolId != null)
            {
                if (ProposedEducationOrganizationIdIsInUse(schoolId.Value, apiFacade))
                    return ValidationFailureResult(
                            "SchoolId",
                            "This 'School ID' is already associated with another " +
                            "Education Organization. Please provide a unique value.");
            }

            var model = _mapper.Map<School>(viewModel);
            model.Id = Guid.Empty.ToString();
            var addResult = apiFacade.AddSchool(model);
            return addResult.Success ? JsonSuccess("School Added") : JsonError(addResult.ErrorMessage);
        }

        [HttpPost]
        [AddTelemetry("Add Post-Secondary Institution School")]
        public async Task<ActionResult> AddPsiSchool(AddPsiSchoolModel viewModel)
        {
            var apiFacade = await _odsApiFacadeFactory.Create();

            var schoolId = viewModel.SchoolId;

            if (schoolId != null)
            {
                if (ProposedEducationOrganizationIdIsInUse(schoolId.Value, apiFacade))
                    return ValidationFailureResult(
                        "SchoolId",
                        "This 'School ID' is already associated with another " +
                        "Education Organization. Please provide a unique value.");
            }

            var model = _mapper.Map<PsiSchool>(viewModel);
            model.Id = Guid.Empty.ToString();
            var addResult = apiFacade.AddPsiSchool(model);
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
        [AddTelemetry("Edit Local Education Agency")]
        public async Task<ActionResult> EditLocalEducationAgency(EditLocalEducationAgencyModel model)
        {
            var editResult = (await _odsApiFacadeFactory.Create()).EditLocalEducationAgency(_mapper.Map<LocalEducationAgency>(model));
            return editResult.Success ? JsonSuccess("Organization Updated") : JsonError(editResult.ErrorMessage);
        }

        public async Task<ActionResult> EditPostSecondaryInstitutionModal(string id)
        {
            var api = await _odsApiFacadeFactory.Create();
            var postSecondaryInstitution = api.GetPostSecondaryInstitutionById(id);
            var postSecondaryInstitutionLevelOptions = BuildListWithEmptyOption(api.GetPostSecondaryInstitutionLevels);
            var administrativeFundingControlOptions = BuildListWithEmptyOption(api.GetAdministrativeFundingControls);
            var stateOptions = api.GetAllStateAbbreviations();

            var model = _mapper.Map<EditPostSecondaryInstitutionModel>(postSecondaryInstitution);
            model.PostSecondaryInstitutionLevelOptions = postSecondaryInstitutionLevelOptions;
            model.AdministrativeFundingControlOptions = administrativeFundingControlOptions;
            model.StateOptions = stateOptions;

            return PartialView("_EditPostSecondaryInstitutionModal", model);
        }

        [HttpPost]
        [AddTelemetry("Edit Post-Secondary Institution")]
        public async Task<ActionResult> EditPostSecondaryInstitution(EditPostSecondaryInstitutionModel model)
        {
            var editResult = (await _odsApiFacadeFactory.Create()).EditPostSecondaryInstitution(_mapper.Map<PostSecondaryInstitution>(model));
            return editResult.Success ? JsonSuccess("Post-Secondary Institution Updated") : JsonError(editResult.ErrorMessage);
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
        [AddTelemetry("Edit School")]
        public async Task<ActionResult> EditSchool(EditSchoolModel model)
        {
            var editResult = (await _odsApiFacadeFactory.Create()).EditSchool(_mapper.Map<School>(model));
            return editResult.Success ? JsonSuccess("School Updated") : JsonError(editResult.ErrorMessage);
        }

        public async Task<ActionResult> EditPsiSchoolModal(string id)
        {
            var api = await _odsApiFacadeFactory.Create();
            var psiSchool = api.GetPsiSchoolById(id);
            var gradeLevelOptions = api.GetAllGradeLevels();
            var stateOptions = api.GetAllStateAbbreviations();
            var federalLocaleCodeOptions = await CheckAndFillIfTpdmCommunityVersion(api.GetFederalLocaleCodes);
            var accreditationStatusOptions = await CheckAndFillIfTpdmCommunityVersion(api.GetAccreditationStatusOptions);

            var model = _mapper.Map<EditPsiSchoolModel>(psiSchool);
            model.GradeLevelOptions = gradeLevelOptions;
            model.StateOptions = stateOptions;
            model.FederalLocaleCodeOptions = federalLocaleCodeOptions;
            model.AccreditationStatusOptions = accreditationStatusOptions;

            return PartialView("_EditPsiSchoolModal", model);
        }

        [HttpPost]
        [AddTelemetry("Edit Post-Secondary Institution School")]
        public async Task<ActionResult> EditPsiSchool(EditPsiSchoolModel model)
        {
            var editResult = (await _odsApiFacadeFactory.Create()).EditPsiSchool(_mapper.Map<PsiSchool>(model));
            return editResult.Success ? JsonSuccess("School Updated") : JsonError(editResult.ErrorMessage);
        }

        public async Task<ActionResult> LocalEducationAgencyList(int pageNumber)
        {
            var api = await _odsApiFacadeFactory.Create();
            
            var localEducationAgencies =
                await Page<LocalEducationAgency>.FetchAsync(GetLocalEducationAgencies, pageNumber);

            var schools = api.GetSchoolsByLeaIds(
                localEducationAgencies.Items.Select(x => x.EducationOrganizationId));

            var requiredApiDataExist = (await _odsApiFacadeFactory.Create()).DoesApiDataExist();

            var model = new LocalEducationAgencyViewModel
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

            return PartialView("_LocalEducationAgencies", model);
        }

        public async Task<ActionResult> PostSecondaryInstitutionsList(int pageNumber)
        {
            var api = await _odsApiFacadeFactory.Create();
            var schools = api.GetAllPsiSchools();

            var postSecondaryInstitutions =
                await Page<PostSecondaryInstitution>.FetchAsync(GetPostSecondaryInstitutions, pageNumber);

            var requiredApiDataExist = (await _odsApiFacadeFactory.Create()).DoesApiDataExist();

            var model = new PostSecondaryInstitutionViewModel
            {
                Schools = schools,
                PostSecondaryInstitutions = postSecondaryInstitutions,
                AddPsiSchoolModel = new AddPsiSchoolModel
                {
                    GradeLevelOptions = api.GetAllGradeLevels(),
                    StateOptions = api.GetAllStateAbbreviations(),
                    FederalLocaleCodeOptions = await CheckAndFillIfTpdmCommunityVersion(api.GetFederalLocaleCodes),
                    AccreditationStatusOptions = await CheckAndFillIfTpdmCommunityVersion(api.GetAccreditationStatusOptions),
                    RequiredApiDataExist = requiredApiDataExist
                },
                AddPostSecondaryInstitutionModel = new AddPostSecondaryInstitutionModel
                {
                    PostSecondaryInstitutionLevelOptions = BuildListWithEmptyOption(api.GetPostSecondaryInstitutionLevels),
                    AdministrativeFundingControlOptions = BuildListWithEmptyOption(api.GetAdministrativeFundingControls),
                    StateOptions = api.GetAllStateAbbreviations(),
                    RequiredApiDataExist = requiredApiDataExist
                }
            };

            return PartialView("_PostSecondaryInstitutions", model);
        }

        private async Task<IReadOnlyList<LocalEducationAgency>> GetLocalEducationAgencies(int offset, int limit)
        {
            var api = await _odsApiFacadeFactory.Create();
            var localEducationAgencies = api.GetLocalEducationAgenciesByPage(offset, limit);
            return localEducationAgencies;
        }

        private async Task<IReadOnlyList<PostSecondaryInstitution>> GetPostSecondaryInstitutions(int offset, int limit)
        {
            var api = await _odsApiFacadeFactory.Create();
            var postSecondaryInstitutions = api.GetPostSecondaryInstitutionsByPage(offset, limit);
            return postSecondaryInstitutions;
        }

        [HttpPost]
        [AddTelemetry("Delete Local Education Agency")]
        public async Task<ActionResult> DeleteLocalEducationAgency(DeleteEducationOrganizationModel model)
        {
            var deletionResult = (await _odsApiFacadeFactory.Create()).DeleteLocalEducationAgency(model.Id);
            return deletionResult.Success ? JsonSuccess("Organization Removed") : JsonError(deletionResult.ErrorMessage);
        }

        [HttpPost]
        [AddTelemetry("Delete Post-Secondary Institution")]
        public async Task<ActionResult> DeletePostSecondaryInstitution(DeleteEducationOrganizationModel model)
        {
            var deletionResult = (await _odsApiFacadeFactory.Create()).DeletePostSecondaryInstitution(model.Id);
            return deletionResult.Success ? JsonSuccess("Post-Secondary Institution Removed") : JsonError(deletionResult.ErrorMessage);
        }

        [HttpPost]
        [AddTelemetry("Delete School")]
        public async Task<ActionResult> DeleteSchool(DeleteEducationOrganizationModel model)
        {
            var deletionResult = (await _odsApiFacadeFactory.Create()).DeleteSchool(model.Id);
            return deletionResult.Success ? JsonSuccess("School Removed") : JsonError(deletionResult.ErrorMessage);
        }

        private async Task<bool> TpdmEnabled()
        {
            var versionDetails = await GetTpdmExtensionDetails();
            return !string.IsNullOrEmpty(versionDetails?.TpdmVersion);
        }

        private async Task<TpdmExtensionDetails> GetTpdmExtensionDetails()
        {
            return await InMemoryCache.Instance.GetOrSet(
                "TpdmExtensionVersion", async () =>
                   await _inferExtensionDetails.TpdmExtensionVersion(
                        CloudOdsAdminAppSettings.Instance.ProductionApiUrl));

        }

        private async Task<List<SelectOptionModel>> CheckAndFillIfTpdmCommunityVersion(Func<List<SelectOptionModel>> optionList)
        {
            var details = await GetTpdmExtensionDetails();
            return details.IsTpdmCommunityVersion ? BuildListWithEmptyOption(optionList) : null;
        }      

        private List<SelectOptionModel> BuildListWithEmptyOption(Func<List<SelectOptionModel>> getSelectOptionList)
        {
            var selectOptionList = new List<SelectOptionModel>
            {
                new SelectOptionModel
                {
                    DisplayText = "",
                    Value = null
                }
            };
            selectOptionList.AddRange(getSelectOptionList.Invoke());
            return selectOptionList;
        }

        private ActionResult ValidationFailureResult(string modelStateKey, string errorMessage)
        {
            ModelState.AddModelError(modelStateKey, errorMessage);

            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(
                    ModelState,
                    new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore}),
                ContentType = "application/json",
                StatusCode = 400
            };
        }
    }
}
