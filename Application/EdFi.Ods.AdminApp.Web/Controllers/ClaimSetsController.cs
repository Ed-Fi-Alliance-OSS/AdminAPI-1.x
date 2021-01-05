// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Management.Database.Queries;
using EdFi.Ods.AdminApp.Web.ActionFilters;
using EdFi.Ods.AdminApp.Web.Display.TabEnumeration;
using EdFi.Ods.AdminApp.Web.Helpers;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.ClaimSets;
using Newtonsoft.Json;
using static EdFi.Ods.AdminApp.Web.Infrastructure.ResourceClaimSelectListBuilder;
using AddClaimSetModel = EdFi.Ods.AdminApp.Web.Models.ViewModels.ClaimSets.AddClaimSetModel;

namespace EdFi.Ods.AdminApp.Web.Controllers
{
    [BypassInstanceContextFilter]
    [PermissionRequired(Permission.AccessGlobalSettings)]
    public class ClaimSetsController : ControllerBase
    {
        private readonly IGetClaimSetByIdQuery _getClaimSetByIdQuery;
        private readonly IGetApplicationsByClaimSetIdQuery _getApplicationsByClaimSetIdQuery;
        private readonly IGetResourcesByClaimSetIdQuery _getResourcesByClaimSetIdQuery;
        private readonly IGetClaimSetsByApplicationNameQuery _getClaimSetsByApplicationNameQuery;
        private readonly IGetAuthStrategiesByApplicationNameQuery _getAuthStrategiesByApplicationNameQuery;
        private readonly ITabDisplayService _tabDisplayService;
        private readonly CopyClaimSetCommand _copyClaimSetCommand;
        private readonly AddClaimSetCommand _addClaimSetCommand;
        private readonly EditClaimSetCommand _editClaimSetCommand;
        private readonly GetResourceClaimsQuery _getResourceClaimsQuery;
        private readonly GetChildResourceClaimsForParentQuery _getChildResourceClaimsForParentQuery;
        private readonly DeleteClaimSetCommand _deleteClaimSetCommand;
        private readonly EditResourceOnClaimSetCommand _editResourceOnClaimSetCommand;
        private readonly DeleteResourceOnClaimSetCommand _deleteResourceOnClaimSetCommand;
        private readonly ClaimSetFileImportCommand _claimSetFileImportCommand;
        private readonly ClaimSetFileExportCommand _claimSetFileExportCommand;
        private readonly OverrideDefaultAuthorizationStrategyCommand _overrideDefaultAuthorizationStrategyCommand;
        private readonly ResetToDefaultAuthStrategyCommand _resetToDefaultAuthStrategyCommand;

        public ClaimSetsController(IGetClaimSetByIdQuery getClaimSetByIdQuery
            , IGetApplicationsByClaimSetIdQuery getApplicationsByClaimSetIdQuery
            , IGetResourcesByClaimSetIdQuery getResourcesByClaimSetIdQuery
            , IGetClaimSetsByApplicationNameQuery getClaimSetsByApplicationNameQuery
            , IGetAuthStrategiesByApplicationNameQuery getAuthStrategiesByApplicationNameQuery
            , ITabDisplayService tabDisplayService
            , CopyClaimSetCommand copyClaimSetCommand
            , AddClaimSetCommand addClaimSetCommand
            , EditClaimSetCommand editClaimSetCommand
            , GetResourceClaimsQuery getResourceClaimsQuery
            , GetChildResourceClaimsForParentQuery getChildResourceClaimsForParentQuery
            , DeleteClaimSetCommand deleteClaimSetCommand
            , EditResourceOnClaimSetCommand editResourceOnClaimSetCommand
            , DeleteResourceOnClaimSetCommand deleteResourceOnClaimSetCommand
            , ClaimSetFileExportCommand claimSetFileExportCommand
            , ClaimSetFileImportCommand claimSetFileImportCommand
            , OverrideDefaultAuthorizationStrategyCommand overrideDefaultAuthorizationStrategyCommand
            , ResetToDefaultAuthStrategyCommand resetToDefaultAuthStrategyCommand)
        {
            _getClaimSetByIdQuery = getClaimSetByIdQuery;
            _getApplicationsByClaimSetIdQuery = getApplicationsByClaimSetIdQuery;
            _getResourcesByClaimSetIdQuery = getResourcesByClaimSetIdQuery;
            _getClaimSetsByApplicationNameQuery = getClaimSetsByApplicationNameQuery;
            _getAuthStrategiesByApplicationNameQuery = getAuthStrategiesByApplicationNameQuery;
            _tabDisplayService = tabDisplayService;
            _copyClaimSetCommand = copyClaimSetCommand;
            _addClaimSetCommand = addClaimSetCommand;
            _editClaimSetCommand = editClaimSetCommand;
            _getClaimSetByIdQuery = getClaimSetByIdQuery;
            _getResourceClaimsQuery = getResourceClaimsQuery;
            _getChildResourceClaimsForParentQuery = getChildResourceClaimsForParentQuery;
            _deleteClaimSetCommand = deleteClaimSetCommand;
            _editResourceOnClaimSetCommand = editResourceOnClaimSetCommand;
            _deleteResourceOnClaimSetCommand = deleteResourceOnClaimSetCommand;
            _claimSetFileExportCommand = claimSetFileExportCommand;
            _claimSetFileImportCommand = claimSetFileImportCommand;
            _overrideDefaultAuthorizationStrategyCommand = overrideDefaultAuthorizationStrategyCommand;
            _resetToDefaultAuthStrategyCommand = resetToDefaultAuthStrategyCommand;
        }

        public ActionResult ClaimSetDetails(int claimSetId)
        {
            var model = new ClaimSetModel
            {
                ClaimSetDetailsModel = new ClaimSetDetailsModel
                {
                    ClaimSet = _getClaimSetByIdQuery.Execute(claimSetId),
                    Applications = _getApplicationsByClaimSetIdQuery.Execute(claimSetId),
                    ResourceClaims = _getResourcesByClaimSetIdQuery.AllResources(claimSetId)
                },
                GlobalSettingsTabEnumerations = _tabDisplayService.GetGlobalSettingsTabDisplay(
                    GlobalSettingsTabEnumeration.ClaimSets)
            };

           return View(model);
        }

        public ActionResult AuthStrategyModal(int claimSetId, int resourceClaimId)
        {
            var model = new AuthStrategyEditorModel
            {
                AuthStrategies = GetSelectListForAuthStrategies(),
                ResourceClaim = _getResourcesByClaimSetIdQuery.SingleResource(claimSetId, resourceClaimId)
            };

            return PartialView("_AuthStrategiesModal", model);
        }

        public string GetUpdatedResourceClaim(int claimSetId, int resourceClaimId)
        {
            var resourceClaim = _getResourcesByClaimSetIdQuery.SingleResource(claimSetId, resourceClaimId);
            return JsonConvert.SerializeObject(resourceClaim);
        }

        private string GetSelectListForAuthStrategies()
        {
            var selectList = new List<SelectListItem>{
                new SelectListItem{ Text="Please select a value", Value = "0" , Disabled = true, Selected = true},
            };
            var authorizationStrategies = _getAuthStrategiesByApplicationNameQuery.Execute(CloudOdsAdminApp.SecurityContextApplicationName);
            selectList.AddRange(new SelectList(authorizationStrategies.Select(x => new SelectListItem()
            {
                Text = x.AuthStrategyName,
                Value = x.AuthStrategyId.ToString()
            }), "Value", "Text"));
            return JsonConvert.SerializeObject(selectList);
        }

        public ActionResult CopyClaimSetModal(int claimSetId)
        {
            var existingClaimSet = _getClaimSetByIdQuery.Execute(claimSetId);
            var model = new CopyClaimSetModel
            {
                Name = existingClaimSet.Name+" Copy",
                OriginalId = claimSetId,
                OriginalName = existingClaimSet.Name
            };

            return PartialView("_CopyClaimSetModal", model);
        }

        [HttpPost]
        public ActionResult CopyClaimSet(CopyClaimSetModel model)
        {
            _copyClaimSetCommand.Execute(model);
            return JsonResult(new
            {
                successMessage = $"Claimset copied successfully to {model.Name}"
            });
        }

        public ActionResult AddClaimSet()
        {
            return PartialView("_AddClaimSet", new AddClaimSetModel());
        }

        [HttpPost]
        public ActionResult AddClaimSet(AddClaimSetModel model)
        {
            var claimSetId = _addClaimSetCommand.Execute(model);

            var editClaimSetModel = GetEditClaimSetModel(claimSetId);

            return PartialView("_EditClaimSet",editClaimSetModel);
        }

        public ActionResult EditClaimSet(int claimSetId)
        {
            var model = new ClaimSetModel
            {
                EditClaimSetModel = GetEditClaimSetModel(claimSetId),
                GlobalSettingsTabEnumerations = _tabDisplayService.GetGlobalSettingsTabDisplay(GlobalSettingsTabEnumeration.ClaimSets)
            };
  
            return View(model);
        }

        private EditClaimSetModel GetEditClaimSetModel(int claimSetId)
        {
            var existingClaimSet = _getClaimSetByIdQuery.Execute(claimSetId);
            var allResourceClaims = _getResourceClaimsQuery.Execute().ToList();

            return new EditClaimSetModel
            {
                ClaimSetName = existingClaimSet.Name,
                ClaimSetId = claimSetId,
                Applications = _getApplicationsByClaimSetIdQuery.Execute(claimSetId),
                ResourceClaims = _getResourcesByClaimSetIdQuery.AllResources(claimSetId),
                AllResourceClaims = GetSelectListForResourceClaims(allResourceClaims)
            };
        }

        [HttpGet]
        public JsonResult GetSelectListForChildResourceClaims(int parentResourceClaimId)
        {
            var selectList = new List<SelectListItem>{
                new SelectListItem{ Text="Please select a value", Value = "0" , Disabled = true, Selected = true},
            };
            var childResources = _getChildResourceClaimsForParentQuery.Execute(parentResourceClaimId);
            selectList.AddRange(new SelectList(childResources.Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }), "Value", "Text"));
            return Json(selectList);
        }

        [HttpPost]
        public ActionResult EditClaimSet(EditClaimSetModel model)
        {
            _editClaimSetCommand.Execute(model);
            return RedirectToAction("ClaimSets", "GlobalSettings");
        }

        [HttpPost]
        public ActionResult EditResourceOnClaimSet(EditClaimSetResourceModel model)
        {
            _editResourceOnClaimSetCommand.Execute(model);
            return RedirectToAction("EditClaimSet", "ClaimSets", new { claimSetId = model.ClaimSetId });
        }

        [HttpPost]
        public ActionResult OverrideAuthStrategiesOnResource(OverrideDefaultAuthorizationStrategyModel model)
        {
            _overrideDefaultAuthorizationStrategyCommand.Execute(model);
            return RedirectToAction("EditClaimSet", "ClaimSets", new { claimSetId = model.ClaimSetId });
        }

        [HttpPost]
        public ActionResult ResetAuthStrategiesOnResource(ResetToDefaultAuthStrategyModel model)
        {
            _resetToDefaultAuthStrategyCommand.Execute(model);
            return RedirectToAction("EditClaimSet", "ClaimSets", new { claimSetId = model.ClaimSetId });
        }

        public ActionResult DeleteClaimSetModal(int claimSetId)
        {
            var existingClaimSet = _getClaimSetByIdQuery.Execute(claimSetId);
            var vendorApplicationCountForClaimSet = _getApplicationsByClaimSetIdQuery.Execute(claimSetId).Count();
            var model = new DeleteClaimSetModel
            {
                Id = claimSetId,
                Name = existingClaimSet.Name,
                IsEditable = existingClaimSet.IsEditable,
                VendorApplicationCount = vendorApplicationCountForClaimSet
            };

            return PartialView("_DeleteClaimSetModal", model);
        }

        [HttpPost]
        public ActionResult DeleteClaimSet(DeleteClaimSetModel model)
        {
            _deleteClaimSetCommand.Execute(model);
            return JsonResult(new
            {
                successMessage = $"Claimset deleted successfully"
            });
        }

        public ActionResult DeleteResourceOnClaimSetModal(DeleteClaimSetResourceModel deleteModel)
        {
            return PartialView("_DeleteClaimSetResourceModal", deleteModel);
        }

        [HttpPost]
        public ActionResult DeleteResourceOnClaimSet(DeleteClaimSetResourceModel model)
        {
            _deleteResourceOnClaimSetCommand.Execute(model);
            return RedirectToAction("EditClaimSet", "ClaimSets", new { claimSetId = model.ClaimSetId });
        }

        public ActionResult ImportExportClaimSet()
        {
            return PartialView("_ImportExportClaimSet", new ClaimSetFileImportModel());
        }

        public ActionResult ExportClaimSetIndex()
        {
            var claimSets = _getClaimSetsByApplicationNameQuery.Execute(CloudOdsAdminApp.SecurityContextApplicationName);
            return PartialView("_ExportClaimSet", new ClaimSetFileExportModel()
            {
                ClaimSets = claimSets
            });
        }

        [HttpPost]
        public ActionResult FileImport(ClaimSetFileImportModel claimSetFileImportModel)
        {
            _claimSetFileImportCommand.Execute(claimSetFileImportModel.AsSharingModel());
            return RedirectToAction("ClaimSets", "GlobalSettings");
        }

        [HttpPost]
        public ActionResult FileExport(ClaimSetFileExportModel claimSetFileExportModel)
        {
            var exports = _claimSetFileExportCommand.Execute(claimSetFileExportModel);
            var currentDate = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
            var exportClaimSetModel = new ExportClaimSetPreviewModel
            {
                DownLoadFileTitle = $"{exports.Title}({currentDate})",
                ExportPreviewString = SharingModel.SerializeFromSharingModel(exports)
            };
            return PartialView("_ExportClaimSetPreview", exportClaimSetModel);
        }
    }
}
