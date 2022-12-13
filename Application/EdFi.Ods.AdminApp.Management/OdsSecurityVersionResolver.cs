// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using EdFi.Ods.AdminApp.Management.Api;
using log4net;

namespace EdFi.Ods.AdminApp.Management;

public interface IOdsSecurityModelVersionResolver
{
    EdFiOdsSecurityModelCompatibility DetermineSecurityModel();
}

public class OdsSecurityVersionResolver : IOdsSecurityModelVersionResolver
{
    private readonly ILog _log;
    private Lazy<EdFiOdsSecurityModelCompatibility> _modelVersion;
    private readonly IOdsApiValidator _apiValidator;
    private readonly string _apiServerUrl;

    public OdsSecurityVersionResolver(IOdsApiValidator apiValidator, string apiServerUrl)

    {
        _apiValidator = apiValidator;
        _apiServerUrl = apiServerUrl;
        _log = LogManager.GetLogger(typeof(OdsSecurityVersionResolver));
        _modelVersion = new Lazy<EdFiOdsSecurityModelCompatibility>(InitializeModelVersion);
    }

    public EdFiOdsSecurityModelCompatibility DetermineSecurityModel() => _modelVersion.Value;

    private EdFiOdsSecurityModelCompatibility InitializeModelVersion()
    {
        var validationResult = _apiValidator.Validate(_apiServerUrl).GetAwaiter().GetResult();

        if (!validationResult.IsValidOdsApi || validationResult.Version == null)
        {
            _log.Error("Unable to determine security model from ODS API Response");
            throw validationResult.Exception ?? new Exception("No version reported from ODS API");
        }

        var serverVersion = validationResult.Version;

        return serverVersion.Major < 6
            ? EdFiOdsSecurityModelCompatibility.ThreeThroughFive
            : EdFiOdsSecurityModelCompatibility.Six;
    }
}

public class EdFiOdsSecurityModelCompatibilityException : NotImplementedException
{
    public EdFiOdsSecurityModelCompatibilityException()
        : base("Handling for security model not implemented") { }
    public EdFiOdsSecurityModelCompatibilityException(EdFiOdsSecurityModelCompatibility version)
        : base($"Handling for security model for version {version} not implemented") { }
    public EdFiOdsSecurityModelCompatibilityException(string message) : base(message) { }
}

public enum EdFiOdsSecurityModelCompatibility
{
    ThreeThroughFive = 1,
    Six = 2,
}
