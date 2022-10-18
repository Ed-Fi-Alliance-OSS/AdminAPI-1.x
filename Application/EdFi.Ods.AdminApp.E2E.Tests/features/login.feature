# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

Feature: Log in

    Background: User is registered
        Given user is registered

    #AA-994
    @Sanity
    Scenario: Log in successful
        Given it's on the "Log in" page
        When user enters valid email and password
        And clicks Log in
        Then login is successful

    #AA-995
    Scenario Outline: Log in errors
        Given it's on the "Log in" page
        When user enters "<Scenario>" for Log in
        And clicks Log in
        Then validation errors for Log in scenario: "<Scenario>" appears

        Examples:
            | Scenario             |
            | no data              |
            | email only           |
            | wrong email          |
            | email not registered |
            | password only        |
            | wrong password       |

    #AA-1019
    Scenario: Log out successful
        Given user is logged in
        When clicks sign out
        Then logout is successful
