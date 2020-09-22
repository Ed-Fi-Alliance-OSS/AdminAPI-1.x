// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using CsvHelper;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.OdsInstances;

namespace EdFi.Ods.AdminApp.Web.Helpers
{
    public static class InputFileExtensions
    {
        public static IList<RegisterOdsInstanceModel> DataRecords(this HttpPostedFileBase inputFile)
        {
            List<RegisterOdsInstanceModel> records;
            using (var stream = GetMemoryStream(inputFile))
                using (var streamReader = new StreamReader(stream))
                    using (var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                    {
                        csv.Configuration.PrepareHeaderForMatch =
                            (header, index) => header.ToLower();

                        records = csv.GetRecords<RegisterOdsInstanceModel>().ToList();
                    }

            return records;
        }

        private static MemoryStream GetMemoryStream(HttpPostedFileBase inputFile)
        {
            var streamCopy = new MemoryStream();
            inputFile.InputStream.CopyTo(streamCopy);
            inputFile.InputStream.Position = streamCopy.Position = 0;
            return streamCopy;
        }

        public static string[] MissingHeaders(this HttpPostedFileBase inputFile)
        {
            string[] missingHeaders = null;
            using (var stream = GetMemoryStream(inputFile))
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
                                    missingHeaders = headerNames;
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
