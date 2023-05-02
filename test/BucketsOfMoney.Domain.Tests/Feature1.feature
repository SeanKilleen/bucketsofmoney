Feature: Basic Buckets

Background: 
	Given a customer account exists for seankilleen@gmail.com

Scenario: Basic Bucket Setup
	Given I have $100 in the pool
		And a bucket exists called Bucket A
		And a bucket exists called Bucket B
		And Bucket A has a 50% ingress rule
		And Bucket B has a 50% ingress rule
	When I empty the pool into the buckets
	Then Bucket A should have a total of $50
		And Bucket B should have a total of $50
