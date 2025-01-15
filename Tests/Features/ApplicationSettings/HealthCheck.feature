 Feature: Health Check
     In order to ensure the reliability and general health of the Web API
     As a developer and operations team
     I want to monitor and verify the health of the API

 Scenario: Check the health status of the Web API
     Given I want to check the Web API Health
     When I call the health check endpoint
     Then the Web API status should be heathy
     And the database status should be heathy