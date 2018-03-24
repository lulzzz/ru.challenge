using Dapper;
using MediatR;
using RU.Challenge.Domain.Entities;
using RU.Challenge.Domain.Queries;
using RU.Challenge.Infrastructure.Dapper.DTO;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Dapper.QueryHandlers
{
    public class GetSubscriptionsByIdQueryHandler : IRequestHandler<GetSubscriptionsByIdQuery, IEnumerable<Subscription>>
    {
        private readonly IMediator _mediator;
        private readonly IDbConnection _dbConnection;

        public GetSubscriptionsByIdQueryHandler(IMediator mediator, IDbConnection dbConnection)
        {
            _mediator = mediator;
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<Subscription>> Handle(GetSubscriptionsByIdQuery request, CancellationToken cancellationToken)
        {
            var subscriptions = await _dbConnection.QueryAsync<SubscriptionQueryResult>(
                sql: $@"SELECT
                        id,
                        amount,
                        expiration_date as ""ExpirationDate"",
                        payment_method_id as ""PaymentMethodId"",
                        distribution_platforms_id as ""DistributionPlatformsId""
                        FROM subscription
                        WHERE @Filter IS false OR id = ANY (@Ids)",
                param: new
                {
                    Filter = request.Ids != null && request.Ids.Any(),
                    Ids = request.Ids != null ? request.Ids.ToList() : request.Ids
                });

            var paymentMethods = await _mediator.Send(new GetPaymentMethodsByIdQuery(subscriptions.Select(e => e.PaymentMethodId).Distinct()));
            var distributionPlatforms = await _mediator.Send(new GetDistributionPlatformsByIdQuery(subscriptions.SelectMany(e => e.DistributionPlatformsId).Distinct()));

            return subscriptions
                .ToList()
                .Select(e =>
                {
                    var sub = Subscription.Create(e.Id, e.ExpirationDate, e.Amount);
                    sub.SetPaymentMethod(paymentMethods.SingleOrDefault(p => p.Id == e.PaymentMethodId));

                    if (e.DistributionPlatformsId != null)
                    {
                        foreach (var distId in e.DistributionPlatformsId)
                        {
                            var dist = distributionPlatforms.SingleOrDefault(d => d.Id == distId);
                            if (dist != null)
                                sub.AddDistributionPlatform(dist);
                        }
                    }

                    return sub;
                });
        }
    }
}