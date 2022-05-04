// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace EdFi.Ods.Admin.Api.Features;

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

public class AdminApiResponse<T> : AdminApiResponse
{
    public T Result { get; }

    protected AdminApiResponse(int status, T result) : base(status) { Result = result; }
    protected AdminApiResponse(int status, string title, T result) : base(status, title) { Result = result; }

    public static IResult Ok(T result)
        => Results.Ok(new AdminApiResponse<T>(200, "Request successful", result));

    public static IResult Created(T result, string name, string getUri)
        => Results.Created(getUri, new AdminApiResponse<T>(201, $"{name} created successfully", result));

    public static IResult Updated(T result, string name)
        => Results.Ok(new AdminApiResponse<T>(200, $"{name} updated successfully", result));
}
