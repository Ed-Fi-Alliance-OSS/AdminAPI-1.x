// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Threading.Tasks;
using EdFi.Common.Utils.Extensions;
using FluentValidation;
using FluentValidation.Results;
using log4net;

namespace EdFi.Ods.AdminApp.Web.Infrastructure
{
    public static class ValidationExtensions
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ValidationExtensions));

        public static IRuleBuilderOptionsConditions<T, TProperty> SafeCustom<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, Action<TProperty, ValidationContext<T>> action)
        {
            return ruleBuilder.Custom((command, context) =>
            {
                try
                {
                    action(command, context);
                }
                catch (Exception exception)
                {
                    const string errorMsg = "A validation rule encountered an unexpected error. Check the Application Log for troubleshooting information.";
                    context.AddFailure(errorMsg);
                    _logger.Error(errorMsg, exception);
                }
            });
        }

        public static IRuleBuilderOptionsConditions<T, TProperty> SafeCustomAsync<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, Func<TProperty, ValidationContext<T>, Task> action)
        {
            return ruleBuilder.CustomAsync( async (command, context, cancellationToken) =>
            {
                try
                {
                    await action(command, context);
                }
                catch (Exception exception)
                {
                    const string errorMsg = "A validation rule encountered an unexpected error. Check the Application Log for troubleshooting information.";
                    context.AddFailure(errorMsg);
                    _logger.Error(errorMsg, exception);
                }
            });
        }

        public static void AddFailures<T>(this ValidationContext<T> context, ValidationResult result)
        {
            result.Errors.Select(x => x.ErrorMessage).ForEach(context.AddFailure);
        }
    }
}
