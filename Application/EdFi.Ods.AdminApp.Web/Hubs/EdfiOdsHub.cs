// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace EdFi.Ods.AdminApp.Web.Hubs
{
    [Authorize]
    public class EdfiOdsHub<T> : Hub<T> where T : class
    {
        protected string GroupName => typeof(T).ToString();

        public async Task Subscribe()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, GroupName);
        }

        public async Task Unsubscribe()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName);
        }
    }
}
