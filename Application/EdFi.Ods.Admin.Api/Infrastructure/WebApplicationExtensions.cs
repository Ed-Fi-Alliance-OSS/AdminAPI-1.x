// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Reflection;
using EdFi.Ods.Admin.Api.Features;

namespace EdFi.Ods.Admin.Api.Infrastructure;

public static class WebApplicationExtensions
{
    public static void MapFeatureEndpoints(this WebApplication application)
    {
        var featureInterface = typeof(IFeature);
        var featureImpls = Assembly.GetExecutingAssembly().GetTypes()
            .Where(p => featureInterface.IsAssignableFrom(p) && p.IsClass);

        var features = new List<IFeature>();

        foreach (var featureImpl in featureImpls)
        {
            if(Activator.CreateInstance(featureImpl) is IFeature feature)
                features.Add(feature);
        }

        application.UseEndpoints(endpoints => {
            foreach (var routeBuilder in features)
            {
                routeBuilder.MapEndpoints(endpoints);
            }
        });
    }
}
