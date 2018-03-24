using Dapper;
using MediatR;
using RU.Challenge.Domain.Entities;
using RU.Challenge.Domain.Queries;
using RU.Challenge.Infrastructure.Dapper.DTO;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Dapper.QueryHandlers
{
    public class GetReleasesByIdForUserQueryHandler : IRequestHandler<GetReleasesByIdForUserQuery, IEnumerable<Release>>
    {
        private readonly IMediator _mediator;
        private readonly IDbConnection _dbConnection;

        public GetReleasesByIdForUserQueryHandler(IMediator mediator, IDbConnection dbConnection)
        {
            _mediator = mediator;
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<Release>> Handle(GetReleasesByIdForUserQuery request, CancellationToken cancellationToken)
        {
            var releases = await _dbConnection.QueryAsync<ReleaseQueryResult>(
                sql: $@"SELECT
                        r.id,
	                    r.title,
	                    r.status,
	                    r.cover_art_url as ""CoverArtUrl"",
	                    r.artist_id as ""ArtistId"",
	                    r.genre_id as ""GenreId"",
                        r.subscription_id as ""SubscriptionId"",
	                    ARRAY(SELECT id FROM track WHERE release_id = r.id) as ""TracksId""
                        from release r
                        WHERE r.user_id = @UserId AND @Filter IS false OR id = ANY (@Ids)",
                param: new
                {
                    request.UserId,
                    Filter = request.Ids != null && request.Ids.Any(),
                    Ids = request.Ids != null ? request.Ids.ToList() : request.Ids
                });

            var artists = await _mediator.Send(new GetArtistsByIdQuery(releases.Select(e => e.ArtistId).Distinct()));
            var genres = await _mediator.Send(new GetGenresByIdQuery(releases.Select(e => e.GenreId).Distinct()));

            var subscriptions = Enumerable.Empty<Subscription>();
            if (releases.Any(e => e.SubscriptionId != null))
                subscriptions = await _mediator.Send(new GetSubscriptionsByIdQuery(releases.Where(e => e.SubscriptionId != null).Select(e => e.SubscriptionId.Value).Distinct()));

            return releases
                .ToList()
                .Select(e =>
                {
                    var res = Release.Create(e.Id, e.Title, e.CoverArtUrl, e.Status);

                    res.SetArtist(artists.SingleOrDefault(a => a.Id == e.ArtistId));
                    res.SetGenre(genres.SingleOrDefault(g => g.Id == e.GenreId));
                    res.SetSubscription(subscriptions.SingleOrDefault(s => s.Id == e.SubscriptionId));

                    return res;
                });
        }
    }
}