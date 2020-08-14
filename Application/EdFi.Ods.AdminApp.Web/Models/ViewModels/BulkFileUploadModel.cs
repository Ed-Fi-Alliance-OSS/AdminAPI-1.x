// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using EdFi.Ods.AdminApp.Management;
using EdFi.Ods.AdminApp.Web.Infrastructure.IO;

namespace EdFi.Ods.AdminApp.Web.Models.ViewModels
{
    public class BulkFileUploadModel
    {
        public InterchangeFileType BulkFileType { get; set; }
        public IEnumerable<HttpPostedFileBase> BulkFiles { get; set; }
        public CloudOdsEnvironment CloudOdsEnvironment { get; set; }
        public const int MaxFileSize = 20000000;
        public bool CredentialsSaved => !string.IsNullOrEmpty(ApiKey);
        [Display(Name = "Api Key")]
        public string ApiKey { get; set; }
        [Display(Name = "Api Secret")]
        public string ApiSecret { get; set; }
        public bool IsJobRunning { get; set; }
        public bool IsSameOdsInstance { get; set; }
    }
}