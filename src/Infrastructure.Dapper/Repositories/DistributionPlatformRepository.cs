using Dapper;
using Infrastructure.Repositories;
using RU.Challenge.Domain.Events;
using System.Data;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Dapper.Repositories
{
    public class DistributionPlatformRepository : IDistributionPlatformRepository
    {
        private readonly IDbConnection _dbConnection;

        public DistributionPlatformRepository(IDbConnection dbConnection)
            => _dbConnection = dbConnection;

        public async Task AddAsync(CreateDistributionPlatformEvent @event)
        {
            await _dbConnection.ExecuteAsync(
                sql: $"INSERT INTO distribution_platform (id, name) VALUES (@Id, @Name)",
                param: new { @event.Id, @event.Name });
        }
    }
}