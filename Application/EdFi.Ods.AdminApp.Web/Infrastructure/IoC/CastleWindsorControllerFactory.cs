// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.MicroKernel;

namespace EdFi.Ods.AdminApp.Web.Infrastructure.IoC
{
    public class CastleWindsorControllerFactory : DefaultControllerFactory
    {
        private readonly IKernel _kernel;

        public CastleWindsorControllerFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public override void ReleaseController(IController controller)
        {
            _kernel.ReleaseComponent(controller);
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                throw new HttpException(404, $"The controller for path '{requestContext.HttpContext.Request.Path}' could not be found.");
            }

            return (IController)_kernel.Resolve(controllerType);
        }
    }
}