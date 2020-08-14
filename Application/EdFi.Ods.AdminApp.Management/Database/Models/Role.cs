// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApp.Management.Database.Models
{
    public class Role : Enumeration<Role, int>
    {
        public static readonly Role SuperAdmin = new Role(1, "Super Administrator");
        public static readonly Role Admin = new Role(2, "Administrator");
       
        private Role(int value, string displayName)
            : base(value, displayName)
        {
        }
    }
}