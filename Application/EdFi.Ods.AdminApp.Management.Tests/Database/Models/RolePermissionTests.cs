// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using EdFi.Ods.AdminApp.Management.Database.Models;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests.Database.Models
{
    public class RolePermissionTests
    {
        [Test]
        public void SuperAdminRoleShouldHaveAllPermissions()
        {
            //NOTE: This test asserts the original fact that super admins have
            //      all of the permissions. If this test begins to fail, the
            //      immediate question to answer is whether that general rule
            //      still holds, or whether the assertion needs to change.

            var allPermissions = ((Permission[]) Enum.GetValues(typeof(Permission)))
                .OrderBy(x => (int)x);

            RolePermission.GetPermissions(Role.SuperAdmin.Value.ToString())
                .OrderBy(x => (int) x)
                .ShouldBe(allPermissions);
        }

        [Test]
        public void AdminRoleShouldHaveNoPermissions()
        {
            //NOTE: This test asserts the original fact that non-super admins have
            //      none of the explicit permissions, as all of them describe things
            //      that super admins do in contrast to non-super admins. If this test
            //      begins to fail, the immediate question to answer is whether that
            //      general rule still holds, or whether the assertion needs to change.

            RolePermission.GetPermissions(Role.Admin.Value.ToString())
                .ShouldBeEmpty();
        }

        [Test]
        public void UnrecognizedRolesShouldHaveNoPermissions()
        {
            RolePermission.GetPermissions(null).ShouldBeEmpty();
            RolePermission.GetPermissions(Guid.NewGuid().ToString()).ShouldBeEmpty();
            RolePermission.GetPermissions(int.MaxValue.ToString()).ShouldBeEmpty();
        }
    }
}
