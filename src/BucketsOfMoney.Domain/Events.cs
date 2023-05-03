﻿namespace BucketsOfMoney.Domain;

public record AccountCreated(string AccountName, string AccountUuid);
public record BucketCreated(string BucketName);
public record FundsAddedToPool(decimal Amount);
public record PoolFundsTransferredIntoBucket(string BucketName, decimal Amount);