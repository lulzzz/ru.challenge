using Dapper;
using MediatR;
using RU.Challenge.Domain.Entities;
using RU.Challenge.Domain.Queries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Dapper.QueryHandlers
{
    public class GetDistributionPlatformsByIdQueryHandler : IRequestHandler<GetDistributionPlatformsByIdQuery, IEnumerable<DistributionPlatform>>
    {
        private readonly IDbConnection _dbConnection;

        public GetDistributionPlatformsByIdQueryHandler(IDbConnection dbConnection)
            => _dbConnection = dbConnection;

        public async Task<IEnumerable<DistributionPlatform>> Handle(GetDistributionPlatformsByIdQuery request, CancellationToken cancellationToken)
        {
            // TODO: Why is dapper param for IN not working??
            var ids = $"({string.Join(", ", request.Ids.Select(e => $"'{e}'"))})";

            // Not safe sql
            return await _dbConnection.QueryAsync<DistributionPlatform>(
                sql: $"SELECT * FROM distribution_platform WHERE id IN {ids}");
        }
    }
}
