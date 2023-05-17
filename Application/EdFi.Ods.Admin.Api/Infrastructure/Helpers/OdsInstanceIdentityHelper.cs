﻿// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace EdFi.Ods.AdminApp.Management.Helpers
{
    public static class OdsInstanceIdentityHelper
    {
        public static int GetIdentityValue(string odsInstanceName)
        {
            var index = odsInstanceName.LastIndexOf("_", StringComparison.InvariantCulture);

            var identityValue = odsInstanceName.Substring(index + 1);

            return int.Parse(identityValue);
        }
    }
}
