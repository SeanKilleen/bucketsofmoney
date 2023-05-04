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

Scenario: Thirds
	Given I have created a bucket called Bucket A
		And I have created a bucket called Bucket B
		And I have created a bucket called Bucket C
		And I have added $100 to the pool
	When I empty the pool into the buckets
		And I look at the account
	Then Bucket A should have a total of $33.33
		And Bucket B should have a total of $33.33
		And Bucket C should have a total of $33.33

Scenario: Ceiling on Buckets
	Given I have created a bucket called Bucket A
		And I have created a bucket called Bucket B
		And I have added $200 to the pool
		And Bucket A has a ceiling of $50
	When I empty the pool into the buckets
		And I look at the account
	Then Bucket A should have a total of $50
		And Bucket B should have a total of $150
		And The amount in the pool should be $0.00

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
		And Bucket C should have a total of $85
		And Bucket D should have a total of $85
		And The amount in the pool should be $0.00