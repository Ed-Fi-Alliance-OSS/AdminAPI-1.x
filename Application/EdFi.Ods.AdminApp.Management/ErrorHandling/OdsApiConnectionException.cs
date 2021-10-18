// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Net;

namespace EdFi.Ods.AdminApp.Management.ErrorHandling
{
    public class OdsApiConnectionException : Exception, IAdminAppException
    {
        public OdsApiConnectionException(HttpStatusCode responseCode, string message) : base(message)
        {
            StatusCode = responseCode;
        }

        public OdsApiConnectionException(HttpStatusCode responseCode, string message, Exception innerException) : base(message, innerException)
        {
            StatusCode = responseCode;
        }

        public HttpStatusCode? StatusCode { get; }
        public bool AllowFeedback { get; set; }
        public bool IsStackTraceRelevant { get => false;  }
    }
}
