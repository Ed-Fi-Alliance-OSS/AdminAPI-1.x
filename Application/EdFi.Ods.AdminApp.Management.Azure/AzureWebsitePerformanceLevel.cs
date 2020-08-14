// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApp.Management.Azure
{
    public class AzureWebsitePerformanceLevel : AzurePerformanceLevel
    {
        public AzureWebsitePerformanceLevel(string edition, string serviceObjective) : base(edition, serviceObjective)
        {
        }

        public static readonly AzureWebsitePerformanceLevel Free = new AzureWebsitePerformanceLevel("Free", "F1");

        public static readonly AzureWebsitePerformanceLevel Shared = new AzureWebsitePerformanceLevel("Shared", "D1");

        public static readonly AzureWebsitePerformanceLevel B1 = new AzureWebsitePerformanceLevel("Basic", "B1");
        public static readonly AzureWebsitePerformanceLevel B2 = new AzureWebsitePerformanceLevel("Basic", "B2");
        public static readonly AzureWebsitePerformanceLevel B3 = new AzureWebsitePerformanceLevel("Basic", "B3");

        public static readonly AzureWebsitePerformanceLevel S1 = new AzureWebsitePerformanceLevel("Standard", "S1");
        public static readonly AzureWebsitePerformanceLevel S2 = new AzureWebsitePerformanceLevel("Standard", "S2");
        public static readonly AzureWebsitePerformanceLevel S3 = new AzureWebsitePerformanceLevel("Standard", "S3");

        public static readonly AzureWebsitePerformanceLevel P1 = new AzureWebsitePerformanceLevel("Premium", "P1");
        public static readonly AzureWebsitePerformanceLevel P2 = new AzureWebsitePerformanceLevel("Premium", "P2");
        public static readonly AzureWebsitePerformanceLevel P3 = new AzureWebsitePerformanceLevel("Premium", "P3");
    }
}