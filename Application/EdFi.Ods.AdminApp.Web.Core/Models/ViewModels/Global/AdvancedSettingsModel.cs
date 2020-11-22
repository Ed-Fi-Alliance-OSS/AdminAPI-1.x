// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.Global
{
    public class AdvancedSettingsModel
    {
        [Display(Name = "Bearer Token Timeout in Minutes")]
        public int BearerTokenTimeoutInMinutes { get; set; }
    }

    public class AdvancedSettingsModelValidator : AbstractValidator<AdvancedSettingsModel>
    {
        public AdvancedSettingsModelValidator()
        {
            RuleFor(m => m.BearerTokenTimeoutInMinutes).GreaterThan(0);
        }
    }
}