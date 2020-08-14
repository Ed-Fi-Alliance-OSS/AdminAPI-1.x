// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Net;

namespace EdFi.Ods.AdminApp.Management
{
    public class GetLatestPublishedOdsVersionQuery : IGetLatestPublishedOdsVersionQuery
    {
        private const string LatestVersionFileUrl = "https://odsassets.blob.core.windows.net/public/CloudOds/deploy/release/LatestVersion.txt";

        public string Execute()
        {
            using (var client = new WebClient())
            {
                var version = client.DownloadString(LatestVersionFileUrl);
                return version;
            }   
        }
    }
}
