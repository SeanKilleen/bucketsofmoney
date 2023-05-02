using FluentAssertions;
using TechTalk.SpecFlow;

namespace BucketsOfMoney.Domain.Tests
{
    [Binding]
    public class Steps
    {
        private readonly Manager _manager;
        private Guid _accountGuid;
        private BOMAccount _bomAccount;

        public Steps(Manager manager)
        {
            manager = manager;
        }

        [Given(@"a customer account is created for (.*)")]
        public async Task GivenACustomerAccountIsCreatedFor(string accountName)
        {
            _accountGuid = await _manager.CreateAccount(accountName);
        }

        [When(@"I look at the account")]
        public async Task WhenILookAtTheAccount()
        {
            _bomAccount = await _manager.GetAccount(_accountGuid);
        }


        [Then(@"the number of buckets for the account should be (.*)")]
        public void ThenTheNumberOfBucketsForTheAccountShouldBe(int expectedBucketCount)
        {
            _bomAccount.Buckets.Count.Should().Be(expectedBucketCount);
        }
    }
}
