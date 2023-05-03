using Marten;

namespace BucketsOfMoney.Domain
{
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
    }
    public class Bucket
    {
        public string Name { get; set; }
    }

    public class BOMAccount
    {
        public string Name = string.Empty;
        public List<Bucket> Buckets { get; set; } = new List<Bucket>();

        public Guid Id { get; set; }

        public BOMAccount() { }

        public void Apply(AccountCreated evt)
        {
            Name = evt.AccountName;
        }

        public void Apply(BucketCreated evt)
        {
            this.Buckets.Add(new Bucket(){Name = evt.BucketName});
        }

    }
}
