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

        public ActionResult Vendors()
        {
            var vendors = _getVendorsQuery
                .Execute()
                .Where(v => !v.IsSystemReservedVendor());

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
        public ActionResult AddVendor(AddVendorModel model)
        {
            _addVendorCommand.Execute(model);
            return RedirectToActionJson<GlobalSettingsController>(x => x.Vendors(), "Vendor added successfully");
        }

        [HttpPost]
        public ActionResult DeleteVendor(DeleteVendorModel model)
        {
            var vendor = _getVendorByIdQuery.Execute(model.VendorId);

            if (vendor.IsSystemReservedVendor())
            {
                return JsonError("This Vendor is required for proper system function and may not be deleted");
            }

            _deleteVendorCommand.Execute(model.VendorId);
            return RedirectToActionJson<GlobalSettingsController>(x => x.Vendors(), "Vendor removed successfully");
        }

        public ActionResult EditVendor(int id)
        {
            var vendor = _getVendorByIdQuery.Execute(id);
            var model = _mapper.Map<EditVendorModel>(vendor);
            return PartialView("_EditVendorForm", model);
        }

        [HttpPost]
        public ActionResult EditVendor(EditVendorModel model)
        {
            var vendor = _getVendorByIdQuery.Execute(model.VendorId);

            if (vendor == null)
            {
                return JsonError("This vendor no longer exists.  It may have been recently deleted.");
            }

            if (vendor.IsSystemReservedVendor())
            {
                return JsonError("This vendor is required for proper system function and may not be modified.");
            }

            _editVendorCommand.Execute(model);
            return RedirectToActionJson<GlobalSettingsController>(x => x.Vendors(), "Vendor updated successfully");
        }

        public async Task<ActionResult> AdvancedSettings()
        {
            var currentSettings = await _cloudOdsSettingsService.GetSettings(
                _appSettings.DefaultOdsInstance, CloudOdsEnvironment.Production);

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
        public async Task<ActionResult> UpdateAdvancedSettings(GlobalSettingsModel model)
        {
            var instanceSettings = await _cloudOdsSettingsService.GetSettings(
                _appSettings.DefaultOdsInstance, CloudOdsEnvironment.Production);

            instanceSettings.BearerTokenTimeoutInMinutes = model.AdvancedSettingsModel.BearerTokenTimeoutInMinutes;

            await _cloudOdsSettingsService.UpdateSettings(
                _appSettings.DefaultOdsInstance, CloudOdsEnvironment.Production, instanceSettings);

            return RedirectToActionJson<GlobalSettingsController>(
                x => x.AdvancedSettings(), "Settings updated successfully");
        }

        [HttpPost]
        public async Task<ActionResult> UpdateLogSettings(LogSettingsModel model)
        {
            var settings = await _cloudOdsSettingsService.GetSettings(
                _appSettings.DefaultOdsInstance, model.Environment);

            settings.LogLevel = model.LogLevel;

            await _cloudOdsSettingsService.UpdateSettings(
                _appSettings.DefaultOdsInstance, model.Environment, settings);

            return JsonSuccess("Log settings updated successfully");
        }       
    }
}
