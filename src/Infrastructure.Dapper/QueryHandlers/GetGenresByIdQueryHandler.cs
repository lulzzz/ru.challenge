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
    public class GetGenresByIdQueryHandler : IRequestHandler<GetGenresByIdQuery, IEnumerable<Genre>>
    {
        private readonly IDbConnection _dbConnection;

        public GetGenresByIdQueryHandler(IDbConnection dbConnection)
            => _dbConnection = dbConnection;

        public async Task<IEnumerable<Genre>> Handle(GetGenresByIdQuery request, CancellationToken cancellationToken)
        {
            return await _dbConnection.QueryAsync<Genre>(
                sql: $"SELECT * FROM genre WHERE @Filter IS false OR id = ANY (@Ids)",
                param: new
                {
                    Filter = request.Ids != null && request.Ids.Any(),
                    Ids = request.Ids != null ? request.Ids.ToList() : request.Ids
                });
        }
    }
}