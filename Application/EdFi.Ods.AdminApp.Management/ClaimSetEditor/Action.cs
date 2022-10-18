// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApp.Management.ClaimSetEditor
{
    public class Action: Enumeration<Action, string>
    {
        public static readonly Action Create = new Action("Create", "Create");
        public static readonly Action Read = new Action("Read", "Read");
        public static readonly Action Update = new Action("Update", "Update");
        public static readonly Action Delete = new Action("Delete", "Delete");

        private Action(string value, string displayName) : base(value, displayName) { }
    }
}
