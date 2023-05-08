using System.ComponentModel.DataAnnotations;
using Marten;
using Marten.Linq.SoftDeletes;

namespace BucketsOfMoney.Domain;

public class Manager
{
    private readonly IDocumentStore _documentStore;

    public Manager(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }

    public async Task<Guid> CreateAccount(string accountName)
    {
        // TODO: Check to ensure account by that name doesn't exist?
        await using (var session = _documentStore.LightweightSession())
        {
            var guid = Guid.NewGuid();
            var evt = new AccountCreated(accountName, guid.ToString());
            session.Events.StartStream<BOMAccount>(guid, evt);
            await session.SaveChangesAsync();
            return guid;
        }
    }

    public async Task<BOMAccount> GetAccount(Guid accountGuid)
    {
        // TODO: Ensure exists
        await using (var session = _documentStore.LightweightSession())
        {
            var aggregate = session.Events.AggregateStream<BOMAccount>(accountGuid);
            return aggregate;
        }
    }

    public async Task CreateBucket(Guid accountGuid, string bucketName)
    {
        // TODO: Ensure account exists
        // TODO: Ensure bucket doesn't exist by that name
        // TODO: Create Guid for bucket as well?

        var evt = new BucketCreated(bucketName);

        using (var session = _documentStore.LightweightSession())
        {
            session.Events.Append(accountGuid, evt);
            await session.SaveChangesAsync();
        }
    }

    public async Task AddFundsToPool(Guid accountGuid, decimal amountToAdd)
    {
        var evt = new FundsAddedToPool(amountToAdd);
        using (var session = _documentStore.LightweightSession())
        {
            session.Events.Append(accountGuid, evt);
            await session.SaveChangesAsync();
        }
    }

    public async Task EmptyPool(Guid accountGuid)
    {
        var account = await GetAccount(accountGuid);

        // TODO: Test to ensure there are actually funds to empty

        using (var session = _documentStore.LightweightSession())
        {
            session.Events.Append(accountGuid, new PoolEmptied());
           
            DetermineBucketPercentages(account.Buckets);
            
            foreach (var bucket in account.Buckets)
            {
                var bucketFund = (account.PoolAmount * bucket.IngressStrategy.Value).MoneyRounded();
                var amountRemainingBeforeCeiling = (bucket.CeilingAmount - bucket.Amount).MoneyRounded();
                if (amountRemainingBeforeCeiling >= bucketFund)
                {
                    var standardTransfer = new PoolFundsTransferredIntoBucket(bucket.Name, bucketFund);
                    session.Events.Append(accountGuid, standardTransfer);
                    continue;
                }

                var transferToMeetCeiling = new PoolFundsTransferredIntoBucket(bucket.Name, amountRemainingBeforeCeiling);
                session.Events.Append(accountGuid, transferToMeetCeiling);
            }
            await session.SaveChangesAsync();
        }
    }

    private void DetermineBucketPercentages(List<Bucket> accountBuckets)
    {
        var totalDefinedBucketPercentages = accountBuckets
            .Where(x => x.IngressStrategy is not null && x.IngressStrategy?.Strategy == IngressEgressStrategyType.Percentage).ToList()
            .Sum(x => x.IngressStrategy!.Value);

        var remainingPercentages = 1m - totalDefinedBucketPercentages;

        if (remainingPercentages <= 0)
        {
            return;
        }

        var numberOfBucketsWithoutAssignedPercentages = accountBuckets.Count(x => x.IngressStrategy is null);

        var percentToAssign = remainingPercentages / numberOfBucketsWithoutAssignedPercentages;

        foreach (var bucket in accountBuckets)
        {
            if (bucket.IngressStrategy is null)
            {
                bucket.IngressStrategy = new IngressStrategy(IngressEgressStrategyType.Percentage, percentToAssign);
            }
        }
    }

    public async Task SetBucketCeiling(Guid accountGuid, string bucketName, decimal ceilingAmount)
    {
        using (var session = _documentStore.LightweightSession())
        {
            var evt = new BucketCeilingChanged(bucketName, ceilingAmount);
            
            session.Events.Append(accountGuid, evt);
            
            await session.SaveChangesAsync();
        }
    }

    public async Task SetBucketPercentageIngressStrategy(Guid accountGuid, string bucketName, decimal proposedPercentage)
    {
        if (proposedPercentage < 0)
        {
            throw new IngressStrategyMinimumAmountViolation();
        }

        // TODO: Ensure bucket exists
        using (var session = _documentStore.LightweightSession())
        {
            var aggregate = session.Events.AggregateStream<BOMAccount>(accountGuid);
            var bucket = aggregate.Buckets.Single(x => x.Name == bucketName);

            bucket.IngressStrategy = new IngressStrategy(IngressEgressStrategyType.Percentage, proposedPercentage);

            var sumOfIngressPercentages = aggregate.Buckets.Where(x =>
                    x.IngressStrategy is not null && x.IngressStrategy.Strategy == IngressEgressStrategyType.Percentage)
                .Sum(x => x.IngressStrategy!.Value);

            if (sumOfIngressPercentages > 1)
            {
                throw new IngressStrategyMaximumAmountViolation();
            }

            var evt = new BucketIngressStrategyChangedToPercentStrategy(bucketName, proposedPercentage);

            session.Events.Append(accountGuid, evt);

            await session.SaveChangesAsync();
        }
    }

    public async Task UpdateAccountBalance(Guid accountGuid, decimal newAccountBalance)
    {
        if (newAccountBalance < 0) { throw new AccountMinimumAmountViolation(); }

        using (var session = _documentStore.LightweightSession())
        {
            // TODO: Ensure aggregate exists
            var aggregate = session.Events.AggregateStream<BOMAccount>(accountGuid);

            var existingBalance = aggregate.Balance;
            var newBalance = newAccountBalance;

            var balanceDifference = Math.Abs(newBalance - existingBalance);

            if (newBalance - existingBalance >= 0)
            {
                session.Events.Append(accountGuid, new FundsAddedToPool(balanceDifference));
            }
            else
            {
                session.Events.Append(accountGuid, new FundsRemovedFromPool(balanceDifference));
;           }

            await session.SaveChangesAsync();
        }
    }

    public async Task RemoveBucket(Guid accountGuid, string bucketToRemove)
    {
        using (var session = _documentStore.LightweightSession())
        {
            // TODO: Ensure aggregate exists
            // TODO: Check that bucket exists to remove
            var aggregate = session.Events.AggregateStream<BOMAccount>(accountGuid);
            var bucketBalance = aggregate.Buckets.Single(x => x.Name == bucketToRemove).Amount;

            session.Events.Append(accountGuid, new BucketFundsTransferredIntoPool(bucketToRemove, bucketBalance));
            session.Events.Append(accountGuid, new BucketRemoved(bucketToRemove));
            
            await session.SaveChangesAsync();
        }
    }

    public async Task TransferFunds(Guid accountGuid, string originatingBucket, string receivingBucket, decimal amountToTransfer)
    {
        // TODO: Ensure aggregate exists
        // TODO Check originating bucket exists
        // TODO Check receiving bucket exists
        // TODO Check originating bucket has enough to cover transfer
        // TODO Check ceiling amount in receiving bucket
        using (var session = _documentStore.LightweightSession())
        {
            var aggregate = session.Events.AggregateStream<BOMAccount>(accountGuid);

            session.Events.Append(accountGuid, new BucketFundsTransferredToBucket(originatingBucket, receivingBucket, amountToTransfer));

            await session.SaveChangesAsync();
        }
    }
}