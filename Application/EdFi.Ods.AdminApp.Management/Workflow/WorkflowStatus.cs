// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.AdminApp.Management.Workflow
{
    public delegate void WorkflowStatusUpdated(WorkflowStatus status);

    public class WorkflowStatus
    {
        public bool Complete { get; set; }
        public bool Executed { get; set; }
        public bool Error { get; set; }
        public string ErrorMessage { get; set; }
        public int CurrentStep { get; set; }
        public int TotalSteps { get; set; }
        public string StatusMessage { get; set; }
        public bool Warning { get; set; }
    }
}