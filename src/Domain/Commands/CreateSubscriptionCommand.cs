using MediatR;
using System;
using System.Collections.Generic;

namespace RU.Challenge.Domain.Commands
{
    public class CreateSubscriptionCommand : IRequest
    {
        public Guid SubscriptionId { get; set; }

        public Guid PaymentMethodId { get; set; }

        public IEnumerable<Guid> DistributionPlatformIds { get; set; }

        public DateTimeOffset ExpirationDate { get; set; }

        public decimal Amount { get; set; }

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

        public CreateSubscriptionCommand(
            Guid subscriptionId,
            Guid paymentMethodId,
            IEnumerable<Guid> distributionPlatformIds,
            DateTimeOffset expirationDate,
            decimal amount)
            : this(paymentMethodId, distributionPlatformIds, expirationDate, amount)
        {
            SubscriptionId = subscriptionId;
        }
    }
}