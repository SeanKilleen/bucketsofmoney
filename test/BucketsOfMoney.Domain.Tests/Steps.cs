using FluentAssertions;
using TechTalk.SpecFlow;

namespace BucketsOfMoney.Domain.Tests
{
    [Binding]
    public class Steps
    {
        private readonly BOMAccount _bomAccount;

        public Steps(BOMAccount bomAccount)
        {
            _bomAccount = bomAccount;
        }

        [Given(@"a customer account is created for (.*)")]
        public async Task GivenACustomerAccountIsCreatedFor(string accountName)
        {
            await _bomAccount.CreateAccount(accountName);
        }

        [Then(@"the number of buckets for the account should be (.*)")]
        public void ThenTheNumberOfBucketsForTheAccountShouldBe(int expectedBucketCount)
        {
            _bomAccount.Buckets.Count.Should().Be(expectedBucketCount);
        }
    }
}
