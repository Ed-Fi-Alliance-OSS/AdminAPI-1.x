// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace EdFi.Ods.AdminApp.Management.Workflow
{
    public class WorkflowResult
    {
        public bool Error { get; set; }
        
        public string ErrorMessage { get; set; }
        public Exception ActionException { get; set; }

        public bool ErrorDuringRollback { get; set; }
        public string RollbackErrorMessage { get; set; }
        public Exception RollbackException { get; set; }

        public int TotalSteps { get; set; }
        public int StepsCompletedSuccessfully { get; set; }
        public int StepsRolledBack { get; set; }
    }
}