// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.Global
{
    public class VendorsListModel
    {
        public List<VendorOverviewModel> Vendors { get; set; }

        public VendorsListModel()
        {
            Vendors = new List<VendorOverviewModel>();
        }
    }
}