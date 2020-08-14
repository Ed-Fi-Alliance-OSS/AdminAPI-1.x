// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApp.Management.Azure
{
    public class AzurePerformanceLevel
    {
        public AzurePerformanceLevel(string edition, string serviceObjective)
        {
            Edition = edition;
            ServiceObjective = serviceObjective;
        }

        public override bool Equals(object obj)
        {
            var rhs = obj as AzurePerformanceLevel;
            return rhs != null &&
                   string.Equals(rhs.Edition, Edition) &&
                   string.Equals(rhs.ServiceObjective, ServiceObjective);
        }

        public override int GetHashCode()
        {
            return Edition.GetHashCode() ^ ServiceObjective.GetHashCode();
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Edition) && !string.IsNullOrEmpty(ServiceObjective);
        }


        public string Edition { get;  }
        public string ServiceObjective { get; }

        public int ServiceOrdering
        {
            get
            {
                int ordering;
                return ServiceObjective == null || ServiceObjective.Length < 2
                    ? 0
                    : int.TryParse(ServiceObjective.Substring(1), out ordering) 
                        ? ordering 
                        : 0;
            }
        }

        public static bool operator <(AzurePerformanceLevel lhs, AzurePerformanceLevel rhs)
        {
            return lhs.PerformanceTier < rhs.PerformanceTier ||
                   (lhs.PerformanceTier == rhs.PerformanceTier && lhs.ServiceOrdering < rhs.ServiceOrdering);
        }

        public static bool operator >(AzurePerformanceLevel lhs, AzurePerformanceLevel rhs)
        {
            return lhs.PerformanceTier > rhs.PerformanceTier ||
                   (lhs.PerformanceTier == rhs.PerformanceTier && lhs.ServiceOrdering > rhs.ServiceOrdering);
        }

        public AzurePerformanceTier PerformanceTier
        {
            get
            {
                AzurePerformanceTier result;
                return AzurePerformanceTier.TryParse(Edition, out result) 
                    ? result 
                    : AzurePerformanceTier.Unknown;
            }
        }
    }
}