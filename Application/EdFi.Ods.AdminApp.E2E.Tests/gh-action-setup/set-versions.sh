#! /bin/bash

NUGET_URL="https://pkgs.dev.azure.com/ed-fi-alliance/Ed-Fi-Alliance-OSS/_packaging/EdFi/nuget/v3/index.json"

WEB="EdFi.Suite3.ODS.AdminApp.Web"
WEB_PACKAGE=$(nuget list $WEB -Source $NUGET_URL)
WEB_VERSION=$(echo $WEB_PACKAGE| cut -d' ' -f 2)
echo "Using $WEB Version: $WEB_VERSION"

DB="EdFi.Suite3.ODS.AdminApp.Database"
DB_PACKAGE=$(nuget list $DB -Source $NUGET_URL)
DB_VERSION=$(echo $DB_PACKAGE| cut -d' ' -f 2)
echo "Using $DB Version: $DB_VERSION"

sed -i "s/ENV VERSION=\"[0-9].[0-9].[0-9]\"/ENV VERSION=\"$WEB_VERSION\"/g" ods-docker/Web-Ods-AdminApp/Alpine/pgsql/Dockerfile

sed -i "s/ENV ADMINAPP_DATABASE_VERSION=\"[0-9].[0-9].[0-9]\"/ENV ADMINAPP_DATABASE_VERSION=\"$DB_VERSION\"/g" ods-docker/DB-Admin/Alpine/pgsql/Dockerfile

# This must be removed once ODS-Docker supports .NET 6.
# These are workarounds to support Alpine 15

sed -i "s/core\/aspnet@sha256:[a-z0-9]\+/aspnet:6.0-alpine/g" ods-docker/Web-Ods-AdminApp/Alpine/pgsql/Dockerfile

sed -i "s/RUN apk --no-cache add unzip=~6.0 dos2unix=~7.4 bash=~5.1 gettext=~0.21 postgresql-client=~13.6 jq=~1.6 icu=~67.1 curl=~7.79/RUN apk --no-cache add unzip=~6.0 dos2unix=~7.4 bash=~5.1 gettext=~0.21 postgresql13-client=~13.6 jq=~1.6 icu=~69.1 curl=~7.80/g" ods-docker/Web-Ods-AdminApp/Alpine/pgsql/Dockerfile
