# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

# First two layers use a dotnet/sdk image to build the Admin API from source
# code. The next two layers use the dotnet/aspnet image to run the built code.
# The extra layers in the middle support caching of base layers.

FROM mcr.microsoft.com/dotnet/sdk:8.0.403-alpine3.20@sha256:07cb8622ca6c4d7600b42b2eccba968dff4b37d41b43a9bf4bd800aa02fab117 AS buildBase
RUN apk --no-cache add curl=~8

# hadolint ignore=DL3006
FROM buildbase AS publish
WORKDIR /source
COPY Application/NuGet.Config EdFi.Ods.AdminApi/
COPY Application/EdFi.Ods.AdminApi EdFi.Ods.AdminApi/

WORKDIR /source/EdFi.Ods.AdminApi
RUN dotnet restore && dotnet build -c Release
RUN dotnet publish -c Release /p:EnvironmentName=Production --no-build -o /app/EdFi.Ods.AdminApi

##
FROM mcr.microsoft.com/dotnet/aspnet:8.0.10-alpine3.20-amd64@sha256:1659f678b93c82db5b42fb1fb12d98035ce482b85747c2c54e514756fa241095 AS runtime
RUN apk --no-cache add curl=~8 dos2unix=~7 bash=~5 gettext=~0 icu=~74 && \
    addgroup -S edfi && adduser -S edfi -G edfi

# hadolint ignore=DL3006
FROM runtime AS setup
LABEL maintainer="Ed-Fi Alliance, LLC and Contributors <techsupport@ed-fi.org>"

# Alpine image does not contain Globalization Cultures library so we need to install ICU library to get for LINQ expression to work
# Disable the globaliztion invariant mode (set in base image)
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_HTTP_PORTS=80
ENV DB_FOLDER=pgsql

WORKDIR /app
COPY --from=publish /app/EdFi.Ods.AdminApi .

COPY --chmod=500 Settings/dev/${DB_FOLDER}/run.sh /app/run.sh
COPY Settings/dev/log4net.config /app/log4net.txt

RUN cp /app/log4net.txt /app/log4net.config && \
    dos2unix /app/*.json && \
    dos2unix /app/*.sh && \
    dos2unix /app/log4net.config && \
    chmod 500 /app/*.sh -- ** && \
    chown -R edfi /app

EXPOSE ${ASPNETCORE_HTTP_PORTS}
USER edfi

ENTRYPOINT ["/app/run.sh"]
