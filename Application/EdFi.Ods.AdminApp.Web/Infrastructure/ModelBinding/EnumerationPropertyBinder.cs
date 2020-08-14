// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.ComponentModel;
using System.Reflection;
using System.Web.Mvc;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Web.Helpers;

namespace EdFi.Ods.AdminApp.Web.Infrastructure.ModelBinding
{
	public class EnumerationPropertyBinder : FilteredPropertyBinderBase
	{
		public override bool ShouldBind(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
		{
			return propertyDescriptor.PropertyType.Closes(typeof(Enumeration<>));
		}

		public override object GetPropertyValue(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
		{
			try
			{
				var value = GetValue(bindingContext, propertyDescriptor);

				if (value == null)
				{
					return null;
				}

				var enumeration = GetEnumeration(propertyDescriptor.PropertyType, value.AttemptedValue);
				return enumeration;
			}
			catch (Exception ex)
			{
				var message = $"Unable to locate a valid value for query string parameter '{bindingContext.ModelName}'";
				throw new ApplicationException(message, ex);
			}
		}

		private static object GetEnumeration(IReflect enumerationType, string value)
		{
            if (string.IsNullOrEmpty(value))
                return null;

            int listItemValue;

            if (int.TryParse(value, out listItemValue))
            {
                var fromValueMethod = enumerationType.GetMethod("FromValue", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy, null, new[] { typeof(int) }, null);
                return fromValueMethod.Invoke(null, new object[] { listItemValue });
            }

            var parseMethod = enumerationType.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy, null, new[] { typeof(string) }, null);
            return parseMethod.Invoke(null, new object[] { value });
        }
	}
}