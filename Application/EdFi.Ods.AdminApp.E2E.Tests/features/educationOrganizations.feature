# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

Feature: Education Organizations
    Background:
        Given user is registered
        And user is logged in
        And setup is complete

    #AA-859
    @Sanity
    Scenario: Add local education agency
        Given it's on the "Education Organizations" page
        And education organization list has loaded
        When adding new local education agency
        And filling local education agency form
        And clicking save local education agency
        Then local education agency is added
        And added local education agency appears on list

    #AA-860
    Scenario: Edit local education agency
        Given there's a local education agency added
        And it's on the "Education Organizations" page
        And education organization list has loaded
        When clicking edit local education agency
        And modifying added local education agency
        And clicking save edited local education agency
        Then local education agency is edited
        And edited local education agency appears on list

    #AA-861
    Scenario: Delete local education agency
        Given there's a local education agency added
        And it's on the "Education Organizations" page
        And education organization list has loaded
        When clicking delete local education agency
        And delete local education agency modal is open
        And confirming delete local education agency
        Then local education agency is deleted

    #AA-862
    Scenario: Collapse local education agency Section
        Given there's a local education agency added
        And it's on the "Education Organizations" page
        And education organization list has loaded
        When clicking collapse local education agency section
        Then local education agency section is collapsed

    #AA-934
    Scenario Outline: Add local education agency validations
        Given it's on the "Education Organizations" page
        And education organization list has loaded
        When adding new local education agency
        And entering local education agency form "<Scenario>"
        And clicking save local education agency with errors
        Then local education agency validation for "<Scenario>" appears
        And modal is dismissed

        Examples:
            | Scenario |
            | no data  |
            | wrong id |

    #AA-1448
    Scenario: Add Duplicated local education agency Id
        Given there's a local education agency added
        And it's on the "Education Organizations" page
        And education organization list has loaded
        When adding new local education agency
        And filling local education agency form
        And clicking save local education agency with errors
        Then local education agency validation for "duplicated id" appears
        And modal is dismissed
