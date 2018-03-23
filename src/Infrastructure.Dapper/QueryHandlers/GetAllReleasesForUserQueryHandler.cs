using MediatR;
using RU.Challenge.Domain.Entities;
using RU.Challenge.Domain.Queries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Dapper.QueryHandlers
{
    public class GetAllReleasesForUserQueryHandler : IRequestHandler<GetAllReleasesForUserQuery, IEnumerable<Release>>
    {
        private readonly IDbConnection _dbConnection;

        public GetAllReleasesForUserQueryHandler(IDbConnection dbConnection)
            => _dbConnection = dbConnection;

        public async Task<IEnumerable<Release>> Handle(GetAllReleasesForUserQuery request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }
    }
}