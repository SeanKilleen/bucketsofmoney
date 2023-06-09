﻿namespace BucketsOfMoney.Domain;

public record AccountCreated(string AccountName, string AccountUuid);
public record BucketCreated(string BucketName);
public record BucketRemoved(string BucketName);
public record FundsAddedToPool(decimal Amount);
public record FundsRemovedFromPool(decimal Amount);
public record PoolFundsTransferredIntoBucket(string BucketName, decimal Amount);
public record BucketFundsTransferredIntoPool(string BucketName, decimal Amount);
public record BucketFundsTransferredToBucket(string OriginatingBucket, string ReceivingBucket, decimal Amount);
public record BucketCeilingChanged(string BucketName, decimal CeilingAmount);
public record PoolEmptied();
public record BucketIngressStrategyChangedToPercentStrategy(string BucketName, decimal Percentage);

public enum IngressEgressStrategyType
{
    Percentage,
    DollarValue
}

public record IngressStrategy(IngressEgressStrategyType Strategy, decimal Value);