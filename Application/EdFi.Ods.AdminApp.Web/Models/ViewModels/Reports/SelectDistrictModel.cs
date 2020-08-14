// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using EdFi.Ods.AdminApp.Web.Display.TabEnumeration;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.Reports
{
    public class SelectDistrictModel
    {
        [Display(Name ="Select District")]
        public int LocalEducationAgencyId { get; set; }
        public ITabEnumeration Tab { get; set; }
        public SelectListItem[] DistrictOptions { get; set; }
    }
}