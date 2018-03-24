﻿using Dapper;
using MediatR;
using RU.Challenge.Domain.Entities;
using RU.Challenge.Domain.Queries;
using RU.Challenge.Infrastructure.Dapper.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
                sql: SubscriptionQueryResult.Query, param: new { Id = default(Guid?) });

            return auxSubscriptions
                .GroupBy(e => new { e.Id, e.ExpirationDate, e.Amount, e.PaymentMethodId, e.PaymentMethodName })
                .Select(groupMembers =>
                {
                    var sub = Subscription.Create(groupMembers.Key.Id, groupMembers.Key.ExpirationDate, groupMembers.Key.Amount);
                    sub.SetPaymentMethod(PaymentMethod.Create(groupMembers.Key.PaymentMethodId, groupMembers.Key.PaymentMethodName));

                    foreach (var dp in groupMembers)
                        sub.AddDistributionPlatform(DistributionPlatform.Create(dp.DistributionPlatformId, dp.DistributionPlatformName));

                    return sub;
                });
        }
    }
}