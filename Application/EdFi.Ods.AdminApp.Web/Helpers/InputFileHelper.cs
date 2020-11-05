// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.OdsInstances;

namespace EdFi.Ods.AdminApp.Web.Helpers
{
    public static class InputFileHelper
    {
        public static IList<RegisterOdsInstanceModel> DataRecords(Stream stream)
        {
            List<RegisterOdsInstanceModel> records;
            using (var streamReader = new StreamReader(stream))
                using (var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                {
                    csv.Configuration.PrepareHeaderForMatch =
                        (header, index) => header.ToLower();

                    records = csv.GetRecords<RegisterOdsInstanceModel>().ToList();
                }
            return records;
        }

        public static IList<string> MissingHeaders(Stream stream)
        {
            var missingHeaders = new List<string>();

            using (var streamReader = new StreamReader(stream))
                using (var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                {
                    csv.Configuration.PrepareHeaderForMatch =
                        (header, index) => header.ToLower();

                    csv.Configuration.HasHeaderRecord = true;

                    csv.Configuration.HeaderValidated =
                        (isValid, headerNames, headerNameIndex, context) =>
                        {
                            if (!isValid)
                            {
                                missingHeaders.AddRange(headerNames.ToList());
                            }
                        };

                    csv.Read();
                    csv.ReadHeader();
                    csv.ValidateHeader<RegisterOdsInstanceModel>();
                }

            return missingHeaders;
        }
    }
}
