// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using HtmlTags;
using HtmlTags.Conventions;
using HtmlTags.Conventions.Elements;
using System.Linq;
using System.Text.RegularExpressions;

namespace EdFi.Ods.AdminApp.Web.Infrastructure.Tags
{
    public class DefaultDisplayLabelBuilder : IElementBuilder
    {
        public bool Matches(ElementRequest subject)
        {
            return true;
        }

        public HtmlTag Build(ElementRequest request)
        {
            return new HtmlTag("strong").Text(BreakUpCamelCase(request.Accessor.Name) + ": ");
        }

        public static string BreakUpCamelCase(string fieldName)
        {
            var patterns = new[]
            {
                "([a-z])([A-Z])",
                "([0-9])([a-zA-Z])",
                "([a-zA-Z])([0-9])"
            };

            var output = patterns.Aggregate(fieldName,
                (current, pattern) => Regex.Replace(current, pattern, "$1 $2", RegexOptions.IgnorePatternWhitespace));
            return output.Replace('_', ' ');
        }
    }
}