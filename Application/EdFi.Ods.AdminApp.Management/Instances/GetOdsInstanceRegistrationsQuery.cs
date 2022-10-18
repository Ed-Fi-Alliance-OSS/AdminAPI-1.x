// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using EdFi.Ods.AdminApp.Management.Database;
using EdFi.Ods.AdminApp.Management.Database.Models;

namespace EdFi.Ods.AdminApp.Management.Instances
{
    public class GetOdsInstanceRegistrationsQuery : IGetOdsInstanceRegistrationsQuery
    {
        private readonly AdminAppDbContext _database;

        public GetOdsInstanceRegistrationsQuery(AdminAppDbContext database)
        {
            _database = database;
        }

        public IEnumerable<OdsInstanceRegistration> Execute()
        {
            var instances = _database.OdsInstanceRegistrations.OrderBy(x => x.Name);

            return instances;
        }

        public int ExecuteCount()
        {
            return _database.OdsInstanceRegistrations.Count();
        }

        public OdsInstanceRegistration Execute(string odsInstanceRegistrationName)
        {
            return _database.OdsInstanceRegistrations.SingleOrDefault(x => x.Name == odsInstanceRegistrationName);
        }

        public OdsInstanceRegistration Execute(int odsInstanceId)
        {
            return _database.OdsInstanceRegistrations.SingleOrDefault(x => x.Id == odsInstanceId);
        }
    }

    public interface IGetOdsInstanceRegistrationsQuery
    {
        IEnumerable<OdsInstanceRegistration> Execute();
        int ExecuteCount();
        OdsInstanceRegistration Execute(string odsInstanceRegistrationName);
        OdsInstanceRegistration Execute(int odsInstanceId);
    }
}
