using System;

namespace EdFi.Ods.AdminApp.Web.ErrorHandler
{
    public class AdminAppException : Exception
    {
        public AdminAppException(string stackTrace, string error, string status, Exception innerException)
            : base($"Error: {error}\nStatus:{status}\nStackTrace: {stackTrace}\n", innerException)
        { }
    }
}
