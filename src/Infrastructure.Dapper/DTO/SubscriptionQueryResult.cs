using System;

namespace RU.Challenge.Infrastructure.Dapper.DTO
{
    internal class SubscriptionQueryResult
    {
        internal Guid Id { get; set; }

        internal DateTimeOffset ExpirationDate { get; set; }

        internal decimal Amount { get; set; }

        internal Guid PaymentMethodId { get; set; }

        internal string PaymentMethodName { get; set; }

        internal Guid DistributionPlatformId { get; set; }

        internal string DistributionPlatformName { get; set; }

        internal static string Query =>
            @"SELECT
                s.id,
                expiration_date as ""ExpirationDate"",
                amount,
                p.id as ""PaymentMethodId"",
                p.name as ""PaymentMethodName"",
                d.id as ""DistributionPlatformId"",
                d.name as ""DistributionPlatformName""
             FROM subscription s
             INNER JOIN payment_method p ON s.payment_method_id = p.id
             INNER JOIN distribution_platform d ON d.id = any (s.distribution_platforms_id)
             WHERE @Id IS NULL OR s.id = @Id";
    }
}