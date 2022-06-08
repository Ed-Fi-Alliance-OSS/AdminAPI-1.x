// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.Admin.Api.Features.Vendors;

public class VendorModel
{
    public int? VendorId { get; set; }
    public string? Company { get; set; }
    public string? NamespacePrefixes { get; set; }
    public string? ContactName { get; set; }
    public string? ContactEmailAddress { get; set; }
}
