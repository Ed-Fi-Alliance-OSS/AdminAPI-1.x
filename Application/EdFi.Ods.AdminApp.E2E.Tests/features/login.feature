Feature: Log in

  #AA-994
  @Sanity
  Scenario: Log in successful
    Given it's on the "Log in" page
    When user enters valid username and password
    And clicks Log in
    Then login is successful
