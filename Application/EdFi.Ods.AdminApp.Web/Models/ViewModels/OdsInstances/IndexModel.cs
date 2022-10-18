// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using EdFi.Ods.AdminApp.Management;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.OdsInstances
{
    public class IndexModel
    {
        public AdminAppUserContext UserContext { get; set; }
        public List<OdsInstanceModel> OdsInstances { get; set; }
    }

    public class OdsInstanceModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DatabaseName { get; set; }
        public string SchoolYearDescription { get; set; }
    }
}
