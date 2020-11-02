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
using EdFi.Ods.AdminApp.Management.ClaimSetEditor;
using EdFi.Ods.AdminApp.Web.Models.ViewModels.OdsInstances;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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

        public static IList<string> MissingHeaders(this HttpPostedFileBase inputFile)
        {
            var missingHeaders = new List<string>();
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
                                    missingHeaders.AddRange(headerNames.ToList());
                                }
                            };

                        csv.Read();
                        csv.ReadHeader();
                        csv.ValidateHeader<RegisterOdsInstanceModel>();
                    }

            return missingHeaders;
        }

        public static string SerializeFromSharingModel(SharingModel model)
        {
            return JsonConvert.SerializeObject(model, UsingIndentedCamelCase());
        }

        public static SharingModel DeserializeToSharingModel(Stream jsonStream)
        {
            using (var streamReader = new StreamReader(jsonStream))
                using (JsonReader jsonReader = new JsonTextReader(streamReader))
                {
                    return JsonSerializer
                        .Create(UsingIndentedCamelCase())
                        .Deserialize<SharingModel>(jsonReader);
                }
        }

        private static JsonSerializerSettings UsingIndentedCamelCase()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };
        }

    }
}
