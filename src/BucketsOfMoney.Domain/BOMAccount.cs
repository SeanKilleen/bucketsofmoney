using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marten;
using Marten.Internal.Storage;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace BucketsOfMoney.Domain
{
    public class Bucket
    {

    }

    public class BOMAccount
    {
        private readonly IDocumentStore _documentStore;

        public List<Bucket> Buckets { get; set; } = new List<Bucket>();

        public BOMAccount(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public async Task<string> CreateAccount(string accountName)
        {
            await using var session = _documentStore.LightweightSession();
            var guid = Guid.NewGuid().ToString();
            var evt = new AccountCreated(accountName, guid);
            session.Events.StartStream<BOMAccount>(guid, evt);
            await session.SaveChangesAsync();

            return guid;
        }
    }
}
