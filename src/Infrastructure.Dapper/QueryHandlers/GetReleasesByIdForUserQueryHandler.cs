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
	                    ARRAY(SELECT t.id FROM track t WHERE release_id = r.id) as ""TracksIds""
                        from release r
                        WHERE r.user_id = @UserId AND @Filter IS false OR r.id = ANY (@Ids)",
                param: new
                {
                    request.UserId,
                    Filter = request.Ids != null && request.Ids.Any(),
                    Ids = request.Ids != null ? request.Ids.ToList() : request.Ids
                });

            var genres = await _mediator.Send(new GetGenresByIdQuery(releases.Select(e => e.GenreId).Distinct()));
            var artists = await _mediator.Send(new GetArtistsByIdQuery(releases.Select(e => e.ArtistId).Distinct()));

            var tracks = Enumerable.Empty<Track>();
            if (releases.Any(e => e.TracksIds != null && e.TracksIds.Any()))
                tracks = await _mediator.Send(new GetTracksByIdQuery(releases.Where(e => e.TracksIds != null && e.TracksIds.Any()).SelectMany(e => e.TracksIds).Distinct()));

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

                    if (e.TracksIds != null)
                    {
                        foreach (var tr in e.TracksIds)
                        {
                            var track = tracks.SingleOrDefault(t => t.Id == tr);
                            if (track != null)
                                res.AddTrack(track);
                        }
                    }

                    return res;
                });
        }
    }
}