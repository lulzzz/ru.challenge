using MediatR;
using RU.Challenge.Domain.Entities;
using RU.Challenge.Domain.Queries;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Dapper.QueryHandlers
{
    public class GetReleaseByIdForUserQueryHandler : IRequestHandler<GetReleaseByIdForUserQuery, Release>
    {
        private readonly IDbConnection _dbConnection;

        public GetReleaseByIdForUserQueryHandler(IDbConnection dbConnection)
            => _dbConnection = dbConnection;

        public async Task<Release> Handle(GetReleaseByIdForUserQuery request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }
    }
}