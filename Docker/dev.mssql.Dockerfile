# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

# First layer uses a dotnet/sdk image to build the Admin API from source code
# Second layer uses the dotnet/aspnet image to run the built code

FROM mcr.microsoft.com/dotnet/sdk:8.0.203-alpine3.19@sha256:b1275049a8fe922cbc9f1d173ffec044664f30b94e99e2c85dd9b7454fbf596c as buildBase
RUN apk --no-cache add curl=~8
FROM buildbase AS publish
WORKDIR /source

COPY --from=assets ./Application/NuGet.Config EdFi.Ods.AdminApi/
COPY --from=assets ./Application/EdFi.Ods.AdminApi EdFi.Ods.AdminApi/


WORKDIR /source/EdFi.Ods.AdminApi
RUN dotnet restore && dotnet build -c Release

RUN dotnet publish -c Release /p:EnvironmentName=Production --no-build -o /app/EdFi.Ods.AdminApi

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine3.19-amd64@sha256:edc046db633d2eac3acfa494c10c6b7b3b9ff9f66f1ed92cec8021f5ee38d755 AS runtimebase
FROM runtimebase AS runtime
RUN apk --no-cache add curl=~8 dos2unix=~7 bash=~5 gettext=~0 icu=~74 && \
    addgroup -S edfi && adduser -S edfi -G edfi

FROM runtime AS setup
LABEL maintainer="Ed-Fi Alliance, LLC and Contributors <techsupport@ed-fi.org>"

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_HTTP_PORTS=80

COPY --chmod=500 Settings/dev/mssql/run.sh /app/run.sh
COPY Settings/dev/log4net.config /app/log4net.txt

WORKDIR /app
COPY --from=publish /app/EdFi.Ods.AdminApi .

RUN cp /app/log4net.txt /app/log4net.config && \
    dos2unix /app/*.json && \
    dos2unix /app/*.sh && \
    dos2unix /app/log4net.config && \
    chmod 500 /app/*.sh -- ** && \
    rm -f /app/log4net.txt && \
    rm -f /app/*.exe && \
    apk del dos2unix && \
    mkdir -p /app && chmod -R 777 /app && \
    chown -R edfi /app  && \
    wget -nv -O /tmp/msodbcsql18_18.4.1.1-1_amd64.apk https://download.microsoft.com/download/7/6/d/76de322a-d860-4894-9945-f0cc5d6a45f8/msodbcsql18_18.4.1.1-1_amd64.apk && \
    wget -nv -O /tmp/mssql-tools18_18.4.1.1-1_amd64.apk https://download.microsoft.com/download/7/6/d/76de322a-d860-4894-9945-f0cc5d6a45f8/mssql-tools18_18.4.1.1-1_amd64.apk && \
    apk --no-cache add --allow-untrusted /tmp/msodbcsql18_18.4.1.1-1_amd64.apk  && \
    apk --no-cache add --allow-untrusted /tmp/mssql-tools18_18.4.1.1-1_amd64.apk

EXPOSE ${ASPNETCORE_HTTP_PORTS}
USER edfi

ENTRYPOINT ["/app/run.sh"]
