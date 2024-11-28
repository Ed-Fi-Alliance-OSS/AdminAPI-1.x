// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Common.Features;
using EdFi.Ods.AdminApi.Common.Infrastructure.Helpers;
using System.Reflection;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Helpers;

public static class AdminConsoleFeatureHelper
{
    public static List<IFeature> GetFeatures() => FeatureHelper.GetFeatures(Assembly.GetExecutingAssembly());
}
