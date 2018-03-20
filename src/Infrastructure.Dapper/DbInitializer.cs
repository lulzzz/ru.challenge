using Dapper;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Data;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Dapper
{
    public class DbInitializer
    {
        private readonly ILogger<DbInitializer> _logger;
        private readonly IDbConnection _dbConnection;

        public DbInitializer(IDbConnection dbConnection, ILogger<DbInitializer> logger)
        {
            _logger = logger;
            _dbConnection = dbConnection;
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

            var roleTable = await _dbConnection.ExecuteScalarAsync<bool>(GetExistsScript(tableName: "role_auth"));
            if (!roleTable)
                await _dbConnection.ExecuteAsync("CREATE TABLE role_auth (id UUID PRIMARY KEY, role_name VARCHAR(50), normalized_role_name VARCHAR(50))");

            var genreTable = await _dbConnection.ExecuteScalarAsync<bool>(GetExistsScript(tableName: "genre"));
            if (!genreTable)
                await _dbConnection.ExecuteAsync("CREATE TABLE genre (id UUID PRIMARY KEY, name VARCHAR(500))");

            var distributionPlatformTable = await _dbConnection.ExecuteScalarAsync<bool>(GetExistsScript(tableName: "distribution_platform"));
            if (!distributionPlatformTable)
                await _dbConnection.ExecuteAsync("CREATE TABLE distribution_platform (id UUID PRIMARY KEY, name VARCHAR(500))");

            var paymentMethodTable = await _dbConnection.ExecuteScalarAsync<bool>(GetExistsScript(tableName: "payment_method"));
            if (!paymentMethodTable)
                await _dbConnection.ExecuteAsync("CREATE TABLE payment_method (id UUID PRIMARY KEY, name VARCHAR(500))");

            var artistTable = await _dbConnection.ExecuteScalarAsync<bool>(GetExistsScript(tableName: "artist"));
            if (!artistTable)
                await _dbConnection.ExecuteAsync("CREATE TABLE artist (id UUID PRIMARY KEY, name VARCHAR(500), age INT)");

            var subscriptionTable = await _dbConnection.ExecuteScalarAsync<bool>(GetExistsScript(tableName: "subscription"));
            if (!subscriptionTable)
                await _dbConnection.ExecuteAsync("CREATE TABLE subscription (id UUID PRIMARY KEY, expiration_date TIMESTAMP, amount DECIMAL, payment_method_id UUID, distribution_platforms_id UUID ARRAY)");

            _logger.LogInformation("All the tables were created");
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