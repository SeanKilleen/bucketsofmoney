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
		# Each bucket now has 333.33
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

# TODO: Buckets without percentage set receive split percentage of remainder of 1000
# TODO: Can't set a bucket percentage if it would send the total over 100 percent
# TODO: If 0% is leftover for buckets, those other accounts don't grow 
# TODO: Set specific dollar amount on a given bucket
# TODO: Re-arrange bucket order and process rules in order of bucket
# TODO: By Default, remaining pool funds go to last bucket
# TODO: Can set which bucket receives remaining pool funds
	# TODO: Remaining funds bucket can't have a ceiling? (Or, goes back to pool?)