// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace EdFi.Ods.AdminApp.Management
{
    public class CloudOdsUpdateInfo
    {
        public CloudOdsInstance Instance { get; set; }
        public Version LatestPublishedVersion { get; set; }
        public Version CurrentInstanceVersion { get; set; }
        public bool VersionInformationIsValid { get; set; }
        public bool UpdateIsCompatible { get; set; }
    }
}
