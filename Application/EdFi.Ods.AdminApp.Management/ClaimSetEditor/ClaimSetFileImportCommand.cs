// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using EdFi.Ods.AdminApp.Management.Database.Queries;

namespace EdFi.Ods.AdminApp.Management.ClaimSetEditor
{
    public class ClaimSetFileImportCommand
    {
        private readonly AddClaimSetCommand _addClaimSetCommand;
        private readonly EditResourceOnClaimSetCommand _editResourceOnClaimSetCommand;
        private readonly GetResourceClaimsQuery _getResourceClaimsQuery;

        public ClaimSetFileImportCommand(AddClaimSetCommand addClaimSetCommand, EditResourceOnClaimSetCommand editResourceOnClaimSetCommand, GetResourceClaimsQuery getResourceClaimsQuery)
        {
            _addClaimSetCommand = addClaimSetCommand;
            _editResourceOnClaimSetCommand = editResourceOnClaimSetCommand;
            _getResourceClaimsQuery = getResourceClaimsQuery;
        }

        public void Execute(SharingModel model)
        {
            var sharingClaimSets = model.Template.ClaimSets;
            var allResources = GetDbResources();
            foreach (var claimSet in sharingClaimSets)
            {
                var claimSetId = _addClaimSetCommand.Execute(new AddClaimSetModel
                {
                    ClaimSetName = claimSet.Name
                });

                var resources = claimSet.ResourceClaims.Select(x => x.ToObject<ResourceClaim>()).ToList();
                var childResources = new List<ResourceClaim>();
                foreach (var resourceClaims in resources.Select(x => x.Children))
                    childResources.AddRange(resourceClaims);
                resources.AddRange(childResources);
                var currentResources = resources.Select(r =>
                    {
                        var resource = allResources.FirstOrDefault(dr => dr.Name.Equals(r.Name));
                        if (resource != null)
                        {
                            resource.Create = r.Create;
                            resource.Read = r.Read;
                            resource.Update = r.Update;
                            resource.Delete = r.Delete;
                        }
                        return resource;
                    }).ToList();
                currentResources.RemoveAll(x => x == null);
                foreach (var resource in currentResources)
                {
                    var editResourceModel = new EditResourceOnClaimSetModel
                    {
                        ClaimSetId = claimSetId,
                        ResourceClaim = resource 
                    };

                    _editResourceOnClaimSetCommand.Execute(editResourceModel);
                }
            }
        }

        private List<ResourceClaim> GetDbResources()
        {
            var allResources = new List<ResourceClaim>();
            var parentResources = _getResourceClaimsQuery.Execute().ToList();
            allResources.AddRange(parentResources);
            foreach (var children in parentResources.Select(x => x.Children))
            {
                allResources.AddRange(children);
            }

            return allResources;
        }
    }

    public class AddClaimSetModel: IAddClaimSetModel
    {
        public string ClaimSetName { get; set; }
    }

    public class EditResourceOnClaimSetModel : IEditResourceOnClaimSetModel
    {
        public int ClaimSetId { get; set; }
        public ResourceClaim ResourceClaim { get; set;  }
    }
}
