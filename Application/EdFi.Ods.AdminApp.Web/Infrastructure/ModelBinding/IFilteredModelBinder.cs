// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Web.Mvc;

namespace EdFi.Ods.AdminApp.Web.Infrastructure.ModelBinding
{
	public interface IFilteredModelBinder
	{
		bool ShouldBind(ControllerContext controllerContext, ModelBindingContext bindingContext);
		object GetModelValue(ControllerContext controllerContext, ModelBindingContext bindingContext);
	}
}