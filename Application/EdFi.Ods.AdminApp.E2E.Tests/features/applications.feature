# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

Feature: Applications
    Background:
        Given user is registered
        And user is logged in
        And setup is complete

    #AA-868
    Scenario: API URL
        Given it's on the "Applications" page
        And applications page has loaded
        When clicking API URL
        Then copied URL message appears
        And copied URL is valid

    Rule: Application interaction

        Background:
            Given there's a local education agency added
            And there's a vendor added

        #AA-889
        @Sanity
        Scenario: Add Application
            Given it's on the "Applications" page
            And applications page has loaded
            When adding new application
            And filling application form
            And clicking save application
            And key-secret modal appears
            And clicking modal message
            Then generated key-secret is valid
            And application appears on list

        #AA-896
        Scenario: Delete Application
            Given there's an application added
            And it's on the "Applications" page
            And applications page has loaded
            When clicking delete application
            And delete application modal is open
            And confirming delete application
            Then application is deleted
            And generated key-secret is not valid

        #AA-892
        Scenario: Edit Application
            Given there's an application added
            And it's on the "Applications" page
            And applications page has loaded
            When clicking edit application
            And modifying added application
            And clicking save edited application
            Then application is edited
            And edited application appears on list

        #AA-894
        Scenario: Regenerate Application Secret
            Given there's an application added
            And it's on the "Applications" page
            And applications page has loaded
            When clicking regenerate
            And regenerate application modal appears
            And clicking confirm regeneration
            And key-secret modal appears
            And clicking modal message
            Then credentials are updated
            And generated key-secret is valid
            And application appears on list

        #AA-1107
        Scenario Outline: Form validations
            Given it's on the "Applications" page
            And applications page has loaded
            When adding new application
            And entering application form "<Scenario>"
            And clicking save application with errors
            Then application validation for "<Scenario>" appears
            And application modal can be closed by "clicking x"

            Examples:
                | Scenario      |
                | no data       |
                | long app name |

        #AA-1298
        Scenario Outline: Modal interactions
            Given it's on the "Applications" page
            And applications page has loaded
            When adding new application
            Then application modal can be closed by "<Scenario>"
            And application modal is closed

            Examples:
                | Scenario        |
                | clicking x      |
                | clicking cancel |

        #AA-1299
        Scenario: Vendor divider
            Given there's an application added
            And it's on the "Applications" page
            And applications page has loaded
            When clicking collapse application
            Then application is collapsed
            And section is divided by vendor
