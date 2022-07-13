#!/bin/bash
# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

set -e
set -x

envsubst < /app/AdminApi/appsettings.template.json > /app/AdminApi/temp.json

measurementId=`jq -r '.AppSettings.GoogleAnalyticsMeasurementId' /app/AdminApi/appsettings.json`

tmp=$(mktemp)
jq --arg variable "$measurementId" '.AppSettings.GoogleAnalyticsMeasurementId = $variable' /app/AdminApi/temp.json > "$tmp" && mv "$tmp" /app/AdminApi/temp.json

mv /app/AdminApi/temp.json /app/AdminApi/appsettings.json

dotnet EdFi.Ods.Admin.Api.dll