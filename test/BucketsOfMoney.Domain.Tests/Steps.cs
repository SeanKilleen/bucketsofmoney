﻿using FluentAssertions;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Infrastructure;
using Xunit.Sdk;

namespace BucketsOfMoney.Domain.Tests
{
    [Binding]
    public class Steps
    {
        private readonly Manager _manager;
        private readonly ISpecFlowOutputHelper _outputHelper;
        private Guid _accountGuid;
        private BOMAccount _bomAccount;
        private Exception _exception;

        public Steps(Manager manager, ISpecFlowOutputHelper outputHelper)
        {
            _manager = manager;
            _outputHelper = outputHelper;
        }

        [Given(@"I have created a bucket called (.*)")]
        public async Task GivenIHaveCreatedABucketCalled(string bucketName)
        {
            await WhenICreateABucketCalled(bucketName);
        }

        [Given(@"(.*) has a percentage ingress strategy of (.*)")]
        public async Task GivenTripHasAPercentageIngressStrategyOf(string bucketName, decimal percentageIngressStrategy)
        {
            await _manager.SetBucketPercentageIngressStrategy(_accountGuid, bucketName, percentageIngressStrategy);
        }


        [Given(@"I have added \$(.*) to the pool")]
        public async Task GivenIHaveAddedToThePool(decimal amountToAdd)
        {
            await _manager.AddFundsToPool(_accountGuid, amountToAdd);
        }

        [Given(@"(.*) has a ceiling of \$(.*)")]
        public async Task GivenBucketHasACeiling(string bucketName, decimal ceilingAmount)
        {
            await _manager.SetBucketCeiling(_accountGuid, bucketName, ceilingAmount);
        }


        [Given(@"a customer account is created for (.*)")]
        public async Task GivenACustomerAccountIsCreatedFor(string accountName)
        {
            _accountGuid = await _manager.CreateAccount(accountName);
        }

        [Given(@"I have emptied the pool into the buckets")]
        public async Task GivenIHaveEmptiedThePoolIntoTheBuckets()
        {
            await _manager.EmptyPool(_accountGuid);
        }

        [When(@"I look at the account")]
        public async Task WhenILookAtTheAccount()
        {
            _bomAccount = await _manager.GetAccount(_accountGuid);
        }

        [When(@"I create a bucket called (.*)")]
        public async Task WhenICreateABucketCalled(string bucketName)
        {
            await _manager.CreateBucket(_accountGuid, bucketName);
        }

        [When(@"I attempt to set the (.*) ingress strategy to a percentage of (.*)")]
        public async Task WhenIAttemptToSetTheIngressStrategyToAPercentageOf(string bucketName, decimal percentage)
        {
            try
            {
                await _manager.SetBucketPercentageIngressStrategy(_accountGuid, bucketName, percentage);
            }
            catch (Exception ex)
            {
                CaptureException(ex);
            }
        }

        private void CaptureException(Exception ex)
        {
            _outputHelper.WriteLine($"EXCEPTION: {ex.Message}. InnerException: {ex.InnerException?.Message}");
            _exception = ex;
        }

        [When(@"I empty the pool into the buckets")]
        public async Task WhenIEmptyThePoolIntoTheBuckets()
        {
            await _manager.EmptyPool(_accountGuid);
        }

        [Then(@"(.*) should have a total of \$(.*)")]
        public void ThenBucketShouldHaveATotalOf(string bucketName, decimal expectedTotal)
        {
            var bucket = _bomAccount.Buckets.Single(x => x.Name == bucketName);
            bucket.Amount.Should().Be(expectedTotal);
        }

        [Then(@"the bucket (.*) should exist")]
        public void ThenTheBucketBucketAShouldExist(string expectedBucketName)
        {
            _bomAccount.Buckets.Should().Contain(bucket => bucket.Name == expectedBucketName);
        }

        [Then(@"the account name should be (.*)")]
        public void ThenTheAccountNameShouldBe(string expectedAccountName)
        {
            _bomAccount.Name.Should().Be(expectedAccountName);
        }

        [Then(@"the number of buckets for the account should be (.*)")]
        public void ThenTheNumberOfBucketsForTheAccountShouldBe(int expectedBucketCount)
        {
            _bomAccount.Buckets.Count.Should().Be(expectedBucketCount);
        }

        [Then(@"an exception should be thrown")]
        public void ThenAnExceptionShouldBeThrown()
        {
            _exception.Should().NotBeNull();
        }

        [Then(@"the error should indicate I would exceed 100%")]
        public void ThenTheErrorShouldIndicateIWouldExceed100Percent()
        {
            _exception.Message.Should().ContainEquivalentOf("Would exceed 100%"); 
        }

        [Then(@"The amount in the pool should be \$(.*)")]
        public void ThenTheAmountInThePoolShouldBe(decimal expectedPoolAmount)
        {
            _bomAccount.PoolAmount.Should().Be(expectedPoolAmount);
        }

    }
}
