using Dapper;
using MediatR;
using RU.Challenge.Domain.Entities;
using RU.Challenge.Domain.Queries;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Dapper.QueryHandlers
{
    public class GetArtistByIdQueryHandler : IRequestHandler<GetArtistByIdQuery, Artist>
    {
        private readonly IDbConnection _dbConnection;

        public GetArtistByIdQueryHandler(IDbConnection dbConnection)
            => _dbConnection = dbConnection;

        public async Task<Artist> Handle(GetArtistByIdQuery request, CancellationToken cancellationToken)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<Artist>(
                sql: "SELECT * FROM artist WHERE id = @Id",
                param: new { request.Id });
        }
    }
}