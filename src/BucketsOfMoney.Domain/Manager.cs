﻿using Marten;

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

        var unaccountedPoolAmount = account.PoolAmount;

        using (var session = _documentStore.LightweightSession())
        {
            session.Events.Append(accountGuid, new PoolEmptied());

           
            DetermineBucketPercentages(account.Buckets);
            
            foreach (var bucket in account.Buckets)
            {
                var bucketFund = (unaccountedPoolAmount * bucket.IngressStrategy.Value).MoneyRounded();
                var amountRemainingBeforeCeiling = (bucket.CeilingAmount - bucket.Amount).MoneyRounded();
                if (amountRemainingBeforeCeiling >= bucketFund)
                {
                    var standardTransfer = new PoolFundsTransferredIntoBucket(bucket.Name, bucketFund);
                    session.Events.Append(accountGuid, standardTransfer);
                    unaccountedPoolAmount -= bucketFund;
                    continue;
                }

                var transferToMeetCeiling = new PoolFundsTransferredIntoBucket(bucket.Name, amountRemainingBeforeCeiling);
                session.Events.Append(accountGuid, transferToMeetCeiling);

                unaccountedPoolAmount -= amountRemainingBeforeCeiling;
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

    public async Task SetBucketPercentageIngressStrategy(Guid accountGuid, string bucketName, decimal percentageIngressStrategy)
    {
        // TODO: Ensure bucket exists
        using (var session = _documentStore.LightweightSession())
        {
            var evt = new BucketIngressStrategyChangedToPercentStrategy(bucketName, percentageIngressStrategy);

            session.Events.Append(accountGuid, evt);

            await session.SaveChangesAsync();
        }
    }
}