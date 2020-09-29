// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#if NET48
using System.Web.Mvc;
#else
using Microsoft.AspNetCore.Mvc;
#endif
using AutoMapper;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Management.Database.Commands;
using EdFi.Ods.AdminApp.Management.Database.Queries;
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
        private readonly GetClaimSetNamesQuery _getClaimSetNamesQuery;
        private readonly GetProfilesQuery _getProfilesQuery;
        private readonly IGetVendorByIdQuery _getVendorByIdQuery;
        private readonly IMapper _mapper;
        private readonly IOdsApiFacadeFactory _odsApiFacadeFactory;
        private readonly InstanceContext _instanceContext;
        private readonly RegenerateApiClientSecretCommand _regenerateApiClientSecretCommand;

        public ApplicationController(IMapper mapper
            , IDeleteApplicationCommand deleteApplicationCommand
            , IAddApplicationCommand addApplicationCommand
            , IEditApplicationCommand editApplicationCommand
            , GetApplicationByIdQuery getApplicationByIdQuery
            , IGetVendorByIdQuery getVendorByIdQuery
            , GetClaimSetNamesQuery getClaimSetNamesQuery
            , GetProfilesQuery getProfilesQuery
            , RegenerateApiClientSecretCommand regenerateApiClientSecretCommand
            , IOdsApiFacadeFactory odsApiFacadeFactory
            , InstanceContext instanceContext)
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
        }

        public async Task<ActionResult> Add(int vendorId)
        {
            var vendor = _getVendorByIdQuery.Execute(vendorId);
            var apiFacade = await _odsApiFacadeFactory.Create();
            var leas = apiFacade.GetAllLocalEducationAgencies().ToList();
            var schools = apiFacade.GetAllSchools().ToList();
            var profiles = _mapper.Map<List<ProfileModel>>(_getProfilesQuery.Execute());

            var model = new AddApplicationViewModel
            {
                VendorId = vendorId,
                VendorName = vendor.VendorName,
                Environment = CloudOdsEnvironment.Production,
                LocalEducationAgencies = leas,
                Schools = schools,
                ClaimSetNames = GetClaimSetNames(),
                Profiles = profiles
            };

            return PartialView("_AddApplicationModal", model);
        }

        [HttpPost]
        public ActionResult Add(AddApplicationModel model)
        {
            var result = _addApplicationCommand.Execute(model);

            return PartialView(
                "_ApplicationKeyAndSecretContent", new ApplicationKeyModel
                {
                    ApplicationName = model.ApplicationName,
                    ApplicationEnvironment = model.Environment,
                    Key = result.Key,
                    Secret = result.Secret,
                    ApiUrl = CloudOdsApiConnectionInformationProvider.GetConnectionInformationForEnvironment(
                        model.Environment, new OdsApiCredential(result.Key, result.Secret), _instanceContext.Name, CloudOdsAdminAppSettings.Instance.Mode).ApiBaseUrl
                });
        }

        [HttpPost]
        public ActionResult Delete(DeleteApplicationModel model)
        {
            var application = _getApplicationByIdQuery.Execute(model.ApplicationId);

            if (application.IsSystemReservedApplication())
            {
                return JsonError("This Application is required for proper system function and may not be deleted");
            }

            _deleteApplicationCommand.Execute(model.ApplicationId);
            return JsonSuccess("Application deleted successfully");
        }

        public async Task<ActionResult> Edit(int applicationId, CloudOdsEnvironment environment)
        {
            var application = _getApplicationByIdQuery.Execute(applicationId);
            var apiFacade = await _odsApiFacadeFactory.Create(environment);
            var leas = apiFacade.GetAllLocalEducationAgencies().ToList();
            var schools = apiFacade.GetAllSchools().ToList();
            var profiles = _mapper.Map<List<ProfileModel>>(_getProfilesQuery.Execute());

            var educationOrganizationIds =
                application.ApplicationEducationOrganizations.Select(x => x.EducationOrganizationId).ToList();

            var edOrgType = schools.Any(x => educationOrganizationIds.Contains(x.EducationOrganizationId))
                ? ApplicationEducationOrganizationType.School
                : ApplicationEducationOrganizationType.LocalEducationAgency;

            var model = new EditApplicationViewModel
            {
                ApplicationId = applicationId,
                ApplicationName = application.DisplayName(),
                ClaimSetName = application.ClaimSetName,
                ClaimSetNames = GetClaimSetNames(),
                EducationOrganizationIds = application.ApplicationEducationOrganizations.Select(x => x.EducationOrganizationId),
                LocalEducationAgencies = leas,
                Schools = schools,
                Environment = environment,
                ProfileId = application.Profiles.FirstOrDefault()?.ProfileId ?? 0,
                Profiles = profiles,
                VendorId = application.Vendor.VendorId,
                EducationOrganizationType = edOrgType
            };

            return PartialView("_EditApplicationModal", model);
        }

        [HttpPost]
        public ActionResult Edit(EditApplicationModel model)
        {
            var application = _getApplicationByIdQuery.Execute(model.ApplicationId);

            if (application.IsSystemReservedApplication())
            {
                return JsonError("This Application is required for proper system function and may not be modified");
            }

            _editApplicationCommand.Execute(model);
            return JsonSuccess("Application updated successfully");
        }

        [HttpPost]
        public ActionResult RegenerateSecret(RegenerateSecretModel model)
        {
            var regenerationResult = _regenerateApiClientSecretCommand.Execute(model.ApplicationId);
            var application = regenerationResult.Application;

            return PartialView("_ApplicationKeyAndSecretContent", new ApplicationKeyModel
            {
                ApplicationName = application.ApplicationName,
                ApplicationEnvironment = application.GetEnvironment(),
                Key = regenerationResult.Key,
                Secret = regenerationResult.Secret,
                ApiUrl = CloudOdsApiConnectionInformationProvider.GetConnectionInformationForEnvironment(
                    application.GetEnvironment(), new OdsApiCredential(regenerationResult.Key, regenerationResult.Secret), _instanceContext.Name, CloudOdsAdminAppSettings.Instance.Mode).ApiBaseUrl
            });
        }

        private List<string> GetClaimSetNames()
        {
            return _getClaimSetNamesQuery.Execute().Except(CloudOdsAdminApp.SystemReservedClaimSets).ToList();
        }
    }
}
