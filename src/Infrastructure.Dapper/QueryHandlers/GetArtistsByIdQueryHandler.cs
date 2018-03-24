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
    public class GetArtistsByIdQueryHandler : IRequestHandler<GetArtistsByIdQuery, IEnumerable<Artist>>
    {
        private readonly IDbConnection _dbConnection;

        public GetArtistsByIdQueryHandler(IDbConnection dbConnection)
            => _dbConnection = dbConnection;

        public async Task<IEnumerable<Artist>> Handle(GetArtistsByIdQuery request, CancellationToken cancellationToken)
        {
            return await _dbConnection.QueryAsync<Artist>(
                sql: $"SELECT * FROM artist WHERE @Filter IS false OR id = ANY (@Ids)",
                param: new
                {
                    Filter = request.Ids != null && request.Ids.Any(),
                    Ids = request.Ids != null ? request.Ids.ToList() : request.Ids
                });
        }
    }
}