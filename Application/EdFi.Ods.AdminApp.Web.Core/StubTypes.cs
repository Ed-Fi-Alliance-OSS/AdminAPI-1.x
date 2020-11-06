// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

//This file contains temporary type declarations in service of net48/netcoreapp3.1 cross-compilation.
//These types are intended to be removed as they are ported in full to .NET Core.

using System;
using EdFi.Ods.AdminApp.Management.Workflow;
using EdFi.Ods.AdminApp.Web.Infrastructure.Jobs;
using EdFi.Ods.Common.Security;

namespace EdFi.Ods.AdminApp.Web.Controllers
{
    public class OdsInstanceSettingsController
    {
        public object Applications() => throw new System.NotImplementedException();
        public object Logging() => throw new System.NotImplementedException();
        public object EducationOrganizations() => throw new System.NotImplementedException();
        public object Setup() => throw new System.NotImplementedException();
        public object BulkLoad() => throw new System.NotImplementedException();
        public object LearningStandards() => throw new System.NotImplementedException();
        public object SelectDistrict(int i) => throw new System.NotImplementedException();
    }

    public class OdsInstancesController
    {
        public object Index() => throw new System.NotImplementedException();
        public object RegisterOdsInstance() => throw new System.NotImplementedException();
        public object BulkRegisterOdsInstances() => throw new System.NotImplementedException();
        public object ActivateOdsInstance() => throw new System.NotImplementedException();
        public object DeregisterOdsInstance() => throw new System.NotImplementedException();
    }
}

namespace EdFi.Ods.AdminApp.Web.Infrastructure.IO
{
    public interface IFileUploadHandler { }
    public class LocalFileSystemFileUploadHandler : IFileUploadHandler { }
    public class BulkImportService
    {
        public event WorkflowStatusUpdated StatusUpdated
        {
            add { throw new System.NotImplementedException(); }
            remove { throw new System.NotImplementedException(); }
        }

        public WorkflowResult Execute(BulkUploadJobContext bulkUploadJobContext)
        {
            throw new System.NotImplementedException();
        }
    }
}

namespace EdFi.Ods.AdminApp.Web.Hubs
{
    public abstract class EdfiOdsHub<T>
    {
        public void SendOperationStatusUpdate(WorkflowStatus status) => throw new System.NotImplementedException();
    }
}

public class StubPbkdf2HmacSha1SecureHasher : ISecureHasher
{
    public PackedHash ComputeHash(string secret, int hashAlgorithm, int iterations, byte[] salt)
        => throw new System.NotImplementedException();

    public PackedHash ComputeHash(string secret, int hashAlgorithm, int iterations, int saltSizeInBytes)
        => throw new System.NotImplementedException();

    public string Algorithm { get => throw new System.NotImplementedException(); }
}

public class HandleErrorInfo
{
    public HandleErrorInfo(Exception exception)
    {
        Exception = exception;
    }

    public Exception Exception { get; }
}
