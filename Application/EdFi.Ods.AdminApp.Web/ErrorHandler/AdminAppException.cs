using System;
using System.Net;

namespace EdFi.Ods.AdminApp.Web.ErrorHandler
{
    public class AdminAppException : Exception
    {
        public AdminAppException(string stackTrace, string error, string status, Exception innerException)
            : base($"Error: {error}\nStatus:{status}\nStackTrace: {stackTrace}\n", innerException)
        { }

        public HttpStatusCode? StatusCode { get; set; }
        public bool IsExceptionMessageRelevant { get; set; }
        public bool IsStackTraceRelevant { get; set; }
    }
}
