# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

# First layer uses a dotnet/sdk image to build the Admin API from source code
# Second layer uses the dotnet/aspnet image to run the built code


#tag sdk:6.0-alpine
FROM mcr.microsoft.com/dotnet/sdk@sha256:42f4f47d10ee569b4ae0d60f0f078ab4b9a1550dadcdccabecd3741fb82f63d9 AS build
WORKDIR /source

COPY Application/NuGet.Config EdFi.Ods.AdminApi/
COPY Application/EdFi.Ods.AdminApi EdFi.Ods.AdminApi/

WORKDIR /source/EdFi.Ods.AdminApi
RUN dotnet restore && dotnet build -c Release

FROM build AS publish
RUN dotnet publish -c Release /p:EnvironmentName=Production --no-build -o /app/EdFi.Ods.AdminApi

#tag aspnet:6.0-alpine
FROM mcr.microsoft.com/dotnet/aspnet@sha256:64dc2a4bc6e20d759ad80c00e2f9864d4f11c56ae66f398284cdf7c321ab31c5
LABEL maintainer="Ed-Fi Alliance, LLC and Contributors <techsupport@ed-fi.org>"
# Alpine image does not contain Globalization Cultures library so we need to install ICU library to get for LINQ expression to work
# Disable the globaliztion invariant mode (set in base image)
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV ASPNETCORE_ENVIRONMENT Production

COPY Settings/dev/run.sh /app/run.sh
COPY Settings/dev/log4net.config /app/log4net.txt

WORKDIR /app
COPY --from=publish /app/EdFi.Ods.AdminApi .

RUN apk --no-cache add curl=~8 dos2unix=~7 bash=~5 gettext=~0 icu=~72 && \
    cp /app/log4net.txt /app/log4net.config && \
    dos2unix /app/*.json && \
    dos2unix /app/*.sh && \
    dos2unix /app/log4net.config && \
    chmod 700 /app/*.sh -- **

EXPOSE 443

ENTRYPOINT ["/app/run.sh"]
