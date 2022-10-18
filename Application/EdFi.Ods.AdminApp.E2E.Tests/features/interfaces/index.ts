// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.
export interface Credentials {
    Key: string;
    Secret: string;
    URL: string;
}

export interface Test {
    Feature: string;
    Scenario: string;
}
