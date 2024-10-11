# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine3.19-amd64@sha256:edc046db633d2eac3acfa494c10c6b7b3b9ff9f66f1ed92cec8021f5ee38d755 as base
RUN apk --no-cache add curl=~8 unzip=~6 dos2unix=~7 bash=~5 gettext=~0 jq=~1 icu=~74 && \

    addgroup -S edfi && adduser -S edfi -G edfi

FROM base as build

LABEL maintainer="Ed-Fi Alliance, LLC and Contributors <techsupport@ed-fi.org>"
ARG DB="mssql"

ARG VERSION=latest
ARG ADMIN_API_VERSION="1.4.2"
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV ASPNETCORE_HTTP_PORTS=80

WORKDIR /app

COPY --chmod=600 Settings/"${DB}"/appsettings.template.json /app/appsettings.template.json
COPY --chmod=500 Settings/"${DB}"/run.sh /app/run.sh
COPY Settings/"${DB}"/log4net.config /app/log4net.txt

RUN umask 0077 && \
	wget -nv -O /tmp/msodbcsql18_18.4.1.1-1_amd64.apk https://download.microsoft.com/download/7/6/d/76de322a-d860-4894-9945-f0cc5d6a45f8/msodbcsql18_18.4.1.1-1_amd64.apk && \
    wget -nv -O /tmp/mssql-tools18_18.4.1.1-1_amd64.apk https://download.microsoft.com/download/7/6/d/76de322a-d860-4894-9945-f0cc5d6a45f8/mssql-tools18_18.4.1.1-1_amd64.apk && \
    apk --no-cache add --allow-untrusted /tmp/msodbcsql18_18.4.1.1-1_amd64.apk  && \
    apk --no-cache add --allow-untrusted /tmp/mssql-tools18_18.4.1.1-1_amd64.apk && \
    wget -nv -O /app/AdminApi.zip "https://pkgs.dev.azure.com/ed-fi-alliance/Ed-Fi-Alliance-OSS/_apis/packaging/feeds/EdFi/nuget/packages/EdFi.Suite3.ODS.AdminApi/versions/${ADMIN_API_VERSION}/content" && \
    unzip /app/AdminApi.zip AdminApi/* -d /app/ && \
    cp -r /app/AdminApi/. /app/ && \
    rm -f /app/AdminApi.zip && \
    rm -r /app/AdminApi && \
    cp /app/log4net.txt /app/log4net.config && \
    dos2unix /app/*.json && \
    dos2unix /app/*.sh && \
    dos2unix /app/log4net.config && \
	chmod 700 /app/*.sh -- ** && \
    rm -f /app/*.exe && \
    apk del unzip dos2unix curl && \
    chown -R edfi /app

EXPOSE ${ASPNETCORE_HTTP_PORTS}
USER edfi

ENTRYPOINT [ "/app/run.sh" ]
