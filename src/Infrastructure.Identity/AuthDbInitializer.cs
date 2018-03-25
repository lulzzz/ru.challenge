using Dapper;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Data;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Identity
{
    public class AuthDbInitializer
    {
        private readonly IDbConnection _dbConnection;
        private readonly ILogger<AuthDbInitializer> _logger;

        public AuthDbInitializer(IDbConnection dbConnection, ILogger<AuthDbInitializer> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        }

        public async Task Init()
        {
            await Policy
              .Handle<Exception>()
              .WaitAndRetryAsync(3, (count) => TimeSpan.FromSeconds(10))
              .ExecuteAsync(InnerInit);
        }

        private async Task InnerInit()
        {
            var userTable = await _dbConnection.ExecuteScalarAsync<bool>(GetExistsScript(tableName: "user_auth"));
            if (!userTable)
                await _dbConnection.ExecuteAsync("CREATE TABLE user_auth (id UUID PRIMARY KEY, user_name VARCHAR(50), normalized_user_name VARCHAR(50), email VARCHAR(200), password_hash VARCHAR(300))");

            var roleTable = await _dbConnection.ExecuteScalarAsync<bool>(GetExistsScript(tableName: "claims_auth"));
            if (!roleTable)
                await _dbConnection.ExecuteAsync("CREATE TABLE claims_auth (user_id UUID, type VARCHAR(50), value VARCHAR(50))");

            _logger.LogInformation("Auth database tables created");
        }

        private string GetExistsScript(string tableName, string tableSchema = "public")
        {
            return $@"SELECT EXISTS (
                SELECT 1
                FROM information_schema.tables
                WHERE table_schema = '{tableSchema}'
                AND table_name = '{tableName}');";
        }
    }
}