// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.OdsInstances;

namespace EdFi.Ods.AdminApp.Web.Helpers
{
    public static class InputFileHelper
    {
        public static IList<RegisterOdsInstanceModel> DataRecords(Stream stream, out IList<string> missingHeaders)
        {
            var localMissingHeaders = new List<string>();
            List<RegisterOdsInstanceModel> records = new List<RegisterOdsInstanceModel>();

            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = headerArgs => headerArgs.Header.ToLower(),
                HasHeaderRecord = true,
                HeaderValidated = (headerArgs) =>
                {
                    if (!headerArgs.InvalidHeaders.Any())
                    {
                        return;
                    }

                    foreach (var invalidHeader in headerArgs.InvalidHeaders)
                    {
                        localMissingHeaders.AddRange(invalidHeader.Names.ToList());
                    }
                }
            };

            using (var streamReader = new StreamReader(stream))
                using (var csv = new CsvReader(streamReader, configuration))
                {
                    csv.Read();
                    csv.ReadHeader();
                    csv.ValidateHeader<RegisterOdsInstanceModel>();

                    if (!localMissingHeaders.Any())
                        records.AddRange(csv.GetRecords<RegisterOdsInstanceModel>());
                }

            missingHeaders = localMissingHeaders;
            return records;
        }
    }
}
