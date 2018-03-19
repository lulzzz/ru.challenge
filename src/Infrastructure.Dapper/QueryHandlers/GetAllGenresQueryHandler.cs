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
    public class GetAllGenresQueryHandler : IRequestHandler<GetAllGenresQuery, IEnumerable<Genre>>
    {
        private readonly IDbConnection _dbConnection;

        public GetAllGenresQueryHandler(IDbConnection dbConnection)
            => _dbConnection = dbConnection;

        public async Task<IEnumerable<Genre>> Handle(GetAllGenresQuery request, CancellationToken cancellationToken)
        {
            return await _dbConnection.QueryAsync<Genre>(sql: "SELECT * FROM genre");
        }
    }
}