using System;
using System.Net;
using System.Threading.Tasks;
using EdFi.Ods.AdminApp.Web.Helpers;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;

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
            try
            {
                await _requestHandler(context);
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }

            void HandleException(Exception exception)
            {
                _logger.Error(exception);

                if (context.Request.IsAjaxRequest())
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    var controllerName = context.Request.RouteValues["controller"].ToString();
                    var responseText = IsReportsController(controllerName) && exception is SqlException
                        ? "An error occurred trying to access the SQL views for reports."
                        : exception.Message;
                    context.Response.WriteAsync(responseText);
                }
                else
                {
                    var status = exception is WebException webException
                        ? webException.Status.ToString()
                        : WebExceptionStatus.UnknownError.ToString();

                    // TODO: in AA-1377 we will start making use of this adminAppError object to show proper error on error page
                    var adminAppError = new AdminAppException(exception.StackTrace, exception.Message, status, exception.InnerException );
                    context.Response.Redirect("/Home/Error/");
                }
            }

            static bool IsReportsController(string controllerName) => controllerName.ToLower().Equals("reports");
        }

    }

    public class AdminAppException : Exception
    {
        public AdminAppException(string stackTrace, string error, string status, Exception innerException)
            : base($"Error: {error}\nStatus:{status}\nStackTrace: {stackTrace}\n", innerException)
        { }
    }
}
