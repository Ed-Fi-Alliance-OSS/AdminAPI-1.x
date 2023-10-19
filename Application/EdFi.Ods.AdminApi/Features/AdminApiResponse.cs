// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Net.Mime;
using System.Text;

namespace EdFi.Ods.AdminApi.Features;

public class AdminApiResponse
{
    protected AdminApiResponse(int status) { Status = status; Title = $"Status {status}"; }
    protected AdminApiResponse(int status, string title) : this(status) { Title = title; }

    public int Status { get; }
    public string Title { get; }

    public static IResult Ok(string message)
        => Results.Ok(new AdminApiResponse(200, message));

    public static IResult Deleted(string name)
        => Results.Ok(new AdminApiResponse(200, $"{name} deleted successfully"));
}

[SwaggerSchema(Title = "AdminApiResponse", Description = "Wrapper schema for all successful responses")]
public class AdminApiResponse<T> : AdminApiResponse
{
    public T Result { get; }

    protected AdminApiResponse(int status, T result) : base(status) { Result = result; }
    protected AdminApiResponse(int status, string title, T result) : base(status, title) { Result = result; }

    public static IResult Ok(T result)
        => Results.Ok(new AdminApiResponse<T>(200, "Request successful", result));

    public static IResult Ok(T result, JsonSerializerSettings jsonSerializerSettings)
    {
        var statusCode = HttpStatusCode.OK;
        var jsonResponse = SerializeObjectAsJson(result, statusCode, "Request successful", jsonSerializerSettings);
        return new JsonSerializeResult(jsonResponse, statusCode);
    }

    public static IResult Created(T result, string name, string getUri)
        => Results.Created(getUri, new AdminApiResponse<T>(201, $"{name} created successfully", result));

    public static IResult Created(T result, string name, string getUri, JsonSerializerSettings jsonSerializerSettings)
    {
        var statusCode = HttpStatusCode.Created;
        var jsonResponse = SerializeObjectAsJson(result, statusCode, $"{name} created successfully", jsonSerializerSettings);
        return new JsonSerializeResult(jsonResponse, statusCode, getUri);
    }

    public static IResult Updated(T result, string name)
        => Results.Ok(new AdminApiResponse<T>(200, $"{name} updated successfully", result));

    public static IResult Updated(T result, string name, JsonSerializerSettings jsonSerializerSettings)
    {
        var statusCode = HttpStatusCode.OK;
        var jsonResponse = SerializeObjectAsJson(result, statusCode, $"{name} updated successfully", jsonSerializerSettings);
        return new JsonSerializeResult(jsonResponse, statusCode);
    }

    private static string SerializeObjectAsJson(T result, HttpStatusCode statusCode, string message, JsonSerializerSettings jsonSerializerSettings)
    {
        var data = new AdminApiResponse<T>((int)statusCode, message, result);
        var dataSerialize = JsonConvert.SerializeObject(data, Formatting.Indented, jsonSerializerSettings);

        return dataSerialize;
    }
}


public class JsonSerializeResult : IResult
{
    private readonly string _jsonResponse;
    private readonly HttpStatusCode _statusCode;
    private readonly string _location;

    public JsonSerializeResult(string jsonResponse, HttpStatusCode statusCode, string location = "")
    {
        _jsonResponse = jsonResponse;
        _statusCode = statusCode;
        _location = location;
    }
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.ContentType = MediaTypeNames.Application.Json;
        httpContext.Response.ContentLength = Encoding.UTF8.GetByteCount(_jsonResponse);
        httpContext.Response.StatusCode = (int)_statusCode;

        if (!string.IsNullOrEmpty(_location))
            httpContext.Response.Headers.Location = _location;

        await httpContext.Response.WriteAsync(_jsonResponse);
    }
}
