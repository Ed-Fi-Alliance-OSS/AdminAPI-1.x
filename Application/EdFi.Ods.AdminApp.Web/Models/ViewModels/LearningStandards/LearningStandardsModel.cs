// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.LearningStandards
{
    public class LearningStandardsModel
    {
        [Display(Name = "AB Connect ID")]
        public string ApiKey { get; set; }
        [Display(Name = "AB Connect Key")]
        public string ApiSecret { get; set; }
        public bool HasApiData { get; set; }
        public bool IsJobRunning { get; set; }
        public bool IsSameOdsInstance { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public bool SynchronizationWasSuccessful { get; set; }
    }

    public class LearningStandardsModelValidator : AbstractValidator<LearningStandardsModel>
    {
        public LearningStandardsModelValidator()
        {
            RuleFor(m => m.ApiKey).NotEmpty();
            RuleFor(m => m.ApiSecret).NotEmpty();
        }
    }
}
