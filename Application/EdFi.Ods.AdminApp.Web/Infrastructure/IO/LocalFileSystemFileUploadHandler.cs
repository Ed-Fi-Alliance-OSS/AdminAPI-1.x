// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace EdFi.Ods.AdminApp.Web.Infrastructure.IO
{
    public class LocalFileSystemFileUploadHandler : IFileUploadHandler
    {
        public string GetNewUploadDirectory(IWebHostEnvironment environment)
        {
            const string baseUploadDirectory = "uploads";
            var sortableDatetimeString = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            var directoryName = GetSafeFilename($"{sortableDatetimeString}");
            var uploadDirectoryPath = Path.Combine(baseUploadDirectory, directoryName);
            var fullPath = Path.Combine(environment.ContentRootPath, uploadDirectoryPath);
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
            if (!Directory.Exists(customDirectory))
            {
                Directory.CreateDirectory(customDirectory);
            }
            return customDirectory;
        }

        public string GetUploadFilePath(string directory, string fileName)
        {
            fileName = GetSafeFilename(fileName);
            return Path.Combine(directory, fileName);
        }
        
        public FileUploadResult SaveFileToUploadDirectory(IFormFile uploadedFile, IWebHostEnvironment environment)
        {
            return SaveFileToUploadDirectory(uploadedFile, uploadedFile.FileName, environment);
        }

        public FileUploadResult SaveFileToUploadDirectory(IFormFile uploadedFile, string fileName, IWebHostEnvironment environment)
        {
            var uploadDirectory = GetNewUploadDirectory(environment);
            var uploadedFilePath = GetUploadFilePath(uploadDirectory, fileName);

            SaveFileToPath(uploadedFilePath, uploadedFile);

            return new FileUploadResult
            {
                Directory = uploadDirectory,
                FileNames = new[] {uploadedFilePath}
            };
        }

        public FileUploadResult SaveFilesToUploadDirectory(IFormFile[] uploadedFiles, IWebHostEnvironment environment)
        {
            return SaveFilesToUploadDirectory(uploadedFiles, filename => filename, environment);
        }

        public FileUploadResult SaveFilesToUploadDirectory(IFormFile[] uploadedFiles, Func<string, string> fileNameTransformFunc, IWebHostEnvironment environment)
        {
            if (uploadedFiles == null || uploadedFiles.Length == 0) return null;

            var savedFiles = new List<string>(uploadedFiles.Length);

            var uploadDirectory = GetNewUploadDirectory(environment);
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

        private static void SaveFileToPath(string destinationPath, IFormFile uploadedFile)
        {
            if (uploadedFile.Length > 0)
            {
                using (Stream fileStream = new FileStream(destinationPath, FileMode.Create))
                {
                    uploadedFile.CopyTo(fileStream);
                }
            }
        }

        public static string GetSafeFilename(string filename)
        {
            return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
        }
    }
}
