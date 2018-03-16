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

        public void SetPaymentMethod(PaymentMethod paymentMethod)
            => PaymentMethod = paymentMethod;

        public void AddDistributionPlatform(DistributionPlatform distributionPlatform)
            => DistributionPlatforms = DistributionPlatforms.Add(distributionPlatform);

        private Subscription(
            Guid id,
            DateTimeOffset expirationDate,
            decimal amount) : this()
        {
            Id = id;
            ExpirationDate = expirationDate;
            Amount = amount;
        }

        private Subscription()
            => DistributionPlatforms = ImmutableList.Create<DistributionPlatform>();

        public static Subscription Create(
            Guid id,
            DateTimeOffset expirationDate,
            decimal amount)
            => new Subscription(id, expirationDate, amount);
    }
}