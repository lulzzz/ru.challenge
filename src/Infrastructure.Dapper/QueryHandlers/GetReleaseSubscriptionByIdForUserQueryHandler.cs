using Dapper;
using MediatR;
using RU.Challenge.Domain.Entities;
using RU.Challenge.Domain.Queries;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Dapper.QueryHandlers
{
    public class GetReleaseSubscriptionByIdForUserQueryHandler : IRequestHandler<GetReleaseSubscriptionByIdForUserQuery, Subscription>
    {
        private readonly IDbConnection _dbConnection;

        public GetReleaseSubscriptionByIdForUserQueryHandler(IDbConnection dbConnection)
            => _dbConnection = dbConnection ?? throw new System.ArgumentNullException(nameof(dbConnection));

        public async Task<Subscription> Handle(GetReleaseSubscriptionByIdForUserQuery request, CancellationToken cancellationToken)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<Subscription>(
                sql: $@"SELECT
                        s.expiration_date as ""ExpirationDate""
                        FROM release r
                        INNER JOIN subscription s ON s.id = r.subscription_id
                        WHERE r.user_id = @UserId AND r.id = @Id",
                param: new
                {
                    request.Id,
                    request.UserId
                });
        }
    }
}