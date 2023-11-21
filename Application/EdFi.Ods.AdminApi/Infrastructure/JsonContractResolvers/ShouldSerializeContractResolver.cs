// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Features.ClaimSets;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Reflection;

namespace EdFi.Ods.AdminApi.Infrastructure.JsonContractResolvers;

public class ShouldSerializeContractResolver : DefaultContractResolver
{
    private readonly IOdsSecurityModelVersionResolver _odsSecurityModelResolver;

    public ShouldSerializeContractResolver(IOdsSecurityModelVersionResolver resolver) : base()
    {
      _odsSecurityModelResolver = resolver;
    }
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty property = base.CreateProperty(member, memberSerialization);

        if (property.DeclaringType == typeof(ResourceClaimModel) && (property.PropertyName is not null && property.PropertyName.ToLowerInvariant() == "readchanges"))
        {
            // SFUQUA
            property.ShouldSerialize =
                instance =>
                {
                    var securityModel = _odsSecurityModelResolver.DetermineSecurityModel();
                    return securityModel is EdFiOdsSecurityModelCompatibility.Six or
                           EdFiOdsSecurityModelCompatibility.FiveThreeCqe;
                };
        }
        property.PropertyName = char.ToLowerInvariant(property.PropertyName![0]) + property.PropertyName[1..];
        return property;
    }
}
