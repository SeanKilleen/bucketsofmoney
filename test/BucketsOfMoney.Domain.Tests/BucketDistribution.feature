Feature: Basic Bucket Distribution

Background: 
	Given a customer account is created for seankilleen@gmail.com

Scenario: Halvsies
	Given I have created a bucket called Bucket A
		And I have created a bucket called Bucket B
		And I have added $100 to the pool
	When I empty the pool into the buckets
		And I look at the account
	Then Bucket A should have a total of $50
		And Bucket B should have a total of $50

Scenario: Thirds leaves a penny left over
	Given I have created a bucket called Bucket A
		And I have created a bucket called Bucket B
		And I have created a bucket called Bucket C
		And I have added $100 to the pool
	When I empty the pool into the buckets
		And I look at the account
	Then Bucket A should have a total of $33.33
		And Bucket B should have a total of $33.33
		And Bucket C should have a total of $33.33
		And The amount in the pool should be $0.01

Scenario: One Penny doesn't get split between buckets

	Given I have created a bucket called Bucket A
		And I have created a bucket called Bucket B
		And I have added $0.01 to the pool
	When I empty the pool into the buckets
		And I look at the account
	Then Bucket A should have a total of $0.00
		And Bucket B should have a total of $0.00
		And The amount in the pool should be $0.01

Scenario: Half a Cent doesn't get split 

	Given I have created a bucket called Bucket A
		And I have created a bucket called Bucket B
		And I have added $0.03 to the pool
	When I empty the pool into the buckets
		And I look at the account
	Then Bucket A should have a total of $0.01
		And Bucket B should have a total of $0.01
		And The amount in the pool should be $0.01

Scenario: Ceiling on Buckets
	Given I have created a bucket called Bucket A
		And I have created a bucket called Bucket B
		And I have added $200 to the pool
		And Bucket A has a ceiling of $50
	When I empty the pool into the buckets
		And I look at the account
	Then Bucket A should have a total of $50
		And Bucket B should have a total of $100
		And The amount in the pool should be $50

Scenario: Multiple Ceilings
	Given I have added $200 to the pool
		And I have created a bucket called Bucket A
		And I have created a bucket called Bucket B
		And I have created a bucket called Bucket C
		And I have created a bucket called Bucket D
		And Bucket A has a ceiling of $10
		And Bucket B has a ceiling of $20
	When I empty the pool into the buckets
		And I look at the account
	Then Bucket A should have a total of $10
		And Bucket B should have a total of $20
		And Bucket C should have a total of $50
		And Bucket D should have a total of $50
		And The amount in the pool should be $70

Scenario: Multiple Rounds of Adding
	Given I have created a bucket called Trip
		And I have created a bucket called Home Repairs
		And I have created a bucket called Emergency Fund
		And Trip has a ceiling of $1000
		And I have added $1000 to the pool
		And I have emptied the pool into the buckets
		And I have added $3000 to the pool
	When I empty the pool into the buckets
		And I look at the account
	Then Trip should have a total of $1000
		And Home Repairs should have a total of $1333.33
		And Emergency Fund should have a total of $1333.33
		And The amount in the pool should be $333.34

Scenario: Set percentages at bucket level
	Given I have created a bucket called Trip
		And I have created a bucket called Home Repairs
		And I have created a bucket called Emergency Fund
		And Trip has a percentage ingress strategy of .10
		And I have added $100 to the pool
	When I empty the pool into the buckets
		And I look at the account
	Then Trip should have a total of $10
		And Home Repairs should have a total of $45.00
		And Emergency Fund should have a total of $45.00
		And The amount in the pool should be $0

Scenario: Can't set a bucket percentage if it would send the total over 100 percent
	Given I have created a bucket called Trip
		And I have created a bucket called Home Repairs
		And Trip has a percentage ingress strategy of .5
	When I attempt to set the Home Repairs ingress strategy to a percentage of .51
	Then an exception should be thrown
		And the error should indicate I would exceed 100%

Scenario: Can't set an ingress to less than zero
	Given I have created a bucket called Trip
	When I attempt to set the Trip ingress strategy to a percentage of -.01
	Then an exception should be thrown
		And the error should indicate I can't set an ingress strategy below 0%

Scenario: Can't set an ingress to less than zero (multiple buckets)
	Given I have created a bucket called Bucket A
		And I have created a bucket called Bucket B
		And Bucket A has a percentage ingress strategy of .5
	When I attempt to set the Bucket B ingress strategy to a percentage of -.01
	Then an exception should be thrown
		And the error should indicate I can't set an ingress strategy below 0%

Scenario: Account Balance When Only Pool Has Funds
	Given I have added $100 to the pool
	When I look at the account
	Then the account balance should be $100

Scenario: Account Balance When All Funds are in Buckets
	Given I have created a bucket called Bucket A
		And I have created a bucket called Bucket B
		And I have added $100 to the pool
	When I empty the pool into the buckets
		And I look at the account
	Then Bucket A should have a total of $50
		And Bucket B should have a total of $50
		And the account balance should be $100

Scenario: Account Balance When Funds include Pool and Buckets
	Given I have created a bucket called Bucket A
		And I have created a bucket called Bucket B
		And I have added $200 to the pool
		And Bucket A has a ceiling of $50
	When I empty the pool into the buckets
		And I look at the account
	Then Bucket A should have a total of $50
		And Bucket B should have a total of $100
		And The amount in the pool should be $50
		And the account balance should be $200

Scenario: New account balance with no transactions sets pool to that new balance
	Given I have added $<previousPoolAmount> to the pool
	When I update my account balance to $<newAccountBalance>
		And I look at the account
	Then The amount in the pool should be $<newAccountBalance>

Examples: 
| previousPoolAmount | newAccountBalance |
| -1                 | 0                 |
| -1                 | 100               |
| -1                 | -5                |
| 0                  | 0                 |
| 0                  | 100               |
| 0                  | -5                |
| 1                  | 0                 |
| 1                  | 100               |
| 1                  | -5                |

Scenario: New Account Balance With Transactions
	Given I have updated the account balance to $<initialBalance>
		And I have created a bucket called First
		And I have created a bucket called Second
		And I have emptied the pool into the buckets
	When I update my account balance to $<newBalance>
		And I look at the account
	Then The amount in the pool should be $<expectedPoolAmount>

Examples: 
| initialBalance | newBalance | expectedPoolAmount |
| 0              | 0          | 0                  |
| 1.00           | 1.00       | 0                  |
| 1.00           | 2.00       | 1.00               |
| 10             | 5          | -5                 |
| 10             | 20         | 10                 |

# TODO: Can't set account balance to below 0
# TODO: Removing a bucket
# TODO: Removing a bucket moves its funds back to the pool
# TODO: Transfer between buckets
# TODO: Transfer back to pool

# TODO: Egress rules - percentage
# TODO: Egress rules - dollar value
# TODO: Egress rules - combination of both

# TODO: Set specific dollar amount on a given bucket
# TODO: Re-arrange bucket order and process rules in order of bucket
# TODO: Empty pool vs "Reconcile pool" (since pool might be negative?) Refactor the language?