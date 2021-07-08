// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Net;

namespace EdFi.Ods.AdminApp.Management.Services
{
    public interface ISimpleGetRequest
    {
        public string DownloadString(string address);
    }

    public class SimpleGetRequest : ISimpleGetRequest
    {
        public string DownloadString(string address)
        {
            using var client = new WebClient();

            return client.DownloadString(address);
        }
    }
}
