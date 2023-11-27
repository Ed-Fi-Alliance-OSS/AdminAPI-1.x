# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

FROM edfialliance/ods-api-db-admin:v2.3.1
LABEL maintainer="Ed-Fi Alliance, LLC and Contributors <techsupport@ed-fi.org>"

ARG POSTGRES_USER=postgres
ARG POSTGRES_PASSWORD=abcdefgh1!

ENV POSTGRES_USER=${POSTGRES_USER}
ENV POSTGRES_PASSWORD=${POSTGRES_PASSWORD}
ENV POSTGRES_DB=postgres

COPY Settings/DB-Admin/pgsql/run-adminapi-migrations.sh /docker-entrypoint-initdb.d/3-run-adminapi-migrations.sh
COPY Application/EdFi.Ods.AdminApi/Artifacts/PgSql/Structure/Admin/ /tmp/AdminApiScripts/PgSql

RUN apk --no-cache add dos2unix=~7.4 unzip=~6.0 && \
    dos2unix /docker-entrypoint-initdb.d/3-run-adminapi-migrations.sh && \
    dos2unix /tmp/AdminApiScripts/PgSql/* && \
    chmod -R 777 /tmp/AdminApiScripts/PgSql/*

EXPOSE 5432

CMD ["docker-entrypoint.sh", "postgres"]
