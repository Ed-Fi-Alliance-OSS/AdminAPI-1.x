// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using EdFi.Ods.AdminApp.Management.Database.Commands;
using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Management.Database.Queries;
using EdFi.Ods.AdminApp.Management.Helpers;
using EdFi.Ods.AdminApp.Management.Settings;
using EdFi.Ods.AdminApp.Management.User;
using EdFi.Ods.AdminApp.Web.ActionFilters;
using EdFi.Ods.AdminApp.Web.Display.TabEnumeration;
using EdFi.Ods.AdminApp.Web.Helpers;
using EdFi.Ods.AdminApp.Web.Models.ViewModels;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.Global;
using Microsoft.Extensions.Options;
using System;

namespace EdFi.Ods.AdminApp.Web.Controllers
{
    [BypassInstanceContextFilter]
    [PermissionRequired(Permission.AccessGlobalSettings)]
    public class GlobalSettingsController : ControllerBase
    {
        private readonly AddVendorCommand _addVendorCommand;
        private readonly ICloudOdsSettingsService _cloudOdsSettingsService;
        private readonly DeleteVendorCommand _deleteVendorCommand;
        private readonly EditVendorCommand _editVendorCommand;
        private readonly IGetVendorByIdQuery _getVendorByIdQuery;
        private readonly IGetVendorsQuery _getVendorsQuery;
        private readonly IMapper _mapper;
        private readonly ITabDisplayService _tabDisplayService;
        private readonly IGetClaimSetsByApplicationNameQuery _getClaimSetsByApplicationNameQuery;
        private readonly GetAdminAppUsersQuery _getAdminAppUsersQuery;
        private readonly AppSettings _appSettings;

        public GlobalSettingsController(IMapper mapper
            , IGetVendorsQuery getVendorsQuery
            , AddVendorCommand addVendorCommand
            , DeleteVendorCommand deleteVendorCommand
            , IGetVendorByIdQuery getVendorByIdQuery
            , EditVendorCommand editVendorCommand
            , ICloudOdsSettingsService cloudOdsSettingsService
            , ITabDisplayService tabDisplayService
            , IGetClaimSetsByApplicationNameQuery getClaimSetsByApplicationNameQuery
            , GetAdminAppUsersQuery getAdminAppUsersQuery
            , IOptions<AppSettings> appSettingsAccessor)
        {
            _mapper = mapper;
            _getVendorsQuery = getVendorsQuery;
            _addVendorCommand = addVendorCommand;
            _deleteVendorCommand = deleteVendorCommand;
            _getVendorByIdQuery = getVendorByIdQuery;
            _editVendorCommand = editVendorCommand;
            _cloudOdsSettingsService = cloudOdsSettingsService;
            _tabDisplayService = tabDisplayService;
            _getClaimSetsByApplicationNameQuery = getClaimSetsByApplicationNameQuery;
            _getAdminAppUsersQuery = getAdminAppUsersQuery;
            _appSettings = appSettingsAccessor.Value;
        }

        [AddTelemetry("Global Settings > Vendors", TelemetryType.View)]
        public ActionResult Vendors()
        {
            var vendors = _getVendorsQuery
                .Execute();

            var model = new GlobalSettingsModel
            {
                VendorListModel = new VendorsListModel
                {
                    Vendors = _mapper.Map<List<VendorOverviewModel>>(vendors)
                },
                GlobalSettingsTabEnumerations =
                    _tabDisplayService.GetGlobalSettingsTabDisplay(
                        GlobalSettingsTabEnumeration.Vendors)
            };

            return View(model);
        }

        [AddTelemetry("Global Settings > Claim Sets", TelemetryType.View)]
        public ActionResult ClaimSets()
        {
            var model = new GlobalSettingsModel
            {
                ClaimSetEditorModel = new ClaimSetEditorModel
                {
                    ClaimSets = _getClaimSetsByApplicationNameQuery.Execute(CloudOdsAdminApp.SecurityContextApplicationName)
                },
                GlobalSettingsTabEnumerations =
                    _tabDisplayService.GetGlobalSettingsTabDisplay(
                        GlobalSettingsTabEnumeration.ClaimSets)
            };

            return View(model);
        }

        [AddTelemetry("Global Settings > Users", TelemetryType.View)]
        public ActionResult Users()
        {
            var users = _getAdminAppUsersQuery.Execute();

            var model = new GlobalSettingsModel
            {
                UserIndexModel = new UserIndexModel
                {
                    Users = _mapper.Map<List<UserModel>>(users)
                },
                GlobalSettingsTabEnumerations =
                    _tabDisplayService.GetGlobalSettingsTabDisplay(
                        GlobalSettingsTabEnumeration.Users)
            };

            return View(model);
        }

        [HttpPost]
        [AddTelemetry("Add Vendor")]
        public ActionResult AddVendor(AddVendorModel model)
        {
            _addVendorCommand.Execute(model);
            return RedirectToActionJson<GlobalSettingsController>(x => x.Vendors(), "Vendor added successfully");
        }

        [HttpPost]
        [AddTelemetry("Delete Vendor")]
        public ActionResult DeleteVendor(DeleteVendorModel model)
        {
            try
            {
                _deleteVendorCommand.Execute(model.VendorId);
            }
            catch(Exception ex)
            {
                return JsonError(ex.Message);
            }
            return RedirectToActionJson<GlobalSettingsController>(x => x.Vendors(), "Vendor removed successfully");
        }

        public ActionResult EditVendor(int id)
        {
            var vendor = _getVendorByIdQuery.Execute(id);
            var model = _mapper.Map<EditVendorModel>(vendor);
            return PartialView("_EditVendorForm", model);
        }

        [HttpPost]
        [AddTelemetry("Edit Vendor")]
        public ActionResult EditVendor(EditVendorModel model)
        {
            try
            {
                _editVendorCommand.Execute(model);
            }
            catch (Exception ex)
            {
                return JsonError(ex.Message);
            }
            return RedirectToActionJson<GlobalSettingsController>(x => x.Vendors(), "Vendor updated successfully");
        }

        public ActionResult DefineApplications()
        {
            TempData["IsVendorApplications"] = true;
            return RedirectToAction("Index", "OdsInstances");
        }

        [AddTelemetry("Global Settings > Advanced Settings", TelemetryType.View)]
        public async Task<ActionResult> AdvancedSettings()
        {
            var currentSettings = await _cloudOdsSettingsService.GetSettings(_appSettings.DefaultOdsInstance);

            var model = new GlobalSettingsModel
            {
                AdvancedSettingsModel = new AdvancedSettingsModel
                {
                    BearerTokenTimeoutInMinutes = currentSettings.BearerTokenTimeoutInMinutes
                },
                GlobalSettingsTabEnumerations =
                    _tabDisplayService.GetGlobalSettingsTabDisplay(
                        GlobalSettingsTabEnumeration.AdvancedSettings)
            };

            return View(model);
        }

        [HttpPost]
        [AddTelemetry("Update Advanced Settings")]
        public async Task<ActionResult> UpdateAdvancedSettings(GlobalSettingsModel model)
        {
            var instanceSettings = await _cloudOdsSettingsService.GetSettings(_appSettings.DefaultOdsInstance);

            instanceSettings.BearerTokenTimeoutInMinutes = model.AdvancedSettingsModel.BearerTokenTimeoutInMinutes;

            await _cloudOdsSettingsService.UpdateSettings(_appSettings.DefaultOdsInstance, instanceSettings);

            return RedirectToActionJson<GlobalSettingsController>(
                x => x.AdvancedSettings(), "Settings updated successfully");
        }

        [HttpPost]
        [AddTelemetry("Update Log Settings")]
        public async Task<ActionResult> UpdateLogSettings(int logLevel)
        {
            var parsedLogLevel = LogLevel.FromInt32(logLevel);

            var settings = await _cloudOdsSettingsService.GetSettings(_appSettings.DefaultOdsInstance);

            settings.LogLevel = parsedLogLevel;

            await _cloudOdsSettingsService.UpdateSettings(_appSettings.DefaultOdsInstance, settings);

            return JsonSuccess("Log settings updated successfully");
        }
    }
}
