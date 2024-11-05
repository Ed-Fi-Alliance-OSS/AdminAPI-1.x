// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Reflection;
using EdFi.Ods.AdminApi.AdminConsole.Features;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;


namespace EdFi.Ods.AdminApi.AdminConsole;

public class AdminApiAdminConsoleEndpointBuilder
{
    private AdminApiAdminConsoleEndpointBuilder(IEndpointRouteBuilder endpoints,
        HttpVerb verb, string route, Delegate handler)
    {
        _endpoints = endpoints;
        _verb = verb;
        _route = route.Trim('/');
        _handler = handler;
        _pluralResourceName = _route.Split('/')[0];
    }

    private readonly IEndpointRouteBuilder _endpoints;
    private readonly HttpVerb? _verb;
    private readonly string _route;
    private readonly Delegate? _handler;
    private readonly List<Action<RouteHandlerBuilder>> _routeOptions = new();
    private readonly string _pluralResourceName;
    private bool _allowAnonymous = false;

    public static AdminApiAdminConsoleEndpointBuilder MapGet(IEndpointRouteBuilder endpoints, string route, Delegate handler)
        => new(endpoints, HttpVerb.GET, route, handler);

    public static AdminApiAdminConsoleEndpointBuilder MapPost(IEndpointRouteBuilder endpoints, string route, Delegate handler)
        => new(endpoints, HttpVerb.POST, route, handler);

    public static AdminApiAdminConsoleEndpointBuilder MapPut(IEndpointRouteBuilder endpoints, string route, Delegate handler)
        => new(endpoints, HttpVerb.PUT, route, handler);

    public static AdminApiAdminConsoleEndpointBuilder MapDelete(IEndpointRouteBuilder endpoints, string route, Delegate handler)
        => new(endpoints, HttpVerb.DELETE, route, handler);

    public AdminApiAdminConsoleEndpointBuilder WithRouteOptions(Action<RouteHandlerBuilder> routeHandlerBuilderAction)
    {
        _routeOptions.Add(routeHandlerBuilderAction);
        return this;
    }

    public void BuildForVersions()
    {
        var version = "adminconsole";
        if (_route == null)
            throw new InvalidOperationException("Invalid endpoint registration. Route must be specified");
        if (_handler == null)
            throw new InvalidOperationException("Invalid endpoint registration. Handler must be specified");


        if (version == null)
            throw new ArgumentException("Version cannot be null");

        var versionedRoute = $"/{version}/{_route}";

        var builder = _verb switch
        {
            HttpVerb.GET => _endpoints.MapGet(versionedRoute, _handler),
            HttpVerb.POST => _endpoints.MapPost(versionedRoute, _handler),
            HttpVerb.PUT => _endpoints.MapPut(versionedRoute, _handler),
            HttpVerb.DELETE => _endpoints.MapDelete(versionedRoute, _handler),
            _ => throw new ArgumentOutOfRangeException($"Unconfigured HTTP verb for mapping: {_verb}")
        };

        if (_allowAnonymous)
        {
            builder.AllowAnonymous();
        }
        else
        {
            builder.RequireAuthorization();
        }

        builder.WithGroupName(version.ToString());
        builder.WithResponseCode(401, "Unauthorized. The request requires authentication");
        builder.WithResponseCode(403, "Forbidden. The request is authenticated, but not authorized to access this resource");
        builder.WithResponseCode(409, "Conflict. The request is authenticated, but it has a conflict with an existing element");
        builder.WithResponseCode(500, "FeatureConstants.InternalServerErrorResponseDescription");

        if (_route.Contains("id", StringComparison.InvariantCultureIgnoreCase))
        {
            builder.WithResponseCode(404, "Not found. A resource with given identifier could not be found.");
        }

        if (_verb is HttpVerb.PUT or HttpVerb.POST)
        {
            builder.WithResponseCode(400, "FeatureConstants.BadRequestResponseDescription");
        }

        foreach (var action in _routeOptions)
        {
            action(builder);
        }

    }


    public AdminApiAdminConsoleEndpointBuilder AllowAnonymous()
    {
        _allowAnonymous = true;
        return this;
    }

    private enum HttpVerb { GET, POST, PUT, DELETE }
}

public static class EndpointRouteBuilderExtensions
{
    public static RouteHandlerBuilder WithResponseCode(this RouteHandlerBuilder builder, int code, string? description = null)
    {
        builder.Produces(code);
        builder.WithMetadata(new SwaggerResponseAttribute(code, description));
        return builder;
    }

    public static RouteHandlerBuilder WithResponse<T>(this RouteHandlerBuilder builder, int code, string? description = null)
    {
        builder.Produces(code, responseType: typeof(T));
        builder.WithMetadata(new SwaggerResponseAttribute(code, description, typeof(T)));
        return builder;
    }
}

public static class FeaturesHelper
{
    public static List<IFeature> GetFeatures()
    {
        var featureInterface = typeof(IFeature);
        var featureImpls = Assembly.GetExecutingAssembly().GetTypes()
            .Where(p => featureInterface.IsAssignableFrom(p) && p.IsClass);

        var features = new List<IFeature>();

        foreach (var featureImpl in featureImpls)
        {
            if (Activator.CreateInstance(featureImpl) is IFeature feature)
                features.Add(feature);
        }
        return features;
    }
}

public static class ValidatorExtensions
{
    public static async Task GuardAsync<TRequest>(this IValidator<TRequest> validator, TRequest request)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
    }
}
