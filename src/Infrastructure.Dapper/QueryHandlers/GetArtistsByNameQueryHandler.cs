using MediatR;
using RU.Challenge.Domain.Entities;
using RU.Challenge.Domain.Queries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace RU.Challenge.Infrastructure.Dapper.QueryHandlers
{
    public class GetArtistsByNameQueryHandler : IRequestHandler<GetArtistsByNameQuery, IEnumerable<Artist>>
    {
        private readonly IDbConnection _dbConnection;

        public GetArtistsByNameQueryHandler(IDbConnection dbConnection)
            => _dbConnection = dbConnection;

        public async Task<IEnumerable<Artist>> Handle(GetArtistsByNameQuery request, CancellationToken cancellationToken)
        {
            return await _dbConnection.QueryAsync<Artist>(
                sql: "SELECT * FROM artist WHERE @Name IS NULL OR name LIKE @Name",
                param: new {
                    Name = request.Name != null ? $"%{request.Name}%" : request.Name
                });
        }
    }
}