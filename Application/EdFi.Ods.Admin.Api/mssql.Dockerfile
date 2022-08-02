# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

#tag 6.0-alpine 
FROM mcr.microsoft.com/dotnet/aspnet@sha256:5d7911e8485a58ac50eefa09e2cea8f3d59268fd7f1501f72324e37e29d9d6ee
LABEL maintainer="Ed-Fi Alliance, LLC and Contributors <techsupport@ed-fi.org>"
ENV VERSION="1.0.20"

# Alpine image does not contain Globalization Cultures library so we need to install ICU library to get for LINQ expression to work
# Disable the globaliztion invariant mode (set in base image)
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

WORKDIR /app

COPY ./Docker/mssql/appsettings.template.json /app/appsettings.template.json
COPY ./Docker/mssql/run.sh /app/run.sh
COPY ./Docker/mssql/log4net.config /app/log4net.txt

RUN apk --no-cache add curl unzip=~6.0 dos2unix=~7.4 bash=~5.1 gettext=~0.21 postgresql-client=~13.7-r0 jq=~1.6 icu=~67.1 gcompat && \
    wget -O /app/AdminApi.zip https://pkgs.dev.azure.com/ed-fi-alliance/Ed-Fi-Alliance-OSS/_apis/packaging/feeds/EdFi/nuget/packages/EdFi.Suite3.ODS.Admin.Api/versions/${VERSION}/content && \
    unzip /app/AdminApi.zip -d /app && \
    rm -f /app/AdminApi.zip && \
    cp /app/log4net.txt /app/log4net.config && \
    dos2unix /app/*.json && \
    dos2unix /app/*.sh && \
    dos2unix /app/log4net.config && \
    chmod 700 /app/*.sh -- ** && \
    rm -f /app/*.exe

EXPOSE 443

ENTRYPOINT [ "/app/run.sh" ]