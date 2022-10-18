// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApp.Management.Configuration.Claims
{
    public sealed class CloudOdsClaimAction : Enumeration<CloudOdsClaimAction>
    {
        public string ActionName => DisplayName;

        public static CloudOdsClaimAction Create = new CloudOdsClaimAction(1, "Create");
        public static CloudOdsClaimAction Read = new CloudOdsClaimAction(2, "Read");
        public static CloudOdsClaimAction Update = new CloudOdsClaimAction(3, "Update");
        public static CloudOdsClaimAction Delete = new CloudOdsClaimAction(4, "Delete");

        private CloudOdsClaimAction(int value, string displayName) : base(value, displayName)
        {
        }
    }
}