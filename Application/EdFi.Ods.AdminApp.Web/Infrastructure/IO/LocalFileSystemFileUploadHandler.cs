// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

#if !NET48
using System;
using Microsoft.AspNetCore.Http;

namespace EdFi.Ods.AdminApp.Web.Infrastructure.IO
{
    //Temporary stub allowing for compilation, but expected to fail at runtime.
    //Port the NET48 code below and then remove this stub.
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
#else
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Hosting;

namespace EdFi.Ods.AdminApp.Web.Infrastructure.IO
{
    public class LocalFileSystemFileUploadHandler : IFileUploadHandler
    {
        public string GetNewUploadDirectory()
        {
            const string baseUploadDirectory = "~/uploads/";
            var sortableDatetimeString = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            var directoryName = GetSafeFilename($"{sortableDatetimeString}");
            var relativePath = Path.Combine(baseUploadDirectory, directoryName);

            var fullPath = HostingEnvironment.MapPath(relativePath);
            Directory.CreateDirectory(fullPath);

            return fullPath;
        }

        public string GetNewTempDirectory()
        {
            var sortableDatetimeString = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            var directoryName = GetSafeFilename($"[Temp]-{sortableDatetimeString}");
            var fullPath = Path.Combine(Path.GetTempPath(), directoryName);
            Directory.CreateDirectory(fullPath);
            return fullPath;
        }

        public string GetWorkingDirectory(string customDirectory)
        {
            var parentFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var workingDirectory = $"{parentFolder}\\{customDirectory}";
            if (string.IsNullOrEmpty(parentFolder) || !Directory.Exists(parentFolder)) return string.Empty;
            if (!Directory.Exists(workingDirectory))
            {
                Directory.CreateDirectory(workingDirectory);
            }
            return workingDirectory;
        }

        public string GetUploadFilePath(string directory, string fileName)
        {
            fileName = GetSafeFilename(fileName);
            return Path.Combine(directory, fileName);
        }
        
        public FileUploadResult SaveFileToUploadDirectory(HttpPostedFileBase uploadedFile)
        {
            return SaveFileToUploadDirectory(uploadedFile, uploadedFile.FileName);
        }
        
        public FileUploadResult SaveFileToUploadDirectory(HttpPostedFileBase uploadedFile, string fileName)
        {
            var uploadDirectory = GetNewUploadDirectory();
            var uploadedFilePath = GetUploadFilePath(uploadDirectory, fileName);

            SaveFileToPath(uploadedFilePath, uploadedFile);

            return new FileUploadResult
            {
                Directory = uploadDirectory,
                FileNames = new[] {uploadedFilePath}
            };
        }

        public FileUploadResult SaveFilesToUploadDirectory(HttpPostedFileBase[] uploadedFiles)
        {
            return SaveFilesToUploadDirectory(uploadedFiles, filename => filename);
        }

        public FileUploadResult SaveFilesToUploadDirectory(HttpPostedFileBase[] uploadedFiles, Func<string, string> fileNameTransformFunc)
        {
            if (uploadedFiles == null || uploadedFiles.Length == 0) return null;

            var savedFiles = new List<string>(uploadedFiles.Length);

            var uploadDirectory = GetNewUploadDirectory();
            foreach (var uploadedFile in uploadedFiles)
            {
                var uploadFilePath = GetUploadFilePath(uploadDirectory, fileNameTransformFunc(uploadedFile.FileName));
                SaveFileToPath(uploadFilePath, uploadedFile);

                savedFiles.Add(uploadFilePath);
            }

            return new FileUploadResult
            {
                Directory = uploadDirectory,
                FileNames = savedFiles.ToArray()
            };
        }

        private static void SaveFileToPath(string destinationPath, HttpPostedFileBase uploadedFile)
        {
            uploadedFile.SaveAs(destinationPath);
        }

        public static string GetSafeFilename(string filename)
        {
            return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
        }
    }
}
#endif
