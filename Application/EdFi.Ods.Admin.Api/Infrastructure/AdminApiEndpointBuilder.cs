// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.Admin.Api.Infrastructure;

public class AdminApiEndpointBuilder
{
    private AdminApiEndpointBuilder(IEndpointRouteBuilder endpoints,
        HttpVerb verb, string route, Delegate handler)
    {
        _endpoints = endpoints;
        _verb = verb;
        _route = route;
        _handler = handler;
    }

    private readonly IEndpointRouteBuilder _endpoints;
    private readonly HttpVerb _verb;
    private string _route;
    private Delegate _handler;
    private List<Action<RouteHandlerBuilder>> _routeOptions = new();

    public static AdminApiEndpointBuilder MapGet(IEndpointRouteBuilder endpoints, string route, Delegate handler)
        => new(endpoints, HttpVerb.GET, route, handler);

    public static AdminApiEndpointBuilder MapPost(IEndpointRouteBuilder endpoints, string route, Delegate handler)
        => new(endpoints, HttpVerb.POST, route, handler);

    public static AdminApiEndpointBuilder MapPut(IEndpointRouteBuilder endpoints, string route, Delegate handler)
        => new(endpoints, HttpVerb.PUT, route, handler);

    public static AdminApiEndpointBuilder MapDelete(IEndpointRouteBuilder endpoints, string route, Delegate handler)
        => new(endpoints, HttpVerb.DELETE, route, handler);

    public void BuildForVersions(params string[] versions)
    {
        if(versions.Length == 0) throw new ArgumentException("Must register for at least 1 version");
        if(string.IsNullOrEmpty(_route)) throw new Exception("Invalid endpoint registration. Route must be specified");

        foreach (var version in versions)
        {
            var versionedRoute = $"/{version}/{_route}";

            var builder = _verb switch
            {
                HttpVerb.GET => _endpoints.MapGet(versionedRoute, _handler),
                HttpVerb.POST => _endpoints.MapPost(versionedRoute, _handler),
                HttpVerb.PUT => _endpoints.MapPut(versionedRoute, _handler),
                HttpVerb.DELETE => _endpoints.MapDelete(versionedRoute, _handler),
                _ => throw new ArgumentOutOfRangeException($"Unconfigured HTTP verb for mapping: {_verb}")
            };

            foreach (var action in _routeOptions)
            {
                action(builder);
            }
        }
    }

    public AdminApiEndpointBuilder WithRouteOptions(Action<RouteHandlerBuilder> routeHandlerBuilderAction)
    {
        _routeOptions.Add(routeHandlerBuilderAction);
        return this;
    }

    private enum HttpVerb { GET, POST, PUT, DELETE }
}
