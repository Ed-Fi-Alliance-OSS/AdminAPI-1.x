// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

#if NET48
using System.Web.Mvc;
#else
using Microsoft.AspNetCore.Mvc.Filters;
#endif
using System.Linq;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Configuration.Application;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Web.Controllers;
using EdFi.Ods.AdminApp.Web.Helpers;
using EdFi.Ods.AdminApp.Web.Infrastructure;

namespace EdFi.Ods.AdminApp.Web.ActionFilters
{
    public class SetupRequiredFilter : ActionFilterAttribute
    {
        private readonly IGetOdsStatusQuery _getOdsStatusQuery;
        #if !NET48
        private readonly AdminAppDbContext _database;
        #endif

        public SetupRequiredFilter(IGetOdsStatusQuery getOdsStatusQuery
            #if !NET48
            , AdminAppDbContext database
            #endif
            )
        {
            _getOdsStatusQuery = getOdsStatusQuery;

            #if !NET48
            _database = database;
            #endif
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (ShouldBypassThisFilter(filterContext))
                return;

            filterContext.Result = OdsInstanceFirstTimeSetupCompleted()
                ? RouteHelpers.RedirectToActionRoute<SetupController>(x => x.PostUpdateSetup())
                : RouteHelpers.RedirectToActionRoute<SetupController>(x => x.FirstTimeSetup());
        }

        private bool ShouldBypassThisFilter(ActionExecutingContext filterContext)
        {
            return filterContext.Controller.GetType().GetAttribute<BypassSetupRequiredFilterAttribute>() != null
                || GeneralFirstTimeSetUpCompleted();
        }

        private bool GeneralFirstTimeSetUpCompleted()
        {
            #if NET48
            using (var _database = new AdminAppDbContext())
            #endif
            {
                var generalFirstTimeSetUpCompleted = _database
                                                         .ApplicationConfigurations
                                                         .SingleOrDefault()?
                                                         .FirstTimeSetUpCompleted ?? false;

                return generalFirstTimeSetUpCompleted;
            }
        }

        private bool OdsInstanceFirstTimeSetupCompleted()
        {
            var defaultInstanceName = CloudOdsAdminAppSettings.Instance.OdsInstanceName;
            var status = _getOdsStatusQuery.Execute(defaultInstanceName);
            return status != null && status == CloudOdsStatus.Ok;
        }
    }
}
