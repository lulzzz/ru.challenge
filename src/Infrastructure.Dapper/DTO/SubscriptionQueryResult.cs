using System;
using System.Collections.Generic;

namespace RU.Challenge.Infrastructure.Dapper.DTO
{
    internal class SubscriptionQueryResult
    {
        internal Guid Id { get; set; }

        internal DateTimeOffset ExpirationDate { get; set; }

        internal decimal Amount { get; set; }

        internal Guid PaymentMethodId { get; set; }

        internal IEnumerable<Guid> DistributionPlatformsId { get; set; }
    }
}