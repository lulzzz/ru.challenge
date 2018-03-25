using Dapper;
using Infrastructure.Repositories;
using RU.Challenge.Domain.Events;
using System;
using System.Data;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Dapper.Repositories
{
    public class TrackRepository : ITrackRepository
    {
        private readonly IDbConnection _dbConnection;

        public TrackRepository(IDbConnection dbConnection)
            => _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));

        public async Task AddAsync(CreateTrackEvent @event)
        {
            await _dbConnection.ExecuteAsync(
                sql: $"INSERT INTO track (id, release_id, name, song_url, artist_id, genre_id, track_order) " +
                     $"VALUES (@Id, @ReleaseId, @Name, @SongUrl, @ArtistId, @GenreId, @Order)",
                param: new
                {
                    @event.Id,
                    @event.ReleaseId,
                    @event.Name,
                    @event.SongUrl,
                    @event.ArtistId,
                    @event.GenreId,
                    @event.Order
                });
        }
    }
}