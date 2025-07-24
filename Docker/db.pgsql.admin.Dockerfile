# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

# Base image for the Ed-Fi ODS/API 6.2 Admin database setup
FROM edfialliance/ods-api-db-admin:v2.3.5 as base
LABEL maintainer="Ed-Fi Alliance, LLC and Contributors <techsupport@ed-fi.org>"

ENV POSTGRES_USER=${POSTGRES_USER}
ENV POSTGRES_PASSWORD=${POSTGRES_PASSWORD}
ENV POSTGRES_DB=postgres

USER root
COPY Application/EdFi.Ods.AdminApi/Artifacts/PgSql/Structure/Admin/ /tmp/AdminApiScripts/PgSql
COPY Settings/DB-Admin/pgsql/run-adminapi-migrations.sh /docker-entrypoint-initdb.d/3-run-adminapi-migrations.sh

RUN apk upgrade --no-cache && apk add dos2unix=~7 unzip=~6 openssl 'musl>=1.2.4_git20230717-r5'
USER postgres

FROM base AS setup

USER root
RUN dos2unix /docker-entrypoint-initdb.d/3-run-adminapi-migrations.sh && \
    dos2unix /tmp/AdminApiScripts/PgSql/* && \
    chmod -R 777 /tmp/AdminApiScripts/PgSql/*
USER postgres

EXPOSE 5432

CMD ["docker-entrypoint.sh", "postgres"]
