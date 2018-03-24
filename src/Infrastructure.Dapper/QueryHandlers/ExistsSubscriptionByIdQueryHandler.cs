using Dapper;
using MediatR;
using RU.Challenge.Domain.Queries;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Dapper.QueryHandlers
{
    public class ExistsSubscriptionByIdQueryHandler : IRequestHandler<ExistsSubscriptionByIdQuery, bool>
    {
        private readonly IDbConnection _dbConnection;

        public ExistsSubscriptionByIdQueryHandler(IDbConnection dbConnection)
            => _dbConnection = dbConnection;

        public async Task<bool> Handle(ExistsSubscriptionByIdQuery request, CancellationToken cancellationToken)
        {
            return await _dbConnection.ExecuteScalarAsync<bool>(
                sql: "SELECT EXISTS (SELECT 1 FROM subscription WHERE id = @Id)",
                param: new { request.Id });
        }
    }
}