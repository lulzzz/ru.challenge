using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace RU.Challenge.Domain.Entities
{
    public class Subscription
    {
        public Guid Id { get; private set; }

        public PaymentMethod PaymentMethod { get; private set; }

        public IImmutableList<DistributionPlatform> DistributionPlatforms { get; private set; }

        public DateTimeOffset ExpirationDate { get; private set; }

        public decimal Amount { get; private set; }

        private Subscription(
            Guid id,
            PaymentMethod paymentMethod,
            IImmutableList<DistributionPlatform> distributionPlatforms,
            DateTimeOffset expirationDate,
            decimal amount) : this()
        {
            Id = id;
            PaymentMethod = paymentMethod;
            DistributionPlatforms = distributionPlatforms.ToImmutableList();
            ExpirationDate = expirationDate;
            Amount = amount;
        }

        private Subscription()
            => DistributionPlatforms = ImmutableList.Create<DistributionPlatform>();

        public static Subscription Create(
            PaymentMethod paymentMethod,
            IImmutableList<DistributionPlatform> distributionPlatforms,
            DateTimeOffset expirationDate,
            decimal amount)
            => new Subscription(Guid.NewGuid(), paymentMethod, distributionPlatforms, expirationDate, amount);
    }
}