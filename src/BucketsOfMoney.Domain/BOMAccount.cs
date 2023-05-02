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
            await using var session = _documentStore.LightweightSession();
            var guid = Guid.NewGuid();
            var evt = new AccountCreated(accountName, guid.ToString());
            session.Events.StartStream<BOMAccount>(guid, evt);
            await session.SaveChangesAsync();

            return guid;
        }

        public async Task<BOMAccount> GetAccount(Guid accountGuid)
        {
            await using (var session = _documentStore.LightweightSession())
            {
                var aggregate = session.Events.AggregateStream<BOMAccount>(accountGuid);
                return aggregate;
            }
        }
    }
    public class Bucket
    {

    }

    public class BOMAccount
    {
        public List<Bucket> Buckets { get; set; } = new List<Bucket>();

        public Guid Id { get; set; }

        public BOMAccount() { }

    }
}
