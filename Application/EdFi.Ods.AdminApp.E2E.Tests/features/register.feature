# SPDX-License-Identifier: Apache-2.0
# Licensed to the Ed-Fi Alliance under one or more agreements.
# The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
# See the LICENSE and NOTICES files in the project root for more information.

# These tests needs to have a clean environment to run successfully, and should be only executed manually
Feature: Register

  #AA-925
  @Register
  Scenario: Register successfully
    Given it's on the "Log in" page
    And there are no users registered
    When clicking on register as a new user
    And user enters valid email and password
    And password confirmation
    And clicks Register
    Then registration is successful
