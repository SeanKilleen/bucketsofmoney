using System.Security.Cryptography;

namespace BucketsOfMoney.Domain
{
    public class Bucket
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public decimal CeilingAmount { get; set; } = decimal.MaxValue;
        public IngressStrategy? IngressStrategy { get; set; } = null;
    }

    public class BOMAccount
    {
        public string Name = string.Empty;
        public List<Bucket> Buckets { get; set; } = new List<Bucket>();
        public decimal PoolAmount { get; set; } = 0;
        public decimal Balance => PoolAmount + Buckets.Sum(x=>x.Amount);

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

        public void Apply(FundsRemovedFromPool evt)
        {
            this.PoolAmount -= evt.Amount;
        }

        public void Apply(BucketRemoved evt)
        {
            var bucket = this.Buckets.Single(x => x.Name == evt.BucketName);
            this.Buckets.Remove(bucket);
        }

        public void Apply(PoolFundsTransferredIntoBucket evt)
        {
            var bucket = this.Buckets.Single(x => x.Name == evt.BucketName);
            bucket.Amount += evt.Amount;
            this.PoolAmount -= evt.Amount;
        }

        public void Apply(BucketFundsTransferredIntoPool evt)
        {
            var bucket = this.Buckets.Single(x => x.Name == evt.BucketName);

            bucket.Amount -= evt.Amount;
            this.PoolAmount += evt.Amount;
        }

        public void Apply(BucketCeilingChanged evt)
        {
            var bucket = this.Buckets.Single(x => x.Name == evt.BucketName);
            bucket.CeilingAmount = evt.CeilingAmount;
        }

        public void Apply(BucketFundsTransferredToBucket evt)
        {
            var originatingBucket = Buckets.Single(x => x.Name == evt.OriginatingBucket);
            var recipientBucket = Buckets.Single(x => x.Name == evt.ReceivingBucket);

            originatingBucket.Amount -= evt.Amount;
            recipientBucket.Amount += evt.Amount;
        }

        public void Apply(BucketIngressStrategyChangedToPercentStrategy evt)
        {
            var bucket = this.Buckets.Single(x => x.Name == evt.BucketName);
            bucket.IngressStrategy = new IngressStrategy(IngressEgressStrategyType.Percentage, evt.Percentage);
        }
    }
}
