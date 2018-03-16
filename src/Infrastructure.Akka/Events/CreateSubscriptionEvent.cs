﻿using System;
using System.Collections.Generic;

namespace RU.Challenge.Domain.Commands
{
    public class CreateSubscriptionEvent
    {
        public Guid Id { get; private set; }

        public Guid PaymentMethodId { get; private set; }

        public IEnumerable<Guid> DistributionPlatformsIds { get; private set; }

        public DateTimeOffset ExpirationDate { get; private set; }

        public decimal Amount { get; private set; }

        public CreateSubscriptionEvent(
            Guid id,
            Guid paymentMethodId,
            IEnumerable<Guid> distributionPlatformIds,
            DateTimeOffset expirationDate,
            decimal amount)
        {
            Id = id;
            PaymentMethodId = paymentMethodId;
            DistributionPlatformsIds = distributionPlatformIds;
            ExpirationDate = expirationDate;
            Amount = amount;
        }

        public static CreateSubscriptionEvent CreateFromCommand(CreateSubscriptionCommand command, Guid id)
            => new CreateSubscriptionEvent(id, command.PaymentMethodId, command.DistributionPlatformIds, command.ExpirationDate, command.Amount);
    }
}