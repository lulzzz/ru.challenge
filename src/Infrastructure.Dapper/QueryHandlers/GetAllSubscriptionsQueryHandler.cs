using MediatR;
using RU.Challenge.Domain.Entities;
using RU.Challenge.Domain.Queries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using System.Linq;

namespace RU.Challenge.Infrastructure.Dapper.QueryHandlers
{
    public class GetAllSubscriptionsQueryHandler : IRequestHandler<GetAllSubscriptionsQuery, IEnumerable<Subscription>>
    {
        private readonly IDbConnection _dbConnection;

        public GetAllSubscriptionsQueryHandler(IDbConnection dbConnection)
            => _dbConnection = dbConnection;

        public async Task<IEnumerable<Subscription>> Handle(GetAllSubscriptionsQuery request, CancellationToken cancellationToken)
        {
            var auxSubscriptions = await _dbConnection.QueryAsync<SubscriptionQueryResult>(
                sql: @"SELECT
                          s.id,
                          expiration_date as ""ExpirationDate"",
                          amount,
                          p.id as ""PaymentMethodId"",
                          p.name as ""PaymentMethodName"",
                          d.id as ""DistributionPlatformId"",
                          d.name as ""DistributionPlatformName""
                       FROM subscription s
                       INNER JOIN payment_method p ON s.payment_method_id = p.id
                       INNER JOIN distribution_platform d ON d.id = any (s.distribution_platforms_id)");

            return auxSubscriptions
                .GroupBy(e => new { e.Id, e.ExpirationDate, e.Amount, e.PaymentMethodId, e.PaymentMethodName })
                .Select(groupMembers => {

                    var sub = Subscription.Create(groupMembers.Key.Id, groupMembers.Key.ExpirationDate, groupMembers.Key.Amount);
                    sub.SetPaymentMethod(PaymentMethod.Create(groupMembers.Key.PaymentMethodId, groupMembers.Key.PaymentMethodName));

                    foreach (var dp in groupMembers)
                        sub.AddDistributionPlatform(DistributionPlatform.Create(dp.DistributionPlatformId, dp.DistributionPlatformName));

                    return sub;
                });
        }

        internal class SubscriptionQueryResult
        {
            public Guid Id { get; set; }

            public DateTimeOffset ExpirationDate { get; set; }

            public decimal Amount { get; set; }

            public Guid PaymentMethodId { get; set; }

            public string PaymentMethodName { get; set; }

            public Guid DistributionPlatformId { get; set; }

            public string DistributionPlatformName { get; set; }
        }
    }
}