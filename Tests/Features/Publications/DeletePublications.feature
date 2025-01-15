Feature: Delete Publications

Scenario: Delete an unsent publication
    Given I have a valid unsent scheduled publication created
    When I delete this publication
    Then the publication should be deleted successfully

Scenario: Delete an unsent publication without the API Key
    Given I have a valid unsent scheduled publication created
    When I delete this publication without the API Key
    Then the request should be unauthorized

Scenario: Delete a publication that does not exist
    Given I do not have any publication
    When I delete a publication that does not exist
    Then the request should fail