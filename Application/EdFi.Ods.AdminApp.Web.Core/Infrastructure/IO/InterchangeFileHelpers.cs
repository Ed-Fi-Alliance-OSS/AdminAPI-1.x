// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.IO;

namespace EdFi.Ods.AdminApp.Web.Infrastructure.IO
{
    public static class InterchangeFileHelpers
    {
        public static string BuildFileNameForImport(InterchangeFileType interchangeFileType, string fileNameForImport)
        {
            return fileNameForImport.StartsWith(interchangeFileType.InterchangeFileNamePrefix, StringComparison.InvariantCultureIgnoreCase)
                ? $"{Path.GetFileNameWithoutExtension(fileNameForImport)}.xml"
                : $"{interchangeFileType.InterchangeFileNamePrefix}-{Path.GetFileNameWithoutExtension(fileNameForImport)}.xml";
        }
    }
}