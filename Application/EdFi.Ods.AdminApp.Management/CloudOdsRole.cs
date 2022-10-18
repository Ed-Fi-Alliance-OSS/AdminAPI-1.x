// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApp.Management
{
    public class CloudOdsRole : Enumeration<CloudOdsRole>
    {
        public static readonly CloudOdsRole AdminApp = new CloudOdsRole(1, "AdminApp");
        public static readonly CloudOdsRole Api = new CloudOdsRole(2, "Api");
        public static readonly CloudOdsRole Swagger = new CloudOdsRole(3, "Swagger");
        

        protected CloudOdsRole(int value, string displayName) : base(value, displayName)
        {
        }
    }
}
