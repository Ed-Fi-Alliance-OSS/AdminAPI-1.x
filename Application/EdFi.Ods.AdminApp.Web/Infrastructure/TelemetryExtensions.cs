// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading.Tasks;

namespace EdFi.Ods.AdminApp.Web.Infrastructure
{
    public static class TelemetryExtensions
    {
        public static async Task View(this ITelemetry telemetry, string item)
            => await telemetry.Event($"View {item}");

        public static async Task Event(this ITelemetry telemetry, string action, string label = null)
            => await telemetry.Event("Admin App Web", action, label);
    }
}
