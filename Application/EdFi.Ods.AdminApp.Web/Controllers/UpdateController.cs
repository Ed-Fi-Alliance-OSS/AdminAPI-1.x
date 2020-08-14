// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading.Tasks;
using System.Web.Mvc;
using EdFi.Ods.AdminApp.Web.Infrastructure;

namespace EdFi.Ods.AdminApp.Web.Controllers
{
    public class UpdateController : ControllerBase
    {
        private readonly CloudOdsUpdateService _cloudOdsUpdateService;

        public UpdateController(CloudOdsUpdateService cloudOdsUpdateService)
        {
            _cloudOdsUpdateService = cloudOdsUpdateService;
        }

        public async Task<ActionResult> Index()
        {
            var model = await _cloudOdsUpdateService.GetUpdateInfo();
            return View(model);
        }
    }
}