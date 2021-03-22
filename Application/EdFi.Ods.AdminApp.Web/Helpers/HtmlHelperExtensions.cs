// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Web.Infrastructure.Tags;
using HtmlTags;
using HtmlTags.Conventions;
using HtmlTags.Conventions.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EdFi.Ods.AdminApp.Web.Display.RadioButton;
using EdFi.Ods.AdminApp.Web.Display.TabEnumeration;
using EdFi.Ods.AdminApp.Web.Infrastructure;
using EdFi.Ods.AdminApp.Web.Infrastructure.IO;
using HtmlTags.Reflection;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Property = EdFi.Ods.AdminApp.Web.Infrastructure.Property;
using Preconditions = EdFi.Common.Preconditions;
using EdFi.Ods.AdminApp.Management.Api;
using EdFi.Ods.AdminApp.Web.Display.Pagination;
using log4net;
using Microsoft.AspNetCore.WebUtilities;

namespace EdFi.Ods.AdminApp.Web.Helpers
{
    public static class HtmlHelperExtensions
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(HtmlHelperExtensions));

        private static readonly HtmlConventionLibrary HtmlConventionLibrary = OdsAdminHtmlConventionLibrary.CreateHtmlConventionLibrary();

        private static IElementGenerator<T> GetGenerator<T>(T model) where T : class
        {
            return ElementGenerator<T>.For(HtmlConventionLibrary, null, model);
        }

        public static HtmlTag Input<T>(this IHtmlHelper<T> helper, Expression<Func<T, object>> expression) where T : class
        {
            var generator = GetGenerator(helper.ViewData.Model);
            return generator.InputFor(expression);
        }

        public static HtmlTag FileInputBlock<T>(this IHtmlHelper<T> helper, Expression<Func<T, object>> expression) where T : class
        {
            var accept = Property.From(expression).GetCustomAttributes<AcceptAttribute>().SingleOrDefault();

            Action<HtmlTag> action = input =>
            {
                input.Attr("type", "file");
                input.AddClass("form-control");
                if (accept != null)
                    input.Attr("accept", accept.FileTypeSpecifier);
            }; 

            return helper.InputBlock(expression, null, null, action);
        }

        public static HtmlTag Label<T>(this IHtmlHelper<T> helper, Expression<Func<T, object>> expression) where T : class
        {
            var generator = GetGenerator(helper.ViewData.Model);
            return generator.LabelFor(expression);
        }

        public static HtmlTag ToolTip<T>(this IHtmlHelper<T> helper, string helpTooltipText)
        {
            var helpTooltip = new HtmlTag("span");
            if (!string.IsNullOrEmpty(helpTooltipText))
            {
                helpTooltip
                    .Attr("title", helpTooltipText)
                    .Data("toggle", "tooltip")
                    .AddClasses("fa", "fa-question-circle-o", "form-icons");
            }
            helpTooltip = helpTooltip.WrapWith(new HtmlTag("span").AddClasses("text-left", "help-form"));

            return helpTooltip;
        }

        private static HtmlTag FormBlock(HtmlTag label, HtmlTag input, HtmlTag toolTip)
        {
            var formRow = new DivTag().AddClasses("row", "form-group");
            formRow.Append(label);
            formRow.Append(input);
            formRow.Append(toolTip);

            var wrapper = new DivTag().AddClass("form-horizontal");
            wrapper.Append(formRow);

            return wrapper;
        }

        public static HtmlTag NumberOnlyInputBlock<T>(this IHtmlHelper<T> helper, Expression<Func<T, object>> expression, string placeholderText = null, string helpTooltipText = null, string customLabelText = null, int maxValue = int.MaxValue, int minValue=0) where T : class
        {
            void Action(HtmlTag input)
            {
                input.Attr("type", "number");
                input.Attr("max", maxValue);
                input.Attr("min", minValue);
                input.AddClass("form-control");
            }

            return helper.InputBlock(expression, placeholderText, helpTooltipText, Action, customLabelText);
        }

        public static HtmlTag InputBlock<T>(this IHtmlHelper<T> helper, Expression<Func<T, object>> expression, string placeholderText = null, string helpTooltipText = null, Action<HtmlTag> inputModifier = null, string customLabelText = null) where T : class
        {
            Preconditions.ThrowIfNull(helper, nameof(helper));
            Preconditions.ThrowIfNull(expression, nameof(expression));

            var label = helper.Label(expression);
            if (customLabelText != null)
            {
                label.Text(customLabelText);
            }
            label = label.WrapWith(new DivTag().AddClasses("col-xs-4", "text-right"));

            var input = helper.Input(expression);
            if (!string.IsNullOrEmpty(placeholderText))
            {
                input.AddPlaceholder(placeholderText);
            }
            inputModifier?.Invoke(input);

            var isCheckbox = expression?.ToAccessor()?.PropertyType == typeof(bool);

            input = input.WrapWith(new DivTag().AddClasses("col-xs-6", isCheckbox ? "text-left" : "text-center"));
            

            var helpTooltip = helper.ToolTip(helpTooltipText);
            helpTooltip = helpTooltip.AddClasses("col-xs-2");

            return FormBlock(label, input, helpTooltip);
        }

        public static HtmlTag SelectList<T, TR>(this IHtmlHelper<T> helper, Expression<Func<T, TR>> expression, bool includeBlankOption = false)
            where T : class
            where TR : Enumeration<TR>
        {
            var getAllMethod = typeof (TR).GetMethod("GetAll", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            var enumerationValues = (IEnumerable<TR>)getAllMethod.Invoke(null, null);

            var model = helper.ViewData.Model;
            var expressionValue = expression.Compile().Invoke(model);
            var convertedExpression = expression.Cast<T, TR, object>();

            return helper.SelectList(convertedExpression, enumerationValues, i => new SelectListItem { Text = i.DisplayName, Value = i.Value.ToString(), Selected = i == expressionValue}, includeBlankOption);
        }

        public static HtmlTag SelectList<T>(this IHtmlHelper<T> helper, int? bulkFileType, string fieldName, bool includeBlankOption = false)
            where T : class
        {
            var getAllMethod = typeof(InterchangeFileType).GetMethod("GetAll", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            var enumerationValues = (IEnumerable<InterchangeFileType>)getAllMethod.Invoke(null, null);

            var input = new HtmlTag("select").Attr("name", fieldName).Attr("id", fieldName).RemoveAttr("value");
        
            if (includeBlankOption)
            {
                var blankSelectListItem = new SelectListItem
                {
                    Text = "",
                    Value = ""
                };
        
                AppendSelectListOption(blankSelectListItem, input);
            }
            
            InterchangeFileType bulkFileUploadType = null;
            if (bulkFileType != null)
                bulkFileUploadType = InterchangeFileType.FromInt32(bulkFileType.Value);
        
            var selectListItems = enumerationValues.Select(
                i => new SelectListItem
                    {Text = i.DisplayName, Value = i.Value.ToString(), Selected = i == bulkFileUploadType}); 
        
            foreach (var selectListItem in selectListItems)
            {
                AppendSelectListOption(selectListItem, input);
            }
        
            return input;
        }

        public static HtmlTag SelectList<T, TR>(this IHtmlHelper<T> helper, Expression<Func<T, object>> expression, IEnumerable<TR> options, Func<TR, SelectListItem> selectListItemBuilder, bool includeBlankOption = false) where T : class
        {
            var input = helper.Input(expression).TagName("select").RemoveAttr("value");

            if (includeBlankOption)
            {
                var blankSelectListItem = new SelectListItem
                {
                    Text = "",
                    Value = ""
                };

                AppendSelectListOption(blankSelectListItem, input);
            }

            foreach (var selectListItem in options.Select(selectListItemBuilder))
            {
                AppendSelectListOption(selectListItem, input);
            }

            return input;
        }

        private static void AppendSelectListOption(SelectListItem selectListItem, HtmlTag selectTag)
        {
            var optionTag = new HtmlTag("option").Attr("value", selectListItem.Value).Text(selectListItem.Text);
            if (selectListItem.Selected)
            {
                optionTag.Attr("selected", "selected");
            }

            selectTag.Append(optionTag);
        }

        public static HtmlTag SelectListBlock<T>(this IHtmlHelper<T> helper,
            Expression<Func<T, object>> expression, IReadOnlyList<SelectListItem> options)
            where T : class
        {
            var model = helper.ViewData.Model;
            var value = expression.Compile()(model);
            var valueLiteral = value?.ToString();

            return helper.SelectListBlock(
                expression, options, x => new SelectListItem
                {
                    Disabled = x.Disabled,
                    Group = x.Group,
                    Selected = x.Selected || valueLiteral == x.Value,
                    Text = x.Text,
                    Value = x.Value
                });
        }

        public static HtmlTag SelectListBlock<T, TR>(this IHtmlHelper<T> helper, Expression<Func<T, object>> expression, IEnumerable<TR> options, Func<TR, SelectListItem> selectListItemBuilder, string helpTooltipText = null, bool includeBlankOption = false) where T: class
        {
            var selectList = SelectList(helper, expression, options, selectListItemBuilder, includeBlankOption);
            return helper.SelectListBlock(expression, selectList, helpTooltipText, includeBlankOption);
        }

        public static HtmlTag SelectListBlock<T, TR>(this IHtmlHelper<T> helper, Expression<Func<T, TR>> expression, string helpTooltipText = null, bool includeBlankOption = false) 
            where T : class
            where TR: Enumeration<TR>
        {
            var selectList = SelectList(helper, expression, includeBlankOption);
            var convertedExpression = expression.Cast<T, TR, object>();

            return helper.SelectListBlock(convertedExpression, selectList, helpTooltipText, includeBlankOption);
        }

        public static HtmlTag SelectListBlock<T>(this IHtmlHelper<T> helper, Expression<Func<T, object>> expression, HtmlTag selectList, string helpTooltipText = null, bool includeBlankOption = false)
            where T : class
        {
            var label = helper.Label(expression);
            label = label.WrapWith(new DivTag().AddClasses("col-xs-4", "text-right"));

            var input = selectList;
            input = input.WrapWith(new DivTag().AddClasses("col-xs-6", "text-left"));

            var helpTooltip = helper.ToolTip(helpTooltipText);
            helpTooltip = helpTooltip.AddClasses("col-xs-2");

            return FormBlock(label, input, helpTooltip);
        }

        public static HtmlTag MultiSelectList<T, TR>(this IHtmlHelper<T> helper, Expression<Func<T, object>> expression,
            IEnumerable<TR> options,
            Func<TR, SelectListItem> selectListItemBuilder) where T : class
        {
            var input = helper.SelectList(expression, options, selectListItemBuilder);
            input.Attr("multiple", "multiple");

            return input;
        }

        public static HtmlTag MultiSelectListBlock<T, TR>(this IHtmlHelper<T> helper,
            Expression<Func<T, object>> expression,
            IEnumerable<TR> options, 
            Func<TR, SelectListItem> selectListItemBuilder, 
            string helpTooltipText = null
        )
            where T : class
        {
            var label = helper.Label(expression);
            label = label.WrapWith(new DivTag().AddClasses("col-xs-4", "text-right"));

            var input = helper.MultiSelectList(expression, options, selectListItemBuilder);
            input = input.WrapWith(new DivTag().AddClasses("col-xs-6", "text-left"));

            var helpTooltip = helper.ToolTip(helpTooltipText);
            helpTooltip = helpTooltip.AddClasses("col-xs-2");

            return FormBlock(label, input, helpTooltip);
        }

        public static HtmlTag ModalFormButtons<T>(this IHtmlHelper<T> helper, string confirmButtonText = "Save Changes", string updateTargetId = "")
        {
            var cancelButton = helper.CancelModalButton();
            var saveButton = helper.SaveModalButton(confirmButtonText, updateTargetId);

            return cancelButton.After(saveButton);
        }

        public static HtmlTag Button<T>(this IHtmlHelper<T> helper, string buttonText)
        {
            var button = new HtmlTag("button")
                .Text(buttonText)
                .AddClasses("btn", "btn-primary", "cta");

            return button;
        }

        public static HtmlTag SaveButton<T>(this IHtmlHelper<T> helper, string buttonText = "Save Changes", string updateTargetId = "")
        {
            var saveButton = helper.Button(buttonText)
                .Attr("type", "submit");

            if (!string.IsNullOrEmpty(updateTargetId))
            {
                saveButton = saveButton.Data("update-target-id", updateTargetId);
            }

            return saveButton;
        }

        public static HtmlTag SaveModalButton<T>(this IHtmlHelper<T> helper, string buttonText = "Save Changes", string updateTargetId = "")
        {
            return helper.SaveButton(buttonText, updateTargetId).Data("confirm", "true");
        }

        public static HtmlTag CancelButton<T>(this IHtmlHelper<T> helper, string buttonText = "Cancel")
        {
            return new HtmlTag("button")
                .Text(buttonText)
                .AddClasses("btn", "btn-default");
        }

        public static HtmlTag CancelModalButton<T>(this IHtmlHelper<T> helper, string buttonText = "Cancel")
        {
            return helper.CancelButton(buttonText).Data("dismiss", "modal");
        }

        public static HtmlTag ValidationBlock<T>(this IHtmlHelper<T> helper)
        {
            return new DivTag().AddClasses("validationSummary", "alert", "alert-danger", "hidden");
        }

        public static HtmlTag NavTabs<T>(this IHtmlHelper helper, IUrlHelper urlHelper, List<TabDisplay<T>> tabs, object commonRouteValues = null) where T: Enumeration<T>, ITabEnumeration
        {
            var tabTag = new HtmlTag("ul").AddClasses("nav", "nav-tabs");
            BuildNavEntries(urlHelper, tabs, tabTag, commonRouteValues);
            return tabTag;
        }

        public static HtmlTag NavPills<T>(this IHtmlHelper helper, IUrlHelper urlHelper, List<TabDisplay<T>> tabs, object commonRouteValues = null) where T : Enumeration<T>, ITabEnumeration
        {
            var tabTag = new HtmlTag("ul").AddClasses("nav", "nav-pills", "nav-pills-custom");
            BuildNavEntries(urlHelper, tabs, tabTag, commonRouteValues);
            return tabTag;
        }

        private static void BuildNavEntries<T>(IUrlHelper urlHelper, List<TabDisplay<T>> tabs, HtmlTag tabTag, object commonRouteValues) where T : Enumeration<T>, ITabEnumeration
        {
            foreach (var tab in tabs.OrderBy(a => a.Tab.Value))
            {
                var listItem = new HtmlTag("li");
                if (!tab.IsEnabled)
                {
                    listItem.AddClass("disabled");
                }

                if (!tab.IsVisible)
                {
                    listItem.AddClass("hidden");
                }

                if (tab.IsSelected)
                {
                    listItem.AddClass("active");
                    listItem.Append(new HtmlTag("a").Attr("href", "#").Text(tab.Tab.DisplayName));
                }

                else
                {
                    var url = urlHelper.Action(tab.Tab.ActionName, tab.Tab.ControllerName, commonRouteValues);
                    listItem.Append(new HtmlTag("a").Attr("href", url).Text(tab.Tab.DisplayName));
                }

                tabTag.Append(listItem);
            }
        }
        
        public static HtmlTag InlineRadioButton<T, TEnumeration>(this IHtmlHelper<T> helper, Expression<Func<T, object>> expression, TEnumeration option, string helpTooltipText = null, string id = null, bool enabled = true) 
            where T: class
            where TEnumeration: Enumeration<TEnumeration> 
        {
            var model = helper.ViewData.Model;
            var value = model == null ? default(TEnumeration) : expression.Compile()(model);

            if (id == null)
            {
                id = Guid.NewGuid().ToString();
            }

            var input = helper.Input(expression)
                .Attr("type", "radio")
                .Attr("value", option.Value)
                .AddClass("radio-inline")
                .Id(id);

            if (option == value)
            {
                input.Attr("checked", "checked");
            }

            if (!enabled)
            {
                input.Attr("disabled", "true");
            }

            var label = new HtmlTag("label")
                .AddClass("radio-inline-label")
                .Text(option.DisplayName)
                .Attr("for", id);

            input.Append(label);

            if (!string.IsNullOrWhiteSpace(helpTooltipText))
            {
                var helpTooltip = helper.ToolTip(helpTooltipText);
                input.Append(helpTooltip);
            }

            var inputContainer = new HtmlTag("span")
                .AddClass("radio-inline-container");

            return input.WrapWith(inputContainer);
        }

        public static HtmlTag InlineCustomRadioButton<T, TEnumeration>(this IHtmlHelper<T> helper, Expression<Func<T, object>> expression, RadioButtonDisplay<TEnumeration> option, string id = null)
            where T : class 
            where TEnumeration : Enumeration<TEnumeration>, IRadioButton
        {
            var model = helper.ViewData.Model;
            var value = model == null ? default(TEnumeration) : expression.Compile()(model);

            if (id == null)
            {
                id = Guid.NewGuid().ToString();
            }

            var input = helper.Input(expression)
                .Attr("type", "radio")
                .Attr("value", option.RadioButton.Value)
                .AddClass("radio-inline")
                .Id(id);

            if (option == value)
            {
                input.Attr("checked", "checked");
            }

            if (!option.IsEnabled)
            {
                input.Attr("disabled", "true");
            }

            var label = new HtmlTag("label")
                .AddClass("radio-inline-label")
                .Text(option.RadioButton.DisplayName)
                .Attr("for", id);

            input.Append(label);

            if (!string.IsNullOrWhiteSpace(option.RadioButton.HelpTooltip))
            {
                var helpTooltip = helper.ToolTip(option.RadioButton.HelpTooltip);
                input.Append(helpTooltip);
            }

            var inputContainer = new HtmlTag("span")
                .AddClass("radio-inline-container");

            return input.WrapWith(inputContainer);
        }

        public static HtmlTag ActionAjax(this IHtmlHelper helper, string url, int minHeight, string placeholderText)
        {
            var placeholderTag = new DivTag();
            placeholderTag.Append(new HtmlTag("h6").Text(placeholderText));
            var spinnerTag = new DivTag();
            spinnerTag.Append(new HtmlTag("i").AddClasses("fa", "fa-spinner", "fa-pulse", "fa-fw"));

            var errorMessage = "Restarting the ODS / API is known to solve issues with first time setup or previously cached information, and may help resolve this issue on its own." +
                               " Please try restarting the ODS / API now and reload this to see if this same error occurs." +
                               " If the error persists, please check the application logs and then feel to schedule a ticket via <a href='https://tracker.ed-fi.org/projects/EDFI/issues'>Ed-Fi Tracker</a>" +
                               " or visit <a href='https://techdocs.ed-fi.org/display/ADMIN'>Admin App documentation</a> for more information.";

            var contentLoadingArea = new DivTag().Data("source-url", url).Data("error-message", errorMessage).AddClass("load-action-async");
            if (minHeight > 0)
            {
                //adding a minimum height is optional, but can help prevent the page scrollbar from jumping around while content loads
                contentLoadingArea.Attr("style", $"min-height: {minHeight}px"); 
            }

            contentLoadingArea.Append(placeholderTag);
            contentLoadingArea.Append(spinnerTag);
            return contentLoadingArea;
        }

        public static HtmlTag AjaxPostButton<T>(this IHtmlHelper<T> helper, string url, string buttonText)
        {
            var ajaxPostLink = new HtmlTag("a", tag =>
            {
                tag.AddClasses("btn", "btn-primary", "cta", "ajax-button");
                tag.Attr("href", url);
                tag.Text(buttonText);
            });

            return ajaxPostLink;
        }

        public static HtmlString ApplicationVersion(this IHtmlHelper helper)
        {            
            var informationVersion = InMemoryCache.Instance
                .GetOrSet("informationVersion",
                    () => Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion);

            return !string.IsNullOrEmpty(informationVersion) ? new HtmlString($"<span>Admin App Version: {informationVersion}</span>") : new HtmlString("");
        }

        public static HtmlTag CheckBoxSquare<T>(this IHtmlHelper<T> helper, bool expression, string action) where T : class
        {
            var label = new HtmlTag("label");
            var input = new HtmlTag("input").Attr("type", "checkbox").Attr("disabled","disabled").Attr("checked", true).AddClasses("hide", $"{action}-checkbox");
            const string icon = "<i class='fa fa-fw fa-check-square'></i>";
            if (expression)
                label.Append(input).AppendHtml(icon);
            return label;
        }

        public static HtmlString OdsApiVersion(this IHtmlHelper helper)
        {
            try
            {
                var odsApiVersion = InMemoryCache.Instance
                    .GetOrSet("OdsApiVersion", () => new InferOdsApiVersion().Version(CloudOdsAdminAppSettings.Instance.ProductionApiUrl));

                return !string.IsNullOrEmpty(odsApiVersion.ToString()) ? new HtmlString($"<span>ODS/API Version: {odsApiVersion}</span>") : new HtmlString("");
            }
            catch (Exception exception)
            {
                _logger.Error("Failed to infer ODS / API version. This can happen when the ODS / API is unreachable.", exception);

                return new HtmlString("");
            }
        }

        public static HtmlString PagingControl<TModel, T>(this IHtmlHelper<TModel> helper, string url, PagedList<T> pagedContent)
        {
            var pageNumber = pagedContent.PageNumber;
            var previousUrl = QueryHelpers.AddQueryString(url, "pageNumber", (pageNumber - 1).ToString());
            var nextUrl = QueryHelpers.AddQueryString(url, "pageNumber", (pageNumber + 1).ToString());

            var previousLink = PreviousButton(previousUrl, pagedContent);
            var nextLink = NextButton(nextUrl, pagedContent);
            var pageNumberItem = PageNumber(pagedContent, previousLink, nextLink);

            var contentWrapper = new HtmlTag("ul");
            contentWrapper.AddClass("pagination");

            if (previousLink != null) contentWrapper.Append(previousLink);
            if (pageNumberItem != null) contentWrapper.Append(pageNumberItem);
            if (nextLink != null) contentWrapper.Append(nextLink);

            var paginationNav = new HtmlTag("nav");
            paginationNav.Append(contentWrapper);
            paginationNav.Attr("aria-label", "Page navigation");

            return new HtmlString(paginationNav.ToString());
        }

        private static HtmlTag PageNumber<T>(PagedList<T> pagedContent, HtmlTag previousLink, HtmlTag nextLink)
        {
            if (previousLink == null && nextLink == null)
                return null;

            var pageNumber = new HtmlTag("span");

            pageNumber.Text(" " + pagedContent.PageNumber + " ");

            var listItem = new HtmlTag("li");
            listItem.Append(pageNumber);

            return listItem;
        }

        private static HtmlTag NextButton<T>(string nextUrl, PagedList<T> pagedContent)
        {
            if (!pagedContent.NextPageHasResults)
                return null;

            var nextLink = new HtmlTag("a");

            nextLink.Attr("href", nextUrl);
            nextLink.Attr("aria-label", "Next");
            nextLink.AddClass("navigate-next-page");

            var symbolSpan = new HtmlTag("span");
            symbolSpan.Attr("aria-hidden", "true");
            symbolSpan.AppendHtml("&raquo;");

            nextLink.Append(symbolSpan);

            var listItem = new HtmlTag("li");
            listItem.Append(nextLink);

            return listItem;
        }

        private static HtmlTag PreviousButton<T>(string previousUrl, PagedList<T> pagedContent)
        {
            if (pagedContent.PageNumber <= 1)
                return null;

            var previousLink = new HtmlTag("a");

            previousLink.Attr("href", previousUrl);
            previousLink.Attr("aria-label", "Previous");
            previousLink.AddClass("navigate-previous-page");

            var symbolSpan = new HtmlTag("span");
            symbolSpan.Attr("aria-hidden", "true");
            symbolSpan.AppendHtml("&laquo;");

            previousLink.Append(symbolSpan);

            var listItem = new HtmlTag("li");
            listItem.Append(previousLink);

            return listItem;
        }
    }
}
