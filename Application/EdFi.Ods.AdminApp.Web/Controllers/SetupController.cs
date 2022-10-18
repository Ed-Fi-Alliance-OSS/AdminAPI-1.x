// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Data.SqlClient;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Configuration.Application;
using EdFi.Ods.AdminApp.Management.Helpers;
using EdFi.Ods.AdminApp.Web.ActionFilters;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using EdFi.Ods.AdminApp.Web.Infrastructure.HangFire;
using EdFi.Ods.AdminApp.Web.Models.ViewModels;
using log4net;
using Microsoft.Extensions.Options;

namespace EdFi.Ods.AdminApp.Web.Controllers
{
    [BypassSetupRequiredFilter, BypassInstanceContextFilter]
    public class SetupController : ControllerBase
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(SetupController));

        private readonly ICompleteOdsFirstTimeSetupCommand _completeOdsFirstTimeSetupCommand;
        private readonly ICompleteOdsPostUpdateSetupCommand _completeOdsPostUpdateSetupCommand;
        private readonly ApplicationConfigurationService _applicationConfigurationService;
        private readonly AppSettings _appSettings;

        public SetupController(ICompleteOdsFirstTimeSetupCommand completeOdsFirstTimeSetupCommand
            , ICompleteOdsPostUpdateSetupCommand completeOdsPostUpdateSetupCommand
            , ApplicationConfigurationService applicationConfigurationService
            , IOptions<AppSettings> appSettingsAccessor)
        {
            _completeOdsFirstTimeSetupCommand = completeOdsFirstTimeSetupCommand;
            _completeOdsPostUpdateSetupCommand = completeOdsPostUpdateSetupCommand;
            _applicationConfigurationService = applicationConfigurationService;
            _appSettings = appSettingsAccessor.Value;
        }

        public ActionResult FirstTimeSetup()
        {
            var model = new FirstTimeSetupModel
            {
                AreProductImprovementSettingsEnabled = _appSettings.EnableProductImprovementSettings,
            };
            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> CompleteFirstTimeSetup()
        {
            _logger.Info("User intiated First Time Setup");
            return await RunSetup(async () =>
            {
                //HangFire schema init has been deferred since the SQL login provided in the EdFi_Admin
                //connection string by default does not have DBOwner permissions.  Hooking into the first-time
                //setup command gives us the access to run the schema init here.
                _completeOdsFirstTimeSetupCommand.ExtraDatabaseInitializationAction = HangFireInstance.RemoveAllScheduledJobs;

                var restartRequired = await _completeOdsFirstTimeSetupCommand.Execute(_appSettings.DefaultOdsInstance, CloudOdsAdminAppClaimSetConfiguration.Default, CloudOdsAdminAppSettings.Instance.Mode);

                Response.Cookies.Append("RestartRequired", restartRequired.ToString());
            });
        }

        public ActionResult PostUpdateSetup()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> CompletePostUpdateSetup()
        {
            _logger.Info("User initiated Post-Update Setup");
            return await RunSetup(async () =>
            {
                await _completeOdsPostUpdateSetupCommand.Execute(_appSettings.DefaultOdsInstance);
            });
        }

        private async Task<JsonResult> RunSetup(Func<Task> setupAction)
        {
            try
            {
                _logger.Info("Beginning setup process");
                await setupAction();
                _applicationConfigurationService.UpdateFirstTimeSetUpStatus(true);
                _logger.Info("Setup process completed");
                return SetupSucess();
            }
            catch (Exception e)
            {
                _logger.Error("Setup failed", e);
                return SetupFailure(e);
            }
        }

        private JsonResult SetupFailure(Exception e)
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return Json(new { success = false, message = $"Failed to complete setup: {e.Message}", isTransientError = IsTransientError(e) });
        }

        private JsonResult SetupSucess()
        {
            Response.StatusCode = (int)HttpStatusCode.OK;
            return Json(new { success = true, message = "Setup complete" });
        }

        private bool IsTransientError(Exception e)
        {
            return e is SqlException;
        }
    }
}
