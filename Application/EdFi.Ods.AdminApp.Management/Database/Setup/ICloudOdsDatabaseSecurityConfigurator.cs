// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;

namespace EdFi.Ods.AdminApp.Management.Database.Setup
{
    public interface ICloudOdsDatabaseSecurityConfigurator
    {
        void ApplyConfiguration(OdsSqlConfiguration sqlConfiguration, IEnumerable<CloudOdsDatabaseSecurityConfiguration> securityConfigurations);
        void ApplyConfiguration(OdsSqlConfiguration sqlConfiguration, CloudOdsDatabaseSecurityConfiguration securityConfiguration);
        void RemoveConfiguration(OdsSqlConfiguration sqlConfiguration, IEnumerable<CloudOdsDatabaseSecurityConfiguration> securityConfigurations);
        void RemoveConfiguration(OdsSqlConfiguration sqlConfiguration, CloudOdsDatabaseSecurityConfiguration securityConfiguration);

        void CreateServerLogins(OdsSqlConfiguration odsSqlConfiguration);
    }
}
