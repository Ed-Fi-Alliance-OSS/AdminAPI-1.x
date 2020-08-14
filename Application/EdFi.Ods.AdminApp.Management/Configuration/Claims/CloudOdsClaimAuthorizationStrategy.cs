// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApp.Management.Configuration.Claims
{
    public sealed class CloudOdsClaimAuthorizationStrategy : Enumeration<CloudOdsClaimAuthorizationStrategy>
    {
        public string StrategyName => DisplayName;
        
        public static CloudOdsClaimAuthorizationStrategy NoFurtherAuthorizationRequired = new CloudOdsClaimAuthorizationStrategy(1, "NoFurtherAuthorizationRequired");

        private CloudOdsClaimAuthorizationStrategy(int value, string displayName) : base(value, displayName)
        {
        }
    }
}