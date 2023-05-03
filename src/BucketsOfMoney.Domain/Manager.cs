using Marten;
using Microsoft.CodeAnalysis.VisualBasic;
using System.Security.Principal;

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

        var bucketCount = account.Buckets.Count;

        var fundForEachBucket = account.PoolAmount / bucketCount;

        using (var session = _documentStore.LightweightSession())
        {
            foreach (var bucket in account.Buckets)
            {
                var evt = new PoolFundsTransferredIntoBucket(bucket.Name, fundForEachBucket);
                session.Events.Append(accountGuid, evt);
            }
            await session.SaveChangesAsync();
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
}