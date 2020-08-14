// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Models;

namespace EdFi.Ods.AdminApp.Management.User
{
    public class EditOdsInstanceRegistrationForUserCommand
    {
        public void Execute(IEditOdsInstanceRegistrationForUserModel model)
        {
            using (var database = AdminAppIdentityDbContext.Create())
            {
                var preexistingAssociations = database.UserOdsInstanceRegistrations.Where(x => x.UserId == model.UserId).ToList();

                var selectedOdsInstanceRegistrationIds =
                    model.OdsInstanceRegistrations.Where(x => x.Selected).Select(x => x.OdsInstanceRegistrationId).ToList();

                var recordsToAdd = NewAssignments(model.UserId, selectedOdsInstanceRegistrationIds, preexistingAssociations);

                if (recordsToAdd.Any())
                    database.UserOdsInstanceRegistrations.AddRange(recordsToAdd);

                var recordsToRemove = AssignmentsToRemove(selectedOdsInstanceRegistrationIds, preexistingAssociations);

                if (recordsToRemove.Any())
                    database.UserOdsInstanceRegistrations.RemoveRange(recordsToRemove);

                database.SaveChanges();
            }
        }

        private static List<UserOdsInstanceRegistration> AssignmentsToRemove(List<int> requestedOdsInstanceRegistrationIds, List<UserOdsInstanceRegistration> preexistingAssociations)
        {
            return preexistingAssociations
                .Where(record => !requestedOdsInstanceRegistrationIds.Contains(record.OdsInstanceRegistrationId))
                .ToList();
        }

        private static List<UserOdsInstanceRegistration> NewAssignments(string userId, List<int> requestedOdsInstanceRegistrationIds, List<UserOdsInstanceRegistration> preexistingAssociations)
        {
            var missingOdsInstanceRegistrationIds =
                requestedOdsInstanceRegistrationIds.Except(
                    preexistingAssociations.Select(x => x.OdsInstanceRegistrationId));

            return missingOdsInstanceRegistrationIds
                .Select(x => new UserOdsInstanceRegistration
                {
                    UserId = userId,
                    OdsInstanceRegistrationId = x
                }).ToList();
        }
    }

    public interface IEditOdsInstanceRegistrationForUserModel
    {
        string UserId  { get; }
        List<OdsInstanceRegistrationSelection> OdsInstanceRegistrations { get; }
    }

    public class OdsInstanceRegistrationSelection
    {
        public int OdsInstanceRegistrationId { get; set; }
        public string Name { get; set; }
        public bool Selected { get; set; }
    }
}
