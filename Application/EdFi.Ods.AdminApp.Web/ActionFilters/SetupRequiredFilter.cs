// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

#if NET48
using System.Web.Mvc;
#else
using Microsoft.AspNetCore.Mvc.Filters;
#endif
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Management.Configuration.Application;
using EdFi.Ods.AdminApp.Web.Controllers;
using EdFi.Ods.AdminApp.Web.Helpers;
using EdFi.Ods.AdminApp.Web.Infrastructure;

namespace EdFi.Ods.AdminApp.Web.ActionFilters
{
    public class SetupRequiredFilter : ActionFilterAttribute
    {
        private readonly IGetOdsStatusQuery _getOdsStatusQuery;
        private readonly ApplicationConfigurationService _applicationConfigurationService;

        public SetupRequiredFilter(IGetOdsStatusQuery getOdsStatusQuery, ApplicationConfigurationService applicationConfigurationService)
        {
            _getOdsStatusQuery = getOdsStatusQuery;
            _applicationConfigurationService = applicationConfigurationService;
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
            return _applicationConfigurationService.IsFirstTimeSetUpCompleted();
        }

        private bool OdsInstanceFirstTimeSetupCompleted()
        {
            var defaultInstanceName = CloudOdsAdminAppSettings.Instance.OdsInstanceName;
            var status = _getOdsStatusQuery.Execute(defaultInstanceName);
            return status != null && status == CloudOdsStatus.Ok;
        }
    }
}
