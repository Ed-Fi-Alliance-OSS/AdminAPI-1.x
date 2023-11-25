# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

#tag 6.0-alpine
FROM mcr.microsoft.com/dotnet/aspnet@sha256:201cedd60cb295b2ebea7184561a45c5c0ee337e37300ea0f25cff5a2c762538
LABEL maintainer="Ed-Fi Alliance, LLC and Contributors <techsupport@ed-fi.org>"
ARG VERSION=latest
ARG DB=pgsql

# Alpine image does not contain Globalization Cultures library so we need to install ICU library to get for LINQ expression to work
# Disable the globaliztion invariant mode (set in base image)
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

WORKDIR /app

COPY Settings/"${DB}"/appsettings.template.json /app/appsettings.template.json
COPY Settings/"${DB}"/run.sh /app/run.sh
COPY Settings/"${DB}"/log4net.config /app/log4net.txt

RUN apk --no-cache add curl=~8 unzip=~6 dos2unix=~7 bash=~5 gettext=~0 jq=~1 icu=~72 && \
    if [ "$DB" == "pgsql" ]; then apk --no-cache add postgresql13-client=~13; fi && \
    wget -nv -O /app/AdminApi.zip https://pkgs.dev.azure.com/ed-fi-alliance/Ed-Fi-Alliance-OSS/_apis/packaging/feeds/EdFi/nuget/packages/EdFi.Suite3.ODS.AdminApi/versions/${VERSION}/content && \
    unzip /app/AdminApi.zip AdminApi/* -d /app/ && \
    cp -r /app/AdminApi/. /app/ && \
    rm -f /app/AdminApi.zip && \
    rm -r /app/AdminApi && \
    cp /app/log4net.txt /app/log4net.config && \
    dos2unix /app/*.json && \
    dos2unix /app/*.sh && \
    dos2unix /app/log4net.config && \
    chmod 700 /app/*.sh -- ** && \
    rm -f /app/*.exe

EXPOSE 443

ENTRYPOINT [ "/app/run.sh" ]
