Feature: Basic Bucket Distribution

Background: 
	Given a customer account is created for seankilleen@gmail.com

Scenario: Basic Bucket Splits
	Given I have created a bucket called Bucket A
		And I have created a bucket called Bucket B
		And I have added $100 to the pool
	When I empty the pool into the buckets
		And I look at the account
	Then Bucket A should have a total of $50
		And Bucket B should have a total of $50

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