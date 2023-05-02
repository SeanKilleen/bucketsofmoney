Feature: Basic Buckets

Background: 

Scenario: No Buckets
	Given a customer account is created for seankilleen@gmail.com
	When I look at the account
	Then the account name should be seankilleen@gmail.com
		And the number of buckets for the account should be 0

#Scenario: Creating Buckets
#	Given a customer account is created for seankilleen@gmail.com
#	When I create a bucket called Bucket A
#	Then Bucket A should exist
#	And the number of buckets for the account should be 1

#Scenario: Basic Bucket Setup
#	Given I have $100 in the pool
#		And a bucket exists called Bucket A
#		And a bucket exists called Bucket B
#		And Bucket A has a 50% ingress rule
#		And Bucket B has a 50% ingress rule
#	When I empty the pool into the buckets
#	Then Bucket A should have a total of $50
#		And Bucket B should have a total of $50
