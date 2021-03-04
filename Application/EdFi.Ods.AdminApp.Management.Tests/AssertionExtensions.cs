// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using EdFi.Ods.AdminApp.Management.Database.Ods;
using FluentValidation;
using FluentValidation.Results;
using Shouldly;
using static System.Environment;

namespace EdFi.Ods.AdminApp.Management.Tests
{
    public static class AssertionExtensions
    {
        public static void ShouldValidate<TModel>(this AbstractValidator<TModel> validator, TModel model)
            => validator.Validate(model).ShouldBeSuccessful();

        public static void ShouldNotValidate<TModel>(this AbstractValidator<TModel> validator, TModel model, params string[] expectedErrors)
            => validator.Validate(model).ShouldBeFailure(expectedErrors);

        private static void ShouldBeSuccessful(this ValidationResult result)
        {
            var indentedErrorMessages = result
                .Errors
                .OrderBy(x => x.ErrorMessage)
                .Select(x => "    " + x.ErrorMessage)
                .ToArray();

            var actual = String.Join(NewLine, indentedErrorMessages);

            result.IsValid.ShouldBeTrue($"Expected no validation errors, but found {result.Errors.Count}:{NewLine}{actual}");
        }

        private static void ShouldBeFailure(this ValidationResult result, params string[] expectedErrors)
        {
            result.IsValid.ShouldBeFalse("Expected validation errors, but the message passed validation.");

            result.Errors
                .OrderBy(x => x.ErrorMessage)
                .Select(x => x.ErrorMessage)
                .ToArray()
                .ShouldBe(expectedErrors.OrderBy(x => x).ToArray());
        }

        public static void ShouldBeSchoolYear(this SchoolYearType actual, short expectedSchoolYear, bool isCurrent = false)
        {
            actual.SchoolYear.ShouldBe(expectedSchoolYear);
            actual.SchoolYearDescription.ShouldBe((expectedSchoolYear - 1) + "-" + expectedSchoolYear);
            actual.CurrentSchoolYear.ShouldBe(isCurrent);
        }
    }
}
