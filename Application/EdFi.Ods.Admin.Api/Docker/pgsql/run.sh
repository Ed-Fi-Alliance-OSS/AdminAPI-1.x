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

if [[ -z "$ODS_WAIT_POSTGRES_HOSTS" ]]; then
  # if there are no hosts to wait then fallback to $ODS_POSTGRES_HOST
  export ODS_WAIT_POSTGRES_HOSTS=$ODS_POSTGRES_HOST
fi

export ODS_WAIT_POSTGRES_HOSTS_ARR=($ODS_WAIT_POSTGRES_HOSTS) 
for HOST in ${ODS_WAIT_POSTGRES_HOSTS_ARR[@]}
do
  until PGPASSWORD=$POSTGRES_PASSWORD psql -h $HOST -p $POSTGRES_PORT -U $POSTGRES_USER -c '\q';
  do
    >&2 echo "ODS Postgres '$HOST' is unavailable - sleeping"
    sleep 10
  done
done

until PGPASSWORD=$POSTGRES_PASSWORD psql -h $ADMIN_POSTGRES_HOST -p $POSTGRES_PORT -U $POSTGRES_USER -c '\q';
do
  >&2 echo "Admin Postgres is unavailable - sleeping"
  sleep 10
done

>&2 echo "Postgres is up - executing command"
exec $cmd

dotnet EdFi.Ods.Admin.Api.dll