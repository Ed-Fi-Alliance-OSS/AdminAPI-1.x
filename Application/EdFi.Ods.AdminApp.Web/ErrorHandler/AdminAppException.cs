using System;

namespace EdFi.Ods.AdminApp.Web.ErrorHandler
{

    // TODO: in AA-1377 we will start making use this class to define specific errors
    public class AdminAppException : Exception
    {
        public AdminAppException(string stackTrace, string error, string status, Exception innerException)
            : base($"Error: {error}\nStatus:{status}\nStackTrace: {stackTrace}\n", innerException)
        { }
    }
}
