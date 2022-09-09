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

sed -i -E "s/ENV ADMINAPP_DATABASE_VERSION=\"[0-9]+.[0-9]+.[0-9]+\"/ENV ADMINAPP_DATABASE_VERSION=\"$DB_VERSION\"/w changelog.txt" ods-docker/DB-Admin/Alpine/pgsql/Dockerfile

sed -i -E "s/ENV VERSION=\"[0-9]+.[0-9]+.[0-9]+\"/ENV VERSION=\"$WEB_VERSION\"/w changelog.txt" ods-docker/Web-Ods-AdminApp/Alpine/pgsql/Dockerfile

if [ -s changelog.txt ]; then
    echo "Files updated"
    exit 0
else
    echo "Unable to edit Files"
    exit 1
fi
