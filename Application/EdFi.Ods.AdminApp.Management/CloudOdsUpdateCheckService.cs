// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace EdFi.Ods.AdminApp.Management
{
    public class CloudOdsUpdateCheckService 
    {
        [Obsolete]
        public bool VersionInformationIsValid(CloudOdsUpdateInfo cloudOdsUpdateInfo)
        {
            return cloudOdsUpdateInfo.LatestPublishedVersion != null &&
                   cloudOdsUpdateInfo.CurrentInstanceVersion != null;
        }

        [Obsolete]
        public bool UpdateAvailable(CloudOdsUpdateInfo cloudOdsUpdateInfo)
        {
            return VersionInformationIsValid(cloudOdsUpdateInfo) &&
                   cloudOdsUpdateInfo.CurrentInstanceVersion < cloudOdsUpdateInfo.LatestPublishedVersion;
        }

        [Obsolete]
        public bool UpdateIsCompatible(CloudOdsUpdateInfo cloudOdsUpdateInfo)
        {
            return VersionInformationIsValid(cloudOdsUpdateInfo) &&
                   UpdateAvailable(cloudOdsUpdateInfo) &&
                   cloudOdsUpdateInfo.CurrentInstanceVersion.Major == cloudOdsUpdateInfo.LatestPublishedVersion.Major;
        }
    }
}
