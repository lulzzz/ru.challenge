using Dapper;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Data;
using System.Threading.Tasks;

namespace RU.Challenge.Infrastructure.Dapper
{
    public class ReadDbInitializer
    {
        private readonly ILogger<ReadDbInitializer> _logger;
        private readonly IDbConnection _dbConnection;

        public ReadDbInitializer(IDbConnection dbConnection, ILogger<ReadDbInitializer> logger)
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

            var releaseTable = await _dbConnection.ExecuteScalarAsync<bool>(GetExistsScript(tableName: "release"));
            if (!releaseTable)
                await _dbConnection.ExecuteAsync("CREATE TABLE release (id UUID PRIMARY KEY, title VARCHAR(100), cover_art_url TEXT, artist_id UUID, genre_id UUID, user_id UUID, status VARCHAR(10), subscription_id UUID)");

            var trackTable = await _dbConnection.ExecuteScalarAsync<bool>(GetExistsScript(tableName: "track"));
            if (!trackTable)
                await _dbConnection.ExecuteAsync("CREATE TABLE track (id UUID PRIMARY KEY, release_id UUID, name VARCHAR(100), song_url TEXT, artist_id UUID, genre_id UUID, track_order INT)");

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