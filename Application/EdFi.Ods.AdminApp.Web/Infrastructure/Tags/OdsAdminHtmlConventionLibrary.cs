// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using HtmlTags.Conventions;

namespace EdFi.Ods.AdminApp.Web.Infrastructure.Tags
{
    public static class OdsAdminHtmlConventionLibrary
    {
        public static HtmlConventionLibrary CreateHtmlConventionLibrary()
        {
            var htmlConventionLibrary = new HtmlConventionLibrary();
            new OdsAdminHtmlTagConventions().Apply(htmlConventionLibrary);
            new DefaultHtmlConventions().Apply(htmlConventionLibrary);
            return htmlConventionLibrary;
        }
    }
}