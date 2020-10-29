// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Castle.Windsor;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Web.ActionFilters;
using System.Web.Mvc;

namespace EdFi.Ods.AdminApp.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters, IWindsorContainer container)
        {
            filters.Add(new RequreSecureConnectionFilter());
            filters.Add(new AuthorizationFilter());
            filters.Add(new ValidateAntiForgeryTokenOnPostActionsFilter());
            filters.Add(new JsonValidationFilter());
            filters.Add(new HandleAjaxErrorAttribute());
            filters.Add(new DoNotCacheAjaxRequestsFilter());
            
            var odsStatusQuery = container.Resolve<IGetOdsStatusQuery>();
            filters.Add(new SetupRequiredFilter(odsStatusQuery));

            filters.Add(new UserContextFilter());

            filters.Add(new PasswordChangeRequiredFilter());

            filters.Add(new InstanceContextFilter());

            filters.Add(new PermissionRequiredFilter());
        }
    }
}
