// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Net;

namespace EdFi.Ods.AdminApi.Common.Infrastructure.ErrorHandling;

public interface IAdminApiException
{
    string? Message { get; }
    string? StackTrace { get; }
    HttpStatusCode? StatusCode { get; }
    bool AllowFeedback { get; }
    bool IsStackTraceRelevant { get; }
}

public class AdminApiException : Exception, IAdminApiException
{
    public AdminApiException(string message) : base(message) { }
    public AdminApiException(string message, Exception innerException) : base(message, innerException) { }

    public HttpStatusCode? StatusCode { get; set; }
    public bool AllowFeedback { get; set; } = true;
    public bool IsStackTraceRelevant { get; set; } = false;
}
