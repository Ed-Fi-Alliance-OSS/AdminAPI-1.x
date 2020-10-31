-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

CREATE TABLE adminapp.Users (
    Id NVARCHAR(450) NOT NULL,
    UserName NVARCHAR(256) NULL,
    NormalizedUserName NVARCHAR(256) NULL,
    Email NVARCHAR(256) NULL,
    NormalizedEmail NVARCHAR(256) NULL,
    EmailConfirmed BIT NOT NULL,
    PasswordHash NVARCHAR(MAX) NULL,
    SecurityStamp NVARCHAR(MAX) NULL,
    ConcurrencyStamp NVARCHAR(MAX) NULL,
    PhoneNumber NVARCHAR(MAX) NULL,
    PhoneNumberConfirmed BIT NOT NULL,
    TwoFactorEnabled BIT NOT NULL,
    LockoutEnd DATETIMEOFFSET NULL,
    LockoutEnabled BIT NOT NULL,
    AccessFailedCount INT NOT NULL,
    CONSTRAINT PK_Users PRIMARY KEY (Id)
);

CREATE TABLE adminapp.Roles (
    Id NVARCHAR(450) NOT NULL,
    Name NVARCHAR(256) NULL,
    NormalizedName NVARCHAR(256) NULL,
    ConcurrencyStamp NVARCHAR(MAX) NULL,
    CONSTRAINT PK_Roles PRIMARY KEY (Id)
);

CREATE TABLE adminapp.UserClaims (
    Id INT NOT NULL IDENTITY,
    UserId NVARCHAR(450) NOT NULL,
    ClaimType NVARCHAR(MAX) NULL,
    ClaimValue NVARCHAR(MAX) NULL,
    CONSTRAINT PK_UserClaims PRIMARY KEY (Id),
    CONSTRAINT FK_UserClaims_Users_UserId FOREIGN KEY (UserId) REFERENCES adminapp.Users (Id) ON DELETE CASCADE
);

CREATE TABLE adminapp.RoleClaims (
    Id INT NOT NULL IDENTITY,
    RoleId NVARCHAR(450) NOT NULL,
    ClaimType NVARCHAR(MAX) NULL,
    ClaimValue NVARCHAR(MAX) NULL,
    CONSTRAINT PK_RoleClaims PRIMARY KEY (Id),
    CONSTRAINT FK_RoleClaims_Roles_RoleId FOREIGN KEY (RoleId) REFERENCES adminapp.Roles (Id) ON DELETE CASCADE
);

CREATE TABLE adminapp.UserLogins (
    LoginProvider NVARCHAR(128) NOT NULL,
    ProviderKey NVARCHAR(128) NOT NULL,
    ProviderDisplayName NVARCHAR(MAX) NULL,
    UserId NVARCHAR(450) NOT NULL,
    CONSTRAINT PK_UserLogins PRIMARY KEY (LoginProvider, ProviderKey),
    CONSTRAINT FK_UserLogins_Users_UserId FOREIGN KEY (UserId) REFERENCES adminapp.Users (Id) ON DELETE CASCADE
);

CREATE TABLE adminapp.UserRoles (
    UserId NVARCHAR(450) NOT NULL,
    RoleId NVARCHAR(450) NOT NULL,
    CONSTRAINT PK_UserRoles PRIMARY KEY (UserId, RoleId),
    CONSTRAINT FK_UserRoles_Roles_RoleId FOREIGN KEY (RoleId) REFERENCES adminapp.Roles (Id) ON DELETE CASCADE,
    CONSTRAINT FK_UserRoles_Users_UserId FOREIGN KEY (UserId) REFERENCES adminapp.Users (Id) ON DELETE CASCADE
);

CREATE TABLE adminapp.UserTokens (
    UserId NVARCHAR(450) NOT NULL,
    LoginProvider NVARCHAR(128) NOT NULL,
    Name NVARCHAR(128) NOT NULL,
    Value NVARCHAR(MAX) NULL,
    CONSTRAINT PK_UserTokens PRIMARY KEY (UserId, LoginProvider, Name),
    CONSTRAINT FK_UserTokens_Users_UserId FOREIGN KEY (UserId) REFERENCES adminapp.Users (Id) ON DELETE CASCADE
);

CREATE INDEX IX_RoleClaims_RoleId ON adminapp.RoleClaims (RoleId);
CREATE UNIQUE INDEX RoleNameIndex ON adminapp.Roles (NormalizedName) WHERE NormalizedName IS NOT NULL;
CREATE INDEX IX_UserClaims_UserId ON adminapp.UserClaims (UserId);
CREATE INDEX IX_UserLogins_UserId ON adminapp.UserLogins (UserId);
CREATE INDEX IX_UserRoles_RoleId ON adminapp.UserRoles (RoleId);
CREATE INDEX EmailIndex ON adminapp.Users (NormalizedEmail);
CREATE UNIQUE INDEX UserNameIndex ON adminapp.Users (NormalizedUserName) WHERE NormalizedUserName IS NOT NULL;
