// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Net;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.ErrorHandling;

public interface IAdminApiAdminConsoleException
{
    string? Message { get; }
    string? StackTrace { get; }
    HttpStatusCode? StatusCode { get; }
    bool AllowFeedback { get; }
    bool IsStackTraceRelevant { get; }
}

public class AdminApiAdminConsoleException : Exception, IAdminApiAdminConsoleException
{
    public AdminApiAdminConsoleException(string message) : base(message) { }
    public AdminApiAdminConsoleException(string message, Exception innerException) : base(message, innerException) { }

    public HttpStatusCode? StatusCode { get; set; }
    public bool AllowFeedback { get; set; } = true;
    public bool IsStackTraceRelevant { get; set; } = false;
}
