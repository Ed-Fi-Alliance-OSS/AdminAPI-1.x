// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

#if NET48
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
#else
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
#endif
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Management.Workflow;

namespace EdFi.Ods.AdminApp.Web.Hubs
{
#if !NET48
    //Temporary stub allowing for compilation, but expected to fail at runtime.
    //Port the NET48 code below and then remove this stub.
    public abstract class EdfiOdsHub<T>
    {
        public void SendOperationStatusUpdate(WorkflowStatus status) => throw new System.NotImplementedException();
    }
#else
    [Authorize]
    public abstract class EdfiOdsHub<T> : Hub<T> where T : class, IHub
    {
        protected string GroupName => typeof (T).ToString();

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
#endif
}
