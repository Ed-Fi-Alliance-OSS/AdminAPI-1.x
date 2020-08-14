-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

CREATE TABLE adminapp.Users (
    Id VARCHAR(128) NOT NULL,
    Email VARCHAR(256) NULL,
    EmailConfirmed BOOLEAN NOT NULL,
    PasswordHash VARCHAR NULL,
    SecurityStamp VARCHAR NULL,
    PhoneNumber VARCHAR NULL,
    PhoneNumberConfirmed BOOLEAN NOT NULL,
    TwoFactorEnabled BOOLEAN NOT NULL,
    LockoutEndDateUtc TIMESTAMP WITHOUT TIME ZONE NULL,
    LockoutEnabled BOOLEAN NOT NULL,
    AccessFailedCount INT NOT NULL,
    UserName VARCHAR(256) NOT NULL,
    CONSTRAINT PK_Users PRIMARY KEY (Id)
);

CREATE TABLE adminapp.Roles (
    Id VARCHAR(128) NOT NULL,
    Name VARCHAR(256) NOT NULL,
    CONSTRAINT PK_Roles PRIMARY KEY (Id)
);

CREATE TABLE adminapp.UserClaims (
    Id INT NOT NULL GENERATED ALWAYS AS IDENTITY,
    UserId VARCHAR(128) NOT NULL,
    ClaimType VARCHAR NULL,
    ClaimValue VARCHAR NULL,
    CONSTRAINT PK_UserClaims PRIMARY KEY (Id),
    CONSTRAINT FK_UserClaims_Users_Id FOREIGN KEY (UserId) REFERENCES adminapp.Users (Id) ON DELETE CASCADE
);

CREATE TABLE adminapp.UserLogins (
    LoginProvider VARCHAR(128) NOT NULL,
    ProviderKey VARCHAR(128) NOT NULL,
    UserId VARCHAR(128) NOT NULL,
    CONSTRAINT PK_UserLogins PRIMARY KEY (LoginProvider, ProviderKey, UserId),
    CONSTRAINT FK_UserLogins_Users_Id FOREIGN KEY (UserId) REFERENCES adminapp.Users (Id) ON DELETE CASCADE
);

CREATE TABLE adminapp.UserRoles (
    UserId VARCHAR(128) NOT NULL,
    RoleId VARCHAR(128) NOT NULL,
    CONSTRAINT PK_UserRoles PRIMARY KEY (UserId, RoleId),
    CONSTRAINT FK_UserRoles_Roles_Id FOREIGN KEY (RoleId) REFERENCES adminapp.Roles (Id) ON DELETE CASCADE,
    CONSTRAINT FK_UserRoles_Users_Id FOREIGN KEY (UserId) REFERENCES adminapp.Users (Id) ON DELETE CASCADE
);

CREATE UNIQUE INDEX UQ_Roles_Name ON adminapp.Roles (Name);
CREATE INDEX IX_UserClaims_UserId ON adminapp.UserClaims (UserId);
CREATE INDEX IX_UserLogins_UserId ON adminapp.UserLogins (UserId);
CREATE INDEX IX_UserRoles_RoleId ON adminapp.UserRoles (RoleId);
CREATE INDEX IX_UserRoles_UserId ON adminapp.UserRoles (UserId);
CREATE UNIQUE INDEX UQ_Users_UserName ON adminapp.Users (UserName);
