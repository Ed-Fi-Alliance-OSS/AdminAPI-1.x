// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using HtmlTags;

namespace EdFi.Ods.AdminApp.Web.Helpers
{
    public static class HtmlTagExtensions
    {
        public static HtmlTag AppendSpinner(this HtmlTag htmlTag, string id, bool hidden = true)
        {
            htmlTag.Data("spinner-id", id);

            var spinner = new HtmlTag("i")
                .Id(id)
                .AddClasses("fa", "fa-spinner", "fa-pulse", "fa-fw", "inline-icon");

            if (hidden)
                spinner.AddClass("invisible");

            return htmlTag.After(spinner);
        }

        public static HtmlTag AddPlaceholder(this HtmlTag tag, string placeholder)
        {
            return tag.Attr("placeholder", placeholder);
        }

        public static HtmlTag AddPattern(this HtmlTag tag, string pattern)
        {
            var retVal = tag.Data("pattern", pattern);
            return retVal;
        }
    }
}