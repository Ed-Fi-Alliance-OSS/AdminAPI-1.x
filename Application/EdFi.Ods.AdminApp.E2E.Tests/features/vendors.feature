# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

Feature: Vendors
    Background:
        Given user is registered
        And user is logged in
        And setup is complete

    #AA-1143
    Scenario: Empty vendor list
        Given it's on the "Vendors" page
        And vendor page has loaded
        Then no vendors message appears

    #AA-871
    Scenario: Add vendor
        Given it's on the "Vendors" page
        And vendor page has loaded
        When clicking add vendor
        And filling vendor form
        And adding vendor namespace prefix
        And clicking save vendor
        Then vendor is added
        And added vendor appears on list

    #AA-872
    Scenario: Edit vendor
        Given there's a vendor added
        And it's on the "Vendors" page
        And vendor page has loaded
        When clicking edit vendor
        And modifying added vendor
        And clicking save edited vendor
        Then vendor is edited
        And edited vendor appears on list

    #AA-873
    Scenario: Delete vendor
        Given there's a vendor added
        And it's on the "Vendors" page
        And vendor page has loaded
        When clicking delete vendor
        And delete vendor modal is open
        And confirming delete vendor
        Then vendor is deleted

    #AA-880
    Scenario: Help section
        Given it's on the "Vendors" page
        And vendor page has loaded
        When help section is present
        Then help section can be collapsed
        And help section can be expanded

    #AA-999
    Scenario: Define Applications (Single Instance)
        Given there's a vendor added
        And it's on the "Vendors" page
        And vendor page has loaded
        When clicking define applications
        Then it navigates to the applications page

    #AA-1000
    Scenario Outline: Add vendor form validations
        Given it's on the "Vendors" page
        And vendor page has loaded
        When clicking add vendor
        And entering vendor form "<Scenario>"
        And clicking save vendor with errors
        Then vendor validation for "<Scenario>" appears
        And vendor modal can be closed by "clicking x"

        Examples:
            | Scenario    |
            | no data     |
            | wrong email |

    #AA-1503
    Scenario Outline: Modal interactions
        Given it's on the "Vendors" page
        And vendor page has loaded
        When clicking add vendor
        Then vendor modal can be closed by "<Scenario>"
        And vendor modal is closed

        Examples:
            | Scenario         |
            | clicking outside |
            | clicking x       |
            | clicking cancel  |
