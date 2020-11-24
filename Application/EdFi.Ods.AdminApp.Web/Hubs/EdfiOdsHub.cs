// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Workflow;

namespace EdFi.Ods.AdminApp.Web.Hubs
{
    [Authorize]
    public abstract class EdfiOdsHub<T> : Hub<T> where T : class, IHub
    {
        protected string GroupName => typeof(T).ToString();

        public Task Subscribe()
        {
            return Groups.Add(Context.ConnectionId, GroupName);
        }

        public Task Unsubscribe()
        {
            return Groups.Remove(Context.ConnectionId, GroupName);
        }

        public void SendOperationStatusUpdate(WorkflowStatus status)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<T>();
            hubContext.Clients.Group(GroupName).updateStatus(status);
        }
    }
}
