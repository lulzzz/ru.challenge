using Dapper;
using Infrastructure.Repositories;
using RU.Challenge.Domain.Events;
using System.Data;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Dapper.Repositories
{
    public class ArtistRepository : IArtistRepository
    {
        private readonly IDbConnection _dbConnection;

        public ArtistRepository(IDbConnection dbConnection)
            => _dbConnection = dbConnection ?? throw new System.ArgumentNullException(nameof(dbConnection));

        public async Task AddAsync(CreateArtistEvent @event)
        {
            await _dbConnection.ExecuteAsync(
                sql: $"INSERT INTO artist (id, name, age) VALUES (@Id, @Name, @Age)",
                param: new { @event.Id, @event.Name, @event.Age });
        }
    }
}