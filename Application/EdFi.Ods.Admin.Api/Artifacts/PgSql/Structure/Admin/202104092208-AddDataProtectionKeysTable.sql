-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

CREATE TABLE adminapp.DataProtectionKeys (
    Id INT NOT NULL GENERATED ALWAYS AS IDENTITY,
    FriendlyName VARCHAR NULL,
    Xml VARCHAR NULL,
    CONSTRAINT PK_DataProtectionKeys PRIMARY KEY (Id)
)
