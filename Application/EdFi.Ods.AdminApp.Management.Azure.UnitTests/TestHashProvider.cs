// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

#if NET48
using EdFi.Ods.Common.Security;
#else
using EdFi.Common.Security;
#endif

namespace EdFi.Ods.AdminApp.Management.Azure.UnitTests
{
    public class TestHashProvider : ISecurePackedHashProvider
    {
        public string ComputePackedHashString(string secret, int hashAlgorithm, int iterations, int saltSize)
        {
            return secret;
        }
    }

    public class TestHashConfigurationProvider : IHashConfigurationProvider
    {
        public HashConfiguration GetHashConfiguration()
        {
            return new HashConfiguration
            {
                Algorithm = "test",
                Iterations = 1,
                SaltSize = 128
            };
        }
    }
}
