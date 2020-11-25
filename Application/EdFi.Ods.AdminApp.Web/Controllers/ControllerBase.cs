// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using EdFi.Ods.AdminApp.Web.Helpers;

namespace EdFi.Ods.AdminApp.Web.Controllers
{
    public class ControllerBase : Controller
    {
        protected ActionResult RedirectToAction<TController>(Expression<Func<TController, object>> actionExpression) 
            where TController : Controller
        {
            return RouteHelpers.RedirectToActionRoute(actionExpression);
        }

        protected ActionResult RedirectToActionJson<TController>(Expression<Func<TController, object>> actionExpression, string successMessage = null) 
            where TController : Controller
        {
            return RedirectToActionJson(actionExpression, null, successMessage);
        }

        protected ActionResult RedirectToActionJson<TController>(Expression<Func<TController, object>> actionExpression, object routeValues, string successMessage = null) 
            where TController : Controller
        {
            var controllerName = actionExpression.GetControllerName();
            var actionName = actionExpression.GetActionName();

            return JsonResult(new
            {
                redirect = Url.Action(actionName, controllerName, routeValues),
                successMessage = successMessage
            });
        }

        protected ActionResult JsonSuccess(string successMessage)
        {
            return JsonResult(new
            {
                successMessage = successMessage
            });
        }

        protected ActionResult JsonError(string errorMessage)
        {
            return JsonResult(new
            {
                errorMessage = errorMessage
            });
        }

        protected ContentResult JsonResult(object model)
        {
            return ResponseHelpers.JsonResult(model);
        }

        protected void SuccessToastMessage(string message)
        {
            Toast(message, "success");
        }

        protected void ErrorToastMessage(string message)
        {
            Toast(message, "error");
        }

        private void Toast(string message, string type)
        {
            TempData["ToastMessage"] = message;
            TempData["ToastType"] = type;
        }
    }
}
