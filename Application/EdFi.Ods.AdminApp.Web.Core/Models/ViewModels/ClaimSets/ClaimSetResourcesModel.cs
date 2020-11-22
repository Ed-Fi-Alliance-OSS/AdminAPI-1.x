// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
#if NET48
using System.Web.Mvc;
#else
using Microsoft.AspNetCore.Mvc.Rendering;
#endif
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.ClaimSets
{
    public class ClaimSetResourcesModel
    {
        public int ClaimSetId { get; set; }
        public string ClaimSetName { get; set; }
        public IEnumerable<ResourceClaim> ResourceClaims { get; set; }
        public List<SelectListItem> AllResourceClaims { get; set; }

        public ClaimSetResourcesModel()
        {
            ResourceClaims = new List<ResourceClaim>();
        }
    }
}
