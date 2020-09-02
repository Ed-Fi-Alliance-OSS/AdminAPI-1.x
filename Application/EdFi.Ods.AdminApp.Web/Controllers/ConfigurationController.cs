// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Web.Mvc;
using EdFi.Ods.AdminApp.Management.Configuration.Application;
using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Web.ActionFilters;

namespace EdFi.Ods.AdminApp.Web.Controllers
{
    [BypassSetupRequiredFilter]
    [BypassInstanceContextFilter]
    public class ConfigurationController : ControllerBase
    {
        private readonly EditConfiguration.QueryHandler _editConfigurationQuery;
        private readonly EditConfiguration.CommandHandler _editConfigurationCommand;

        public ConfigurationController(
            EditConfiguration.QueryHandler editConfigurationQuery,
            EditConfiguration.CommandHandler editConfigurationCommand)
        {
            _editConfigurationQuery = editConfigurationQuery;
            _editConfigurationCommand = editConfigurationCommand;
        }

        public ActionResult Edit()
        {
            return View(_editConfigurationQuery.Execute());
        }

        [HttpPost]
        [PermissionRequired(Permission.AccessGlobalSettings)]
        public ActionResult Edit(EditConfiguration.Command command)
        {
            _editConfigurationCommand.Execute(command);

            return JsonSuccess("Successfully updated configuration");
        }
    }
}