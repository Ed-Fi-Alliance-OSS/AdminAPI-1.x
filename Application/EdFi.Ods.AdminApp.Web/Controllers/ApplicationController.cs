// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Management.Database.Commands;
using EdFi.Ods.AdminApp.Management.Database.Queries;
using EdFi.Ods.AdminApp.Web.ActionFilters;
using EdFi.Ods.AdminApp.Web.Display.Pagination;
using EdFi.Ods.AdminApp.Web.Display.TabEnumeration;
using EdFi.Ods.AdminApp.Web.Helpers;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using EdFi.Ods.AdminApp.Web.Models.ViewModels;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.Application;

namespace EdFi.Ods.AdminApp.Web.Controllers
{
    public class ApplicationController : ControllerBase
    {
        private readonly IAddApplicationCommand _addApplicationCommand;
        private readonly IDeleteApplicationCommand _deleteApplicationCommand;
        private readonly IEditApplicationCommand _editApplicationCommand;
        private readonly GetApplicationByIdQuery _getApplicationByIdQuery;
        private readonly GetAllClaimSetsQuery _getClaimSetNamesQuery;
        private readonly GetProfilesQuery _getProfilesQuery;
        private readonly IGetVendorByIdQuery _getVendorByIdQuery;
        private readonly IMapper _mapper;
        private readonly IOdsApiFacadeFactory _odsApiFacadeFactory;
        private readonly InstanceContext _instanceContext;
        private readonly RegenerateApiClientSecretCommand _regenerateApiClientSecretCommand;
        private readonly ITabDisplayService _tabDisplayService;
        private readonly IOdsApiConnectionInformationProvider _apiConnectionInformationProvider;
        private readonly IGetVendorsQuery _getVendorsQuery;
        private readonly IInferExtensionDetails _inferExtensionDetails;

        public ApplicationController(IMapper mapper
            , IDeleteApplicationCommand deleteApplicationCommand
            , IAddApplicationCommand addApplicationCommand
            , IEditApplicationCommand editApplicationCommand
            , GetApplicationByIdQuery getApplicationByIdQuery
            , IGetVendorByIdQuery getVendorByIdQuery
            , GetAllClaimSetsQuery getClaimSetNamesQuery
            , GetProfilesQuery getProfilesQuery
            , RegenerateApiClientSecretCommand regenerateApiClientSecretCommand
            , IOdsApiFacadeFactory odsApiFacadeFactory
            , InstanceContext instanceContext
            , ITabDisplayService tabDisplayService
            , IOdsApiConnectionInformationProvider apiConnectionInformationProvider
            , IGetVendorsQuery getVendorsQuery
            , IInferExtensionDetails inferExtensionDetails)
        {
            _mapper = mapper;
            _deleteApplicationCommand = deleteApplicationCommand;
            _addApplicationCommand = addApplicationCommand;
            _editApplicationCommand = editApplicationCommand;
            _getApplicationByIdQuery = getApplicationByIdQuery;
            _getVendorByIdQuery = getVendorByIdQuery;
            _getClaimSetNamesQuery = getClaimSetNamesQuery;
            _getProfilesQuery = getProfilesQuery;
            _regenerateApiClientSecretCommand = regenerateApiClientSecretCommand;
            _odsApiFacadeFactory = odsApiFacadeFactory;
            _instanceContext = instanceContext;
            _tabDisplayService = tabDisplayService;
            _apiConnectionInformationProvider = apiConnectionInformationProvider;
            _getVendorsQuery = getVendorsQuery;
            _inferExtensionDetails = inferExtensionDetails;
        }

        [AddTelemetry("Application Index", TelemetryType.View)]
        public async Task<ActionResult> Index()
        {
            var apiUrl = (await _apiConnectionInformationProvider.GetConnectionInformationForEnvironment())
                .ApiServerUrl;

            var model = new ApplicationsIndexModel
            {
                OdsInstanceSettingsTabEnumerations =
                    _tabDisplayService.GetOdsInstanceSettingsTabDisplay(OdsInstanceSettingsTabEnumeration.Applications),
                OdsInstance = _instanceContext,
                DisplayApiUrl = GetApiUrlForDisplay(apiUrl)
            };

            return View("Index", model);
        }

        public async Task<ActionResult> ApplicationList(int pageNumber)
        {
            var edOrgs = (await _odsApiFacadeFactory.Create())
                .GetAllEducationOrganizations(_mapper);

            var vendorsIndexModel = new VendorApplicationsIndexModel
            {
                Vendors = Page<VendorApplicationsModel>.Fetch(VendorsApplicationsModel, pageNumber)
            };

            return PartialView("_Applications", vendorsIndexModel);

            List<VendorApplicationsModel> VendorsApplicationsModel(int offset, int limit)
            {
                var vendors = _getVendorsQuery.Execute(offset, limit).ToList();

                var vendorsApplicationsModel = _mapper.Map<List<VendorApplicationsModel>>(
                    vendors, opts => opts.WithEducationOrganizations(edOrgs));

                if (CloudOdsAdminAppSettings.Instance.Mode.SupportsMultipleInstances)
                {
                    foreach (var model in vendorsApplicationsModel)
                    {
                        FilterInstanceSpecificApplications(model);
                    }
                }

                return vendorsApplicationsModel;
            }
        }

        private void FilterInstanceSpecificApplications(VendorApplicationsModel vendor)
        {
            var applications = vendor.Applications.Where(x =>
                    x.OdsInstanceName.Equals(_instanceContext.Name, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
            vendor.Applications = applications;
        }

        public async Task<ActionResult> Add(int vendorId)
        {
            var vendor = _getVendorByIdQuery.Execute(vendorId);
            var apiFacade = await _odsApiFacadeFactory.Create();
            var leas = apiFacade.GetAllLocalEducationAgencies().ToList();
            var psis = apiFacade.GetAllPostSecondaryInstitutions().ToList();
            var schools = apiFacade.GetAllSchools().ToList();
            var profiles = _mapper.Map<List<ProfileModel>>(_getProfilesQuery.Execute());

            var model = new AddApplicationViewModel
            {
                VendorId = vendorId,
                VendorName = vendor.VendorName,
                LocalEducationAgencies = leas,
                PostSecondaryInstitutions = psis,
                Schools = schools,
                ClaimSetNames = GetClaimSetNames(),
                Profiles = profiles,
                TpdmEnabled = await TpdmEnabled()
            };

            return PartialView("_AddApplicationModal", model);
        }

        [HttpPost]
        [AddTelemetry("Add Application")]
        public ActionResult Add(AddApplicationModel model)
        {
            var result = _addApplicationCommand.Execute(model);

            var apiUrl = CloudOdsApiConnectionInformationProvider.GetConnectionInformationForEnvironment(
                new OdsApiCredential(result.Key, result.Secret), _instanceContext.Name,
                CloudOdsAdminAppSettings.Instance.Mode).ApiBaseUrl;

            return PartialView(
                "_ApplicationKeyAndSecretContent", new ApplicationKeyModel
                {
                    ApplicationName = model.ApplicationName,
                    Key = result.Key,
                    Secret = result.Secret,
                    DisplayApiUrl = GetApiUrlForDisplay(apiUrl)
                });
        }

        [HttpPost]
        [AddTelemetry("Delete Application")]
        public ActionResult Delete(DeleteApplicationModel model)
        {
            try
            {
                _deleteApplicationCommand.Execute(model.ApplicationId);
                return JsonSuccess("Application deleted successfully");
            }
            catch (Exception ex)
            {
                return JsonError(ex.Message);
            }
        }

        public async Task<ActionResult> Edit(int applicationId)
        {
            var application = _getApplicationByIdQuery.Execute(applicationId);
            var apiFacade = await _odsApiFacadeFactory.Create();
            var leas = apiFacade.GetAllLocalEducationAgencies().ToList();
            var psis = apiFacade.GetAllPostSecondaryInstitutions().ToList();
            var schools = apiFacade.GetAllSchools().ToList();
            var profiles = _mapper.Map<List<ProfileModel>>(_getProfilesQuery.Execute());

            var educationOrganizationIds =
                application.ApplicationEducationOrganizations.Select(x => x.EducationOrganizationId).ToList();

            var edOrgType = schools.Any(x => educationOrganizationIds.Contains(x.EducationOrganizationId))
                ? ApplicationEducationOrganizationType.School
                : psis.Any(x => educationOrganizationIds.Contains(x.EducationOrganizationId))
                    ? ApplicationEducationOrganizationType.PostSecondaryInstitution
                    : ApplicationEducationOrganizationType.LocalEducationAgency ;

            var model = new EditApplicationViewModel
            {
                ApplicationId = applicationId,
                ApplicationName = application.ApplicationName,
                ClaimSetName = application.ClaimSetName,
                ClaimSetNames = GetClaimSetNames(),
                EducationOrganizationIds = application.ApplicationEducationOrganizations.Select(x => x.EducationOrganizationId),
                LocalEducationAgencies = leas,
                PostSecondaryInstitutions = psis,
                Schools = schools,
                ProfileId = application.Profiles.FirstOrDefault()?.ProfileId ?? 0,
                Profiles = profiles,
                VendorId = application.Vendor.VendorId,
                EducationOrganizationType = edOrgType,
                TpdmEnabled = await TpdmEnabled()
            };

            return PartialView("_EditApplicationModal", model);
        }

        [HttpPost]
        [AddTelemetry("Edit Application")]
        public ActionResult Edit(EditApplicationModel model)
        {
            try
            {
                _editApplicationCommand.Execute(model);
                return JsonSuccess("Application updated successfully");
            }
            catch (Exception ex)
            {
                return JsonError(ex.Message);
            }
        }

        [HttpPost]
        [AddTelemetry("Regenerate Secret")]
        public ActionResult RegenerateSecret(RegenerateSecretModel model)
        {
            var regenerationResult = _regenerateApiClientSecretCommand.Execute(model.ApplicationId);
            var application = regenerationResult.Application;

            var apiUrl = CloudOdsApiConnectionInformationProvider.GetConnectionInformationForEnvironment(
                new OdsApiCredential(regenerationResult.Key, regenerationResult.Secret),
                _instanceContext.Name, CloudOdsAdminAppSettings.Instance.Mode).ApiBaseUrl;

            return PartialView("_ApplicationKeyAndSecretContent", new ApplicationKeyModel
            {
                ApplicationName = application.ApplicationName,
                Key = regenerationResult.Key,
                Secret = regenerationResult.Secret,
                DisplayApiUrl = GetApiUrlForDisplay(apiUrl)
            });
        }

        private List<string> GetClaimSetNames()
        {
            return _getClaimSetNamesQuery.Execute().Select(x => x.Name).Except(CloudOdsAdminApp.SystemReservedClaimSets).ToList();
        }

        private static string GetApiUrlForDisplay(string apiUrl)
        {
            return !string.IsNullOrEmpty(CloudOdsAdminAppSettings.Instance.ApiExternalUrl)
                ? apiUrl.Replace(CloudOdsAdminAppSettings.Instance.ProductionApiUrl, CloudOdsAdminAppSettings.Instance.ApiExternalUrl)
                : apiUrl;
        }

        private async Task<bool> TpdmEnabled()
        {
            var versionDetails = await InMemoryCache.Instance.GetOrSet(
                "TpdmExtensionVersion", async () =>
                    await _inferExtensionDetails.TpdmExtensionVersion(
                        CloudOdsAdminAppSettings.Instance.ProductionApiUrl));

            return !string.IsNullOrEmpty(versionDetails.TpdmVersion);
        }
    }
}
