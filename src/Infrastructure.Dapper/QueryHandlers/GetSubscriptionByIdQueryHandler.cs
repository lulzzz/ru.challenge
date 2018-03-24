using Dapper;
using MediatR;
using RU.Challenge.Domain.Entities;
using RU.Challenge.Domain.Queries;
using RU.Challenge.Infrastructure.Dapper.DTO;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Dapper.QueryHandlers
{
    public class GetSubscriptionByIdQueryHandler : IRequestHandler<GetSubscriptionByIdQuery, Subscription>
    {
        private readonly IDbConnection _dbConnection;

        public GetSubscriptionByIdQueryHandler(IDbConnection dbConnection)
            => _dbConnection = dbConnection;

        public async Task<Subscription> Handle(GetSubscriptionByIdQuery request, CancellationToken cancellationToken)
        {
            var auxSubscriptions = await _dbConnection.QueryAsync<SubscriptionQueryResult>(
                sql: SubscriptionQueryResult.Query, param: new { request.Id });

            return auxSubscriptions
                .GroupBy(e => new { e.Id, e.ExpirationDate, e.Amount, e.PaymentMethodId, e.PaymentMethodName })
                .Select(groupMembers =>
                {
                    var sub = Subscription.Create(groupMembers.Key.Id, groupMembers.Key.ExpirationDate, groupMembers.Key.Amount);
                    sub.SetPaymentMethod(PaymentMethod.Create(groupMembers.Key.PaymentMethodId, groupMembers.Key.PaymentMethodName));

                    foreach (var dp in groupMembers)
                        sub.AddDistributionPlatform(DistributionPlatform.Create(dp.DistributionPlatformId, dp.DistributionPlatformName));

                    return sub;
                }).FirstOrDefault();
        }
    }
}