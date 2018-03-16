using System;
using System.Collections.Generic;

namespace RU.Challenge.Domain.Commands
{
    public class CreateSubscriptionCommand
    {
        public Guid PaymentMethodId { get; private set; }

        public IEnumerable<Guid> DistributionPlatformIds { get; private set; }

        public DateTimeOffset ExpirationDate { get; private set; }

        public decimal Amount { get; private set; }

        public CreateSubscriptionCommand(
            Guid paymentMethodId,
            IEnumerable<Guid> distributionPlatformIds,
            DateTimeOffset expirationDate,
            decimal amount)
        {
            PaymentMethodId = paymentMethodId;
            DistributionPlatformIds = distributionPlatformIds;
            ExpirationDate = expirationDate;
            Amount = amount;
        }
    }
}