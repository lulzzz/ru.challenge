using Dapper;
using Infrastructure.Repositories;
using RU.Challenge.Domain.Enums;
using RU.Challenge.Domain.Events;
using System.Data;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Dapper.Repositories
{
    public class ReleaseRepository : IReleaseRepository
    {
        private readonly IDbConnection _dbConnection;

        public ReleaseRepository(IDbConnection dbConnection)
            => _dbConnection = dbConnection ?? throw new System.ArgumentNullException(nameof(dbConnection));

        public async Task AddAsync(CreateReleaseEvent @event)
        {
            await _dbConnection.ExecuteAsync(
                sql: $"INSERT INTO release (id, title, cover_art_url, artist_id, genre_id, user_id, status) " +
                     $"VALUES (@Id, @Title, @CoverArtUrl, @ArtistId, @GenreId, @UserId, @Status)",
                param: new
                {
                    @event.Id,
                    @event.Title,
                    @event.CoverArtUrl,
                    @event.ArtistId,
                    @event.GenreId,
                    @event.UserId,
                    Status = ReleaseStatus.Created.ToString()
                });
        }

        public async Task AddSubscriptionAsync(AddSubscriptionToReleaseEvent @event)
        {
            await _dbConnection.ExecuteAsync(
                sql: $"UPDATE release SET subscription_id = @SubscriptionId, status = @Status WHERE id = @ReleaseId",
                param: new
                {
                    @event.SubscriptionId,
                    @event.ReleaseId,
                    Status = ReleaseStatus.Published.ToString()
                });
        }
    }
}