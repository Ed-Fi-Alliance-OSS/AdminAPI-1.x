// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Castle.MicroKernel;
using FluentValidation;
using System;

namespace EdFi.Ods.AdminApp.Web.Infrastructure.IoC
{
    public class CastleWindsorFluentValidatorFactory : ValidatorFactoryBase
    {
        private readonly IKernel _kernel;

        public CastleWindsorFluentValidatorFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public override IValidator CreateInstance(Type validatorType)
        {
            return _kernel.HasComponent(validatorType)
             ? (_kernel.Resolve(validatorType) as IValidator)
             : null;
        }
    }
}