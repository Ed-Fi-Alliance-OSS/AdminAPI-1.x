// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
#if NET48
using System.Web;
#else
using Microsoft.AspNetCore.Http;
#endif

namespace EdFi.Ods.AdminApp.Web.Infrastructure.IO
{
    public interface IFileUploadHandler
    {
#if NET48
        FileUploadResult SaveFileToUploadDirectory(HttpPostedFileBase uploadedFile);
        FileUploadResult SaveFileToUploadDirectory(HttpPostedFileBase uploadedFile, string fileName);
        FileUploadResult SaveFilesToUploadDirectory(HttpPostedFileBase[] uploadedFiles);
        FileUploadResult SaveFilesToUploadDirectory(HttpPostedFileBase[] uploadedFiles, Func<string, string> fileNameTransformFunc);
#else
        FileUploadResult SaveFileToUploadDirectory(IFormFile uploadedFile);
        FileUploadResult SaveFileToUploadDirectory(IFormFile uploadedFile, string fileName);
        FileUploadResult SaveFilesToUploadDirectory(IFormFile[] uploadedFiles);
        FileUploadResult SaveFilesToUploadDirectory(IFormFile[] uploadedFiles, Func<string, string> fileNameTransformFunc);
#endif

        string GetNewTempDirectory();
        string GetWorkingDirectory(string customDirectory);
    }

    public class FileUploadResult
    {
        public string Directory { get; set; }
        public string[] FileNames { get; set; }
    }
}
