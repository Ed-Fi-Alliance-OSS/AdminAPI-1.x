// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

//This file contains temporary type declarations in service of net48/netcoreapp3.1 cross-compilation.
//These types are intended to be removed as they are ported in full to .NET Core.

using System;
using EdFi.Ods.AdminApp.Management.Workflow;
using EdFi.Ods.AdminApp.Web.Infrastructure.Jobs;
using Microsoft.AspNetCore.Http;

namespace EdFi.Ods.AdminApp.Web.Infrastructure.IO
{
    public class LocalFileSystemFileUploadHandler : IFileUploadHandler
    {
        public FileUploadResult SaveFileToUploadDirectory(IFormFile uploadedFile) => throw new NotImplementedException();
        public FileUploadResult SaveFileToUploadDirectory(IFormFile uploadedFile, string fileName) => throw new NotImplementedException();
        public FileUploadResult SaveFilesToUploadDirectory(IFormFile[] uploadedFiles) => throw new NotImplementedException();
        public FileUploadResult SaveFilesToUploadDirectory(IFormFile[] uploadedFiles, Func<string, string> fileNameTransformFunc) => throw new NotImplementedException();
        public string GetNewTempDirectory() => throw new NotImplementedException();
        public string GetWorkingDirectory(string customDirectory) => throw new NotImplementedException();
    }
}

namespace EdFi.Ods.AdminApp.Web.Hubs
{
    public abstract class EdfiOdsHub<T>
    {
        public void SendOperationStatusUpdate(WorkflowStatus status) => throw new System.NotImplementedException();
    }
}

public class HandleErrorInfo
{
    public HandleErrorInfo(Exception exception)
    {
        Exception = exception;
    }

    public Exception Exception { get; }
}
