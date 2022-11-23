#!/bin/bash
# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

set -e
set -x

envsubst < /app/appsettings.Docker.json > /app/temp.json

mv /app/temp.json /app/appsettings.json

cp /ssl/server.crt /usr/local/share/ca-certificates/
update-ca-certificates

dotnet EdFi.Ods.Admin.Api.dll