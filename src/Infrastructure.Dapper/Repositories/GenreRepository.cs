using Dapper;
using Infrastructure.Repositories;
using RU.Challenge.Domain.Events;
using System.Data;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Dapper.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly IDbConnection _dbConnection;

        public GenreRepository(IDbConnection dbConnection)
            => _dbConnection = dbConnection;

        public async Task AddAsync(CreateGenreEvent @event)
        {
            // TODO: This is not SQLi safe
            await _dbConnection.ExecuteAsync($"INSERT INTO genre (id, name) VALUES ('{@event.Id}', '{@event.Name}')");
        }
    }
}