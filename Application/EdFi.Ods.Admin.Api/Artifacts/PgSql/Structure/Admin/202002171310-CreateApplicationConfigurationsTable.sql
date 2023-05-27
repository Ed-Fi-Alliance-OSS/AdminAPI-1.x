-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

CREATE TABLE adminapp.ApplicationConfigurations (
    Id INT NOT NULL GENERATED ALWAYS AS IDENTITY,
    AllowUserRegistration BOOLEAN NOT NULL,
    CONSTRAINT PK_ApplicationConfigurations PRIMARY KEY (Id)
)