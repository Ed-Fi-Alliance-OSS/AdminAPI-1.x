using System.Collections.Generic;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels.SchoolYears
{
    public class EditSchoolYearModel
    {
        public short? SchoolYear { get; set; }
        public IReadOnlyList<SelectListItem> SchoolYears { get; set; }
    }

    public class EditSchoolYearModelValidator : AbstractValidator<EditSchoolYearModel>
    {
        public EditSchoolYearModelValidator()
            => RuleFor(m => m.SchoolYear).NotEmpty();
    }
}
