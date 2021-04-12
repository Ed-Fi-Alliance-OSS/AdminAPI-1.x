-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

CREATE TABLE adminapp.XmlKeys (
    Id INT NOT NULL GENERATED ALWAYS AS IDENTITY,
    KeyXmlContent VARCHAR NOT NULL,
    CONSTRAINT PK_XmlKeys PRIMARY KEY (Id)
)
