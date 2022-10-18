// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using EdFi.Admin.DataAccess.Contexts;
using EdFi.Admin.DataAccess.Models;
using Npgsql;

namespace EdFi.Ods.AdminApp.Management
{
    public class GetOdsStatusQuery : IGetOdsStatusQuery
    {
        private readonly IUsersContext _usersContext;

        public GetOdsStatusQuery(IUsersContext usersContext)
        {
            _usersContext = usersContext;
        }

        public CloudOdsStatus Execute(string instanceName)
        {
            OdsInstance instance = null;

            try
            {
                instance = _usersContext.OdsInstances.SingleOrDefault(i => i.Name == instanceName);
            }
            catch (SqlException s)
            {
                //Login failed likely means we haven't yet completed initial setup
                //so we'll return InstanceNotRegistered below
                if (!s.Message.StartsWith("Login failed"))
                {
                    throw;
                }
            }
            catch (EntityCommandExecutionException s)
            {
                //Similarly check for the corresponding exception from NpgSql
                var inner = s.InnerException as PostgresException;
                if (inner == null || !(inner.Message.Contains("relation") && inner.Message.Contains("does not exist")))
                {
                    throw;
                }
            }


            if (instance == null)
            {
                return CloudOdsStatus.InstanceNotRegistered;
            }

            return CloudOdsStatus.Parse(instance.Status);
        }
    }
}
