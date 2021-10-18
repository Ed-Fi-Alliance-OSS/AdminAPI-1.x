using System;
using System.Net;

namespace EdFi.Ods.AdminApp.Web.ErrorHandler
{
    public class AdminAppException : Exception
    {
        public HttpStatusCode? StatusCode { get; set; }
        public bool IsExceptionMessageRelevant { get; set; }
        public bool IsStackTraceRelevant { get; set; }
    }
}
