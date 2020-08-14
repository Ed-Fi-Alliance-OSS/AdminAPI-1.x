// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.ComponentModel;
using System.Web.Mvc;

namespace EdFi.Ods.AdminApp.Web.Infrastructure.ModelBinding
{
	public abstract class FilteredPropertyBinderBase : IFilteredPropertyBinder
	{
		protected static ValueProviderResult GetValue(ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
		{
			var prefix = "";

			if (!string.IsNullOrWhiteSpace(bindingContext.ModelName))
				prefix = bindingContext.ModelName + ".";

			return bindingContext.ValueProvider.GetValue(prefix + propertyDescriptor.Name);
		}

		public abstract bool ShouldBind(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor);
		public abstract object GetPropertyValue(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor);
	}
}