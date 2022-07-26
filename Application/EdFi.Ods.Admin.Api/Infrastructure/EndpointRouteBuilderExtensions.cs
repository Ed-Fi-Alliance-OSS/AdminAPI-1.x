using Swashbuckle.AspNetCore.Annotations;

namespace EdFi.Ods.Admin.Api.Infrastructure
{
    public static class EndpointRouteBuilderExtensions
    {
        private static RouteHandlerBuilder SetDefaultOptions(RouteHandlerBuilder routeHandlerBuilder, string operationSummary, string tag)
        {
            routeHandlerBuilder.WithMetadata(new SwaggerOperationAttribute(operationSummary, null));
            routeHandlerBuilder.RequireAuthorization();

            return routeHandlerBuilder;
        }
    }
}
