// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;

namespace EdFi.Ods.AdminApp.Web.Infrastructure.ModelBinding
{
	public class CompositeModelBinder : DefaultModelBinder
	{
		private readonly IEnumerable<IFilteredPropertyBinder> _propertyBinders;
		private readonly IEnumerable<IFilteredModelBinder> _modelBinders;

		public CompositeModelBinder()
		{
			_propertyBinders = new IFilteredPropertyBinder[]
			                   	{
			                   		new EnumerationPropertyBinder()
								};

			_modelBinders = new IFilteredModelBinder[]
			                	{
			                		new EnumerationModelBinder()
			                   	};
		}

		public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			var matchingBinders = _modelBinders.Where(b => b.ShouldBind(controllerContext, bindingContext));
			if (matchingBinders.Any())
				return matchingBinders.Select(result => result.GetModelValue(controllerContext, bindingContext)).FirstOrDefault();

			return base.BindModel(controllerContext, bindingContext);
		}

		protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
		{
			var matchingBinders = _propertyBinders.Where(b => b.ShouldBind(controllerContext, bindingContext, propertyDescriptor));

			foreach (var result in matchingBinders.Select(filteredModelBinder => filteredModelBinder.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor)))
			{
				propertyDescriptor.SetValue(bindingContext.Model, result);
				return;
			}

			base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
		}
	}
}