using Dapper;
using MediatR;
using RU.Challenge.Domain.Enums;
using RU.Challenge.Domain.Queries;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Dapper.QueryHandlers
{
    public class GetReleaseStateByIdForUserQueryHandler : IRequestHandler<GetReleaseStateByIdForUserQuery, ReleaseStatus?>
    {
        private readonly IDbConnection _dbConnection;

        public GetReleaseStateByIdForUserQueryHandler(IDbConnection dbConnection)
            => _dbConnection = dbConnection;

        public async Task<ReleaseStatus?> Handle(GetReleaseStateByIdForUserQuery request, CancellationToken cancellationToken)
        {
            var status = await _dbConnection.ExecuteScalarAsync<string>(
                sql: @"SELECT status FROM release WHERE id = @Id AND user_id = @UserId",
                param: new { request.Id, request.UserId });

            if (status == null)
                return null;

            return (ReleaseStatus)Enum.Parse(typeof(ReleaseStatus), status);
        }

    }
}