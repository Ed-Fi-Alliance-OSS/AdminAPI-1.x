// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Features.Actions;
using EdFi.Ods.AdminApi.Features.Applications;
using EdFi.Ods.AdminApi.Features.ClaimSets;
using EdFi.Ods.AdminApi.Features.ODSInstances;
using EdFi.Ods.AdminApi.Features.Profiles;
using EdFi.Ods.AdminApi.Features.Vendors;
using EdFi.Ods.AdminApi.Infrastructure.Extensions;

namespace EdFi.Ods.AdminApi.Features
{
    public class SortableList<T> : List<T>
    {
        public List<T> Sort(string orderBy, string direction = "asc")
        {
            var descending = SortingDirection.IsDescendingSorting(direction);
            var defaultSortColumn = string.Empty;

            if (typeof(ActionModel) == typeof(T))
                defaultSortColumn = FeatureConstants.ActionDefaultSorting;
            else if (typeof(ApplicationModel) == typeof(T))
                defaultSortColumn = FeatureConstants.ApplicationsDefaultSorting;
            else if (typeof(ClaimSetModel) == typeof(T))
                defaultSortColumn = FeatureConstants.ClaimsetDefaultSorting;
            else if (typeof(OdsInstanceModel) == typeof(T))
                defaultSortColumn = FeatureConstants.OdsInstanceDefaultSorting;
            else if (typeof(ProfileModel) == typeof(T))
                defaultSortColumn = FeatureConstants.ProfileDefaultSorting;
            else if (typeof(ResourceClaimModel) == typeof(T))
                defaultSortColumn = FeatureConstants.ProfileDefaultSorting;
            else if (typeof(VendorModel) == typeof(T))
                defaultSortColumn = FeatureConstants.VendorDefaultSorting;

            if (!string.IsNullOrEmpty(defaultSortColumn))
            {
                var queriable = this.AsQueryable<T>().OrderByColumn(orderBy, defaultSortColumn, descending);
                return queriable.ToList();
            }
            return this;
        }
    }
}
