// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApi.Infrastructure.Api;
using log4net;

namespace EdFi.Ods.AdminApi.Infrastructure;

public interface IOdsSecurityModelVersionResolver
{
    EdFiOdsSecurityModelCompatibility DetermineSecurityModel();
}

public class OdsSecurityVersionResolver : IOdsSecurityModelVersionResolver
{
    private readonly ILog _log;
    private readonly Lazy<EdFiOdsSecurityModelCompatibility> _modelVersion;
    private readonly IOdsApiValidator _apiValidator;
    private readonly string _apiServerUrl;

    // In ODS/API version 5.3 there is a special branch/tag that provides
    // additional security functionality, which was originally developed for
    // ODS/API 6.1. In general, the Admin and Security databases still look like
    // ODS/API 5.3, but we need to know that it is in fact "CQE" so that we can
    // handle the "ReadChanges" action properly.
    private readonly bool _using53Cqe;

    public OdsSecurityVersionResolver(IOdsApiValidator apiValidator, string apiServerUrl, bool using53Cqe = false)
    {
        _apiValidator = apiValidator;
        _apiServerUrl = apiServerUrl;
        _using53Cqe = using53Cqe;
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
            throw validationResult.Exception ?? new Exception("No version reported from the Ed-Fi API");
        }

        if (_using53Cqe)
        {
            return EdFiOdsSecurityModelCompatibility.FiveThreeCqe;
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
    Both = 3,
    FiveThreeCqe = 4
}
