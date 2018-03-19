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
            => _dbConnection = dbConnection;

        public async Task AddAsync(CreateArtistEvent @event)
        {
            // TODO: This is not SQLi safe
            await _dbConnection.ExecuteAsync($"INSERT INTO artist (id, name, age) VALUES ('{@event.Id}', '{@event.Name}', '{@event.Age}')");
        }
    }
}