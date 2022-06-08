using EdFi.Ods.Admin.Api.ActionFilters;
using EdFi.Ods.Admin.Api.Infrastructure.Extensions;

namespace EdFi.Ods.Admin.Api.Infrastructure
{
    public static class EndpointEndpointRouteBuilderExtensions
    {
        internal static RouteHandlerBuilder MapGetWithDefaultOptions(this IEndpointRouteBuilder builder,
           string pattern, Delegate handler, string tag)
        {
            var routeHandler = builder.MapGet(pattern, handler);
            SetDefaultOptions(routeHandler, 1, $"Retrieves all the {tag}.", tag);
            return routeHandler;
        }

        internal static RouteHandlerBuilder MapGetByIdWithDefaultOptions(this IEndpointRouteBuilder builder,
          string pattern, Delegate handler, string tag)
        {
            var routeHandler = builder.MapGet(pattern, handler);
            SetDefaultOptions(routeHandler, 2, $"Retrieves a specific {tag.ToSingleEntity()} based on the resource identifier.", tag);
            return routeHandler;
        }

        internal static RouteHandlerBuilder MapPostWithDefaultOptions(this IEndpointRouteBuilder builder,
           string pattern, Delegate handler, string tag)
        {
            var routeHandler = builder.MapPost(pattern, handler);
            SetDefaultOptions(routeHandler, 3, $"Creates {tag.ToSingleEntity()} based on the supplied values.", tag);
            return routeHandler;
        }

        internal static RouteHandlerBuilder MapPutWithDefaultOptions(this IEndpointRouteBuilder builder,
           string pattern, Delegate handler, string tag)
        {
            var routeHandler = builder.MapPut(pattern, handler);
            SetDefaultOptions(routeHandler, 4, $"Updates {tag.ToSingleEntity()} based on the resource identifier.", tag);
            return routeHandler;
        }

        internal static RouteHandlerBuilder MapDeleteWithDefaultOptions(this IEndpointRouteBuilder builder,
   string pattern, Delegate handler, string tag)
        {
            var routeHandler = builder.MapDelete(pattern, handler);
            SetDefaultOptions(routeHandler, 5, $"Deletes an existing {tag.ToSingleEntity()} using the resource identifier.", tag);
            return routeHandler;
        }

        private static void SetDefaultOptions(RouteHandlerBuilder routeHandlerBuilder, int operationOrder, string operationSummary, string tag)
        {
            routeHandlerBuilder.WithMetadata(new OperationOrderAttribute(operationOrder));
            routeHandlerBuilder.WithMetadata(new OperationDescriptionAttribute(operationSummary, null));
            routeHandlerBuilder.WithTags(tag);
            routeHandlerBuilder.RequireAuthorization();
        }
    }
}
