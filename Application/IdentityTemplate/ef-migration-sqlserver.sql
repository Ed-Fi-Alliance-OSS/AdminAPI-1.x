-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

CREATE TABLE adminapp.AspNetUsers (
    Id nvarchar(450) NOT NULL,
    UserName nvarchar(256) NULL,
    NormalizedUserName nvarchar(256) NULL,
    Email nvarchar(256) NULL,
    NormalizedEmail nvarchar(256) NULL,
    EmailConfirmed bit NOT NULL,
    PasswordHash nvarchar(max) NULL,
    SecurityStamp nvarchar(max) NULL,
    ConcurrencyStamp nvarchar(max) NULL,
    PhoneNumber nvarchar(max) NULL,
    PhoneNumberConfirmed bit NOT NULL,
    TwoFactorEnabled bit NOT NULL,
    LockoutEnd datetimeoffset NULL,
    LockoutEnabled bit NOT NULL,
    AccessFailedCount int NOT NULL,
    CONSTRAINT PK_AspNetUsers PRIMARY KEY (Id)
);

CREATE TABLE adminapp.AspNetRoles (
    Id nvarchar(450) NOT NULL,
    Name nvarchar(256) NULL,
    NormalizedName nvarchar(256) NULL,
    ConcurrencyStamp nvarchar(max) NULL,
    CONSTRAINT PK_AspNetRoles PRIMARY KEY (Id)
);

CREATE TABLE adminapp.AspNetUserClaims (
    Id int NOT NULL IDENTITY,
    UserId nvarchar(450) NOT NULL,
    ClaimType nvarchar(max) NULL,
    ClaimValue nvarchar(max) NULL,
    CONSTRAINT PK_AspNetUserClaims PRIMARY KEY (Id),
    CONSTRAINT FK_AspNetUserClaims_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES adminapp.AspNetUsers (Id) ON DELETE CASCADE
);

CREATE TABLE adminapp.AspNetRoleClaims (
    Id int NOT NULL IDENTITY,
    RoleId nvarchar(450) NOT NULL,
    ClaimType nvarchar(max) NULL,
    ClaimValue nvarchar(max) NULL,
    CONSTRAINT PK_AspNetRoleClaims PRIMARY KEY (Id),
    CONSTRAINT FK_AspNetRoleClaims_AspNetRoles_RoleId FOREIGN KEY (RoleId) REFERENCES adminapp.AspNetRoles (Id) ON DELETE CASCADE
);

CREATE TABLE adminapp.AspNetUserLogins (
    LoginProvider nvarchar(128) NOT NULL,
    ProviderKey nvarchar(128) NOT NULL,
    ProviderDisplayName nvarchar(max) NULL,
    UserId nvarchar(450) NOT NULL,
    CONSTRAINT PK_AspNetUserLogins PRIMARY KEY (LoginProvider, ProviderKey),
    CONSTRAINT FK_AspNetUserLogins_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES adminapp.AspNetUsers (Id) ON DELETE CASCADE
);

CREATE TABLE adminapp.AspNetUserRoles (
    UserId nvarchar(450) NOT NULL,
    RoleId nvarchar(450) NOT NULL,
    CONSTRAINT PK_AspNetUserRoles PRIMARY KEY (UserId, RoleId),
    CONSTRAINT FK_AspNetUserRoles_AspNetRoles_RoleId FOREIGN KEY (RoleId) REFERENCES adminapp.AspNetRoles (Id) ON DELETE CASCADE,
    CONSTRAINT FK_AspNetUserRoles_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES adminapp.AspNetUsers (Id) ON DELETE CASCADE
);

CREATE TABLE adminapp.AspNetUserTokens (
    UserId nvarchar(450) NOT NULL,
    LoginProvider nvarchar(128) NOT NULL,
    Name nvarchar(128) NOT NULL,
    Value nvarchar(max) NULL,
    CONSTRAINT PK_AspNetUserTokens PRIMARY KEY (UserId, LoginProvider, Name),
    CONSTRAINT FK_AspNetUserTokens_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES adminapp.AspNetUsers (Id) ON DELETE CASCADE
);

CREATE INDEX IX_AspNetRoleClaims_RoleId ON adminapp.AspNetRoleClaims (RoleId);
CREATE UNIQUE INDEX RoleNameIndex ON adminapp.AspNetRoles (NormalizedName) WHERE NormalizedName IS NOT NULL;
CREATE INDEX IX_AspNetUserClaims_UserId ON adminapp.AspNetUserClaims (UserId);
CREATE INDEX IX_AspNetUserLogins_UserId ON adminapp.AspNetUserLogins (UserId);
CREATE INDEX IX_AspNetUserRoles_RoleId ON adminapp.AspNetUserRoles (RoleId);
CREATE INDEX EmailIndex ON adminapp.AspNetUsers (NormalizedEmail);
CREATE UNIQUE INDEX UserNameIndex ON adminapp.AspNetUsers (NormalizedUserName) WHERE NormalizedUserName IS NOT NULL;
