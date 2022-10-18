// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Database.Ods;
using EdFi.Ods.AdminApp.Management.Instances;

namespace EdFi.Ods.AdminApp.Management.OdsInstanceServices
{
    public interface IDatabaseValidationService
    {
        bool IsValidDatabase(int odsInstanceNumericSuffix, ApiMode mode);
    }

    public class DatabaseValidationService : IDatabaseValidationService
    {
        private readonly IDatabaseConnectionProvider _connectionProvider;

        public DatabaseValidationService(IDatabaseConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public bool IsValidDatabase(int odsInstanceNumericSuffix, ApiMode mode)
        {
            try
            {
                using (var odsInstanceConnection = _connectionProvider.CreateNewConnection(odsInstanceNumericSuffix, mode))
                {
                    odsInstanceConnection.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
