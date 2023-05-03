using System.Security.Cryptography;

namespace BucketsOfMoney.Domain
{
    public class Bucket
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
    }

    public class BOMAccount
    {
        public string Name = string.Empty;
        public List<Bucket> Buckets { get; set; } = new List<Bucket>();
        public decimal PoolAmount { get; set; } = 0;

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

        public void Apply(FundsAddedToPool evt)
        {
            this.PoolAmount += evt.Amount;
        }

        public void Apply(PoolFundsTransferredIntoBucket evt)
        {
            var bucket = this.Buckets.Single(x => x.Name == evt.BucketName);
            bucket.Amount += evt.Amount;
            this.PoolAmount -= evt.Amount;
        }
    }
}
