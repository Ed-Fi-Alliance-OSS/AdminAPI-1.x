using EdFi.Ods.Admin.Api.ActionFilters;
using EdFi.Ods.Admin.Api.Infrastructure.Extensions;

namespace EdFi.Ods.Admin.Api.Infrastructure
{
    public static class EndpointRouteBuilderExtensions
    {
        internal static AdminApiEndpointBuilder WithDefaultOptions(this AdminApiEndpointBuilder builder, string tag)
        {
            builder.WithRouteOptions(rhb => SetDefaultOptions(rhb, "Temp Message Will Fix", tag));
            return builder;
        }

        internal static RouteHandlerBuilder WithDefaultGetOptions(this RouteHandlerBuilder builder, string tag)
            => SetDefaultOptions(builder, $"Retrieves all {tag}.", tag);

        internal static RouteHandlerBuilder WithDefaultGetByIdOptions(this RouteHandlerBuilder builder, string tag)
            => SetDefaultOptions(builder, $"Retrieves a specific {tag.ToSingleEntity()} based on the identifier.", tag);

        internal static RouteHandlerBuilder WithDefaultPostOptions(this RouteHandlerBuilder builder, string tag)
            => SetDefaultOptions(builder, $"Creates {tag.ToSingleEntity()} based on the supplied values.", tag);

        internal static RouteHandlerBuilder WithDefaultPutOptions(this RouteHandlerBuilder builder, string tag)
            => SetDefaultOptions(builder, $"Updates {tag.ToSingleEntity()} based on the resource identifier.", tag);

        internal static RouteHandlerBuilder WithDefaultDeleteOptions(this RouteHandlerBuilder builder, string tag)
            => SetDefaultOptions(builder, $"Deletes an existing {tag.ToSingleEntity()} using the resource identifier.", tag);

        private static RouteHandlerBuilder SetDefaultOptions(RouteHandlerBuilder routeHandlerBuilder, string operationSummary, string tag)
        {
            routeHandlerBuilder.WithMetadata(new OperationDescriptionAttribute(operationSummary, null));
            routeHandlerBuilder.WithTags(tag);
            routeHandlerBuilder.RequireAuthorization();

            return routeHandlerBuilder;
        }
    }
}
