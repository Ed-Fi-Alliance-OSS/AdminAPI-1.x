using System;
using System.Net;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Http;

namespace EdFi.Ods.AdminApp.Web.ErrorHandler
{
    public class GlobalErrorHandler
    {
        private readonly RequestDelegate _requestHandler;
        private readonly ILog _logger = LogManager.GetLogger(typeof(GlobalErrorHandler));

        public GlobalErrorHandler(RequestDelegate requestHandler)
        {
            _requestHandler = requestHandler;
        }

        public async Task Invoke(HttpContext context)
        {
            var adminAppError = new AdminAppException();
            try
            {
                await _requestHandler(context);
            }
            catch (WebException webException)
            {
                _logger.Error(webException);
                adminAppError.StackTrace = webException.StackTrace;
                adminAppError.ErrorMessage = webException.Message;
                adminAppError.Status = webException.Status.ToString();
                adminAppError.InnerException = webException.InnerException;

            }
            catch (Exception exception)
            {
                _logger.Error(exception);
                adminAppError.StackTrace = exception.StackTrace;
                adminAppError.ErrorMessage = exception.Message;
                adminAppError.InnerException = exception.InnerException;
            }
        }
    }

    public class AdminAppException
    {
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
        public string Status { get; set; }
        public Exception InnerException { get; set; }
    }
}
