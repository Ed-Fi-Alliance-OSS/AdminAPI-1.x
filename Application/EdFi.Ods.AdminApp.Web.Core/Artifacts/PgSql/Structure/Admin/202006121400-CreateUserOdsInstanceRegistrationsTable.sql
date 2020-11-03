-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

CREATE TABLE adminapp.UserOdsInstanceRegistrations (
    UserId VARCHAR(225) NOT NULL,
    OdsInstanceRegistrationId INT NOT NULL,
    CONSTRAINT PK_UserOdsInstanceRegistrations PRIMARY KEY (UserId, OdsInstanceRegistrationId),
    CONSTRAINT FK_UserOdsInstanceRegistrations_OdsInstanceRegistrations_Id FOREIGN KEY (OdsInstanceRegistrationId) REFERENCES adminapp.OdsInstanceRegistrations (Id) ON DELETE CASCADE,
    CONSTRAINT FK_UserOdsInstanceRegistrations_Users_Id FOREIGN KEY (UserId) REFERENCES adminapp.Users (Id) ON DELETE CASCADE
);