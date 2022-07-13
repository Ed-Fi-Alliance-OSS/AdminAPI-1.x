#!/bin/bash

# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

set -e
set -x

PARENT="localhost"
openssl req -x509 -newkey rsa:4096 -sha256 -days 365 -nodes -keyout $PARENT.key -out $PARENT.crt -subj "//CN=${PARENT}" -extensions v3_ca -extensions v3_req -config openssl.config
openssl x509 -noout -text -in $PARENT.crt
winpty openssl pkcs12 -export -out $PARENT.pfx -inkey $PARENT.key -in $PARENT.crt
