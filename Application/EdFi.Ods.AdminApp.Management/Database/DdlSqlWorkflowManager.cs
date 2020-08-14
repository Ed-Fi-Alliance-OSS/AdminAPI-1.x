// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Data.SqlClient;
using System.Threading;
using EdFi.Ods.AdminApp.Management.Workflow;

namespace EdFi.Ods.AdminApp.Management.Database
{
    /// <summary>
    /// Implements a best-effort approach to execute a batch of DDL statements as a single unit of work
    /// since DDL statements cannot be run in transactions.
    /// 
    /// This is accomplished by defining a set of steps with actions and rollback actions,
    /// and trying to automatically run each rollback action in reverse order in case a step
    /// in the process fails.
    /// </summary>
    public class DdlSqlWorkflowManager : WorkflowManager<DdlSqlWorkflowStep, SqlConnection>
    {
        public DdlSqlWorkflowManager(SqlConnection context) : base(context)
        {
        }

        public DdlSqlWorkflowManager(SqlConnection context, CancellationToken cancellationToken) : base(context, cancellationToken)
        {
        }
    }
}