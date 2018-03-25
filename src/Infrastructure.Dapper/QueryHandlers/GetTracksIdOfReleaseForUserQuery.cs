using Dapper;
using MediatR;
using RU.Challenge.Domain.Queries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Dapper.QueryHandlers
{
    public class GetTracksIdOfReleaseForUserQueryHandler : IRequestHandler<GetTracksIdOfReleaseForUserQuery, IEnumerable<Guid>>
    {
        private readonly IDbConnection _dbConnection;

        public GetTracksIdOfReleaseForUserQueryHandler(IDbConnection dbConnection)
            => _dbConnection = dbConnection;

        public async Task<IEnumerable<Guid>> Handle(GetTracksIdOfReleaseForUserQuery request, CancellationToken cancellationToken)
        {
            return (await _dbConnection.QueryFirstAsync<QueryResult>(
                sql: @"SELECT ARRAY(SELECT t.id FROM track t WHERE release_id = r.id) as ""TrackIds"" FROM release r WHERE id = @Id AND user_id = @UserId",
                param: new { Id = request.ReleaseId, request.UserId })).TrackIds;
        }

        internal class QueryResult
        {
            internal IEnumerable<Guid> TrackIds { get; set; }
        }
    }
}