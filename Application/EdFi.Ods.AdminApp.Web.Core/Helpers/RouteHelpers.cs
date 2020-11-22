// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq.Expressions;
#if NET48
using System.Web.Mvc;
using System.Web.Routing;
#else
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
#endif

namespace EdFi.Ods.AdminApp.Web.Helpers
{
    public static class RouteHelpers
    {
        public static string GetControllerName(this Type controllerType)
        {
            var controllerName = controllerType.Name.Replace("Controller", string.Empty);
            return controllerName;
        }

        public static string GetControllerName<TController>()
        {
            return GetControllerName(typeof(TController));
        }

        public static string GetControllerName<TController>(this Expression<Func<TController, object>> actionExpression)
        {
            var controllerName = typeof(TController).Name.Replace("Controller", string.Empty);
            return controllerName;
        }

        public static string GetActionName<TController>(this Expression<Func<TController, object>> actionExpression)
        {
            var actionName = ((MethodCallExpression)actionExpression.Body).Method.Name;
            return actionName;
        }
        
        public static RouteValueDictionary GetRoute<TController>(this Expression<Func<TController, object>> actionExpression)
        {
            var result = new RouteValueDictionary();
            var expressionBody = (MethodCallExpression)actionExpression.Body;

            var parameters = expressionBody.Method.GetParameters();

            //expression tree cannot represent a call with optional params
            //so our method param count and should match the expression body arg count
            //but just the same, let's check...
            if (parameters.Length != expressionBody.Arguments.Count)
                throw new InvalidOperationException("Mismatched parameter/argument count");

            for (var i = 0; i < expressionBody.Arguments.Count; ++i)
            {
                var parameter = parameters[i];
                var argument = expressionBody.Arguments[i];

                var parameterName = parameter.Name;
                var argumentValue = argument.GetValue();

                result.Add(parameterName, argumentValue);
            }

            result["controller"] = typeof(TController).GetControllerName();
            result["action"] = actionExpression.GetActionName();

            return result;
        }

        private static object GetValue(this Expression exp)
        {
            var objectMember = Expression.Convert(exp, typeof(object));
            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
            var getter = getterLambda.Compile();

            return getter();
        }

        public static RedirectToRouteResult RedirectToActionRoute<TController>(Expression<Func<TController, object>> action)
        {
            var route = GetRoute(action);
            return new RedirectToRouteResult("Default", route);
        }
    }
}
