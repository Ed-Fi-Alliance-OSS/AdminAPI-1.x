# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

# These tests needs to have a clean environment to run successfully, and should be only executed manually
Feature: First Time Setup

    Background: User is registered
        Given user is registered
        And user is logged in

    #AA-1429
    @FTS
    Scenario: First Time Setup Successful
        Given it's on the "First Time Setup" page
        When clicking Continue
        Then first time setup is successful
