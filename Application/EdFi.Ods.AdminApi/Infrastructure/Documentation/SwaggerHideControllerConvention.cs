// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace EdFi.Ods.AdminApi.Infrastructure.Documentation
{
    public class SwaggerHideControllerConvention : IActionModelConvention
    {
        private readonly IConfiguration _configuration;
        private readonly List<string> _controllerNames;

        public SwaggerHideControllerConvention(IConfiguration configuration, List<string> controllerNames)
        {
            _configuration = configuration;
            _controllerNames = controllerNames;
        }

        public void Apply(ActionModel action)
        {
            if (_controllerNames.Contains(action.Controller.ControllerType.Name))
            {
                var hideController = !_configuration.GetValue<bool>("AppSettings:UseSelfcontainedAuthorization");

                if (hideController)
                {
                    action.ApiExplorer.IsVisible = false;
                }
            }
        }
    }
}
