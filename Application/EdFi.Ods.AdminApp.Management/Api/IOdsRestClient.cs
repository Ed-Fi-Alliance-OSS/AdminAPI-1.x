// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;

namespace EdFi.Ods.AdminApp.Management.Api
{
    public interface IOdsRestClient
    {
        IReadOnlyList<T> GetAll<T>(string elementPath) where T : class;
        IReadOnlyList<T> GetAll<T>(string elementPath, int offset, int limit) where T : class;
        T GetById<T>(string elementPath, string id) where T : class;
        OdsApiResult PostResource<T>(T resource, string elementPath, bool refreshToken = false);
        OdsApiResult PutResource<T>(T resource, string elementPath, string id, bool refreshToken = false);
        IReadOnlyList<string> GetAllDescriptors();
        OdsApiResult DeleteResource(string elementPath, string id, bool refreshToken = false);
    }
}