// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using EdFi.Security.DataAccess.Contexts;
using Newtonsoft.Json.Linq;

namespace EdFi.Ods.AdminApp.Management.ClaimSetEditor
{
    public class ClaimSetFileExportCommand
    {
        private readonly ISecurityContext _context;
        private readonly IGetResourcesByClaimSetIdQuery _getResourcesByClaimSetIdQuery;

        public ClaimSetFileExportCommand(ISecurityContext context, IGetResourcesByClaimSetIdQuery getResourcesByClaimSetIdQuery)
        {
            _context = context;
            _getResourcesByClaimSetIdQuery = getResourcesByClaimSetIdQuery;
        }

        public SharingModel Execute(IClaimSetFileExportModel model)
        {
            var sharingClaimSets = ClaimSetExports(_context, model.SelectedForExport.ToArray());
            var sharingTemplate = new SharingTemplate
            {
                ClaimSets = sharingClaimSets.ToArray()
            };

            return new SharingModel
            {
                Title = model.Title,
                Template = sharingTemplate
            };
        }

        private List<SharingClaimSet> ClaimSetExports(ISecurityContext context, int[] selectedIds)
        {
            if (!selectedIds.Any())
                return new List<SharingClaimSet>();
            var sharingClaimSets = new List<SharingClaimSet>();
            foreach (var claimSetId in selectedIds)
            {
                var sharingClaimSet = new SharingClaimSet();
                var claimSet = context.ClaimSets
                    .Single(x => x.ClaimSetId == claimSetId);
                sharingClaimSet.Name = claimSet.ClaimSetName;

                var resources = _getResourcesByClaimSetIdQuery.AllResources(claimSetId);
                sharingClaimSet.ResourceClaims =
                    resources.Select(x =>
                    {
                        var jsonObject = JObject.FromObject(x);
                        jsonObject = JsonObjectManipulation.RemoveProperties(jsonObject, new List<string>
                        {
                            "ParentId",
                            "Id"
                        });
                        return jsonObject;
                    }).ToList();
                sharingClaimSets.Add(sharingClaimSet);
            }

            return sharingClaimSets;
        }
    }

    public interface IClaimSetFileExportModel
    {
        string Title { get; set; }
        IList<int> SelectedForExport { get; }
    }
}
