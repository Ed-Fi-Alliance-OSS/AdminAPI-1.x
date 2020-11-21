// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Models;
using EdFi.Ods.AdminApp.Management.Instances;
using EdFi.Ods.AdminApp.Management.User;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.OdsInstances;
using NUnit.Framework;
using Shouldly;
using static EdFi.Ods.AdminApp.Management.Tests.Testing;
using static EdFi.Ods.AdminApp.Management.Tests.Instance.InstanceTestSetup;
using static EdFi.Ods.AdminApp.Management.Tests.User.UserTestSetup;
using static EdFi.Ods.AdminApp.Management.Tests.TestingHelper;

namespace EdFi.Ods.AdminApp.Management.Tests.Instance
{
    [TestFixture]
    public class DeregisterOdsInstanceCommandTests: AdminAppDataTestBase
    {
        [Test]
        public void ShouldDeregisterOdsInstance()
        {
            var users = SetupUsers(2).ToList();
            var testUser1 = users[0];
            var testUser2 = users[1];

            var testInstances = SetupOdsInstanceRegistrations(2).OrderBy(x => x.Name).ToList();
            var testInstanceToBeDeregistered = testInstances[0];
            var testInstanceNotToBeDeregistered = testInstances[1];

            MockInstanceRegistrationSetup(testInstances);

            Scoped<IUsersContext>(database =>
            {
                database.OdsInstances.Count().ShouldBe(2);
                database.Applications.Count().ShouldBe(2);
                database.Clients.Count().ShouldBe(2);
                database.ApplicationEducationOrganizations.Count().ShouldBe(2);
                database.ClientAccessTokens.Count().ShouldBe(2);
            });

            ShouldNotBeNull<SecretConfiguration>(x => x.OdsInstanceRegistrationId == testInstanceToBeDeregistered.Id);
            ShouldNotBeNull<SecretConfiguration>(x => x.OdsInstanceRegistrationId == testInstanceNotToBeDeregistered.Id);

            SetupUserWithOdsInstanceRegistrations(testUser1.Id, testInstances);
            SetupUserWithOdsInstanceRegistrations(testUser2.Id, testInstances);

            Scoped<IGetOdsInstanceRegistrationsByUserIdQuery>(queryInstances =>
            {
                queryInstances.Execute(testUser1.Id).Count().ShouldBe(2);
                queryInstances.Execute(testUser2.Id).Count().ShouldBe(2);
            });

            var deregisterModel = new DeregisterOdsInstanceModel
            {
                OdsInstanceId = testInstanceToBeDeregistered.Id,
                Name = testInstanceToBeDeregistered.Name,
                Description = testInstanceToBeDeregistered.Description
            };

            Scoped<DeregisterOdsInstanceCommand>(command => command.Execute(deregisterModel));

            var deregisteredOdsInstance = Transaction(database => database.OdsInstanceRegistrations.SingleOrDefault(x => x.Id == testInstanceToBeDeregistered.Id));
            deregisteredOdsInstance.ShouldBeNull();

            var notDeregisteredOdsInstance = Transaction(database => database.OdsInstanceRegistrations.SingleOrDefault(x => x.Id == testInstanceNotToBeDeregistered.Id));
            notDeregisteredOdsInstance.ShouldNotBeNull();

            ShouldBeNull<SecretConfiguration>(x => x.OdsInstanceRegistrationId == testInstanceToBeDeregistered.Id);
            ShouldNotBeNull<SecretConfiguration>(x => x.OdsInstanceRegistrationId == testInstanceNotToBeDeregistered.Id);

            Scoped<IGetOdsInstanceRegistrationsByUserIdQuery>(queryInstances =>
            {
                var instancesAssignedToUser1 = queryInstances.Execute(testUser1.Id).ToList();
                instancesAssignedToUser1.Count.ShouldBe(1);
                var onlyInstanceAssignedToUser1 = instancesAssignedToUser1.Single();
                onlyInstanceAssignedToUser1.Id.ShouldBe(testInstanceNotToBeDeregistered.Id);
                onlyInstanceAssignedToUser1.Name.ShouldBe(testInstanceNotToBeDeregistered.Name);
                onlyInstanceAssignedToUser1.Description.ShouldBe(testInstanceNotToBeDeregistered.Description);

                var instancesAssignedToUser2 = queryInstances.Execute(testUser2.Id).ToList();
                instancesAssignedToUser2.Count.ShouldBe(1);
                var onlyInstanceAssignedToUser2 = instancesAssignedToUser2.Single();
                onlyInstanceAssignedToUser2.Id.ShouldBe(testInstanceNotToBeDeregistered.Id);
                onlyInstanceAssignedToUser2.Name.ShouldBe(testInstanceNotToBeDeregistered.Name);
                onlyInstanceAssignedToUser2.Description.ShouldBe(testInstanceNotToBeDeregistered.Description);
            });

            Scoped<IUsersContext>(database =>
            {
                database.OdsInstances.Count().ShouldBe(2);
                database.Applications.Count().ShouldBe(1);
                database.Clients.Count().ShouldBe(1);
                database.ApplicationEducationOrganizations.Count().ShouldBe(1);
                database.ClientAccessTokens.Count().ShouldBe(1);
            });
        }

        private static void MockInstanceRegistrationSetup(List<OdsInstanceRegistration> odsInstanceRegistrations)
        {
            var odsInstances = odsInstanceRegistrations.Select(x => new OdsInstance
            {
                OdsInstanceId = x.Id,
                Name = x.Name,
                InstanceType = "Ods",
                IsExtended = false,
                Status = "OK",
                Version = "1.0.0"
            }).ToList();

            var applications = odsInstances.Select(x => new Application
            {
                ApplicationName = x.Name.GetAdminApplicationName(),
                OperationalContextUri = OperationalContext.DefaultOperationalContextUri,
                OdsInstance = x
            }).ToList();

            foreach (var application in applications)
            {
                var client = new ApiClient
                {
                    Name = application.ApplicationName,
                    Key = "test key",
                    Secret = "test secret",
                    ActivationCode = "test activation code"
                };
                var clientAccessToken = new ClientAccessToken
                {
                    ApiClient = client,
                    Expiration = DateTime.Now.AddDays(1)
                };
                var appEduOrganization = new ApplicationEducationOrganization
                {
                    Application = application,
                    EducationOrganizationId = application.OdsInstance.Name.ExtractNumericInstanceSuffix()
                };

                client.ClientAccessTokens.Add(clientAccessToken);
                application.ApiClients.Add(client);
                application.ApplicationEducationOrganizations.Add(appEduOrganization);
            }

            Scoped<IUsersContext>(database =>
            {
                foreach (var odsInstance in odsInstances)
                {
                    database.OdsInstances.Add(odsInstance);
                }

                foreach (var application in applications)
                {
                    database.Applications.Add(application);
                }

                database.SaveChanges();
            });
        }

        [Test]
        public void ShouldNotDeregisterOdsInstanceIfRequiredFieldsEmpty()
        {
            var deregisterModel = new DeregisterOdsInstanceModel
            {
                OdsInstanceId = 0,
                Name = "",
                Description = ""
            };

            Scoped<AdminAppDbContext>(database =>
            {
                var validator = new DeregisterOdsInstanceModelValidator(database);
                validator.ShouldNotValidate(deregisterModel,
                    "'ODS Instance Database' must not be empty.", 
                    "'ODS Instance Description' must not be empty.", 
                    "'Ods Instance Id' must not be empty.", 
                    "The instance you are trying to deregister does not exist in the database.");
            });
        }

        [Test]
        public void ShouldNotDeregisterIfOdsInstanceDoesNotExist()
        {
            var testInstanceNotInSystem = new OdsInstanceRegistration
            {
                Id = 1,
                Description = "Test Description",
                Name = "EdFi_Ods_1234"
            };

            var deregisterModel = new DeregisterOdsInstanceModel
            {
                OdsInstanceId = testInstanceNotInSystem.Id,
                Name = testInstanceNotInSystem.Name,
                Description = testInstanceNotInSystem.Description
            };

            Scoped<AdminAppDbContext>(database =>
            {
                var validator = new DeregisterOdsInstanceModelValidator(database);
                validator.ShouldNotValidate(deregisterModel,"The instance you are trying to deregister does not exist in the database.");
            });
        }
    }
}
