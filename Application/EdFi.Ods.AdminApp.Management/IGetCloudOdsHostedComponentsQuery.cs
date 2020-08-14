// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace EdFi.Ods.AdminApp.Management
{
    public interface IGetCloudOdsHostedComponentsQuery
    {
        Task<IEnumerable<CloudOdsWebsite>> Execute(ICloudOdsOperationContext context);
        Task<IEnumerable<OdsComponent>> Execute(CloudOdsInstance instance);
    }
}