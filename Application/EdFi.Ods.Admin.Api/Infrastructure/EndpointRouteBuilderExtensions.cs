using Swashbuckle.AspNetCore.Annotations;

namespace EdFi.Ods.Admin.Api.Infrastructure
{
    public static class EndpointRouteBuilderExtensions
    {
        public static RouteHandlerBuilder WithResponseCode(this RouteHandlerBuilder builder, int code, string? description = null)
        {
            builder.Produces(code);
            builder.WithMetadata(new SwaggerResponseAttribute(code, description));
            return builder;
        }

        private static RouteHandlerBuilder SetDefaultOptions(RouteHandlerBuilder routeHandlerBuilder, string operationSummary, string tag)
        {
            routeHandlerBuilder.WithMetadata(new SwaggerOperationAttribute(operationSummary, null));
            routeHandlerBuilder.RequireAuthorization();

            return routeHandlerBuilder;
        }
    }
}
