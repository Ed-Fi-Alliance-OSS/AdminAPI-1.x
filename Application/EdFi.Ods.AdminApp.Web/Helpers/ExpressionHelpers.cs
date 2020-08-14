// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq.Expressions;

namespace EdFi.Ods.AdminApp.Web.Helpers
{
    public static class ExpressionHelpers
    {
        public static Expression<Func<TModel, TToProperty>> Cast<TModel, TFromProperty, TToProperty>(this Expression<Func<TModel, TFromProperty>> expression)
        {
            Expression converted = Expression.Convert(expression.Body, typeof(TToProperty));
            return Expression.Lambda<Func<TModel, TToProperty>>(converted, expression.Parameters);
        }
    }
}