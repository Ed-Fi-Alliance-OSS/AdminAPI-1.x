-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

CREATE TABLE adminapp.Users (
    Id NVARCHAR(128) NOT NULL,
    Email NVARCHAR(256) NULL,
    EmailConfirmed BIT NOT NULL,
    PasswordHash NVARCHAR(MAX) NULL,
    SecurityStamp NVARCHAR(MAX) NULL,
    PhoneNumber NVARCHAR(MAX) NULL,
    PhoneNumberConfirmed BIT NOT NULL,
    TwoFactorEnabled BIT NOT NULL,
    LockoutEndDateUtc DATETIME NULL,
    LockoutEnabled BIT NOT NULL,
    AccessFailedCount INT NOT NULL,
    UserName NVARCHAR(256) NOT NULL,
    CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (Id)
);

CREATE TABLE adminapp.Roles (
    Id NVARCHAR(128) NOT NULL,
    Name NVARCHAR(256) NOT NULL,
    CONSTRAINT PK_Roles PRIMARY KEY CLUSTERED (Id)
);

CREATE TABLE adminapp.UserClaims (
    Id INT NOT NULL IDENTITY,
    UserId NVARCHAR(128) NOT NULL,
    ClaimType NVARCHAR(MAX) NULL,
    ClaimValue NVARCHAR(MAX) NULL,
    CONSTRAINT PK_UserClaims PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_UserClaims_Users_Id FOREIGN KEY (UserId) REFERENCES adminapp.Users (Id) ON DELETE CASCADE
);

CREATE TABLE adminapp.UserLogins (
    LoginProvider NVARCHAR(128) NOT NULL,
    ProviderKey NVARCHAR(128) NOT NULL,
    UserId NVARCHAR(128) NOT NULL,
    CONSTRAINT PK_UserLogins PRIMARY KEY CLUSTERED (LoginProvider, ProviderKey, UserId),
    CONSTRAINT FK_UserLogins_Users_Id FOREIGN KEY (UserId) REFERENCES adminapp.Users (Id) ON DELETE CASCADE
);

CREATE TABLE adminapp.UserRoles (
    UserId NVARCHAR(128) NOT NULL,
    RoleId NVARCHAR(128) NOT NULL,
    CONSTRAINT PK_UserRoles PRIMARY KEY CLUSTERED (UserId, RoleId),
    CONSTRAINT FK_UserRoles_Roles_Id FOREIGN KEY (RoleId) REFERENCES adminapp.Roles (Id) ON DELETE CASCADE,
    CONSTRAINT FK_UserRoles_Users_Id FOREIGN KEY (UserId) REFERENCES adminapp.Users (Id) ON DELETE CASCADE
);

CREATE UNIQUE NONCLUSTERED INDEX UQ_Roles_Name ON adminapp.Roles (Name);
CREATE NONCLUSTERED INDEX IX_UserClaims_UserId ON adminapp.UserClaims (UserId);
CREATE NONCLUSTERED INDEX IX_UserLogins_UserId ON adminapp.UserLogins (UserId);
CREATE NONCLUSTERED INDEX IX_UserRoles_RoleId ON adminapp.UserRoles (RoleId);
CREATE NONCLUSTERED INDEX IX_UserRoles_UserId ON adminapp.UserRoles (UserId);
CREATE UNIQUE NONCLUSTERED INDEX UQ_Users_UserName ON adminapp.Users (UserName);
