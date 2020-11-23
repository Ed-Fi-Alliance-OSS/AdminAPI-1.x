// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using EdFi.Ods.AdminApp.Web.Helpers;
using log4net;

namespace EdFi.Ods.AdminApp.Web.Controllers
{
    public class ControllerBase : Controller
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(ControllerBase));

        public ActionResult RedirectToAction<TController>(Expression<Func<TController, object>> actionExpression) 
            where TController : Controller
        {
            return RouteHelpers.RedirectToActionRoute(actionExpression);
        }

        public ActionResult RedirectToActionJson<TController>(Expression<Func<TController, object>> actionExpression, string successMessage = null) 
            where TController : Controller
        {
            return RedirectToActionJson(actionExpression, null, successMessage);
        }

        public ActionResult RedirectToActionJson<TController>(Expression<Func<TController, object>> actionExpression, object routeValues, string successMessage = null) 
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

        public ActionResult JsonSuccess(string successMessage)
        {
            return JsonResult(new
            {
                successMessage = successMessage
            });
        }

        public ActionResult JsonError(string errorMessage)
        {
            return JsonResult(new
            {
                errorMessage = errorMessage
            });
        }

        public ContentResult JsonResult(object model)
        {
            return ResponseHelpers.JsonResult(model);
        }

        #if NET48
        protected override void OnException(ExceptionContext exceptionContext)
        {
            if (exceptionContext.ExceptionHandled)
                return;

            _logger.Error("Unhandled exception", exceptionContext.Exception);

            var controllerName = (exceptionContext.RouteData.Values["controller"] as string) ?? "";
            var actionName = (exceptionContext.RouteData.Values["action"] as string) ?? "";

            exceptionContext.ExceptionHandled = true;
            exceptionContext.Result = new ViewResult
            {
                ViewName = "~/Views/Shared/Error.cshtml",
                ViewData = new ViewDataDictionary(new HandleErrorInfo(exceptionContext.Exception, controllerName, actionName))
            };
        }
        #endif

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
