// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Runtime.CompilerServices;
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
    private readonly string _odsApiVersion;

    public OdsSecurityVersionResolver(string odsApiVersion)
    {
        _odsApiVersion = odsApiVersion;
        _log = LogManager.GetLogger(typeof(OdsSecurityVersionResolver));
        _modelVersion = new Lazy<EdFiOdsSecurityModelCompatibility>(InitializeModelVersion);
    }

    public EdFiOdsSecurityModelCompatibility DetermineSecurityModel() => _modelVersion.Value;

    private EdFiOdsSecurityModelCompatibility InitializeModelVersion()
    {
        try
        {
            return _odsApiVersion switch
            {
                "5.3" => EdFiOdsSecurityModelCompatibility.ThreeThroughFive,
                "5.3-cqe" => EdFiOdsSecurityModelCompatibility.FiveThreeCqe,
                "6.0" or "6.1" => EdFiOdsSecurityModelCompatibility.Six,
                _ => throw new SwitchExpressionException()
            };
        }
        catch (SwitchExpressionException)
        {
            _log.Error("OdsApiVersion not configured. Valid values are 5.3, 5.3-cqe, 6.0 and 6.1");
            throw new Exception("OdsApiVersion not configured. Valid values are 5.3, 5.3-cqe, 6.0 and 6.1");
        }
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
