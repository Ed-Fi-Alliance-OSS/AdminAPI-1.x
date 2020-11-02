// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
#if NET48
using System.Web.Mvc;
#else
using Microsoft.AspNetCore.Mvc.Rendering;
#endif
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;


namespace EdFi.Ods.AdminApp.Web.Infrastructure
{
    public class ResourceClaimSelectListBuilder
    {
        public static List<SelectListItem> GetSelectListForResourceClaims(List<ResourceClaim> allResourceClaims)
        {
            var selectList = new List<SelectListItem>{
                new SelectListItem{ Text="Please select a value", Value = "0" , Disabled = true, Selected = true},
            };
            var parentGroup = new SelectListGroup
            {
                Name = "Groups"
            };
            var childGroup = new SelectListGroup
            {
                Name = "Resources"
            };
            var parentGroupList = new List<SelectListItem>();
            var childGroupList = new List<SelectListItem>();
            foreach (var resourceClaim in allResourceClaims)
            {
                var item = new SelectListItem
                {
                    Text = resourceClaim.Name,
                    Value = resourceClaim.Id.ToString(),
                    Group = parentGroup
                };
                parentGroupList.Add(item);
                if (resourceClaim.Children.Count > 0)
                {
                    childGroupList.AddRange(resourceClaim.Children.Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString(),
                        Group = childGroup
                    }));
                }
            }
            parentGroupList = parentGroupList.OrderBy(x => x.Text).ToList();
            parentGroupList.AddRange(childGroupList.OrderBy(x => x.Text));
            #if NET48
                selectList.AddRange(new SelectList(parentGroupList, "Value", "Text", "Group.Name", -1));
            #else
                selectList.AddRange(new SelectList(parentGroupList, "Value", "Text", -1, "Group.Name"));
            #endif
            return selectList;
        }
    }
}
