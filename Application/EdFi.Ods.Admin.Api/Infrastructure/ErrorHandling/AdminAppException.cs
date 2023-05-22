using System;
using System.Net;

namespace EdFi.Ods.Admin.Api.Infrastructure.ErrorHandling;

public interface IAdminAppException
{
    string Message { get; }
    string StackTrace { get; }
    HttpStatusCode? StatusCode { get; }
    bool AllowFeedback { get; }
    bool IsStackTraceRelevant { get; }
}

public class AdminAppException : Exception, IAdminAppException
{
    public AdminAppException(string message) : base(message) { }
    public AdminAppException(string message, Exception innerException) : base(message, innerException) { }

    public HttpStatusCode? StatusCode { get; set; }
    public bool AllowFeedback { get; set; } = true;
    public bool IsStackTraceRelevant { get; set; } = false;
}
