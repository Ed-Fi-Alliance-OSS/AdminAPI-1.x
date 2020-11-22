// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.Application
{
    public class ApplicationKeyModel
    {
        public string ApplicationName { get; set; }
        public string Key { get; set; }
        public string Secret { get; set; }
        public string ApiUrl { get; set; }
    }
}
