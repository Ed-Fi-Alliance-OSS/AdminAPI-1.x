// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Management.Configuration.Application;
using EdFi.Ods.AdminApp.Management.Database.Models;
using NUnit.Framework;
using Shouldly;

namespace EdFi.Ods.AdminApp.Management.Tests.Configuration.Configuration
{
    public class EditConfigurationTests : AdminAppDataTestBase
    {
        [Test]
        public void ShouldViewAndSaveSingleApplicationConfigurationRecord()
        {
            DeleteAll<ApplicationConfiguration>();

            // Even with no config record, a reasonable value is inferred.
            ExecuteEditQuery().AllowUserRegistration.ShouldBe(false);
            Count<ApplicationConfiguration>().ShouldBe(0);

            // The first save creates the single config record.
            ExecuteEditCommand(new EditConfiguration.Command { AllowUserRegistration = false });
            ExecuteEditQuery().AllowUserRegistration.ShouldBe(false);
            Count<ApplicationConfiguration>().ShouldBe(1);

            // Subsequent saves merely update the single config record.
            ExecuteEditCommand(new EditConfiguration.Command { AllowUserRegistration = true });
            ExecuteEditQuery().AllowUserRegistration.ShouldBe(true);
            Count<ApplicationConfiguration>().ShouldBe(1);
        }

        private EditConfiguration.Command ExecuteEditQuery()
        {
            return Transaction(database =>
                new EditConfiguration.QueryHandler(database).Execute());
        }

        private void ExecuteEditCommand(EditConfiguration.Command command)
        {
            Transaction(database =>
                new EditConfiguration.CommandHandler(database).Execute(command));
        }
    }
}