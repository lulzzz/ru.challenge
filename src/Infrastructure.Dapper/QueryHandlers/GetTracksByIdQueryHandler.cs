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
    public class GetTracksByIdQueryHandler : IRequestHandler<GetTracksByIdQuery, IEnumerable<Track>>
    {
        private readonly IMediator _mediator;
        private readonly IDbConnection _dbConnection;

        public GetTracksByIdQueryHandler(IMediator mediator, IDbConnection dbConnection)
        {
            _mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
            _dbConnection = dbConnection ?? throw new System.ArgumentNullException(nameof(dbConnection));
        }

        public async Task<IEnumerable<Track>> Handle(GetTracksByIdQuery request, CancellationToken cancellationToken)
        {
            var tracks = await _dbConnection.QueryAsync<TrackQueryResult>(
                sql: $@"SELECT
                        id,
                        name,
                        track_order as ""Order"",
                        song_url as ""SongUrl"",
                        genre_id as ""GenreId"",
                        artist_id as ""ArtistId""
                        FROM track
                        WHERE @Filter IS false OR id = ANY (@Ids)",
                param: new
                {
                    Filter = request.Ids != null && request.Ids.Any(),
                    Ids = request.Ids != null ? request.Ids.ToList() : request.Ids
                });

            var genres = await _mediator.Send(new GetGenresByIdQuery(tracks.Select(e => e.GenreId).Distinct()));
            var artists = await _mediator.Send(new GetArtistsByIdQuery(tracks.Select(e => e.ArtistId).Distinct()));

            return tracks
                .ToList()
                .Select(e =>
                {
                    var track = Track.Create(e.Id, e.Name, e.SongUrl, e.Order);
                    track.SetArtist(artists.SingleOrDefault(a => a.Id == e.ArtistId));
                    track.SetGenre(genres.SingleOrDefault(g => g.Id == e.GenreId));
                    return track;
                });
        }
    }
}