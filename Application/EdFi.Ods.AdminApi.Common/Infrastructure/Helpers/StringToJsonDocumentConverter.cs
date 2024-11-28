// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EdFi.Ods.AdminApi.Common.Helpers
{
    public class StringToJsonDocumentConverter : JsonConverter<string>
    {
#pragma warning disable CS8603 // Possible null reference return.
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.GetString();
#pragma warning restore CS8603 // Possible null reference return.

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            JsonDocument jsonDocument = JsonDocument.Parse(value);
            jsonDocument.WriteTo(writer);
        }
    }
}
