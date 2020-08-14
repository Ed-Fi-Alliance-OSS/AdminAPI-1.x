// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Web.Infrastructure.IO;
using EdFi.Ods.AdminApp.Web.Infrastructure.Jobs;
using EdFi.Ods.AdminApp.Web.Models.ViewModels;

namespace EdFi.Ods.AdminApp.Web.Controllers
{
    public class BulkUploadController : ControllerBase
    {
        private readonly IFileUploadHandler _fileUploadHandler;
        private readonly BulkUploadJob _bulkUploadJob;

        public BulkUploadController(IFileUploadHandler fileUploadHandler, BulkUploadJob bulkUploadJob)
        {
            _fileUploadHandler = fileUploadHandler;
            _bulkUploadJob = bulkUploadJob;
        }

        [ChildActionOnly]
        public PartialViewResult BulkUploadForm(CloudOdsEnvironment environment)
        {
            return PartialView("_BulkUploadForm", new BulkFileUploadModel { CloudOdsEnvironment = environment });
        }

        [HttpPost]
        public ActionResult BulkFileUpload(BulkFileUploadModel model)
        {
            var bulkFiles = model.BulkFiles.Where(file => file != null && file.ContentLength > 0).ToArray();

            if (!bulkFiles.Any())
            {
                return new HttpStatusCodeResult(HttpStatusCode.NoContent);
            }

            if (bulkFiles.Sum(f => f.ContentLength) > BulkFileUploadModel.MaxFileSize)
            {
                throw new Exception($"Upload exceeds maximum limit of {BulkFileUploadModel.MaxFileSize} bytes");
            }

            if (bulkFiles.Length > 1)
            {
                throw new Exception("Currently, the bulk import process only supports a single file at a time");
            }

            var uploadedFiles = _fileUploadHandler.SaveFilesToUploadDirectory(bulkFiles, fileName => InterchangeFileHelpers.BuildFileNameForImport(model.BulkFileType, fileName));

            var jobContext = new BulkUploadJobContext
            {
                DataDirectoryFullPath = uploadedFiles.Directory
            };
            _bulkUploadJob.EnqueueJob(jobContext);

            return new HttpStatusCodeResult(HttpStatusCode.Accepted);
        }
    }
}