// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using HtmlTags;
using HtmlTags.Conventions;
using HtmlTags.Conventions.Elements;
using System;
using System.ComponentModel.DataAnnotations;
using EdFi.Ods.AdminApp.Web.Helpers;
using HtmlTags.Extended.Attributes;
using HtmlTags.Reflection;

namespace EdFi.Ods.AdminApp.Web.Infrastructure.Tags
{
    public class OdsAdminHtmlTagConventions : HtmlConventionRegistry
    {
        public OdsAdminHtmlTagConventions()
        {
            Editors.If(e => e.Accessor.PropertyType != typeof(bool) && e.Accessor.PropertyType != typeof(bool?))
                .AddClass("form-control");

            Editors.IfPropertyIs<bool>().BuildBy(BuildCheckBox);

            Editors.If(e => e.Accessor.PropertyType == typeof(DateTime) || e.Accessor.PropertyType == typeof(DateTime?))
                .ModifyWith(m => m.CurrentTag
                .AddPattern("9{1,2}/9{1,2}/9999")
                .AddPlaceholder("MM/DD/YYYY")
                .AddClass("datepicker")
                .Value(m.Value<DateTime?>() != null ? m.Value<DateTime>().ToShortDateString() : string.Empty));

            Editors.If(e => e.Accessor.GetAttribute<DataTypeAttribute>()?.DataType == DataType.Password)
                .ModifyWith(m => m.CurrentTag.PasswordMode());

            Editors.If(e => e.Accessor.PropertyType == typeof(decimal) || e.Accessor.PropertyType == typeof(decimal?))
                .ModifyWith(m => m.CurrentTag.Value(m.Value<decimal?>() != null ? m.Value<decimal>().ToString("F") : ""));

            Labels.Always.BuildBy<DefaultDisplayLabelBuilder>();
            Labels.Always.AddClass("control-label");
            Labels.ModifyForAttribute<DisplayAttribute>((t, a) => t.Text($"{a.Name}:"));
        }

        private static HtmlTag BuildCheckBox(ElementRequest request)
        {
            var namingConvention = new DotNotationElementNamingConvention();
            var name = namingConvention.GetName(request.HolderType(), request.Accessor);

            var checkboxTag = new CheckboxTag(request.Value<bool>())
                .Attr("value", "true")
                .Attr("name", name)
                .Attr("id", name);

            var mvcConventionHiddenInput = new HiddenTag()
                .Attr("value", "false")
                .Attr("name", name);

            checkboxTag.After(mvcConventionHiddenInput);
            return checkboxTag;
        }
    }
}
