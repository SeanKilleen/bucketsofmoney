Feature: Bucket Creation

Background: 
	Given a customer account is created for seankilleen@gmail.com

Scenario: No Buckets
	When I look at the account
	Then the account name should be seankilleen@gmail.com
		And the number of buckets for the account should be 0

Scenario: Creating A Single Bucket
	When I create a bucket called Bucket A
		And I look at the account
	Then the bucket Bucket A should exist
		And the number of buckets for the account should be 1

Scenario: Creating Multiple Buckets
	When I create a bucket called Bucket A
		And I create a bucket called Bucket B
		And I create a bucket called Bucket C
		And I look at the account
	Then the number of buckets for the account should be 3
		And the bucket Bucket A should exist
		And the bucket Bucket B should exist
		And the bucket Bucket C should exist