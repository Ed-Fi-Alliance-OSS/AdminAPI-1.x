// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApp.Management.Tests;

public static class StubOdsSecurityModelVersionResolver
{
    public class V3_5 : IOdsSecurityModelVersionResolver
    {
        public EdFiOdsSecurityModelCompatibility DetermineSecurityModel()
            => EdFiOdsSecurityModelCompatibility.ThreeThroughFive;
    }

    public class V6 : IOdsSecurityModelVersionResolver
    {
        public EdFiOdsSecurityModelCompatibility DetermineSecurityModel()
            => EdFiOdsSecurityModelCompatibility.Six;
    }
}
