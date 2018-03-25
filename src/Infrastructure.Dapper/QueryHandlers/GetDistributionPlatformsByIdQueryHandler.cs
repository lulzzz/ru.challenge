using Dapper;
using MediatR;
using RU.Challenge.Domain.Entities;
using RU.Challenge.Domain.Queries;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Dapper.QueryHandlers
{
    public class GetDistributionPlatformsByIdQueryHandler : IRequestHandler<GetDistributionPlatformsByIdQuery, IEnumerable<DistributionPlatform>>
    {
        private readonly IDbConnection _dbConnection;

        public GetDistributionPlatformsByIdQueryHandler(IDbConnection dbConnection)
            => _dbConnection = dbConnection ?? throw new System.ArgumentNullException(nameof(dbConnection));

        public async Task<IEnumerable<DistributionPlatform>> Handle(GetDistributionPlatformsByIdQuery request, CancellationToken cancellationToken)
        {
            return await _dbConnection.QueryAsync<DistributionPlatform>(
                sql: $"SELECT * FROM distribution_platform WHERE @Filter IS false OR id = ANY (@Ids)",
                param: new
                {
                    Filter = request.Ids != null && request.Ids.Any(),
                    Ids = request.Ids != null ? request.Ids.ToList() : request.Ids
                });
        }
    }
}