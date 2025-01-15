 Feature: Version
     In order to ensure the current version of the Web API
     As a developer and operations team
     I want to monitor and verify the version of the API

 Scenario: Check the version of the Web API
     Given I want to check the Web API Version
     When I call the version endpoint
     Then the version should be correct