using System;
using System.Net;

namespace EdFi.Ods.AdminApp.Web.ErrorHandler
{
    public interface IAdminAppException
    {
        string Message { get; }
        string StackTrace { get; }
        HttpStatusCode? StatusCode { get; }
        bool IsExceptionMessageRelevant { get; }
        bool IsStackTraceRelevant { get; }
    }

    public class AdminAppException : Exception, IAdminAppException
    {
        public HttpStatusCode? StatusCode { get; set; }
        public bool IsExceptionMessageRelevant { get; set; }
        public bool IsStackTraceRelevant { get; set; }
    }
}
