Feature: Create Publications

Scenario: Create a new publication
    Given I have a valid publication payload
    When I create a new publication
    Then the publication should be created successfully

 Scenario Outline: Create a new scheduled publication
    Given I have a valid scheduled publication payload for <ScheduledTime>
    When I create a new publication
    Then the publication scheduled at <ScheduledTime> should be created successfully
    Examples:
        | ScheduledTime |
        | 02:00         |
        | 02:30         |
        | 03:00         |
        | 00:00         |
        | 00:30         |
        | 23:30         |
        | 12:00         |

Scenario Outline: Create invalid publication
    Given I have an invalid publication payload with Title <Title> and Content <Content>
    When I create a new publication
    Then I should get a status code <ExpectedStatus>
    And the reason is <Reason>
    Examples:
        | Title          | Content                          | ExpectedStatus | Reason                                                                                                  |
        | ""             | Regular content                  | 400            | The title is invalid. It should not be empty or white space and should be less than 512 charactes long. |
        | " "            | Regular content                  | 400            | The title is invalid. It should not be empty or white space and should be less than 512 charactes long. |        
        | null           | Regular content                  | 400            | The title is invalid. It should not be empty or white space and should be less than 512 charactes long. |
        | Regular title  | ""                               | 400            | The content is null, empty or white space.                                                              |
        | Regular title  | " "                              | 400            | The content is null, empty or white space.                                                              |
        | Regular title  | null                             | 400            | The content is null, empty or white space.                                                              |        
        | Regular title  | <script>alert('XSS');</script>   | 422            | No <script> tags allowed in the content.    	                                                           |
        | Nam quis nulla. Integer malesuada. In in enim a arcu imperdiet malesuada. Sed vel lectus. Donec odio urna, tempus molestie, porttitor ut, iaculis quis, sem. Phasellus rhoncus. Aenean id metus id velit ullamcorper pulvinar. Vestibulum fermentum tortor id mi. Pellentesque ipsum. Nulla non arcu lacinia neque faucibus fringilla. Nulla non lectus sed nisl molestie malesuada. Proin in tellus sit amet nibh dignissim sagittis. Vivamus luctus egestas leo. Maecenas sollicitudin. Nullam rhoncus aliquam metus. Etiam tortor. | Regular content | 400 | The title is invalid. It should not be empty or white space and should be less than 512 charactes long. |

Scenario Outline: Create invalid scheduled publication
    Given I have an invalid scheduled publication payload with <ScheduledTime>
    When I create a new publication
    Then I should get a status code <ExpectedStatus>
    And the reason is <Reason>
    Examples:  
        | ScheduledTime | ExpectedStatus | Reason                                                                 |
        | Yesterday     | 400            | The given time must not be in the past.                                |
        | 00:01         | 400            | The given time must be on a 30-minute interval (e.g., 02:00 or 02:30). |
        | 00:29         | 400            | The given time must be on a 30-minute interval (e.g., 02:00 or 02:30). |
        | 00:31         | 400            | The given time must be on a 30-minute interval (e.g., 02:00 or 02:30). |

 Scenario: Create a new publication without the API Key
    Given I have a valid publication payload
    When I create a new publication without the API Key
    Then the request should be unauthorized