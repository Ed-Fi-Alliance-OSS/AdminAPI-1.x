// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Admin.DataAccess.Models;
#if NET48
using EdFi.Ods.Common.Security;
#else
using EdFi.Common.Security;
#endif

namespace EdFi.Ods.AdminApp.Management.Database
{
    public static class ApiClientExtensions
    {
        /// <summary>
        /// Generates a new Secret for the given ApiClient object; the new Secret is hashed, and the pre-hash plain text is returned
        /// </summary>
        /// <param name="apiClient">ApiClient entity for which a new secure secret should be generated</param>
        /// <param name="securePackedHashProvider">Hash provider service</param>
        /// <param name="hashConfigurationProvider">Hash configuration settings provider</param>
        /// <returns>Plain text value of Secret, before hasing algorithm has been applied</returns>
        public static string GenerateSecureClientSecret(this ApiClient apiClient, ISecurePackedHashProvider securePackedHashProvider, IHashConfigurationProvider hashConfigurationProvider)
        {
            var hashConfiguration = hashConfigurationProvider.GetHashConfiguration();
            var plainTextSecret = apiClient.GenerateSecret();

            var hashedSecret = securePackedHashProvider.ComputePackedHashString(
                plainTextSecret,
                hashConfiguration.GetAlgorithmHashCode(),
                hashConfiguration.Iterations,
                hashConfiguration.GetSaltSizeInBytes());

            apiClient.Secret = hashedSecret;
            apiClient.SecretIsHashed = true;

            return plainTextSecret;
        }
    }
}
