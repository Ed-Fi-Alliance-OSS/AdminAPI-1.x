// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using FluentValidation;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.ClaimSets
{
    public class ClaimSetFileExportModel: IClaimSetFileExportModel
    {
        public string Title { get; set; }
        public IEnumerable<ClaimSet> ClaimSets { get; set; }
        public IList<int> SelectedForExport { get; set; }

        public class ClaimSetFileExportModelValidator : AbstractValidator<ClaimSetFileExportModel>
        { 
            public ClaimSetFileExportModelValidator()
            {
                RuleFor(m => m.SelectedForExport).NotEmpty().WithMessage("You must select at least one claimset to proceed.");
                RuleFor(m => m.Title).NotEmpty();
            }
        }
    }
}