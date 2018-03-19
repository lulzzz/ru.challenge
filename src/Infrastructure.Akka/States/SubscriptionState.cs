using System;
using System.Collections.Generic;

namespace RU.Challenge.Infrastructure.Akka.Snapshot
{
    public class SubscriptionState
    {
        public Guid Id { get; private set; }

        public DateTimeOffset ExpirationDate { get; private set; }

        public decimal Amount { get; private set; }

        public Guid PaymentMethodId { get; private set; }

        // For some reason the snapshot does not like IEnumerable but IList yes
        public IList<Guid> DistributionPlatformIds { get; private set; }

        public SubscriptionState(
            Guid id,
            DateTimeOffset expirationDate,
            decimal amount,
            Guid paymentMethodId,
            IList<Guid> distributionPlatformIds)
        {
            Id = id;
            ExpirationDate = expirationDate;
            Amount = amount;
            PaymentMethodId = paymentMethodId;
            DistributionPlatformIds = distributionPlatformIds;
        }
    }
}