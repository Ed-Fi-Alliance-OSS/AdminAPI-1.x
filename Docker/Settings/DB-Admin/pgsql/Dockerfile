# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

#edfialliance/ods-api-db-admin:7.1
FROM edfialliance/ods-api-db-admin:7.2.5570-5.1.0@sha256:53a9c8bbed76075bc438e63ba9579a48f1539bf7d5a3ee1b5171739c7e496ab0 AS base
LABEL maintainer="Ed-Fi Alliance, LLC and Contributors <techsupport@ed-fi.org>"

ENV POSTGRES_USER=${POSTGRES_USER}
ENV POSTGRES_PASSWORD=${POSTGRES_PASSWORD}
ENV POSTGRES_DB=postgres

ARG VERSION=latest

USER root
COPY run-adminapi-migrations.sh /docker-entrypoint-initdb.d/3-run-adminapi-migrations.sh
RUN apk --upgrade --no-cache add dos2unix=~7 unzip=~6
USER postgres

FROM base AS setup

USER root
RUN wget -nv -O /tmp/EdFi_AdminApi_Scripts.zip "https://pkgs.dev.azure.com/ed-fi-alliance/Ed-Fi-Alliance-OSS/_apis/packaging/feeds/EdFi/nuget/packages/EdFi.Suite3.ODS.AdminApi/versions/${VERSION}/content" && \
    unzip /tmp/EdFi_AdminApi_Scripts.zip AdminApi/Artifacts/PgSql/Structure/Admin/* -d /tmp/AdminApiScripts/ && \
    cp -r /tmp/AdminApiScripts/AdminApi/Artifacts/PgSql/Structure/Admin/. /tmp/AdminApiScripts/PgSql/ && \
    rm -f /tmp/EdFi_AdminApi_Scripts.zip && \
    rm -r /tmp/AdminApiScripts/AdminApi && \
    dos2unix /docker-entrypoint-initdb.d/3-run-adminapi-migrations.sh && \
    dos2unix /tmp/AdminApiScripts/PgSql/* && \
    chmod -R 777 /tmp/AdminApiScripts/PgSql
USER postgres

EXPOSE 5432

CMD ["docker-entrypoint.sh", "postgres"]
