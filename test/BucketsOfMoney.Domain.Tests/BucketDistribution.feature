Feature: Basic Bucket Distribution

Background: 
	Given a customer account is created for seankilleen@gmail.com

Scenario: Basic Bucket Splits
	Given I have created a bucket called Bucket A
		And I have created a bucket called Bucket B
		And I have added $100 to the pool
	When I empty the pool into the buckets
	Then Bucket A should have a total of $50
		And Bucket B should have a total of $50
