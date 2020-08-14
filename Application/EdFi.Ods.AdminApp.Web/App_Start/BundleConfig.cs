// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Web;
using System.Web.Optimization;

namespace EdFi.Ods.AdminApp.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/bootstrap-multiselect.js"));

            bundles.Add(new ScriptBundle("~/bundles/toastr").Include(
                    "~/Scripts/toastr.min.js"));

            bundles.Add(new StyleBundle("~/Content/css/styles").Include("~/Content/css/bootstrap.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/css/bootstrap-multiselect.css")
                .Include("~/Content/css/font-awesome.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/css/toastr.min.css")
                .Include("~/Content/css/site.css", new CssRewriteUrlTransformWrapper()));

            bundles.Add(new ScriptBundle("~/bundles/scripts/lodash").Include("~/Scripts/lodash.js"));

            bundles.Add(new ScriptBundle("~/bundles/scripts/site").Include(
                    "~/Scripts/site.js",
                    "~/Scripts/site-form-handlers.js"));

            bundles.Add(new ScriptBundle("~/bundles/scripts/clipboard").Include(
                    "~/Scripts/clipboard.js",
                    "~/Scripts/clipboard-min.js"));

            bundles.Add(new ScriptBundle("~/bundles/scripts/signalr")
                .Include(
                "~/Scripts/jquery.signalR-2.2.1.js",
                "~/Scripts/signalr.js"));

            bundles.Add(new ScriptBundle("~/bundles/scripts/claimset").Include(
                "~/Scripts/resource-editor.js"));

            bundles.Add(new ScriptBundle("~/bundles/scripts/authstrategy").Include(
                "~/Scripts/auth-editor.js"));
        }
    }

    public class CssRewriteUrlTransformWrapper : IItemTransform
    {
        public string Process(string includedVirtualPath, string input)
        {
            return new CssRewriteUrlTransform().Process("~" + VirtualPathUtility.ToAbsolute(includedVirtualPath), input);
        }
    }
}
