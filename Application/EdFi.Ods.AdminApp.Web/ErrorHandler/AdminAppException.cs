using System;
using System.Net;

namespace EdFi.Ods.AdminApp.Web.ErrorHandler
{
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
        public HttpStatusCode? StatusCode { get; set; }
        public bool AllowFeedback { get; set; }
        public bool IsStackTraceRelevant { get; set; }
    }
}
