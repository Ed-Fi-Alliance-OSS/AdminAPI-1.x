// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApp.Management
{
    public interface ICloudOdsOperationContext
    {
        CloudOdsInstance Instance { get; }
        CloudOdsEnvironment TargetEnvironment { get; }
        CloudOdsRole TargetRole { get; }
    }
    public class CloudOdsOperationContext : ICloudOdsOperationContext
    {
        public CloudOdsInstance Instance { get; set; }
        public CloudOdsEnvironment TargetEnvironment { get; set; }
        public CloudOdsRole TargetRole { get; set; }
    }

    public class CloudOdsApiOperationContext : ICloudOdsOperationContext
    {
        public CloudOdsApiOperationContext()
        {
        }

        public CloudOdsApiOperationContext(CloudOdsInstance instance, CloudOdsEnvironment targetEnvironment)
        {
            Instance = instance;
            TargetEnvironment = targetEnvironment;
        }

        public CloudOdsInstance Instance { get; set; }
        public CloudOdsEnvironment TargetEnvironment { get; set; }
        public CloudOdsRole TargetRole => CloudOdsRole.Api;
    }
}