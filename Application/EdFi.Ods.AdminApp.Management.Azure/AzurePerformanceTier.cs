// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApp.Management.Azure
{
    public class AzurePerformanceTier : Enumeration<AzurePerformanceTier>
    {
        private AzurePerformanceTier(int value, string displayName, string code) : base(value, displayName)
        {
            Code = code;
        }

        public static bool operator <(AzurePerformanceTier lhs, AzurePerformanceTier rhs)
        {
            return lhs.Value < rhs.Value;
        }

        public static bool operator >(AzurePerformanceTier lhs, AzurePerformanceTier rhs)
        {
            return lhs.Value > rhs.Value;
        }

        public string Code { get; }

        public static readonly AzurePerformanceTier Unknown = new AzurePerformanceTier(-1, "Unknown", "");
        public static readonly AzurePerformanceTier Free = new AzurePerformanceTier(0, "Free", "F");
        public static readonly AzurePerformanceTier Shared = new AzurePerformanceTier(1, "Shared", "D");
        public static readonly AzurePerformanceTier Basic = new AzurePerformanceTier(2, "Basic", "B");
        public static readonly AzurePerformanceTier Standard = new AzurePerformanceTier(3, "Standard", "S");
        public static readonly AzurePerformanceTier Premium = new AzurePerformanceTier(4, "Premium","P");
    }
}