// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Ods.AdminApp.Management.User;
using EdFi.Ods.AdminApp.Web.ActionFilters;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Web.Helpers;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.OdsInstances;
using Microsoft.AspNet.Identity;
using System.Linq;

namespace EdFi.Ods.AdminApp.Web.Controllers
{
    [BypassInstanceContextFilter]
    public class OdsInstancesController : ControllerBase
    {
        private readonly RegisterOdsInstanceCommand _registerOdsInstanceCommand;
        private readonly DeregisterOdsInstanceCommand _deregisterOdsInstanceCommand;
        private readonly IGetOdsInstanceRegistrationsByUserIdQuery _getOdsInstanceRegistrationsByUserIdQuery;
        private readonly IGetOdsInstanceRegistrationsQuery _getOdsInstanceRegistrationsQuery;
        private readonly AdminAppUserContext _userContext;
        private readonly IMapper _mapper;
        private readonly BulkRegisterOdsInstancesCommand _bulkRegisterOdsInstancesCommand;

        public OdsInstancesController(IMapper mapper
            , RegisterOdsInstanceCommand registerOdsInstanceCommand
            , DeregisterOdsInstanceCommand deregisterOdsInstanceCommand
            , IGetOdsInstanceRegistrationsByUserIdQuery getOdsInstanceRegistrationsByUserIdQuery
            , IGetOdsInstanceRegistrationsQuery getOdsInstanceRegistrationsQuery
            , AdminAppUserContext userContext
            , BulkRegisterOdsInstancesCommand bulkRegisterOdsInstancesCommand)
        {
            _mapper = mapper;
            _registerOdsInstanceCommand = registerOdsInstanceCommand;
            _deregisterOdsInstanceCommand = deregisterOdsInstanceCommand;
            _getOdsInstanceRegistrationsByUserIdQuery = getOdsInstanceRegistrationsByUserIdQuery;
            _getOdsInstanceRegistrationsQuery = getOdsInstanceRegistrationsQuery;
            _userContext = userContext;
            _bulkRegisterOdsInstancesCommand = bulkRegisterOdsInstancesCommand;
        }

        public ViewResult Index()
        {
            var instances = _getOdsInstanceRegistrationsByUserIdQuery.Execute(User.Identity.GetUserId());

            var model = new IndexModel
            {
                UserContext = _userContext,
                OdsInstances = _mapper.Map<List<OdsInstanceModel>>(instances)
            };

            return View(model);
        }

        [PermissionRequired(Permission.AccessGlobalSettings)]
        public ViewResult RegisterOdsInstance()
        {
            return View(new RegisterOdsInstanceModel());
        }

        [HttpPost]
        [PermissionRequired(Permission.AccessGlobalSettings)]
        public async Task<ActionResult> RegisterOdsInstance(RegisterOdsInstanceModel model)
        {
            var currentUserId = User.Identity.GetUserId();

            await _registerOdsInstanceCommand.Execute(model,
                CloudOdsAdminAppSettings.Instance.Mode, currentUserId, CloudOdsAdminAppClaimSetConfiguration.Default);

            return RedirectToAction("Index", "OdsInstances");
        }

        [PermissionRequired(Permission.AccessGlobalSettings)]
        public ViewResult BulkRegisterOdsInstances()
        {
            return View(new BulkRegisterOdsInstancesModel());
        }

        [HttpPost]
        [PermissionRequired(Permission.AccessGlobalSettings)]
        public async Task<ActionResult> BulkRegisterOdsInstances(BulkRegisterOdsInstancesModel model)
        {
            var currentUserId = User.Identity.GetUserId();

            var results = await _bulkRegisterOdsInstancesCommand.Execute(
                model.OdsInstancesFile.DataRecords(),
                CloudOdsAdminAppSettings.Instance.Mode, currentUserId,
                CloudOdsAdminAppClaimSetConfiguration.Default);

            var bulkRegisterOdsInstancesResults = results.ToList();
            var successCount = bulkRegisterOdsInstancesResults.Count(x => x.Success);
            var failCount = bulkRegisterOdsInstancesResults.Count(x => !x.Success);
            return JsonSuccess(
                $"Successful instance registrations: {successCount}. Failed instance registrations: {failCount}. Please refer log file for further details.");
        }

        public ActionResult ActivateOdsInstance(string instanceId)
        {
            var myCookie = new HttpCookie("Instance", instanceId);

            Response.Cookies.Add(myCookie);

            return RedirectToAction("Applications", "OdsInstanceSettings");
        }

        [PermissionRequired(Permission.AccessGlobalSettings)]
        public ActionResult DeregisterOdsInstance(int instanceId)
        {
            var odsInstance = _getOdsInstanceRegistrationsQuery.Execute(instanceId);

            return PartialView("_DeregisterOdsInstanceModal", new DeregisterOdsInstanceModel
            {
                OdsInstanceId = odsInstance.Id,
                Name = odsInstance.Name,
                Description = odsInstance.Description
            });
        }

        [HttpPost]
        [PermissionRequired(Permission.AccessGlobalSettings)]
        public ActionResult DeregisterOdsInstance(DeregisterOdsInstanceModel model)
        {
            _deregisterOdsInstanceCommand.Execute(model);

            var instanceCookie = Request.Cookies.Get("Instance");

            if (instanceCookie != null && instanceCookie.Value == model.OdsInstanceId.ToString())
            {
                instanceCookie.Expires = DateTime.Now.AddDays(-1);
                instanceCookie.Value = null;
                Response.Cookies.Add(instanceCookie);
            }

            return RedirectToActionJson<OdsInstancesController>(x => x.Index(), "Ods Instance deregistered successfully.");
        }
    }
}