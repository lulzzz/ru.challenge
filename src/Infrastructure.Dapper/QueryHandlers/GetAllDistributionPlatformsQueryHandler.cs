using Dapper;
using MediatR;
using RU.Challenge.Domain.Entities;
using RU.Challenge.Domain.Queries;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Dapper.QueryHandlers
{
    public class GetAllDistributionPlatformsQueryHandler : IRequestHandler<GetAllDistributionPlatformsQuery, IEnumerable<DistributionPlatform>>
    {
        private readonly IDbConnection _dbConnection;

        public GetAllDistributionPlatformsQueryHandler(IDbConnection dbConnection)
            => _dbConnection = dbConnection;

        public async Task<IEnumerable<DistributionPlatform>> Handle(GetAllDistributionPlatformsQuery request, CancellationToken cancellationToken)
        {
            return await _dbConnection.QueryAsync<DistributionPlatform>(sql: "SELECT * FROM distribution_platform");
        }
    }
}