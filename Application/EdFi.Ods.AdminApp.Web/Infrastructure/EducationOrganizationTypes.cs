// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Configuration;

namespace EdFi.Ods.AdminApp.Web.Infrastructure
{
    public class EducationOrganizationTypes
    {
        private static readonly Lazy<EducationOrganizationTypes>  _instance = new Lazy<EducationOrganizationTypes>(() => new EducationOrganizationTypes());

        protected EducationOrganizationTypes()
        {
        }

        public static EducationOrganizationTypes Instance = _instance.Value;
        public string LocalEducationAgency => ConfigurationManager.AppSettings["LocalEducationAgencyTypeValue"];
        public string SchoolType => ConfigurationManager.AppSettings["SchoolTypeValue"];
    }
}