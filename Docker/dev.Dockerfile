# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

# First two layers use a dotnet/sdk image to build the Admin API from source
# code. The next two layers use the dotnet/aspnet image to run the built code.
# The extra layers in the middle support caching of base layers.

FROM mcr.microsoft.com/dotnet/sdk:8.0.203-alpine3.18@sha256:2a8dca3af111071172b1629c12eefaeca0d6c2954887c4489195771c9e90833c as buildBase

FROM buildbase as build
RUN apk --no-cache add curl=~8

FROM build AS publish
WORKDIR /source
COPY Application/NuGet.Config EdFi.Ods.AdminApi/
COPY Application/EdFi.Ods.AdminApi EdFi.Ods.AdminApi/

WORKDIR /source/EdFi.Ods.AdminApi
RUN dotnet restore && dotnet build -c Release
RUN dotnet publish -c Release /p:EnvironmentName=Production --no-build -o /app/EdFi.Ods.AdminApi

# TODO: update to .NET 8, will be handled in AdminAPI-983
#tag aspnet:6.0-alpine
FROM mcr.microsoft.com/dotnet/aspnet@sha256:201cedd60cb295b2ebea7184561a45c5c0ee337e37300ea0f25cff5a2c762538 AS runtimebase

FROM runtimebase AS runtime
RUN apk --no-cache add curl=~8 dos2unix=~7 bash=~5 gettext=~0 icu=~72 && \
    addgroup -S edfi && adduser -S edfi -G edfi

FROM runtime AS setup
LABEL maintainer="Ed-Fi Alliance, LLC and Contributors <techsupport@ed-fi.org>"
# Alpine image does not contain Globalization Cultures library so we need to install ICU library to get for LINQ expression to work
# Disable the globaliztion invariant mode (set in base image)
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV ASPNETCORE_ENVIRONMENT=Production

WORKDIR /app
COPY --from=publish /app/EdFi.Ods.AdminApi .

COPY --chmod=500 Settings/dev/run.sh /app/run.sh
COPY Settings/dev/log4net.config /app/log4net.txt

RUN cp /app/log4net.txt /app/log4net.config && \
    dos2unix /app/*.json && \
    dos2unix /app/*.sh && \
    dos2unix /app/log4net.config && \
    chmod 500 /app/*.sh -- ** && \
    chown -R edfi /app

EXPOSE 443
USER edfi
WORKDIR /app

ENTRYPOINT ["/app/run.sh"]
