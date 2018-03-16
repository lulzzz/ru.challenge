using System;
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
            DistributionPlatforms = distributionPlatforms;
            ExpirationDate = expirationDate;
            Amount = amount;
        }

        private Subscription(Subscription other) : this()
        {
            Id = other.Id;
            PaymentMethod = other.PaymentMethod;
            DistributionPlatforms = other.DistributionPlatforms;
            ExpirationDate = other.ExpirationDate;
            Amount = other.Amount;
        }

        private Subscription()
            => DistributionPlatforms = ImmutableList.Create<DistributionPlatform>();

        public Subscription Clone() => new Subscription(this);

        public static Subscription Create(
            Guid id,
            PaymentMethod paymentMethod,
            IImmutableList<DistributionPlatform> distributionPlatforms,
            DateTimeOffset expirationDate,
            decimal amount)
            => new Subscription(id, paymentMethod, distributionPlatforms, expirationDate, amount);
    }
}