// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using EdFi.Ods.AdminApp.Web.Helpers;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests.Controllers
{
    [TestFixture]
    public class ControllerBaseTester
    {
        [Test, TestCaseSource(nameof(ListOfControllers))]
        public void AllControllersShouldInheritFromControllerBaseClass(Type controllerType)
        {
            //This test was created to enforce inheritance from ControllerBase since there's exception handling/logging 
            //baked in to the OnException method there, and we don't want to duplicate error logging code in a HandleErrorAttribute

            controllerType.IsAssignableTo<EdFi.Ods.AdminApp.Web.Controllers.ControllerBase>().ShouldBeTrue($"{controllerType.Name} must inherit from {nameof(EdFi.Ods.AdminApp.Web.Controllers.ControllerBase)}");
        }

        private static IEnumerable<Type> ListOfControllers
        {
            get
            {
                return Assembly.GetAssembly(typeof(EdFi.Ods.AdminApp.Web.Controllers.ControllerBase))
                .GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(IController)));
            }
        } 
    }
}
